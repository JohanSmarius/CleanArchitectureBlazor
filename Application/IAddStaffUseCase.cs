using Application.DataAdapters;
using Entities;

namespace Application;

/// <summary>
/// Use case for adding a new staff member.
/// </summary>
public interface IAddStaffUseCase
{
    Task<StaffDTO> ExecuteAsync(StaffDTO staff);
}
