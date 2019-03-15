using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ADSD
{
    /// <summary>Represents a key identifier clause that identifies a <see cref="T:System.IdentityModel.Tokens.X509SecurityToken" /> security tokens using the distinguished name of the certificate issuer and the X.509 certificate's serial number.</summary>
    public class X509IssuerSerialKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        private readonly string issuerName;
        private readonly string issuerSerialNumber;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509IssuerSerialKeyIdentifierClause" /> class using the specified distinguished name of the certificate issuer and the serial number of the X.509 certificate. </summary>
        /// <param name="issuerName">The distinguished name of the certificate authority that issued the X.509 certificate. Sets the value of the <see cref="P:System.IdentityModel.Tokens.X509IssuerSerialKeyIdentifierClause.IssuerName" /> property.</param>
        /// <param name="issuerSerialNumber">The serial number of the X.509 certificate. Sets the value of the <see cref="P:System.IdentityModel.Tokens.X509IssuerSerialKeyIdentifierClause.IssuerSerialNumber" /> property.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="issuerName" /> is <see langword="null" />.-or-
        /// <paramref name="issuerSerialNumber" /> is <see langword="null" />.</exception>
        public X509IssuerSerialKeyIdentifierClause(string issuerName, string issuerSerialNumber)
            : base((string) null)
        {
            if (string.IsNullOrEmpty(issuerName)) throw new ArgumentNullException(nameof (issuerName));
            if (string.IsNullOrEmpty(issuerSerialNumber)) throw new ArgumentNullException(nameof (issuerSerialNumber));

            this.issuerName = issuerName;
            this.issuerSerialNumber = issuerSerialNumber;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509IssuerSerialKeyIdentifierClause" /> class using the specified X.509 certificate. </summary>
        /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="certificate" /> is <see langword="null" />.</exception>
        public X509IssuerSerialKeyIdentifierClause(X509Certificate2 certificate)
            : base((string) null)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            this.issuerName = certificate.Issuer;
            this.issuerSerialNumber = Asn1IntegerConverter.Asn1IntegerToDecimalString(certificate.GetSerialNumber());
        }

        /// <summary>Gets the distinguished name of the certificate authority that issued the X.509 certificate.</summary>
        /// <returns>The distinguished name of the certificate authority that issued the X.509 certificate.</returns>
        public string IssuerName
        {
            get
            {
                return this.issuerName;
            }
        }

        /// <summary>Gets the serial number of the X.509 certificate.</summary>
        /// <returns>The serial number of the X.509 certificate.</returns>
        public string IssuerSerialNumber
        {
            get
            {
                return this.issuerSerialNumber;
            }
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance matches the specified key identifier.</summary>
        /// <param name="keyIdentifierClause">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to compare to this instance.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="keyIdentifierClause" /> is a <see cref="T:System.IdentityModel.Tokens.X509IssuerSerialKeyIdentifierClause" /> type and the key identifier clauses match; otherwise, <see langword="false" />.</returns>
        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            X509IssuerSerialKeyIdentifierClause identifierClause = keyIdentifierClause as X509IssuerSerialKeyIdentifierClause;
            if (this == identifierClause)
                return true;
            if (identifierClause != null)
                return identifierClause.Matches(this.issuerName, this.issuerSerialNumber);
            return false;
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance matches the specified X.509 certificate.</summary>
        /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="certificate" /> has the same issuer name and issuer serial number as the current instance; otherwise, <see langword="false" />.</returns>
        public bool Matches(X509Certificate2 certificate)
        {
            if (certificate == null)
                return false;
            return this.Matches(certificate.Issuer, Asn1IntegerConverter.Asn1IntegerToDecimalString(certificate.GetSerialNumber()));
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance is equivalent to the specified issuer name and issuer serial number.</summary>
        /// <param name="issuerName">The distinguished name of the certificate authority that issued the X.509 certificate.</param>
        /// <param name="issuerSerialNumber">The serial number of the X.509 certificate.</param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="issuerName " />and <paramref name="issuerSerialNumber" /> parameters match the <see cref="P:System.IdentityModel.Tokens.X509IssuerSerialKeyIdentifierClause.IssuerName" /> and <see cref="P:System.IdentityModel.Tokens.X509IssuerSerialKeyIdentifierClause.IssuerSerialNumber" /> properties; otherwise, <see langword="false" />.</returns>
        public bool Matches(string issuerName, string issuerSerialNumber)
        {
            if (issuerName == null || this.issuerSerialNumber != issuerSerialNumber)
                return false;
            if (this.issuerName == issuerName)
                return true;
            bool flag = false;
            try
            {
                if (CryptoHelper.IsEqual(new X500DistinguishedName(this.issuerName).RawData, new X500DistinguishedName(issuerName).RawData))
                    flag = true;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine(ex);
            }
            return flag;
        }

        /// <summary>Returns the current object.</summary>
        /// <returns>A <see cref="T:System.String" /> that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "X509IssuerSerialKeyIdentifierClause(Issuer = '{0}', Serial = '{1}')", new object[2]
            {
                (object) this.IssuerName,
                (object) this.IssuerSerialNumber
            });
        }
    }
}