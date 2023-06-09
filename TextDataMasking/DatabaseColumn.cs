﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TextDataMasking
{
    public class DatabaseColumn
    {
        public string ColumnName { get; set; }

        [JsonIgnore]
        public Type DataType { get; set; }

        [JsonPropertyName("DataType")]
        public string DataTypeName
        {
            get
            {
                var type = this.DataType;
                if (type == null)
                    return string.Empty;

                return type.FullName;
            }
            set
            {
                var type = Type.GetType(value, false, true);
                this.DataType = type;
            }
        }

        public string DbType { get; set; }

        public bool IsUnique { get; set; }
    }
}
