using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

//[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]
[assembly: FunctionsStartup(typeof(howhotistheoffice.Startup))]
namespace howhotistheoffice
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IApplicationSettings, ApplicationSettings>();

        }
    }
}