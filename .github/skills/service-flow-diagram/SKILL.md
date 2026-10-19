# Skill: service-flow-diagram
# Purpose: Generate Draw.io-safe Mermaid diagrams from the pre-mapped flow-index
# Output: Mermaid only. Do not scan repos.
## No Hallucinations (Required)
Analyze ONLY the indexed content provided in flow-index.md (and any explicitly indexed STEP content if present).
Do NOT scan repositories.
Do NOT invent steps, interfaces, endpoints, tables, procedures, targets, or error paths.
If something is not explicitly present in flow-index.md, omit it.
## Source of Truth
The sole source of truth is:
- github/skills/flow-index.md (or user-provided flow_index_path)
This skill must not derive new information from code.
## Inputs (User Parameters)
- scope: workspace | current-repo (default workspace)
- flow_index_path: path to flow-index markdown (default github/skills/flow-index.md)
### Flow Selection
Choose ONE:
A) service_key: service key prefix
   Example:
   service_key=CONTOSO_WEATHERMETERSERVICE
   Expand to all FLOW keys that begin with:
   "<service_key>-FLOW-"
B) flow_keys:
   - explicit list, wildcard, or "all"
   Examples:
   flow_keys=SERVICE_A-FLOW-001,SERVICE_A-FLOW-002
   flow_keys=SERVICE_A-FLOW-*
   flow_keys=all
If both provided: service_key takes precedence.
### Scope Mode
- scope_mode: per-flow | holistic-service (default per-flow)
per-flow:
- One subgraph per FLOW.
holistic-service:
- ONE consolidated service-level diagram.
- Shared nodes must be deduplicated.
- Branches must reflect differences (sources/oracle/targets/outbound).
- Strictly forbidden: representing flows as single “FLOW-###” summary nodes.
### Detail Level
- detail_level: high | detailed (default high)
high:
- Major stages only (Trigger, Sources, Processing, Oracle, Targets, Outbound)
detailed:
- Include indexed STEP nodes only if they exist in flow-index. Make sure to always include Detail for stages- Trigger + Frequency, ingestion_source + external_calls(if not blank)
- In holistic-service mode, only show STEP nodes if they can be attached to a specific branch (source→oracle/target). If steps are global or ambiguous, collapse to a single Processing node.
- If there are common processes that are used by multiple flows put them at the top of the diagram and connect them to each flow with dashed lines. For example, if multiple flows share the same error handling process, you could represent that process as a single node at the top of the diagram and connect it to each flow with dashed lines to indicate that it is shared. If there are multiple shared processes, you could group them together in a subgraph or swimlane to further clarify that they are shared.
### Layout
- layout: TD | LR (default TD)
### Error Paths
- include_error_paths: true | false (default false)
Only include if explicitly indexed.
## Output Requirements (Strict)
Return only a single fenced Mermaid block:
```mermaid
...