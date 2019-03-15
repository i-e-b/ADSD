using System;
using System.Security.Cryptography.X509Certificates;

namespace ADSD
{
    /// <summary>Represents an X.509 token used as the signing credential.</summary>
    public class X509SigningCredentials : SigningCredentials
    {
        private X509Certificate2 certificate;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509SigningCredentials" /> class based on the specified X.509 certificate.</summary>
        /// <param name="certificate">The X.509 certificate.</param>
        public X509SigningCredentials(X509Certificate2 certificate)
            : this(certificate, new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[1]
            {
                (SecurityKeyIdentifierClause) new X509SecurityToken(certificate).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>()
            }))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509SigningCredentials" /> class based on the specified X.509 certificate, signature algorithm, and digest algorithm. </summary>
        /// <param name="certificate">The X.509 certificate.</param>
        /// <param name="signatureAlgorithm">The signature algorithm.</param>
        /// <param name="digestAlgorithm">The digest algorithm.</param>
        public X509SigningCredentials(
            X509Certificate2 certificate,
            string signatureAlgorithm,
            string digestAlgorithm)
            : this(new X509SecurityToken(certificate), new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[1]
            {
                (SecurityKeyIdentifierClause) new X509SecurityToken(certificate).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>()
            }), signatureAlgorithm, digestAlgorithm)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509SigningCredentials" /> class based on the specified X.509 certificate and security key identifier. </summary>
        /// <param name="certificate">The X.509 certificate.</param>
        /// <param name="ski">The security key identifier.</param>
        public X509SigningCredentials(X509Certificate2 certificate, SecurityKeyIdentifier ski)
            : this(new X509SecurityToken(certificate), ski, "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", "http://www.w3.org/2001/04/xmlenc#sha256")
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509SigningCredentials" /> class based on the specified X.509 certificate, security key identifier, signature algorithm, and digest algorithm. </summary>
        /// <param name="certificate">The X.509 certificate.</param>
        /// <param name="ski">The security key identifier.</param>
        /// <param name="signatureAlgorithm">The signature algorithm.</param>
        /// <param name="digestAlgorithm">The digest algorithm.</param>
        public X509SigningCredentials(
            X509Certificate2 certificate,
            SecurityKeyIdentifier ski,
            string signatureAlgorithm,
            string digestAlgorithm)
            : this(new X509SecurityToken(certificate), ski, signatureAlgorithm, digestAlgorithm)
        {
        }

        internal X509SigningCredentials(
            X509SecurityToken token,
            SecurityKeyIdentifier ski,
            string signatureAlgorithm,
            string digestAlgorithm)
            : base(token.SecurityKeys[0], signatureAlgorithm, digestAlgorithm, ski)
        {
            this.certificate = token.Certificate;
            if (!this.certificate.HasPrivateKey)
                throw new Exception("Certificate has no private key");
        }

        /// <summary>Gets the X.509 certificate.</summary>
        /// <returns>The X.509 certificate.</returns>
        public X509Certificate2 Certificate
        {
            get
            {
                return this.certificate;
            }
        }
    }
}