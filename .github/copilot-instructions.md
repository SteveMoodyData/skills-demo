# Copilot Instructions (Repo-Scoped)
## Hard Rules (No Hallucinations)
- Analyze ONLY content present in this repository.
- Do NOT use external knowledge, assumptions, or inferred behavior.
- If something is not explicitly supported by code/config/scripts in the repo, output:
 - "NOT FOUND IN CODE" (for missing artifacts)
 - "INSUFFICIENT EVIDENCE" (for ambiguous interpretation)
- Never fabricate endpoints, schedules, sources, transformations, procedures, tables, or reports.
## Evidence Requirement
Every finding must include evidence using one of these modes:
evidence_mode=line
- Format: repo/relative/path.ext:L### (starting line)
evidence_mode=anchor
- Format:
 file: repo/relative/path.ext
 anchor: "unique snippet to search for"
If you cannot reliably provide line numbers, use anchor mode.
## Key System (Required)
- FLOW-### for end-to-end flow patterns
- STEP-## for ordered steps within a flow
- MAP-#### for column-level mappings within a flow
Keys must be deterministic and reproducible per the standards in .github/skills/_standards.md
## Output Discipline
- Follow the output schemas defined in each SKILL.md exactly.
- Do not add extra sections outside the schema.
- If the user requests a diagram, return ONLY Mermaid code for that step.