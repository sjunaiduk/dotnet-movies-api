using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Contracts.Requests
{
    public class CreateRatingRequest
    {
        public required Guid MovieId { get; init; }
        public required int Rating { get; init; }
    }
}
