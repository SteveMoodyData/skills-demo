# Skill: Transformation Inventory
## No Hallucinations
Analyze ONLY the repository.
Do NOT use external knowledge, assumptions, or inferred behavior.
If not explicitly supported by repo evidence, output "NOT FOUND IN CODE" or "INSUFFICIENT EVIDENCE".
Never fabricate.
## Purpose
Extract transformation points with evidence and optional explanation.
## Inputs (User Parameters)
- flow_keys: FLOW-001,FLOW-002 | all
- mode: cite-only | explain-with-evidence
- evidence_mode: line | anchor
## What Counts as a Transformation
- field conversion/casting
- timezone conversion / date normalization
- header-detail join or merge
- defaulting/hardcoding
- filtering, aggregation
- branching business logic
- pre-procedure transformation in service code
- mapping into domain objects (e.g., Observation/MeterReading)
## Output Schema
For each FLOW:
FLOW-###
transformations:
 - transform_id: XFORM-01
   step_ref: STEP-## (if known, else blank)
   type:
   description: (omit if cite-only)
   evidence:
     - repo/path/file.ext:L### (or anchor)
## Rules
- If not found, output: "NOT FOUND IN CODE"
- If ambiguous, output: "INSUFFICIENT EVIDENCE"