using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace System.Security.Policy
{
    internal interface ILegacyEvidenceAdapter
    {
        object EvidenceObject { get; }

        Type EvidenceType { get; }
    }
    
    internal interface IDelayEvaluatedEvidence
    {
        bool IsVerified { [SecurityCritical] get; }

        bool WasUsed { get; }

        void MarkUsed();
    }
}


namespace ADSD
{
    /// <summary>
    /// Policy evidence
    /// </summary>
    [Serializable]
    public sealed class Evidence : ICollection, IEnumerable
    {
        private Dictionary<Type, EvidenceTypeDescriptor> m_evidence;
        private bool m_deserializedTargetEvidence;
        private volatile ArrayList m_hostList;
        private volatile ArrayList m_assemblyList;
        [NonSerialized]
        private ReaderWriterLock m_evidenceLock;
        [NonSerialized]
        private uint m_version;
        [NonSerialized]
        private IRuntimeEvidenceFactory m_target;
        private bool m_locked;
        [NonSerialized]
        private WeakReference m_cloneOrigin;
        private static volatile Type[] s_runtimeEvidenceTypes;
        private const int LockTimeout = 5000;

        /// <summary>Initializes a new empty instance of the <see cref="T:System.Security.Policy.Evidence" /> class.</summary>
        public Evidence()
        {
            this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
            this.m_evidenceLock = new ReaderWriterLock();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Policy.Evidence" /> class from a shallow copy of an existing one.</summary>
        /// <param name="evidence">The <see cref="T:System.Security.Policy.Evidence" /> instance from which to create the new instance. This instance is not deep-copied. </param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="evidence" /> parameter is not a valid instance of <see cref="T:System.Security.Policy.Evidence" />. </exception>
        public Evidence(Evidence evidence)
        {
            this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
            if (evidence != null)
            {
                using (new Evidence.EvidenceLockHolder(evidence, Evidence.EvidenceLockHolder.LockType.Reader))
                {
                    foreach (KeyValuePair<Type, EvidenceTypeDescriptor> keyValuePair in evidence.m_evidence)
                    {
                        EvidenceTypeDescriptor evidenceTypeDescriptor = keyValuePair.Value;
                        if (evidenceTypeDescriptor != null)
                            evidenceTypeDescriptor = evidenceTypeDescriptor.Clone();
                        this.m_evidence[keyValuePair.Key] = evidenceTypeDescriptor;
                    }
                    this.m_target = evidence.m_target;
                    this.m_locked = evidence.m_locked;
                    this.m_deserializedTargetEvidence = evidence.m_deserializedTargetEvidence;
                    if (evidence.Target != null)
                        this.m_cloneOrigin = new WeakReference((object) evidence);
                }
            }
            this.m_evidenceLock = new ReaderWriterLock();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Policy.Evidence" /> class from multiple sets of host and assembly evidence.</summary>
        /// <param name="hostEvidence">The host evidence from which to create the new instance. </param>
        /// <param name="assemblyEvidence">The assembly evidence from which to create the new instance. </param>
        [Obsolete("This constructor is obsolete. Please use the constructor which takes arrays of EvidenceBase instead.")]
        public Evidence(object[] hostEvidence, object[] assemblyEvidence)
        {
            this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
            if (hostEvidence != null)
            {
                foreach (object id in hostEvidence)
                    this.AddHost(id);
            }
            if (assemblyEvidence != null)
            {
                foreach (object id in assemblyEvidence)
                    this.AddAssembly(id);
            }
            this.m_evidenceLock = new ReaderWriterLock();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Policy.Evidence" /> class from multiple sets of host and assembly evidence. </summary>
        /// <param name="hostEvidence">The host evidence from which to create the new instance. </param>
        /// <param name="assemblyEvidence">The assembly evidence from which to create the new instance. </param>
        public Evidence(EvidenceBase[] hostEvidence, EvidenceBase[] assemblyEvidence)
        {
            this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
            if (hostEvidence != null)
            {
                foreach (EvidenceBase evidence in hostEvidence)
                    this.AddHostEvidence(evidence, Evidence.GetEvidenceIndexType(evidence), Evidence.DuplicateEvidenceAction.Throw);
            }
            if (assemblyEvidence != null)
            {
                foreach (EvidenceBase evidence in assemblyEvidence)
                    this.AddAssemblyEvidence(evidence, Evidence.GetEvidenceIndexType(evidence), Evidence.DuplicateEvidenceAction.Throw);
            }
            this.m_evidenceLock = new ReaderWriterLock();
        }

        [SecuritySafeCritical]
        internal Evidence(IRuntimeEvidenceFactory target)
        {
            this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
            this.m_target = target;
            foreach (Type runtimeEvidenceType in Evidence.RuntimeEvidenceTypes)
                this.m_evidence[runtimeEvidenceType] = (EvidenceTypeDescriptor) null;
            this.QueryHostForPossibleEvidenceTypes();
            this.m_evidenceLock = new ReaderWriterLock();
        }

        internal static Type[] RuntimeEvidenceTypes
        {
            get
            {
                if (Evidence.s_runtimeEvidenceTypes == null)
                {
                    Type[] array = new Type[]
                    {
                        /*typeof (ActivationArguments),
                        typeof (ApplicationDirectory),
                        typeof (ApplicationTrust),
                        typeof (GacInstalled),
                        typeof (Hash),
                        typeof (Publisher),
                        typeof (Site),
                        typeof (StrongName),
                        typeof (Url),
                        typeof (Zone)*/
                    };
                    Evidence.s_runtimeEvidenceTypes = array;
                }
                return Evidence.s_runtimeEvidenceTypes;
            }
        }

        private bool IsReaderLockHeld
        {
            get
            {
                if (this.m_evidenceLock != null)
                    return this.m_evidenceLock.IsReaderLockHeld;
                return true;
            }
        }

        private bool IsWriterLockHeld
        {
            get
            {
                if (this.m_evidenceLock != null)
                    return this.m_evidenceLock.IsWriterLockHeld;
                return true;
            }
        }

        private void AcquireReaderLock()
        {
            if (this.m_evidenceLock == null)
                return;
            this.m_evidenceLock.AcquireReaderLock(5000);
        }

        private void AcquireWriterlock()
        {
            if (this.m_evidenceLock == null)
                return;
            this.m_evidenceLock.AcquireWriterLock(5000);
        }

        private void DowngradeFromWriterLock(ref LockCookie lockCookie)
        {
            if (this.m_evidenceLock == null)
                return;
            this.m_evidenceLock.DowngradeFromWriterLock(ref lockCookie);
        }

        private LockCookie UpgradeToWriterLock()
        {
            if (this.m_evidenceLock == null)
                return new LockCookie();
            return this.m_evidenceLock.UpgradeToWriterLock(5000);
        }

        private void ReleaseReaderLock()
        {
            if (this.m_evidenceLock == null)
                return;
            this.m_evidenceLock.ReleaseReaderLock();
        }

        private void ReleaseWriterLock()
        {
            if (this.m_evidenceLock == null)
                return;
            this.m_evidenceLock.ReleaseWriterLock();
        }

        /// <summary>Adds the specified evidence supplied by the host to the evidence set.</summary>
        /// <param name="id">Any evidence object. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="id" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="id" /> is not serializable.</exception>
        [SecuritySafeCritical]
        public void AddHost(object id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof (id));
            if (!id.GetType().IsSerializable)
                throw new ArgumentException("Policy_EvidenceMustBeSerializable", nameof (id));
            //if (this.m_locked)
              //  new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
            EvidenceBase evidence = Evidence.WrapLegacyEvidence(id);
            Type evidenceIndexType = Evidence.GetEvidenceIndexType(evidence);
            this.AddHostEvidence(evidence, evidenceIndexType, Evidence.DuplicateEvidenceAction.Merge);
        }

        /// <summary>Adds the specified assembly evidence to the evidence set.</summary>
        /// <param name="id">Any evidence object. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="id" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="id" /> is not serializable.</exception>
        public void AddAssembly(object id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof (id));
            if (!id.GetType().IsSerializable)
                throw new ArgumentException("Policy_EvidenceMustBeSerializable", nameof (id));
            EvidenceBase evidence = Evidence.WrapLegacyEvidence(id);
            Type evidenceIndexType = Evidence.GetEvidenceIndexType(evidence);
            this.AddAssemblyEvidence(evidence, evidenceIndexType, Evidence.DuplicateEvidenceAction.Merge);
        }

        /// <summary>Adds an evidence object of the specified type to the assembly-supplied evidence list. </summary>
        /// <param name="evidence">The assembly evidence to add.</param>
        /// <typeparam name="T">The type of the object in <paramref name="evidence" />.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="evidence" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="evidence" /> is not serializable.</exception>
        [ComVisible(false)]
        public void AddAssemblyEvidence<T>(T evidence) where T : EvidenceBase
        {
            if ((object) evidence == null)
                throw new ArgumentNullException(nameof (evidence));
            Type evidenceType = typeof (T);
            if (typeof (T) == typeof (EvidenceBase) || (object) evidence is ILegacyEvidenceAdapter)
                evidenceType = Evidence.GetEvidenceIndexType((EvidenceBase) evidence);
            this.AddAssemblyEvidence((EvidenceBase) evidence, evidenceType, Evidence.DuplicateEvidenceAction.Throw);
        }

        private void AddAssemblyEvidence(
            EvidenceBase evidence,
            Type evidenceType,
            Evidence.DuplicateEvidenceAction duplicateAction)
        {
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
                this.AddAssemblyEvidenceNoLock(evidence, evidenceType, duplicateAction);
        }

        private void AddAssemblyEvidenceNoLock(
            EvidenceBase evidence,
            Type evidenceType,
            Evidence.DuplicateEvidenceAction duplicateAction)
        {
            this.DeserializeTargetEvidence();
            EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(evidenceType, true);
            ++this.m_version;
            if (evidenceTypeDescriptor.AssemblyEvidence == null)
                evidenceTypeDescriptor.AssemblyEvidence = evidence;
            else
                evidenceTypeDescriptor.AssemblyEvidence = Evidence.HandleDuplicateEvidence(evidenceTypeDescriptor.AssemblyEvidence, evidence, duplicateAction);
        }

        /// <summary>Adds host evidence of the specified type to the host evidence collection.</summary>
        /// <param name="evidence">The host evidence to add.</param>
        /// <typeparam name="T">The type of the object in <paramref name="evidence" />.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="evidence" /> is <see langword="null" />.</exception>
        [ComVisible(false)]
        public void AddHostEvidence<T>(T evidence) where T : EvidenceBase
        {
            if ((object) evidence == null)
                throw new ArgumentNullException(nameof (evidence));
            Type evidenceType = typeof (T);
            if (typeof (T) == typeof (EvidenceBase) || (object) evidence is ILegacyEvidenceAdapter)
                evidenceType = Evidence.GetEvidenceIndexType((EvidenceBase) evidence);
            this.AddHostEvidence((EvidenceBase) evidence, evidenceType, Evidence.DuplicateEvidenceAction.Throw);
        }

        [SecuritySafeCritical]
        private void AddHostEvidence(
            EvidenceBase evidence,
            Type evidenceType,
            Evidence.DuplicateEvidenceAction duplicateAction)
        {
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
                this.AddHostEvidenceNoLock(evidence, evidenceType, duplicateAction);
        }

        private void AddHostEvidenceNoLock(
            EvidenceBase evidence,
            Type evidenceType,
            Evidence.DuplicateEvidenceAction duplicateAction)
        {
            EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(evidenceType, true);
            ++this.m_version;
            if (evidenceTypeDescriptor.HostEvidence == null)
                evidenceTypeDescriptor.HostEvidence = evidence;
            else
                evidenceTypeDescriptor.HostEvidence = Evidence.HandleDuplicateEvidence(evidenceTypeDescriptor.HostEvidence, evidence, duplicateAction);
        }

        [SecurityCritical]
        private void QueryHostForPossibleEvidenceTypes()
        {
            /*
            if (AppDomain.CurrentDomain.DomainManager == null)
                return;
            HostSecurityManager hostSecurityManager = AppDomain.CurrentDomain.DomainManager.HostSecurityManager;
            if (hostSecurityManager == null)
                return;
            Type[] typeArray = (Type[]) null;
            AppDomain target1 = this.m_target.Target as AppDomain;
            Assembly target2 = this.m_target.Target as Assembly;
            if (target2 != (Assembly) null && (hostSecurityManager.Flags & HostSecurityManagerOptions.HostAssemblyEvidence) == HostSecurityManagerOptions.HostAssemblyEvidence)
                typeArray = hostSecurityManager.GetHostSuppliedAssemblyEvidenceTypes(target2);
            else if (target1 != null && (hostSecurityManager.Flags & HostSecurityManagerOptions.HostAppDomainEvidence) == HostSecurityManagerOptions.HostAppDomainEvidence)
                typeArray = hostSecurityManager.GetHostSuppliedAppDomainEvidenceTypes();
            if (typeArray == null)
                return;
            foreach (Type evidenceType in typeArray)
                this.GetEvidenceTypeDescriptor(evidenceType, true).HostCanGenerate = true;
                */
        }

        internal bool IsUnmodified
        {
            get
            {
                return this.m_version == 0U;
            }
        }

        /// <summary>Gets or sets a value indicating whether the evidence is locked.</summary>
        /// <returns>
        /// <see langword="true" /> if the evidence is locked; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
        public bool Locked
        {
            get
            {
                return this.m_locked;
            }
            [SecuritySafeCritical] set
            {
                if (!value)
                {
                    this.m_locked = false;
                }
                else
                    this.m_locked = true;
            }
        }

        internal IRuntimeEvidenceFactory Target
        {
            get
            {
                return this.m_target;
            }
            [SecurityCritical] set
            {
                using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
                {
                    this.m_target = value;
                    this.QueryHostForPossibleEvidenceTypes();
                }
            }
        }

        private static Type GetEvidenceIndexType(EvidenceBase evidence)
        {
            ILegacyEvidenceAdapter legacyEvidenceAdapter = evidence as ILegacyEvidenceAdapter;
            if (legacyEvidenceAdapter != null)
                return legacyEvidenceAdapter.EvidenceType;
            return evidence.GetType();
        }

        internal EvidenceTypeDescriptor GetEvidenceTypeDescriptor(
            Type evidenceType)
        {
            return this.GetEvidenceTypeDescriptor(evidenceType, false);
        }

        private EvidenceTypeDescriptor GetEvidenceTypeDescriptor(
            Type evidenceType,
            bool addIfNotExist)
        {
            EvidenceTypeDescriptor evidenceTypeDescriptor = (EvidenceTypeDescriptor) null;
            if (!this.m_evidence.TryGetValue(evidenceType, out evidenceTypeDescriptor) && !addIfNotExist)
                return (EvidenceTypeDescriptor) null;
            if (evidenceTypeDescriptor == null)
            {
                evidenceTypeDescriptor = new EvidenceTypeDescriptor();
                bool flag = false;
                LockCookie lockCookie = new LockCookie();
                try
                {
                    if (!this.IsWriterLockHeld)
                    {
                        lockCookie = this.UpgradeToWriterLock();
                        flag = true;
                    }
                    this.m_evidence[evidenceType] = evidenceTypeDescriptor;
                }
                finally
                {
                    if (flag)
                        this.DowngradeFromWriterLock(ref lockCookie);
                }
            }
            return evidenceTypeDescriptor;
        }

        private static EvidenceBase HandleDuplicateEvidence(
            EvidenceBase original,
            EvidenceBase duplicate,
            Evidence.DuplicateEvidenceAction action)
        {
            switch (action)
            {
                case Evidence.DuplicateEvidenceAction.Throw:
                    throw new InvalidOperationException("Policy_DuplicateEvidence: " + duplicate.GetType().FullName);
                case Evidence.DuplicateEvidenceAction.Merge:
                    LegacyEvidenceList legacyEvidenceList = original as LegacyEvidenceList;
                    if (legacyEvidenceList == null)
                    {
                        legacyEvidenceList = new LegacyEvidenceList();
                        legacyEvidenceList.Add(original);
                    }
                    legacyEvidenceList.Add(duplicate);
                    return (EvidenceBase) legacyEvidenceList;
                case Evidence.DuplicateEvidenceAction.SelectNewObject:
                    return duplicate;
                default:
                    return (EvidenceBase) null;
            }
        }

        private static EvidenceBase WrapLegacyEvidence(object evidence)
        {
            return evidence as EvidenceBase ?? (EvidenceBase) new LegacyEvidenceWrapper(evidence);
        }

        private static object UnwrapEvidence(EvidenceBase evidence)
        {
            ILegacyEvidenceAdapter legacyEvidenceAdapter = evidence as ILegacyEvidenceAdapter;
            if (legacyEvidenceAdapter != null)
                return legacyEvidenceAdapter.EvidenceObject;
            return (object) evidence;
        }

        /// <summary>Merges the specified evidence set into the current evidence set.</summary>
        /// <param name="evidence">The evidence set to be merged into the current evidence set. </param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="evidence" /> parameter is not a valid instance of <see cref="T:System.Security.Policy.Evidence" />. </exception>
        /// <exception cref="T:System.Security.SecurityException">
        /// <see cref="P:System.Security.Policy.Evidence.Locked" /> is <see langword="true" />, the code that calls this method does not have <see cref="F:System.Security.Permissions.SecurityPermissionFlag.ControlEvidence" />, and the <paramref name="evidence" /> parameter has a host list that is not empty. </exception>
        [SecuritySafeCritical]
        public void Merge(Evidence evidence)
        {
            if (evidence == null)
                return;
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
            {
                bool flag = false;
                IEnumerator hostEnumerator = evidence.GetHostEnumerator();
                while (hostEnumerator.MoveNext())
                {
                    if (this.Locked && !flag)
                    {
                        flag = true;
                    }
                    Type type = hostEnumerator.Current.GetType();
                    if (this.m_evidence.ContainsKey(type))
                        this.GetHostEvidenceNoLock(type);
                    EvidenceBase evidence1 = Evidence.WrapLegacyEvidence(hostEnumerator.Current);
                    this.AddHostEvidenceNoLock(evidence1, Evidence.GetEvidenceIndexType(evidence1), Evidence.DuplicateEvidenceAction.Merge);
                }
                IEnumerator assemblyEnumerator = evidence.GetAssemblyEnumerator();
                while (assemblyEnumerator.MoveNext())
                {
                    EvidenceBase evidence1 = Evidence.WrapLegacyEvidence(assemblyEnumerator.Current);
                    this.AddAssemblyEvidenceNoLock(evidence1, Evidence.GetEvidenceIndexType(evidence1), Evidence.DuplicateEvidenceAction.Merge);
                }
            }
        }

        internal void MergeWithNoDuplicates(Evidence evidence)
        {
            if (evidence == null)
                return;
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
            {
                IEnumerator hostEnumerator = evidence.GetHostEnumerator();
                while (hostEnumerator.MoveNext())
                {
                    EvidenceBase evidence1 = Evidence.WrapLegacyEvidence(hostEnumerator.Current);
                    this.AddHostEvidenceNoLock(evidence1, Evidence.GetEvidenceIndexType(evidence1), Evidence.DuplicateEvidenceAction.SelectNewObject);
                }
                IEnumerator assemblyEnumerator = evidence.GetAssemblyEnumerator();
                while (assemblyEnumerator.MoveNext())
                {
                    EvidenceBase evidence1 = Evidence.WrapLegacyEvidence(assemblyEnumerator.Current);
                    this.AddAssemblyEvidenceNoLock(evidence1, Evidence.GetEvidenceIndexType(evidence1), Evidence.DuplicateEvidenceAction.SelectNewObject);
                }
            }
        }

        [ComVisible(false)]
        [OnSerializing]
        [SecurityCritical]
        private void OnSerializing(StreamingContext context)
        {
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
            {
                foreach (Type type in new List<Type>((IEnumerable<Type>) this.m_evidence.Keys))
                    this.GetHostEvidenceNoLock(type);
                this.DeserializeTargetEvidence();
            }
            ArrayList arrayList1 = new ArrayList();
            IEnumerator hostEnumerator = this.GetHostEnumerator();
            while (hostEnumerator.MoveNext())
                arrayList1.Add(hostEnumerator.Current);
            this.m_hostList = arrayList1;
            ArrayList arrayList2 = new ArrayList();
            IEnumerator assemblyEnumerator = this.GetAssemblyEnumerator();
            while (assemblyEnumerator.MoveNext())
                arrayList2.Add(assemblyEnumerator.Current);
            this.m_assemblyList = arrayList2;
        }

        [ComVisible(false)]
        [OnDeserialized]
        [SecurityCritical]
        private void OnDeserialized(StreamingContext context)
        {
            if (this.m_evidence == null)
            {
                this.m_evidence = new Dictionary<Type, EvidenceTypeDescriptor>();
                if (this.m_hostList != null)
                {
                    foreach (object host in this.m_hostList)
                    {
                        if (host != null)
                            this.AddHost(host);
                    }
                    this.m_hostList = (ArrayList) null;
                }
                if (this.m_assemblyList != null)
                {
                    foreach (object assembly in this.m_assemblyList)
                    {
                        if (assembly != null)
                            this.AddAssembly(assembly);
                    }
                    this.m_assemblyList = (ArrayList) null;
                }
            }
            this.m_evidenceLock = new ReaderWriterLock();
        }

        private void DeserializeTargetEvidence()
        {
            if (this.m_target == null || this.m_deserializedTargetEvidence)
                return;
            bool flag = false;
            LockCookie lockCookie = new LockCookie();
            try
            {
                if (!this.IsWriterLockHeld)
                {
                    lockCookie = this.UpgradeToWriterLock();
                    flag = true;
                }
                this.m_deserializedTargetEvidence = true;
                foreach (EvidenceBase evidence in this.m_target.GetFactorySuppliedEvidence())
                    this.AddAssemblyEvidenceNoLock(evidence, Evidence.GetEvidenceIndexType(evidence), Evidence.DuplicateEvidenceAction.Throw);
            }
            finally
            {
                if (flag)
                    this.DowngradeFromWriterLock(ref lockCookie);
            }
        }

        [SecurityCritical]
        internal byte[] RawSerialize()
        {
            try
            {
                using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
                {
                    Dictionary<Type, EvidenceBase> dictionary = new Dictionary<Type, EvidenceBase>();
                    foreach (KeyValuePair<Type, EvidenceTypeDescriptor> keyValuePair in this.m_evidence)
                    {
                        if (keyValuePair.Value != null && keyValuePair.Value.HostEvidence != null)
                            dictionary[keyValuePair.Key] = keyValuePair.Value.HostEvidence;
                    }
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        new BinaryFormatter().Serialize((Stream) memoryStream, (object) dictionary);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (SecurityException)
            {
                return (byte[]) null;
            }
        }

        /// <summary>Copies evidence objects to an <see cref="T:System.Array" />.</summary>
        public void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof (array));
            if (index < 0 || index > array.Length - this.Count)
                throw new ArgumentOutOfRangeException(nameof (index));
            int index1 = index;
            IEnumerator hostEnumerator = this.GetHostEnumerator();
            while (hostEnumerator.MoveNext())
            {
                array.SetValue(hostEnumerator.Current, index1);
                ++index1;
            }
            IEnumerator assemblyEnumerator = this.GetAssemblyEnumerator();
            while (assemblyEnumerator.MoveNext())
            {
                array.SetValue(assemblyEnumerator.Current, index1);
                ++index1;
            }
        }

        /// <summary>Enumerates evidence supplied by the host.</summary>
        /// <returns>An enumerator for evidence added by the <see cref="M:System.Security.Policy.Evidence.AddHost(System.Object)" /> method.</returns>
        public IEnumerator GetHostEnumerator()
        {
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
                return (IEnumerator) new Evidence.EvidenceEnumerator(this, Evidence.EvidenceEnumerator.Category.Host);
        }

        /// <summary>Enumerates evidence provided by the assembly.</summary>
        /// <returns>An enumerator for evidence added by the <see cref="M:System.Security.Policy.Evidence.AddAssembly(System.Object)" /> method.</returns>
        public IEnumerator GetAssemblyEnumerator()
        {
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
            {
                this.DeserializeTargetEvidence();
                return (IEnumerator) new Evidence.EvidenceEnumerator(this, Evidence.EvidenceEnumerator.Category.Assembly);
            }
        }

        internal Evidence.RawEvidenceEnumerator GetRawAssemblyEvidenceEnumerator()
        {
            this.DeserializeTargetEvidence();
            return new Evidence.RawEvidenceEnumerator(this, (IEnumerable<Type>) new List<Type>((IEnumerable<Type>) this.m_evidence.Keys), false);
        }

        internal Evidence.RawEvidenceEnumerator GetRawHostEvidenceEnumerator()
        {
            return new Evidence.RawEvidenceEnumerator(this, (IEnumerable<Type>) new List<Type>((IEnumerable<Type>) this.m_evidence.Keys), true);
        }

        /// <summary>Enumerates all evidence in the set, both that provided by the host and that provided by the assembly.</summary>
        /// <returns>An enumerator for evidence added by both the <see cref="M:System.Security.Policy.Evidence.AddHost(System.Object)" /> method and the <see cref="M:System.Security.Policy.Evidence.AddAssembly(System.Object)" /> method.</returns>
        public IEnumerator GetEnumerator()
        {
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
                return (IEnumerator) new Evidence.EvidenceEnumerator(this, Evidence.EvidenceEnumerator.Category.Host | Evidence.EvidenceEnumerator.Category.Assembly);
        }

        /// <summary>Gets assembly evidence of the specified type from the collection.</summary>
        /// <typeparam name="T">The type of the evidence to get.</typeparam>
        [ComVisible(false)]
        public T GetAssemblyEvidence<T>() where T : EvidenceBase
        {
            return Evidence.UnwrapEvidence(this.GetAssemblyEvidence(typeof (T))) as T;
        }

        internal EvidenceBase GetAssemblyEvidence(Type type)
        {
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
                return this.GetAssemblyEvidenceNoLock(type);
        }

        private EvidenceBase GetAssemblyEvidenceNoLock(Type type)
        {
            this.DeserializeTargetEvidence();
            return this.GetEvidenceTypeDescriptor(type)?.AssemblyEvidence;
        }

        /// <summary>Gets host evidence of the specified type from the collection.</summary>
        /// <typeparam name="T">The type of the evidence to get.</typeparam>
        [ComVisible(false)]
        public T GetHostEvidence<T>() where T : EvidenceBase
        {
            return Evidence.UnwrapEvidence(this.GetHostEvidence(typeof (T))) as T;
        }

        internal T GetDelayEvaluatedHostEvidence<T>() where T : EvidenceBase, IDelayEvaluatedEvidence
        {
            return Evidence.UnwrapEvidence(this.GetHostEvidence(typeof (T), false)) as T;
        }

        internal EvidenceBase GetHostEvidence(Type type)
        {
            return this.GetHostEvidence(type, true);
        }

        [SecuritySafeCritical]
        private EvidenceBase GetHostEvidence(Type type, bool markDelayEvaluatedEvidenceUsed)
        {
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
            {
                EvidenceBase hostEvidenceNoLock = this.GetHostEvidenceNoLock(type);
                if (markDelayEvaluatedEvidenceUsed)
                    (hostEvidenceNoLock as IDelayEvaluatedEvidence)?.MarkUsed();
                return hostEvidenceNoLock;
            }
        }

        [SecurityCritical]
        private EvidenceBase GetHostEvidenceNoLock(Type type)
        {
            EvidenceTypeDescriptor evidenceTypeDescriptor1 = this.GetEvidenceTypeDescriptor(type);
            if (evidenceTypeDescriptor1 == null)
                return (EvidenceBase) null;
            if (evidenceTypeDescriptor1.HostEvidence != null)
                return evidenceTypeDescriptor1.HostEvidence;
            if (this.m_target == null || evidenceTypeDescriptor1.Generated)
                return (EvidenceBase) null;
            using (new Evidence.EvidenceUpgradeLockHolder(this))
            {
                evidenceTypeDescriptor1.Generated = true;
                EvidenceBase hostEvidence = this.GenerateHostEvidence(type, evidenceTypeDescriptor1.HostCanGenerate);
                if (hostEvidence != null)
                {
                    evidenceTypeDescriptor1.HostEvidence = hostEvidence;
                    Evidence target = this.m_cloneOrigin != null ? this.m_cloneOrigin.Target as Evidence : (Evidence) null;
                    if (target != null)
                    {
                        using (new Evidence.EvidenceLockHolder(target, Evidence.EvidenceLockHolder.LockType.Writer))
                        {
                            EvidenceTypeDescriptor evidenceTypeDescriptor2 = target.GetEvidenceTypeDescriptor(type);
                            if (evidenceTypeDescriptor2 != null)
                            {
                                if (evidenceTypeDescriptor2.HostEvidence == null)
                                    evidenceTypeDescriptor2.HostEvidence = hostEvidence.Clone();
                            }
                        }
                    }
                }
                return hostEvidence;
            }
        }

        [SecurityCritical]
        private EvidenceBase GenerateHostEvidence(Type type, bool hostCanGenerate)
        {
            /*
            if (hostCanGenerate)
            {
                AppDomain target1 = this.m_target.Target as AppDomain;
                Assembly target2 = this.m_target.Target as Assembly;
                EvidenceBase evidenceBase = (EvidenceBase) null;
                if (target1 != null)
                    evidenceBase = AppDomain.CurrentDomain.HostSecurityManager.GenerateAppDomainEvidence(type);
                else if (target2 != (Assembly) null)
                    evidenceBase = AppDomain.CurrentDomain.HostSecurityManager.GenerateAssemblyEvidence(type, target2);
                if (evidenceBase != null)
                {
                    if (!type.IsAssignableFrom(evidenceBase.GetType()))
                        throw new InvalidOperationException(Environment.GetResourceString("Policy_IncorrectHostEvidence", (object) AppDomain.CurrentDomain.HostSecurityManager.GetType().FullName, (object) evidenceBase.GetType().FullName, (object) type.FullName));
                    return evidenceBase;
                }
            }
            return this.m_target.GenerateEvidence(type);*/
            return null;
        }

        /// <summary>Gets the number of evidence objects in the evidence set.</summary>
        /// <returns>The number of evidence objects in the evidence set.</returns>
        public int Count
        {
            get
            {
                int num = 0;
                IEnumerator hostEnumerator = this.GetHostEnumerator();
                while (hostEnumerator.MoveNext())
                    ++num;
                IEnumerator assemblyEnumerator = this.GetAssemblyEnumerator();
                while (assemblyEnumerator.MoveNext())
                    ++num;
                return num;
            }
        }

        [ComVisible(false)]
        internal int RawCount
        {
            get
            {
                int num = 0;
                using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
                {
                    foreach (Type evidenceType in new List<Type>((IEnumerable<Type>) this.m_evidence.Keys))
                    {
                        EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(evidenceType);
                        if (evidenceTypeDescriptor != null)
                        {
                            if (evidenceTypeDescriptor.AssemblyEvidence != null)
                                ++num;
                            if (evidenceTypeDescriptor.HostEvidence != null)
                                ++num;
                        }
                    }
                }
                return num;
            }
        }

        /// <summary>Gets the synchronization root.</summary>
        /// <returns>Always <see langword="this" /> (<see langword="Me" /> in Visual Basic), because synchronization of evidence sets is not supported.</returns>
        public object SyncRoot
        {
            get
            {
                return (object) this;
            }
        }

        /// <summary>Gets a value indicating whether the evidence set is thread-safe.</summary>
        /// <returns>Always <see langword="false" /> because thread-safe evidence sets are not supported.</returns>
        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        /// <summary>Gets a value indicating whether the evidence set is read-only.</summary>
        /// <returns>Always <see langword="false" />, because read-only evidence sets are not supported.</returns>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>Returns a duplicate copy of this evidence object.</summary>
        /// <returns>A duplicate copy of this evidence object.</returns>
        [ComVisible(false)]
        public Evidence Clone()
        {
            return new Evidence(this);
        }

        /// <summary>Removes the host and assembly evidence from the evidence set.</summary>
        [ComVisible(false)]
        [SecuritySafeCritical]
        public void Clear()
        {
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
            {
                ++this.m_version;
                this.m_evidence.Clear();
            }
        }

        /// <summary>Removes the evidence for a given type from the host and assembly enumerations.</summary>
        /// <param name="t">The type of the evidence to be removed. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="t" /> is null.</exception>
        [ComVisible(false)]
        [SecuritySafeCritical]
        public void RemoveType(Type t)
        {
            if (t == (Type) null)
                throw new ArgumentNullException(nameof (t));
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Writer))
            {
                EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(t);
                if (evidenceTypeDescriptor == null)
                    return;
                ++this.m_version;
                this.m_evidence.Remove(t);
            }
        }

