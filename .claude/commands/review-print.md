---
description: Review print integration — Print Library, PrintTypeCode, PDO, MpsPrinter, PreviewType, EMR sign
argument-hint: <file hoặc folder path>
---

# Review Print Integration

Review: $ARGUMENTS

## 1. Ưu Tiên Print Library
- Có sử dụng 1 trong 12 Print Libraries (PrintPrescription, PrintBordereau, PrintServiceReq...)?
- Nếu có Library phù hợp mà tự build PDO + MpsPrinter.Run → SAI — dùng Library
- Kiểm tra: constructor Library truyền ĐỦ tham số BẮT BUỘC?

## 2. PrintTypeCode
- PrintTypeCode là constant (PrintTypeCodeWorker.cs hoặc PrintTypeCodeStore)?
- KHÔNG hardcode string "Mps000102" trực tiếp?
- Format đúng: "Mps000XXX" (M hoa, ps thường, 6 số)?

## 3. Print Library Usage (nếu dùng Library)
- Constructor nhận đủ data (SDO, treatment, module)?
- Có set tùy chọn trước Print() (SetOutHospital, IsActionButtonPrintBill)?
- Gọi Print() với đúng overload (printCode, printNow, previewType)?

## 4. MpsPrinter.Run Trực Tiếp (nếu KHÔNG có Library)
- Có RichEditorStore.RunPrintTemplate() với callback?
- Callback switch printCode → method riêng cho mỗi Mps code?
- PDO tạo đúng namespace: MPS.Processor.{Code}.PDO.{Code}PDO?
- PDO constructor truyền đủ EFMODEL objects?
- PrintData tạo đúng: printTypeCode, fileName, pdo, previewType, printerName?

## 5. WaitingManager
- Show trước load data cho PDO?
- Hide TRƯỚC MpsPrinter.Run / Print()?
- Hide trong catch?

## 6. Printer Config
- Printer name từ GlobalVariables.dicPrinter[printTypeCode]? KHÔNG hardcode?
- PreviewType từ ConfigApplications.CheDoIn? KHÔNG hardcode?

## 7. EMR Sign (nếu cần)
- Có tạo EmrInputADO qua EmrGenerateProcessor?
- Set EmrInputADO trong PrintData?
- PreviewType đúng Emr* enum (EmrSignAndPrintNow, EmrShow...)?

## 8. Reference .csproj
- MPS PDO: `..\..\..\..\LIB\MPSv2\MPS.PDO\MPS.Processor.{Code}.PDO.dll`?
- MPS.ProcessorBase: `..\..\..\..\LIB\MPSv2\MPS.ProcessorBase\MPS.ProcessorBase.dll`?
- Print Library: `..\..\..\..\LIB\HIS\HIS.Desktop.Plugins.Library.Print*.dll`?

## Output
[CRITICAL] Tự build PDO khi có Print Library — file:line — dùng Library
[HIGH] Hardcode PrintTypeCode, thiếu WaitingManager.Hide — file:line — fix
[MEDIUM] Thiếu EMR sign, hardcode printer/preview — file:line — fix
[LOW] Thiếu reference PDO trong csproj — file:line — fix
