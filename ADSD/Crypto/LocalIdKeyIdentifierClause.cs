using System;
using System.Globalization;

namespace ADSD.Crypto
{
    /// <summary>Represents a key identifier clause that identifies a security tokens specified in the security header of the SOAP message.</summary>
    public class LocalIdKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        private readonly string localId;
        private readonly Type[] ownerTypes;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause" /> class using the specified identifier and array of types. </summary>
        /// <param name="localId">The value of the <see langword="wsu:Id" /> attribute for an XML element within the current SOAP message. Sets the value of the <see cref="P:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause.LocalId" /> property.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="localId" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="localId" /> is empty.</exception>
        public LocalIdKeyIdentifierClause(string localId)
            : this(localId, (Type[]) null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause" /> class using the specified identifier, nonce, derived key length an owner security token type.</summary>
        /// <param name="localId">The value of the <see langword="wsu:Id" /> attribute for an XML element within the current SOAP message. Sets the value of the <see cref="P:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause.LocalId" /> property.</param>
        /// <param name="derivationNonce">An array of <see cref="T:System.Byte" /> that contains the nonce that was used to create a derived key. Sets the value that is returned by the <see cref="M:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.GetDerivationNonce" /> method.</param>
        /// <param name="derivationLength">The size of the derived key. Sets the value of the <see cref="P:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.DerivationLength" /> property.</param>
        /// <param name="ownerType">A <see cref="T:System.Type" /> that is the type of security token that is referred to by the <paramref name="localId" /> parameter. Sets the value of the <see cref="P:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause.OwnerType" /> property.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="localId" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="localId" /> is empty.</exception>
        public LocalIdKeyIdentifierClause(
            string localId,
            byte[] derivationNonce,
            int derivationLength,
            Type ownerType):base(localId)
        {
            byte[] derivationNonce1 = derivationNonce;
            int derivationLength1 = derivationLength;
            if (!(ownerType == (Type) null))
                this.ownerTypes = new Type[1]{ ownerType };
            else
                this.ownerTypes = (Type[]) null;

            UpdateBytes(derivationNonce1, derivationLength1);
        }

        internal LocalIdKeyIdentifierClause(string localId, Type[] ownerTypes)
            : this(localId, (byte[]) null, 0, ownerTypes)
        {
        }

        internal LocalIdKeyIdentifierClause(
            string localId,
            byte[] derivationNonce,
            int derivationLength,
            Type[] ownerTypes)
            : base((string) null, derivationNonce, derivationLength)
        {
            if (localId == null) throw new ArgumentNullException(nameof(localId));
            if (localId == string.Empty) throw new ArgumentException("LocalIdCannotBeEmpty");

            this.localId = localId;
            this.ownerTypes = ownerTypes;
        }

        /// <summary>Gets the value of the <see langword="wsu:Id" /> attribute for an XML element within the current SOAP message.</summary>
        /// <returns>The value of the <see langword="wsu:Id" /> attribute for an XML element within the current SOAP message.</returns>
        public string LocalId
        {
            get
            {
                return this.localId;
            }
        }

        /// <summary>Gets the type of security token that is referred to by the <see cref="P:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause.LocalId" /> property.</summary>
        /// <returns>A <see cref="T:System.Type" /> that contains the type of security token that is referred to by the <see cref="P:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause.LocalId" /> property.</returns>
        public Type OwnerType
        {
            get
            {
                if (this.ownerTypes != null && this.ownerTypes.Length != 0)
                    return this.ownerTypes[0];
                return (Type) null;
            }
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance is equivalent to the specified key identifier clause.</summary>
        /// <param name="keyIdentifierClause">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to compare to.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="keyIdentifierClause" /> is of type <see cref="T:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause" /> and the values of the <see cref="P:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause.LocalId" /> and <see cref="P:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause.OwnerType" /> properties match the current instance; otherwise, <see langword="false" />. See the remarks for more details.</returns>
        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            LocalIdKeyIdentifierClause identifierClause = keyIdentifierClause as LocalIdKeyIdentifierClause;
            if (this == identifierClause)
                return true;
            if (identifierClause != null)
                return identifierClause.Matches(this.localId, this.OwnerType);
            return false;
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance is equivalent to the specified reference and type.</summary>
        /// <param name="localId">The value of the <see langword="wsu:Id" /> attribute for an XML element within the current SOAP message. </param>
        /// <param name="ownerType">A <see cref="T:System.Type" /> that is the type of security token that is referred to by the <paramref name="localId" /> parameter. </param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="localId" /> and <paramref name="ownerType" /> parameters match the values of the <see cref="P:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause.LocalId" /> and <see cref="P:System.IdentityModel.Tokens.LocalIdKeyIdentifierClause.OwnerType" /> properties; otherwise, <see langword="false" />. See the remarks for more details.</returns>
        public bool Matches(string localId, Type ownerType)
        {
            if (string.IsNullOrEmpty(localId) || this.localId != localId)
                return false;
            if (this.ownerTypes == null || ownerType == (Type) null)
                return true;
            for (int index = 0; index < this.ownerTypes.Length; ++index)
            {
                if (this.ownerTypes[index] == (Type) null || this.ownerTypes[index] == ownerType)
                    return true;
            }
            return false;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A <see cref="T:System.String" /> that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "LocalIdKeyIdentifierClause(LocalId = '{0}', Owner = '{1}')", new object[2]
            {
                (object) this.LocalId,
                (object) this.OwnerType
            });
        }
    }
}