using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Data.Models;

namespace server.Domain.ChartDataDomain
{

    public interface IChartDataModelMapper
    {
        ChartDataModel[] CreateModel(List<TemperatureRecord> temperatureRecords);
    }

    public class ChartDataModelMapper : IChartDataModelMapper
    {
        public ChartDataModel[] CreateModel(List<TemperatureRecord> temperatureRecords)
        {
             var chartData = temperatureRecords.GroupBy(x => x.TemperatureTimeStamp.Hour)
                .Select(z => new ChartDataModel() { Temperature = Math.Round(z.Average(s => s.Temperature), 1), Time = $"{z.First().TemperatureTimeStamp.Hour}:00" })
                .ToArray();
            return chartData;
        }
    }
}