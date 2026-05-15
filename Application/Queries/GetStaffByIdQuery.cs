namespace Application.Queries;

/// <summary>
/// Query to retrieve the staff member with the specified identifier.
/// </summary>
public class GetStaffByIdQuery
{
    /// <summary>The unique identifier of the staff member to retrieve.</summary>
    public int Id { get; set; }
}
