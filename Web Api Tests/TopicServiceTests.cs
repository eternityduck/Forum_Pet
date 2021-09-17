using System.Linq;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Web_Api_Tests
{
    public class TopicServiceTests
    {
        [Test]
        public void Filtered_Posts_Returns_Correct_Result_Count()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase( "Search_Database").Options;

            using (var ctx = new ForumContext(options))
            {
                ctx.Topics.Add(new Topic()
                {
                    Id = 19
                });

                ctx.Posts.Add(new Post
                {
                    Topic = ctx.Topics.Find(19),
                    Id = 21341,
                    Title = "Functional programming",
                    Text = "Does anyone have experience deploying Haskell to production?" 
                });

                ctx.Posts.Add(new Post
                {
                    Topic = ctx.Topics.Find(19),
                    Id = -324,
                    Title = "Haskell Tail Recursion",
                    Text = "Haskell Haskell" 
                });

                ctx.SaveChanges();
            }

            using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var forumService = new TopicService(ctx, postService);
                var postCount = forumService.GetFilteredPosts(19, "Haskell").Count();
                Assert.AreEqual(2, postCount);
            }
        }
    }
}