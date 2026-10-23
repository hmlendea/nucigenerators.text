# API reference index

## Applicability

This repository exposes a CLR library API, not HTTP controllers. There are no endpoints to catalogue, controller files to generate, authentication schemes, response headers, HTTP error codes, API route versions, or request-throttling policies. Generating controller documentation would invent capabilities absent from [production source](../../NuciGenerators.Text/).

## Contract catalogue

| Public contract | Principal methods or fields | Reference | Flow |
|-----------------|-----------------------------|-----------|------|
| `NuciGenerators.Text.INameGenerator` | Settings, collections, `Generate(int)`, `Reset()` | [Generator API](./generators.md) | [Generation](../flows/generation.md) |
| `NuciGenerators.Text.NameGenerator` | Interface implementation, `OnlyNewNames`, protected algorithm/validation/random seams | [Generator API](./generators.md) | [Validation](../flows/validation.md) |
| `NuciGenerators.Text.Models.Word` | `Id`, `Values` | [Model API](./models.md) | [Mutation](../flows/mutation-and-reset.md) |
| `NuciGenerators.Text.Models.Wordlist` | Constructors, `Values`, `Add`, `Get`, enumeration | [Model API](./models.md) | [Mutation](../flows/mutation-and-reset.md) |
| `NuciGenerators.Text.Models.GenerationSchema` | Six metadata properties, constructor, equality/hash methods | [Model API](./models.md) | No base-generator execution flow |
| `NuciGenerators.Text.Models.WordCase` | `Original`, `Lower`, `Upper`, `Title`, `Sentence` | [Model API](./models.md) | Metadata only |

## Versioning and limits

The [manifest](../../NuciGenerators.Text/NuciGenerators.Text.csproj) declares version 1.0.1 and target `net10.0`; no explicit semantic-versioning or deprecation procedure is implemented by source. The [Security policy](../../SECURITY.md) separately describes supported distributions. The batch clock budget is a cooperative execution limit, not a rate limiter. There is no cancellation or network protocol version.

See [Usage examples](../api-usage-examples.md), [Data model](../data-model.md), [Generation component](../components/generation-engine.md), and [Errors](../error-handling.md). Full namespaces identify declarations; inherited BCL object members are not separately redefined here.
