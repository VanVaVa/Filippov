using lab04.Tasks;
using Xunit;

namespace lab04.Tests;

public class DiningPhilosophersTests
{
    [Fact]
    public void StartWithoutDeadlock_ShouldNotDeadlock()
    {
        var instance = new DiningPhilosophers();
        instance.StartWithoutDeadlock();

        Thread.Sleep(1000);

        instance.Stop();
        var result = instance.GetEatCounts();

        Assert.True(result.Values.Sum() > 0);
    }

    [Fact]
    public void GetEatCounts_ShouldReturnCounts()
    {
        var instance = new DiningPhilosophers();
        instance.StartWithoutDeadlock();

        Thread.Sleep(500);

        instance.Stop();
        var result = instance.GetEatCounts();

        Assert.Equal(5, result.Count);
    }
}

