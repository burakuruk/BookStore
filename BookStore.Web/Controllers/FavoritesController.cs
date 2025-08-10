using BookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ApiService _api;
        public FavoritesController(ApiService api) { _api = api; }

        public async Task<IActionResult> Index()
        {
            var apiList = new List<FavoriteDto>();
            try
            {
                apiList = await _api.GetFavoritesAsync(1);
            }
            catch
            {
                // ignore, offline fallback aşağıda
            }

            var local = HttpContext.Session.GetObjectFromJson<List<int>>("FAVS_LOCAL") ?? new();
            var union = new List<FavoriteDto>(apiList);
            foreach (var id in local)
            {
                if (!union.Any(x => x.BookId == id))
                {
                    var book = await _api.GetBookAsync(id);
                    if (book != null)
                    {
                        union.Add(new FavoriteDto(0, 1, id, DateTime.UtcNow, book));
                    }
                }
            }
            return View(union);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int bookId)
        {
            if (id > 0)
            {
                await _api.DeleteFavoriteAsync(id);
            }
            else
            {
                var list = HttpContext.Session.GetObjectFromJson<List<int>>("FAVS_LOCAL") ?? new();
                list = list.Where(x => x != bookId).ToList();
                HttpContext.Session.SetObjectAsJson("FAVS_LOCAL", list);
            }
            return RedirectToAction("Index");
        }
    }
}


