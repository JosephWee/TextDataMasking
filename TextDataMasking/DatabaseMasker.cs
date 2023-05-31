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

        protected void MaskData(DbProviderFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = this.connectionString;
                
                connection.Open();

                DataTable tables = connection.GetSchema("Tables");

                for (int c = 0; c < tables.Columns.Count; c++)
                {
                    var column = tables.Columns[c];
                    string columnName = column.ColumnName;
                    Type columnType = column.DataType;
                }

                DataMaskerOptions options = new DataMaskerOptions()
                {
                    IgnoreAngleBracketedTags = true,
                    IgnoreJsonAttributes = true
                };

                for (int r = 0; r < tables.Rows.Count; r++)
                {
                    var row = tables.Rows[r];
                    string tableName = row["TABLE_NAME"].ToString();

                    var selectCommand = factory.CreateCommand();
                    selectCommand.CommandText = $"SELECT * FROM {tableName};";
                    selectCommand.Connection = connection;

                    var adapter = factory.CreateDataAdapter();
                    adapter.SelectCommand = selectCommand;

                    var commandBuilder = factory.CreateCommandBuilder();
                    commandBuilder.DataAdapter = adapter;

                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    for (int t = 0; t < ds.Tables.Count; t++)
                    {
                        var table = ds.Tables[t];
                        for (int c = 0; c < table.Columns.Count; c++)
                        {
                            var column = table.Columns[c];
                            if (column.DataType == typeof(String))
                            {
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    var record = table.Rows[i];
                                    string originalText = record[column.ColumnName].ToString();
                                    record[column.ColumnName] = TextDataMasker.MaskText(originalText, options, maskDictionary);
                                }
                            }
                        }

                        adapter.Update(
                            table
                            .Rows
                            .OfType<DataRow>()
                            .Where(x => x.RowState != DataRowState.Unchanged)
                            .ToArray());
                    }
                }

                connection.Close();
            }
        }

        public abstract void MaskData();
    }
}