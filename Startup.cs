using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using server.Data;
using server.Domain.ChartDataDomain;
using server.Domain.InsertTemperatureRecordDomain;

//[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]
[assembly: FunctionsStartup(typeof(server.Startup))]
namespace server
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IApplicationSettings, ApplicationSettings>();
            builder.Services.AddTransient<ITemperatureLogRepository, TemperatureLogAzureTable>();
            builder.Services.AddTransient<IGetChartDataService, GetChartDataService>();
            builder.Services.AddTransient<IChartDataModelMapper, ChartDataModelMapper>();
            builder.Services.AddTransient<ITemperatureLogUpdateService, TemperatureLogUpdateService>();
        }
    }
}