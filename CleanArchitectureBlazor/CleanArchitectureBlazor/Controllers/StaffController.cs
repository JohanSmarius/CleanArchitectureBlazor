using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CleanArchitectureBlazor.Data;
using CleanArchitectureBlazor.Models;

namespace CleanArchitectureBlazor.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StaffController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StaffController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Staff
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Staff>>> GetStaff()
    {
        return await _context.Staff
            .Include(s => s.StaffAssignments)
            .ToListAsync();
    }

    // GET: api/Staff/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Staff>> GetStaff(int id)
    {
        var staff = await _context.Staff
            .Include(s => s.StaffAssignments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (staff == null)
        {
            return NotFound();
        }

        return staff;
    }

    // PUT: api/Staff/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutStaff(int id, Staff staff)
    {
        if (id != staff.Id)
        {
            return BadRequest();
        }

        staff.UpdatedAt = DateTime.UtcNow;
        _context.Entry(staff).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StaffExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Staff
    [HttpPost]
    public async Task<ActionResult<Staff>> PostStaff(Staff staff)
    {
        staff.CreatedAt = DateTime.UtcNow;
        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetStaff", new { id = staff.Id }, staff);
    }

    // DELETE: api/Staff/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null)
        {
            return NotFound();
        }

        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool StaffExists(int id)
    {
        return _context.Staff.Any(e => e.Id == id);
    }
}

