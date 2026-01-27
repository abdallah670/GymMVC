namespace GymDAL.Repo.Abstract.Users
{
    public interface IMemberRepository : IRepository<Member>
    {

        Task<Member> GetByEmailAsync(string email);





    }
}
