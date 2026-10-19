# Skill: Service Source Inventory (Inbound Feeds with Config Resolution)
## No Hallucinations
Analyze ONLY the code and files present in the selected scope.
Do NOT use external knowledge, assumptions, or inferred behavior.
If something is not explicitly supported by evidence in this repo/scope, output:
- "NOT FOUND IN CODE"
- "INSUFFICIENT EVIDENCE"
Never fabricate endpoints, schedules, config values, or frequency.
## Purpose
Produce a feed-level inventory of inbound data sources:
one row per distinct inbound feed/call pattern.
This is higher-level than STM and does not map individual fields.
This skill is designed to:
- inventory all inbound calls (HTTP/SOAP/SFTP/FTP/file/db poll/queue/SDK pull)
- reconstruct SourceCall exactly as built in code (with placeholders)
- resolve config-based placeholders into SourceCallFilled when values are present in repo
- identify ingestion frequency if explicitly found
- report unresolved config variables and unresolved frequency
## Inputs (User Parameters)
- scope: current-repo | workspace (default current-repo)
- evidence_mode: line | anchor
- include_endpoints: true | false (default true)
- redact_secrets: true | false (default true)
- max_depth: integer (default 5)
## Output Template
Use:
.github/skills/_templates/source-inventory.md
## What to Find (Inbound Feed Types)
Scan for inbound patterns including:
- HttpClient/WebClient/HttpWebRequest/RestSharp calls
- SOAP/WSDL clients
- SFTP/FTP libraries (WinSCP, Renci.SshNet, etc.)
- File reads from external paths (File.ReadAllText, StreamReader, etc.)
- Database poll reads from external DB (SELECT as source)
- Queue/topic consumers
- Vendor SDK pull methods (Get/Fetch/Query)
## Distinct Feed Definition
A distinct feed is a unique combination of:
- call type (HTTP GET/POST, SOAP, SFTP, FTP, File, DB Poll, Queue, SDK Pull)
- base endpoint/path (or host + path pattern)
- business feed identity if evident (config key name, method name, comments)
If the same endpoint is called in loops with varying node/date parameters:
- capture as one row, keep runtime parameters as placeholders.
## Required Columns (Exact Order)
ServiceName | Type | SourceName | SourceFileType | SourceDataLoad | Frequency | SourceCall | SourceCallFilled | Evidence
## Evidence Rules
- Every row must have evidence.
- If include_endpoints=true and redact_secrets=true:
 - redact tokens/credentials
 - include "REDACTED SECRET IN ENDPOINT" if needed
- If a config value cannot be resolved from repo:
 - put "${VarName}=[NOT FOUND IN CODE]" in SourceCallFilled
 - list it again in Unresolved Items
## Unresolved Items Section (Required)
After the table, include:
- any config variables that could not be resolved
- any frequency that could not be determined
- any call that could not be reconstructed
Do not guess.