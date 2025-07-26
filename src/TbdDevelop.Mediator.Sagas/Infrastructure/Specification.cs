using System.Linq.Expressions;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas.Infrastructure;

public abstract class Specification<TEntity> : ISpecification
    where TEntity : class
{
    protected Expression<Func<TEntity, bool>> Query { get; set; } = null!;

    public IQueryable<TEntity> Execute(IQueryable<TEntity> query)
    {
        return query.Where(Query);
    }

    IQueryable<object> ISpecification.Execute(IQueryable<object> query)
    {
        return Execute((IQueryable<TEntity>)query);
    }
}