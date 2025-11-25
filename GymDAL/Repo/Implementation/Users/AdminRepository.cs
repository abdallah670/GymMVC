
using GymDAL.Repo.Abstract.Users;

namespace GymDAL.Repo.Implementation
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        IApplicationUserRepository _applicationUserRepository;
        GymDbContext gymDbContext;
     
        public AdminRepository(GymDbContext context,IApplicationUserRepository applicationUserRepository,IMapper mapper) : base(context,mapper)
        {
            _applicationUserRepository = applicationUserRepository;
            gymDbContext = context;
           
        }

        public Task<Admin> CreateAsync(Admin user, string password, string Createdby)
        {
            try
            {
                var userCreated =_applicationUserRepository.CreateUserAsync(user, password, Createdby);
                if (userCreated != null)
                {
                    user.Id = userCreated.Result.Id;
                    var result = gymDbContext.Admins.Add(user);
                    gymDbContext.SaveChanges();
                    if(result.Entity.Id != null)
                        return Task.FromResult<Admin>(userCreated.Result as Admin);
                    else
                        return Task.FromResult<Admin>(null);
                }
                else
                {
                    return Task.FromResult<Admin>(null);
                }

            }
            catch (Exception)
            {
                return Task.FromResult<Admin>(null);
            }
        }

        public Task<IEnumerable<Admin>> GetAsync(Expression<Func<Admin, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = gymDbContext.Admins.Where(Filter).AsEnumerable();
                    return Task.FromResult<IEnumerable<Admin>>(result);
                }
                else
                {
                    var result = gymDbContext.Admins.AsEnumerable();
                    return Task.FromResult<IEnumerable<Admin>>(result);
                }
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<Admin>>(null);
            }
        }

        public Task<Admin> GetByAsync(Expression<Func<Admin, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = gymDbContext.Admins.FirstOrDefault(Filter);
                    return Task.FromResult<Admin>(result);
                }
                else
                {
                    return Task.FromResult<Admin>(null);
                }
            }
            catch (Exception)
            {
                return Task.FromResult<Admin>(null);
            }
        }

        public Task<Admin> GetWithNotificationsAsync(string userId)
        {
            try
            {
                var result = gymDbContext.Admins
                     .Where(u => u.Id == userId)
                    .Select(u => new Admin
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        // Copy other properties as needed
                        Notifications = u.Notifications
                    }).FirstOrDefault();
                return Task.FromResult<Admin>(result);
            }
            catch (Exception)
            {
                return Task.FromResult<Admin>(null);
            }

        }
        public Task<Admin> GetWithReportsAsync(string userId)
        {
            try
            {
                var result = gymDbContext.Admins.Where(u => u.Id == userId)
                    .Select(u => new Admin
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        // Copy other properties as needed
                        GeneratedReports = u.GeneratedReports
                    }).FirstOrDefault();
                return Task.FromResult<Admin>(result);
            }
            catch (Exception)
            {
                return Task.FromResult<Admin>(null);
            }
        }

        public Task<Admin> GetWithSystemLogsAsync(string userId)
        {
            try
            {
                var result = gymDbContext.Admins.Where(u => u.Id == userId)
                    .Select(u => new Admin
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        // Copy other properties as needed
                        SystemLogs = u.SystemLogs
                    }).FirstOrDefault();
                return Task.FromResult<Admin>(result);
            }
            catch (Exception)
            {
                return Task.FromResult<Admin>(null);
            }
        }
        public Task<bool> ToggleStatusUserAsync(string userId, string currentSuperAdminId)
        {
            try
            {
                var admin = gymDbContext.Admins.Find(userId);
                if (admin != null)
                {
                   if(admin.ToggleStatus(currentSuperAdminId))
                    {
                        return Task.FromResult<bool>(true);
                    }
                   
                }
                return Task.FromResult<bool>(false);    
            }
            catch
            {
                return Task.FromResult<bool>(false);
            }
        }

        public Task<Admin> UpdateAsync(Admin user, string UpdatedBy)
        {
            try
            {
                var existingAdmin = gymDbContext.Admins.Find(user.Id);
                if (existingAdmin != null)
                {
                    // Update properties using AutoMapper
                    _mapper.Map(user, existingAdmin);

                    // Ensure audit fields are properly set
                    existingAdmin.UpdatedBy = UpdatedBy;
                    existingAdmin.UpdatedAt = DateTime.UtcNow;

                    gymDbContext.Admins.Update(existingAdmin);
                    gymDbContext.SaveChanges();

                    return Task.FromResult(existingAdmin);
                }
                return Task.FromResult<Admin>(null);
            }
            catch
            {
                return Task.FromResult<Admin>(null);
            }
        }
    }
}