using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDataMasking
{
    public class DatabaseColumn
    {
        public string ColumnName { get; set; }

        public Type DataType { get; set; }

        public bool IsUnique { get; set; }
    }
}
