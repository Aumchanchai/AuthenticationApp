using Authentication.Controllers;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Authentication.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Authentication.Tests
{
    public static class DependencyResolver
    {
        public static ServiceProvider BuildServiceProvider()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("globalTestSettings.json", optional: false)
            .Build();


            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddScoped<JwtService>();
            services.AddScoped<AuthController>();

            return services.BuildServiceProvider();
        }
    }
}
