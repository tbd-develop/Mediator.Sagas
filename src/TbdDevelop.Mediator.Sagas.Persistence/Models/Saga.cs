namespace TbdDevelop.Mediator.Sagas.Persistence.Models;

public class Saga
{
    public Guid OrchestrationIdentifier { get; set; }
    public required string TypeIdentifier { get; set; }
    public bool IsComplete { get; set; }
    public string State { get; set; } = null!;
    public DateTime? NextTriggerTime { get; set; }
    public TimeSpan? TriggerInterval { get; set; }
    public DateTime? LastTriggered { get; set; }
}