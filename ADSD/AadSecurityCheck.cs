using System;
using System.Collections.Generic;
using System.Text;

namespace ADSD
{
    /// <summary>
    /// Verify security with Azure Active Directory
    /// </summary>
    public class AadSecurityCheck: ISecurityCheck {
        private readonly SecurityConfig _config;

        private readonly string TenantKey;
        private readonly string Audience;
        private readonly string Issuer;

        /// <summary>
        /// Setup a security token checker
        /// </summary>
        public AadSecurityCheck(SecurityConfig config)
        {
            _config = config;
            Issuer = _config.AadTokenIssuer + TenantKey + "/";
            Audience = _config.Audience;
            TenantKey = _config.TenantKey;
        }

        /// <summary>
        /// Read authentication headers and check them against an AAD server.
        /// </summary>
        public SecurityOutcome Validate(string ctx)
        {
            try {
                var token = ctx; //ctx.Request.Headers.Get("Authorization") ?? ctx.Request.Headers.Get("WWW-Authenticate");
                if (string.IsNullOrWhiteSpace(token)) return SecurityOutcome.Fail;
                token = token.Replace("Bearer ", "");
                if (string.IsNullOrWhiteSpace(token)) return SecurityOutcome.Fail;

                // Set-up the validator...
                using (var signingTokens = SigningKeys.AllAvailableKeys())
                {
                    var validationParams = new TokenValidationParameters
                    {
                        ValidAudience = Audience,
                        ValidIssuer = Issuer,
                        IssuerSigningTokens = signingTokens
                    };
                    var x = new JwtSecurityTokenHandler();
                    x.ValidateToken(token, validationParams, out var y);

                    return (y == null) ? SecurityOutcome.Fail : SecurityOutcome.Pass;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return SecurityOutcome.Fail;
            }
        }
    }
}
