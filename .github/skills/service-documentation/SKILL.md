# Skill: Reverse Engineering Documentation (Outline + Section-by-Section)
## No Hallucinations
Analyze ONLY the repository.
Do NOT use external knowledge, assumptions, or inferred behavior.
If not explicitly supported by repo evidence, output "NOT FOUND IN CODE" or "INSUFFICIENT EVIDENCE".
Never fabricate.
## Purpose
Produce reverse engineering documentation that a new reader can follow.
## Inputs (User Parameters)
- service_hint: optional (e.g., WeatherMeterService)
- mode: outline | write-section
- section_id: optional (required when mode=write-section)
- evidence_mode: line | anchor
## Outline Phase (mode=outline)
- Use template: .github/skills/_templates/documentation-outline.md
- Propose an outline based ONLY on what exists in repo.
- Include a "Database Objects" section and an "Issues Found" section.
- End with: "WAITING FOR APPROVAL"
## Write Phase (mode=write-section)
- Write ONLY the requested section_id.
- Include file/class/method labels and evidence for each finding.
- End with: "WAITING FOR NEXT SECTION REQUEST"