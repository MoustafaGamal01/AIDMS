using AIDMS.Security_Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
    }

    // making logout method async
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("Logged out");
    }

    // making login method async
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if(ModelState.IsValid)
        {
            var applicationUser = await _userManager.FindByEmailAsync(model.Email);
            if(applicationUser != null)
            {
                bool found = await _userManager.CheckPasswordAsync(applicationUser, model.Password);
                if (found)
                {
                    await _signInManager.SignInAsync(applicationUser, model.RememberMe);

                    // return end the function
                    return Ok("You're Signed In!");
                }
            }
            return BadRequest("Invalid Username or Password");
        }
        return BadRequest("Invalid Request");
    }

    #region Old Login With JWT stuff

    //[HttpPost("login")]
    //public async Task<IActionResult> Login([FromBody] LoginModel model)
    //{
    //    _logger.LogInformation("Login attempt for user: {Email}", model.Email);

    //    var user = await _userManager.FindByEmailAsync(model.Email);
    //    if (user != null)
    //    {
    //        var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
    //        if (isPasswordValid)
    //        {
    //            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
    //            if (result.Succeeded)
    //            {
    //                _logger.LogInformation("User {Email} logged in successfully", model.Email);

    //                var roles = await _userManager.GetRolesAsync(user);
    //                var token = GenerateJwtToken(user, roles);
    //                return Ok(new { token });
    //            }
    //            else
    //            {
    //                // Handle specific login failure scenarios here
    //                if (result.IsLockedOut)
    //                {
    //                    _logger.LogWarning("User {Email} is locked out", model.Email);
    //                    return Unauthorized("User is locked out.");
    //                }
    //                else if (result.RequiresTwoFactor)
    //                {
    //                    _logger.LogWarning("User {Email} requires two-factor authentication", model.Email);
    //                    return Unauthorized("Two-factor authentication required.");
    //                }
    //                else
    //                {
    //                    _logger.LogWarning("Invalid login attempt for user: {Email}", model.Email);
    //                    return Unauthorized("Invalid login credentials."); // More informative message
    //                }
    //            }
    //        }
    //        else
    //        {
    //            return Ok("Invalid Password"); // Consider a more generic message for security reasons
    //        }
    //    }
    //    else
    //    {
    //        return Ok("Invalid Email");  // Consider a more generic message for security reasons
    //    }
    //}


    //private string GenerateJwtToken(IdentityUser user, IList<string> roles)
    //{
    //    var claims = new List<Claim>
    //    {
    //        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
    //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    //    };

    //    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var token = new JwtSecurityToken(
    //        issuer: _configuration["Jwt:Issuer"],
    //        audience: _configuration["Jwt:Issuer"],
    //        claims: claims,
    //        expires: DateTime.Now.AddMinutes(30),
    //        signingCredentials: creds);

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //} 
    #endregion
}
