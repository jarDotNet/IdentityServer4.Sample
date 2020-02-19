# IdentityServer4.Sample

Several OIDC authentication sample projects using Identity Server 4.

```
Make sure to change [global.json](./global.json) file in order to set your own .NET Core SDK version.
```

## OpenIdConnectServer

OpenID Connect and OAuth 2.0 authentication server using ASP.NET Core and Identity Server 4.

## AuthorizationCodeClient

ASP.NET Core MVC Client using Identity Server 4 as authentication server.

## ProtectedResource

Sample of secured Web API with ASP.NET Core and OpenId Connect — IdentityServer4 using Postman.

You can find a Postman collection file to test this sample at [Postman/aspnet_core_webapi_identity_server_4.postman_collection.json](./Postman/aspnet_core_webapi_identity_server_4.postman_collection.json)

For details see the post [Testing your ASP.NET Core WebApi secured with IdentityServer4 in Postman](https://medium.com/all-technology-feeds/testing-your-asp-net-core-webapi-secured-with-identityserver4-in-postman-97eee976aa16)

## DemoMVC

This is an ASP.NET MVC Client sample using OWIN and .NET Framework.

Just remember to run first the Identity Server project in order to be able to perform this test.
