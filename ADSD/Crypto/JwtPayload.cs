using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;

namespace ADSD.Crypto
{
    /// <summary>
    /// Initializes a new instance of <see cref="T:System.IdentityModel.Tokens.JwtPayload" /> which contains JSON objects representing the claims contained in the JWT. Each claim is a JSON object of the form { Name, Value }.
    /// </summary>
    public class JwtPayload : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.JwtPayload" /> class with no claims. Default string comparer <see cref="P:System.StringComparer.Ordinal" />.
        /// Creates a empty <see cref="T:System.IdentityModel.Tokens.JwtPayload" />
        /// </summary>
        public JwtPayload()
            : this((string) null, (string) null, (IEnumerable<Claim>) null, new DateTime?(), new DateTime?())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.JwtPayload" /> class with <see cref="T:System.Collections.Generic.IEnumerable`1" />. Default string comparer <see cref="P:System.StringComparer.Ordinal" />.
        /// <param name="claims">the claims to add.</param>
        /// </summary>
        public JwtPayload(IEnumerable<Claim> claims)
            : this((string) null, (string) null, claims, new DateTime?(), new DateTime?())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.JwtPayload" /> class with claims added for each parameter specified. Default string comparer <see cref="P:System.StringComparer.Ordinal" />.
        /// </summary>
        /// <param name="issuer">if this value is not null, a { iss, 'issuer' } claim will be added.</param>
        /// <param name="audience">if this value is not null, a { aud, 'audience' } claim will be added</param>
        /// <param name="claims">if this value is not null then for each <see cref="T:System.Security.Claims.Claim" /> a { 'Claim.Type', 'Claim.Value' } is added. If duplicate claims are found then a { 'Claim.Type', List&lt;object&gt; } will be created to contain the duplicate values.</param>
        /// <param name="notBefore">if notbefore.HasValue is 'true' a { nbf, 'value' } claim is added.</param>
        /// <param name="expires">if expires.HasValue is 'true' a { exp, 'value' } claim is added.</param>
        /// <remarks>Comparison is set to <see cref="P:System.StringComparer.Ordinal" />
        /// <para>The 4 parameters: 'issuer', 'audience', 'notBefore', 'expires' take precednece over <see cref="T:System.Security.Claims.Claim" />(s) in 'claims'. The values in 'claims' will be overridden.</para></remarks>
        /// <exception cref="T:System.ArgumentException">if 'expires' &lt;= 'notbefore'.</exception>
        public JwtPayload(
            string issuer,
            string audience,
            IEnumerable<Claim> claims,
            DateTime? notBefore,
            DateTime? expires)
            : base((IEqualityComparer<string>) StringComparer.Ordinal)
        {
            if (expires.HasValue && notBefore.HasValue)
            {
                DateTime? nullable1 = notBefore;
                DateTime? nullable2 = expires;
                if ((nullable1.HasValue & nullable2.HasValue ? (nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                    throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10401: Expires: '{0}' must be after NotBefore: '{1}'.", (object) expires.Value, (object) notBefore.Value));
            }
            if (claims != null)
                this.AddClaims(claims);
            if (!string.IsNullOrWhiteSpace(issuer))
                this["iss"] = (object) issuer;
            if (!string.IsNullOrWhiteSpace(audience))
                this["aud"] = (object) audience;
            if (expires.HasValue)
                this["exp"] = (object) EpochTime.GetIntDate(expires.Value.ToUniversalTime());
            if (!notBefore.HasValue)
                return;
            this["nbf"] = (object) EpochTime.GetIntDate(notBefore.Value.ToUniversalTime());
        }

        /// <summary>
        /// Gets the 'value' of the 'actor' claim { actort, 'value' }.
        /// </summary>
        /// <remarks>If the 'actor' claim is not found, null is returned.</remarks>
        public string Actort
        {
            get
            {
                return this.GetStandardClaim("actort");
            }
        }

        /// <summary>Gets the 'value' of the 'acr' claim { acr, 'value' }.</summary>
        /// <remarks>If the 'acr' claim is not found, null is returned.</remarks>
        public string Acr
        {
            get
            {
                return this.GetStandardClaim("acr");
            }
        }

        /// <summary>Gets the 'value' of the 'amr' claim { amr, 'value' }.</summary>
        /// <remarks>If the 'amr' claim is not found, null is returned.</remarks>
        public string Amr
        {
            get
            {
                return this.GetStandardClaim("amr");
            }
        }

        /// <summary>
        /// Gets the 'value' of the 'auth_time' claim { auth_time, 'value' }.
        /// </summary>
        /// <remarks>If the 'auth_time' claim is not found, null is returned.</remarks>
        public string AuthTime
        {
            get
            {
                return this.GetStandardClaim("auth_time");
            }
        }

        /// <summary>
        /// Gets the 'value' of the 'audience' claim { aud, 'value' } as a list of strings.
        /// </summary>
        /// <remarks>If the 'audience' claim is not found, an empty enumerable is returned.</remarks>
        public IList<string> Aud
        {
            get
            {
                List<string> stringList = new List<string>();
                object obj1 = (object) null;
                if (!this.TryGetValue("aud", out obj1))
                    return (IList<string>) stringList;
                string str1 = obj1 as string;
                if (str1 != null)
                {
                    stringList.Add(str1);
                    return (IList<string>) stringList;
                }
                IEnumerable<object> objects = obj1 as IEnumerable<object>;
                if (objects != null)
                {
                    foreach (object obj2 in objects)
                    {
                        string str2 = obj2 as string;
                        if (str2 != null)
                            stringList.Add(str2);
                        else
                            stringList.Add(JsonExtensions.SerializeToJson(obj2));
                    }
                }
                else
                    stringList.Add(JsonExtensions.SerializeToJson(obj1));
                return (IList<string>) stringList;
            }
        }

        /// <summary>Gets the 'value' of the 'azp' claim { azp, 'value' }.</summary>
        /// <remarks>If the 'azp' claim is not found, null is returned.</remarks>
        public string Azp
        {
            get
            {
                return this.GetStandardClaim("azp");
            }
        }

        /// <summary>
        /// Gets 'value' of the 'c_hash' claim { c_hash, 'value' }.
        /// </summary>
        /// <remarks>If the 'c_hash' claim is not found, null is returned.</remarks>
        public string CHash
        {
            get
            {
                return this.GetStandardClaim("c_hash");
            }
        }

        /// <summary>
        /// Gets the 'value' of the 'expiration' claim { exp, 'value' }.
        /// </summary>
        /// <remarks>If the 'expiration' claim is not found OR could not be converted to <see cref="T:System.Int32" />, null is returned.</remarks>
        public int? Exp
        {
            get
            {
                return this.GetIntClaim("exp");
            }
        }

        /// <summary>
        /// Gets the 'value' of the 'JWT ID' claim { jti, 'value' }.
        /// </summary>
        /// <remarks>If the 'JWT ID' claim is not found, null is returned.</remarks>
        public string Jti
        {
            get
            {
                return this.GetStandardClaim("jti");
            }
        }

        /// <summary>
        /// Gets the 'value' of the 'Issued At' claim { iat, 'value' }.
        /// </summary>
        /// <remarks>If the 'Issued At' claim is not found OR cannot be converted to <see cref="T:System.Int32" /> null is returned.</remarks>
        public int? Iat
        {
            get
            {
                return this.GetIntClaim("iat");
            }
        }

        /// <summary>Gets 'value' of the 'issuer' claim { iss, 'value' }.</summary>
        /// <remarks>If the 'issuer' claim is not found, null is returned.</remarks>
        public string Iss
        {
            get
            {
                return this.GetStandardClaim("iss");
            }
        }

        /// <summary>
        /// Gets the 'value' of the 'expiration' claim { nbf, 'value' }.
        /// </summary>
        /// <remarks>If the 'notbefore' claim is not found OR could not be converted to <see cref="T:System.Int32" />, null is returned.</remarks>
        public int? Nbf
        {
            get
            {
                return this.GetIntClaim("nbf");
            }
        }

        /// <summary>Gets 'value' of the 'nonce' claim { nonce, 'value' }.</summary>
        /// <remarks>If the 'nonce' claim is not found, null is returned.</remarks>
        public string Nonce
        {
            get
            {
                return this.GetStandardClaim("nonce");
            }
        }

        /// <summary>Gets "value" of the 'subject' claim { sub, 'value' }.</summary>
        /// <remarks>If the 'subject' claim is not found, null is returned.</remarks>
        public string Sub
        {
            get
            {
                return this.GetStandardClaim("sub");
            }
        }

        /// <summary>
        /// Gets 'value' of the 'notbefore' claim { nbf, 'value' } converted to a <see cref="T:System.DateTime" /> assuming 'value' is seconds since UnixEpoch (UTC 1970-01-01T0:0:0Z).
        /// </summary>
        /// <remarks>If the 'notbefore' claim is not found, then <see cref="F:System.DateTime.MinValue" /> is returned.</remarks>
        internal DateTime ValidFrom
        {
            get
            {
                return this.GetDateTime("nbf");
            }
        }

        /// <summary>
        /// Gets 'value' of the 'expiration' claim { exp, 'value' } converted to a <see cref="T:System.DateTime" /> assuming 'value' is seconds since UnixEpoch (UTC 1970-01-01T0:0:0Z).
        /// </summary>
        /// <remarks>If the 'expiration' claim is not found, then <see cref="F:System.DateTime.MinValue" /> is returned.</remarks>
        internal DateTime ValidTo
        {
            get
            {
                return this.GetDateTime("exp");
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Collections.Generic.IEnumerable`1" /><see cref="T:System.Security.Claims.Claim" /> for each JSON { name, value }.
        /// </summary>
        /// <remarks>Each <see cref="T:System.Security.Claims.Claim" />(s) returned will have the <see cref="P:System.Security.Claims.Claim.Type" /> translated according to the mapping found in <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.InboundClaimTypeMap" />. Adding and removing to <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.InboundClaimTypeMap" /> will affect the value of the <see cref="P:System.Security.Claims.Claim.Type" />.
        /// <para><see cref="P:System.Security.Claims.Claim.Issuer" /> and <see cref="P:System.Security.Claims.Claim.OriginalIssuer" /> will be set to the value of <see cref="P:System.IdentityModel.Tokens.JwtPayload.Iss" /> ( <see cref="F:System.String.Empty" /> if null).</para></remarks>
        public virtual IEnumerable<Claim> Claims
        {
            get
            {
                List<Claim> claimList = new List<Claim>();
                string str1 = this.Iss ?? "LOCAL AUTHORITY";
                foreach (KeyValuePair<string, object> keyValuePair1 in (Dictionary<string, object>) this)
                {
                    string key = keyValuePair1.Key;
                    string str2 = keyValuePair1.Value as string;
                    if (str2 != null)
                    {
                        claimList.Add(new Claim(keyValuePair1.Key, str2, "http://www.w3.org/2001/XMLSchema#string", str1, str1));
                    }
                    else
                    {
                        IEnumerable<object> objects = keyValuePair1.Value as IEnumerable<object>;
                        if (objects != null)
                        {
                            foreach (object obj in objects)
                            {
                                string str3 = obj as string;
                                if (str3 != null)
                                    claimList.Add(new Claim(keyValuePair1.Key, str3, "http://www.w3.org/2001/XMLSchema#string", str1, str1));
                                else
                                    claimList.Add(new Claim(keyValuePair1.Key, JsonExtensions.SerializeToJson(obj), "JSON", str1, str1)
                                    {
                                        Properties = {
                                            [JwtSecurityTokenHandler.JsonClaimTypeProperty] = obj.GetType().ToString()
                                        }
                                    });
                            }
                        }
                        else
                        {
                            IDictionary<string, object> dictionary = keyValuePair1.Value as IDictionary<string, object>;
                            if (dictionary != null)
                            {
                                foreach (KeyValuePair<string, object> keyValuePair2 in (IEnumerable<KeyValuePair<string, object>>) dictionary)
                                    claimList.Add(new Claim(key, "{" + JsonExtensions.SerializeToJson((object) keyValuePair2.Key) + ":" + JsonExtensions.SerializeToJson(keyValuePair2.Value) + "}", "JSON", str1, str1)
                                    {
                                        Properties = {
                                            [JwtSecurityTokenHandler.JsonClaimTypeProperty] = typeof (IDictionary<string, object>).ToString()
                                        }
                                    });
                            }
                            else
                                claimList.Add(new Claim(key, JsonExtensions.SerializeToJson(keyValuePair1.Value), "JSON", str1, str1)
                                {
                                    Properties = {
                                        [JwtSecurityTokenHandler.JsonClaimTypeProperty] = keyValuePair1.Value.GetType().ToString()
                                    }
                                });
                        }
                    }
                }
                return (IEnumerable<Claim>) claimList;
            }
        }

