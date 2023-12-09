using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validations
{
    public class MovieValidator : AbstractValidator<Movie>
    {
        private readonly IMovieRepository _repository;
        public MovieValidator(IMovieRepository repository)
            
        {
            _repository = repository;
            RuleFor(m => m.Id)
                .NotEmpty();

            RuleFor(m => m.Title)
                .NotEmpty();

            RuleFor(m => m.Genres)
                .NotEmpty();

            RuleFor(m => m.YearOfRelease)
                .LessThanOrEqualTo(DateTime.UtcNow.Year);

            RuleFor(m => m.Slug)
                .MustAsync(ValidateSlug)
                .WithMessage("Movie already exists in the database");
            
           }

        private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token)
        {
            var existingMovie = await _repository.GetBySlugAsync(slug);

            if (existingMovie is not null)
            {
                return existingMovie.Id == movie.Id;
            }

            return existingMovie is null;
        }
    }
}
