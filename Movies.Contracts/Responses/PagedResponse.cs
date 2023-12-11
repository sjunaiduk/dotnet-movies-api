using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Contracts.Responses
{
    public class PagedResponse<T>
    {
        public required IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
        public required int Page { get; init; }
        public required int PageSize { get; init; }

        public required int TotalItems { get; init; }

        public bool HasMorePages => TotalItems > Page * PageSize;

    }
}
