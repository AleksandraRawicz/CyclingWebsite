using CyclingWebsite.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Validators
{
    public class UserEditValidator:AbstractValidator<UserEditDto>
    {
        public UserEditValidator()
        {
            RuleFor(u => u.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(u => u.ConfirmPassword)
                .Equal(u => u.Password);

        }
    }
}
