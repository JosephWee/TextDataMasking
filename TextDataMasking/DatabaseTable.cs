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

        protected List<DatabaseColumn> _Columns = new List<DatabaseColumn>();
        public List<DatabaseColumn> Columns
        {
            get
            {
                return _Columns;
            }
        }
    }
}
