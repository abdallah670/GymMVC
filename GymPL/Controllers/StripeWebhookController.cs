using GymBLL.Common;
using GymBLL.ModelVM.Communication;
using GymBLL.ModelVM.Financial;
using GymBLL.ModelVM.Member;
using GymBLL.Service.Abstract;
using GymBLL.Service.Abstract.Communication;
using GymBLL.Service.Abstract.Financial;
using GymBLL.Service.Abstract.Member;
using GymDAL.Entities.Users;
using GymDAL.Repo.Abstract;
using GymDAL.Repo.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.IO;
using System.Threading.Tasks;

namespace GymPL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly StripeSettings _stripeSettings;
        private readonly ITempRegistrationService _tempRegistrationService;
        private readonly IMemberService _memberService;
      
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMembershipService _membershipService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<StripeWebhookController> _logger;

     
        public StripeWebhookController(
            IOptions<StripeSettings> stripeSettings,
            ITempRegistrationService tempRegistrationService,
            IMemberService memberService,
          
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IPaymentService paymentService,
            ILogger<StripeWebhookController> logger,
            ISubscriptionService subscriptionService,
            IMembershipService membershipService)
        {
            _stripeSettings = stripeSettings.Value;
            _tempRegistrationService = tempRegistrationService;
            _memberService = memberService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _paymentService = paymentService;
            _subscriptionService = subscriptionService;
            _membershipService = membershipService;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            _logger.LogInformation("Stripe Webhook Received: Starting processing...");
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _stripeSettings.WebhookSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    await HandleSuccessfulPayment(session);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError($"Stripe Signature Verification Failed: {e.Message}");
                return BadRequest($"Webhook error: {e.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected Error in Stripe Webhook: {ex.Message} \n {ex.StackTrace}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        private async Task HandleSuccessfulPayment(Session session)
        {
            var tempRegistrationId = int.Parse(session.Metadata["tempRegistrationId"]);
            var membershipId = int.Parse(session.Metadata["membershipId"]);

            // 1. Get the temp registration
            var tempReg = await 
                _tempRegistrationService.GetByIdAsync(tempRegistrationId);
            
            if (tempReg == null || tempReg.Result == null)
            {
                _logger.LogError($"Temp Registration not found for ID: {tempRegistrationId}");
                return;
            }

            // Lookup Fitness Goal
            int? fitnessGoalId = null;
            if (!string.IsNullOrEmpty(tempReg.Result.FitnessGoal))
            {
                var goal = await _unitOfWork.FitnessGoalsRepository.FirstOrDefaultAsync(g => g.GoalName == tempReg.Result.FitnessGoal);
                if (goal != null) fitnessGoalId = goal.Id;
            }

            // 2. Create the actual Member account
            var member = new MemberProfileVM
            {
                Email = tempReg.Result.Email,
                Phone = tempReg.Result.PhoneNumber, // Map Phone
                FullName = $"{tempReg.Result.FirstName} {tempReg.Result.LastName}",
                Gender = tempReg.Result.Gender,
                Age = tempReg.Result.DateOfBirth.HasValue ? CalculateAge(tempReg.Result.DateOfBirth.Value) : 0,
                CurrentWeight = Convert.ToDouble(tempReg.Result.Weight ?? 0),
                Height = Convert.ToDouble(tempReg.Result.Height ?? 0),
                ActivityLevel = tempReg.Result.ActivityLevel, // Map Activity Level
                FitnessGoalId = fitnessGoalId, // Map Fitness Goal ID
                HasCompletedProfile = true,
                JoinDate = System.DateTime.UtcNow
            };


            // Generate a simpler password
            var randomPassword = "MenoPro" + new Random().Next(1000, 9999) ;
            member.Password = randomPassword; // Assign generated password
            _logger.LogInformation($"Generated Password for {member.Email}: {randomPassword}"); // Debug log

            // 3. Register the Member
            var result = await _memberService.Register(member);

            if (result.Succeeded)
            {
                // 3a. Retrieve the created user's ID
                var createdUserResponse = await _memberService.GetMemberByEmailAsync(member.Email);
                if (createdUserResponse.ISHaveErrorOrnNot || createdUserResponse.Result == null)
                {
                    _logger.LogError($"Failed to retrieve created user: {member.Email}");
                    return;
                }
                var memberId = createdUserResponse.Result.Id;

                // 4. Find selected membership
                var membershipResponse = await _membershipService.GetByIdAsync(membershipId);
                if (membershipResponse.Result == null)
                {
                    // Log error or handle failure
                    _logger.LogError($"Membership with ID {membershipId} not found.");

                    return;
                }
                var selectedMembership = membershipResponse.Result;

                // 5. Create Payment
                var paymentModel = new PaymentVM
                {
                    MemberId = memberId,
                    Amount = selectedMembership.Price,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = "Credit Card - Stripe",
                    Description = $"Membership Payment: {selectedMembership.MembershipType}",
                    PaymentType = "Membership Fee"
                };

                var paymentResult = await _paymentService.CreateAsync(paymentModel);
                if (paymentResult.ISHaveErrorOrnNot)
                {
                    // Log error
                    _logger.LogError($"Payment creation failed for Member ID {memberId}: {paymentResult.ErrorMessage}");

                }

                // 6. Create Subscription
                if (paymentResult?.Result != null)
                {
                    var subscriptionModel = new SubscriptionVM
                    {
                        MemberId = memberId,
                        MembershipId = membershipId,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(selectedMembership.DurationInMonths), 
                        PaymentId = paymentResult.Result.Id,
                        Status = "Active",
                        MemberName = member.FullName,
                        MembershipType = selectedMembership.MembershipType
                    };

                    await _subscriptionService.CreateAsync(subscriptionModel);
                }

                // 7. Mark temp registration as completed
                await _tempRegistrationService.CompleteRegistrationAsync(tempRegistrationId);

                // 8. Send welcome email with login instructions
                var subject = "Welcome to MenoPro Gym!";
                var body = $@"
                    <h1>Welcome, {member.FullName}!</h1>
                    <p>Your membership has been successfully activated.</p>
                    <p><strong>Membership:</strong> {selectedMembership.MembershipType}</p>
                    <p><strong>Your Login Credentials:</strong></p>
                    <ul>
                        <li>Email: {member.Email}</li>
                        <li>Password: {randomPassword}</li>
                    </ul>
                    <p>Please login and change your password immediately.</p>
                    <a href='https://localhost:5000/Account/Login'>Login Here</a>
                ";
                await _emailService.SendEmailAsync(member.Email, subject, body);

                
              
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError($"Failed to register member {member.Email}: {errors}");
            }
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
