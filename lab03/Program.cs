using lab03.Collections;

namespace lab03;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("тестирование SimpleList:");
        var collection = new SimpleList<int>();
        collection.Add(1);
        collection.Add(2);
        collection.Add(3);
        Console.WriteLine($"количество элементов: {collection.Count}");
        foreach (var element in collection)
        {
            Console.WriteLine(element);
        }

        Console.WriteLine("\nтестирование SimpleDictionary:");
        var map = new SimpleDictionary<string, int>();
        map.Add("один", 1);
        map.Add("два", 2);
        map.Add("три", 3);
        Console.WriteLine($"количество элементов: {map.Count}");
        foreach (var pair in map)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }

        Console.WriteLine("\nтестирование DoublyLinkedList:");
        var chain = new DoublyLinkedList<int>();
        chain.Add(10);
        chain.Add(20);
        chain.Add(30);
        Console.WriteLine($"количество элементов: {chain.Count}");
        foreach (var element in chain)
        {
            Console.WriteLine(element);
        }
    }
}