        internal void MarkAllEvidenceAsUsed()
        {
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
            {
                foreach (KeyValuePair<Type, EvidenceTypeDescriptor> keyValuePair in this.m_evidence)
                {
                    if (keyValuePair.Value != null)
                    {
                        (keyValuePair.Value.HostEvidence as IDelayEvaluatedEvidence)?.MarkUsed();
                        (keyValuePair.Value.AssemblyEvidence as IDelayEvaluatedEvidence)?.MarkUsed();
                    }
                }
            }
        }

        private bool WasStrongNameEvidenceUsed()
        {
            return false;/*
            using (new Evidence.EvidenceLockHolder(this, Evidence.EvidenceLockHolder.LockType.Reader))
            {
                EvidenceTypeDescriptor evidenceTypeDescriptor = this.GetEvidenceTypeDescriptor(typeof (StrongName));
                if (evidenceTypeDescriptor == null)
                    return false;
                IDelayEvaluatedEvidence hostEvidence = evidenceTypeDescriptor.HostEvidence as IDelayEvaluatedEvidence;
                return hostEvidence != null && hostEvidence.WasUsed;
            }*/
        }

        private enum DuplicateEvidenceAction
        {
            Throw,
            Merge,
            SelectNewObject,
        }

        private class EvidenceLockHolder : IDisposable
        {
            private Evidence m_target;
            private Evidence.EvidenceLockHolder.LockType m_lockType;

