using Entities;

namespace Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="StaffAssignment"/> entity.
/// </summary>
public class StaffAssignmentTests
{
    [Fact]
    public void StaffAssignment_DefaultStatus_IsAssigned()
    {
        // Arrange & Act
        var assignment = new StaffAssignment();

        // Assert
        Assert.Equal(AssignmentStatus.Assigned, assignment.Status);
    }

    [Fact]
    public void StaffAssignment_DefaultCheckInTime_IsNull()
    {
        // Arrange & Act
        var assignment = new StaffAssignment();

        // Assert
        Assert.Null(assignment.CheckInTime);
    }

    [Fact]
    public void StaffAssignment_DefaultCheckOutTime_IsNull()
    {
        // Arrange & Act
        var assignment = new StaffAssignment();

        // Assert
        Assert.Null(assignment.CheckOutTime);
    }

    [Fact]
    public void StaffAssignment_DefaultNotes_IsNull()
    {
        // Arrange & Act
        var assignment = new StaffAssignment();

        // Assert
        Assert.Null(assignment.Notes);
    }

    [Fact]
    public void StaffAssignment_AssignedAt_IsSetOnConstruction()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var assignment = new StaffAssignment();

        // Assert
        Assert.True(assignment.AssignedAt >= before);
    }

    [Fact]
    public void StaffAssignment_UpdatedAt_DefaultIsNull()
    {
        // Arrange & Act
        var assignment = new StaffAssignment();

        // Assert
        Assert.Null(assignment.UpdatedAt);
    }

    [Fact]
    public void StaffAssignment_CanSetStatus_ToCheckedIn()
    {
        // Arrange
        var assignment = new StaffAssignment();

        // Act
        assignment.Status = AssignmentStatus.CheckedIn;

        // Assert
        Assert.Equal(AssignmentStatus.CheckedIn, assignment.Status);
    }
}
