namespace nfzf.Tests;

using FluentAssertions;
using nfzf;
using System.Diagnostics;
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

    [Fact(Skip ="playground")]
    public void BasicAssert()
    {
        int expectedScore = ScoreMatch * 3 + BonusCamel123 + ScoreGapStart + ScoreGapExtension * 3;
        AssertMatch(false, true, "fooBarbaz1", "oBZ", 2, 9, expectedScore);
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

    [InlineData(false, true, "", "", 0, 0, 0)]



    public void AssertMatch(
        bool caseSensitive, bool forward, string input, string pattern, int sidx, int eidx, int expectedScore)
    {
        Debug.WriteLine(input);
        Debug.WriteLine(pattern);
        if (!caseSensitive)
            pattern = pattern.ToLower();

        (Result result, int[]? positions) = Algo.FuzzyMatchV1(caseSensitive: caseSensitive,
                                                      normalize: false,
                                                      forward: forward,
                                                      input,
                                                      pattern,
                                                      withPos: true);

        result.Score.Should().Be(expectedScore);
    }
}
