using System.Collections.Generic;

namespace ADSD
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc7517
    /// </summary>
    public class JwkSet
    {
        /// <summary>
        /// List of known keys
        /// </summary>
        public List<JsonWebKey> keys { get; set; }
    }
}