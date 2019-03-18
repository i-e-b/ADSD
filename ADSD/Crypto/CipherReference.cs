using System;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>
    /// CipherReference
    /// </summary>
    public sealed class CipherReference : EncryptedReference
    {
        private byte[] m_cipherValue;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> class.</summary>
        public CipherReference()
        {
            ReferenceType = nameof (CipherReference);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> class using the specified Uniform Resource Identifier (URI).</summary>
        /// <param name="uri">A Uniform Resource Identifier (URI) pointing to the encrypted data.</param>
        public CipherReference(string uri)
            : base(uri)
        {
            ReferenceType = nameof (CipherReference);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> class using the specified Uniform Resource Identifier (URI) and transform chain information.</summary>
        /// <param name="uri">A Uniform Resource Identifier (URI) pointing to the encrypted data.</param>
        /// <param name="transformChain">A <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object that describes transforms to do on the encrypted data.</param>
        public CipherReference(string uri, TransformChain transformChain)
            : base(uri, transformChain)
        {
            ReferenceType = nameof (CipherReference);
        }

        internal byte[] CipherValue
        {
            get
            {
                if (!CacheValid)
                    return (byte[]) null;
                return m_cipherValue;
            }
            set
            {
                m_cipherValue = value;
            }
        }

        /// <summary>Returns the XML representation of a <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> object.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlElement" /> that represents the <see langword="&lt;CipherReference&gt;" /> element in XML encryption.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> value is <see langword="null" />.</exception>
        public override XmlElement GetXml()
        {
            if (CacheValid)
                return m_cachedXml;
            return GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal new XmlElement GetXml(XmlDocument document)
        {
            if (ReferenceType == null)
                throw new CryptographicException("Cryptography_Xml_ReferenceTypeRequired");
            XmlElement element = document.CreateElement(ReferenceType, "http://www.w3.org/2001/04/xmlenc#");
            if (!string.IsNullOrEmpty(Uri))
                element.SetAttribute("URI", Uri);
            if (TransformChain.Count > 0)
                element.AppendChild((XmlNode) TransformChain.GetXml(document, "http://www.w3.org/2001/04/xmlenc#"));
            return element;
        }

        /// <summary>Loads XML information into the <see langword="&lt;CipherReference&gt;" /> element in XML encryption.</summary>
        /// <param name="value">An <see cref="T:System.Xml.XmlElement" /> object that represents an XML element to use as the reference.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> provided is <see langword="null" />.</exception>
        public override void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            ReferenceType = value.LocalName;
            string attribute = Exml.GetAttribute(value, "URI", "http://www.w3.org/2001/04/xmlenc#");
            if (attribute == null)
                throw new CryptographicException("Cryptography_Xml_UriRequired");
            Uri = attribute;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsmgr.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
            XmlNode xmlNode = value.SelectSingleNode("enc:Transforms", nsmgr);
            if (xmlNode != null)
                TransformChain.LoadXml(xmlNode as XmlElement);
            m_cachedXml = value;
        }
    }
}