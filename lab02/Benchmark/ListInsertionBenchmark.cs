using System.Collections.Generic;

namespace CollectionsBenchmark.Tests.InsertionTests;

public class ListInsertionBenchmark : IInsertionBenchmark
{
    private List<int> _list;
    private const int CapacityPreallocation = 100_000;

    public string CollectionName => "List<T>";

    public void InitializeCollection(int initialSize)
    {
        _list = new List<int>(CapacityPreallocation);

        for (int i = 0; i < initialSize; i++)
        {
            _list.Add(i);
        }
    }

    public long MeasureAddToEnd(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            _list.Add(dataSize);
        });
    }

    public long MeasureAddToBeginning(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            _list.Insert(0, dataSize);
        });
    }

    public long MeasureAddToMiddle(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            int middleIndex = dataSize / 2;
            _list.Insert(middleIndex, dataSize);
        });
    }

    public long MeasureInsertWithIndex(int dataSize, int index)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            _list.Insert(index, dataSize);
        });
    }

    public void Cleanup()
    {
        _list?.Clear();
        _list = null;
    }
}