using Microsoft.Extensions.Options;
using Etiket.DtoClient.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Etiket.DtoClient
{
    public class ApiClientService
    {
        private readonly HttpClient httpClient;
        public ApiClientService(IOptions<ApiClientOptions> apiClientOptions)
        {
            var options = apiClientOptions.Value;
            httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(options.ApiBaseAddress)
            };
        }
        public async Task<List<KumasTopDto>?> GetKumasTop(string topNo)
        {
            var url = $"/api/KumasTop/{topNo}";
            return await httpClient.GetFromJsonAsync<List<KumasTopDto>>(url);
        }
        public async Task<bool> KumasTopCheck(string topNo)
        {
            var url = $"/api/KumasTop/exists/{topNo}";
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return bool.Parse(result);
            }
            else return false;
        }
      

    }

}
