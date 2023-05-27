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

        protected void MaskData(DbConnection connection)
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

        public abstract void MaskData();
    }
}