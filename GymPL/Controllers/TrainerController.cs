using AutoMapper.Execution;
using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.User.AppUser;
using GymBLL.ModelVM.User.Member;
using GymBLL.ModelVM.User.Trainer;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract;
using GymBLL.Service.Implementation;
using GymDAL.Entities.Users;
using GymPL.Services;
using GymPL.ViewModels;
using GymWeb.Extensions;
using MenoBLL.ModelVM.AccountVM;
using MenoBLL.Service.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GymWeb.Controllers
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

        public TrainerController(
            ITrainerService trainerService,
            IFileUploadService fileUploadService,
            IMemberService memberService,
            IWorkoutAssignmentService workoutAssignmentService,
            IDietPlanAssignmentService dietPlanAssignmentService,
            IWorkoutPlanService workoutPlanService,
            IDietPlanService dietPlanService,
            ISubscriptionService subscripionService,
            IMembershipService membershipService)
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
        //     [AllowAnonymous]
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
        public IActionResult Dashboard()
        {
            
            // General user claims
            ViewBag.UserId = User.GetUserId();
            ViewBag.FullName = User.GetUserFullName();
            ViewBag.Email = User.GetUserEmail();
            ViewBag.Phone = User.GetUserPhone();
            ViewBag.UserType = User.GetUserType();
           
            ViewBag.IsActive = User.IsUserActive();
            
            // Trainer-specific claims
            if (User.IsTrainer())
            {
                ViewBag.ExperienceYears = User.GetTrainerExperienceYears();
                ViewBag.Bio = User.GetTrainerBio();
                ViewBag.HeaderType = "Trainer";
            }
            return View();
        }
        public async Task<IActionResult> Profile()
        {
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

        // View Assigned Members
        public async Task<IActionResult> MyMembers()
        {
           
            
            // Get all members (in a real app, you'd filter by trainer assignment)
            var membersResponse = await _memberService.GetAllMembersAsync();
            
            if (membersResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Unable to load members";
                return View(new List<MemberVM>());
            }

            // For each member, check if they have workout and diet plans
            var membersList = membersResponse.Result;
            
            return View(membersList);
        }

        // View Member Details
        public async Task<IActionResult> MemberDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var memberResponse = await _memberService.GetMemberByIdAsync(id);
            
            if (memberResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Member not found";
                return RedirectToAction("MyMembers");
            }
            var subscriptionsResponse = await _subscripionService.GetByMemeberIdAsync(id);
            var Member = new MemberDietWorkoutPlansVM
            {
                Id = memberResponse.Result.Id,
                FullName = memberResponse.Result.FullName,
                Email = memberResponse.Result.Email,
                JoinDate = memberResponse.Result.JoinDate,
                CurrentWeight = memberResponse.Result.CurrentWeight,
                CurrentHeight = memberResponse.Result.CurrentHeight,
                FitnessGoal=subscriptionsResponse.Result.MemberVM?.FitnessGoal
                ,
                // Initialize empty collections if needed
                DietPlanAssignmentVM = subscriptionsResponse.Result.DietPlanAssignmentVM,
                WorkoutAssignmentVM = subscriptionsResponse.Result.WorkoutAssignmentVM,
                

            };
            if(Member.DietPlanAssignmentVM!=null)
            Member.DietPlanAssignmentVM.DietPlan = subscriptionsResponse.Result.DietPlanAssignmentVM?.DietPlan;
            if(Member.WorkoutAssignmentVM!=null) 
            Member.WorkoutAssignmentVM.WorkoutPlan = subscriptionsResponse.Result.WorkoutAssignmentVM?.WorkoutPlan;

            ViewBag.hasDiet=subscriptionsResponse.Result.DietPlanAssignmentVM!=null?true:false;
            ViewBag.hasWorkout= subscriptionsResponse.Result.WorkoutAssignmentVM != null ? true : false;

            return View(Member);
        }

        // Get Member Plan Status (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetMemberPlanStatus(string memberId)
        {
            // Check workout plans
            var Response = await _subscripionService.CheckActivePlanAsync(memberId);
            if (Response.ISHaveErrorOrnNot)
            {
                return Json(new { error = "Unable to check workout plans" });
            }
            var hasWorkoutPlan =Response.Result.hasWorkout;

        

            var hasDietPlan =  Response.Result.hasDiet;

            return Json(new 
            { 
                hasWorkoutPlan = hasWorkoutPlan,
                hasDietPlan = hasDietPlan,
                
            });
        }

        // Create Workout Plan for Member
        [HttpGet]
        public async Task<IActionResult> CreateWorkoutPlan(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
                return NotFound();

            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            if (memberResponse.ISHaveErrorOrnNot)
            {
                TempData["Error"] = "Member not found";
                return RedirectToAction("MyMembers");
            }

            // Get available workout plans
            var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();
            
            ViewBag.Member = memberResponse.Result;
            ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
          
            var model = new WorkoutAssignmentVM
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkoutPlan(WorkoutAssignmentVM model,string memberId)
        {
            if (!ModelState.IsValid)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();
                ViewBag.Member = memberResponse.Result;
                ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
                return View(model);
            }
          
            var response = await _workoutAssignmentService.CreateAsync(model);
            
            if (response.ISHaveErrorOrnNot)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                if (memberResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = "Member not found";
                    return RedirectToAction("MyMembers");
                }

                // Get available workout plans
                var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();

                ViewBag.Member = memberResponse.Result;
                ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }
            
            var resulteResponse = await _subscripionService.GetByMemeberIdAsync(memberId);
            resulteResponse.Result.WorkoutAssignmentVM = new WorkoutAssignmentVM();
            resulteResponse.Result.WorkoutAssignmentVM.Id = response.Result.Id;
            var Updated = await _subscripionService.UpdateAsync(resulteResponse.Result);
            if (Updated.ISHaveErrorOrnNot)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(ViewBag.Member.Id);
                if (memberResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = "Member not found";
                    return RedirectToAction("MyMembers");
                }

                // Get available workout plans
                var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();

                ViewBag.Member = memberResponse.Result;
                ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }
            TempData["Success"] = "Workout plan assigned successfully!";
            return RedirectToAction("MemberDetails", new { id = memberId });
        }

        // Create Diet Plan for Member
        [HttpGet]
        public async Task<IActionResult> CreateDietPlan(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
                return NotFound();

            var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
            if (memberResponse.ISHaveErrorOrnNot)
            {

                TempData["Error"] = "Member not found";
                return RedirectToAction("MyMembers");
            }

            // Get available diet plans
            var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();
            
            ViewBag.Member = memberResponse.Result;
            ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
            
            var model = new DietPlanAssignmentVM
            {
               
                MemberName = memberResponse.Result.FullName,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDietPlan(DietPlanAssignmentVM model, string memberId)
        {
            if (!ModelState.IsValid)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();
                ViewBag.Member = memberResponse.Result;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                return View(model);
            }
           
            var response = await _dietPlanAssignmentService.CreateAsync(model);
            
            if (response.ISHaveErrorOrnNot)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId  );
                if (memberResponse.ISHaveErrorOrnNot)
                {

                    TempData["Error"] = "Member not found";
                    return RedirectToAction("MyMembers");
                }

                // Get available diet plans
                var dietPlansResponse = await _dietPlanService.GetAllDietPlansAsync();

                ViewBag.Member = memberResponse.Result;
                ViewBag.DietPlans = dietPlansResponse.Result ?? new List<DietPlanVM>();
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }
            
            var resulteResponse = await _subscripionService.GetByMemeberIdAsync(memberId);
            resulteResponse.Result.DietPlanAssignmentVM = new DietPlanAssignmentVM();
            resulteResponse.Result.DietPlanAssignmentVM.Id = response.Result.Id;
            
            var Updated = await _subscripionService.UpdateAsync(resulteResponse.Result);
            if (Updated.ISHaveErrorOrnNot)
            {
                var memberResponse = await _memberService.GetMemberByIdAsync(memberId);
                if (memberResponse.ISHaveErrorOrnNot)
                {
                    TempData["Error"] = "Member not found";
                    return RedirectToAction("MyMembers");
                }

                // Get available workout plans
                var workoutPlansResponse = await _workoutPlanService.GetAllWorkoutPlansAsync();

                ViewBag.Member = memberResponse.Result;
                ViewBag.WorkoutPlans = workoutPlansResponse.Result ?? new List<WorkoutPlanVM>();
                TempData["Error"] = response.ErrorMessage;
                return View(model);
            }

            TempData["Success"] = "Diet plan assigned successfully!";
            return RedirectToAction("MemberDetails", new { id = memberId });
        }
    }
}