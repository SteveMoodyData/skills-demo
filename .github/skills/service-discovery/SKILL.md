# Skill: Service Discovery (Service-Prefixed Deterministic FLOW Keys + Interfaces)
## No Hallucinations
Analyze ONLY the code and files present in the selected scope.
Do NOT use external knowledge, assumptions, or inferred behavior.
If something is not explicitly supported by evidence in this repo/scope, output:
- NOT FOUND IN CODE (missing artifact)
- INSUFFICIENT EVIDENCE (ambiguous meaning)
Never fabricate endpoints, schedules, sources, transformations, procedures, tables, reports, or interfaces.
## Purpose
Identify distinct end-to-end ingestion flows and assign deterministic FLOW keys that are globally unique by service prefix.
Capture cross-repo interfaces consumed/produced (Oracle objects, file drops, queues, service-to-service calls).
Optionally emit a compact inbound feed summary that links to evidence.
## Inputs (User Parameters)
- scope: current-repo | workspace (default current-repo)
- evidence_mode: line | anchor
- include_reports: true | false (default false)
- include_object_names: true | false (default false)
- max_depth: integer (default 3)
- persist_flow_index: true | false (default true)
- include_inbound_feed_summary: true | false (default false)
## SERVICE_KEY (Required)
Determine SERVICE_KEY deterministically using standards in:
.github/skills/_standards.md
Prefer:
- repo folder name (workspace root folder)
Else:
- solution/project name evidenced in repo
Normalize to SERVICE_KEY format:
- uppercase
- replace non-alphanumeric with underscore
- collapse multiple underscores
- trim leading/trailing underscores
## What to Discover
For each distinct flow, extract:
- service_name (prefer repo/project/solution name evidenced in repo)
- service_key (computed SERVICE_KEY)
- trigger_type (timer, scheduler, job, manual, etc.)
- ingestion_source (weather API, meter vendor, partner system, etc.)
- ingestion_method (API, SOAP, SFTP, FTP, file drop, database poll, queue, SDK pull)
- frequency (timer interval / schedule config if explicitly found)
- endpoint (exact URL or path if present; redact secrets)
- external_calls (domains/systems called externally, if visible in code/config)
- interfaces_consumed (cross-repo handoffs read/used by this service)
- interfaces_produced (cross-repo handoffs written/emitted by this service)
- report_endpoint (optional, only if include_reports=true and explicitly found)
- evidence (minimum: entry point + one ingestion call site + one Oracle or output evidence if present)
## What Counts as an Interface
Interfaces are cross-repo handoffs, including:
- Oracle packages/procedures/functions called
- Oracle tables/views written to or read from (if explicitly referenced)
- Shared file drops (UNC/network paths or explicit external directories)
- Message queues/topics
- Inter-service HTTP/SOAP calls
Interface keys must follow:
.github/skills/_standards.md
### Critical Interface Rule
Do not list an interface unless there is explicit evidence in repo.
If an interface is suspected but not proven:
- Do NOT list it under interfaces_consumed/produced
- Put it under notes as INSUFFICIENT EVIDENCE.
## Distinct Flow Definition
Distinct flow = unique combination of:
service_name + ingestion_source + ingestion_method + endpoint + trigger_type
If endpoint is NOT FOUND IN CODE:
- Use the best available stable identifier (e.g., config key name) in the endpoint field
- Still mark endpoint as NOT FOUND IN CODE if the actual URL/path cannot be constructed
- Provide evidence for the config key usage
## Deterministic FLOW Key Generation (Service-Prefixed)
Within a given SERVICE_KEY:
1) Normalize values (lowercase, trim whitespace, collapse repeated whitespace).
2) Sort flows by:
  ingestion_source, ingestion_method, endpoint, trigger_type
3) Assign:
<SERVICE_KEY>-FLOW-001, <SERVICE_KEY>-FLOW-002, ...
Do not interleave numbering across services.
## Output Schema (Required)
Return exactly this structure:
### FLOW INDEX (COPY INTO .github/skills/flow-index.md)
## <SERVICE_KEY>-FLOW-001
service_name:
service_key:
trigger_type:
ingestion_source:
ingestion_method:
frequency:
endpoint:
external_calls:
interfaces_consumed:
 - interface_key:
   interface_type:
   interface_name_or_location:
   evidence:
     - repo/path/file.ext:L###   # or anchor form
interfaces_produced:
 - interface_key:
   interface_type:
   interface_name_or_location:
   evidence:
     - repo/path/file.ext:L###   # or anchor form
report_endpoint:                 # only if include_reports=true and explicitly found
inbound_feed_summary:            # only if include_inbound_feed_summary=true
 - feed_key: FEED-01
   type:
   source_call:
   source_call_filled:
   frequency:
   evidence:
     - repo/path/file.ext:L###
evidence:
 - repo/path/file.ext:L###
notes:
 - INSUFFICIENT EVIDENCE: <only if needed>
## <SERVICE_KEY>-FLOW-002
...
## Evidence Rules
- Every populated field must have at least one evidence entry.
- If a field cannot be supported by evidence, set it to NOT FOUND IN CODE.
- If an endpoint exists but contains secrets, redact and state:
 REDACTED SECRET IN ENDPOINT
 and still provide evidence.
- Evidence must be either:
 - evidence_mode=line: repo/path/file.ext:L###
 - evidence_mode=anchor: file + unique anchor snippet
## Persisting Keys
If persist_flow_index=true:
- Include the header line exactly:
 ### FLOW INDEX (COPY INTO .github/skills/flow-index.md)
so the user can paste the output directly into that file and commit.
## Notes
- This skill does not generate full feed inventories or STM mappings.
- Use service-source-inventory for full inbound feed inventory.
- Use service-stm for column mappings.