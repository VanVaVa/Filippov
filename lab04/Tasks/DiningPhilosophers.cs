using System.Collections.Concurrent;

namespace lab04.Tasks;

public class DiningPhilosophers
{
    private const int TotalPhilosophers = 5;
    private readonly object[] utensils;
    private readonly Thread[] philosopherThreads;
    private readonly CancellationTokenSource cancellationSource;
    private readonly ConcurrentDictionary<int, int> mealCounter;

    public DiningPhilosophers()
    {
        utensils = new object[TotalPhilosophers];
        philosopherThreads = new Thread[TotalPhilosophers];
        cancellationSource = new CancellationTokenSource();
        mealCounter = new ConcurrentDictionary<int, int>();

        for (int index = 0; index < TotalPhilosophers; index++)
        {
            utensils[index] = new object();
            mealCounter[index] = 0;
        }
    }

    public void StartWithDeadlock()
    {
        for (int index = 0; index < TotalPhilosophers; index++)
        {
            int currentId = index;
            philosopherThreads[index] = new Thread(() => ExecutePhilosopherWithDeadlock(currentId, cancellationSource.Token));
            philosopherThreads[index].Start();
        }
    }

    public void StartWithoutDeadlock()
    {
        for (int index = 0; index < TotalPhilosophers; index++)
        {
            int currentId = index;
            philosopherThreads[index] = new Thread(() => ExecutePhilosopherWithoutDeadlock(currentId, cancellationSource.Token));
            philosopherThreads[index].Start();
        }
    }

    public void Stop()
    {
        cancellationSource.Cancel();
        foreach (var thread in philosopherThreads)
        {
            thread.Join(TimeSpan.FromSeconds(2));
        }
    }

    public Dictionary<int, int> GetEatCounts()
    {
        return mealCounter.ToDictionary(entry => entry.Key, entry => entry.Value);
    }

    private void ExecutePhilosopherWithDeadlock(int philosopherId, CancellationToken token)
    {
        int leftUtensil = philosopherId;
        int rightUtensil = (philosopherId + 1) % TotalPhilosophers;

        while (!token.IsCancellationRequested)
        {
            PerformThinking(philosopherId);

            lock (utensils[leftUtensil])
            {
                Thread.Sleep(10);
                lock (utensils[rightUtensil])
                {
                    PerformEating(philosopherId);
                    mealCounter[philosopherId]++;
                }
            }
        }
    }

    private void ExecutePhilosopherWithoutDeadlock(int philosopherId, CancellationToken token)
    {
        int leftUtensil = philosopherId;
        int rightUtensil = (philosopherId + 1) % TotalPhilosophers;

        int lowerIndex = Math.Min(leftUtensil, rightUtensil);
        int higherIndex = Math.Max(leftUtensil, rightUtensil);

        while (!token.IsCancellationRequested)
        {
            PerformThinking(philosopherId);

            lock (utensils[lowerIndex])
            {
                Thread.Sleep(10);
                lock (utensils[higherIndex])
                {
                    PerformEating(philosopherId);
                    mealCounter[philosopherId]++;
                }
            }
        }
    }

    private void PerformThinking(int philosopherId)
    {
        Thread.Sleep(Random.Shared.Next(50, 150));
    }

    private void PerformEating(int philosopherId)
    {
        Thread.Sleep(Random.Shared.Next(50, 150));
    }
}

