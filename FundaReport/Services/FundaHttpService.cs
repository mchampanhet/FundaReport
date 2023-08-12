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

        public async Task<FundaResponseModel> GetQueryResultsAsync(string query, int pageNumber, int pageSize)
        {
            var response = new FundaResponseModel();
            var uri = $"?{query}&page={pageNumber}&pageSize={pageSize}";
            try
            {
                var fundaResponse = await _httpClient.GetAsync(uri);
                
                if (fundaResponse.IsSuccessStatusCode)
                {
                    response.Content = JsonConvert.DeserializeObject<FundaResponseBaseModel>(await fundaResponse.Content.ReadAsStringAsync());
                }
                else
                {
                    response.IsFailed = true;
                }
            }
            catch (Exception ex)
            {
                response.IsFailed = true;
                response.Error = ex.Message;
            }

            return response;
        }
    }
}
