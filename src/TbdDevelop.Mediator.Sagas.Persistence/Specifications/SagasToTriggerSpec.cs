using TbdDevelop.Mediator.Sagas.Infrastructure;
using TbdDevelop.Mediator.Sagas.Persistence.Models;

namespace TbdDevelop.Mediator.Sagas.Persistence.Specifications;

public sealed class SagasToTriggerSpec : Specification<Saga>
{
    public SagasToTriggerSpec(int withinMinutes)
    {
        var minutesBeforeNow = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(withinMinutes));

        Query = e => e.NextTriggerTime != null && e.NextTriggerTime >= minutesBeforeNow &&
                     (e.LastTriggered != null || e.LastTriggered < minutesBeforeNow);
    }
}