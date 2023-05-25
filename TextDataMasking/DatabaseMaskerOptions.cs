using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace TextDataMasking
{
    public class DatabaseMaskerOptions
    {
        protected bool _IgnoreAngleBracketedTags = false;
        public bool IgnoreAngleBracketedTags
        {
            get
            {
                return _IgnoreAngleBracketedTags;
            }
            set
            {
                _IgnoreAngleBracketedTags |= value;
            }
        }

        protected bool _IgnoreJson = false;
        public bool IgnoreJson
        {
            get
            {
                return _IgnoreJson;
            }
            set
            {
                _IgnoreJson |= value;
            }
        }
    }
}
