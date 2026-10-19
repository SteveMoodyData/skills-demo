# Skills Standards & Deterministic Output Rules
This document defines global standards used by all skills in this repository.
These standards exist to ensure:
- Deterministic outputs
- Cross-repo stitching
- No hallucination
- Auditability
- Team consistency
All skills MUST comply with these rules.
---
# 1. No Hallucination Rule (Global)
The model must:
- Analyze ONLY the repository and files present in the selected scope.
- Never use external knowledge.
- Never assume implementation patterns.
- Never infer behavior without evidence.
- Never fabricate endpoints, schedules, transformations, Oracle procedures/tables, interfaces, config values, auth methods, or business meaning.
If evidence is missing, use:
- NOT FOUND IN CODE → artifact not present in repo
- INSUFFICIENT EVIDENCE → ambiguous or partially supported
Never silently omit missing artifacts.
Never guess.
---
# 2. Evidence Standards
Every material claim must include evidence.
Accepted evidence formats:
Line Mode:
repo/path/file.ext:L###
Anchor Mode:
file.ext — unique anchor snippet
Each artifact must include evidence unless explicitly marked:
- NOT FOUND IN CODE
- INSUFFICIENT EVIDENCE
If no evidence can be provided:
- The artifact must not be included
- Or must be explicitly marked as insufficient
---
# 3. Deterministic Key Standards
Keys exist to ensure:
- Repeatability
- Diff stability
- Cross-repo traceability
- Audit alignment
All key generation must be deterministic.
---
## 3.1 FLOW Keys (Service-Prefixed, Globally Unique)
A FLOW represents a distinct end-to-end ingestion path.
### FLOW Key Format
<SERVICE_KEY>-FLOW-###
Examples:
- WEATHER-FLOW-001
- METER-FLOW-002
- BILLING-FLOW-001
### SERVICE_KEY Rules
SERVICE_KEY must be deterministic and stable:
- Prefer the repo folder name (workspace root folder name) if available.
- Otherwise prefer solution or project name.
- Normalize:
  - uppercase
  - replace non-alphanumeric with underscore
  - collapse multiple underscores
  - trim leading/trailing underscores
Example:
Contoso.WeatherMeterService → CONTOSO_WEATHERMETERSERVICE
If service naming is ambiguous:
- Use IF-UNKNOWN style is not allowed for SERVICE_KEY.
- Use the best available stable identifier and add a note: INSUFFICIENT EVIDENCE.
### Distinct Flow Definition
Distinct Flow = unique combination of:
service_name
+ ingestion_source
+ ingestion_method
+ endpoint (or stable identifier if endpoint unavailable)
+ trigger_type
### Deterministic Generation
Within each SERVICE_KEY:
1) Normalize values (lowercase, trim whitespace, collapse duplicate spaces).
2) Sort flows by:
   ingestion_source, ingestion_method, endpoint, trigger_type
3) Assign:
<SERVICE_KEY>-FLOW-001, <SERVICE_KEY>-FLOW-002, ...
Do not interleave numbering across services.
Never skip numbers.
Never renumber existing persisted flows unless inputs change.
---
## 3.2 STEP Keys
STEP keys describe execution order within a FLOW.
Format:
STEP-01
STEP-02
...
Rules:
- Must reflect execution order as supported by evidence.
- If ordering is uncertain:
  - Set inferred=Y
  - Provide explanation.
- Each STEP must include evidence.
---
## 3.3 MAP Keys
MAP keys apply to STM row-level mappings.
Deterministic ordering:
Sort rows by:
1) SourceName
2) SourceField
3) ProcedureOrPackage (or TargetTable if procedure unknown)
4) TargetColumn
Assign:
MAP-0001, MAP-0002, ...
Never generate MAP keys out of order.
Never randomize ordering.
---
# 4. Interface Key Standards (Cross-Repo Stitching)
Interfaces represent cross-repo handoffs.
Only include an interface if explicit evidence exists.
Do not list assumed shared tables or implied dependencies.
Interface key formats:
Oracle:
- IF-ORACLE-PKG-<SCHEMA>.<PACKAGE>
- IF-ORACLE-PROC-<SCHEMA>.<PACKAGE>.<PROC>
- IF-ORACLE-FUNC-<SCHEMA>.<PACKAGE>.<FUNC>
- IF-ORACLE-TABLE-<SCHEMA>.<TABLE>
- IF-ORACLE-VIEW-<SCHEMA>.<VIEW>
HTTP / SOAP:
- IF-HTTP-<REDACTED_HOST_OR_BASEURL>
- IF-SOAP-<REDACTED_HOST_OR_WSDL>
Files:
- IF-FILE-UNC-<REDACTED_PATH>
- IF-FILE-PATH-<REDACTED_PATH>
Queues:
- IF-QUEUE-<NAME>
SDK Integrations:
- IF-SDK-<VENDOR_OR_SYSTEM>
Unknown:
- IF-UNKNOWN-<SHORT_DESC>
Evidence requirement:
Do not list an interface unless there is explicit evidence in repo.
If interface presence is ambiguous:
- Do NOT include it
- Instead add to notes:
  INSUFFICIENT EVIDENCE: <description>
---
# 5. Redaction Standards
Never expose:
- API keys
- Secrets
- Tokens
- Passwords
- Connection strings with credentials
If found:
- Replace sensitive portion with:
  REDACTED SECRET IN ENDPOINT
Still provide evidence pointer.
---
# 6. Config Resolution Standards
When resolving config placeholders:
- Preserve runtime parameters like {start}, {end}, {node}, etc.
- Only resolve values that exist in repo config files.
- If config value not found:
  ${VarName}=[NOT FOUND IN CODE]
- Do not infer environment-specific values.
---
# 7. Oracle Metadata Rules
If Oracle DDL or package body is NOT present in repo:
- Do not infer data types.
- Set metadata fields to NOT FOUND IN CODE
- Only map what is explicitly traceable from code.
---
# 8. Frequency Resolution Rules
When determining ingestion frequency, look for:
- System.Timers.Timer
- System.Threading.Timer
- Quartz.NET configs
- Task Scheduler XML
- Cron expressions
- Config keys containing: Interval, Frequency, Schedule, Minutes, Cron
If frequency cannot be explicitly determined:
Unknown — verify with team
Never guess based on naming conventions.
---
# 9. Unresolved Sections (Required When Applicable)
Whenever applicable, skills must include:
- Unresolved config variables
- Unresolved frequency
- Untraceable Oracle parameters
- Target columns without clear source mapping
- Ambiguous interface references
Never silently drop unresolved artifacts.
---
# 10. Scope Rules
Skills must respect the scope parameter:
- current-repo → only analyze active repository
- workspace → may analyze across open workspace roots
Do not reference files outside selected scope.
---
# 11. Ordering and Stability
Outputs must be:
- Deterministically ordered
- Stable across re-runs
- Not dependent on scan order randomness
- Not dependent on filesystem traversal order
Always sort before assigning keys.
---
# 12. Separation of Responsibilities
Skills must not overlap responsibilities:
- Service Discovery → identify flows and interfaces
- Source Inventory → enumerate inbound feeds
- STM → column-level mapping
- Flow Diagram → visual representation only
- Architecture → business-readable narrative
- System Integration Map → cross-repo stitching
--
# 13. Delimiter Standard for Tabular Exports
All tabular exports must be QUOTED PSV:
- Delimiter is |
- Every field is double-quoted
- Any internal double quotes are escaped by doubling ("")
- Newlines are replaced with a single space