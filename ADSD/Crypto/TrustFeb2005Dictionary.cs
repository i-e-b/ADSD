using System;
using System.Xml;

namespace ADSD.Crypto
{
    internal class TrustFeb2005Dictionary : TrustDictionary
    {
        public TrustFeb2005Dictionary(IdentityModelDictionary dictionary)
            : base(dictionary)
        {
            RequestSecurityTokenResponseCollection = dictionary.CreateString("RequestSecurityTokenResponseCollection", 193);
            Namespace = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust", 194);
            BinarySecretClauseType = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust#BinarySecret", 195);
            CombinedHashLabel = dictionary.CreateString("AUTH-HASH", 196);
            RequestSecurityTokenResponse = dictionary.CreateString("RequestSecurityTokenResponse", 197);
            TokenType = dictionary.CreateString("TokenType", 147);
            KeySize = dictionary.CreateString("KeySize", 198);
            RequestedTokenReference = dictionary.CreateString("RequestedTokenReference", 199);
            AppliesTo = dictionary.CreateString("AppliesTo", 200);
            Authenticator = dictionary.CreateString("Authenticator", 201);
            CombinedHash = dictionary.CreateString("CombinedHash", 202);
            BinaryExchange = dictionary.CreateString("BinaryExchange", 203);
            Lifetime = dictionary.CreateString("Lifetime", 204);
            RequestedSecurityToken = dictionary.CreateString("RequestedSecurityToken", 205);
            Entropy = dictionary.CreateString("Entropy", 206);
            RequestedProofToken = dictionary.CreateString("RequestedProofToken", 207);
            ComputedKey = dictionary.CreateString("ComputedKey", 208);
            RequestSecurityToken = dictionary.CreateString("RequestSecurityToken", 209);
            RequestType = dictionary.CreateString("RequestType", 210);
            Context = dictionary.CreateString("Context", 211);
            BinarySecret = dictionary.CreateString("BinarySecret", 212);
            Type = dictionary.CreateString("Type", 83);
            SpnegoValueTypeUri = dictionary.CreateString("http://schemas.microsoft.com/net/2004/07/secext/WS-SPNego", 213);
            TlsnegoValueTypeUri = dictionary.CreateString("http://schemas.microsoft.com/net/2004/07/secext/TLSNego", 214);
            Prefix = dictionary.CreateString("t", 215);
            RequestSecurityTokenIssuance = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue", 216);
            RequestSecurityTokenIssuanceResponse = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue", 217);
            RequestTypeIssue = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/Issue", 218);
            SymmetricKeyBinarySecret = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey", 219);
            Psha1ComputedKeyUri = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/CK/PSHA1", 220);
            NonceBinarySecret = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/Nonce", 221);
            RenewTarget = dictionary.CreateString("RenewTarget", 222);
            CloseTarget = dictionary.CreateString("CancelTarget", 223);
            RequestedTokenClosed = dictionary.CreateString("RequestedTokenCancelled", 224);
            RequestedAttachedReference = dictionary.CreateString("RequestedAttachedReference", 225);
            RequestedUnattachedReference = dictionary.CreateString("RequestedUnattachedReference", 226);
            IssuedTokensHeader = dictionary.CreateString("IssuedTokens", 227);
            RequestTypeRenew = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/Renew", 228);
            RequestTypeClose = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/Cancel", 229);
            KeyType = dictionary.CreateString("KeyType", 230);
            SymmetricKeyType = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey", 219);
            PublicKeyType = dictionary.CreateString("http://schemas.xmlsoap.org/ws/2005/02/trust/PublicKey", 231);
            Claims = dictionary.CreateString("Claims", 232);
            InvalidRequestFaultCode = dictionary.CreateString("InvalidRequest", 233);
            FailedAuthenticationFaultCode = dictionary.CreateString("FailedAuthentication", 136);
            UseKey = dictionary.CreateString("UseKey", 234);
            SignWith = dictionary.CreateString("SignWith", 235);
            EncryptWith = dictionary.CreateString("EncryptWith", 236);
            EncryptionAlgorithm = dictionary.CreateString("EncryptionAlgorithm", 237);
            CanonicalizationAlgorithm = dictionary.CreateString("CanonicalizationAlgorithm", 238);
            ComputedKeyAlgorithm = dictionary.CreateString("ComputedKeyAlgorithm", 239);
        }

