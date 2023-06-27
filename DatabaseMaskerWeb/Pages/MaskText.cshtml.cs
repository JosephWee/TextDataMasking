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
    public class MaskTextModel : PageModel
    {
        public class MaskTextRequest
        {
            public string RequestId { get; set; }
            public string OriginalText { get; set; }
            public DataMaskerOptions Options { get; set; }
        }

        public class MaskTextResponse
        {
            public string RequestId { get; set; }
            public string MaskedText { get; set; }
        }

        private readonly ILogger<MaskTextModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        //private readonly string CacheKey_RunningJobs = "RunningJobs";

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

        public MaskTextModel(IConfiguration configuration, IMemoryCache memoryCache, ILogger<MaskTextModel> logger)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
        }

        public async Task<IActionResult> OnPostMaskTextAsync()
        {
            var readResult = await this.Request.BodyReader.ReadAsync();

            var bytes = readResult.Buffer.ToArray();
            var bodyText = System.Text.Encoding.UTF8.GetString(bytes);

            try
            {
                var maskTextRequest =
                    JsonSerializer.Deserialize<MaskTextRequest>(bodyText);

                DataMaskerOptions options = maskTextRequest.Options;

                MaskDictionary maskDictionary = new MaskDictionary();
                string maskedText = TextDataMasker.MaskText(maskTextRequest.OriginalText, options, maskDictionary);

                MaskTextResponse maskTextResponse = new MaskTextResponse();
                maskTextResponse.RequestId = maskTextRequest.RequestId;
                maskTextResponse.MaskedText = maskedText;

                JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
                jsonSerializerOptions.PropertyNameCaseInsensitive = true;

                return new JsonResult(maskTextResponse, jsonSerializerOptions);
            }
            catch (Exception ex)
            {
            }

            return new BadRequestResult();
        }
    }
}
