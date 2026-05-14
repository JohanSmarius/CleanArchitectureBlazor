using Microsoft.AspNetCore.Mvc;
using Domain;
using DomainService;

namespace CleanArchitectureBlazor.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StaffController : ControllerBase
{
    private readonly IStaffRepository _staffRepository;

    public StaffController(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    // GET: api/Staff
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Staff>>> GetStaff()
    {
        return Ok(await _staffRepository.GetAllStaffAsync());
    }

    // GET: api/Staff/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Staff>> GetStaffById(int id)
    {
        var staff = await _staffRepository.GetStaffByIdAsync(id);

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
            return BadRequest("The route id must match the staff payload id.");
        }

        if (!await _staffRepository.IsEmailUniqueAsync(staff.Email, staff.Id))
        {
            return BadRequest("A staff member with this email address already exists.");
        }

        if (staff.Birthday.HasValue && staff.Birthday.Value.Date > DateTime.UtcNow.Date)
        {
            return BadRequest("Birthday cannot be in the future.");
        }

        var existingStaff = await _staffRepository.GetStaffByIdAsync(id);
        if (existingStaff == null)
        {
            return NotFound();
        }

        await _staffRepository.UpdateStaffAsync(staff);

        return NoContent();
    }

    // POST: api/Staff
    [HttpPost]
    public async Task<ActionResult<Staff>> PostStaff(Staff staff)
    {
        if (!await _staffRepository.IsEmailUniqueAsync(staff.Email))
        {
            return BadRequest("A staff member with this email address already exists.");
        }

        if (staff.Birthday.HasValue && staff.Birthday.Value.Date > DateTime.UtcNow.Date)
        {
            return BadRequest("Birthday cannot be in the future.");
        }

        var createdStaff = await _staffRepository.CreateStaffAsync(staff);

        return CreatedAtAction(nameof(GetStaffById), new { id = createdStaff.Id }, createdStaff);
    }

    // DELETE: api/Staff/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        var staff = await _staffRepository.GetStaffByIdAsync(id);
        if (staff == null)
        {
            return NotFound();
        }

        await _staffRepository.DeleteStaffAsync(id);

        return NoContent();
    }
}
