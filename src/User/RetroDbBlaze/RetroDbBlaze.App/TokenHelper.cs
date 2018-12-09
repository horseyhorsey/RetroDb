using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RetroDbBlaze.App
{
    public class TokenHelper
    {
        //public string GenerateToken()
        //{
        //    var claims = new[] { new Claim(ClaimTypes.NameIdentifier, this.Request.Query["user"]) };
        //    var credentials = new SigningCredentials(Startup.SecurityKey, SecurityAlgorithms.HmacSha256); // Too lazy to inject the key as a service
        //    var token = new JwtSecurityToken("SignalRTestServer", "SignalRTests", claims, expires: DateTime.UtcNow.AddSeconds(30), signingCredentials: credentials);
        //    return Startup.JwtTokenHandler.WriteToken(token); // Even more lazy here
        //}
    }
}
