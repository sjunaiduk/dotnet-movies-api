using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    public interface IRatingService
    {
        Task<bool> CreateRatingAsync(Guid movieId, Guid? userId, int rating, CancellationToken token = default);
        Task<bool> DeleteRatingAsync(Guid movieId, Guid? userId, CancellationToken token = default);

        Task<MovieRating?> GetRatingAsync(Guid movieId, CancellationToken token = default);

        Task<IEnumerable<MovieRating>> GetUserRatings(Guid? userId, CancellationToken token = default);
    }
}
