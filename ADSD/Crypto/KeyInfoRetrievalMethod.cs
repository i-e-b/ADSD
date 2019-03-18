using System;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>References <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> objects stored at a different location when using XMLDSIG or XML encryption.</summary>
    public class KeyInfoRetrievalMethod : KeyInfoClause
    {
        private string m_uri;
        private string m_type;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" /> class.</summary>
        public KeyInfoRetrievalMethod()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" /> class with the specified Uniform Resource Identifier (URI) pointing to the referenced <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object.</summary>
        /// <param name="strUri">The Uniform Resource Identifier (URI) of the information to be referenced by the new instance of <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" />. </param>
        public KeyInfoRetrievalMethod(string strUri)
        {
            this.m_uri = strUri;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" /> class with the specified Uniform Resource Identifier (URI) pointing to the referenced <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object and the URI that describes the type of data to retrieve.  </summary>
        /// <param name="strUri">The Uniform Resource Identifier (URI) of the information to be referenced by the new instance of <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" />.</param>
        /// <param name="typeName">The URI that describes the type of data to retrieve.</param>
        public KeyInfoRetrievalMethod(string strUri, string typeName)
        {
            this.m_uri = strUri;
            this.m_type = typeName;
        }

        /// <summary>Gets or sets the Uniform Resource Identifier (URI) of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" /> object.</summary>
        /// <returns>The Uniform Resource Identifier (URI) of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" /> object.</returns>
        public string Uri
        {
            get
            {
                return this.m_uri;
            }
            set
            {
                this.m_uri = value;
            }
        }

        /// <summary>Gets or sets a Uniform Resource Identifier (URI) that describes the type of data to be retrieved.</summary>
        /// <returns>A Uniform Resource Identifier (URI) that describes the type of data to be retrieved.</returns>
        public string Type
        {
            get
            {
                return this.m_type;
            }
            set
            {
                this.m_type = value;
            }
        }

        /// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" /> object.</summary>
        /// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" /> object.</returns>
        public override XmlElement GetXml()
        {
            return this.GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal override XmlElement GetXml(XmlDocument xmlDocument)
        {
            XmlElement element = xmlDocument.CreateElement("RetrievalMethod", "http://www.w3.org/2000/09/xmldsig#");
            if (!string.IsNullOrEmpty(this.m_uri))
                element.SetAttribute("URI", this.m_uri);
            if (!string.IsNullOrEmpty(this.m_type))
                element.SetAttribute("Type", this.m_type);
            return element;
        }

        /// <summary>Parses the input <see cref="T:System.Xml.XmlElement" /> object and configures the internal state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" /> object to match.</summary>
        /// <param name="value">The XML element that specifies the state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoRetrievalMethod" /> object. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />. </exception>
        public override void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            this.m_uri = Exml.GetAttribute(value, "URI", "http://www.w3.org/2000/09/xmldsig#");
            this.m_type = Exml.GetAttribute(value, "Type", "http://www.w3.org/2000/09/xmldsig#");
        }
    }
}