using System.Data.SqlClient;
using webapi.Entities;
using webapi.Services;

namespace webapi.Repository
{
    public interface IPostRepository
    {

        Task<IEnumerable<Post>> GetPostsAsync();

        Task<(IEnumerable<Post>, PaginationMetadata)> GetPostsAsync(string? subject, string? searchQuery, int pageNUmber, int pageSize);

        Task<Post> GetPostByIdAsync(int postId);

        Task<bool> PostExistAsync(int postId);

        Task AddPost(Post newPost);

        Task UpdatePostAsync(int postId, Post newPost);

        Task DeletePostAsync(Post post);

        Task<bool> SaveChangesAsync();

        Task<IEnumerable<Post>> GetPostsByAuthorAsync(string? Author);

        bool AddNewPost(string connString, string procName, List<SqlParameter> paramters);

        bool UpdatePost(string connString, string procName, List<SqlParameter> paramters);

        Post GetPostByID(int postID, string connString);
    }
}
