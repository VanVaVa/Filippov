using lab04.Tasks;
using Xunit;

namespace lab04.Tests;

public class SleepingBarberTests
{
    [Fact]
    public void CustomerArrives_ShouldAcceptCustomer()
    {
        var shop = new SleepingBarber(5);
        shop.Start();

        bool result = shop.CustomerArrives(1);

        Thread.Sleep(500);
        shop.Stop();

        Assert.True(result);
    }

    [Fact]
    public void CustomerArrives_ShouldRejectWhenFull()
    {
        var shop = new SleepingBarber(2);

        shop.CustomerArrives(1);
        shop.CustomerArrives(2);

        Assert.Equal(2, shop.GetWaitingRoomCount());

        bool result = shop.CustomerArrives(3);

        Assert.False(result);
        Assert.Equal(2, shop.GetWaitingRoomCount());
    }

    [Fact]
    public void GetCustomersServed_ShouldReturnCount()
    {
        var shop = new SleepingBarber(5);
        shop.Start();

        for (int idx = 0; idx < 3; idx++)
        {
            shop.CustomerArrives(idx);
        }

        Thread.Sleep(1000);
        shop.Stop();

        Assert.True(shop.GetCustomersServed() > 0);
    }
}

