using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    internal class MovieService : IMovieService
    {
        private readonly IMovieRepository _repository;
        private readonly IValidator<Movie> _movieValidator;

        public MovieService(IMovieRepository repository, IValidator<Movie> movieValidator)
        {
            _repository = repository;
            _movieValidator = movieValidator;
        }

        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie);
            return await _repository.CreateAsync(movie, token);
        }


        public Task<Guid> CreateGenreAndReturnIdAsync(string genre, CancellationToken token = default)
        {
            return _repository.CreateGenreAndReturnIdAsync(genre, token);
        }

        public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            return _repository.DeleteByIdAsync(id, token);
        }

        public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
        {
            return _repository.GetAllAsync(token);
        }

        public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return _repository.GetByIdAsync(id,token);
        }

        public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
        {
            return _repository.GetBySlugAsync(slug, token);
        }

        public Task<bool> MovieExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            return (_repository.MovieExistsByIdAsync(id, token));
        }

        public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie);
            var exists = await MovieExistsByIdAsync(movie.Id, token);
            if (!exists)
            {
                return null;                
            }

            await _repository.UpdateAsync(movie, token);

            return movie;
        }
    }
}
