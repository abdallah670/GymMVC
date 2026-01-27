using AutoMapper;
using GymBLL.Common;
using GymBLL.ModelVM.Member;
using GymBLL.Service.Abstract.Member;
using GymDAL.Entities.Users;
using GymDAL.Repo.Abstract.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation.Member
{
    public class WeightLogService : IWeightLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WeightLogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Response<bool>> LogWeightAsync(WeightLogVM model)
        {
            try
            {
                var entity = new WeightLog
                {
                    MemberId = model.MemberId,
                    Weight = model.Weight,
                    DateRecorded = DateTime.UtcNow,
                    Notes = model.Notes
                };

                await _unitOfWork.WeightLogs.AddAsync(entity);
                await _unitOfWork.SaveAsync();

                // Also update the member's current weight
                var member = await _unitOfWork.Members.GetByIdAsync(model.MemberId);
                if (member != null)
                {
                    member.CurrentWeight = model.Weight;
                    _unitOfWork.Members.Update(member);
                    await _unitOfWork.SaveAsync();
                }

                return new Response<bool> (true, null, false);
            }
            catch (Exception ex)
            {
                return new Response<bool>
                (
                 
                    false,
                    "Failed to log weight: " + ex.Message,
                   true
                   
                );
            }
        }

        public async Task<Response<IEnumerable<WeightLogVM>>> GetHistoryAsync(string memberId)
        {
            try
            {
                var history = await _unitOfWork.WeightLogs.GetWeightHistoryAsync(memberId);
                var historyVM = new List<WeightLogVM>();

                foreach (var item in history)
                {
                    historyVM.Add(new WeightLogVM
                    {
                        Id = item.Id,
                        MemberId = item.MemberId,
                        Weight = item.Weight,
                        DateRecorded = item.DateRecorded,
                        Notes = item.Notes
                    });
                }

                return new Response<IEnumerable<WeightLogVM>>(historyVM,null , false);
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<WeightLogVM>>(null, "Failed to retrieve history: " + ex.Message, true);
                
            }
        }
    }
}
