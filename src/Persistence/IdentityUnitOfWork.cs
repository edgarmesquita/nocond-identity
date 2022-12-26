using eQuantic.Core.Data.EntityFramework.Repository;
using eQuantic.Core.Data.Repository;
using Lamar;

namespace NoCond.Identity.Persistence
{
    public class IdentityUnitOfWork : UnitOfWork, IIdentityUnitOfWork
    {
        private readonly IContainer container;
        private readonly IdentityContext dbContext;

        public IdentityUnitOfWork (IdentityContext dbContext, IContainer container) : base (dbContext)
        {
            this.dbContext = dbContext;
            this.container = container;
        }

        public override TRepository GetRepository<TRepository> ()
        {
            return container.GetInstance<TRepository> ();
        }

        public override TRepository GetRepository<TRepository> (string name)
        {
            return container.GetInstance<TRepository> (name);
        }

        IAsyncRelationalRepository<IIdentityUnitOfWork, TEntity, TKey> IIdentityUnitOfWork.GetAsyncRelationalRepository<TEntity, TKey> ()
        {
            return new AsyncRelationalRepository<IIdentityUnitOfWork, TEntity, TKey> (this);
        }

        IAsyncRepository<IIdentityUnitOfWork, TEntity, TKey> IIdentityUnitOfWork.GetAsyncRepository<TEntity, TKey> ()
        {
            return new AsyncRepository<IIdentityUnitOfWork, TEntity, TKey> (this);
        }

        IRelationalRepository<IIdentityUnitOfWork, TEntity, TKey> IIdentityUnitOfWork.GetRelationalRepository<TEntity, TKey> ()
        {
            return new RelationalRepository<IIdentityUnitOfWork, TEntity, TKey> (this);
        }

        IRepository<IIdentityUnitOfWork, TEntity, TKey> IIdentityUnitOfWork.GetRepository<TEntity, TKey> ()
        {
            return new Repository<IIdentityUnitOfWork, TEntity, TKey> (this);
        }
    }
}