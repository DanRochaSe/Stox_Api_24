using API.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using webapi.Data;
using webapi.Entities;
using webapi.Services;

namespace webapi.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly StoxContext _context;

        public PostRepository(StoxContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool AddNewPost(string connString, string procName, List<SqlParameter> paramters)
        {

            bool success = false;

            try
            {
                if (SqlHelper.ExecuteProcedureNonQuery(connString, procName, paramters))
                {

                    success = true;

                }
            }
            catch(Exception e)
            {
                success = false;
            }


            return success;

        }

        public bool UpdatePost(string connString, string procName, List<SqlParameter> paramters)
        {
            bool success = false;

            try
            {
                if(SqlHelper.ExecuteProcedureNonQuery(connString, procName, paramters))
                {
                    success= true;

                }
            }
            catch(Exception e)
            {
                success = false;
            }

            return success;

        }


        public Post GetPostByID(int postID, string connString)
        {

            SqlParameter[] prm = { new SqlParameter("@PostID", postID) };


            DataTable dt = SqlHelper.GetTableFromSP(connString , "sp_GetPostByID", prm);
            Post post = new Post();


            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                post.PostID = Convert.ToInt32(dr["PostID"]);
                post.Title = Convert.ToString(dr["Title"]);
                post.Subject = Convert.ToString(dr["Subject"]);
                post.Body = Convert.ToString(dr["Body"]);
                post.Tickers = Convert.ToString(dr["Tickers"]);
                post.Author = Convert.ToString(dr["Author"]);

            }

            return post;

        }

        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            return await _context.Posts.OrderBy(p => p.CreatedAt).ToListAsync();
        }

        public async Task<(IEnumerable<Post>, PaginationMetadata)> GetPostsAsync(string? subject, string? searchQuery, int pageNUmber, int pageSize)
        {
            var collection = _context.Posts as IQueryable<Post>;

            if (!string.IsNullOrEmpty(subject))
            {
                subject = subject.Trim();
                collection = collection.Where(c => c.Subject == subject);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => (c.Subject != null && c.Subject.Contains(searchQuery)) || (c.Title != null && c.Title.Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetadate = new PaginationMetadata(totalItemCount, pageSize, pageNUmber);

            var collectionToReturn = await collection.OrderBy(c => c.CreatedAt).Skip(pageSize * (pageNUmber - 1)).Take(pageSize).ToListAsync();

            return (collectionToReturn, paginationMetadate);

        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {

            return await _context.Posts.Where(p => p.PostID == postId).OrderBy(p => p.CreatedAt).FirstOrDefaultAsync();

        }

        public async Task<bool> PostExistAsync(int postId)
        {
            return await _context.Posts.AnyAsync(p => p.PostID == postId);
        }

        public async Task AddPost(Post newPost)
        {
            await _context.Posts.AddAsync(newPost);
        }

        public async Task UpdatePostAsync(int postId, Post newPost)
        {
            var postToUpdate = await _context.Posts.Where(p => p.PostID == postId).FirstOrDefaultAsync();
            if (postToUpdate != null)
            {
                postToUpdate.Title = newPost.Title;
                postToUpdate.Tickers = newPost.Tickers;
                postToUpdate.Body = newPost.Body;
                postToUpdate.Subject = newPost.Subject;
                postToUpdate.UpdatedAt = DateTime.Now;
            }
        }

        public async Task DeletePostAsync(Post post)
        {
            _context.Posts.Remove(post);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task<IEnumerable<Post>> GetPostsByAuthorAsync(string? Author)
        {
            return await _context.Posts.Where(p => p.Author == Author).ToListAsync();
            
        }

               
    }
}
