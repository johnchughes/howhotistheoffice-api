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
            var data = new List<ChartDataModel>();
            for(int i = 0; i < 24; i++) {
                string hour = $"{i}:00";
                var cdm = new ChartDataModel() {
                    Time = hour
                };
                data.Add(cdm);
            }

             var chartData = temperatureRecords.GroupBy(x => x.TemperatureTimeStamp.Hour)
                .Select(z => new ChartDataModel() { Temperature = Math.Round(z.Average(s => s.Temperature), 1), Time = $"{z.First().TemperatureTimeStamp.Hour}:00" })
                .ToArray();

            foreach(var cdm in data) {
                var item = chartData.Where(x => x.Time == cdm.Time).FirstOrDefault();
                cdm.Temperature = item == null ? 0 : item.Temperature;
            }

            return data.ToArray();
        }
    }
}