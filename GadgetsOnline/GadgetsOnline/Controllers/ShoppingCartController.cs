using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GadgetsOnline.Services;
using GadgetsOnline.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GadgetsOnline.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly Inventory _inventory;
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly HtmlEncoder _htmlEncoder;

        public ShoppingCartController(
            Inventory inventory,
            ILogger<ShoppingCartController> logger,
            HtmlEncoder htmlEncoder)
        {
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _htmlEncoder = htmlEncoder ?? throw new ArgumentNullException(nameof(htmlEncoder));
        }

        // GET: ShoppingCart
        public async Task<IActionResult> Index()
        {
            try
            {
                var cart = await ShoppingCart.GetCartAsync(HttpContext);

                var viewModel = new ShoppingCartViewModel
                {
                    CartItems = await cart.GetCartItemsAsync(),
                    CartTotal = await cart.GetTotalAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving shopping cart");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(int id)
        {
            try
            {
                var cart = await ShoppingCart.GetCartAsync(HttpContext);

                await cart.AddToCartAsync(id);

                _logger.LogInformation("Added product {ProductId} to cart", id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            try
            {
                var cart = await ShoppingCart.GetCartAsync(HttpContext);
                int itemCount = await cart.RemoveFromCartAsync(id);

                var productName = await _inventory.GetProductNameById(id);

                var results = new ShoppingCartRemoveViewModel()
                {
                    Message = $"{_htmlEncoder.Encode(productName)} has been removed from your shopping cart.",
                    CartTotal = await cart.GetTotalAsync(),
                    CartCount = await cart.GetCountAsync(),
                    ItemCount = itemCount,
                    DeleteId = id
                };

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product {ProductId} from cart", id);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}