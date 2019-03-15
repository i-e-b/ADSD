using System;
using System.Xml;

namespace ADSD
{
    internal class TrustFeb2005Dictionary : TrustDictionary
    {
        public TrustFeb2005Dictionary(IdentityModelDictionary dictionary)
            : base(dictionary)
        {
            this.RequestSecurityTokenResponseCollection = dictionary.CreateString("RequestSecurityTokenResponseCollection", 193);
            this.Namespace = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust", 194);
            this.BinarySecretClauseType = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust#BinarySecret", 195);
            this.CombinedHashLabel = dictionary.CreateString("AUTH-HASH", 196);
            this.RequestSecurityTokenResponse = dictionary.CreateString("RequestSecurityTokenResponse", 197);
            this.TokenType = dictionary.CreateString("TokenType", 147);
            this.KeySize = dictionary.CreateString("KeySize", 198);
            this.RequestedTokenReference = dictionary.CreateString("RequestedTokenReference", 199);
            this.AppliesTo = dictionary.CreateString("AppliesTo", 200);
            this.Authenticator = dictionary.CreateString("Authenticator", 201);
            this.CombinedHash = dictionary.CreateString("CombinedHash", 202);
            this.BinaryExchange = dictionary.CreateString("BinaryExchange", 203);
            this.Lifetime = dictionary.CreateString("Lifetime", 204);
            this.RequestedSecurityToken = dictionary.CreateString("RequestedSecurityToken", 205);
            this.Entropy = dictionary.CreateString("Entropy", 206);
            this.RequestedProofToken = dictionary.CreateString("RequestedProofToken", 207);
            this.ComputedKey = dictionary.CreateString("ComputedKey", 208);
            this.RequestSecurityToken = dictionary.CreateString("RequestSecurityToken", 209);
            this.RequestType = dictionary.CreateString("RequestType", 210);
            this.Context = dictionary.CreateString("Context", 211);
            this.BinarySecret = dictionary.CreateString("BinarySecret", 212);
            this.Type = dictionary.CreateString("Type", 83);
            this.SpnegoValueTypeUri = dictionary.CreateString("http://schemas.microsoft.com/net/2004/07/secext/WS-SPNego", 213);
            this.TlsnegoValueTypeUri = dictionary.CreateString("http://schemas.microsoft.com/net/2004/07/secext/TLSNego", 214);
            this.Prefix = dictionary.CreateString("t", 215);
            this.RequestSecurityTokenIssuance = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue", 216);
            this.RequestSecurityTokenIssuanceResponse = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue", 217);
            this.RequestTypeIssue = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/Issue", 218);
            this.SymmetricKeyBinarySecret = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey", 219);
            this.Psha1ComputedKeyUri = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/CK/PSHA1", 220);
            this.NonceBinarySecret = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/Nonce", 221);
            this.RenewTarget = dictionary.CreateString("RenewTarget", 222);
            this.CloseTarget = dictionary.CreateString("CancelTarget", 223);
            this.RequestedTokenClosed = dictionary.CreateString("RequestedTokenCancelled", 224);
            this.RequestedAttachedReference = dictionary.CreateString("RequestedAttachedReference", 225);
            this.RequestedUnattachedReference = dictionary.CreateString("RequestedUnattachedReference", 226);
            this.IssuedTokensHeader = dictionary.CreateString("IssuedTokens", 227);
            this.RequestTypeRenew = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/Renew", 228);
            this.RequestTypeClose = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/Cancel", 229);
            this.KeyType = dictionary.CreateString("KeyType", 230);
            this.SymmetricKeyType = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey", 219);
            this.PublicKeyType = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/PublicKey", 231);
            this.Claims = dictionary.CreateString("Claims", 232);
            this.InvalidRequestFaultCode = dictionary.CreateString("InvalidRequest", 233);
            this.FailedAuthenticationFaultCode = dictionary.CreateString("FailedAuthentication", 136);
            this.UseKey = dictionary.CreateString("UseKey", 234);
            this.SignWith = dictionary.CreateString("SignWith", 235);
            this.EncryptWith = dictionary.CreateString("EncryptWith", 236);
            this.EncryptionAlgorithm = dictionary.CreateString("EncryptionAlgorithm", 237);
            this.CanonicalizationAlgorithm = dictionary.CreateString("CanonicalizationAlgorithm", 238);
            this.ComputedKeyAlgorithm = dictionary.CreateString("ComputedKeyAlgorithm", 239);
        }

