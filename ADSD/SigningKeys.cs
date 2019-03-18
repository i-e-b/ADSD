using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using JetBrains.Annotations;
using SkinnyJson;

namespace ADSD
{
    /// <summary>
    /// A caching store of JWT signing keys
    /// </summary>
    public static class SigningKeys
    {
        /// <summary>
        /// Map of kid=>x5c data
        /// </summary>
        [NotNull]private static readonly Dictionary<string,string> KeyCache = new Dictionary<string,string>();
        [NotNull]private static readonly object KeyLock = new object();
        private static DateTime _lastRefresh = DateTime.MinValue;

        /// <summary>
        /// Return a disposable collection of security tokens for all known signing keys.
        /// <para>Caller must dispose</para>
        /// </summary>
        public static DisposingContainer<X509SecurityToken> AllAvailableKeys()
        {
            var collection = new DisposingContainer<X509SecurityToken>();
            lock (KeyLock)
            {
                foreach (var key in KeyCache.Keys)
                {
                    collection.Add(new X509SecurityToken(PublicKeyForKid(key)));
                }
            }
            return collection;
        }

        /// <summary>
        /// Look up the Public Key for a given KID.
        /// KID is case sensitive. Returns `null` if no public key is found.
        /// <para>A new certificate is created for each call, and should be disposed by the caller</para>
        /// </summary>
        public static X509Certificate2 PublicKeyForKid(string kid) {
            lock (KeyLock)
            {
                if (kid == null) return null;
                if (!KeyCache.ContainsKey(kid)) return null;

                var value = KeyCache[kid];
                if (string.IsNullOrWhiteSpace(value)) return null;
                return new X509Certificate2(Convert.FromBase64String(value));
            }
        }

        /// <summary>
        /// Ensure signing keys are available and fresh.
        /// If no keys are loaded, this method will block. Otherwise,
        /// any refresh will happen in the background
        /// </summary>
        public static void RefreshKeys(string keyDiscoveryUrl, TimeSpan maxAge)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (KeyCache.Count < 1)
            {
                LoadCacheSync(keyDiscoveryUrl);
                return;
            }
            if (DateTime.UtcNow - _lastRefresh > maxAge) {
                _lastRefresh = DateTime.UtcNow;
                UpdateKeyCache(keyDiscoveryUrl);
            }
        }

        /// <summary>
        /// Update the key cache on a background thread. This method will return before the new keys are available.
        /// </summary>
        public static void UpdateKeyCache(string keyDiscoveryUrl)
        {
            if (keyDiscoveryUrl == null) return;

            new Thread(()=>{
                LoadCacheSync(keyDiscoveryUrl);
            }).Start();
        }

        /// <summary>
        /// Update the key cache synchronously.
        /// This method will not return until keys are read.
        /// </summary>
        public static void LoadCacheSync(string keyDiscoveryUrl) {
            
            if (keyDiscoveryUrl == null) return;
                using (var client = ClientWithDefaultProxy()) {
                    if (client == null) throw new Exception("Failed to create a HTTP client. No tokens will be validated.");

                    // ReSharper disable once AccessToDisposedClosure
                    var str = Sync.Run(() => client.GetStringAsync(keyDiscoveryUrl));

                    var data = Json.Defrost<JwkSet>(str);
                    if (data?.keys == null) return;

                    foreach (var key in data.keys)
                    {
                        if (key.x5c?.Count != 1) continue; // we don't handle multi-part certificates
                        if (key.use != "sig") continue;   // we only use signature keys
                        if (key.kty != "RSA") continue;   // currently only uses RSA certificates
                        if (string.IsNullOrWhiteSpace(key.kid)) continue; // invalid ID

                        lock (KeyLock)
                        {
                            if (KeyCache.ContainsKey(key.kid)) KeyCache[key.kid] = key.x5c[0];
                            else KeyCache.Add(key.kid, key.x5c[0]);

                            Console.WriteLine("Added key " + key.kid);
                        }
                    }

                }
        }

        /// <summary>
        /// Create a HttpClient with system default proxy
        /// </summary>
        private static HttpClient ClientWithDefaultProxy()
        {
            var defaultProxy = WebRequest.DefaultWebProxy;
            if (defaultProxy == null)
            {
                return new HttpClient { Timeout = TimeSpan.FromSeconds(5) }; // no proxy on this system
            }

            defaultProxy.Credentials = CredentialCache.DefaultCredentials;
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = defaultProxy,
                PreAuthenticate = true,
                UseDefaultCredentials = true
            };

            return new HttpClient(httpClientHandler) { Timeout = TimeSpan.FromSeconds(5) };
        }

    }
}
