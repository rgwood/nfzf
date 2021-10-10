# nfzf (WIP)

[fzf](https://github.com/junegunn/fzf) is really good at fuzzy-finding. This library ports the core fzf algorithms to .NET.

## Status

This is a very early WIP and it currently only includes fzf's v1 algorithm. Expect breaking changes.

Remaining known work:

- Decide how we're gonna expose the fzf interface.
- Test+use the `positions` array
- Publish to NuGet
- Write benchmarks
- Port remaining fzf tests
- Understand fzf's Unicode handling and normalization better, then port it