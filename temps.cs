using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;
using System.Linq;

namespace server
{
    public class temps
    {
        private readonly string TABLE_URL = "https://fntemps.table.core.windows.net/temp";
        private readonly string TABLE_NAME = "temp";
        private readonly string TABLE_ACCOUNT_NAME = "fntemps";
        private readonly string TABLE_ACCOUNT_KEY = "crtflSMKFnKWIhJglDTq3e/HUCvezkUFD/zyYiAwoNY5PH8XgsF0c9Wvogfec1c55NXMLmi2HCPo+AStMHQyHg==";
        private readonly string LATEST_PARTITION = "latest";
        private readonly string LATEST_ROWKEY = "now";
        //partition by: Date
        // row key: timestamp

        //partiion: latest:
        //row key: NOW (current displayed value)

        //when we get an update shit from now to the logs, update now. 

        [FunctionName("temps")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // string name = req.Query["name"];

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // name = name ?? data?.name;

            // string responseMessage = string.IsNullOrEmpty(name)
            //     ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //     : $"Hello, {name}. This HTTP triggered function executed successfully.";

            var current = GetLatest();

            return new OkObjectResult(current);
        }

        [FunctionName("CreateLatest")]
        public async Task<IActionResult> CreateLatest(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var tempRecord = JsonConvert.DeserializeObject<TemperatureRecord>(requestBody);

            log.LogInformation(requestBody);

            tempRecord.PartitionKey = LATEST_PARTITION;
            tempRecord.RowKey = LATEST_ROWKEY;
            tempRecord.Timestamp = DateTimeOffset.Now;
            tempRecord.ETag = ETag.All;
            var tableClient = new TableClient(new Uri(TABLE_URL), TABLE_NAME, new TableSharedKeyCredential(TABLE_ACCOUNT_NAME, TABLE_ACCOUNT_KEY));
            tableClient.AddEntity(tempRecord);

            log.LogInformation($"Temp: {tempRecord.Temperature} Time: {tempRecord.TemperatureTimeStamp}");

            return new OkObjectResult("");
        }

        [FunctionName("NewTemp")]
        public async Task<IActionResult> CreateNewTempRecord(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var tempRecord = JsonConvert.DeserializeObject<TemperatureRecord>(requestBody);

            log.LogInformation(requestBody);

            var current = GetLatest();
            InsertLog(current);
            UpdateLatest(tempRecord);

            //insert into table storage.

            log.LogInformation($"Temp: {tempRecord.Temperature} Time: {tempRecord.TemperatureTimeStamp}");

            return new OkObjectResult("");
        }

        private class ChartData {
            public string Time { get; set; }
            public double Temperature { get; set; }
        }

        [FunctionName("TempsForDay")]
        public async Task<IActionResult> GetTempsForDay( [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
            {
                string date = req.Query["date"];

                var tableClient = new TableClient(new Uri(TABLE_URL), TABLE_NAME, new TableSharedKeyCredential(TABLE_ACCOUNT_NAME, TABLE_ACCOUNT_KEY));
                var queryResult = tableClient.Query<TemperatureRecord>(filter: $"PartitionKey eq '{date}'").ToList();

                //var chartData = queryResult.Select(x => new ChartData() { Time = x.TemperatureTimeStamp, Temperature = x.Temperature}).OrderBy(x => x.Time);

                var chartData = queryResult.GroupBy(x => x.TemperatureTimeStamp.Hour)
                .Select(z => new ChartData() { Temperature = Math.Round(z.Average(s => s.Temperature), 1), Time = $"{z.First().TemperatureTimeStamp.Hour}:00" })
                .ToList();

                return new OkObjectResult(chartData);
            }

        private void UpdateLatest(TemperatureRecord temperatureReading)
        {
            var tableClient = new TableClient(new Uri(TABLE_URL), TABLE_NAME, new TableSharedKeyCredential(TABLE_ACCOUNT_NAME, TABLE_ACCOUNT_KEY));
            TemperatureRecord currentRecord = tableClient.GetEntity<TemperatureRecord>(LATEST_PARTITION, LATEST_ROWKEY);
            currentRecord.Temperature = temperatureReading.Temperature;
            currentRecord.TemperatureTimeStamp = temperatureReading.TemperatureTimeStamp;
            currentRecord.Timestamp = DateTimeOffset.Now;
            tableClient.UpdateEntity(currentRecord, ETag.All, TableUpdateMode.Merge);
        }

        private void InsertLog(TemperatureRecord temperatureReading)
        {
            string partition = temperatureReading.TemperatureTimeStamp.ToString("yyyy-MM-dd");
            string rowKey = temperatureReading.TemperatureTimeStamp.Ticks.ToString();
            InsertToTable(partition, rowKey, temperatureReading);
        }

        private void InsertToTable<T>(string partitionkey, string rowKey, T data) where T : ITableEntity
        {
            data.PartitionKey = partitionkey;
            data.RowKey = rowKey;
            data.Timestamp = DateTimeOffset.Now;
            data.ETag = ETag.All;
            var tableClient = new TableClient(new Uri(TABLE_URL), TABLE_NAME, new TableSharedKeyCredential(TABLE_ACCOUNT_NAME, TABLE_ACCOUNT_KEY));
            tableClient.AddEntity(data);
        }

        private TemperatureRecord GetLatest()
        {
            var tableClient = new TableClient(new Uri(TABLE_URL), TABLE_NAME, new TableSharedKeyCredential(TABLE_ACCOUNT_NAME, TABLE_ACCOUNT_KEY));
            var queryResult = tableClient.Query<TemperatureRecord>(filter: $"PartitionKey eq '{LATEST_PARTITION}' and RowKey eq '{LATEST_ROWKEY}'").Single();
            return queryResult;
        }

        private class TemperatureRecord : ITableEntity
        {

            public double Temperature { get; set; }
            public DateTimeOffset TemperatureTimeStamp { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public ETag ETag { get; set; }

            public TemperatureRecord()
            {
                Timestamp = DateTimeOffset.Now;
            }

            public TemperatureRecord(double Temperature, DateTime TemperatureTimeStamp)
            {
                this.Timestamp = DateTimeOffset.Now;
                this.Temperature = Temperature;
                this.TemperatureTimeStamp = TemperatureTimeStamp;
            }
        }
    }
}
