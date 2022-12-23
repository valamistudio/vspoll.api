using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using VSPoll.API.Extensions;
using Xunit;

#pragma warning disable CA1806 // Do not ignore method results
namespace VSPoll.API.UnitTest.Extensions;

[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
public class CollectionsTests
{
    [Fact]
    public void AtLeast_Zero_ShouldAlwaysReturnTrue()
        => Enumerable.Empty<object>().AtLeast(0).Should().BeTrue();

    [Fact]
    public void AtLeast_Negative_ShouldThrowArgumentException()
        => new Action(() => Enumerable.Empty<object>().AtLeast(-1)).Should().Throw<ArgumentOutOfRangeException>();

    [Fact]
    public void AtLeast_One_ShouldReturnFalseForEmpty()
        => Enumerable.Empty<object>().AtLeast(1).Should().BeFalse();

    [Fact]
    public void AtLeast_One_ShouldReturnTrueForOne()
        => new[] { new object() }.AtLeast(1).Should().BeTrue();

    [Fact]
    public void AtLeast_One_ShouldReturnTrueForTwo()
        => new[] { new object(), new object() }.AtLeast(1).Should().BeTrue();

    [Fact]
    public void Count_Zero_ShouldAlwaysReturnTrue()
        => Enumerable.Empty<object>().Count(0).Should().BeTrue();

    [Fact]
    public void Count_Negative_ShouldThrowArgumentException()
        => new Action(() => Enumerable.Empty<object>().Count(-1)).Should().Throw<ArgumentOutOfRangeException>();

    [Fact]
    public void Count_One_ShouldReturnFalseForEmpty()
        => Enumerable.Empty<object>().Count(1).Should().BeFalse();

    [Fact]
    public void Count_One_ShouldReturnTrueForOne()
        => new[] { new object() }.Count(1).Should().BeTrue();

    [Fact]
    public void Count_One_ShouldReturnFalseForTwo()
        => new[] { new object(), new object() }.Count(1).Should().BeFalse();

    [Fact]
    public void Count_Two_ShouldReturnFalseForOne()
        => new[] { new object() }.Count(2).Should().BeFalse();

    [Fact]
    public void AppendAll_Empty_ShouldReturnEmptyString()
        => Array.Empty<char>().AppendAll().Should().BeEmpty();

    [Fact]
    public void AppendAll_TestScenario1()
    {
        var chars = new[] { 'f', 'o', 'o' };
        var ret = chars.AppendAll();
        ret.Should().HaveLength(chars.Length);
        for (var i = 0; i < chars.Length; i++)
            ret[i].Should().Be(chars[i]);
    }
}
