using System;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD
{
    /// <summary>
    /// CipherData
    /// </summary>
    public class CipherData
    {
        private XmlElement m_cachedXml;
        private CipherReference m_cipherReference;
        private byte[] m_cipherValue;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> class.</summary>
        public CipherData()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> class using a byte array as the <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> value.</summary>
        /// <param name="cipherValue">The encrypted data to use for the <see langword="&lt;CipherValue&gt;" /> element.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="cipherValue" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> property has already been set.</exception>
        public CipherData(byte[] cipherValue)
        {
            this.CipherValue = cipherValue;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> class using a <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> object.</summary>
        /// <param name="cipherReference">The <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> object to use.</param>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> property has already been set.</exception>
        public CipherData(CipherReference cipherReference)
        {
            this.CipherReference = cipherReference;
        }

        private bool CacheValid
        {
            get
            {
                return this.m_cachedXml != null;
            }
        }

        /// <summary>Gets or sets the <see langword="&lt;CipherReference&gt;" /> element.</summary>
        /// <returns>A <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherReference" />  property was set to <see langword="null" />.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherReference" />  property was set more than once.</exception>
        public CipherReference CipherReference
        {
            get
            {
                return this.m_cipherReference;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                if (this.CipherValue != null)
                    throw new CryptographicException("Cryptography_Xml_CipherValueElementRequired");
                this.m_cipherReference = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the <see langword="&lt;CipherValue&gt;" /> element.</summary>
        /// <returns>A byte array that represents the <see langword="&lt;CipherValue&gt;" /> element.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" />  property was set to <see langword="null" />.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" />  property was set more than once.</exception>
        public byte[] CipherValue
        {
            get
            {
                return this.m_cipherValue;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                if (this.CipherReference != null)
                    throw new CryptographicException("Cryptography_Xml_CipherValueElementRequired");
                this.m_cipherValue = (byte[]) value.Clone();
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets the XML values for the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> object.</summary>
        /// <returns>A <see cref="T:System.Xml.XmlElement" /> object that represents the XML information for the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> object.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> property and the <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherReference" /> property are <see langword="null" />.</exception>
        public XmlElement GetXml()
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
            XmlElement element1 = document.CreateElement(nameof (CipherData), "http://www.w3.org/2001/04/xmlenc#");
            if (this.CipherValue != null)
            {
                XmlElement element2 = document.CreateElement("CipherValue", "http://www.w3.org/2001/04/xmlenc#");
                element2.AppendChild((XmlNode) document.CreateTextNode(Convert.ToBase64String(this.CipherValue)));
                element1.AppendChild((XmlNode) element2);
            }
            else
            {
                if (this.CipherReference == null)
                    throw new CryptographicException("Cryptography_Xml_CipherValueElementRequired");
                element1.AppendChild((XmlNode) this.CipherReference.GetXml(document));
            }
            return element1;
        }

        /// <summary>Loads XML data from an <see cref="T:System.Xml.XmlElement" /> into a <see cref="T:System.Security.Cryptography.Xml.CipherData" /> object.</summary>
        /// <param name="value">An <see cref="T:System.Xml.XmlElement" /> that represents the XML data to load.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> property and the <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherReference" /> property are <see langword="null" />.</exception>
        public void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsmgr.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
            XmlNode xmlNode1 = value.SelectSingleNode("enc:CipherValue", nsmgr);
            XmlNode xmlNode2 = value.SelectSingleNode("enc:CipherReference", nsmgr);
            if (xmlNode1 != null)
            {
                if (xmlNode2 != null)
                    throw new CryptographicException("Cryptography_Xml_CipherValueElementRequired");
                this.m_cipherValue = Convert.FromBase64String(Exml.DiscardWhiteSpaces(xmlNode1.InnerText));
            }
            else
            {
                if (xmlNode2 == null)
                    throw new CryptographicException("Cryptography_Xml_CipherValueElementRequired");
                this.m_cipherReference = new CipherReference();
                this.m_cipherReference.LoadXml((XmlElement) xmlNode2);
            }
            this.m_cachedXml = value;
        }
    }
}