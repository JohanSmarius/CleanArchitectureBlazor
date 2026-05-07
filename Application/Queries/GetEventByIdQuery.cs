namespace Application.Queries;

/// <summary>
/// Query to retrieve the event with the specified identifier.
/// </summary>
public class GetEventByIdQuery
{
    /// <summary>The unique identifier of the event to retrieve.</summary>
    public int Id { get; set; }
}
