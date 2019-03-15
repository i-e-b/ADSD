﻿using System;
using System.Xml;

namespace ADSD
{
    /// <summary>Represents a <see langword="&lt;KeyName&gt;" /> subelement of an XMLDSIG or XML Encryption <see langword="&lt;KeyInfo&gt;" /> element.</summary>
    public class KeyInfoName : KeyInfoClause
    {
        private string m_keyName;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoName" /> class.</summary>
        public KeyInfoName()
            : this((string) null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoName" /> class by specifying the string identifier that is the value of the <see langword="&lt;KeyName&gt;" /> element.</summary>
        /// <param name="keyName">The string identifier that is the value of the <see langword="&lt;KeyName&gt;" /> element.</param>
        public KeyInfoName(string keyName)
        {
            this.Value = keyName;
        }

        /// <summary>Gets or sets the string identifier contained within a <see langword="&lt;KeyName&gt;" /> element.</summary>
        /// <returns>The string identifier that is the value of the <see langword="&lt;KeyName&gt;" /> element.</returns>
        public string Value
        {
            get
            {
                return this.m_keyName;
            }
            set
            {
                this.m_keyName = value;
            }
        }

        /// <summary>Returns an XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoName" /> object.</summary>
        /// <returns>An XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoName" /> object.</returns>
        public override XmlElement GetXml()
        {
            return this.GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal override XmlElement GetXml(XmlDocument xmlDocument)
        {
            XmlElement element = xmlDocument.CreateElement("KeyName", "http://www.w3.org/2000/09/xmldsig#");
            element.AppendChild((XmlNode) xmlDocument.CreateTextNode(this.m_keyName));
            return element;
        }

        /// <summary>Parses the input <see cref="T:System.Xml.XmlElement" /> object and configures the internal state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoName" /> object to match.</summary>
        /// <param name="value">The <see cref="T:System.Xml.XmlElement" /> object that specifies the state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoName" /> object. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />. </exception>
        public override void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            this.m_keyName = value.InnerText.Trim();
        }
    }
}