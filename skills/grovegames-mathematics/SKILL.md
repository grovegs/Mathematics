---
name: grovegames-mathematics
description: Rules for random numbers, big numbers and shuffle bags in game code with GroveGames.Mathematics. Use when writing or reviewing code that rolls a random value, picks a random item, seeds a generator, draws from a ShuffleBag, or stores, compares or formats a BigNumber.
---

# Using GroveGames.Mathematics

Rules for game code that uses this package. They hold in every game. The API is in the package's `README.md`.

- Take `IRandomNumberGenerator` through the constructor. Never use `System.Random` or an engine's random API in game code.
- Integer ranges include both ends: `NextInt(max)` returns 0 to `max` and `NextInt(min, max)` returns `min` to `max`. To pick an index, call `NextInt(count - 1)`, and check that `count` is not zero first.
- Float and double ranges exclude the maximum.
- For content that must come out the same every time, such as a generated level, create a `RandomNumberGenerator` with a seed derived from the content's id, such as `layerIndex + 1`. Small and sequential seeds are fine: the generator mixes the seed itself, so do not hash it first. The same seed gives the same sequence on every platform. Never seed with zero; it throws.
- Tests create `new RandomNumberGenerator(seed)` with a fixed seed instead of a fake generator.
- Use `ShuffleBag` to pick from a small set without repeating the last pick. Call `Reset(count)` when the set changes, and treat the drawn value as an index.
- Use `BigNumber` for values that can outgrow `double`, such as idle-game currency, cost and damage. Do not convert it to `double` to compare or add; use its operators, `Max`, `Min` and `Pow`.
- Dividing a `BigNumber` by zero returns `BigNumber.Zero`, and so does a `NaN` or infinite input. Check `IsZero` where that matters.
