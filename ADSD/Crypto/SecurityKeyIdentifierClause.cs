using System;

namespace ADSD.Crypto
{
    /// <summary>Represents an abstract base class for a key identifier clause.</summary>
    public abstract class SecurityKeyIdentifierClause
    {
        private readonly string clauseType;
        private byte[] derivationNonce;
        private int derivationLength;
        private string id;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> class using the specified key identifier clause type. </summary>
        /// <param name="clauseType">The key identifier clause type. Sets the value of the <see cref="P:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.ClauseType" /> property.</param>
        protected SecurityKeyIdentifierClause(string clauseType)
            : this(clauseType, (byte[]) null, 0)
        {
        }

        /// <summary>
        /// Reset stored data
        /// </summary>
        protected void UpdateBytes(byte[] nonce, int length) {
            this.derivationNonce = nonce;
            this.derivationLength = length;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> class using the specified key identifier clause type, nonce, and the derived key length. </summary>
        /// <param name="clauseType">The key identifier clause type. Sets the value of the <see cref="P:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.ClauseType" /> property.</param>
        /// <param name="nonce">An array of <see cref="T:System.Byte" /> that contains the nonce that was used to create a derived key. Sets the value that is returned by the <see cref="M:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.GetDerivationNonce" /> method.</param>
        /// <param name="length">The size of the derived key. Sets the value of the <see cref="P:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.DerivationLength" /> property.</param>
        protected SecurityKeyIdentifierClause(string clauseType, byte[] nonce, int length)
        {
            this.clauseType = clauseType;
            this.derivationNonce = nonce;
            this.derivationLength = length;
        }

        /// <summary>Gets a value that indicates whether a key can be created. </summary>
        /// <returns>
        /// <see langword="true" /> if a key can be created; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
        public virtual bool CanCreateKey
        {
            get
            {
                return false;
            }
        }

        /// <summary>Gets the key identifier clause type.</summary>
        /// <returns>The key identifier clause type.</returns>
        public string ClauseType
        {
            get
            {
                return this.clauseType;
            }
        }

        /// <summary>Gets or sets the key identifier clause ID.</summary>
        /// <returns>The key identifier clause ID. The default is <see langword="null" />.</returns>
        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>Creates a key based on the parameters passed into the constructor.</summary>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that contains the created key.</returns>
        public virtual SecurityKey CreateKey()
        {
            throw new NotSupportedException("KeyIdentifierClauseDoesNotSupportKeyCreation");
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance is equivalent to the specified key identifier clause.</summary>
        /// <param name="keyIdentifierClause">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to compare to.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="keyIdentifierClause" /> is the same instance as the current instance; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="keyIdentifierClause" /> is <see langword="null" />.</exception>
        public virtual bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            return this == keyIdentifierClause;
        }

        /// <summary>Gets the nonce that was used to generate the derived key.</summary>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the nonce that was used to generate the derived key.</returns>
        public byte[] GetDerivationNonce()
        {
            if (this.derivationNonce == null)
                return (byte[]) null;
            return (byte[]) this.derivationNonce.Clone();
        }

        /// <summary>Gets the size of the derived key.</summary>
        /// <returns>The size of the derived key.</returns>
        public int DerivationLength
        {
            get
            {
                return this.derivationLength;
            }
        }
    }
}