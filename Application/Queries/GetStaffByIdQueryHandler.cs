using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetStaffByIdQuery"/> by delegating to the repository.
/// </summary>
public class GetStaffByIdQueryHandler : IGetStaffByIdQueryHandler
{
    private readonly IStaffRepository _staffRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetStaffByIdQueryHandler"/>.
    /// </summary>
    public GetStaffByIdQueryHandler(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    /// <inheritdoc />
    public Task<Staff?> Handle(GetStaffByIdQuery query)
        => _staffRepository.GetStaffByIdAsync(query.Id);
}
