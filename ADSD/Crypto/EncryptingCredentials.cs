using System;

namespace ADSD.Crypto
{
    /// <summary>Represents the cryptographic key and encrypting algorithm that are used to encrypt the proof key. </summary>
    public class EncryptingCredentials
    {
        private string _algorithm;
        private SecurityKey _key;
        private SecurityKeyIdentifier _keyIdentifier;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.EncryptingCredentials" /> class.</summary>
        public EncryptingCredentials()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.EncryptingCredentials" /> class with the specified cryptographic key, key identifier, and encryption algorithm.</summary>
        /// <param name="key">A <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that contains the cryptographic key that is used for encryption.</param>
        /// <param name="keyIdentifier">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that specifies the identifier that represents the key that is used for encryption.</param>
        /// <param name="algorithm">A URI that represents the cryptographic algorithm that is used for encryption.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is <see langword="null" />.-or-
        /// <paramref name="keyIdentifier" /> is <see langword="null" />.-or-
        /// <paramref name="algorithm" /> is <see langword="null" />.</exception>
        public EncryptingCredentials(SecurityKey key, SecurityKeyIdentifier keyIdentifier, string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm)) throw new ArgumentNullException(nameof (algorithm));
            this._algorithm = algorithm;
            this._key = key ?? throw new ArgumentNullException(nameof (key));
            this._keyIdentifier = keyIdentifier ?? throw new ArgumentNullException(nameof (keyIdentifier));
        }

        /// <summary>Gets or sets the encryption algorithm.</summary>
        /// <returns>A URI that represents the cryptographic algorithm that is used to encrypt the proof key.</returns>
        /// <exception cref="T:System.ArgumentException">An attempt is made to set the property to <see langword="null" /> or to an empty string.</exception>
        public string Algorithm
        {
            get
            {
                return this._algorithm;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof (value));
                this._algorithm = value;
            }
        }

        /// <summary>Gets or sets the encryption key material.</summary>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that contains the cryptographic key that is used to encrypt the proof key.</returns>
        /// <exception cref="T:System.ArgumentNullException">An attempt is made to set the property to <see langword="null" />.</exception>
        public SecurityKey SecurityKey
        {
            get
            {
                return this._key;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                this._key = value;
            }
        }

        /// <summary>Gets or sets the identifier that identifies the encrypting credential.</summary>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that identifies the key that is used to encrypt the proof key.</returns>
        /// <exception cref="T:System.ArgumentNullException">An attempt is made to set the property to <see langword="null" />.</exception>
        public SecurityKeyIdentifier SecurityKeyIdentifier
        {
            get
            {
                return this._keyIdentifier;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                this._keyIdentifier = value;
            }
        }
    }
}