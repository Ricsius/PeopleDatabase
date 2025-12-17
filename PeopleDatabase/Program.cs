using PeopleDatabase.Middlewares;
using PeopleDatabase.StartupExtensions;
using Serilog;

namespace PeopleDatabase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /*
            //Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddEventLog();
            */

            //Serilog
            builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider serviceProvider, LoggerConfiguration configuration) => 
            {
                configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(serviceProvider);
            });
            builder.Services.ConfigureServices(builder.Configuration, builder.Environment);
            
            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseHttpLogging();

            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else 
            {
                app.UseExceptionHandlerMiddleware();
            }

            if (!builder.Environment.IsEnvironment("Test"))
            {
                Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", "Rotativa");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}
