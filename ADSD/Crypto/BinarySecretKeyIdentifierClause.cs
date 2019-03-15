using System.Runtime.CompilerServices;

namespace ADSD
{
    /// <summary>Represents the key identifier clause in a binary secret security token.</summary>
    [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class BinarySecretKeyIdentifierClause : BinaryKeyIdentifierClause
    {
        private InMemorySymmetricSecurityKey symmetricKey;

        /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.BinarySecretKeyIdentifierClause" /> class using the specified key.</summary>
        /// <param name="key">A <see cref="T:System.Byte" /> array that represents the key.</param>
        public BinarySecretKeyIdentifierClause(byte[] key)
            : this(key, true)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.BinarySecretKeyIdentifierClause" /> class using the specified values.</summary>
        /// <param name="key">A <see cref="T:System.Byte" /> array that represents the key.</param>
        /// <param name="cloneBuffer">
        /// <see langword="true" /> to clone the buffer; otherwise, <see langword="false" />.</param>
        public BinarySecretKeyIdentifierClause(byte[] key, bool cloneBuffer)
            : this(key, cloneBuffer, (byte[]) null, 0)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.BinarySecretKeyIdentifierClause" /> class using the specified values.</summary>
        /// <param name="key">A <see cref="T:System.Byte" /> array that represents the key.</param>
        /// <param name="cloneBuffer">
        /// <see langword="true" /> to clone the buffer; otherwise, <see langword="false" />.</param>
        /// <param name="derivationNonce">The "number used once" (nonce) used to derive the key.</param>
        /// <param name="derivationLength">The length of the key to be derived.</param>
        public BinarySecretKeyIdentifierClause(
            byte[] key,
            bool cloneBuffer,
            byte[] derivationNonce,
            int derivationLength)
            : base(new TrustFeb2005Dictionary(new IdentityModelDictionary(new IdentityModelStringsVersion1())).BinarySecretClauseType.Value, key, cloneBuffer, derivationNonce, derivationLength)
        {
        }

        /// <summary>Gets an array of bytes that represents the key.</summary>
        /// <returns>An array of bytes that represents the key.</returns>
        public byte[] GetKeyBytes()
        {
            return this.GetBuffer();
        }

        /// <summary>Gets a value that indicates whether this instance of the class can create a security key.</summary>
        /// <returns>Always <see langword="true" />.</returns>
        public override bool CanCreateKey
        {
            get
            {
                return true;
            }
        }

        /// <summary>Creates a security key.</summary>
        /// <returns>The newly created security key.</returns>
        public override SecurityKey CreateKey()
        {
            if (this.symmetricKey == null)
                this.symmetricKey = new InMemorySymmetricSecurityKey(this.GetBuffer(), false);
            return (SecurityKey) this.symmetricKey;
        }

        /// <summary>Compares whether the key of a specified clause matches this instance's key.</summary>
        /// <param name="keyIdentifierClause">The <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to match.</param>
        /// <returns>
        /// <see langword="true" /> if there is a match; otherwise, <see langword="false" />.</returns>
        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            BinarySecretKeyIdentifierClause identifierClause = keyIdentifierClause as BinarySecretKeyIdentifierClause;
            if (this == identifierClause)
                return true;
            if (identifierClause != null)
                return identifierClause.Matches(this.GetRawBuffer());
            return false;
        }
    }
}