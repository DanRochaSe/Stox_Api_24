using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Cryptography;
using webapi.Helpers;
using webapi.Services;

namespace webapi.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthenticationRepository _authRepository;
        private readonly IMapper _mapper;

        public AuthenticationController(IConfiguration config, IAuthenticationRepository authRepository, IMapper mapper )
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            var user = await _authRepository.GetUserByUsername(authenticationRequestBody.UserName);

            if (user == null)
            {
                return NotFound($"The user with username {authenticationRequestBody.UserName} could not be found");
            }

            var userSalt = await _authRepository.GetUserSalt(user.UserName);

            if (!VerifyPassword(authenticationRequestBody.Password, user.PasswordHash, Convert.FromHexString(userSalt)))
            {
                return Unauthorized("Incorrect Password");
            }


            var AuthHelper = new AuthHelper(_config);
            var token = AuthHelper.JWTGenerator(user);
            var refreshToken = AuthHelper.GenerateRefreshToken();
            user.Token = token;
            user.RefreshToken = refreshToken;
            await _authRepository.SaveChangesAsync();
            AuthHelper.AppendJwtToCookie(HttpContext, token, "token");
            AuthHelper.AppendJwtToCookie(HttpContext, refreshToken, "refreshToken");

            //AuthHelper.AppendJwtToCookie(HttpContext, refreshToken, "refreshToken");
            //var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Authentication:SecretForKey"]));

            //var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //var claimsForToken = new List<Claim>();
            //claimsForToken.Add(new Claim("Sub", user.UserID.ToString()));
            //claimsForToken.Add(new Claim("username", user.UserName));
            //claimsForToken.Add(new Claim("given_name", user.FirstName));
            //claimsForToken.Add(new Claim("family_name", user.LastName));
            //claimsForToken.Add(new Claim("DOB", user.DOB.ToString("dd/MM/yyyy")));
            //claimsForToken.Add(new Claim("role", "Admin"));

            //var jwtSecurityToken = new JwtSecurityToken(
            //    _config["Authentication:Issuer"],
            //    _config["Authentication:Audience"],
            //    claimsForToken,
            //    DateTime.UtcNow,
            //    DateTime.UtcNow.AddHours(1),
            //    signingCredentials
            //    );

            //var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);


            //HttpContext.Response.Cookies.Append("token", tokenToReturn, 
            //    new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    IsEssential = true,
            //    SameSite = SameSiteMode.Strict,
            //    Expires = DateTime.UtcNow.AddHours(1)
            //});


            return Ok();


            //return Ok(tokenToReturn);

        }

        [HttpGet("GetUserClaims")]
        [Authorize]
        public ActionResult GetUserClaims()
        {
            return new JsonResult(User.Claims.Select(c => new {Type= c.Type, Value = c.Value }));
        }


        [HttpGet("Logout")]
        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("token");
            return Ok();
        }

        //[HttpPost("GetRefreshToken", Name = "GetRefreshToken")]
        //public async Task<IActionResult> GetRefreshToken()
        //{
        //    var author = User.FindFirst("username")?.Value;

        //    if (author == null)
        //    {
        //        return BadRequest("The user does not exist");
        //    }




        //}


        [HttpPost("refresh", Name = "RefreshToken")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var author = User.FindFirst("username")?.Value;

            if (author == null)
            {
                return BadRequest("The user does not exist");
            }

            var refreshToken = string.Empty;
            var user = await _authRepository.GetUserByUsername(author);

            string accessToken = HttpContext.Request.Cookies["token"];
            var authHelper = new AuthHelper(_config);
            var principal = authHelper.GetPrincipalFromExpiredToken(accessToken);

            if (HttpContext.Request.Cookies["refreshToken"] == null)
            {
                return BadRequest("No valid refresh token sent");
            }

            refreshToken = HttpContext.Request.Cookies["refreshToken"];

            if (user == null || user.RefreshToken != refreshToken)
            {
                return BadRequest("Invalid Request");
            }

            var newAccessToken = authHelper.JWTGenerator(user);
            var newRefreshToken = authHelper.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.Token = newAccessToken;
            await _authRepository.SaveChangesAsync();

            authHelper.AppendJwtToCookie(HttpContext, newAccessToken, "token");
            authHelper.AppendJwtToCookie(HttpContext, newRefreshToken, "refreshToken");

            //return Ok(new TokenResponseObject { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
            return Ok();
        }





        private bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, 20000, HashAlgorithmName.SHA512, 64);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }

    }


    public class AuthenticationRequestBody
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public class TokenResponseObject
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
