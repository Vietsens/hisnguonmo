# JsonExport — Dev guide

Internal documentation for developers maintaining or extending the JSON template renderer.

## File layout

```
Core/JsonExport/
├── PlaceholderParser.cs              ← depth-aware tokenizer for <#...;> and arg/pipe splitting
├── FunctionRegistry.cs               ← FunctionContext + 16 built-in functions + plugin API
├── CellRefResolver.cs                ← FlexCel cell + named range reader (Phase 3)
├── JsonTemplateRenderer.cs           ← main engine: loop expansion + placeholder resolve + conditional omit
├── JsonTemplateExtractor.cs          ← reverse engineer: scan .xlsx for <#KEY;> → JSON skeleton
├── WordTemplateExtractor.cs          ← reverse engineer: scan .docx for [[KEY]] → JSON skeleton
├── XtraReportTemplateExtractor.cs    ← reverse engineer: scan .repx for [FIELD] → JSON skeleton
├── TEMPLATE_SYNTAX.md                ← user-facing template syntax docs
└── README.md                         ← this file
```

## Public API

| Method | Purpose |
|---|---|
| `JsonTemplateRenderer.Render(template, singleKeys, listData)` | Render JSON template using dictionary + lists |
| `JsonTemplateRenderer.Render(template, singleKeys, listData, workbook)` | Same + FlexCel workbook for cell/named-range refs |
| `FunctionRegistry.Register(name, handler)` | Plugin custom function |

## Pipeline

```
Render(template, …)
 ├─ Preprocess [[Sheet!A1]] → <#cell("Sheet!A1");>      (Phase 3)
 ├─ JToken.Parse(preprocessed)                           ← validates JSON shape early
 ├─ ExpandLoops(root, ctx, null)                          ← Phase 6 (nested via itemContext)
 │   └─ For each array with one JObject template:
 │       - Resolve list: item.Property first, then ctx.ListData
 │       - Clone N times, recursively expand + resolve placeholders
 ├─ ResolvePlaceholders(root, ctx)                       ← visit each JValue(String)
 │   └─ ResolveValueTemplate → SplitPipeOptions → ResolveOption per option
 │       └─ ResolvePlaceholderBody → key / @named / function dispatch
 ├─ ApplyConditionalOmit(root)                            ← Phase 7 (strip ? suffix or remove)
 └─ root.ToString(Formatting.Indented)
```

## Adding a new function (Phase 2-style)

1. Add the function name to the `switch` in `FunctionRegistry.IsKnown` and `Invoke`.
2. Implement a private static `InvokeXxx(List<string> args, FunctionContext ctx)` method.
3. Use `ctx.ResolveArgAsValue(arg)` for args that should resolve placeholders.
4. Use `ctx.GetRawName(arg)` for args that are literal identifiers (list/field/format names).
5. Return `ResolveResult.Ok(value)` or `ResolveResult.Fail()`.

Example:
```csharp
case "myfn": return InvokeMyFn(rawArgs, ctx);

private static ResolveResult InvokeMyFn(List<string> args, FunctionContext ctx)
{
    if (args == null || args.Count < 1) return ResolveResult.Fail();
    var v = ctx.ResolveArgAsValue(args[0]);
    if (v.Failed || v.Value == null) return ResolveResult.Fail();
    string s = v.Value.ToString();
    return ResolveResult.Ok("MY-" + s);
}
```

## Adding a custom function via plugin API

For one-off cases that don't warrant changes to the core registry:

```csharp
FunctionRegistry.Register("hospital_code", (args, ctx) =>
{
    return ResolveResult.Ok("HCM-General-001");
});
```

The handler runs on every invocation, so keep it cheap. Pass `null` handler to unregister.

## Adding a new processor

1. In `ProcessData()`:
   ```csharp
   SetSingleKey("patient_name", rdo.PatientName);
   SetSingleKey("total_price", computedTotal);

   // For each list ADO that the JSON template loops over:
   RegisterListForJson("danhsachdichvu", rdo.Services.Cast<object>());
   ```
2. Place a `Mps000XXX.json` template alongside the `.xlsx`/`.docx`/`.repx` template.
3. Run print — the printed file (`.xlsx`/`.docx`/`.pdf`) and `.json` are emitted together.

