using Xunit;

namespace CollectionsBenchmark.Tests;

public class StackTests
{
    private readonly Stack<int> _stack;

    public StackTests()
    {
        _stack = new Stack<int>();
        _stack.Push(1);
        _stack.Push(2);
        _stack.Push(3);
        _stack.Push(4);
        _stack.Push(5);
    }

    [Fact]
    public void Stack_Push_ShouldAddElementToTop()
    {
        var initialCount = _stack.Count;

        _stack.Push(6);

        Assert.Equal(initialCount + 1, _stack.Count);
        Assert.Equal(6, _stack.Peek());
        Assert.Contains(6, _stack);
    }

    [Fact]
    public void Stack_Pop_ShouldRemoveAndReturnTopElement()
    {
        var initialCount = _stack.Count;
        var expectedTop = _stack.Peek();

        var popped = _stack.Pop();

        Assert.Equal(initialCount - 1, _stack.Count);
        Assert.Equal(expectedTop, popped);
        Assert.Equal(4, _stack.Peek());
        Assert.DoesNotContain(5, _stack);

        Assert.Equal(4, _stack.Pop());
        Assert.Equal(3, _stack.Pop());
        Assert.Equal(2, _stack.Pop());
        Assert.Equal(1, _stack.Pop());
        Assert.Empty(_stack);
    }

    [Fact]
    public void Stack_Peek_ShouldReturnTopElementWithoutRemoving()
    {
        var initialCount = _stack.Count;

        var topElement = _stack.Peek();

        Assert.Equal(5, topElement);
        Assert.Equal(initialCount, _stack.Count);
        Assert.Equal(5, _stack.Peek());
    }

    [Fact]
    public void Stack_Contains_ShouldReturnCorrectResult()
    {
        Assert.True(_stack.Contains(1));
        Assert.True(_stack.Contains(3));
        Assert.True(_stack.Contains(5));
        Assert.False(_stack.Contains(99));
        Assert.False(_stack.Contains(0));
    }

    [Fact]
    public void Stack_Clear_ShouldRemoveAllElements()
    {
        _stack.Clear();

        Assert.Empty(_stack);
        Assert.Equal(0, _stack.Count);

        _stack.Push(10);
        Assert.Single(_stack);
        Assert.Contains(10, _stack);
    }

    [Fact]
    public void Stack_ToArray_ShouldReturnElementsInLIFOOrder()
    {
        var array = _stack.ToArray();

        Assert.Equal(5, array.Length);
        Assert.Equal(new[] { 5, 4, 3, 2, 1 }, array);

        Assert.Equal(5, _stack.Count);
        Assert.Equal(5, _stack.Peek());
    }

    [Fact]
    public void Stack_Operations_ShouldBeDeterministic()
    {
        var stack1 = new Stack<int>();
        var stack2 = new Stack<int>();

        for (int i = 0; i < 5; i++)
        {
            stack1.Push(i);
            stack2.Push(i);
        }

        while (stack1.Count > 0 && stack2.Count > 0)
        {
            Assert.Equal(stack1.Pop(), stack2.Pop());
        }

        Assert.Empty(stack1);
        Assert.Empty(stack2);
    }

    [Fact]
    public void Stack_EdgeCases_ShouldThrowAppropriateExceptions()
    {
        var emptyStack = new Stack<int>();

        Assert.Throws<InvalidOperationException>(() => emptyStack.Pop());
        Assert.Throws<InvalidOperationException>(() => emptyStack.Peek());

        var stringStack = new Stack<string>();
        stringStack.Push(null);
        Assert.Single(stringStack);
        Assert.Null(stringStack.Pop());
    }

    [Fact]
    public void Stack_Count_ShouldReflectCorrectNumberOfElements()
    {
        Assert.Equal(5, _stack.Count);

        _stack.Push(6);
        Assert.Equal(6, _stack.Count);

        _stack.Pop();
        Assert.Equal(5, _stack.Count);

        _stack.Clear();
        Assert.Equal(0, _stack.Count);
    }

    [Fact]
    public void Stack_TryPeek_ShouldReturnTrueWhenStackHasElements()
    {
        var emptyStack = new Stack<int>();

        bool result = _stack.TryPeek(out int top);
        Assert.True(result);
        Assert.Equal(5, top);
        Assert.Equal(5, _stack.Count);

        result = emptyStack.TryPeek(out int emptyTop);
        Assert.False(result);
        Assert.Equal(0, emptyTop);
    }

    [Fact]
    public void Stack_TryPop_ShouldReturnTrueWhenStackHasElements()
    {
        var emptyStack = new Stack<int>();

        bool result = _stack.TryPop(out int popped);
        Assert.True(result);
        Assert.Equal(5, popped);
        Assert.Equal(4, _stack.Count);

        result = emptyStack.TryPop(out int emptyPopped);
        Assert.False(result);
        Assert.Equal(0, emptyPopped);
    }
}