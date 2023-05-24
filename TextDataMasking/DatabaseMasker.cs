using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDataMasking
{
    public class DatabaseMasker
    {
        protected string _ConnectionString = string.Empty;
        public DatabaseMasker(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }

        public void MaskData()
        {

        }
    }
}
