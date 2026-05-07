using Entities;

namespace Application;

/// <summary>
/// Combined event repository interface that exposes both the query side and the command side.
/// Use <see cref="IEventQueryRepository"/> or <see cref="IEventCommandRepository"/> directly
/// when only one side is required.
/// </summary>
public interface IEventRepository : IEventQueryRepository, IEventCommandRepository
{
}
