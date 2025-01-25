using System;
using System.Threading.Tasks;
using GadgetsOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GadgetsOnline.Controllers
{
    public class StoreController : Controller
    {
        private readonly Inventory _inventory;
        private readonly ILogger<StoreController> _logger;

        public StoreController(Inventory inventory, ILogger<StoreController> logger)
        {
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Store
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Browse(string category)
        {
            try
            {
                var products = await _inventory.GetAllProductsInCategory(category);

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error browsing category {Category}", category);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _inventory.GetProductById(id);

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product {ProductId}", id);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}