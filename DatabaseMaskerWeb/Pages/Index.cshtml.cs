using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Common;
using System.Reflection;
using System.Text.Json;
using TextDataMasking;
using static DatabaseMaskerWeb.Pages.IndexModel;

namespace DatabaseMaskerWeb.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public class DataSource
        {
            protected string _ConnectionString = string.Empty;
            public string ConnectionString
            {
                get
                {
                    return _ConnectionString;
                }
            }

            protected string _DatabaseMaskingProvider = string.Empty;
            public string DatabaseMaskingProvider
            {
                get
                {
                    return _DatabaseMaskingProvider;
                }
            }

            public DataSource(string connectionString, string databaseMaskingProvider)
            {
                _ConnectionString = connectionString;
                _DatabaseMaskingProvider = databaseMaskingProvider;
            }
        }

        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;

        protected JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();
        protected JsonSerializerOptions jsonSerializerOptions
        {
            get
            {
                _jsonSerializerOptions.PropertyNameCaseInsensitive = true;
                return _jsonSerializerOptions;
            }
        }

        public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected Dictionary<string, string> GetConnectionStrings()
        {
            var connectionStringsSection = _configuration.GetSection("ConnectionStrings");
            var connectionStrings = connectionStringsSection.GetChildren().ToList();

            return connectionStrings.ToDictionary(x => x.Key, x => x.Value);
        }

        protected Dictionary<string, string> GetDatabaseMaskingProviders()
        {
            var databaseMaskingProvidersSection = _configuration.GetSection("DatabaseMaskingProviders");
            var databaseMaskingProviders = databaseMaskingProvidersSection.GetChildren().ToList();

            return databaseMaskingProviders.ToDictionary(x => x.Key, x => x.Value);
        }

        protected Dictionary<string, DataSource> GetDataSources()
        {
            var connectionStrings = GetConnectionStrings();
            var databaseMaskingProviders = GetDatabaseMaskingProviders();

            Dictionary<string, DataSource> dataSources = new Dictionary<string, DataSource>();
            for (int i = 0; i < connectionStrings.Count; i++)
            {
                var key = connectionStrings.Keys.ElementAt(i);
                if (databaseMaskingProviders.ContainsKey(key))
                {
                    var dataSource =
                        new DataSource(
                            connectionStrings[key],
                            databaseMaskingProviders[key]
                        );

                    dataSources.Add(key, dataSource);
                }
            }

            return dataSources;
        }

        protected TextDataMasking.DatabaseMasker GetDatabaseMasker(DataSource dataSource)
        {
            TextDataMasking.DatabaseMasker databaseMasker = null;

            try
            {
                var assembly = Assembly.Load(dataSource.DatabaseMaskingProvider);
                var types = assembly.GetTypes();
                var type = types.FirstOrDefault(x => x.FullName == assembly.GetName().Name + ".DatabaseMasker");

                databaseMasker =
                    assembly.CreateInstance(
                        type.FullName,
                        false,
                        BindingFlags.Default,
                        null,
                        new object[] { dataSource.ConnectionString, new TextDataMasking.MaskDictionary() },
                        System.Globalization.CultureInfo.CurrentCulture,
                        null) as TextDataMasking.DatabaseMasker;

            }
            catch (Exception ex)
            {
            }

            return databaseMasker;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnGetListTargetDataSourcesAsync()
        {
            var dataSources = GetDataSources();

            return new JsonResult(dataSources.Keys.ToList(), jsonSerializerOptions);
        }

        public async Task<IActionResult> OnGetListDatabaseTablesAsync(string dataSourceName)
        {
            var datasources = GetDataSources();

            if (!datasources.Any(x => x.Key == dataSourceName))
                return new NotFoundResult();

            DataSource dataSource = datasources[dataSourceName];

            var databaseMasker = GetDatabaseMasker(dataSource);

            if (databaseMasker == null)
                return new NotFoundResult();

            try
            {
                DbProviderFactory factory = databaseMasker.GetDbProviderFactory();
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = dataSource.ConnectionString;

                    connection.Open();

                    List<DatabaseTable> databaseTables = databaseMasker.ListTables(connection);

                    connection.Close();

                    return new JsonResult(databaseTables, jsonSerializerOptions);
                }
            }
            catch (Exception ex)
            {
            }

            return new NotFoundResult();
        }
    }
}