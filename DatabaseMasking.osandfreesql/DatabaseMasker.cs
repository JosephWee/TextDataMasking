using Npgsql;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Text;
using TextDataMasking;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

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

        protected override void UpdateDatabaseTable(DatabaseTable table, DataTable dt, DbDataAdapter adapter)
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

            if (dtTarget.Rows.Count > 0)
            {
                NpgsqlDataAdapter dataAdapter = adapter as NpgsqlDataAdapter;
                NpgsqlCommandBuilder cmdBuilder =
                    new NpgsqlCommandBuilder(dataAdapter);

                NpgsqlCommand insertCommand = cmdBuilder.GetInsertCommand(true);
                
                List<string> columnAssignments = new List<string>();
                List<string> whereClauses = new List<string>();
                
                var DataColumns = dt.Columns.OfType<DataColumn>().ToList();
                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
                List<NpgsqlParameter> pkParameters = new List<NpgsqlParameter>();
                
                for (int p = 0; p < insertCommand.Parameters.Count; p++)
                {
                    var parameter = insertCommand.Parameters[p].Clone();

                    if (parameter.SourceVersion == DataRowVersion.Current)
                    {
                        var column = table.Columns.FirstOrDefault(x => x.ColumnName == parameter.SourceColumn);
                        if (column != null)
                        {
                            if (parameter.NpgsqlDbType == NpgsqlDbType.Json
                                || parameter.NpgsqlDbType == NpgsqlDbType.Json)
                            {
                                columnAssignments.Add(
                                    $" \"{column.ColumnName}\" = (" +
                                    @" CASE" +
                                    $" WHEN {parameter.ParameterName} IS NULL THEN NULL" +
                                    $" ELSE {parameter.ParameterName}::json" +
                                    @" END )");
                            }
                            else
                            {
                                columnAssignments.Add(
                                    $" \"{column.ColumnName}\" = {parameter.ParameterName}"
                                );
                            }

                            parameters.Add(parameter);
                        }

                        var pkColumn = dt.PrimaryKey.FirstOrDefault(x => x.ColumnName == parameter.SourceColumn);
                        if (pkColumn != null)
                        {
                            whereClauses.Add(
                                $" \"{pkColumn.ColumnName}\" = {parameter.ParameterName}"
                            );

                            pkParameters.Add(parameter);
                        }
                    }
                }

                if (!whereClauses.Any())
                {
                    for (int i = 0; i < dt.PrimaryKey.Length; i++)
                    {
                        var pkColumn = dt.PrimaryKey[i];

                        NpgsqlDbType dbType = NpgsqlDbType.Integer;
                        if (pkColumn.DataType == typeof(Guid))
                        {
                            dbType = NpgsqlDbType.Uuid;
                        }

                        var parameter =
                            new NpgsqlParameter($"@{pkColumn.ColumnName}", dbType, -1, pkColumn.ColumnName);
                        
                        whereClauses.Add(
                                $" \"{pkColumn.ColumnName}\" = {parameter.ParameterName}"
                            );

                        pkParameters.Add(parameter);
                    }
                }

                StringBuilder updateQuery = new StringBuilder();
                updateQuery
                    .AppendLine(
                        $"UPDATE \"{table.TableSchema}\".\"{table.TableName}\""
                    );
                updateQuery.AppendLine("SET");
                updateQuery.AppendLine(string.Join(",\r\n", columnAssignments));
                updateQuery.AppendLine("WHERE");
                updateQuery.Append(string.Join("\r\nAND ", whereClauses));
                updateQuery.Append(";");

                var connection =
                    (NpgsqlConnection)insertCommand.Connection;

                var updateCommand =
                    new NpgsqlCommand(updateQuery.ToString(), connection);
                updateCommand.Parameters.AddRange(parameters.Select(x => x.Clone()).ToArray());
                updateCommand.Parameters.AddRange(pkParameters.Select(x => x.Clone()).ToArray());

                dataAdapter.UpdateCommand = updateCommand;

                dataAdapter.Update(
                    dtTarget.Rows.OfType<DataRow>().ToArray()
                );
            }
        }
    }
}