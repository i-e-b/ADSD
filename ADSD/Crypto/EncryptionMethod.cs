using System;
using System.Xml;

namespace ADSD
{
    /// <summary>
    /// EncryptionMethod
    /// </summary>
    public class EncryptionMethod
    {
        private XmlElement m_cachedXml;
        private int m_keySize;
        private string m_algorithm;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> class. </summary>
        public EncryptionMethod()
        {
            this.m_cachedXml = (XmlElement) null;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> class specifying an algorithm Uniform Resource Identifier (URI). </summary>
        /// <param name="algorithm">The Uniform Resource Identifier (URI) that describes the algorithm represented by an instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> class. </param>
        public EncryptionMethod(string algorithm)
        {
            this.m_algorithm = algorithm;
            this.m_cachedXml = (XmlElement) null;
        }

        private bool CacheValid
        {
            get
            {
                return this.m_cachedXml != null;
            }
        }

        /// <summary>Gets or sets the algorithm key size used for XML encryption. </summary>
        /// <returns>The algorithm key size, in bits, used for XML encryption.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="P:System.Security.Cryptography.Xml.EncryptionMethod.KeySize" /> property was set to a value that was less than 0.</exception>
        public int KeySize
        {
            get
            {
                return this.m_keySize;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(KeySize), "Cryptography error: Invalid key size (7)");
                this.m_keySize = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets a Uniform Resource Identifier (URI) that describes the algorithm to use for XML encryption. </summary>
        /// <returns>A Uniform Resource Identifier (URI) that describes the algorithm to use for XML encryption.</returns>
        public string KeyAlgorithm
        {
            get
            {
                return this.m_algorithm;
            }
            set
            {
                this.m_algorithm = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Returns an <see cref="T:System.Xml.XmlElement" /> object that encapsulates an instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> class.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlElement" /> object that encapsulates an instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> class.</returns>
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
            XmlElement element1 = document.CreateElement(nameof (EncryptionMethod), "http://www.w3.org/2001/04/xmlenc#");
            if (!string.IsNullOrEmpty(this.m_algorithm))
                element1.SetAttribute("Algorithm", this.m_algorithm);
            if (this.m_keySize > 0)
            {
                XmlElement element2 = document.CreateElement("KeySize", "http://www.w3.org/2001/04/xmlenc#");
                element2.AppendChild((XmlNode) document.CreateTextNode(this.m_keySize.ToString((string) null, (IFormatProvider) null)));
                element1.AppendChild((XmlNode) element2);
            }
            return element1;
        }
        
        internal static string GetAttribute(XmlElement element, string localName, string namespaceURI)
        {
            string str = element.HasAttribute(localName) ? element.GetAttribute(localName) : (string) null;
            if (str == null && element.HasAttribute(localName, namespaceURI))
                str = element.GetAttribute(localName, namespaceURI);
            return str;
        }
        
        internal static string DiscardWhiteSpaces(string inputBuffer, int inputOffset, int inputCount)
        {
            int num1 = 0;
            for (int index = 0; index < inputCount; ++index)
            {
                if (char.IsWhiteSpace(inputBuffer[inputOffset + index]))
                    ++num1;
            }
            char[] chArray = new char[inputCount - num1];
            int num2 = 0;
            for (int index = 0; index < inputCount; ++index)
            {
                if (!char.IsWhiteSpace(inputBuffer[inputOffset + index]))
                    chArray[num2++] = inputBuffer[inputOffset + index];
            }
            return new string(chArray);
        }

        /// <summary>Parses the specified <see cref="T:System.Xml.XmlElement" /> object and configures the internal state of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> object to match.</summary>
        /// <param name="value">An <see cref="T:System.Xml.XmlElement" /> object to parse.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The key size expressed in the <paramref name="value" /> parameter was less than 0. </exception>
        public void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsmgr.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
            this.m_algorithm = GetAttribute(value, "Algorithm", "http://www.w3.org/2001/04/xmlenc#");
            XmlNode xmlNode = value.SelectSingleNode("enc:KeySize", nsmgr);
            if (xmlNode != null)
                this.KeySize = Convert.ToInt32(DiscardWhiteSpaces(xmlNode.InnerText, 0, xmlNode.InnerText.Length), (IFormatProvider) null);
            this.m_cachedXml = value;
        }
    }
}