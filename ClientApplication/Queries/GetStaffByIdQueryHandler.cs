using ClientApplication.DataAdapters;

namespace ClientApplication.Queries;

/// <summary>
/// Handles the <see cref="GetStaffByIdQuery"/> by delegating to the repository.
/// </summary>
public class GetStaffByIdQueryHandler : IGetStaffByIdQueryHandler
{
    private readonly IExternalStaffRepository _staffRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetStaffByIdQueryHandler"/>.
    /// </summary>
    public GetStaffByIdQueryHandler(IExternalStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    /// <inheritdoc />
    public async Task<StaffDTO?> Handle(GetStaffByIdQuery query)
        => await _staffRepository.GetStaffByIdAsync(query.Id);
}
