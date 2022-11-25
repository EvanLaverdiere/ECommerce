using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using ECommerce.Api.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly OrderDbContext dbContext;
        private readonly ILogger<OrdersProvider> logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrderDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            
            if (!dbContext.Orders.Any())
            {
                Db.OrderItem orderItem1 = new Db.OrderItem() { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 20 };
                Db.OrderItem orderItem2 = new Db.OrderItem() { Id = 2, OrderId = 1, ProductId = 2, Quantity = 1, UnitPrice = 20 };
                Db.OrderItem orderItem3 = new Db.OrderItem() { Id = 3, OrderId = 2, ProductId = 2, Quantity = 2, UnitPrice = 20 };
                Db.OrderItem orderItem4 = new Db.OrderItem() { Id = 4, OrderId = 3, ProductId = 3, Quantity = 1, UnitPrice = 20 };
                Db.OrderItem orderItem5 = new Db.OrderItem() { Id = 5, OrderId = 3, ProductId = 4, Quantity = 1, UnitPrice = 200 };
                Db.OrderItem orderItem6 = new Db.OrderItem() { Id = 6, OrderId = 4, ProductId = 3, Quantity = 1, UnitPrice = 20 };

                dbContext.OrderItems.Add(orderItem1);
                dbContext.OrderItems.Add(orderItem2);
                dbContext.OrderItems.Add(orderItem3);
                dbContext.OrderItems.Add(orderItem4);
                dbContext.OrderItems.Add(orderItem5);
                dbContext.OrderItems.Add(orderItem6);
                
                dbContext.Orders.Add(new Db.Order() { Id = 1, CustomerId = 1, OrderDate = DateTime.Now, Total = 40, Items = new List<Db.OrderItem>
                {
                    orderItem1,
                    orderItem2
                }
                });
                dbContext.Orders.Add(new Db.Order() { Id = 2, CustomerId = 2, OrderDate = DateTime.Now, Total = 40, Items = new List<Db.OrderItem>
                {
                    orderItem3
                }
                });
                dbContext.Orders.Add(new Db.Order() { Id = 3, CustomerId = 3, OrderDate = DateTime.Now, Total = 220, Items = new List<Db.OrderItem>
                {
                    orderItem4,
                    orderItem5
                }
                });
                dbContext.Orders.Add(new Db.Order() { Id = 4, CustomerId = 4, OrderDate = DateTime.Now, Total = 20, Items = new List<Db.OrderItem>
                {
                    orderItem6
                }
                });
                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync()
        {
            try
            {
                // Ensure that the list of Orders includes all child OrderItems.
                // Discovered the solution to this here: https://stackoverflow.com/questions/27176014/how-to-add-update-child-entities-when-updating-a-parent-entity-in-ef
                var orders = await dbContext.Orders.Include(o => o.Items).ToListAsync();
                if(orders != null && orders.Any())
                {
                    var result = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                    return (true, result, null);
                }
                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                //var orders = await dbContext.Orders.FirstOrDefaultAsync(o => o.CustomerId == customerId);
                // Found a way to implement this from here: https://stackoverflow.com/questions/26643752/c-sharp-async-when-returning-linq
                var orders = await dbContext.Orders.Where(o => o.CustomerId == customerId).Include(o => o.Items).ToListAsync();
                if (orders != null & orders.Any())
                {
                    var result = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                    return (true, result, null);
                }
                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }

        }
    }
}
