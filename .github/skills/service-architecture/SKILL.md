# Skill: service-architecture
# Description: Generate a formal service architecture document for a VB.NET service using flow-index.md,
# optionally enriched by oracle-metastore-output.psv (no repo scanning).
#
# Outputs:
# - Markdown (default, printed in chat or saved)
# - Word document (.docx) when output_format=docx (saved to output_path)
## Standards Compliance (Required)
This skill must follow the rules defined in:
github/skills/_standards.md
If any rule in this skill conflicts with the standards document, the standards document takes precedence.
## No Hallucinations (Required)
Use ONLY:
- flow-index.md (authoritative for flow existence, triggers, sources, endpoints, interfaces, and notes)
- oracle-metastore-output.psv (optional enrichment only; do not regenerate it)
Do NOT scan repositories.
Do NOT infer behavior from code.
Do NOT invent auth methods, concrete Oracle schema/package/table names, endpoints, transformations, retry logic, or outputs.
If something is not explicitly present in the allowed sources, write:
NOT FOUND IN CODE
For schedule/frequency when present but ambiguous, write:
UNKNOWN
## Inputs (User Parameters)
- flow_index_path: path to flow-index markdown (default github/skills/flow-index.md)
Flow selection (choose one):
- service_key: service key prefix (recommended)
  Example: service_key=CONTOSO_WEATHERMETERSERVICE
  Expand to all FLOW keys beginning with:
<service_key>-FLOW-
OR
- flow_keys: explicit list, wildcard, or "all"
  Examples:
  flow_keys=CONTOSO_WEATHERMETERSERVICE-FLOW-001
  flow_keys=CONTOSO_WEATHERMETERSERVICE-FLOW-*
  flow_keys=all
Oracle enrichment (optional):
- use_oracle_metastore: true|false (default false)
- oracle_metastore_psv_path: path to oracle metastore output (default- \github\skills\oracle-metastore-output.psv)
Output:
- output_format: markdown|docx (default markdown)
- output_path: optional path
  - If output_format=markdown and output_path is provided: save markdown to that path
  - If output_format=docx and output_path is provided: save docx to that path
  - If output_path is not provided: print markdown in chat (docx requires output_path)
## Deterministic Expansion Rules
If service_key is provided:
1) Parse flow_index_path.
2) Collect every FLOW key matching: <service_key>-FLOW-
3) Sort by numeric suffix ascending if possible (FLOW-001, FLOW-002, ...), else lexicographic.
4) If no flows found: output a minimal document stating no indexed flows found for service_key.
If flow_keys includes wildcard:
- Expand wildcard against flow_index_path and sort deterministically.
## Oracle Metastore Enrichment Rules (use_oracle_metastore=true)
Purpose:
- Fill in missing/unknown Oracle details in the architecture doc using oracle-metastore-output.psv
- WITHOUT reproducing the full metastore output in the architecture doc
Allowed uses:
- Populate Procedure Name, Schema, Target Table(s), CRUD (when absent in flow-index)
- Provide better Oracle “Purpose” phrasing using the metastore fields available (if any)
- Confirm/normalize Oracle object names
Forbidden:
- Do not invent Oracle objects not present in flow-index flows
- Do not override flow-index facts about which flows exist
- Do not introduce new integrations that are not referenced by selected flows
### How to parse oracle-metastore-output.psv
- It is a pipe-delimited file where EVERY field is double-quoted.
- Treat it as quoted PSV:
  - Split rows on pipe (|)
  - Remove surrounding quotes per field
  - Unescape "" to "
  - Newlines are already normalized in the file; do not reintroduce them
### How to match metastore rows to selected flows
Use rows where:
- FlowKey matches one of the selected FLOW keys
AND
- EvidenceSource contains "flow-index" (preferred) OR matches selected flow scope
For service_key selection:
- Load all metastore rows whose FlowKey starts with: <service_key>-FLOW-
### How to enrich Oracle Integration (Section 5)
For each selected flow:
- Build an Oracle interaction set from flow-index:
  - interfaces_consumed + interfaces_produced where interface_type contains "oracle" (case-insensitive)
- For each such flow-index interface, attempt to find metastore rows where:
  - RawInterfaceName OR NormalizedObjectName contains a matching token (case-insensitive)
  - OR ParentPackage/ProcedureName matches obvious names referenced (when present)
If flow-index only references wrapper methods (e.g., MRDb.InsertRT_UNIT_OUTPUT):
- Metastore may still provide mapped Oracle objects, but only use them IF:
  - The metastore row FlowKey matches the flow
  AND
  - The metastore RawInterfaceName/NormalizedObjectName clearly ties back to the wrapper method context
If not clearly tied, keep NOT FOUND IN CODE.
### Which metastore fields to use
- Schema: SchemaName
- Procedure Name:
  - Prefer: ParentPackage + "." + ProcedureName if both present
  - Else: NormalizedObjectName when ObjectType indicates procedure
- Target Table(s):
  - Use TableViewName when ObjectType is ORACLE_PROCEDURE/ORACLE_PACKAGE and TableViewName is not NOT FOUND IN CODE
- CRUD:
  - Use CRUDType when present and not UNKNOWN/NOT FOUND
