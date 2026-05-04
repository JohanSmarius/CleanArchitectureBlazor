using Entities;

namespace Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Staff"/> entity.
/// </summary>
public class StaffTests
{
    [Fact]
    public void GivenStaffWithFirstAndLastName_WhenFullNameAccessed_ThenReturnsCombinedName()
    {
        // Given
        var staff = new Staff { FirstName = "Jane", LastName = "Doe" };

        // When
        var fullName = staff.FullName;

        // Then
        Assert.Equal("Jane Doe", fullName);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenIsActiveIsTrue()
    {
        // Given & When
        var staff = new Staff();

        // Then
        Assert.True(staff.IsActive);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenStaffAssignmentsIsEmpty()
    {
        // Given & When
        var staff = new Staff();

        // Then
        Assert.Empty(staff.StaffAssignments);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenFirstNameIsEmpty()
    {
        // Given & When
        var staff = new Staff();

        // Then
        Assert.Equal(string.Empty, staff.FirstName);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenLastNameIsEmpty()
    {
        // Given & When
        var staff = new Staff();

        // Then
        Assert.Equal(string.Empty, staff.LastName);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenEmailIsEmpty()
    {
        // Given & When
        var staff = new Staff();

        // Then
        Assert.Equal(string.Empty, staff.Email);
    }

    [Fact]
    public void GivenNewStaff_WhenRoleIsSet_ThenRoleIsUpdated()
    {
        // Given
        var staff = new Staff();

        // When
        staff.Role = StaffRole.Paramedic;

        // Then
        Assert.Equal(StaffRole.Paramedic, staff.Role);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenCreatedAtIsSet()
    {
        // Given
        var before = DateTime.UtcNow.AddSeconds(-1);

        // When
        var staff = new Staff();

        // Then
        Assert.True(staff.CreatedAt >= before);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenUpdatedAtIsNull()
    {
        // Given & When
        var staff = new Staff();

        // Then
        Assert.Null(staff.UpdatedAt);
    }
}
