using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD
{
    /// <summary>
    /// XML Transform
    /// </summary>
    public abstract class Transform
    {
        private string m_algorithm;
        private string m_baseUri;
        internal XmlResolver m_xmlResolver;
        private bool m_bResolverSet;
        private SignedXml m_signedXml;
        private Reference m_reference;
        private Hashtable m_propagatedNamespaces;
        private XmlElement m_context;

        internal string BaseURI
        {
            get
            {
                return this.m_baseUri;
            }
            set
            {
                this.m_baseUri = value;
            }
        }

        internal SignedXml SignedXml
        {
            get
            {
                return this.m_signedXml;
            }
            set
            {
                this.m_signedXml = value;
            }
        }

        internal Reference Reference
        {
            get
            {
                return this.m_reference;
            }
            set
            {
                this.m_reference = value;
            }
        }

        /// <summary>Gets or sets the Uniform Resource Identifier (URI) that identifies the algorithm performed by the current transform.</summary>
        /// <returns>The URI that identifies the algorithm performed by the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
        public string Algorithm
        {
            get
            {
                return this.m_algorithm;
            }
            set
            {
                this.m_algorithm = value;
            }
        }

        /// <summary>Sets the current <see cref="T:System.Xml.XmlResolver" /> object.</summary>
        /// <returns>The current <see cref="T:System.Xml.XmlResolver" /> object. This property defaults to an <see cref="T:System.Xml.XmlSecureResolver" /> object.</returns>
        [ComVisible(false)]
        public XmlResolver Resolver
        {
            set
            {
                this.m_xmlResolver = value;
                this.m_bResolverSet = true;
            }
            internal get
            {
                return this.m_xmlResolver;
            }
        }

        internal bool ResolverSet
        {
            get
            {
                return this.m_bResolverSet;
            }
        }

        /// <summary>When overridden in a derived class, gets an array of types that are valid inputs to the <see cref="M:System.Security.Cryptography.Xml.Transform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
        /// <returns>An array of valid input types for the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object; you can pass only objects of one of these types to the <see cref="M:System.Security.Cryptography.Xml.Transform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
        public abstract Type[] InputTypes { get; }

        /// <summary>When overridden in a derived class, gets an array of types that are possible outputs from the <see cref="M:System.Security.Cryptography.Xml.Transform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
        /// <returns>An array of valid output types for the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object; only objects of one of these types are returned from the <see cref="M:System.Security.Cryptography.Xml.Transform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
        public abstract Type[] OutputTypes { get; }

        internal bool AcceptsType(Type inputType)
        {
            if (this.InputTypes != null)
            {
                for (int index = 0; index < this.InputTypes.Length; ++index)
                {
                    if (inputType == this.InputTypes[index] || inputType.IsSubclassOf(this.InputTypes[index]))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns the XML representation of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
        /// <returns>The XML representation of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
        public XmlElement GetXml()
        {
            return this.GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal XmlElement GetXml(XmlDocument document)
        {
            return this.GetXml(document, nameof (Transform));
        }

        internal XmlElement GetXml(XmlDocument document, string name)
        {
            XmlElement element = document.CreateElement(name, "http://www.w3.org/2000/09/xmldsig#");
            if (!string.IsNullOrEmpty(this.Algorithm))
                element.SetAttribute("Algorithm", this.Algorithm);
            XmlNodeList innerXml = this.GetInnerXml();
            if (innerXml != null)
            {
                foreach (XmlNode node in innerXml)
                    element.AppendChild(document.ImportNode(node, true));
            }
            return element;
        }

        /// <summary>When overridden in a derived class, parses the specified <see cref="T:System.Xml.XmlNodeList" /> object as transform-specific content of a <see langword="&lt;Transform&gt;" /> element and configures the internal state of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object to match the <see langword="&lt;Transform&gt;" /> element.</summary>
        /// <param name="nodeList">An <see cref="T:System.Xml.XmlNodeList" /> object that specifies transform-specific content for the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object. </param>
        public abstract void LoadInnerXml(XmlNodeList nodeList);

        /// <summary>When overridden in a derived class, returns an XML representation of the parameters of the <see cref="T:System.Security.Cryptography.Xml.Transform" /> object that are suitable to be included as subelements of an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</summary>
        /// <returns>A list of the XML nodes that represent the transform-specific content needed to describe the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object in an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</returns>
        protected abstract XmlNodeList GetInnerXml();

        /// <summary>When overridden in a derived class, loads the specified input into the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
        /// <param name="obj">The input to load into the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object. </param>
        public abstract void LoadInput(object obj);

        /// <summary>When overridden in a derived class, returns the output of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
        /// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
        public abstract object GetOutput();

        /// <summary>When overridden in a derived class, returns the output of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object of the specified type.</summary>
        /// <param name="type">The type of the output to return. This must be one of the types in the <see cref="P:System.Security.Cryptography.Xml.Transform.OutputTypes" /> property. </param>
        /// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object as an object of the specified type.</returns>
        public abstract object GetOutput(Type type);

        /// <summary>When overridden in a derived class, returns the digest associated with a <see cref="T:System.Security.Cryptography.Xml.Transform" /> object. </summary>
        /// <param name="hash">The <see cref="T:System.Security.Cryptography.HashAlgorithm" /> object used to create a digest.</param>
        /// <returns>The digest associated with a <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
        [ComVisible(false)]
        public virtual byte[] GetDigestedOutput(HashAlgorithm hash)
        {
            return hash.ComputeHash((Stream) this.GetOutput(typeof (Stream)));
        }

        /// <summary>Gets or sets an <see cref="T:System.Xml.XmlElement" /> object that represents the document context under which the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object is running. </summary>
        /// <returns>An <see cref="T:System.Xml.XmlElement" /> object that represents the document context under which the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object is running.</returns>
        [ComVisible(false)]
        public XmlElement Context
        {
            get
            {
                if (this.m_context != null)
                    return this.m_context;
                Reference reference = this.Reference;
                return (reference == null ? this.SignedXml : reference.SignedXml)?.m_context;
            }
            set
            {
                this.m_context = value;
            }
        }

        /// <summary>Gets or sets a <see cref="T:System.Collections.Hashtable" /> object that contains the namespaces that are propagated into the signature. </summary>
        /// <returns>A <see cref="T:System.Collections.Hashtable" /> object that contains the namespaces that are propagated into the signature.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Security.Cryptography.Xml.Transform.PropagatedNamespaces" /> property was set to <see langword="null" />.</exception>
        [ComVisible(false)]
        public Hashtable PropagatedNamespaces
        {
            get
            {
                if (this.m_propagatedNamespaces != null)
                    return this.m_propagatedNamespaces;
                Reference reference = this.Reference;
                SignedXml signedXml = reference == null ? this.SignedXml : reference.SignedXml;
                if (reference != null && (reference.ReferenceTargetType != ReferenceTargetType.UriReference || reference.Uri == null || (reference.Uri.Length == 0 || reference.Uri[0] != '#')))
                {
                    this.m_propagatedNamespaces = new Hashtable(0);
                    return this.m_propagatedNamespaces;
                }
                CanonicalXmlNodeList canonicalXmlNodeList = (CanonicalXmlNodeList) null;
                if (reference != null)
                    canonicalXmlNodeList = reference.m_namespaces;
                else if (signedXml.m_context != null)
                    canonicalXmlNodeList = CanonicalXmlNodeList.GetPropagatedAttributes(signedXml.m_context);
                if (canonicalXmlNodeList == null)
                {
                    this.m_propagatedNamespaces = new Hashtable(0);
                    return this.m_propagatedNamespaces;
                }
                this.m_propagatedNamespaces = new Hashtable(canonicalXmlNodeList.Count);
                foreach (XmlNode xmlNode in (XmlNodeList) canonicalXmlNodeList)
                {
                    string str = xmlNode.Prefix.Length > 0 ? xmlNode.Prefix + ":" + xmlNode.LocalName : xmlNode.LocalName;
                    if (!this.m_propagatedNamespaces.Contains((object) str))
                        this.m_propagatedNamespaces.Add((object) str, (object) xmlNode.Value);
                }
                return this.m_propagatedNamespaces;
            }
        }
    }
}