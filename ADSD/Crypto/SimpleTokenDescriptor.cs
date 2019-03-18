using System;
using System.Security.Claims;

namespace ADSD.Crypto
{
    /// <summary>
    /// 
    /// </summary>
    public class Lifetime
    {
        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime? Created;

        /// <summary>
        /// Expiry date
        /// </summary>
        public DateTime? Expires;
    }

    /// <summary>
    /// Minimal token descriptor
    /// </summary>
    public class SimpleTokenDescriptor
    {
        /// <summary>
        /// Lifetime
        /// </summary>
        public Lifetime Lifetime;

        /// <summary>
        /// TokenIssuerName
        /// </summary>
        public string TokenIssuerName;

        /// <summary>
        /// Target address
        /// </summary>
        public string AppliesToAddress;

        /// <summary>
        /// Subject
        /// </summary>
        public ClaimsIdentity Subject;

        /// <summary>
        /// Credentials
        /// </summary>
        public SigningCredentials SigningCredentials;
    }
}