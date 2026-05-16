using ClientApplication.DataAdapters;
using Entities;

namespace ClientApplication.Queries;

/// <summary>
/// Handles the <see cref="GetAllStaffQuery"/> by delegating to the repository.
/// </summary>
public class GetAllStaffQueryHandler : IGetAllStaffQueryHandler
{
    private readonly IExternalStaffRepository _staffRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetAllStaffQueryHandler"/>.
    /// </summary>
    public GetAllStaffQueryHandler(IExternalStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    /// <inheritdoc />
    public async Task<List<StaffDTO>> Handle(GetAllStaffQuery query)
        => await _staffRepository.GetAllStaffAsync();
}
