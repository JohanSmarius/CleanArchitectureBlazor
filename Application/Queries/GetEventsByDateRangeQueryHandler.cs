using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetEventsByDateRangeQuery"/> by delegating to the read-side repository.
/// </summary>
public class GetEventsByDateRangeQueryHandler : IGetEventsByDateRangeQueryHandler
{
    private readonly IEventQueryRepository _queryRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetEventsByDateRangeQueryHandler"/>.
    /// </summary>
    /// <param name="queryRepository">Read-side repository for event retrieval.</param>
    public GetEventsByDateRangeQueryHandler(IEventQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    /// <inheritdoc />
    public Task<List<Event>> Handle(GetEventsByDateRangeQuery query)
        => _queryRepository.GetEventsByDateRangeAsync(query.StartDate, query.EndDate);
}
