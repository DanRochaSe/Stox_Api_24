using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Entities;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/goal")]
    [Authorize]
    public class GoalController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IGoalRepository _repo;
        private readonly IAuthenticationRepository _authRepo;

        public GoalController(IConfiguration config, ILogger<GoalController> logger, IMapper mapper, IGoalRepository repo, IAuthenticationRepository authRepo)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _authRepo = authRepo ?? throw new ArgumentNullException(nameof(authRepo));
        }


        [HttpGet("GetAllGoals", Name = "GetAllGoals")]
        public async Task<IActionResult> GetAllGoals()
        {
            var claims = User.Claims.ToList();

            var goals = await _repo.GetAllGoals();
            return Ok(goals);
        }

        [HttpGet("{goalId}", Name = "GetGoalById")]
        public async Task<IActionResult> GetGoalById(int goalId)
        {
            var goal = await _repo.GetGoalByGoalID(goalId);
            if (goal == null)
            {
                return BadRequest($"Goal with Id {goalId} does not exists");
            }


            var goalToReturn = _mapper.Map<GoalForCreationDTO>(goal);

            return Ok(goalToReturn);
        }


        [HttpPost("AddNewGoal", Name = "AddNewGoal")]
        public async Task<IActionResult> AddNewGoal(GoalForCreationDTO newGoal)
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

            var goalToAdd = _mapper.Map<Goals>(newGoal);
            goalToAdd.Author = author;
            goalToAdd.UserID = (int)user.UserID;

            await _repo.AddGoal(goalToAdd);
            await _repo.SaveChangesAsync();

            var goalToReturn = _mapper.Map<GoalForReturnDTO>(goalToAdd);

            return CreatedAtRoute("GetGoalById", new
            {
                goalId = goalToAdd.GoalID
            }, goalToReturn);
        }

        [HttpPut("{goalId}", Name = "UpdateGoal")]
        public async Task<IActionResult> UpdateGoal(GoalForCreationDTO newGoal, int goalId)
        {
            if (! await _repo.GoalExistAsync(goalId))
            {
                _logger.LogInformation($"Goals with id {goalId} does not exist");
                return NotFound();
            }

            var goal = await _repo.GetGoalByGoalID(goalId);

            if (goal == null)
            {
                return NotFound();
            }

            _mapper.Map(newGoal, goal);

            await _repo.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{goalId}", Name ="DeleteGoal")]
        public async Task<IActionResult> DeleteGoal(int goalId)
        {

            if(! await _repo.GoalExistAsync(goalId))
            {
                _logger.LogInformation($"Goals with id {goalId} does not exist");
                return NotFound();
            }

            var goalToDelete = await _repo.GetGoalByGoalID(goalId);
            if (goalToDelete == null)
            {
                return NotFound();
            }

            await _repo.DeleteGoalAsync(goalToDelete);
            await _repo.SaveChangesAsync();

            return NoContent();
        }

    
    }
}
