﻿using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>Represents the abstract base class from which all implementations of <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> subelements inherit.</summary>
    public abstract class KeyInfoClause
    {
        /// <summary>When overridden in a derived class, returns an XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoClause" />.</summary>
        /// <returns>An XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoClause" />.</returns>
        public abstract XmlElement GetXml();

        internal virtual XmlElement GetXml(XmlDocument xmlDocument)
        {
            XmlElement xml = this.GetXml();
            return (XmlElement) xmlDocument.ImportNode((XmlNode) xml, true);
        }

        /// <summary>When overridden in a derived class, parses the input <see cref="T:System.Xml.XmlElement" /> and configures the internal state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoClause" /> to match.</summary>
        /// <param name="element">The <see cref="T:System.Xml.XmlElement" /> that specifies the state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoClause" />. </param>
        public abstract void LoadXml(XmlElement element);
    }
}