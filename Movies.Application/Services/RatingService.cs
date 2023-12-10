using Movies.Application.Models;
using Movies.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    internal class RatingService : IRatingService
    {
        private readonly IRatingRepository _repository;

        public RatingService(IRatingRepository repository)
        {
            _repository = repository;
        }

        public Task<bool> CreateRatingAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default)
        {
            return _repository.CreateRatingAsync(movieId,
                userId,
                rating,
                token);
        }

        public Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
        {
            return _repository.DeleteRatingAsync(movieId,
                userId,
                 token);
        }

        public  Task<RatingModel?> GetRatingAsync(Guid movieId, CancellationToken token = default)
        {
            return _repository.GetRatingAsync(movieId, token);
        }

        public Task<IEnumerable<RatingModel>> GetUserRatings(Guid userId, CancellationToken token = default)
        {
            return _repository.GetUserRatings(userId, token);
        }
    }
}
