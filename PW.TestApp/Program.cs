using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PW.TestApp.Middlewares;
using PW.TestApp.Services;

namespace PW.TestApp
{
    public static class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(builder => 
                {
                    //Add middleware
                    builder.UseMiddleware<FunctionMiddleware>();
                })
                .ConfigureServices(services => 
                {
                    //Dependency injection
                    services.AddLogging();

                    services.AddScoped<IPetService, PetService>();
                })
                .Build();

            host.Run();
        }
    }
}