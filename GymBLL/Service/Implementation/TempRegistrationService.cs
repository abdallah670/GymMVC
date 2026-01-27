using AutoMapper;
using GymBLL.ModelVM;
using GymBLL.Service.Abstract;
using GymBLL.Response;
using GymBLL.Service.Abstract.Communication;
using GymDAL.Entities.Users;
using GymDAL.Repo.Abstract;
using System;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation
{
    public class TempRegistrationService : ITempRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public TempRegistrationService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<Response<TempRegistrationVM>> InitiateregistrationAsync(string email)
        {
            try
            {
                // Check if already exists
                var existing = await _unitOfWork.TempRegistrationRepository.GetByEmailAsync(email);
                if (existing != null)
                {
                     // Update OTP
                     var otp = new Random().Next(100000, 999999).ToString();
                     existing.OtpCode = otp;
                     existing.OtpExpiry = DateTime.UtcNow.AddMinutes(15);
                     _unitOfWork.TempRegistrationRepository.Update(existing);
                     await _unitOfWork.SaveAsync();
                     
                     await _emailService.SendEmailAsync(email, "Verify Your Email - MenoPro Gym", $"Your verification code is: {otp}");

                     return new Response<TempRegistrationVM>(_mapper.Map<TempRegistrationVM>(existing), null, false);
                }

                // Create new
                var otpNew = new Random().Next(100000, 999999).ToString();
                var newTemp = new TempRegistration
                {
                    Email = email,
                    OtpCode = otpNew,
                    OtpExpiry = DateTime.UtcNow.AddMinutes(15),
                    IsOtpVerified = false,
                    RegistrationStatus = "Pending"
                };

                await _unitOfWork.TempRegistrationRepository.AddAsync(newTemp);
                await _unitOfWork.SaveAsync();

                await _emailService.SendEmailAsync(email, "Verify Your Email - MenoPro Gym", $"Your verification code is: {otpNew}");

                return new Response<TempRegistrationVM>(_mapper.Map<TempRegistrationVM>(newTemp), null, false);

            }
            catch (Exception ex)
            {
                return new Response<TempRegistrationVM>(null, ex.Message, true);
            }
        }

        public async Task<Response<TempRegistrationVM>> VerifyOtpAsync(string email, string otp)
        {
             var existing = await _unitOfWork.TempRegistrationRepository.GetByOtpAsync(email, otp);
             if (existing == null)
                return new Response<TempRegistrationVM>(null, "Invalid or expired OTP", true);

             if (existing.OtpExpiry < DateTime.UtcNow)
                return new Response<TempRegistrationVM>(null, "OTP Expired", true);

             existing.IsOtpVerified = true;
             _unitOfWork.TempRegistrationRepository.Update(existing);
             await _unitOfWork.SaveAsync();

             return new Response<TempRegistrationVM>(_mapper.Map<TempRegistrationVM>(existing), null, false);
        }

        public async Task<Response<TempRegistrationVM>> UpdateDetailsAsync(TempRegistrationVM model)
        {
            var existing = await _unitOfWork.TempRegistrationRepository.GetByEmailAsync(model.Email);
            if (existing == null) return new Response<TempRegistrationVM>(null, "Record not found", true);

            // Map updates
            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.DateOfBirth = model.DateOfBirth;
            existing.Gender = model.Gender;
            existing.PhoneNumber = model.PhoneNumber;
            existing.Height = model.Height;
            existing.Weight = model.Weight;
            existing.FitnessGoal = model.FitnessGoal;
            existing.ActivityLevel = model.ActivityLevel;
            existing.SelectedMembershipId = model.SelectedMembershipId;

            _unitOfWork.TempRegistrationRepository.Update(existing);
            await _unitOfWork.SaveAsync();

            return new Response<TempRegistrationVM>(_mapper.Map<TempRegistrationVM>(existing), null, false);
        }

           public async Task<Response<TempRegistrationVM>> GetByEmailAsync(string email)
        {
             var existing = await _unitOfWork.TempRegistrationRepository.GetByEmailAsync(email);
             if (existing == null) return new Response<TempRegistrationVM>(null, "Not Found", true);
             return new Response<TempRegistrationVM>(_mapper.Map<TempRegistrationVM>(existing), null, false);
        }

        public async Task<Response<bool>> CompleteRegistrationAsync(int tempRegistrationId)
        {
            var temp = await _unitOfWork.TempRegistrationRepository.GetByIdAsync(tempRegistrationId);
            if (temp == null) return new Response<bool>(false, "Temp registration not found", true);

            temp.RegistrationStatus = "Completed";
            _unitOfWork.TempRegistrationRepository.Update(temp);
            await _unitOfWork.SaveAsync();
            
            return new Response<bool>(true, null, false);
        }

        public async Task<Response<TempRegistrationVM>> GetByIdAsync(int tempRegistrationId)
        
            {
             var existing = await _unitOfWork.TempRegistrationRepository.GetByIdAsync(tempRegistrationId);
             if (existing == null) return new Response<TempRegistrationVM>(null, "Not Found", true);
             return new Response<TempRegistrationVM>(_mapper.Map<TempRegistrationVM>(existing), null, false);


        }
    }
}
