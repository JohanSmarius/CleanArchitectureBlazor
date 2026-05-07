namespace Application.Queries;

/// <summary>
/// Query to retrieve events whose start date falls within the specified range.
/// </summary>
public class GetEventsByDateRangeQuery
{
    /// <summary>Inclusive lower bound of the date range.</summary>
    public DateTime StartDate { get; set; }

    /// <summary>Inclusive upper bound of the date range.</summary>
    public DateTime EndDate { get; set; }
}
