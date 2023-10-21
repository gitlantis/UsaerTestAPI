using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using UserTestMonnitorAPI.Services;
using System.Diagnostics;
using System.Net;

namespace UserTestAPI.Helpers
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private IConfiguration _configuration;
        public AuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
        public async Task Invoke(HttpContext context, UserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null && token.Length > 2)
            {
                try
                {
                    await attachUserToContext(context, userService, token);
                }
                catch (Exception)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }
            await _next(context);
        }
        private async Task attachUserToContext(HttpContext context, UserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Secret")));
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false, 
                    ValidateAudience = false, 
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero 
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = jwtToken.Claims.First(x => x.Type == "UserId").Value;
                var user = await userService.GetUserByGuid(new Guid(userId));

                if (user.IsBlocked || user == null)
                {
                    context.Items = null;
                    throw new Exception();
                }

                context.Items["User"] = user;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}