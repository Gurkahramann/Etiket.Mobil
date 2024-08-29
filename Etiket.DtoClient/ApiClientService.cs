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
        //public async Task<IEnumerable<KumasTopDto>> GetKumasTopByTopNoAsync(string topNo)
        //{
        //    // API endpoint'ine istek atılır
        //    var response = await httpClient.GetAsync($"/api/KumasTop/{topNo}");

        //    // Eğer API isteği başarısız olursa null döndür
        //    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        //    {
        //        return null;
        //    }

        //    response.EnsureSuccessStatusCode();

        //    // JSON formatındaki veri string olarak alınır
        //    var jsonResponse = await response.Content.ReadAsStringAsync();

        //    // JSON verisi KumasTopDto listesine dönüştürülür
        //    var kumasTopList = JsonConvert.DeserializeObject<IEnumerable<KumasTopDto>>(jsonResponse);

        //    return kumasTopList;
        //}

    }

}
