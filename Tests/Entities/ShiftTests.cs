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
        // Given & When
        var shift = new Shift();

        // Then
        Assert.Equal(ShiftStatus.Open, shift.Status);
    }

    [Fact]
    public void GivenNewShift_WhenCreated_ThenRequiredStaffIsOne()
    {
        // Given & When
        var shift = new Shift();

        // Then
        Assert.Equal(1, shift.RequiredStaff);
    }

    [Fact]
    public void GivenNewShift_WhenCreated_ThenStaffAssignmentsIsEmpty()
    {
        // Given & When
        var shift = new Shift();

        // Then
        Assert.Empty(shift.StaffAssignments);
    }

    [Fact]
    public void GivenNewShift_WhenCreated_ThenNameIsEmpty()
    {
        // Given & When
        var shift = new Shift();

        // Then
        Assert.Equal(string.Empty, shift.Name);
    }

    [Fact]
    public void GivenNewShift_WhenNameIsSet_ThenNameIsUpdated()
    {
        // Given
        var shift = new Shift();

        // When
        shift.Name = "Evening Shift";

        // Then
        Assert.Equal("Evening Shift", shift.Name);
    }

    [Fact]
    public void GivenNewShift_WhenCreated_ThenCreatedAtIsSet()
    {
        // Given
        var before = DateTime.UtcNow.AddSeconds(-1);

        // When
        var shift = new Shift();

        // Then
        Assert.True(shift.CreatedAt >= before);
    }

    [Fact]
    public void GivenNewShift_WhenCreated_ThenUpdatedAtIsNull()
    {
        // Given & When
        var shift = new Shift();

        // Then
        Assert.Null(shift.UpdatedAt);
    }
}
