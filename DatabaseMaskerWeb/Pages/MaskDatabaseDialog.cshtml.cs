using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Buffers;
using System.Data.Common;
using System.Reflection;
using System.Text.Json;
using TextDataMasking;

namespace DatabaseMaskerWeb.Pages
{
    //[Authorize]
    public class MaskDatabaseDialogModel : PageModel
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

        public class MaskDatabaseRequest
        {
            public string DataSourceName { get; set;}
            public List<DatabaseTable> DatabaseTables { get; set; }
            public Dictionary<string, DataMaskerOptions> DataMaskerOptions { get; set; }
        }

        private readonly ILogger<MaskDatabaseDialogModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly string CacheKey_RunningJobs = "RunningJobs";

        protected JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();
        protected JsonSerializerOptions jsonSerializerOptions
        {
            get
            {
                _jsonSerializerOptions.PropertyNameCaseInsensitive = true;
                return _jsonSerializerOptions;
            }
        }

        public Dictionary<string, string> RunningJobs { get; set; }

        public MaskDatabaseDialogModel(IConfiguration configuration, IMemoryCache memoryCache, ILogger<MaskDatabaseDialogModel> logger)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
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

        protected void Init_RunningJobsCache_IfNotExist()
        {
            Dictionary<string, string> runningJobs = null;

            if (!_memoryCache.TryGetValue(CacheKey_RunningJobs, out runningJobs))
            {
                runningJobs = new Dictionary<string, string>();

                var cacheEntryOptions =
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));

                _memoryCache.Set(CacheKey_RunningJobs, runningJobs, cacheEntryOptions);
            }
        }

        protected Dictionary<string, string> GetRunningJobsCache()
        {
            Dictionary<string, string> runningJobs = null;
            _memoryCache.TryGetValue(CacheKey_RunningJobs, out runningJobs);

            return runningJobs;
        }

        public void OnGet()
        {
            Init_RunningJobsCache_IfNotExist();
            RunningJobs = GetRunningJobsCache();
        }

        public async Task<IActionResult> OnGetListTargetDataSourcesAsync()
        {
            var availDataSources = new List<string>();

            var dataSources = GetDataSources();
            var runningJobs = GetRunningJobsCache();

            var dataSourceNames = dataSources.Keys.ToList();
            for (int i = 0; i < dataSourceNames.Count; i++)
            {
                var dataSourceName = dataSourceNames[i];
                if (!runningJobs.ContainsKey(dataSourceName))
                    availDataSources.Add(dataSourceName);
            }

            return new JsonResult(availDataSources, jsonSerializerOptions);
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

                    List<DatabaseTable> databaseTables = databaseMasker.ListTables(connection, false);

                    connection.Close();

                    return new JsonResult(databaseTables, jsonSerializerOptions);
                }
            }
            catch (Exception ex)
            {
            }

            return new NotFoundResult();
        }

        public void OnPost()
        {
        }

        public async Task<IActionResult> OnPostMaskTablesAsync()
        {
            var readResult = await this.Request.BodyReader.ReadAsync();

            var bytes = readResult.Buffer.ToArray();
            var bodyText = System.Text.Encoding.UTF8.GetString(bytes);

            try
            {
                var maskDatabaseRequest =
                    JsonSerializer.Deserialize<MaskDatabaseRequest>(bodyText);

                var DataSources = GetDataSources();
                if (!DataSources.ContainsKey(maskDatabaseRequest.DataSourceName))
                    return new BadRequestResult();

                var runningJobs = GetRunningJobsCache();

                if (runningJobs == null)
                    return new BadRequestResult();

                if (runningJobs.ContainsKey(maskDatabaseRequest.DataSourceName))
                    return new BadRequestResult();

                var dataSource = DataSources[maskDatabaseRequest.DataSourceName];
                var databaseMasker = GetDatabaseMasker(dataSource);
                var factory = databaseMasker.GetDbProviderFactory();
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = dataSource.ConnectionString;
                    connection.Open();

                    var dbTables = databaseMasker.ListTables(connection, false);
                    for (int i = 0; i < maskDatabaseRequest.DatabaseTables.Count; i++)
                    {
                        var reqdbTable = maskDatabaseRequest.DatabaseTables[i];
                        
                        var dbTable =
                            dbTables
                            .FirstOrDefault(
                                x =>
                                    x.TableSchema == reqdbTable.TableSchema
                                    && x.TableName == reqdbTable.TableName);

                        if (dbTable == null)
                            return new BadRequestResult();

                        for (int c = 0; c < reqdbTable.Columns.Count; c++)
                        {
                            var reqdbColumn = reqdbTable.Columns[c];

                            var dbColumn =
                                dbTable
                                .Columns
                                .FirstOrDefault(
                                    x =>
                                        x.ColumnName == reqdbColumn.ColumnName
                                        && x.DataType == reqdbColumn.DataType);

                            if (dbColumn == null)
                                return new BadRequestResult();
                        }
                    }
                    
                    connection.Close();
                }

                var job =
                    Task.Run(delegate {

                        try
                        {
                            using (DbConnection connection = factory.CreateConnection())
                            {
                                connection.ConnectionString = dataSource.ConnectionString;
                                connection.Open();

                                DataMaskerOptions defaultOptions = new DataMaskerOptions();
                                defaultOptions.IgnoreAngleBracketedTags = true;
                                defaultOptions.IgnoreJsonAttributes = true;

                                for (int i = 0; i < maskDatabaseRequest.DatabaseTables.Count; i++)
                                {
                                    var dbtable = maskDatabaseRequest.DatabaseTables[i];

                                    Dictionary<string, DataMaskerOptions> dbcolumnsOptions = new Dictionary<string, DataMaskerOptions>();

                                    for (int c = 0; c < dbtable.Columns.Count; c++)
                                    {
                                        var dbcolumn = dbtable.Columns[c];
                                        if (maskDatabaseRequest.DataMaskerOptions.ContainsKey(dbcolumn.ColumnName))
                                        {
                                            dbcolumnsOptions.Add(
                                                dbcolumn.ColumnName,
                                                maskDatabaseRequest.DataMaskerOptions[dbcolumn.ColumnName]);
                                        }
                                        else
                                        {
                                            dbcolumnsOptions.Add(dbcolumn.ColumnName, defaultOptions);
                                        }
                                    }

                                    databaseMasker.MaskTable(dbtable, dbcolumnsOptions, connection);
                                }

                                connection.Close();
                            }

                            runningJobs.Remove(maskDatabaseRequest.DataSourceName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Unknown Error @ OnPostMaskTablesAsync Database Masking Task");
                        }
                    });

                runningJobs.Add(maskDatabaseRequest.DataSourceName, DateTime.Now.ToString("O"));

                return new JsonResult(maskDatabaseRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown Error @ OnPostMaskTablesAsync");
            }

            return new BadRequestResult();
        }
    }
}
