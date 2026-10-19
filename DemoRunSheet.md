# Live Demo Run Sheet (about 7 min, slide 11)

Demo folder: `c:\Users\the4m\github\skills-demo` (open this as the VS Code workspace)

## Pre-flight (do 30 min before)

- [ ] Copilot signed in, agent/chat mode working, model selected
- [ ] Mermaid preview extension installed; test-render `outputs/flow-diagram.md`
- [ ] Zoom VS Code to ~150%, hide sidebar clutter, close all other workspaces
- [ ] Do one full dry run of Beat 3 so the session is warm
- [ ] Record a 60-second screen capture of a successful Beat 3 (the backup video)
- [ ] Do NOT open the original repo or any real-code folder during the talk

## Tab order (open left to right)

1. `src/WeatherMeterService/WeatherMeterService.vb`
2. `src/WeatherMeterService/WeatherClient.vb`
3. `.github/skills/flow-index.md`
4. `.github/skills/service-flow-diagram/SKILL.md`
5. Copilot chat panel
6. `outputs/flow-diagram.md` (preview pane)
7. `outputs/architecture.md`
8. `outputs/stm.psv`

## Beats

| # | Time | Show | Say | Do |
|---|---|---|---|---|
| 1 Input | 1:00 | Tab 1, then Tab 2 | "This is a stand-in for the real code: a Windows service with two timers, an HTTP pull and a file drop, writing to Oracle." | Scroll to the two timer handlers. Point at the unit conversions in `WeatherClient.vb`. |
| 2 Index | 1:00 | Tab 3 | "The index is built once. Two flows, deterministic keys, every fact points at a file and line." | Point at `FLOW-001` vs `FLOW-002`. Say: "Keys are sorted, not random, so a re-run gives the same keys." |
| 3 Skill (live) | 2:00 | Tab 4, then Tab 5 | "The skill tells the AI what to look for, what format to return, and what to say when it doesn't know." | Paste the prompt below. While it runs, scroll the skill's "No Hallucinations" and "Output Requirements" sections. Render the result in the Mermaid preview. |
| 4 Standard doc | 1:30 | Tab 7 (pre-generated) | "Same index, same template, every service. Notice what it admits it does not know." | Scroll to Section 5 (Oracle: "NOT FOUND IN CODE") and Section 8 (Open Questions). |
| 5 Human check | 1:30 | Tab 8, then Tab 1/2 | "This is the human review step. Every row cites a line. Let's check one." | Pick the `P_TEMP_C` row. Open `WeatherClient.vb` at line 30 and confirm the formula matches the "Transformation" column. |

## Prompt for Beat 3 (paste as-is)

```
Use .github/skills/service-flow-diagram/SKILL.md.
Use .github/skills/flow-index.md as authoritative.
service_key=CONTOSO_WEATHERMETERSERVICE
scope_mode=holistic-service
detail_level=detailed
layout=TD
Do not scan repos.
Do not invent anything.
```

Expected: a single Mermaid block with two flows, a shared error-handling node, and Oracle nodes for `INSERT_OBSERVATION`, `INSERT_METER_READING` and `PURGE_METER_READINGS`.

Say up front: "Wording and layout may vary run to run; keys, sources and line numbers should not."

## Optional 30-second contrast (only if time allows)

Before Beat 3, paste: `Explain what this service does` with `WeatherMeterService.vb` attached. Show that the answer has no keys, no evidence and a different shape from the skill output. Then run Beat 3.

## Other prompts (backups, not for live use)

Architecture doc:
```
Use .github/skills/service-architecture/SKILL.md.
service_key=CONTOSO_WEATHERMETERSERVICE
flow_index_path=.github/skills/flow-index.md
use_oracle_metastore=false
output_format=markdown
Do not scan repositories. Do not invent information.
```

STM:
```
Use .github/skills/service-stm/SKILL.md.
scope=current-repo
flow_keys=CONTOSO_WEATHERMETERSERVICE-FLOW-001,CONTOSO_WEATHERMETERSERVICE-FLOW-002
evidence_mode=line
Output STEP list first, then interfaces, then the PSV exactly matching the template.
```

## If something goes wrong

| Problem | Move |
|---|---|
| Copilot is slow (> 45 s) | Say "while that runs" and walk the skill file. If still nothing at 60 s, open `outputs/flow-diagram.md` and say "here's one I ran earlier". |
| Copilot says it can't find the skill or index | Re-attach `SKILL.md` and `flow-index.md` to the chat explicitly, then retry once. Otherwise fall back to the pre-generated file. |
| Mermaid won't render | Show the code block and describe it, or paste into mermaid.live in a pre-opened browser tab. |
| Output differs from the pre-generated file | Fine. Say "wording varies, the keys and evidence are the contract", then compare the flow keys. |
| Network or Copilot down | Play the backup video, then show `outputs/` files. |

## Reset after the demo

- Close the Copilot chat so the next run starts clean
- Don't save the live chat output over `outputs/`, so the pre-generated backups stay intact
