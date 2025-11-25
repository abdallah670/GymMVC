





using GymDAL.Repo.Abstract.Users;

namespace GymDAL.Repo.Implementation
{
    public class MemberRepository : Repository<Member>, IMemberRepository
    {
        IApplicationUserRepository _applicationUserRepository;
        GymDbContext gymDbContext;
        public MemberRepository(GymDbContext context, IApplicationUserRepository applicationUserRepository,IMapper mapper) : base(context,mapper)
        {
            _applicationUserRepository = applicationUserRepository;
            gymDbContext = context;
        }

        public Task<IEnumerable<Member>> GetMembersWithWorkoutPlansAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Member>> GetMembersWithDietPlansAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Member>> GetMemberWithProgressLogsAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Member>> GetMemberWithDietAssignmentsAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Member>> GetMemberWithPaymentsAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Member>> GetMemberWithMembershipsAsync(int memberId)
        {
            throw new NotImplementedException();
        }

     

        public Task<Member> GetByAsync(Expression<Func<Member, bool>>? Filter)
        {
            try
            {

                if (Filter != null)
                {
                    var result = gymDbContext.Members.FirstOrDefault(Filter);
                    return Task.FromResult<Member>(result);
                }
                else
                {
                    return Task.FromResult<Member>(null);
                }
            }
            catch (Exception)
            {
                return Task.FromResult<Member>(null);
            }
        }

        public Task<IEnumerable<Member>> GetAsync(Expression<Func<Member, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = gymDbContext.Members.Where(Filter).AsEnumerable();
                    return Task.FromResult<IEnumerable<Member>>(result);
                }
                else
                {
                    var result = gymDbContext.Members.AsEnumerable();
                    return Task.FromResult<IEnumerable<Member>>(result);
                }
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<Member>>(null);
            }
        }

        public Task<Member> GetWithReportsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Member> GetWithNotificationsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Member> GetWithSystemLogsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Member> CreateAsync(Member user, string password, string Createdby)
        {
            try
            {
                var userCreated = _applicationUserRepository.CreateUserAsync(user, password, Createdby);
                if (userCreated != null)
                {
                    user.Id = userCreated.Result.Id;
                    var result = gymDbContext.Members.Add(user);
                    gymDbContext.SaveChanges();
                    if (result.Entity.Id != null)
                        return Task.FromResult<Member>(userCreated.Result as Member);
                    else
                        return Task.FromResult<Member>(null);
                }
                else
                {
                    return Task.FromResult<Member>(null);
                }

            }
            catch (Exception)
            {
                return Task.FromResult<Member>(null);
            }
        }

        public Task<Member> UpdateAsync(Member user, string UpdatedBy)
        {
            try
            {
                var existingMember = gymDbContext.Members.Find(user.Id);
                if (existingMember  != null)
                {
                    // Update properties using AutoMapper
                    _mapper.Map(user, existingMember);

                    // Ensure audit fields are properly set
                    existingMember.UpdatedBy = UpdatedBy;
                    existingMember.UpdatedAt = DateTime.UtcNow;

                    gymDbContext.Members.Update(existingMember);
                    gymDbContext.SaveChanges();

                    return Task.FromResult(existingMember);
                }
                return Task.FromResult<Member>(null);
            }
            catch
            {
                return Task.FromResult<Member>(null);
            }
        }

        public Task<bool> ToggleStatusUserAsync(string userId, string DeletedBy)
        {
            try
            {
                var member = gymDbContext.Members.Find(userId);
                if (member != null)
                {
                    if (member.ToggleStatus(DeletedBy))
                    {
                        return Task.FromResult<bool>(true);
                    }

                }
                return Task.FromResult<bool>(false);
            }
            
            catch (Exception)
            {
                return Task.FromResult<bool>(false);
            }
        }
    }
}