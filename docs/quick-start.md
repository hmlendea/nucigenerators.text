# Quick start

## Choose the correct boundary

This is a .NET 10 library, not an executable. Add the package to a consuming project or reference the [library project](../NuciGenerators.Text/NuciGenerators.Text.csproj) locally. The package must be given a concrete candidate algorithm. [Build instructions](./build-and-deployment.md) describe repository commands; [API reference](./api-reference/generators.md) lists all settings.

## Deterministic consumer example

This complete example supplies a small cycling algorithm rather than relying on random choices or remote inputs. It belongs in a consuming executable, not the library project.

```csharp
using System;
using System.Collections.Generic;

using NuciGenerators.Text;

namespace DocumentationExample
{
    internal sealed class ExampleNameGenerator() : NameGenerator([])
    {
        private static readonly string[] candidates = ["Oscar", "Robert", "Ilarion"];
        private int nextCandidate;

        protected override string GenerationAlogrithm()
        {
            string candidate = candidates[nextCandidate];
            nextCandidate = (nextCandidate + 1) % candidates.Length;

            return candidate;
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            ExampleNameGenerator generator = new();
            IEnumerable<string> names = generator.Generate(2);

            foreach (string name in names)
            {
                Console.WriteLine(name);
            }

            Console.WriteLine(generator.GeneratedWords.Count);
            generator.Reset();
            Console.WriteLine(generator.GeneratedWords.Count);
        }
    }
}
```

The first batch contains `Oscar`, then `Robert`; history count is two before reset and zero afterwards. The values meet default length bounds and there are no seeds. If more distinct results are requested than this algorithm can produce, history rejection prevents duplicates and generation can consume its remaining cooperative budget. Reset does not rewind `nextCandidate`.

## Next steps

- Read [Acceptance](./behaviour/candidate-acceptance.md) before combining filters.
- Read [State](./state-and-persistence.md) before sharing wordlists or retaining generators.
- Read [Error handling](./error-handling.md) before defining an exhaustion or retry policy.
- Use [Testing](./testing.md) to reproduce the repository's deterministic queue-backed verification.
