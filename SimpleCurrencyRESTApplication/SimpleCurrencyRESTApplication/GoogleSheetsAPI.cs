using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleCurrencyRESTApplication
{
    internal class GoogleSheetsAPI
    {
        static string credentialsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json");
        static string sheetId = "/*YOUR_SHEET_ID*/";

        internal static SheetsService AuthenticateViaJson()
        {
            GoogleCredential credential;

            using(var stream = new FileStream(credentialsFilePath,FileMode.Open, FileAccess.Read))
                credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.Spreadsheets);

            var sheetService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                //ApplicationName = ""
            });
            return sheetService;
        }

        internal static void FillCurrencyHeaders(SheetsService sheetservice, List<string> currency)
        {
            var range = $"A1:{(char)('A' + currency.Count - 1)}1"; 

            var values = new List<object>() { "Date:" };

            foreach (var item in currency)
            {
                values.Add(item + " bid");
                values.Add(item + " ask");
            }
            var valueRange = new ValueRange { Values = new List<IList<object>> { values } };

            var request = sheetservice.Spreadsheets.Values.Append(valueRange, sheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            request.Execute();

            Console.WriteLine("Headers appended successfully!");
        }

        internal static void FillCurrencyValues(SheetsService sheetservice, List<CurrencyModel> dailyCurrencyValues)
        {
            var batchRequests = new List<Request>();

            foreach (var day in dailyCurrencyValues)
            {
                var rowData = new RowData();
                rowData.Values = new List<CellData>
                {
                    new CellData { UserEnteredValue = new ExtendedValue { StringValue = day.tradingDate } }
                };

                foreach (var rate in day.rates)
                {
                    rowData.Values.Add(new CellData { UserEnteredValue = new ExtendedValue { NumberValue = rate.bid } });
                    rowData.Values.Add(new CellData { UserEnteredValue = new ExtendedValue { NumberValue = rate.ask} });
                }

                var request = new Request
                {
                    AppendCells = new AppendCellsRequest
                    {
                        SheetId = 0,
                        Rows = new List<RowData> { rowData },
                        Fields = "userEnteredValue"
                    }
                };

                batchRequests.Add(request);

            }
            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = batchRequests
            };

            sheetservice.Spreadsheets.BatchUpdate(batchUpdateRequest, sheetId).Execute();
            Console.WriteLine("Data appended successfully!");
        }
    }
}
