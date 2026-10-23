# Logging

## Implemented diagnostics inventory

Inspection of [production source](../NuciGenerators.Text/) and the [manifest](../NuciGenerators.Text/NuciGenerators.Text.csproj) establishes that no logging framework or logging calls are configured. Generation returns strings or throws; it does not emit acceptance/rejection events. Console output in a README example belongs to its consuming example, not the library.

| Required diagnostic concern | Current implementation | Consumer consequence |
|-----------------------------|------------------------|----------------------|
| Framework and configuration source | None | Host selects any diagnostics stack |
| Levels | No Debug, Info, Warning, Error, or Fatal events | No runtime level switch exists |
| Structured fields | No event schema, timestamp, user identifier, or request identifier | Host defines its own non-sensitive fields |
| Correlation | No trace/span/correlation propagation | Host must correlate calls itself |
| Sinks and destinations | No console, file, database, or external sink | No sink configuration in the package |
| Redaction and masking | No logger and no masking API | Do not infer safe input disclosure; host must sanitise |
| Retention and rotation | No library logs to retain or rotate | Host policy owns any created records |
| Sampling | No sampling implementation | Host controls diagnostic volume |
| Performance | No library logging pipeline or asynchronous sink | Timing overhead comes from host instrumentation if introduced |
| Error logging | Exceptions propagate without being logged | Host can observe exceptions, not an internal error event |
| Audit logging | No separate security audit stream | History is mutable data, not a tamper-resistant audit log |
| Runtime configuration | No logging configuration or reload API | Adding logging is a new contract, not enabling a hidden option |

## Diagnostic limitations and safe instrumentation

The caller cannot directly retrieve rejection reasons, attempt counts, or a timeout-completion reason from the base result. The [test adapter](../NuciGenerators.Text.UnitTests/ScriptedNameGenerator.cs) counts algorithm calls for tests only. Instrumenting a consumer algorithm counts candidate requests but does not, by itself, explain which validation rule rejected a string.

If diagnostics are introduced, review public API compatibility, sensitive strings/patterns, synchronous timing overhead, failure propagation from sinks, and test determinism. Do not log complete seeds, generated personal names, or secrets merely to diagnose a count shortfall. See [Security mechanics](./security.md) and [Change guide](./change-guide.md).
