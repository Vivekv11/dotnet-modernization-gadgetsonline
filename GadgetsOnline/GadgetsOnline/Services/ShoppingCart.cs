using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadgetsOnline.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GadgetsOnline.Services
{
    public class ShoppingCart
    {
        private readonly GadgetsOnlineEntities _context;
        private readonly ILogger<ShoppingCart> _logger;

        private string ShoppingCartId { get; set; }

        public const string CartSessionKey = "CartId";

        public ShoppingCart(GadgetsOnlineEntities context, ILogger<ShoppingCart> logger)
        {
            _context = context;
            _logger = logger;
        }

        public static async Task<ShoppingCart> GetCartAsync(HttpContext context)
        {
            var services = context.RequestServices;
            var dbContext = services.GetRequiredService<GadgetsOnlineEntities>();
            var logger = services.GetRequiredService<ILogger<ShoppingCart>>();

            var cart = new ShoppingCart(dbContext, logger);
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        public async Task<int> CreateOrderAsync(Order order)
        {
            try
            {
                decimal orderTotal = 0;
                var cartItems = await GetCartItemsAsync();

                // Iterate over the items in the cart, adding the order details for each
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderId = order.OrderId,
                        UnitPrice = item.Product.Price,
                        Quantity = item.Count
                    };

                    orderTotal += (item.Count * item.Product.Price);
                    await _context.OrderDetails.AddAsync(orderDetail);
                }

                order.Total = orderTotal;
                await _context.SaveChangesAsync();
                await EmptyCartAsync();

                return order.OrderId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for cart {CartId}", ShoppingCartId);
                throw;
            }
        }

        private async Task EmptyCartAsync()
        {
            var cartItems = await _context.Carts
                .Where(cart => cart.CartId == ShoppingCartId)
                .ToListAsync();

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public string GetCartId(HttpContext context)
        {
            if (context.Session.GetString(CartSessionKey) == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity?.Name))
                {
                    context.Session.SetString(CartSessionKey, context.User.Identity.Name);
                }
                else
                {
                    var tempCartId = Guid.NewGuid().ToString();
                    context.Session.SetString(CartSessionKey, tempCartId);
                }
            }

            return context.Session.GetString(CartSessionKey) ??
                throw new InvalidOperationException("Failed to create or retrieve cart ID");
        }

        public async Task AddToCartAsync(int id)
        {
            try
            {
                var cartItem = await _context.Carts
                    .SingleOrDefaultAsync(c => c.CartId == ShoppingCartId && c.ProductId == id);

                if (cartItem == null)
                {
                    cartItem = new Cart
                    {
                        ProductId = id,
                        CartId = ShoppingCartId,
                        Count = 1,
                        DateCreated = DateTime.UtcNow
                    };

                    await _context.Carts.AddAsync(cartItem);
                }
                else
                {
                    cartItem.Count++;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart {CartId}", id, ShoppingCartId);
                throw;
            }
        }

        public async Task<int> GetCountAsync()
        {
            try
            {
                var count = await _context.Carts
                    .Where(c => c.CartId == ShoppingCartId)
                    .SumAsync(c => (int?)c.Count);

                return count ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting count for cart {CartId}", ShoppingCartId);
                throw;
            }
        }

        public async Task<int> RemoveFromCartAsync(int id)
        {
            try
            {
                // Get the cart item
                var cartItem = await _context.Carts
                    .SingleOrDefaultAsync(cart => cart.CartId == ShoppingCartId
                                             && cart.ProductId == id);

                int itemCount = 0;

                if (cartItem != null)
                {
                    if (cartItem.Count > 1)
                    {
                        cartItem.Count--;
                        itemCount = cartItem.Count;
                    }
                    else
                    {
                        _context.Carts.Remove(cartItem);
                    }

                    await _context.SaveChangesAsync();
                }

                return itemCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product {ProductId} from cart {CartId}",
                    id, ShoppingCartId);
                throw;
            }
        }

        public async Task<List<Cart>> GetCartItemsAsync()
        {
            try
            {
                return await _context.Carts
                    .Where(cart => cart.CartId == ShoppingCartId)
                    .Include(c => c.Product) // Include related Product data
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items for cart {CartId}",
                    ShoppingCartId);
                throw;
            }
        }

        public async Task<decimal> GetTotalAsync()
        {
            try
            {
                var total = await _context.Carts
                    .Where(cart => cart.CartId == ShoppingCartId)
                    .Include(c => c.Product)
                    .Select(cartItem => (decimal)cartItem.Count * cartItem.Product.Price)
                    .SumAsync();

                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total for cart {CartId}",
                    ShoppingCartId);
                throw;
            }
        }
    }
}