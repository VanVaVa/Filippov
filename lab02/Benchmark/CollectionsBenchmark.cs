using CollectionsBenchmark.Tests.InsertionTests;

namespace CollectionsBenchmark;

public class InsertionBenchmarkRunner
{
    private const int DataSize = 100_000;
    private const int WarmupRuns = 3;

    private readonly List<IInsertionBenchmark> _benchmarks;

    public InsertionBenchmarkRunner()
    {
        _benchmarks = new List<IInsertionBenchmark>
        {
            new ListInsertionBenchmark(),
            new LinkedListInsertionBenchmark(),
            new QueueInsertionBenchmark(),
            new StackInsertionBenchmark(),
            new ImmutableListInsertionBenchmark()
        };
    }

    public void RunAllInsertionTests()
    {
        Console.WriteLine("ТЕСТИРОВАНИЕ ОПЕРАЦИЙ ВСТАВКИ\n");

        foreach (var benchmark in _benchmarks)
        {
            Console.WriteLine($"--- {benchmark.CollectionName} ---");

            try
            {
                RunSingleBenchmark(benchmark);
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"Операция не поддерживается: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                benchmark.Cleanup();
            }
        }
    }

    private void RunSingleBenchmark(IInsertionBenchmark benchmark)
    {
        var results = new Dictionary<string, long>();

        try { results["В конец"] = benchmark.MeasureAddToEnd(DataSize); }
        catch (NotSupportedException) { results["В конец"] = -1; }

        try { results["В начало"] = benchmark.MeasureAddToBeginning(DataSize); }
        catch (NotSupportedException) { results["В начало"] = -1; }

        try { results["В середину"] = benchmark.MeasureAddToMiddle(DataSize); }
        catch (NotSupportedException) { results["В середину"] = -1; }

        foreach (var result in results)
        {
            if (result.Value == -1)
                Console.WriteLine($"{result.Key}: НЕ ПОДДЕРЖИВАЕТСЯ");
            else
                Console.WriteLine($"{result.Key}: {result.Value} тактов");
        }
    }
}