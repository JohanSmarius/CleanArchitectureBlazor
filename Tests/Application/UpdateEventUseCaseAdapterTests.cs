using Application;
using Application.Commands;
using Application.DataAdapters;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="UpdateEventUseCase"/>.
/// </summary>
public class UpdateEventUseCaseAdapterTests
{
    [Fact]
    public async Task Execute_DelegatesToUpdateEventCommandHandler()
    {
        // Arrange
        var handlerMock = new Mock<IUpdateEventCommandHandler>();
        var useCase = new UpdateEventUseCase(handlerMock.Object);
        var dto = new EventDTO
        {
            Id = 123,
            Name = "Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Location"
        };

        handlerMock
            .Setup(h => h.Handle(It.IsAny<UpdateEventCommand>()))
            .ReturnsAsync(dto);

        // Act
        var result = await useCase.Execute(dto);

        // Assert
        Assert.Equal(dto.Id, result.Id);
        handlerMock.Verify(h => h.Handle(It.Is<UpdateEventCommand>(c =>
            c.Id == dto.Id &&
            c.Name == dto.Name &&
            c.StartDate == dto.StartDate &&
            c.EndDate == dto.EndDate &&
            c.Location == dto.Location)), Times.Once);
    }
}
