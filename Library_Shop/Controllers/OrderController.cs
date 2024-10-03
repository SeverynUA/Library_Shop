using ClassLibrary_Shop.Models.Cart_m;
using ClassLibrary_Shop.Models.Order_m;
using Library_Shop.Data.Entities;
using Library_Shop.Extensions;
using Library_Shop.Models.ViewModel.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library_Shop.Services;

namespace Library_Shop.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderDbContext _context;

        public OrderController(OrderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View(new CustomerInfoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CustomerInfoViewModel customerInfo)
        {
            if (!ModelState.IsValid)
            {
                return View(customerInfo);
            }

            // Отримуємо кошик з сесії
            var cart = HttpContext.Session.Get<Cart>("cart");

            if (cart == null || !cart.CartItems.Any())
            {
                ModelState.AddModelError("", "Ваш кошик порожній!");
                return View(customerInfo);
            }

            // Створюємо нового клієнта
            var customer = new Customer
            {
                FirstName = customerInfo.FirstName,
                LastName = customerInfo.LastName,
                Email = customerInfo.Email,
                Phone = customerInfo.Phone,
                Address = customerInfo.Address
            };

            // Створюємо нове замовлення
            var order = new Order
            {
                Customer = customer,
                OrderDate = DateTime.Now,
                TotalAmount = cart.GetTotalPrice()
            };

            // Додаємо деталі замовлення
            foreach (var item in cart.CartItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    BookID = item.Book.Id,
                    Quantity = item.Count,
                    UnitPrice = item.Book.Price
                });
            }

            // Зберігаємо замовлення до бази даних
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Очищуємо кошик після оформлення замовлення
            HttpContext.Session.Remove("cart");

            // Переадресація на сторінку підтвердження замовлення
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}