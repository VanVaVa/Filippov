using System.Diagnostics;

namespace CollectionsBenchmark.Tests.InsertionTests;

public interface IInsertionBenchmark
{
    string CollectionName { get; }

    long MeasureAddToEnd(int dataSize);
    long MeasureAddToBeginning(int dataSize);
    long MeasureAddToMiddle(int dataSize);
    long MeasureInsertWithIndex(int dataSize, int index);

    void InitializeCollection(int initialSize);
    void Cleanup();

    protected static long MeasureOperation(Action operation, int iterations = 5)
    {
        var times = new List<long>();

        for (int i = 0; i < iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            operation();
            sw.Stop();
            times.Add(sw.ElapsedTicks);

            Thread.Sleep(10);
        }

        return (long)times.Average();
    }
}