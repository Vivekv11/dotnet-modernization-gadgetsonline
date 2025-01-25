using System;
using System.Threading.Tasks;
using GadgetsOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GadgetsOnline.Controllers
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly Inventory _inventory;
        private readonly ILogger<CategoryMenuViewComponent> _logger;

        public CategoryMenuViewComponent(
                Inventory inventory,
                ILogger<CategoryMenuViewComponent> logger)
        {
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var categories = await _inventory.GetAllCategories();

                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category menu");
                return Content("Error loading categories");
            }
        }
    }
}