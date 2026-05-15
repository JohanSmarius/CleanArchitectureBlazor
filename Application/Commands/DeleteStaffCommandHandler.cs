using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// Handles the <see cref="DeleteStaffCommand"/> by delegating to the command repository.
/// </summary>
public class DeleteStaffCommandHandler : IDeleteStaffCommandHandler
{
    private readonly IStaffRepository _staffRepository;
    private readonly ILogger<DeleteStaffCommandHandler> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="DeleteStaffCommandHandler"/>.
    /// </summary>
    public DeleteStaffCommandHandler(
        IStaffRepository staffRepository,
        ILogger<DeleteStaffCommandHandler> logger)
    {
        _staffRepository = staffRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Handle(DeleteStaffCommand command)
    {
        await _staffRepository.DeleteStaffAsync(command.Id);
        _logger.LogInformation("Staff member deactivated successfully.");
    }
}
