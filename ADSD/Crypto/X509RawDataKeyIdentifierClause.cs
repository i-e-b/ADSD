using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace ADSD.Crypto
{
    /// <summary>Represents a key identifier clause that identifies a <see cref="T:System.IdentityModel.Tokens.X509SecurityToken" /> security token using the X.509 certificate's raw data.</summary>
    public class X509RawDataKeyIdentifierClause : BinaryKeyIdentifierClause
    {
        private X509Certificate2 certificate;
        private X509AsymmetricSecurityKey key;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509RawDataKeyIdentifierClause" /> class using the specified X.509 certificate. </summary>
        /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="certificate" /> is <see langword="null" />.</exception>
        public X509RawDataKeyIdentifierClause(X509Certificate2 certificate)
            : this(X509RawDataKeyIdentifierClause.GetRawData((X509Certificate) certificate), false)
        {
            this.certificate = certificate;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509RawDataKeyIdentifierClause" /> class using the specified raw data of an X.509 certificate. </summary>
        /// <param name="certificateRawData">An array of <see cref="T:System.Byte" /> that contains the raw data of an X.509 certificate.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="certificateRawData" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="certificateRawData" /> is zero length.</exception>
        public X509RawDataKeyIdentifierClause(byte[] certificateRawData)
            : this(certificateRawData, true)
        {
        }

        internal X509RawDataKeyIdentifierClause(byte[] certificateRawData, bool cloneBuffer)
            : base((string) null, certificateRawData, cloneBuffer)
        {
        }

        /// <summary>Gets a value that indicates whether a key can be created from the raw data of the X.509 certificate or byte array that is specified in the constructor. </summary>
        /// <returns>
        /// <see langword="true" /> in all cases.</returns>
        public override bool CanCreateKey
        {
            get
            {
                return true;
            }
        }

        /// <summary>Creates a key from the raw data of the X.509 certificate or byte array that is specified in the constructor.</summary>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.X509AsymmetricSecurityKey" /> that contains the key(s) associated with the X.509 certificate.</returns>
        public override SecurityKey CreateKey()
        {
            if (this.key == null)
            {
                if (this.certificate == null)
                    this.certificate = new X509Certificate2(this.GetBuffer());
                this.key = new X509AsymmetricSecurityKey(this.certificate);
            }
            return (SecurityKey) this.key;
        }

        private static byte[] GetRawData(X509Certificate certificate)
        {
            if (certificate == null) throw new ArgumentNullException(nameof (certificate));
            return certificate.GetRawCertData();
        }

        /// <summary>Gets the raw data associated with the X.509 certificate.</summary>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the raw data associated with the X.509 certificate.</returns>
        public byte[] GetX509RawData()
        {
            return this.GetBuffer();
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance is equivalent to the specified X.509 certificate.</summary>
        /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="certificate" /> has the raw data that matches the current instance; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="certificate" /> is <see langword="null" />.</exception>
        public bool Matches(X509Certificate2 certificate)
        {
            if (certificate == null)
                return false;
            return this.Matches(X509RawDataKeyIdentifierClause.GetRawData((X509Certificate) certificate));
        }

        /// <summary>Returns the current object.</summary>
        /// <returns>A <see cref="T:System.String" /> that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "X509RawDataKeyIdentifierClause(RawData = {0})", new object[1]
            {
                (object) this.ToBase64String()
            });
        }
    }
}