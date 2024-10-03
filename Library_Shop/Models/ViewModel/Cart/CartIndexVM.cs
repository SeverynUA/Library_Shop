using ClassLibrary_Shop.Models.Cart_m;

namespace Library_Shop.Models.ViewModel.Cart
{
    public class CartIndexVM
    {

        public IEnumerable<CartItem> CartItems { get; set; } = default!;
        public decimal TotalPrice { get; set; }
        public string? ReturnUrl { get; set; } = default!;
    }
}
