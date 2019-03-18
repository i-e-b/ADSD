using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;

namespace ADSD.Crypto
{
    internal class CanonicalXmlNodeList : XmlNodeList, IList
    {


        internal static bool IsCommittedNamespace(XmlElement element, string prefix, string value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            string name = prefix.Length > 0 ? "xmlns:" + prefix : "xmlns";
            return element.HasAttribute(name) && element.GetAttribute(name) == value;
        }
        private static bool HasNamespace(XmlElement element, string prefix, string value)
        {
            return IsCommittedNamespace(element, prefix, value) || element.Prefix == prefix && element.NamespaceURI == value;
        }
        internal static bool IsRedundantNamespace(XmlElement element, string prefix, string value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            for (XmlNode parentNode = element.ParentNode; parentNode != null; parentNode = parentNode.ParentNode)
            {
                XmlElement element1 = parentNode as XmlElement;
                if (element1 != null && HasNamespace(element1, prefix, value))
                    return true;
            }
            return false;
        }


        public static CanonicalXmlNodeList GetPropagatedAttributes(XmlElement elem)
        {
            if (elem == null)
                return (CanonicalXmlNodeList)null;
            CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
            XmlNode xmlNode = (XmlNode)elem;
            if (xmlNode == null)
                return (CanonicalXmlNodeList)null;
            bool flag = true;
            while (xmlNode != null)
            {
                XmlElement element = xmlNode as XmlElement;
                if (element == null)
                {
                    xmlNode = xmlNode.ParentNode;
                }
                else
                {
                    if (!IsCommittedNamespace(element, element.Prefix, element.NamespaceURI) && !IsRedundantNamespace(element, element.Prefix, element.NamespaceURI))
                    {
                        string name = element.Prefix.Length > 0 ? "xmlns:" + element.Prefix : "xmlns";
                        XmlAttribute attribute = elem.OwnerDocument.CreateAttribute(name);
                        attribute.Value = element.NamespaceURI;
                        canonicalXmlNodeList.Add((object)attribute);
                    }
                    if (element.HasAttributes)
                    {
                        foreach (XmlAttribute attribute1 in (XmlNamedNodeMap)element.Attributes)
                        {
                            if (flag && attribute1.LocalName == "xmlns")
                            {
                                XmlAttribute attribute2 = elem.OwnerDocument.CreateAttribute("xmlns");
                                attribute2.Value = attribute1.Value;
                                canonicalXmlNodeList.Add((object)attribute2);
                                flag = false;
                            }
                            else if (attribute1.Prefix == "xmlns" || attribute1.Prefix == "xml")
                                canonicalXmlNodeList.Add((object)attribute1);
                            else if (attribute1.NamespaceURI.Length > 0
                                     && !IsCommittedNamespace(element, attribute1.Prefix, attribute1.NamespaceURI)
                                     && !IsRedundantNamespace(element, attribute1.Prefix, attribute1.NamespaceURI))
                            {
                                string name = attribute1.Prefix.Length > 0 ? "xmlns:" + attribute1.Prefix : "xmlns";
                                XmlAttribute attribute2 = elem.OwnerDocument.CreateAttribute(name);
                                attribute2.Value = attribute1.NamespaceURI;
                                canonicalXmlNodeList.Add((object)attribute2);
                            }
                        }
                    }
                    xmlNode = xmlNode.ParentNode;
                }
            }
            return canonicalXmlNodeList;
        }



        [NotNull] private readonly List<XmlNode> nodes;

        public CanonicalXmlNodeList() { nodes = new List<XmlNode>(); }

        public override XmlNode Item(int index) { return nodes[index]; }

        public override IEnumerator GetEnumerator() { return nodes.GetEnumerator(); }

        public override int Count => nodes.Count;

        public int Add(object value)
        {
            if (!(value is XmlNode)) throw new ArgumentException("Cryptography error: Incorrect object type", nameof(value));
            nodes.Add((XmlNode)value);
            return nodes.Count;
        }

        public void Clear() { nodes.Clear(); }

        public bool Contains(object value) { return nodes.Contains(value); }

        public int IndexOf(object value) { return nodes.IndexOf(value as XmlNode); }

        public void Insert(int index, object value)
        {
            if (!(value is XmlNode)) throw new ArgumentException("Cryptography error: Incorrect object type", nameof(value));
            nodes.Insert(index, (XmlNode)value);
        }

        public void Remove(object value) { nodes.Remove(value as XmlNode); }

        public void RemoveAt(int index) { nodes.RemoveAt(index); }

        public bool IsFixedSize { get; } = false;
        public bool IsReadOnly { get; } = false;

        object IList.this[int index]
        {
            get { return nodes[index]; }
            set
            {
                if (!(value is XmlNode)) throw new ArgumentException("Cryptography error: Incorrect object type", "index");
                nodes[index] = (XmlNode)value;
            }
        }

        public void CopyTo(Array array, int index) { nodes.ToArray().CopyTo(array, index); }

        public object SyncRoot { get; } = new object();

        public bool IsSynchronized { get; } = false;
    }
}