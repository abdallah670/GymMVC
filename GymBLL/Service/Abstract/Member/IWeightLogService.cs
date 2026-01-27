using GymBLL.Common;
using GymBLL.ModelVM.Member;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Member
{
    public interface IWeightLogService
    {
        Task<Response<bool>> LogWeightAsync(WeightLogVM model);
        Task<Response<IEnumerable<WeightLogVM>>> GetHistoryAsync(string memberId);
    }
}
