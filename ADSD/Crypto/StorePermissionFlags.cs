using System;

namespace ADSD.Crypto
{
    /// <summary>Specifies the permitted access to X.509 certificate stores.</summary>
    [Flags]
    [Serializable]
    public enum StorePermissionFlags
    {
        /// <summary>Permission is not given to perform any certificate or store operations.</summary>
        NoFlags = 0,
        /// <summary>The ability to create a new store.</summary>
        CreateStore = 1,
        /// <summary>The ability to delete a store.</summary>
        DeleteStore = 2,
        /// <summary>The ability to enumerate the stores on a computer.</summary>
        EnumerateStores = 4,
        /// <summary>The ability to open a store.</summary>
        OpenStore = 16, // 0x00000010
        /// <summary>The ability to add a certificate to a store.</summary>
        AddToStore = 32, // 0x00000020
        /// <summary>The ability to remove a certificate from a store.</summary>
        RemoveFromStore = 64, // 0x00000040
        /// <summary>The ability to enumerate the certificates in a store.</summary>
        EnumerateCertificates = 128, // 0x00000080
        /// <summary>The ability to perform all certificate and store operations.</summary>
        AllFlags = EnumerateCertificates | RemoveFromStore | AddToStore | OpenStore | EnumerateStores | DeleteStore | CreateStore, // 0x000000F7
    }
}