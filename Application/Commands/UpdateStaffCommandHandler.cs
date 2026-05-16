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
        var storedStaff = await _staffRepository.GetStaffByIdAsync(command.Id);

        if (storedStaff is null)
        {
            throw new ApplicationException($"Staff member with id {command.Id} not found.");
        }

        storedStaff.FirstName = command.FirstName;
        storedStaff.LastName = command.LastName;
        storedStaff.Email = command.Email;
        storedStaff.Phone = command.Phone;
        storedStaff.Role = (StaffRole)command.Role;
        storedStaff.CertificationLevel = command.CertificationLevel;
        storedStaff.CertificationExpiry = command.CertificationExpiry;
        storedStaff.Birthday = command.Birthday;
        storedStaff.IsActive = command.IsActive;
        

        var updatedStaff = await _staffRepository.UpdateStaffAsync(storedStaff);
        _logger.LogInformation("Staff member {StaffId} updated successfully.", storedStaff.Id);
        return storedStaff;
    }
}
