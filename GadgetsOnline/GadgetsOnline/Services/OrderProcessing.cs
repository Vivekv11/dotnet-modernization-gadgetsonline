using System;
using System.Threading.Tasks;
using GadgetsOnline.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GadgetsOnline.Services
{
    public class OrderProcessing
    {
        private readonly GadgetsOnlineEntities _context;
        private readonly ILogger<OrderProcessing> _logger;
        public OrderProcessing(GadgetsOnlineEntities context, ILogger<OrderProcessing> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ProcessOrderAsync(Order order, HttpContext httpContext)
        {
            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                // Process the order
                var cart = await ShoppingCart.GetCartAsync(httpContext);
                await cart.CreateOrderAsync(order);

                _logger.LogInformation("Order {OrderId} processed successfully", order.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order for user {Username}", order.Username);
                throw;
            }
        }
    }
}