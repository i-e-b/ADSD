using System;
using System.Xml;

namespace ADSD.Crypto
{
    internal class TrustDictionary
    {
        public XmlDictionaryString RequestSecurityTokenResponseCollection = null;
        public XmlDictionaryString Namespace = null;
        public XmlDictionaryString BinarySecretClauseType = null;
        public XmlDictionaryString CombinedHashLabel = null;
        public XmlDictionaryString RequestSecurityTokenResponse = null;
        public XmlDictionaryString TokenType = null;
        public XmlDictionaryString KeySize = null;
        public XmlDictionaryString RequestedTokenReference = null;
        public XmlDictionaryString AppliesTo = null;
        public XmlDictionaryString Authenticator = null;
        public XmlDictionaryString CombinedHash = null;
        public XmlDictionaryString BinaryExchange = null;
        public XmlDictionaryString Lifetime = null;
        public XmlDictionaryString RequestedSecurityToken = null;
        public XmlDictionaryString Entropy = null;
        public XmlDictionaryString RequestedProofToken = null;
        public XmlDictionaryString ComputedKey = null;
        public XmlDictionaryString RequestSecurityToken = null;
        public XmlDictionaryString RequestType = null;
        public XmlDictionaryString Context = null;
        public XmlDictionaryString BinarySecret = null;
        public XmlDictionaryString Type = null;
        public XmlDictionaryString SpnegoValueTypeUri = null;
        public XmlDictionaryString TlsnegoValueTypeUri = null;
        public XmlDictionaryString Prefix = null;
        public XmlDictionaryString RequestSecurityTokenIssuance = null;
        public XmlDictionaryString RequestSecurityTokenIssuanceResponse = null;
        public XmlDictionaryString RequestTypeIssue = null;
        public XmlDictionaryString SymmetricKeyBinarySecret = null;
        public XmlDictionaryString Psha1ComputedKeyUri = null;
        public XmlDictionaryString NonceBinarySecret = null;
        public XmlDictionaryString RenewTarget = null;
        public XmlDictionaryString CloseTarget = null;
        public XmlDictionaryString RequestedTokenClosed = null;
        public XmlDictionaryString RequestedAttachedReference = null;
        public XmlDictionaryString RequestedUnattachedReference = null;
        public XmlDictionaryString IssuedTokensHeader = null;
        public XmlDictionaryString RequestTypeRenew = null;
        public XmlDictionaryString RequestTypeClose = null;
        public XmlDictionaryString KeyType = null;
        public XmlDictionaryString SymmetricKeyType = null;
        public XmlDictionaryString PublicKeyType = null;
        public XmlDictionaryString Claims = null;
        public XmlDictionaryString InvalidRequestFaultCode = null;
        public XmlDictionaryString FailedAuthenticationFaultCode = null;
        public XmlDictionaryString UseKey = null;
        public XmlDictionaryString SignWith = null;
        public XmlDictionaryString EncryptWith = null;
        public XmlDictionaryString EncryptionAlgorithm = null;
        public XmlDictionaryString CanonicalizationAlgorithm = null;
        public XmlDictionaryString ComputedKeyAlgorithm = null;
        public XmlDictionaryString AsymmetricKeyBinarySecret = null;
        public XmlDictionaryString RequestSecurityTokenCollectionIssuanceFinalResponse = null;
        public XmlDictionaryString RequestSecurityTokenRenewal = null;
        public XmlDictionaryString RequestSecurityTokenRenewalResponse = null;
        public XmlDictionaryString RequestSecurityTokenCollectionRenewalFinalResponse = null;
        public XmlDictionaryString RequestSecurityTokenCancellation = null;
        public XmlDictionaryString RequestSecurityTokenCancellationResponse = null;
        public XmlDictionaryString RequestSecurityTokenCollectionCancellationFinalResponse = null;
        public XmlDictionaryString KeyWrapAlgorithm = null;
        public XmlDictionaryString BearerKeyType = null;
        public XmlDictionaryString SecondaryParameters = null;
        public XmlDictionaryString Dialect = null;
        public XmlDictionaryString DialectType = null;

        public TrustDictionary()
        {
        }

        public TrustDictionary(IXmlDictionary dictionary)
        {
        }

        private XmlDictionaryString LookupDictionaryString(
            IXmlDictionary dictionary,
            string value)
        {
            XmlDictionaryString result;
            if (!dictionary.TryLookup(value, out result)) throw new ArgumentException("XDCannotFindValueInDictionaryString");
            return result;
        }
    }
}