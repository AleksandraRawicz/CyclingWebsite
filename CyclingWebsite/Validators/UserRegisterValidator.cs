using CyclingWebsite.Entities;
using CyclingWebsite.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Validators
{
    public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterValidator(WebsiteDbContext _context)
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(p => p.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(p => p.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(p => p.ConfirmPassword)
                .Equal(c => c.Password);

            RuleFor(p => p.Email)
                .Custom((value, context) =>
                {
                    bool emailInUse = _context.Users.Any(u => u.Email == value);
                    if(emailInUse)
                    {
                        context.AddFailure("Email", "That email is already taken");
                    }    
                });

            RuleFor(p => p.Name)
                .Custom((value, context) =>
                {
                    bool loginInUse = _context.Users.Any(u => u.Name == value);
                    if (loginInUse)
                    {
                        context.AddFailure("Login", "That login is already taken");
                    }
                });

        }
    }
}
