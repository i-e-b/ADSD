using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace ADSD
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EncryptedData : EncryptedType
  {
    /// <summary>Loads XML information into the <see langword="&lt;EncryptedData&gt;" /> element in XML encryption.</summary>
    /// <param name="value">An <see cref="T:System.Xml.XmlElement" /> object representing an XML element to use for the <see langword="&lt;EncryptedData&gt;" /> element.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> provided is <see langword="null" />.</exception>
    /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="value" /> parameter does not contain a &lt;<see langword="CypherData" />&gt; node.</exception>
    public override void LoadXml(XmlElement value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
      nsmgr.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
      nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
      this.Id = Exml.GetAttribute(value, "Id", "http://www.w3.org/2001/04/xmlenc#");
      this.Type = Exml.GetAttribute(value, "Type", "http://www.w3.org/2001/04/xmlenc#");
      this.MimeType = Exml.GetAttribute(value, "MimeType", "http://www.w3.org/2001/04/xmlenc#");
      this.Encoding = Exml.GetAttribute(value, "Encoding", "http://www.w3.org/2001/04/xmlenc#");
      XmlNode xmlNode1 = value.SelectSingleNode("enc:EncryptionMethod", nsmgr);
      this.EncryptionMethod = new EncryptionMethod();
      if (xmlNode1 != null)
        this.EncryptionMethod.LoadXml(xmlNode1 as XmlElement);
      this.KeyInfo = new KeyInfo();
      XmlNode xmlNode2 = value.SelectSingleNode("ds:KeyInfo", nsmgr);
      if (xmlNode2 != null)
        this.KeyInfo.LoadXml(xmlNode2 as XmlElement);
      XmlNode xmlNode3 = value.SelectSingleNode("enc:CipherData", nsmgr);
      if (xmlNode3 == null)
        throw new CryptographicException("Cryptography_Xml_MissingCipherData");
      this.CipherData = new CipherData();
      this.CipherData.LoadXml(xmlNode3 as XmlElement);
      XmlNode xmlNode4 = value.SelectSingleNode("enc:EncryptionProperties", nsmgr);
      if (xmlNode4 != null)
      {
        XmlNodeList xmlNodeList = xmlNode4.SelectNodes("enc:EncryptionProperty", nsmgr);
        if (xmlNodeList != null)
        {
          foreach (XmlNode xmlNode5 in xmlNodeList)
          {
            EncryptionProperty encryptionProperty = new EncryptionProperty();
            encryptionProperty.LoadXml(xmlNode5 as XmlElement);
            this.EncryptionProperties.Add(encryptionProperty);
          }
        }
      }
      this.m_cachedXml = value;
    }

    /// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> object.</summary>
    /// <returns>An <see cref="T:System.Xml.XmlElement" /> that represents the <see langword="&lt;EncryptedData&gt;" /> element in XML encryption.</returns>
    /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> value is <see langword="null" />.</exception>
    public override XmlElement GetXml()
    {
      if (this.CacheValid)
        return this.m_cachedXml;
      return this.GetXml(new XmlDocument()
      {
        PreserveWhitespace = true
      });
    }

    internal XmlElement GetXml(XmlDocument document)
    {
      XmlElement element1 = document.CreateElement(nameof (EncryptedData), "http://www.w3.org/2001/04/xmlenc#");
      if (!string.IsNullOrEmpty(this.Id))
        element1.SetAttribute("Id", this.Id);
      if (!string.IsNullOrEmpty(this.Type))
        element1.SetAttribute("Type", this.Type);
      if (!string.IsNullOrEmpty(this.MimeType))
        element1.SetAttribute("MimeType", this.MimeType);
      if (!string.IsNullOrEmpty(this.Encoding))
        element1.SetAttribute("Encoding", this.Encoding);
      if (this.EncryptionMethod != null)
        element1.AppendChild((XmlNode) this.EncryptionMethod.GetXml(document));
      if (this.KeyInfo.Count > 0)
        element1.AppendChild((XmlNode) this.KeyInfo.GetXml(document));
      if (this.CipherData == null)
        throw new CryptographicException("Cryptography_Xml_MissingCipherData");
      element1.AppendChild((XmlNode) this.CipherData.GetXml(document));
      if (this.EncryptionProperties.Count > 0)
      {
        XmlElement element2 = document.CreateElement("EncryptionProperties", "http://www.w3.org/2001/04/xmlenc#");
        for (int index = 0; index < this.EncryptionProperties.Count; ++index)
        {
          EncryptionProperty encryptionProperty = this.EncryptionProperties.Item(index);
          element2.AppendChild((XmlNode) encryptionProperty.GetXml(document));
        }
        element1.AppendChild((XmlNode) element2);
      }
      return element1;
    }
  }
    /// <summary>Represents the process model for implementing XML encryption.</summary>
  public class EncryptedXml
  {
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for XML encryption syntax and processing. This field is constant.</summary>
    public const string XmlEncNamespaceUrl = "http://www.w3.org/2001/04/xmlenc#";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for an XML encryption element. This field is constant.</summary>
    public const string XmlEncElementUrl = "http://www.w3.org/2001/04/xmlenc#Element";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for XML encryption element content. This field is constant.</summary>
    public const string XmlEncElementContentUrl = "http://www.w3.org/2001/04/xmlenc#Content";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the XML encryption <see langword="&lt;EncryptedKey&gt;" /> element. This field is constant.</summary>
    public const string XmlEncEncryptedKeyUrl = "http://www.w3.org/2001/04/xmlenc#EncryptedKey";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the Digital Encryption Standard (DES) algorithm. This field is constant.</summary>
    public const string XmlEncDESUrl = "http://www.w3.org/2001/04/xmlenc#des-cbc";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the Triple DES algorithm. This field is constant.</summary>
    public const string XmlEncTripleDESUrl = "http://www.w3.org/2001/04/xmlenc#tripledes-cbc";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the 128-bit Advanced Encryption Standard (AES) algorithm (also known as the Rijndael algorithm). This field is constant.</summary>
    public const string XmlEncAES128Url = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the 256-bit Advanced Encryption Standard (AES) algorithm (also known as the Rijndael algorithm). This field is constant.</summary>
    public const string XmlEncAES256Url = "http://www.w3.org/2001/04/xmlenc#aes256-cbc";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the 192-bit Advanced Encryption Standard (AES) algorithm (also known as the Rijndael algorithm). This field is constant.</summary>
    public const string XmlEncAES192Url = "http://www.w3.org/2001/04/xmlenc#aes192-cbc";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the RSA Public Key Cryptography Standard (PKCS) Version 1.5 algorithm. This field is constant.</summary>
    public const string XmlEncRSA15Url = "http://www.w3.org/2001/04/xmlenc#rsa-1_5";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the RSA Optimal Asymmetric Encryption Padding (OAEP) encryption algorithm. This field is constant.</summary>
    public const string XmlEncRSAOAEPUrl = "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the TRIPLEDES key wrap algorithm. This field is constant.</summary>
    public const string XmlEncTripleDESKeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-tripledes";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the 128-bit Advanced Encryption Standard (AES) Key Wrap algorithm (also known as the Rijndael Key Wrap algorithm). This field is constant. </summary>
    public const string XmlEncAES128KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes128";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the 256-bit Advanced Encryption Standard (AES) Key Wrap algorithm (also known as the Rijndael Key Wrap algorithm). This field is constant.</summary>
    public const string XmlEncAES256KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes256";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the 192-bit Advanced Encryption Standard (AES) Key Wrap algorithm (also known as the Rijndael Key Wrap algorithm). This field is constant.</summary>
    public const string XmlEncAES192KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes192";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the SHA-256 algorithm. This field is constant.</summary>
    public const string XmlEncSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";
    /// <summary>Represents the namespace Uniform Resource Identifier (URI) for the SHA-512 algorithm. This field is constant.</summary>
    public const string XmlEncSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";
    private XmlDocument m_document;
    private Evidence m_evidence;
    private XmlResolver m_xmlResolver;
    private const int m_capacity = 4;
    private Hashtable m_keyNameMapping;
    private PaddingMode m_padding;
    private CipherMode m_mode;
    private Encoding m_encoding;
    private string m_recipient;
    private int m_xmlDsigSearchDepthCounter;
    private int m_xmlDsigSearchDepth;

    /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> class.</summary>
    public EncryptedXml()
      : this(new XmlDocument())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> class using the specified XML document.</summary>
    /// <param name="document">An <see cref="T:System.Xml.XmlDocument" /> object used to initialize the <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> object.</param>
    public EncryptedXml(XmlDocument document)
      : this(document, (Evidence) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> class using the specified XML document and evidence.</summary>
    /// <param name="document">An <see cref="T:System.Xml.XmlDocument" /> object used to initialize the <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> object.</param>
    /// <param name="evidence">An <see cref="T:System.Security.Policy.Evidence" /> object associated with the <see cref="T:System.Xml.XmlDocument" /> object.</param>
    public EncryptedXml(XmlDocument document, Evidence evidence)
    {
      this.m_document = document;
      this.m_evidence = evidence;
      this.m_xmlResolver = (XmlResolver) null;
      this.m_padding = PaddingMode.ISO10126;
      this.m_mode = CipherMode.CBC;
      this.m_encoding = Encoding.UTF8;
      this.m_keyNameMapping = new Hashtable(4);
      this.m_xmlDsigSearchDepth = 20;
    }

    private bool IsOverXmlDsigRecursionLimit()
    {
      return this.m_xmlDsigSearchDepthCounter > this.XmlDSigSearchDepth;
    }

    /// <summary>Gets or sets the XML digital signature recursion depth to prevent infinite recursion and stack overflow. This might happen if the digital signature XML contains the URI which then points back to the original XML. </summary>
    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public int XmlDSigSearchDepth
    {
      get
      {
        return this.m_xmlDsigSearchDepth;
      }
      set
      {
        this.m_xmlDsigSearchDepth = value;
      }
    }

    /// <summary>Gets or sets the evidence of the <see cref="T:System.Xml.XmlDocument" /> object from which the <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> object is constructed.</summary>
    /// <returns>An <see cref="T:System.Security.Policy.Evidence" /> object.</returns>
    public Evidence DocumentEvidence
    {
      get
      {
        return this.m_evidence;
      }
      set
      {
        this.m_evidence = value;
      }
    }

    /// <summary>Gets or sets the <see cref="T:System.Xml.XmlResolver" /> object used by the Document Object Model (DOM) to resolve external XML references.</summary>
    /// <returns>An <see cref="T:System.Xml.XmlResolver" /> object.</returns>
    public XmlResolver Resolver
    {
      get
      {
        return this.m_xmlResolver;
      }
      set
      {
        this.m_xmlResolver = value;
      }
    }

    /// <summary>Gets or sets the padding mode used for XML encryption.</summary>
    /// <returns>One of the <see cref="T:System.Security.Cryptography.PaddingMode" /> values that specifies the type of padding used for encryption.</returns>
    public PaddingMode Padding
    {
      get
      {
        return this.m_padding;
      }
      set
      {
        this.m_padding = value;
      }
    }

    /// <summary>Gets or sets the cipher mode used for XML encryption.</summary>
    /// <returns>One of the <see cref="T:System.Security.Cryptography.CipherMode" /> values.</returns>
    public CipherMode Mode
    {
      get
      {
        return this.m_mode;
      }
      set
      {
        this.m_mode = value;
      }
    }

    /// <summary>Gets or sets the encoding used for XML encryption.</summary>
    /// <returns>An <see cref="T:System.Text.Encoding" /> object.</returns>
    public Encoding Encoding
    {
      get
      {
        return this.m_encoding;
      }
      set
      {
        this.m_encoding = value;
      }
    }

    /// <summary>Gets or sets the recipient of the encrypted key information.</summary>
    /// <returns>The recipient of the encrypted key information.</returns>
    public string Recipient
    {
      get
      {
        if (this.m_recipient == null)
          this.m_recipient = string.Empty;
        return this.m_recipient;
      }
      set
      {
        this.m_recipient = value;
      }
    }

    private byte[] GetCipherValue(CipherData cipherData)
    {
      if (cipherData == null)
        throw new ArgumentNullException(nameof (cipherData));
      WebResponse response = (WebResponse) null;
      Stream inputStream = (Stream) null;
      if (cipherData.CipherValue != null)
        return cipherData.CipherValue;
      if (cipherData.CipherReference == null)
        throw new CryptographicException("Cryptography_Xml_MissingCipherData");
      if (cipherData.CipherReference.CipherValue != null)
        return cipherData.CipherReference.CipherValue;
      Stream decInputStream = (Stream) null;
      if (cipherData.CipherReference.Uri == null)
        throw new CryptographicException("Cryptography_Xml_UriNotSupported");
      if (cipherData.CipherReference.Uri.Length == 0)
      {
        string baseUri = this.m_document == null ? (string) null : this.m_document.BaseURI;
        TransformChain transformChain = cipherData.CipherReference.TransformChain;
        if (transformChain == null)
          throw new CryptographicException("Cryptography_Xml_UriNotSupported");
        decInputStream = transformChain.TransformToOctetStream(this.m_document, this.m_xmlResolver, baseUri);
      }
      else if (cipherData.CipherReference.Uri[0] == '#')
      {
        string idFromLocalUri = Exml.ExtractIdFromLocalUri(cipherData.CipherReference.Uri);
       
          XmlElement idElement = this.GetIdElement(this.m_document, idFromLocalUri);
          if (idElement == null || idElement.OuterXml == null)
            throw new CryptographicException("Cryptography_Xml_UriNotSupported");
          inputStream = (Stream) new MemoryStream(this.m_encoding.GetBytes(idElement.OuterXml));
        
        string baseUri = this.m_document == null ? (string) null : this.m_document.BaseURI;
        TransformChain transformChain = cipherData.CipherReference.TransformChain;
        if (transformChain == null)
          throw new CryptographicException("Cryptography_Xml_UriNotSupported");
        decInputStream = transformChain.TransformToOctetStream(inputStream, this.m_xmlResolver, baseUri);
      }
      else
        this.DownloadCipherValue(cipherData, out inputStream, out decInputStream, out response);
      byte[] numArray = (byte[]) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        Pump(decInputStream, (Stream) memoryStream);
        numArray = memoryStream.ToArray();
        response?.Close();
        inputStream?.Close();
        decInputStream.Close();
      }
      cipherData.CipherReference.CipherValue = numArray;
      return numArray;
    }

    internal static long Pump(Stream input, Stream output)
    {
        MemoryStream memoryStream = input as MemoryStream;
        if (memoryStream != null && memoryStream.Position == 0L)
        {
            memoryStream.WriteTo(output);
            return memoryStream.Length;
        }
        byte[] buffer = new byte[4096];
        long num = 0;
        int count;
        while ((count = input.Read(buffer, 0, 4096)) > 0)
        {
            output.Write(buffer, 0, count);
            num += (long) count;
        }
        return num;
    }

    private void DownloadCipherValue(
      CipherData cipherData,
      out Stream inputStream,
      out Stream decInputStream,
      out WebResponse response)
    {
      WebRequest webRequest = WebRequest.Create(cipherData.CipherReference.Uri);
      if (webRequest == null)
        throw new CryptographicException("Cryptography_Xml_UriNotResolved (1) "+cipherData.CipherReference.Uri);
      response = webRequest.GetResponse();
      if (response == null)
        throw new CryptographicException("Cryptography_Xml_UriNotResolved (2) "+cipherData.CipherReference.Uri);
      inputStream = response.GetResponseStream();
      if (inputStream == null)
        throw new CryptographicException("Cryptography_Xml_UriNotResolved (3) "+cipherData.CipherReference.Uri);
      TransformChain transformChain = cipherData.CipherReference.TransformChain;
      decInputStream = transformChain.TransformToOctetStream(inputStream, this.m_xmlResolver, cipherData.CipherReference.Uri);
    }

    /// <summary>Determines how to resolve internal Uniform Resource Identifier (URI) references.</summary>
    /// <param name="document">An <see cref="T:System.Xml.XmlDocument" /> object that contains an element with an ID value.</param>
    /// <param name="idValue">A string that represents the ID value.</param>
    /// <returns>An <see cref="T:System.Xml.XmlElement" /> object that contains an ID indicating how internal Uniform Resource Identifiers (URIs) are to be resolved.</returns>
    public virtual XmlElement GetIdElement(XmlDocument document, string idValue)
    {
      return SignedXml.DefaultGetIdElement(document, idValue);
    }

    /// <summary>Retrieves the decryption initialization vector (IV) from an <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> object.</summary>
    /// <param name="encryptedData">The <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> object that contains the initialization vector (IV) to retrieve.</param>
    /// <param name="symmetricAlgorithmUri">The Uniform Resource Identifier (URI) that describes the cryptographic algorithm associated with the <paramref name="encryptedData" /> value.</param>
    /// <returns>A byte array that contains the decryption initialization vector (IV).</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="encryptedData" /> parameter is <see langword="null" />.</exception>
    public virtual byte[] GetDecryptionIV(EncryptedData encryptedData, string symmetricAlgorithmUri)
    {
      if (encryptedData == null)
        throw new ArgumentNullException(nameof (encryptedData));
      if (symmetricAlgorithmUri == null)
      {
        if (encryptedData.EncryptionMethod == null)
          throw new CryptographicException("Cryptography_Xml_MissingAlgorithm");
        symmetricAlgorithmUri = encryptedData.EncryptionMethod.KeyAlgorithm;
      }
      int length;
      if (symmetricAlgorithmUri != "http://www.w3.org/2001/04/xmlenc#des-cbc" && symmetricAlgorithmUri != "http://www.w3.org/2001/04/xmlenc#tripledes-cbc")
      {
        if (symmetricAlgorithmUri != "http://www.w3.org/2001/04/xmlenc#aes128-cbc" && symmetricAlgorithmUri != "http://www.w3.org/2001/04/xmlenc#aes192-cbc" && symmetricAlgorithmUri != "http://www.w3.org/2001/04/xmlenc#aes256-cbc")
          throw new CryptographicException("Cryptography_Xml_UriNotSupported");
        length = 16;
      }
      else
        length = 8;
      byte[] numArray = new byte[length];
      Buffer.BlockCopy((Array) this.GetCipherValue(encryptedData.CipherData), 0, (Array) numArray, 0, numArray.Length);
      return numArray;
    }

    /// <summary>Retrieves the decryption key from the specified <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> object.</summary>
    /// <param name="encryptedData">The <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> object that contains the decryption key to retrieve.</param>
    /// <param name="symmetricAlgorithmUri">The size of the decryption key to retrieve.</param>
    /// <returns>A <see cref="T:System.Security.Cryptography.SymmetricAlgorithm" /> object associated with the decryption key.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="encryptedData" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.Security.Cryptography.CryptographicException">The encryptedData parameter has an <see cref="P:System.Security.Cryptography.Xml.EncryptedType.EncryptionMethod" /> property that is null.-or-The encrypted key cannot be retrieved using the specified parameters.</exception>
    public virtual SymmetricAlgorithm GetDecryptionKey(
      EncryptedData encryptedData,
      string symmetricAlgorithmUri)
    {
      if (encryptedData == null)
        throw new ArgumentNullException(nameof (encryptedData));
      if (encryptedData.KeyInfo == null)
        return (SymmetricAlgorithm) null;
      IEnumerator enumerator1 = encryptedData.KeyInfo.GetEnumerator();
      EncryptedKey encryptedKey1 = (EncryptedKey) null;
      while (enumerator1.MoveNext())
      {
        KeyInfoName current1 = enumerator1.Current as KeyInfoName;
        if (current1 != null)
        {
          string str = current1.Value;
          if ((SymmetricAlgorithm) this.m_keyNameMapping[(object) str] != null)
            return (SymmetricAlgorithm) this.m_keyNameMapping[(object) str];
          XmlNamespaceManager nsmgr = new XmlNamespaceManager(this.m_document.NameTable);
          nsmgr.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
          XmlNodeList xmlNodeList = this.m_document.SelectNodes("//enc:EncryptedKey", nsmgr);
          if (xmlNodeList != null)
          {
            IEnumerator enumerator2 = xmlNodeList.GetEnumerator();
            try
            {
              while (enumerator2.MoveNext())
              {
                XmlElement current2 = (XmlNode) enumerator2.Current as XmlElement;
                EncryptedKey encryptedKey2 = new EncryptedKey();
                encryptedKey2.LoadXml(current2);
                if (encryptedKey2.CarriedKeyName == str && encryptedKey2.Recipient == this.Recipient)
                {
                  encryptedKey1 = encryptedKey2;
                  break;
                }
              }
              break;
            }
            finally
            {
              (enumerator2 as IDisposable)?.Dispose();
            }
          }
          else
            break;
        }
        else
        {
          KeyInfoRetrievalMethod current2 = enumerator1.Current as KeyInfoRetrievalMethod;
          if (current2 != null)
          {
            string idFromLocalUri = Exml.ExtractIdFromLocalUri(current2.Uri);
            encryptedKey1 = new EncryptedKey();
            encryptedKey1.LoadXml(this.GetIdElement(this.m_document, idFromLocalUri));
            break;
          }
          KeyInfoEncryptedKey current3 = enumerator1.Current as KeyInfoEncryptedKey;
          if (current3 != null)
          {
            encryptedKey1 = current3.EncryptedKey;
            break;
          }
        }
      }
      if (encryptedKey1 == null)
        return (SymmetricAlgorithm) null;
      if (symmetricAlgorithmUri == null)
      {
        if (encryptedData.EncryptionMethod == null)
          throw new CryptographicException("Cryptography_Xml_MissingAlgorithm");
        symmetricAlgorithmUri = encryptedData.EncryptionMethod.KeyAlgorithm;
      }
      byte[] numArray = this.DecryptEncryptedKey(encryptedKey1);
      if (numArray == null)
        throw new CryptographicException("Cryptography_Xml_MissingDecryptionKey");
      SymmetricAlgorithm fromName = Exml.CreateFromName<SymmetricAlgorithm>(symmetricAlgorithmUri);
      if (fromName == null)
        throw new CryptographicException("Cryptography_Xml_MissingAlgorithm");
      fromName.Key = numArray;
      return fromName;
    }

    /// <summary>Determines the key represented by the <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> element.</summary>
    /// <param name="encryptedKey">The <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> object that contains the key to retrieve.</param>
    /// <returns>A byte array that contains the key.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="encryptedKey" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.Security.Cryptography.CryptographicException">The value of the <paramref name="encryptedKey" /> parameter is not the Triple DES Key Wrap algorithm or the Advanced Encryption Standard (AES) Key Wrap algorithm (also called Rijndael). </exception>
    public virtual byte[] DecryptEncryptedKey(EncryptedKey encryptedKey)
    {
      if (encryptedKey == null)
        throw new ArgumentNullException(nameof (encryptedKey));
      if (encryptedKey.KeyInfo == null)
        return (byte[]) null;
      IEnumerator enumerator = encryptedKey.KeyInfo.GetEnumerator();
      while (enumerator.MoveNext())
      {
        KeyInfoName current1 = enumerator.Current as KeyInfoName;
        if (current1 != null)
        {
          object obj = this.m_keyNameMapping[(object) current1.Value];
          if (obj != null)
          {
            if ( encryptedKey.CipherData == null || encryptedKey.CipherData.CipherValue == null)
              throw new CryptographicException("Cryptography_Xml_MissingAlgorithm");
            if (obj is SymmetricAlgorithm)
              return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, (SymmetricAlgorithm) obj);
            bool useOAEP = encryptedKey.EncryptionMethod != null && encryptedKey.EncryptionMethod.KeyAlgorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";
            return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, (RSA) obj, useOAEP);
          }
          break;
        }
        KeyInfoX509Data current2 = enumerator.Current as KeyInfoX509Data;
        if (current2 != null)
        {
          foreach (X509Certificate2 cert in SignedXml.BuildBagOfCerts(current2, CertUsageType.Decryption))
          {
            using (RSA rsaPrivateKey = (RSA)cert.PublicKey.Key)
            {
              if (rsaPrivateKey != null)
              {
                if ( encryptedKey.CipherData == null || encryptedKey.CipherData.CipherValue == null)
                  throw new CryptographicException("Cryptography_Xml_MissingAlgorithm");
                bool useOAEP = encryptedKey.EncryptionMethod != null && encryptedKey.EncryptionMethod.KeyAlgorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";
                return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, rsaPrivateKey, useOAEP);
              }
            }
          }
          break;
        }
        KeyInfoRetrievalMethod current3 = enumerator.Current as KeyInfoRetrievalMethod;
        if (current3 != null)
        {
          string idFromLocalUri = Exml.ExtractIdFromLocalUri(current3.Uri);
          EncryptedKey encryptedKey1 = new EncryptedKey();
          encryptedKey1.LoadXml(this.GetIdElement(this.m_document, idFromLocalUri));
          try
          {
            ++this.m_xmlDsigSearchDepthCounter;
            if (this.IsOverXmlDsigRecursionLimit())
              throw new Exception("Passed recursion limit");
            return this.DecryptEncryptedKey(encryptedKey1);
          }
          finally
          {
            --this.m_xmlDsigSearchDepthCounter;
          }
        }
        else
        {
          KeyInfoEncryptedKey current4 = enumerator.Current as KeyInfoEncryptedKey;
          if (current4 != null)
          {
            byte[] numArray = this.DecryptEncryptedKey(current4.EncryptedKey);
            if (numArray != null)
            {
              SymmetricAlgorithm fromName = Exml.CreateFromName<SymmetricAlgorithm>(encryptedKey.EncryptionMethod.KeyAlgorithm);
              if (fromName == null)
                throw new CryptographicException("Cryptography_Xml_MissingAlgorithm");
              fromName.Key = numArray;
              if ((encryptedKey.CipherData == null || encryptedKey.CipherData.CipherValue == null))
                throw new CryptographicException("Cryptography_Xml_MissingAlgorithm");
              return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, fromName);
            }
          }
        }
      }
      return (byte[]) null;
    }

    /// <summary>Defines a mapping between a key name and a symmetric key or an asymmetric key.</summary>
    /// <param name="keyName">The name to map to <paramref name="keyObject" />.</param>
    /// <param name="keyObject">The symmetric key to map to <paramref name="keyName" />.</param>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="keyName" /> parameter is <see langword="null" />.-or-The value of the <paramref name="keyObject" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.Security.Cryptography.CryptographicException">The value of the <paramref name="keyObject" /> parameter is not an RSA algorithm or a symmetric key. </exception>
    public void AddKeyNameMapping(string keyName, object keyObject)
    {
      if (keyName == null)
        throw new ArgumentNullException(nameof (keyName));
      if (keyObject == null)
        throw new ArgumentNullException(nameof (keyObject));
      if (!(keyObject is SymmetricAlgorithm) && !(keyObject is RSA))
        throw new CryptographicException("Cryptography_Xml_NotSupportedCryptographicTransform");
      this.m_keyNameMapping.Add((object) keyName, keyObject);
    }

    /// <summary>Resets all key name mapping.</summary>
    public void ClearKeyNameMappings()
    {
      this.m_keyNameMapping.Clear();
    }

    /// <summary>Encrypts the outer XML of an element using the specified X.509 certificate.</summary>
    /// <param name="inputElement">The XML element to encrypt.</param>
    /// <param name="certificate">The X.509 certificate to use for encryption.</param>
    /// <returns>An <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> element that represents the encrypted XML data.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="inputElement" /> parameter is <see langword="null" />.-or-The value of the <paramref name="certificate" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.NotSupportedException">The value of the <paramref name="certificate" /> parameter does not represent an RSA key algorithm.</exception>
    public EncryptedData Encrypt(XmlElement inputElement, X509Certificate2 certificate)
    {
      if (inputElement == null)
        throw new ArgumentNullException(nameof (inputElement));
      if (certificate == null)
        throw new ArgumentNullException(nameof (certificate));
      using (RSA rsaPublicKey = certificate.GetRSAPublicKey())
      {
        if (rsaPublicKey == null)
          throw new NotSupportedException("NotSupported_KeyAlgorithm");
        EncryptedData encryptedData = new EncryptedData();
        encryptedData.Type = "http://www.w3.org/2001/04/xmlenc#Element";
        encryptedData.EncryptionMethod = new EncryptionMethod("http://www.w3.org/2001/04/xmlenc#aes256-cbc");
        EncryptedKey encryptedKey = new EncryptedKey();
        encryptedKey.EncryptionMethod = new EncryptionMethod("http://www.w3.org/2001/04/xmlenc#rsa-1_5");
        encryptedKey.KeyInfo.AddClause((KeyInfoClause) new KeyInfoX509Data((X509Certificate) certificate));
        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        encryptedKey.CipherData.CipherValue = EncryptedXml.EncryptKey(rijndaelManaged.Key, rsaPublicKey, false);
        KeyInfoEncryptedKey infoEncryptedKey = new KeyInfoEncryptedKey(encryptedKey);
        encryptedData.KeyInfo.AddClause((KeyInfoClause) infoEncryptedKey);
        encryptedData.CipherData.CipherValue = this.EncryptData(inputElement, (SymmetricAlgorithm) rijndaelManaged, false);
        return encryptedData;
      }
    }

    /// <summary>Encrypts the outer XML of an element using the specified key in the key mapping table.</summary>
    /// <param name="inputElement">The XML element to encrypt.</param>
    /// <param name="keyName">A key name that can be found in the key mapping table.</param>
    /// <returns>An <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> object that represents the encrypted XML data.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="inputElement" /> parameter is <see langword="null" />.-or-The value of the <paramref name="keyName" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.Security.Cryptography.CryptographicException">The value of the <paramref name="keyName" /> parameter does not match a registered key name pair.-or-The cryptographic key described by the <paramref name="keyName" /> parameter is not supported. </exception>
    public EncryptedData Encrypt(XmlElement inputElement, string keyName)
    {
      if (inputElement == null)
        throw new ArgumentNullException(nameof (inputElement));
      if (keyName == null)
        throw new ArgumentNullException(nameof (keyName));
      object obj = (object) null;
      if (this.m_keyNameMapping != null)
        obj = this.m_keyNameMapping[(object) keyName];
      if (obj == null)
        throw new CryptographicException("Cryptography_Xml_MissingEncryptionKey");
      SymmetricAlgorithm symmetricAlgorithm = obj as SymmetricAlgorithm;
      RSA rsa = obj as RSA;
      EncryptedData encryptedData = new EncryptedData();
      encryptedData.Type = "http://www.w3.org/2001/04/xmlenc#Element";
      encryptedData.EncryptionMethod = new EncryptionMethod("http://www.w3.org/2001/04/xmlenc#aes256-cbc");
      string algorithm = (string) null;
      if (symmetricAlgorithm == null)
        algorithm = "http://www.w3.org/2001/04/xmlenc#rsa-1_5";
      else if (symmetricAlgorithm is TripleDES)
      {
        algorithm = "http://www.w3.org/2001/04/xmlenc#kw-tripledes";
      }
      else
      {
        if (!(symmetricAlgorithm is Rijndael) && !(symmetricAlgorithm is Aes))
          throw new CryptographicException("Cryptography_Xml_NotSupportedCryptographicTransform");
        switch (symmetricAlgorithm.KeySize)
        {
          case 128:
            algorithm = "http://www.w3.org/2001/04/xmlenc#kw-aes128";
            break;
          case 192:
            algorithm = "http://www.w3.org/2001/04/xmlenc#kw-aes192";
            break;
          case 256:
            algorithm = "http://www.w3.org/2001/04/xmlenc#kw-aes256";
            break;
        }
      }
      EncryptedKey encryptedKey = new EncryptedKey();
      encryptedKey.EncryptionMethod = new EncryptionMethod(algorithm);
      encryptedKey.KeyInfo.AddClause((KeyInfoClause) new KeyInfoName(keyName));
      RijndaelManaged rijndaelManaged = new RijndaelManaged();
      encryptedKey.CipherData.CipherValue = symmetricAlgorithm == null ? EncryptedXml.EncryptKey(rijndaelManaged.Key, rsa, false) : EncryptedXml.EncryptKey(rijndaelManaged.Key, symmetricAlgorithm);
      KeyInfoEncryptedKey infoEncryptedKey = new KeyInfoEncryptedKey(encryptedKey);
      encryptedData.KeyInfo.AddClause((KeyInfoClause) infoEncryptedKey);
      encryptedData.CipherData.CipherValue = this.EncryptData(inputElement, (SymmetricAlgorithm) rijndaelManaged, false);
      return encryptedData;
    }

    /// <summary>Decrypts all <see langword="&lt;EncryptedData&gt;" /> elements of the XML document that were specified during initialization of the <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> class.</summary>
    /// <exception cref="T:System.Security.Cryptography.CryptographicException">The cryptographic key used to decrypt the document was not found. </exception>
    public void DecryptDocument()
    {
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(this.m_document.NameTable);
      nsmgr.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
      XmlNodeList xmlNodeList = this.m_document.SelectNodes("//enc:EncryptedData", nsmgr);
      if (xmlNodeList == null)
        return;
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        XmlElement inputElement = xmlNode as XmlElement;
        EncryptedData encryptedData = new EncryptedData();
        encryptedData.LoadXml(inputElement);
        SymmetricAlgorithm decryptionKey = this.GetDecryptionKey(encryptedData, (string) null);
        if (decryptionKey == null)
          throw new CryptographicException("Cryptography_Xml_MissingDecryptionKey");
        byte[] decryptedData = this.DecryptData(encryptedData, decryptionKey);
        this.ReplaceData(inputElement, decryptedData);
      }
    }

    /// <summary>Encrypts data in the specified byte array using the specified symmetric algorithm.</summary>
    /// <param name="plaintext">The data to encrypt.</param>
    /// <param name="symmetricAlgorithm">The symmetric algorithm to use for encryption.</param>
    /// <returns>A byte array of encrypted data.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="plaintext" /> parameter is <see langword="null" />.-or-The value of the <paramref name="symmetricAlgorithm" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.Security.Cryptography.CryptographicException">The data could not be encrypted using the specified parameters.</exception>
    public byte[] EncryptData(byte[] plaintext, SymmetricAlgorithm symmetricAlgorithm)
    {
      if (plaintext == null)
        throw new ArgumentNullException(nameof (plaintext));
      if (symmetricAlgorithm == null)
        throw new ArgumentNullException(nameof (symmetricAlgorithm));
      CipherMode mode = symmetricAlgorithm.Mode;
      PaddingMode padding = symmetricAlgorithm.Padding;
      byte[] numArray1 = (byte[]) null;
      try
      {
        symmetricAlgorithm.Mode = this.m_mode;
        symmetricAlgorithm.Padding = this.m_padding;
        numArray1 = symmetricAlgorithm.CreateEncryptor().TransformFinalBlock(plaintext, 0, plaintext.Length);
      }
      finally
      {
        symmetricAlgorithm.Mode = mode;
        symmetricAlgorithm.Padding = padding;
      }
      byte[] numArray2;
      if (this.m_mode == CipherMode.ECB)
      {
        numArray2 = numArray1;
      }
      else
      {
        byte[] iv = symmetricAlgorithm.IV;
        numArray2 = new byte[numArray1.Length + iv.Length];
        Buffer.BlockCopy((Array) iv, 0, (Array) numArray2, 0, iv.Length);
        Buffer.BlockCopy((Array) numArray1, 0, (Array) numArray2, iv.Length, numArray1.Length);
      }
      return numArray2;
    }

    /// <summary>Encrypts the specified element or its contents using the specified symmetric algorithm.</summary>
    /// <param name="inputElement">The element or its contents to encrypt.</param>
    /// <param name="symmetricAlgorithm">The symmetric algorithm to use for encryption.</param>
    /// <param name="content">
    /// <see langword="true" /> to encrypt only the contents of the element; <see langword="false" /> to encrypt the entire element.</param>
    /// <returns>A byte array that contains the encrypted data.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="inputElement" /> parameter is <see langword="null" />.-or-The value of the <paramref name="symmetricAlgorithm" /> parameter is <see langword="null" />.</exception>
    public byte[] EncryptData(
      XmlElement inputElement,
      SymmetricAlgorithm symmetricAlgorithm,
      bool content)
    {
      if (inputElement == null)
        throw new ArgumentNullException(nameof (inputElement));
      if (symmetricAlgorithm == null)
        throw new ArgumentNullException(nameof (symmetricAlgorithm));
      return this.EncryptData(content ? this.m_encoding.GetBytes(inputElement.InnerXml) : this.m_encoding.GetBytes(inputElement.OuterXml), symmetricAlgorithm);
    }

    /// <summary>Decrypts an <see langword="&lt;EncryptedData&gt;" /> element using the specified symmetric algorithm.</summary>
    /// <param name="encryptedData">The data to decrypt.</param>
    /// <param name="symmetricAlgorithm">The symmetric key used to decrypt <paramref name="encryptedData" />.</param>
    /// <returns>A byte array that contains the raw decrypted plain text.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="encryptedData" /> parameter is <see langword="null" />.-or-The value of the <paramref name="symmetricAlgorithm" /> parameter is <see langword="null" />.</exception>
    public byte[] DecryptData(EncryptedData encryptedData, SymmetricAlgorithm symmetricAlgorithm)
    {
      if (encryptedData == null)
        throw new ArgumentNullException(nameof (encryptedData));
      if (symmetricAlgorithm == null)
        throw new ArgumentNullException(nameof (symmetricAlgorithm));
      byte[] cipherValue = this.GetCipherValue(encryptedData.CipherData);
      CipherMode mode = symmetricAlgorithm.Mode;
      PaddingMode padding = symmetricAlgorithm.Padding;
      byte[] iv = symmetricAlgorithm.IV;
      byte[] numArray = (byte[]) null;
      if (this.m_mode != CipherMode.ECB)
        numArray = this.GetDecryptionIV(encryptedData, (string) null);
      try
      {
        int inputOffset = 0;
        if (numArray != null)
        {
          symmetricAlgorithm.IV = numArray;
          inputOffset = numArray.Length;
        }
        symmetricAlgorithm.Mode = this.m_mode;
        symmetricAlgorithm.Padding = this.m_padding;
        return symmetricAlgorithm.CreateDecryptor().TransformFinalBlock(cipherValue, inputOffset, cipherValue.Length - inputOffset);
      }
      finally
      {
        symmetricAlgorithm.Mode = mode;
        symmetricAlgorithm.Padding = padding;
        symmetricAlgorithm.IV = iv;
      }
    }

    /// <summary>Replaces an <see langword="&lt;EncryptedData&gt;" /> element with a specified decrypted sequence of bytes.</summary>
    /// <param name="inputElement">The <see langword="&lt;EncryptedData&gt;" /> element to replace.</param>
    /// <param name="decryptedData">The decrypted data to replace <paramref name="inputElement" /> with.</param>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="inputElement" /> parameter is <see langword="null" />.-or-The value of the <paramref name="decryptedData" /> parameter is <see langword="null" />.</exception>
    public void ReplaceData(XmlElement inputElement, byte[] decryptedData)
    {
      if (inputElement == null)
        throw new ArgumentNullException(nameof (inputElement));
      if (decryptedData == null)
        throw new ArgumentNullException(nameof (decryptedData));
      XmlNode parentNode = inputElement.ParentNode;
      if (parentNode.NodeType == XmlNodeType.Document)
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.PreserveWhitespace = true;
        using (StringReader stringReader = new StringReader(this.m_encoding.GetString(decryptedData)))
        {
          using (XmlReader reader = XmlReader.Create((TextReader) stringReader, new XmlReaderSettings
          {
              XmlResolver = m_xmlResolver,
              DtdProcessing = DtdProcessing.Parse,
              MaxCharactersFromEntities = 10000000L,
              MaxCharactersInDocument = 0
          }))
            xmlDocument.Load(reader);
        }
        XmlNode newChild = inputElement.OwnerDocument.ImportNode((XmlNode) xmlDocument.DocumentElement, true);
        parentNode.RemoveChild((XmlNode) inputElement);
        parentNode.AppendChild(newChild);
      }
      else
      {
        XmlNode element = (XmlNode) parentNode.OwnerDocument.CreateElement(parentNode.Prefix, parentNode.LocalName, parentNode.NamespaceURI);
        try
        {
          parentNode.AppendChild(element);
          element.InnerXml = this.m_encoding.GetString(decryptedData);
          XmlNode newChild = element.FirstChild;
          XmlNode nextSibling1 = inputElement.NextSibling;
          XmlNode nextSibling2;
          for (; newChild != null; newChild = nextSibling2)
          {
            nextSibling2 = newChild.NextSibling;
            parentNode.InsertBefore(newChild, nextSibling1);
          }
        }
        finally
        {
          parentNode.RemoveChild(element);
        }
        parentNode.RemoveChild((XmlNode) inputElement);
      }
    }

    /// <summary>Replaces the specified element with the specified <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> object.</summary>
    /// <param name="inputElement">The element to replace with an <see langword="&lt;EncryptedData&gt;" /> element.</param>
    /// <param name="encryptedData">The <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> object to replace the <paramref name="inputElement" /> parameter with.</param>
    /// <param name="content">
    /// <see langword="true" /> to replace only the contents of the element; <see langword="false" /> to replace the entire element.</param>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="inputElement" /> parameter is <see langword="null" />.-or-The value of the <paramref name="encryptedData" /> parameter is <see langword="null" />.</exception>
    public static void ReplaceElement(
      XmlElement inputElement,
      EncryptedData encryptedData,
      bool content)
    {
      if (inputElement == null)
        throw new ArgumentNullException(nameof (inputElement));
      if (encryptedData == null)
        throw new ArgumentNullException(nameof (encryptedData));
      XmlElement xml = encryptedData.GetXml(inputElement.OwnerDocument);
      if (content)
      {
        if (!content)
          return;
        Exml.RemoveAllChildren(inputElement);
        inputElement.AppendChild((XmlNode) xml);
      }
      else
        inputElement.ParentNode.ReplaceChild((XmlNode) xml, (XmlNode) inputElement);
    }

    /// <summary>Encrypts a key using a symmetric algorithm that a recipient uses to decrypt an <see langword="&lt;EncryptedData&gt;" /> element.</summary>
    /// <param name="keyData">The key to encrypt.</param>
    /// <param name="symmetricAlgorithm">The symmetric key used to encrypt <paramref name="keyData" />.</param>
    /// <returns>A byte array that represents the encrypted value of the <paramref name="keyData" /> parameter.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="keyData" /> parameter is <see langword="null" />.-or-The value of the <paramref name="symmetricAlgorithm" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.Security.Cryptography.CryptographicException">The value of the <paramref name="symmetricAlgorithm" /> parameter is not the Triple DES Key Wrap algorithm or the Advanced Encryption Standard (AES) Key Wrap algorithm (also called Rijndael). </exception>
    public static byte[] EncryptKey(byte[] keyData, SymmetricAlgorithm symmetricAlgorithm)
    {
      if (keyData == null)
        throw new ArgumentNullException(nameof (keyData));
      if (symmetricAlgorithm == null)
        throw new ArgumentNullException(nameof (symmetricAlgorithm));
      if (symmetricAlgorithm is TripleDES)
        return SymmetricKeyWrap.TripleDESKeyWrapEncrypt(symmetricAlgorithm.Key, keyData);
      if (symmetricAlgorithm is Rijndael || symmetricAlgorithm is Aes)
        return SymmetricKeyWrap.AESKeyWrapEncrypt(symmetricAlgorithm.Key, keyData);
      throw new CryptographicException("Cryptography_Xml_NotSupportedCryptographicTransform");
    }

    /// <summary>Encrypts the key that a recipient uses to decrypt an <see langword="&lt;EncryptedData&gt;" /> element.</summary>
    /// <param name="keyData">The key to encrypt.</param>
    /// <param name="rsa">The asymmetric key used to encrypt <paramref name="keyData" />.</param>
    /// <param name="useOAEP">A value that specifies whether to use Optimal Asymmetric Encryption Padding (OAEP).</param>
    /// <returns>A byte array that represents the encrypted value of the <paramref name="keyData" /> parameter.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="keyData" /> parameter is <see langword="null" />.-or-The value of the <paramref name="rsa" /> parameter is <see langword="null" />.</exception>
    public static byte[] EncryptKey(byte[] keyData, RSA rsa, bool useOAEP)
    {
      if (keyData == null)
        throw new ArgumentNullException(nameof (keyData));
      if (rsa == null)
        throw new ArgumentNullException(nameof (rsa));
      if (useOAEP)
        return new RSAOAEPKeyExchangeFormatter((AsymmetricAlgorithm) rsa).CreateKeyExchange(keyData);
      return new RSAPKCS1KeyExchangeFormatter((AsymmetricAlgorithm) rsa).CreateKeyExchange(keyData);
    }

    /// <summary>Decrypts an <see langword="&lt;EncryptedKey&gt;" /> element using a symmetric algorithm.</summary>
    /// <param name="keyData">An array of bytes that represents an encrypted <see langword="&lt;EncryptedKey&gt;" /> element.</param>
    /// <param name="symmetricAlgorithm">The symmetric key used to decrypt <paramref name="keyData" />.</param>
    /// <returns>A byte array that contains the plain text key.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="keyData" /> parameter is <see langword="null" />.-or-The value of the <paramref name="symmetricAlgorithm" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.Security.Cryptography.CryptographicException">The value of the <paramref name="symmetricAlgorithm" /> element is not the Triple DES Key Wrap algorithm or the Advanced Encryption Standard (AES) Key Wrap algorithm (also called Rijndael). </exception>
    public static byte[] DecryptKey(byte[] keyData, SymmetricAlgorithm symmetricAlgorithm)
    {
      if (keyData == null)
        throw new ArgumentNullException(nameof (keyData));
      if (symmetricAlgorithm == null)
        throw new ArgumentNullException(nameof (symmetricAlgorithm));
      if (symmetricAlgorithm is TripleDES)
        return SymmetricKeyWrap.TripleDESKeyWrapDecrypt(symmetricAlgorithm.Key, keyData);
      if (symmetricAlgorithm is Rijndael || symmetricAlgorithm is Aes)
        return SymmetricKeyWrap.AESKeyWrapDecrypt(symmetricAlgorithm.Key, keyData);
      throw new CryptographicException("Cryptography_Xml_NotSupportedCryptographicTransform");
    }

    /// <summary>Decrypts an <see langword="&lt;EncryptedKey&gt;" /> element using an asymmetric algorithm.</summary>
    /// <param name="keyData">An array of bytes that represents an encrypted <see langword="&lt;EncryptedKey&gt;" /> element.</param>
    /// <param name="rsa">The asymmetric key used to decrypt <paramref name="keyData" />.</param>
    /// <param name="useOAEP">A value that specifies whether to use Optimal Asymmetric Encryption Padding (OAEP).</param>
    /// <returns>A byte array that contains the plain text key.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="keyData" /> parameter is <see langword="null" />.-or-The value of the <paramref name="rsa" /> parameter is <see langword="null" />.</exception>
    public static byte[] DecryptKey(byte[] keyData, RSA rsa, bool useOAEP)
    {
      if (keyData == null)
        throw new ArgumentNullException(nameof (keyData));
      if (rsa == null)
        throw new ArgumentNullException(nameof (rsa));
      if (useOAEP)
        return new RSAOAEPKeyExchangeDeformatter((AsymmetricAlgorithm) rsa).DecryptKeyExchange(keyData);
      return new RSAPKCS1KeyExchangeDeformatter((AsymmetricAlgorithm) rsa).DecryptKeyExchange(keyData);
    }
  }
}
