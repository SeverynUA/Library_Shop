using ClassLibrary_Shop.Models.Book_m;
using ClassLibrary_Shop.Models.Cart_m;
using Library_Shop.Data.Entities;
using Library_Shop.Extensions;
using Library_Shop.Models.DTOs.CarItemDTO;
using Library_Shop.Models.ViewModel.Cart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly LibraryDbContext context;

        public CartController(LibraryDbContext context)
        {
            this.context = context;
        }

        private CartDTO ToCartDTO(Cart cart)
        {
            return new CartDTO
            {
                Items = cart.CartItems.Select(item => new CartItemDTO
                {
                    BookId = item.Book.Id,
                    Title = item.Book.Title,
                    Price = item.Book.Price,
                    Quantity = item.Count
                }).ToList()
            };
        }

        private Cart ToCart(CartDTO cartDTO)
        {
            var cartItems = new List<CartItem>();

            foreach (var itemDTO in cartDTO.Items)
            {
                var book = context.Books.Find(itemDTO.BookId);
                if (book != null)
                {
                    cartItems.Add(new CartItem
                    {
                        Book = book,
                        Count = itemDTO.Quantity
                    });
                }
            }

            return new Cart(cartItems);
        }

        public IActionResult Index(string? returnUrl)
        {
            Cart cart = GetCart();
            CartIndexVM cartIndexVM = new CartIndexVM()
            {
                CartItems = cart.CartItems,
                TotalPrice = cart.GetTotalPrice(),
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home")
            };
            return View(cartIndexVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, string returnUrl)
        {
            Cart cart = GetCart();
            Book? book = await context.Books.Include(b => b.Image).FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            cart.AddToCart(new CartItem { Book = book, Count = 1 });
            SetCart(cart);

            return Redirect(returnUrl);
        }

        [HttpPost]
        public IActionResult IncCount(int id)
        {
            Cart cart = GetCart();
            CartItem? cartItem = cart.CartItems.FirstOrDefault(t => t.Book.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }
            cart.IncCount(cartItem.Book!.Id);
            SetCart(cart);
            return Ok(new { cartItem.Count, cartItem.TotalPrice });
        }

        [HttpPost]
        public IActionResult DecCount(int id)
        {
            Cart cart = GetCart();
            CartItem? cartItem = cart.CartItems.FirstOrDefault(t => t.Book.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }
            cart.DecCount(cartItem.Book!.Id);
            SetCart(cart);
            return Ok(new { cartItem.Count, cartItem.TotalPrice });
        }

        [HttpPost]
        public IActionResult GetTotalPrice()
        {
            Cart cart = GetCart();
            return Ok(new { TotalPrice = cart.GetTotalPrice() });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int? id, string? returnUrl)
        {
            Cart cart = GetCart();
            if (id == null)
            {
                return NotFound();
            }
            cart.RemoveFromCart(id.Value);
            SetCart(cart);
            return RedirectToAction("Index", new { returnUrl });
        }

        public IActionResult Checkout()
        {
            Cart cart = GetCart();

            if (cart.CartItems.Count() == 0)
            {
                ModelState.AddModelError("", "Ваш кошик порожній!");
                return RedirectToAction("Index");
            }

            return RedirectToAction("Checkout", "Order");
        }

        public Cart GetCart()
        {
            var cartDTO = HttpContext.Session.Get<CartDTO>("cart");
            if (cartDTO == null)
            {
                cartDTO = new CartDTO();
                HttpContext.Session.Set("cart", cartDTO);
            }
            return ToCart(cartDTO);
        }

        public void SetCart(Cart cart)
        {
            var cartDTO = ToCartDTO(cart);
            HttpContext.Session.Set("cart", cartDTO);
        }

    }


}
