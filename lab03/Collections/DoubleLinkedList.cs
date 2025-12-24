using System;
using System.Collections;
using System.Collections.Generic;

public class DoublyLinkedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
{
    private class Node
    {
        public T Value { get; set; }
        public Node Next { get; set; }
        public Node Previous { get; set; }

        public Node(T value)
        {
            Value = value;
        }
    }

    private Node _head;
    private Node _tail;
    private int _count;
    private int _version;

    // Конструкторы
    public DoublyLinkedList()
    {
    }

    public DoublyLinkedList(IEnumerable<T> collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));

        foreach (var item in collection)
            AddLast(item);
    }

    // Свойства
    public int Count => _count;

    public bool IsReadOnly => false;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public bool IsFixedSize => false;

    // Методы доступа
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
            return GetNodeAt(index).Value;
        }
        set
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
            GetNodeAt(index).Value = value;
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

    // Основные операции
    public void Add(T item)
    {
        AddLast(item);
    }

    public void AddLast(T item)
    {
        Node newNode = new Node(item);

        if (_head == null)
        {
            _head = newNode;
            _tail = newNode;
        }
        else
        {
            _tail.Next = newNode;
            newNode.Previous = _tail;
            _tail = newNode;
        }

        _count++;
        _version++;
    }

    public void AddFirst(T item)
    {
        Node newNode = new Node(item);

        if (_head == null)
        {
            _head = newNode;
            _tail = newNode;
        }
        else
        {
            newNode.Next = _head;
            _head.Previous = newNode;
            _head = newNode;
        }

        _count++;
        _version++;
    }

    public void Clear()
    {
        Node current = _head;
        while (current != null)
        {
            Node next = current.Next;
            current.Next = null;
            current.Previous = null;
            current = next;
        }

        _head = null;
        _tail = null;
        _count = 0;
        _version++;
    }

    public bool Contains(T item)
    {
        return Find(item) != null;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < _count) throw new ArgumentException("Destination array is too small");

        Node current = _head;
        while (current != null)
        {
            array[arrayIndex++] = current.Value;
            current = current.Next;
        }
    }

    public void CopyTo(Array array, int index)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (array.Rank != 1) throw new ArgumentException("Multi-dimensional arrays are not supported");

        T[] typedArray = array as T[];
        if (typedArray != null)
        {
            CopyTo(typedArray, index);
        }
        else
        {
            object[] objectArray = array as object[];
            if (objectArray == null) throw new ArgumentException("Invalid array type");

            try
            {
                Node current = _head;
                while (current != null)
                {
                    objectArray[index++] = current.Value;
                    current = current.Next;
                }
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException("Invalid array type");
            }
        }
    }

    public bool Remove(T item)
    {
        Node node = Find(item);
        if (node != null)
        {
            RemoveNode(node);
            return true;
        }
        return false;
    }

    public void RemoveFirst()
    {
        if (_head == null) throw new InvalidOperationException("List is empty");

        RemoveNode(_head);
    }

    public void RemoveLast()
    {
        if (_tail == null) throw new InvalidOperationException("List is empty");

        RemoveNode(_tail);
    }

    // IList<T> методы
    public int IndexOf(T item)
    {
        Node current = _head;
        int index = 0;

        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, item))
                return index;

            current = current.Next;
            index++;
        }

        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > _count) throw new ArgumentOutOfRangeException(nameof(index));

        if (index == 0)
        {
            AddFirst(item);
        }
        else if (index == _count)
        {
            AddLast(item);
        }
        else
        {
            Node currentNode = GetNodeAt(index);
            Node newNode = new Node(item);

            newNode.Previous = currentNode.Previous;
            newNode.Next = currentNode;
            currentNode.Previous.Next = newNode;
            currentNode.Previous = newNode;

            _count++;
            _version++;
        }
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));

        Node node = GetNodeAt(index);
        RemoveNode(node);
    }

    // IEnumerable<T>
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // IList (явная реализация)
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
        return _count - 1;
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
    private Node GetNodeAt(int index)
    {
        if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));

        Node current;
        if (index < _count / 2)
        {
            current = _head;
            for (int i = 0; i < index; i++)
                current = current.Next;
        }
        else
        {
            current = _tail;
            for (int i = _count - 1; i > index; i--)
                current = current.Previous;
        }

        return current;
    }

    private Node Find(T value)
    {
        Node current = _head;
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;

        while (current != null)
        {
            if (comparer.Equals(current.Value, value))
                return current;

            current = current.Next;
        }

        return null;
    }

    private void RemoveNode(Node node)
    {
        if (node.Previous != null)
            node.Previous.Next = node.Next;
        else
            _head = node.Next;

        if (node.Next != null)
            node.Next.Previous = node.Previous;
        else
            _tail = node.Previous;

        node.Next = null;
        node.Previous = null;

        _count--;
        _version++;
    }

    // Вложенный класс Enumerator
    private class Enumerator : IEnumerator<T>, IEnumerator
    {
        private DoublyLinkedList<T> _list;
        private Node _current;
        private Node _next;
        private int _version;

        public Enumerator(DoublyLinkedList<T> list)
        {
            _list = list;
            _version = list._version;
            _current = null;
            _next = list._head;
        }

        public T Current => _current.Value;

        object IEnumerator.Current
        {
            get
            {
                if (_current == null)
                    throw new InvalidOperationException();
                return _current.Value;
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_version != _list._version)
                throw new InvalidOperationException("Collection was modified");

            if (_next != null)
            {
                _current = _next;
                _next = _next.Next;
                return true;
            }

            _current = null;
            return false;
        }

        public void Reset()
        {
            if (_version != _list._version)
                throw new InvalidOperationException("Collection was modified");

            _current = null;
            _next = _list._head;
        }
    }
}