namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface IOrchestratedNotification
{
    Guid OrchestrationIdentifier { get; }
}