            public EvidenceLockHolder(Evidence target, Evidence.EvidenceLockHolder.LockType lockType)
            {
                this.m_target = target;
                this.m_lockType = lockType;
                if (this.m_lockType == Evidence.EvidenceLockHolder.LockType.Reader)
                    this.m_target.AcquireReaderLock();
                else
                    this.m_target.AcquireWriterlock();
            }

            public void Dispose()
            {
                if (this.m_lockType == Evidence.EvidenceLockHolder.LockType.Reader && this.m_target.IsReaderLockHeld)
                {
                    this.m_target.ReleaseReaderLock();
                }
                else
                {
                    if (this.m_lockType != Evidence.EvidenceLockHolder.LockType.Writer || !this.m_target.IsWriterLockHeld)
                        return;
                    this.m_target.ReleaseWriterLock();
                }
            }

            public enum LockType
            {
                Reader,
                Writer,
            }
        }

        private class EvidenceUpgradeLockHolder : IDisposable
        {
            private Evidence m_target;
            private LockCookie m_cookie;

            public EvidenceUpgradeLockHolder(Evidence target)
            {
                this.m_target = target;
                this.m_cookie = this.m_target.UpgradeToWriterLock();
            }

            public void Dispose()
            {
                if (!this.m_target.IsWriterLockHeld)
                    return;
                this.m_target.DowngradeFromWriterLock(ref this.m_cookie);
            }
        }

