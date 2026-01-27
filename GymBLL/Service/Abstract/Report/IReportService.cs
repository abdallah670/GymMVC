using GymBLL.Common;
using GymBLL.ModelVM.Report;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Report
{
    public interface IReportService
    {
        Task<Response<MemberReportVM>> GenerateMemberReportAsync(string memberId);
    }
}
