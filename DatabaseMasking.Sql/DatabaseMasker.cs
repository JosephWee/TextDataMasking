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

        public override DbProviderFactory GetDbProviderFactory()
        {
            return Microsoft.Data.SqlClient.SqlClientFactory.Instance;
        }

        protected override string GetDatabaseColumnSelectStatement(DatabaseTable table)
        {
            string selectCommandText = $"SELECT TOP 1 * FROM {table.TableSchema}.{table.TableName};";

            return selectCommandText;
        }

        protected override string GetDatabaseTableSelectStatement(DatabaseTable table)
        {
            string selectCommandText = $"SELECT * FROM {table.TableSchema}.{table.TableName};";

            return selectCommandText;
        }

        protected override string GetDatabaseTableCountRowsStatement(DatabaseTable table, DatabaseColumn column)
        {
            string selectCommandText = $"SELECT COUNT({column.ColumnName}) FROM {table.TableSchema}.{table.TableName};";

            return selectCommandText;
        }

        protected override void UpdateDatabaseTable(DatabaseTable table, DataTable dt, DbDataAdapter adapter)
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

            SqlDataAdapter dataAdapter = adapter as SqlDataAdapter;
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(dataAdapter);
            
            SqlCommand updateCommand = cmdBuilder.GetUpdateCommand(true);
            adapter.UpdateCommand = updateCommand;

            adapter.Update(
                dtTarget.Rows.OfType<DataRow>().ToArray()
            );
        }
    }
}