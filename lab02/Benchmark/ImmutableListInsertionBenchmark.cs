using System.Collections.Immutable;

namespace CollectionsBenchmark.Tests.InsertionTests;

public class ImmutableListInsertionBenchmark : IInsertionBenchmark
{
    private ImmutableList<int> _immutableList;

    public string CollectionName => "ImmutableList<T>";

    public void InitializeCollection(int initialSize)
    {
        _immutableList = ImmutableList<int>.Empty;

        for (int i = 0; i < initialSize; i++)
        {
            _immutableList = _immutableList.Add(i);
        }
    }

    public long MeasureAddToEnd(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            _immutableList = _immutableList.Add(dataSize);
        });
    }

    public long MeasureAddToBeginning(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            _immutableList = _immutableList.Insert(0, dataSize);
        });
    }

    public long MeasureAddToMiddle(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            int middleIndex = dataSize / 2;
            _immutableList = _immutableList.Insert(middleIndex, dataSize);
        });
    }

    public long MeasureInsertWithIndex(int dataSize, int index)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            _immutableList = _immutableList.Insert(index, dataSize);
        });
    }

    public void Cleanup()
    {
        _immutableList = null;
    }
}