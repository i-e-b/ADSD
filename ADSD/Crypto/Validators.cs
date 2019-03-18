using System;
using System.Collections.Generic;
using System.Globalization;

namespace ADSD.Crypto
{
    /// <summary>AudienceValidator</summary>
    public static class Validators
    {
        /// <summary>
        /// Determines if the audiences found in a <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> are valid.
        /// </summary>
        /// <param name="audiences">The audiences found in the <see cref="T:System.IdentityModel.Tokens.SecurityToken" />.</param>
        /// <param name="securityToken">The <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> being validated.</param>
        /// <param name="validationParameters"><see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" /> required for validation.</param>
        /// <exception cref="T:System.ArgumentNullException"> if 'vaidationParameters' is null.</exception>
        /// <exception cref="T:System.ArgumentNullException"> if 'audiences' is null and <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ValidateAudience" /> is true.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenInvalidAudienceException"> if <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ValidAudience" /> is null or whitespace and <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ValidAudiences" /> is null.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenInvalidAudienceException"> if none of the 'audiences' matched either <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ValidAudience" /> or one of <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ValidAudiences" />.</exception>
        /// <remarks>An EXACT match is required.</remarks>
        public static void ValidateAudience(
            IEnumerable<string> audiences,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            if (validationParameters == null)
                throw new ArgumentNullException(nameof (validationParameters));
            if (!validationParameters.ValidateAudience)
                return;
            if (audiences == null)
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10214: Audience validation failed. Audiences: '{0}'. Did not match:  validationParameters.ValidAudience: '{1}' or validationParameters.ValidAudiences: '{2}'", string.Join(",",audiences), validationParameters.ValidAudience ?? "null", string.Join(",",validationParameters.ValidAudiences)));
            if (string.IsNullOrWhiteSpace(validationParameters.ValidAudience) && validationParameters.ValidAudiences == null)
                throw new Exception("IDX10208: Unable to validate audience. validationParameters.ValidAudience is null or whitespace and validationParameters.ValidAudiences is null.");
            foreach (string audience in audiences)
            {
                if (!string.IsNullOrWhiteSpace(audience))
                {
                    if (validationParameters.ValidAudiences != null)
                    {
                        foreach (string validAudience in validationParameters.ValidAudiences)
                        {
                            if (string.Equals(audience, validAudience, StringComparison.Ordinal))
                                return;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(validationParameters.ValidAudience) && string.Equals(audience, validationParameters.ValidAudience, StringComparison.Ordinal))
                        return;
                }
            }
            throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10214: Audience validation failed. Audiences: '{0}'. Did not match:  validationParameters.ValidAudience: '{1}' or validationParameters.ValidAudiences: '{2}'", string.Join(",",audiences), validationParameters.ValidAudience ?? "null", string.Join(",",validationParameters.ValidAudiences)));
        }

        /// <summary>
        /// Determines if an issuer found in a <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> is valid.
        /// </summary>
        /// <param name="issuer">The issuer to validate</param>
        /// <param name="securityToken">The <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> that is being validated.</param>
        /// <param name="validationParameters"><see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" /> required for validation.</param>
        /// <returns>The issuer to use when creating the "Claim"(s) in a "ClaimsIdentity".</returns>
        /// <exception cref="T:System.ArgumentNullException"> if 'vaidationParameters' is null.</exception>
        /// <exception cref="T:System.ArgumentNullException"> if 'issuer' is null or whitespace and <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ValidateIssuer" /> is true.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenInvalidIssuerException"> if <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ValidIssuer" /> is null or whitespace and <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ValidIssuers" /> is null.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenInvalidIssuerException"> if 'issuer' failed to matched either <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ValidIssuer" /> or one of <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ValidIssuers" />.</exception>
        /// <remarks>An EXACT match is required.</remarks>
        public static string ValidateIssuer(
            string issuer,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            if (validationParameters == null)
                throw new ArgumentNullException(nameof (validationParameters));
            if (!validationParameters.ValidateIssuer)
                return issuer;
            if (string.IsNullOrWhiteSpace(issuer))
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10211: Unable to validate issuer. The 'issuer' parameter is null or whitespace"));
            if (string.IsNullOrWhiteSpace(validationParameters.ValidIssuer) && validationParameters.ValidIssuers == null)
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10204: Unable to validate issuer. validationParameters.ValidIssuer is null or whitespace AND validationParameters.ValidIssuers is null."));
            if (string.Equals(validationParameters.ValidIssuer, issuer, StringComparison.Ordinal))
                return issuer;
            if (validationParameters.ValidIssuers != null)
            {
                foreach (string validIssuer in validationParameters.ValidIssuers)
                {
                    if (string.Equals(validIssuer, issuer, StringComparison.Ordinal))
                        return issuer;
                }
            }
            throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10205: Issuer validation failed. Issuer: '{0}'. Did not match: validationParameters.ValidIssuer: '{1}' or validationParameters.ValidIssuers: '{2}'.", (object) issuer, (object) (validationParameters.ValidIssuer ?? "null"), (object) string.Join(",",validationParameters.ValidIssuers)));
        }

        /// <summary>
        /// Validates the <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that signed a <see cref="T:System.IdentityModel.Tokens.SecurityToken" />.
        /// </summary>
        /// <param name="securityKey">The <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that signed the <see cref="T:System.IdentityModel.Tokens.SecurityToken" />.</param>
        /// <param name="securityToken">The <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> being validated.</param>
        /// <param name="validationParameters"><see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" /> required for validation.</param>
        /// <exception cref="T:System.ArgumentNullException"> if 'vaidationParameters' is null.</exception>
        public static void ValidateIssuerSecurityKey(
            SecurityKey securityKey,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            if (validationParameters == null)
                throw new ArgumentNullException(nameof (validationParameters));
            if (!validationParameters.ValidateIssuerSigningKey)
                return;
            X509SecurityKey x509SecurityKey = securityKey as X509SecurityKey;
        }

        /// <summary>
        /// Validates the lifetime of a <see cref="T:System.IdentityModel.Tokens.SecurityToken" />.
        /// </summary>
        /// <param name="notBefore">The 'notBefore' time found in the <see cref="T:System.IdentityModel.Tokens.SecurityToken" />.</param>
        /// <param name="expires">The 'expiration' time found in the <see cref="T:System.IdentityModel.Tokens.SecurityToken" />.</param>
        /// <param name="securityToken">The <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> being validated.</param>
        /// <param name="validationParameters"><see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" /> required for validation.</param>
        /// <exception cref="T:System.ArgumentNullException"> if 'vaidationParameters' is null.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenNoExpirationException"> if 'expires.HasValue' is false and <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.RequireExpirationTime" /> is true.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenInvalidLifetimeException"> if 'notBefore' is &gt; 'expires'.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenNotYetValidException"> if 'notBefore' is &gt; DateTime.UtcNow.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenExpiredException"> if 'expires' is &lt; DateTime.UtcNow.</exception>
        /// <remarks>All time comparisons apply <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.ClockSkew" />.</remarks>
        public static void ValidateLifetime(
            DateTime? notBefore,
            DateTime? expires,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            if (validationParameters == null)
                throw new ArgumentNullException(nameof (validationParameters));
            if (!validationParameters.ValidateLifetime)
                return;
            if (!expires.HasValue && validationParameters.RequireExpirationTime)
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10225: Lifetime validation failed. The token is missing an Expiration Time.\nTokentype: '{0}'.", securityToken == null ? (object) "null" : (object) securityToken.GetType().ToString()));
            if (notBefore.HasValue && expires.HasValue && notBefore.Value > expires.Value)
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10224: Lifetime validation failed. The NotBefore: '{0}' is after Expires: '{1}'.", (object) notBefore.Value, (object) expires.Value));
            DateTime utcNow = DateTime.UtcNow;
            if (notBefore.HasValue && notBefore.Value > DateTimeUtil.Add(utcNow, validationParameters.ClockSkew))
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10222: Lifetime validation failed. The token is not yet valid.\nValidFrom: '{0}'\nCurrent time: '{1}'.", (object) notBefore.Value, (object) utcNow));
            if (expires.HasValue && expires.Value < DateTimeUtil.Add(utcNow, validationParameters.ClockSkew.Negate()))
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10223: Lifetime validation failed. The token is expired.\nValidTo: '{0}'\nCurrent time: '{1}'.", (object) expires.Value, (object) utcNow));
        }

        /// <summary>Validates if a token has been replayed.</summary>
        /// <param name="securityToken">The <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> being validated.</param>
        /// <param name="expirationTime">When does the security token expire.</param>
        /// <param name="validationParameters"><see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" /> required for validation.</param>
        /// <exception cref="T:System.ArgumentNullException">if 'securityToken' is null or whitespace.</exception>
        /// <exception cref="T:System.ArgumentNullException">if 'validationParameters' is null or whitespace.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenNoExpirationException">if <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.TokenReplayCache" /> is not null and expirationTime.HasValue is false. When a TokenReplayCache is set, tokens require an expiration time.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenReplayDetectedException">if the 'securityToken' is found in the cache.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenReplayAddFailedException">if the 'securityToken' could not be added to the <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.TokenReplayCache" />.</exception>
        public static void ValidateTokenReplay(
            string securityToken,
            DateTime? expirationTime,
            TokenValidationParameters validationParameters)
        {
            if (string.IsNullOrWhiteSpace(securityToken))
                throw new ArgumentNullException(nameof (securityToken));
            if (validationParameters == null)
                throw new ArgumentNullException(nameof (validationParameters));
            if (validationParameters.TokenReplayCache == null)
                return;
            if (!expirationTime.HasValue)
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10227: TokenValidationParameters.TokenReplayCache is not null, indicating to check for token replay but the security token has no expiration time: token '{0}'.", (object) securityToken));
            if (validationParameters.TokenReplayCache.TryFind(securityToken))
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10228: The securityToken has previously been validated, securityToken: '{0}'.", (object) securityToken));
            if (!validationParameters.TokenReplayCache.TryAdd(securityToken, expirationTime.Value))
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10229: TokenValidationParameters.TokenReplayCache was unable to add the securityToken: '{0}'.", (object) securityToken));
        }
    }
}