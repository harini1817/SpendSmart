using Microsoft.AspNetCore.Mvc;
using Cart.Models;
using CartItemService.Services;

namespace YourNamespace.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly CartService _cartService;

        public CartViewComponent(CartService cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke()
        {
            var cartItems = _cartService.GetCartItems();
            return View("Default", cartItems);
        }
    }
}
