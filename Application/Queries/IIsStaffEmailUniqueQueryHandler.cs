namespace Application.Queries;

/// <summary>
/// Handles the <see cref="IsStaffEmailUniqueQuery"/>.
/// </summary>
public interface IIsStaffEmailUniqueQueryHandler
{
    /// <summary>
    /// Returns <c>true</c> when the email is unique for staff records.
    /// </summary>
    Task<bool> Handle(IsStaffEmailUniqueQuery query);
}
