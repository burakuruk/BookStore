using BookStore.Api.Data;
using BookStore.Api.Dtos;
using BookStore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await _db.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _db.Categories.Include(c => c.Books).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create(CategoryCreateDto request)
        {
            var entity = new Category { Name = request.Name, Description = request.Description };
            _db.Categories.Add(entity);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CategoryUpdateDto request)
        {
            var entity = await _db.Categories.FindAsync(id);
            if (entity == null) return NotFound();
            entity.Name = request.Name;
            entity.Description = request.Description;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Categories.FindAsync(id);
            if (entity == null) return NotFound();
            _db.Categories.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}


