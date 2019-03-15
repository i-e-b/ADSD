﻿namespace ADSD
{
    /// <summary>Represents the <see langword="&lt;DataReference&gt;" /> element used in XML encryption. This class cannot be inherited.</summary>
    public sealed class DataReference : EncryptedReference
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.DataReference" /> class.</summary>
        public DataReference()
        {
            this.ReferenceType = nameof (DataReference);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.DataReference" /> class using the specified Uniform Resource Identifier (URI).</summary>
        /// <param name="uri">A Uniform Resource Identifier (URI) that points to the encrypted data.</param>
        public DataReference(string uri)
            : base(uri)
        {
            this.ReferenceType = nameof (DataReference);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.DataReference" /> class using the specified Uniform Resource Identifier (URI) and a <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object.</summary>
        /// <param name="uri">A Uniform Resource Identifier (URI) that points to the encrypted data.</param>
        /// <param name="transformChain">A <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object that describes transforms to do on the encrypted data.</param>
        public DataReference(string uri, TransformChain transformChain)
            : base(uri, transformChain)
        {
            this.ReferenceType = nameof (DataReference);
        }
    }
}