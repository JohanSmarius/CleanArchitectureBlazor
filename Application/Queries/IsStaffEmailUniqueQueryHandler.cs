namespace Application.Queries;

/// <summary>
/// Handles the <see cref="IsStaffEmailUniqueQuery"/> by delegating to the repository.
/// </summary>
public class IsStaffEmailUniqueQueryHandler : IIsStaffEmailUniqueQueryHandler
{
    private readonly IStaffRepository _staffRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="IsStaffEmailUniqueQueryHandler"/>.
    /// </summary>
    public IsStaffEmailUniqueQueryHandler(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    /// <inheritdoc />
    public Task<bool> Handle(IsStaffEmailUniqueQuery query)
        => _staffRepository.IsEmailUniqueAsync(query.Email, query.ExcludeId);
}