        internal sealed class RawEvidenceEnumerator : IEnumerator<EvidenceBase>, IDisposable, IEnumerator
        {
            private Evidence m_evidence;
            private bool m_hostEnumerator;
            private uint m_evidenceVersion;
            private Type[] m_evidenceTypes;
            private int m_typeIndex;
            private EvidenceBase m_currentEvidence;
            //private static volatile List<Type> s_expensiveEvidence;

            public RawEvidenceEnumerator(
                Evidence evidence,
                IEnumerable<Type> evidenceTypes,
                bool hostEnumerator)
            {
                this.m_evidence = evidence;
                this.m_hostEnumerator = hostEnumerator;
                this.m_evidenceTypes = Evidence.RawEvidenceEnumerator.GenerateEvidenceTypes(evidence, evidenceTypes, hostEnumerator);
                this.m_evidenceVersion = evidence.m_version;
                this.Reset();
            }

            public EvidenceBase Current
            {
                get
                {
                    if ((int) this.m_evidence.m_version != (int) this.m_evidenceVersion)
                        throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                    return this.m_currentEvidence;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if ((int) this.m_evidence.m_version != (int) this.m_evidenceVersion)
                        throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                    return (object) this.m_currentEvidence;
                }
            }

            private static List<Type> ExpensiveEvidence
            {
                get
                {
                    /*if (Evidence.RawEvidenceEnumerator.s_expensiveEvidence == null)
                        Evidence.RawEvidenceEnumerator.s_expensiveEvidence = new List<Type>()
                        {
                            typeof (Hash),
                            typeof (Publisher)
                        };
                    return Evidence.RawEvidenceEnumerator.s_expensiveEvidence;*/
                    return new List<Type>();
                }
            }

