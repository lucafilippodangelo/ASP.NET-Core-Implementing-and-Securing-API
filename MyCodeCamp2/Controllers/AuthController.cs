using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyCodeCamp2.Data;
using MyCodeCamp2.Entities;
using MyCodeCamp2.Filters;
using MyCodeCamp2.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyCodeCamp2.Controllers
{
    //LD STEP23
    public class AuthController : Controller
    {
        private CampContext _context;
        private ILogger<AuthController> _logger;
        private SignInManager<CampUser> _signInMgr;
        private UserManager<CampUser> _userMgr;
        private IPasswordHasher<CampUser> _hasher;
        private IConfigurationRoot _config;

        public AuthController(CampContext context,
                              SignInManager<CampUser> signInMgr,
                              UserManager<CampUser> userMgr,
                              IPasswordHasher<CampUser> hasher,
                              ILogger<AuthController> logger,
                              IConfigurationRoot config)
        {
            _context = context;
            _signInMgr = signInMgr;
            _logger = logger;
            _userMgr = userMgr;
            _hasher = hasher;
            _config = config;
        }

        [HttpPost("api/auth/login")]//LD STEP24
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] CredentialModel model) 
        {
            try
            {
                //LD STEP25
                var result = await _signInMgr.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while logging in: {ex}");
            }

            return BadRequest("Failed to login");
        }



        [ValidateModel]
        [HttpPost("api/auth/token")] //LD STEP32
        public async Task<IActionResult> CreateToken([FromBody] CredentialModel model) 
        {
            try
            {
                var user = await _userMgr.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if (_hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                    {
                        //LD STEP33
                        //LD once the user is authenticated but without "cookie" created and dropped, we proceed 
                        // by creating an ARRAY of CLAIMS to use in the token

                        var userClaims = await _userMgr.GetClaimsAsync(user);

                        //LD we specify the CLAIMS to include in  the token.
                        var claims = new[]
                        {
                          new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                          new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                          new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                          new Claim(JwtRegisteredClaimNames.Email, user.Email)
                          }.Union(userClaims);

                        //LD then we ON SERVER SIDE specify the KEY to include in the token
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));//var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sfdbiuebfiubuewfu"));
                        


                        //LD then we specify the CREDENTIALS based on the key
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                          _config["Tokens:Issuer"], //LD "issuer" mean "EMITTENTE" //issuer: "www.lucadangelo.it",
                          _config["Tokens:Audience"], //LD "audience" mean "PUBBLICO" //audience: "www.lucadangelo.it",
                          claims: claims,
                          expires: DateTime.UtcNow.AddMinutes(15),
                          signingCredentials: creds
                          );

                        return Ok(new
                        {
                            //LD we add the token in the responce
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while creating JWT: {ex}");
            }

            return BadRequest("Failed to generate token");
        }
    }
}
