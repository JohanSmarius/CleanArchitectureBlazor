using Entities;

namespace Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Staff"/> entity.
/// </summary>
public class StaffTests
{
    [Fact]
    public void Staff_FullName_CombinesFirstAndLastName()
    {
        // Arrange
        var staff = new Staff { FirstName = "Jane", LastName = "Doe" };

        // Act
        var fullName = staff.FullName;

        // Assert
        Assert.Equal("Jane Doe", fullName);
    }

    [Fact]
    public void Staff_DefaultIsActive_IsTrue()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.True(staff.IsActive);
    }

    [Fact]
    public void Staff_DefaultStaffAssignments_IsEmptyList()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.Empty(staff.StaffAssignments);
    }

    [Fact]
    public void Staff_DefaultFirstName_IsEmptyString()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.Equal(string.Empty, staff.FirstName);
    }

    [Fact]
    public void Staff_DefaultLastName_IsEmptyString()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.Equal(string.Empty, staff.LastName);
    }

    [Fact]
    public void Staff_DefaultEmail_IsEmptyString()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.Equal(string.Empty, staff.Email);
    }

    [Fact]
    public void Staff_CanSetRole()
    {
        // Arrange
        var staff = new Staff();

        // Act
        staff.Role = StaffRole.Paramedic;

        // Assert
        Assert.Equal(StaffRole.Paramedic, staff.Role);
    }

    [Fact]
    public void Staff_CreatedAt_IsSetOnConstruction()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var staff = new Staff();

        // Assert
        Assert.True(staff.CreatedAt >= before);
    }

    [Fact]
    public void Staff_UpdatedAt_DefaultIsNull()
    {
        // Arrange & Act
        var staff = new Staff();

        // Assert
        Assert.Null(staff.UpdatedAt);
    }
}
