using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetUpcomingEventsQuery"/> by delegating to the read-side repository.
/// </summary>
public class GetUpcomingEventsQueryHandler : IGetUpcomingEventsQueryHandler
{
    private readonly IEventQueryRepository _queryRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetUpcomingEventsQueryHandler"/>.
    /// </summary>
    /// <param name="queryRepository">Read-side repository for event retrieval.</param>
    public GetUpcomingEventsQueryHandler(IEventQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    /// <inheritdoc />
    public Task<List<Event>> Handle(GetUpcomingEventsQuery query)
        => _queryRepository.GetUpcomingEventsAsync();
}
