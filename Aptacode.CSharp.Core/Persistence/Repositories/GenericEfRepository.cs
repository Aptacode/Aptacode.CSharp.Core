using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aptacode.CSharp.Common.Persistence;
using Aptacode.CSharp.Common.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace Aptacode.CSharp.Core.Persistence.Repositories
{
    /// <summary>
    ///     A Generic Wrapper of EfCore's DBSet
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class GenericEfRepository<TKey, TEntity> : IGenericAsyncRepository<TKey, TEntity>
        where TEntity : class, IEntity<TKey>
    {
        public GenericEfRepository(DbSet<TEntity> dbSet)
        {
            DbSet = dbSet;
        }

        protected DbSet<TEntity> DbSet { get; }

        public virtual Task CreateAsync(TEntity entity)
        {
            DbSet.Add(entity);
            return Task.CompletedTask;
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            DbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync() =>
            await DbSet.ToListAsync().ConfigureAwait(false);

        public virtual async Task<TEntity> GetAsync(TKey id) => await DbSet.FindAsync(id).ConfigureAwait(false);

        public virtual async Task DeleteAsync(TKey id)
        {
            var entity = await GetAsync(id).ConfigureAwait(false);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
        }

        public void Create(TEntity entity) => DbSet.Add(entity);
        public void Update(TEntity entity) => DbSet.Update(entity);
        public IReadOnlyCollection<TEntity> GetAll() => DbSet.ToList();
        public TEntity Get(TKey id) => DbSet.Find(id);

        public void Delete(TKey id)
        {
            var entity = Get(id);
            DbSet.Remove(entity);
        }
    }
}