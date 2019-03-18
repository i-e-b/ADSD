using System.Collections;
using System.Xml;

namespace ADSD.Crypto
{
    internal class ExcAncestralNamespaceContextManager : AncestralNamespaceContextManager
    {
        private readonly Hashtable m_inclusivePrefixSet;

        internal ExcAncestralNamespaceContextManager(string inclusiveNamespacesPrefixList)
        {
            m_inclusivePrefixSet = Exml.TokenizePrefixListString(inclusiveNamespacesPrefixList);
        }

        private bool HasNonRedundantInclusivePrefix(XmlAttribute attr)
        {
            string namespacePrefix = Exml.GetNamespacePrefix(attr);
            if (m_inclusivePrefixSet.ContainsKey((object) namespacePrefix))
            {
                int depth;
                return Exml.IsNonRedundantNamespaceDecl(attr, GetNearestRenderedNamespaceWithMatchingPrefix(namespacePrefix, out depth));
            }
            return false;
        }

        private void GatherNamespaceToRender(
            string nsPrefix,
            SortedList nsListToRender,
            Hashtable nsLocallyDeclared)
        {
            foreach (XmlAttribute key in (IEnumerable) nsListToRender.GetKeyList())
            {
                if (Exml.HasNamespacePrefix(key, nsPrefix))
                    return;
            }
            XmlAttribute a = (XmlAttribute) nsLocallyDeclared[(object) nsPrefix];
            int depth1;
            XmlAttribute withMatchingPrefix1 = GetNearestRenderedNamespaceWithMatchingPrefix(nsPrefix, out depth1);
            if (a != null)
            {
                if (!Exml.IsNonRedundantNamespaceDecl(a, withMatchingPrefix1))
                    return;
                nsLocallyDeclared.Remove((object) nsPrefix);
                nsListToRender.Add((object) a, (object) null);
            }
            else
            {
                int depth2;
                XmlAttribute withMatchingPrefix2 = GetNearestUnrenderedNamespaceWithMatchingPrefix(nsPrefix, out depth2);
                if (withMatchingPrefix2 == null || depth2 <= depth1 || !Exml.IsNonRedundantNamespaceDecl(withMatchingPrefix2, withMatchingPrefix1))
                    return;
                nsListToRender.Add((object) withMatchingPrefix2, (object) null);
            }
        }

        internal override void GetNamespacesToRender(
            XmlElement element,
            SortedList attrListToRender,
            SortedList nsListToRender,
            Hashtable nsLocallyDeclared)
        {
            GatherNamespaceToRender(element.Prefix, nsListToRender, nsLocallyDeclared);
            foreach (XmlNode key in (IEnumerable) attrListToRender.GetKeyList())
            {
                string prefix = key.Prefix;
                if (prefix.Length > 0)
                    GatherNamespaceToRender(prefix, nsListToRender, nsLocallyDeclared);
            }
        }

        internal override void TrackNamespaceNode(
            XmlAttribute attr,
            SortedList nsListToRender,
            Hashtable nsLocallyDeclared)
        {
            if (HasNonRedundantInclusivePrefix(attr)) nsListToRender.Add((object) attr, (object) null);
            else nsLocallyDeclared.Add((object) Exml.GetNamespacePrefix(attr), (object) attr);
        }

        internal override void TrackXmlNamespaceNode(
            XmlAttribute attr,
            SortedList nsListToRender,
            SortedList attrListToRender,
            Hashtable nsLocallyDeclared)
        {
            attrListToRender.Add((object) attr, (object) null);
        }
    }
}