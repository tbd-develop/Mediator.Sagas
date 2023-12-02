namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface IAmStartedBy<in TEvent> : IHandle<TEvent>
    where TEvent : class
{
}