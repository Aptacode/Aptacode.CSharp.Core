using System.Collections.Generic;
using System.Threading.Tasks;
using Aptacode.CSharp.Utilities.Persistence;

namespace Aptacode.CSharp.Core.Services
{
    public interface IGenericHttpService<TEntity> where TEntity : IEntity
    {
        Task<IEnumerable<TEntity>> Get();
        Task<IEnumerable<TEntity>> Get(int id);
        Task<TEntity> Push(TEntity entity);
        Task Put(TEntity entity);
        Task Delete(int id);
    }
}