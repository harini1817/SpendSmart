using CartItemService.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApp1.Controllers
{
    public class Cart : Controller
    {
        private readonly CartService _cartService;

        public Cart(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult CartPartial()
        {
            var cartItems = _cartService.GetCartItems(); // Fetch cart items from service
            return PartialView("CartPartial", cartItems); // Ensure the view name matches
        }

        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            var count = _cartService.GetCartItemCount(); // Fetch cart item count from service
            return Json(count);
        }
    }
}
