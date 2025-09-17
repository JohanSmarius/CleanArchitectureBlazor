using Entities;
using CleanArchitectureBlazor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class StaffAssignmentRepository : IStaffAssignmentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<StaffAssignmentRepository> _logger;

    public StaffAssignmentRepository(ApplicationDbContext context, IEmailService emailService, ILogger<StaffAssignmentRepository> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<List<StaffAssignment>> GetAllAssignmentsAsync()
    {
        return await _context.StaffAssignments
            .Include(sa => sa.Staff)
            .Include(sa => sa.Shift)
            .ThenInclude(s => s.Event)
            .OrderBy(sa => sa.Shift.StartTime)
            .ToListAsync();
    }

    public async Task<StaffAssignment?> GetAssignmentByIdAsync(int id)
    {
        return await _context.StaffAssignments
            .Include(sa => sa.Staff)
            .Include(sa => sa.Shift)
            .ThenInclude(s => s.Event)
            .FirstOrDefaultAsync(sa => sa.Id == id);
    }

    public async Task<List<StaffAssignment>> GetAssignmentsByShiftIdAsync(int shiftId)
    {
        return await _context.StaffAssignments
            .Include(sa => sa.Staff)
            .Where(sa => sa.ShiftId == shiftId)
            .OrderBy(sa => sa.Staff.LastName)
            .ThenBy(sa => sa.Staff.FirstName)
            .ToListAsync();
    }

    public async Task<List<StaffAssignment>> GetAssignmentsByStaffIdAsync(int staffId)
    {
        return await _context.StaffAssignments
            .Include(sa => sa.Shift)
            .ThenInclude(s => s.Event)
            .Where(sa => sa.StaffId == staffId)
            .OrderBy(sa => sa.Shift.StartTime)
            .ToListAsync();
    }

    public async Task<StaffAssignment> CreateAssignmentAsync(StaffAssignment assignment)
    {
        assignment.AssignedAt = DateTime.UtcNow;
        _context.StaffAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Load the full assignment with related data for email notification
        var fullAssignment = await GetAssignmentByIdAsync(assignment.Id);
        if (fullAssignment?.Staff != null && fullAssignment.Shift?.Event != null)
        {
            try
            {
                // Send email notification to the assigned staff member
                await _emailService.SendStaffAssignmentNotificationAsync(
                    fullAssignment.Staff, 
                    fullAssignment.Shift, 
                    fullAssignment.Shift.Event);

                _logger.LogInformation("Assignment notification email sent to {Email} for shift {ShiftName}", 
                    fullAssignment.Staff.Email, fullAssignment.Shift.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send assignment notification email to {Email} for shift {ShiftName}", 
                    fullAssignment.Staff?.Email, fullAssignment.Shift?.Name);
                // Don't throw - we don't want email failures to prevent assignment creation
            }
        }

        return assignment;
    }

    public async Task<StaffAssignment> UpdateAssignmentAsync(StaffAssignment assignment)
    {
        assignment.UpdatedAt = DateTime.UtcNow;
        _context.StaffAssignments.Update(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task DeleteAssignmentAsync(int id)
    {
        var assignment = await _context.StaffAssignments.FindAsync(id);
        if (assignment != null)
        {
            _context.StaffAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<StaffAssignment?> CheckInStaffAsync(int assignmentId)
    {
        var assignment = await GetAssignmentByIdAsync(assignmentId);
        if (assignment != null)
        {
            assignment.CheckInTime = DateTime.UtcNow;
            assignment.Status = AssignmentStatus.CheckedIn;
            await UpdateAssignmentAsync(assignment);
        }
        return assignment;
    }

    public async Task<StaffAssignment?> CheckOutStaffAsync(int assignmentId)
    {
        var assignment = await GetAssignmentByIdAsync(assignmentId);
        if (assignment != null)
        {
            assignment.CheckOutTime = DateTime.UtcNow;
            assignment.Status = AssignmentStatus.CheckedOut;
            await UpdateAssignmentAsync(assignment);
        }
        return assignment;
    }

    public async Task<bool> IsStaffAvailableAsync(int staffId, DateTime startTime, DateTime endTime, int? excludeAssignmentId = null)
    {
        var query = _context.StaffAssignments
            .Include(sa => sa.Shift)
            .Where(sa => sa.StaffId == staffId &&
                        sa.Status != AssignmentStatus.Cancelled &&
                        sa.Shift.StartTime < endTime &&
                        sa.Shift.EndTime > startTime);

        if (excludeAssignmentId.HasValue)
        {
            query = query.Where(sa => sa.Id != excludeAssignmentId.Value);
        }

        return !await query.AnyAsync();
    }
}
