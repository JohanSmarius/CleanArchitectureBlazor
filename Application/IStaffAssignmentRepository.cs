using Entities;

namespace Application;

/// <summary>
/// Service for managing staff assignments
/// </summary>
public interface IStaffAssignmentRepository
{
    Task<List<StaffAssignment>> GetAllAssignmentsAsync();
    Task<StaffAssignment?> GetAssignmentByIdAsync(int id);
    Task<List<StaffAssignment>> GetAssignmentsByShiftIdAsync(int shiftId);
    Task<List<StaffAssignment>> GetAssignmentsByStaffIdAsync(int staffId);
    Task<StaffAssignment> CreateAssignmentAsync(StaffAssignment assignment);
    Task<StaffAssignment> UpdateAssignmentAsync(StaffAssignment assignment);
    Task DeleteAssignmentAsync(int id);
    Task<StaffAssignment?> CheckInStaffAsync(int assignmentId);
    Task<StaffAssignment?> CheckOutStaffAsync(int assignmentId);
    Task<bool> IsStaffAvailableAsync(int staffId, DateTime startTime, DateTime endTime, int? excludeAssignmentId = null);
}
