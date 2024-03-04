using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleCurrencyRESTApplication
{
    internal class CurrencyModel
    {
        [JsonProperty("rates")]
        public List<Currency> rates { get; set; }

        [JsonProperty("tradingDate")]
        public string tradingDate { get; set; }
    }
    internal class Currency
    {
        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("bid")]
        public double bid { get; set; }

        [JsonProperty("ask")]
        public double ask { get; set; }
    }
}
