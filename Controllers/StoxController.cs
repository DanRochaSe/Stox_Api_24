using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi.Entities;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/stox")]
    public class StoxController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<StoxController> _logger;
        private readonly IMapper _mapper;
        private readonly IStoxRepository _repo;
        private readonly IAuthenticationRepository _authRepo;

        public StoxController(IConfiguration config, ILogger<StoxController> logger, IMapper mapper, IStoxRepository repo, IAuthenticationRepository authRepo)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _authRepo = authRepo ?? throw new ArgumentNullException(nameof(authRepo));
        }


        [HttpGet("GetAllStox", Name = "GetAllStox")]
        public async Task<IActionResult> GetAllStox()
        {
            var stoxList = await _repo.GetAllStoxAsync();

            return Ok(stoxList);
        }

        [HttpGet("{stoxId}", Name = "GetStoxByID")]
        public async Task<IActionResult> GetStoxByID(int stoxId)
        {
            if (! await _repo.StoxExistsAsync(stoxId))
            {
                return NotFound($"The stox with id {stoxId} does not exist");
            }

            var stox = await _repo.GetStoxById(stoxId);

            return Ok(stox);
        }

        [HttpGet("GetStoxByUserId", Name = "GetStoxByUserId")]
        [Authorize]
        public async Task<IActionResult> GetStoxByUserId()
        {
            var author = User.FindFirst("username")?.Value;

            if (author == null)
            {
                return BadRequest("The user does not exist");
            }

            var user = await _authRepo.GetUserByUsername(author);

            if (user == null)
            {
                return BadRequest("The user does not exist");
            }

            var stoxList = await _repo.GetStoxByUser(user.UserID);

            return Ok(stoxList);
        }


        [HttpPost("AddStox", Name = "AddStox")]
        [Authorize]
        public async Task<IActionResult> AddStox(StoxForCreationDTO newStox)
        {
            var author = User.FindFirst("username")?.Value;

            if (author == null)
            {
                return BadRequest("The user does not exist");
            }

            var user = await _authRepo.GetUserByUsername(author);

            if (user == null)
            {
                return BadRequest("The user does not exist");
            }


            var stoxToAdd = _mapper.Map<Stox>(newStox);
            stoxToAdd.Author = author;
            stoxToAdd.UserID = user.UserID;
            stoxToAdd.DateAdded = DateTime.UtcNow;

            await _repo.AddStox(stoxToAdd);
            await _repo.SaveChangesAsync();

            return Ok(stoxToAdd);


        }

        [HttpPut("{stoxId}", Name ="UpdateStox")]
        public async Task<IActionResult> UpdateStox(StoxForCreationDTO stox, int stoxId)
        {
            if (! await _repo.StoxExistsAsync(stoxId))
            {
                return BadRequest("The stox does not exist");
            }

            var stoxToUpdate = await _repo.GetStoxById(stoxId);

            _mapper.Map(stox, stoxToUpdate);
            await _repo.SaveChangesAsync();

            return Ok(stoxToUpdate);
        }



    }
}
