

using MenoBLL.ModelVM.AccountVM;

namespace MenoBLL.Mappper
{
    public class DomainProfile:Profile
    {
        public DomainProfile()
        {
            
            CreateMap<IdentityRole,RoleVM>().ReverseMap();
           
        }
    }
}
