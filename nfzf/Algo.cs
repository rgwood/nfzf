using System.Text;

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


    // func FuzzyMatchV1(caseSensitive bool, normalize bool, forward bool, text *util.Chars, pattern []rune, withPos bool, slab *util.Slab) (Result, *[]int) {

    public static (Result Result, int[]? Positions) FuzzyMatchV1(bool caseSensitive, bool normalize, bool forward,
        string text, string pattern, bool withPos)
    {
        if (string.IsNullOrEmpty(pattern))
            return (new Result(0, 0, 0), null);

        // I think this checks for invalid strings? TODO: what exactly is invalid?
        if (asciiFuzzyIndex(text, pattern, caseSensitive) < 0)
            return (new Result(-1, -1, 0), null);

        int patternIdx = 0;
        int startIdx = -1;
        int endIdx = -1;

        // TODO: fzf expects runes here but we are using chars 😱
        int lenRunes = text.Length;
        int lenPattern = pattern.Length;


        // TODO: fzf has an optimization here instead of lowering the whole string, should we port it? 
        if(!caseSensitive)
            text = text.ToLowerInvariant();


        for (int index = 0; index < lenRunes; index++)
        {
            char c = text[indexAt(index, lenRunes, forward)];

            // TODO not sure what normalization is here
            if (normalize)
            {
            }

            char pchar = pattern[indexAt(patternIdx, lenPattern, forward)];

            if (c == pchar)
            {
                if (startIdx < 0)
                    startIdx = index;
                patternIdx++;
                if(patternIdx == lenPattern)
                {
                    endIdx = index + 1;
                    break;
                }
            }
        }

        // go back
        if (startIdx >= 0 && endIdx >= 0)
        {
            patternIdx--;

            for (int index = endIdx - 1; index >= startIdx; index--)
            {
                int tidx = indexAt(index, lenRunes, forward);
                char c = text[tidx];

                int patternIdx_ = indexAt(patternIdx, lenPattern, forward);
                char pchar = pattern[patternIdx_];

                if(c == pchar)
                {
                    patternIdx--;
                    if(patternIdx < 0)
                    {
                        startIdx = index;
                        break;
                    }
                }
            }

            if(!forward)
            {
                startIdx = lenRunes - endIdx;
                endIdx = lenRunes - startIdx;
            }

            var r = CalculateScore(caseSensitive, normalize, text, pattern, startIdx, endIdx, withPos);
            return new(new(startIdx, endIdx, r.score), r.pos);
        }

        Result res = new(0, 0, 0);
        return new(res, new int[0]);
    }

    private static int indexAt(int index, int max, bool forward) => forward ? index : max - index - 1;

    private static int asciiFuzzyIndex(string input, string pattern, bool caseSensitive)
    {
        // TODO implement
        return 0;
        //throw new NotImplementedException();
    }

    // func calculateScore(caseSensitive bool, normalize bool, text *util.Chars, pattern []rune, sidx int, eidx int, withPos bool) (int, *[]int) {
    public static (int score, int[]? pos) CalculateScore(bool caseSensitive, bool normalize, 
        string text, string pattern, int startIdx, int endIdx, bool withPos)
    {
        int pidx = 0;
        int score = 0;
        bool inGap = false;
        int consecutive = 0;
        int firstBonus = 0;

        List<int> pos = new();
        CharClass prevClass = CharClass.charNonWord;

        if (startIdx > 0)
            prevClass = charClassOf(text[startIdx - 1]);

        for (int idx = startIdx; idx < endIdx; idx++)
        {
            char c = text[idx];
            var c_class = charClassOf(c);


            if (!caseSensitive)
            {
                c = char.ToLowerInvariant(c);
            }

            if (normalize)
            {
                // TODO
            }

            if (c == pattern[pidx])
            {
                if(withPos)
                    pos.Add(idx);

                score += ScoreMatch;
                int bonus = bonusFor(prevClass, c_class);

                if (consecutive == 0)
                {
                    firstBonus = bonus;
                }
                else
                {
                    // Break consecutive chunk
                    if(bonus == BonusBoundary)
                    {
                        firstBonus = bonus;
                    }
                    bonus = Math.Max(Math.Max(bonus, firstBonus), BonusConsecutive);
                }

                if (pidx == 0)
                    score += bonus * BonusFirstCharMultiplier;
                else
                    score += bonus;

                inGap = false;
                consecutive++;
                pidx++;
            }
            else
            {
                if (inGap)
                    score += ScoreGapExtension;
                else
                    score += ScoreGapStart;

                inGap = true;
                consecutive = 0;
                firstBonus = 0;
            }
            prevClass = c_class;
        }

        int[]? returnPos = withPos ? pos.ToArray() : null;

        return (score, returnPos);
    }

    private static int bonusFor(CharClass prevClass, CharClass c_class)
    {
        if(prevClass == CharClass.charNonWord && c_class != CharClass.charNonWord)
        {
            // word boundary
            return BonusBoundary;
        } 
        else if (prevClass == CharClass.charLower && c_class == CharClass.charUpper 
            ||
                 prevClass != CharClass.charNumber && c_class == CharClass.charNumber)
        {
            // camelCase letter123
            return BonusCamel123;
        }
        else if (c_class == CharClass.charNonWord)
        {
            return BonusNonWord;
        }

        return 0;
    }

    private static CharClass charClassOf(char c)
    {
        // TODO: fzf has separate branches for Ascii and non-Asii (probably for perf), try benchmarking
        if (char.IsLower(c))
            return CharClass.charLower;
        if (char.IsUpper(c))
            return CharClass.charUpper;
        if (char.IsNumber(c))
            return CharClass.charNumber;
        if (char.IsLetter(c))
            return CharClass.charLetter;
        return CharClass.charNonWord;
    }

    private static int[]? posArray(bool withPos, int length)
    {
        return withPos ? new int[length] : null;
    }

    public enum CharClass
    {
        charNonWord, charLower, charUpper, charLetter, charNumber
    }
}
