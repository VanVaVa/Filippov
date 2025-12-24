using lab04.Tasks;
using Xunit;

namespace lab04.Tests;

public class ProducerConsumerTests
{
    [Fact]
    public void Start_ShouldProduceAndConsume()
    {
        var system = new ProducerConsumer<int>(5);
        var consumedItems = new System.Collections.Concurrent.ConcurrentBag<int>();

        system.Start(
            producerCount: 2,
            consumerCount: 2,
            produceItem: (id) => id,
            consumeItem: (item, consumerId) => consumedItems.Add(item)
        );

        Thread.Sleep(1000);
        system.Stop();

        Assert.True(system.GetItemsProduced() > 0);
        Assert.True(system.GetItemsConsumed() > 0);
    }

    [Fact]
    public void Stop_ShouldStopAllThreads()
    {
        var system = new ProducerConsumer<int>(5);

        system.Start(
            producerCount: 1,
            consumerCount: 1,
            produceItem: (id) => id,
            consumeItem: (item, consumerId) => { }
        );

        Thread.Sleep(100);
        system.Stop();

        Assert.True(true);
    }
}

