using System;
using System.Collections;
using System.Xml;

namespace ADSD.Crypto
{
    internal class C14NAncestralNamespaceContextManager : AncestralNamespaceContextManager
    {

        private void GetNamespaceToRender(
            string nsPrefix,
            SortedList attrListToRender,
            SortedList nsListToRender,
            Hashtable nsLocallyDeclared)
        {
            foreach (XmlAttribute key in (IEnumerable) nsListToRender.GetKeyList())
            {
                if (Exml.HasNamespacePrefix(key, nsPrefix))
                    return;
            }
            foreach (XmlNode key in (IEnumerable) attrListToRender.GetKeyList())
            {
                if (key.LocalName.Equals(nsPrefix))
                    return;
            }
            XmlAttribute a = (XmlAttribute) nsLocallyDeclared[(object) nsPrefix];
            int depth1;
            XmlAttribute withMatchingPrefix1 = this.GetNearestRenderedNamespaceWithMatchingPrefix(nsPrefix, out depth1);
            if (a != null)
            {
                if (!Exml.IsNonRedundantNamespaceDecl(a, withMatchingPrefix1))
                    return;
                nsLocallyDeclared.Remove((object) nsPrefix);
                if (Exml.IsXmlNamespaceNode((XmlNode) a))
                    attrListToRender.Add((object) a, (object) null);
                else
                    nsListToRender.Add((object) a, (object) null);
            }
            else
            {
                int depth2;
                XmlAttribute withMatchingPrefix2 = this.GetNearestUnrenderedNamespaceWithMatchingPrefix(nsPrefix, out depth2);
                if (withMatchingPrefix2 == null || depth2 <= depth1 || !Exml.IsNonRedundantNamespaceDecl(withMatchingPrefix2, withMatchingPrefix1))
                    return;
                if (Exml.IsXmlNamespaceNode((XmlNode) withMatchingPrefix2))
                    attrListToRender.Add((object) withMatchingPrefix2, (object) null);
                else
                    nsListToRender.Add((object) withMatchingPrefix2, (object) null);
            }
        }

        internal override void GetNamespacesToRender(
            XmlElement element,
            SortedList attrListToRender,
            SortedList nsListToRender,
            Hashtable nsLocallyDeclared)
        {
            object[] objArray = new object[nsLocallyDeclared.Count];
            nsLocallyDeclared.Values.CopyTo((Array) objArray, 0);
            foreach (XmlAttribute a in objArray)
            {
                int depth;
                XmlAttribute withMatchingPrefix = this.GetNearestRenderedNamespaceWithMatchingPrefix(Exml.GetNamespacePrefix(a), out depth);
                if (Exml.IsNonRedundantNamespaceDecl(a, withMatchingPrefix))
                {
                    nsLocallyDeclared.Remove((object) Exml.GetNamespacePrefix(a));
                    if (Exml.IsXmlNamespaceNode((XmlNode) a))
                        attrListToRender.Add((object) a, (object) null);
                    else
                        nsListToRender.Add((object) a, (object) null);
                }
            }
            for (int i = this.m_ancestorStack.Count - 1; i >= 0; --i)
            {
                foreach (XmlAttribute a in (IEnumerable) this.GetScopeAt(i).GetUnrendered().Values)
                {
                    if (a != null)
                        this.GetNamespaceToRender(Exml.GetNamespacePrefix(a), attrListToRender, nsListToRender, nsLocallyDeclared);
                }
            }
        }

        internal override void TrackNamespaceNode(
            XmlAttribute attr,
            SortedList nsListToRender,
            Hashtable nsLocallyDeclared)
        {
            nsLocallyDeclared.Add((object) Exml.GetNamespacePrefix(attr), (object) attr);
        }

        internal override void TrackXmlNamespaceNode(
            XmlAttribute attr,
            SortedList nsListToRender,
            SortedList attrListToRender,
            Hashtable nsLocallyDeclared)
        {
            nsLocallyDeclared.Add((object) Exml.GetNamespacePrefix(attr), (object) attr);
        }
    }
}