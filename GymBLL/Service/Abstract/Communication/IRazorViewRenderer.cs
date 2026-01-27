using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Communication
{
    public interface IRazorViewRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}
