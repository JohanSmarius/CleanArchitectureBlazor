using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// Handles the <see cref="CreateStaffCommand"/> by delegating to the command repository.
/// </summary>
public class CreateStaffCommandHandler : ICreateStaffCommandHandler
{
    private readonly IStaffRepository _staffRepository;
    private readonly ILogger<CreateStaffCommandHandler> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="CreateStaffCommandHandler"/>.
    /// </summary>
    public CreateStaffCommandHandler(
        IStaffRepository staffRepository,
        ILogger<CreateStaffCommandHandler> logger)
    {
        _staffRepository = staffRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Staff> Handle(CreateStaffCommand command)
    {
        var entity = new Staff
        {
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

        var createdStaff = await _staffRepository.CreateStaffAsync(entity);
        _logger.LogInformation("Staff member {StaffId} created successfully.", createdStaff.Id);
        return createdStaff;
    }
}
