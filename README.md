# nfzf (WIP)

[![build and test](https://github.com/rgwood/nfzf/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/rgwood/nfzf/actions/workflows/build-and-test.yml)

[fzf](https://github.com/junegunn/fzf) is really good at fuzzy-finding. This library ports the core fzf algorithms to .NET.

## Status

This is a very early WIP and it currently only includes fzf's v1 algorithm. Expect breaking changes.

Remaining known work:

- Decide exactly how we're gonna expose both fzf algorithms to consumers. Follow fzf closely or do something more idiomatic to .NET?
- Port fzf's v2 algorithm
- Test+use the `positions` array
- Publish to NuGet
- Write benchmarks
- Port remaining fzf tests
- Understand fzf's Unicode handling and normalization better, then port it
