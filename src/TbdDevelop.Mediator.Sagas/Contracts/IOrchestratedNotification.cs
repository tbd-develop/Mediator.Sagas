namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface IOrchestratedNotification
{
    Func<IOrchestratedNotification, Guid> OrchestrationIdentifier { get; }
}