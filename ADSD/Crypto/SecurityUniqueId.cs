using System;
using System.Globalization;
using System.Threading;
using JetBrains.Annotations;

namespace ADSD
{
    internal class SecurityUniqueId
    {
        private static long nextId = 0;
        private static string commonPrefix = "uuid-" + Guid.NewGuid().ToString() + "-";
        private long id;
        private string prefix;
        private string val;

        private SecurityUniqueId(string prefix, long id)
        {
            this.id = id;
            this.prefix = prefix;
            this.val = (string) null;
        }

        [NotNull]public static SecurityUniqueId Create()
        {
            return SecurityUniqueId.Create(SecurityUniqueId.commonPrefix);
        }

        [NotNull]public static SecurityUniqueId Create(string prefix)
        {
            return new SecurityUniqueId(prefix, Interlocked.Increment(ref SecurityUniqueId.nextId));
        }

        public string Value
        {
            get
            {
                if (this.val == null)
                    this.val = this.prefix + this.id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                return this.val;
            }
        }
    }
}