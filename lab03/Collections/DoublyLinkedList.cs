using System.Collections;

namespace lab03.Collections;

public class DoublyLinkedList<T> : IList<T>
{
    private class ListNode
    {
        public T Data;
        public ListNode? Prev;
        public ListNode? Next;

        public ListNode(T data)
        {
            Data = data;
        }
    }

    private ListNode? first;
    private ListNode? last;
    private int length;

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= length)
                throw new Exception("индекс вне диапазона");

            ListNode? target = LocateNode(index);
            return target!.Data;
        }
        set
        {
            if (index < 0 || index >= length)
                throw new Exception("индекс вне диапазона");

            ListNode? target = LocateNode(index);
            target!.Data = value;
        }
    }

    public int Count => length;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        ListNode newElement = new ListNode(item);

        if (first == null)
        {
            first = newElement;
            last = newElement;
        }
        else
        {
            last!.Next = newElement;
            newElement.Prev = last;
            last = newElement;
        }

        length++;
    }

    public void Clear()
    {
        first = null;
        last = null;
        length = 0;
    }

    public bool Contains(T item)
    {
        return FindPosition(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
            throw new Exception("массив не может быть null");
        if (arrayIndex < 0)
            throw new Exception("индекс не может быть отрицательным");
        if (array.Length - arrayIndex < length)
            throw new Exception("недостаточно места в массиве");

        ListNode? current = first;
        int position = arrayIndex;
        while (current != null)
        {
            array[position++] = current.Data;
            current = current.Next;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        ListNode? current = first;
        while (current != null)
        {
            yield return current.Data;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return FindPosition(item);
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > length)
            throw new Exception("индекс вне диапазона");

        ListNode newElement = new ListNode(item);

        if (index == 0)
        {
            if (first == null)
            {
                first = newElement;
                last = newElement;
            }
            else
            {
                newElement.Next = first;
                first.Prev = newElement;
                first = newElement;
            }
        }
        else if (index == length)
        {
            last!.Next = newElement;
            newElement.Prev = last;
            last = newElement;
        }
        else
        {
            ListNode? target = LocateNode(index);
            newElement.Next = target;
            newElement.Prev = target!.Prev;
            target.Prev!.Next = newElement;
            target.Prev = newElement;
        }

        length++;
    }

    public bool Remove(T item)
    {
        ListNode? current = first;

        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Data, item))
            {
                DeleteNode(current);
                return true;
            }
            current = current.Next;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= length)
            throw new Exception("индекс вне диапазона");

        ListNode? target = LocateNode(index);
        DeleteNode(target!);
    }

    private int FindPosition(T item)
    {
        ListNode? current = first;
        int position = 0;

        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Data, item))
                return position;

            current = current.Next;
            position++;
        }

        return -1;
    }

    private ListNode? LocateNode(int index)
    {
        if (index < 0 || index >= length)
            return null;

        if (index < length / 2)
        {
            ListNode? current = first;
            for (int idx = 0; idx < index; idx++)
            {
                current = current!.Next;
            }
            return current;
        }
        else
        {
            ListNode? current = last;
            for (int idx = length - 1; idx > index; idx--)
            {
                current = current!.Prev;
            }
            return current;
        }
    }

    private void DeleteNode(ListNode node)
    {
        if (node.Prev != null)
        {
            node.Prev.Next = node.Next;
        }
        else
        {
            first = node.Next;
        }

        if (node.Next != null)
        {
            node.Next.Prev = node.Prev;
        }
        else
        {
            last = node.Prev;
        }

        length--;
    }
}

