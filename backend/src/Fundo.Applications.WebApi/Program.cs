using Fundo.DAL;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Fundo.Applications.WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = CreateWebHostBuilder(args).Build();

                // Seed only on Development env
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var env = services.GetRequiredService<IWebHostEnvironment>();

                    if (env.IsDevelopment())
                    {
                        try
                        {
                            var context = services.GetRequiredService<FundoDbContext>();
                            DbInitializer.SeedAsync(context).Wait();
                            Console.WriteLine("Database seeded successfully.");
                        }
                        catch (Exception seedEx)
                        {
                            Console.WriteLine($"An error occurred seeding the database: {seedEx.Message}");
                        }
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled WebApi exception: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Application shutting down.");
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}
