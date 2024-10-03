using ClassLibrary_Shop.Models.Cart_m;
using Library_Shop.Extensions;

namespace Library_Shop.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetCart(Cart cart)
        {
            _httpContextAccessor.HttpContext.Session.Set("cart", cart.CartItems);
        }

        public Cart GetCart()
        {
            var items = _httpContextAccessor.HttpContext.Session.Get<IEnumerable<CartItem>>("cart");
            return items == null ? new Cart(new List<CartItem>()) : new Cart(items);
        }
    }

}
