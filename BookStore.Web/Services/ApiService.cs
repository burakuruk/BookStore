using System.Net.Http.Json;

namespace BookStore.Web.Services
{
        public class ApiService
    {
            private readonly IHttpClientFactory _httpClientFactory;
            public ApiService(IHttpClientFactory httpClientFactory)
        {
                _httpClientFactory = httpClientFactory;
        }

        public async Task<string?> GetTitleAsync(int id)
        {
            var client = CreateClient();
            var book = await client.GetFromJsonAsync<BookDto>($"/api/books/{id}");
            return book?.Title;
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("ApiClient");
        }

        // Categories
        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<CategoryDto>>("/api/categories") ?? new();
        }
        public async Task<CategoryDto?> GetCategoryAsync(int id)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<CategoryDto>($"/api/categories/{id}");
        }
        public async Task CreateCategoryAsync(CategoryDto model)
        {
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/categories", model);
            resp.EnsureSuccessStatusCode();
        }
        public async Task UpdateCategoryAsync(int id, CategoryDto model)
        {
            var client = CreateClient();
            var resp = await client.PutAsJsonAsync($"/api/categories/{id}", model);
            resp.EnsureSuccessStatusCode();
        }
        public async Task DeleteCategoryAsync(int id)
        {
            var client = CreateClient();
            var resp = await client.DeleteAsync($"/api/categories/{id}");
            resp.EnsureSuccessStatusCode();
        }

        // Books
        public async Task<List<BookDto>> GetBooksAsync(int? categoryId = null)
        {
            var client = CreateClient();
            var url = "/api/books" + (categoryId.HasValue ? $"?categoryId={categoryId}" : "");
            return await client.GetFromJsonAsync<List<BookDto>>(url) ?? new();
        }
        public async Task<BookDto?> GetBookAsync(int id)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<BookDto>($"/api/books/{id}");
        }
        public async Task CreateBookAsync(BookCreateUpdateDto model)
        {
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/books", model);
            resp.EnsureSuccessStatusCode();
        }
        public async Task UpdateBookAsync(int id, BookCreateUpdateDto model)
        {
            var client = CreateClient();
            var resp = await client.PutAsJsonAsync($"/api/books/{id}", model);
            resp.EnsureSuccessStatusCode();
        }
        public async Task DeleteBookAsync(int id)
        {
            var client = CreateClient();
            var resp = await client.DeleteAsync($"/api/books/{id}");
            resp.EnsureSuccessStatusCode();
        }

        // Favorites
        public async Task<List<FavoriteDto>> GetFavoritesAsync(int userId)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<FavoriteDto>>($"/api/favorites?userId={userId}") ?? new();
        }
        public async Task AddFavoriteAsync(FavoriteCreateDto model)
        {
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/favorites", model);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteFavoriteAsync(int id)
        {
            var client = CreateClient();
            var resp = await client.DeleteAsync($"/api/favorites/{id}");
            resp.EnsureSuccessStatusCode();
        }

        // Orders
        public async Task<int> CreateOrderAsync(OrderCreateDto model)
        {
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/orders", model);
            resp.EnsureSuccessStatusCode();
            var created = await resp.Content.ReadFromJsonAsync<OrderDto>();
            return created?.Id ?? 0;
        }

        // File Upload (for admin image upload)
        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            var client = CreateClient();
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            try
            {
                // Set content-type based on extension so API'deki MIME kontrolünü geçsin
                var ct = GuessImageContentType(fileName);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ct);
            }
            catch { /* ignore */ }
            content.Add(streamContent, "file", fileName);
            var resp = await client.PostAsync("/api/files/upload", content);
            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadFromJsonAsync<UploadResult>();
            var url = result?.Url ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(url) && !url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                // combine with API base address to form absolute url for <img src>
                url = new Uri(CreateClient().BaseAddress!, url).ToString();
            }
            return url;
        }

        private static string GuessImageContentType(string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }

    // DTOs
    public record CategoryDto(int Id, string Name, string? Description);
    public record BookDto(int Id, string Title, string? Author, decimal Price, string? ImageUrl, int CategoryId, CategoryDto? Category);
    public record BookCreateUpdateDto(int Id, string Title, string? Author, decimal Price, string? ImageUrl, int CategoryId);
    public record FavoriteDto(int Id, int UserId, int BookId, DateTime CreatedAt, BookDto? Book);
    public record FavoriteCreateDto(int UserId, int BookId);
    public record OrderItemCreateDto(int BookId, int Quantity, decimal UnitPrice);
    public record OrderCreateDto(int UserId, List<OrderItemCreateDto> Items);
    public record OrderDto(int Id, int UserId, DateTime CreatedAt, decimal TotalPrice, string Status, List<OrderItemCreateDto> Items);
    public record UploadResult(string Url);
}


