---
name: scaffold-mps
description: Tạo MPS Processor mới — PDO (RDOBase), Processor (AbstractProcessor), ExtendSingleKey, SetBarcodeKey đầy đủ, template keys
user-invocable: true
argument-hint: <Mps code + mô tả VD: "Mps000999 in phiếu xuất thuốc" hoặc "Mps000888 giấy ra viện">
---

# Scaffold MPS Processor

Target: $ARGUMENTS

## Bước 1: Xác định yêu cầu

- Mps code: Mps{NNNNNN}
- In gì? (phiếu khám, đơn thuốc, phiếu xuất, giấy ra viện, báo cáo...)
- Data nào? (treatment, patient, sereserv, serviceReq, expMest...)
- Template: Excel (FlexCel) hay Word (RichEditor)?

## Bước 2: Tìm processor tương tự

Search trong MPS/MPS.Processor/ tìm processor cùng loại để làm mẫu:
```
Grep "{EntityName}" trong MPS.Processor/ → tìm processor dùng entity tương tự
```
Đọc processor mẫu → hiểu data flow và keys.

## Bước 3: Tạo cấu trúc files

```
MPS/MPS.Processor/
├── MPS.Processor.Mps{Code}/
│   ├── Mps{Code}Processor.cs         ← Override ProcessData
│   └── Mps{Code}ExtendSingleKey.cs   ← Key constants
│
└── MPS.Processor.Mps{Code}.PDO/
    ├── Mps{Code}PDO.cs               ← Kế thừa RDOBase
    └── Mps{Code}ADO.cs               ← ADO phụ (nếu cần)
```

## Bước 4: Sinh PDO

```csharp
namespace MPS.Processor.Mps{Code}.PDO
{
    public class Mps{Code}PDO : RDOBase
    {
        public V_HIS_SERVICE_REQ ServiceReq { get; set; }
        public V_HIS_PATIENT currentPatient { get; set; }
        public HIS_TREATMENT currentTreatment { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        public List<{ListItemType}> sereServs { get; set; }
        // Thêm properties theo yêu cầu...

        public Mps{Code}PDO() { }

        public Mps{Code}PDO(
            V_HIS_SERVICE_REQ serviceReq,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            V_HIS_PATIENT currentPatient,
            List<{ListItemType}> sereServs,
            HIS_TREATMENT treatment)
        {
            try
            {
                this.ServiceReq = serviceReq;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentPatient = currentPatient;
                this.sereServs = sereServs;
                this.currentTreatment = treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
```

## Bước 5: Sinh ExtendSingleKey

```csharp
namespace MPS.Processor.Mps{Code}
{
    class Mps{Code}ExtendSingleKey
    {
        // Barcode keys
        public const string BARCODE_PATIENT_CODE = "BARCODE_PATIENT_CODE";
        public const string BARCODE_TREATMENT_CODE = "BARCODE_TREATMENT_CODE";
        public const string BARCODE_SERVICE_REQ_CODE = "BARCODE_SERVICE_REQ_CODE";

        // DateTime format keys
        public const string FINISH_TIME_STR = "FINISH_TIME_STR";
        public const string START_TIME_STR = "START_TIME_STR";
        public const string CREATE_TIME_STR = "CREATE_TIME_STR";

        // Computed keys (thêm theo yêu cầu)
        // public const string TOTAL_AMOUNT = "TOTAL_AMOUNT";
    }
}
```

## Bước 6: Sinh Processor

```csharp
namespace MPS.Processor.Mps{Code}
{
    class Mps{Code}Processor : AbstractProcessor
    {
        Mps{Code}PDO.Mps{Code}PDO rdo;

        public Mps{Code}Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps{Code}PDO.Mps{Code}PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag =
                    new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag =
                    new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag =
                    new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessPrintLogData();
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetBarcodeKey();
                SetSingleKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "SereServ", rdo.sereServs);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.ServiceReq != null)
                {
                    if (!String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatientCode =
                            new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.TDL_PATIENT_CODE);
                        barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatientCode.IncludeLabel = true;
                        barcodePatientCode.Width = 120;
                        barcodePatientCode.Height = 40;
                        barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        dicImage.Add(Mps{Code}ExtendSingleKey.BARCODE_PATIENT_CODE, barcodePatientCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.ServiceReq.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment =
                            new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = true;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        dicImage.Add(Mps{Code}ExtendSingleKey.BARCODE_TREATMENT_CODE, barcodeTreatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                // Tự động map TẤT CẢ properties → template keys
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.currentPatient, false);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentTreatment, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReq, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);

                // Thủ công cho computed/format fields
                if (rdo.ServiceReq != null)
                {
                    SetSingleKey(new KeyValue(
                        Mps{Code}ExtendSingleKey.FINISH_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                            rdo.ServiceReq.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(
                        Mps{Code}ExtendSingleKey.START_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                            rdo.ServiceReq.START_TIME ?? 0)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
```

## Bước 7: Template keys mapping

| Loại | Key trong template | Nguồn |
|------|-------------------|-------|
| Auto property | `{PATIENT_NAME}` | AddObjectKeyIntoListkey (TDL_ strip) |
| DateTime format | `{FINISH_TIME_STR}` | SetSingleKey(KeyValue) |
| Barcode image | `{BARCODE_PATIENT_CODE}` | dicImage.Add |
| Repeating row | `{SERVICE_NAME}` (trong row) | objectTag.AddObjectData "SereServ" |

## Bước 8: Verify

- [ ] PDO kế thừa RDOBase
- [ ] Processor kế thừa AbstractProcessor (KHÔNG ProcessorBase)
- [ ] Constructor: base(param, printData) + cast rdoBase
- [ ] Override ProcessData() (KHÔNG Run())
- [ ] Flow: ReadTemplate → Log → Barcode → SingleKey → Process → ObjectData
- [ ] SetBarcodeKey: mỗi barcode ĐẦY ĐỦ 7 properties
- [ ] SetSingleKey: AddObjectKeyIntoListkey + KeyValue cho format
- [ ] ExtendSingleKey: KHÔNG hardcode string trong Processor
- [ ] Try-catch trong ProcessData, SetBarcodeKey, SetSingleKey
- [ ] result = true cuối try, false = catch
