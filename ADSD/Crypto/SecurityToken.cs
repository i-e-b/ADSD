using System;
using System.Collections.ObjectModel;

namespace ADSD
{
    /// <summary>Represents a base class used to implement all security tokens.</summary>
    public abstract class SecurityToken
    {
        /// <summary>Gets a unique identifier of the security token.</summary>
        /// <returns>The unique identifier of the security token.</returns>
        public abstract string Id { get; }

        /// <summary>Gets the cryptographic keys associated with the security token.</summary>
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of type <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that contains the set of keys associated with the security token.</returns>
        public abstract ReadOnlyCollection<SecurityKey> SecurityKeys { get; }

        /// <summary>Gets the first instant in time at which this security token is valid.</summary>
        /// <returns>A <see cref="T:System.DateTime" /> that represents the instant in time at which this security token is first valid.</returns>
        public abstract DateTime ValidFrom { get; }

        /// <summary>Gets the last instant in time at which this security token is valid.</summary>
        /// <returns>A <see cref="T:System.DateTime" /> that represents the last instant in time at which this security token is valid.</returns>
        public abstract DateTime ValidTo { get; }

        /// <summary>Gets a value that indicates whether this security token is capable of creating the specified key identifier. </summary>
        public virtual bool CanCreateKeyIdentifierClause<T>() where T : SecurityKeyIdentifierClause
        {
            if (typeof (T) == typeof (LocalIdKeyIdentifierClause))
                return this.CanCreateLocalKeyIdentifierClause();
            return false;
        }

        /// <summary>Creates the specified key identifier clause.</summary>
        /// <typeparam name="T">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that specifies the key identifier to create.</typeparam>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that is a key identifier clause for the security token.</returns>
        public virtual T CreateKeyIdentifierClause<T>() where T : SecurityKeyIdentifierClause
        {
            if (typeof (T) == typeof (LocalIdKeyIdentifierClause) && this.CanCreateLocalKeyIdentifierClause())
                return new LocalIdKeyIdentifierClause(this.Id, new[] { this.GetType() }) as T;
            throw new NotSupportedException("TokenDoesNotSupportKeyIdentifierClauseCreation");
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance can be resolved to the specified key identifier.</summary>
        /// <param name="keyIdentifierClause">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to compare to this instance.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="keyIdentifierClause" /> is a <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> and it has the same unique identifier as the <see cref="P:System.IdentityModel.Tokens.SecurityToken.Id" /> property; otherwise, <see langword="false" />.</returns>
        public virtual bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            LocalIdKeyIdentifierClause identifierClause = keyIdentifierClause as LocalIdKeyIdentifierClause;
            if (identifierClause != null)
                return identifierClause.Matches(this.Id, this.GetType());
            return false;
        }

        /// <summary>Gets the key for the specified key identifier clause.</summary>
        /// <param name="keyIdentifierClause">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to get the key for.</param>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that represents the key.</returns>
        public virtual SecurityKey ResolveKeyIdentifierClause(
            SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (this.SecurityKeys.Count != 0 && this.MatchesKeyIdentifierClause(keyIdentifierClause))
                return this.SecurityKeys[0];
            return (SecurityKey) null;
        }

        private bool CanCreateLocalKeyIdentifierClause()
        {
            return this.Id != null;
        }
    }
}