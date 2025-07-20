using TbdDevelop.Mediator.Sagas.Contracts;

namespace Integration.Base.Sagas.Sample;

public class SampleNotification(Guid orchestrationIdentifier) : IOrchestratedNotification
{
    public int Id { get; set; }
    public Guid OrchestrationIdentifier { get; } = orchestrationIdentifier;
}