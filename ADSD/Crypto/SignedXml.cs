using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>Provides a wrapper on a core XML signature object to facilitate creating XML signatures.</summary>
    public class SignedXml
    {
        private static IList<string> s_knownCanonicalizationMethods = (IList<string>) null;
        private static IList<string> s_defaultSafeTransformMethods = (IList<string>) null;
        internal static readonly string XmlDsigDigestDefault = GlobalSettings.UseInsecureHashAlgorithmsForXml ? "http://www.w3.org/2000/09/xmldsig#sha1" : "http://www.w3.org/2001/04/xmlenc#sha256";
        internal static readonly string XmlDsigRSADefault = GlobalSettings.UseInsecureHashAlgorithmsForXml ? "http://www.w3.org/2000/09/xmldsig#rsa-sha1" : "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        private Func<SignedXml, bool> m_signatureFormatValidator = new Func<SignedXml, bool>(SignedXml.DefaultSignatureFormatValidator);
        /// <summary>Represents the <see cref="T:System.Security.Cryptography.Xml.Signature" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object. </summary>
        protected Signature m_signature;
        /// <summary>Represents the name of the installed key to be used for signing the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object. </summary>
        protected string m_strSigningKeyName;
        private AsymmetricAlgorithm m_signingKey;
        private XmlDocument m_containingDocument;
        private IEnumerator m_keyInfoEnum;
        private X509Certificate2Collection m_x509Collection;
        private IEnumerator m_x509Enum;
        private bool[] m_refProcessed;
        private int[] m_refLevelCache;
        internal XmlResolver m_xmlResolver;
        internal XmlElement m_context;
        private bool m_bResolverSet;
        private Collection<string> m_safeCanonicalizationMethods;
        private EncryptedXml m_exml;
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard namespace for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigNamespaceUrl = "http://www.w3.org/2000/09/xmldsig#";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard minimal canonicalization algorithm for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigMinimalCanonicalizationUrl = "http://www.w3.org/2000/09/xmldsig#minimal";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard canonicalization algorithm for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigCanonicalizationUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard canonicalization algorithm for XML digital signatures and includes comments. This field is constant.</summary>
        public const string XmlDsigCanonicalizationWithCommentsUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.SHA1" /> digest method for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigSHA1Url = "http://www.w3.org/2000/09/xmldsig#sha1";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.DSA" /> algorithm for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.RSA" /> signature method for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.HMACSHA1" /> algorithm for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigHMACSHA1Url = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.SHA256" /> digest method for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the  <see cref="T:System.Security.Cryptography.RSA" /> SHA-256 signature method variation for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigRSASHA256Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.SHA384" /> digest method for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#sha384";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the  <see cref="T:System.Security.Cryptography.RSA" /> SHA-384 signature method variation for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigRSASHA384Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.SHA512" /> digest method for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the  <see cref="T:System.Security.Cryptography.RSA" /> SHA-512 signature method variation for XML digital signatures. This field is constant.</summary>
        public const string XmlDsigRSASHA512Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the Canonical XML transformation. This field is constant.</summary>
        public const string XmlDsigC14NTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the Canonical XML transformation, with comments. This field is constant.</summary>
        public const string XmlDsigC14NWithCommentsTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";
        /// <summary>Represents the Uniform Resource Identifier (URI) for exclusive XML canonicalization. This field is constant.</summary>
        public const string XmlDsigExcC14NTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#";
        /// <summary>Represents the Uniform Resource Identifier (URI) for exclusive XML canonicalization, with comments. This field is constant.</summary>
        public const string XmlDsigExcC14NWithCommentsTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the base 64 transformation. This field is constant.</summary>
        public const string XmlDsigBase64TransformUrl = "http://www.w3.org/2000/09/xmldsig#base64";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the XML Path Language (XPath). This field is constant.</summary>
        public const string XmlDsigXPathTransformUrl = "http://www.w3.org/TR/1999/REC-xpath-19991116";
        /// <summary>Represents the Uniform Resource Identifier (URI) for XSLT transformations. This field is constant.</summary>
        public const string XmlDsigXsltTransformUrl = "http://www.w3.org/TR/1999/REC-xslt-19991116";
        /// <summary>Represents the Uniform Resource Identifier (URI) for enveloped signature transformation. This field is constant.</summary>
        public const string XmlDsigEnvelopedSignatureTransformUrl = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the XML mode decryption transformation. This field is constant.</summary>
        public const string XmlDecryptionTransformUrl = "http://www.w3.org/2002/07/decrypt#XML";
        /// <summary>Represents the Uniform Resource Identifier (URI) for the license transform algorithm used to normalize XrML licenses for signatures.</summary>
        public const string XmlLicenseTransformUrl = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";
        private bool bCacheValid;
        private byte[] _digestedSignedInfo;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> class.</summary>
        public SignedXml()
        {
            this.Initialize((XmlElement) null);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> class from the specified XML document.</summary>
        /// <param name="document">The <see cref="T:System.Xml.XmlDocument" /> object to use to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.SignedXml" />. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="document" /> parameter is <see langword="null" />.-or-The <paramref name="document" /> parameter contains a null <see cref="P:System.Xml.XmlDocument.DocumentElement" /> property.</exception>
        public SignedXml(XmlDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof (document));
            this.Initialize(document.DocumentElement);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> class from the specified <see cref="T:System.Xml.XmlElement" /> object.</summary>
        /// <param name="elem">The <see cref="T:System.Xml.XmlElement" /> object to use to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.SignedXml" />. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="elem" /> parameter is <see langword="null" />. </exception>
        public SignedXml(XmlElement elem)
        {
            if (elem == null)
                throw new ArgumentNullException(nameof (elem));
            this.Initialize(elem);
        }

        private void Initialize(XmlElement element)
        {
            this.m_containingDocument = element == null ? (XmlDocument) null : element.OwnerDocument;
            this.m_context = element;
            this.m_signature = new Signature();
            this.m_signature.SignedXml = this;
            this.m_signature.SignedInfo = new SignedInfo();
            this.m_signingKey = (AsymmetricAlgorithm) null;
            this.m_safeCanonicalizationMethods = new Collection<string>(SignedXml.KnownCanonicalizationMethods);
        }

        /// <summary>Gets or sets the name of the installed key to be used for signing the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
        /// <returns>The name of the installed key to be used for signing the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
        public string SigningKeyName
        {
            get
            {
                return this.m_strSigningKeyName;
            }
            set
            {
                this.m_strSigningKeyName = value;
            }
        }

        /// <summary>Sets the current <see cref="T:System.Xml.XmlResolver" /> object.</summary>
        /// <returns>The current <see cref="T:System.Xml.XmlResolver" /> object. The defaults is a <see cref="T:System.Xml.XmlSecureResolver" /> object.</returns>
        [ComVisible(false)]
        public XmlResolver Resolver
        {
            set
            {
                this.m_xmlResolver = value;
                this.m_bResolverSet = true;
            }
        }

        internal bool ResolverSet
        {
            get
            {
                return this.m_bResolverSet;
            }
        }

        /// <summary>Gets a delegate that will be called to validate the format (not the cryptographic security) of an XML signature.</summary>
        /// <returns>
        /// <see langword="true" /> if the format is acceptable; otherwise, <see langword="false" />.</returns>
        public Func<SignedXml, bool> SignatureFormatValidator
        {
            get
            {
                return this.m_signatureFormatValidator;
            }
            set
            {
                this.m_signatureFormatValidator = value;
            }
        }

        /// <summary>[Supported in the .NET Framework 4.5.1 and later versions] Gets the names of methods whose canonicalization algorithms are explicitly allowed. </summary>
        /// <returns>A collection of the names of methods that safely produce canonical XML. </returns>
        public Collection<string> SafeCanonicalizationMethods
        {
            get
            {
                return this.m_safeCanonicalizationMethods;
            }
        }

        /// <summary>Gets or sets the asymmetric algorithm key used for signing a <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
        /// <returns>The asymmetric algorithm key used for signing the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
        public AsymmetricAlgorithm SigningKey
        {
            get
            {
                return this.m_signingKey;
            }
            set
            {
                this.m_signingKey = value;
            }
        }

        /// <summary>Gets or sets an <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> object that defines the XML encryption processing rules.</summary>
        /// <returns>An <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> object that defines the XML encryption processing rules.</returns>
        [ComVisible(false)]
        public EncryptedXml EncryptedXml
        {
            get
            {
                if (this.m_exml == null)
                    this.m_exml = new EncryptedXml(this.m_containingDocument);
                return this.m_exml;
            }
            set
            {
                this.m_exml = value;
            }
        }

        /// <summary>Gets the <see cref="T:System.Security.Cryptography.Xml.Signature" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
        /// <returns>The <see cref="T:System.Security.Cryptography.Xml.Signature" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
        public Signature Signature
        {
            get
            {
                return this.m_signature;
            }
        }

        /// <summary>Gets the <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
        /// <returns>The <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
        public SignedInfo SignedInfo
        {
            get
            {
                return this.m_signature.SignedInfo;
            }
        }

        /// <summary>Gets the signature method of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
        /// <returns>The signature method of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
        public string SignatureMethod
        {
            get
            {
                return this.m_signature.SignedInfo.SignatureMethod;
            }
        }

        /// <summary>Gets the length of the signature for the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
        /// <returns>The length of the signature for the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
        public string SignatureLength
        {
            get
            {
                return this.m_signature.SignedInfo.SignatureLength;
            }
        }

        /// <summary>Gets the signature value of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
        /// <returns>A byte array that contains the signature value of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
        public byte[] SignatureValue
        {
            get
            {
                return this.m_signature.SignatureValue;
            }
        }

        /// <summary>Gets or sets the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
        /// <returns>The <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
        public KeyInfo KeyInfo
        {
            get
            {
                return this.m_signature.KeyInfo;
            }
            set
            {
                this.m_signature.KeyInfo = value;
            }
        }

        /// <summary>Returns the XML representation of a <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
        /// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.Xml.Signature" /> object.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignedInfo" /> property is <see langword="null" />.-or- The <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignatureValue" /> property is <see langword="null" />. </exception>
        public XmlElement GetXml()
        {
            if (this.m_containingDocument != null)
                return this.m_signature.GetXml(this.m_containingDocument);
            return this.m_signature.GetXml();
        }

        /// <summary>Loads a <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> state from an XML element.</summary>
        /// <param name="value">The XML element to load the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> state from. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="value" /> parameter does not contain a valid <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignatureValue" /> property.-or- The <paramref name="value" /> parameter does not contain a valid <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignedInfo" /> property.</exception>
        public void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            this.m_signature.LoadXml(value);
            if (this.m_context == null)
                this.m_context = value;
            this.bCacheValid = false;
        }

        /// <summary>Adds a <see cref="T:System.Security.Cryptography.Xml.Reference" /> object to the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object that describes a digest method, digest value, and transform to use for creating an XML digital signature.</summary>
        /// <param name="reference">The  <see cref="T:System.Security.Cryptography.Xml.Reference" /> object that describes a digest method, digest value, and transform to use for creating an XML digital signature.</param>
        public void AddReference(Reference reference)
        {
            this.m_signature.SignedInfo.AddReference(reference);
        }

        /// <summary>Adds a <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object to the list of objects to be signed.</summary>
        /// <param name="dataObject">The <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object to add to the list of objects to be signed. </param>
        public void AddObject(DataObject dataObject)
        {
            this.m_signature.AddObject(dataObject);
        }

        /// <summary>Determines whether the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies using the public key in the signature.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.AsymmetricAlgorithm.SignatureAlgorithm" /> property of the public key in the signature does not match the <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignatureMethod" /> property.-or- The signature description could not be created.-or The hash algorithm could not be created. </exception>
        public bool CheckSignature()
        {
            AsymmetricAlgorithm signingKey;
            return this.CheckSignatureReturningKey(out signingKey);
        }

        /// <summary>Determines whether the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies using the public key in the signature.</summary>
        /// <param name="signingKey">When this method returns, contains the implementation of <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> that holds the public key in the signature. This parameter is passed uninitialized. </param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies using the public key in the signature; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="signingKey" /> parameter is null.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.AsymmetricAlgorithm.SignatureAlgorithm" /> property of the public key in the signature does not match the <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignatureMethod" /> property.-or- The signature description could not be created.-or The hash algorithm could not be created. </exception>
        public bool CheckSignatureReturningKey(out AsymmetricAlgorithm signingKey)
        {
            //SignedXmlDebugLog.LogBeginSignatureVerification(this, this.m_context);
            signingKey = (AsymmetricAlgorithm) null;
            bool verified = false;
            if (!this.CheckSignatureFormat())
                return false;
            AsymmetricAlgorithm publicKey;
            do
            {
                publicKey = this.GetPublicKey();
                if (publicKey != null)
                {
                    verified = this.CheckSignature(publicKey);
                    //SignedXmlDebugLog.LogVerificationResult(this, (object) publicKey, verified);
                }
            }
            while (publicKey != null && !verified);
            signingKey = publicKey;
            return verified;
        }

        /// <summary>Determines whether the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies for the specified key.</summary>
        /// <param name="key">The implementation of the <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> property that holds the key to be used to verify the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property. </param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies for the specified key; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="key" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.AsymmetricAlgorithm.SignatureAlgorithm" /> property of the <paramref name="key" /> parameter does not match the <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignatureMethod" /> property.-or- The signature description could not be created.-or The hash algorithm could not be created. </exception>
        public bool CheckSignature(AsymmetricAlgorithm key)
        {
            if (!this.CheckSignatureFormat())
                return false;
            if (!this.CheckSignedInfo(key))
            {
                //SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_SignedInfo"));
                return false;
            }
            if (!this.CheckDigestedReferences())
            {
                //SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_References"));
                return false;
            }
            //SignedXmlDebugLog.LogVerificationResult(this, (object) key, true);
            return true;
        }

        /// <summary>Determines whether the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies for the specified message authentication code (MAC) algorithm.</summary>
        /// <param name="macAlg">The implementation of <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> that holds the MAC to be used to verify the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property. </param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies for the specified MAC; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="macAlg" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.HashAlgorithm.HashSize" /> property of the specified <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> object is not valid.-or- The <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property is <see langword="null" />.-or- The cryptographic transform used to check the signature could not be created. </exception>
        public bool CheckSignature(KeyedHashAlgorithm macAlg)
        {
            if (!this.CheckSignatureFormat())
                return false;
            if (!this.CheckSignedInfo(macAlg))
            {
                //SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_SignedInfo"));
                return false;
            }
            if (!this.CheckDigestedReferences())
            {
                //SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_References"));
                return false;
            }
            //SignedXmlDebugLog.LogVerificationResult(this, (object) macAlg, true);
            return true;
        }

        /// <summary>Determines whether the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies for the specified <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> object and, optionally, whether the certificate is valid.</summary>
        /// <param name="certificate">The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> object to use to verify the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property.</param>
        /// <param name="verifySignatureOnly">
        /// <see langword="true" /> to verify the signature only; <see langword="false" /> to verify both the signature and certificate.</param>
        /// <returns>
        /// <see langword="true" /> if the signature is valid; otherwise, <see langword="false" />. -or-
        /// <see langword="true" /> if the signature and certificate are valid; otherwise, <see langword="false" />. </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="certificate" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">A signature description could not be created for the <paramref name="certificate" /> parameter.</exception>
        [ComVisible(false)]
        [SecuritySafeCritical]
        public bool CheckSignature(X509Certificate2 certificate, bool verifySignatureOnly)
        {
            if (!verifySignatureOnly)
            {
                foreach (X509Extension extension in certificate.Extensions)
                {
                    if (string.Compare(extension.Oid.Value, "2.5.29.15", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        X509KeyUsageExtension keyUsages = new X509KeyUsageExtension();
                        keyUsages.CopyFrom((AsnEncodedData) extension);
                        //SignedXmlDebugLog.LogVerifyKeyUsage(this, (X509Certificate) certificate, keyUsages);
                        if ((keyUsages.KeyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.None && (uint) (keyUsages.KeyUsages & X509KeyUsageFlags.NonRepudiation) <= 0U)
                        {
                            //SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_X509KeyUsage"));
                            return false;
                        }
                        break;
                    }
                }
                X509Chain chain = new X509Chain();
                chain.ChainPolicy.ExtraStore.AddRange(this.BuildBagOfCerts());
                bool flag = chain.Build(certificate);
                //SignedXmlDebugLog.LogVerifyX509Chain(this, chain, (X509Certificate) certificate);
                if (!flag)
                {
                    //SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_X509Chain"));
                    return false;
                }
            }
            if (!this.CheckSignature(GetAnyPublicKey(certificate)))
                return false;
            //SignedXmlDebugLog.LogVerificationResult(this, (object) certificate, true);
            return true;
        }
        
        internal static AsymmetricAlgorithm GetAnyPublicKey(X509Certificate2 c)
        {
            return c.PublicKey.Key;
            /*
            AsymmetricAlgorithm rsaPublicKey = (AsymmetricAlgorithm) X509CertificateExtensions.GetRSAPublicKey(c);
            if (rsaPublicKey != null)
                return rsaPublicKey;
            AsymmetricAlgorithm dsaPublicKey = (AsymmetricAlgorithm) X509CertificateExtensions.GetDSAPublicKey(c);
            if (dsaPublicKey != null)
                return dsaPublicKey;
            AsymmetricAlgorithm ecDsaPublicKey = X509CertificateExtensions.GetECDsaPublicKey(c);
            if (ecDsaPublicKey != null)
                return ecDsaPublicKey;
            throw new NotSupportedException(SecurityResources.GetResourceString("NotSupported_KeyAlgorithm"));*/
        }

        /// <summary>Computes an XML digital signature.</summary>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.SignedXml.SigningKey" /> property is <see langword="null" />.-or- The <see cref="P:System.Security.Cryptography.Xml.SignedXml.SigningKey" /> property is not a <see cref="T:System.Security.Cryptography.DSA" /> object or <see cref="T:System.Security.Cryptography.RSA" /> object.-or- The key could not be loaded. </exception>
        public void ComputeSignature()
        {
            //SignedXmlDebugLog.LogBeginSignatureComputation(this, this.m_context);
            this.BuildDigestedReferences();
            AsymmetricAlgorithm signingKey = this.SigningKey;
            if (signingKey == null)
                throw new CryptographicException("Cryptography_Xml_LoadKeyFailed");
            if (this.SignedInfo.SignatureMethod == null)
            {
                if (signingKey is DSA)
                {
                    this.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
                }
                else
                {
                    if (!(signingKey is RSA))
                        throw new CryptographicException("Cryptography_Xml_CreatedKeyFailed");
                    if (this.SignedInfo.SignatureMethod == null)
                        this.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSADefault;
                }
            }
            SignatureDescription fromName = Exml.CreateFromName<SignatureDescription>(this.SignedInfo.SignatureMethod);
            if (fromName == null)
                throw new CryptographicException("Cryptography_Xml_SignatureDescriptionNotCreated");
            HashAlgorithm digest = fromName.CreateDigest();
            if (digest == null)
                throw new CryptographicException("Cryptography_Xml_CreateHashAlgorithmFailed");
            this.GetC14NDigest(digest);
            AsymmetricSignatureFormatter formatter = fromName.CreateFormatter(signingKey);
            //SignedXmlDebugLog.LogSigning(this, (object) signingKey, fromName, digest, formatter);
            this.m_signature.SignatureValue = formatter.CreateSignature(digest);
        }

        /// <summary>Computes an XML digital signature using the specified message authentication code (MAC) algorithm.</summary>
        /// <param name="macAlg">A <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> object that holds the MAC to be used to compute the value of the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="macAlg" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> object specified by the <paramref name="macAlg" /> parameter is not an instance of <see cref="T:System.Security.Cryptography.HMACSHA1" />.-or- The <see cref="P:System.Security.Cryptography.HashAlgorithm.HashSize" /> property of the specified <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> object is not valid.-or- The cryptographic transform used to check the signature could not be created. </exception>
        public void ComputeSignature(KeyedHashAlgorithm macAlg)
        {
            if (macAlg == null)
                throw new ArgumentNullException(nameof (macAlg));
            HMAC hmac = macAlg as HMAC;
            if (hmac == null)
                throw new CryptographicException("Cryptography_Xml_SignatureMethodKeyMismatch");
            int num = this.m_signature.SignedInfo.SignatureLength != null ? Convert.ToInt32(this.m_signature.SignedInfo.SignatureLength, (IFormatProvider) null) : hmac.HashSize;
            if (num < 0 || num > hmac.HashSize)
                throw new CryptographicException("Cryptography_Xml_InvalidSignatureLength");
            if (num % 8 != 0)
                throw new CryptographicException("Cryptography_Xml_InvalidSignatureLength2");
            this.BuildDigestedReferences();
            string hashName = hmac.HashName;
            if (hashName != "SHA1")
            {
                if (hashName != "SHA256")
                {
                    if (hashName != "SHA384")
                    {
                        if (hashName != "SHA512")
                        {
                            if (hashName != "MD5")
                            {
                                if (hashName != "RIPEMD160")
                                    throw new CryptographicException("Cryptography_Xml_SignatureMethodKeyMismatch");
                                this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";
                            }
                            else
                                this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-md5";
                        }
                        else
                            this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
                    }
                    else
                        this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
                }
                else
                    this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
            }
            else
                this.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
            byte[] c14Ndigest = this.GetC14NDigest((HashAlgorithm) hmac);
            //SignedXmlDebugLog.LogSigning(this, (KeyedHashAlgorithm) hmac);
            this.m_signature.SignatureValue = new byte[num / 8];
            Buffer.BlockCopy((Array) c14Ndigest, 0, (Array) this.m_signature.SignatureValue, 0, num / 8);
        }

        /// <summary>Returns the public key of a signature.</summary>
        /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> object that contains the public key of the signature, or <see langword="null" /> if the key cannot be found.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.SignedXml.KeyInfo" /> property is <see langword="null" />.</exception>
        protected virtual AsymmetricAlgorithm GetPublicKey()
        {
            if (this.KeyInfo == null)
                throw new CryptographicException("Cryptography_Xml_KeyInfoRequired");
            if (this.m_x509Enum != null)
            {
                AsymmetricAlgorithm certificatePublicKey = this.GetNextCertificatePublicKey();
                if (certificatePublicKey != null)
                    return certificatePublicKey;
            }
            if (this.m_keyInfoEnum == null)
                this.m_keyInfoEnum = this.KeyInfo.GetEnumerator();
            while (this.m_keyInfoEnum.MoveNext())
            {
                RSAKeyValue current1 = this.m_keyInfoEnum.Current as RSAKeyValue;
                if (current1 != null)
                    return (AsymmetricAlgorithm) current1.Key;
                DSAKeyValue current2 = this.m_keyInfoEnum.Current as DSAKeyValue;
                if (current2 != null)
                    return (AsymmetricAlgorithm) current2.Key;
                KeyInfoX509Data current3 = this.m_keyInfoEnum.Current as KeyInfoX509Data;
                if (current3 != null)
                {
                    this.m_x509Collection = BuildBagOfCerts(current3, CertUsageType.Verification);
                    if (this.m_x509Collection.Count > 0)
                    {
                        this.m_x509Enum = (IEnumerator) this.m_x509Collection.GetEnumerator();
                        AsymmetricAlgorithm certificatePublicKey = this.GetNextCertificatePublicKey();
                        if (certificatePublicKey != null)
                            return certificatePublicKey;
                    }
                }
            }
            return (AsymmetricAlgorithm) null;
        }
        
    [SecuritySafeCritical]
    internal static X509Certificate2Collection BuildBagOfCerts(
      KeyInfoX509Data keyInfoX509Data,
      CertUsageType certUsageType)
    {
      X509Certificate2Collection certificate2Collection = new X509Certificate2Collection();
      ArrayList arrayList = certUsageType == CertUsageType.Decryption ? new ArrayList() : (ArrayList) null;
      if (keyInfoX509Data.Certificates != null)
      {
        foreach (X509Certificate2 certificate in keyInfoX509Data.Certificates)
        {
          switch (certUsageType)
          {
            case CertUsageType.Verification:
              certificate2Collection.Add(certificate);
              continue;
            case CertUsageType.Decryption:
              arrayList.Add((object) new X509IssuerSerial(certificate.IssuerName.Name, certificate.SerialNumber));
              continue;
            default:
              continue;
          }
        }
      }
      if (keyInfoX509Data.SubjectNames == null && keyInfoX509Data.IssuerSerials == null && (keyInfoX509Data.SubjectKeyIds == null && arrayList == null))
        return certificate2Collection;
      //new StorePermission(StorePermissionFlags.OpenStore).Assert();
      X509Store[] x509StoreArray = new X509Store[2];
      string storeName = certUsageType == CertUsageType.Verification ? "AddressBook" : "My";
      x509StoreArray[0] = new X509Store(storeName, StoreLocation.CurrentUser);
      x509StoreArray[1] = new X509Store(storeName, StoreLocation.LocalMachine);
      for (int index = 0; index < x509StoreArray.Length; ++index)
      {
        if (x509StoreArray[index] != null)
        {
          X509Certificate2Collection certificates = (X509Certificate2Collection) null;
          try
          {
            x509StoreArray[index].Open(OpenFlags.OpenExistingOnly);
            certificates = x509StoreArray[index].Certificates;
            x509StoreArray[index].Close();
            if (keyInfoX509Data.SubjectNames != null)
            {
              foreach (string subjectName in keyInfoX509Data.SubjectNames)
                certificates = certificates.Find(X509FindType.FindBySubjectDistinguishedName, (object) subjectName, false);
            }
            if (keyInfoX509Data.IssuerSerials != null)
            {
              foreach (X509IssuerSerial issuerSerial in keyInfoX509Data.IssuerSerials)
              {
                certificates = certificates.Find(X509FindType.FindByIssuerDistinguishedName, (object) issuerSerial.IssuerName, false);
                certificates = certificates.Find(X509FindType.FindBySerialNumber, (object) issuerSerial.SerialNumber, false);
              }
            }
            if (keyInfoX509Data.SubjectKeyIds != null)
            {
              foreach (byte[] subjectKeyId in keyInfoX509Data.SubjectKeyIds)
              {
                string str = X509Utils.EncodeHexString(subjectKeyId);
                certificates = certificates.Find(X509FindType.FindBySubjectKeyIdentifier, (object) str, false);
              }
            }
            if (arrayList != null)
            {
              foreach (X509IssuerSerial x509IssuerSerial in arrayList)
              {
                certificates = certificates.Find(X509FindType.FindByIssuerDistinguishedName, (object) x509IssuerSerial.IssuerName, false);
                certificates = certificates.Find(X509FindType.FindBySerialNumber, (object) x509IssuerSerial.SerialNumber, false);
              }
            }
          }
          catch (CryptographicException ex)
          {
                        Console.WriteLine(ex);
                    }
                    if (certificates != null)
            certificate2Collection.AddRange(certificates);
        }
      }
      return certificate2Collection;
    }

        private X509Certificate2Collection BuildBagOfCerts()
        {
            X509Certificate2Collection certificate2Collection = new X509Certificate2Collection();
            if (this.KeyInfo != null)
            {
                foreach (KeyInfoClause keyInfoClause in this.KeyInfo)
                {
                    KeyInfoX509Data keyInfoX509Data = keyInfoClause as KeyInfoX509Data;
                    if (keyInfoX509Data != null)
                        certificate2Collection.AddRange(BuildBagOfCerts(keyInfoX509Data, CertUsageType.Verification));
                }
            }
            return certificate2Collection;
        }

        private AsymmetricAlgorithm GetNextCertificatePublicKey()
        {
            while (this.m_x509Enum.MoveNext())
            {
                X509Certificate2 current = (X509Certificate2) this.m_x509Enum.Current;
                if (current != null)
                {
                    if (!GlobalSettings.UseLegacyCertificatePrivateKey) return GetAnyPublicKey(current);
                    return current.PublicKey.Key;
                }
            }
            return (AsymmetricAlgorithm) null;
        }

        /// <summary>Returns the <see cref="T:System.Xml.XmlElement" /> object with the specified ID from the specified <see cref="T:System.Xml.XmlDocument" /> object.</summary>
        /// <param name="document">The <see cref="T:System.Xml.XmlDocument" /> object to retrieve the <see cref="T:System.Xml.XmlElement" /> object from.</param>
        /// <param name="idValue">The ID of the <see cref="T:System.Xml.XmlElement" /> object to retrieve from the <see cref="T:System.Xml.XmlDocument" /> object.</param>
        /// <returns>The <see cref="T:System.Xml.XmlElement" /> object with the specified ID from the specified <see cref="T:System.Xml.XmlDocument" /> object, or <see langword="null" /> if it could not be found.</returns>
        public virtual XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            return SignedXml.DefaultGetIdElement(document, idValue);
        }

        internal static XmlElement DefaultGetIdElement(XmlDocument document, string idValue)
        {
            if (document == null) return (XmlElement) null;

            try
            {
                XmlConvert.VerifyNCName(idValue);
            }
            catch (XmlException)
            {
                return (XmlElement)null;
            }

            XmlElement elementById1 = document.GetElementById(idValue);
            if (elementById1 == null)
                return SignedXml.GetSingleReferenceTarget(document, "Id", idValue) ?? SignedXml.GetSingleReferenceTarget(document, "id", idValue) ?? SignedXml.GetSingleReferenceTarget(document, "ID", idValue);

            return elementById1;
        }

        private static bool DefaultSignatureFormatValidator(SignedXml signedXml)
        {
            return !signedXml.DoesSignatureUseTruncatedHmac() && signedXml.DoesSignatureUseSafeCanonicalizationMethod();
        }

        private bool DoesSignatureUseTruncatedHmac()
        {
            if (this.SignedInfo.SignatureLength == null)
                return false;
            HMAC fromName = Exml.CreateFromName<HMAC>(this.SignatureMethod);
            if (fromName == null)
                return false;
            int result = 0;
            if (!int.TryParse(this.SignedInfo.SignatureLength, out result))
                return true;
            return result != fromName.HashSize;
        }

        private bool DoesSignatureUseSafeCanonicalizationMethod()
        {
            foreach (string canonicalizationMethod in this.SafeCanonicalizationMethods)
            {
                if (string.Equals(canonicalizationMethod, this.SignedInfo.CanonicalizationMethod, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            //SignedXmlDebugLog.LogUnsafeCanonicalizationMethod(this, this.SignedInfo.CanonicalizationMethod, (IEnumerable<string>) this.SafeCanonicalizationMethods);
            return false;
        }

        private bool ReferenceUsesSafeTransformMethods(Reference reference)
        {
            TransformChain transformChain = reference.TransformChain;
            int count = transformChain.Count;
            for (int index = 0; index < count; ++index)
            {
                if (!this.IsSafeTransform(transformChain[index].Algorithm))
                    return false;
            }
            return true;
        }

        private bool IsSafeTransform(string transformAlgorithm)
        {
            foreach (string canonicalizationMethod in this.SafeCanonicalizationMethods)
            {
                if (string.Equals(canonicalizationMethod, transformAlgorithm, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            foreach (string safeTransformMethod in (IEnumerable<string>) SignedXml.DefaultSafeTransformMethods)
            {
                if (string.Equals(safeTransformMethod, transformAlgorithm, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            //SignedXmlDebugLog.LogUnsafeTransformMethod(this, transformAlgorithm, (IEnumerable<string>) this.SafeCanonicalizationMethods, (IEnumerable<string>) SignedXml.DefaultSafeTransformMethods);
            return false;
        }

        private static IList<string> KnownCanonicalizationMethods
        {
            get
            {
                if (SignedXml.s_knownCanonicalizationMethods == null)
                {
                    List<string> stringList = new List<string>();
                    stringList.Add("http://www.w3.org/TR/2001/REC-xml-c14n-20010315");
                    stringList.Add("http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments");
                    stringList.Add("http://www.w3.org/2001/10/xml-exc-c14n#");
                    stringList.Add("http://www.w3.org/2001/10/xml-exc-c14n#WithComments");
                    SignedXml.s_knownCanonicalizationMethods = (IList<string>) stringList;
                }
                return SignedXml.s_knownCanonicalizationMethods;
            }
        }

        private static IList<string> DefaultSafeTransformMethods
        {
            get
            {
                if (SignedXml.s_defaultSafeTransformMethods == null)
                {
                    List<string> stringList = new List<string>();
                    stringList.Add("http://www.w3.org/2000/09/xmldsig#enveloped-signature");
                    stringList.Add("http://www.w3.org/2000/09/xmldsig#base64");
                    stringList.Add("urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform");
                    stringList.Add("http://www.w3.org/2002/07/decrypt#XML");
                    SignedXml.s_defaultSafeTransformMethods = (IList<string>) stringList;
                }
                return SignedXml.s_defaultSafeTransformMethods;
            }
        }

        private byte[] GetC14NDigest(HashAlgorithm hash)
        {
            if (!this.bCacheValid || !this.SignedInfo.CacheValid)
            {
                string str = this.m_containingDocument == null ? (string) null : this.m_containingDocument.BaseURI;
                XmlResolver xmlResolver = this.m_bResolverSet ? this.m_xmlResolver : (XmlResolver) new XmlSecureResolver((XmlResolver) new XmlUrlResolver(), str);
                XmlDocument xmlDocument = Exml.PreProcessElementInput(this.SignedInfo.GetXml(), xmlResolver, str);
                CanonicalXmlNodeList namespaces = this.m_context == null ? (CanonicalXmlNodeList) null : CanonicalXmlNodeList.GetPropagatedAttributes(this.m_context);
                //SignedXmlDebugLog.LogNamespacePropagation(this, (XmlNodeList) namespaces);
                Exml.AddNamespaces(xmlDocument.DocumentElement, namespaces);
                Transform canonicalizationMethodObject = this.SignedInfo.CanonicalizationMethodObject;
                canonicalizationMethodObject.Resolver = xmlResolver;
                canonicalizationMethodObject.BaseURI = str;
                //SignedXmlDebugLog.LogBeginCanonicalization(this, canonicalizationMethodObject);
                canonicalizationMethodObject.LoadInput((object) xmlDocument);
                //SignedXmlDebugLog.LogCanonicalizedOutput(this, canonicalizationMethodObject);
                this._digestedSignedInfo = canonicalizationMethodObject.GetDigestedOutput(hash);
                this.bCacheValid = true;
            }
            return this._digestedSignedInfo;
        }

        private int GetReferenceLevel(int index, ArrayList references)
        {
            if (this.m_refProcessed[index])
                return this.m_refLevelCache[index];
            this.m_refProcessed[index] = true;
            Reference reference = (Reference) references[index];
            if (reference.Uri == null || reference.Uri.Length == 0 || reference.Uri.Length > 0 && reference.Uri[0] != '#')
            {
                this.m_refLevelCache[index] = 0;
                return 0;
            }
            if (reference.Uri.Length <= 0 || reference.Uri[0] != '#')
                throw new CryptographicException("Cryptography_Xml_InvalidReference");
            string idFromLocalUri = Exml.ExtractIdFromLocalUri(reference.Uri);
            if (idFromLocalUri == "xpointer(/)")
            {
                this.m_refLevelCache[index] = 0;
                return 0;
            }
            for (int index1 = 0; index1 < references.Count; ++index1)
            {
                if (((Reference) references[index1]).Id == idFromLocalUri)
                {
                    this.m_refLevelCache[index] = this.GetReferenceLevel(index1, references) + 1;
                    return this.m_refLevelCache[index];
                }
            }
            this.m_refLevelCache[index] = 0;
            return 0;
        }

        private void BuildDigestedReferences()
        {
            ArrayList references = this.SignedInfo.References;
            this.m_refProcessed = new bool[references.Count];
            this.m_refLevelCache = new int[references.Count];
            SignedXml.ReferenceLevelSortOrder referenceLevelSortOrder = new SignedXml.ReferenceLevelSortOrder();
            referenceLevelSortOrder.References = references;
            ArrayList arrayList = new ArrayList();
            foreach (Reference reference in references)
                arrayList.Add((object) reference);
            arrayList.Sort((IComparer) referenceLevelSortOrder);
            CanonicalXmlNodeList refList = new CanonicalXmlNodeList();
            foreach (DataObject dataObject in (IEnumerable) this.m_signature.ObjectList)
                refList.Add((object) dataObject.GetXml());
            foreach (Reference reference in arrayList)
            {
                if (reference.DigestMethod == null)
                    reference.DigestMethod = SignedXml.XmlDsigDigestDefault;
                //SignedXmlDebugLog.LogSigningReference(this, reference);
                reference.UpdateHashValue(this.m_containingDocument, refList);
                if (reference.Id != null)
                    refList.Add((object) reference.GetXml());
            }
        }

        private bool CheckDigestedReferences()
        {
            ArrayList references = this.m_signature.SignedInfo.References;
            for (int index = 0; index < references.Count; ++index)
            {
                Reference reference = (Reference) references[index];
                if (!this.ReferenceUsesSafeTransformMethods(reference))
                    return false;
                //SignedXmlDebugLog.LogVerifyReference(this, reference);
                byte[] hashValue;
                try
                {
                    hashValue = reference.CalculateHashValue(this.m_containingDocument, this.m_signature.ReferencedItems);
                }
                catch (Exception)
                {
                    //SignedXmlDebugLog.LogSignedXmlRecursionLimit(this, reference);
                    return false;
                }
                //SignedXmlDebugLog.LogVerifyReferenceHash(this, reference, hashValue, reference.DigestValue);
                if (!SignedXml.CryptographicEquals(hashValue, reference.DigestValue))
                    return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool CryptographicEquals(byte[] a, byte[] b)
        {
            int num = 0;
            if (a.Length != b.Length)
                return false;
            int length = a.Length;
            for (int index = 0; index < length; ++index)
                num |= (int) a[index] - (int) b[index];
            return num == 0;
        }

        private bool CheckSignatureFormat()
        {
            if (this.m_signatureFormatValidator == null)
                return true;
            //SignedXmlDebugLog.LogBeginCheckSignatureFormat(this, this.m_signatureFormatValidator);
            bool result = this.m_signatureFormatValidator(this);
            //SignedXmlDebugLog.LogFormatValidationResult(this, result);
            return result;
        }

        private bool CheckSignedInfo(AsymmetricAlgorithm key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof (key));
            //SignedXmlDebugLog.LogBeginCheckSignedInfo(this, this.m_signature.SignedInfo);
            SignatureDescription fromName = Exml.CreateFromName<SignatureDescription>(this.SignatureMethod);
            if (fromName == null)
                throw new CryptographicException("Cryptography_Xml_SignatureDescriptionNotCreated");
            Type type = Type.GetType(fromName.KeyAlgorithm);
            if (!SignedXml.IsKeyTheCorrectAlgorithm(key, type))
                return false;
            HashAlgorithm digest = fromName.CreateDigest();
            if (digest == null)
                throw new CryptographicException("Cryptography_Xml_CreateHashAlgorithmFailed");
            byte[] c14Ndigest = this.GetC14NDigest(digest);
            AsymmetricSignatureDeformatter deformatter = fromName.CreateDeformatter(key);
            //SignedXmlDebugLog.LogVerifySignedInfo(this, key, fromName, digest, deformatter, c14Ndigest, this.m_signature.SignatureValue);
            return deformatter.VerifySignature(c14Ndigest, this.m_signature.SignatureValue);
        }

        private bool CheckSignedInfo(KeyedHashAlgorithm macAlg)
        {
            if (macAlg == null)
                throw new ArgumentNullException(nameof (macAlg));
            //SignedXmlDebugLog.LogBeginCheckSignedInfo(this, this.m_signature.SignedInfo);
            int num = this.m_signature.SignedInfo.SignatureLength != null ? Convert.ToInt32(this.m_signature.SignedInfo.SignatureLength, (IFormatProvider) null) : macAlg.HashSize;
            if (num < 0 || num > macAlg.HashSize)
                throw new CryptographicException("Cryptography_Xml_InvalidSignatureLength");
            if (num % 8 != 0)
                throw new CryptographicException("Cryptography_Xml_InvalidSignatureLength2");
            if (this.m_signature.SignatureValue == null)
                throw new CryptographicException("Cryptography_Xml_SignatureValueRequired");
            if (this.m_signature.SignatureValue.Length != num / 8)
                throw new CryptographicException("Cryptography_Xml_InvalidSignatureLength");
            byte[] c14Ndigest = this.GetC14NDigest((HashAlgorithm) macAlg);
            //SignedXmlDebugLog.LogVerifySignedInfo(this, macAlg, c14Ndigest, this.m_signature.SignatureValue);
            for (int index = 0; index < this.m_signature.SignatureValue.Length; ++index)
            {
                if ((int) this.m_signature.SignatureValue[index] != (int) c14Ndigest[index])
                    return false;
            }
            return true;
        }

        private static XmlElement GetSingleReferenceTarget(
            XmlDocument document,
            string idAttributeName,
            string idValue)
        {
            string xpath = "//*[@" + idAttributeName + "=\"" + idValue + "\"]";
            //if (Utils.AllowAmbiguousReferenceTargets())
            //    return document.SelectSingleNode(xpath) as XmlElement;
            XmlNodeList xmlNodeList = document.SelectNodes(xpath);
            if (xmlNodeList == null || xmlNodeList.Count == 0)
                return (XmlElement) null;
            if (xmlNodeList.Count == 1)
                return xmlNodeList[0] as XmlElement;
            throw new CryptographicException("Cryptography_Xml_InvalidReference");
        }

        private static bool IsKeyTheCorrectAlgorithm(AsymmetricAlgorithm key, Type expectedType)
        {
            Type type = key.GetType();
            if (type == expectedType || expectedType.IsSubclassOf(type))
                return true;
            while (expectedType != (Type) null && expectedType.BaseType != typeof (AsymmetricAlgorithm))
                expectedType = expectedType.BaseType;
            return !(expectedType == (Type) null) && type.IsSubclassOf(expectedType);
        }

        private class ReferenceLevelSortOrder : IComparer
        {
            private ArrayList m_references;

            public ArrayList References
            {
                get
                {
                    return this.m_references;
                }
                set
                {
                    this.m_references = value;
                }
            }

            public int Compare(object a, object b)
            {
                Reference reference1 = a as Reference;
                Reference reference2 = b as Reference;
                int index1 = 0;
                int index2 = 0;
                int num = 0;
                foreach (Reference reference3 in this.References)
                {
                    if (reference3 == reference1)
                        index1 = num;
                    if (reference3 == reference2)
                        index2 = num;
                    ++num;
                }
                return reference1.SignedXml.GetReferenceLevel(index1, this.References).CompareTo(reference2.SignedXml.GetReferenceLevel(index2, this.References));
            }
        }
    }
}