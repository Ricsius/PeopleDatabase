using ServiceContracts;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace PeopleDatabase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPeopleService, PeopleService>();
            builder.Services.AddDbContext<PeopleDbContext>(options => 
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            var app = builder.Build();

            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", "Rotativa");

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}