        /// <summary>
        /// Adds a JSON object representing the <see cref="T:System.Security.Claims.Claim" /> to the <see cref="T:System.IdentityModel.Tokens.JwtPayload" />
        /// </summary>
        /// <param name="claim">{ 'Claim.Type', 'Claim.Value' } is added. If a JSON object is found with the name == <see cref="P:System.Security.Claims.Claim.Type" /> then a { 'Claim.Type', List&lt;object&gt; } will be created to contain the duplicate values.</param>
        /// <remarks>See <see cref="M:System.IdentityModel.Tokens.JwtPayload.AddClaims(System.Collections.Generic.IEnumerable{System.Security.Claims.Claim})" /> for details on how <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.OutboundClaimTypeMap" /> is applied.</remarks>
        /// <exception cref="T:System.ArgumentNullException">'claim' is null.</exception>
        public void AddClaim(Claim claim)
        {
            if (claim == null)
                throw new ArgumentNullException(nameof (claim));
            this.AddClaims((IEnumerable<Claim>) new Claim[1]
            {
                claim
            });
        }

        /// <summary>
        /// Adds a number of <see cref="T:System.Security.Claims.Claim" /> to the <see cref="T:System.IdentityModel.Tokens.JwtPayload" /> as JSON { name, value } pairs.
        /// </summary>
        /// <param name="claims">for each <see cref="T:System.Security.Claims.Claim" /> a JSON pair { 'Claim.Type', 'Claim.Value' } is added. If duplicate claims are found then a { 'Claim.Type', List&lt;object&gt; } will be created to contain the duplicate values.</param>
        /// <remarks><para>Each <see cref="T:System.Security.Claims.Claim" /> added will have <see cref="P:System.Security.Claims.Claim.Type" /> translated according to the mapping found in <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.OutboundClaimTypeMap" />. Adding and removing to <see cref="P:System.IdentityModel.Tokens.JwtSecurityTokenHandler.OutboundClaimTypeMap" />
        /// will affect the name component of the Json claim</para>
        /// <para>Any <see cref="T:System.Security.Claims.Claim" /> in the <see cref="T:System.Collections.Generic.IEnumerable`1" /> that is null, will be ignored.</para></remarks>
        /// <exception cref="T:System.ArgumentNullException">'claims' is null.</exception>
        public void AddClaims(IEnumerable<Claim> claims)
        {
            if (claims == null)
                throw new ArgumentNullException(nameof (claims));
            foreach (Claim claim in claims)
            {
                if (claim != null)
                {
                    string key = (string) null;
                    if (!JwtSecurityTokenHandler.OutboundClaimTypeMap.TryGetValue(claim.Type, out key))
                        key = claim.Type;
                    object obj1 = claim.ValueType.Equals("http://www.w3.org/2001/XMLSchema#string", StringComparison.Ordinal) ? (object) claim.Value : JwtPayload.GetClaimValueUsingValueType(claim);
                    object obj2;
                    if (this.TryGetValue(key, out obj2))
                    {
                        IList<object> objectList = obj2 as IList<object>;
                        if (objectList == null)
                        {
                            objectList = (IList<object>) new List<object>();
                            objectList.Add(obj2);
                            this[key] = (object) objectList;
                        }
                        objectList.Add(obj1);
                    }
                    else
                        this[key] = obj1;
                }
            }
        }

