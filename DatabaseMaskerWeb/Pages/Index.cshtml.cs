using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace DatabaseMaskerWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly string CacheKey_RunningJobs = "RunningJobs";

        public Dictionary<string, string> RunningJobs { get; set; }

        public IndexModel(IConfiguration configuration, IMemoryCache memoryCache, ILogger<IndexModel> logger)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
            _logger = logger;
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
    }
}