using BookStore.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public SeedController(ApplicationDbContext db) { _db = db; }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            await DbInitializer.SeedCuratedAsync(_db, wipeExisting: true);
            return Ok(new { message = "Seed refreshed" });
        }
    }
}


