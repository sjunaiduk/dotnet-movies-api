using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Models
{
    public class MoviesOptions
    {
        public required string? Title { get; init; }
        public required int? YearOfRelease { get; init; }

        public Guid? UserId { get; set; }

        public required string? SortField { get; init ; }

        public required SortOrder SortOrder { get; init ; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }

    public enum SortOrder
    {
        Ascending,
        Descending,
        Unsorted
    }
}
