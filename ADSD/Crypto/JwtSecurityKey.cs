using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;

namespace ADSD.Crypto
{
    /// <summary>
    /// An X509 key used in signing JWT tokens
    /// </summary>
    public class JwtSecurityKey
    {
        [NotNull] private readonly object thisLock = new object();
        [NotNull] private readonly X509Certificate2 certificate;
        private AsymmetricAlgorithm privateKey;
        private bool privateKeyAvailabilityDetermined;
        private AsymmetricAlgorithm publicKey;
        private bool publicKeyAvailabilityDetermined;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509AsymmetricSecurityKey" /> class using the specified X.509 certificate. </summary>
        /// <param name="certificate">The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that represents the X.509 certificate.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="certificate" /> is <see langword="null" />.</exception>
        public JwtSecurityKey(X509Certificate2 certificate)
        {
            this.certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
        }


        /// <summary>
        /// Gets the size, in bits, of the public key associated with the X.509 certificate.
        /// If the public key is not valid, this will return zero.
        /// </summary>
        public int KeySize => PublicKey?.KeySize ?? 0;

        /// <summary>
        /// Private side of certificate provided
        /// </summary>
        public AsymmetricAlgorithm PrivateKey
        {
            get
            {
                if (privateKeyAvailabilityDetermined) return privateKey;
                lock (thisLock)
                {
                    if (privateKeyAvailabilityDetermined) return privateKey;
                    privateKey = certificate.PrivateKey;
                    privateKeyAvailabilityDetermined = true;
                    return privateKey;
                }
            }
        }

        /// <summary>
        /// Public side of certificate provided
        /// </summary>
        public AsymmetricAlgorithm PublicKey
        {
            get
            {
                if (publicKeyAvailabilityDetermined) return publicKey;
                lock (thisLock)
                {
                    if (publicKeyAvailabilityDetermined) return publicKey;
                    publicKey = certificate.PublicKey.Key;
                    publicKeyAvailabilityDetermined = true;
                    return publicKey;
                }
            }
        }

