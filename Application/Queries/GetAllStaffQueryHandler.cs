using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetAllStaffQuery"/> by delegating to the repository.
/// </summary>
public class GetAllStaffQueryHandler : IGetAllStaffQueryHandler
{
    private readonly IStaffRepository _staffRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetAllStaffQueryHandler"/>.
    /// </summary>
    public GetAllStaffQueryHandler(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    /// <inheritdoc />
    public Task<List<Staff>> Handle(GetAllStaffQuery query)
        => _staffRepository.GetAllStaffAsync();
}
