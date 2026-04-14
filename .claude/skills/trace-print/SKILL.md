---
name: trace-print
description: Trace end-to-end print flow — plugin button → PrintTypeCode → MPS Processor → Template → output
user-invocable: true
argument-hint: <plugin name hoặc Mps code VD: HIS.Desktop.Plugins.TreatmentList, Mps000123>
---

# Trace Print Flow

Target: $ARGUMENTS

## Bước 1: Tìm print trigger trong plugin

Nếu argument là plugin name:
- Search trong folder plugin: `PrintTypeCode`, `MPS`, `Print`, `Mps`, `PrintConfig`
- Tìm button/method gọi print
- Xác định PrintTypeCode đang dùng

Nếu argument là Mps code:
- Nhảy đến Bước 4

## Bước 2: Trace PrintTypeCode config

- Tìm trong HIS.Desktop.LocalStorage.ConfigPrintType/
- PrintTypeCode map business operation → Mps processor code(s)
- Config từ backend: HIS_SAR_PRINT_TYPE table

## Bước 3: Print dispatch

- HIS.Desktop.Print/ dispatch logic
- ProcessorFactory load DLL động từ Plugins/MpsProcessor/
- Pattern: `MPS.Processor.{MpsCode}.{MpsCode}Processor`

## Bước 4: Phân tích MPS Processor

Đọc `MPS/MPS.Processor/MPS.Processor.{MpsCode}/`:

### {MpsCode}Processor.cs
- Kế thừa ProcessorBase
- Constructor: `base(param, printData)`
- Override Run(): query data, fill dataset
- API calls để lấy data (GlobalQuery hoặc BackendAdapter trực tiếp)
- Barcode/QR generation
- Multi-page handling

### {MpsCode}PDO.cs
- Print Data Object
- Dataset definitions
- Field mappings

## Bước 5: Tìm template

- Template location: `Tmp/Mps/` trong deployed app
- Format: Excel (.xlsx cho FlexCelPrint) hoặc Word (.docx cho RichEditor)
- Template name thường = Mps code

## Bước 6: Trace ProcessorBase

Đọc `MPS/MPS.ProcessorBase/`:
- ProcessorBase.cs: template loading, data binding, preview/print/export
- CommonKey.cs: shared key constants
- GlobalQuery.cs: shared data queries
- PrintData.cs: print job metadata
- PrintConfig.cs: print configuration

## Bước 7: Output

```
PRINT FLOW:
  Plugin: {plugin name}
  Trigger: {button/method name}
  PrintTypeCode: {code value}

  MPS Processor: {Mps code}
  Processor File: MPS/MPS.Processor/MPS.Processor.{Code}/{Code}Processor.cs
  Base Class: ProcessorBase
  PDO File: {Code}PDO.cs

  Data Queries:
    - {API URI 1} via {Consumer}
    - {API URI 2} via {Consumer}
    - GlobalQuery.{method}()

  Template: Tmp/Mps/{template file}
  Format: Excel (FlexCelPrint) / Word (RichEditor)
  Output: Preview / Direct Print / Export PDF

  CommonKeys used: [{list}]
```
