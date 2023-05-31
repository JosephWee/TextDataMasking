using System.Data;
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
            base.MaskData(Microsoft.Data.SqlClient.SqlClientFactory.Instance);
        }
    }
}