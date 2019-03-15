using System;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD
{
    /// <summary>Represents the <see cref="T:System.Security.Cryptography.DSA" /> private key of the <see langword="&lt;KeyInfo&gt;" /> element.</summary>
    public class DSAKeyValue : KeyInfoClause
    {
        private DSA m_key;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.DSAKeyValue" /> class with a new, randomly-generated <see cref="T:System.Security.Cryptography.DSA" /> public key.</summary>
        public DSAKeyValue()
        {
            this.m_key = DSA.Create();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.DSAKeyValue" /> class with the specified <see cref="T:System.Security.Cryptography.DSA" /> public key.</summary>
        /// <param name="key">The instance of an implementation of the <see cref="T:System.Security.Cryptography.DSA" /> class that holds the public key. </param>
        public DSAKeyValue(DSA key)
        {
            this.m_key = key;
        }

        /// <summary>Gets or sets the key value represented by a <see cref="T:System.Security.Cryptography.DSA" /> object.</summary>
        /// <returns>The public key represented by a <see cref="T:System.Security.Cryptography.DSA" /> object.</returns>
        public DSA Key
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

        /// <summary>Returns the XML representation of a <see cref="T:System.Security.Cryptography.Xml.DSAKeyValue" /> element.</summary>
        /// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.Xml.DSAKeyValue" /> element.</returns>
        public override XmlElement GetXml()
        {
            return this.GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal override XmlElement GetXml(XmlDocument xmlDocument)
        {
            DSAParameters dsaParameters = this.m_key.ExportParameters(false);
            XmlElement element1 = xmlDocument.CreateElement("KeyValue", "http://www.w3.org/2000/09/xmldsig#");
            XmlElement element2 = xmlDocument.CreateElement(nameof (DSAKeyValue), "http://www.w3.org/2000/09/xmldsig#");
            XmlElement element3 = xmlDocument.CreateElement("P", "http://www.w3.org/2000/09/xmldsig#");
            element3.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParameters.P)));
            element2.AppendChild((XmlNode) element3);
            XmlElement element4 = xmlDocument.CreateElement("Q", "http://www.w3.org/2000/09/xmldsig#");
            element4.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParameters.Q)));
            element2.AppendChild((XmlNode) element4);
            XmlElement element5 = xmlDocument.CreateElement("G", "http://www.w3.org/2000/09/xmldsig#");
            element5.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParameters.G)));
            element2.AppendChild((XmlNode) element5);
            XmlElement element6 = xmlDocument.CreateElement("Y", "http://www.w3.org/2000/09/xmldsig#");
            element6.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParameters.Y)));
            element2.AppendChild((XmlNode) element6);
            if (dsaParameters.J != null)
            {
                XmlElement element7 = xmlDocument.CreateElement("J", "http://www.w3.org/2000/09/xmldsig#");
                element7.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParameters.J)));
                element2.AppendChild((XmlNode) element7);
            }
            if (dsaParameters.Seed != null)
            {
                XmlElement element7 = xmlDocument.CreateElement("Seed", "http://www.w3.org/2000/09/xmldsig#");
                element7.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParameters.Seed)));
                element2.AppendChild((XmlNode) element7);
                XmlElement element8 = xmlDocument.CreateElement("PgenCounter", "http://www.w3.org/2000/09/xmldsig#");
                element8.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(ConvertIntToByteArray(dsaParameters.Counter))));
                element2.AppendChild((XmlNode) element8);
            }
            element1.AppendChild((XmlNode) element2);
            return element1;
        }
        internal static byte[] ConvertIntToByteArray(int dwInput)
        {
            byte[] numArray1 = new byte[8];
            int length = 0;
            if (dwInput == 0)
                return new byte[1];
            int num1 = dwInput;
            while (num1 > 0)
            {
                int num2 = num1 % 256;
                numArray1[length] = (byte) num2;
                num1 = (num1 - num2) / 256;
                ++length;
            }
            byte[] numArray2 = new byte[length];
            for (int index = 0; index < length; ++index)
                numArray2[index] = numArray1[length - index - 1];
            return numArray2;
        }

        /// <summary>Loads a <see cref="T:System.Security.Cryptography.Xml.DSAKeyValue" /> state from an XML element.</summary>
        /// <param name="value">The XML element to load the <see cref="T:System.Security.Cryptography.Xml.DSAKeyValue" /> state from. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="value" /> parameter is not a valid <see cref="T:System.Security.Cryptography.Xml.DSAKeyValue" /> XML element. </exception>
        public override void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            this.m_key.FromXmlString(value.OuterXml);
        }
    }
}