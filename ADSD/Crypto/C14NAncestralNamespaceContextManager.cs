using System;
using System.Collections;
using System.Xml;
using JetBrains.Annotations;

namespace ADSD.Crypto
{
    internal class C14NAncestralNamespaceContextManager : AncestralNamespaceContextManager
    {
        private void GetNamespaceToRender(
            [NotNull]string nsPrefix,
            [NotNull]SortedList attrListToRender,
            [NotNull]SortedList nsListToRender,
            [NotNull]Hashtable nsLocallyDeclared)
        {
            foreach (XmlAttribute key in nsListToRender.GetKeyList())
            {
                if (Exml.HasNamespacePrefix(key, nsPrefix))
                    return;
            }
            foreach (XmlNode key in attrListToRender.GetKeyList())
            {
                if (key.LocalName.Equals(nsPrefix))
                    return;
            }
            XmlAttribute a = (XmlAttribute) nsLocallyDeclared[nsPrefix];
            int depth1;
            XmlAttribute withMatchingPrefix1 = GetNearestRenderedNamespaceWithMatchingPrefix(nsPrefix, out depth1);
            if (a != null)
            {
                if (!Exml.IsNonRedundantNamespaceDecl(a, withMatchingPrefix1)) return;
                nsLocallyDeclared.Remove(nsPrefix);
                if (Exml.IsXmlNamespaceNode(a)) attrListToRender.Add(a, null);
                else nsListToRender.Add(a, null);
            }
            else
            {
                int depth2;
                var withMatchingPrefix2 = GetNearestUnrenderedNamespaceWithMatchingPrefix(nsPrefix, out depth2);
                if (withMatchingPrefix2 == null || depth2 <= depth1 || !Exml.IsNonRedundantNamespaceDecl(withMatchingPrefix2, withMatchingPrefix1))
                    return;
                if (Exml.IsXmlNamespaceNode(withMatchingPrefix2))
                    attrListToRender.Add(withMatchingPrefix2, null);
                else
                    nsListToRender.Add(withMatchingPrefix2, null);
            }
        }

        internal override void GetNamespacesToRender(
            [NotNull]XmlElement element,
            [NotNull]SortedList attrListToRender,
            [NotNull]SortedList nsListToRender,
            [NotNull]Hashtable nsLocallyDeclared)
        {
            object[] objArray = new object[nsLocallyDeclared.Count];
            nsLocallyDeclared.Values.CopyTo(objArray, 0);
            foreach (XmlAttribute a in objArray)
            {
                var namespacePrefix = Exml.GetNamespacePrefix(a);
                if (namespacePrefix == null) continue;
                var withMatchingPrefix = GetNearestRenderedNamespaceWithMatchingPrefix(namespacePrefix, out _);
                if (!Exml.IsNonRedundantNamespaceDecl(a, withMatchingPrefix)) continue;

                nsLocallyDeclared.Remove(namespacePrefix);
                if (Exml.IsXmlNamespaceNode(a)) attrListToRender.Add(a, null);
                else nsListToRender.Add(a, null);
            }
            for (int i = m_ancestorStack.Count - 1; i >= 0; --i)
            {
                var values = GetScopeAt(i)?.GetUnrendered()?.Values;
                if (values != null) foreach (XmlAttribute a in values)
                {
                    if (a == null) continue;
                    var prefix = Exml.GetNamespacePrefix(a);
                    if (prefix == null) continue;

                    GetNamespaceToRender(prefix, attrListToRender, nsListToRender, nsLocallyDeclared);
                }
            }
        }

        internal override void TrackNamespaceNode(
            [NotNull]XmlAttribute attr,
            [NotNull]SortedList nsListToRender,
            [NotNull]Hashtable nsLocallyDeclared)
        {
            nsLocallyDeclared.Add(Exml.GetNamespacePrefix(attr) ?? throw new InvalidOperationException(), attr);
        }

        internal override void TrackXmlNamespaceNode(
            [NotNull]XmlAttribute attr,
            [NotNull]SortedList nsListToRender,
            [NotNull]SortedList attrListToRender,
            [NotNull]Hashtable nsLocallyDeclared)
        {
            nsLocallyDeclared.Add(Exml.GetNamespacePrefix(attr) ?? throw new InvalidOperationException(), attr);
        }
    }
}