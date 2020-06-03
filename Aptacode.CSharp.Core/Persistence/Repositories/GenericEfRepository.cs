using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace Aptacode.CSharp.Core.Persistence.Repositories
{
    public class GenericEfRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        public GenericEfRepository(DbSet<TEntity> dbSet)
        {
            DbSet = dbSet;
        }

        protected DbSet<TEntity> DbSet { get; }

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
            if (entity != null) DbSet.Remove(entity);
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return DbSet.AsQueryable();
        }
    }
}