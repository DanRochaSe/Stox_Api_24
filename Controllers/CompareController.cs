using AutoMapper;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.Data;
using Microsoft.ML;
using webapi.Entities;
using webapi.Models;
using webapi.Services;
using static Microsoft.ML.DataOperationsCatalog;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/v{apiversion:apiVersion}/compare")]
    public class CompareController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<CompareController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthenticationRepository _authRepo;
        private readonly ICompareRepository _repo;

        public CompareController(IConfiguration config, ILogger<CompareController> logger, IMapper mapper, IAuthenticationRepository authRepo, ICompareRepository repo)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authRepo = authRepo ?? throw new ArgumentNullException(nameof(authRepo));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }


        [HttpGet("GetAllComparisons", Name = "GetAllComparisons")]
        public async Task<IActionResult> GetAllComparisons()
        {
            var comparisons = await _repo.GetStoxComparisons();

            return Ok(comparisons);
        }

        [HttpGet("{comparisonId}", Name = "GetComparisonById")]
        public async Task<IActionResult> GetComparisonById(int comparisonId)
        {
            if (!await _repo.ComparisonExistsAsync(comparisonId))
            {
                return BadRequest($"The comparison with id {comparisonId} does not exist");
            }

            var comparisonToReturn = await _repo.GetStoxComparisonByID(comparisonId);

            return Ok(comparisonToReturn);
        }

        [HttpGet("stox/in/{comparisonId}", Name = "GetStoxsInComparison")]
        public async Task<IActionResult> GetStoxsInComparison(int comparisonId)
        {
            if (!await _repo.ComparisonExistsAsync(comparisonId))
            {
                return BadRequest("The Stox Comparison does not exist");
            }

            var stoxList = await _repo.GetStoxInComparison(comparisonId);

            return Ok(stoxList);
        }

        [HttpPost("AddComparison", Name = "AddComparison")]
        public async Task<IActionResult> AddComparison(StoxComparisonForCreationDTO newComparison)
        {
            var author = User.FindFirst("username")?.Value;

            if (author == null)
            {
                return BadRequest("The User does not exist");
            }

            var user = await _authRepo.GetUserByUsername(author);

            if (user == null)
            {
                return BadRequest("The User does not exist");
            }

            var comparisonToAdd = _mapper.Map<StoxComparison>(newComparison);
            comparisonToAdd.Author = author;
            comparisonToAdd.UserID = user.UserID;
            comparisonToAdd.CreatedAt = DateTime.UtcNow;

            await _repo.AddComparison(comparisonToAdd);
            await _repo.SaveChangesAsync();

            return CreatedAtRoute("GetComparisonById", new { comparisonId = comparisonToAdd.ComparisonID }, comparisonToAdd);
        }

        [HttpPut("{comparisonId}", Name = "UpdateComparison")]
        public async Task<IActionResult> UpdateComparison(StoxComparisonForCreationDTO comparison, int comparisonId)
        {
            if (!await _repo.ComparisonExistsAsync(comparisonId))
            {
                return BadRequest("The Stox Comparison does not exist");
            }

            var comparisonToUpdate = await _repo.GetStoxComparisonByID(comparisonId);

            _mapper.Map(comparison, comparisonToUpdate);
            await _repo.SaveChangesAsync();

            return Ok(comparisonToUpdate);



        }

        [HttpDelete("{comparisonId}", Name ="DeleteComparison")]
        public async Task<IActionResult> DeleteComparison(int comparisonId)
        {
            if (!await _repo.ComparisonExistsAsync(comparisonId))
            {
                return BadRequest("The Stox Comparison does not exist");
            }

            var comparisonToRemove = await _repo.GetStoxComparisonByID(comparisonId);

            await _repo.DeleteComparison(comparisonToRemove);

            return NoContent();
        }

        
        


    }
}
