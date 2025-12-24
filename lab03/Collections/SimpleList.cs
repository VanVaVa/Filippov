using System.Collections;

namespace lab03.Collections;

public class SimpleList<T> : IList<T>, ICollection<T>, IEnumerable<T>
{
    private T[] elements;
    private int size;
    private const int InitialCapacity = 4;

    public SimpleList()
    {
        elements = new T[InitialCapacity];
        size = 0;
    }

    public SimpleList(int capacity)
    {
        if (capacity < 0)
            throw new Exception("емкость не может быть отрицательной");
        elements = new T[capacity];
        size = 0;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= size)
                throw new Exception("индекс вне диапазона");
            return elements[index];
        }
        set
        {
            if (index < 0 || index >= size)
                throw new Exception("индекс вне диапазона");
            elements[index] = value;
        }
    }

    public int Count => size;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        if (size >= elements.Length)
        {
            ExpandCapacity();
        }
        elements[size] = item;
        size++;
    }

    public void Clear()
    {
        Array.Clear(elements, 0, size);
        size = 0;
    }

    public bool Contains(T item)
    {
        return FindIndex(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
            throw new Exception("массив не может быть null");
        if (arrayIndex < 0)
            throw new Exception("индекс не может быть отрицательным");
        if (array.Length - arrayIndex < size)
            throw new Exception("недостаточно места в массиве");

        Array.Copy(elements, 0, array, arrayIndex, size);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int idx = 0; idx < size; idx++)
        {
            yield return elements[idx];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return FindIndex(item);
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > size)
            throw new Exception("индекс вне диапазона");

        if (size >= elements.Length)
        {
            ExpandCapacity();
        }

        if (index < size)
        {
            Array.Copy(elements, index, elements, index + 1, size - index);
        }

        elements[index] = item;
        size++;
    }

    public bool Remove(T item)
    {
        int position = FindIndex(item);
        if (position >= 0)
        {
            RemoveAt(position);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= size)
            throw new Exception("индекс вне диапазона");

        size--;
        if (index < size)
        {
            Array.Copy(elements, index + 1, elements, index, size - index);
        }
        elements[size] = default(T)!;
    }

    private int FindIndex(T item)
    {
        for (int idx = 0; idx < size; idx++)
        {
            if (EqualityComparer<T>.Default.Equals(elements[idx], item))
                return idx;
        }
        return -1;
    }

    private void ExpandCapacity()
    {
        int newSize = elements.Length == 0 ? InitialCapacity : elements.Length * 2;
        T[] expanded = new T[newSize];
        Array.Copy(elements, expanded, size);
        elements = expanded;
    }
}

