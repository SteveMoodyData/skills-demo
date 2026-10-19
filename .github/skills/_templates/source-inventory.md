# Service Source Inventory
## Inventory Table
| ServiceName | Type | SourceName | SourceFileType | SourceDataLoad | Frequency | SourceCall | SourceCallFilled | Evidence |
|---|---|---|---|---|---|---|---|---|
| <ServiceName> | <HTTP GET/HTTP POST/SOAP/SFTP/FTP/File/Database Poll/Message Queue/SDK Pull> | <SourceName> | <json/xml/csv/etc or NOT FOUND IN CODE> | <Business feed name or NOT FOUND IN CODE> | <e.g. every 5 minutes or Unknown — verify with team> | <call string with placeholders> | <call string with config resolved where possible> | <repo/path/file.ext:L###> |
## Unresolved Items
- Config variables not resolved:
 - <VarName> (evidence)
- Frequency not resolved:
 - <Feed description> (evidence)
- Calls not fully reconstructed:
 - <Feed description> (evidence)