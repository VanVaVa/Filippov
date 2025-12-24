using System.Collections;

namespace lab03.Collections;

public class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    private struct DictionaryEntry
    {
        public int Hash;
        public int Chain;
        public TKey Key;
        public TValue Value;
    }

    private int[] hashTable;
    private DictionaryEntry[] data;
    private int totalEntries;
    private int freeSlotIndex;
    private int freeSlotCount;
    private const int InitialCapacity = 4;

    public SimpleDictionary()
    {
        hashTable = new int[InitialCapacity];
        data = new DictionaryEntry[InitialCapacity];
        totalEntries = 0;
        freeSlotIndex = -1;
        freeSlotCount = 0;

        for (int idx = 0; idx < hashTable.Length; idx++)
        {
            hashTable[idx] = -1;
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue? result))
                return result;
            throw new Exception($"ключ '{key}' не найден");
        }
        set
        {
            AddOrUpdate(key, value, false);
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keyCollection = new List<TKey>();
            for (int idx = 0; idx < totalEntries; idx++)
            {
                if (data[idx].Hash >= 0)
                {
                    keyCollection.Add(data[idx].Key);
                }
            }
            return keyCollection;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var valueCollection = new List<TValue>();
            for (int idx = 0; idx < totalEntries; idx++)
            {
                if (data[idx].Hash >= 0)
                {
                    valueCollection.Add(data[idx].Value);
                }
            }
            return valueCollection;
        }
    }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    public int Count => totalEntries - freeSlotCount;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if (!AddOrUpdate(key, value, true))
            throw new Exception($"элемент с ключом '{key}' уже существует");
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        if (totalEntries > 0)
        {
            for (int idx = 0; idx < hashTable.Length; idx++)
            {
                hashTable[idx] = -1;
            }
            Array.Clear(data, 0, totalEntries);
            freeSlotIndex = -1;
            totalEntries = 0;
            freeSlotCount = 0;
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue? result))
            return EqualityComparer<TValue>.Default.Equals(result, item.Value);
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new Exception("массив не может быть null");
        if (arrayIndex < 0)
            throw new Exception("индекс не может быть отрицательным");
        if (array.Length - arrayIndex < Count)
            throw new Exception("недостаточно места в массиве");

        int position = arrayIndex;
        foreach (var pair in this)
        {
            array[position++] = pair;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int idx = 0; idx < totalEntries; idx++)
        {
            if (data[idx].Hash >= 0)
            {
                yield return new KeyValuePair<TKey, TValue>(data[idx].Key, data[idx].Value);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new Exception("ключ не может быть null");

        int hash = key.GetHashCode() & 0x7FFFFFFF;
        int bucketIndex = hash % hashTable.Length;
        int previousIndex = -1;

        for (int currentIndex = hashTable[bucketIndex]; currentIndex >= 0; previousIndex = currentIndex, currentIndex = data[currentIndex].Chain)
        {
            if (data[currentIndex].Hash == hash && EqualityComparer<TKey>.Default.Equals(data[currentIndex].Key, key))
            {
                if (previousIndex < 0)
                {
                    hashTable[bucketIndex] = data[currentIndex].Chain;
                }
                else
                {
                    data[previousIndex].Chain = data[currentIndex].Chain;
                }

                data[currentIndex].Hash = -1;
                data[currentIndex].Chain = freeSlotIndex;
                data[currentIndex].Key = default(TKey)!;
                data[currentIndex].Value = default(TValue)!;
                freeSlotIndex = currentIndex;
                freeSlotCount++;
                return true;
            }
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (Contains(item))
        {
            return Remove(item.Key);
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new Exception("ключ не может быть null");

        int hash = key.GetHashCode() & 0x7FFFFFFF;
        int bucketIndex = hash % hashTable.Length;

        for (int currentIndex = hashTable[bucketIndex]; currentIndex >= 0; currentIndex = data[currentIndex].Chain)
        {
            if (data[currentIndex].Hash == hash && EqualityComparer<TKey>.Default.Equals(data[currentIndex].Key, key))
            {
                value = data[currentIndex].Value;
                return true;
            }
        }

        value = default(TValue)!;
        return false;
    }

    private bool AddOrUpdate(TKey key, TValue value, bool addOnly)
    {
        if (key == null)
            throw new Exception("ключ не может быть null");

        int hash = key.GetHashCode() & 0x7FFFFFFF;
        int targetBucket = hash % hashTable.Length;

        for (int currentIndex = hashTable[targetBucket]; currentIndex >= 0; currentIndex = data[currentIndex].Chain)
        {
            if (data[currentIndex].Hash == hash && EqualityComparer<TKey>.Default.Equals(data[currentIndex].Key, key))
            {
                if (addOnly)
                    return false;

                data[currentIndex].Value = value;
                return true;
            }
        }

        int entryIndex;
        if (freeSlotCount > 0)
        {
            entryIndex = freeSlotIndex;
            freeSlotIndex = data[entryIndex].Chain;
            freeSlotCount--;
        }
        else
        {
            if (totalEntries == data.Length)
            {
                ExpandTable();
                targetBucket = hash % hashTable.Length;
            }
            entryIndex = totalEntries;
            totalEntries++;
        }

        data[entryIndex].Hash = hash;
        data[entryIndex].Chain = hashTable[targetBucket];
        data[entryIndex].Key = key;
        data[entryIndex].Value = value;
        hashTable[targetBucket] = entryIndex;

        return true;
    }

    private void ExpandTable()
    {
        int newCapacity = hashTable.Length * 2;
        int[] newHashTable = new int[newCapacity];
        DictionaryEntry[] newData = new DictionaryEntry[newCapacity];

        for (int idx = 0; idx < newHashTable.Length; idx++)
        {
            newHashTable[idx] = -1;
        }

        Array.Copy(data, 0, newData, 0, totalEntries);

        for (int idx = 0; idx < totalEntries; idx++)
        {
            if (newData[idx].Hash >= 0)
            {
                int bucket = newData[idx].Hash % newCapacity;
                newData[idx].Chain = newHashTable[bucket];
                newHashTable[bucket] = idx;
            }
        }

        hashTable = newHashTable;
        data = newData;
    }
}

