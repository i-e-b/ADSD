using System;

namespace ADSD
{
    /// <summary>Represents the cryptographic key and security algorithms that are used to generate a digital signature.</summary>
    public class SigningCredentials
    {
        private string digestAlgorithm;
        private string signatureAlgorithm;
        private SecurityKey signingKey;
        private SecurityKeyIdentifier signingKeyIdentifier;

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
            if (signingKey == null)
                throw new ArgumentNullException(nameof (signingKey));
            if (signatureAlgorithm == null)
                throw new ArgumentNullException(nameof (signatureAlgorithm));
            if (digestAlgorithm == null)
                throw new ArgumentNullException(nameof (digestAlgorithm));
            this.signingKey = signingKey;
            this.signatureAlgorithm = signatureAlgorithm;
            this.digestAlgorithm = digestAlgorithm;
            this.signingKeyIdentifier = signingKeyIdentifier;
        }

        /// <summary>Gets the cryptographic algorithm that is used to compute the digest for the portion of the SOAP message that is to be digitally signed.</summary>
        /// <returns>A URI that represents the cryptographic algorithm that is used to compute the digest for the portion of the SOAP message that is to be digitally signed.</returns>
        public string DigestAlgorithm
        {
            get
            {
                return this.digestAlgorithm;
            }
        }

        /// <summary>Gets the cryptographic algorithm that is used to generate the digital signature.</summary>
        /// <returns>A URI that represents the cryptographic algorithm that is used to generate the digital signature.</returns>
        public string SignatureAlgorithm
        {
            get
            {
                return this.signatureAlgorithm;
            }
        }

        /// <summary>Gets the cryptographic key that is used to generate the digital signature.</summary>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that contains the cryptographic key that is used to generate the digital signature.</returns>
        public SecurityKey SigningKey
        {
            get
            {
                return this.signingKey;
            }
        }

        /// <summary>Gets the identifier that represents the key that is used to create a digital signature.</summary>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that specifies the identifier that represents the key that is used to create a digital signature.</returns>
        public SecurityKeyIdentifier SigningKeyIdentifier
        {
            get
            {
                return this.signingKeyIdentifier;
            }
        }
    }
}