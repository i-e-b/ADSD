using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace ADSD.Crypto
{
    // TODO: replace with List<EncryptionProperty>

    /// <summary>Represents a collection of <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> classes used in XML encryption. This class cannot be inherited.</summary>
    public sealed class EncryptionPropertyCollection : IList, ICollection, IEnumerable
    {
        private readonly ArrayList m_props;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> class.</summary>
        public EncryptionPropertyCollection()
        {
            m_props = new ArrayList();
        }

        /// <summary>Returns an enumerator that iterates through an <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through an <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</returns>
        public IEnumerator GetEnumerator()
        {
            return m_props.GetEnumerator();
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</returns>
        public int Count
        {
            get
            {
                return m_props.Count;
            }
        }

        int IList.Add(object value)
        {
            if (!(value is EncryptionProperty))
                throw new ArgumentException("Cryptography_Xml_IncorrectObjectType", nameof (value));
            return m_props.Add(value);
        }

        /// <summary>Adds an <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
        /// <param name="value">An <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to add to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</param>
        /// <returns>The position at which the new element is inserted.</returns>
        public int Add(EncryptionProperty value)
        {
            return m_props.Add((object) value);
        }

        /// <summary>Removes all items from the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
        public void Clear()
        {
            m_props.Clear();
        }

        bool IList.Contains(object value)
        {
            if (!(value is EncryptionProperty))
                throw new ArgumentException("Cryptography_Xml_IncorrectObjectType", nameof (value));
            return m_props.Contains(value);
        }

        /// <summary>Determines whether the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object contains a specific <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object.</summary>
        /// <param name="value">The <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to locate in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object. </param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object is found in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object; otherwise, <see langword="false" />. </returns>
        public bool Contains(EncryptionProperty value)
        {
            return m_props.Contains((object) value);
        }

        int IList.IndexOf(object value)
        {
            if (!(value is EncryptionProperty))
                throw new ArgumentException("Cryptography_Xml_IncorrectObjectType", nameof (value));
            return m_props.IndexOf(value);
        }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
        /// <param name="value">The <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to locate in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</param>
        /// <returns>The index of <paramref name="value" /> if found in the collection; otherwise, -1.</returns>
        public int IndexOf(EncryptionProperty value)
        {
            return m_props.IndexOf((object) value);
        }

        void IList.Insert(int index, object value)
        {
            if (!(value is EncryptionProperty))
                throw new ArgumentException("Cryptography_Xml_IncorrectObjectType", nameof (value));
            m_props.Insert(index, value);
        }

        /// <summary>Inserts an <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object into the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object at the specified position.</summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
        /// <param name="value">An <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to insert into the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</param>
        public void Insert(int index, EncryptionProperty value)
        {
            m_props.Insert(index, (object) value);
        }

        void IList.Remove(object value)
        {
            if (!(value is EncryptionProperty))
                throw new ArgumentException("Cryptography_Xml_IncorrectObjectType", nameof (value));
            m_props.Remove(value);
        }

        /// <summary>Removes the first occurrence of a specific <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object from the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
        /// <param name="value">The <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to remove from the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</param>
        public void Remove(EncryptionProperty value)
        {
            m_props.Remove((object) value);
        }

        /// <summary>Removes the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object at the specified index.</summary>
        /// <param name="index">The zero-based index of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to remove.</param>
        public void RemoveAt(int index)
        {
            m_props.RemoveAt(index);
        }

        /// <summary>Gets a value that indicates whether the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object has a fixed size.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object has a fixed size; otherwise, <see langword="false" />.</returns>
        public bool IsFixedSize
        {
            get
            {
                return m_props.IsFixedSize;
            }
        }

        /// <summary>Gets a value that indicates whether the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object is read-only.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object is read-only; otherwise, <see langword="false" />. </returns>
        public bool IsReadOnly
        {
            get
            {
                return m_props.IsReadOnly;
            }
        }

        /// <summary>Returns the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object at the specified index.</summary>
        /// <param name="index">The index of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to return.</param>
        /// <returns>The <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object at the specified index.</returns>
        public EncryptionProperty Item(int index)
        {
            return (EncryptionProperty) m_props[index];
        }

        /// <summary>Gets or sets the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object at the specified index.</summary>
        /// <param name="index">The index of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to return.</param>
        /// <returns>The <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object at the specified index.</returns>
        [IndexerName("ItemOf")]
        public EncryptionProperty this[int index]
        {
            get
            {
                return (EncryptionProperty) ((IList) this)[index];
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
                return m_props[index];
            }
            set
            {
                if (!(value is EncryptionProperty))
                    throw new ArgumentException("Cryptography_Xml_IncorrectObjectType", nameof (value));
                m_props[index] = value;
            }
        }

        /// <summary>Copies the elements of the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object to an array, starting at a particular array index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> object that is the destination of the elements copied from the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins. </param>
        public void CopyTo(Array array, int index)
        {
            m_props.CopyTo(array, index);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object to an array of <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> objects, starting at a particular array index.</summary>
        /// <param name="array">The one-dimensional array of  <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> objects that is the destination of the elements copied from the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(EncryptionProperty[] array, int index)
        {
            m_props.CopyTo((Array) array, index);
        }

        /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</returns>
        public object SyncRoot
        {
            get
            {
                return m_props.SyncRoot;
            }
        }

        /// <summary>Gets a value that indicates whether access to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object is synchronized (thread safe).</summary>
        /// <returns>
        /// <see langword="true" /> if access to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object is synchronized (thread safe); otherwise, <see langword="false" />.</returns>
        public bool IsSynchronized
        {
            get
            {
                return m_props.IsSynchronized;
            }
        }
    }
}