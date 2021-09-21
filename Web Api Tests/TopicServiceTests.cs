using System;
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
    public class TopicServiceTests
    {
        [Test]
        public async Task Create_Topic_Creates_New_Post_Via_Context()
        {

            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase("Add_Topic_Adds_Topic_To_Database").Options;


            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var topicService = new TopicService(ctx, postService);

                var topic = new Topic
                {
                    Title = "writing functional javascript",
                    
                };

                await topicService.AddAsync(topic);
            }


            await using (var ctx = new ForumContext(options))
            {
                Assert.AreEqual(1, ctx.Topics.CountAsync().Result);
                Assert.AreEqual("writing functional javascript", ctx.Topics.SingleAsync().Result.Title);
            }
        }
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
                var topicService = new TopicService(ctx, postService);
                var postCount = topicService.GetFilteredPosts(19, "Haskell").Count();
                Assert.AreEqual(2, postCount);
            }
        }

        [Test]
        public async Task GetAllAsync_Returns_All_Topics()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase( "GetAllTopicsAsync").Options;

            await using (var ctx = new ForumContext(options))
            {
                ctx.Topics.Add(new Topic()
                {
                    Id = 20
                });
                await ctx.Topics.AddAsync(new Topic());
                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var topicService = new TopicService(ctx, postService);
                var postCount =await topicService.GetAllAsync();
                Assert.AreEqual(2, postCount.Count());
            }
        }

        [Test]
        public async Task UpdateTopic_Returns_Updated_Topic()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase( "Update_Topic").Options;

            await using (var ctx = new ForumContext(options))
            {
                ctx.Topics.Add(new Topic()
                {
                    Id = 20,
                    Title = "oldtitle",
                    Description = "oldDescription"
                });
                
                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var topicService = new TopicService(ctx, postService);
                await topicService.UpdateContentAsync(20, "newDesc");
                await topicService.UpdateTopicTitle(20, "newtitle");
                var topic = await topicService.GetByIdAsync(20);
                Assert.AreEqual(topic.Description, "newDesc");
                Assert.AreEqual(topic.Title, "newtitle");
            }
        }

        [Test]
        public async Task DeleteTopic_Deletes_The_Topic()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase( "DeleteTopic_ById_Database").Options;

            await using (var ctx = new ForumContext(options))
            {
                ctx.Topics.Add(new Topic()
                {
                    Id = 20,
                    Title = "oldtitle",
                    Description = "oldDescription"
                });
                
                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var topicService = new TopicService(ctx, postService);
                await topicService.DeleteByIdAsync(20);
                var topics = await topicService.GetAllAsync();
                Assert.AreEqual(0, topics.Count());
            }
        }

        [Test]
        public async Task GetFilterdPosts_Returns_CorrectPosts()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase( "GetFilteredPosts_Database").Options;

            await using (var ctx = new ForumContext(options))
            {
                ctx.Topics.Add(new Topic()
                {
                    Id = 20,
                    Title = "oldtitle",
                    Description = "oldDescription",
                    Posts = new List<Post>()
                    {
                        new Post()
                        {
                            Title = "dsa",
                        }
                    }
                });
                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var topicService = new TopicService(ctx, postService);
                var posts = topicService.GetFilteredPosts("dsa");
                Assert.AreEqual(1, posts.Count());
            }
        }

        [Test]
        public async Task GetUsers_Returns_All_Users()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase( "GetAllUsers_DataBase_Topic").Options;

            await using (var ctx = new ForumContext(options))
            {
                ctx.Topics.Add(new Topic()
                {
                    Id = 20,
                    Title = "oldtitle",
                    Description = "oldDescription",
                    Posts = new List<Post>()
                    {
                        new Post()
                        {
                            Title = "dsa",
                            Author = new User(),
                        }
                    }
                });
                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var topicService = new TopicService(ctx, postService);
                var posts = topicService.GetUsers(20);
                Assert.AreEqual(1, posts.Count());
            }
        }
        [Test]
        public async Task GetRecentPosts_Returns_Correct_Recent_Posts()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase( "GetRecentPosts_Database_Topic").Options;

            await using (var ctx = new ForumContext(options))
            {
                ctx.Topics.Add(new Topic()
                {
                    Id = 20,
                    Title = "oldtitle",
                    Description = "oldDescription",
                    Posts = new List<Post>()
                    {
                        new Post()
                        {
                            Title = "dsa",
                            Author = new User(),
                            CreatedAt = DateTime.Now
                        },
                        new Post()
                        {
                            CreatedAt = DateTime.Parse("11/11/2000")
                        }
                    }
                });
                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var postService = new PostService(ctx);
                var topicService = new TopicService(ctx, postService);
                var posts = await topicService.GetRecentPostsAsync(20, 10);
                Assert.AreEqual(1, posts.Count());
            }
        }
    }
}