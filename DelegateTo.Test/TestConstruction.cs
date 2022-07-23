using System;
using Xunit;
using DelegateTo.Test;
using DelegateTo;

namespace DelegateTo.Test;
public class TestConstruction
{
    [Fact]
    public void Test1()
    {
        var parent = new Parent { Child = new() { X = 1 } };
        Assert.Equal(1, parent.X);
        Assert.Equal(1, parent.A());
    }

}

partial class Parent
{
    [GenerateDelegate]
    public Child Child { get; set; }

    [GenerateDelegate]
    public Child2 Child2 { get; set; }
}

public partial class Parent2
{
    [GenerateDelegate]
    public Child Child { get; set; }

    [GenerateDelegate]
    public Child2 Child2 { get; set; }
}

public class Child
{
    public int X { get; set; }
    public int Y { get; }
    private int Z { get; set; }

    public int A() => 1;
}


public class Child2
{
    public int R { get; set; }
    public int Q { get; }
    private int Y { get; set; }
}
