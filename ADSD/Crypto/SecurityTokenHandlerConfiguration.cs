using System;
using System.Security.Cryptography.X509Certificates;

namespace ADSD
{
    /// <summary>Configuration common to all security token handlers.</summary>
    public class SecurityTokenHandlerConfiguration
    {
        /// <summary>Specifies the default issuer name registry instance; an instance of the configuration-based issuer name registry.</summary>
        public static readonly IssuerNameRegistry DefaultIssuerNameRegistry = (IssuerNameRegistry) new ConfigurationBasedIssuerNameRegistry();
        /// <summary>Specifies the default issuer token resolver instance.</summary>
        public static readonly SecurityTokenResolver DefaultIssuerTokenResolver = (SecurityTokenResolver) System.IdentityModel.Tokens.IssuerTokenResolver.DefaultInstance;
        /// <summary>Specifies the default maximum clock skew.</summary>
        public static readonly TimeSpan DefaultMaxClockSkew = new TimeSpan(0, 5, 0);
        /// <summary>Specifies the default token replay cache expiration period.</summary>
        public static readonly TimeSpan DefaultTokenReplayCacheExpirationPeriod = TimeSpan.MaxValue;
        /// <summary>Specifies the default X.509 certificate validation mode.</summary>
        public static readonly X509CertificateValidationMode DefaultCertificateValidationMode = IdentityConfiguration.DefaultCertificateValidationMode;
        /// <summary>Specifies the default X.509 certificate revocation mode.</summary>
        public static readonly X509RevocationMode DefaultRevocationMode = IdentityConfiguration.DefaultRevocationMode;
        /// <summary>Specifies the default X.509 certificate trusted store location.</summary>
        public static readonly StoreLocation DefaultTrustedStoreLocation = IdentityConfiguration.DefaultTrustedStoreLocation;
        /// <summary>Specifies the default X.509 certificate validator instance.</summary>
        public static readonly X509CertificateValidator DefaultCertificateValidator = X509Util.CreateCertificateValidator(SecurityTokenHandlerConfiguration.DefaultCertificateValidationMode, SecurityTokenHandlerConfiguration.DefaultRevocationMode, SecurityTokenHandlerConfiguration.DefaultTrustedStoreLocation);
        private StoreLocation trustedStoreLocation = SecurityTokenHandlerConfiguration.DefaultTrustedStoreLocation;
        private X509RevocationMode revocationMode = SecurityTokenHandlerConfiguration.DefaultRevocationMode;
        private X509CertificateValidationMode certificateValidationMode = SecurityTokenHandlerConfiguration.DefaultCertificateValidationMode;
        private AudienceRestriction audienceRestriction = new AudienceRestriction();
        private X509CertificateValidator certificateValidator = SecurityTokenHandlerConfiguration.DefaultCertificateValidator;
        private bool detectReplayedTokens = SecurityTokenHandlerConfiguration.DefaultDetectReplayedTokens;
        private IssuerNameRegistry issuerNameRegistry = SecurityTokenHandlerConfiguration.DefaultIssuerNameRegistry;
        private SecurityTokenResolver issuerTokenResolver = SecurityTokenHandlerConfiguration.DefaultIssuerTokenResolver;
        private TimeSpan maxClockSkew = SecurityTokenHandlerConfiguration.DefaultMaxClockSkew;
        private bool saveBootstrapContext = SecurityTokenHandlerConfiguration.DefaultSaveBootstrapContext;
        private SecurityTokenResolver serviceTokenResolver = EmptySecurityTokenResolver.Instance;
        private TimeSpan tokenReplayCacheExpirationPeriod = SecurityTokenHandlerConfiguration.DefaultTokenReplayCacheExpirationPeriod;
        private IdentityModelCaches caches = new IdentityModelCaches();
        /// <summary>Specifies a value that determines whether to detect replayed tokens; <see langword="false" />, do not detect replayed tokens.</summary>
        public static readonly bool DefaultDetectReplayedTokens;
        /// <summary>Specifies whether to save bootstrap tokens; <see langword="false" />, bootstrap tokens are not saved.</summary>
        public static readonly bool DefaultSaveBootstrapContext;

        /// <summary>Gets or sets the audience restriction.</summary>
        /// <returns>The audience restriction.</returns>
        public AudienceRestriction AudienceRestriction
        {
            get
            {
                return this.audienceRestriction;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (value));
                this.audienceRestriction = value;
            }
        }

