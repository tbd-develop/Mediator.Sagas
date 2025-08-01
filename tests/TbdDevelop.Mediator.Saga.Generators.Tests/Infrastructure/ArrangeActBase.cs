namespace TbdDevelop.Mediator.Saga.Generators.Tests.Infrastructure;

public abstract class ArrangeActBase : IAsyncLifetime
{
    protected virtual ValueTask ArrangeAsync()
    {
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask ActAsync()
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask InitializeAsync()
    {
        await ArrangeAsync();

        await ActAsync();
    }
}