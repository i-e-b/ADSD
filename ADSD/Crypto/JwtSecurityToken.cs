using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ADSD
{
    /// <summary>
    /// A <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> designed for representing a JSON Web Token (JWT).
    /// </summary>
    public class JwtSecurityToken : SecurityToken
    {
        private string rawSignature = string.Empty;
        private JwtHeader header;
        private string id;
        private JwtPayload payload;
        private string rawData;
        private string rawHeader;
        private string rawPayload;

        /// <summary>
        /// Initializes a new instance of <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> from a string in JWS Compact serialized format.
        /// </summary>
        /// <param name="jwtEncodedString">A JSON Web Token that has been serialized in JWS Compact serialized format.</param>
        /// <exception cref="T:System.ArgumentNullException">'jwtEncodedString' is null.</exception>
        /// <exception cref="T:System.ArgumentException">'jwtEncodedString' contains only whitespace.</exception>
        /// <exception cref="T:System.ArgumentException">'jwtEncodedString' is not in JWS Compact serialized format.</exception>
        /// <remarks>
        /// The contents of this <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> have not been validated, the JSON Web Token is simply decoded. Validation can be accomplished using <see cref="M:System.IdentityModel.Tokens.JwtSecurityTokenHandler.ValidateToken(System.String,System.IdentityModel.Tokens.TokenValidationParameters,System.IdentityModel.Tokens.SecurityToken@)" />
        /// </remarks>
        public JwtSecurityToken(string jwtEncodedString)
        {
            if (jwtEncodedString == null)
                throw new ArgumentNullException(nameof (jwtEncodedString));
            if (string.IsNullOrWhiteSpace(jwtEncodedString))
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10002: The parameter '{0}' cannot be 'null' or a string containing only whitespace.", (object) nameof (jwtEncodedString)));
            if (!Regex.IsMatch(jwtEncodedString, "^[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]*$"))
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10709: '{0}' is not well formed: '{1}'. The string needs to be in compact JSON format, which is of the form: '<Base64UrlEncodedHeader>.<Base64UrlEndcodedPayload>.<OPTIONAL, Base64UrlEncodedSignature>'.", (object) nameof (jwtEncodedString), (object) jwtEncodedString));
            this.Decode(jwtEncodedString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> class where the <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> contains the crypto algorithms applied to the encoded <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> and <see cref="T:System.IdentityModel.Tokens.JwtPayload" />. The jwtEncodedString is the result of those operations.
        /// </summary>
        /// <param name="header">Contains JSON objects representing the cryptographic operations applied to the JWT and optionally any additional properties of the JWT</param>
        /// <param name="payload">Contains JSON objects representing the claims contained in the JWT. Each claim is a JSON object of the form { Name, Value }</param>
        /// <param name="rawHeader">base64urlencoded JwtHeader</param>
        /// <param name="rawPayload">base64urlencoded JwtPayload</param>
        /// <param name="rawSignature">base64urlencoded JwtSignature</param>
        /// <exception cref="T:System.ArgumentNullException">'header' is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">'payload' is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">'rawSignature' is null.</exception>
        /// <exception cref="T:System.ArgumentException">'rawHeader' or 'rawPayload' is null or whitespace.</exception>
        public JwtSecurityToken(
            JwtHeader header,
            JwtPayload payload,
            string rawHeader,
            string rawPayload,
            string rawSignature)
        {
            if (header == null)
                throw new ArgumentNullException(nameof (header));
            if (payload == null)
                throw new ArgumentNullException(nameof (payload));
            if (rawSignature == null)
                throw new ArgumentNullException(nameof (rawSignature));
            if (string.IsNullOrWhiteSpace(rawHeader))
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10002: The parameter '{0}' cannot be 'null' or a string containing only whitespace.", (object) nameof (rawHeader)));
            if (string.IsNullOrWhiteSpace(rawPayload))
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10002: The parameter '{0}' cannot be 'null' or a string containing only whitespace.", (object) nameof (rawPayload)));
            this.header = header;
            this.payload = payload;
            this.rawData = rawHeader + "." + rawPayload + "." + rawSignature;
            this.rawHeader = rawHeader;
            this.rawPayload = rawPayload;
            this.rawSignature = rawSignature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> class where the <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> contains the crypto algorithms applied to the encoded <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> and <see cref="T:System.IdentityModel.Tokens.JwtPayload" />. The jwtEncodedString is the result of those operations.
        /// </summary>
        /// <param name="header">Contains JSON objects representing the cryptographic operations applied to the JWT and optionally any additional properties of the JWT</param>
        /// <param name="payload">Contains JSON objects representing the claims contained in the JWT. Each claim is a JSON object of the form { Name, Value }</param>
        /// <exception cref="T:System.ArgumentNullException">'header' is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">'payload' is null.</exception>
        public JwtSecurityToken(JwtHeader header, JwtPayload payload)
        {
            if (header == null)
                throw new ArgumentNullException(nameof (header));
            if (payload == null)
                throw new ArgumentNullException(nameof (payload));
            this.header = header;
            this.payload = payload;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> class specifying optional parameters.
        /// </summary>
        /// <param name="issuer">if this value is not null, a { iss, 'issuer' } claim will be added.</param>
        /// <param name="audience">if this value is not null, a { aud, 'audience' } claim will be added</param>
        /// <param name="claims">if this value is not null then for each <see cref="T:System.Security.Claims.Claim" /> a { 'Claim.Type', 'Claim.Value' } is added. If duplicate claims are found then a { 'Claim.Type', List&lt;object&gt; } will be created to contain the duplicate values.</param>
        /// <param name="expires">if expires.HasValue a { exp, 'value' } claim is added.</param>
        /// <param name="notBefore">if notbefore.HasValue a { nbf, 'value' } claim is added.</param>
        /// <param name="signingCredentials">The <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.SigningCredentials" /> that will be used to sign the <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />. See <see cref="M:System.IdentityModel.Tokens.JwtHeader.#ctor(System.IdentityModel.Tokens.SigningCredentials)" /> for details pertaining to the Header Parameter(s).</param>
        /// <exception cref="T:System.ArgumentException">if 'expires' &lt;= 'notbefore'.</exception>
        public JwtSecurityToken(
            string issuer = null,
            string audience = null,
            IEnumerable<Claim> claims = null,
            DateTime? notBefore = null,
            DateTime? expires = null,
            SigningCredentials signingCredentials = null)
        {
            if (expires.HasValue && notBefore.HasValue)
            {
                DateTime? nullable1 = notBefore;
                DateTime? nullable2 = expires;
                if ((nullable1.HasValue & nullable2.HasValue ? (nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                    throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10401: Expires: '{0}' must be after NotBefore: '{1}'.", (object) expires.Value, (object) notBefore.Value));
            }
            this.payload = new JwtPayload(issuer, audience, claims, notBefore, expires);
            this.header = new JwtHeader(signingCredentials);
        }

        /// <summary>
        /// Gets the 'value' of the 'actor' claim { actort, 'value' }.
        /// </summary>
        /// <remarks>If the 'actor' claim is not found, null is returned.</remarks>
        public string Actor
        {
            get
            {
                return this.payload.Actort;
            }
        }

        /// <summary>Gets the list of 'audience' claim { aud, 'value' }.</summary>
        /// <remarks>If the 'audience' claim is not found, enumeration will be empty.</remarks>
        public IEnumerable<string> Audiences
        {
            get
            {
                return (IEnumerable<string>) this.payload.Aud;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Security.Claims.Claim" />(s) for this token.
        /// </summary>
        /// <remarks><para><see cref="T:System.Security.Claims.Claim" />(s) returned will NOT have the <see cref="P:System.Security.Claims.Claim.Type" /> translated according to <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.InboundClaimTypeMap" /></para></remarks>
        public IEnumerable<Claim> Claims
        {
            get
            {
                return this.payload.Claims;
            }
        }

        /// <summary>
        /// Gets the Base64UrlEncoded <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> associated with this instance.
        /// </summary>
        public virtual string EncodedHeader
        {
            get
            {
                return this.header.Base64UrlEncode();
            }
        }

        /// <summary>
        /// Gets the Base64UrlEncoded <see cref="T:System.IdentityModel.Tokens.JwtPayload" /> associated with this instance.
        /// </summary>
        public virtual string EncodedPayload
        {
            get
            {
                return this.payload.Base64UrlEncode();
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> associated with this instance.
        /// </summary>
        public JwtHeader Header
        {
            get
            {
                return this.header;
            }
        }

        /// <summary>
        /// Gets the 'value' of the 'JWT ID' claim { jti, ''value' }.
        /// </summary>
        /// <remarks>If the 'JWT ID' claim is not found, null is returned.</remarks>
        public override string Id
        {
            get
            {
                return this.payload.Jti;
            }
        }

        /// <summary>
        /// Gets the 'value' of the 'issuer' claim { iss, 'value' }.
        /// </summary>
        /// <remarks>If the 'issuer' claim is not found, null is returned.</remarks>
        public string Issuer
        {
            get
            {
                return this.payload.Iss;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.IdentityModel.Tokens.JwtPayload" /> associated with this instance.
        /// </summary>
        public JwtPayload Payload
        {
            get
            {
                return this.payload;
            }
        }

        /// <summary>
        /// Gets the original raw data of this instance when it was created.
        /// </summary>
        /// <remarks>The original JSON Compact serialized format passed to one of the two constructors <see cref="M:System.IdentityModel.Tokens.JwtSecurityToken.#ctor(System.String)" />
        /// or <see cref="M:System.IdentityModel.Tokens.JwtSecurityToken.#ctor(System.IdentityModel.Tokens.JwtHeader,System.IdentityModel.Tokens.JwtPayload,System.String,System.String,System.String)" /></remarks>
        public string RawData
        {
            get
            {
                return this.rawData;
            }
        }

        /// <summary>
        /// Gets the original raw data of this instance when it was created.
        /// </summary>
        /// <remarks>The original JSON Compact serialized format passed to one of the two constructors <see cref="M:System.IdentityModel.Tokens.JwtSecurityToken.#ctor(System.String)" />
        /// or <see cref="M:System.IdentityModel.Tokens.JwtSecurityToken.#ctor(System.IdentityModel.Tokens.JwtHeader,System.IdentityModel.Tokens.JwtPayload,System.String,System.String,System.String)" /></remarks>
        public string RawHeader
        {
            get
            {
                return this.rawHeader;
            }
        }

        /// <summary>
        /// Gets the original raw data of this instance when it was created.
        /// </summary>
        /// <remarks>The original JSON Compact serialized format passed to one of the two constructors <see cref="M:System.IdentityModel.Tokens.JwtSecurityToken.#ctor(System.String)" />
        /// or <see cref="M:System.IdentityModel.Tokens.JwtSecurityToken.#ctor(System.IdentityModel.Tokens.JwtHeader,System.IdentityModel.Tokens.JwtPayload,System.String,System.String,System.String)" /></remarks>
        public string RawPayload
        {
            get
            {
                return this.rawPayload;
            }
        }

        /// <summary>
        /// Gets the original raw data of this instance when it was created.
        /// </summary>
        /// <remarks>The original JSON Compact serialized format passed to one of the two constructors <see cref="M:System.IdentityModel.Tokens.JwtSecurityToken.#ctor(System.String)" />
        /// or <see cref="M:System.IdentityModel.Tokens.JwtSecurityToken.#ctor(System.IdentityModel.Tokens.JwtHeader,System.IdentityModel.Tokens.JwtPayload,System.String,System.String,System.String)" /></remarks>
        public string RawSignature
        {
            get
            {
                return this.rawSignature;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.IdentityModel.Tokens.SecurityKey" />s for this instance.
        /// </summary>
        /// <remarks>By default an empty collection is returned.</remarks>
        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                return new ReadOnlyCollection<SecurityKey>((IList<SecurityKey>) new List<SecurityKey>());
            }
        }

        /// <summary>
        /// Gets the signature algorithm associated with this instance.
        /// </summary>
        /// <remarks>if there is a <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.SigningCredentials" /> associated with this instance, a value will be returned.  Null otherwise.</remarks>
        public string SignatureAlgorithm
        {
            get
            {
                return this.header.Alg;
            }
        }

        /// <summary>
        /// Gets the <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.SigningCredentials" /> associated with this instance.
        /// </summary>
        public SigningCredentials SigningCredentials
        {
            get
            {
                return this.header.SigningCredentials;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that signed this instance.
        /// </summary>
        /// <remarks><see cref="T:System.IdentityModel.Tokens.JwtSecurityTokenHandler" />.ValidateSignature(...) sets this value when a <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> is used to successfully validate a signature.</remarks>
        public SecurityKey SigningKey { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> that contains a <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that signed this instance.
        /// </summary>
        /// <remarks><see cref="T:System.IdentityModel.Tokens.JwtSecurityTokenHandler" />.ValidateSignature(...) sets this value when a <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> is used to successfully validate a signature.</remarks>
        public SecurityToken SigningToken { get; set; }

        /// <summary>Gets "value" of the 'subject' claim { sub, 'value' }.</summary>
        /// <remarks>If the 'subject' claim is not found, null is returned.</remarks>
        public string Subject
        {
            get
            {
                return this.payload.Sub;
            }
        }

        /// <summary>
        /// Gets 'value' of the 'notbefore' claim { nbf, 'value' } converted to a <see cref="T:System.DateTime" /> assuming 'value' is seconds since UnixEpoch (UTC 1970-01-01T0:0:0Z).
        /// </summary>
        /// <remarks>If the 'notbefore' claim is not found, then <see cref="F:System.DateTime.MinValue" /> is returned.</remarks>
        public override DateTime ValidFrom
        {
            get
            {
                return this.payload.ValidFrom;
            }
        }

        /// <summary>
        /// Gets 'value' of the 'expiration' claim { exp, 'value' } converted to a <see cref="T:System.DateTime" /> assuming 'value' is seconds since UnixEpoch (UTC 1970-01-01T0:0:0Z).
        /// </summary>
        /// <remarks>If the 'expiration' claim is not found, then <see cref="F:System.DateTime.MinValue" /> is returned.</remarks>
        public override DateTime ValidTo
        {
            get
            {
                return this.payload.ValidTo;
            }
        }

        /// <summary>
        /// Decodes the <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> and <see cref="T:System.IdentityModel.Tokens.JwtPayload" />
        /// </summary>
        /// <returns>A string containing the header and payload in JSON format</returns>
        public override string ToString()
        {
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}\nRawData: {2}", (object) this.header.SerializeToJson(), (object) this.payload.SerializeToJson(), (object) (this.rawData ?? "Empty"));
        }

        /// <summary>
        /// Decodes the string into the header, payload and signature
        /// </summary>
        /// <param name="jwtEncodedString">Base64Url encoded string.</param>
        internal void Decode(string jwtEncodedString)
        {
            string[] strArray = jwtEncodedString.Split(new char[1]
            {
                '.'
            }, 4);
            if (strArray.Length != 3)
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10709: '{0}' is not well formed: '{1}'. The string needs to be in compact JSON format, which is of the form: '<Base64UrlEncodedHeader>.<Base64UrlEndcodedPayload>.<OPTIONAL, Base64UrlEncodedSignature>'.", (object) nameof (jwtEncodedString), (object) jwtEncodedString));
            try
            {
                this.header = JwtHeader.Base64UrlDeserialize(strArray[0]);
                string typ = this.header.Typ;
                if (typ != null)
                {
                    if (!StringComparer.Ordinal.Equals(typ, "JWT"))
                    {
                        if (!StringComparer.Ordinal.Equals(typ, "http://openid.net/specs/jwt/1.0"))
                            throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10702: Jwt header type specified, must be '{0}' or '{1}'.  Type received: '{2}'.", (object) "JWT", (object) "http://openid.net/specs/jwt/1.0", (object) typ));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "IDX10703: Unable to decode the '{0}': '{1}' as Base64url encoded string. jwtEncodedString: '{2}'.", (object)"header", (object)strArray[0], (object)jwtEncodedString), ex);
            }
            try
            {
                this.payload = JwtPayload.Base64UrlDeserialize(strArray[1]);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "IDX10703: Unable to decode the '{0}': '{1}' as Base64url encoded string. jwtEncodedString: '{2}'.", (object)"payload", (object)strArray[1], (object)jwtEncodedString), ex);
            }
            this.rawData = jwtEncodedString;
            this.rawHeader = strArray[0];
            this.rawPayload = strArray[1];
            this.rawSignature = strArray[2];
        }

        internal void SetId(string id)
        {
            this.id = id;
        }
    }
}