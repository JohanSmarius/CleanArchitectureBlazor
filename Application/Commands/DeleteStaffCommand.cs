namespace Application.Commands;

/// <summary>
/// Command to deactivate the staff member with the specified identifier.
/// </summary>
public class DeleteStaffCommand
{
    /// <summary>The unique identifier of the staff member to deactivate.</summary>
    public int Id { get; set; }
}
