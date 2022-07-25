using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace server.Data.Models
{
    public class TemperatureRecord : ITableEntity
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