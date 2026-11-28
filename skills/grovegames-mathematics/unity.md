## Unity

- The types are in the `GroveGames.Mathematics` namespace. `GroveGames.Mathematics.Unity` holds only the integrations.
- Register the shared generator with `builder.AddRandomNumberGenerator()` in the root installer.
- An asmdef that overrides references lists `GroveGames.Mathematics.dll` in its precompiled references.
- Never use `UnityEngine.Random` or `Unity.Mathematics.Random` in game code.
- Show a `BigNumber` to the player with `BigNumberCompactFormatter` or `BigNumberPercentFormatter`, into a `stackalloc char[X.MaxLength]` buffer. Read a `BigNumber` typed in a setting or developer option with their `TryParse`, so it uses the same notation. They need GroveGames.Globalization.
- Persist a `BigNumber` as a schema member; `BigNumberFormatter` writes it. Never store it as a `double` or a string.
