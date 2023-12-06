using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mappings;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{
    [ApiController]

    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpGet]
        [Route(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _movieRepository.GetAllAsync();
            var moviesResponse = movies.ToMoviesResponse();
            return Ok(moviesResponse);
        }

        [HttpGet]
        [Route(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get(string idOrSlug)
        {
            var movie = Guid.TryParse(idOrSlug, out Guid id) ? 
                await _movieRepository.GetByIdAsync(id) : 
                await _movieRepository.GetBySlugAsync(idOrSlug);

            if (movie is null)
            {
                return NotFound();
            }

            var movieResponse = movie.ToMovieResponse();


            return Ok(movieResponse);
        }

        [HttpPost]
        [Route(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create(CreateMovieRequest request)
        {
            var movie = request.ToMovie();

            await _movieRepository.CreateAsync(movie);

            var movieResponse = movie.ToMovieResponse();

            return CreatedAtAction(nameof(Get), new {idOrSlug = movie.Slug}, movieResponse);
        }

        [HttpPut]
        [Route(ApiEndpoints.Movies.Update)]

        public async Task<IActionResult> Update([FromBody] UpdateMovieRequest request,
            [FromRoute] Guid id)
        {
            var movie = request.ToMovie(id);
            var updated = await _movieRepository.UpdateAsync(movie);
            if (!updated)
            {
                return NotFound();
            }

            return Ok(movie.ToMovieResponse());
        }

        [HttpDelete]
        [Route(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var deleted = await _movieRepository.DeleteByIdAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
