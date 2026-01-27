using GymBLL.ModelVM.AI;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.AI
{
    public interface IAIService
    {
        Task<AIChatResponse> ProcessMessageAsync(string userId, string message);
    }
}
