using AutoMapper;
using GymDAL.Repo.Abstract.Workout;

namespace GymDAL.Repo.Implementation
{
    public class WorkoutAssignmentRepository : Repository<WorkoutAssignment>, IWorkoutAssignmentRepository
    {
        public WorkoutAssignmentRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<WorkoutAssignment> CreateAsync(WorkoutAssignment assignment, string Createdby)
        {
            try
            {
                assignment.CreatedBy = Createdby;
                assignment.CreatedAt = DateTime.UtcNow;
                _context.WorkoutAssignments.Add(assignment);
                _context.SaveChanges();
                return Task.FromResult(assignment);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutAssignment>(null);
            }
        }

        public Task<IEnumerable<WorkoutAssignment>> GetAsync(Expression<Func<WorkoutAssignment, bool>>? Filter)
        {
            try
            {
                var query = _context.WorkoutAssignments.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<WorkoutAssignment>>(null);
            }
        }

        public Task<WorkoutAssignment> GetByAsync(Expression<Func<WorkoutAssignment, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.WorkoutAssignments.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<WorkoutAssignment>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutAssignment>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int assignmentId, string DeletedBy)
        {
            try
            {
                var assignment = _context.WorkoutAssignments.FirstOrDefault(a => a.Id == assignmentId);
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

        public Task<WorkoutAssignment> UpdateAsync(WorkoutAssignment assignment, string UpdatedBy)
        {
            try
            {
                var existingAssignment = _context.WorkoutAssignments.FirstOrDefault(a => a.Id == assignment.Id);
                if (existingAssignment != null)
                {
                    _mapper.Map(assignment, existingAssignment);
                    existingAssignment.UpdatedBy = UpdatedBy;
                    existingAssignment.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingAssignment);
                }
                return Task.FromResult<WorkoutAssignment>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutAssignment>(null);
            }
        }
    }
}