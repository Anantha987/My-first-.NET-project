//using Microsoft.AspNetCore.Mvc;

//namespace ShopxCodeProject
//{
//    public class AdminController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopX.Data;
using ShopX.Models;

namespace ShopX.Controllers
{
    [Authorize] // Admin only in real app; demo lets any signed user manage
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _config;

        public AdminController(AppDbContext db, BlobServiceClient blobServiceClient, IConfiguration config)
        {
            _db = db;
            _blobServiceClient = blobServiceClient;
            _config = config;
        }

        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            return View(products);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var container = _blobServiceClient.GetBlobContainerClient(_config["AzureStorage:Container"]);
                await container.CreateIfNotExistsAsync();
                var blob = container.GetBlobClient($"{Guid.NewGuid()}-{image.FileName}");
                await blob.UploadAsync(image.OpenReadStream(), overwrite: true);
                product.ImageUrl = blob.Uri.ToString();
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile? image)
        {
            var existing = await _db.Products.FindAsync(product.Id);
            if (existing == null) return NotFound();

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Description = product.Description;

            if (image != null && image.Length > 0)
            {
                var container = _blobServiceClient.GetBlobContainerClient(_config["AzureStorage:Container"]);
                await container.CreateIfNotExistsAsync();
                var blob = container.GetBlobClient($"{Guid.NewGuid()}-{image.FileName}");
                await blob.UploadAsync(image.OpenReadStream(), overwrite: true);
                existing.ImageUrl = blob.Uri.ToString();
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
