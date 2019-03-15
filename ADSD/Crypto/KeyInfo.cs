using System;
using System.Collections;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD
{
    /// <summary>Represents an XML digital signature or XML encryption <see langword="&lt;KeyInfo&gt;" /> element.</summary>
    public class KeyInfo : IEnumerable
    {
        private string m_id;
        private ArrayList m_KeyInfoClauses;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> class.</summary>
        public KeyInfo()
        {
            this.m_KeyInfoClauses = new ArrayList();
        }

        /// <summary>Gets or sets the key information identity.</summary>
        /// <returns>The key information identity.</returns>
        public string Id
        {
            get
            {
                return this.m_id;
            }
            set
            {
                this.m_id = value;
            }
        }

        /// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object.</summary>
        /// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object.</returns>
        public XmlElement GetXml()
        {
            return this.GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal XmlElement GetXml(XmlDocument xmlDocument)
        {
            XmlElement element = xmlDocument.CreateElement(nameof (KeyInfo), "http://www.w3.org/2000/09/xmldsig#");
            if (!string.IsNullOrEmpty(this.m_id))
                element.SetAttribute("Id", this.m_id);
            for (int index = 0; index < this.m_KeyInfoClauses.Count; ++index)
            {
                XmlElement xml = ((KeyInfoClause) this.m_KeyInfoClauses[index]).GetXml(xmlDocument);
                if (xml != null)
                    element.AppendChild((XmlNode) xml);
            }
            return element;
        }

        /// <summary>Loads a <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> state from an XML element.</summary>
        /// <param name="value">The XML element from which to load the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> state. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />. </exception>
        public void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            XmlElement element1 = value;
            this.m_id = Exml.GetAttribute(element1, "Id", "http://www.w3.org/2000/09/xmldsig#");
            if (!Exml.VerifyAttributes(element1, "Id"))
                throw new CryptographicException("Invalid XML element: KeyInfo");
            for (XmlNode xmlNode = element1.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
            {
                XmlElement element2 = xmlNode as XmlElement;
                if (element2 != null)
                {
                    string key = element2.NamespaceURI + " " + element2.LocalName;
                    if (key == "http://www.w3.org/2000/09/xmldsig# KeyValue")
                    {
                        if (!Exml.VerifyAttributes(element2, (string[]) null))
                            throw new CryptographicException("Invalid XML element: KeyInfo/KeyValue");
                        foreach (XmlNode childNode in element2.ChildNodes)
                        {
                            XmlElement xmlElement = childNode as XmlElement;
                            if (xmlElement != null)
                            {
                                key = key + "/" + xmlElement.LocalName;
                                break;
                            }
                        }
                    }
                    KeyInfoClause clause = Exml.CreateFromName<KeyInfoClause>(key) ?? (KeyInfoClause) new KeyInfoNode();
                    clause.LoadXml(element2);
                    this.AddClause(clause);
                }
            }
        }

        /// <summary>Gets the number of <see cref="T:System.Security.Cryptography.Xml.KeyInfoClause" /> objects contained in the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object.</summary>
        /// <returns>The number of <see cref="T:System.Security.Cryptography.Xml.KeyInfoClause" /> objects contained in the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object.</returns>
        public int Count
        {
            get
            {
                return this.m_KeyInfoClauses.Count;
            }
        }

        /// <summary>Adds a <see cref="T:System.Security.Cryptography.Xml.KeyInfoClause" /> that represents a particular type of <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> information to the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object.</summary>
        /// <param name="clause">The <see cref="T:System.Security.Cryptography.Xml.KeyInfoClause" /> to add to the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object. </param>
        public void AddClause(KeyInfoClause clause)
        {
            this.m_KeyInfoClauses.Add((object) clause);
        }

        /// <summary>Returns an enumerator of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoClause" /> objects in the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object.</summary>
        /// <returns>An enumerator of the subelements of <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return this.m_KeyInfoClauses.GetEnumerator();
        }

        /// <summary>Returns an enumerator of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoClause" /> objects of the specified type in the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object.</summary>
        /// <param name="requestedObjectType">The type of object to enumerate. </param>
        /// <returns>An enumerator of the subelements of <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator(Type requestedObjectType)
        {
            ArrayList arrayList = new ArrayList();
            foreach (object keyInfoClause in this.m_KeyInfoClauses)
            {
                if (requestedObjectType.Equals(keyInfoClause.GetType()))
                    arrayList.Add(keyInfoClause);
            }
            return arrayList.GetEnumerator();
        }
    }
}