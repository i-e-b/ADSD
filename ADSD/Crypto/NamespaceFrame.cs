using System.Collections;
using System.Xml;

namespace ADSD.Crypto
{
    internal class NamespaceFrame
    {
        private readonly Hashtable m_rendered = new Hashtable();
        private readonly Hashtable m_unrendered = new Hashtable();

        internal NamespaceFrame()
        {
        }

        internal void AddRendered(XmlAttribute attr)
        {
            m_rendered.Add((object) Exml.GetNamespacePrefix(attr), (object) attr);
        }

        internal XmlAttribute GetRendered(string nsPrefix)
        {
            return (XmlAttribute) m_rendered[(object) nsPrefix];
        }

        internal void AddUnrendered(XmlAttribute attr)
        {
            m_unrendered.Add((object) Exml.GetNamespacePrefix(attr), (object) attr);
        }

        internal XmlAttribute GetUnrendered(string nsPrefix)
        {
            return (XmlAttribute) m_unrendered[(object) nsPrefix];
        }

        internal Hashtable GetUnrendered()
        {
            return m_unrendered;
        }
    }
}