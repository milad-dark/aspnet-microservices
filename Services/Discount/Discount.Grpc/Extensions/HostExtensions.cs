using Npgsql;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrationDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                // migration databse
                try
                {
                    logger.LogInformation("migration postgresql databse");

                    using var connection = new NpgsqlConnection
                        (configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
                                                                ProductName VARCHAR(200) NOT NULL,
                                                                Description Text,
                                                                Amount INT)";
                    command.ExecuteNonQuery();

                    // seed data
                    command.CommandText = @"INSERT INTO Coupon(ProductName,Description,Amount)
                                                VALUES('IPhone X','IPhone x description', 100)";
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO Coupon(ProductName,Description,Amount)
                                                VALUES('Samsung 10','Samsung description', 200)";
                    command.ExecuteNonQuery();

                    logger.LogInformation("migration has be completed");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError("an error has been occured");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrationDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }
            return host;
        }
    }
}
