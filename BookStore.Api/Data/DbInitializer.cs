using BookStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Data
{
    public static class DbInitializer
    {
        public static async Task MigrateAndSeedAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();
            await SeedCuratedAsync(db, wipeExisting: false);
        }

        public static async Task SeedCuratedAsync(ApplicationDbContext db, bool wipeExisting)
        {
            if (wipeExisting)
            {
                await db.Favorites.ExecuteDeleteAsync();
                await db.Orders.ExecuteDeleteAsync();
                await db.Books.ExecuteDeleteAsync();
                await db.Categories.ExecuteDeleteAsync();
                await db.Users.ExecuteDeleteAsync();
            }

            if (!await db.Users.AnyAsync())
            {
                db.Users.Add(new AppUser { FullName = "Demo Kullanıcı", Email = "demo@bookstore.local", Role = "User" });
                await db.SaveChangesAsync();
            }

            if (await db.Categories.AnyAsync() && await db.Books.AnyAsync()) return;

            var catClassics = new Category { Name = "Dünya Klasikleri", Description = "Edebiyatın ölümsüz eserleri" };
            var catTurkish  = new Category { Name = "Türk Edebiyatı", Description = "Türk yazarların seçkin eserleri" };
            var catSciFi    = new Category { Name = "Bilim Kurgu", Description = "Bilim kurgu başyapıtları" };
            var catSelf     = new Category { Name = "Kişisel Gelişim", Description = "Motivasyon ve gelişim" };

            db.Categories.AddRange(catClassics, catTurkish, catSciFi, catSelf);
            await db.SaveChangesAsync();

            var data = new (string Title, string Author, Category Cat, string? Image)[]
            {
                ("Suç ve Ceza","Fyodor Dostoyevski",catClassics, "https://upload.wikimedia.org/wikipedia/commons/4/4b/Dostoevsky_-_Crime_and_Punishment_-_first_edition%2C_1866.jpg"),
                ("Sefiller","Victor Hugo",catClassics, "https://upload.wikimedia.org/wikipedia/commons/6/6e/Les_Miserables_%281862%29_-_1st_edition.jpg"),
                ("Don Kişot","Miguel de Cervantes",catClassics, "https://upload.wikimedia.org/wikipedia/commons/6/6f/Don_Quijote_de_la_Mancha.jpg"),
                ("Savaş ve Barış","Lev Tolstoy",catClassics, "https://upload.wikimedia.org/wikipedia/commons/3/35/War-and-peace_1873.jpg"),
                ("Madame Bovary","Gustave Flaubert",catClassics, null),
                ("Anna Karenina","Lev Tolstoy",catClassics, null),
                ("Karamazov Kardeşler","Fyodor Dostoyevski",catClassics, null),
                ("Bülbülü Öldürmek","Harper Lee",catClassics, null),
                ("Fareler ve İnsanlar","John Steinbeck",catClassics, null),
                ("Yeraltından Notlar","Fyodor Dostoyevski",catClassics, null),
                ("Dorian Gray’in Portresi","Oscar Wilde",catClassics, null),
                ("Gurur ve Önyargı","Jane Austen",catClassics, null),
                ("Yakıcı Sırlar","Stefan Zweig",catClassics, null),
                ("İki Şehrin Hikayesi","Charles Dickens",catClassics, null),

                ("Kürk Mantolu Madonna","Sabahattin Ali",catTurkish, "https://upload.wikimedia.org/wikipedia/tr/2/2e/K%C3%BCrk_Mantolu_Madonna_kitap_kapak.jpg"),
                ("Tutunamayanlar","Oğuz Atay",catTurkish, "https://upload.wikimedia.org/wikipedia/tr/e/ee/Tutunamayanlar.jpg"),
                ("İnce Memed","Yaşar Kemal",catTurkish, "https://upload.wikimedia.org/wikipedia/tr/5/56/%C4%B0nce_Memed_1.jpg"),
                ("Saatleri Ayarlama Enstitüsü","Ahmet Hamdi Tanpınar",catTurkish, "https://upload.wikimedia.org/wikipedia/tr/8/81/Saatleri_Ayarlama_Enstit%C3%BCs%C3%BC.jpg"),
                ("Çalıkuşu","Reşat Nuri Güntekin",catTurkish, null),
                ("Kuyucaklı Yusuf","Sabahattin Ali",catTurkish, null),
                ("Aylak Adam","Yusuf Atılgan",catTurkish, null),
                ("Tehlikeli Oyunlar","Oğuz Atay",catTurkish, null),
                ("Huzur","Ahmet Hamdi Tanpınar",catTurkish, null),
                ("Sinekli Bakkal","Halide Edip Adıvar",catTurkish, null),
                ("Yeni Hayat","Orhan Pamuk",catTurkish, null),
                ("Puslu Kıtalar Atlası","İhsan Oktay Anar",catTurkish, null),

                ("Dune","Frank Herbert",catSciFi, "https://upload.wikimedia.org/wikipedia/en/5/51/Dune_first_edition.jpg"),
                ("1984","George Orwell",catSciFi, "https://upload.wikimedia.org/wikipedia/en/c/c3/1984first.jpg"),
                ("Vakıf","Isaac Asimov",catSciFi, "https://upload.wikimedia.org/wikipedia/en/b/bd/Foundation_gnome.jpg"),
                ("Cesur Yeni Dünya","Aldous Huxley",catSciFi, null),
                ("Marslı","Andy Weir",catSciFi, null),
                ("Zaman Makinesi","H. G. Wells",catSciFi, null),
                ("Ben, Robot","Isaac Asimov",catSciFi, null),
                ("Neuromancer","William Gibson",catSciFi, null),
                ("Karanlığın Yüreği","Joseph Conrad",catClassics, null),

                ("Alışkanlıkların Gücü","Charles Duhigg",catSelf, "https://upload.wikimedia.org/wikipedia/en/3/3c/The_Power_of_Habit.jpg"),
                ("Atomik Alışkanlıklar","James Clear",catSelf, "https://m.media-amazon.com/images/I/81YkqyaFVEL.jpg"),
                ("Derin Çalışma","Cal Newport",catSelf, null),
                ("Düşün ve Zengin Ol","Napoleon Hill",catSelf, null),
                ("Akıllı Yatırımcı","Benjamin Graham",catSelf, null),
                ("Net %100","Stephen R. Covey",catSelf, null),
                ("Grit – Azim","Angela Duckworth",catSelf, null),
                ("Outliers","Malcolm Gladwell",catSelf, null),
                ("Akış","Mihaly Csikszentmihalyi",catSelf, null),
                ("Başarının 7 Alışkanlığı","Stephen R. Covey",catSelf, null)
            };

            var books = new List<Book>();
            foreach (var it in data)
            {
                var seed = Uri.EscapeDataString(it.Title.Replace(" ", "_"));
                var image = it.Image ?? $"https://picsum.photos/seed/{seed}/600/400";
                var price = Math.Round(Random.Shared.Next(89, 300) + (decimal)Random.Shared.Next(0, 99) / 100m, 2);
                books.Add(new Book
                {
                    Title = it.Title,
                    Author = it.Author,
                    CategoryId = it.Cat.Id,
                    Price = price,
                    ImageUrl = image
                });
            }

            // 50'ye tamamlamak için gerekirse placeholder ekle
            while (books.Count < 50)
            {
                var idx = books.Count + 1;
                var c = (idx % 4) switch { 0 => catClassics.Id, 1 => catTurkish.Id, 2 => catSciFi.Id, _ => catSelf.Id };
                var price = Math.Round(Random.Shared.Next(89, 300) + (decimal)Random.Shared.Next(0, 99) / 100m, 2);
                books.Add(new Book
                {
                    Title = $"Klasik Seçki {idx}",
                    Author = "Derleme",
                    CategoryId = c,
                    Price = price,
                    ImageUrl = $"https://picsum.photos/seed/extra_{idx}/600/400"
                });
            }

            db.Books.AddRange(books);
            await db.SaveChangesAsync();
        }
    }
}


