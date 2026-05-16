using ClientApplication.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;

namespace ClientApplication.Commands;

/// <summary>
/// Handles the <see cref="UpdateStaffCommand"/> by delegating to the command repository.
/// </summary>
public class UpdateStaffCommandHandler : IUpdateStaffCommandHandler
{
    private readonly IExternalStaffRepository _staffRepository;
    private readonly ILogger<UpdateStaffCommandHandler> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="UpdateStaffCommandHandler"/>.
    /// </summary>
    public UpdateStaffCommandHandler(
        IExternalStaffRepository staffRepository,
        ILogger<UpdateStaffCommandHandler> logger)
    {
        _staffRepository = staffRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Handle(UpdateStaffCommand command)
    {
        var entity = new StaffDTO
        {
            Id = command.Id,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Phone = command.Phone,
            Role = (StaffRoleDTO)command.Role,
            CertificationLevel = command.CertificationLevel,
            CertificationExpiry = command.CertificationExpiry,
            Birthday = command.Birthday,
            IsActive = command.IsActive
        };

         await _staffRepository.UpdateStaffAsync(entity);
        _logger.LogInformation("Staff member {StaffId} updated successfully.", entity.Id);
    }
}
