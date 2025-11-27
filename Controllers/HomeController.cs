using Microsoft.AspNetCore.Mvc;
using ShopX.Models;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // TEMP DATA for test
        var products = new List<Product>()
        {
            new Product { Name="Laptop", Price=50000, Description="Test laptop" },
            new Product { Name="Mobile", Price=20000, Description="Test mobile" }
        };

        return View(products);
    }
}
