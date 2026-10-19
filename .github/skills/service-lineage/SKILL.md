# Skill: Service Data Lineage (High Level)
## No Hallucinations
Analyze ONLY the repository.
Do NOT use external knowledge, assumptions, or inferred behavior.
If not explicitly supported by repo evidence, output "NOT FOUND IN CODE" or "INSUFFICIENT EVIDENCE".
Never fabricate.
## Purpose
Produce a lineage report per FLOW key with optional report endpoint.
## Inputs (User Parameters)
- flow_keys: FLOW-001,FLOW-002 | all
- include_reports: true | false
- include_object_names: true | false (default false)
- evidence_mode: line | anchor
- transformations: none | cite-only | explain-with-evidence (default cite-only)
## Output
Use template: .github/skills/_templates/lineage-report.md
## Requirements Per FLOW
- Identify source, method, frequency, endpoint
- List STEP sequence (STEP-##) if known
- List transformations per selected mode
- List Oracle objects if found
- If include_reports=true, include report endpoint type and path only if in repo
## Evidence
Every statement must have evidence.