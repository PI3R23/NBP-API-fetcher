using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleCurrencyRESTApplication
{
    internal class EndpointHandler
    {
        static List<CurrencyModel> tables = new List<CurrencyModel>();
        private static DateTime _startDate = new DateTime(2011, 1, 3);
        private static DateTime _endDate = _startDate.AddDays(90);
        private static readonly DateTime _today = DateTime.Today;
        public static string _apiUrl = $"https://api.nbp.pl/api/exchangerates/tables/C/{_startDate:yyyy-MM-dd}/{_endDate:yyyy-MM-dd}?format=json";

        public static async Task GetData()
        {
            using(HttpClient httpClient = new HttpClient()) 
            {
                try
                {
                    int xd = _today.Subtract(_startDate).Days / 90;
                    for (int i = 0; i <= xd; i++) 
                    {
                        HttpResponseMessage responseMessage = await httpClient.GetAsync(_apiUrl);
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            string responseData = await responseMessage.Content.ReadAsStringAsync();
                            List<CurrencyModel> temp = JsonConvert.DeserializeObject<List<CurrencyModel>>(responseData);
                            tables.AddRange(temp);

                            _startDate = _endDate;
                            _endDate = _startDate.AddDays(90);
                            if (_endDate > _today)
                                _endDate = _today;

                            _apiUrl = $"https://api.nbp.pl/api/exchangerates/tables/C/{_startDate:yyyy-MM-dd}/{_endDate:yyyy-MM-dd}?format=json";

                        }
                        else { await Console.Out.WriteLineAsync($"Error: {responseMessage.StatusCode}"); }
                    }
                }
                catch(Exception ex) 
                {
                    await Console.Out.WriteLineAsync($"Error: {ex.Message}");
                }
            }
        }

        public static List<string> GetCurrencyHeaders()
        {
            List<string> headers = new List<string>();
            foreach(Currency val in tables[0].rates) 
            {
                headers.Add(val.code);
            }
            return headers;
        }

        public static List<CurrencyModel> GetMultipleCurrencyValue()
        {

            return tables.OrderByDescending(date => DateTime.Parse(date.tradingDate)).ToList();
        }
    }
}
