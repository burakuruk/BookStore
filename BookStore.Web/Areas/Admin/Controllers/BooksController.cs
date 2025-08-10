using BookStore.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly ApiService _api;
        private readonly IWebHostEnvironment _env;
        public BooksController(ApiService api, IWebHostEnvironment env)
        {
            _api = api;
            _env = env;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            ViewBag.Categories = await _api.GetCategoriesAsync();
            var list = await _api.GetBooksAsync(categoryId);
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _api.GetCategoriesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookCreateUpdateDto model, IFormFile? image)
        {
            if (image != null && image.Length > 0)
            {
                using var stream = image.OpenReadStream();
                var url = await _api.UploadImageAsync(stream, image.FileName);
                model = model with { ImageUrl = url };
            }
            await _api.CreateBookAsync(model);
            TempData["Success"] = "Kitap oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Categories = await _api.GetCategoriesAsync();
            var book = await _api.GetBookAsync(id);
            if (book == null) return NotFound();
            var vm = new BookCreateUpdateDto(book.Id, book.Title, book.Author, book.Price, book.ImageUrl, book.CategoryId);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BookCreateUpdateDto model, IFormFile? image)
        {
            if (image != null && image.Length > 0)
            {
                using var stream = image.OpenReadStream();
                var url = await _api.UploadImageAsync(stream, image.FileName);
                model = model with { ImageUrl = url };
            }
            await _api.UpdateBookAsync(id, model);
            TempData["Success"] = "Kitap güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _api.DeleteBookAsync(id);
            TempData["Info"] = "Kitap silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}


