# BookStore (Kılavuz)

İki proje içerir:
- `BookStore.Api` (REST API, EF Core + SQLite)
- `BookStore.Web` (ASP.NET Core MVC + Identity, Admin alanı)

Gereksinim
- .NET 8 SDK

Çalıştırma (Geliştirme) (TERMİNALDE)
1) SSL (bir kez):
dotnet dev-certs https --trust

2) API (https):
dotnet run --project ./BookStore.Api/BookStore.Api.csproj --launch-profile https
Swagger: https://localhost:5001/swagger

3) Web (https):
dotnet tool install --global dotnet-ef
dotnet ef database update --project ./BookStore.Web/BookStore.Web.csproj
dotnet run --project ./BookStore.Web/BookStore.Web.csproj --launch-profile https
Site: https://localhost:7186

Admin Girişi
- E-posta: `admin@bookstore.local`
- Parola: `Admin123!`
- Panel: `https://localhost:7186/Admin`

Öne Çıkanlar
- Katalog, kategori filtreleme, detay, favoriler, sepet ve sipariş akışı
- Admin’den kategori/kitap CRUD ve resim yükleme (API üzerinden)
- Doğrulama: FluentValidation (API), fiyatlar server-side hesaplanır

