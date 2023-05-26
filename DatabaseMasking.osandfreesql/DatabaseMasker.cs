using System.Data;
using Npgsql;
using TextDataMasking;

namespace DatabaseMasking.osandfreesql
{
    public class DatabaseMasker: IDatabaseMasker
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

        public void MaskData()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                // Connect to the database then retrieve the schema information.  
                connection.Open();
                DataTable tables = connection.GetSchema("Tables");
                for (int i = 0; i < tables.Columns.Count; i++)
                {
                    var column = tables.Columns[i];

                }

                for (int i = 0; i < tables.Rows.Count; i++)
                {
                    var row = tables.Rows[i];
                    var table_name = row["TABLE_NAME"];
                }
            }
        }
    }
}