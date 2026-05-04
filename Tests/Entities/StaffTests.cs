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
        // Arrange
        var staff = new Staff { FirstName = "Jane", LastName = "Doe" };

        // Act
        var fullName = staff.FullName;

        // Assert
        Assert.Equal("Jane Doe", fullName);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenIsActiveIsTrue()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.True(staff.IsActive);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenStaffAssignmentsIsEmpty()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.Empty(staff.StaffAssignments);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenFirstNameIsEmpty()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.Equal(string.Empty, staff.FirstName);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenLastNameIsEmpty()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.Equal(string.Empty, staff.LastName);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenEmailIsEmpty()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.Equal(string.Empty, staff.Email);
    }

    [Fact]
    public void GivenNewStaff_WhenRoleIsSet_ThenRoleIsUpdated()
    {
        // Arrange
        var staff = new Staff();

        // Act
        staff.Role = StaffRole.Paramedic;

        // Assert
        Assert.Equal(StaffRole.Paramedic, staff.Role);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenCreatedAtIsSet()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var staff = new Staff();

        // Assert
        Assert.True(staff.CreatedAt >= before);
    }

    [Fact]
    public void GivenNewStaff_WhenCreated_ThenUpdatedAtIsNull()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.Null(staff.UpdatedAt);
    }
}
