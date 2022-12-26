using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Sql;

namespace NoCond.Identity.Persistence
{
    public interface IIdentityUnitOfWork : IQueryableUnitOfWork
    {
        IAsyncRelationalRepository<IIdentityUnitOfWork, TEntity, TKey> GetAsyncRelationalRepository<TEntity, TKey> () where TEntity : class, IEntity, new ();

        IAsyncRepository<IIdentityUnitOfWork, TEntity, TKey> GetAsyncRepository<TEntity, TKey> () where TEntity : class, IEntity, new ();

        IRelationalRepository<IIdentityUnitOfWork, TEntity, TKey> GetRelationalRepository<TEntity, TKey> () where TEntity : class, IEntity, new ();

        IRepository<IIdentityUnitOfWork, TEntity, TKey> GetRepository<TEntity, TKey> () where TEntity : class, IEntity, new ();
    }
}