        /// <summary>Gets or sets the X.509 certificate validator used by handlers to validate issuer certificates</summary>
        /// <returns>The certificate validator.</returns>
        public X509CertificateValidator CertificateValidator
        {
            get
            {
                return this.certificateValidator;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (value));
                this.certificateValidator = value;
            }
        }

        /// <summary>Gets or sets the X.509 revocation mode for this configuration.</summary>
        /// <returns>The X.509 revocation mode.</returns>
        public X509RevocationMode RevocationMode
        {
            get
            {
                return this.revocationMode;
            }
            set
            {
                this.revocationMode = value;
            }
        }

        /// <summary>Gets or sets the X.509 trusted store location used by handlers to validate issuer certificates.</summary>
        /// <returns>The trusted store location.</returns>
        public StoreLocation TrustedStoreLocation
        {
            get
            {
                return this.trustedStoreLocation;
            }
            set
            {
                this.trustedStoreLocation = value;
            }
        }

        /// <summary>Gets or sets the X.509 certificate validation mode used by handlers to validate issuer certificates.</summary>
        /// <returns>The certificate validation mode.</returns>
        public X509CertificateValidationMode CertificateValidationMode
        {
            get
            {
                return this.certificateValidationMode;
            }
            set
            {
                this.certificateValidationMode = value;
            }
        }

        /// <summary>Gets or sets a value that indicates whether replayed tokens should be detected by handlers in this configuration.</summary>
        /// <returns>
        /// <see langword="true" /> if replayed tokens should be detected; otherwise, <see langword="false" />.</returns>
        public bool DetectReplayedTokens
        {
            get
            {
                return this.detectReplayedTokens;
            }
            set
            {
                this.detectReplayedTokens = value;
            }
        }

        /// <summary>Gets or sets the issuer name registry for this configuration.</summary>
        /// <returns>The issuer name registry.</returns>
        public IssuerNameRegistry IssuerNameRegistry
        {
            get
            {
                return this.issuerNameRegistry;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (value));
                this.issuerNameRegistry = value;
            }
        }

        /// <summary>Gets or sets the issuer token resolver for this configuration.</summary>
        /// <returns>The issuer token resolver.</returns>
        public SecurityTokenResolver IssuerTokenResolver
        {
            get
            {
                return this.issuerTokenResolver;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (value));
                this.issuerTokenResolver = value;
            }
        }

        /// <summary>Gets or sets the maximum clock skew for handlers using this configuration.</summary>
        /// <returns>The maximum clock skew.</returns>
        public TimeSpan MaxClockSkew
        {
            get
            {
                return this.maxClockSkew;
            }
            set
            {
                if (value < TimeSpan.Zero)
                    throw DiagnosticUtility.ThrowHelperArgumentOutOfRange(nameof (value), (object) value, System.IdentityModel.SR.GetString("ID2070"));
                this.maxClockSkew = value;
            }
        }

        /// <summary>Gets or sets a value that indicates whether the bootstrap context (token) is saved in the <see cref="T:System.Security.Claims.ClaimsIdentity" /> and Sessions after token validation.</summary>
        /// <returns>
        /// <see langword="true" /> to save the bootstrap token; otherwise, <see langword="false" />.</returns>
        public bool SaveBootstrapContext
        {
            get
            {
                return this.saveBootstrapContext;
            }
            set
            {
                this.saveBootstrapContext = value;
            }
        }

        /// <summary>Gets or sets the security token resolver to use to resolve service tokens.</summary>
        /// <returns>The token resolver.</returns>
        public SecurityTokenResolver ServiceTokenResolver
        {
            get
            {
                return this.serviceTokenResolver;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (value));
                this.serviceTokenResolver = value;
            }
        }

        /// <summary>Gets or sets the caches that are used for this configuration.</summary>
        /// <returns>The caches.</returns>
        public IdentityModelCaches Caches
        {
            get
            {
                return this.caches;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (value));
                this.caches = value;
            }
        }

        /// <summary>Gets or sets the expiration period for items put in the token replay cache.</summary>
        /// <returns>The expiration period.</returns>
        public TimeSpan TokenReplayCacheExpirationPeriod
        {
            get
            {
                return this.tokenReplayCacheExpirationPeriod;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                    throw DiagnosticUtility.ThrowHelperArgumentOutOfRange(nameof (value), (object) value, System.IdentityModel.SR.GetString("ID0016"));
                this.tokenReplayCacheExpirationPeriod = value;
            }
        }
    }
}