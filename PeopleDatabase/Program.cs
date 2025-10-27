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
            builder.Services.AddSingleton<ICountriesService, CountriesService>();
            builder.Services.AddSingleton<IPeopleService, PeopleService>();
            builder.Services.AddDbContext<PeopleDbContext>(options => 
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            var app = builder.Build();

            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}
