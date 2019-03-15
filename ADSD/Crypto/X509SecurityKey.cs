using System.Security.Cryptography.X509Certificates;

namespace ADSD
{
    /// <summary>Security key that allows access to cert</summary>
    public class X509SecurityKey : X509AsymmetricSecurityKey
    {
        private X509Certificate2 _certificate;

        /// <summary>
        /// Instantiates a <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> using a <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" />
        /// </summary>
        /// <param name="certificate"> cert to use.</param>
        public X509SecurityKey(X509Certificate2 certificate)
            : base(certificate)
        {
            this._certificate = certificate;
        }

        /// <summary>
        /// Gets the <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" />.
        /// </summary>
        public X509Certificate2 Certificate
        {
            get
            {
                return this._certificate;
            }
        }
    }
}