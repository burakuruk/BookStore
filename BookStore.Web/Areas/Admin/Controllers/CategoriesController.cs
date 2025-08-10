using BookStore.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApiService _api;
        public CategoriesController(ApiService api) { _api = api; }

        public async Task<IActionResult> Index()
        {
            var list = await _api.GetCategoriesAsync();
            return View(list);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto model)
        {
            if (!ModelState.IsValid) return View(model);
            await _api.CreateCategoryAsync(model);
            TempData["Success"] = "Kategori oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _api.GetCategoryAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryDto model)
        {
            if (id != model.Id) return BadRequest();
            await _api.UpdateCategoryAsync(id, model);
            TempData["Success"] = "Kategori güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _api.DeleteCategoryAsync(id);
            TempData["Info"] = "Kategori silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}


