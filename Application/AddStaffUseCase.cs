using Domain;

namespace Application;

/// <summary>
/// Adds a staff member through the external staff repository.
/// </summary>
public class AddStaffUseCase(IExternalStaffRepository externalStaffRepository) : IAddStaffUseCase
{
    public Task<Staff> ExecuteAsync(Staff staff)
    {
        return externalStaffRepository.AddStaffAsync(staff);
    }
}
