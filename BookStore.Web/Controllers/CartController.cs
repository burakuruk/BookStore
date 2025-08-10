using BookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ApiService _api;
        public CartController(ApiService api) { _api = api; }

        private const string SessionKey = "CART";

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItemCreateDto>>(SessionKey) ?? new();
            var vm = cart.Select(i => new Models.CartItemViewModel
            {
                BookId = i.BookId,
                Title = _titleCache.GetOrAdd(i.BookId, () => _api.GetTitleAsync(i.BookId).GetAwaiter().GetResult() ?? $"#" + i.BookId),
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();
            ViewBag.Total = cart.Sum(i => i.UnitPrice * i.Quantity);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Add(int id, string title, decimal price)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItemCreateDto>>(SessionKey) ?? new();
            var existing = cart.FirstOrDefault(i => i.BookId == id);
            if (existing is null) cart.Add(new OrderItemCreateDto(id, 1, price));
            else cart[cart.IndexOf(existing)] = existing with { Quantity = existing.Quantity + 1 };
            HttpContext.Session.SetObjectAsJson(SessionKey, cart);
            TempData["CartMessage"] = $"{title} sepete eklendi.";
            TempData["CartCount"] = cart.Sum(i => i.Quantity);
            return RedirectToAction("Index", "Store");
        }

        [HttpPost]
        public IActionResult AddAjax(int id, string title, decimal price)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItemCreateDto>>(SessionKey) ?? new();
            var existing = cart.FirstOrDefault(i => i.BookId == id);
            if (existing is null) cart.Add(new OrderItemCreateDto(id, 1, price));
            else cart[cart.IndexOf(existing)] = existing with { Quantity = existing.Quantity + 1 };
            HttpContext.Session.SetObjectAsJson(SessionKey, cart);
            var count = cart.Sum(i => i.Quantity);
            return new JsonResult(new { cartCount = count, message = $"{title} sepete eklendi." });
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItemCreateDto>>(SessionKey) ?? new();
            cart = cart.Where(i => i.BookId != id).ToList();
            HttpContext.Session.SetObjectAsJson(SessionKey, cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Increase(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItemCreateDto>>(SessionKey) ?? new();
            var it = cart.FirstOrDefault(x => x.BookId == id);
            if (it != null)
            {
                it = it with { Quantity = it.Quantity + 1 };
                cart[cart.FindIndex(x => x.BookId == id)] = it;
                HttpContext.Session.SetObjectAsJson(SessionKey, cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Decrease(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItemCreateDto>>(SessionKey) ?? new();
            var it = cart.FirstOrDefault(x => x.BookId == id);
            if (it != null)
            {
                var newQty = Math.Max(1, it.Quantity - 1);
                it = it with { Quantity = newQty };
                cart[cart.FindIndex(x => x.BookId == id)] = it;
                HttpContext.Session.SetObjectAsJson(SessionKey, cart);
            }
            return RedirectToAction("Index");
        }

        // lightweight title cache
        private readonly TitleCache _titleCache = new();

        private class TitleCache
        {
            private readonly Dictionary<int, string> _map = new();
            public string GetOrAdd(int id, Func<string> factory)
            {
                if (_map.TryGetValue(id, out var title)) return title;
                title = factory() ?? string.Empty;
                _map[id] = title;
                return title;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(int userId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItemCreateDto>>(SessionKey) ?? new();
            if (!cart.Any()) return RedirectToAction("Index");
            var orderId = await _api.CreateOrderAsync(new OrderCreateDto(userId, cart));
            HttpContext.Session.Remove(SessionKey);
            ViewBag.OrderId = orderId;
            return View("Success");
        }
    }
}


