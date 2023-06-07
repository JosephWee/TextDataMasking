using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace TextDataMasking
{
    public class DataMaskerOptions
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

        protected bool _IgnoreJsonAttributes = false;
        public bool IgnoreJsonAttributes
        {
            get
            {
                return _IgnoreJsonAttributes;
            }
            set
            {
                _IgnoreJsonAttributes |= value;
            }
        }

        protected bool _IgnoreNumbers = false;
        public bool IgnoreNumbers
        {
            get
            {
                return _IgnoreNumbers;
            }
            set
            {
                _IgnoreNumbers |= value;
            }
        }

        protected bool _PreserveCase = false;
        public bool PreserveCase
        {
            get
            {
                return _PreserveCase;
            }
            set
            {
                _PreserveCase |= value;
            }
        }
    }
}
