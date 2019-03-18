﻿using System;
using System.Collections;

namespace ADSD.Crypto
{
    /// <summary>Represents the <see langword="&lt;ReferenceList&gt;" /> element used in XML encryption. This class cannot be inherited.</summary>
    public sealed class ReferenceList : IList, ICollection, IEnumerable
    {
        private readonly ArrayList m_references;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> class. </summary>
        public ReferenceList()
        {
            m_references = new ArrayList();
        }

        /// <summary>Returns an enumerator that iterates through a <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through a <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return m_references.GetEnumerator();
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object.</returns>
        public int Count
        {
            get
            {
                return m_references.Count;
            }
        }

        /// <summary>Adds a <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection.</summary>
        /// <param name="value">A <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to add to the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection.</param>
        /// <returns>The position at which the new element was inserted.</returns>
        /// <exception cref="T:System.ArgumentException">The <paramref name="value" /> parameter is not a <see cref="T:System.Security.Cryptography.Xml.DataReference" />  object.-or-The <paramref name="value" /> parameter is not a <see cref="T:System.Security.Cryptography.Xml.KeyReference" />  object.</exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
        public int Add(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            if (!(value is DataReference) && !(value is KeyReference))
                throw new ArgumentException("Cryptography_Xml_IncorrectObjectType", nameof (value));
            return m_references.Add(value);
        }

        /// <summary>Removes all items from the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection.</summary>
        public void Clear()
        {
            m_references.Clear();
        }

        /// <summary>Determines whether the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection contains a specific <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object.</summary>
        /// <param name="value">The <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to locate in the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection. </param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object is found in the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection; otherwise, <see langword="false" />. </returns>
        public bool Contains(object value)
        {
            return m_references.Contains(value);
        }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection.</summary>
        /// <param name="value">The <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to locate in the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection.</param>
        /// <returns>The index of <paramref name="value" /> if found in the collection; otherwise, -1.</returns>
        public int IndexOf(object value)
        {
            return m_references.IndexOf(value);
        }

        /// <summary>Inserts a <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object into the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection at the specified position.</summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
        /// <param name="value">A <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to insert into the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="value" /> parameter is not a <see cref="T:System.Security.Cryptography.Xml.DataReference" />  object.-or-The <paramref name="value" /> parameter is not a <see cref="T:System.Security.Cryptography.Xml.KeyReference" />  object.</exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
        public void Insert(int index, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            if (!(value is DataReference) && !(value is KeyReference))
                throw new ArgumentException("Cryptography_Xml_IncorrectObjectType", nameof (value));
            m_references.Insert(index, value);
        }

        /// <summary>Removes the first occurrence of a specific <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object from the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection.</summary>
        /// <param name="value">The <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to remove from the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> collection.</param>
        public void Remove(object value)
        {
            m_references.Remove(value);
        }

        /// <summary>Removes the <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object at the specified index.</summary>
        /// <param name="index">The zero-based index of the <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to remove.</param>
        public void RemoveAt(int index)
        {
            m_references.RemoveAt(index);
        }
 /*
        /// <summary>Returns the <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object at the specified index.</summary>
        /// <param name="index">The index of the <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to return.</param>
        /// <returns>The <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object at the specified index.</returns>
        public EncryptedReference Item(int index)
        {
            return (EncryptedReference) this.m_references[index];
        }
*/
       
        /// <summary>Gets or sets the <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object at the specified index.</summary>
        /// <param name="index">The index of the <see cref="T:System.Security.Cryptography.Xml.DataReference" /> or <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to return.</param>
        /// <returns>The <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object at the specified index.</returns>
        public EncryptedReference this[int index]
        {
            get
            {
                return (EncryptedReference)m_references[index];
            }
            set
            {
                ((IList) this)[index] = (object) value;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return m_references[index];
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                if (!(value is DataReference) && !(value is KeyReference))
                    throw new ArgumentException("Cryptography_Xml_IncorrectObjectType", nameof (value));
                m_references[index] = value;
            }
        }

        /// <summary>Copies the elements of the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object to an array, starting at a specified array index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> object that is the destination of the elements copied from the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            m_references.CopyTo(array, index);
        }

        bool IList.IsFixedSize
        {
            get
            {
                return m_references.IsFixedSize;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return m_references.IsReadOnly;
            }
        }

        /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object.</summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object.</returns>
        public object SyncRoot
        {
            get
            {
                return m_references.SyncRoot;
            }
        }

        /// <summary>Gets a value that indicates whether access to the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object is synchronized (thread safe).</summary>
        /// <returns>
        /// <see langword="true" /> if access to the <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object is synchronized (thread safe); otherwise, <see langword="false" />. </returns>
        public bool IsSynchronized
        {
            get
            {
                return m_references.IsSynchronized;
            }
        }
    }
}