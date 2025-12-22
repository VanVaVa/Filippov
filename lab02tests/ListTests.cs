using Xunit;

namespace CollectionsBenchmark.Tests;

public class ListTests
{
    private readonly List<int> _list;

    public ListTests()
    {
        _list = new List<int> { 1, 2, 3, 4, 5 };
    }

    [Fact]
    public void List_Add_ShouldAddElementToEnd()
    {
        var initialCount = _list.Count;

        _list.Add(6);

        Assert.Equal(initialCount + 1, _list.Count);
        Assert.Equal(6, _list[^1]);
        Assert.Equal(new List<int> { 1, 2, 3, 4, 5, 6 }, _list);
    }

    [Fact]
    public void List_Insert_ShouldAddElementAtSpecificPosition()
    {
        var initialCount = _list.Count;

        _list.Insert(0, 0);

        Assert.Equal(initialCount + 1, _list.Count);
        Assert.Equal(0, _list[0]);
        Assert.Equal(new List<int> { 0, 1, 2, 3, 4, 5 }, _list);

        _list.Insert(3, 99);

        Assert.Equal(7, _list.Count);
        Assert.Equal(99, _list[3]);
    }

    [Fact]
    public void List_Remove_ShouldRemoveElementByValue()
    {
        var initialCount = _list.Count;

        bool removed = _list.Remove(3);

        Assert.True(removed);
        Assert.Equal(initialCount - 1, _list.Count);
        Assert.DoesNotContain(3, _list);
        Assert.Equal(new List<int> { 1, 2, 4, 5 }, _list);
    }

    [Fact]
    public void List_RemoveAt_ShouldRemoveElementAtIndex()
    {
        var initialCount = _list.Count;

        _list.RemoveAt(0);

        Assert.Equal(initialCount - 1, _list.Count);
        Assert.Equal(2, _list[0]);
        Assert.Equal(new List<int> { 2, 3, 4, 5 }, _list);

        _list.RemoveAt(_list.Count - 1);

        Assert.Equal(3, _list.Count);
        Assert.Equal(4, _list[^1]);
    }

    [Fact]
    public void List_Contains_ShouldReturnCorrectResult()
    {
        Assert.True(_list.Contains(3));
        Assert.True(_list.Contains(5));
        Assert.False(_list.Contains(99));
        Assert.False(_list.Contains(-1));
    }

    [Fact]
    public void List_IndexOf_ShouldReturnCorrectIndex()
    {
        Assert.Equal(0, _list.IndexOf(1));
        Assert.Equal(2, _list.IndexOf(3));
        Assert.Equal(4, _list.IndexOf(5));
        Assert.Equal(-1, _list.IndexOf(99));
    }

    [Fact]
    public void List_Operations_ShouldBeDeterministic()
    {
        var list1 = new List<int>();
        var list2 = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            list1.Add(i);
            list2.Add(i);
        }

        list1.Insert(5, 99);
        list2.Insert(5, 99);

        list1.Remove(7);
        list2.Remove(7);

        Assert.Equal(list1.Count, list2.Count);
        Assert.Equal(list1, list2);

        for (int i = 0; i < list1.Count; i++)
        {
            Assert.Equal(list1[i], list2[i]);
        }
    }

    [Fact]
    public void List_Clear_ShouldRemoveAllElements()
    {
        _list.Clear();

        Assert.Empty(_list);
        Assert.Equal(0, _list.Count);
    }

    [Fact]
    public void List_GetByIndex_ShouldReturnCorrectElement()
    {
        Assert.Equal(1, _list[0]);
        Assert.Equal(3, _list[2]);
        Assert.Equal(5, _list[4]);

        Assert.Throws<ArgumentOutOfRangeException>(() => _list[10]);
        Assert.Throws<ArgumentOutOfRangeException>(() => _list[-1]);
    }
}