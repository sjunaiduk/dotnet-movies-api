using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mappings
{
    public static class ContractMappings
    {
        public static Movie ToMovie(this CreateMovieRequest request)
        {
            return new Movie
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Genres = request.Genres.ToList(),
            };
        }

        public static Movie ToMovie(this UpdateMovieRequest request, Guid id)
        {
            return new Movie
            {
                Id = id,
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Genres = request.Genres.ToList(),
            };
        }

        public static MovieResponse ToMovieResponse(this Movie movie)
        {
            return new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                YearOfRelease = movie.YearOfRelease,
                Genres = movie.Genres,
                Slug = movie.Slug,
                Rating = movie.Rating,
                UserRating = movie.UserRating,
            };
        }

        public static MoviesResponse ToMoviesResponse(this IEnumerable<Movie> movies,
            int page,
            int pageSize,
            int totalItems)
        {
            return new MoviesResponse
            {
                Items = movies.Select(movie => movie.ToMovieResponse()),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
                
            };
        }

        public static MoviesOptions ToMoviesOptions(this MoviesOptionsRequest options)
        {
            return new MoviesOptions
            {
                Title = options.Title,
                YearOfRelease = options.YearOfRelease,
                SortOrder = options.SortField is null ?
                SortOrder.Unsorted :
                options.SortField.StartsWith('-') ?
                SortOrder.Descending :
                SortOrder.Ascending,
                SortField = options.SortField?.Trim('+','-'),
                PageSize = options.PageSize,
                Page = options.Page
          

            };
        }

        public static MoviesOptions AddUser(this MoviesOptions options,
            Guid? userId)
        {
            options.UserId = userId;
            return options;
        }
    }
}
