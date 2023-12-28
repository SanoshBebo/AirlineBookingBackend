using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SanoshAirlines.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace SanoshAirlines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AirlineDbContext _context;
        IConfiguration _configuration;

        public AuthController(AirlineDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody] RegistrationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_context.Users.FirstOrDefault(u => u.Email == model.Email) != null)
                    {
                        return BadRequest("Email already exists");
                    }
                    
                    if (_context.Users.FirstOrDefault(u => u.Role == model.Role) != null && model.Role == "admin")
                    {
                        return BadRequest("Admin Account Cannot Be Created");
                    }
                    var userId = Guid.NewGuid();

                    var user = new User
                    {
                        UserId = userId,
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password,
                        Role = model.Role
                    };


                    string fromMail = "businessreports8@gmail.com";
                    string fromPassword = "dmvibdlolcdpmavr";

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(fromMail);
                    message.Subject = "Welcome to Red Sparrow!";
                    message.To.Add(new MailAddress($"{model.Email}"));
                    message.Body = EmailTemplates.GetWelcomeEmailBody(model.Name);
                    message.IsBodyHtml = true;

                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(fromMail, fromPassword),
                        EnableSsl = true,
                    };


                    smtpClient.Send(message);


                    _context.Users.Add(user);
                    _context.SaveChanges();

                    return Ok(model);
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    return BadRequest("User Acount Does Not Exist");
                }


                if (model.Password != user.Password && ConvertToEncrypt(model.Password) != user.Password)
                {
                    return BadRequest("Invalid Credentials");
                }

                var issuer = _configuration["Jwt:ValidIssuer"];
                var audience = _configuration["Jwt:ValidAudience"];
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:key"]);
                var signingCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

                var subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,model.Email),
                     new Claim(JwtRegisteredClaimNames.Email,model.Email),
                });

                var expires = DateTime.UtcNow.AddMinutes(10);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = subject,
                    Expires = expires,
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = signingCredentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                SetJWT(jwtToken);

                return Ok(new { Token = jwtToken, User = user });
            }

            return BadRequest(model + "Failed to login");
        }

        private static string Key = "ajlnlsg@DK%&";

        private static string ConvertToEncrypt(string password)
        {
            if (string.IsNullOrEmpty(password)) return "";
            password += Key;
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(passwordBytes);
        }

        private void SetJWT(string encrypterToken)
        {

            HttpContext.Response.Cookies.Append("X-Access-Token", encrypterToken,
                  new CookieOptions
                  {
                      Expires = DateTime.Now.AddMinutes(15),
                      HttpOnly = true,
                      Secure = true,
                      IsEssential = true,
                      SameSite = SameSiteMode.None
                  });
        }
    }
}
