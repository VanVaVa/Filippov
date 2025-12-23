using Xunit;

namespace CollectionsBenchmark.Tests;

public class QueueTests
{
    private readonly Queue<int> _queue;

    public QueueTests()
    {
        _queue = new Queue<int>();
        _queue.Enqueue(1);
        _queue.Enqueue(2);
        _queue.Enqueue(3);
        _queue.Enqueue(4);
        _queue.Enqueue(5);
    }

    [Fact]
    public void Queue_Enqueue_ShouldAddElementToEnd()
    {
        var initialCount = _queue.Count;

        _queue.Enqueue(6);

        Assert.Equal(initialCount + 1, _queue.Count);
        Assert.Contains(6, _queue);

        Assert.Equal(1, _queue.Peek());
    }

    [Fact]
    public void Queue_Dequeue_ShouldRemoveAndReturnFirstElement()
    {
        var initialCount = _queue.Count;
        var expectedFirst = _queue.Peek();

        var dequeued = _queue.Dequeue();

        Assert.Equal(initialCount - 1, _queue.Count);
        Assert.Equal(expectedFirst, dequeued);
        Assert.Equal(2, _queue.Peek());
        Assert.DoesNotContain(1, _queue);

        Assert.Equal(2, _queue.Dequeue());
        Assert.Equal(3, _queue.Dequeue());
        Assert.Equal(4, _queue.Dequeue());
        Assert.Equal(5, _queue.Dequeue());
        Assert.Empty(_queue);
    }

    [Fact]
    public void Queue_Peek_ShouldReturnFirstElementWithoutRemoving()
    {
        var initialCount = _queue.Count;

        var firstElement = _queue.Peek();

        Assert.Equal(1, firstElement);
        Assert.Equal(initialCount, _queue.Count);
        Assert.Equal(1, _queue.Peek());
    }

    [Fact]
    public void Queue_Contains_ShouldReturnCorrectResult()
    {
        Assert.True(_queue.Contains(1));
        Assert.True(_queue.Contains(3));
        Assert.True(_queue.Contains(5));
        Assert.False(_queue.Contains(99));
        Assert.False(_queue.Contains(0));
    }

    [Fact]
    public void Queue_Clear_ShouldRemoveAllElements()
    {
        _queue.Clear();

        Assert.Empty(_queue);
        Assert.Equal(0, _queue.Count);

        _queue.Enqueue(10);
        Assert.Single(_queue);
        Assert.Contains(10, _queue);
    }

    [Fact]
    public void Queue_ToArray_ShouldReturnElementsInFIFOOrder()
    {
        var array = _queue.ToArray();

        Assert.Equal(5, array.Length);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, array);

        Assert.Equal(5, _queue.Count);
        Assert.Equal(1, _queue.Peek());
    }

    [Fact]
    public void Queue_Operations_ShouldBeDeterministic()
    {
        var queue1 = new Queue<int>();
        var queue2 = new Queue<int>();

        for (int i = 0; i < 5; i++)
        {
            queue1.Enqueue(i);
            queue2.Enqueue(i);
        }

        while (queue1.Count > 0 && queue2.Count > 0)
        {
            Assert.Equal(queue1.Dequeue(), queue2.Dequeue());
        }

        Assert.Empty(queue1);
        Assert.Empty(queue2);
    }

    [Fact]
    public void Queue_EdgeCases_ShouldThrowAppropriateExceptions()
    {
        var emptyQueue = new Queue<int>();

        Assert.Throws<InvalidOperationException>(() => emptyQueue.Dequeue());
        Assert.Throws<InvalidOperationException>(() => emptyQueue.Peek());

        var stringQueue = new Queue<string>();
        stringQueue.Enqueue(null);
        Assert.Single(stringQueue);
        Assert.Null(stringQueue.Dequeue());
    }

    [Fact]
    public void Queue_Count_ShouldReflectCorrectNumberOfElements()
    {
        Assert.Equal(5, _queue.Count);

        _queue.Enqueue(6);
        Assert.Equal(6, _queue.Count);

        _queue.Dequeue();
        Assert.Equal(5, _queue.Count);

        _queue.Clear();
        Assert.Equal(0, _queue.Count);
    }
}