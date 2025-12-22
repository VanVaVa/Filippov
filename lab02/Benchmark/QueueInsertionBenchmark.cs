using System.Collections.Generic;

namespace CollectionsBenchmark.Tests.InsertionTests;

public class QueueInsertionBenchmark : IInsertionBenchmark
{
    private Queue<int> _queue;

    public string CollectionName => "Queue<T>";

    public void InitializeCollection(int initialSize)
    {
        _queue = new Queue<int>(initialSize + 1);

        for (int i = 0; i < initialSize; i++)
        {
            _queue.Enqueue(i);
        }
    }

    public long MeasureAddToEnd(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            _queue.Enqueue(dataSize);
        });
    }

    public long MeasureAddToBeginning(int dataSize)
    {
        throw new NotSupportedException("Queue<T> не поддерживает вставку в начало");
    }

    public long MeasureAddToMiddle(int dataSize)
    {
        throw new NotSupportedException("Queue<T> не поддерживает вставку в середину");
    }

    public long MeasureInsertWithIndex(int dataSize, int index)
    {
        throw new NotSupportedException("Queue<T> не поддерживает вставку по индексу");
    }

    public void Cleanup()
    {
        _queue?.Clear();
        _queue = null;
    }
}