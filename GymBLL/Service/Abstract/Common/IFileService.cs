using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Common
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        void DeleteFile(string fileName, string folderName);
    }
}
