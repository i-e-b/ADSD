using System;
using System.Collections;
using System.Xml;

namespace ADSD.Crypto
{
    internal class AttributeSortOrder : IComparer
    {
        public int Compare(object a, object b)
        {
            var xmlNode1 = a as XmlNode ?? throw new ArgumentException();
            var xmlNode2 = b as XmlNode ?? throw new ArgumentException();

            var num = string.CompareOrdinal(xmlNode1.NamespaceURI, xmlNode2.NamespaceURI);
            return num != 0 ? num : string.CompareOrdinal(xmlNode1.LocalName, xmlNode2.LocalName);
        }
    }
}