using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Communication;
using GymBLL.Service.Abstract.Communication;
using AutoMapper;
using GymBLL.Service.Abstract;
using GymDAL.Enums;
using GymDAL.Entities.Communication;
using GymDAL.Repo.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace GymBLL.Service.Implementation.Communication
{
    public class NotificationService : INotificationService
    {
        public IMapper Mapper
        {
            get;
        }
        public IUnitOfWork UnitOfWork
        {
            get;
        }
        private readonly IRealTimeNotificationService _realTimeService; // Added this field

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IRealTimeNotificationService realTimeService) // Modified constructor
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            _realTimeService = realTimeService; // Assigned the new service
        }
        public async Task<Response<NotificationVM>> CreateAsync(NotificationVM model)
        {
            try
            {

                // Manual mapping for enums
                var notification = new Notification
                {


                    UserId = model.UserId,
                    Message = model.Message,
                    SendTime = model.SendTime,
                    RelatedEntity = model.RelatedEntity,
                    RelatedEntityId = model.RelatedEntityId,
                    IsActive = model.IsActive,
                    // Map string to enum with safe parsing
                    Type = Enum.TryParse<NotificationType>(model.Type, out var type) ? type : NotificationType.SystemAlert,
                    // Map string to enum with safe parsing
                    Status = Enum.TryParse<NotificationStatus>(model.Status, out var status) ? status : NotificationStatus.Unread,
                    // Map string to nullable enum
                    DeliveryMethod = !string.IsNullOrEmpty(model.DeliveryMethod) && Enum.TryParse<DeliveryMethod>(model.DeliveryMethod, out var deliveryMethod) ? deliveryMethod : null
                };


                await UnitOfWork.Notifications.AddAsync(notification);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    model.Id = notification.Id;
                    
                    // Push real-time notification
                    await _realTimeService.PushNotificationAsync(model.UserId, model.Message); // Added this line

                    return new Response<NotificationVM>(model, null, false);
                }
                return new Response<NotificationVM>(null, "Failed to create notification.", true);
            }
            catch (Exception ex)
            {
                return new Response<NotificationVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<NotificationVM>> GetByIdAsync(int id)
        {
            try
            {
                var notification = await UnitOfWork.Notifications.GetByIdAsync(id);
                if (notification != null)
                {
                    var vm = Mapper.Map<NotificationVM>(notification);
                    return new Response<NotificationVM>(vm, null, false);
                }
                return new Response<NotificationVM>(null, "Notification not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<NotificationVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<NotificationVM>>> GetAllAsync()
        {
            try
            {
                var notifications = await UnitOfWork.Notifications.GetAllAsync();
                var vms = notifications.Select(n => Mapper.Map<NotificationVM>(n)).ToList();
                return new Response<List<NotificationVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<NotificationVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<NotificationVM>>> GetByUserIdAsync(string userId)
        {
            try
            {
                var notifications = await UnitOfWork.Notifications.FindAsync(n => n.UserId == userId);
                var vms = notifications.Select(n => Mapper.Map<NotificationVM>(n)).ToList();
                return new Response<List<NotificationVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<NotificationVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<NotificationVM>>> GetUnreadByUserIdAsync(string userId)
        {
            try
            {
                var notifications = await UnitOfWork.Notifications.FindAsync(n => n.UserId == userId && n.Status == NotificationStatus.Unread);
                var vms = notifications.Select(n => Mapper.Map<NotificationVM>(n)).ToList();
                return new Response<List<NotificationVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<NotificationVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<NotificationVM>> UpdateAsync(NotificationVM model)
        {
            try
            {
                var notification = await UnitOfWork.Notifications.GetByIdAsync(model.Id);
                if (notification == null) return new Response<NotificationVM>(null, "Notification not found.", true);
                notification.Message = model.Message;
                notification.Status = Enum.Parse<NotificationStatus>(model.Status);
                notification.IsActive = model.IsActive;
                UnitOfWork.Notifications.Update(notification);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<NotificationVM>(model, null, false);
                return new Response<NotificationVM>(null, "Failed to update notification.", true);
            }
            catch (Exception ex)
            {
                return new Response<NotificationVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> MarkAsReadAsync(int id)
        {
            try
            {
                var notification = await UnitOfWork.Notifications.GetByIdAsync(id);
                if (notification == null) return new Response<bool>(false, "Notification not found.", true);
                notification.Status = NotificationStatus.Read;
                UnitOfWork.Notifications.Update(notification);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to mark as read.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var notification = await UnitOfWork.Notifications.GetByIdAsync(id);
                if (notification == null) return new Response<bool>(false, "Notification not found.", true);
                UnitOfWork.Notifications.Remove(notification);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to delete notification.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> ToggleStatusAsync(int id)
        {
            try
            {
                var notification = await UnitOfWork.Notifications.GetByIdAsync(id);
                if (notification == null) return new Response<bool>(false, "Notification not found.", true);
                notification.ToggleStatus();
                UnitOfWork.Notifications.Update(notification);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to toggle status.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }
    }
}



