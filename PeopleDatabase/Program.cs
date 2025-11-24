using ServiceContracts;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using Repositories;

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

            builder.Services.AddControllersWithViews();
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
