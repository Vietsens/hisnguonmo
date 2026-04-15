---
description: Tìm User Control phù hợp theo chức năng tiếng Việt — trả về UC name, Processor API, code mẫu
argument-hint: <chức năng VD: chọn ngày, chẩn đoán, địa chỉ, thuốc tồn kho, chọn bệnh nhân>
---

# Tìm User Control

Tìm UC cho: $ARGUMENTS

## Bước 1: Map keyword

| Tiếng Việt | UC |
|-----------|-----|
| Chẩn đoán chính | HIS.UC.Icd (105 plugins) |
| Chẩn đoán phụ | HIS.UC.SecondaryIcd (124 plugins) |
| Chọn ngày/giờ | HIS.UC.DateEditor (25 plugins) |
| Địa chỉ Tỉnh/Huyện/Xã | HIS.UC.AddressCombo (16 plugins) |
| Chọn bệnh nhân | HIS.UC.PatientSelect (20 plugins) |
| Cây dịch vụ | HIS.UC.SereServTree (32 plugins) |
| Thuốc xuất/nhập | HIS.UC.ExpMestMedicineGrid (60 plugins) |
| Vật tư xuất/nhập | HIS.UC.ExpMestMaterialGrid (54 plugins) |
| Thuốc tồn kho | HIS.UC.MedicineTypeInStock (23 plugins) |
| Vật tư tồn kho | HIS.UC.MaterialTypeInStock (23 plugins) |
| Chọn dịch vụ | HIS.UC.Service (21 plugins) |
| Nơi làm việc | HIS.UC.WorkPlace (20 plugins) |
| Cây DV v7 | HIS.UC.TreeSereServ7 (19 plugins) |
| Ra viện | HIS.UC.TreatmentFinish (17 plugins) |
| Chọn phòng | HIS.UC.Room (16 plugins) |
| Chọn khoa | HIS.UC.Department (14 plugins) |
| Chọn kho | HIS.UC.MediStock (12 plugins) |
| Mẫu in | HIS.UC.MenuPrint (22 plugins) |
| Nhập BN | HIS.UC.UCPatientRaw (23 plugins) |
| Info điều trị | HIS.UC.TreatmentInfo |
| Người nhà | HIS.UC.UCRelativeInfo |
| Giường | HIS.UC.Bed |
| Máy thiết bị | HIS.UC.Machine |
| Chỉ số XN | HIS.UC.TestIndex |
| Loại mẫu XN | HIS.UC.TestSample |
| Quốc gia | HIS.UC.National |
| Phân trang | Inventec.UC.Paging |

## Bước 2: Đọc uc_guide.md để lấy Processor API

Tra cứu Processor methods: Run, GetValue, SetValue, Reload, FocusControl, ReadOnly, Validate, Reset.

## Bước 3: Tra code mẫu

```csharp
var proc = new {UC}Processor(commonParam);
var ado = new {UC}InitADO();
// Set properties...
UserControl uc = (UserControl)proc.Run(ado);
panel.Controls.Add(uc);
uc.Dock = DockStyle.Fill;
```

## Bước 4: Tìm plugins đang dùng UC này

Search `using {UC.Namespace}` trong HIS/Plugins/ để xem code mẫu thật.
