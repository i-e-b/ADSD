using System;
using System.Globalization;

namespace ADSD.Crypto
{
    /// <summary>Represents a key identifier clause that identifies an encrypted key.</summary>
    public sealed class EncryptedKeyIdentifierClause : BinaryKeyIdentifierClause
    {
        private readonly string carriedKeyName;
        private readonly string encryptionMethod;
        private readonly SecurityKeyIdentifier encryptingKeyIdentifier;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause" /> class using the specified key that is encrypted and the cryptographic algorithm used to encrypt the key.</summary>
        /// <param name="encryptedKey">An array of <see cref="T:System.Byte" /> that contains a key that is encrypted. Sets the value that is returned from the <see cref="M:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.GetEncryptedKey" /> method.</param>
        /// <param name="encryptionMethod">The cryptographic algorithm that is used to encrypt the key. Sets the value of the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.EncryptionMethod" /> property.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="encryptionMethod" /> is <see langword="null" />.-or-
        /// <paramref name="encryptedKey" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="encryptedKey" /> is zero length.</exception>
        public EncryptedKeyIdentifierClause(byte[] encryptedKey, string encryptionMethod)
            : this(encryptedKey, encryptionMethod, (SecurityKeyIdentifier) null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause" /> class using the specified key that is encrypted, the cryptographic algorithm used to encrypt the key, and a key identifier for the encrypting key.</summary>
        /// <param name="encryptedKey">An array of <see cref="T:System.Byte" /> that contains a key that is encrypted. Sets the value that is returned from the <see cref="M:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.GetEncryptedKey" /> method.</param>
        /// <param name="encryptionMethod">The cryptographic algorithm that is used to encrypt the key. Sets the value of the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.EncryptionMethod" /> property.</param>
        /// <param name="encryptingKeyIdentifier">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that represents the key identifier for the encrypting key that is specified in the <paramref name="encryptedKey" /> parameter. Sets the value of the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.EncryptingKeyIdentifier" /> property.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="encryptionMethod" /> is <see langword="null" />.-or-
        /// <paramref name="encryptedKey" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="encryptedKey" /> is zero length.</exception>
        public EncryptedKeyIdentifierClause(
            byte[] encryptedKey,
            string encryptionMethod,
            SecurityKeyIdentifier encryptingKeyIdentifier)
            : this(encryptedKey, encryptionMethod, encryptingKeyIdentifier, (string) null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause" /> class using the specified key that is encrypted, the cryptographic algorithm used to encrypt the key, a key identifier for the encrypting key and a user-readable name.</summary>
        /// <param name="encryptedKey">An array of <see cref="T:System.Byte" /> that contains a key that is encrypted. Sets the value that is returned from the <see cref="M:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.GetEncryptedKey" /> method.</param>
        /// <param name="encryptionMethod">The cryptographic algorithm that is used to encrypt the key. Sets the value of the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.EncryptionMethod" /> property.</param>
        /// <param name="encryptingKeyIdentifier">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that represents the key identifier for the encrypting key specified in the <paramref name="encryptedKey" /> parameter. Sets the value of the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.EncryptingKeyIdentifier" /> property.</param>
        /// <param name="carriedKeyName">A user-readable name that is associated with the key specified in the <paramref name="encryptedKey" /> parameter. Sets the value of the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.CarriedKeyName" /> property.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="encryptionMethod" /> is <see langword="null" />.-or-
        /// <paramref name="encryptedKey" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="encryptedKey" /> is zero length.</exception>
        public EncryptedKeyIdentifierClause(
            byte[] encryptedKey,
            string encryptionMethod,
            SecurityKeyIdentifier encryptingKeyIdentifier,
            string carriedKeyName)
            : this(encryptedKey, encryptionMethod, encryptingKeyIdentifier, carriedKeyName, true, (byte[]) null, 0)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause" /> class using the specified key that is encrypted, the cryptographic algorithm used to encrypt the key, a key identifier for the key and a user-readable name.</summary>
        /// <param name="encryptedKey">An array of <see cref="T:System.Byte" /> that contains a key that is encrypted. Sets the value that is returned from the <see cref="M:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.GetEncryptedKey" /> method.</param>
        /// <param name="encryptionMethod">The cryptographic algorithm that is used to encrypt the key. Sets the value of the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.EncryptionMethod" /> property.</param>
        /// <param name="encryptingKeyIdentifier">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that represents the key identifier for the key specified in the <paramref name="encryptedKey" /> parameter. Sets the value of the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.EncryptingKeyIdentifier" /> property.</param>
        /// <param name="carriedKeyName">A user-readable name that is associated with the key specified in the <paramref name="encryptedKey" /> parameter. Sets the value of the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.CarriedKeyName" /> property.</param>
        /// <param name="derivationNonce">An array of <see cref="T:System.Byte" /> that contains the nonce that was used to create a derived key. Sets the value that is returned by the <see cref="M:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.GetDerivationNonce" /> method.</param>
        /// <param name="derivationLength">The size of the derived key. Sets the value of the <see cref="P:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.DerivationLength" /> property.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="encryptionMethod" /> is <see langword="null" />.-or-
        /// <paramref name="encryptedKey" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="encryptedKey" /> is zero length.</exception>
        public EncryptedKeyIdentifierClause(
            byte[] encryptedKey,
            string encryptionMethod,
            SecurityKeyIdentifier encryptingKeyIdentifier,
            string carriedKeyName,
            byte[] derivationNonce,
            int derivationLength)
            : this(encryptedKey, encryptionMethod, encryptingKeyIdentifier, carriedKeyName, true, derivationNonce, derivationLength)
        {
        }

        internal EncryptedKeyIdentifierClause(
            byte[] encryptedKey,
            string encryptionMethod,
            SecurityKeyIdentifier encryptingKeyIdentifier,
            string carriedKeyName,
            bool cloneBuffer,
            byte[] derivationNonce,
            int derivationLength)
            : base("http://www.w3.org/2001/04/xmlenc#EncryptedKey", encryptedKey, cloneBuffer, derivationNonce, derivationLength)
        {
            if (encryptionMethod == null)
                throw new ArgumentNullException(nameof (encryptionMethod));
            this.carriedKeyName = carriedKeyName;
            this.encryptionMethod = encryptionMethod;
            this.encryptingKeyIdentifier = encryptingKeyIdentifier;
        }

        /// <summary>Gets a user-readable name that is associated with the encrypted key.</summary>
        /// <returns>A user-readable name that is associated with the encrypted key.</returns>
        public string CarriedKeyName
        {
            get
            {
                return this.carriedKeyName;
            }
        }

        /// <summary>Gets a key identifier for the encrypting key.</summary>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that represents the key identifier for the encrypting key.</returns>
        public SecurityKeyIdentifier EncryptingKeyIdentifier
        {
            get
            {
                return this.encryptingKeyIdentifier;
            }
        }

        /// <summary>Gets the cryptographic algorithm that is used to encrypt the key.</summary>
        /// <returns>The cryptographic algorithm that is used to encrypt the key.</returns>
        public string EncryptionMethod
        {
            get
            {
                return this.encryptionMethod;
            }
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance is equivalent to the specified key identifier clause.</summary>
        /// <param name="keyIdentifierClause">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to compare to.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="keyIdentifierClause" /> is of type <see cref="T:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause" /> and has the same encrypted key, encryption method and user-readable name as the current instance; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="keyIdentifierClause" /> is <see langword="null" />.</exception>
        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            EncryptedKeyIdentifierClause identifierClause = keyIdentifierClause as EncryptedKeyIdentifierClause;
            if (this == identifierClause)
                return true;
            if (identifierClause != null)
                return identifierClause.Matches(this.GetRawBuffer(), this.encryptionMethod, this.carriedKeyName);
            return false;
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance is equivalent to the specified encrypted key, encryption method and user-readable name.</summary>
        /// <param name="encryptedKey">An array of <see cref="T:System.Byte" /> that contains a key that is encrypted.</param>
        /// <param name="encryptionMethod">The cryptographic algorithm that is used to encrypt the key. </param>
        /// <param name="carriedKeyName">A user-readable name that is associated with the encrypted key.</param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="encryptedKey" />, <paramref name="encryptionMethod" /> and <paramref name="carriedKeyName" /> parameters have the same values returned by the <see cref="M:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.GetEncryptedKey" /> method and the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.EncryptionMethod" /> and the <see cref="P:System.IdentityModel.Tokens.EncryptedKeyIdentifierClause.CarriedKeyName" /> properties, respectively; otherwise, <see langword="false" />.</returns>
        public bool Matches(byte[] encryptedKey, string encryptionMethod, string carriedKeyName)
        {
            if (this.Matches(encryptedKey) && this.encryptionMethod == encryptionMethod)
                return this.carriedKeyName == carriedKeyName;
            return false;
        }

        /// <summary>Gets the encrypted key.</summary>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the encrypted key.</returns>
        public byte[] GetEncryptedKey()
        {
            return this.GetBuffer();
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>The current object.</returns>
        public override string ToString()
        {
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "EncryptedKeyIdentifierClause(EncryptedKey = {0}, Method '{1}')", new object[2]
            {
                (object) Convert.ToBase64String(this.GetRawBuffer()),
                (object) this.EncryptionMethod
            });
        }
    }
}