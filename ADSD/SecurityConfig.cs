namespace ADSD
{
    /// <summary>
    /// Config settings to create or verify an AAD JWT token
    /// </summary>
    public struct SecurityConfig
    {
        /// <summary>
        /// Security tennant key. Must be the same across the AAD organisation.
        /// </summary>
        public string TennantKey;
        
        /// <summary>
        /// Optional: Signing authority.
        /// Used only for generating tokens
        /// </summary>
        public string AadAuthorityRoot;
        /// <summary>
        /// Optional: Application ID from AAD instance
        /// Used only for generating tokens
        /// </summary>
        public string ClientId;
        /// <summary>
        /// Optional: Secret key owned by the Application in the AAD instance
        /// Used only for generating tokens
        /// </summary>
        public string AppKey;
        /// <summary>
        /// Optional: Resource ID being authorised.
        /// Used only for generating tokens.
        /// <para></para>
        /// In practice, this is the same as the `Audience` ID
        /// </summary>
        public string ResourceId;


        /// <summary>
        /// The required audience any token must be signed for.
        /// In practice, it's the same as the `ResourceID` used to create the token.
        /// </summary>
        public string Audience;
        /// <summary>
        /// X5C AAD signing key source.
        /// <para>For AAD, this is almost always "https://login.microsoftonline.com/common/discovery/keys"</para>
        /// </summary>
        public string KeyDiscoveryUrl;
        /// <summary>
        /// URL Used to authenticate a security token
        /// <para>For AAD, this is almost always "https://sts.windows.net/"</para>
        /// </summary>
        public string AadTokenIssuer;
    }
}