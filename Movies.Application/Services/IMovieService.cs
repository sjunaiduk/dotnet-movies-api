using Movies.Application.Models;

namespace Movies.Application.Services
{
    public interface IMovieService
    {
        Task<Movie?> GetByIdAsync(Guid id,Guid? userId = default, CancellationToken token = default);

        Task<Movie?> GetBySlugAsync(string slug,Guid? userId = default, CancellationToken token = default);


        Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

        Task<IEnumerable<Movie>> GetAllAsync(MoviesOptions options , CancellationToken token = default);

        Task<Movie?> UpdateAsync(Movie movie, CancellationToken token = default);

        Task<bool> CreateAsync(Movie movie, CancellationToken token = default);

        Task<bool> MovieExistsByIdAsync(Guid id, CancellationToken token = default);

        Task<Guid> CreateGenreAndReturnIdAsync(string genre, CancellationToken token = default);

        Task<int> TotalItems(MoviesOptions options, CancellationToken token);

    }
}