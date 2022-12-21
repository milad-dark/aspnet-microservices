using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!await orderContext.Orders.AnyAsync())
            {
                await orderContext.Orders.AddRangeAsync(GetPreConfiguredOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("data seed section configured");
            }
        }

        public static IEnumerable<Order> GetPreConfiguredOrders()
        {
            return new List<Order>
            {
                new Order
                {
                FirstName = "Milad",
                LastName = "Eghlimapour",
                UserName = "milad",
                EmailAddress = "m@m.com",
                City = "tehran",
                Country = "iran",
                TotalPrice = 10000
                }
            };
        }
    }
}

