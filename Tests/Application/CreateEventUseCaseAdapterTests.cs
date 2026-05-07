using Application;
using Application.Commands;
using Application.DataAdapters;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="CreateEventUseCase"/>.
/// </summary>
public class CreateEventUseCaseAdapterTests
{
    [Fact]
    public async Task Execute_DelegatesToCreateEventCommandHandler()
    {
        var handlerMock = new Mock<ICreateEventCommandHandler>();
        var useCase = new CreateEventUseCase(handlerMock.Object);
        var dto = new EventDTO
        {
            Name = "Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Location"
        };

        handlerMock
            .Setup(h => h.Handle(It.IsAny<CreateEventCommand>()))
            .ReturnsAsync(dto);

        var result = await useCase.Execute(dto);

        Assert.Equal(dto.Name, result.Name);
        handlerMock.Verify(h => h.Handle(It.Is<CreateEventCommand>(c =>
            c.Name == dto.Name &&
            c.StartDate == dto.StartDate &&
            c.EndDate == dto.EndDate &&
            c.Location == dto.Location)), Times.Once);
    }
}
