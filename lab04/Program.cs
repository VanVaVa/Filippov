using lab04.Tasks;

namespace lab04;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Философы с deadlock");
        var deadlockPhilosophers = new DiningPhilosophers();
        deadlockPhilosophers.StartWithDeadlock();
        Thread.Sleep(2000);
        deadlockPhilosophers.Stop();
        Console.WriteLine("Стоп");

        Console.WriteLine("\nФилософы без deadlock");
        var safePhilosophers = new DiningPhilosophers();
        safePhilosophers.StartWithoutDeadlock();
        Thread.Sleep(2000);
        safePhilosophers.Stop();
        var mealStats = safePhilosophers.GetEatCounts();
        foreach (var stat in mealStats)
        {
            Console.WriteLine($"Философ {stat.Key} поел {stat.Value} раз");
        }

        Console.WriteLine("\nПарикмахер");
        var barberShop = new SleepingBarber(5);
        barberShop.Start();

        for (int idx = 0; idx < 10; idx++)
        {
            int clientId = idx;
            new Thread(() =>
            {
                if (barberShop.CustomerArrives(clientId))
                {
                    Console.WriteLine($"Клиент {clientId} зашел");
                }
                else
                {
                    Console.WriteLine($"Клиент {clientId} ушел - нет места");
                }
            }).Start();
            Thread.Sleep(100);
        }

        Thread.Sleep(3000);
        barberShop.Stop();
        Console.WriteLine($"Обслужено: {barberShop.GetCustomersServed()}");

        Console.WriteLine("\nПроизводитель-потребитель");
        var pcSystem = new ProducerConsumer<int>(5);
        pcSystem.Start(
            producerCount: 2,
            consumerCount: 3,
            produceItem: (id) => id * 10,
            consumeItem: (item, consumerId) => Console.WriteLine($"Потребитель {consumerId} взял {item}")
        );

        Thread.Sleep(2000);
        pcSystem.Stop();
        Console.WriteLine($"Произведено: {pcSystem.GetItemsProduced()}, потреблено: {pcSystem.GetItemsConsumed()}");
    }
}

