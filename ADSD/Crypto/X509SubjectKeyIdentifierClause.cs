using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace ADSD.Crypto
{
    /// <summary>Represents a key identifier clause that identifies a <see cref="T:System.IdentityModel.Tokens.X509SecurityToken" /> security token using the X.509 certificate's subject key identifier extension.</summary>
    public class X509SubjectKeyIdentifierClause : BinaryKeyIdentifierClause
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509SubjectKeyIdentifierClause" /> class using the specified subject key identifier. </summary>
        /// <param name="ski">An array of <see cref="T:System.Byte" /> that contains the subject key identifier.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="ski" /> is <see langword="null" />.</exception>
        public X509SubjectKeyIdentifierClause(byte[] ski)
            : this(ski, true)
        {
        }

        internal X509SubjectKeyIdentifierClause(byte[] ski, bool cloneBuffer)
            : base((string) null, ski, cloneBuffer)
        {
        }

        private static byte[] GetSkiRawData(X509Certificate2 certificate)
        {
            if (certificate == null) throw new ArgumentNullException(nameof (certificate));
            return (certificate.Extensions["2.5.29.14"] as X509SubjectKeyIdentifierExtension)?.RawData;
        }

        /// <summary>Gets the subject key identifier.</summary>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the subject key identifier.</returns>
        public byte[] GetX509SubjectKeyIdentifier()
        {
            return this.GetBuffer();
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance is equivalent to the key identifier of the specified X.509 certificate.</summary>
        /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="certificate" /> has the same subject key identifier as the current instance; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="certificate" /> is <see langword="null" />.</exception>
        public bool Matches(X509Certificate2 certificate)
        {
            if (certificate == null)
                return false;
            byte[] skiRawData = X509SubjectKeyIdentifierClause.GetSkiRawData(certificate);
            if (skiRawData != null)
                return this.Matches(skiRawData, 2);
            return false;
        }

        /// <summary>Creates a key identifier clause using the specified X.509 certificate.</summary>
        /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> to create the key identifier clause for.</param>
        /// <param name="keyIdentifierClause">When this method returns, contains a <see cref="T:System.IdentityModel.Tokens.X509SubjectKeyIdentifierClause" /> that represents the key identifier clause. This parameter is passed uninitialized. </param>
        /// <returns>
        /// <see langword="true" /> when a key identifier clause can be created for the specified X.509 certificate; otherwise, <see langword="false" />. </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="certificate" /> is <see langword="null" />.</exception>
        public static bool TryCreateFrom(
            X509Certificate2 certificate,
            out X509SubjectKeyIdentifierClause keyIdentifierClause)
        {
            byte[] skiRawData = X509SubjectKeyIdentifierClause.GetSkiRawData(certificate);
            keyIdentifierClause = (X509SubjectKeyIdentifierClause) null;
            if (skiRawData != null)
            {
                byte[] ski = CloneBuffer(skiRawData, 2, skiRawData.Length - 2);
                keyIdentifierClause = new X509SubjectKeyIdentifierClause(ski, false);
            }
            return keyIdentifierClause != null;
        }

        /// <summary>Gets a value that indicates whether a key identifier clause can be created for the specified X.509 certificate. </summary>
        /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate.</param>
        /// <returns>
        /// <see langword="true" /> if a key identifier clause can be created for <paramref name="certificate" />; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="certificate" /> is <see langword="null" />.</exception>
        public static bool CanCreateFrom(X509Certificate2 certificate)
        {
            return X509SubjectKeyIdentifierClause.GetSkiRawData(certificate) != null;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A <see cref="T:System.String" /> that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "X509SubjectKeyIdentifierClause(SKI = 0x{0})", new object[1]
            {
                (object) this.ToHexString()
            });
        }
    }
}