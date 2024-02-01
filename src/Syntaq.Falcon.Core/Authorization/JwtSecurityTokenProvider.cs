using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Syntaq.Falcon.Authorization
{
	public static class JwtSecurityTokenProvider
	{


		private static readonly string Secret = "T3eJ2FIuKOhRJr9x8TADog==";
		public static string GenerateToken(string name, int expiry = 365)
		{
			byte[] key = Convert.FromBase64String(Secret);
			SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
			SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] {
					  new Claim(ClaimTypes.Name, name)}),
				Expires = DateTime.UtcNow.AddDays(expiry),
				SigningCredentials = new SigningCredentials(securityKey,
				SecurityAlgorithms.HmacSha256Signature)
			};

			JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
			JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
			return handler.WriteToken(token);
		}

		public static ClaimsPrincipal GetPrincipal(string token)
		{
			try
			{
				JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
				JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
				if (jwtToken == null)
					return null;
				byte[] key = Convert.FromBase64String(Secret);
				TokenValidationParameters parameters = new TokenValidationParameters()
				{
					RequireExpirationTime = true,

					ValidateIssuer = false,
					ValidateAudience = false,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};
				SecurityToken securityToken;
				ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
				return principal;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public static string ValidateToken(string token)
		{
			string username = null;
			ClaimsPrincipal principal = GetPrincipal(token);
			if (principal == null) return null;

			ClaimsIdentity identity = null;
			try
			{
				identity = (ClaimsIdentity)principal.Identity;
			}
			catch (NullReferenceException)
			{
				return null;
			}
			Claim usernameClaim = identity.FindFirst(ClaimTypes.Name);
			username = usernameClaim.Value;
			return username;
		}

        public static bool IsTokenExpired(string jwtToken)
        {
            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();

                // Validate the token format before reading its content.
                if (!tokenHandler.CanReadToken(jwtToken))
                {
					return true;
                    //throw new ArgumentException("Invalid JWT token format.");
                }

                var token = tokenHandler.ReadJwtToken(jwtToken);

                // Get the 'exp' claim value, which represents the token's expiry date (in Unix time).
                var expClaim = token.Claims.FirstOrDefault(c => c.Type == "exp");

                if (expClaim == null)
                {
					return true;
                    //throw new InvalidOperationException("The 'exp' claim is missing from the JWT token.");
                }

                // Convert the Unix timestamp to a DateTime object.
                long unixTimestamp;
                if (!long.TryParse(expClaim.Value, out unixTimestamp))
                {
					return true;
                    // throw new InvalidOperationException("Invalid value for the 'exp' claim in the JWT token.");
                }

                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
				if (expiryDate > DateTime.Now) return false;
				return true;

            }
            catch (Exception e)
            {
                return true;
            }
        }
    }
}