            public void Dispose()
            {
            }

            private static Type[] GenerateEvidenceTypes(
                Evidence evidence,
                IEnumerable<Type> evidenceTypes,
                bool hostEvidence)
            {
                List<Type> typeList1 = new List<Type>();
                List<Type> typeList2 = new List<Type>();
                List<Type> typeList3 = new List<Type>(Evidence.RawEvidenceEnumerator.ExpensiveEvidence.Count);
                foreach (Type evidenceType in evidenceTypes)
                {
                    EvidenceTypeDescriptor evidenceTypeDescriptor = evidence.GetEvidenceTypeDescriptor(evidenceType);
                    if (hostEvidence && evidenceTypeDescriptor.HostEvidence != null || !hostEvidence && evidenceTypeDescriptor.AssemblyEvidence != null)
                        typeList1.Add(evidenceType);
                    else if (Evidence.RawEvidenceEnumerator.ExpensiveEvidence.Contains(evidenceType))
                        typeList3.Add(evidenceType);
                    else
                        typeList2.Add(evidenceType);
                }
                Type[] array = new Type[typeList1.Count + typeList2.Count + typeList3.Count];
                typeList1.CopyTo(array, 0);
                typeList2.CopyTo(array, typeList1.Count);
                typeList3.CopyTo(array, typeList1.Count + typeList2.Count);
                return array;
            }

