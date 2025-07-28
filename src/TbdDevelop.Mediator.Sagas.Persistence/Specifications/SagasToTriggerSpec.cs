using TbdDevelop.Mediator.Sagas.Infrastructure;
using TbdDevelop.Mediator.Sagas.Persistence.Models;

namespace TbdDevelop.Mediator.Sagas.Persistence.Specifications;

public sealed class SagasToTriggerSpec : Specification<Saga>
{
    public SagasToTriggerSpec(int withinMs)
    {
        var now = DateTime.UtcNow;
        var msBeforeNow = now.Subtract(TimeSpan.FromMilliseconds(withinMs));

        Query = e => (e.NextTriggerTime != null && e.LastTriggered == null && e.NextTriggerTime <= msBeforeNow) ||
                     (e.NextTriggerTime != null && e.LastTriggered != null && e.NextTriggerTime >= msBeforeNow &&
                      e.NextTriggerTime <= now && e.LastTriggered <= msBeforeNow);
    }
}