        internal static object GetClaimValueUsingValueType(Claim claim)
        {
            int result1;
            if (claim.ValueType == "http://www.w3.org/2001/XMLSchema#integer" && int.TryParse(claim.Value, out result1))
                return (object) result1;
            int result2;
            if (claim.ValueType == "http://www.w3.org/2001/XMLSchema#integer32" && int.TryParse(claim.Value, out result2))
                return (object) result2;
            long result3;
            if (claim.ValueType == "http://www.w3.org/2001/XMLSchema#integer64" && long.TryParse(claim.Value, out result3))
                return (object) result3;
            bool result4;
            if (claim.ValueType == "http://www.w3.org/2001/XMLSchema#boolean" && bool.TryParse(claim.Value, out result4))
                return (object) result4;
            double result5;
            if (claim.ValueType == "http://www.w3.org/2001/XMLSchema#double" && double.TryParse(claim.Value, out result5))
                return (object) result5;
            return (object) claim.Value;
        }

        internal string GetStandardClaim(string claimType)
        {
            object obj = (object) null;
            if (this.TryGetValue(claimType, out obj))
                return obj as string ?? JsonExtensions.SerializeToJson(obj);
            return (string) null;
        }

        internal int? GetIntClaim(string claimType)
        {
            int? nullable = new int?();
            object obj1;
            if (!this.TryGetValue(claimType, out obj1))
                return nullable;
            IList<object> objectList = obj1 as IList<object>;
            if (objectList != null)
            {
                foreach (object obj2 in (IEnumerable<object>) objectList)
                {
                    nullable = new int?();
                    if (obj2 != null)
                    {
                        try
                        {
                            nullable = new int?(Convert.ToInt32(obj2, (IFormatProvider) CultureInfo.InvariantCulture));
                        }
                        catch (FormatException )
                        {
                            nullable = new int?();
                        }
                        catch (InvalidCastException )
                        {
                            nullable = new int?();
                        }
                        catch (OverflowException )
                        {
                            nullable = new int?();
                        }
                        if (nullable.HasValue)
                            return nullable;
                    }
                }
            }
            else
            {
                try
                {
                    nullable = new int?(Convert.ToInt32(obj1, (IFormatProvider) CultureInfo.InvariantCulture));
                }
                catch (FormatException )
                {
                    nullable = new int?();
                }
                catch (OverflowException )
                {
                    nullable = new int?();
                }
            }
            return nullable;
        }

