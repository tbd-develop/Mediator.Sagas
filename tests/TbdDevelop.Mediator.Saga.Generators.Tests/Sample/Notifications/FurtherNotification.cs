using Mediator;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Notifications;

public class FurtherNotification : INotification, IOrchestratedNotification
{
    public Guid MyUseableIdentifier { get; set; }

    public string Message { get; set; } = null!;
    public bool IsAlert { get; set; }
    public Guid OrchestrationIdentifier => MyUseableIdentifier;
}