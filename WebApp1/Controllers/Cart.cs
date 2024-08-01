using CartItemService.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApp1.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult CartPartial()
        {
            var cartItems = _cartService.GetCartItems(); 
            return PartialView("CartPartial", cartItems); 
        }

        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            var count = _cartService.GetCartItemCount(); 
            return Json(count);
        }

        [HttpPost]
        public IActionResult CheckOut()
        {
           
            var message = _cartService.CheckOut();

            
            return Json(new { success = true, message });
        }
        [HttpGet]
        public IActionResult GetProductDetails()
        {
            var cartItems = _cartService.GetCartItems();
            var ProductDetails = cartItems.Select(item => new
            {
                productId = item.Id,
                productName = item.Name,
                price = item.Price,
                quantity = item.Quantity
            }).ToList();

            return Json(new { items = ProductDetails });
        }
        [HttpGet]
        public IActionResult GetTotalCost()
        { 
            var totalCost = _cartService.GetTotalCost();
            return Json(new { success = true, totalCost });

        }

            [HttpPost]
        public IActionResult ClearCart()
        {
            // Call your CartService to clear the cart
            _cartService.ClearCart();

            // Return a success response
            return Json(new { success = true });
        }


        public IActionResult Cart()
        {
            // Assuming _cartService.GetCartItems() returns a list of cart items
            var cartItems = _cartService.GetCartItems();
            decimal totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

            // Pass the cart items and total amount to the view
            ViewBag.TotalAmount = totalAmount;
            return View(cartItems);
        }

    }
}
