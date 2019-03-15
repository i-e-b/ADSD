using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace ADSD
{
    /// <summary>
    /// A <see cref="T:System.IdentityModel.Tokens.SecurityTokenHandler" /> designed for creating and validating Json Web Tokens. See http://tools.ietf.org/html/draft-ietf-oauth-json-web-token-07.
    /// </summary>
    public class JwtSecurityTokenHandler 
    {
        SignatureProviderFactory signatureProviderFactory = new SignatureProviderFactory();
        private int _maximumTokenSizeInBytes = 2097152;
        private int _defaultTokenLifetimeInMinutes = JwtSecurityTokenHandler.DefaultTokenLifetimeInMinutes;
        private static IDictionary<string, string> outboundAlgorithmMap = (IDictionary<string, string>) new Dictionary<string, string>()
        {
            {
                "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
                "RS256"
            },
            {
                "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                "HS256"
            }
        };
        private static IDictionary<string, string> inboundAlgorithmMap = (IDictionary<string, string>) new Dictionary<string, string>()
        {
            {
                "RS256",
                "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"
            },
            {
                "HS256",
                "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256"
            }
        };
        private static IDictionary<string, string> inboundClaimTypeMap = ClaimTypeMapping.InboundClaimTypeMap;
        private static IDictionary<string, string> outboundClaimTypeMap = ClaimTypeMapping.OutboundClaimTypeMap;
        private static string shortClaimTypeProperty = "http://schemas.xmlsoap.org/ws/2005/05/identity/claimproperties/ShortTypeName";
        private static string jsonClaimTypeProperty = "http://schemas.xmlsoap.org/ws/2005/05/identity/claimproperties/json_type";
        private static ISet<string> inboundClaimFilter = ClaimTypeMapping.InboundClaimFilter;
        private static string[] tokenTypeIdentifiers = new string[2]
        {
            "urn:ietf:params:oauth:token-type:jwt",
            "JWT"
        };
        /// <summary>
        /// Default lifetime of tokens created. When creating tokens, if 'expires' and 'notbefore' are both null, then a default will be set to: expires = DateTime.UtcNow, notbefore = DateTime.UtcNow + TimeSpan.FromMinutes(TokenLifetimeInMinutes).
        /// </summary>
        public static readonly int DefaultTokenLifetimeInMinutes = 60;
        private static Type _x509AsymmKeyType = typeof (X509AsymmetricSecurityKey);
        private static FieldInfo _certFieldInfo = JwtSecurityTokenHandler._x509AsymmKeyType.GetField("certificate", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>Gets or sets the <see cref="T:System.Collections.Generic.IDictionary`2" /> used to map Inbound Cryptographic Algorithms.</summary>
        /// <remarks>Strings that describe Cryptographic Algorithms that are understood by the runtime are not necessarily the same values used in the JsonWebToken specification.
        /// <para>When a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> signature is validated, the algorithm is obtained from the HeaderParameter { alg, 'value' }.
        /// The 'value' is translated according to this mapping and the translated 'value' is used when performing cryptographic operations.</para>
        /// <para>Default mapping is:</para>
        /// <para>    RS256 =&gt; http://www.w3.org/2001/04/xmldsig-more#rsa-sha256 </para>
        /// <para>    HS256 =&gt; http://www.w3.org/2001/04/xmldsig-more#hmac-sha256 </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">'value' is null.</exception>
        public static IDictionary<string, string> InboundAlgorithmMap
        {
            get
            {
                return JwtSecurityTokenHandler.inboundAlgorithmMap;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                JwtSecurityTokenHandler.inboundAlgorithmMap = value;
            }
        }

        /// <summary>Gets or sets the <see cref="T:System.Collections.Generic.IDictionary`2" /> used to map Outbound Cryptographic Algorithms.</summary>
        /// <remarks>Strings that describe Cryptographic Algorithms understood by the runtime are not necessarily the same in the JsonWebToken specification.
        /// <para>This property contains mappings the will be used to when creating a <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> and setting the HeaderParameter { alg, 'value' }.
        /// The 'value' set is translated according to this mapping.
        /// </para>
        /// <para>Default mapping is:</para>
        /// <para>    http://www.w3.org/2001/04/xmldsig-more#rsa-sha256  =&gt; RS256</para>
        /// <para>    http://www.w3.org/2001/04/xmldsig-more#hmac-sha256 =&gt; HS256</para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">'value' is null.</exception>
        public static IDictionary<string, string> OutboundAlgorithmMap
        {
            get
            {
                return JwtSecurityTokenHandler.outboundAlgorithmMap;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                JwtSecurityTokenHandler.outboundAlgorithmMap = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.InboundClaimTypeMap" /> that is used when setting the <see cref="P:System.Security.Claims.Claim.Type" /> for claims in the <see cref="T:System.Security.Claims.ClaimsPrincipal" /> extracted when validating a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.
        /// <para>The <see cref="P:System.Security.Claims.Claim.Type" /> is set to the JSON claim 'name' after translating using this mapping.</para>
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">'value is null.</exception>
        public static IDictionary<string, string> InboundClaimTypeMap
        {
            get
            {
                return JwtSecurityTokenHandler.inboundClaimTypeMap;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                JwtSecurityTokenHandler.inboundClaimTypeMap = value;
            }
        }

        /// <summary>
        /// <para>Gets or sets the <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.OutboundClaimTypeMap" /> that is used when creating a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> from <see cref="T:System.Security.Claims.Claim" />(s).</para>
        /// <para>The JSON claim 'name' value is set to <see cref="P:System.Security.Claims.Claim.Type" /> after translating using this mapping.</para>
        /// </summary>
        /// <remarks>This mapping is applied only when using <see cref="M:System.IdentityModel.Tokens.JwtPayload.AddClaim(System.Security.Claims.Claim)" /> or <see cref="M:System.IdentityModel.Tokens.JwtPayload.AddClaims(System.Collections.Generic.IEnumerable{System.Security.Claims.Claim})" />. Adding values directly will not result in translation.</remarks>
        /// <exception cref="T:System.ArgumentNullException">'value is null.</exception>
        public static IDictionary<string, string> OutboundClaimTypeMap
        {
            get
            {
                return JwtSecurityTokenHandler.outboundClaimTypeMap;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                JwtSecurityTokenHandler.outboundClaimTypeMap = value;
            }
        }

        /// <summary>Gets or sets the <see cref="T:System.Collections.Generic.ISet`1" /> used to filter claims when populating a <see cref="T:System.Security.Claims.ClaimsIdentity" /> claims form a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.
        /// When a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> is validated, claims with types found in this <see cref="T:System.Collections.Generic.ISet`1" /> will not be added to the <see cref="T:System.Security.Claims.ClaimsIdentity" />.</summary>
        /// <exception cref="T:System.ArgumentNullException">'value' is null.</exception>
        public static ISet<string> InboundClaimFilter
        {
            get
            {
                return JwtSecurityTokenHandler.inboundClaimFilter;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                JwtSecurityTokenHandler.inboundClaimFilter = value;
            }
        }

        /// <summary>
        /// Gets or sets the property name of <see cref="P:System.Security.Claims.Claim.Properties" /> the will contain the original JSON claim 'name' if a mapping occurred when the <see cref="T:System.Security.Claims.Claim" />(s) were created.
        /// <para>See <seealso cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.InboundClaimTypeMap" /> for more information.</para>
        /// </summary>
        /// <exception cref="T:System.ArgumentException">if <see cref="T:System.String" />.IsIsNullOrWhiteSpace('value') is true.</exception>
        public static string ShortClaimTypeProperty
        {
            get
            {
                return JwtSecurityTokenHandler.shortClaimTypeProperty;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10000: The parameter '{0}' cannot be a 'null' or an empty string.", (object) nameof (value)));
                JwtSecurityTokenHandler.shortClaimTypeProperty = value;
            }
        }

        /// <summary>
        /// Gets or sets the property name of <see cref="P:System.Security.Claims.Claim.Properties" /> the will contain .Net type that was recogninzed when JwtPayload.Claims serialized the value to JSON.
        /// <para>See <seealso cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.InboundClaimTypeMap" /> for more information.</para>
        /// </summary>
        /// <exception cref="T:System.ArgumentException">if <see cref="T:System.String" />.IsIsNullOrWhiteSpace('value') is true.</exception>
        public static string JsonClaimTypeProperty
        {
            get
            {
                return JwtSecurityTokenHandler.jsonClaimTypeProperty;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10000: The parameter '{0}' cannot be a 'null' or an empty string.", (object) nameof (value)));
                JwtSecurityTokenHandler.jsonClaimTypeProperty = value;
            }
        }

        /// <summary>
        /// Returns 'true' which indicates this instance can validate a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.
        /// </summary>
        public bool CanValidateToken
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns 'true', which indicates this instance can write <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.
        /// </summary>
        public bool CanWriteToken
        {
            get
            {
                return true;
            }
        }

        /// <summary>Gets and sets the token lifetime in minutes.</summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">'value' less than 1.</exception>
        public int TokenLifetimeInMinutes
        {
            get
            {
                return this._defaultTokenLifetimeInMinutes;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10104: TokenLifetimeInMinutes must be greater than zero. value: '{0}'", (object) value.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
                this._defaultTokenLifetimeInMinutes = value;
            }
        }

        /// <summary>
        /// Gets and sets the maximum size in bytes, that a will be processed.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">'value' less than 1.</exception>
        public int MaximumTokenSizeInBytes
        {
            get
            {
                return this._maximumTokenSizeInBytes;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10101: MaximumTokenSizeInBytes must be greater than zero. value: '{0}'", (object) value.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
                this._maximumTokenSizeInBytes = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.SignatureProviderFactory" /> for creating <see cref="T:System.IdentityModel.Tokens.SignatureProvider" />(s).
        /// </summary>
        /// <remarks>This extensibility point can be used to insert custom <see cref="T:System.IdentityModel.Tokens.SignatureProvider" />(s).
        /// <para><see cref="M:System.IdentityModel.Tokens.SignatureProviderFactory.CreateForVerifying(System.IdentityModel.Tokens.SecurityKey,System.String)" /> is called to obtain a <see cref="T:System.IdentityModel.Tokens.SignatureProvider" />(s) when needed.</para></remarks>
        /// <exception cref="T:System.ArgumentNullException">'value' is null.</exception>
        public SignatureProviderFactory SignatureProviderFactory
        {
            get
            {
                return this.signatureProviderFactory;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                this.signatureProviderFactory = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type" /> supported by this handler.
        /// </summary>
        public Type TokenType
        {
            get
            {
                return typeof (JwtSecurityToken);
            }
        }

        /// <summary>
        /// Determines if the <see cref="T:System.Xml.XmlReader" /> is positioned on a well formed &lt;BinarySecurityToken&gt; element.
        /// </summary>
        /// <param name="reader"><see cref="T:System.Xml.XmlReader" /> positioned at xml.</param>
        /// <returns>
        /// <para>'true' if the reader is positioned at an element &lt;BinarySecurityToken&gt;.
        /// in the namespace: 'http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'</para>
        /// <para>With an attribute of 'valueType' equal to one of: </para>
        /// <para>    "urn:ietf:params:oauth:token-type:jwt", "JWT" </para>
        /// <para>
        /// For example: &lt;wsse:BinarySecurityToken valueType = "JWT"&gt; ...
        /// </para>
        /// 'false' otherwise.
        /// </returns>
        /// <remarks>The 'EncodingType' attribute is optional, if it is set, it must be equal to: "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary".</remarks>
        /// <exception cref="T:System.ArgumentNullException">'reader' is null.</exception>
        public bool CanReadToken(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof (reader));
            try
            {
                int content = (int) reader.MoveToContent();
                if (reader.IsStartElement("BinarySecurityToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"))
                {
                    string attribute1 = reader.GetAttribute("ValueType", (string) null);
                    string attribute2 = reader.GetAttribute("EncodingType", (string) null);
                    return (attribute2 == null || StringComparer.Ordinal.Equals(attribute2, "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary")) && (attribute1 == null || StringComparer.Ordinal.Equals(attribute1, "urn:ietf:params:oauth:token-type:jwt") || StringComparer.OrdinalIgnoreCase.Equals(attribute1, "JWT"));
                }
            }
            catch (XmlException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            return false;
        }

        /// <summary>
        /// Determines if the string is a well formed Json Web token (see http://tools.ietf.org/html/draft-ietf-oauth-json-web-token-07)
        /// </summary>
        /// <param name="tokenString">string that should represent a valid JSON Web Token.</param>
        /// <remarks>Uses <see cref="M:System.Text.RegularExpressions.Regex.IsMatch(System.String,System.String)" />( token, @"^[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+\.[A-Za-z0-9-_]*$" ).
        /// </remarks>
        /// <returns>
        /// <para>'true' if the token is in JSON compact serialization format.</para>
        /// <para>'false' if token.Length * 2 &gt;  <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.MaximumTokenSizeInBytes" />.</para>
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">'tokenString' is null.</exception>
        public bool CanReadToken(string tokenString)
        {
            if (tokenString == null)
                throw new ArgumentNullException(nameof (tokenString));
            if (tokenString.Length * 2 > this.MaximumTokenSizeInBytes)
                return false;
            if (!Regex.IsMatch(tokenString, "^[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]*$"))
                return this.CanReadToken(XmlReader.Create((Stream) new MemoryStream(Encoding.UTF8.GetBytes(tokenString))));
            return true;
        }

        /// <summary>
        /// Creating <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> is not NotSupported.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException"> to create a <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" />.</exception>
        public SecurityKeyIdentifierClause CreateSecurityTokenReference(
            SecurityToken token,
            bool attached)
        {
            throw new NotSupportedException("IDX11005: Creating a SecurityKeyIdentifierClause is not supported.");
        }

        /// <summary>
        /// Creates a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> based on values found in the <see cref="T:System.IdentityModel.Tokens.SecurityTokenDescriptor" />.
        /// </summary>
        /// <param name="tokenDescriptor">Contains the parameters used to create the token.</param>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.</returns>
        /// <remarks>
        /// If <see cref="P:System.IdentityModel.Tokens.SecurityTokenDescriptor.SigningCredentials" /> is not null, <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.RawData" /> will be signed.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">'tokenDescriptor' is null.</exception>
        public SecurityToken CreateToken(SimpleTokenDescriptor tokenDescriptor)
        {
            if (tokenDescriptor == null)
                throw new ArgumentNullException(nameof (tokenDescriptor));
            DateTime? notBefore = tokenDescriptor.Lifetime == null ? new DateTime?() : tokenDescriptor.Lifetime.Created;
            DateTime? expires = tokenDescriptor.Lifetime == null ? new DateTime?() : tokenDescriptor.Lifetime.Expires;
            return (SecurityToken) this.CreateToken(tokenDescriptor.TokenIssuerName, tokenDescriptor.AppliesToAddress, tokenDescriptor.Subject, notBefore, expires, tokenDescriptor.SigningCredentials, (SignatureProvider) null);
        }

        /// <summary>
        /// Uses the <see cref="M:System.IdentityModel.Tokens.JwtSecurityToken.#ctor(System.IdentityModel.Tokens.JwtHeader,System.IdentityModel.Tokens.JwtPayload,System.String,System.String,System.String)" /> constructor, first creating the <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> and <see cref="T:System.IdentityModel.Tokens.JwtPayload" />.
        /// <para>If <see cref="T:System.IdentityModel.Tokens.SigningCredentials" /> is not null, <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.RawData" /> will be signed.</para>
        /// </summary>
        /// <param name="issuer">the issuer of the token.</param>
        /// <param name="audience">the audience for this token.</param>
        /// <param name="subject">the source of the <see cref="T:System.Security.Claims.Claim" />(s) for this token.</param>
        /// <param name="notBefore">the notbefore time for this token.</param>
        /// <param name="expires">the expiration time for this token.</param>
        /// <param name="signingCredentials">contains cryptographic material for generating a signature.</param>
        /// <param name="signatureProvider">optional <see cref="T:System.IdentityModel.Tokens.SignatureProvider" />.</param>
        /// <remarks>If <see cref="P:System.Security.Claims.ClaimsIdentity.Actor" /> is not null, then a claim { actort, 'value' } will be added to the payload. <see cref="M:System.IdentityModel.Tokens.JwtSecurityTokenHandler.CreateActorValue(System.Security.Claims.ClaimsIdentity)" /> for details on how the value is created.
        /// <para>See <seealso cref="T:System.IdentityModel.Tokens.JwtHeader" /> for details on how the HeaderParameters are added to the header.</para>
        /// <para>See <seealso cref="T:System.IdentityModel.Tokens.JwtPayload" /> for details on how the values are added to the payload.</para></remarks>
        /// <para>If signautureProvider is not null, then it will be used to create the signature and <see cref="M:System.IdentityModel.Tokens.SignatureProviderFactory.CreateForSigning(System.IdentityModel.Tokens.SecurityKey,System.String)" /> will not be called.</para>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.</returns>
        /// <exception cref="T:System.ArgumentException">if 'expires' &lt;= 'notBefore'.</exception>
        public virtual JwtSecurityToken CreateToken(
            string issuer = null,
            string audience = null,
            ClaimsIdentity subject = null,
            DateTime? notBefore = null,
            DateTime? expires = null,
            SigningCredentials signingCredentials = null,
            SignatureProvider signatureProvider = null)
        {
            if (expires.HasValue && notBefore.HasValue)
            {
                DateTime? nullable1 = notBefore;
                DateTime? nullable2 = expires;
                if ((nullable1.HasValue & nullable2.HasValue ? (nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                    throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10401: Expires: '{0}' must be after NotBefore: '{1}'.", (object) expires.Value, (object) notBefore.Value));
            }
            if (!expires.HasValue && !notBefore.HasValue)
            {
                DateTime utcNow = DateTime.UtcNow;
                expires = new DateTime?(utcNow + TimeSpan.FromMinutes((double) this.TokenLifetimeInMinutes));
                notBefore = new DateTime?(utcNow);
            }
            JwtPayload payload = new JwtPayload(issuer, audience, subject == null ? (IEnumerable<Claim>) null : subject.Claims, notBefore, expires);
            JwtHeader header = new JwtHeader(signingCredentials);
            if (subject != null && subject.Actor != null)
                payload.AddClaim(new Claim("actort", this.CreateActorValue(subject.Actor)));
            string rawHeader = header.Base64UrlEncode();
            string rawPayload = payload.Base64UrlEncode();
            string rawSignature = string.Empty;
            string inputString = rawHeader + "." + rawPayload;
            if (signatureProvider != null)
                rawSignature = Base64UrlEncoder.Encode(this.CreateSignature(inputString, (SecurityKey) null, (string) null, signatureProvider));
            else if (signingCredentials != null)
                rawSignature = Base64UrlEncoder.Encode(this.CreateSignature(inputString, signingCredentials.SigningKey, signingCredentials.SignatureAlgorithm, signatureProvider));
            return new JwtSecurityToken(header, payload, rawHeader, rawPayload, rawSignature);
        }

        /// <summary>
        /// Gets the token type identifier(s) supported by this handler.
        /// </summary>
        /// <returns>A collection of strings that identify the tokens this instance can handle.</returns>
        /// <remarks>When receiving a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> wrapped inside a &lt;wsse:BinarySecurityToken&gt; element. The &lt;wsse:BinarySecurityToken&gt; element must have the ValueType attribute set to one of these values
        /// in order for this handler to recognize that it can read the token.</remarks>
        public string[] GetTokenTypeIdentifiers()
        {
            return JwtSecurityTokenHandler.tokenTypeIdentifiers;
        }

        /// <summary>
        /// Reads a JSON web token wrapped inside a WS-Security BinarySecurityToken xml element.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> pointing at the jwt.</param>
        /// <returns>An instance of <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /></returns>
        /// <remarks>First calls <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.CanReadToken
        /// <para>The reader must be positioned at an element named:</para>
        /// <para>BinarySecurityToken'.
        /// in the namespace: 'http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'
        /// with a 'ValueType' attribute equal to one of: "urn:ietf:params:oauth:token-type:jwt", "JWT".</para>
        /// <para>
        /// For example &lt;wsse:BinarySecurityToken valueType = "JWT"&gt; ...
        /// </para>
        /// <para>
        /// The 'EncodingType' attribute is optional, if it is set, it must be equal to: "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"
        /// </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">'reader' is null.</exception>
        /// <exception cref="T:System.ArgumentException">if <see cref="M:System.IdentityModel.Tokens.JwtSecurityTokenHandler.CanReadToken(System.Xml.XmlReader)" /> returns false.</exception>
        public SecurityToken ReadToken(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof (reader));
            if (!this.CanReadToken(reader))
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10707: '{0}' cannot read this xml: '{1}'. The reader needs to be positioned at an element: '{2}', within the namespace: '{3}', with an attribute: '{4}' equal to one of the following: '{5}', '{6}'.", (object) this.GetType().ToString(), (object) reader.ReadOuterXml(), (object) "BinarySecurityToken", (object) "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", (object) "ValueType", (object) "urn:ietf:params:oauth:token-type:jwt", (object) "JWT"));
            using (XmlDictionaryReader dictionaryReader = XmlDictionaryReader.CreateDictionaryReader(reader))
            {
                string attribute = dictionaryReader.GetAttribute("Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
                JwtSecurityToken jwtSecurityToken = this.ReadToken(Encoding.UTF8.GetString(dictionaryReader.ReadElementContentAsBase64())) as JwtSecurityToken;
                if (attribute != null && jwtSecurityToken != null)
                    jwtSecurityToken.SetId(attribute);
                return (SecurityToken) jwtSecurityToken;
            }
        }

        /// <summary>
        /// Reads a token encoded in JSON Compact serialized format.
        /// </summary>
        /// <param name="tokenString">A 'JSON Web Token' (JWT) that has been encoded as a JSON object. May be signed
        /// using 'JSON Web Signature' (JWS).</param>
        /// <remarks>
        /// The JWT must be encoded using Base64Url encoding of the UTF-8 representation of the JWT: Header, Payload and Signature.
        /// The contents of the JWT returned are not validated in any way, the token is simply decoded. Use ValidateToken to validate the JWT.
        /// </remarks>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /></returns>
        public SecurityToken ReadToken(string tokenString)
        {
            if (tokenString == null)
                throw new ArgumentNullException("token");
            if (tokenString.Length * 2 > this.MaximumTokenSizeInBytes)
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10209: token has length: '{0}' which is larger than the MaximumTokenSizeInBytes: '{1}'.", (object) tokenString.Length, (object) this.MaximumTokenSizeInBytes));
            if (!this.CanReadToken(tokenString))
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10708: '{0}' cannot read this string: '{1}'.\nThe string needs to be in compact JSON format, which is of the form: '<Base64UrlEncodedHeader>.<Base64UrlEndcodedPayload>.<OPTIONAL, Base64UrlEncodedSignature>'.", (object) this.GetType(), (object) tokenString));
            if (Regex.IsMatch(tokenString, "^[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]*$"))
                return (SecurityToken) new JwtSecurityToken(tokenString);
            return this.ReadToken(XmlReader.Create((Stream) new MemoryStream(Encoding.UTF8.GetBytes(tokenString))));
        }

        /// <summary>
        /// Obsolete method, use <see cref="M:System.IdentityModel.Tokens.JwtSecurityTokenHandler.ValidateToken(System.String,System.IdentityModel.Tokens.TokenValidationParameters,System.IdentityModel.Tokens.SecurityToken@)" />.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException"> use <see cref="M:System.IdentityModel.Tokens.JwtSecurityTokenHandler.ValidateToken(System.String,System.IdentityModel.Tokens.TokenValidationParameters,System.IdentityModel.Tokens.SecurityToken@)" />.</exception>
        public ReadOnlyCollection<ClaimsIdentity> ValidateToken(
            SecurityToken token)
        {
            throw new NotSupportedException("IDX11008: This method is not supported to validate a 'jwt' use the method: ValidateToken(String, TokenValidationParameters, out SecurityToken).");
        }

        /// <summary>
        /// Reads and validates a token encoded in JSON Compact serialized format.
        /// </summary>
        /// <param name="securityToken">A 'JSON Web Token' (JWT) that has been encoded as a JSON object. May be signed using 'JSON Web Signature' (JWS).</param>
        /// <param name="validationParameters">Contains validation parameters for the <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.</param>
        /// <param name="validatedToken">The <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> that was validated.</param>
        /// <exception cref="T:System.ArgumentNullException">'securityToken' is null or whitespace.</exception>
        /// <exception cref="T:System.ArgumentNullException">'validationParameters' is null.</exception>
        /// <exception cref="T:System.ArgumentException">'securityToken.Length' &gt; <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.MaximumTokenSizeInBytes" />.</exception>
        /// <returns>A <see cref="T:System.Security.Claims.ClaimsPrincipal" /> from the jwt. Does not include the header claims.</returns>
        public virtual ClaimsPrincipal ValidateToken(
            string securityToken,
            TokenValidationParameters validationParameters,
            out SecurityToken validatedToken)
        {
            if (string.IsNullOrWhiteSpace(securityToken))
                throw new ArgumentNullException(nameof (securityToken));
            if (validationParameters == null)
                throw new ArgumentNullException(nameof (validationParameters));
            if (securityToken.Length > this.MaximumTokenSizeInBytes)
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10209: token has length: '{0}' which is larger than the MaximumTokenSizeInBytes: '{1}'.", (object) securityToken.Length, (object) this.MaximumTokenSizeInBytes));
            JwtSecurityToken jwt = this.ValidateSignature(securityToken, validationParameters);
            if (jwt.SigningKey != null)
                this.ValidateIssuerSecurityKey(jwt.SigningKey, (SecurityToken) jwt, validationParameters);
            DateTime? notBefore = new DateTime?();
            if (jwt.Payload.Nbf.HasValue)
                notBefore = new DateTime?(jwt.ValidFrom);
            DateTime? nullable = new DateTime?();
            if (jwt.Payload.Exp.HasValue)
                nullable = new DateTime?(jwt.ValidTo);
            Validators.ValidateTokenReplay(securityToken, nullable, validationParameters);
            if (validationParameters.ValidateLifetime)
            {
                ValidateLifetime(notBefore, nullable, (SecurityToken) jwt, validationParameters);
            }
            if (validationParameters.ValidateAudience)
            {
                ValidateAudience(jwt.Audiences, (SecurityToken) jwt, validationParameters);
            }
            string issuer = jwt.Issuer;
            if (validationParameters.ValidateIssuer)
                issuer = this.ValidateIssuer(issuer, (SecurityToken) jwt, validationParameters);

            if (validationParameters.ValidateActor && !string.IsNullOrWhiteSpace(jwt.Actor))
            {
                SecurityToken validatedToken1 = (SecurityToken) null;
                this.ValidateToken(jwt.Actor, validationParameters, out validatedToken1);
            }
            ClaimsIdentity claimsIdentity = this.CreateClaimsIdentity(jwt, issuer, validationParameters);

            if (validationParameters.SaveSigninToken)
                claimsIdentity.BootstrapContext = (object) new BootstrapContext(securityToken);
            validatedToken = (SecurityToken) jwt;
            return new ClaimsPrincipal((IIdentity) claimsIdentity);
        }

        /// <summary>
        /// Writes the <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> wrapped in a WS-Security BinarySecurityToken using the <see cref="T:System.Xml.XmlWriter" />.
        /// </summary>
        /// <param name="writer"><see cref="T:System.Xml.XmlWriter" /> used to write token.</param>
        /// <param name="token">The <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> that will be written.</param>
        /// <exception cref="T:System.ArgumentNullException">'writer' is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">'token' is null.</exception>
        /// <exception cref="T:System.ArgumentException">'token' is not a not <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.</exception>
        /// <remarks>The <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> current contents are encoded. If <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.SigningCredentials" /> is not null, the encoding will contain a signature.</remarks>
        public void WriteToken(XmlWriter writer, SecurityToken token)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof (writer));
            if (token == null)
                throw new ArgumentNullException(nameof (token));
            if (!(token is JwtSecurityToken))
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10226: '{0}' can only write SecurityTokens of type: '{1}', 'token' type is: '{2}'.", (object) this.GetType(), (object) typeof (JwtSecurityToken), (object) token.GetType()));
            byte[] bytes = Encoding.UTF8.GetBytes(this.WriteToken(token));
            writer.WriteStartElement("wsse", "BinarySecurityToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            if (token.Id != null)
                writer.WriteAttributeString("wsse", "Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", token.Id);
            writer.WriteAttributeString("ValueType", (string) null, "urn:ietf:params:oauth:token-type:jwt");
            writer.WriteAttributeString("EncodingType", (string) null, "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
            writer.WriteBase64(bytes, 0, bytes.Length);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> as a JSON Compact serialized format string.
        /// </summary>
        /// <param name="token"><see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> to serialize.</param>
        /// <remarks>
        /// <para>If the <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.SigningCredentials" /> are not null, the encoding will contain a signature.</para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">'token' is null.</exception>
        /// <exception cref="T:System.ArgumentException">'token' is not a not <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.</exception>
        /// <returns>The <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> as a signed (if <see cref="T:System.IdentityModel.Tokens.SigningCredentials" /> exist) encoded string.</returns>
        public string WriteToken(SecurityToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof (token));
            JwtSecurityToken jwtSecurityToken = token as JwtSecurityToken;
            if (jwtSecurityToken == null)
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10706: '{0}' can only write SecurityTokens of type: '{1}', 'token' type is: '{2}'.", (object) this.GetType(), (object) typeof (JwtSecurityToken), (object) token.GetType()));
            string str = string.Empty;
            string inputString = jwtSecurityToken.EncodedHeader + "." + jwtSecurityToken.EncodedPayload;
            if (jwtSecurityToken.SigningCredentials != null)
                str = Base64UrlEncoder.Encode(this.CreateSignature(inputString, jwtSecurityToken.SigningCredentials.SigningKey, jwtSecurityToken.SigningCredentials.SignatureAlgorithm, (SignatureProvider) null));
            return inputString + "." + str;
        }

        /// <summary>
        /// Produces a signature over the 'input' using the <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> and algorithm specified.
        /// </summary>
        /// <param name="inputString">string to be signed</param>
        /// <param name="key">the <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> to use.</param>
        /// <param name="algorithm">the algorithm to use.</param>
        /// <param name="signatureProvider">if provided, the <see cref="T:System.IdentityModel.Tokens.SignatureProvider" /> will be used to sign the token</param>
        /// <returns>The signature over the bytes obtained from UTF8Encoding.GetBytes( 'input' ).</returns>
        /// <remarks>The <see cref="T:System.IdentityModel.Tokens.SignatureProvider" /> used to created the signature is obtained by calling <see cref="M:System.IdentityModel.Tokens.SignatureProviderFactory.CreateForSigning(System.IdentityModel.Tokens.SecurityKey,System.String)" />.</remarks>
        /// <exception cref="T:System.ArgumentNullException">'input' is null.</exception>
        /// <exception cref="T:System.InvalidProgramException"><see cref="M:System.IdentityModel.Tokens.SignatureProviderFactory.CreateForSigning(System.IdentityModel.Tokens.SecurityKey,System.String)" /> returns null.</exception>
        internal byte[] CreateSignature(
            string inputString,
            SecurityKey key,
            string algorithm,
            SignatureProvider signatureProvider = null)
        {
            if (inputString == null)
                throw new ArgumentNullException(nameof (inputString));
            if (signatureProvider != null)
                return signatureProvider.Sign(Encoding.UTF8.GetBytes(inputString));
            SignatureProvider forSigning = this.SignatureProviderFactory.CreateForSigning(key, algorithm);
            if (forSigning == null)
                throw new InvalidProgramException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10635: Unable to create signature. '{0}' returned a null '{1}'. SecurityKey: '{2}', Algorithm: '{3}'", (object) this.SignatureProviderFactory.GetType(), (object) typeof (SignatureProvider), key == null ? (object) "<null>" : (object) key.GetType().ToString(), algorithm == null ? (object) "<null>" : (object) algorithm));
            byte[] numArray = forSigning.Sign(Encoding.UTF8.GetBytes(inputString));
            this.SignatureProviderFactory.ReleaseProvider(forSigning);
            return numArray;
        }

        private bool ValidateSignature(
            byte[] encodedBytes,
            byte[] signature,
            SecurityKey key,
            string algorithm)
        {
            SignatureProvider forVerifying = this.SignatureProviderFactory.CreateForVerifying(key, algorithm);
            if (forVerifying == null)
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10636: SignatureProviderFactory.CreateForVerifying returned null for key: '{0}', signatureAlgorithm: '{1}'.", key == null ? (object) "null" : (object) key.ToString(), algorithm == null ? (object) "null" : (object) algorithm));
            return forVerifying.Verify(encodedBytes, signature);
        }

        /// <summary>
        /// Validates that the signature, if found and / or required is valid.
        /// </summary>
        /// <param name="token">A 'JSON Web Token' (JWT) that has been encoded as a JSON object. May be signed
        /// using 'JSON Web Signature' (JWS).</param>
        /// <param name="validationParameters"><see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" /> that contains signing keys.</param>
        /// <exception cref="T:System.ArgumentNullException"> thrown if 'token is null or whitespace.</exception>
        /// <exception cref="T:System.ArgumentNullException"> thrown if 'validationParameters is null.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenValidationException"> thrown if a signature is not found and <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.RequireSignedTokens" /> is true.</exception>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenSignatureKeyNotFoundException"> thrown if the 'token' has a key identifier and none of the <see cref="T:System.IdentityModel.Tokens.SecurityKey" />(s) provided result in a validated signature.
        /// This can indicate that a key refresh is required.</exception>
        /// <exception cref="T:System.IdentityModel.SignatureVerificationFailedException"> thrown if after trying all the <see cref="T:System.IdentityModel.Tokens.SecurityKey" />(s), none result in a validated signture AND the 'token' does not have a key identifier.</exception>
        /// <returns><see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> that has the signature validated if token was signed and <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.RequireSignedTokens" /> is true.</returns>
        /// <remarks><para>If the 'token' is signed, the signature is validated even if <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.RequireSignedTokens" /> is false.</para>
        /// <para>If the 'token' signature is validated, then the <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.SigningKey" /> will be set to the key that signed the 'token'.</para></remarks>
        protected virtual JwtSecurityToken ValidateSignature(
            string token,
            TokenValidationParameters validationParameters)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof (token));
            if (validationParameters == null)
                throw new ArgumentNullException(nameof (validationParameters));
            JwtSecurityToken jwtSecurityToken = this.ReadToken(token) as JwtSecurityToken;
            byte[] bytes = Encoding.UTF8.GetBytes(jwtSecurityToken.RawHeader + "." + jwtSecurityToken.RawPayload);
            byte[] signature = Base64UrlEncoder.DecodeBytes(jwtSecurityToken.RawSignature);
            if (signature == null)
                throw new ArgumentNullException("signatureBytes");
            if (signature.Length == 0)
            {
                if (!validationParameters.RequireSignedTokens)
                    return jwtSecurityToken;
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10504: Unable to validate signature, token does not have a signature: '{0}'", (object) jwtSecurityToken.ToString()));
            }
            string index = jwtSecurityToken.Header.Alg;
            if (index != null && JwtSecurityTokenHandler.InboundAlgorithmMap.ContainsKey(index))
                index = JwtSecurityTokenHandler.InboundAlgorithmMap[index];
            SecurityKeyIdentifier signingKeyIdentifier = jwtSecurityToken.Header.SigningKeyIdentifier;
            if (signingKeyIdentifier.Count > 0)
            {
                SecurityKey securityKey;
                    securityKey = this.ResolveIssuerSigningKey(token, (SecurityToken) jwtSecurityToken, signingKeyIdentifier, validationParameters);
                    if (securityKey == null)
                        throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10500: Signature validation failed. Unable to resolve SecurityKeyIdentifier: '{0}', \ntoken: '{1}'.", (object) signingKeyIdentifier, (object) jwtSecurityToken.ToString()));
                try
                {
                    if (this.ValidateSignature(bytes, signature, securityKey, index))
                    {
                        jwtSecurityToken.SigningKey = securityKey;
                        return jwtSecurityToken;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10502: Signature validation failed. Key tried: '{0}'.\nException caught:\n '{1}'.\ntoken: '{2}'", (object) JwtSecurityTokenHandler.CreateKeyString(securityKey), (object) ex.ToString(), (object) jwtSecurityToken.ToString()), ex);
                }
                throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10501: Signature validation failed. Key tried: '{0}'.\ntoken: '{1}'", (object) JwtSecurityTokenHandler.CreateKeyString(securityKey), (object) jwtSecurityToken.ToString()));
            }
            Exception inner = (Exception) null;
            StringBuilder stringBuilder1 = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            foreach (SecurityKey allKey in this.GetAllKeys(token, (SecurityToken) jwtSecurityToken, signingKeyIdentifier, validationParameters))
            {
                try
                {
                    if (this.ValidateSignature(bytes, signature, allKey, index))
                    {
                        jwtSecurityToken.SigningKey = allKey;
                        return jwtSecurityToken;
                    }
                }
                catch (Exception ex)
                {
                    if (inner == null) inner = ex;
                    stringBuilder1.AppendLine(ex.ToString());
                }
                stringBuilder2.AppendLine(JwtSecurityTokenHandler.CreateKeyString(allKey));
            }
            throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10503: Signature validation failed. Keys tried: '{0}'.\nExceptions caught:\n '{1}'.\ntoken: '{2}'", (object) stringBuilder2.ToString(), (object) stringBuilder1.ToString(), (object) jwtSecurityToken.ToString()), inner);
        }

        private IEnumerable<SecurityKey> GetAllKeys(
            string token,
            SecurityToken securityToken,
            SecurityKeyIdentifier keyIdentifier,
            TokenValidationParameters validationParameters)
        {
            foreach (SecurityToken issuerSigningToken in validationParameters.IssuerSigningTokens)
            {
                foreach (SecurityKey securityKey in issuerSigningToken.SecurityKeys)
                    yield return securityKey;
            }
        }

        /// <summary>
        /// Produces a readable string for a key, used in error messages.
        /// </summary>
        /// <param name="securityKey"></param>
        /// <returns></returns>
        private static string CreateKeyString(SecurityKey securityKey)
        {
            if (securityKey == null)
                return "null";
            return securityKey.ToString();
        }

        /// <summary>
        /// Creates a <see cref="T:System.Security.Claims.ClaimsIdentity" /> from a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.
        /// </summary>
        /// <param name="jwt">The <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> to use as a <see cref="T:System.Security.Claims.Claim" /> source.</param>
        /// <param name="issuer">The value to set <see cref="P:System.Security.Claims.Claim.Issuer" /></param>
        /// <param name="validationParameters"> contains parameters for validating the token.</param>
        /// <returns>A <see cref="T:System.Security.Claims.ClaimsIdentity" /> containing the <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.Claims" />.</returns>
        protected virtual ClaimsIdentity CreateClaimsIdentity(
            JwtSecurityToken jwt,
            string issuer,
            TokenValidationParameters validationParameters)
        {
            if (jwt == null)
                throw new ArgumentNullException(nameof (jwt));
            if (string.IsNullOrWhiteSpace(issuer))
                throw new ArgumentException("IDX10221: Unable to create claims from securityToken, 'issuer' is null or empty.");
            ClaimsIdentity claimsIdentity = validationParameters.CreateClaimsIdentity((SecurityToken) jwt, issuer);
            foreach (Claim claim1 in jwt.Claims)
            {
                if (!JwtSecurityTokenHandler.InboundClaimFilter.Contains(claim1.Type))
                {
                    bool flag = true;
                    string type;
                    if (!JwtSecurityTokenHandler.InboundClaimTypeMap.TryGetValue(claim1.Type, out type))
                    {
                        type = claim1.Type;
                        flag = false;
                    }
                    if (type == "http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor")
                    {
                        if (claimsIdentity.Actor != null)
                            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10710: Only a single 'Actor' is supported. Found second claim of type: '{0}', value: '{1}'", (object) "actort", (object) claim1.Value));
                        if (this.CanReadToken(claim1.Value))
                        {
                            JwtSecurityToken jwt1 = this.ReadToken(claim1.Value) as JwtSecurityToken;
                            claimsIdentity.Actor = this.CreateClaimsIdentity(jwt1, issuer, validationParameters);
                        }
                    }
                    Claim claim2 = new Claim(type, claim1.Value, claim1.ValueType, issuer, issuer, claimsIdentity);
                    if (claim1.Properties.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) claim1.Properties)
                            claim2.Properties[property.Key] = property.Value;
                    }
                    if (flag)
                        claim2.Properties[JwtSecurityTokenHandler.ShortClaimTypeProperty] = claim1.Type;
                    claimsIdentity.AddClaim(claim2);
                }
            }
            return claimsIdentity;
        }

        /// <summary>
        /// Creates the 'value' for the actor claim: { actort, 'value' }
        /// </summary>
        /// <param name="actor"><see cref="T:System.Security.Claims.ClaimsIdentity" /> as actor.</param>
        /// <returns><see cref="T:System.String" /> representing the actor.</returns>
        /// <remarks>If <see cref="P:System.Security.Claims.ClaimsIdentity.BootstrapContext" /> is not null:
        /// <para>  if 'type' is 'string', return as string.</para>
        /// <para>  if 'type' is 'BootstrapContext' and 'BootstrapContext.SecurityToken' is 'JwtSecurityToken'</para>
        /// <para>    if 'JwtSecurityToken.RawData' != null, return RawData.</para>
        /// <para>    else return <see cref="M:System.IdentityModel.Tokens.JwtSecurityTokenHandler.WriteToken(System.IdentityModel.Tokens.SecurityToken)" />.</para>
        /// <para>  if 'BootstrapContext.Token' != null, return 'Token'.</para>
        /// <para>default: <see cref="M:System.IdentityModel.Tokens.JwtSecurityTokenHandler.WriteToken(System.IdentityModel.Tokens.SecurityToken)" /> new ( <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />( actor.Claims ).</para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">'actor' is null.</exception>
        protected virtual string CreateActorValue(ClaimsIdentity actor)
        {
            if (actor == null)
                throw new ArgumentNullException(nameof (actor));
            if (actor.BootstrapContext != null)
            {
                string bootstrapContext1 = actor.BootstrapContext as string;
                if (bootstrapContext1 != null)
                    return bootstrapContext1;
                BootstrapContext bootstrapContext2 = actor.BootstrapContext as BootstrapContext;
                if (bootstrapContext2 != null)
                {
                    if (bootstrapContext2.Token != null)
                        return bootstrapContext2.Token;
                }
            }
            return this.WriteToken((SecurityToken) new JwtSecurityToken((string) null, (string) null, actor.Claims, new DateTime?(), new DateTime?(), (SigningCredentials) null));
        }

        /// <summary>
        /// Determines if the audiences found in a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> are valid.
        /// </summary>
        /// <param name="audiences">The audiences found in the <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.</param>
        /// <param name="securityToken">The <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> being validated.</param>
        /// <param name="validationParameters"><see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" /> required for validation.</param>
        /// <remarks>see <see cref="M:System.IdentityModel.Tokens.Validators.ValidateAudience(System.Collections.Generic.IEnumerable{System.String},System.IdentityModel.Tokens.SecurityToken,System.IdentityModel.Tokens.TokenValidationParameters)" /> for additional details.</remarks>
        protected virtual void ValidateAudience(
            IEnumerable<string> audiences,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            Validators.ValidateAudience(audiences, securityToken, validationParameters);
        }

        /// <summary>
        /// Validates the lifetime of a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.
        /// </summary>
        /// <param name="notBefore">The <see cref="T:System.DateTime" /> value of the 'nbf' claim if it exists in the 'jwt'.</param>
        /// <param name="expires">The <see cref="T:System.DateTime" /> value of the 'exp' claim if it exists in the 'jwt'.</param>
        /// <param name="securityToken">The <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> being validated.</param>
        /// <param name="validationParameters"><see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" /> required for validation.</param>
        /// <remarks><see cref="M:System.IdentityModel.Tokens.Validators.ValidateLifetime(System.Nullable{System.DateTime},System.Nullable{System.DateTime},System.IdentityModel.Tokens.SecurityToken,System.IdentityModel.Tokens.TokenValidationParameters)" /> for additional details.</remarks>
        protected virtual void ValidateLifetime(
            DateTime? notBefore,
            DateTime? expires,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            Validators.ValidateLifetime(notBefore, expires, securityToken, validationParameters);
        }

        /// <summary>
        /// Determines if an issuer found in a <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> is valid.
        /// </summary>
        /// <param name="issuer">The issuer to validate</param>
        /// <param name="securityToken">The <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> that is being validated.</param>
        /// <param name="validationParameters"><see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" /> required for validation.</param>
        /// <returns>The issuer to use when creating the <see cref="T:System.Security.Claims.Claim" />(s) in the <see cref="T:System.Security.Claims.ClaimsIdentity" />.</returns>
        /// <remarks><see cref="M:System.IdentityModel.Tokens.Validators.ValidateIssuer(System.String,System.IdentityModel.Tokens.SecurityToken,System.IdentityModel.Tokens.TokenValidationParameters)" /> for additional details.</remarks>
        protected virtual string ValidateIssuer(
            string issuer,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            return Validators.ValidateIssuer(issuer, securityToken, validationParameters);
        }

        /// <summary>
        /// Returns a <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> to use when validating the signature of a token.
        /// </summary>
        /// <param name="token">the <see cref="T:System.String" /> representation of the token that is being validated.</param>
        /// <param name="securityToken">the <SecurityToken> that is being validated.</SecurityToken></param>
        /// <param name="keyIdentifier">the <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> found in the token.</param>
        /// <param name="validationParameters">A <see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" />  required for validation.</param>
        /// <returns>Returns a <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> to use for signature validation.</returns>
        /// <exception cref="T:System.ArgumentNullException">if 'keyIdentifier' is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">if 'validationParameters' is null.</exception>
        /// <remarks>If key fails to resolve, then null is returned</remarks>
        protected virtual SecurityKey ResolveIssuerSigningKey(
            string token,
            SecurityToken securityToken,
            SecurityKeyIdentifier keyIdentifier,
            TokenValidationParameters validationParameters)
        {
            if (keyIdentifier == null)
                throw new ArgumentNullException(nameof (keyIdentifier));
            if (validationParameters == null)
                throw new ArgumentNullException(nameof (validationParameters));
            foreach (SecurityKeyIdentifierClause keyIdentifierClause in keyIdentifier)
            {
                JwtSecurityTokenHandler.CertMatcher certMatcher = (JwtSecurityTokenHandler.CertMatcher) null;
                X509RawDataKeyIdentifierClause identifierClause1 = keyIdentifierClause as X509RawDataKeyIdentifierClause;
                if (identifierClause1 != null)
                {
                    certMatcher = new JwtSecurityTokenHandler.CertMatcher(identifierClause1.Matches);
                }
                else
                {
                    X509SubjectKeyIdentifierClause identifierClause2 = keyIdentifierClause as X509SubjectKeyIdentifierClause;
                    if (identifierClause2 != null)
                    {
                        certMatcher = new JwtSecurityTokenHandler.CertMatcher(identifierClause2.Matches);
                    }
                    else
                    {
                        X509ThumbprintKeyIdentifierClause identifierClause3 = keyIdentifierClause as X509ThumbprintKeyIdentifierClause;
                        if (identifierClause3 != null)
                        {
                            certMatcher = new JwtSecurityTokenHandler.CertMatcher(identifierClause3.Matches);
                        }
                        else
                        {
                            X509IssuerSerialKeyIdentifierClause identifierClause4 = keyIdentifierClause as X509IssuerSerialKeyIdentifierClause;
                            if (identifierClause4 != null)
                                certMatcher = new JwtSecurityTokenHandler.CertMatcher(identifierClause4.Matches);
                        }
                    }
                }
                if (validationParameters.IssuerSigningTokens != null)
                {
                    foreach (SecurityToken issuerSigningToken in validationParameters.IssuerSigningTokens)
                    {
                        if (issuerSigningToken.MatchesKeyIdentifierClause(keyIdentifierClause))
                            return issuerSigningToken.SecurityKeys[0];
                    }
                }
            }
            return (SecurityKey) null;
        }

        private static bool Matches(
            SecurityKeyIdentifierClause keyIdentifierClause,
            SecurityKey key,
            JwtSecurityTokenHandler.CertMatcher certMatcher,
            out SecurityToken token)
        {
            token = (SecurityToken) null;
            if (certMatcher != null)
            {
                X509SecurityKey x509SecurityKey = key as X509SecurityKey;
                if (x509SecurityKey != null)
                {
                    if (certMatcher(x509SecurityKey.Certificate))
                    {
                        token = (SecurityToken) new X509SecurityToken(x509SecurityKey.Certificate);
                        return true;
                    }
                }
                else
                {
                    X509AsymmetricSecurityKey asymmetricSecurityKey = key as X509AsymmetricSecurityKey;
                    if (asymmetricSecurityKey != null)
                    {
                        X509Certificate2 x509Certificate2 = JwtSecurityTokenHandler._certFieldInfo.GetValue((object) asymmetricSecurityKey) as X509Certificate2;
                        if (x509Certificate2 != null && certMatcher(x509Certificate2))
                        {
                            token = (SecurityToken) new X509SecurityToken(x509Certificate2);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Validates the <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.SigningKey" /> is an expected value.
        /// </summary>
        /// <param name="securityKey">The <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that signed the <see cref="T:System.IdentityModel.Tokens.SecurityToken" />.</param>
        /// <param name="securityToken">The <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" /> to validate.</param>
        /// <param name="validationParameters">the current <see cref="T:System.IdentityModel.Tokens.TokenValidationParameters" />.</param>
        /// <remarks>If the <see cref="P:System.IdentityModel.Tokens.JwtSecurityToken.SigningKey" /> is a <see cref="T:System.IdentityModel.Tokens.X509SecurityKey" /> then the X509Certificate2 will be validated using <see cref="P:System.IdentityModel.Tokens.TokenValidationParameters.CertificateValidator" />.</remarks>
        protected virtual void ValidateIssuerSecurityKey(
            SecurityKey securityKey,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            Validators.ValidateIssuerSecurityKey(securityKey, securityToken, validationParameters);
        }

        private delegate bool CertMatcher(X509Certificate2 cert);
    }
}