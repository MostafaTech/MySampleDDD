using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.Persistence
{
    public interface IUnitOfWork
    {
        IQueryable<TEntity> FindAll<TEntity>() where TEntity : class;
        TEntity FindById<TEntity>(Guid id) where TEntity : class;
        Task AddAsync<TEntity>(TEntity entry) where TEntity : class;
        Task UpdateAsync<TEntity>(TEntity entry) where TEntity : class;
        Task DeleteAsync<TEntity>(TEntity entry) where TEntity : class;

        Task SaveChangesAsync();

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        Task PurgeData(params string[] collections);
    }
}
