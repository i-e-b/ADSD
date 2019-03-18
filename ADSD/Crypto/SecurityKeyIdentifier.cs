using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ADSD.Crypto
{
    /// <summary>Represents a key identifier.</summary>
    public class SecurityKeyIdentifier : IEnumerable<SecurityKeyIdentifierClause>, IEnumerable
    {
        private const int InitialSize = 2;
        private readonly List<SecurityKeyIdentifierClause> clauses;
        private bool isReadOnly;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> class.  </summary>
        public SecurityKeyIdentifier()
        {
            this.clauses = new List<SecurityKeyIdentifierClause>(2);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> class using the specified key identifier clauses. </summary>
        /// <param name="clauses">An array of <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that contains the key identifier clauses.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="clauses" /> is <see langword="null" />.</exception>
        public SecurityKeyIdentifier(params SecurityKeyIdentifierClause[] clauses)
        {
            if (clauses == null) throw new ArgumentNullException(nameof (clauses));
            this.clauses = new List<SecurityKeyIdentifierClause>(clauses.Length);
            for (int index = 0; index < clauses.Length; ++index)
                this.Add(clauses[index]);
        }

        /// <summary>Gets the key identifier clause at the specified index.</summary>
        /// <param name="index">The zero-based index of the key identifier clause in the collection of key identifier clauses.</param>
        /// <returns>The <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> at the specified index.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="index" /> is equal to or greater than <see cref="P:System.IdentityModel.Tokens.SecurityKeyIdentifier.Count" />.</exception>
        public SecurityKeyIdentifierClause this[int index]
        {
            get
            {
                return this.clauses[index];
            }
        }

        /// <summary>Gets a value that indicates whether a key can be created for at least one of the key identifier clauses. </summary>
        /// <returns>
        /// <see langword="true" /> if a key can be created for at least one of the key identifier clauses; otherwise, <see langword="false" />. </returns>
        public bool CanCreateKey
        {
            get
            {
                for (int index = 0; index < this.Count; ++index)
                {
                    if (this[index].CanCreateKey)
                        return true;
                }
                return false;
            }
        }

        /// <summary>Gets the number of key identifier clauses.</summary>
        /// <returns>The number of key identifier clauses.</returns>
        public int Count
        {
            get
            {
                return this.clauses.Count;
            }
        }

        /// <summary>Gets a value that indicates whether the properties of this instance are read-only.</summary>
        /// <returns>
        /// <see langword="true" /> if the properties of this instance are read-only; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
        public bool IsReadOnly
        {
            get
            {
                return this.isReadOnly;
            }
        }

        /// <summary>Adds a key identifier clause to the end of the list.</summary>
        /// <param name="clause">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to be added to the end of the list.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="clause" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The value of the <see cref="P:System.IdentityModel.Tokens.SecurityKeyIdentifier.IsReadOnly" /> property is <see langword="true" />.</exception>
        public void Add(SecurityKeyIdentifierClause clause)
        {
            if (this.isReadOnly) throw new InvalidOperationException("ObjectIsReadOnly");
            if (clause == null) throw new ArgumentNullException(nameof (clause));
            this.clauses.Add(clause);
        }

        /// <summary>Creates a key for one of the key identifier clauses.</summary>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that represents the created key.</returns>
        /// <exception cref="T:System.InvalidOperationException">A key could not be created for any of the key identifier clauses.</exception>
        public SecurityKey CreateKey()
        {
            for (int index = 0; index < this.Count; ++index)
            {
                if (this[index].CanCreateKey)
                    return this[index].CreateKey();
            }
            throw new InvalidOperationException("KeyIdentifierCannotCreateKey");
        }

        /// <summary>Searches for a key identifier clause of the specified type and returns the first occurrence within the entire collection. </summary>
        /// <typeparam name="TClause">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that represents the type of key identifier clause to search the collection for.</typeparam>
        public TClause Find<TClause>() where TClause : SecurityKeyIdentifierClause
        {
            TClause clause;
            if (!this.TryFind<TClause>(out clause)) throw new ArgumentException("NoKeyIdentifierClauseFound", nameof(TClause));
            return clause;
        }

        /// <summary>Returns an enumerator that iterates through the collection of key identifier clauses.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.List`1.Enumerator" /> of type <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> for the collection.</returns>
        public IEnumerator<SecurityKeyIdentifierClause> GetEnumerator()
        {
            return (IEnumerator<SecurityKeyIdentifierClause>) this.clauses.GetEnumerator();
        }

        /// <summary>Causes this instance to be read-only.</summary>
        public void MakeReadOnly()
        {
            this.isReadOnly = true;
        }

        /// <summary>Returns the current object.</summary>
        /// <returns>The current object.</returns>
        public override string ToString()
        {
            using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
            {
                stringWriter.WriteLine(nameof (SecurityKeyIdentifier));
                stringWriter.WriteLine("    (");
                stringWriter.WriteLine("    IsReadOnly = {0},", (object) this.IsReadOnly);
                stringWriter.WriteLine("    Count = {0}{1}", (object) this.Count, this.Count > 0 ? (object) "," : (object) "");
                for (int index = 0; index < this.Count; ++index)
                    stringWriter.WriteLine("    Clause[{0}] = {1}{2}", (object) index, (object) this[index], index < this.Count - 1 ? (object) "," : (object) "");
                stringWriter.WriteLine("    )");
                return stringWriter.ToString();
            }
        }

        /// <summary>Searches for a key identifier clause of the specified type and returns a value that indicates whether a clause of that type could be found. When a type is found it is returned in the <see langword="out" /> parameter. </summary>
        public bool TryFind<TClause>(out TClause clause) where TClause : SecurityKeyIdentifierClause
        {
            for (int index = 0; index < this.clauses.Count; ++index)
            {
                TClause clause1 = this.clauses[index] as TClause;
                if ((object) clause1 != null)
                {
                    clause = clause1;
                    return true;
                }
            }
            clause = default (TClause);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) this.GetEnumerator();
        }
    }
}