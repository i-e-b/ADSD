using System;
using System.Collections;
using System.Xml;

namespace ADSD
{
    internal class AttributeSortOrder : IComparer
    {
        internal AttributeSortOrder()
        {
        }

        public int Compare(object a, object b)
        {
            XmlNode xmlNode1 = a as XmlNode;
            XmlNode xmlNode2 = b as XmlNode;
            if (a == null || b == null)
                throw new ArgumentException();
            int num = string.CompareOrdinal(xmlNode1.NamespaceURI, xmlNode2.NamespaceURI);
            if (num != 0)
                return num;
            return string.CompareOrdinal(xmlNode1.LocalName, xmlNode2.LocalName);
        }
    }
}