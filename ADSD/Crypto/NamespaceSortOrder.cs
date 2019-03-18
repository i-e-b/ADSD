using System;
using System.Collections;
using System.Xml;

namespace ADSD.Crypto
{
    internal class NamespaceSortOrder : IComparer
    {
        public int Compare(object a, object b)
        {
            XmlNode n1 = a as XmlNode;
            XmlNode n2 = b as XmlNode;
            if (a == null || b == null)
                throw new ArgumentException();
            bool flag1 = Exml.IsDefaultNamespaceNode(n1);
            bool flag2 = Exml.IsDefaultNamespaceNode(n2);
            if (flag1 & flag2)
                return 0;
            if (flag1)
                return -1;
            if (flag2)
                return 1;
            return string.CompareOrdinal(n1.LocalName, n2.LocalName);
        }
    }
}