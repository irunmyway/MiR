using System.Threading.Tasks;

namespace Foundation
{
    public interface IOnServerError
    {
        Task<bool> ShouldRetry();
    }
}
