using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Web_Api_Tests
{
    public class PostServiceTests
    {
       

        [Test]
        public async Task Create_Post_Creates_New_Post_Via_Context()
        {
            
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase("Add_Post_Writes_Post_To_Database").Options;


            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);

                var post = new Post
                {
                    Title = "writing functional javascript",
                    Text = "some post content"
                };

                await postService.AddAsync(post);
            }


            await using (var ctx = new ForumContext(options))
            {
                Assert.AreEqual(1, ctx.Posts.CountAsync().Result);
                Assert.AreEqual("writing functional javascript", ctx.Posts.SingleAsync().Result.Title);
            }
        }
        [Test]
        public void Get_Post_By_Id_Returns_Correct_Post()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase("Get_Post_By_Id_Db").Options;

            using (var ctx = new ForumContext(options))
            {
                ctx.Posts.Add(new Post { Id = 1986, Title = "first post" });
                ctx.Posts.Add(new Post { Id = 223, Title = "second post" });
                ctx.Posts.Add(new Post { Id = 12, Title = "third post" });
                ctx.SaveChanges();
            }

            using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var result = postService.GetById(223);
                Assert.AreEqual(result.Title, "second post");
            }
        }
         [Test]
        public void Get_All_Posts_Returns_All_Posts()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase(databaseName: "Get_Post_By_Id_Db").Options;

            using (var ctx = new ForumContext(options))
            {
                ctx.Posts.Add(new Post { Id = 21341, Title = "first post" });
                ctx.Posts.Add(new Post { Id = 8144, Title = "second post" });
                ctx.Posts.Add(new Post { Id = 1245, Title = "third post" });
                ctx.SaveChanges();
            }

            using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var result = postService.GetAll();
                Assert.AreEqual(3, result.Count());
            }
        }

        [Test]
        public async Task Checking_Reply_Count_Returns_Number_Of_Replies()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase("Comments_Check").Options;

            await using (var ctx = new ForumContext(options))
            {
                ctx.Posts.Add(new Post
                {
                    Id = 21341,
                });

                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var post = await postService.GetByIdAsync(21341);

                await postService.AddCommentAsync(new Comment()
                {
                    Post = post,
                    Text = "Here's a post reply."
                });
            }

            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var replyCount = postService.GetCommentsCount(21341);
                Assert.AreEqual(replyCount, 1);
            }
        }

        [Test]
        public async Task Edit_Post_Edits_Post_Correctly()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase("Edit_DataBase").Options;
            await using (var ctx = new ForumContext(options))
            {
                ctx.Posts.Add(new Post { Id = 21342, Title = "first post" });
                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                await postService.EditPostContent(21342, "new post content");
                Assert.AreEqual(ctx.Posts.Find(21342).Text, "new post content");
            }
        }

        [Test]
        public async Task GetPostsByTopicId_Returns_CorrectPosts()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase("GetPostsByTopic_DataBase").Options;
            var posts = new List<Post>()
            {
                new Post {Id = 21342, Title = "first post"},
                new Post {Id = 8144, Title = "second post"}
            };
            await using (var ctx = new ForumContext(options))
            {
                ctx.Posts.Add(new Post { Id = 21341, Title = "first post" });
                ctx.Posts.Add(new Post { Id = 8144, Title = "second post" });
                ctx.Topics.Add(new Topic() {Id = 21, Posts = posts});
                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var result = postService.GetPostsByTopicId(21).ToList();
                Assert.AreEqual(posts.Count, result.Count());
                Assert.AreEqual(posts[0].Title, result[0].Title);
            }
        }
    }
}