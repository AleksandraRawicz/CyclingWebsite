using CyclingWebsite.Entities;
using CyclingWebsite.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;




namespace CyclingWebsite.Services
{
    public interface IAccountService
    {
        public Task<bool> LoginAsync(UserLoginDto dto);
        string Register(UserRegisterDto dto);
        public bool Edit(UserEditDto dto);
        public Task LogOutAsync();
        public bool ConfirmEmail(string token);
    }

    public class AccountService : IAccountService
    {
        private readonly WebsiteDbContext _context;
        private readonly IPasswordHasher<User> _hasher;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IEmailSender _emailService;

        public AccountService(WebsiteDbContext context, IPasswordHasher<User> hasher, AuthenticationSettings authenticationSettings,
           IHttpContextAccessor httpAccessor, IEmailSender emailSender)
        {
            _context = context;
            _hasher = hasher;
            _authenticationSettings = authenticationSettings;
            _httpAccessor = httpAccessor;
            _emailService = emailSender;
        }

        public string Register(UserRegisterDto dto)
        {
            var newUser = new User()
            {
                Name = dto.Name,
                Email = dto.Email,
                RoleId = dto.RoleId,
                EmailConfirmed = false
            };

            var hash = _hasher.HashPassword(newUser, dto.Password);
            newUser.HashedPassword = hash;

            _context.Users.Add(newUser);
            _context.SaveChanges();

            newUser = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            string token = GenerateToken(newUser);
            newUser.SecurityToken = token;

            _context.Users.Update(newUser);
            _context.SaveChanges();

            return token;
        }

        public bool ConfirmEmail(string token)
        {         
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));

            var key2 = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKeyEnc));

            var tokenValidationParamteres = new TokenValidationParameters()
            {
                IssuerSigningKey = key,
                TokenDecryptionKey = key2,
                ValidAudience = _authenticationSettings.JwtIssuer,
                ValidIssuer =_authenticationSettings.JwtIssuer
                        };
            SecurityToken validated;
            var identity = tokenHandler.ValidateToken(token, tokenValidationParamteres, out validated);
            var expir = validated.ValidFrom.Date + " " + validated.ValidTo.Date;

            int? userId = (int?)int.Parse(identity.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user is null)
                return false;

            user.EmailConfirmed = true;
            _context.Users.Update(user);
            _context.SaveChanges();
            return true;        
        }

        public async Task<bool> LoginAsync(UserLoginDto dto)
        {
            var email = dto.Email;
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email);

            if (user is null)
                return false;

            var result = _hasher.VerifyHashedPassword(user, user.HashedPassword, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return false;

            if (user.EmailConfirmed == false)
                return false;

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await AuthenticationHttpContextExtensions.SignInAsync(_httpAccessor.HttpContext, new ClaimsPrincipal(claimsIdentity));
            return true;
        }

        public async Task LogOutAsync()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(_httpAccessor.HttpContext);
        }

        public bool Edit(UserEditDto dto)
        {
            var id = int.Parse(_httpAccessor.HttpContext.User.Claims.FirstOrDefault(
                c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            var result =  _hasher.VerifyHashedPassword(user, user.HashedPassword, dto.OldPassword);

            if (result == PasswordVerificationResult.Failed)
                return false;

            var newHashedPassword = _hasher.HashPassword(user, dto.Password);
            user.HashedPassword = newHashedPassword;
            _context.Users.Update(user);
            _context.SaveChanges();

            return true;
        }

        private string GenerateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddDays(_authenticationSettings.ExpirationTime);


            var key2 = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKeyEnc));
            var enccredentials = new EncryptingCredentials(key2, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer,
                new ClaimsIdentity(claims),null, expiration, null, credentials, enccredentials);

            return tokenHandler.WriteToken(token);
        }
    }
}
