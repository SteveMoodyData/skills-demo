# Skills-Based Reverse Engineering - Demo Kit

Everything here is fictional (Contoso, example hostnames, fake keys).

- `src/WeatherMeterService/` - dummy VB.NET service (2 timers, HTTP pull, CSV file drop, 3 Oracle procedures)
- `.github/skills/` - sanitized skills, templates, standards, and the `flow-index.md` built from the dummy code
- `outputs/` - results the skills produce: `flow-diagram.md`, `architecture.md`, `stm.psv`

## Demo prompts

Flow diagram:
```
Use .github/skills/service-flow-diagram/SKILL.md.
Use .github/skills/flow-index.md as authoritative.
service_key=CONTOSO_WEATHERMETERSERVICE, scope_mode=holistic-service, detail_level=detailed
Do not scan repos. Do not invent anything.
```

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
