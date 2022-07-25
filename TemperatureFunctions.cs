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
using server.Data.Models;
using server.Data;
using server.Domain.ChartDataDomain;
using server.Domain.InsertTemperatureRecordDomain;

namespace server
{
    public class TemperatureFunctions
    {
        private const string TABLE_NAME = "temperaturelog";
        private readonly string LATEST_PARTITION = "latest";
        private readonly string LATEST_ROWKEY = "now";
        private readonly IApplicationSettings _appSettings;
        private readonly ITemperatureLogRepository _temperatureLogRepository;
        private readonly IGetChartDataService _getChartDataService;
        private readonly ITemperatureLogUpdateService _temperatureLogUpdateService;
        public TemperatureFunctions(IApplicationSettings appSettings, ITemperatureLogRepository temperatureLogRepository, IGetChartDataService getChartDataService, ITemperatureLogUpdateService temperatureLogUpdateService)
        {
            _appSettings = appSettings;
            _temperatureLogRepository = temperatureLogRepository;
            _getChartDataService = getChartDataService;
            _temperatureLogUpdateService = temperatureLogUpdateService;
        }

        [FunctionName("temps")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            var current = _temperatureLogRepository.GetTemperatureRecord(_appSettings.CURRENT_LOG_PARTITION, _appSettings.CURRENT_LOG_ROWKEY);
            return new OkObjectResult(current);
        }

        [FunctionName("NewTemp")]
        public async Task<IActionResult> CreateNewTempRecord(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var tempRecord = JsonConvert.DeserializeObject<TemperatureRecord>(requestBody);

            _temperatureLogUpdateService.UpdateLatest(tempRecord);
            _temperatureLogUpdateService.InsertTemperatureRecord(tempRecord);

            return new OkObjectResult("");
        }



        [FunctionName("TempsForDay")]
        public IActionResult GetTempsForDay([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string date = req.Query["date"];
            var chartData = _getChartDataService.GetChartDataByDate(date);
            return new OkObjectResult(chartData);
        }


    }
}
