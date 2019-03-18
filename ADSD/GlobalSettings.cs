namespace ADSD
{
    /// <summary>
    /// Settings to change internal behaviour
    /// </summary>
    public static class GlobalSettings
    {
        /// <summary>
        /// Default: false.
        /// Should XML signature attribute checks be skipped? If false, they are enforced
        /// </summary>
        public static bool SkipSignatureAttributeChecks { get; set; } = false;

        /// <summary>
        /// Default: false.
        /// If true, Signature URLs don't need to resolve to an FQDN
        /// </summary>
        public static bool AllowDetachedSignature { get; set; } = false;

        /// <summary>
        /// Default: false
        /// If true, use SHA1 hash for XML. This should only be used for legacy systems that can't be updated.
        /// </summary>
        public static bool UseInsecureHashAlgorithmsForXml { get; set; } = false;

        /// <summary>
        /// Default: false
        /// If true, allow older certificate forms. This should only be used for legacy systems that can't be updated.
        /// </summary>
        public static bool UseLegacyCertificatePrivateKey { get; set; } = false;

        /// <summary>
        /// Default: false
        /// If true, disable updates/upgrades inside the RSA resolution process.
        /// </summary>
        public static bool DisableUpdatingRsaProviderType { get; set; } = false;
    }
}