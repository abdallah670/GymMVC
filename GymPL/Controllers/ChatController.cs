using GymBLL.ModelVM.Communication;
using GymBLL.Service.Abstract.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymDAL.Repo.Abstract;
using System.Security.Claims;
using System.Threading.Tasks;
using GymPL.Extensions;

namespace GymPL.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IUnitOfWork _unitOfWork;

        public ChatController(IChatService chatService, IUnitOfWork unitOfWork)
        {
            _chatService = chatService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? otherUserId, int skip = 0, int take = 50)
        {
            var currentUserId = User.GetUserId();
            
            if (User.IsInRole("Trainer") && string.IsNullOrEmpty(otherUserId))
            {
                var conversations = await _chatService.GetRecentConversationsAsync(currentUserId);
                return View("Inbox", conversations.Result);
            }

            if (string.IsNullOrEmpty(otherUserId))
            {
                if (User.IsInRole("Member"))
                {
                    // Try to find the last person they chatted with
                    var conversations = await _chatService.GetRecentConversationsAsync(currentUserId);
                    if (conversations.Result != null && conversations.Result.Any())
                    {
                        var lastPartnerId = conversations.Result.First().OtherUserId;
                        return RedirectToAction("Index", new { otherUserId = lastPartnerId });
                    }

                    // No history, find the first available trainer
                    var trainers = await _unitOfWork.Trainers.GetAllAsync();
                    if (trainers != null && trainers.Any())
                    {
                        var firstTrainerId = trainers.First().Id;
                        return RedirectToAction("Index", new { otherUserId = firstTrainerId });
                    }
                }

                // Fallback if no partner found or trainer without ID
                return RedirectToAction("Dashboard", User.IsInRole("Trainer") ? "Trainer" : "Member");
            }

            var history = await _chatService.GetChatHistoryAsync(currentUserId, otherUserId, skip, take);
            ViewBag.OtherUserId = otherUserId;
            ViewBag.Skip = skip + take;
            ViewBag.Take = take;
            
            var otherUser = await _unitOfWork.ApplicationUsers.GetByIdAsync(otherUserId);
            if (otherUser != null)
            {
                ViewBag.OtherUserName = otherUser.FullName;
                ViewBag.OtherUserPicture = otherUser.ProfilePicture;
            }
            
            return View(history.Result);
        }

        [HttpGet]
        public async Task<IActionResult> GetChatHistory(string otherUserId, int skip, int take = 50)
        {
            var currentUserId = User.GetUserId();
            var response = await _chatService.GetChatHistoryAsync(currentUserId, otherUserId, skip, take);
            return Json(new { success = !response.ISHaveErrorOrnNot, messages = response.Result });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] ChatMessageVM model, IFormFile? attachment)
        {
            model.SenderId = User.GetUserId();
            
            if (attachment != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(attachment.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "chat");
                var filePath = Path.Combine(folderPath, fileName);
                
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }

                model.AttachmentUrl = "/uploads/chat/" + fileName;
                model.AttachmentType = GetAttachmentType(attachment.ContentType);
            }

            var response = await _chatService.SendMessageAsync(model);
            
            if (response.ISHaveErrorOrnNot)
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }

            return Json(new { success = true, result = response.Result });
        }

        private string GetAttachmentType(string contentType)
        {
            if (contentType.StartsWith("image/")) return "image";
            if (contentType.StartsWith("video/")) return "video";
            return "file";
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadMessages()
        {
            var userId = User.GetUserId();
            var response = await _chatService.GetUnreadMessagesAsync(userId);
            return Json(new { success = !response.ISHaveErrorOrnNot, messages = response.Result });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead([FromQuery] int messageId)
        {
            var response = await _chatService.MarkAsReadAsync(messageId);
            return Json(new { success = !response.ISHaveErrorOrnNot });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsDelivered([FromQuery] int messageId)
        {
            var response = await _chatService.MarkAsDeliveredAsync(messageId);
            return Json(new { success = !response.ISHaveErrorOrnNot });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMessage([FromQuery] int messageId)
        {
            var userId = User.GetUserId();
            var response = await _chatService.DeleteMessageAsync(messageId, userId);
            
            if (response.ISHaveErrorOrnNot)
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }
            
            return Json(new { success = true });
        }
    }
}