            [SecuritySafeCritical]
            public bool MoveNext()
            {
                using (new Evidence.EvidenceLockHolder(this.m_evidence, Evidence.EvidenceLockHolder.LockType.Reader))
                {
                    if ((int) this.m_evidence.m_version != (int) this.m_evidenceVersion)
                        throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                    this.m_currentEvidence = (EvidenceBase) null;
                    do
                    {
                        ++this.m_typeIndex;
                        if (this.m_typeIndex < this.m_evidenceTypes.Length)
                            this.m_currentEvidence = !this.m_hostEnumerator ? this.m_evidence.GetAssemblyEvidenceNoLock(this.m_evidenceTypes[this.m_typeIndex]) : this.m_evidence.GetHostEvidenceNoLock(this.m_evidenceTypes[this.m_typeIndex]);
                        if (this.m_typeIndex >= this.m_evidenceTypes.Length)
                            break;
                    }
                    while (this.m_currentEvidence == null);
                }
                return this.m_currentEvidence != null;
            }

            public void Reset()
            {
                if ((int) this.m_evidence.m_version != (int) this.m_evidenceVersion)
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                this.m_typeIndex = -1;
                this.m_currentEvidence = (EvidenceBase) null;
            }
        }

        private sealed class EvidenceEnumerator : IEnumerator
        {
            private Evidence m_evidence;
            private Evidence.EvidenceEnumerator.Category m_category;
            private Stack m_enumerators;
            private object m_currentEvidence;

