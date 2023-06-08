using System.Data;
using System.Data.Common;
using TextDataMasking;

namespace TextDataMasking
{
    public abstract class DatabaseMasker: IDatabaseMasker
    {
        protected string connectionString = string.Empty;
        protected MaskDictionary maskDictionary = null;

        public DatabaseMasker(string ConnectionString)
            :this(ConnectionString, new MaskDictionary())
        {
        }

        public DatabaseMasker(string ConnectionString, MaskDictionary MaskDictionary)
        {
            this.connectionString = ConnectionString;
            this.maskDictionary = MaskDictionary;
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        public abstract DbProviderFactory GetDbProviderFactory();

        protected abstract string GetDatabaseColumnSelectStatement(DatabaseTable table);

        protected abstract string GetDatabaseTableSelectStatement(DatabaseTable table);

        protected abstract string GetDatabaseTableCountRowsStatement(DatabaseTable table, DatabaseColumn column);

        public List<DatabaseTable> ListTables(DbConnection connection)
        {
            var tableList = new List<DatabaseTable>();

            DbProviderFactory factory = GetDbProviderFactory();

            DataTable tables = connection.GetSchema("Tables");
            for (int i = 0; i < tables.Rows.Count; i++)
            {
                var row = tables.Rows[i];

                string tableSchema = row["TABLE_SCHEMA"].ToString();
                string tableName = row["TABLE_NAME"].ToString();
                var databaseTable =
                    new DatabaseTable()
                    {
                        TableSchema = tableSchema,
                        TableName = tableName
                    };

                string selectStatement = GetDatabaseColumnSelectStatement(databaseTable);

                var selectCommand = factory.CreateCommand();
                selectCommand.CommandText = selectStatement;
                selectCommand.Connection = connection;

                var adapter = factory.CreateDataAdapter();
                adapter.SelectCommand = selectCommand;
                
                var ds = new DataSet();
                adapter.Fill(ds);

                List<DataColumn> dataColumns = new List<DataColumn>();
                List<DatabaseColumn> databaseColumns = new List<DatabaseColumn>();
                if (ds.Tables.Count > 0)
                {
                    dataColumns = ds.Tables[0].Columns.OfType<DataColumn>().ToList();

                    for (int j = 0; j < dataColumns.Count; j++)
                    {
                        var dc = dataColumns[j];

                        if (dc.DataType == typeof(String))
                        {
                            DatabaseColumn databaseColumn = new DatabaseColumn()
                            {
                                ColumnName = dc.ColumnName,
                                DataType = dc.DataType,
                                IsUnique = dc.Unique
                            };

                            databaseColumns.Add(databaseColumn);
                        }
                    }
                }

                databaseTable.Columns.AddRange(databaseColumns);

                var countRowsCommand = factory.CreateCommand();
                var countRowColumn =
                    dataColumns.FirstOrDefault(x => x.Unique && x.ColumnName.ToLower().EndsWith("id"))
                    ?? dataColumns.FirstOrDefault(x => x.Unique)
                    ?? dataColumns.First();
                countRowsCommand.CommandText =
                    GetDatabaseTableCountRowsStatement(
                        databaseTable,
                        new DatabaseColumn()
                        {
                            ColumnName = countRowColumn.ColumnName,
                            DataType = countRowColumn.DataType,
                            IsUnique = countRowColumn.Unique
                        }
                    );
                countRowsCommand.Connection = connection;

                DbDataReader reader = countRowsCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        databaseTable.RowCount = reader.GetInt32(0);
                    }
                }
                reader.Close();

                tableList.Add(databaseTable);
            }

            return tableList;
        }

        protected abstract void UpdateDatabaseTable(DataTable dt, DbDataAdapter adapter);

        public void MaskTable(DatabaseTable table, Dictionary<string, DataMaskerOptions> columnOptions, DbConnection connection)
        {
            DbProviderFactory factory = GetDbProviderFactory();

            var selectCommand = factory.CreateCommand();
            selectCommand.CommandText = GetDatabaseTableSelectStatement(table);
            selectCommand.Connection = connection;

            var adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = selectCommand;

            var commandBuilder = factory.CreateCommandBuilder();
            commandBuilder.DataAdapter = adapter;

            DataSet ds = new DataSet();
            adapter.Fill(ds);

            for (int t = 0; t < ds.Tables.Count; t++)
            {
                var dt = ds.Tables[t];
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    var dc = dt.Columns[c];
                    if (dc.DataType == typeof(String)
                        && table.Columns.Any(
                            x =>
                                x.ColumnName == dc.ColumnName
                                && x.DataType == dc.DataType
                                && x.IsUnique == dc.Unique)
                        )
                    {
                        var options =
                            columnOptions.ContainsKey(dc.ColumnName)
                            ? columnOptions[dc.ColumnName]
                            : new DataMaskerOptions() { IgnoreAngleBracketedTags = true, IgnoreJsonAttributes = true, IgnoreNumbers = false };

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var dr = dt.Rows[i];
                            string originalText = dr[dc.ColumnName].ToString();
                            dr[dc.ColumnName] = TextDataMasker.MaskText(originalText, options, maskDictionary);
                        }
                    }
                }

                UpdateDatabaseTable(dt, adapter);
            }
        }

        public void MaskDatabase()
        {
            DbProviderFactory factory = GetDbProviderFactory();

            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = this.connectionString;

                connection.Open();

                var databaseTables = ListTables(connection);
                for (int i = 0; i < databaseTables.Count; i++)
                {
                    var table = databaseTables[i];
                    MaskTable(table, new Dictionary<string, DataMaskerOptions>(), connection);
                }

                connection.Close();
            }
        }
    }
}