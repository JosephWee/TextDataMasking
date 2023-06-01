using System.Data;
using System.Data.Common;
using TextDataMasking;
using Microsoft.Data.SqlClient;

namespace DatabaseMasking.Sql
{
    public class DatabaseMasker : TextDataMasking.DatabaseMasker
    {
        public DatabaseMasker(string ConnectionString)
            : base(ConnectionString, new MaskDictionary())
        {
        }

        public DatabaseMasker(string ConnectionString, MaskDictionary MaskDictionary)
            : base(ConnectionString, MaskDictionary)
        {
        }

        protected override string GetDatabaseColumnSelectStatement(DatabaseTable table)
        {
            string selectCommanText = $"SELECT TOP 1 * FROM {table.TableSchema}.{table.TableName};";

            return selectCommanText;
        }

        protected override string GetDatabaseTableSelectStatement(DatabaseTable table)
        {
            string selectCommanText = $"SELECT * FROM {table.TableSchema}.{table.TableName};";

            return selectCommanText;
        }

        protected override void UpdateDatabaseTable(DataTable dt, DbDataAdapter adapter)
        {
            var dtTarget = dt.Clone();
            
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

        public override void MaskData()
        {
            base.MaskData(Microsoft.Data.SqlClient.SqlClientFactory.Instance);
        }
    }
}