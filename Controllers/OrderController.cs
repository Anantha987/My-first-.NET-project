using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopX.Data;
using ShopX.Models;

namespace ShopX.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _db;
        public OrderController(AppDbContext db) { _db = db; }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int productId, int quantity)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var product = await _db.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var order = new Order
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                TotalAmount = product.Price * quantity
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess() => View();
    }
}
