using System;
using System.Collections.Generic;
using System.Globalization;

namespace ADSD
{
    /// <summary>
    /// Initializes a new instance of <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> which contains JSON objects representing the cryptographic operations applied to the JWT and optionally any additional properties of the JWT.
    /// The member names within the JWT Header are referred to as Header Parameter Names.
    /// <para>These names MUST be unique and the values must be <see cref="T:System.String" />(s). The corresponding values are referred to as Header Parameter Values.</para>
    /// </summary>
    public class JwtHeader : Dictionary<string, object>
    {
        private SigningCredentials signingCredentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> class. Default string comparer <see cref="P:System.StringComparer.Ordinal" />.
        /// </summary>
        public JwtHeader()
            : base((IEqualityComparer<string>) StringComparer.Ordinal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> class. With the Header Parameters as follows:
        /// <para>{ { typ, JWT }, { alg, Mapped( <see cref="P:System.IdentityModel.Tokens.SigningCredentials.SignatureAlgorithm" /> } }
        /// See: Algorithm Mapping below.</para>
        /// </summary>
        /// <param name="signingCredentials">The <see cref="P:System.IdentityModel.Tokens.JwtHeader.SigningCredentials" /> that will be or were used to sign the <see cref="T:System.IdentityModel.Tokens.JwtSecurityToken" />.</param>
        /// <remarks>
        /// <para>For each <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> in signingCredentials.SigningKeyIdentifier</para>
        /// <para>if the clause  is a <see cref="T:System.IdentityModel.Tokens.NamedKeySecurityKeyIdentifierClause" /> Header Parameter { clause.Name, clause.Id } will be added.</para>
        /// <para>For example, if clause.Name == 'kid' and clause.Id == 'SecretKey99'. The JSON object { kid, SecretKey99 } would be added.</para>
        /// <para>In addition, if the <see cref="P:System.IdentityModel.Tokens.JwtHeader.SigningCredentials" /> is a <see cref="T:System.IdentityModel.Tokens.X509SigningCredentials" /> the JSON object { x5t, Base64UrlEncoded( <see cref="M:System.Security.Cryptography.X509Certificates.X509Certificate.GetCertHashString" /> } will be added.</para>
        /// <para>This simplifies the common case where a X509Certificate is used.</para>
        /// <para>================= </para>
        /// <para>Algorithm Mapping</para>
        /// <para>================= </para>
        /// <para><see cref="P:System.IdentityModel.Tokens.SigningCredentials.SignatureAlgorithm" /> describes the algorithm that is discoverable by the CLR runtime.</para>
        /// <para>The  { alg, 'value' } placed in the header reflects the JWT specification.</para>
        /// <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.OutboundAlgorithmMap" /> contains a signature mapping where the 'value' above will be translated according to this mapping.
        /// <para>Current mapping is:</para>
        /// <para>    'http://www.w3.org/2001/04/xmldsig-more#rsa-sha256' =&gt; 'RS256'</para>
        /// <para>    'http://www.w3.org/2001/04/xmldsig-more#hmac-sha256' =&gt; 'HS256'</para>
        /// </remarks>
        public JwtHeader(SigningCredentials signingCredentials = null)
            : base((IEqualityComparer<string>) StringComparer.Ordinal)
        {
            this["typ"] = (object) "JWT";
            if (signingCredentials != null)
            {
                this.signingCredentials = signingCredentials;
                string index = signingCredentials.SignatureAlgorithm;
                if (JwtSecurityTokenHandler.OutboundAlgorithmMap.ContainsKey(signingCredentials.SignatureAlgorithm))
                    index = JwtSecurityTokenHandler.OutboundAlgorithmMap[index];
                this["alg"] = (object) index;
                if (signingCredentials.SigningKeyIdentifier != null)
                {
                    foreach (SecurityKeyIdentifierClause identifierClause1 in signingCredentials.SigningKeyIdentifier)
                    {
                        NamedKeySecurityKeyIdentifierClause identifierClause2 = identifierClause1 as NamedKeySecurityKeyIdentifierClause;
                        if (identifierClause2 != null)
                            this[identifierClause2.Name] = (object) identifierClause2.Id;
                    }
                }
                X509SigningCredentials signingCredentials1 = signingCredentials as X509SigningCredentials;
                if (signingCredentials1 == null || signingCredentials1.Certificate == null)
                    return;
                this["x5t"] = (object) Base64UrlEncoder.Encode(signingCredentials1.Certificate.GetCertHash());
            }
            else
                this["alg"] = (object) "none";
        }

        /// <summary>
        /// Gets the signature algorithm that was used to create the signature.
        /// </summary>
        /// <remarks>If the signature algorithm is not found, null is returned.</remarks>
        public string Alg
        {
            get
            {
                return this.GetStandardClaim("alg");
            }
        }

        /// <summary>
        /// Gets the <see cref="P:System.IdentityModel.Tokens.JwtHeader.SigningCredentials" /> passed in the constructor.
        /// </summary>
        /// <remarks>This value may be null.</remarks>
        public SigningCredentials SigningCredentials
        {
            get
            {
                return this.signingCredentials;
            }
        }

        /// <summary>Gets the mime type (Typ) of the token.</summary>
        /// <remarks>If the mime type is not found, null is returned.</remarks>
        public string Typ
        {
            get
            {
                return this.GetStandardClaim("typ");
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that contains a <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> for each key found.
        /// </summary>
        /// <remarks>
        /// Keys are identified by matching a 'Reserved Header Parameter Name' found in the in JSON Web Signature specification.
        /// <para>Names recognized are: jku, jkw, kid, x5c, x5t, x5u</para>
        /// <para>'x5t' adds a <see cref="T:System.IdentityModel.Tokens.X509ThumbprintKeyIdentifierClause" /> passing a the Base64UrlDecoded( Value ) to the constructor.</para>
        /// <para>'jku', 'jkw', 'kid', 'x5u', 'x5c' each add a <see cref="T:System.IdentityModel.Tokens.NamedKeySecurityKeyIdentifierClause" /> with the { Name, Value } passed to the <see cref="M:System.IdentityModel.Tokens.NamedKeySecurityKeyIdentifierClause.#ctor(System.String,System.String)" />.</para>
        /// <para>   </para>
        /// <para>If no keys are found, an empty <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> will be returned.</para>
        /// </remarks>
        public virtual SecurityKeyIdentifier SigningKeyIdentifier
        {
            get
            {
                SecurityKeyIdentifier securityKeyIdentifier = new SecurityKeyIdentifier();
                if (this.ContainsKey("x5t"))
                {
                    try
                    {
                        securityKeyIdentifier.Add((SecurityKeyIdentifierClause) new X509ThumbprintKeyIdentifierClause(Base64UrlEncoder.DecodeBytes(this.GetStandardClaim("x5t"))));
                    }
                    catch (Exception ex)
                    {
                        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10705: The SigningKeyIdentifier was of type: '{0}' and was expected to be encoded as a Base64UrlEncoded string. See inner exception for more details.", (object) "x5t"), ex);
                    }
                }
                if (this.ContainsKey("jku"))
                    securityKeyIdentifier.Add((SecurityKeyIdentifierClause) new NamedKeySecurityKeyIdentifierClause("jku", this.GetStandardClaim("jku")));
                if (this.ContainsKey("jwk"))
                    securityKeyIdentifier.Add((SecurityKeyIdentifierClause) new NamedKeySecurityKeyIdentifierClause("jwk", this.GetStandardClaim("jwk")));
                if (this.ContainsKey("x5u"))
                    securityKeyIdentifier.Add((SecurityKeyIdentifierClause) new NamedKeySecurityKeyIdentifierClause("x5u", this.GetStandardClaim("x5u")));
                if (this.ContainsKey("x5c"))
                    securityKeyIdentifier.Add((SecurityKeyIdentifierClause) new NamedKeySecurityKeyIdentifierClause("x5c", this.GetStandardClaim("x5c")));
                if (this.ContainsKey("kid"))
                    securityKeyIdentifier.Add((SecurityKeyIdentifierClause) new NamedKeySecurityKeyIdentifierClause("kid", this.GetStandardClaim("kid")));
                return securityKeyIdentifier;
            }
        }

        internal string GetStandardClaim(string claimType)
        {
            object obj = (object) null;
            if (this.TryGetValue(claimType, out obj))
                return obj as string ?? JsonExtensions.SerializeToJson(obj);
            return (string) null;
        }

        /// <summary>Serializes this instance to JSON.</summary>
        /// <returns>this instance as JSON.</returns>
        /// <remarks>use <see cref="P:System.IdentityModel.Tokens.JsonExtensions.Serializer" /> to customize JSON serialization.</remarks>
        public virtual string SerializeToJson()
        {
            return JsonExtensions.SerializeToJson((object) this);
        }

        /// <summary>Encodes this instance as Base64UrlEncoded JSON.</summary>
        /// <returns>Base64UrlEncoded JSON.</returns>
        /// <remarks>use <see cref="P:System.IdentityModel.Tokens.JsonExtensions.Serializer" /> to customize JSON serialization.</remarks>
        public virtual string Base64UrlEncode()
        {
            return Base64UrlEncoder.Encode(this.SerializeToJson());
        }

        /// <summary>
        /// Deserializes Base64UrlEncoded JSON into a <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> instance.
        /// </summary>
        /// <param name="base64UrlEncodedJsonString">base64url encoded JSON to deserialize.</param>
        /// <returns>an instance of <see cref="T:System.IdentityModel.Tokens.JwtHeader" />.</returns>
        /// <remarks>use <see cref="P:System.IdentityModel.Tokens.JsonExtensions.Deserializer" /> to customize JSON serialization.</remarks>
        public static JwtHeader Base64UrlDeserialize(string base64UrlEncodedJsonString)
        {
            return JsonExtensions.DeserializeJwtHeader(Base64UrlEncoder.Decode(base64UrlEncodedJsonString));
        }

        /// <summary>
        /// Deserialzes JSON into a <see cref="T:System.IdentityModel.Tokens.JwtHeader" /> instance.
        /// </summary>
        /// <param name="jsonString"> the JSON to deserialize.</param>
        /// <returns>an instance of <see cref="T:System.IdentityModel.Tokens.JwtHeader" />.</returns>
        /// <remarks>use <see cref="P:System.IdentityModel.Tokens.JsonExtensions.Deserializer" /> to customize JSON serialization.</remarks>
        public static JwtHeader Deserialize(string jsonString)
        {
            return JsonExtensions.DeserializeJwtHeader(jsonString);
        }
    }
}