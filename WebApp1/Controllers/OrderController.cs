using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApp1.Services;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using WebApp1.Models;
using Microsoft.Extensions.Logging;

namespace OrderProduct.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ApplicationDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("Order/SaveOrderDetails")]
        public async Task<IActionResult> SaveOrderDetails([FromBody] OrderDetailsModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Log the received model data
                    _logger.LogInformation("Received order details: {OrderDetails}", JsonSerializer.Serialize(model));

                    // Create a new order entity
                    var order = new Order
                    {
                        Address = model.Address,
                        Contact = model.Contact,
                        OrderedBy = model.OrderedBy,
                        ProductDetails = model.ProductDetails, // Serialize the list of products
                        TotalAmount = model.TotalAmount
                    };

                    // Log the order entity before saving
                    _logger.LogInformation("Order entity to be saved: {Order}", JsonSerializer.Serialize(order));

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Your order has been placed successfully." });
                }
                catch (Exception ex)
                {
                    // Log the exception details
                    _logger.LogError(ex, "Error saving order details.");
                    return Json(new { success = false, message = "There was an error saving your order. Please try again." });
                }
            }

            // Log the validation errors
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogError("Validation error on {Key}: {ErrorMessage}", state.Key, error.ErrorMessage);
                }
            }

            return Json(new { success = false, message = "Not able to save your order. Please try again." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Order/Details")]
        public async Task<IActionResult> Details()
        {
            var orders = await _context.Orders.ToListAsync();
            return View(orders);
        }
    }

    public class OrderDetailsModel
    {
        public string Address { get; set; }
        public string Contact { get; set; }

        public string OrderedBy { get; set; }
        public string ProductDetails { get; set; } // Serialized JSON string

        public decimal TotalAmount { get; set; }


    }

    public class Order
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }

        public string OrderedBy { get; set; }
        public string ProductDetails { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
