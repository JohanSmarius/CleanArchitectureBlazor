using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetAllEventsQuery"/> by delegating to the read-side repository.
/// </summary>
public class GetAllEventsQueryHandler : IGetAllEventsQueryHandler
{
    private readonly IEventQueryRepository _queryRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetAllEventsQueryHandler"/>.
    /// </summary>
    /// <param name="queryRepository">Read-side repository for event retrieval.</param>
    public GetAllEventsQueryHandler(IEventQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    /// <inheritdoc />
    public Task<List<Event>> Handle(GetAllEventsQuery query)
        => _queryRepository.GetAllEventsAsync();
}
