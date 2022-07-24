using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

//[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]
[assembly: FunctionsStartup(typeof(server.Startup))]
namespace server
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IApplicationSettings, ApplicationSettings>();
            
        }
    }
}