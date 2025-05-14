using App.DAL.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tests.WebApp
{
    public class CustomWebAppFactory<TStartup> : WebApplicationFactory<TStartup> 
        where TStartup : class
    {
        private static bool dbInitialized = false;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing AppDbContext registrations
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add InMemoryDb
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDbForEventRegistration");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<CustomWebAppFactory<TStartup>>>();

                try
                {
                    db.Database.EnsureCreated();

                    if (!dbInitialized)
                    {
                        dbInitialized = true;

                        // --- Siin saad soovi korral seedida näiteks Test Event'id ---
                        db.Events.Add(new App.Domain.Event
                        {
                            Id = Guid.NewGuid(),
                            Name = "Seeded Event",
                            DateTime = DateTime.UtcNow.AddDays(10),
                            Location = "Test Location",
                            AdditionalInfo = "Seeded Info"
                        });
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
                }
            });
        }
    }
}