        /// <summary>
        /// Gets the DateTime using the number of seconds from 1970-01-01T0:0:0Z (UTC)
        /// </summary>
        /// <param name="key">Claim in the payload that should map to an integer.</param>
        /// <remarks>If the claim is not found, the function returns: DateTime.MinValue</remarks>
        /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenException">if an overflow exception is thrown by the runtime.</exception>
        /// <returns>the DateTime representation of a claim.</returns>
        private DateTime GetDateTime(string key)
        {
            object obj;
            if (!this.TryGetValue(key, out obj))
                return DateTime.MinValue;
            try
            {
                IList<object> objectList = obj as IList<object>;
                if (objectList != null)
                {
                    if (objectList.Count == 0)
                        return DateTime.MinValue;
                    obj = objectList[0];
                }
                return EpochTime.DateTime(Convert.ToInt64(obj, (IFormatProvider) CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is ArgumentException || ex is InvalidCastException)
                    throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10700: Error found while parsing date time. The '{0}' claim has value '{1}' which is could not be parsed to an integer.\nInnerException: '{2}'.", (object) key, obj ?? (object) "<null>", (object) ex));
                if (ex is OverflowException)
                    throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10701: Error found while parsing date time. The '{0}' claim has value '{1}' does not lie in the valid range. \nInnerException: '{2}'.", (object) key, obj ?? (object) "<null>", (object) ex));
                throw;
            }
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
        /// Deserializes Base64UrlEncoded JSON into a <see cref="T:System.IdentityModel.Tokens.JwtPayload" /> instance.
        /// </summary>
        /// <param name="base64UrlEncodedJsonString">base64url encoded JSON to deserialize.</param>
        /// <returns>an instance of <see cref="T:System.IdentityModel.Tokens.JwtPayload" />.</returns>
        /// <remarks>use <see cref="P:System.IdentityModel.Tokens.JsonExtensions.Deserializer" /> to customize JSON serialization.</remarks>
        public static JwtPayload Base64UrlDeserialize(string base64UrlEncodedJsonString)
        {
            return JsonExtensions.DeserializeJwtPayload(Base64UrlEncoder.Decode(base64UrlEncodedJsonString));
        }

        /// <summary>
        /// Deserialzes JSON into a <see cref="T:System.IdentityModel.Tokens.JwtPayload" /> instance.
        /// </summary>
        /// <param name="jsonString">the JSON to deserialize.</param>
        /// <returns>an instance of <see cref="T:System.IdentityModel.Tokens.JwtPayload" />.</returns>
        /// <remarks>use <see cref="P:System.IdentityModel.Tokens.JsonExtensions.Deserializer" /> to customize JSON serialization.</remarks>
        public static JwtPayload Deserialize(string jsonString)
        {
            return JsonExtensions.DeserializeJwtPayload(jsonString);
        }
    }
}