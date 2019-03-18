using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>Represents an <see langword="&lt;X509Data&gt;" /> subelement of an XMLDSIG or XML Encryption <see langword="&lt;KeyInfo&gt;" /> element.</summary>
    public class KeyInfoX509Data : KeyInfoClause
    {
        private ArrayList m_certificates;
        private ArrayList m_issuerSerials;
        private ArrayList m_subjectKeyIds;
        private ArrayList m_subjectNames;
        private byte[] m_CRL;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> class.</summary>
        public KeyInfoX509Data()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> class from the specified ASN.1 DER encoding of an X.509v3 certificate.</summary>
        /// <param name="rgbCert">The ASN.1 DER encoding of an <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate" /> object to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> from.</param>
        public KeyInfoX509Data(byte[] rgbCert)
        {
            this.AddCertificate((X509Certificate) new X509Certificate2(rgbCert));
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> class from the specified X.509v3 certificate.</summary>
        /// <param name="cert">The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate" /> object to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> from.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="cert" /> parameter is <see langword="null" />.</exception>
        public KeyInfoX509Data(X509Certificate cert)
        {
            this.AddCertificate(cert);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> class from the specified X.509v3 certificate.</summary>
        /// <param name="cert">The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate" /> object to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> from.</param>
        /// <param name="includeOption">One of the <see cref="T:System.Security.Cryptography.X509Certificates.X509IncludeOption" /> values that specifies how much of the certificate chain to include.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="cert" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The certificate has only a partial certificate chain.</exception>
        [SecuritySafeCritical]
        public KeyInfoX509Data(X509Certificate cert, X509IncludeOption includeOption)
        {
            if (cert == null)
                throw new ArgumentNullException(nameof (cert));
            X509Certificate2 certificate = new X509Certificate2(cert);
            switch (includeOption)
            {
                case X509IncludeOption.ExcludeRoot:
                    X509Chain chain = new X509Chain();
                    chain.Build(certificate);
                    if (chain.ChainStatus.Length != 0 && (chain.ChainStatus[0].Status & X509ChainStatusFlags.PartialChain) == X509ChainStatusFlags.PartialChain)
                        throw new CryptographicException(-2146762486);
                    X509ChainElementCollection chainElements = chain.ChainElements;
                    for (int index = 0; index < (X509Utils.IsSelfSigned(chain) ? 1 : chainElements.Count - 1); ++index)
                        this.AddCertificate((X509Certificate) chainElements[index].Certificate);
                    break;
                case X509IncludeOption.EndCertOnly:
                    this.AddCertificate((X509Certificate) certificate);
                    break;
                case X509IncludeOption.WholeChain:
                    X509Chain x509Chain = new X509Chain();
                    x509Chain.Build(certificate);
                    if (x509Chain.ChainStatus.Length != 0 && (x509Chain.ChainStatus[0].Status & X509ChainStatusFlags.PartialChain) == X509ChainStatusFlags.PartialChain)
                        throw new CryptographicException(-2146762486);
                    foreach (X509ChainElement chainElement in x509Chain.ChainElements)
                        this.AddCertificate((X509Certificate) chainElement.Certificate);
                    break;
            }
        }

        /// <summary>Gets a list of the X.509v3 certificates contained in the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</summary>
        /// <returns>A list of the X.509 certificates contained in the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</returns>
        public ArrayList Certificates
        {
            get
            {
                return this.m_certificates;
            }
        }

        /// <summary>Adds the specified X.509v3 certificate to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" />.</summary>
        /// <param name="certificate">The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate" /> object to add to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="certificate" /> parameter is <see langword="null" />.</exception>
        public void AddCertificate(X509Certificate certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException(nameof (certificate));
            if (this.m_certificates == null)
                this.m_certificates = new ArrayList();
            this.m_certificates.Add((object) new X509Certificate2(certificate));
        }

        /// <summary>Gets a list of the subject key identifiers (SKIs) contained in the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</summary>
        /// <returns>A list of the subject key identifiers (SKIs) contained in the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</returns>
        public ArrayList SubjectKeyIds
        {
            get
            {
                return this.m_subjectKeyIds;
            }
        }

        /// <summary>Adds the specified subject key identifier (SKI) byte array to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</summary>
        /// <param name="subjectKeyId">A byte array that represents the subject key identifier (SKI) to add to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object. </param>
        public void AddSubjectKeyId(byte[] subjectKeyId)
        {
            if (this.m_subjectKeyIds == null)
                this.m_subjectKeyIds = new ArrayList();
            this.m_subjectKeyIds.Add((object) subjectKeyId);
        }

        /// <summary>Adds the specified subject key identifier (SKI) string to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</summary>
        /// <param name="subjectKeyId">A string that represents the subject key identifier (SKI) to add to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</param>
        [ComVisible(false)]
        public void AddSubjectKeyId(string subjectKeyId)
        {
            if (this.m_subjectKeyIds == null)
                this.m_subjectKeyIds = new ArrayList();
            this.m_subjectKeyIds.Add((object) X509Utils.DecodeHexString(subjectKeyId));
        }

        /// <summary>Gets a list of the subject names of the entities contained in the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</summary>
        /// <returns>A list of the subject names of the entities contained in the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</returns>
        public ArrayList SubjectNames
        {
            get
            {
                return this.m_subjectNames;
            }
        }

        /// <summary>Adds the subject name of the entity that was issued an X.509v3 certificate to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</summary>
        /// <param name="subjectName">The name of the entity that was issued an X.509 certificate to add to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object. </param>
        public void AddSubjectName(string subjectName)
        {
            if (this.m_subjectNames == null)
                this.m_subjectNames = new ArrayList();
            this.m_subjectNames.Add((object) subjectName);
        }

        /// <summary>Gets a list of <see cref="T:System.Security.Cryptography.Xml.X509IssuerSerial" /> structures that represent an issuer name and serial number pair.</summary>
        /// <returns>A list of <see cref="T:System.Security.Cryptography.Xml.X509IssuerSerial" /> structures that represent an issuer name and serial number pair.</returns>
        public ArrayList IssuerSerials
        {
            get
            {
                return this.m_issuerSerials;
            }
        }

        /// <summary>Adds the specified issuer name and serial number pair to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</summary>
        /// <param name="issuerName">The issuer name portion of the pair to add to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object. </param>
        /// <param name="serialNumber">The serial number portion of the pair to add to the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object. </param>
        public void AddIssuerSerial(string issuerName, string serialNumber)
        {
            BigInt bigInt = new BigInt();
            bigInt.FromHexadecimal(serialNumber);
            if (this.m_issuerSerials == null)
                this.m_issuerSerials = new ArrayList();
            this.m_issuerSerials.Add((object) new X509IssuerSerial(issuerName, bigInt.ToDecimal()));
        }

        internal void InternalAddIssuerSerial(string issuerName, string serialNumber)
        {
            if (this.m_issuerSerials == null)
                this.m_issuerSerials = new ArrayList();
            this.m_issuerSerials.Add((object) new X509IssuerSerial(issuerName, serialNumber));
        }

        /// <summary>Gets or sets the Certificate Revocation List (CRL) contained within the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</summary>
        /// <returns>The Certificate Revocation List (CRL) contained within the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</returns>
        public byte[] CRL
        {
            get
            {
                return this.m_CRL;
            }
            set
            {
                this.m_CRL = value;
            }
        }

        private void Clear()
        {
            this.m_CRL = (byte[]) null;
            if (this.m_subjectKeyIds != null)
                this.m_subjectKeyIds.Clear();
            if (this.m_subjectNames != null)
                this.m_subjectNames.Clear();
            if (this.m_issuerSerials != null)
                this.m_issuerSerials.Clear();
            if (this.m_certificates == null)
                return;
            this.m_certificates.Clear();
        }

        /// <summary>Returns an XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</summary>
        /// <returns>An XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object.</returns>
        public override XmlElement GetXml()
        {
            return this.GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal override XmlElement GetXml(XmlDocument xmlDocument)
        {
            XmlElement element1 = xmlDocument.CreateElement("X509Data", "http://www.w3.org/2000/09/xmldsig#");
            if (this.m_issuerSerials != null)
            {
                foreach (X509IssuerSerial issuerSerial in this.m_issuerSerials)
                {
                    XmlElement element2 = xmlDocument.CreateElement("X509IssuerSerial", "http://www.w3.org/2000/09/xmldsig#");
                    XmlElement element3 = xmlDocument.CreateElement("X509IssuerName", "http://www.w3.org/2000/09/xmldsig#");
                    element3.AppendChild((XmlNode) xmlDocument.CreateTextNode(issuerSerial.IssuerName));
                    element2.AppendChild((XmlNode) element3);
                    XmlElement element4 = xmlDocument.CreateElement("X509SerialNumber", "http://www.w3.org/2000/09/xmldsig#");
                    element4.AppendChild((XmlNode) xmlDocument.CreateTextNode(issuerSerial.SerialNumber));
                    element2.AppendChild((XmlNode) element4);
                    element1.AppendChild((XmlNode) element2);
                }
            }
            if (this.m_subjectKeyIds != null)
            {
                foreach (byte[] subjectKeyId in this.m_subjectKeyIds)
                {
                    XmlElement element2 = xmlDocument.CreateElement("X509SKI", "http://www.w3.org/2000/09/xmldsig#");
                    element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(subjectKeyId)));
                    element1.AppendChild((XmlNode) element2);
                }
            }
            if (this.m_subjectNames != null)
            {
                foreach (string subjectName in this.m_subjectNames)
                {
                    XmlElement element2 = xmlDocument.CreateElement("X509SubjectName", "http://www.w3.org/2000/09/xmldsig#");
                    element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(subjectName));
                    element1.AppendChild((XmlNode) element2);
                }
            }
            if (this.m_certificates != null)
            {
                foreach (X509Certificate certificate in this.m_certificates)
                {
                    XmlElement element2 = xmlDocument.CreateElement("X509Certificate", "http://www.w3.org/2000/09/xmldsig#");
                    element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(certificate.GetRawCertData())));
                    element1.AppendChild((XmlNode) element2);
                }
            }
            if (this.m_CRL != null)
            {
                XmlElement element2 = xmlDocument.CreateElement("X509CRL", "http://www.w3.org/2000/09/xmldsig#");
                element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(Convert.ToBase64String(this.m_CRL)));
                element1.AppendChild((XmlNode) element2);
            }
            return element1;
        }

        /// <summary>Parses the input <see cref="T:System.Xml.XmlElement" /> object and configures the internal state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object to match.</summary>
        /// <param name="element">The <see cref="T:System.Xml.XmlElement" /> object that specifies the state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoX509Data" /> object. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="element" /> parameter does not contain an &lt;<see langword="X509IssuerName" />&gt; node.-or-The <paramref name="element" /> parameter does not contain an &lt;<see langword="X509SerialNumber" />&gt; node.</exception>
        public override void LoadXml(XmlElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof (element));
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(element.OwnerDocument.NameTable);
            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            XmlNodeList xmlNodeList1 = element.SelectNodes("ds:X509IssuerSerial", nsmgr);
            XmlNodeList xmlNodeList2 = element.SelectNodes("ds:X509SKI", nsmgr);
            XmlNodeList xmlNodeList3 = element.SelectNodes("ds:X509SubjectName", nsmgr);
            XmlNodeList xmlNodeList4 = element.SelectNodes("ds:X509Certificate", nsmgr);
            XmlNodeList xmlNodeList5 = element.SelectNodes("ds:X509CRL", nsmgr);
            if (xmlNodeList5.Count == 0 && xmlNodeList1.Count == 0 && (xmlNodeList2.Count == 0 && xmlNodeList3.Count == 0) && xmlNodeList4.Count == 0)
                throw new CryptographicException("Invalid XML element: X509Data");
            this.Clear();
            if (xmlNodeList5.Count != 0)
                this.m_CRL = Convert.FromBase64String(Exml.DiscardWhiteSpaces(xmlNodeList5.Item(0).InnerText));
            foreach (XmlNode xmlNode1 in xmlNodeList1)
            {
                XmlNode xmlNode2 = xmlNode1.SelectSingleNode("ds:X509IssuerName", nsmgr);
                XmlNode xmlNode3 = xmlNode1.SelectSingleNode("ds:X509SerialNumber", nsmgr);
                if (xmlNode2 == null || xmlNode3 == null)
                    throw new CryptographicException("Invalid XML element: IssuerSerial");
                this.InternalAddIssuerSerial(xmlNode2.InnerText.Trim(), xmlNode3.InnerText.Trim());
            }
            foreach (XmlNode xmlNode in xmlNodeList2)
                this.AddSubjectKeyId(Convert.FromBase64String(Exml.DiscardWhiteSpaces(xmlNode.InnerText)));
            foreach (XmlNode xmlNode in xmlNodeList3)
                this.AddSubjectName(xmlNode.InnerText.Trim());
            foreach (XmlNode xmlNode in xmlNodeList4)
                this.AddCertificate((X509Certificate) new X509Certificate2(Convert.FromBase64String(Exml.DiscardWhiteSpaces(xmlNode.InnerText))));
        }
    }
}