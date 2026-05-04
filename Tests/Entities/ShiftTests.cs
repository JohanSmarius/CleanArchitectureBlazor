using Entities;

namespace Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Shift"/> entity.
/// </summary>
public class ShiftTests
{
    [Fact]
    public void GivenNewShift_WhenCreated_ThenStatusIsOpen()
    {
        // Arrange & Act
        var shift = new Shift();

        // Assert
        Assert.Equal(ShiftStatus.Open, shift.Status);
    }

    [Fact]
    public void GivenNewShift_WhenCreated_ThenRequiredStaffIsOne()
    {
        // Arrange & Act
        var shift = new Shift();

        // Assert
        Assert.Equal(1, shift.RequiredStaff);
    }

    [Fact]
    public void GivenNewShift_WhenCreated_ThenStaffAssignmentsIsEmpty()
    {
        // Arrange & Act
        var shift = new Shift();

        // Assert
        Assert.Empty(shift.StaffAssignments);
    }

    [Fact]
    public void GivenNewShift_WhenCreated_ThenNameIsEmpty()
    {
        // Arrange & Act
        var shift = new Shift();

        // Assert
        Assert.Equal(string.Empty, shift.Name);
    }

    [Fact]
    public void GivenNewShift_WhenNameIsSet_ThenNameIsUpdated()
    {
        // Arrange
        var shift = new Shift();

        // Act
        shift.Name = "Evening Shift";

        // Assert
        Assert.Equal("Evening Shift", shift.Name);
    }

    [Fact]
    public void GivenNewShift_WhenCreated_ThenCreatedAtIsSet()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var shift = new Shift();

        // Assert
        Assert.True(shift.CreatedAt >= before);
    }

    [Fact]
    public void GivenNewShift_WhenCreated_ThenUpdatedAtIsNull()
    {
        // Arrange & Act
        var shift = new Shift();

        // Assert
        Assert.Null(shift.UpdatedAt);
    }
}
