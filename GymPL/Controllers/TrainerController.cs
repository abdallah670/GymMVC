using AutoMapper.Execution;
using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.Identity;
using GymBLL.ModelVM.Member;
using GymBLL.ModelVM.Trainer;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract.Trainer;
using GymBLL.Service.Abstract.Member;
using GymBLL.Service.Abstract.Nutrition;
using GymBLL.Service.Abstract.Workout;
using GymBLL.Service.Abstract.Financial;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using GymBLL.ModelVM;
using GymPL.Services;
using GymPL.Extensions;
using GymDAL.Repo.Abstract;
using GymBLL.Service.Abstract.AI;
using GymBLL.ModelVM.AI;

namespace GymPL.Controllers
{
    public class TrainerController : Controller
    {

        private readonly ITrainerService trainerService;
        private readonly IMemberService _memberService;
        private readonly IWorkoutAssignmentService _workoutAssignmentService;
        private readonly IDietPlanAssignmentService _dietPlanAssignmentService;
        private readonly IWorkoutPlanService _workoutPlanService;
        private readonly IDietPlanService _dietPlanService;
        private readonly ISubscriptionService _subscripionService;
        private readonly IMembershipService _membershipService;
        private IFileUploadService _fileUploadService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAIService _aiService;

        public TrainerController(
            ITrainerService trainerService,
            IFileUploadService fileUploadService,
            IMemberService memberService,
            IWorkoutAssignmentService workoutAssignmentService,
            IDietPlanAssignmentService dietPlanAssignmentService,
            IWorkoutPlanService workoutPlanService,
            IDietPlanService dietPlanService,
            ISubscriptionService subscripionService,
            IMembershipService membershipService,
            IUnitOfWork unitOfWork,
            IAIService aiService)
        {

            this.trainerService = trainerService;
            _fileUploadService = fileUploadService; 
            _memberService = memberService;
            _workoutAssignmentService = workoutAssignmentService;
            _dietPlanAssignmentService
                = dietPlanAssignmentService;
            _workoutPlanService = workoutPlanService;
            _dietPlanService = dietPlanService;
            _subscripionService = subscripionService;
            _membershipService = membershipService;
            _unitOfWork = unitOfWork;
            _aiService = aiService;


        }
        private async Task AddTrainerClaimsAsync(TrainerVM result)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, result.Id));                      // Unique ID
            claims.Add(new Claim(ClaimTypes.Name, result.FullName ?? string.Empty));         // Username
            claims.Add(new Claim(ClaimTypes.Email, result.Email ?? string.Empty));           // Standard email
            claims.Add(new Claim("Email", result.Email ?? string.Empty));                     // Backup email
            claims.Add(new Claim("Phone", result.Phone ?? string.Empty));                     // Phone number
            claims.Add(new Claim(ClaimTypes.Role, "Trainer"));                               // Standard role
            claims.Add(new Claim("Role", "Trainer"));                                         // Backup role
            claims.Add(new Claim("ProfilePicture", result.ProfilePicture ?? string.Empty));   // Profile image
            claims.Add(new Claim("DisplayName", result.FullName ?? string.Empty));           // Display name

            var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);
           
        }

        // GET: /Account/Register
        // GET: /Account/Register
        [HttpGet]
        // [AllowAnonymous]
        public IActionResult CreateOwner()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateOwner(RegisterUserVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await trainerService.Register(model);

                if (result.Succeeded)
                {

                   
                    var user = await trainerService.GetTrainerByEmailAsync(model.Email);
                    if (!user.ISHaveErrorOrnNot)
                    {
                        // Store user info in claims
                        if (AddTrainerClaimsAsync(user.Result).IsCompletedSuccessfully)
                        {

                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                            var json = System.IO.File.ReadAllText(filePath);
                            dynamic jsonObj = JsonConvert.DeserializeObject(json);
                            jsonObj["GymSettings"]["IsFirstRun"] = false;
                            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                            System.IO.File.WriteAllText(filePath, output);

                        }
                    
                    }
                    ViewBag.HeaderType = "Trainer";
                    return RedirectToAction("Profile");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        public async Task<IActionResult> Dashboard()
        {
            // Dashboard Logic
            var membersResponse = await _memberService.GetAllMembersAsync();
            var workoutPlansResponse = await _workoutPlanService.GetActiveWorkoutPlansAsync();
            var dietPlansResponse = await _dietPlanService.GetActiveDietPlansAsync();
            var allPayments = await _unitOfWork.Payments.GetAllAsync();

            var totalMembers = 0;
            var recentMembers = new List<MemberVM>();
            if (!membersResponse.ISHaveErrorOrnNot)
            {
                totalMembers = membersResponse.Result.Count;
                recentMembers = membersResponse.Result.OrderByDescending(m => m.JoinDate).Take(5).ToList(); 
            }

            var activeWorkouts = 0;
            if (!workoutPlansResponse.ISHaveErrorOrnNot)
            {
                activeWorkouts = workoutPlansResponse.Result.Count;
            }

            var activeDiets = 0;
            if (!dietPlansResponse.ISHaveErrorOrnNot)
            {
                activeDiets = dietPlansResponse.Result.Count;
            }

            var dashboardVM = new TrainerDashboardVM
            {
                TotalMembers = totalMembers,
                ActiveWorkoutPlans = activeWorkouts,
                ActiveDietPlans = activeDiets,
                RecentMembers = recentMembers
            };

            // Calculate Chart Data (Last 6 Months)
            var last6Months = Enumerable.Range(0, 6)
                .Select(i => DateTime.UtcNow.AddMonths(-i))
                .OrderBy(d => d)
                .ToList();

            foreach (var month in last6Months)
            {
                dashboardVM.ChartLabels.Add(month.ToString("MMM yyyy"));

                // Revenue
                var monthlyRevenue = allPayments
                    .Where(p => p.PaymentDate.Month == month.Month && p.PaymentDate.Year == month.Year && p.Status == "Completed")
                    .Sum(p => p.Amount);
                dashboardVM.MonthlyRevenue.Add(monthlyRevenue);

                // New Members
                var newMembers = membersResponse.Result
                    .Count(m => m.JoinDate.Month == month.Month && m.JoinDate.Year == month.Year);
                dashboardVM.NewMembersTrend.Add(newMembers);
            }

            // Keep existing ViewBag claims for header if needed, or remove if ViewModel covers it
            ViewBag.UserId = User.GetUserId();
            ViewBag.FullName = User.GetUserFullName();
            ViewBag.UserType = User.GetUserType();
            ViewBag.HeaderType = "Trainer";

            return View(dashboardVM);
        }

        public async Task<IActionResult> Profile()
        {
            var membersResponse = await _memberService.GetAllMembersAsync();
            var workoutPlansResponse = await _workoutPlanService.GetActiveWorkoutPlansAsync();
            var dietPlansResponse = await _dietPlanService.GetActiveDietPlansAsync();
            var totalMembers = 0;
            
            if (!membersResponse.ISHaveErrorOrnNot)
            {
                totalMembers = membersResponse.Result.Count;
             
            }

            var activeWorkouts = 0;
            if (!workoutPlansResponse.ISHaveErrorOrnNot)
            {
                activeWorkouts = workoutPlansResponse.Result.Count;
            }

            var activeDiets = 0;
            if (!dietPlansResponse.ISHaveErrorOrnNot)
            {
                activeDiets = dietPlansResponse.Result.Count;
            }
            var trainerDashboardVM = new TrainerDashboardVM
            {
                TotalMembers = totalMembers,
                ActiveWorkoutPlans = activeWorkouts,
                ActiveDietPlans = activeDiets,
              
            };
            
            ViewBag.trainerDashboardVM= trainerDashboardVM ?? new TrainerDashboardVM();
            var userId = User.GetUserId();
               var response = await trainerService.GetTrainerByIdAsync(userId);

            if (response.ISHaveErrorOrnNot)
            {
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction("Dashboard");
            }
            var profileVM = new TrainerProfileVM
            {
                Id = response.Result.Id,
                FullName = response.Result.FullName,
                Email = response.Result.Email,
                Phone = response.Result.Phone,
                ProfilePicture = response.Result.ProfilePicture,
                ExperienceYears = response.Result.ExperienceYears,
                Bio = response.Result.Bio,
                ProfileImageFile= null

            };

            return View(profileVM);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(TrainerProfileVM model)
        {
            if(model.ProfileImageFile==null&&model.ProfilePicture!=null) ModelState.Remove("ProfileImageFile");
            if (ModelState.IsValid)
            {
                string profilePicturePath = model.ProfilePicture;

                // Handle file upload if new picture is provided
                if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
                {
                    // Get current profile picture to delete old one
                    var currentProfilePicture = User.GetProfilePicture();
                    if (!string.IsNullOrEmpty(currentProfilePicture))
                    {
                        _fileUploadService.DeleteProfilePicture(currentProfilePicture);
                    }

                    // Upload new picture
                    profilePicturePath = await _fileUploadService.UploadProfilePictureAsync(
                        model.ProfileImageFile, model.Id);
                }

                var trainerVm = new TrainerVM
                {
                    Id = model.Id,
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    ProfilePicture = profilePicturePath,
                    ExperienceYears = model.ExperienceYears ?? 0,
                    Bio = model.Bio
                };

                var response = await trainerService.UpdateTrainerAsync(trainerVm);

                if (!response.ISHaveErrorOrnNot)
                {
                    // Update claims for immediate UI update
                    await UpdateUserClaims(trainerVm);

                    TempData["Success"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Profile));
                }

                TempData["Error"] = response.ErrorMessage;
            }
            else
            {
                // Log validation errors
                var errors = ModelState
                    .Where(kv => kv.Value.Errors.Count > 0)
                    .Select(kv => new { Key = kv.Key, Errors = kv.Value.Errors.Select(e => e.ErrorMessage).ToList() })
                    .ToList();

             

                TempData["Error"] = "Please fix validation errors";
            }

            // If we got here, something went wrong - redisplay form
            return View("Profile", model);
        }

        [HttpGet]
        public async Task<IActionResult> MemberDetails(string id, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            
            // Set returnUrl for the back button - default to Trainer Dashboard if not provided
            ViewBag.ReturnUrl = !string.IsNullOrEmpty(returnUrl) ? returnUrl : Url.Action("Dashboard", "Trainer");

            var memberResponse = await _memberService.GetMemberByIdAsync(id);

            if (memberResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Member not found";
                return RedirectToAction("Dashboard");
            }

            var subscriptionsResponse = await _subscripionService.GetByMemeberIdAsync(id);
            
            var model = new MemberDietWorkoutPlansVM
            {
                Id = memberResponse.Result.Id,
                FullName = memberResponse.Result.FullName,
                Email = memberResponse.Result.Email,
                JoinDate = memberResponse.Result.JoinDate,
                CurrentWeight = memberResponse.Result.CurrentWeight,
                Height = memberResponse.Result.Height,
                FitnessGoal = subscriptionsResponse.Result?.MemberVM?.FitnessGoal,
                DietPlanAssignmentVM = subscriptionsResponse.Result?.DietPlanAssignmentVM,
                WorkoutAssignmentVM = subscriptionsResponse.Result?.WorkoutAssignmentVM,
                Age = memberResponse.Result.Age,
                ActivityLevel = memberResponse.Result.ActivityLevel,
                Gender = memberResponse.Result.Gender,
                Phone = memberResponse.Result.Phone,
            };

            if (subscriptionsResponse.Result != null)
            {
                if (model.DietPlanAssignmentVM != null)
                    model.DietPlanAssignmentVM.DietPlan = subscriptionsResponse.Result.DietPlanAssignmentVM?.DietPlan;
                
                if (model.WorkoutAssignmentVM != null)
                    model.WorkoutAssignmentVM.WorkoutPlan = subscriptionsResponse.Result.WorkoutAssignmentVM?.WorkoutPlan;
            }

            ViewBag.HasDiet = subscriptionsResponse.Result?.DietPlanAssignmentVM != null;
            ViewBag.HasWorkout = subscriptionsResponse.Result?.WorkoutAssignmentVM != null;
            ViewBag.HasActiveSubscription = subscriptionsResponse.Result != null;

            return View(model);
        }

  
        private async Task UpdateUserClaims(TrainerVM trainer)
        {
          

                // Sign out and sign back in with updated claims
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

                await AddTrainerClaimsAsync(trainer);
          
        }
        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }





        [HttpGet]
        public async Task<IActionResult> GetAISuggestion(string memberId, string type)
        {
            if (string.IsNullOrEmpty(memberId)) return BadRequest("Member ID is required");

            if (type.ToLower() == "diet")
            {
                var suggestion = await _aiService.SuggestDietPlanAsync(memberId);
                return Json(suggestion);
            }
            else
            {
                var suggestion = await _aiService.SuggestWorkoutPlanAsync(memberId);
                return Json(suggestion);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApplyAISuggestion([FromBody] PlanSuggestionVM suggestion, string memberId)
        {
            if (suggestion == null || string.IsNullOrEmpty(memberId)) return BadRequest("Invalid data");

            try
            {
                if (suggestion.Type == "Diet")
                {
                    // Create partial diet plan
                    var dietPlanVm = new DietPlanVM
                    {
                        Name = suggestion.PlanName,
                        Description = suggestion.Description,
                        TotalCalories = (int)(suggestion.TotalCalories ?? 2000),
                        DietType = suggestion.RecommendedDietType,
                        IsActive = true,
                        DietPlanItemsVM = suggestion.SuggestedMeals?.Select(m => new DietPlanItemVM
                        {
                            MealName = m.MealName,
                            MealType = m.MealType ?? "Lunch",
                            Notes = m.Notes,
                            Calories = m.Calories,
                            DayNumber = 1 
                        }).ToList()
                    };

                    var createResult = await _dietPlanService.CreateDietPlanAsync(dietPlanVm);
                    if (!createResult.ISHaveErrorOrnNot)
                    {
                        // Assign it
                        var assignment = new DietPlanAssignmentVM
                        {
                         
                            DietPlanId = createResult.Result.Id,
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMonths(1)
                        };
                       var result=  await _dietPlanAssignmentService.CreateAsync(assignment);
                        //reassign in subscription
                        var subscription = await _subscripionService.GetActiveSubscriptionByMemberIdAsync(memberId);
                        if (subscription.ISHaveErrorOrnNot) 
                            return BadRequest("This member doesn't have an active subscription. Please create a subscription first before assigning plans.");
                        subscription.Result.DietPlanAssignmentId = result.Result.Id;
                        var updateResult = await _subscripionService.UpdateAsync(subscription.Result);
                        if (updateResult.ISHaveErrorOrnNot) return BadRequest(updateResult.ErrorMessage);

                        return Ok(new { message = "AI Diet Plan applied successfully!" });
                    }
                }
                else
                {
                    // Create partial workout plan
                    var workoutPlanVm = new WorkoutPlanVM
                    {
                        Name = suggestion.PlanName,
                        Description = suggestion.Description,
                        Difficulty = suggestion.DifficultyLevel,
                        IsActive = true,
                        workoutPlanItemVMs = suggestion.SuggestedExercises?.Select(e => new WorkoutPlanItemVM
                        {
                            ExerciseName = e.ExerciseName,
                            Sets = e.Sets,
                            Reps = e.Reps,
                            Notes = e.Notes,
                            DayNumber = 1
                        }).ToList()
                    };

                    var createResult = await _workoutPlanService.CreateWorkoutPlanAsync(workoutPlanVm);
                    if (!createResult.ISHaveErrorOrnNot)
                    {
                        // Assign it
                        var assignment = new WorkoutAssignmentVM
                        {
                           
                            WorkoutPlanId = createResult.Result.Id,
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMonths(1)
                        };
                      var result =await _workoutAssignmentService.CreateAsync(assignment);
                        //reassign in subscription
                         var subscription = await _subscripionService.GetActiveSubscriptionByMemberIdAsync(memberId);
                        if (subscription.ISHaveErrorOrnNot) 
                            return BadRequest("This member doesn't have an active subscription. Please create a subscription first before assigning plans.");
                        subscription.Result.WorkoutAssignmentId = result.Result.Id;
                        var updateResult = await _subscripionService.UpdateAsync(subscription.Result);
                        if (updateResult.ISHaveErrorOrnNot) return BadRequest(updateResult.ErrorMessage);
                        return Ok(new { message = "AI Workout Plan applied successfully!" });
                    }
                }

                return BadRequest("Failed to apply AI suggestion");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetActivePlans(string type)
        {
            if (type.ToLower() == "diet")
            {
                var plans = await _dietPlanService.GetActiveDietPlansAsync();
                return Json(plans.Result.Select(p => new { id = p.Id, name = p.Name }));
            }
            else
            {
                var plans = await _workoutPlanService.GetActiveWorkoutPlansAsync();
                return Json(plans.Result.Select(p => new { id = p.Id, name = p.Name }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReassignPlan(string memberId, int planId, string type)
        {
            try
            {
                if (type.ToLower() == "diet")
                {
                    var assignment = new DietPlanAssignmentVM
                    {
                        
                        DietPlanId = planId,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(1) // Default 1 month
                    };
                    var result = await _dietPlanAssignmentService.CreateAsync(assignment);
                    if (result.ISHaveErrorOrnNot) return BadRequest(result.ErrorMessage);
                    //reassign in subscription
                    var subscription = await _subscripionService.GetActiveSubscriptionByMemberIdAsync(memberId);
                    if (subscription.ISHaveErrorOrnNot) return BadRequest(subscription.ErrorMessage);

                    subscription.Result.DietPlanAssignmentId = result.Result.Id;
                    var updateResult = await _subscripionService.UpdateAsync(subscription.Result);
                    if (updateResult.ISHaveErrorOrnNot) return BadRequest(updateResult.ErrorMessage);
                }
                else
                {
                    var assignment = new WorkoutAssignmentVM
                    {
                       
                        WorkoutPlanId = planId,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(1)
                    };
                    var result = await _workoutAssignmentService.CreateAsync(assignment);
                    //reassign in subscription
                    if (result.ISHaveErrorOrnNot) return BadRequest(result.ErrorMessage);
                    var subscription = await _subscripionService.GetActiveSubscriptionByMemberIdAsync(memberId);
                    if (subscription.ISHaveErrorOrnNot) return BadRequest(subscription.ErrorMessage);
                    subscription.Result.WorkoutAssignmentId = result.Result.Id;
                    var updateResult = await _subscripionService.UpdateAsync(subscription.Result);
                    if (updateResult.ISHaveErrorOrnNot) return BadRequest(updateResult.ErrorMessage);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
