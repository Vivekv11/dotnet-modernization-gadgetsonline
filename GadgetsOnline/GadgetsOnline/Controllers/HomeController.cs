using System;
using System.Threading.Tasks;
using GadgetsOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GadgetsOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly Inventory _inventory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(Inventory inventory, ILogger<HomeController> logger)
        {
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _inventory.GetBestSellers(6);
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving best sellers");
                return View("Error");
            }
        }
        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}