using lab03.Collections;
using Xunit;

namespace lab03.Tests;

public class SimpleDictionaryTests
{
    [Fact]
    public void Add_ShouldIncreaseCount()
    {
        var instance = new SimpleDictionary<string, int>();
        instance.Add("один", 1);
        instance.Add("два", 2);

        Assert.Equal(2, instance.Count);
    }

    [Fact]
    public void Indexer_ShouldGetAndSet()
    {
        var instance = new SimpleDictionary<string, int>();
        instance.Add("один", 1);

        Assert.Equal(1, instance["один"]);

        instance["один"] = 10;
        Assert.Equal(10, instance["один"]);
    }

    [Fact]
    public void ContainsKey_ShouldReturnTrueForExistingKey()
    {
        var instance = new SimpleDictionary<string, int>();
        instance.Add("один", 1);

        Assert.True(instance.ContainsKey("один"));
        Assert.False(instance.ContainsKey("два"));
    }

    [Fact]
    public void Remove_ShouldDecreaseCount()
    {
        var instance = new SimpleDictionary<string, int>();
        instance.Add("один", 1);
        instance.Add("два", 2);

        bool result = instance.Remove("один");

        Assert.True(result);
        Assert.Equal(1, instance.Count);
        Assert.False(instance.ContainsKey("один"));
    }

    [Fact]
    public void TryGetValue_ShouldReturnTrueForExistingKey()
    {
        var instance = new SimpleDictionary<string, int>();
        instance.Add("один", 1);

        Assert.True(instance.TryGetValue("один", out int result));
        Assert.Equal(1, result);

        Assert.False(instance.TryGetValue("два", out _));
    }

    [Fact]
    public void GetEnumerator_ShouldIterateAllItems()
    {
        var instance = new SimpleDictionary<string, int>();
        instance.Add("один", 1);
        instance.Add("два", 2);

        var collected = new List<KeyValuePair<string, int>>();
        foreach (var pair in instance)
        {
            collected.Add(pair);
        }

        Assert.Equal(2, collected.Count);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var instance = new SimpleDictionary<string, int>();
        instance.Add("один", 1);
        instance.Add("два", 2);

        instance.Clear();

        Assert.Equal(0, instance.Count);
    }
}

