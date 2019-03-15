using System;

namespace ADSD
{
    /// <summary>
    /// A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that can be used to match <see cref="T:System.IdentityModel.Tokens.NamedKeySecurityToken" />.
    /// </summary>
    public class NamedKeySecurityKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        private const string NameKeySecurityKeyIdentifierClauseType = "NamedKeySecurityKeyIdentifierClause";
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.NamedKeySecurityKeyIdentifierClause" /> class. The 'name' for matching key identifiers found in the securityToken.
        /// </summary>
        /// <param name="name">Used to identify a named collection of keys.</param>
        /// <param name="id">Additional information for matching.</param>
        /// <exception cref="T:System.ArgumentNullException">if 'name' is null or whitespace.</exception>
        /// <exception cref="T:System.ArgumentNullException">if 'id' is null or whitespace</exception>
        public NamedKeySecurityKeyIdentifierClause(string name, string id)
            : base(nameof (NamedKeySecurityKeyIdentifierClause))
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof (name));
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof (id));
            this.name = name;
            this.Id = id;
        }

        /// <summary>
        /// Gets the name of the <see cref="T:System.IdentityModel.Tokens.SecurityKey" />(s) this <see cref="T:System.IdentityModel.Tokens.NamedKeySecurityKeyIdentifierClause" /> represents.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Determines if a <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> matches this instance.
        /// </summary>
        /// <param name="keyIdentifierClause">The <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to match.</param>
        /// <returns>true if:
        /// <para>    1. keyIdentifierClause is a <see cref="T:System.IdentityModel.Tokens.NamedKeySecurityKeyIdentifierClause" />.</para>
        /// <para>    2. string.Equals( keyIdentifierClause.Name, this.Name, StringComparison.Ordinal).</para>
        /// <para>    2. string.Equals( keyIdentifierClause.Id, this.Id, StringComparison.Ordinal).</para>
        /// <para>Otherwise calls base.Matches( keyIdentifierClause ).</para>
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">'keyIdentifierClause' is null.</exception>
        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
                throw new ArgumentNullException(nameof (keyIdentifierClause));
            NamedKeySecurityKeyIdentifierClause identifierClause = keyIdentifierClause as NamedKeySecurityKeyIdentifierClause;
            if (identifierClause != null && string.Equals(identifierClause.Name, this.Name, StringComparison.Ordinal) && string.Equals(identifierClause.Id, this.Id, StringComparison.Ordinal))
                return true;
            return base.Matches(keyIdentifierClause);
        }
    }
}