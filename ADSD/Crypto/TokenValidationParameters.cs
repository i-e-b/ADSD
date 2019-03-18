using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ADSD
{
    /// <summary>
    /// Temp?
    /// </summary>
    public class TokenValidationParameters
    {
        /// <summary>
        /// Deafult authentication type
        /// </summary>
        public static readonly string DefaultAuthenticationType = "Federation";
        private string _nameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        private string _roleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        /// <summary>
        /// Creates a <see cref="T:System.Security.Claims.ClaimsIdentity" /> using:
        /// <para><see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.AuthenticationType" /></para>
        /// <para>'NameClaimType' is calculated: If NameClaimTypeRetriever call that else use NameClaimType. If the result is a null or empty string, use <see cref="F:System.Security.Claims.ClaimsIdentity.DefaultNameClaimType" /></para>.
        /// <para>'RoleClaimType' is calculated: If RoleClaimTypeRetriever call that else use RoleClaimType. If the result is a null or empty string, use <see cref="F:System.Security.Claims.ClaimsIdentity.DefaultRoleClaimType" /></para>.
        /// </summary>
        /// <returns>A <see cref="T:System.Security.Claims.ClaimsIdentity" /> with Authentication, NameClaimType and RoleClaimType set.</returns>
        public virtual ClaimsIdentity CreateClaimsIdentity(
            SecurityToken securityToken,
            string issuer)
        {
            string str1 = _nameClaimType;
            string str2 = _roleClaimType;
            return new ClaimsIdentity(this.AuthenticationType ?? TokenValidationParameters.DefaultAuthenticationType,
                str1 ?? "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                str2 ?? "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        }

        /// <summary>
        /// Gets or sets the AuthenticationType when creating a <see cref="T:System.Security.Claims.ClaimsIdentity" /> during token validation.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException"> if 'value' is null or whitespace.</exception>
        public string AuthenticationType { get; set; }

        /// <summary>
        /// Audience that is considered valid
        /// </summary>
        public string ValidAudience { get; set; }

        /// <summary>
        /// Default: true.
        /// Gets or sets a value indicating whether a <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> can be valid if not signed.
        /// </summary>
        public bool RequireSignedTokens { get; set; } = true;

        /// <summary>
        /// Issuer that is trusted
        /// </summary>
        public string ValidIssuer { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Collections.Generic.ICollection`1" /> that contains valid issuers that will be used during token validation.
        /// </summary>
        public IEnumerable<string> ValidIssuers { get; set; }
        
        /// <summary>
        /// Default: true.
        /// Gets or sets a boolean to control if the lifetime will be validated during token validation.
        /// </summary>
        public bool ValidateLifetime { get; set; } = true;
        
        /// <summary>
        /// Default: true.
        /// Gets or sets a value indicating whether tokens must have an 'expiration' value.
        /// </summary>
        public bool RequireExpirationTime { get; set; } = true;

        /// <summary>
        /// Gets or sets the clock skew to apply when validating times
        /// </summary>
        public TimeSpan ClockSkew {get;set;}

        /// <summary>
        /// Acceptable signing tokens
        /// </summary>
        public IEnumerable<X509SecurityToken> IssuerSigningTokens { get; set; }

        /// <summary>
        /// Gets or sets a boolean to control if the audience will be validated during token validation.
        /// </summary>
        public bool ValidateAudience { get; set; }

        /// <summary>
        /// Default: true.
        /// Gets or sets a boolean to control if the issuer will be validated during token validation.
        /// </summary>
        public bool ValidateIssuer { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="T:System.Collections.Generic.ICollection`1" /> that contains valid audiences that will be used during token validation.
        /// </summary>
        public IEnumerable<string> ValidAudiences { get; set; }
        
        /// <summary>
        /// Default: true.
        /// Gets or sets a boolean that controls if validation of the <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that signed the securityToken is called.
        /// </summary>
        public bool ValidateIssuerSigningKey { get; set; } = true;

        /// <summary>
        /// Gets or sets a boolean to control if the original token is saved when a session is created.
        /// </summary>
        /// <remarks>The SecurityTokenValidator will use this value to save the orginal string that was validated.</remarks>
        public bool SaveSigninToken { get; set; }
        
        /// <summary>
        /// Gets or set the <see cref="T:System.IdentityModel.Tokens.ITokenReplayCache" /> that will be checked to help in detecting that a token has been 'seen' before.
        /// </summary>
        public ITokenReplayCache TokenReplayCache { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.Actor" /> should be validated.
        /// </summary>
        public bool ValidateActor { get; set; }
    }
}