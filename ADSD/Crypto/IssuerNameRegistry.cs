namespace ADSD
{
    /// <summary>The abstract base class for an issuer name registry. An issuer name registry is used to associate a mnemonic name to the cryptographic material that is needed to verify the signatures of tokens produced by the corresponding issuer. The issuer name registry maintains a list of issuers that are trusted by a relying party (RP) application.</summary>
    public abstract class IssuerNameRegistry
    {
        /// <summary>When overridden in a derived class, returns the name of the issuer of the specified security token.</summary>
        /// <param name="securityToken">The security token for which to return the issuer name.</param>
        /// <returns>The issuer name.</returns>
        public abstract string GetIssuerName(SecurityToken securityToken);

        /// <summary>When overridden in a derived class, returns the name of the issuer of the specified security token. The specified issuer name may be considered in determining the issuer name to return.</summary>
        /// <param name="securityToken">The security token for which to return the issuer name.</param>
        /// <param name="requestedIssuerName">An issuer name to consider in the request.</param>
        /// <returns>The issuer name.</returns>
        public virtual string GetIssuerName(SecurityToken securityToken, string requestedIssuerName)
        {
            return this.GetIssuerName(securityToken);
        }

        /// <summary>Returns the default issuer name to be used for Windows claims.</summary>
        /// <returns>The default issuer name for Windows claims.</returns>
        public virtual string GetWindowsIssuerName()
        {
            return "LOCAL AUTHORITY";
        }

    }
}