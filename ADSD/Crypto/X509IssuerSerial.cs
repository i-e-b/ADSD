using System;

namespace ADSD.Crypto
{
    /// <summary>Represents the &lt;<see langword="X509IssuerSerial" />&gt; element of an XML digital signature.</summary>
    public struct X509IssuerSerial
    {
        private string issuerName;
        private string serialNumber;

        internal X509IssuerSerial(string issuerName, string serialNumber)
        {
            if (string.IsNullOrEmpty(issuerName))
                throw new ArgumentException("Arg_EmptyOrNullString", nameof (issuerName));
            if (string.IsNullOrEmpty(serialNumber))
                throw new ArgumentException("Arg_EmptyOrNullString", nameof (serialNumber));
            this.issuerName = issuerName;
            this.serialNumber = serialNumber;
        }

        /// <summary>Gets or sets an X.509 certificate issuer's distinguished name.</summary>
        /// <returns>An X.509 certificate issuer's distinguished name.</returns>
        public string IssuerName
        {
            get
            {
                return this.issuerName;
            }
            set
            {
                this.issuerName = value;
            }
        }

        /// <summary>Gets or sets an X.509 certificate issuer's serial number.</summary>
        /// <returns>An X.509 certificate issuer's serial number.</returns>
        public string SerialNumber
        {
            get
            {
                return this.serialNumber;
            }
            set
            {
                this.serialNumber = value;
            }
        }
    }
}