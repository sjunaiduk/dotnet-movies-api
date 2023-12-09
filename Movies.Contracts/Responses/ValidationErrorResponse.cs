using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Contracts.Responses
{
    public class ValidationErrorResponse
    {
        public required IEnumerable<ValidationError> Errors { get; init; }
    }

    public class ValidationError
    {
        public required string Property { get; init; }
        public required string Error { get; init; }
    }
}
