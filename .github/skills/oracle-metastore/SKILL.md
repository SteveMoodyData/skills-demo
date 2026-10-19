# Skill: oracle-meta-store
# Output: ONE quoted pipe-delimited dataset printed in chat (no markdown, no files)
## No Hallucinations (Required)
Analyze ONLY the files present in the workspace scope.
Do NOT use external knowledge, assumptions, or inferred behavior.
Do NOT invent Oracle objects, CRUD operations, tables/views, columns, datatypes, defaults, PK flags, comments, or procedure text.
If something is not explicitly supported by evidence in flow-index, the catalog CSV, or .pck files, write:
- NOT FOUND IN CODE
- or UNKNOWN (only for classification-like fields where absence is expected, e.g., CRUDType)
Do not list an Oracle object unless there is explicit evidence in flow-index or code/package files.
Do not list a column unless it exists in the catalog CSV.
## Purpose
Produce a single, pipe-delimited (|) output that enables lineage in this hierarchy:
PACKAGE → PROCEDURE (or config/other oracle object) → TABLE/VIEW → COLUMN
This skill:
1) Reads Oracle interfaces from flow-index OR explicit object list
2) Normalizes Oracle object names (adds schema prefix like DEMO. if missing)
3) Optionally pulls matching procedure text from .pck files in a hardcoded repo
4) Joins downstream column metadata from a provided Oracle object catalog CSV
5) Outputs ONE dataset (quoted PSV) printed directly in chat
No markdown. No separate sections. No templates. No file creation.
---
# Required Input Parameters (User supplies)
- scope: workspace | current-repo (default workspace)
- source_mode: flow-index | explicit-list (default flow-index)
If source_mode=flow-index:
- flow_index_path: path to the flow index (default .github/skills/flow-index.md)
- flow_keys: comma-separated FLOW keys or "all" (default all)
- only_oracle_interfaces: true|false (default true)
If source_mode=explicit-list:
- object_refs: comma-separated Oracle references (package/proc/table/view), examples:
 - DEMO.WEATHER_PKG
 - DEMO.WEATHER_PKG.INSERT_OBSERVATION
 - OBSERVATION
 - WEATHER_PKG.INSERT_OBSERVATION
Required:
- oracle_catalog_csv_path: path to the Oracle column catalog CSV (no default)
Normalization:
- default_schema: schema to apply when missing (default DEMO)
- normalize_uppercase: true|false (default true)
- normalize_add_schema: true|false (default true)
Procedure text:
- include_proc_text: true|false (default false)
- include_proc_text_mode: package-only | package-and-proc (default package-and-proc)
Evidence:
- evidence_mode: line | anchor (default line)
---
# Hardcoded Constants (Update Once)
# This is the WORKSPACE ROOT folder name (repo) that contains the .pck files.
PCK_REPO_ROOT_FOLDER = "<path-to-folder-containing-.pck-files>"
# The skill must ONLY search for .pck inside that workspace folder when include_proc_text=true.
---
# Output Format (Strict)
## Output is ONE dataset printed in chat.
- Pipe delimiter: |
- Every field is double-quoted
- Header printed once
- Then one row per (FlowKey + Oracle object + Table/View + Column)
## Quoted PSV rules (mandatory)
For every field value:
1) Wrap with double quotes: "value"
2) Escape internal quotes by doubling: "He said ""hello"""
3) Replace any newline or carriage return with a single space
4) Replace tabs with a single space
5) Never output unescaped raw quotes
Pipes (|) and commas are allowed inside values because all fields are quoted.
---
# Output Columns (Exact Order)
"FlowKey"|
"SourceMode"|
"InterfaceType"|
"RawInterfaceName"|
"CRUDType"|
"NormalizedObjectName"|
"SchemaName"|
"ObjectType"|
"ParentPackage"|
"ProcedureName"|
"ProcedureTextIncluded"|
"TableViewName"|
"ColumnName"|
"ColumnID"|
"ColumnTransformationsLogic"|
"DataType"|
"DataLength"|
"DataPrecision"|
"Nullable"|
"DataDefault"|
"ColumnComment"|
"PrimaryKeyFlag"|
"EvidenceSource"
EvidenceSource is a multi-value field joined with "; " exactly, e.g.:
"flow-index; .pck; CSV"
---
# Deterministic Processing Rules
## 1) Discover Oracle interfaces
### If source_mode=flow-index
- Parse flow_index_path.
- Select FLOW blocks by flow_keys (or all).
- For each selected FLOW:
 - gather every item in interfaces_consumed and interfaces_produced
 - keep only Oracle interfaces if only_oracle_interfaces=true
 - Oracle interfaces are those where interface_type contains "Oracle" (case-insensitive)
