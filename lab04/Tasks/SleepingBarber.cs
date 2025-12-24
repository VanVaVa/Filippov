using System.Collections.Concurrent;

namespace lab04.Tasks;

public class SleepingBarber
{
    private readonly SemaphoreSlim barberSignal;
    private readonly SemaphoreSlim customerSignal;
    private readonly Mutex accessLock;
    private readonly ConcurrentQueue<int> queue;
    private readonly int maxCapacity;
    private Thread? barberWorker;
    private bool running;
    private int servedCount;

    public SleepingBarber(int maxWaitingRoomSize = 5)
    {
        maxCapacity = maxWaitingRoomSize;
        barberSignal = new SemaphoreSlim(0, 1);
        customerSignal = new SemaphoreSlim(0);
        accessLock = new Mutex();
        queue = new ConcurrentQueue<int>();
        running = false;
        servedCount = 0;
    }

    public void Start()
    {
        if (running)
            return;

        running = true;
        barberWorker = new Thread(ProcessBarberWork);
        barberWorker.Start();
    }

    public void Stop()
    {
        running = false;
        try
        {
            barberSignal.Release();
        }
        catch (SemaphoreFullException)
        {
        }
        barberWorker?.Join(TimeSpan.FromSeconds(2));
    }

    public bool CustomerArrives(int customerId)
    {
        accessLock.WaitOne();
        try
        {
            if (queue.Count >= maxCapacity)
            {
                return false;
            }

            bool emptyBefore = queue.Count == 0;
            queue.Enqueue(customerId);

            if (emptyBefore)
            {
                try
                {
                    barberSignal.Release();
                }
                catch (SemaphoreFullException)
                {
                }
            }

            return true;
        }
        finally
        {
            accessLock.ReleaseMutex();
        }
    }

    public int GetCustomersServed()
    {
        return servedCount;
    }

    public int GetWaitingRoomCount()
    {
        accessLock.WaitOne();
        try
        {
            return queue.Count;
        }
        finally
        {
            accessLock.ReleaseMutex();
        }
    }

    private void ProcessBarberWork()
    {
        while (running)
        {
            barberSignal.Wait();

            if (!running)
                break;

            while (true)
            {
                accessLock.WaitOne();
                try
                {
                    if (queue.TryDequeue(out int clientId))
                    {
                        accessLock.ReleaseMutex();
                        ExecuteHaircut(clientId);
                        servedCount++;
                    }
                    else
                    {
                        accessLock.ReleaseMutex();
                        break;
                    }
                }
                catch
                {
                    accessLock.ReleaseMutex();
                    break;
                }
            }
        }
    }

    private void ExecuteHaircut(int customerId)
    {
        Thread.Sleep(Random.Shared.Next(100, 300));
    }
}

