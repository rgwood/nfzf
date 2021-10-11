namespace nfzf.Tests;

using FluentAssertions;
using nfzf;
using Xunit;

using static nfzf.Algo;

public class AlgoTests
{
    [Fact]
    public void EmptyPatternGetsZeroResult()
    {
        (Result result, int[]? positions) = Algo.FuzzyMatchV1(caseSensitive: false,
                                                              normalize: false,
                                                              forward: true,
                                                              text: "",
                                                              pattern: "",
                                                              true);

        positions.Should().BeNull();
        result.Start.Should().Be(0);
        result.End.Should().Be(0);
        result.Score.Should().Be(0);
    }

    [Fact(Skip ="For testing+development")]
    public void BasicAssert()
    {
        int expectedScore = ScoreMatch * 3 + BonusBoundary * BonusFirstCharMultiplier + BonusCamel123 * 2 + 2 * ScoreGapStart + 2 * ScoreGapExtension;
        AssertMatch(false, true, "fooBarBaz", "fbb", 0, 7, expectedScore);
    }

    [Theory]
    [InlineData(false, true, "fooBarbaz1", "oBZ", 2, 9, ScoreMatch * 3 + BonusCamel123 + ScoreGapStart + ScoreGapExtension * 3)]
    [InlineData(false, true, "foo bar baz", "fbb", 0, 9, ScoreMatch * 3 + BonusBoundary * BonusFirstCharMultiplier + BonusBoundary * 2 + 2 * ScoreGapStart + 4 * ScoreGapExtension)]
    [InlineData(false, true, "/AutomatorDocument.icns", "rdoc",9,13, ScoreMatch * 4 + BonusCamel123 + BonusConsecutive * 2)]
    [InlineData(false, true, "/man1/zshcompctl.1", "zshc", 6, 10, ScoreMatch * 4 + BonusBoundary * BonusFirstCharMultiplier + BonusBoundary * 3)]
    [InlineData(false, true, "/.oh-my-zsh/cache", "zshc", 8, 13, ScoreMatch * 4 + BonusBoundary * BonusFirstCharMultiplier + BonusBoundary * 3 + ScoreGapStart)]
    [InlineData(false, true, "ab0123 456", "12356", 3, 10, ScoreMatch * 5 + BonusConsecutive * 3 + ScoreGapStart + ScoreGapExtension)]

    [InlineData(false, true, "abc123 456", "12356", 3, 10, ScoreMatch * 5 + BonusCamel123 * BonusFirstCharMultiplier + BonusCamel123 * 2 + BonusConsecutive + ScoreGapStart + ScoreGapExtension)]

    [InlineData(false, true, "foo/bar/baz", "fbb", 0, 9, ScoreMatch * 3 + BonusBoundary * BonusFirstCharMultiplier + BonusBoundary * 2 + 2 * ScoreGapStart + 4 * ScoreGapExtension)]

    [InlineData(false, true, "fooBarBaz", "fbb", 0, 7, ScoreMatch * 3 + BonusBoundary * BonusFirstCharMultiplier + BonusCamel123 * 2 + 2 * ScoreGapStart + 2 * ScoreGapExtension)]
    [InlineData(false, true, "foo barbaz", "fbb", 0, 8, ScoreMatch * 3 + BonusBoundary * BonusFirstCharMultiplier + BonusBoundary +
                    ScoreGapStart * 2 + ScoreGapExtension * 3)]

    [InlineData(false, true, "fooBar Baz", "foob", 0, 4, ScoreMatch * 4 + BonusBoundary * BonusFirstCharMultiplier + BonusBoundary * 3)]

    [InlineData(false, true, "xFoo-Bar Baz", "foo-b", 1, 6, ScoreMatch * 5 + BonusCamel123 * BonusFirstCharMultiplier + BonusCamel123 * 2 +
                    BonusNonWord + BonusBoundary)]

    [InlineData(true, true, "fooBarbaz", "oBz", 2, 9, ScoreMatch * 3 + BonusCamel123 + ScoreGapStart + ScoreGapExtension * 3)]
    [InlineData(true, true, "Foo/Bar/Baz", "FBB", 0, 9, ScoreMatch * 3 + BonusBoundary * (BonusFirstCharMultiplier + 2) + ScoreGapStart * 2 + ScoreGapExtension * 4)]
    [InlineData(true, true, "FooBarBaz", "FBB", 0, 7, ScoreMatch * 3 + BonusBoundary * BonusFirstCharMultiplier + BonusCamel123 * 2 +
                    ScoreGapStart * 2 + ScoreGapExtension * 2)]
    [InlineData(true, true, "FooBar Baz", "FooB", 0, 4, ScoreMatch * 4 + BonusBoundary * BonusFirstCharMultiplier + BonusBoundary * 2 + BonusBoundary)]
    // Consecutive bonus updated
    [InlineData(true, true, "foo-bar", "o-ba", 2, 6, ScoreMatch * 4 + BonusBoundary * 3)]
    // Non-match
    [InlineData(true, true, "fooBarbaz", "oBZ", -1, -1, 0)]
    [InlineData(true, true, "Foo Bar Baz", "fbb", -1, -1, 0)]
    [InlineData(true, true, "fooBarbaz", "fooBarbazz", -1, -1, 0)]

    public void AssertMatch(
        bool caseSensitive, bool forward, string input, string pattern, int sidx, int eidx, int expectedScore)
    {
        if (!caseSensitive)
            pattern = pattern.ToLower();

        (Result result, int[]? positions) = Algo.FuzzyMatchV1(caseSensitive: caseSensitive,
                                                      normalize: false,
                                                      forward: forward,
                                                      input,
                                                      pattern,
                                                      withPos: true);

        int start, end;

        if(positions is null || positions.Length == 0)
        {
            start = result.Start;
            end = result.End;
        } 
        else
        {
            Array.Sort(positions);
            start = positions[0];
            end = positions.Last() + 1;
        }

        start.Should().Be(sidx);
        end.Should().Be(eidx);

        result.Score.Should().Be(expectedScore);
    }

    [Fact]
    public void AssertPositions()
    {
        (Result _, int[]? positions) = Algo.FuzzyMatchV1(caseSensitive: false,
                                                      normalize: false,
                                                      forward: true,
                                                      "fooBarBaz",
                                                      "fbb",
                                                      withPos: true);

        positions!.Length.Should().Be(3);
        // TODO: are there any situations where positions wouldn't be sorted? this will break if so
        positions[0].Should().Be(0);
        positions[1].Should().Be(3);
        positions[2].Should().Be(6);
    }


    [Fact]
    public void TestAsciiFuzzyIndex()
    {
        trySkip("fooBarbaz", true, 'z', 9).Should().Be(-1);
        asciiFuzzyIndex("fooBarbaz", "fooBarbazz", true).Should().Be(-1);
    }

}
