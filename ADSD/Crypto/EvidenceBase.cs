using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;

namespace ADSD
{
    /// <summary>
    /// EvidenceBase
    /// </summary>
    [Serializable]
    public abstract class EvidenceBase
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Policy.EvidenceBase" /> class. </summary>
        /// <exception cref="T:System.InvalidOperationException">An object to be used as evidence is not serializable.</exception>
        protected EvidenceBase()
        {
            if (!this.GetType().IsSerializable)
                throw new InvalidOperationException("Policy_EvidenceMustBeSerializable");
        }

        /// <summary>Creates a new object that is a complete copy of the current instance.</summary>
        /// <returns>A duplicate copy of this evidence object.</returns>
        [SecuritySafeCritical]
        [SecurityPermission(SecurityAction.Assert, SerializationFormatter = true)]
        public virtual EvidenceBase Clone()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize((Stream) memoryStream, (object) this);
                memoryStream.Position = 0L;
                return binaryFormatter.Deserialize((Stream) memoryStream) as EvidenceBase;
            }
        }
    }
}