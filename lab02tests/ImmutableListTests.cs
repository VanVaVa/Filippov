using Xunit;
using System.Collections.Immutable;

namespace CollectionsBenchmark.Tests;

public class ImmutableListTests
{
    private readonly ImmutableList<int> _immutableList;

    public ImmutableListTests()
    {
        _immutableList = ImmutableList<int>.Empty
            .Add(1)
            .Add(2)
            .Add(3)
            .Add(4)
            .Add(5);
    }

    [Fact]
    public void ImmutableList_Add_ShouldCreateNewListWithElementAtEnd()
    {
        var newList = _immutableList.Add(6);

        Assert.Equal(5, _immutableList.Count);
        Assert.Equal(6, newList.Count);
        Assert.Equal(6, newList[^1]);
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, newList);

        Assert.NotSame(_immutableList, newList);
    }

    [Fact]
    public void ImmutableList_Insert_ShouldCreateNewListWithElementAtPosition()
    {
        var listWithStart = _immutableList.Insert(0, 0);

        Assert.Equal(5, _immutableList.Count);
        Assert.Equal(6, listWithStart.Count);
        Assert.Equal(0, listWithStart[0]);
        Assert.Equal(new[] { 0, 1, 2, 3, 4, 5 }, listWithStart);

        var listWithMiddle = _immutableList.Insert(3, 99);

        Assert.Equal(6, listWithMiddle.Count);
        Assert.Equal(99, listWithMiddle[3]);
        Assert.Equal(new[] { 1, 2, 3, 99, 4, 5 }, listWithMiddle);
    }

    [Fact]
    public void ImmutableList_Remove_ShouldCreateNewListWithoutElement()
    {
        var newList = _immutableList.Remove(3);

        Assert.Equal(5, _immutableList.Count);
        Assert.Equal(4, newList.Count);
        Assert.DoesNotContain(3, newList);
        Assert.Equal(new[] { 1, 2, 4, 5 }, newList);

        var sameList = _immutableList.Remove(99);
        Assert.Same(_immutableList, sameList);
    }

    [Fact]
    public void ImmutableList_RemoveAt_ShouldCreateNewListWithoutElementAtIndex()
    {
        var listWithoutStart = _immutableList.RemoveAt(0);

        Assert.Equal(5, _immutableList.Count);
        Assert.Equal(4, listWithoutStart.Count);
        Assert.Equal(2, listWithoutStart[0]);
        Assert.Equal(new[] { 2, 3, 4, 5 }, listWithoutStart);

        var listWithoutEnd = _immutableList.RemoveAt(_immutableList.Count - 1);

        Assert.Equal(4, listWithoutEnd.Count);
        Assert.Equal(4, listWithoutEnd[^1]);
    }

    [Fact]
    public void ImmutableList_RemoveAll_ShouldCreateNewListWithoutMatchingElements()
    {
        var listWithDuplicates = _immutableList
            .Add(2)
            .Add(3)
            .Add(2);

        var newList = listWithDuplicates.RemoveAll(x => x == 2);

        Assert.Equal(8, listWithDuplicates.Count);
        Assert.Equal(5, newList.Count);
        Assert.DoesNotContain(2, newList);
        Assert.Equal(new[] { 1, 3, 4, 5, 3 }, newList);
    }

    [Fact]
    public void ImmutableList_Contains_ShouldReturnCorrectResult()
    {
        Assert.True(_immutableList.Contains(1));
        Assert.True(_immutableList.Contains(3));
        Assert.True(_immutableList.Contains(5));
        Assert.False(_immutableList.Contains(99));
        Assert.False(_immutableList.Contains(0));
    }

    [Fact]
    public void ImmutableList_IndexOf_ShouldReturnCorrectIndex()
    {
        Assert.Equal(0, _immutableList.IndexOf(1));
        Assert.Equal(2, _immutableList.IndexOf(3));
        Assert.Equal(4, _immutableList.IndexOf(5));
        Assert.Equal(-1, _immutableList.IndexOf(99));

        var listWithDuplicates = _immutableList
            .Add(3)
            .Add(3);
        Assert.Equal(2, listWithDuplicates.IndexOf(3));
        Assert.Equal(6, listWithDuplicates.LastIndexOf(3));
    }

    [Fact]
    public void ImmutableList_GetByIndex_ShouldReturnCorrectElement()
    {
        Assert.Equal(1, _immutableList[0]);
        Assert.Equal(3, _immutableList[2]);
        Assert.Equal(5, _immutableList[4]);

        Assert.Throws<ArgumentOutOfRangeException>(() => _immutableList[10]);
        Assert.Throws<ArgumentOutOfRangeException>(() => _immutableList[-1]);
    }

    [Fact]
    public void ImmutableList_Clear_ShouldCreateNewEmptyList()
    {
        var emptyList = _immutableList.Clear();

        Assert.Equal(5, _immutableList.Count);
        Assert.Empty(emptyList);
        Assert.Equal(0, emptyList.Count);

        var newList = emptyList.Add(10).Add(20);
        Assert.Equal(2, newList.Count);
        Assert.Equal(new[] { 10, 20 }, newList);
    }

    [Fact]
    public void ImmutableList_Operations_ShouldBeDeterministic()
    {

        var list1 = ImmutableList<int>.Empty;
        var list2 = ImmutableList<int>.Empty;

        for (int i = 0; i < 5; i++)
        {
            list1 = list1.Add(i);
            list2 = list2.Add(i);
        }

        list1 = list1.Insert(3, 99);
        list2 = list2.Insert(3, 99);

        list1 = list1.Remove(2);
        list2 = list2.Remove(2);

        Assert.Equal(list1.Count, list2.Count);

        for (int i = 0; i < list1.Count; i++)
        {
            Assert.Equal(list1[i], list2[i]);
        }
    }

    [Fact]
    public void ImmutableList_AddRange_ShouldCreateNewListWithMultipleElements()
    {
        var newElements = new[] { 6, 7, 8, 9, 10 };

        var newList = _immutableList.AddRange(newElements);

        Assert.Equal(5, _immutableList.Count);
        Assert.Equal(10, newList.Count);
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, newList);
    }
}