        public TrustFeb2005Dictionary(IXmlDictionary dictionary)
            : base(dictionary)
        {
            RequestSecurityTokenResponseCollection = LookupDictionaryString(dictionary, "RequestSecurityTokenResponseCollection");
            Namespace = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust");
            BinarySecretClauseType = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust#BinarySecret");
            CombinedHashLabel = LookupDictionaryString(dictionary, "AUTH-HASH");
            RequestSecurityTokenResponse = LookupDictionaryString(dictionary, "RequestSecurityTokenResponse");
            TokenType = LookupDictionaryString(dictionary, "TokenType");
            KeySize = LookupDictionaryString(dictionary, "KeySize");
            RequestedTokenReference = LookupDictionaryString(dictionary, "RequestedTokenReference");
            AppliesTo = LookupDictionaryString(dictionary, "AppliesTo");
            Authenticator = LookupDictionaryString(dictionary, "Authenticator");
            CombinedHash = LookupDictionaryString(dictionary, "CombinedHash");
            BinaryExchange = LookupDictionaryString(dictionary, "BinaryExchange");
            Lifetime = LookupDictionaryString(dictionary, "Lifetime");
            RequestedSecurityToken = LookupDictionaryString(dictionary, "RequestedSecurityToken");
            Entropy = LookupDictionaryString(dictionary, "Entropy");
            RequestedProofToken = LookupDictionaryString(dictionary, "RequestedProofToken");
            ComputedKey = LookupDictionaryString(dictionary, "ComputedKey");
            RequestSecurityToken = LookupDictionaryString(dictionary, "RequestSecurityToken");
            RequestType = LookupDictionaryString(dictionary, "RequestType");
            Context = LookupDictionaryString(dictionary, "Context");
            BinarySecret = LookupDictionaryString(dictionary, "BinarySecret");
            Type = LookupDictionaryString(dictionary, "Type");
            SpnegoValueTypeUri = LookupDictionaryString(dictionary, "http://schemas.microsoft.com/net/2004/07/secext/WS-SPNego");
            TlsnegoValueTypeUri = LookupDictionaryString(dictionary, "http://schemas.microsoft.com/net/2004/07/secext/TLSNego");
            Prefix = LookupDictionaryString(dictionary, "t");
            RequestSecurityTokenIssuance = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue");
            RequestSecurityTokenIssuanceResponse = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue");
            RequestTypeIssue = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/Issue");
            SymmetricKeyBinarySecret = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey");
            Psha1ComputedKeyUri = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/CK/PSHA1");
            NonceBinarySecret = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/Nonce");
            RenewTarget = LookupDictionaryString(dictionary, "RenewTarget");
            CloseTarget = LookupDictionaryString(dictionary, "CancelTarget");
            RequestedTokenClosed = LookupDictionaryString(dictionary, "RequestedTokenCancelled");
            RequestedAttachedReference = LookupDictionaryString(dictionary, "RequestedAttachedReference");
            RequestedUnattachedReference = LookupDictionaryString(dictionary, "RequestedUnattachedReference");
            IssuedTokensHeader = LookupDictionaryString(dictionary, "IssuedTokens");
            RequestTypeRenew = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/Renew");
            RequestTypeClose = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/Cancel");
            KeyType = LookupDictionaryString(dictionary, "KeyType");
            SymmetricKeyType = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey");
            PublicKeyType = LookupDictionaryString(dictionary, "http://schemas.xmlsoap.org/ws/2005/02/trust/PublicKey");
            Claims = LookupDictionaryString(dictionary, "Claims");
            InvalidRequestFaultCode = LookupDictionaryString(dictionary, "InvalidRequest");
            FailedAuthenticationFaultCode = LookupDictionaryString(dictionary, "FailedAuthentication");
            UseKey = LookupDictionaryString(dictionary, "UseKey");
            SignWith = LookupDictionaryString(dictionary, "SignWith");
            EncryptWith = LookupDictionaryString(dictionary, "EncryptWith");
            EncryptionAlgorithm = LookupDictionaryString(dictionary, "EncryptionAlgorithm");
            CanonicalizationAlgorithm = LookupDictionaryString(dictionary, "CanonicalizationAlgorithm");
            ComputedKeyAlgorithm = LookupDictionaryString(dictionary, "ComputedKeyAlgorithm");
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