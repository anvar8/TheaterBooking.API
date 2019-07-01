using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using TheaterBooking.Data;
using TheaterBooking.Models;
using TheaterBooking.Models.Users;
using TheaterBooking.Services;

namespace TheaterBooking
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
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(
                   Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            })
            .AddDefaultUI()
             .AddUserManager<UserManager<User>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = false;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                        // issuer validation required
                        ValidateIssuer = true,
                        // token issuer
                        ValidIssuer = AuthOptions.ISSUER,

                        // toekn receiver validation required
                        ValidateAudience = true,
                        // token receiver is set
                        ValidAudience = AuthOptions.AUDIENCE,
                        // validation of token lifetime
                        ValidateLifetime = true,

                        // security key setup
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        // security key validation required
                        ValidateIssuerSigningKey = true,
                   };
               });
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(JwtBearerDefaults.AuthenticationScheme, new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info
            //    {
            //        Version = "v1",
            //        Title = "My API",
            //        Description = "My First ASP.NET Core Web API",
            //        TermsOfService = "None",
            //        Contact = new Contact() { Name = "Talking Dotnet", Email = "contact@talkingdotnet.com", Url = "www.talkingdotnet.com" }
            //    });
            //});
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<EmailSenderOptions>(Configuration.GetSection("EmailSenderOptions"));
            services.AddCors(options =>
             options.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
                                                        .AllowAnyMethod()
                                                        .AllowAnyHeader()
                                                        .AllowCredentials()
                         ));
            AddServices(services);

        }
        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<HttpContextService>();
            services.AddScoped<UsersService>();
            services.AddScoped<BookingService>();
            services.AddScoped<ShowService>();         
            services.AddScoped<EmailSender>();
     
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();

                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roles = context.Roles.ToArray();

                if (!roles.Any(r => r.Name == AuthConstants.Administrator))
                    roleManager.CreateAsync(new IdentityRole(AuthConstants.Administrator)).Wait();

                if (!roles.Any(r => r.Name == AuthConstants.Client))
                    roleManager.CreateAsync(new IdentityRole(AuthConstants.Client)).Wait();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors();
            app.UseMvc();
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //});
        }
    }
}
