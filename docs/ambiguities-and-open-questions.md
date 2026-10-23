# Ambiguities and open questions

## Confirmed source-versus-documentation differences

| Finding | Evidence | Consequence and disposition |
|---------|----------|-----------------------------|
| README source-build commands name a removed solution file | [README](../README.md) names a `.sln`; tracked [solution](../NuciGenerators.Text.slnx) is `.slnx` | Use verified commands in [Build](./build-and-deployment.md); original root prose not comprehensively rewritten by this documentation task |
| Personal-data claims exceed what source can establish | [README](../README.md) states no personal-data processing; [Privacy](../PRIVACY.md) uses broad claims, while caller strings are unrestricted | The code does process whatever strings the consumer supplies; no built-in transmission/persistence is narrower than no personal-data processing. Maintainer policy revision requires careful scope confirmation |
| Privacy prose suggests schema-based generation | [Privacy](../PRIVACY.md) refers to strings based on schemas; [base generator](../NuciGenerators.Text/NameGenerator.cs) never consumes schema metadata | Document metadata-only role; do not invent a grammar engine |
| Root architecture could suggest all constructor inputs are unmaterialised aliases | [Architecture](../ARCHITECTURE.md) summarises non-copying; [Wordlist](../NuciGenerators.Text/Models/Wordlist.cs) actually materialises source membership | Preserve distinction: outer generator list aliased, wordlist membership copied, word objects retained |
| Casing mode comments are ambiguous | [Enum](../NuciGenerators.Text/Models/WordCase.cs) describes first-word versus every-word capitalisation | No implementation resolves labels. Confirm intended semantics before adding transformation |

## Consequential implementation gaps

| Area | Observed fact | Unknown or unverified |
|------|---------------|----------------------|
| Derived schema equality | Nonsealed class; typed overload compares fields, object overload checks runtime type | Whether differing overload results for inheritance are intentional; no derived-schema tests |
| Deadline arithmetic | Integer multiplication, wall-clock comparison, no validation/pre-emption | Desired overflow/zero-budget/clock-adjustment policy; no dedicated tests |
| Null configuration | Public properties accept runtime null and collections are mutable | Desired validation/failure contract for all null branches; not comprehensively tested |
| Enumerable inputs | Arbitrary `IEnumerable<string>` consumed during seed checks | Desired policy for side effects, repeated/throwing enumeration, or concurrent mutation |
| Concurrency/re-entrancy | No synchronisation or re-entrancy guard | No supported parallel-use contract or stress tests |
| Interface/base mismatch | `OnlyNewNames` exists only on base | Whether future interface exposure is desired; a new interface member affects independent implementers |
| Dependency internals | NuciExtensions 5.3.1 call site present | Matching-version dependency source not inspected; do not infer internals from package naming |
| Publication | Package metadata and supported channels are documented; CI only validates | Actual release/push automation, registry state, and hosted security-advisory availability not independently verified |
| Performance | Repeated scans and regex work are visible | No benchmark, memory limit, capacity recommendation, or adversarial regex guarantee |
| Serialisation | No serializer or format configured | Whether downstream packages depend on names, enum numbers, or annotations as wire contracts |

## Not repository capabilities

There are no controllers, browser state, calendar flows, localisation resources, repositories/ORM adapters, background jobs, authentication, or runtime logging. Their absence is an applicability decision, not unfinished documentation. No duplicate root architecture, roadmap, privacy policy, security notice, or licence is generated under this directory.

## Resolution procedure

Use [Change guide](./change-guide.md) to turn a confirmed intended change into a tested contract. Record maintainer decisions and evidence dates here; remove an ambiguity only when source/tests or verified policy resolve it. Do not retroactively mark inferred intent as historical fact.
