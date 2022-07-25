using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Data;

namespace server.Domain.ChartDataDomain
{

    public interface IGetChartDataService
    {
        ChartDataModel[] GetChartDataByDate(string date);
    }

    public class GetChartDataService : IGetChartDataService
    {

        private readonly ITemperatureLogRepository _temperatureLogRepository;
        private readonly IChartDataModelMapper _chartDataModelMapper;

        public GetChartDataService(ITemperatureLogRepository temperatureLogRepository,  IChartDataModelMapper chartDataModelMapper) 
        {
            _temperatureLogRepository = temperatureLogRepository;
            _chartDataModelMapper = chartDataModelMapper;
        }

        public ChartDataModel[] GetChartDataByDate(string date)
        {
            var temperatureLogs = _temperatureLogRepository.GetByDate(date);
            var chartData = _chartDataModelMapper.CreateModel(temperatureLogs);
            return chartData;
        }
    }
}