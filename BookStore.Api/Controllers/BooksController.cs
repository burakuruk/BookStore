using BookStore.Api.Data;
using BookStore.Api.Dtos;
using BookStore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAll([FromQuery] int? categoryId)
        {
            var query = _db.Books.Include(b => b.Category).AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId);
            }
            return Ok(await query.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> GetById(int id)
        {
            var book = await _db.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Create(BookCreateDto request)
        {
            var entity = new Book
            {
                Title = request.Title,
                Author = request.Author,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId
            };
            _db.Books.Add(entity);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, BookUpdateDto request)
        {
            var entity = await _db.Books.FindAsync(id);
            if (entity == null) return NotFound();
            entity.Title = request.Title;
            entity.Author = request.Author;
            entity.Price = request.Price;
            entity.ImageUrl = request.ImageUrl;
            entity.CategoryId = request.CategoryId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Books.FindAsync(id);
            if (entity == null) return NotFound();
            _db.Books.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}


