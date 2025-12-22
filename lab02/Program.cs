using CollectionsBenchmark.Tests.InsertionTests;

namespace CollectionsBenchmark;

public class Program
{
    public static void Main()
    {
        InsertionBenchmarkRunner runner = new();

        runner.RunAllInsertionTests();
    }
}