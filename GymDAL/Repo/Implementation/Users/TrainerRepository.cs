


using GymDAL.Repo.Abstract.Users;

namespace GymDAL.Repo.Implementation
{
    public class TrainerRepository : Repository<Trainer>, ITrainerRepository
    {
        IApplicationUserRepository _applicationUserRepository;
        GymDbContext gymDbContext;
        
        public TrainerRepository(GymDbContext context, IApplicationUserRepository applicationUserRepository, IMapper mapper) : base(context,mapper)
        {
            _applicationUserRepository = applicationUserRepository;
          
        }

        public Task<Trainer> GetTrainerWithWorkoutPlansAsync(int trainerId)
        {
            throw new NotImplementedException();
        }

        public Task<Trainer> GetTrainerWithDietPlansAsync(int trainerId)
        {
            throw new NotImplementedException();
        }

        public Task<Trainer> GetTrainerWithAvailabilitiesAsync(int trainerId)
        {
            throw new NotImplementedException();
        }

        public Task<Trainer> GetTrainerWithTrainingSessionsAsync(int trainerId)
        {
            throw new NotImplementedException();
        }

        public Task<Trainer> GetTrainerWithMembershipsAsync(int trainerId)
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
                var existingUser = gymDbContext.Members.Find(user.Id);
                if (existingUser != null)
                {
                    _mapper.Map(user, existingUser);
                    gymDbContext.Members.Update(existingUser);
                    gymDbContext.SaveChanges();
                    return Task.FromResult<Member>(existingUser);
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
        public Task<bool> ToggleStatusUserAsync(string userId, string DeletedBy)
        {
            try
            {
                var existingUser = gymDbContext.Members.Find(userId);
                if (existingUser != null)
                {
                    existingUser.IsActive = !existingUser.IsActive;
                    gymDbContext.Members.Update(existingUser);
                    gymDbContext.SaveChanges();
                    return Task.FromResult<bool>(true);
                }
                else
                {
                    return Task.FromResult<bool>(false);
                }
            }
            catch (Exception)
            {
                return Task.FromResult<bool>(false);
            }

        }
    }
}