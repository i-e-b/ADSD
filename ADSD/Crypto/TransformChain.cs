using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD
{
    /// <summary>
    /// A sequence of transforms
    /// </summary>
    public class TransformChain
    {
        private ArrayList m_transforms;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> class.</summary>
        public TransformChain()
        {
            this.m_transforms = new ArrayList();
        }

        /// <summary>Adds a transform to the list of transforms to be applied to the unsigned content prior to digest calculation.</summary>
        /// <param name="transform">The transform to add to the list of transforms. </param>
        public void Add(Transform transform)
        {
            if (transform == null)
                return;
            this.m_transforms.Add((object) transform);
        }

        /// <summary>Returns an enumerator of the transforms in the <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object.</summary>
        /// <returns>An enumerator of the transforms in the <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object.</returns>
        public IEnumerator GetEnumerator()
        {
            return this.m_transforms.GetEnumerator();
        }

        /// <summary>Gets the number of transforms in the <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object.</summary>
        /// <returns>The number of transforms in the <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object.</returns>
        public int Count
        {
            get
            {
                return this.m_transforms.Count;
            }
        }

        /// <summary>Gets the transform at the specified index in the <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object.</summary>
        /// <param name="index">The index into the <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object that specifies which transform to return. </param>
        /// <returns>The transform at the specified index in the <see cref="T:System.Security.Cryptography.Xml.TransformChain" /> object.</returns>
        /// <exception cref="T:System.ArgumentException">The <paramref name="index" /> parameter is greater than the number of transforms.</exception>
        public Transform this[int index]
        {
            get
            {
                if (index >= this.m_transforms.Count)
                    throw new ArgumentException("ArgumentOutOfRange: index");
                return (Transform) this.m_transforms[index];
            }
        }

        internal Stream TransformToOctetStream(
            object inputObject,
            Type inputType,
            XmlResolver resolver,
            string baseUri)
        {
            object obj = inputObject;
            foreach (Transform transform in this.m_transforms)
            {
                if (obj == null || transform.AcceptsType(obj.GetType()))
                {
                    transform.Resolver = resolver;
                    transform.BaseURI = baseUri;
                    transform.LoadInput(obj);
                    obj = transform.GetOutput();
                }
                else if (obj is Stream)
                {
                    if (!transform.AcceptsType(typeof (XmlDocument)))
                        throw new CryptographicException("Cryptography_Xml_TransformIncorrectInputType");
                    Stream inputStream = obj as Stream;
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.PreserveWhitespace = true;
                    XmlReader reader = Exml.PreProcessStreamInput(inputStream, resolver, baseUri);
                    xmlDocument.Load(reader);
                    transform.LoadInput((object) xmlDocument);
                    inputStream.Close();
                    obj = transform.GetOutput();
                }
                else if (obj is XmlNodeList)
                {
                    if (!transform.AcceptsType(typeof (Stream)))
                        throw new CryptographicException("Cryptography_Xml_TransformIncorrectInputType");
                    MemoryStream memoryStream = new MemoryStream(new CanonicalXml((XmlNodeList) obj, resolver, false).GetBytes());
                    transform.LoadInput((object) memoryStream);
                    obj = transform.GetOutput();
                    memoryStream.Close();
                }
                else
                {
                    if (!(obj is XmlDocument))
                        throw new CryptographicException("Cryptography_Xml_TransformIncorrectInputType");
                    if (!transform.AcceptsType(typeof (Stream)))
                        throw new CryptographicException("Cryptography_Xml_TransformIncorrectInputType");
                    MemoryStream memoryStream = new MemoryStream(new CanonicalXml((XmlDocument) obj, resolver).GetBytes());
                    transform.LoadInput((object) memoryStream);
                    obj = transform.GetOutput();
                    memoryStream.Close();
                }
            }
            if (obj is Stream)
                return obj as Stream;
            if (obj is XmlNodeList)
                return (Stream) new MemoryStream(new CanonicalXml((XmlNodeList) obj, resolver, false).GetBytes());
            if (obj is XmlDocument)
                return (Stream) new MemoryStream(new CanonicalXml((XmlDocument) obj, resolver).GetBytes());
            throw new CryptographicException("Cryptography_Xml_TransformIncorrectInputType");
        }

        internal Stream TransformToOctetStream(
            Stream input,
            XmlResolver resolver,
            string baseUri)
        {
            return this.TransformToOctetStream((object) input, typeof (Stream), resolver, baseUri);
        }

        internal Stream TransformToOctetStream(
            XmlDocument document,
            XmlResolver resolver,
            string baseUri)
        {
            return this.TransformToOctetStream((object) document, typeof (XmlDocument), resolver, baseUri);
        }

        internal XmlElement GetXml(XmlDocument document, string ns)
        {
            XmlElement element = document.CreateElement("Transforms", ns);
            foreach (Transform transform in this.m_transforms)
            {
                if (transform != null)
                {
                    XmlElement xml = transform.GetXml(document);
                    if (xml != null)
                        element.AppendChild((XmlNode) xml);
                }
            }
            return element;
        }

        internal void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            XmlNodeList xmlNodeList = value.SelectNodes("ds:Transform", nsmgr);
            if (xmlNodeList.Count == 0)
                throw new CryptographicException("Cryptography_Xml_InvalidElement: Transforms");
            this.m_transforms.Clear();
            for (int index = 0; index < xmlNodeList.Count; ++index)
            {
                XmlElement element = (XmlElement) xmlNodeList.Item(index);
                Transform fromName = Exml.CreateFromName<Transform>(Exml.GetAttribute(element, "Algorithm", "http://www.w3.org/2000/09/xmldsig#"));
                if (fromName == null)
                    throw new CryptographicException("Cryptography_Xml_UnknownTransform");
                fromName.LoadInnerXml(element.ChildNodes);
                this.m_transforms.Add((object) fromName);
            }
        }
    }
}