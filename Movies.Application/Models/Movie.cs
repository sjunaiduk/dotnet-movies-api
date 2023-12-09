using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Movies.Application.Models
{
    public class Movie
    {
        public required Guid Id { get; init; }
        public required string Title { get; set; }
        public required int YearOfRelease { get; set; }

        public string Slug => ComputeSlug();
        public required List<string> Genres { get; init; } = new();

        private string ComputeSlug()
        {
            // Convert to lowercase
            var slug = Title.ToLowerInvariant();

            // Remove all special characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-");

            return $"{slug}-{YearOfRelease}";
        }
    }
}
