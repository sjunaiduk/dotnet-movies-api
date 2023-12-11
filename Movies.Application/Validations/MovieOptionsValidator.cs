using FluentValidation;
using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Validations
{
    public class MovieOptionsValidator : AbstractValidator<MoviesOptions>
    {

        private string[] sortFields =
        {
            "yearofrelease",
            "title"
        };
        public MovieOptionsValidator()
        {
            RuleFor(x => x.SortField)
                .Must(x => x == null || sortFields.Contains(x))
                .WithMessage("You can only sort by fields 'title' and 'yearofrelease'");
        }
    }
}
