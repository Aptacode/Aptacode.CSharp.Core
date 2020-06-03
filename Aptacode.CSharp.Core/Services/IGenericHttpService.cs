using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aptacode.CSharp.Core.Services
{
    public interface IGenericHttpService<TGetViewModel, TPutViewModel>
    {
        Task<IEnumerable<TGetViewModel>> Get();
        Task<TGetViewModel> Get(int id);
        Task<TPutViewModel> Push(int id, TPutViewModel entity);
        Task Put(TPutViewModel entity);
        Task Delete(int id);
    }
}