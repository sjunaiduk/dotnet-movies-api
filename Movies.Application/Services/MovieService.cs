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
        private readonly IValidator<MoviesOptions> _optionsValidator;

        public MovieService(IMovieRepository repository, IValidator<Movie> movieValidator, IValidator<MoviesOptions> optionsValidator)
        {
            _repository = repository;
            _movieValidator = movieValidator;
            _optionsValidator = optionsValidator;
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

        public async Task<IEnumerable<Movie>> GetAllAsync(MoviesOptions options, CancellationToken token = default)
        {
            await _optionsValidator.ValidateAndThrowAsync(options);
            return await _repository.GetAllAsync(options, token);
        }

        public Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
        {
            return _repository.GetByIdAsync(id,userId, token);
        }

        public Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
        {
            return _repository.GetBySlugAsync(slug, userId, token);
        }

        public Task<bool> MovieExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            return (_repository.MovieExistsByIdAsync(id, token));
        }

        public Task<int> TotalItems(MoviesOptions options, CancellationToken token)
        {
            return _repository.TotalItems(options, token);
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
