using System.Data;
using Microsoft.Data.SqlClient;
using TextDataMasking;

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

        public override void MaskData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                base.MaskData(connection);
            }
        }
    }
}