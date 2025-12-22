using Xunit;

namespace CollectionsBenchmark.Tests;

public class LinkedListTests
{
    private readonly LinkedList<int> _linkedList;

    public LinkedListTests()
    {
        _linkedList = new LinkedList<int>();
        _linkedList.AddLast(1);
        _linkedList.AddLast(2);
        _linkedList.AddLast(3);
        _linkedList.AddLast(4);
        _linkedList.AddLast(5);
    }

    [Fact]
    public void LinkedList_AddLast_ShouldAddElementToEnd()
    {
        var initialCount = _linkedList.Count;

        _linkedList.AddLast(6);

        Assert.Equal(initialCount + 1, _linkedList.Count);
        Assert.Equal(6, _linkedList.Last.Value);
        Assert.Equal(1, _linkedList.First.Value);
        Assert.Contains(6, _linkedList);
    }

    [Fact]
    public void LinkedList_AddFirst_ShouldAddElementToBeginning()
    {
        var initialCount = _linkedList.Count;

        _linkedList.AddFirst(0);

        Assert.Equal(initialCount + 1, _linkedList.Count);
        Assert.Equal(0, _linkedList.First.Value);
        Assert.Equal(5, _linkedList.Last.Value);
    }

    [Fact]
    public void LinkedList_AddAfter_ShouldAddElementAfterSpecificNode()
    {
        var node = _linkedList.Find(3);

        _linkedList.AddAfter(node!, 99);

        Assert.Equal(6, _linkedList.Count);
        Assert.Equal(99, node!.Next!.Value);
        Assert.Equal(4, node.Next.Next!.Value);
    }

    [Fact]
    public void LinkedList_AddBefore_ShouldAddElementBeforeSpecificNode()
    {
        var node = _linkedList.Find(3);

        _linkedList.AddBefore(node!, 99);

        Assert.Equal(6, _linkedList.Count);
        Assert.Equal(99, node!.Previous!.Value);
        Assert.Equal(2, node.Previous.Previous!.Value);
    }

    [Fact]
    public void LinkedList_RemoveFirst_ShouldRemoveFirstElement()
    {
        var initialCount = _linkedList.Count;
        var firstValue = _linkedList.First.Value;

        _linkedList.RemoveFirst();

        Assert.Equal(initialCount - 1, _linkedList.Count);
        Assert.NotEqual(firstValue, _linkedList.First.Value);
        Assert.Equal(2, _linkedList.First.Value);
        Assert.DoesNotContain(1, _linkedList);
    }

    [Fact]
    public void LinkedList_RemoveLast_ShouldRemoveLastElement()
    {
        var initialCount = _linkedList.Count;
        var lastValue = _linkedList.Last.Value;

        _linkedList.RemoveLast();

        Assert.Equal(initialCount - 1, _linkedList.Count);
        Assert.NotEqual(lastValue, _linkedList.Last.Value);
        Assert.Equal(4, _linkedList.Last.Value);
        Assert.DoesNotContain(5, _linkedList);
    }

    [Fact]
    public void LinkedList_Remove_ShouldRemoveElementByValue()
    {
        var initialCount = _linkedList.Count;

        bool removed = _linkedList.Remove(3);

        Assert.True(removed);
        Assert.Equal(initialCount - 1, _linkedList.Count);
        Assert.DoesNotContain(3, _linkedList);

        Assert.Equal(4, _linkedList.Find(2)!.Next!.Value);
        Assert.Equal(2, _linkedList.Find(4)!.Previous!.Value);
    }

    [Fact]
    public void LinkedList_Contains_ShouldReturnCorrectResult()
    {
        Assert.True(_linkedList.Contains(1));
        Assert.True(_linkedList.Contains(3));
        Assert.True(_linkedList.Contains(5));
        Assert.False(_linkedList.Contains(99));
        Assert.False(_linkedList.Contains(0));
    }

    [Fact]
    public void LinkedList_Find_ShouldReturnCorrectNode()
    {
        var node1 = _linkedList.Find(1);
        Assert.NotNull(node1);
        Assert.Equal(1, node1!.Value);

        var node3 = _linkedList.Find(3);
        Assert.NotNull(node3);
        Assert.Equal(3, node3!.Value);

        var node5 = _linkedList.Find(5);
        Assert.NotNull(node5);
        Assert.Equal(5, node5!.Value);

        var node99 = _linkedList.Find(99);
        Assert.Null(node99);
    }

    [Fact]
    public void LinkedList_Clear_ShouldRemoveAllElements()
    {
        _linkedList.Clear();

        Assert.Empty(_linkedList);
        Assert.Equal(0, _linkedList.Count);
        Assert.Null(_linkedList.First);
        Assert.Null(_linkedList.Last);
    }

    [Fact]
    public void LinkedList_Operations_ShouldBeDeterministic()
    {
        var list1 = new LinkedList<int>();
        var list2 = new LinkedList<int>();

        for (int i = 0; i < 5; i++)
        {
            list1.AddLast(i);
            list2.AddLast(i);
        }

        list1.AddFirst(-1);
        list2.AddFirst(-1);

        var node1 = list1.Find(2);
        var node2 = list2.Find(2);
        list1.AddAfter(node1!, 99);
        list2.AddAfter(node2!, 99);

        list1.Remove(3);
        list2.Remove(3);

        Assert.Equal(list1.Count, list2.Count);

        var current1 = list1.First;
        var current2 = list2.First;

        while (current1 != null && current2 != null)
        {
            Assert.Equal(current1.Value, current2.Value);
            current1 = current1.Next;
            current2 = current2.Next;
        }
    }
}