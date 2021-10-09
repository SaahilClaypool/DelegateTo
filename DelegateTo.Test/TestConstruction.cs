using System;
using Xunit;
using DelegateTo.SourceGenerator;

namespace DelegateTo.Test
{
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
    }

    public class Child
    {
        public int X { get; set; }
        public int Y { get; }
        private int Z { get; set; }

        public int A() => 1;
    }
}
