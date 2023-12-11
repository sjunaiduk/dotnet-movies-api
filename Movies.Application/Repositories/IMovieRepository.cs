using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    public interface IMovieRepository
    {
        Task<Movie?> GetByIdAsync(Guid id,Guid? userId = default, CancellationToken token = default);

        Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);


        Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

        Task<IEnumerable<Movie>> GetAllAsync(MoviesOptions options, CancellationToken token = default);

        Task<bool> UpdateAsync(Movie movie, CancellationToken token = default);

        Task<bool> CreateAsync(Movie movie, CancellationToken token = default);

        Task<bool> MovieExistsByIdAsync(Guid id, CancellationToken token = default);

        Task<Guid> CreateGenreAndReturnIdAsync(string genre, CancellationToken token = default);

        Task<int> TotalItems(MoviesOptions options, CancellationToken token = default);


    }
}
