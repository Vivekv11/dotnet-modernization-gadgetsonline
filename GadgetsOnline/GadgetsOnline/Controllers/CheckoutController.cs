using System;
using System.Threading.Tasks;
using GadgetsOnline.Models;
using GadgetsOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GadgetsOnline.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly OrderProcessing _orderProcessing;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(OrderProcessing orderProcessing, ILogger<CheckoutController> logger)
        {
            _orderProcessing = orderProcessing ?? throw new ArgumentNullException(nameof(orderProcessing));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Checkout
        public IActionResult Index()
        {
            return View();
        }

        // GET: Checkout/AddressAndPayment
        public IActionResult AddressAndPayment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressAndPayment([FromForm] Order order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(order);
                }

                order.Username = User.Identity?.IsAuthenticated == true
                    ? User.Identity.Name
                    : "Anonymous";
                order.OrderDate = DateTime.UtcNow;

                var result = await _orderProcessing.ProcessOrderAsync(order, HttpContext);

                if (result)
                {
                    return RedirectToAction(nameof(Complete), new { id = order.OrderId });
                }

                ModelState.AddModelError("", "Unable to process your order. Please try again.");
                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order for user {Username}", order.Username);
                ModelState.AddModelError("", "An error occurred while processing your order.");
                return View(order);
            }
        }

        // GET: Checkout/Complete/5
        public Task<IActionResult> Complete(int id)
        {
            try
            {
                return Task.FromResult<IActionResult>(View(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return Task.FromResult<IActionResult>(RedirectToAction("Error", "Home"));
            }
        }
    }
}