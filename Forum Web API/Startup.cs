using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL;
using BLL.Interfaces;
using BLL.Services;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenApiInfo = Microsoft.OpenApi.Models.OpenApiInfo;
using OpenApiOAuthFlow = Microsoft.OpenApi.Models.OpenApiOAuthFlow;
using OpenApiOAuthFlows = Microsoft.OpenApi.Models.OpenApiOAuthFlows;
using OpenApiSecurityScheme = Microsoft.OpenApi.Models.OpenApiSecurityScheme;

namespace Forum_Web_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<ForumContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Forum")));

            services.AddIdentity<User, IdentityRole>(opts =>
                {
                    opts.Password.RequiredLength = 5;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequireDigit = true;
                    opts.User.RequireUniqueEmail = true;
                    opts.User.AllowedUserNameCharacters =
                        ".@abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPRSTUVWZYX";
                })
                .AddEntityFrameworkStores<ForumContext>();

            var mapperProfile = new AutoMapperProfile();
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            services.AddScoped(m => new Mapper(mapperConfiguration));

            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    
                    options.ApiName = "api1";
            
                    // auth server base endpoint (this will be used to search for disco doc)
                    options.Authority = "https://localhost:5000";
                });
            // services.AddAuthentication("Basic")
            //     .AddScheme<AuthenticationsSchemeOptions>()
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<AuthorizeCheckOperationFilter>();
               
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Forum_Web_API", Version = "v1"});
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Auth Header"
                });
                // c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                // {
                //     Type = SecuritySchemeType.OAuth2,
                //     Flows = new OpenApiOAuthFlows
                //     {
                //         AuthorizationCode = new OpenApiOAuthFlow
                //         {
                //             AuthorizationUrl = new Uri("https://localhost:5000/connect/authorize"),
                //             TokenUrl = new Uri("https://localhost:5000/connect/token"),
                //             Scopes = new Dictionary<string, string>
                //             {
                //                 {"api1", "Demo API - full access"}
                //             }
                //         }
                //     }
                // });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                
                app.UseSwaggerUI(c =>
                {
                    
                    c.OAuthClientId("demo_api_swagger");
                    c.OAuthAppName("Demo API - Swagger");
                    c.OAuthUsePkce();
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Forum_Web_API v1");
                });
                
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}