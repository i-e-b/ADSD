using System;
using System.Collections;
using System.Xml;

namespace ADSD.Crypto
{
    internal abstract class AncestralNamespaceContextManager
    {
        internal ArrayList m_ancestorStack = new ArrayList();

        internal NamespaceFrame GetScopeAt(int i)
        {
            return (NamespaceFrame) this.m_ancestorStack[i];
        }

        internal NamespaceFrame GetCurrentScope()
        {
            return this.GetScopeAt(this.m_ancestorStack.Count - 1);
        }

        protected XmlAttribute GetNearestRenderedNamespaceWithMatchingPrefix(
            string nsPrefix,
            out int depth)
        {
            depth = -1;
            for (int i = this.m_ancestorStack.Count - 1; i >= 0; --i)
            {
                XmlAttribute rendered;
                if ((rendered = this.GetScopeAt(i).GetRendered(nsPrefix)) != null)
                {
                    depth = i;
                    return rendered;
                }
            }
            return (XmlAttribute) null;
        }

        protected XmlAttribute GetNearestUnrenderedNamespaceWithMatchingPrefix(
            string nsPrefix,
            out int depth)
        {
            depth = -1;
            for (int i = this.m_ancestorStack.Count - 1; i >= 0; --i)
            {
                XmlAttribute unrendered;
                if ((unrendered = this.GetScopeAt(i).GetUnrendered(nsPrefix)) != null)
                {
                    depth = i;
                    return unrendered;
                }
            }
            return (XmlAttribute) null;
        }

        internal void EnterElementContext()
        {
            this.m_ancestorStack.Add((object) new NamespaceFrame());
        }

        internal void ExitElementContext()
        {
            this.m_ancestorStack.RemoveAt(this.m_ancestorStack.Count - 1);
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
            object[] objArray = new object[nsLocallyDeclared.Count];
            nsLocallyDeclared.Values.CopyTo((Array) objArray, 0);
            foreach (XmlAttribute attr in objArray)
                this.AddUnrendered(attr);
        }

        internal void LoadRenderedNamespaces(SortedList nsRenderedList)
        {
            foreach (XmlAttribute key in (IEnumerable) nsRenderedList.GetKeyList())
                this.AddRendered(key);
        }

        internal void AddRendered(XmlAttribute attr)
        {
            this.GetCurrentScope().AddRendered(attr);
        }

        internal void AddUnrendered(XmlAttribute attr)
        {
            this.GetCurrentScope().AddUnrendered(attr);
        }
    }
}