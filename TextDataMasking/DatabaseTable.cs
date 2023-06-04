using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDataMasking
{
    public class DatabaseTable
    {
        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public List<DatabaseColumn> Columns { get; set; } = new List<DatabaseColumn>();
    }
}
