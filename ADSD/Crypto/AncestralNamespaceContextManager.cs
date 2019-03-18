using System;
using System.Collections;
using System.Xml;
using JetBrains.Annotations;

namespace ADSD.Crypto
{
    internal abstract class AncestralNamespaceContextManager
    {
        [NotNull]internal ArrayList m_ancestorStack = new ArrayList();

        internal NamespaceFrame GetScopeAt(int i)
        {
            return (NamespaceFrame) m_ancestorStack[i];
        }

        internal NamespaceFrame GetCurrentScope()
        {
            return GetScopeAt(m_ancestorStack.Count - 1);
        }

        protected XmlAttribute GetNearestRenderedNamespaceWithMatchingPrefix(string nsPrefix, out int depth)
        {
            depth = -1;
            for (int i = m_ancestorStack.Count - 1; i >= 0; --i)
            {
                XmlAttribute rendered;
                if ((rendered = GetScopeAt(i)?.GetRendered(nsPrefix)) == null) continue;

                depth = i;
                return rendered;
            }
            return null;
        }

        protected XmlAttribute GetNearestUnrenderedNamespaceWithMatchingPrefix(string nsPrefix, out int depth)
        {
            depth = -1;
            for (int i = m_ancestorStack.Count - 1; i >= 0; --i)
            {
                XmlAttribute unrendered;
                if ((unrendered = GetScopeAt(i)?.GetUnrendered(nsPrefix)) == null) continue;

                depth = i;
                return unrendered;
            }
            return null;
        }

        internal void EnterElementContext()
        {
            m_ancestorStack.Add(new NamespaceFrame());
        }

        internal void ExitElementContext()
        {
            m_ancestorStack.RemoveAt(m_ancestorStack.Count - 1);
        }

        internal abstract void TrackNamespaceNode(
            XmlAttribute attr,
            SortedList nsListToRender,
            Hashtable nsLocallyDeclared);

        internal abstract void TrackXmlNamespaceNode(
            XmlAttribute attr,
            SortedList nsListToRender,
            SortedList attrListToRender,
            Hashtable nsLocallyDeclared);

        internal abstract void GetNamespacesToRender(
            XmlElement element,
            SortedList attrListToRender,
            SortedList nsListToRender,
            Hashtable nsLocallyDeclared);

        internal void LoadUnrenderedNamespaces(Hashtable nsLocallyDeclared)
        {
            if (nsLocallyDeclared == null) throw new ArgumentNullException(nameof(nsLocallyDeclared));
            var objArray = new object[nsLocallyDeclared.Count];
            nsLocallyDeclared.Values.CopyTo(objArray, 0);
            foreach (XmlAttribute attr in objArray) AddUnrendered(attr);
        }

        internal void LoadRenderedNamespaces(SortedList nsRenderedList)
        {
            var list = nsRenderedList?.GetKeyList() ?? throw new Exception("Invalid namespace list");
            foreach (XmlAttribute key in list) AddRendered(key);
        }

        internal void AddRendered(XmlAttribute attr)
        {
            GetCurrentScope()?.AddRendered(attr);
        }

        internal void AddUnrendered(XmlAttribute attr)
        {
            GetCurrentScope()?.AddUnrendered(attr);
        }
    }
}