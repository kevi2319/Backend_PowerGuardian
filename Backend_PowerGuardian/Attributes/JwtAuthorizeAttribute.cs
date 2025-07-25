using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace Backend_PowerGuardian.Attributes
{
    public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedObjectResult("Token requerido");
                return;
            }

            try
            {
                var token = authHeader.Substring(7);
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                
                // Verificar que el token tenga el claim sub (user ID)
                var subClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub");
                if (subClaim == null)
                {
                    context.Result = new UnauthorizedObjectResult("Token inválido");
                    return;
                }
                
                // Verificar expiración
                var exp = jsonToken.Claims.FirstOrDefault(c => c.Type == "exp");
                if (exp != null && long.TryParse(exp.Value, out var expTime))
                {
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(expTime);
                    if (expDate < DateTimeOffset.UtcNow)
                    {
                        context.Result = new UnauthorizedObjectResult("Token expirado");
                        return;
                    }
                }
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedObjectResult("Token inválido");
            }
        }
    }
}
