using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Aptacode.NetCore.Persistence
{
    public abstract class GenericEfRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly DbSet<TEntity> DbSet;

        protected GenericEfRepository(DbSet<TEntity> dbSet)
        {
            DbSet = dbSet;
        }

        public virtual async Task<int> Create(TEntity entity)
        {
            DbSet.Add(entity);
            return 0;
        }

        public virtual async Task Update(TEntity entity)
        {
            DbSet.Attach(entity);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await DbSet.ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<TEntity> Get(int id)
        {
            return await DbSet.FindAsync(id).ConfigureAwait(false);
        }

        public virtual async Task Delete(int id)
        {
            var entity = await Get(id).ConfigureAwait(false);
            DbSet.Remove(entity);

        }

        public IQueryable<TEntity> AsQueryable()
        {
            return DbSet.AsQueryable();
        }
    }
}
