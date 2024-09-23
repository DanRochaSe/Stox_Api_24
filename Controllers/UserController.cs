using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using webapi.Data;
using webapi.Entities;
using webapi.Helpers;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private readonly IOptions<SysConfigOptions> appSettings;
        private readonly IConfiguration _config;
        private readonly StoxContext _context;
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public SysConfigOptions? sysConfigoptions { get; private set; }

        public UserController(IOptions<SysConfigOptions> app, IConfiguration configuration, StoxContext context, IUserRepository repo, IMapper mapper)
        {
            appSettings = app ?? throw new ArgumentNullException(nameof(app));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        // GET: UserController


        // GET: HomeController
        [HttpGet(Name = "GetAllUsers")]
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {

            //if (User.FindFirst(c => c.ToString().Contains("role"))?.Value != "Admin")
            //{
            //    return BadRequest("Not Authorized");
            //} 

            List<User> Users = await _context.Users.ToListAsync();
            List<UserForReturnDTO> usersForReturn = _mapper.Map<List<UserForReturnDTO>>(Users);


            return Ok(usersForReturn);

        }

        [HttpGet("/api/User/Goals", Name = "GetUserWithGoals")]
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]


        public async Task<IActionResult> GetUserWithGoals()
        {

            if (User.FindFirst(c => c.ToString().Contains("role"))?.Value != "Admin")
            {
                return BadRequest("Not Authorized");
            }

            List<User> Users = await _context.Users.Include(u => u.Goals).Where(g => g.Goals != null && g.Goals.Count > 0).ToListAsync();

            return Ok(Users);

        }


        [HttpGet("{userID}", Name = "GetUserByID")]
        public async Task<IActionResult> GetUserByID(int userID)
        {

            User? user = await _context.Users.FindAsync(userID);

            if (user != null)
            {
                return Ok(user);

            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet("{userID}/Goals", Name = "GetUserByIdWithGoals")]
        public async Task<IActionResult> GetUserByIdWithGoals(int userID)
        {
            User? user = await _context.Users.Include(u => u.Goals).Where(u => u.UserID == userID).FirstOrDefaultAsync();

            if (user != null)
            {
                return Ok(user);

            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost(Name = "AddUser")]
        public async Task<IActionResult> AddUser(UserForCreationDTO newUser)
        {
            if (newUser.UserName != null)
            {
                if (await _repo.UserExistsAsync(newUser.UserName))
                {
                    return BadRequest("The user already exists");
                }
            }
            else
            {
                return BadRequest("Please provide a valid userName");

            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userToCreate = _mapper.Map<User>(newUser);

            userToCreate.PasswordHash = HashPassword(newUser.Password, out byte[] salt);
            userToCreate.Role = StoxEnum.RoleType.Admin;

            var AuthHelper = new AuthHelper(_config);
            var token = AuthHelper.JWTGenerator(userToCreate);
            var refreshToken = AuthHelper.GenerateRefreshToken();

            userToCreate.Token = token;
            userToCreate.RefreshToken = refreshToken;

            _context.UserSalts.Add(new UserSalt { Salt = Convert.ToHexString(salt), UserName = userToCreate.UserName});

            _context.Users.Add(userToCreate);
            await _context.SaveChangesAsync();

            

            AuthHelper.AppendJwtToCookie(HttpContext, token, "token");
            AuthHelper.AppendJwtToCookie(HttpContext, refreshToken, "refreshToken");

            var userToReturn = _mapper.Map<UserForCreationDTO>(userToCreate);

            return Ok(userToReturn);

        }


        private string HashPassword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(64);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                20000,
                HashAlgorithmName.SHA512,
                64);

            return Convert.ToHexString(hash);
        }








    }
}
