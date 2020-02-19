using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace OpenIdConnectServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            services.AddIdentityServer()
            .AddInMemoryIdentityResources(new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("ir-app",new string[]{"custom.claim"}),
                new IdentityResources.Email(),
                new IdentityResource("given-name", new string[]{JwtClaimTypes.GivenName})
            })
            .AddInMemoryApiResources(new ApiResource[]
            {
                new ApiResource()
                {
                    Name ="sampleapi",
                    DisplayName= "The sample api",
                    Scopes = new Scope[]{
                        new Scope()
                        {
                            Name = "sampleapi",
                            DisplayName = "sampleapi",
                            UserClaims  = {ClaimTypes.Email},
                        }
                    }
                }
            })
            .AddInMemoryClients(new Client[]
            {
                new Client()
                {
                    ClientName = "authorization-code",
                    ClientId = "authorization-code",
                    ClientSecrets = {new Secret("secret".Sha256())},
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedScopes = {"sampleapi","openid","email"},
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true, // Refresh Token
                    // RequirePkce = true,
                    RedirectUris = { "http://localhost:4001/signin-oidc" },
                    RequireConsent = false,
                },
                new Client
                {
                    // See: https://medium.com/all-technology-feeds/testing-your-asp-net-core-webapi-secured-with-identityserver4-in-postman-97eee976aa16
                    ClientId = "sampleapi",
                    ClientName = "Postman Test Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedScopes = {"sampleapi","openid","profile","email"},
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RedirectUris =           { "https://www.getpostman.com/oauth2/callback" },
                    //NOTE: This link needs to match the link from the presentation layer - oidc-client
                    //      otherwise IdentityServer won't display the link to go back to the site
                    PostLogoutRedirectUris = { "https://www.getpostman.com" },
                    AllowedCorsOrigins =     { "https://www.getpostman.com" },
                    EnableLocalLogin = true
                },
                new Client()
                {
                    ClientName = "mvc-demo",
                    ClientId = "mvc-demo",
                    ClientSecrets = {new Secret("secret".Sha256())},
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedScopes = {"sampleapi","openid","email"},
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true, // Refresh Token
                    // RequirePkce = true,
                    RedirectUris = { "http://localhost:1500/Auth" },
                    RequireConsent = false,
                }              
            })
            .AddTestUsers(new List<TestUser>()
            {
                new TestUser()
                {
                    Username ="alice",
                    IsActive = true,
                    Password = "alice",
                    SubjectId = Guid.NewGuid().ToString(),
                    Claims = new List<Claim>()
                    {
                        new Claim(JwtClaimTypes.Email,"alice@identityserver.org"),
                        new Claim(JwtClaimTypes.PhoneNumber,"617497293"),
                        new Claim(JwtClaimTypes.GivenName,"workshop"),
                        new Claim(JwtClaimTypes.FamilyName,"cerditos"),
                        new Claim("custom.claim","my-custom-claim-value")
                    }
                }
            })
            .AddDeveloperSigningCredential(persistKey: true);

            services.AddAuthentication()
                .AddOpenIdConnect("oidc", "OpenID Connect", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SaveTokens = true;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "server.code";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });


            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseDeveloperExceptionPage();
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
