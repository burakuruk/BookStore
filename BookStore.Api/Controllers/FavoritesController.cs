using BookStore.Api.Data;
using BookStore.Api.Dtos;
using BookStore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public FavoritesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetAll([FromQuery] int? userId)
        {
            var query = _db.Favorites.Include(f => f.Book).AsQueryable();
            if (userId.HasValue)
            {
                query = query.Where(f => f.UserId == userId);
            }
            return Ok(await query.AsNoTracking().ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Favorite>> Create(FavoriteCreateDto request)
        {
            // duplicate kontrolü
            bool exists = await _db.Favorites.AnyAsync(f => f.UserId == request.UserId && f.BookId == request.BookId);
            if (exists) return Conflict("Book already in favorites.");
            var entity = new Favorite { UserId = request.UserId, BookId = request.BookId };
            _db.Favorites.Add(entity);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { userId = request.UserId }, entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Favorites.FindAsync(id);
            if (entity == null) return NotFound();
            _db.Favorites.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}


