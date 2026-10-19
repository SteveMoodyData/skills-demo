# Skill: System Integration Map (Cross-Repo Interfaces)

## No Hallucinations

Analyze ONLY the code and files present in the workspace folders you can access.

Do NOT use external knowledge, assumptions, or inferred behavior.

If not explicitly supported by repo evidence, output "NOT FOUND IN CODE" or "INSUFFICIENT EVIDENCE".

Never fabricate.

## Purpose

Create a cross-repo integration map by identifying interfaces that services consume and produce.

This skill does not attempt full column-level STM across repos.

It captures the contracts between repos so per-repo FLOW keys can be stitched together.

## Inputs (User Parameters)

- scope: workspace | current-repo (default workspace)

- evidence_mode: line | anchor

- include_details: true | false (default true)

- include_endpoints: true | false (default true, redact secrets)

- include_reports: true | false (default false)

## What Counts as an Interface

An interface is something shared across repositories, including:

- Oracle packages, procedures, functions

- Oracle tables/views used as handoff points

- Shared file drops (UNC paths, network shares, blob paths if present)

- Message queues/topics (if present)

- Inter-service HTTP/SOAP calls (service-to-service integration)

- Shared libraries referenced as external dependencies (optional if include_details=true)

## Interface Key Rules

Create an interface identifier using standards in:

.github/skills/_standards.md

Examples:

- IF-ORACLE-PKG-DEMO.WEATHER_PKG

- IF-ORACLE-PROC-DEMO.WEATHER_PKG.INSERT_OBSERVATION

- IF-ORACLE-TABLE-DEMO.OBSERVATION

- IF-FILE-UNC-<redacted-path>

- IF-HTTP-<redacted-hostname>

- IF-SOAP-<redacted-hostname>

- IF-QUEUE-<name>

If an interface exists but the full name is not visible, use:

IF-UNKNOWN-<short description>

and mark it INSUFFICIENT EVIDENCE.

## Required Output

Use template:

.github/skills/_templates/system-integration.md

## Required Content

1) Inventory of workspace repos scanned (repo root folder names)

2) Interface catalog (unique interfaces, de-duplicated)

3) Producer/consumer mapping per repo

4) Optional: link to per-repo FLOW keys if flow-index.md exists in that repo

## Evidence Requirements

- Every interface must have evidence.

- Evidence must point to call sites, config keys, or script references.

- If include_endpoints=true and endpoints include secrets, redact and state:

  "REDACTED SECRET IN ENDPOINT"
 