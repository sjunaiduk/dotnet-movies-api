using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{

    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [Route(ApiEndpoints.Movies.GetRating)]
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute]Guid movieId, CancellationToken token)
        {
            var rating = await _ratingService.GetRatingAsync(movieId, token);

            if (rating is null)
            {
                return NotFound();
            }

            return Ok(rating);

        }

        [Route(ApiEndpoints.Ratings.GetUserRatings)]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll(CancellationToken token)
        {
            var userClaim = HttpContext.User.Claims.SingleOrDefault(c => c.Type == "userid");
            Guid userId = Guid.Parse(userClaim!.Value);

            var ratings = await _ratingService.GetUserRatings(userId, token);

            return Ok(ratings);
        }

        [Route(ApiEndpoints.Ratings.CreateRating)]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rate([FromBody]CreateRatingRequest request, CancellationToken token)
        {
            var userClaim = HttpContext.User.Claims.SingleOrDefault(c => c.Type == "userid");
            Guid userId = Guid.Parse(userClaim!.Value);

            var success = await _ratingService.CreateRatingAsync(request.MovieId,
                userId,
                request.Rating,
                token);

           if (!success)
            {
                return NotFound();
            }

            return Ok();

        }

        [Route(ApiEndpoints.Ratings.DeleteRating)]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteRating([FromRoute] Guid movieId, CancellationToken token)
        {
            var userClaim = HttpContext.User.Claims.SingleOrDefault(c => c.Type == "userid");
            Guid userId = Guid.Parse(userClaim!.Value);

            var success = await _ratingService.DeleteRatingAsync(movieId, userId, token);

            if (!success)
            {
                return NotFound();
            }

            return Ok();

        }
    }
}
