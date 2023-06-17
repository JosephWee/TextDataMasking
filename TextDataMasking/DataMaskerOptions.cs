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
                _IgnoreAngleBracketedTags = value;
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
                _IgnoreJsonAttributes = value;
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
                _IgnoreNumbers = value;
            }
        }

        protected bool _IgnoreAlphaNumeric = false;
        public bool IgnoreAlphaNumeric
        {
            get
            {
                return _IgnoreAlphaNumeric;
            }
            set
            {
                _IgnoreAlphaNumeric = value;
            }
        }

        protected bool _ProcessCDATA = false;
        public bool ProcessCDATA
        {
            get
            {
                return _ProcessCDATA;
            }
            set
            {
                _ProcessCDATA = value;
            }
        }

        protected bool _ProcessXmlComments = false;
        public bool ProcessXmlComments
        {
            get
            {
                return _ProcessXmlComments;
            }
            set
            {
                _ProcessXmlComments = value;
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
                _PreserveCase = value;
            }
        }

        public DataMaskerOptions Clone()
        {
            DataMaskerOptions clone = new DataMaskerOptions()
            {
                IgnoreAngleBracketedTags = this.IgnoreAngleBracketedTags,
                IgnoreJsonAttributes = this.IgnoreJsonAttributes,
                IgnoreNumbers = this.IgnoreNumbers,
                IgnoreAlphaNumeric = this.IgnoreAlphaNumeric,
                ProcessCDATA = this.ProcessCDATA,
                ProcessXmlComments = this.ProcessXmlComments,
                PreserveCase = this.PreserveCase
            };

            return clone;
        }
    }
}
