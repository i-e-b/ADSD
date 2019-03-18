using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>Represents an issuer name registry that maintains a list of trusted issuers loaded from elements in the application configuration file that associate each issuer name to the X.509 certificate that is needed to verify the signature of tokens produced by the issuer.</summary>
    public class ConfigurationBasedIssuerNameRegistry : IssuerNameRegistry
    {
        private readonly Dictionary<string, string> _configuredTrustedIssuers = new Dictionary<string, string>((IEqualityComparer<string>) new ConfigurationBasedIssuerNameRegistry.ThumbprintKeyComparer());

        /// <summary>Loads the trusted issuers from configuration.</summary>
        /// <param name="customConfiguration">The XML that represents the map of trusted issuers that is specified in the configuration file.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="customConfiguration" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The configuration contains one or more elements that are not recognized.</exception>
        public void LoadCustomConfiguration(XmlNodeList customConfiguration)
        {
            if (customConfiguration == null) throw new ArgumentNullException(nameof (customConfiguration));
            List<XmlElement> xmlElements = Exml.GetXmlElements(customConfiguration);
            if (xmlElements.Count != 1) throw new InvalidOperationException("Issuer registry config invalid");

            XmlElement xmlElement1 = xmlElements[0];
            if (!StringComparer.Ordinal.Equals(xmlElement1.LocalName, "trustedIssuers"))
                throw new InvalidOperationException("trustedIssuers");

            foreach (XmlNode childNode in xmlElement1.ChildNodes)
            {
                XmlElement xmlElement2 = childNode as XmlElement;
                if (xmlElement2 != null)
                {
                    if (StringComparer.Ordinal.Equals(xmlElement2.LocalName, "add"))
                    {
                        XmlNode namedItem1 = xmlElement2.Attributes.GetNamedItem("thumbprint");
                        XmlNode namedItem2 = xmlElement2.Attributes.GetNamedItem("name");
                        if (xmlElement2.Attributes.Count > 2 || namedItem1 == null)
                            throw new Exception("add thumbprint/name does not match");
                        this._configuredTrustedIssuers.Add(namedItem1.Value.Replace(" ", ""), namedItem2 == null || string.IsNullOrEmpty(namedItem2.Value) ? string.Empty : string.Intern(namedItem2.Value));
                    }
                    else if (StringComparer.Ordinal.Equals(xmlElement2.LocalName, "remove"))
                    {
                        if (xmlElement2.Attributes.Count != 1 || !StringComparer.Ordinal.Equals(xmlElement2.Attributes[0].LocalName, "thumbprint"))
                            throw new Exception("remove thumbprint deosn't work");
                        this._configuredTrustedIssuers.Remove(xmlElement2.Attributes.GetNamedItem("thumbprint").Value.Replace(" ", ""));
                    }
                    else if (StringComparer.Ordinal.Equals(xmlElement2.LocalName, "clear"))
                        this._configuredTrustedIssuers.Clear();
                    else
                        throw new Exception("unknown element type");
                }
            }
        }

        /// <summary>Returns the issuer name associated with the specified <see cref="T:System.IdentityModel.Tokens.X509SecurityToken" /> by mapping the certificate thumbprint to a name in the trusted issuers dictionary.</summary>
        /// <param name="securityToken">The security token for which the issuer name is requested. Should be assignable as <see cref="T:System.IdentityModel.Tokens.X509SecurityToken" />.</param>
        /// <returns>The issuer name if an entry for the certificate thumbprint of the token exists in the <see cref="P:System.IdentityModel.Tokens.ConfigurationBasedIssuerNameRegistry.ConfiguredTrustedIssuers" /> dictionary; otherwise, <see langword="null" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="securityToken" /> is <see langword="null" />.</exception>
        public override string GetIssuerName(SecurityToken securityToken)
        {
            if (securityToken == null) throw new ArgumentNullException(nameof (securityToken));
            X509SecurityToken x509SecurityToken = securityToken as X509SecurityToken;
            if (x509SecurityToken != null)
            {
                string thumbprint = x509SecurityToken.Certificate.Thumbprint;
                if (this._configuredTrustedIssuers.ContainsKey(thumbprint))
                {
                    string configuredTrustedIssuer = this._configuredTrustedIssuers[thumbprint];
                    string issuerName = string.IsNullOrEmpty(configuredTrustedIssuer) ? x509SecurityToken.Certificate.Subject : configuredTrustedIssuer;
                    return issuerName;
                }
            }
            return (string) null;
        }

        /// <summary>Gets the dictionary of trusted issuers that have been configured for this instance. </summary>
        /// <returns>A dictionary that contains the trusted issuers.</returns>
        public IDictionary<string, string> ConfiguredTrustedIssuers
        {
            get
            {
                return (IDictionary<string, string>) this._configuredTrustedIssuers;
            }
        }

        /// <summary>Adds an issuer to the dictionary of trusted issuers.</summary>
        /// <param name="certificateThumbprint">ASN.1 encoded form of the issuer's certificate thumbprint.</param>
        /// <param name="name">The name of the issuer.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="certificateThumbprint" /> is <see langword="null" /> or empty.-or-
        /// <paramref name="name" /> is <see langword="null" /> or empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">The issuer specified by <paramref name="certificateThumbprint" /> has already been configured. (The issuer already exists in the <see cref="P:System.IdentityModel.Tokens.ConfigurationBasedIssuerNameRegistry.ConfiguredTrustedIssuers" /> dictionary.)</exception>
        public void AddTrustedIssuer(string certificateThumbprint, string name)
        {
            if (string.IsNullOrEmpty(certificateThumbprint))
                throw new ArgumentNullException(nameof (certificateThumbprint));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof (name));
            if (this._configuredTrustedIssuers.ContainsKey(certificateThumbprint))
                throw new InvalidOperationException("Already added");
            certificateThumbprint = certificateThumbprint.Replace(" ", "");
            this._configuredTrustedIssuers.Add(certificateThumbprint, name);
        }

        private class ThumbprintKeyComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(x, y);
            }

            public int GetHashCode(string obj)
            {
                return obj.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
            }
        }
    }
}