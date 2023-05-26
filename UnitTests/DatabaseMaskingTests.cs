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
        IDatabaseMasker databaseMasker = null;
        
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
                        databaseMasker = assembly.CreateInstance(type.FullName, false, BindingFlags.Default, null, args, System.Globalization.CultureInfo.CurrentCulture, null) as IDatabaseMasker;
                    }
                    catch (Exception ex)
                    {
                    }
                }

                Assert.IsNotNull(databaseMasker);
            }
        }

        [Test]
        public void TestDatabaseMasking()
        {
            databaseMasker.MaskData();
            Assert.IsTrue(true);
        }
    }
}