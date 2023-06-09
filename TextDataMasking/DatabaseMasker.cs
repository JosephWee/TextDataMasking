﻿using System.Data;
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

        protected abstract string GetDbTypeName(DbParameter parameter);

        public List<DatabaseTable> ListTables(DbConnection connection, bool includeAllColumns)
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

                var cmdBuilder = factory.CreateCommandBuilder();
                cmdBuilder.DataAdapter = adapter;
                var updateCommand = cmdBuilder.GetUpdateCommand(true);

                var ds = new DataSet();
                adapter.FillSchema(ds, SchemaType.Mapped);

                List<DataColumn> dataColumns = new List<DataColumn>();
                List<DatabaseColumn> databaseColumns = new List<DatabaseColumn>();
                if (ds.Tables.Count > 0)
                {
                    dataColumns = ds.Tables[0].Columns.OfType<DataColumn>().ToList();

                    for (int j = 0; j < dataColumns.Count; j++)
                    {
                        var dc = dataColumns[j];

                        if (dc.DataType == typeof(String) || includeAllColumns)
                        {
                            var parameterName = "@" + dc.ColumnName;
                            var parameter = updateCommand.Parameters[parameterName];
                            
                            DatabaseColumn databaseColumn = new DatabaseColumn()
                            {
                                ColumnName = dc.ColumnName,
                                DataType = dc.DataType,
                                DbType = GetDbTypeName(parameter),
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

        protected abstract void UpdateDatabaseTable(DatabaseTable table, DataTable dt, DbDataAdapter adapter);

        public void MaskTable(DatabaseTable table, Dictionary<string, DataMaskerOptions> columnOptions, DbConnection connection)
        {
            DbProviderFactory factory = GetDbProviderFactory();

            var selectCommand = factory.CreateCommand();
            selectCommand.CommandText = GetDatabaseTableSelectStatement(table);
            selectCommand.Connection = connection;

            var adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = selectCommand;

            //var commandBuilder = factory.CreateCommandBuilder();
            //commandBuilder.DataAdapter = adapter;

            DataTable dt = new DataTable();
            adapter.FillSchema(dt, SchemaType.Mapped);
            adapter.Fill(dt);

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
                    var column =
                        table
                        .Columns
                        .First(x => x.ColumnName == dc.ColumnName);

                    var options =
                        columnOptions.ContainsKey(dc.ColumnName)
                        ? columnOptions[dc.ColumnName]
                        : new DataMaskerOptions()
                          {
                            IgnoreAngleBracketedTags = true,
                            IgnoreJsonAttributes = true,
                            IgnoreNumbers = false,
                            PreserveCase = true
                          };

                    // Override user's Masking Options because Databases
                    // usually have well-formed validations for XML and
                    // JSON columns:

                    // XML Columns: IgnoreAngleBracketedTags = true
                    options.IgnoreAngleBracketedTags =
                        options.IgnoreAngleBracketedTags
                        || column.DbType.ToLower() == "xml";

                    // JSON Columns: IgnoreJsonAttributes = true
                    options.IgnoreJsonAttributes =
                        options.IgnoreJsonAttributes
                        || column.DbType.ToLower().StartsWith("json");

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];
                        if (dr.IsNull(dc.ColumnName))
                        {
                            dr[dc.ColumnName] = DBNull.Value;
                        }
                        else
                        {
                            string originalText = dr[dc.ColumnName].ToString();
                            dr[dc.ColumnName] = TextDataMasker.MaskText(originalText, options, maskDictionary);
                        }
                    }
                }
            }

            UpdateDatabaseTable(table, dt, adapter);
        }

        public void MaskDatabase()
        {
            DbProviderFactory factory = GetDbProviderFactory();

            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = this.connectionString;

                connection.Open();

                var databaseTables = ListTables(connection, false);
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