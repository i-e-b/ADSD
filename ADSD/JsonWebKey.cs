using System.Collections.Generic;

namespace ADSD
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc7517#section-4
    /// </summary>
    public class JsonWebKey
    {
        /// <summary> Key algorithm type </summary>
        public string kty { get; set; }

        /// <summary> Key purpose </summary>
        public string use { get; set; }

        /// <summary> Key identifier </summary>
        public string kid { get; set; }

        /// <summary> X509 Thumbprint </summary>
        public string x5t { get; set; }
        
        /// <summary> RSA modulus </summary>
        public string n { get; set; }
        
        /// <summary> RSA exponent </summary>
        public string e { get; set; }
        
        /// <summary> X509 certificates </summary>
        public List<string> x5c { get; set; }
    }
}