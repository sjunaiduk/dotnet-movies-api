using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mappings;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{
    [ApiController]
    
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        [Route(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery]MoviesOptionsRequest request,
            CancellationToken token)
        {

            var userId = HttpContext.GetUser();
            var options = request.ToMoviesOptions()
                .AddUser(userId);
            var movies = await _movieService.GetAllAsync(options, token);
            var totalItems = await _movieService.TotalItems(options, token);
            var moviesResponse = movies.ToMoviesResponse(request.Page,request.PageSize,totalItems);
            return Ok(moviesResponse);
        }

        [HttpGet]
        [Route(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get(string idOrSlug, CancellationToken token)
        {
            var userId = HttpContext.GetUser();


            var movie = Guid.TryParse(idOrSlug, out Guid id) ? 
                await _movieService.GetByIdAsync(id,userId, token) : 
                await _movieService.GetBySlugAsync(idOrSlug,userId, token);

            if (movie is null)
            {
                return NotFound();
            }

            var movieResponse = movie.ToMovieResponse();


            return Ok(movieResponse);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost]
        [Route(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create(CreateMovieRequest request, CancellationToken token)
        {
            var movie = request.ToMovie();

            await _movieService.CreateAsync(movie, token);

            var movieResponse = movie.ToMovieResponse();

            return CreatedAtAction(nameof(Get), new {idOrSlug = movie.Slug}, movieResponse);
        }

        [HttpPut]
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [Route(ApiEndpoints.Movies.Update)]

        public async Task<IActionResult> Update([FromBody] UpdateMovieRequest request,
            [FromRoute] Guid id, CancellationToken token)
        {
            var movie = request.ToMovie(id);
            var updatedMovie = await _movieService.UpdateAsync(movie, token);
            if (updatedMovie is null)
            {
                return NotFound();
            }

            return Ok(updatedMovie.ToMovieResponse());
        }

        [HttpDelete]
        [Authorize(AuthConstants.AdminUserPolicyName)]
        [Route(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute]Guid id, CancellationToken token)
        {
            var deleted = await _movieService.DeleteByIdAsync(id, token);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
