using ECommerceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StoreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/store
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetStores()
        {
            var stores = await _context.Stores.ToListAsync();
            return Ok(stores);
        }

        // GET: api/store/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStore(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            // var store = await _context.Stores.Include(s => s.Products)
            //   .FirstOrDefaultAsync(s => s.Id == id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

        // POST: api/store
        [HttpPost]
        public async Task<ActionResult<Store>> CreateStore(Store store)
        {
            if (store == null || string.IsNullOrEmpty(store.Name))
            {
                return BadRequest("Invalid store data.");
            }

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStore), new { id = store.Id }, store);
        }

        // PUT: api/store/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStore(int id, Store store)
        {
            if (id != store.Id || string.IsNullOrEmpty(store.Name))
            {
                return BadRequest("Invalid Create Store data OR mismatch Store ID.");
            }
            _context.Entry(store).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Content("Store updated successfully");
        }

        // DELETE: api/store/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStore(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}