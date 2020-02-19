using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProtectedResource
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
            services
                .AddAuthorization()
                .AddAuthentication(setup=>
                {
                    setup.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    setup.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(setup=>
                {
                    // See: https://medium.com/all-technology-feeds/testing-your-asp-net-core-webapi-secured-with-identityserver4-in-postman-97eee976aa16

                    setup.Authority = "http://localhost:4000";
                    setup.Audience = "sampleapi";
                    setup.RequireHttpsMetadata = false; // The MetadataAddress or Authority must use HTTPS unless disabled for development by setting RequireHttpsMetadata=false.
                }).Services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
