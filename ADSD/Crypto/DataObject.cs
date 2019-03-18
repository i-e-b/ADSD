using System;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>Represents the object element of an XML signature that holds data to be signed.</summary>
    public class DataObject
    {
        private string m_id;
        private string m_mimeType;
        private string m_encoding;
        private CanonicalXmlNodeList m_elData;
        private XmlElement m_cachedXml;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.DataObject" /> class.</summary>
        public DataObject()
        {
            m_cachedXml = (XmlElement) null;
            m_elData = new CanonicalXmlNodeList();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.DataObject" /> class with the specified identification, MIME type, encoding, and data.</summary>
        /// <param name="id">The identification to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.DataObject" /> with. </param>
        /// <param name="mimeType">The MIME type of the data used to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.DataObject" />. </param>
        /// <param name="encoding">The encoding of the data used to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.DataObject" />. </param>
        /// <param name="data">The data to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.DataObject" /> with. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="data" /> parameter is <see langword="null" />. </exception>
        public DataObject(string id, string mimeType, string encoding, XmlElement data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof (data));
            m_id = id;
            m_mimeType = mimeType;
            m_encoding = encoding;
            m_elData = new CanonicalXmlNodeList();
            m_elData.Add((object) data);
            m_cachedXml = (XmlElement) null;
        }

        /// <summary>Gets or sets the identification of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</summary>
        /// <returns>The name of the element that contains data to be used. </returns>
        public string Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
                m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the MIME type of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object. </summary>
        /// <returns>The MIME type of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object. The default is <see langword="null" />.</returns>
        public string MimeType
        {
            get
            {
                return m_mimeType;
            }
            set
            {
                m_mimeType = value;
                m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the encoding of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</summary>
        /// <returns>The type of encoding of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</returns>
        public string Encoding
        {
            get
            {
                return m_encoding;
            }
            set
            {
                m_encoding = value;
                m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the data value of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</summary>
        /// <returns>The data of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The value used to set the property is <see langword="null" />.</exception>
        public XmlNodeList Data
        {
            get
            {
                return (XmlNodeList) m_elData;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                m_elData = new CanonicalXmlNodeList();
                foreach (XmlNode xmlNode in value)
                    m_elData.Add((object) xmlNode);
                m_cachedXml = (XmlElement) null;
            }
        }

        private bool CacheValid
        {
            get
            {
                return m_cachedXml != null;
            }
        }

        /// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</summary>
        /// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</returns>
        public XmlElement GetXml()
        {
            if (CacheValid)
                return m_cachedXml;
            return GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal XmlElement GetXml(XmlDocument document)
        {
            XmlElement element = document.CreateElement("Object", "http://www.w3.org/2000/09/xmldsig#");
            if (!string.IsNullOrEmpty(m_id))
                element.SetAttribute("Id", m_id);
            if (!string.IsNullOrEmpty(m_mimeType))
                element.SetAttribute("MimeType", m_mimeType);
            if (!string.IsNullOrEmpty(m_encoding))
                element.SetAttribute("Encoding", m_encoding);
            if (m_elData != null)
            {
                foreach (XmlNode node in (XmlNodeList) m_elData)
                    element.AppendChild(document.ImportNode(node, true));
            }
            return element;
        }

        /// <summary>Loads a <see cref="T:System.Security.Cryptography.Xml.DataObject" /> state from an XML element.</summary>
        /// <param name="value">The XML element to load the <see cref="T:System.Security.Cryptography.Xml.DataObject" /> state from. </param>
        /// <exception cref="T:System.ArgumentNullException">The value from the XML element is <see langword="null" />.</exception>
        public void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            m_id = Exml.GetAttribute(value, "Id", "http://www.w3.org/2000/09/xmldsig#");
            m_mimeType = Exml.GetAttribute(value, "MimeType", "http://www.w3.org/2000/09/xmldsig#");
            m_encoding = Exml.GetAttribute(value, "Encoding", "http://www.w3.org/2000/09/xmldsig#");
            foreach (XmlNode childNode in value.ChildNodes)
                m_elData.Add((object) childNode);
            m_cachedXml = value;
        }
    }
}