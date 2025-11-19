using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeopleDatabase;

namespace Tests.Helpers
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");
            builder.ConfigureServices(services => 
            {
                services.AddDbContext<PeopleDbContext>(options => 
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });
            });
        }
    }
}
