# System Integration Map (Cross-Repo)
## Repos Scanned
List each workspace root folder scanned:
- <repo-folder-1>
- <repo-folder-2>
## Interface Catalog (De-duplicated)
For each interface, include:
- interface_key:
- interface_type:
- interface_name_or_location:
- evidence:
Example format:
### IF-ORACLE-PKG-DEMO.WEATHER_PKG
interface_type: ORACLE_PACKAGE
interface_name_or_location: DEMO.WEATHER_PKG
producers:
 - <repo name> (evidence)
consumers:
 - <repo name> (evidence)
evidence:
 - repo/path/file.ext:L###
### IF-FILE-UNC-REDACTED
interface_type: FILE_DROP
interface_name_or_location: \\REDACTED\share\path
producers:
 - <repo name> (evidence)
consumers:
 - <repo name> (evidence)
evidence:
 - repo/path/file.ext:L###
## Producer/Consumer Map by Repo
For each repo:
### <repo name>
produces:
 - interface_key:
   evidence:
consumes:
 - interface_key:
   evidence:
## Optional: FLOW Key Cross-References
If .github/skills/flow-index.md exists in a repo, list FLOW keys that reference interfaces.
### <repo name>
- FLOW-001 -> produces: IF-..., consumes: IF-...
- FLOW-002 -> produces: IF-..., consumes: IF-...
## Notes and Gaps
- NOT FOUND IN CODE:
 - list interfaces that are implied by naming but not explicitly evidenced
- INSUFFICIENT EVIDENCE:
 - list unclear call sites or configs that do not fully resolve names/paths