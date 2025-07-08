using TbdDevelop.Mediator.Sagas;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace SqlServer.Integration.Tests.Data;

public class SampleSaga : Saga<SimpleState>
{
    public SampleSaga(Guid orchestrationIdentifier)
        : base(orchestrationIdentifier)
    {
    }

    public override bool IsComplete { get; }
}

public class SimpleState
{
    public string Value { get; set; } = null!;
}