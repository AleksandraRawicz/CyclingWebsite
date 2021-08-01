using CyclingWebsite.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Validators
{
    public class SearchFiltersValidator:AbstractValidator<SearchFilters>
    {
        public SearchFiltersValidator()
        {
            RuleFor(f => f.Page)
                .GreaterThan(0);

            RuleFor(f => f.Size)
                .GreaterThan(0);
        }
    }
}
