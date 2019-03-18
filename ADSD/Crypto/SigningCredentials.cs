using System;

namespace ADSD.Crypto
{
    /// <summary>Represents the cryptographic key and security algorithms that are used to generate a digital signature.</summary>
    public class SigningCredentials
    {

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SigningCredentials" /> class.  </summary>
        /// <param name="signingKey">A <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that contains the cryptographic key that is used to generate the digital signature. </param>
        /// <param name="signatureAlgorithm">A URI that represents the cryptographic algorithm that is used to generate the digital signature.</param>
        /// <param name="digestAlgorithm">A URI that represents the cryptographic algorithm that is used to compute the digest for the portion of the SOAP message that is to be digitally signed.</param>
        public SigningCredentials(
            SecurityKey signingKey,
            string signatureAlgorithm,
            string digestAlgorithm)
            : this(signingKey, signatureAlgorithm, digestAlgorithm, (SecurityKeyIdentifier) null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SigningCredentials" /> class. </summary>
        /// <param name="signingKey">A <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that contains the cryptographic key that is used to generate the digital signature.</param>
        /// <param name="signatureAlgorithm">A URI that represents the cryptographic algorithm that is used to generate the digital signature.</param>
        /// <param name="digestAlgorithm">A URI that represents the cryptographic algorithm that is used to compute the digest for the portion of the SOAP message that is to be digitally signed.</param>
        /// <param name="signingKeyIdentifier">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that specifies the identifier that represents the key that is used to create a digital signature.</param>
        public SigningCredentials(
            SecurityKey signingKey,
            string signatureAlgorithm,
            string digestAlgorithm,
            SecurityKeyIdentifier signingKeyIdentifier)
        {
            SigningKey = signingKey ?? throw new ArgumentNullException(nameof (signingKey));
            SignatureAlgorithm = signatureAlgorithm ?? throw new ArgumentNullException(nameof (signatureAlgorithm));
            DigestAlgorithm = digestAlgorithm ?? throw new ArgumentNullException(nameof (digestAlgorithm));
            SigningKeyIdentifier = signingKeyIdentifier;
        }

        /// <summary>Gets the cryptographic algorithm that is used to compute the digest for the portion of the SOAP message that is to be digitally signed.</summary>
        /// <returns>A URI that represents the cryptographic algorithm that is used to compute the digest for the portion of the SOAP message that is to be digitally signed.</returns>
        public string DigestAlgorithm { get; }

        /// <summary>Gets the cryptographic algorithm that is used to generate the digital signature.</summary>
        /// <returns>A URI that represents the cryptographic algorithm that is used to generate the digital signature.</returns>
        public string SignatureAlgorithm { get; }

        /// <summary>Gets the cryptographic key that is used to generate the digital signature.</summary>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that contains the cryptographic key that is used to generate the digital signature.</returns>
        public SecurityKey SigningKey { get; }

        /// <summary>Gets the identifier that represents the key that is used to create a digital signature.</summary>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that specifies the identifier that represents the key that is used to create a digital signature.</returns>
        public SecurityKeyIdentifier SigningKeyIdentifier { get; }
    }
}