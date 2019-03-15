using System;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD
{
    /// <summary>
    /// EncryptedReference
    /// </summary>
    public abstract class EncryptedReference
    {
        private string m_uri;
        private string m_referenceType;
        private TransformChain m_transformChain;
        internal XmlElement m_cachedXml;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptedReference" /> class.</summary>
        protected EncryptedReference()
            : this(string.Empty, new TransformChain())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptedReference" /> class using the specified Uniform Resource Identifier (URI).</summary>
        /// <param name="uri">The Uniform Resource Identifier (URI) that points to the data to encrypt.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="uri" /> parameter is <see langword="null" />.</exception>
        protected EncryptedReference(string uri)
            : this(uri, new TransformChain())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptedReference" /> class using the specified Uniform Resource Identifier (URI) and transform chain.</summary>
        /// <param name="uri">The Uniform Resource Identifier (URI) that points to the data to encrypt.</param>
        /// <param name="transformChain">A <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object that describes transforms to be done on the data to encrypt.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="uri" /> parameter is <see langword="null" />.</exception>
        protected EncryptedReference(string uri, TransformChain transformChain)
        {
            this.TransformChain = transformChain;
            this.Uri = uri;
            this.m_cachedXml = (XmlElement) null;
        }

        /// <summary>Gets or sets the Uniform Resource Identifier (URI) of an <see cref="T:System.Security.Cryptography.Xml.EncryptedReference" /> object.</summary>
        /// <returns>The Uniform Resource Identifier (URI) of the <see cref="T:System.Security.Cryptography.Xml.EncryptedReference" /> object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Security.Cryptography.Xml.EncryptedReference.Uri" /> property was set to <see langword="null" />.</exception>
        public string Uri
        {
            get
            {
                return this.m_uri;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Cryptography_Xml_UriRequired");
                this.m_uri = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the transform chain of an <see cref="T:System.Security.Cryptography.Xml.EncryptedReference" /> object.</summary>
        /// <returns>A <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object that describes transforms used on the encrypted data.</returns>
        public TransformChain TransformChain
        {
            get
            {
                if (this.m_transformChain == null)
                    this.m_transformChain = new TransformChain();
                return this.m_transformChain;
            }
            set
            {
                this.m_transformChain = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Adds a <see cref="T:System.Security.Cryptography.Xml.Transform" /> object to the current transform chain of an <see cref="T:System.Security.Cryptography.Xml.EncryptedReference" /> object.</summary>
        /// <param name="transform">A <see cref="T:System.Security.Cryptography.Xml.Transform" /> object to add to the transform chain.</param>
        public void AddTransform(Transform transform)
        {
            this.TransformChain.Add(transform);
        }

        /// <summary>Gets or sets a reference type.</summary>
        /// <returns>The reference type of the encrypted data.</returns>
        protected string ReferenceType
        {
            get
            {
                return this.m_referenceType;
            }
            set
            {
                this.m_referenceType = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets a value that indicates whether the cache is valid.</summary>
        /// <returns>
        /// <see langword="true" /> if the cache is valid; otherwise, <see langword="false" />.</returns>
        protected internal bool CacheValid
        {
            get
            {
                return this.m_cachedXml != null;
            }
        }

        /// <summary>Returns the XML representation of an <see cref="T:System.Security.Cryptography.Xml.EncryptedReference" /> object.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlElement" /> object that represents the values of the <see langword="&lt;EncryptedReference&gt;" /> element in XML encryption.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.EncryptedReference.ReferenceType" /> property is <see langword="null" />.</exception>
        public virtual XmlElement GetXml()
        {
            if (this.CacheValid)
                return this.m_cachedXml;
            return this.GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal XmlElement GetXml(XmlDocument document)
        {
            if (this.ReferenceType == null)
                throw new CryptographicException("Cryptography_Xml_ReferenceTypeRequired");
            XmlElement element = document.CreateElement(this.ReferenceType, "http://www.w3.org/2001/04/xmlenc#");
            if (!string.IsNullOrEmpty(this.m_uri))
                element.SetAttribute("URI", this.m_uri);
            if (this.TransformChain.Count > 0)
                element.AppendChild((XmlNode) this.TransformChain.GetXml(document, "http://www.w3.org/2000/09/xmldsig#"));
            return element;
        }

        /// <summary>Loads an XML element into an <see cref="T:System.Security.Cryptography.Xml.EncryptedReference" /> object.</summary>
        /// <param name="value">An <see cref="T:System.Xml.XmlElement" /> object that represents an XML element.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
        public virtual void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            this.ReferenceType = value.LocalName;
            this.Uri = Exml.GetAttribute(value, "URI", "http://www.w3.org/2001/04/xmlenc#");
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            XmlNode xmlNode = value.SelectSingleNode("ds:Transforms", nsmgr);
            if (xmlNode != null)
                this.TransformChain.LoadXml(xmlNode as XmlElement);
            this.m_cachedXml = value;
        }
    }
}