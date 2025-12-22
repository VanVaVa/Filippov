using System.Collections.Generic;

namespace CollectionsBenchmark.Tests.InsertionTests;

public class LinkedListInsertionBenchmark : IInsertionBenchmark
{
    private LinkedList<int> _linkedList;
    private LinkedListNode<int> _middleNode;

    public string CollectionName => "LinkedList<T>";

    public void InitializeCollection(int initialSize)
    {
        _linkedList = new LinkedList<int>();

        for (int i = 0; i < initialSize; i++)
        {
            _linkedList.AddLast(i);
        }

        if (initialSize > 0)
        {
            int target = initialSize / 2;
            _middleNode = _linkedList.Find(target);
        }
    }

    public long MeasureAddToEnd(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            _linkedList.AddLast(dataSize);
        });
    }

    public long MeasureAddToBeginning(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            _linkedList.AddFirst(dataSize);
        });
    }

    public long MeasureAddToMiddle(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            if (_middleNode != null)
            {
                _linkedList.AddAfter(_middleNode, dataSize);
            }
            else
            {
                _linkedList.AddLast(dataSize);
            }
        });
    }

    public long MeasureInsertWithIndex(int dataSize, int index)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            if (index == 0)
            {
                _linkedList.AddFirst(dataSize);
            }
            else if (index >= _linkedList.Count)
            {
                _linkedList.AddLast(dataSize);
            }
            else
            {
                var currentNode = _linkedList.First;
                for (int i = 0; i < index - 1; i++)
                {
                    currentNode = currentNode?.Next;
                }

                if (currentNode != null)
                {
                    _linkedList.AddAfter(currentNode, dataSize);
                }
            }
        });
    }

    public void Cleanup()
    {
        _linkedList?.Clear();
        _linkedList = null;
        _middleNode = null;
    }
}