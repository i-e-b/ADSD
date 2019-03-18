using System;
using System.Globalization;
using System.Threading;
using JetBrains.Annotations;

namespace ADSD.Crypto
{
    internal class SecurityUniqueId
    {
        private static long nextId = 0;
        private static readonly string commonPrefix = "uuid-" + Guid.NewGuid() + "-";
        private readonly long id;
        private readonly string prefix;
        private string val;

        private SecurityUniqueId(string prefix, long id)
        {
            this.id = id;
            this.prefix = prefix;
            val = (string) null;
        }

        [NotNull]public static SecurityUniqueId Create()
        {
            return Create(commonPrefix);
        }

        [NotNull]public static SecurityUniqueId Create(string prefix)
        {
            return new SecurityUniqueId(prefix, Interlocked.Increment(ref nextId));
        }

        public string Value
        {
            get
            {
                if (val == null)
                    val = prefix + id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                return val;
            }
        }
    }
}