using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Models
{
    public class RatingModel
    {
        public required Guid Id { get; init; }

        public required string Slug { get; init; }

        public float? Rating { get; init; }

        public int? UserRating { get; init; }
    }
}
