using TextDataMasking;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Reflection;
using System.Linq;
using System.Security.AccessControl;

namespace UnitTests
{
    public class DatabaseMaskingTests
    {
        MaskDictionary maskDictionary = new MaskDictionary();
        DatabaseMasker databaseMasker = null;
        
        [SetUp]
        public void Setup()
        {
            if (databaseMasker == null)
            {
                string DatabaseMaskingProvider = TestContext.Parameters.Get("DatabaseMaskingProvider", string.Empty);
                string ConnectionString = TestContext.Parameters.Get("ConnectionString", string.Empty);

                if (!string.IsNullOrWhiteSpace(DatabaseMaskingProvider) && !string.IsNullOrWhiteSpace(ConnectionString))
                {
                    try
                    {
                        var assembly = Assembly.Load(DatabaseMaskingProvider);
                        string typeName = assembly.GetName().Name + ".DatabaseMasker";
                        Type type = assembly.GetType(typeName);
                        object[] args = { ConnectionString, maskDictionary };
                        databaseMasker = assembly.CreateInstance(type.FullName, false, BindingFlags.Default, null, args, System.Globalization.CultureInfo.CurrentCulture, null) as DatabaseMasker;
                    }
                    catch (Exception ex)
                    {
                    }
                }

                Assert.IsNotNull(databaseMasker);
            }
        }

        [Test]
        public void MaskDatabase()
        {
            databaseMasker.MaskDatabase();
            Assert.IsTrue(true);
        }

        [Test]
        public void MaskDatabaseTable_IgnoreAll()
        {
            DataMaskerOptions options = new DataMaskerOptions()
            {
                IgnoreAngleBracketedTags = true,
                IgnoreJsonAttributes = true,
                IgnoreNumbers = true,
                PreserveCase = true
            };

            MaskDatabaseTable(options);
        }

        [Test]
        public void MaskDatabaseTable_IgnoreAngleBracketedTags()
        {
            DataMaskerOptions options = new DataMaskerOptions()
            {
                IgnoreAngleBracketedTags = true,
                IgnoreJsonAttributes = false,
                IgnoreNumbers = false,
                PreserveCase = true
            };

            MaskDatabaseTable(options);
        }

        [Test]
        public void MaskDatabaseTable_IgnoreJsonAttributes()
        {
            DataMaskerOptions options = new DataMaskerOptions()
            {
                IgnoreAngleBracketedTags = false,
                IgnoreJsonAttributes = true,
                IgnoreNumbers = false,
                PreserveCase = true
            };

            MaskDatabaseTable(options);
        }

        [Test]
        public void MaskDatabaseTable_IgnoreNumbers()
        {
            DataMaskerOptions options = new DataMaskerOptions()
            {
                IgnoreAngleBracketedTags = false,
                IgnoreJsonAttributes = false,
                IgnoreNumbers = true,
                PreserveCase = true
            };

            MaskDatabaseTable(options);
        }

        public void MaskDatabaseTable(DataMaskerOptions options)
        {
            var factory = databaseMasker.GetDbProviderFactory();
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = databaseMasker.ConnectionString;
                connection.Open();
                
                var DatabaseTables = databaseMasker.ListTables(connection);

                for (int i = 0; i < DatabaseTables.Count; i++)
                {
                    var DatabaseTable = DatabaseTables[i];

                    var columnOptions = new Dictionary<string, DataMaskerOptions>();
                    for (int c = 0; c < DatabaseTable.Columns.Count; c++)
                    {
                        var dbColumn = DatabaseTable.Columns[c];
                        columnOptions.Add(dbColumn.ColumnName, options);
                    }

                    databaseMasker.MaskTable(DatabaseTable, columnOptions, connection);
                }

                connection.Close();
            }

            Assert.IsTrue(true);
        }
    }
}