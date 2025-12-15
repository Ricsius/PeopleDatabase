using Entities;
using Microsoft.EntityFrameworkCore;
using PeopleDatabase.Filters.ActionFilters;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace PeopleDatabase.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddTransient<ResponseHeaderActionFilter>();
            services.AddControllersWithViews(options =>
            {
                var factory = new ResponseHeaderActionFilterFactory("MyKey_FromGlobal", "MyValue_FromGlobal", 2);
                options.Filters.Add(factory.CreateInstance(services.BuildServiceProvider()));
            });
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPeopleRepository, PeopleRepository>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPeopleService, PeopleService>();
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
                | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            if (!environment.IsEnvironment("Test"))
            {
                services.AddDbContext<PeopleDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("Default"));
                });
            }
        }
    }
}
