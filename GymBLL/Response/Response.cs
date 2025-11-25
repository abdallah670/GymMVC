

namespace MenoBLL.Response
{
    public record Response<T>(T Result,string?ErrorMessage,bool ISHaveErrorOrnNot);
   
}
