using Serilog;

namespace Hangfire.Hangfire
{
    public class ApiCallService
    {
        private readonly HttpClient _httpClient;
        public ApiCallService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CallScheduleApi()
        {
            var response = await _httpClient.GetAsync("http://localhost:5012/api/values");
            var content = await response.Content.ReadAsStringAsync();
            Log.Information($"API Response: {content}");
        }
    }
}
