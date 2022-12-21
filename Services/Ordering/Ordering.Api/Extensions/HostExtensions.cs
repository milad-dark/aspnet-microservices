using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ordering.Api.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host,
            Action<TContext,IServiceProvider> seeder,
            int? retry = 0) where TContext : DbContext
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating started for sql server");
                    InvokeSeeder(seeder, context, services);
                    logger.LogInformation("Migrating has been done for sql server");
                }
                catch (SqlException ex)
                {
                    logger.LogError($"Error migrating sql server. {ex}");

                    if(retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, seeder, retryForAvailability);
                    }

                    throw;
                }
            }
            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder,
            TContext context, IServiceProvider services) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }


}
