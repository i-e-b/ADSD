using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>
    /// A massive dump-bag of XML helpers
    /// </summary>
    internal class Exml : XmlDocument
    {
        protected override XmlAttribute CreateDefaultAttribute(
            string prefix,
            string localName,
            string namespaceURI)
        {
            return CreateAttribute(prefix, localName, namespaceURI);
        }
        
        
        public static List<XmlElement> GetXmlElements(XmlNodeList nodeList)
        {
            if (nodeList == null) throw new ArgumentNullException(nameof (nodeList));
            List<XmlElement> xmlElementList = new List<XmlElement>();
            foreach (XmlNode node in nodeList)
            {
                XmlElement xmlElement = node as XmlElement;
                if (xmlElement != null)
                    xmlElementList.Add(xmlElement);
            }
            return xmlElementList;
        }
        
        public static bool HasAttribute(XmlElement element, string localName, string namespaceURI)
        {
            if (!element.HasAttribute(localName))
                return element.HasAttribute(localName, namespaceURI);
            return true;
        }

        public static Hashtable TokenizePrefixListString(string s)
        {
            Hashtable hashtable = new Hashtable();
            if (s != null)
            {
                foreach (string str in s.Split((char[]) null))
                {
                    if (str.Equals("#default"))
                        hashtable.Add((object) string.Empty, (object) true);
                    else if (str.Length > 0)
                        hashtable.Add((object) str, (object) true);
                }
            }
            return hashtable;
        }
        
        public static XmlReader PreProcessStreamInput(
            Stream inputStream,
            XmlResolver xmlResolver,
            string baseUri)
        {
            XmlReaderSettings xmlReaderSettings =  new XmlReaderSettings
            {
                XmlResolver = xmlResolver,
                DtdProcessing = DtdProcessing.Parse,
                MaxCharactersFromEntities = 10000000L,
                MaxCharactersInDocument = 0
            };
            return XmlReader.Create(inputStream, xmlReaderSettings, baseUri);
        }

        
        public static void RemoveAllChildren(XmlElement inputElement)
        {
            XmlNode oldChild = inputElement.FirstChild;
            XmlNode nextSibling;
            for (; oldChild != null; oldChild = nextSibling)
            {
                nextSibling = oldChild.NextSibling;
                inputElement.RemoveChild(oldChild);
            }
        }

        public static bool NodeInList(XmlNode node, XmlNodeList nodeList)
        {
            foreach (XmlNode node1 in nodeList)
            {
                if (node1 == node)
                    return true;
            }
            return false;
        }
        
        public static string EscapeWhitespaceData(string data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(data);
            SBReplaceCharWithString(sb, '\r', "&#xD;");
            return sb.ToString();
        }
        
        
        public static XmlDocument GetOwnerDocument(XmlNodeList nodeList)
        {
            foreach (XmlNode node in nodeList)
            {
                if (node.OwnerDocument != null)
                    return node.OwnerDocument;
            }
            return (XmlDocument) null;
        }

        public static string EscapeTextData(string data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(data);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            SBReplaceCharWithString(sb, '\r', "&#xD;");
            return sb.ToString();
        }
        
        public static bool IsNonRedundantNamespaceDecl(
            XmlAttribute a,
            XmlAttribute nearestAncestorWithSamePrefix)
        {
            if (nearestAncestorWithSamePrefix == null)
                return !IsEmptyDefaultNamespaceNode((XmlNode) a);
            return !nearestAncestorWithSamePrefix.Value.Equals(a.Value);
        }
        
        public static bool HasNamespacePrefix(XmlAttribute a, string nsPrefix)
        {
            return GetNamespacePrefix(a).Equals(nsPrefix);
        }

        public static bool IsNamespaceNode(XmlNode n)
        {
            if (n.NodeType != XmlNodeType.Attribute)
                return false;
            if (n.Prefix.Equals("xmlns"))
                return true;
            if (n.Prefix.Length == 0)
                return n.LocalName.Equals("xmlns");
            return false;
        }

        public static bool IsXmlNamespaceNode(XmlNode n)
        {
            if (n.NodeType == XmlNodeType.Attribute)
                return n.Prefix.Equals("xml");
            return false;
        }

        public static string EscapeAttributeValue(string value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(value);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace("\"", "&quot;");
            SBReplaceCharWithString(sb, '\t', "&#x9;");
            SBReplaceCharWithString(sb, '\n', "&#xA;");
            SBReplaceCharWithString(sb, '\r', "&#xD;");
            return sb.ToString();
        }
        
        internal static void SBReplaceCharWithString(StringBuilder sb, char oldChar, string newString)
        {
            int index = 0;
            int length = newString.Length;
            while (index < sb.Length)
            {
                if ((int) sb[index] == (int) oldChar)
                {
                    sb.Remove(index, 1);
                    sb.Insert(index, newString);
                    index += length;
                }
                else
                    ++index;
            }
        }
        
        public static string DiscardWhiteSpaces(string inputBuffer){
            return DiscardWhiteSpaces(inputBuffer, 0, inputBuffer.Length);
        }
        
        public static string GetNamespacePrefix(XmlAttribute a)
        {
            if (a.Prefix.Length != 0)
                return a.LocalName;
            return string.Empty;
        }
        
        internal static bool IsDefaultNamespaceNode(XmlNode n)
        {
            return (n.NodeType == XmlNodeType.Attribute && n.Prefix.Length == 0 && n.LocalName.Equals("xmlns")) | IsXmlNamespaceNode(n);
        }

        internal static bool IsEmptyDefaultNamespaceNode(XmlNode n)
        {
            if (IsDefaultNamespaceNode(n))
                return n.Value.Length == 0;
            return false;
        }
        
        public static string ExtractIdFromLocalUri(string uri)
        {
            string str = uri.Substring(1);
            if (str.StartsWith("xpointer(id(", StringComparison.Ordinal))
            {
                int num1 = str.IndexOf("id(", StringComparison.Ordinal);
                int num2 = str.IndexOf(")", StringComparison.Ordinal);
                if (num2 < 0 || num2 < num1 + 3)
                    throw new CryptographicException("Cryptography_Xml_InvalidReference");
                str = str.Substring(num1 + 3, num2 - num1 - 3).Replace("'", "").Replace("\"", "");
            }
            return str;
        }
        
        public static XmlDocument PreProcessElementInput(
            XmlElement elem,
            XmlResolver xmlResolver,
            string baseUri)
        {
            if (elem == null)
                throw new ArgumentNullException(nameof (elem));
            Exml exml = new Exml();
            exml.PreserveWhitespace = true;
            using (TextReader input = (TextReader) new StringReader(elem.OuterXml))
            {
                XmlReader reader = XmlReader.Create(input, new XmlReaderSettings()
                {
                    XmlResolver = xmlResolver,
                    DtdProcessing = DtdProcessing.Parse,
                    MaxCharactersFromEntities = 10000000L,
                    MaxCharactersInDocument = 0
                }, baseUri);
                exml.Load(reader);
            }
            return (XmlDocument) exml;
        }

        public static void AddNamespaces(XmlElement elem, CanonicalXmlNodeList namespaces)
        {
            if (namespaces == null)
                return;
            foreach (XmlNode xmlNode in (XmlNodeList) namespaces)
            {
                string name = xmlNode.Prefix.Length > 0 ? xmlNode.Prefix + ":" + xmlNode.LocalName : xmlNode.LocalName;
                if (!elem.HasAttribute(name) && (!name.Equals("xmlns") || elem.Prefix.Length != 0))
                {
                    XmlAttribute attribute = elem.OwnerDocument.CreateAttribute(name);
                    attribute.Value = xmlNode.Value;
                    elem.SetAttributeNode(attribute);
                }
            }
        }

        public static void AddNamespaces(XmlElement elem, Hashtable namespaces)
        {
            if (namespaces == null)
                return;
            foreach (string key in (IEnumerable) namespaces.Keys)
            {
                if (!elem.HasAttribute(key))
                {
                    XmlAttribute attribute = elem.OwnerDocument.CreateAttribute(key);
                    attribute.Value = namespaces[(object) key] as string;
                    elem.SetAttributeNode(attribute);
                }
            }
        }
        
        public static string DiscardWhiteSpaces(string inputBuffer, int inputOffset, int inputCount)
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
        
        public static string GetAttribute(XmlElement element, string localName, string namespaceURI)
        {
            string str = element.HasAttribute(localName) ? element.GetAttribute(localName) : (string) null;
            if (str == null && element.HasAttribute(localName, namespaceURI))
                str = element.GetAttribute(localName, namespaceURI);
            return str;
        }

        
        public static bool VerifyAttributes(XmlElement element, params string[] expectedAttrNames)
        {
            if (GlobalSettings.SkipSignatureAttributeChecks) return true;
            foreach (XmlAttribute attribute in (XmlNamedNodeMap) element.Attributes)
            {
                bool flag = attribute.Name == "xmlns" || attribute.Name.StartsWith("xmlns:") || (attribute.Name == "xml:space" || attribute.Name == "xml:lang") || attribute.Name == "xml:base";
                for (int index = 0; !flag && expectedAttrNames != null && index < expectedAttrNames.Length; ++index)
                    flag = attribute.Name == expectedAttrNames[index];
                if (!flag)
                    return false;
            }
            return true;
        }
        
        public static T CreateFromName<T>(string key) where T : class { return CryptoConfig.CreateFromName(key) as T; }
    }
}