- Purpose:
  - Use best available explicit descriptor:
    - If RawInterfaceName includes a verb (INSERT/UPDATE/DELETE/MERGE) or wrapper name suggests function, summarize minimally.
  - If unclear: NOT FOUND IN CODE
Deduplication:
- Deduplicate Oracle Integration rows by:
  ProcedureName + Schema + TargetTable(s) + CRUD + Purpose
- When multiple flows share the same row, append: (FLOWS: 001,002,...) to Purpose
## Document Structure (Must Match Exactly)
Generate a markdown architecture document following this template exactly.
# <service_name> Architecture
## 1. Service Overview
Include exactly:
• Purpose and business function (1 paragraph)
• Owner system / application context
• Trigger mechanism and schedule (if known)
Rules:
- service_name:
  - Prefer the service_name field from the selected flows when consistent.
  - If multiple service_name values appear, use: <service_key> (Multiple service_name values) and note in Open Questions.
- Purpose paragraph:
  - Derive from indexed ingestion_source(s), ingestion_method(s), inbound_feed_summary.type(s), and Oracle interactions.
  - If unclear: NOT FOUND IN CODE.
- Owner system / application context:
  - Use service_name + service_key. If broader owner system is not explicitly indexed: NOT FOUND IN CODE.
- Trigger mechanism and schedule:
  - Use trigger_type and frequency from flow-index.
  - Group shared trigger patterns into one readable description.
## 2. Source Systems
Table with columns exactly:
System Name | Protocol | Auth Method | Data Provided
Population rules (flow-index first):
- System Name:
  - From ingestion_source.
  - Also include external_calls items that clearly represent external source systems (e.g., a historian server).
- Protocol:
  - From ingestion_method (keep as written).
  - If missing: UNKNOWN.
- Auth Method:
  - Only include if explicitly indexed (e.g., "resourceCert", "cert", token).
  - Else: NOT FOUND IN CODE.
- Data Provided:
  - Prefer inbound_feed_summary.type (and keep it short).
  - If not present: NOT FOUND IN CODE.
Deduplicate identical rows. If multiple flows share a row, append "(FLOWS: ...)" to Data Provided.
## 3. Data Flow Summary
Narrative description of the end-to-end flow in plain English.
Rules:
- Holistic service summary, not one paragraph per flow.
- Use lane order:
  Trigger -> Source -> Endpoint/External Calls -> Processing -> Oracle -> Targets -> Outbound
- Endpoint/External Calls:
  - Use endpoint plus external_calls when present.
  - If endpoint says "actual URL NOT FOUND IN CODE" keep that phrasing; do not invent the actual URL.
- Oracle:
  - Prefer flow-index interfaces; if use_oracle_metastore=true, you may enrich with schema/proc/table names found in metastore.
## 4. Transformation Logic
Bullet list of key transformations.
Rules:
- Only include explicitly indexed transformation/processing notes from:
  - inbound_feed_summary.source_call_filled (when it describes transformation)
  - explicit text in the flow block describing joins/formatting
- Do not infer transformations from method names alone.
- If none found: NOT FOUND IN CODE.
## 5. Oracle Integration
Table with columns exactly:
Procedure Name | Schema | Target Table(s) | Purpose
Population rules:
- Primary source is flow-index interfaces (consumed/produced).
- If use_oracle_metastore=true, enrich missing Schema / Target Table(s) / CRUD / ProcedureName using the metastore rules above.
- Purpose must be a short plain-English phrase grounded in evidence; otherwise NOT FOUND IN CODE.
- If the flow-index notes indicate insufficient evidence behind MRDb wrapper mapping, include that limitation either:
  - in Purpose as "(Mapping behind wrapper not present in service repo)" when applicable, OR
  - in Open Questions.
## 6. Outbound Integrations
Table with columns exactly:
Target System | SDK/Method | Data Pushed | Trigger
Rules:
- Only include outbound integrations if explicitly indexed.
- If none found, write exactly:
No outbound integrations indexed.
## 7. Error Handling & Resilience
Note retry logic, dead-letter handling, logging patterns found in code.
Constraints:
- This skill does not scan repos. Only include what is explicitly indexed in flow-index.
- If frequency includes "on error ..." timing, include that as retry/backoff behavior.
- If nothing indexed: NOT FOUND IN CODE.
## 8. Open Questions
List any ambiguities found during indexing.
Rules:
- Include explicit notes: "INSUFFICIENT EVIDENCE: ..."
- Include gaps such as:
  - actual URL not found
  - auth method not found
  - schema/package/table names not found (unless metastore fills them)
  - transformation logic not found
## 9. Indexed Flows Referenced
List every FLOW key used to produce this document, one per line, sorted.
## Output Format
If output_format=markdown:
- If output_path is provided, save markdown to output_path.
- Else print markdown in chat.
If output_format=docx:
- output_path is REQUIRED and must end in .docx
- Create a Word document with:
  - Title style for "# <service_name> Architecture"
  - Heading 2 style for each "##" section
  - Tables rendered as Word tables
  - Bullet lists as Word bullets
- The docx content must match the markdown content exactly (no added sections).
- Save to output_path.
## Output Rules (Strict)
- Deterministic ordering
- No invented content
- Use NOT FOUND IN CODE and UNKNOWN exactly as specified
- flow-index is authoritative; metastore is enrichment only