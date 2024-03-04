using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleCurrencyRESTApplication
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await EndpointHandler.GetData();
            List<string> headers = EndpointHandler.GetCurrencyHeaders();
            List<CurrencyModel> dailyValues = EndpointHandler.GetMultipleCurrencyValue();
            SheetsService auth = GoogleSheetsAPI.AuthenticateViaJson();
            await Console.Out.WriteLineAsync("Please wait...");
            GoogleSheetsAPI.FillCurrencyHeaders(auth, headers);
            GoogleSheetsAPI.FillCurrencyValues(auth, dailyValues);
            await Console.Out.WriteLineAsync("Press any key to exit window...");
            Console.ReadKey();
        }
    }
}
