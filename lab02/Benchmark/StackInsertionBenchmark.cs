using System.Collections.Generic;

namespace CollectionsBenchmark.Tests.InsertionTests;

public class StackInsertionBenchmark : IInsertionBenchmark
{
    private Stack<int> _stack;

    public string CollectionName => "Stack<T>";

    public void InitializeCollection(int initialSize)
    {
        _stack = new Stack<int>(initialSize + 1);

        for (int i = 0; i < initialSize; i++)
        {
            _stack.Push(i);
        }
    }

    public long MeasureAddToEnd(int dataSize)
    {
        throw new NotSupportedException("Stack<T> не поддерживает добавление в конец");
    }

    public long MeasureAddToBeginning(int dataSize)
    {
        InitializeCollection(dataSize - 1);

        return IInsertionBenchmark.MeasureOperation(() =>
        {
            _stack.Push(dataSize);
        });
    }

    public long MeasureAddToMiddle(int dataSize)
    {
        throw new NotSupportedException("Stack<T> не поддерживает вставку в середину");
    }

    public long MeasureInsertWithIndex(int dataSize, int index)
    {
        throw new NotSupportedException("Stack<T> не поддерживает вставку по индексу");
    }

    public void Cleanup()
    {
        _stack?.Clear();
        _stack = null;
    }
}