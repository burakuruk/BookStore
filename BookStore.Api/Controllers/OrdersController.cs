using BookStore.Api.Data;
using BookStore.Api.Dtos;
using BookStore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _db.Orders.Include(o => o.Items).ThenInclude(i => i.Book).AsNoTracking().ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _db.Orders.Include(o => o.Items).ThenInclude(i => i.Book).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Create(OrderCreateDto request)
        {
            // Kullanıcı doğrulaması (FK hatasını önlemek için)
            var userExists = await _db.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
            {
                return BadRequest("Invalid userId.");
            }

            // Ürün/doğrulama ve fiyatların sunucudan alınması (istemci fiyatına güvenmeyin)
            var requestedByBookId = request.Items
                .GroupBy(i => i.BookId)
                .ToDictionary(g => g.Key, g => new { Quantity = g.Sum(x => x.Quantity) });

            var bookIds = requestedByBookId.Keys.ToList();
            var books = await _db.Books.Where(b => bookIds.Contains(b.Id)).ToListAsync();
            if (books.Count != bookIds.Count)
            {
                return BadRequest("One or more bookIds are invalid.");
            }

            var order = new Order
            {
                UserId = request.UserId,
                Items = books.Select(b => new OrderItem
                {
                    BookId = b.Id,
                    Quantity = requestedByBookId[b.Id].Quantity,
                    UnitPrice = b.Price
                }).ToList()
            };

            order.TotalPrice = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
    }
}


