// Middleware to validate JWT token and attach user to context
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using dotnetapp.Services;
using dotnetapp.Data;

namespace dotnetapp.Middleware {
    public class JwtMiddleware {
        private readonly RequestDelegate _next;
        private readonly AuthService _authService;

        public JwtMiddleware(RequestDelegate next, AuthService authService) {
            _next = next;
            _authService = authService;
        }

        public async Task Invoke(HttpContext context, MockDbContext dbContext) {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
                AttachUserToContext(context, dbContext, token);
            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, MockDbContext dbContext, string token) {
            try {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_authService.SecretKey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1), // Adjust as needed
                    // ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                context.Items["User"] = dbContext.Users.FirstOrDefault(u => u.Id == userId);
            } catch {
                // Do nothing if JWT validation fails
            }
        }
    }
}
