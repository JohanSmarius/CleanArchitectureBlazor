using Entities;

namespace Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Shift"/> entity.
/// </summary>
public class ShiftTests
{
    [Fact]
    public void Shift_DefaultStatus_IsOpen()
    {
        // Arrange & Act
        var shift = new Shift();

        // Assert
        Assert.Equal(ShiftStatus.Open, shift.Status);
    }

    [Fact]
    public void Shift_DefaultRequiredStaff_IsOne()
    {
        // Arrange & Act
        var shift = new Shift();

        // Assert
        Assert.Equal(1, shift.RequiredStaff);
    }

    [Fact]
    public void Shift_DefaultStaffAssignments_IsEmptyList()
    {
        // Arrange & Act
        var shift = new Shift();

        // Assert
        Assert.Empty(shift.StaffAssignments);
    }

    [Fact]
    public void Shift_DefaultName_IsEmptyString()
    {
        // Arrange & Act
        var shift = new Shift();

        // Assert
        Assert.Equal(string.Empty, shift.Name);
    }

    [Fact]
    public void Shift_CanSetName()
    {
        // Arrange
        var shift = new Shift();

        // Act
        shift.Name = "Evening Shift";

        // Assert
        Assert.Equal("Evening Shift", shift.Name);
    }

    [Fact]
    public void Shift_CreatedAt_IsSetOnConstruction()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var shift = new Shift();

        // Assert
        Assert.True(shift.CreatedAt >= before);
    }

    [Fact]
    public void Shift_UpdatedAt_DefaultIsNull()
    {
        // Arrange & Act
        var shift = new Shift();

        // Assert
        Assert.Null(shift.UpdatedAt);
    }
}
