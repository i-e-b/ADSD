using System;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD
{
    /// <summary>Represents the &lt;<see langword="RSAKeyValu" />e&gt; element of an XML signature.</summary>
    public class RSAKeyValue : KeyInfoClause
    {
        private RSA m_key;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.RSAKeyValue" /> class with a new randomly generated <see cref="T:System.Security.Cryptography.RSA" /> public key.</summary>
        public RSAKeyValue()
        {
            this.m_key = RSA.Create();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.RSAKeyValue" /> class with the specified <see cref="T:System.Security.Cryptography.RSA" /> public key.</summary>
        /// <param name="key">The instance of an implementation of <see cref="T:System.Security.Cryptography.RSA" /> that holds the public key. </param>
        public RSAKeyValue(RSA key)
        {
            this.m_key = key;
        }

        /// <summary>Gets or sets the instance of <see cref="T:System.Security.Cryptography.RSA" /> that holds the public key.</summary>
        /// <returns>The instance of <see cref="T:System.Security.Cryptography.RSA" /> that holds the public key.</returns>
        public RSA Key
        {
            get
            {
                return this.m_key;
            }
            set
            {
                this.m_key = value;
            }
        }

        /// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.RSA" /> key clause.</summary>
        /// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.RSA" /> key clause.</returns>
        public override XmlElement GetXml()
        {
            return this.GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal override XmlElement GetXml(XmlDocument xmlDocument)
        {
            RSAParameters rsaParameters = this.m_key.ExportParameters(false);
            XmlElement element1 = xmlDocument.CreateElement("KeyValue", "http://www.w3.org/2000/09/xmldsig#");
            XmlElement element2 = xmlDocument.CreateElement(nameof (RSAKeyValue), "http://www.w3.org/2000/09/xmldsig#");
            XmlElement element3 = xmlDocument.CreateElement("Modulus", "http://www.w3.org/2000/09/xmldsig#");
            element3.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(rsaParameters.Modulus)));
            element2.AppendChild((XmlNode) element3);
            XmlElement element4 = xmlDocument.CreateElement("Exponent", "http://www.w3.org/2000/09/xmldsig#");
            element4.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(rsaParameters.Exponent)));
            element2.AppendChild((XmlNode) element4);
            element1.AppendChild((XmlNode) element2);
            return element1;
        }

        /// <summary>Loads an <see cref="T:System.Security.Cryptography.RSA" /> key clause from an XML element.</summary>
        /// <param name="value">The XML element from which to load the <see cref="T:System.Security.Cryptography.RSA" /> key clause. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="value" /> parameter is not a valid <see cref="T:System.Security.Cryptography.RSA" /> key clause XML element. </exception>
        public override void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            this.m_key.FromXmlString(value.OuterXml);
        }
    }
}