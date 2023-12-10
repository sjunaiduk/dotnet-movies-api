using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Contracts.Responses
{
    public class RatingsResponse
    {
        public required Guid MovieId { get; init; }

        public required string Slug { get; init; }

        public float? Ratings { get; init; }

        public int? UserRating { get; init; }
    }
}
