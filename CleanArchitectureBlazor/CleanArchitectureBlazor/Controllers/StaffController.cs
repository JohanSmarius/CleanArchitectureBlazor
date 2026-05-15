using Application.Commands;
using Application.Queries;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureBlazor.Controllers;

[ApiController]
[Route("api/staff")]
public class StaffController : ControllerBase
{
    private readonly IGetAllStaffQueryHandler _getAllStaffQueryHandler;
    private readonly IGetStaffByIdQueryHandler _getStaffByIdQueryHandler;
    private readonly IIsStaffEmailUniqueQueryHandler _isStaffEmailUniqueQueryHandler;
    private readonly ICreateStaffCommandHandler _createStaffCommandHandler;
    private readonly IUpdateStaffCommandHandler _updateStaffCommandHandler;
    private readonly IDeleteStaffCommandHandler _deleteStaffCommandHandler;

    public StaffController(
        IGetAllStaffQueryHandler getAllStaffQueryHandler,
        IGetStaffByIdQueryHandler getStaffByIdQueryHandler,
        IIsStaffEmailUniqueQueryHandler isStaffEmailUniqueQueryHandler,
        ICreateStaffCommandHandler createStaffCommandHandler,
        IUpdateStaffCommandHandler updateStaffCommandHandler,
        IDeleteStaffCommandHandler deleteStaffCommandHandler)
    {
        _getAllStaffQueryHandler = getAllStaffQueryHandler;
        _getStaffByIdQueryHandler = getStaffByIdQueryHandler;
        _isStaffEmailUniqueQueryHandler = isStaffEmailUniqueQueryHandler;
        _createStaffCommandHandler = createStaffCommandHandler;
        _updateStaffCommandHandler = updateStaffCommandHandler;
        _deleteStaffCommandHandler = deleteStaffCommandHandler;
    }

    [HttpGet]
    public async Task<ActionResult<List<Staff>>> GetAll()
    {
        var staff = await _getAllStaffQueryHandler.Handle(new GetAllStaffQuery());
        return Ok(staff);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Staff>> GetById(int id)
    {
        var staff = await _getStaffByIdQueryHandler.Handle(new GetStaffByIdQuery { Id = id });
        return staff is null ? NotFound() : Ok(staff);
    }

    [HttpGet("email-unique")]
    public async Task<ActionResult<bool>> IsEmailUnique([FromQuery] string email, [FromQuery] int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required.");
        }

        var isUnique = await _isStaffEmailUniqueQueryHandler.Handle(new IsStaffEmailUniqueQuery
        {
            Email = email,
            ExcludeId = excludeId
        });
        return Ok(isUnique);
    }

    [HttpPost]
    public async Task<ActionResult<Staff>> Create([FromBody] Staff staff)
    {
        var isEmailUnique = await _isStaffEmailUniqueQueryHandler.Handle(new IsStaffEmailUniqueQuery
        {
            Email = staff.Email
        });
        if (!isEmailUnique)
        {
            return BadRequest("A staff member with this email address already exists.");
        }

        var created = await _createStaffCommandHandler.Handle(new CreateStaffCommand
        {
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            Phone = staff.Phone,
            Role = staff.Role,
            CertificationLevel = staff.CertificationLevel,
            CertificationExpiry = staff.CertificationExpiry,
            Birthday = staff.Birthday,
            IsActive = staff.IsActive
        });

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Staff>> Update(int id, [FromBody] Staff staff)
    {
        if (id != staff.Id)
        {
            return BadRequest("The route id must match the staff payload id.");
        }

        var existing = await _getStaffByIdQueryHandler.Handle(new GetStaffByIdQuery { Id = id });
        if (existing is null)
        {
            return NotFound();
        }

        var isEmailUnique = await _isStaffEmailUniqueQueryHandler.Handle(new IsStaffEmailUniqueQuery
        {
            Email = staff.Email,
            ExcludeId = staff.Id
        });
        if (!isEmailUnique)
        {
            return BadRequest("A staff member with this email address already exists.");
        }

        var updated = await _updateStaffCommandHandler.Handle(new UpdateStaffCommand
        {
            Id = staff.Id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            Phone = staff.Phone,
            Role = staff.Role,
            CertificationLevel = staff.CertificationLevel,
            CertificationExpiry = staff.CertificationExpiry,
            Birthday = staff.Birthday,
            IsActive = staff.IsActive
        });

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _getStaffByIdQueryHandler.Handle(new GetStaffByIdQuery { Id = id });
        if (existing is null)
        {
            return NotFound();
        }

        await _deleteStaffCommandHandler.Handle(new DeleteStaffCommand { Id = id });
        return NoContent();
    }
}