The JSON path is automatic — no code change is needed in `AbstractProcessor` or per-processor.

## Runtime workflow — placing the .json template

When MPS prints a report, `AbstractProcessor.TryExportJson()` looks for a matching `.json` template in **the same folder as the source template** (`.xlsx`, `.docx`, or `.repx`). Lookup applies identically for all three formats — there is no per-format flag or path override. If a match is found, the runtime renders JSON output beside the printed file; if not, JSON export is silently skipped.

Two lookup strategies, tried in order:

| Strategy | Example: template = `Mps000275\Mps000275_BieuMau.docx` |
|---|---|
| **Same-name** — `Path.ChangeExtension(template, ".json")` | Looks for `Mps000275\Mps000275_BieuMau.json` |
| **Base-name** — match `^Mps\d{6}` in `printTypeCode`, append `.json` in same folder | Looks for `Mps000275\Mps000275.json` (shared across all variants of Mps000275) |

So a single JSON template can be reused across multiple template variants (long Vietnamese filenames, different formats), as long as the file name starts with the `MpsNNNNNN` code.

### Recommended designer workflow

1. Run `JsonExtractor.exe <template>` against the `.xlsx` / `.docx` / `.repx` to generate a skeleton.
2. Edit the resulting `.json` — prune keys you don't need, group lists, rename properties to readable JSON.
3. Drop the `.json` into the folder beside the binary template (same name OR base `Mps000XXX.json`).
4. Print the report — output `.json` appears next to the printed file with placeholder values bound from the data source.

## Pipeline integration points

`AbstractProcessor` calls `TryExportJson()` from each of these methods, for all three template types (Excel, Word, XtraReport):
- `OutFileRun()` — file path save
- `OutFileStreamRun()` — memory stream
- `OutFileStreamRunForSave()` — memory stream for save

Output is written into:
- `this.saveJsonMemoryStream` (always)
- `this.saveJsonFilePath` (if `saveFilePath` is set)
- `printDataBase.saveJsonMemoryStream` + `.saveJsonFilePath`

### Word (.docx) and XtraReport (.repx) support

`TryExportJson()` runs identically for Word and XtraReport flows with two differences:

1. **No FlexCel workbook** — the rendered output is `.docx` / `.pdf`, not a spreadsheet. The workbook load step is skipped (guarded by `templateType == Excel`), so Phase 3 placeholders `[[Sheet!A1]]`, `<#cell("…");>`, `<#named("…");>`, `<#@name;>` resolve to empty via pipe fallback. Templates targeting Word/XtraReport should avoid these placeholders or always provide a pipe fallback like `<#cell("A1");>|<#default;>`.

2. **List data must be registered explicitly** — `FlexCellExport.Store.DictionaryListData` retains every list bound via `ProcessObjectTag.AddObjectData`, but `TemplaterExport.Store` and `XtraReportExport.Store` do not. The Word/XtraReport processor must call `RegisterListForJson("ServiceReqs", listOfADO)` for every loop the JSON template needs.

Scalar keys are already shared: `this.singleValueDictionary` is populated from the relevant store's `DictionaryTemplateKey` at `InitType()`, so single-key placeholders like `<#PATIENT_NAME;>` work the same way for all three template types.

JSON template syntax (`<#KEY;>`) is identical across all three engines — only the binary template format differs.

## Backward compatibility

The entire JSON code path is **opt-in**: triggered only when a `.json` template file is present. Processors without a template see zero overhead — `TryGetJsonTemplatePath()` returns null and `TryExportJson()` exits immediately.

## JsonExtractor CLI — reverse engineer a template into a JSON skeleton

Designers don't have to hand-write the `.json` template: `JsonExtractor.exe` (the CLI build of `Inventec.Common.JsonExport.Cli`) scans a print template and emits a ready-to-edit JSON skeleton with `<#KEY;>` placeholders.

