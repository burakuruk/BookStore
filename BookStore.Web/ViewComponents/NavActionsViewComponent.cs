using BookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.ViewComponents
{
    public class NavActionsViewComponent : ViewComponent
    {
        private readonly ApiService _api;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NavActionsViewComponent(ApiService api, IHttpContextAccessor httpContextAccessor)
        {
            _api = api;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var http = _httpContextAccessor.HttpContext!;
            var cart = http.Session.GetObjectFromJson<List<OrderItemCreateDto>>("CART") ?? new();
            int cartCount = cart.Sum(i => i.Quantity);
            int favCount = 0;
            try
            {
                // demo: userId=1
                favCount = (await _api.GetFavoritesAsync(1)).Count;
            }
            catch { /* ignore network errors for navbar */ }

            var vm = new NavCountsVM(cartCount, favCount);
            return View(vm);
        }
    }

    public record NavCountsVM(int CartCount, int FavoritesCount);
}


