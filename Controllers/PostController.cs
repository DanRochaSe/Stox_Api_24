using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using System.Text.Json;
using webapi.Repository;
using Microsoft.AspNetCore.Authorization;
using webapi.Entities;
using AutoMapper;
using webapi.Services;
using Microsoft.AspNetCore.JsonPatch;

namespace webapi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/post")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IMailService _mailService;
    private readonly IConfiguration _config;
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;
    const int maxPostPageSize = 20;
    public SysConfigOptions? sysConfigoptions { get; private set; }

    public PostController(IConfiguration configuration, IPostRepository postRepository, IMapper mapper, ILogger<PostController> logger, IMailService mailService)
    {
        _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    [HttpGet(Name = "GetAllPosts")]
    public async Task<ActionResult<IEnumerable<Post>>> GetAllPosts([FromQuery]string? subject, string? searchQuery, int pageNumber = 1, int pageSize = 10)
    {

        if(pageSize > maxPostPageSize)
        {
            pageSize = maxPostPageSize;
        }

        var (postEntities, paginationMetada) = await _postRepository.GetPostsAsync(subject, searchQuery, pageNumber, pageSize);

        Response.Headers.Add("X-Pagination",  JsonSerializer.Serialize(paginationMetada));

        return Ok(_mapper.Map<IEnumerable<PostDto>>(postEntities));


    }




    [HttpGet("{postID}", Name = "GetPostByID")]
    public async Task<IActionResult> GetPostByID(int postID)
    {

        //SqlParameter[] prm = { new SqlParameter("@PostID", postID) };


        //DataTable post = SqlHelper.GetTableFromSP(appSettings.Value.DbConnectionString, "sp_GetPostByID", prm);

        //ICollection<Post> post = await _context.Posts.Where(p => p.PostID == postID).ToListAsync();

        if (!await _postRepository.PostExistAsync(postID))
        {
            return BadRequest("The post does not exist");

        }
        var post = await _postRepository.GetPostByIdAsync(postID);

        return Ok(post);

    }


    [HttpPost]
    [Authorize]
    //[EnableCors("_myAllowSpecificOrigins")]
    public async Task<IActionResult> AddNewPost(PostForCreationDTO _newPost)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var postToCreate = _mapper.Map<Post>(_newPost);
        postToCreate.CreatedAt = DateTime.Now;
        postToCreate.UpdatedAt = DateTime.Now;
        postToCreate.Author = User.FindFirst("username")?.Value;
        await _postRepository.AddPost(postToCreate);
        await _postRepository.SaveChangesAsync();

        var createdPostToReturn = _mapper.Map<PostDto>(postToCreate);

        return CreatedAtRoute("GetPostByID",
            new
            {
                postID = createdPostToReturn.PostID 
            }, postToCreate);
        //List<SqlParameter> prm = new List<SqlParameter> {
        //    new SqlParameter("@Title", _newPost.Title),
        //    new SqlParameter("@Body", _newPost.Body),
        //    new SqlParameter("@Author", _newPost.Author),
        //    new SqlParameter("@CreatedAt", DateTime.Now),
        //    new SqlParameter("@Subject", _newPost.Subject),
        //    new SqlParameter("@Ticker", _newPost.Tickers),
        //    };


        //_context.Posts.Add(_newPost);
        //int iResult = await  _context.SaveChangesAsync();
            
        //if (iResult > 0)
        //{
        //    //return Ok("Created");
        //    return Created("Post successfully created", _newPost);
        //}
        //else
        //{
        //    return BadRequest("Your post could not be created. Please contact support");
        //}

    }

    [HttpPut("{postID}", Name = "UpdatePostByID")]
    [Authorize]
    public async Task<ActionResult> UpdatePostById(PostForUpdateDTO _updatedPost, int postID) {


        if (! await _postRepository.PostExistAsync(postID))
        {
            _logger.LogInformation($"Post with id {postID} does not exists");
            return NotFound();
        }

        var postEntity = await _postRepository.GetPostByIdAsync(postID);
        if (postEntity == null)
        {
            return NotFound();
        }


        _mapper.Map(_updatedPost, postEntity);

        await _postRepository.SaveChangesAsync();

        
        return NoContent();
        //if (!ModelState.IsValid)
        //{
        //    return BadRequest();
        //}

        //List<SqlParameter> prm = new List<SqlParameter> {
        //    new SqlParameter("@Title", _updatedPost.Title),
        //    new SqlParameter("@Body", _updatedPost.Body),
        //    new SqlParameter("@Author", _updatedPost.Author),
        //    new SqlParameter("@UpdatedAt", DateTime.Now),
        //    new SqlParameter("@Subject", _updatedPost.Subject),
        //    new SqlParameter("@Ticker", _updatedPost.Tickers),
        //    new SqlParameter("@PostID", PostID),
        //    };


        //if (_postRepository.UpdatePost(_config.GetConnectionString("DbConnectionString"), "sp_UpdatePostByID", prm))
        //{
        //    Post post = _postRepository.GetPostByID(PostID, _config.GetConnectionString("DbConnectionString"));
        //    return Created("Post successfully updated", JsonSerializer.Serialize(post));
        //}
        //else
        //{
        //    return BadRequest("Something went wrong. Please try again.");
        //}

    }


    [HttpPatch("{postId}")]
    [Authorize]
    public async Task<ActionResult> PartialUpdatePost(int postId, JsonPatchDocument<PostForUpdateDTO> patchDocument)
    {
        if (! await _postRepository.PostExistAsync(postId))
        {
            _logger.LogInformation($"Post with id {postId} does not exists");
            return NotFound();
        }

        var postEntity = await _postRepository.GetPostByIdAsync(postId);

        if (postEntity == null)
        {
            return NotFound();
        }


        var postToPatch = _mapper.Map<PostForUpdateDTO>(postEntity);

        patchDocument.ApplyTo(postToPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(postToPatch))
        {
            return BadRequest(ModelState);
        }


        _mapper.Map(postToPatch, postEntity);

        await _postRepository.SaveChangesAsync();

        return NoContent();

    }


    [HttpDelete("{postId}",Name = "DeletePost")]
    [Authorize]
    public async Task<IActionResult> DeletePost(int postId)
    {
        if (! await _postRepository.PostExistAsync(postId))
        {
            _logger.LogInformation($"Post with id {postId} does not exists");
            return NotFound();
        }


        var postToDelete = await _postRepository.GetPostByIdAsync(postId);

        if (postToDelete == null)
        {
            return NotFound();
        }

        await _postRepository.DeletePostAsync(postToDelete);
        await _postRepository.SaveChangesAsync();

        _mailService.Send("PostDeleted", $"The post with id {postId} has been successfully deleted");

        return NoContent();
    }


}
