namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Notifications;

public class CompleteNotification : INotification, IOrchestratedNotification
{
    public Guid OrchestrationIdentifier { get; set; }
}