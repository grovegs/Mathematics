# GroveGames.Mathematics

[![Build Status](https://github.com/grovegs/Mathematics/actions/workflows/release.yml/badge.svg)](https://github.com/grovegs/Mathematics/actions/workflows/release.yml)
[![Tests](https://github.com/grovegs/Mathematics/actions/workflows/tests.yml/badge.svg)](https://github.com/grovegs/Mathematics/actions/workflows/tests.yml)
[![NuGet](https://img.shields.io/nuget/v/GroveGames.Mathematics)](https://www.nuget.org/packages/GroveGames.Mathematics)
[![Latest Release](https://img.shields.io/github/v/release/grovegs/Mathematics)](https://github.com/grovegs/Mathematics/releases/latest)

Deterministic random numbers, big numbers and shuffle bags for .NET and Unity

## Features

- `IRandomNumberGenerator` and `RandomNumberGenerator`: a seeded generator that returns the same sequence for the same seed on every platform
- `BigNumber`: a mantissa and exponent value for idle-game numbers far beyond the range of `double`
- `ShuffleBag`: draws every index once per round and never the same index twice in a row
- Test doubles for your own tests: `FakeRandomNumberGenerator` in the editor-only `GroveGames.Mathematics.Unity.Testing` assembly

## .NET

### Installation

```bash
dotnet add package GroveGames.Mathematics
```

### Usage

```csharp
using GroveGames.Mathematics;

var random = new RandomNumberGenerator(12345u);
var roll = random.NextInt(1, 6);
var chance = random.NextFloat();

var bag = new ShuffleBag(4);
bag.Reset(4);
bag.TryDraw(random, out var index);

var cost = new BigNumber(1.5d, 400L);
var total = cost * BigNumber.Pow(1.15d, 50d);
BigNumber.TryParse("2.5e6", out var parsed);
```

Integer ranges include both ends: `NextInt(10)` returns 0 to 10, and `NextInt(1, 6)` returns 1 to 6. Any range up to the full 32 bits works. A negative `max`, or a `max` below `min`, throws `ArgumentOutOfRangeException`. Float and double ranges exclude the maximum, though rounding can rarely return it, so clamp a result you use as an index. A seed must not be zero. Generators created without a seed in the same instant still get different seeds.

## Unity

### Installation

1. Install the core package via [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity):
   - Open Window > NuGet > Manage NuGet Packages
   - Search for `GroveGames.Mathematics` and install

2. Add the Unity package via Git URL:
   - Open Package Manager (Window > Package Manager)
   - Click "+" > "Add package from git URL"
   - Enter: `https://github.com/grovegs/Mathematics.git?path=src/GroveGames.Mathematics.Unity/Packages/com.grovegames.mathematics`

3. Create a `csc.rsp` file in your `Assets/` directory with the following content to enable C# 10 features:

   ```text
   -langversion:10
   -nullable:enable
   ```

### Usage

The types live in the core namespace. The Unity package adds the integrations.

```csharp
using GroveGames.Mathematics;
using GroveGames.Mathematics.Unity;

builder.AddRandomNumberGenerator();
```

- With [GroveGames.DependencyInjection](https://github.com/grovegs/DependencyInjection) installed, `AddRandomNumberGenerator()` registers `IRandomNumberGenerator` as a singleton.
- With [GroveGames.Serialization](https://github.com/grovegs/Serialization) installed, `BigNumberFormatter` serializes `BigNumber` as its mantissa and exponent.
- With [GroveGames.Globalization](https://github.com/grovegs/Globalization) installed, `BigNumberCompactFormatter` and `BigNumberPercentFormatter` write a `BigNumber` as `1.5aa` or `190M%`, and their `TryParse` reads that text back.

## Development

### Project Structure

- `src/GroveGames.Mathematics/` - Core .NET library (`net10.0`, `netstandard2.1`)
- `src/GroveGames.Mathematics.Unity/` - Unity package
- `tests/` - xUnit tests
- `sandbox/` - Sample applications for testing

### Commands

```bash
dotnet build                        # Build all projects
dotnet format                       # Format code
dotnet test                         # Run tests
dotnet pack -c Release              # Create NuGet packages
```

### Requirements

- .NET 10.0 SDK

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
