namespace Monitor.WebApi.Helpers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Monitor.WebApi.Services;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserRoleService userRoleService)
        {
            var token = context.Request.Headers["x-access-token"].FirstOrDefault()?.Split(" ").Last();
            var currentProject = context.Request.Headers["Project"].FirstOrDefault()?.Split(" ").Last();


            if (token != null)
                attachUserToContext(context, userRoleService, token,currentProject);


            await _next(context);
        }

        private void attachUserToContext(HttpContext context, IUserRoleService userRoleService, string token,string project)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);


                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // attach user to context on successful jwt validation
                Guid projectId = Guid.Empty;
                _ = Guid.TryParse(project, out projectId);
                var usrRole = userRoleService.GetById(userId, projectId);
                context.Items["User"] = usrRole.User;
                if (!string.IsNullOrEmpty(project))
                {
                    context.Items["ActiveProject"] = usrRole.ActiveProject;

                }
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
