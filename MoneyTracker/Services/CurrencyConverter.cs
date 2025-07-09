using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace MoneyTracker.Services
{
    public class CurrencyConverter
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount) 
        {
            string url = $"https://api.exchangerate.host/convert?from={fromCurrency}&to={toCurrency}&amount={amount}";

            var response = await client.GetAsync(url) ;
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<CurrencyApiResponse>(json);

            return apiResponse != null ? apiResponse.Result : 0;

        }

        public class CurrencyApiResponse
        {

            [JsonProperty("result")]
            public decimal Result { get; set; }
        }

    }
}
