namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface IHandle<in TEvent>
    where TEvent : class
{
    void Handle(TEvent @event);
}