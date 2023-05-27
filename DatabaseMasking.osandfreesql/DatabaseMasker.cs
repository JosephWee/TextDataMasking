﻿using System.Data;
using Npgsql;
using TextDataMasking;

namespace DatabaseMasking.osandfreesql
{
    public class DatabaseMasker: TextDataMasking.DatabaseMasker
    {
        public DatabaseMasker(string ConnectionString)
            :base(ConnectionString, new MaskDictionary())
        {
        }

        public DatabaseMasker(string ConnectionString, MaskDictionary MaskDictionary)
            :base(ConnectionString, MaskDictionary)
        {
        }

        public override void MaskData()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                base.MaskData(connection);
            }
        }
    }
}