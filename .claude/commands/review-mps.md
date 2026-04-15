---
description: Review MPS Processor — kế thừa, ProcessData, PDO, SetBarcodeKey, SetSingleKey, template keys, ExtendSingleKey
argument-hint: <Mps code hoặc processor file path VD: Mps000102, MPS.Processor.Mps000001>
---

# Review MPS Processor

Review: $ARGUMENTS

## 1. Cấu Trúc
- Kế thừa AbstractProcessor (KHÔNG phải ProcessorBase trực tiếp)?
- Constructor: base(param, printData) + cast rdoBase sang PDO?
- Có file ExtendSingleKey.cs cho constants?
- PDO kế thừa RDOBase?
- PDO có constructor nhận ĐỦ data từ plugin?

## 2. ProcessData() Override
- Có override ProcessData() (abstract từ AbstractProcessor)?
- KHÔNG override Run()?
- Flow đúng: ReadTemplate → ProcessPrintLogData → SetBarcodeKey → SetSingleKey → singleTag → barCodeTag → objectTag?
- store.ReadTemplate(Path.GetFullPath(fileName))?
- result = true cuối try, false trong catch?

## 3. SetBarcodeKey
- Mỗi Barcode có ĐẦY ĐỦ 7 properties?
  - Alignment = CENTER?
  - IncludeLabel = true?
  - Width = 120?
  - Height = 40?
  - RotateFlipType = Rotate180FlipXY?
  - LabelPosition = BOTTOMCENTER?
  - EncodedType = CODE128?
- Null check string trước khi tạo Barcode?
- dicImage.Add(key, barcode) — key từ ExtendSingleKey?

## 4. SetSingleKey
- AddObjectKeyIntoListkey<T>(data, false) cho EFMODEL objects?
- SetSingleKey(KeyValue) cho datetime format, computed fields?
- KHÔNG query API trong SetSingleKey?
- Key constants từ ExtendSingleKey — KHÔNG hardcode string?

## 5. Repeating Rows
- objectTag.AddObjectData(store, "TagName", list) cho danh sách?
- AddRelationship cho master-detail (nếu có)?
- Tag name trong code KHỚP với template Excel?

## 6. PDO
- Kế thừa RDOBase?
- Constructor assign TẤT CẢ properties?
- Try-catch trong constructor?
- Properties public get/set?

## 7. Exception Handling
- ProcessData(): try-catch với result = false trong catch?
- SetBarcodeKey(): try-catch?
- SetSingleKey(): try-catch?
- LogSystem.Error(ex) trong catch?

## Output
[CRITICAL] Kế thừa sai, thiếu ProcessData override — file:line — fix
[HIGH] Barcode thiếu properties, query trong SetSingleKey — file:line — fix
[MEDIUM] Key hardcode, thiếu null check — file:line — fix
