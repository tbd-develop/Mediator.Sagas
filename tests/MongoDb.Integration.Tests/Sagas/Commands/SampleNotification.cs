using Mediator;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace MongoDb.Integration.Tests.Sagas.Commands;

public class SampleNotification(Guid orchestrationIdentifier) : INotification, IOrchestratedNotification
{
    public Guid OrchestrationIdentifier { get; } = orchestrationIdentifier;
}