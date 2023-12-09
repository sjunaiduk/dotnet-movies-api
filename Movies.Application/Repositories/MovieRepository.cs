using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    internal class MovieRepository : IMovieRepository
    {
        private readonly List<Movie> _movies = new List<Movie>();
        public Task<bool> CreateAsync(Movie movie)
        {
            _movies.Add(movie);
            return Task.FromResult(true);
        }

        public Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateGenreAndReturnIdAsync(string genre)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateGenreAndReturnIdAsync(string genre, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(Guid id)
        {
            var count = _movies.RemoveAll(x => x.Id == id);
            var movieRemoved = count > 0;
            return Task.FromResult(movieRemoved);
        }

        public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Movie>> GetAllAsync()
        {
            return Task.FromResult(_movies.AsEnumerable());
        }

        public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Movie?> GetByIdAsync(Guid id)
        {
            var movie = _movies.SingleOrDefault(x => x.Id == id);

            return Task.FromResult(movie);
        }

        public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<Movie?> GetBySlugAsync(string slug)
        {
            var movie = _movies.SingleOrDefault(x => x.Slug == slug);

            return Task.FromResult(movie);
        }

        public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MovieExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Movie movie)
        {
            var index = _movies.FindIndex(m => m.Id == movie.Id);
            if (index == -1)
            {
                return Task.FromResult(false);
            }

            _movies[index] = movie;

            return Task.FromResult(true);
        }

        public Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
