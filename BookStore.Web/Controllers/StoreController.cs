using BookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApiService _api;
        public StoreController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            ViewBag.Categories = await _api.GetCategoriesAsync();
            var books = await _api.GetBooksAsync(categoryId);
            return View(books);
        }

        public async Task<IActionResult> Book(int id)
        {
            var book = await _api.GetBookAsync(id);
            if (book == null) return NotFound();
            ViewData["CategoryName"] = book.Category?.Name;
            TempData["LastViewedBookId"] = id;
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Favorite(int bookId)
        {
            // demo: userId=1 sabit
            await _api.AddFavoriteAsync(new FavoriteCreateDto(1, bookId));
            TempData["CartMessage"] = "Favorilere eklendi.";
            // Badge güncellemesi: sayfada component yeniden render olduğunda API'den sayılır
            return RedirectToAction("Book", new { id = bookId });
        }

        [HttpPost]
        public async Task<IActionResult> FavoriteAjax(int bookId)
        {
            try
            {
                await _api.AddFavoriteAsync(new FavoriteCreateDto(1, bookId));
                var count = (await _api.GetFavoritesAsync(1)).Count;
                return new JsonResult(new { favoritesCount = count, message = "Favorilere eklendi." });
            }
            catch
            {
                // Offline fallback: session'da yerel favoriler listesi tut
                var list = HttpContext.Session.GetObjectFromJson<List<int>>("FAVS_LOCAL") ?? new();
                if (!list.Contains(bookId)) list.Add(bookId);
                HttpContext.Session.SetObjectAsJson("FAVS_LOCAL", list);
                return new JsonResult(new { favoritesCount = list.Count, message = "Favorilere eklendi (yerel)." });
            }
        }

        public async Task<IActionResult> Category(int id)
        {
            var category = await _api.GetCategoryAsync(id);
            if (category == null) return NotFound();
            ViewBag.Category = category;
            var books = await _api.GetBooksAsync(id);
            return View("Index", books);
        }
    }
}


