using System.Threading.Tasks;

namespace Aptacode.CSharp.Core.Http.Controllers
{
    public delegate Task<ServerResponse<bool>> Validator<T>(T input);

    public delegate Task<ServerResponse<bool>> Validator();
}