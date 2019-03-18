namespace ADSD.Crypto
{
    /// <summary>Represents the <see langword="&lt;KeyReference&gt;" /> element used in XML encryption. This class cannot be inherited.</summary>
    public sealed class KeyReference : EncryptedReference
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> class for XML encryption.</summary>
        public KeyReference()
        {
            ReferenceType = nameof (KeyReference);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> class for XML encryption using the supplied Uniform Resource Identifier (URI).</summary>
        /// <param name="uri">A Uniform Resource Identifier (URI) that points to the encrypted key.</param>
        public KeyReference(string uri)
            : base(uri)
        {
            ReferenceType = nameof (KeyReference);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> class for XML encryption using the specified Uniform Resource Identifier (URI) and a <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object.</summary>
        /// <param name="uri">A Uniform Resource Identifier (URI) that points to the encrypted key.</param>
        /// <param name="transformChain">A <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object that describes transforms to do on the encrypted key.</param>
        public KeyReference(string uri, TransformChain transformChain)
            : base(uri, transformChain)
        {
            ReferenceType = nameof (KeyReference);
        }
    }
}