using AutoMapper;
using GymBLL.Common;
using GymBLL.ModelVM.Member;
using GymBLL.ModelVM.Report;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract.Report;
using GymDAL.Repo.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation.Report
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Response<MemberReportVM>> GenerateMemberReportAsync(string memberId)
        {
            try
            {
                // 1. Get Member Details
                var member = await _unitOfWork.Members.GetByIdAsync(memberId);
                if (member == null)
                    return new Response<MemberReportVM>(null, "Member not found", true);

                var memberVM = _mapper.Map<MemberVM>(member);

                // 2. Get Subscription Details
                var subscriptions = await _unitOfWork.Subscriptions
                    .Get(
                      filter: s => s.MemberId == memberId, null,
                      query => query.Include(q => q.Membership))
                    .ToListAsync();
                var activeSubscription = subscriptions.OrderByDescending(s => s.EndDate).FirstOrDefault();

                // 3. Get Workout Logs
                var logs = await _unitOfWork.WorkoutLogs
                    .Get(l => l.MemberId == memberId)
                    .Include(l => l.WorkoutPlan)
                    .Include(l => l.Entries)
                    .OrderByDescending(l => l.Date)
                    .ToListAsync();

                // 4. Calculate Stats
                var totalWorkouts = logs.Count;
                var lastWorkout = logs.FirstOrDefault()?.Date;
                var avgDuration = logs.Any() ? logs.Average(l => l.DurationMinutes) : 0;

                // 5. Map Logs to VM (Take recent 5)
                var recentLogs = logs.Take(5).Select(l => new WorkoutLogVM
                {
                    Id = l.Id,
                    WorkoutPlanName = l.WorkoutPlan?.Name ?? "Custom Workout",
                    Date = l.Date,
                    DurationMinutes = l.DurationMinutes,
                    Notes = l.Notes,
                    Entries = l.Entries.Select(e => new WorkoutLogEntryVM
                    {
                        ExerciseName = e.ExerciseName,
                        SetsPerformed = e.SetsPerformed,
                        RepsPerformed = e.RepsPerformed,
                        WeightLifted = e.WeightLifted
                    }).ToList()
                }).ToList();

                var report = new MemberReportVM
                {
                    Member = memberVM,
                    MembershipType = activeSubscription?.Membership?.MembershipType ?? "No Active Plan",
                    MembershipEndDate = activeSubscription?.EndDate,
                    TotalWorkoutsLogged = totalWorkouts,
                    LastWorkoutDate = lastWorkout,
                    AverageWorkoutDuration = Math.Round(avgDuration, 1),
                    StartWeight = 0, // Need to implement weight history properly if we want start weight specifically
                    CurrentWeight = member.CurrentWeight,
                    RecentLogs = recentLogs
                };

                return new Response<MemberReportVM>(report, null, false);
            }
            catch (Exception ex)
            {
                return new Response<MemberReportVM>(null, $"Error generating report: {ex.Message}", true);
            }
        }
    }
}
