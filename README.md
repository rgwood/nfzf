# nfzf (WIP)

[![build and test](https://github.com/rgwood/nfzf/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/rgwood/nfzf/actions/workflows/build-and-test.yml)

[fzf](https://github.com/junegunn/fzf) is really good at fuzzy-finding. This library ports the core fzf algorithms to .NET.

## Status

This is a very early WIP and it currently only includes fzf's v1 algorithm. Expect breaking changes.

Remaining known work:

- Decide exactly how we're gonna expose fzf's multiple algorithms to consumers. Follow fzf closely or do something more idiomatic to .NET (interfaces maybe)?
- Understand fzf's Unicode handling and normalization better, then port it. This may involve [Rune](https://docs.microsoft.com/en-us/dotnet/api/system.text.rune?view=net-5.0#rune-in-net-vs-other-languages) which is not present in older .NET versions
- Figure out exactly which TFMs to support. Probably multitarget .NET Standard 2.0 (for reach) and .NET 6 (for nullable reference types and maybe Rune) 
- Publish to NuGet
- Write benchmarks
- Port remaining fzf tests
- Port fzf's v2 algorithm
- Port fzf's other specialty algorithms (PrefixMatch, SuffixMatch, ExactMatchNaive, EqualMatch) if they look useful
