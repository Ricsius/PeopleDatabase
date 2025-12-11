using ServiceContracts;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using Repositories;
using Serilog;
using PeopleDatabase.Filters.ActionFilters;

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
            builder.Services.AddTransient<ResponseHeaderActionFilter>();
            builder.Services.AddControllersWithViews(options => 
            {
                var factory = new ResponseHeaderActionFilterFactory("MyKey_FromGlobal", "MyValue_FromGlobal", 2);
                options.Filters.Add(factory.CreateInstance(builder.Services.BuildServiceProvider()));
            });
            builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
            builder.Services.AddScoped<IPeopleRepository, PeopleRepository>();
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPeopleService, PeopleService>();
            builder.Services.AddHttpLogging(options => 
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
                | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            if (!builder.Environment.IsEnvironment("Test"))
            {
                builder.Services.AddDbContext<PeopleDbContext>(options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
                });
            }
            
            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseHttpLogging();

            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
