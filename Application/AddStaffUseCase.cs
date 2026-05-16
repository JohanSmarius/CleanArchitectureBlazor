using Application.DataAdapters;
using Entities;

namespace Application;


/// <summary>
/// Adds a staff member through the external staff repository.
/// </summary>
public class AddStaffUseCase(IExternalStaffRepository externalStaffRepository) : IAddStaffUseCase
{
    /// <summary>
    /// Executes the add-staff use case for the provided staff member.
    /// </summary>
    /// <param name="staff">The staff member to create.</param>
    /// <returns>The created staff member from the external API.</returns>
    public Task<StaffDTO> ExecuteAsync(StaffDTO staff)
    {
        // Do additional validation here if needed, like validating the certification.
        
        
        return externalStaffRepository.AddStaffAsync(staff);
    }
}
