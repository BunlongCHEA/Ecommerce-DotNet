using ECommerceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SubCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/subcategory
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetSubCategories()
        {
            var subCategories = await _context.SubCategories.ToListAsync();
            return Ok(subCategories);
        }

        // GET: api/subcategory/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SubCategory>> GetSubCategory(int id)
        {
            var subCategory = await _context.SubCategories.FirstOrDefaultAsync(sc => sc.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }
            return Ok(subCategory);
        }

        // POST: api/subcategory
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SubCategory>> CreateSubCategory(SubCategory subCategory)
        {
            if (subCategory == null || string.IsNullOrEmpty(subCategory.Name))
            {
                return BadRequest("Invalid Create SubCategory data. Some fields cannot be null or missing.");
            }

            _context.SubCategories.Add(subCategory);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSubCategory), new { id = subCategory.Id }, subCategory);
        }

        // PUT: api/subcategory/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubCategory(int id, SubCategory subCategory)
        {
            if (id != subCategory.Id || string.IsNullOrEmpty(subCategory.Name))
            {
                return BadRequest("Invalid SubCategory column data OR mismatch SubCategory ID.");
            }

            _context.Entry(subCategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/subcategory/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}