        public TrustFeb2005Dictionary(IXmlDictionary dictionary)
            : base(dictionary)
        {
            this.RequestSecurityTokenResponseCollection = this.LookupDictionaryString(dictionary, "RequestSecurityTokenResponseCollection");
            this.Namespace = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust");
            this.BinarySecretClauseType = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust#BinarySecret");
            this.CombinedHashLabel = this.LookupDictionaryString(dictionary, "AUTH-HASH");
            this.RequestSecurityTokenResponse = this.LookupDictionaryString(dictionary, "RequestSecurityTokenResponse");
            this.TokenType = this.LookupDictionaryString(dictionary, "TokenType");
            this.KeySize = this.LookupDictionaryString(dictionary, "KeySize");
            this.RequestedTokenReference = this.LookupDictionaryString(dictionary, "RequestedTokenReference");
            this.AppliesTo = this.LookupDictionaryString(dictionary, "AppliesTo");
            this.Authenticator = this.LookupDictionaryString(dictionary, "Authenticator");
            this.CombinedHash = this.LookupDictionaryString(dictionary, "CombinedHash");
            this.BinaryExchange = this.LookupDictionaryString(dictionary, "BinaryExchange");
            this.Lifetime = this.LookupDictionaryString(dictionary, "Lifetime");
            this.RequestedSecurityToken = this.LookupDictionaryString(dictionary, "RequestedSecurityToken");
            this.Entropy = this.LookupDictionaryString(dictionary, "Entropy");
            this.RequestedProofToken = this.LookupDictionaryString(dictionary, "RequestedProofToken");
            this.ComputedKey = this.LookupDictionaryString(dictionary, "ComputedKey");
            this.RequestSecurityToken = this.LookupDictionaryString(dictionary, "RequestSecurityToken");
            this.RequestType = this.LookupDictionaryString(dictionary, "RequestType");
            this.Context = this.LookupDictionaryString(dictionary, "Context");
            this.BinarySecret = this.LookupDictionaryString(dictionary, "BinarySecret");
            this.Type = this.LookupDictionaryString(dictionary, "Type");
            this.SpnegoValueTypeUri = this.LookupDictionaryString(dictionary, "http://schemas.microsoft.com/net/2004/07/secext/WS-SPNego");
            this.TlsnegoValueTypeUri = this.LookupDictionaryString(dictionary, "http://schemas.microsoft.com/net/2004/07/secext/TLSNego");
            this.Prefix = this.LookupDictionaryString(dictionary, "t");
            this.RequestSecurityTokenIssuance = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue");
            this.RequestSecurityTokenIssuanceResponse = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue");
            this.RequestTypeIssue = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/Issue");
            this.SymmetricKeyBinarySecret = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey");
            this.Psha1ComputedKeyUri = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/CK/PSHA1");
            this.NonceBinarySecret = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/Nonce");
            this.RenewTarget = this.LookupDictionaryString(dictionary, "RenewTarget");
            this.CloseTarget = this.LookupDictionaryString(dictionary, "CancelTarget");
            this.RequestedTokenClosed = this.LookupDictionaryString(dictionary, "RequestedTokenCancelled");
            this.RequestedAttachedReference = this.LookupDictionaryString(dictionary, "RequestedAttachedReference");
            this.RequestedUnattachedReference = this.LookupDictionaryString(dictionary, "RequestedUnattachedReference");
            this.IssuedTokensHeader = this.LookupDictionaryString(dictionary, "IssuedTokens");
            this.RequestTypeRenew = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/Renew");
            this.RequestTypeClose = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/Cancel");
            this.KeyType = this.LookupDictionaryString(dictionary, "KeyType");
            this.SymmetricKeyType = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey");
            this.PublicKeyType = this.LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/PublicKey");
            this.Claims = this.LookupDictionaryString(dictionary, "Claims");
            this.InvalidRequestFaultCode = this.LookupDictionaryString(dictionary, "InvalidRequest");
            this.FailedAuthenticationFaultCode = this.LookupDictionaryString(dictionary, "FailedAuthentication");
            this.UseKey = this.LookupDictionaryString(dictionary, "UseKey");
            this.SignWith = this.LookupDictionaryString(dictionary, "SignWith");
            this.EncryptWith = this.LookupDictionaryString(dictionary, "EncryptWith");
            this.EncryptionAlgorithm = this.LookupDictionaryString(dictionary, "EncryptionAlgorithm");
            this.CanonicalizationAlgorithm = this.LookupDictionaryString(dictionary, "CanonicalizationAlgorithm");
            this.ComputedKeyAlgorithm = this.LookupDictionaryString(dictionary, "ComputedKeyAlgorithm");
        }

        private XmlDictionaryString LookupDictionaryString(
            IXmlDictionary dictionary,
            string value)
        {
            XmlDictionaryString result;
            if (!dictionary.TryLookup(value, out result))
                throw new ArgumentException("CannotFindValueInDictionaryString");
            return result;
        }
    }
}