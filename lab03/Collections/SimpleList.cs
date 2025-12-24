using System;
using System.Collections;
using System.Collections.Generic;

public class SimpleList<T> : IList<T>, IList, IEnumerable<T>, IEnumerable, ICollection<T>, ICollection
{
    private T[] _items;
    private int _size;
    private int _version;
    private static readonly T[] _emptyArray = new T[0];

    // Конструкторы
    public SimpleList()
    {
        _items = _emptyArray;
    }

    public SimpleList(int capacity)
    {
        if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _items = capacity == 0 ? _emptyArray : new T[capacity];
    }

    public SimpleList(IEnumerable<T> collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));

        if (collection is ICollection<T> c)
        {
            int count = c.Count;
            if (count == 0)
            {
                _items = _emptyArray;
            }
            else
            {
                _items = new T[count];
                c.CopyTo(_items, 0);
                _size = count;
            }
        }
        else
        {
            _size = 0;
            _items = new T[4];
            foreach (var item in collection)
                Add(item);
        }
    }

    // ICollection<T> и ICollection
    public int Count => _size;

    public bool IsReadOnly => false;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public void Add(T item)
    {
        if (_size == _items.Length) EnsureCapacity(_size + 1);
        _items[_size++] = item;
        _version++;
    }

    public void Clear()
    {
        if (_size > 0)
        {
            Array.Clear(_items, 0, _size);
            _size = 0;
        }
        _version++;
    }

    public bool Contains(T item)
    {
        return IndexOf(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(_items, 0, array, arrayIndex, _size);
    }

    public void CopyTo(Array array, int index)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (array.Rank != 1) throw new ArgumentException("Multi-dimensional arrays are not supported");
        Array.Copy(_items, 0, array, index, _size);
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    // IList<T> и IList
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));
            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));
            _items[index] = value;
            _version++;
        }
    }

    object IList.this[int index]
    {
        get => this[index];
        set
        {
            try
            {
                this[index] = (T)value;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException($"Value must be of type {typeof(T)}");
            }
        }
    }

    public int IndexOf(T item)
    {
        return Array.IndexOf(_items, item, 0, _size);
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > _size) throw new ArgumentOutOfRangeException(nameof(index));

        if (_size == _items.Length) EnsureCapacity(_size + 1);

        if (index < _size)
            Array.Copy(_items, index, _items, index + 1, _size - index);

        _items[index] = item;
        _size++;
        _version++;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));

        _size--;
        if (index < _size)
            Array.Copy(_items, index + 1, _items, index, _size - index);

        _items[_size] = default;
        _version++;
    }

    // IEnumerable<T> и IEnumerable
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // IList (явная реализация)
    bool IList.IsFixedSize => false;

    int IList.Add(object value)
    {
        try
        {
            Add((T)value);
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException($"Value must be of type {typeof(T)}");
        }
        return _size - 1;
    }

    bool IList.Contains(object value)
    {
        if (value is T || value == null)
            return Contains((T)value);
        return false;
    }

    int IList.IndexOf(object value)
    {
        if (value is T || value == null)
            return IndexOf((T)value);
        return -1;
    }

    void IList.Insert(int index, object value)
    {
        try
        {
            Insert(index, (T)value);
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException($"Value must be of type {typeof(T)}");
        }
    }

    void IList.Remove(object value)
    {
        if (value is T || value == null)
            Remove((T)value);
    }

    // Вспомогательные методы
    private void EnsureCapacity(int min)
    {
        if (_items.Length < min)
        {
            int newCapacity = _items.Length == 0 ? 4 : _items.Length * 2;
            if (newCapacity < min) newCapacity = min;

            T[] newItems = new T[newCapacity];
            Array.Copy(_items, 0, newItems, 0, _size);
            _items = newItems;
        }
    }

    public void TrimExcess()
    {
        int threshold = (int)(_items.Length * 0.9);
        if (_size < threshold)
        {
            T[] newItems = new T[_size];
            Array.Copy(_items, 0, newItems, 0, _size);
            _items = newItems;
        }
    }

    // Вложенный класс Enumerator
    private class Enumerator : IEnumerator<T>, IEnumerator
    {
        private SimpleList<T> _list;
        private int _index;
        private int _version;
        private T _current;

        public Enumerator(SimpleList<T> list)
        {
            _list = list;
            _index = 0;
            _version = list._version;
            _current = default;
        }

        public T Current => _current;

        object IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _list._size + 1)
                    throw new InvalidOperationException();
                return _current;
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_version != _list._version)
                throw new InvalidOperationException("Collection was modified");

            if (_index < _list._size)
            {
                _current = _list._items[_index];
                _index++;
                return true;
            }

            _index = _list._size + 1;
            _current = default;
            return false;
        }

        public void Reset()
        {
            if (_version != _list._version)
                throw new InvalidOperationException("Collection was modified");

            _index = 0;
            _current = default;
        }
    }
}