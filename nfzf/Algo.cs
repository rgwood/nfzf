namespace nfzf;

// Result contains the results of running a match function.
public record Result(int Start, int End, int Score);

public static class Algo
{
    public const int ScoreMatch = 16;
    public const int ScoreGapStart = -3;
    public const int ScoreGapExtension = -1;

    // We prefer matches at the beginning of a word, but the bonus should not be
    // too great to prevent the longer acronym matches from always winning over
    // shorter fuzzy matches. The bonus point here was specifically chosen that
    // the bonus is cancelled when the gap between the acronyms grows over
    // 8 characters, which is approximately the average length of the words found
    // in web2 dictionary and my file system.
    public const int BonusBoundary = ScoreMatch / 2;

    // Although bonus point for non-word characters is non-contextual, we need it
    // for computing bonus points for consecutive chunks starting with a non-word
    // character.
    public const int BonusNonWord = ScoreMatch / 2;

    // Edge-triggered bonus for matches in camelCase words.
    // Compared to word-boundary case, they don't accompany single-character gaps
    // (e.g. FooBar vs. foo-bar), so we deduct bonus point accordingly.
    public const int BonusCamel123 = BonusBoundary + ScoreGapExtension;

    // Minimum bonus point given to characters in consecutive chunks.
    // Note that bonus points for consecutive matches shouldn't have needed if we
    // used fixed match score as in the original algorithm.
    public const int BonusConsecutive = -(ScoreGapStart + ScoreGapExtension);

    // The first character in the typed pattern usually has more significance
    // than the rest so it's important that it appears at special positions where
    // bonus points are given, e.g. "to-go" vs. "ongoing" on "og" or on "ogo".
    // The amount of the extra bonus should be limited so that the gap penalty is
    // still respected.
    public const int BonusFirstCharMultiplier = 2;


    // Algo functions make two assumptions
    // 1. "pattern" is given in lowercase if "caseSensitive" is false
    // 2. "pattern" is already normalized if "normalize" is true
    // type Algo func(caseSensitive bool, normalize bool, forward bool, input* util.Chars, pattern[] rune, withPos bool, slab* util.Slab) (Result, *[]int)

    // R: idk what the *[]int return value is
    // It's a pointer that only gets returned if withPos=true. But why? Result should already include that
    // A: seems like an array of pointers to all match locations.
}
