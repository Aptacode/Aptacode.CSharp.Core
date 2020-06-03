using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aptacode.CSharp.Core.Services.Interfaces
{
    public interface IGenericHttpService<TGetViewModel, TPutViewModel>
    {
        Task<IEnumerable<TGetViewModel>> Get();
        Task<TGetViewModel> Get(int id);
        Task<TGetViewModel> Push(int id, TPutViewModel entity);
        Task<TGetViewModel> Put(TPutViewModel entity);
        Task<bool> Delete(int id);
    }
}