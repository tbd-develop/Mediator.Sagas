using Mediator;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Notifications;

public class SampleNotification : INotification, IOrchestratedNotification
{
    public string Name { get; set; }
    public DateTime DateRegistered { get; set; }
    public Guid MyUseableIdentifier { get; set; }

    public Guid OrchestrationIdentifier => MyUseableIdentifier;
}