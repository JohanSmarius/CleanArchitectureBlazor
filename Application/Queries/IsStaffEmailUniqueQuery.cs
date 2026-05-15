namespace Application.Queries;

/// <summary>
/// Query to determine whether a staff email address is unique.
/// </summary>
public class IsStaffEmailUniqueQuery
{
    /// <summary>The email address to evaluate.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Optional staff identifier to exclude when checking uniqueness.
    /// </summary>
    public int? ExcludeId { get; set; }
}
