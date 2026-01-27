using AutoMapper;
using GymBLL.Common;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract.Workout;
using GymDAL.Entities.Workout;
using GymDAL.Repo.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation.Workout
{
    public class WorkoutLogService : IWorkoutLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkoutLogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Response<bool>> LogWorkoutAsync(WorkoutLogVM model)
        {
            try
            {
                var log = new WorkoutLog
                {
                    MemberId = model.MemberId,
                    WorkoutPlanId = model.WorkoutPlanId,
                    Date = DateTime.UtcNow,
                    DurationMinutes = model.DurationMinutes,
                    Notes = model.Notes,
                    Entries = model.Entries.Select(e => new WorkoutLogEntry
                    {
                        WorkoutPlanItemId = e.WorkoutPlanItemId,
                        ExerciseName = e.ExerciseName,
                        SetsPerformed = e.SetsPerformed,
                        RepsPerformed = e.RepsPerformed,
                        WeightLifted = e.WeightLifted
                    }).ToList()
                };

                await _unitOfWork.WorkoutLogs.AddAsync(log);
                var result = await _unitOfWork.SaveAsync();

                return new Response<bool>(result > 0, null, result <= 0);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error logging workout: {ex.Message}", true);
            }
        }

        public async Task<Response<List<WorkoutLogVM>>> GetMemberHistoryAsync(string memberId)
        {
            try
            {
                var logs = await _unitOfWork.WorkoutLogs
                    .Get(l => l.MemberId == memberId)
                    .Include(l => l.WorkoutPlan)
                    .Include(l => l.Entries)
                    .OrderByDescending(l => l.Date)
                    .ToListAsync();

                var vms = logs.Select(l => new WorkoutLogVM
                {
                    Id = l.Id,
                    MemberId = l.MemberId,
                    WorkoutPlanId = l.WorkoutPlanId,
                    WorkoutPlanName = l.WorkoutPlan?.Name ?? "Custom Workout",
                    Date = l.Date,
                    DurationMinutes = l.DurationMinutes,
                    Notes = l.Notes,
                    Entries = l.Entries.Select(e => new WorkoutLogEntryVM
                    {
                        Id = e.Id,
                        ExerciseName = e.ExerciseName,
                        SetsPerformed = e.SetsPerformed,
                        RepsPerformed = e.RepsPerformed,
                        WeightLifted = e.WeightLifted
                    }).ToList()
                }).ToList();

                return new Response<List<WorkoutLogVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<WorkoutLogVM>>(null, $"Error fetching history: {ex.Message}", true);
            }
        }
    }
}
