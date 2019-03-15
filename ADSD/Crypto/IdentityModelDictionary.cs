using System;
using System.Collections.Generic;
using System.Xml;

namespace ADSD
{
    internal class IdentityModelDictionary : IXmlDictionary
    {
        public static readonly IdentityModelDictionary Version1 = new IdentityModelDictionary((IdentityModelStrings) new IdentityModelStringsVersion1());
        private IdentityModelStrings strings;
        private int count;
        private XmlDictionaryString[] dictionaryStrings;
        private Dictionary<string, int> dictionary;
        private XmlDictionaryString[] versionedDictionaryStrings;

        public IdentityModelDictionary(IdentityModelStrings strings)
        {
            this.strings = strings;
            this.count = strings.Count;
        }

        public static IdentityModelDictionary CurrentVersion
        {
            get
            {
                return IdentityModelDictionary.Version1;
            }
        }

        public XmlDictionaryString CreateString(string value, int key)
        {
            return new XmlDictionaryString((IXmlDictionary) this, value, key);
        }

        public bool TryLookup(string key, out XmlDictionaryString value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof (key));
            if (this.dictionary == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(this.count);
                for (int index = 0; index < this.count; ++index)
                    dictionary.Add(this.strings[index], index);
                this.dictionary = dictionary;
            }
            int key1;
            if (this.dictionary.TryGetValue(key, out key1))
                return this.TryLookup(key1, out value);
            value = (XmlDictionaryString) null;
            return false;
        }

        public bool TryLookup(int key, out XmlDictionaryString value)
        {
            if (key < 0 || key >= this.count)
            {
                value = (XmlDictionaryString) null;
                return false;
            }
            if (this.dictionaryStrings == null)
                this.dictionaryStrings = new XmlDictionaryString[this.count];
            XmlDictionaryString dictionaryString = this.dictionaryStrings[key];
            if (dictionaryString == null)
            {
                dictionaryString = this.CreateString(this.strings[key], key);
                this.dictionaryStrings[key] = dictionaryString;
            }
            value = dictionaryString;
            return true;
        }

        public bool TryLookup(XmlDictionaryString key, out XmlDictionaryString value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof (key));
            if (key.Dictionary == this)
            {
                value = key;
                return true;
            }
            if (key.Dictionary == IdentityModelDictionary.CurrentVersion)
            {
                if (this.versionedDictionaryStrings == null)
                    this.versionedDictionaryStrings = new XmlDictionaryString[IdentityModelDictionary.CurrentVersion.count];
                XmlDictionaryString dictionaryString = this.versionedDictionaryStrings[key.Key];
                if (dictionaryString == null)
                {
                    if (!this.TryLookup(key.Value, out dictionaryString))
                    {
                        value = (XmlDictionaryString) null;
                        return false;
                    }
                    this.versionedDictionaryStrings[key.Key] = dictionaryString;
                }
                value = dictionaryString;
                return true;
            }
            value = (XmlDictionaryString) null;
            return false;
        }
    }
}