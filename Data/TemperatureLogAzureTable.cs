using System;
using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Data.Tables;
using server.Data.Models;

namespace server.Data
{

    public interface ITemperatureLogRepository
    {
        TemperatureRecord GetLatest();
        List<TemperatureRecord> GetByDate(string date);
        void InsertTemperatureRecord(TemperatureRecord temperatureRecord);
        TemperatureRecord GetTemperatureRecord(string partitionKey, string rowkey);
        void UpdateTemperatureRecord(TemperatureRecord temperatureRecord);
    }

    public class TemperatureLogAzureTable : ITemperatureLogRepository
    {

        private readonly string TABLE_NAME = "temperaturelog";
        private readonly IApplicationSettings _applicationSettings;

        public TemperatureLogAzureTable(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }


        public List<TemperatureRecord> GetByDate(string date)
        {
            var tableClient = GetTableClient();
            var queryResult = tableClient.Query<TemperatureRecord>(filter: $"PartitionKey eq '{date}'").ToList();
            return queryResult;
        }

        public TemperatureRecord GetLatest()
        {
            var tableClient = GetTableClient();
            var queryResult = tableClient.Query<TemperatureRecord>(filter: $"PartitionKey eq '{_applicationSettings.CURRENT_LOG_PARTITION}' and RowKey eq '{_applicationSettings.CURRENT_LOG_ROWKEY}'").Single();
            return queryResult;
        }

        public TemperatureRecord GetTemperatureRecord(string partitionKey, string rowkey)
        {
            var tableClient = GetTableClient();
            var queryResult = tableClient.Query<TemperatureRecord>(filter: $"PartitionKey eq '{partitionKey}' and RowKey eq '{rowkey}'").Single();
            return queryResult;
        }

        public void UpdateTemperatureRecord(TemperatureRecord temperatureRecord)
        {
            var tableClient = GetTableClient();
            tableClient.UpdateEntity(temperatureRecord, ETag.All, TableUpdateMode.Merge);
        }

        public void InsertTemperatureRecord(TemperatureRecord temperatureRecord)
        {
            temperatureRecord.Timestamp = DateTimeOffset.Now;
            temperatureRecord.ETag = ETag.All;
            var tableClient = GetTableClient();
            tableClient.AddEntity(temperatureRecord);
        }

        private TableClient _tableClient = null;
        private TableClient GetTableClient(){
            if(_tableClient != null) return _tableClient;
            return new TableClient(new Uri(_applicationSettings.TABLE_URL(TABLE_NAME)), TABLE_NAME, new TableSharedKeyCredential(_applicationSettings.STORAGE_ACCOUNT_NAME, _applicationSettings.STORAGE_ACCOUNT_KEY));
        }
    }
}