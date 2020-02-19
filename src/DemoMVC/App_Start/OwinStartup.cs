using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(DemoMVC.App_Start.Startup))]
namespace DemoMVC.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {           
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap =
                new Dictionary<string, string>();

            app.UseKentorOwinCookieSaver();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                CookieManager = new SystemWebCookieManager()
            });

            app.UseExternalSignInCookie("oidc");

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "mvc-demo",
                ClientSecret = "secret",
                Authority = "http://localhost:4000",
                RedirectUri = "http://localhost:1500/Auth",
                ResponseType = "code",
                Scope = "openid offline_access email",
                UseTokenLifetime = false,
                RequireHttpsMetadata = false,
                SignInAsAuthenticationType = "Cookies",
                Notifications =
                    new OpenIdConnectAuthenticationNotifications
                    {
                        AuthorizationCodeReceived = (context) =>
                        {
                            var client = new HttpClient();
                            var discovery = client.GetDiscoveryDocumentAsync("http://localhost:4000").Result;

                            if (discovery.IsError)
                            {
                                throw discovery.Exception;
                            }

                            var tokenResponse = client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
                            {
                                Address = discovery.TokenEndpoint,
                                RedirectUri = "http://localhost:1500/Auth",
                                ClientId = "mvc-demo",
                                ClientSecret = "secret",
                                Code = context.Code,
                            }).Result;

                            if (tokenResponse.IsError)
                            {
                                throw tokenResponse.Exception;
                            }

                            var userInfoResponse = client.GetUserInfoAsync(new UserInfoRequest
                            {
                                Address = discovery.UserInfoEndpoint,
                                Token = tokenResponse.AccessToken
                            }).Result;

                            if (userInfoResponse.IsError)
                            {
                                throw userInfoResponse.Exception;
                            }

                            var identity = new ClaimsIdentity(context.Options.AuthenticationType, "name", "role");
                            var properties = new AuthenticationProperties();
                            identity.AddClaims(userInfoResponse.Claims);

                            identity.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
                            identity.AddClaim(new Claim("expires_at", DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToLocalTime().ToString()));
                            identity.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
                            identity.AddClaim(new Claim("id_token", tokenResponse.IdentityToken));

                            context.AuthenticationTicket = new AuthenticationTicket(identity, properties);

                            return Task.FromResult(0);
                        },
                        RedirectToIdentityProvider = (context) =>
                        {
                            // if signing out, add the id_token_hint
                            if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                            {
                                var idTokenHint = context.OwinContext.Authentication.User.FindFirst("id_token");
                                if (idTokenHint != null)
                                {
                                    context.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                                }
                            }

                            return Task.FromResult(0);
                        }
                    }
            });
        }
    }
}