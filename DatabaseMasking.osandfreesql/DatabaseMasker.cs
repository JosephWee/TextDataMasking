using System.Data;
using System.Data.Common;
using TextDataMasking;

namespace DatabaseMasking.osandfreesql
{
    public class DatabaseMasker: TextDataMasking.DatabaseMasker
    {
        public DatabaseMasker(string ConnectionString)
            :base(ConnectionString, new MaskDictionary())
        {
        }

        public DatabaseMasker(string ConnectionString, MaskDictionary MaskDictionary)
            :base(ConnectionString, MaskDictionary)
        {
        }

        public override DbProviderFactory GetDbProviderFactory()
        {
            return Npgsql.NpgsqlFactory.Instance;
        }

        protected override string GetDatabaseColumnSelectStatement(DatabaseTable table)
        {
            string selectCommandText = $"SELECT * FROM \"{table.TableSchema}\".\"{table.TableName}\" LIMIT 1;";
            
            return selectCommandText;
        }

        protected override string GetDatabaseTableSelectStatement(DatabaseTable table)
        {
            string selectCommandText = $"SELECT * FROM \"{table.TableSchema}\".\"{table.TableName}\";";

            return selectCommandText;
        }

        protected override string GetDatabaseTableCountRowsStatement(DatabaseTable table, DatabaseColumn column)
        {
            string selectCommandText = $"SELECT COUNT(\"{column.ColumnName}\") FROM \"{table.TableSchema}\".\"{table.TableName}\";";

            return selectCommandText;
        }

        protected override void UpdateDatabaseTable(DataTable dt, DbDataAdapter adapter)
        {
            var dtTarget = dt.Clone(); ;
            for (int c = 0; c < dt.Columns.Count; c++)
            {
                var dc = dt.Columns[c];
                if (dc.DataType == typeof(DateTime) && dc.DateTimeMode == DataSetDateTime.UnspecifiedLocal)
                {
                    dtTarget.Columns[c].DateTimeMode = DataSetDateTime.Utc;
                }
            }

            var sourceRows =
                dt.Rows
                .OfType<DataRow>()
                .Where(x => x.RowState != DataRowState.Unchanged)
                .ToList();

            for (int r = 0; r < sourceRows.Count; r++)
            {
                dtTarget.ImportRow(sourceRows[r]);
            }

            adapter.Update(
                dtTarget.Rows.OfType<DataRow>().ToArray()
            );
        }
    }
}