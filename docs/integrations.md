# Integrations

## Runtime versus development

| Boundary | Direction and exchanged data | Owner | Failure consequence |
|----------|------------------------------|-------|---------------------|
| Caller to library | Settings, wordlist references, generation/reset calls | Consumer | Invalid configuration can reject candidates or throw |
| Library to subclass | Synchronous candidate request | Consumer implementation | Exception aborts batch without undoing prior history |
| Library to seed enumerable | Repeated enumeration and candidate membership queries | Consumer-provided values | Enumeration failure propagates; enabled only when seed exclusion is on |
| Schema to NuciExtensions | Runtime-type comparison call | Production dependency | No translation at call site; see [Dependencies](./dependencies.md) |
| Build tools to package feeds | Restore declared packages | Build environment | Restore failure blocks build/test, not an already installed generator invocation |
| GitHub Actions | Repository checkout and solution restore/build/test | CI platform and repository workflow | CI job failure; no runtime failover or package publication configured |

There is no built-in HTTP client, email provider, identity service, remote storage, telemetry sender, update checker, or crash-report destination. A consumer subclass can introduce any of these; the base class neither configures nor restricts that code. `FilterlistPath` is not an integration until a consumer interprets it.

## HTTP API applicability

No controller, route, request DTO, authentication scheme, HTTP status mapping, or rate-limiting middleware exists. The [API index](./api-reference/INDEX.md) documents CLR contracts instead of inventing endpoints. [Usage examples](./api-usage-examples.md) consequently show method calls and in-memory results, not HTTP request/response bodies.

For external dependency version limits and uninspected implementations, use [Dependencies](./dependencies.md). For public vulnerability reporting, use the root [Security policy](../SECURITY.md), not a public issue containing exploit details.
