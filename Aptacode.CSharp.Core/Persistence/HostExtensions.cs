using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aptacode.CSharp.Core.Persistence
{
    public static class HostExtensions
    {
        public static IHost MigrateRelationalDb<TDbContext>(this IHost webHost) where TDbContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<TDbContext>();
                    db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger>();
                    logger.LogError(ex, "Database Creation/Migrations failed!");
                }
            }

            return webHost;
        }
    }
}