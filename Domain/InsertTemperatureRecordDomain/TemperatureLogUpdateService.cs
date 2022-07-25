using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Data;
using server.Data.Models;

namespace server.Domain.InsertTemperatureRecordDomain
{

    public interface ITemperatureLogUpdateService
    {
        void UpdateLatest(TemperatureRecord temperatureRecord);
        void InsertTemperatureRecord(TemperatureRecord temperatureRecord);
    }

    public class TemperatureLogUpdateService : ITemperatureLogUpdateService
    {

        private readonly ITemperatureLogRepository _temperatureLogRepository;
        private readonly IApplicationSettings _applicationSettings;

        public TemperatureLogUpdateService(ITemperatureLogRepository temperatureLogRepository,  IApplicationSettings applicationSettings)
        {
            _temperatureLogRepository = temperatureLogRepository;
            _applicationSettings = applicationSettings;
        }

        public void InsertTemperatureRecord(TemperatureRecord temperatureRecord)
        {
            string partition = temperatureRecord.TemperatureTimeStamp.ToString("yyyy-MM-dd");
            string rowKey = temperatureRecord.TemperatureTimeStamp.Ticks.ToString();
            temperatureRecord.PartitionKey = partition;
            temperatureRecord.RowKey = rowKey;
            _temperatureLogRepository.InsertTemperatureRecord(temperatureRecord);
        }

        public void UpdateLatest(TemperatureRecord temperatureRecord)
        {
            var latest = _temperatureLogRepository.GetTemperatureRecord(_applicationSettings.CURRENT_LOG_PARTITION, _applicationSettings.CURRENT_LOG_ROWKEY);
            latest.Temperature = temperatureRecord.Temperature;
            latest.TemperatureTimeStamp = temperatureRecord.TemperatureTimeStamp;
            latest.Timestamp = DateTimeOffset.Now;
            _temperatureLogRepository.UpdateTemperatureRecord(latest);
        }
    }
}