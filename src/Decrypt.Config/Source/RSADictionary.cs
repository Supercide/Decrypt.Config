using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Decrypt.Config.Source {
    public class RSADictionary<TKey> : IDictionary<TKey, string>
    {
        readonly IDictionary<TKey, string> _data;

        private readonly string _containerName;

        public RSADictionary(IDictionary<TKey, string> data, string containerName)
        {
            _data = data;
            _containerName = containerName;
        }

        public bool IsReadOnly => false;

        public int Count => _data.Count;

        public bool ContainsKey(TKey key)
        {
            return _data.ContainsKey(key);
        }

        public void Add(TKey key, string value)
        {
            var data = Encrypt(value);

            _data.Add(key, data);
        }

        public bool Remove(TKey key)
        {
            return _data.Remove(key);
        }

        public bool TryGetValue(TKey key, out string value)
        {
            string encryptedValue;

            if (_data.TryGetValue(key, out encryptedValue))
            {
                value = Decrypt(encryptedValue);

                return true;
            }
            value = null;

            return false;
        }

        string IDictionary<TKey, string>.this[TKey key]
        {
            get => Decrypt(_data[key]);

            set => _data[key] = Encrypt(value);
        }

        public ICollection<TKey> Keys => _data.Keys;

        public ICollection<string> Values => _data.Select(kvp => Decrypt(kvp.Value)).ToList();


        public IEnumerator<KeyValuePair<TKey, string>> GetEnumerator()
        {
            foreach (var kvp in _data)
            {
                yield return new KeyValuePair<TKey, string>(kvp.Key, Decrypt(kvp.Value));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, string> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _data.Clear();
        }

        public bool Contains(KeyValuePair<TKey, string> item)
        {
            return _data.Contains(new KeyValuePair<TKey, string>(item.Key, Encrypt(item.Value)));
        }

        public void CopyTo(KeyValuePair<TKey, string>[] array, int index)
        {
            int count = this._data.Count;

            KeyValuePair<TKey, string>[] entries = this._data.ToArray();

            for (int i = 0; i < count; ++i)
            {
                array[index++] = new KeyValuePair<TKey, string>(entries[i].Key, Encrypt(entries[i].Value));
            }
        }

        public bool Remove(KeyValuePair<TKey, string> item)
        {
            return this._data.Remove(item.Key);
        }

        private string Encrypt(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            var encryptedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);

            string data = Convert.ToBase64String(encryptedBytes);

            return data;
        }

        private string Decrypt(string encryptedValue)
        {
            var dataBytes = Convert.FromBase64String(encryptedValue);

            byte[] data = ProtectedData.Unprotect(dataBytes, null, DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(data);
        }
    }
}