            internal EvidenceEnumerator(Evidence evidence, Evidence.EvidenceEnumerator.Category category)
            {
                this.m_evidence = evidence;
                this.m_category = category;
                this.ResetNoLock();
            }

            public bool MoveNext()
            {
                IEnumerator currentEnumerator = this.CurrentEnumerator;
                if (currentEnumerator == null)
                {
                    this.m_currentEvidence = (object) null;
                    return false;
                }
                if (currentEnumerator.MoveNext())
                {
                    LegacyEvidenceWrapper current1 = currentEnumerator.Current as LegacyEvidenceWrapper;
                    LegacyEvidenceList current2 = currentEnumerator.Current as LegacyEvidenceList;
                    if (current1 != null)
                        this.m_currentEvidence = current1.EvidenceObject;
                    else if (current2 != null)
                    {
                        this.m_enumerators.Push((object) current2.GetEnumerator());
                        this.MoveNext();
                    }
                    else
                        this.m_currentEvidence = currentEnumerator.Current;
                    return true;
                }
                this.m_enumerators.Pop();
                return this.MoveNext();
            }

            public object Current
            {
                get
                {
                    return this.m_currentEvidence;
                }
            }

            private IEnumerator CurrentEnumerator
            {
                get
                {
                    if (this.m_enumerators.Count <= 0)
                        return (IEnumerator) null;
                    return this.m_enumerators.Peek() as IEnumerator;
                }
            }

            public void Reset()
            {
                using (new Evidence.EvidenceLockHolder(this.m_evidence, Evidence.EvidenceLockHolder.LockType.Reader))
                    this.ResetNoLock();
            }

            private void ResetNoLock()
            {
                this.m_currentEvidence = (object) null;
                this.m_enumerators = new Stack();
                if ((this.m_category & Evidence.EvidenceEnumerator.Category.Host) == Evidence.EvidenceEnumerator.Category.Host)
                    this.m_enumerators.Push((object) this.m_evidence.GetRawHostEvidenceEnumerator());
                if ((this.m_category & Evidence.EvidenceEnumerator.Category.Assembly) != Evidence.EvidenceEnumerator.Category.Assembly)
                    return;
                this.m_enumerators.Push((object) this.m_evidence.GetRawAssemblyEvidenceEnumerator());
            }

            [Flags]
            internal enum Category
            {
                Host = 1,
                Assembly = 2,
            }
        }
    }
}