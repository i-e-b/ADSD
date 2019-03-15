using System.Collections;
using System.Xml;

namespace ADSD
{
    internal class NamespaceFrame
    {
        private Hashtable m_rendered = new Hashtable();
        private Hashtable m_unrendered = new Hashtable();

        internal NamespaceFrame()
        {
        }

        internal void AddRendered(XmlAttribute attr)
        {
            this.m_rendered.Add((object) Exml.GetNamespacePrefix(attr), (object) attr);
        }

        internal XmlAttribute GetRendered(string nsPrefix)
        {
            return (XmlAttribute) this.m_rendered[(object) nsPrefix];
        }

        internal void AddUnrendered(XmlAttribute attr)
        {
            this.m_unrendered.Add((object) Exml.GetNamespacePrefix(attr), (object) attr);
        }

        internal XmlAttribute GetUnrendered(string nsPrefix)
        {
            return (XmlAttribute) this.m_unrendered[(object) nsPrefix];
        }

        internal Hashtable GetUnrendered()
        {
            return this.m_unrendered;
        }
    }
}