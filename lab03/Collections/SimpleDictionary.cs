using System;
using System.Collections;
using System.Collections.Generic;

public class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
{
    private struct Entry
    {
        public int hashCode;
        public int next;
        public TKey key;
        public TValue value;
    }

    private int[] _buckets;
    private Entry[] _entries;
    private int _count;
    private int _freeList;
    private int _freeCount;
    private int _version;
    private const int DefaultCapacity = 4;

    // Конструкторы
    public SimpleDictionary() : this(DefaultCapacity, null) { }

    public SimpleDictionary(int capacity) : this(capacity, null) { }

    public SimpleDictionary(IEqualityComparer<TKey> comparer) : this(DefaultCapacity, comparer) { }

    public SimpleDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
        if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        Comparer = comparer ?? EqualityComparer<TKey>.Default;
        Initialize(capacity);
    }

    // Публичные свойства
    public IEqualityComparer<TKey> Comparer { get; private set; }

    public int Count => _count - _freeCount;

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public ICollection<TKey> Keys => new KeyCollection(this);

    public ICollection<TValue> Values => new ValueCollection(this);

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    ICollection IDictionary.Keys => Keys;

    ICollection IDictionary.Values => Values;

    // Индексатор
    public TValue this[TKey key]
    {
        get
        {
            int i = FindEntry(key);
            if (i >= 0) return _entries[i].value;
            throw new KeyNotFoundException();
        }
        set
        {
            Insert(key, value, false);
        }
    }

    public object this[object key]
    {
        get
        {
            if (IsCompatibleKey(key))
            {
                int i = FindEntry((TKey)key);
                if (i >= 0) return _entries[i].value;
            }
            return null;
        }
        set
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!(key is TKey)) throw new ArgumentException($"Key must be of type {typeof(TKey)}");
            if (value == null && !(default(TValue) == null)) throw new ArgumentNullException(nameof(value));

            try
            {
                TKey tempKey = (TKey)key;
                Insert(tempKey, (TValue)value, false);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException($"Value must be of type {typeof(TValue)}");
            }
        }
    }

    // Основные методы
    public void Add(TKey key, TValue value)
    {
        Insert(key, value, true);
    }

    public bool ContainsKey(TKey key)
    {
        return FindEntry(key) >= 0;
    }

    public bool Remove(TKey key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        if (_buckets != null)
        {
            int hashCode = Comparer.GetHashCode(key) & 0x7FFFFFFF;
            int bucket = hashCode % _buckets.Length;
            int last = -1;

            for (int i = _buckets[bucket]; i >= 0; last = i, i = _entries[i].next)
            {
                if (_entries[i].hashCode == hashCode && Comparer.Equals(_entries[i].key, key))
                {
                    if (last < 0)
                        _buckets[bucket] = _entries[i].next;
                    else
                        _entries[last].next = _entries[i].next;

                    _entries[i].hashCode = -1;
                    _entries[i].next = _freeList;
                    _entries[i].key = default;
                    _entries[i].value = default;
                    _freeList = i;
                    _freeCount++;
                    _version++;
                    return true;
                }
            }
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int i = FindEntry(key);
        if (i >= 0)
        {
            value = _entries[i].value;
            return true;
        }
        value = default;
        return false;
    }

    public void Clear()
    {
        if (_count > 0)
        {
            Array.Clear(_buckets, 0, _buckets.Length);
            Array.Clear(_entries, 0, _count);
            _freeList = -1;
            _count = 0;
            _freeCount = 0;
            _version++;
        }
    }

    // IEnumerable
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new Enumerator(this, Enumerator.KeyValuePair);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // ICollection<KeyValuePair<TKey, TValue>>
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        int i = FindEntry(item.Key);
        if (i >= 0 && EqualityComparer<TValue>.Default.Equals(_entries[i].value, item.Value))
            return true;
        return false;
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Destination array is too small");

        int count = _count;
        Entry[] entries = _entries;

        for (int i = 0; i < count; i++)
        {
            if (entries[i].hashCode >= 0)
            {
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
            }
        }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        int i = FindEntry(item.Key);
        if (i >= 0 && EqualityComparer<TValue>.Default.Equals(_entries[i].value, item.Value))
        {
            Remove(item.Key);
            return true;
        }
        return false;
    }

    // IDictionary
    void IDictionary.Add(object key, object value)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (!(key is TKey)) throw new ArgumentException($"Key must be of type {typeof(TKey)}");

        try
        {
            TValue tempValue = (TValue)value;
            Add((TKey)key, tempValue);
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException($"Value must be of type {typeof(TValue)}");
        }
    }

    bool IDictionary.Contains(object key)
    {
        if (IsCompatibleKey(key))
            return ContainsKey((TKey)key);
        return false;
    }

    void IDictionary.Remove(object key)
    {
        if (IsCompatibleKey(key))
            Remove((TKey)key);
    }

    // ICollection
    void ICollection.CopyTo(Array array, int index)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (array.Rank != 1) throw new ArgumentException("Multi-dimensional arrays are not supported");
        if (array.GetLowerBound(0) != 0) throw new ArgumentException("Non-zero lower bound is not supported");
        if (index < 0 || index > array.Length) throw new ArgumentOutOfRangeException(nameof(index));
        if (array.Length - index < Count) throw new ArgumentException("Destination array is too small");

        if (array is KeyValuePair<TKey, TValue>[] pairs)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)this).CopyTo(pairs, index);
        }
        else if (array is DictionaryEntry[] dictEntries)
        {
            Entry[] entries = _entries;
            for (int i = 0; i < _count; i++)
            {
                if (entries[i].hashCode >= 0)
                {
                    dictEntries[index++] = new DictionaryEntry(entries[i].key, entries[i].value);
                }
            }
        }
        else
        {
            object[] objects = array as object[];
            if (objects == null) throw new ArgumentException("Invalid array type");

            try
            {
                int count = _count;
                Entry[] entries = _entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0)
                    {
                        objects[index++] = new KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
                    }
                }
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException("Invalid array type");
            }
        }
    }

    // Вспомогательные методы
    private void Initialize(int capacity)
    {
        int size = GetPrime(capacity);
        _buckets = new int[size];
        for (int i = 0; i < _buckets.Length; i++) _buckets[i] = -1;
        _entries = new Entry[size];
        _freeList = -1;
    }

    private void Insert(TKey key, TValue value, bool add)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        if (_buckets == null) Initialize(0);

        int hashCode = Comparer.GetHashCode(key) & 0x7FFFFFFF;
        int targetBucket = hashCode % _buckets.Length;

        // Проверка существующего ключа
        for (int i = _buckets[targetBucket]; i >= 0; i = _entries[i].next)
        {
            if (_entries[i].hashCode == hashCode && Comparer.Equals(_entries[i].key, key))
            {
                if (add) throw new ArgumentException("An element with the same key already exists");

                _entries[i].value = value;
                _version++;
                return;
            }
        }

        // Добавление нового элемента
        int index;
        if (_freeCount > 0)
        {
            index = _freeList;
            _freeList = _entries[index].next;
            _freeCount--;
        }
        else
        {
            if (_count == _entries.Length)
            {
                Resize();
                targetBucket = hashCode % _buckets.Length;
            }
            index = _count;
            _count++;
        }

        _entries[index].hashCode = hashCode;
        _entries[index].next = _buckets[targetBucket];
        _entries[index].key = key;
        _entries[index].value = value;
        _buckets[targetBucket] = index;
        _version++;
    }

    private void Resize()
    {
        int newSize = GetPrime(_count * 2);
        int[] newBuckets = new int[newSize];
        for (int i = 0; i < newBuckets.Length; i++) newBuckets[i] = -1;

        Entry[] newEntries = new Entry[newSize];
        Array.Copy(_entries, 0, newEntries, 0, _count);

        for (int i = 0; i < _count; i++)
        {
            if (newEntries[i].hashCode >= 0)
            {
                int bucket = newEntries[i].hashCode % newSize;
                newEntries[i].next = newBuckets[bucket];
                newBuckets[bucket] = i;
            }
        }

        _buckets = newBuckets;
        _entries = newEntries;
    }

    private int FindEntry(TKey key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        if (_buckets != null)
        {
            int hashCode = Comparer.GetHashCode(key) & 0x7FFFFFFF;
            for (int i = _buckets[hashCode % _buckets.Length]; i >= 0; i = _entries[i].next)
            {
                if (_entries[i].hashCode == hashCode && Comparer.Equals(_entries[i].key, key))
                    return i;
            }
        }
        return -1;
    }

    private static bool IsCompatibleKey(object key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        return key is TKey;
    }

    private static int GetPrime(int min)
    {
        int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
        };

        foreach (int prime in primes)
        {
            if (prime >= min) return prime;
        }

        for (int i = (min | 1); i < int.MaxValue; i += 2)
        {
            if (IsPrime(i)) return i;
        }

        return min;
    }

    private static bool IsPrime(int candidate)
    {
        if ((candidate & 1) != 0)
        {
            int limit = (int)Math.Sqrt(candidate);
            for (int divisor = 3; divisor <= limit; divisor += 2)
            {
                if ((candidate % divisor) == 0)
                    return false;
            }
            return true;
        }
        return candidate == 2;
    }

    // Вложенные классы коллекций
    private class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey>
    {
        private SimpleDictionary<TKey, TValue> _dictionary;

        public KeyCollection(SimpleDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => true;

        public bool IsSynchronized => false;

        public object SyncRoot => ((ICollection)_dictionary).SyncRoot;

        public void Add(TKey item) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public bool Contains(TKey item) => _dictionary.ContainsKey(item);

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Destination array is too small");

            int count = _dictionary._count;
            Entry[] entries = _dictionary._entries;

            for (int i = 0; i < count; i++)
            {
                if (entries[i].hashCode >= 0)
                {
                    array[arrayIndex++] = entries[i].key;
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1) throw new ArgumentException("Multi-dimensional arrays are not supported");
            if (array.GetLowerBound(0) != 0) throw new ArgumentException("Non-zero lower bound is not supported");

            if (array is TKey[] keys)
            {
                CopyTo(keys, index);
            }
            else
            {
                object[] objects = array as object[];
                if (objects == null) throw new ArgumentException("Invalid array type");

                int count = _dictionary._count;
                Entry[] entries = _dictionary._entries;

                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (entries[i].hashCode >= 0)
                        {
                            objects[index++] = entries[i].key;
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("Invalid array type");
                }
            }
        }

        public IEnumerator<TKey> GetEnumerator() => new Enumerator(_dictionary, Enumerator.Key);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Remove(TKey item) => throw new NotSupportedException();

        private class Enumerator : IEnumerator<TKey>, IEnumerator
        {
            private SimpleDictionary<TKey, TValue> _dictionary;
            private int _index;
            private int _version;
            private TKey _currentKey;

            internal Enumerator(SimpleDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                _dictionary = dictionary;
                _index = 0;
                _version = dictionary._version;
                _currentKey = default;
            }

            public TKey Current => _currentKey;

            object IEnumerator.Current => _currentKey;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_version != _dictionary._version)
                    throw new InvalidOperationException("Collection was modified");

                while (_index < _dictionary._count)
                {
                    if (_dictionary._entries[_index].hashCode >= 0)
                    {
                        _currentKey = _dictionary._entries[_index].key;
                        _index++;
                        return true;
                    }
                    _index++;
                }

                _index = _dictionary._count + 1;
                _currentKey = default;
                return false;
            }

            public void Reset()
            {
                if (_version != _dictionary._version)
                    throw new InvalidOperationException("Collection was modified");

                _index = 0;
                _currentKey = default;
            }
        }
    }

    private class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>
    {
        private SimpleDictionary<TKey, TValue> _dictionary;

        public ValueCollection(SimpleDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => true;

        public bool IsSynchronized => false;

        public object SyncRoot => ((ICollection)_dictionary).SyncRoot;

        public void Add(TValue item) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public bool Contains(TValue item)
        {
            return _dictionary.ContainsValue(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Destination array is too small");

            int count = _dictionary._count;
            Entry[] entries = _dictionary._entries;

            for (int i = 0; i < count; i++)
            {
                if (entries[i].hashCode >= 0)
                {
                    array[arrayIndex++] = entries[i].value;
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1) throw new ArgumentException("Multi-dimensional arrays are not supported");
            if (array.GetLowerBound(0) != 0) throw new ArgumentException("Non-zero lower bound is not supported");

            if (array is TValue[] values)
            {
                CopyTo(values, index);
            }
            else
            {
                object[] objects = array as object[];
                if (objects == null) throw new ArgumentException("Invalid array type");

                int count = _dictionary._count;
                Entry[] entries = _dictionary._entries;

                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (entries[i].hashCode >= 0)
                        {
                            objects[index++] = entries[i].value;
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("Invalid array type");
                }
            }
        }

        public IEnumerator<TValue> GetEnumerator() => new Enumerator(_dictionary, Enumerator.Value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Remove(TValue item) => throw new NotSupportedException();

        private class Enumerator : IEnumerator<TValue>, IEnumerator
        {
            private SimpleDictionary<TKey, TValue> _dictionary;
            private int _index;
            private int _version;
            private TValue _currentValue;

            internal Enumerator(SimpleDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                _dictionary = dictionary;
                _index = 0;
                _version = dictionary._version;
                _currentValue = default;
            }

            public TValue Current => _currentValue;

            object IEnumerator.Current => _currentValue;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_version != _dictionary._version)
                    throw new InvalidOperationException("Collection was modified");

                while (_index < _dictionary._count)
                {
                    if (_dictionary._entries[_index].hashCode >= 0)
                    {
                        _currentValue = _dictionary._entries[_index].value;
                        _index++;
                        return true;
                    }
                    _index++;
                }

                _index = _dictionary._count + 1;
                _currentValue = default;
                return false;
            }

            public void Reset()
            {
                if (_version != _dictionary._version)
                    throw new InvalidOperationException("Collection was modified");

                _index = 0;
                _currentValue = default;
            }
        }
    }

    // Вложенный класс Enumerator для словаря
    private class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator, IEnumerator
    {
        internal const int KeyValuePair = 1;
        internal const int Key = 2;
        internal const int Value = 3;

        private SimpleDictionary<TKey, TValue> _dictionary;
        private int _index;
        private int _version;
        private KeyValuePair<TKey, TValue> _current;
        private int _getEnumeratorRetType;

        internal Enumerator(SimpleDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
        {
            _dictionary = dictionary;
            _index = 0;
            _version = dictionary._version;
            _getEnumeratorRetType = getEnumeratorRetType;
            _current = default;
        }

        public KeyValuePair<TKey, TValue> Current => _current;

        object IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _dictionary._count + 1)
                    throw new InvalidOperationException();

                if (_getEnumeratorRetType == KeyValuePair)
                    return new KeyValuePair<TKey, TValue>(_current.Key, _current.Value);

                if (_getEnumeratorRetType == Key)
                    return _current.Key;

                return _current.Value;
            }
        }

        DictionaryEntry IDictionaryEnumerator.Entry
        {
            get
            {
                if (_index == 0 || _index == _dictionary._count + 1)
                    throw new InvalidOperationException();

                return new DictionaryEntry(_current.Key, _current.Value);
            }
        }

        object IDictionaryEnumerator.Key => _current.Key;

        object IDictionaryEnumerator.Value => _current.Value;

        public void Dispose() { }

        public bool MoveNext()
        {
            if (_version != _dictionary._version)
                throw new InvalidOperationException("Collection was modified");

            while (_index < _dictionary._count)
            {
                if (_dictionary._entries[_index].hashCode >= 0)
                {
                    _current = new KeyValuePair<TKey, TValue>(
                        _dictionary._entries[_index].key,
                        _dictionary._entries[_index].value);
                    _index++;
                    return true;
                }
                _index++;
            }

            _index = _dictionary._count + 1;
            _current = default;
            return false;
        }

        public void Reset()
        {
            if (_version != _dictionary._version)
                throw new InvalidOperationException("Collection was modified");

            _index = 0;
            _current = default;
        }
    }

    // Дополнительные методы
    public bool ContainsValue(TValue value)
    {
        if (value == null)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_entries[i].hashCode >= 0 && _entries[i].value == null)
                    return true;
            }
        }
        else
        {
            EqualityComparer<TValue> c = EqualityComparer<TValue>.Default;
            for (int i = 0; i < _count; i++)
            {
                if (_entries[i].hashCode >= 0 && c.Equals(_entries[i].value, value))
                    return true;
            }
        }
        return false;
    }
}