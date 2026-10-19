# Skill: Source-To-Target Mapping (STM) with STEP + MAP Keys + Interfaces (Pipe-Delimited)
## No Hallucinations
Analyze ONLY the code and files present in the selected scope.
Do NOT use external knowledge, assumptions, or inferred behavior.
If something is not explicitly supported by evidence in this repo/scope, output:
- NOT FOUND IN CODE (missing artifact)
- INSUFFICIENT EVIDENCE (ambiguous meaning)
Never fabricate endpoints, schedules, sources, transformations, procedures, tables, reports, interfaces, or mappings.
Do not leave anything blank if not found or applicable.
## Purpose
Produce column-level STM as a pipe-delimited text output (PSV) rather than comma CSV.
Include derived/hardcoded/proc-filled values where evidenced.
Provide sequencing (STEP) and mapping (MAP) keys for traceability.
Capture interfaces consumed/produced for cross-repo stitching.
## Inputs (User Parameters)
- scope: current-repo | workspace (default current-repo)
- flow_keys: <SERVICE_KEY>-FLOW-001,<SERVICE_KEY>-FLOW-002 | all
- include_reports: true | false (default false)
- completeness: standard | 100pct (default 100pct)
- include_na_rows: true | false (default true)
- include_proc_filled: true | false (default true)
- evidence_mode: line | anchor
- oracle_definition_source: repo-only (fixed)
- ignore_configuration_as_source: true | false (default false)
## Required Output Format
Output MUST be pipe-delimited with "|" as the delimiter.
This is PSV format (pipe-separated values).
Use header exactly from:
.github/skills/_templates/stm.psv.template


## PSV Escaping Rules (Critical)
- Wrap the entire field in double quotes: "value"
- Escape any double quotes inside a value by doubling them:
  "He said ""hello"""
- Replace newlines within a field with a single space.
- Replace carriage returns and tabs with a single space.
- Do NOT omit empty fields; output empty as "NA".
This ensures the output can be safely imported into Excel and parsed consistently.
## Required STM Coverage (Team Requirements)
Must include:
- values from inbound feeds (JSON/XML/CSV/etc.)
- derived values in code (timezone conversions, casts, concatenations)
- hard-coded constants/defaults (example: set MCP values to 0)
- values populated by PL/SQL procedures ONLY if include_proc_filled=true AND evidence exists in repo
- explicit N/A rows (if include_na_rows=true) for:
 - source fields not mapped to any target
 - target columns not populated from any source but populated in code/proc (only with evidence)
## Oracle Metadata Constraint (Repo Only)
If Oracle DDL/package definitions are not in repo:
- Produce mapping rows derived from code/config evidence
- Set Oracle metadata fields to NOT FOUND IN CODE
- Do not infer types, nullability, defaults, or comments.
## STEP Key Rules (Sequencing)
For each FLOW, generate a STEP list first.
STEP schema:
FLOW-###
steps:
 STEP-01:
   description:
   inferred: Y|N
   evidence:
     - repo/path/file.ext:L### (or anchor)
 STEP-02:
   description:
   inferred: Y|N
   evidence:
     - repo/path/file.ext:L### (or anchor)
Rules:
- Prefer explicit scheduling/entrypoint order from code.
- If ordering is unclear, set inferred=Y and explain why.
- Each step must have evidence.
## Interface Capture (Required)
For each FLOW, output interfaces consumed/produced (cross-repo handoffs).
Interfaces must follow:
.github/skills/_standards.md
Critical rule:
Do not list an interface unless there is explicit evidence in repo.
Interface schema:
FLOW-###
interfaces_consumed:
 - interface_key:
   interface_type:
   interface_name_or_location:
   evidence:
     - repo/path/file.ext:L### (or anchor)
interfaces_produced:
 - interface_key:
   interface_type:
   interface_name_or_location:
   evidence:
     - repo/path/file.ext:L### (or anchor)
notes:
 - INSUFFICIENT EVIDENCE: <only if needed>
## MAP Key Rules (Deterministic)
Each PSV data row is a MAP record.
Deterministic MAP ordering:
Sort rows by:
1) SourceName
2) SourceField
3) ProcedureOrPackage (or TargetTable if proc unknown)
4) TargetColumn
Then assign:
MAP-0001, MAP-0002, ...
## Output Requirements (Order Matters)
For each requested FLOW key:
1) Output STEP list (with evidence)
2) Output interfaces_consumed and interfaces_produced (with evidence)
3) Output PSV rows (pipe-delimited) with header first
## PSV Columns (Must Match Template Exactly)
FlowKey|MapKey|ServiceName|SourceName|SourceDataLoad|SourceField|SourceFieldDescription|Transformation|DerivedOrHardcoded|ProcedureOrPackage|Schema|TargetTable|TargetColumn|TargetDataType|TargetLength|TargetPrecision|TargetScale|NullableYN|DefaultValue|TargetComments|PKorAK|Evidence
## PSV Field Rules
- FlowKey must be <SERVICE_KEY>-FLOW-###.
- MapKey must be MAP-####.
- DerivedOrHardcoded must be Y or N.
- Transformation must be NONE or a plain-English transformation supported by evidence.
- Evidence must include at least one evidence pointer per row.
- If include_reports=false, omit report-related rows.
- Redact secrets in any endpoint/path and note REDACTED SECRET IN ENDPOINT.
- If ignore_configuration_as_source=true, omit rows where source is solely from configuration without code evidence.
## Evidence Rules
Evidence must be either:
- evidence_mode=line: repo/path/file.ext:L###
- evidence_mode=anchor: file + unique anchor snippet
If evidence cannot be provided:
- Set the specific field to INSUFFICIENT EVIDENCE
- Omit the mapping row or interface from the final output