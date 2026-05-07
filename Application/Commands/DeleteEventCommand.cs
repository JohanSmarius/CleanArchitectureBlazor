namespace Application.Commands;

/// <summary>
/// Command to delete the event with the specified identifier.
/// </summary>
public class DeleteEventCommand
{
    /// <summary>The unique identifier of the event to delete.</summary>
    public int Id { get; set; }
}
