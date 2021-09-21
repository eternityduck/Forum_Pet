using System.Linq;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Web_Api_Tests
{
    public class CommentServiceTests
    {
        [Test]
        public async Task Get_Comment_By_Id_Returns_Correct_Comment()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase("Get_Comment_By_Id_Db").Options;

            await using (var ctx = new ForumContext(options))
            {
                ctx.Comments.Add(new Comment() {Id = 1986, Text = "first comment"});
                ctx.Comments.Add(new Comment {Id = 223, Text = "second comment"});
                ctx.Comments.Add(new Comment {Id = 12, Text = "third comment"});
                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var commentService = new CommentService(ctx);
                var result = await commentService.GetByIdAsync(223);
                Assert.AreEqual(result.Text, "second comment");
            }
        }
        [Test]
        public async Task Update_Comment_By_Id_Returns_Updated_Comment()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase("Update_Comment_By_Id_Db").Options;

            await using (var ctx = new ForumContext(options))
            {
                ctx.Comments.Add(new Comment() {Id = 1986, Text = "first comment"});
                ctx.Comments.Add(new Comment {Id = 223, Text = "second comment"});
                ctx.Comments.Add(new Comment {Id = 12, Text = "third comment"});
                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ForumContext(options))
            {
                var commentService = new CommentService(ctx);
                await commentService.UpdateContentAsync(223, "Updated comment");
                var comment = await commentService.GetByIdAsync(223);
                Assert.AreEqual(comment.Text, "Updated comment");
            }
        }
    }
}