        /// <summary>Decrypts the specified encrypted key using the specified cryptographic algorithm.</summary>
        /// <param name="algorithm">The cryptographic algorithm to decrypt the key.</param>
        /// <param name="keyData">An array of <see cref="T:System.Byte" /> that contains the encrypted key.</param>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the decrypted key.</returns>
        /// <exception cref="T:System.NotSupportedException">The X.509 certificate specified in the constructor does not have a private key.-or-The X.509 certificate has a private key, but it was not generated using the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.-or-The X.509 certificate has a private key, it was generated using the <see cref="T:System.Security.Cryptography.RSA" /> algorithm, but the <see cref="P:System.Security.Cryptography.AsymmetricAlgorithm.KeyExchangeAlgorithm" /> property is <see langword="null" />.-or-The <paramref name="algorithm" /> parameter is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSA15Url" /> and <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSAOAEPUrl" />.</exception>
        public byte[] DecryptKey(string algorithm, byte[] keyData)
        {
            if (PrivateKey == null) throw new NotSupportedException("Missing Private Key");

            var priv = PrivateKey as RSA;
            if (priv == null) throw new NotSupportedException("Private Key is not an RSA key");


            if (priv.KeyExchangeAlgorithm == null)
                throw new NotSupportedException("Private Key exchange algorithm was not available");

            if (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5")
                return EncryptedXml.DecryptKey(keyData, priv, false);
            if (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p" || this.IsSupportedAlgorithm(algorithm))
                return EncryptedXml.DecryptKey(keyData, priv, true);

            throw new NotSupportedException($"The requested cryptography algorithm '{algorithm}' is not supported by this library");
        }

        /// <summary>Encrypts the specified encrypted key using the specified cryptographic algorithm.</summary>
        /// <param name="algorithm">The cryptographic algorithm to encrypt the key.</param>
        /// <param name="keyData">An array of <see cref="T:System.Byte" /> that contains the key to encrypt.</param>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the encrypted key.</returns>
        /// <exception cref="T:System.NotSupportedException">The X.509 certificate specified in the constructor has a public key that was not generated using the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.-or-The <paramref name="algorithm" /> parameter is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSA15Url" /> and <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSAOAEPUrl" />.</exception>
        public byte[] EncryptKey(string algorithm, byte[] keyData)
        {
            RSA publicKey = this.PublicKey as RSA;
            if (publicKey == null) throw new NotSupportedException("PublicKeyNotRSA");

            if (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5") return EncryptedXml.EncryptKey(keyData, publicKey, false);
            if (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p" || this.IsSupportedAlgorithm(algorithm)) return EncryptedXml.EncryptKey(keyData, publicKey, true);

            throw new NotSupportedException("UnsupportedCryptoAlgorithm: " + algorithm);
        }

        /// <summary>Gets the specified asymmetric cryptographic algorithm.</summary>
        /// <param name="algorithm">The asymmetric algorithm to create.</param>
        /// <param name="privateKey">
        /// <see langword="true" /> when a private key is required to create the algorithm; otherwise, <see langword="false" />. </param>
        /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> that represents the specified asymmetric cryptographic algorithm.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="privateKey" /> is <see langword="true" /> and the X.509 certificate specified in the constructor does not have a private key.-or-
        /// <paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" /> and the public or private key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.DSA" />. -or-
        /// <paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSA15Url" />, <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSAOAEPUrl" />, <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" /> or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" /> and the public or private key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.RSA" />. -or-
        /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" />, <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSA15Url" />, <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSAOAEPUrl" />, <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" />.</exception>
        public AsymmetricAlgorithm GetAsymmetricAlgorithm(
      string algorithm,
      bool privateKey)
        {
            if (privateKey)
            {
                if (this.PrivateKey == null) throw new NotSupportedException("MissingPrivateKey");
                if (string.IsNullOrEmpty(algorithm)) throw new ArgumentException("EmptyOrNullArgumentString", nameof(algorithm));

                if (algorithm != "http://www.w3.org/2000/09/xmldsig#dsa-sha1")
                {
                    if (algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" || (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5" || algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"))
                    {
                        if (this.PrivateKey is RSA) return (AsymmetricAlgorithm)(this.PrivateKey as RSA);

                        throw new NotSupportedException("AlgorithmAndPrivateKeyMisMatch");
                    }
                    if (this.IsSupportedAlgorithm(algorithm)) return this.PrivateKey;

                    throw new NotSupportedException("UnsupportedCryptoAlgorithm: " + algorithm);
                }
                if (this.PrivateKey is DSA) return (AsymmetricAlgorithm)(this.PrivateKey as DSA);

                throw new NotSupportedException("AlgorithmAndPrivateKeyMisMatch");
            }
            if (algorithm != "http://www.w3.org/2000/09/xmldsig#dsa-sha1")
            {
                if (algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" || (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5" || algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"))
                {
                    if (this.PublicKey is RSA) return (AsymmetricAlgorithm)(this.PublicKey as RSA);

                    throw new NotSupportedException("AlgorithmAndPublicKeyMisMatch");
                }
                throw new NotSupportedException("UnsupportedCryptoAlgorithm: " + algorithm);
            }
            if (this.PublicKey is DSA) return (AsymmetricAlgorithm)(this.PublicKey as DSA);

            throw new NotSupportedException("AlgorithmAndPublicKeyMisMatch");
        }

        /// <summary>Gets a cryptographic algorithm that generates a hash for a digital signature.</summary>
        /// <param name="algorithm">The hash algorithm.</param>
        /// <returns>A <see cref="T:System.Security.Cryptography.HashAlgorithm" /> that generates hashes for digital signatures.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" />, <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" />.</exception>
        public HashAlgorithm GetHashAlgorithmForSignature(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm)) throw new ArgumentException("EmptyOrNullArgumentString", nameof(algorithm));

            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                SignatureDescription signatureDescription = algorithmFromConfig as SignatureDescription;
                if (signatureDescription != null)
                    return signatureDescription.CreateDigest();
                HashAlgorithm hashAlgorithm = algorithmFromConfig as HashAlgorithm;
                if (hashAlgorithm != null)
                    return hashAlgorithm;
                throw new CryptographicException("UnsupportedAlgorithmForCryptoOperation");
            }
            if (algorithm == "http://www.w3.org/2000/09/xmldsig#dsa-sha1" || algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1")
                return CryptoHelper.NewSha1HashAlgorithm();
            if (algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")
                return CryptoHelper.NewSha256HashAlgorithm();
            throw new NotSupportedException("UnsupportedCryptoAlgorithm");
        }

        /// <summary>Gets the de-formatter algorithm for the digital signature.</summary>
        /// <param name="algorithm">The de-formatter algorithm for the digital signature to get an instance of.</param>
        /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricSignatureDeformatter" /> that represents the de-formatter algorithm for the digital signature.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" /> and the public key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.DSA" />.-or-
        /// <paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" /> or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" /> and the public key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.RSA" />.-or-
        /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" />,
        /// <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" />.</exception>
        public AsymmetricSignatureDeformatter GetSignatureDeformatter(
          string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm)) throw new ArgumentNullException(nameof(algorithm));

            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                SignatureDescription signatureDescription = algorithmFromConfig as SignatureDescription;
                if (signatureDescription != null)
                    return signatureDescription.CreateDeformatter(this.PublicKey);
                try
                {
                    AsymmetricSignatureDeformatter signatureDeformatter = algorithmFromConfig as AsymmetricSignatureDeformatter;
                    if (signatureDeformatter != null)
                    {
                        signatureDeformatter.SetKey(this.PublicKey);
                        return signatureDeformatter;
                    }
                }
                catch (InvalidCastException ex)
                {
                    throw new NotSupportedException("AlgorithmAndPublicKeyMisMatch", (Exception)ex);
                }
                throw new CryptographicException("UnsupportedAlgorithmForCryptoOperation");
            }
            if (algorithm != "http://www.w3.org/2000/09/xmldsig#dsa-sha1")
            {
                if (algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")
                {
                    RSA publicKey = this.PublicKey as RSA;
                    if (publicKey == null) throw new NotSupportedException("PublicKeyNotRSA");
                    return (AsymmetricSignatureDeformatter)new RSAPKCS1SignatureDeformatter((AsymmetricAlgorithm)publicKey);
                }
                throw new NotSupportedException("UnsupportedCryptoAlgorithm");
            }
            DSA publicKey1 = this.PublicKey as DSA;
            if (publicKey1 == null)
                throw new NotSupportedException("PublicKeyNotDSA");
            return (AsymmetricSignatureDeformatter)new DSASignatureDeformatter((AsymmetricAlgorithm)publicKey1);
        }

        /// <summary>Gets the formatter algorithm for the digital signature.</summary>
        /// <param name="algorithm">The formatter algorithm for the digital signature to get an instance of.</param>
        /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricSignatureDeformatter" /> that represents the formatter algorithm for the digital signature.</returns>
        /// <exception cref="T:System.NotSupportedException">The X.509 certificate specified in the constructor does not have a private key.-or-
        /// <paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" /> and the private key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.DSA" />.-or-
        /// <paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" /> or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" /> and the private key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.RSA" />.-or-
        /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" />,
        /// <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" />.</exception>
        public AsymmetricSignatureFormatter GetSignatureFormatter(
          string algorithm)
        {
            if (this.PrivateKey == null) throw new NotSupportedException("MissingPrivateKey");
            if (string.IsNullOrEmpty(algorithm)) throw new ArgumentNullException(nameof(algorithm));

            AsymmetricAlgorithm key = X509AsymmetricSecurityKey.LevelUpRsa(this.PrivateKey, algorithm);
            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                SignatureDescription signatureDescription = algorithmFromConfig as SignatureDescription;
                if (signatureDescription != null)
                    return signatureDescription.CreateFormatter(key);
                try
                {
                    AsymmetricSignatureFormatter signatureFormatter = algorithmFromConfig as AsymmetricSignatureFormatter;
                    if (signatureFormatter != null)
                    {
                        signatureFormatter.SetKey(key);
                        return signatureFormatter;
                    }
                }
                catch (InvalidCastException ex)
                {
                    throw new NotSupportedException("AlgorithmAndPrivateKeyMisMatch", (Exception)ex);
                }
                throw new CryptographicException("UnsupportedAlgorithmForCryptoOperation");
            }
            if (algorithm != "http://www.w3.org/2000/09/xmldsig#dsa-sha1")
            {
                if (algorithm != "http://www.w3.org/2000/09/xmldsig#rsa-sha1")
                {
                    if (algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")
                    {
                        RSA rsa = key as RSA;
                        if (rsa == null) throw new NotSupportedException("PrivateKeyNotRSA");

                        return (AsymmetricSignatureFormatter)new RSAPKCS1SignatureFormatter((AsymmetricAlgorithm)rsa);
                    }
                    throw new NotSupportedException("UnsupportedCryptoAlgorithm");
                }
                RSA privateKey = this.PrivateKey as RSA;
                if (privateKey == null) throw new NotSupportedException("PrivateKeyNotRSA");

                return (AsymmetricSignatureFormatter)new RSAPKCS1SignatureFormatter((AsymmetricAlgorithm)privateKey);
            }
            DSA privateKey1 = this.PrivateKey as DSA;
            if (privateKey1 == null) throw new NotSupportedException("PrivateKeyNotDSA");

            return (AsymmetricSignatureFormatter)new DSASignatureFormatter((AsymmetricAlgorithm)privateKey1);
        }

        private static AsymmetricAlgorithm LevelUpRsa(
          AsymmetricAlgorithm asymmetricAlgorithm,
          string algorithm)
        {
            if (GlobalSettings.DisableUpdatingRsaProviderType) return asymmetricAlgorithm;

            if (asymmetricAlgorithm == null) throw new ArgumentNullException(nameof(asymmetricAlgorithm));
            if (string.IsNullOrEmpty(algorithm)) throw new ArgumentNullException(nameof(algorithm));

            if (!string.Equals(algorithm, "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")) return asymmetricAlgorithm;

            var cryptoServiceProvider = asymmetricAlgorithm as RSACryptoServiceProvider;
            if (cryptoServiceProvider == null)
                return asymmetricAlgorithm;
            if (cryptoServiceProvider.CspKeyContainerInfo.ProviderType != 1 && cryptoServiceProvider.CspKeyContainerInfo.ProviderType != 12 || cryptoServiceProvider.CspKeyContainerInfo.HardwareDevice)
                return (AsymmetricAlgorithm)cryptoServiceProvider;
            CspParameters parameters = new CspParameters();
            parameters.ProviderType = 24;
            parameters.KeyContainerName = cryptoServiceProvider.CspKeyContainerInfo.KeyContainerName;
            parameters.KeyNumber = (int)cryptoServiceProvider.CspKeyContainerInfo.KeyNumber;
            if (cryptoServiceProvider.CspKeyContainerInfo.MachineKeyStore)
                parameters.Flags = CspProviderFlags.UseMachineKeyStore;
            parameters.Flags |= CspProviderFlags.UseExistingKey;
            return (AsymmetricAlgorithm)new RSACryptoServiceProvider(parameters);
        }

        /// <summary>Gets a value that indicates whether the private key is a available. </summary>
        /// <returns>
        /// <see langword="true" /> when the private key is available; otherwise, <see langword="false" />.</returns>
        public bool HasPrivateKey()
        {
            return this.PrivateKey != null;
        }

        /// <summary>Gets a value that indicates whether the specified algorithm uses asymmetric keys.</summary>
        /// <param name="algorithm">The cryptographic algorithm.</param>
        /// <returns>
        /// <see langword="true" /> when the specified algorithm is <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.DsaSha1Signature" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha1Signature" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaOaepKeyWrap" />, or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaV15KeyWrap" />; otherwise, <see langword="false" />.</returns>
        public bool IsAsymmetricAlgorithm(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm)) throw new ArgumentNullException(nameof(algorithm));
            return CryptoHelper.IsAsymmetricAlgorithm(algorithm);
        }

        /// <summary>Gets a value that indicates whether the specified algorithm is supported by this class. </summary>
        /// <param name="algorithm">The cryptographic algorithm.</param>
        /// <returns>
        /// <see langword="true" /> when the specified algorithm is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" />, <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSA15Url" />, <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSAOAEPUrl" />, <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" />, or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" /> and the public key is of the right type; otherwise, <see langword="false" />. See the remarks for details.</returns>
        public bool IsSupportedAlgorithm(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm)) throw new ArgumentNullException(nameof(algorithm));
            object obj = (object)null;
            try
            {
                obj = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
                algorithm = (string)null;
            }
            if (obj != null) return obj is SignatureDescription || obj is AsymmetricAlgorithm;
            if (algorithm == "http://www.w3.org/2000/09/xmldsig#dsa-sha1") return this.PublicKey is DSA;
            if (algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1"
                || algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"
                || algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5"
                || algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p")
                return this.PublicKey is RSA;
            return false;
        }

        /// <summary>Gets a value that indicates whether the specified algorithm uses symmetric keys.</summary>
        /// <param name="algorithm">The cryptographic algorithm.</param>
        /// <returns>
        /// <see langword="true" /> when the specified algorithm is <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.HmacSha1Signature" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesEncryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128KeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192KeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256KeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesKeyWrap" />, or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Psha1KeyDerivation" />; otherwise, <see langword="false" />.</returns>
        public bool IsSymmetricAlgorithm(string algorithm)
        {
            return CryptoHelper.IsSymmetricAlgorithm(algorithm);
        }



        /// <summary>
        /// Gets the <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" />.
        /// </summary>
        public X509Certificate2 Certificate => certificate;
    }
}
