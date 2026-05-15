using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// Handles the <see cref="UpdateStaffCommand"/> by delegating to the command repository.
/// </summary>
public class UpdateStaffCommandHandler : IUpdateStaffCommandHandler
{
    private readonly IStaffRepository _staffRepository;
    private readonly ILogger<UpdateStaffCommandHandler> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="UpdateStaffCommandHandler"/>.
    /// </summary>
    public UpdateStaffCommandHandler(
        IStaffRepository staffRepository,
        ILogger<UpdateStaffCommandHandler> logger)
    {
        _staffRepository = staffRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Staff> Handle(UpdateStaffCommand command)
    {
        var entity = new Staff
        {
            Id = command.Id,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Phone = command.Phone,
            Role = command.Role,
            CertificationLevel = command.CertificationLevel,
            CertificationExpiry = command.CertificationExpiry,
            Birthday = command.Birthday,
            IsActive = command.IsActive
        };

        var updatedStaff = await _staffRepository.UpdateStaffAsync(entity);
        _logger.LogInformation("Staff member {StaffId} updated successfully.", updatedStaff.Id);
        return updatedStaff;
    }
}
