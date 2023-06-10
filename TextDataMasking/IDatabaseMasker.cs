using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDataMasking
{
    public interface IDatabaseMasker
    {
        DbProviderFactory GetDbProviderFactory();

        List<DatabaseTable> ListTables(DbConnection connection, bool includeAllColumns);

        void MaskTable(DatabaseTable table, Dictionary<string, DataMaskerOptions> columnOptions, DbConnection connection);

        void MaskDatabase();
    }
}
