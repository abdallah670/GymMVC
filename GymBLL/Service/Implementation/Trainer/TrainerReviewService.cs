using AutoMapper;
using GymBLL.Common;
using GymBLL.ModelVM.Trainer;
using GymBLL.Service.Abstract.Trainer;
using GymDAL.Entities.Users;
using GymDAL.Repo.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation.Trainer
{
    public class TrainerReviewService : ITrainerReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Response<bool>> CreateReviewAsync(TrainerReviewVM model, string memberId)
        {
            try
            {
                // Check if already reviewed
                var existingReview = await _unitOfWork.TrainerReviews
                    .Get(r => r.TrainerId == model.TrainerId && r.MemberId == memberId)
                    .FirstOrDefaultAsync();

                if (existingReview != null)
                {
                    return new Response<bool>(false, "You have already reviewed this trainer.", true);
                }

                var review = new TrainerReview
                {
                    TrainerId = model.TrainerId,
                    MemberId = memberId,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.TrainerReviews.AddAsync(review);
                var result = await _unitOfWork.SaveAsync();

                return new Response<bool>(result > 0, null, result <= 0);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error creating review: {ex.Message}", true);
            }
        }

        public async Task<Response<List<TrainerReviewVM>>> GetReviewsForTrainerAsync(string trainerId)
        {
            try
            {
                var reviews = await _unitOfWork.TrainerReviews
                    .Get(r => r.TrainerId == trainerId)
                    .Include(r => r.Member)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                var viewModels = reviews.Select(r => new TrainerReviewVM
                {
                    Id = r.Id,
                    TrainerId = r.TrainerId,
                    MemberId = r.MemberId,
                    MemberName = r.Member?.FullName ?? "Unknown",
                    MemberPicture = r.Member?.ProfilePicture,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList();

                return new Response<List<TrainerReviewVM>>(viewModels, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<TrainerReviewVM>>(null, $"Error fetching reviews: {ex.Message}", true);
            }
        }

        public async Task<Response<TrainerReviewVM>> GetReviewByMemberAsync(string memberId, string trainerId)
        {
            try
            {
                var review = await _unitOfWork.TrainerReviews
                    .GetDetailed(memberId, trainerId);
                if (review == null)
                    return new Response<TrainerReviewVM>(null, "No review found.", false);

                var viewModel = new TrainerReviewVM
                {
                    Id = review.Id,
                    TrainerId = review.TrainerId,
                    MemberId = review.MemberId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt
                };

                return new Response<TrainerReviewVM>(viewModel, null, false);
            }
            catch (Exception ex)
            {
                return new Response<TrainerReviewVM>(null, $"Error fetching review: {ex.Message}", true);
            }
        }

        public async Task<Response<double>> GetAverageRatingAsync(string trainerId)
        {
            try
            {
                var reviews = await _unitOfWork.TrainerReviews
                    .Get(r => r.TrainerId == trainerId).Take(3)
                    .ToListAsync();

                if (!reviews.Any())
                    return new Response<double>(0.0, null, false);

                var average = reviews.Average(r => r.Rating);
                return new Response<double>(average, null, false);
            }
            catch (Exception ex)
            {
                return new Response<double>(0.0, $"Error calculating average: {ex.Message}", true);
            }
        }

        public  async Task<Response<List<TrainerReviewVM>>> GetTop3ReviewsForTrainerAsync(string trainerId)
        {
            try
            {
                var reviews = await _unitOfWork.TrainerReviews
                    .Get(r => r.TrainerId == trainerId).Take(3)
                    .Include(r => r.Member)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                var viewModels = reviews.Select(r => new TrainerReviewVM
                {
                    Id = r.Id,
                    TrainerId = r.TrainerId,
                    MemberId = r.MemberId,
                    MemberName = r.Member?.FullName ?? "Unknown",
                    MemberPicture = r.Member?.ProfilePicture,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList();

                return new Response<List<TrainerReviewVM>>(viewModels, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<TrainerReviewVM>>(null, $"Error fetching reviews: {ex.Message}", true);
            }

        }

        public async Task<Response<List<TrainerReviewVM>>> GetTop3ReviewsAsync()
        {
            try
            {
                var reviews = await _unitOfWork.TrainerReviews
                    .Get().Take(3)
                    .Include(r => r.Member)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                var viewModels = reviews.Select(r => new TrainerReviewVM
                {
                    Id = r.Id,
                    TrainerId = r.TrainerId,
                    MemberId = r.MemberId,
                    MemberName = r.Member?.FullName ?? "Unknown",
                    MemberPicture = r.Member?.ProfilePicture,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    JoinDate = r.Member?.JoinDate
                }).ToList();

                return new Response<List<TrainerReviewVM>>(viewModels, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<TrainerReviewVM>>(null, $"Error fetching reviews: {ex.Message}", true);
            }
        }
    }
}
