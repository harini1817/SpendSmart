using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CartItemService.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddToCart(int productId, string productName, decimal productPrice)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cart = GetCartItems();

            var existingItem = cart.FirstOrDefault(i => i.Id == productId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    Id = productId,
                    Name = productName,
                    Price = productPrice,
                    Quantity = 1
                });
            }

            SaveCartItems(cart);
        }

        public List<CartItem> GetCartItems()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString("Cart");
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }
        public int GetCartItemCount()
        {
            var cart = GetCartItems();
            return cart.Count();
        }
       

        private void SaveCartItems(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString("Cart", cartJson);
        }

        public string CheckOut()
        {
            var cart = GetCartItems();


            // ClearCart();

            return "Order placed successfully!";
        }

        public void ClearCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Remove("Cart");
            GetCartItems();
        }

        public decimal GetTotalCost()
        {
            var cart = GetCartItems();
            return cart.Sum(i => i.Price * i.Quantity);
        }

        public CartItem GetCartItemById(int productId)
        {
            var cart = GetCartItems();
            return cart.FirstOrDefault(i => i.Id == productId);
        }


    }

    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
