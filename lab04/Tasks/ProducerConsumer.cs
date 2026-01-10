using System.Collections.Concurrent;

namespace lab04.Tasks;

public class ProducerConsumer<T>
{
    private readonly BlockingCollection<T> storage;
    private readonly SemaphoreSlim producerLock;
    private readonly SemaphoreSlim consumerLock;
    private readonly int capacity;
    private readonly List<Thread> producerThreads;
    private readonly List<Thread> consumerThreads;
    private bool active;
    private int producedCount;
    private int consumedCount;

    public ProducerConsumer(int bufferSize = 5)
    {
        capacity = bufferSize;
        storage = new BlockingCollection<T>(boundedCapacity: bufferSize);
        producerLock = new SemaphoreSlim(bufferSize, bufferSize);
        consumerLock = new SemaphoreSlim(0, bufferSize);
        producerThreads = new List<Thread>();
        consumerThreads = new List<Thread>();
        active = false;
        producedCount = 0;
        consumedCount = 0;
    }

    public void Start(int producerCount, int consumerCount, Func<int, T> produceItem, Action<T, int> consumeItem)
    {
        if (active)
            return;

        active = true;

        for (int idx = 0; idx < producerCount; idx++)
        {
            int currentProducerId = idx;
            var producerThread = new Thread(() => RunProducer(currentProducerId, produceItem));
            producerThreads.Add(producerThread);
            producerThread.Start();
        }

        for (int idx = 0; idx < consumerCount; idx++)
        {
            int currentConsumerId = idx;
            var consumerThread = new Thread(() => RunConsumer(currentConsumerId, consumeItem));
            consumerThreads.Add(consumerThread);
            consumerThread.Start();
        }
    }

    public void Stop()
    {
        active = false;
        storage.CompleteAdding();

        foreach (var thread in producerThreads)
        {
            thread.Join(TimeSpan.FromSeconds(2));
        }

        foreach (var thread in consumerThreads)
        {
            thread.Join(TimeSpan.FromSeconds(2));
        }
    }

    public int GetItemsProduced()
    {
        return producedCount;
    }

    public int GetItemsConsumed()
    {
        return consumedCount;
    }

    private void RunProducer(int producerId, Func<int, T> produceItem)
    {
        while (active)
        {
            producerLock.Wait();

            if (!active)
                break;

            try
            {
                T newItem = produceItem(producerId);
                storage.Add(newItem);
                Interlocked.Increment(ref producedCount);
                consumerLock.Release();
            }
            catch (InvalidOperationException)
            {
                break;
            }
        }
    }

    private void RunConsumer(int consumerId, Action<T, int> consumeItem)
    {
        while (active || !storage.IsCompleted)
        {
            consumerLock.Wait();

            if (!active && storage.IsCompleted)
                break;

            try
            {
                if (storage.TryTake(out T? item, TimeSpan.FromMilliseconds(100)))
                {
                    consumeItem(item, consumerId);
                    Interlocked.Increment(ref consumedCount);
                    producerLock.Release();
                }
            }
            catch
            {
            }
        }
    }
}

