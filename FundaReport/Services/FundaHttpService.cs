using FundaReport.Models;
using FundaReport.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FundaReport.Services
{
    public class FundaHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly FundaApiSettings _fundaApiSettings;

        public FundaHttpService(HttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _fundaApiSettings = appSettings.Value.FundaApiSettings;
            _httpClient.BaseAddress = new Uri(_fundaApiSettings.BaseUrl);
        }

        public async Task<FundaResponseBaseModel> GetQueryResults(string query, int pageNumber, int pageSize)
        {
            var uri = $"?{query}&page={pageNumber}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(uri);
            return JsonConvert.DeserializeObject<FundaResponseBaseModel>(await response.Content.ReadAsStringAsync());
        }
    }
}
