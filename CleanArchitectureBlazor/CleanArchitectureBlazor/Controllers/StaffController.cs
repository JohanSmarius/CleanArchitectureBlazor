using Microsoft.AspNetCore.Mvc;
using CleanArchitectureBlazor.Models;
using CleanArchitectureBlazor.Services;

namespace CleanArchitectureBlazor.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    // GET: api/Staff
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Staff>>> GetStaff()
    {
        var staff = await _staffService.GetAllStaffAsync();
        return Ok(staff);
    }

    // GET: api/Staff/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Staff>> GetStaff(int id)
    {
        var staff = await _staffService.GetStaffByIdAsync(id);

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

        await _staffService.UpdateStaffAsync(staff);

        return NoContent();
    }

    // POST: api/Staff
    [HttpPost]
    public async Task<ActionResult<Staff>> PostStaff(Staff staff)
    {
        var createdStaff = await _staffService.CreateStaffAsync(staff);

        return CreatedAtAction(nameof(GetStaff), new { id = createdStaff.Id }, createdStaff);
    }

    // DELETE: api/Staff/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        var staff = await _staffService.GetStaffByIdAsync(id);
        if (staff == null)
        {
            return NotFound();
        }

        await _staffService.DeleteStaffAsync(id);

        return NoContent();
    }
}

