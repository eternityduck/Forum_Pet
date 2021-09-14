using System.Linq;
using AutoMapper;
using BLL.Models;
using DAL.Models;

namespace BLL
{
    public class AutoMapperProfile : Profile
    {
        // public AutoMapperProfile()
        // {
        //     CreateMap<Post, PostModel>()
        //         .ForMember(p => p.Comments, c =>
        //             c.MapFrom(posts => posts.Comments))
        //         // .ForMember(p => p.AuthorName, c => 
        //         //     c.MapFrom(x => x.Author.UserName))
        //         .ReverseMap();
        //     CreateMap<Comment, CommentModel>()
        //         .ReverseMap();
        //     CreateMap<User, UserModel>().ReverseMap();
        // }
    }
}