### If source_mode=explicit-list
- Treat each object_refs entry as a RawInterfaceName with SourceMode=explicit-list
- FlowKey must be set to "N/A" in output
## 2) Normalize object names (must be deterministic)
Given RawInterfaceName (and/or interface_name_or_location):
- trim whitespace
- if normalize_uppercase=true: uppercase
- remove wrapping quotes/brackets if present
- if normalize_add_schema=true:
 - If it does not contain a schema prefix (no "<schema>.<name>"):
   prefix with "<default_schema>."
 - Else keep existing schema prefix
- Output:
 - NormalizedObjectName = schema-qualified object reference
 - SchemaName = schema portion if known, else default_schema, else NOT FOUND IN CODE
## 3) Determine ObjectType + ParentPackage + ProcedureName
Use the best available evidence in this order:
1) interface_type from flow-index (InterfaceType)
2) explicit token patterns in object name:
  - If name looks like SCHEMA.PACKAGE.PROC => ObjectType=ORACLE_PROCEDURE, ParentPackage=SCHEMA.PACKAGE, ProcedureName=PROC
  - If name looks like SCHEMA.PACKAGE => ObjectType=ORACLE_PACKAGE
  - If name looks like SCHEMA.TABLE (and interface says table/view) => ORACLE_TABLE/ORACLE_VIEW
3) If still unclear => ObjectType=ORACLE_UNKNOWN
Never guess table vs view unless interface_type says so OR the CSV object_type says so for that object.
## 4) Procedure text (optional)
If include_proc_text=true AND ObjectType is ORACLE_PACKAGE or ORACLE_PROCEDURE:
- Search ONLY within workspace folder PCK_REPO_ROOT_FOLDER for .pck files.
- Find package/procedure by name match (case-insensitive after normalization).
- If include_proc_text_mode=package-and-proc and ProcedureName is known:
 - Extract the smallest procedure/function block that matches ProcedureName.
- If include_proc_text_mode=package-only or ProcedureName unknown:
 - Extract the package block (spec/body) containing the match.
- If found:
 - ProcedureTextIncluded column must contain the full extracted text in a single cell (quoted PSV rules apply).
 - EvidenceSource includes ".pck"
- If not found:
 - ProcedureTextIncluded must be NOT FOUND IN CODE
## 5) CRUDType determination
- If procedure text is available:
 - Determine CRUDType by scanning for first occurrence (in this priority order):
   MERGE, INSERT, UPDATE, DELETE
 - If multiple appear, pick the dominant (MERGE > INSERT > UPDATE > DELETE) unless evidence clearly shows only one.
- If procedure text not available:
 - CRUDType = UNKNOWN
Do not infer CRUDType from names alone.
## 6) Identify downstream table/view names for joining to catalog CSV
- Primary path: if the interface itself is an Oracle Table/View, TableViewName is that object.
- If the interface is a package/procedure:
 - Attempt to extract referenced table/view names from procedure text ONLY if explicit (INSERT INTO, UPDATE <table>, MERGE INTO, DELETE FROM).
 - If no explicit table/view found: TableViewName = NOT FOUND IN CODE
## 7) Join to Oracle catalog CSV (tables/views only)
- The CSV is authoritative for column-level output.
- Normalize CSV object_name similarly:
 - uppercase (if normalize_uppercase=true)
 - if it lacks schema prefix, prefix default_schema + "."
- Join keys:
 - TableViewName normalized == CSV object_name normalized
If match found:
- Output one row per CSV column for that table/view.
- EvidenceSource includes "CSV"
If no match:
- Output exactly one row with:
 ColumnName = NOT FOUND IN CODE
 and the CSV-derived fields set to NOT FOUND IN CODE
- EvidenceSource excludes CSV unless the CSV was searched but no entry found (still exclude CSV)
## 8) ColumnTransformationsLogic
- If procedure text is available AND the table/view was found in that procedure text:
 - Attempt to capture a minimal, evidence-based snippet describing the column logic:
   - Examples: "SET COL = NVL(:P_COL, 0)" or "COL := TO_NUMBER(x)" or "COL populated by sequence" if explicit
 - If no explicit per-column logic can be tied:
   ColumnTransformationsLogic = NOT FOUND IN CODE
- If procedure text not available:
 ColumnTransformationsLogic = NOT FOUND IN CODE
Never invent transformations.
## 9) EvidenceSource (multi-value)
Populate EvidenceSource by row:
- Include "flow-index" if the object came from flow-index
- Include ".pck" if procedure text was found and used
- Include "CSV" if the row includes CSV column metadata
Join with "; " and keep order: flow-index; .pck; CSV
## 10) Output ordering (deterministic)
Sort rows by:
1) FlowKey
2) ParentPackage
3) ProcedureName
4) TableViewName
5) ColumnID (numeric if available else lexicographic)
6) ColumnName
---
# Required Output Behavior (Non-negotiable)
- Print header once.
- Then print rows.
- No markdown.
- No bullets.
- No explanations.
- No additional commentary.