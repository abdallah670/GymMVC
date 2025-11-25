using AutoMapper;
using GymDAL.Repo.Abstract.Nutrition;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class DietPlanAssignmentRepository : Repository<DietPlanAssignment>, IDietPlanAssignmentRepository
    {
        public DietPlanAssignmentRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<DietPlanAssignment> CreateAsync(DietPlanAssignment assignment, string Createdby)
        {
            try
            {
                assignment.CreatedBy = Createdby;
                assignment.CreatedAt = DateTime.UtcNow;
                _context.DietPlanAssignments.Add(assignment);
                _context.SaveChanges();
                return Task.FromResult(assignment);
            }
            catch (Exception)
            {
                return Task.FromResult<DietPlanAssignment>(null);
            }
        }

        public Task<IEnumerable<DietPlanAssignment>> GetAsync(Expression<Func<DietPlanAssignment, bool>>? Filter)
        {
            try
            {
                var query = _context.DietPlanAssignments.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<DietPlanAssignment>>(null);
            }
        }

        public Task<DietPlanAssignment> GetByAsync(Expression<Func<DietPlanAssignment, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.DietPlanAssignments.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<DietPlanAssignment>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<DietPlanAssignment>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int assignmentId, string DeletedBy)
        {
            try
            {
                var assignment = _context.DietPlanAssignments.FirstOrDefault(a => a.Id == assignmentId);
                if (assignment != null)
                {
                    assignment.ToggleStatus(DeletedBy);
                    _context.SaveChanges();
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        public Task<DietPlanAssignment> UpdateAsync(DietPlanAssignment assignment, string UpdatedBy)
        {
            try
            {
                var existingAssignment = _context.DietPlanAssignments.FirstOrDefault(a => a.Id == assignment.Id);
                if (existingAssignment != null)
                {
                    _mapper.Map(assignment, existingAssignment);
                    existingAssignment.UpdatedBy = UpdatedBy;
                    existingAssignment.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingAssignment);
                }
                return Task.FromResult<DietPlanAssignment>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<DietPlanAssignment>(null);
            }
        }
    }
}