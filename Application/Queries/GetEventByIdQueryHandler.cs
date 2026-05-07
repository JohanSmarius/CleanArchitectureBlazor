using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetEventByIdQuery"/> by delegating to the read-side repository.
/// </summary>
public class GetEventByIdQueryHandler : IGetEventByIdQueryHandler
{
    private readonly IEventQueryRepository _queryRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetEventByIdQueryHandler"/>.
    /// </summary>
    /// <param name="queryRepository">Read-side repository for event retrieval.</param>
    public GetEventByIdQueryHandler(IEventQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    /// <inheritdoc />
    public Task<Event?> Handle(GetEventByIdQuery query)
        => _queryRepository.GetEventByIdAsync(query.Id);
}