| Source extension | Engine | Source placeholder syntax | Extractor class |
|---|---|---|---|
| `.xlsx`, `.xls` | FlexCel | `<#KEY;>` / `<#List.Prop;>` | `JsonTemplateExtractor` |
| `.docx`, `.doc` | Templater (ONLYOFFICE) | `<#KEY;>` / `<#List.Prop;>` (same as Excel) | `WordTemplateExtractor` |
| `.repx` | DevExpress XtraReport | `[FIELD]` inside `<Expression>` | `XtraReportTemplateExtractor` |

All three extractors produce the **same JSON skeleton format** with `<#KEY;>` placeholders — that's the syntax `JsonTemplateRenderer` expects, regardless of the source template type.

### Usage

```
JsonExtractor.exe                              # interactive — prompts for path
JsonExtractor.exe Mps000312.xlsx               # write Mps000312.json next to template
JsonExtractor.exe Mps000275_BieuMau.docx -v    # Word + verbose (list keys/lists found)
JsonExtractor.exe Mps000062.repx --stdout      # print JSON to stdout
JsonExtractor.exe template.xlsx -o out.json --force
```

The binary auto-detects format by file extension. Unsupported extensions exit with code 1.

### .docx parsing notes

Templater in this codebase uses the **same `<#KEY;>` placeholder syntax as FlexCel Excel templates** (the placeholders appear as `&lt;#KEY;&gt;` HTML-escaped inside `word/document.xml`, but `XmlDocument.InnerText` un-escapes them automatically).

Placeholders are routinely split across `<w:r>` runs when the user changes formatting (bold/color/font) inside a placeholder — e.g. `<w:t>&lt;#</w:t><w:t>DESCRIPTION</w:t><w:t>_WORD;&gt;</w:t>`. The extractor concatenates every descendant `<w:t>` text inside each paragraph (`<w:p>`) before applying the regex, so split placeholders reassemble correctly.

Only `word/document.xml`, `word/header*.xml`, `word/footer*.xml` parts are scanned — comments, endnotes, styles, etc. are skipped and listed in `report.SkippedSheets` (verbose mode).

### .repx parsing notes

`.repx` is plain XML (root `<XtraReportsLayoutSerializer>`). The extractor walks:
1. Every element whose local name is `Expression` — covers most binding forms.
2. Every attribute whose local name contains `Expression` — covers older XtraReport versions that serialized expressions as attributes.

Field syntax in expressions is `[FieldName]`. Dotted form `[List.Prop]` is recognized as a list binding. Field names inside complex expressions (`Iif([A] > 0, [B], [C])`) are all captured.

### Programmatic API

Each extractor exposes the same shape — pick the one matching your file:

```csharp
ExtractionReport report = JsonTemplateExtractor.ExtractWithReport(xlsxPath);
ExtractionReport report = WordTemplateExtractor.ExtractWithReport(docxPath);
ExtractionReport report = XtraReportTemplateExtractor.ExtractWithReport(repxPath);

string json = report.JsonSkeleton;            // ready to save as .json
List<string> single = report.SingleKeys;      // flat scalar keys discovered
Dictionary<string, List<string>> lists = report.ListProperties; // list name → properties
```

## Tests

`MPS.ProcessorBase.Tests` is a sibling console project. Build + run:
```
MSBuild MPS.ProcessorBase.Tests.csproj /p:Configuration=Release
bin/Release/MPS.ProcessorBase.Tests.exe       # full suite
bin/Release/MPS.ProcessorBase.Tests.exe demo  # print the 3-processor scenario output
```

32 tests cover Phases 1, 2, 6, 7. Phase 3 (FlexCel cell/named range) needs a real `.xlsx` rendered workbook and is verified manually.

## C# version note

Project targets .NET Framework 4.5 with default C# language version (typically 5 or 6). Pattern matching `is X y` is **not** available — use explicit `var x = obj as X; if (x != null)` instead.

## Logging

All errors and warnings go through `Inventec.Common.Logging.LogSystem`. Common log lines:
- `JsonTemplateRenderer.Render failed` — fatal parse/render error
- `JSON evaluate failed: expr=… --` — DataTable.Compute couldn't process the substituted expression
- `CellRefResolver.ReadCell failed for 'addr':` — Phase 3 cell lookup failed (workbook missing, address invalid, FlexCel API mismatch)
- `Custom function '…' threw:` — a plugin function threw an exception
