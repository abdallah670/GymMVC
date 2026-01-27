using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Financial;
using GymBLL.Service.Abstract.Financial;
using AutoMapper;
using GymBLL.Service.Abstract;
using GymDAL.Entities.Financial;
using GymDAL.Repo.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace GymBLL.Service.Implementation.Financial
{
    public class PaymentService : IPaymentService
    {
        public IUnitOfWork UnitOfWork { get; }
        public IMapper Mapper { get; }

        private readonly IStripeService _stripeService;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IStripeService stripeService)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            _stripeService = stripeService;
        }

        public async Task<Response<PaymentVM>> CreateAsync(PaymentVM model)
        {
            try
            {
                // Create Stripe Payment Intent if not already created
                if (string.IsNullOrEmpty(model.TransactionId))
                {
                    var clientSecret = await _stripeService.CreatePaymentIntentAsync((double)model.Amount, model.Currency ?? "usd", model.Description ?? "Gym Membership");
                    model.TransactionId = clientSecret; // Temporarily store client secret here for the view to pick up
                }

                var payment = Mapper.Map<Payment>(model);
                payment.PaymentType = model.PaymentType ?? "Membership";
                payment.Status = "Pending";
                
                await UnitOfWork.Payments.AddAsync(payment);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                {
                    model.Id = payment.Id;
                    return new Response<PaymentVM>(model, null, false);
                }
                return new Response<PaymentVM>(null, "Failed to create payment record.", true);
            }
            catch (Exception ex)
            {
                return new Response<PaymentVM>(null, $"Stripe Error: {ex.Message}", true);
            }
        }
        public async Task<Response<PaymentVM>> GetByIdAsync(int id)
        {
            try
            {
                var payment = await UnitOfWork.Payments.GetByIdAsync(id);
                if (payment != null)
                {
                    var vm = Mapper.Map<PaymentVM>(payment);
                    return new Response<PaymentVM>(vm, null, false);
                }
                return new Response<PaymentVM>(null, "Payment not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<PaymentVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<PaymentVM>>> GetAllAsync()
        {
            try
            {
                var payments = await UnitOfWork.Payments.GetAllAsync();
                var vms = payments.Select(p => Mapper.Map<PaymentVM>(p)).ToList();
                return new Response<List<PaymentVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<PaymentVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<PaymentVM>>> GetByMemberIdAsync(string memberId)
        {
            try
            {
                var payments = await UnitOfWork.Payments.FindAsync(p => p.MemberId == memberId);
                var vms = payments.Select(p => Mapper.Map<PaymentVM>(p)).ToList();
                return new Response<List<PaymentVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<PaymentVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<PaymentVM>>> GetByStatusAsync(string status)
        {
            try
            {
                var payments = await UnitOfWork.Payments.FindAsync(p => p.Status == status);
                var vms = payments.Select(p => Mapper.Map<PaymentVM>(p)).ToList();
                return new Response<List<PaymentVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<PaymentVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<PaymentVM>> UpdateAsync(PaymentVM model)
        {
            try
            {
                var payment = await UnitOfWork.Payments.GetByIdAsync(model.Id);
                if (payment == null) return new Response<PaymentVM>(null, "Payment not found.", true);
                payment.PaymentType = model.PaymentType;
                payment.Description = model.Description;
                payment.Amount = model.Amount;
                payment.Currency = model.Currency;
                payment.PaymentMethod = model.PaymentMethod;
                payment.TransactionId = model.TransactionId;
                payment.Status = model.Status;
                payment.Notes = model.Notes;
                payment.BillingName = model.BillingName;
                payment.BillingAddress = model.BillingAddress;
                payment.BillingEmail = model.BillingEmail;
                UnitOfWork.Payments.Update(payment);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<PaymentVM>(model, null, false);
                return new Response<PaymentVM>(null, "Failed to update payment.", true);
            }
            catch (Exception ex)
            {
                return new Response<PaymentVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> ProcessPaymentAsync(int id)
        {
            try
            {
                var payment = await UnitOfWork.Payments.GetByIdAsync(id);
                if (payment == null) return new Response<bool>(false, "Payment not found.", true);
                payment.Status = "Completed";
                payment.ProcessedDate = DateTime.UtcNow;
                UnitOfWork.Payments.Update(payment);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to process payment.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> RefundPaymentAsync(int id)
        {
            try
            {
                var payment = await UnitOfWork.Payments.GetByIdAsync(id);
                if (payment == null) return new Response<bool>(false, "Payment not found.", true);
                payment.Status = "Refunded";
                UnitOfWork.Payments.Update(payment);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to refund payment.", true);
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
                var payment = await UnitOfWork.Payments.GetByIdAsync(id);
                if (payment == null) return new Response<bool>(false, "Payment not found.", true);
                UnitOfWork.Payments.Remove(payment);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to delete payment.", true);
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
                var payment = await UnitOfWork.Payments.GetByIdAsync(id);
                if (payment == null) return new Response<bool>(false, "Payment not found.", true);
                payment.ToggleStatus();
                UnitOfWork.Payments.Update(payment);
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

