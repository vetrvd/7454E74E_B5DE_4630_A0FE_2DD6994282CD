using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Model;

namespace _7454E74E_B5DE_4630_A0FE_2DD6994282CD
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<RepositorySetting>(options =>
            {
                options.Database = Configuration.GetValue<string>("RepositoryConnection:Database");
                options.ConnectionString = Configuration.GetValue<string>("RepositoryConnection:ConnectionString");
                options.ResponseLimit = Configuration.GetValue<int>("RepositoryConnection:ResponseLimit");
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie(options => options.SlidingExpiration = true)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ClockSkew = TimeSpan.FromMinutes(5),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetValue<string>("ClientSecret"))),
                        RequireSignedTokens = true,
                        RequireExpirationTime = true,
                        ValidateLifetime = false,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidAudience = Configuration.GetValue<string>("AUDIENCE"),
                        ValidIssuer = Configuration.GetValue<string>("ISSUER")
                    };
                });

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IMapper>(at =>
                new MapperConfiguration(it => it.AddProfile(typeof(MapperProfile))).CreateMapper());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHsts();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature.Error;

                    var problemDetails = new ProblemDetails
                    {
                        Instance = $"urn:error:{Guid.NewGuid()}"
                    };

                    if (exception is BadHttpRequestException badHttpRequestException)
                    {
                        problemDetails.Title = "Invalid request";
                        problemDetails.Status = (int) typeof(BadHttpRequestException).GetProperty("StatusCode",
                            BindingFlags.NonPublic | BindingFlags.Instance).GetValue(badHttpRequestException);
                        problemDetails.Detail = badHttpRequestException.Message;
                    }
                    else
                    {
                        problemDetails.Title = "An unexpected error occurred!";
                        problemDetails.Status = 500;
                        problemDetails.Detail = exception.Demystify().ToString();
                    }

                    context.Response.StatusCode = problemDetails.Status.Value;
                    context.Response.ContentType = "application/json";

                    using (var writer = new HttpResponseStreamWriter(context.Response.Body, Encoding.UTF8))
                    {
                        using (var jsonWriter = new JsonTextWriter(writer))
                        {
                            jsonWriter.CloseOutput = false;
                            jsonWriter.AutoCompleteOnClose = false;

                            new JsonSerializer().Serialize(jsonWriter, problemDetails);
                        }
                    }
                });
            });
            app.UseMvc();
        }
    }
}