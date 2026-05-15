using Domain;

namespace Application;

/// <summary>
/// Use case for adding a new staff member.
/// </summary>
public interface IAddStaffUseCase
{
    Task<Staff> ExecuteAsync(Staff staff);
}
