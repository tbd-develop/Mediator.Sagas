namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface ISpecification
{
    IQueryable<object> Execute(IQueryable<object> query);
}