using lab03.Collections;
using Xunit;

namespace lab03.Tests;

public class SimpleListTests
{
    [Fact]
    public void Add_ShouldIncreaseCount()
    {
        var instance = new SimpleList<int>();
        instance.Add(1);
        instance.Add(2);

        Assert.Equal(2, instance.Count);
    }

    [Fact]
    public void Indexer_ShouldGetAndSet()
    {
        var instance = new SimpleList<int>();
        instance.Add(1);
        instance.Add(2);

        Assert.Equal(1, instance[0]);
        Assert.Equal(2, instance[1]);

        instance[0] = 10;
        Assert.Equal(10, instance[0]);
    }

    [Fact]
    public void Remove_ShouldDecreaseCount()
    {
        var instance = new SimpleList<int>();
        instance.Add(1);
        instance.Add(2);
        instance.Add(3);

        bool result = instance.Remove(2);

        Assert.True(result);
        Assert.Equal(2, instance.Count);
        Assert.False(instance.Contains(2));
    }

    [Fact]
    public void Contains_ShouldReturnTrueForExistingItem()
    {
        var instance = new SimpleList<int>();
        instance.Add(1);
        instance.Add(2);

        Assert.True(instance.Contains(1));
        Assert.False(instance.Contains(3));
    }

    [Fact]
    public void IndexOf_ShouldReturnCorrectIndex()
    {
        var instance = new SimpleList<int>();
        instance.Add(1);
        instance.Add(2);
        instance.Add(3);

        Assert.Equal(1, instance.IndexOf(2));
        Assert.Equal(-1, instance.IndexOf(4));
    }

    [Fact]
    public void Insert_ShouldAddAtSpecifiedIndex()
    {
        var instance = new SimpleList<int>();
        instance.Add(1);
        instance.Add(3);

        instance.Insert(1, 2);

        Assert.Equal(3, instance.Count);
        Assert.Equal(2, instance[1]);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var instance = new SimpleList<int>();
        instance.Add(1);
        instance.Add(2);

        instance.Clear();

        Assert.Equal(0, instance.Count);
    }

    [Fact]
    public void GetEnumerator_ShouldIterateAllItems()
    {
        var instance = new SimpleList<int>();
        instance.Add(1);
        instance.Add(2);
        instance.Add(3);

        var collected = new List<int>();
        foreach (var element in instance)
        {
            collected.Add(element);
        }

        Assert.Equal(3, collected.Count);
        Assert.Equal(1, collected[0]);
        Assert.Equal(2, collected[1]);
        Assert.Equal(3, collected[2]);
    }
}

