---
name: suggest-uc
description: Phân tích form/UC → gợi ý UC có sẵn phù hợp từ 134 UCs → sinh code tích hợp
user-invocable: true
argument-hint: <file path hoặc mô tả chức năng VD: "form đăng ký bệnh nhân" hoặc "form kê đơn thuốc">
---

# Suggest UC — Gợi Ý User Control Phù Hợp

Target: $ARGUMENTS

## Bước 1: Phân tích nhu cầu

Đọc file hoặc mô tả chức năng, xác định các thành phần UI cần:

| Thành phần | UC tương ứng | Số plugins dùng |
|-----------|--------------|-----------------|
| Chẩn đoán chính | HIS.UC.Icd | 105 |
| Chẩn đoán phụ | HIS.UC.SecondaryIcd | 124 |
| Chọn ngày/giờ | HIS.UC.DateEditor | 25 |
| Nhập địa chỉ | HIS.UC.AddressCombo | 16 |
| Chọn bệnh nhân | HIS.UC.PatientSelect | 20 |
| Cây dịch vụ | HIS.UC.SereServTree / TreeSereServ7 | 32 / 19 |
| Thuốc tồn kho | HIS.UC.MedicineTypeInStock | 23 |
| Vật tư tồn kho | HIS.UC.MaterialTypeInStock | 23 |
| Lưới thuốc xuất | HIS.UC.ExpMestMedicineGrid | 60 |
| Lưới vật tư xuất | HIS.UC.ExpMestMaterialGrid | 54 |
| Chọn phòng | HIS.UC.Room | 16 |
| Chọn khoa | HIS.UC.Department | 14 |
| Chọn kho | HIS.UC.MediStock | 12 |
| Chọn dịch vụ | HIS.UC.Service | 21 |
| Nơi làm việc | HIS.UC.WorkPlace | 20 |
| Info điều trị | HIS.UC.TreatmentInfo | nhiều |
| Ra viện | HIS.UC.TreatmentFinish | 17 |
| Mẫu in | HIS.UC.MenuPrint | 22 |
| Phân trang | Inventec.UC.Paging | nhiều |
| Nhập BN | HIS.UC.UCPatientRaw | 23 |
| Người nhà | HIS.UC.UCRelativeInfo | nhiều |

## Bước 2: Kiểm tra form hiện tại

Nếu target là file:
- Đọc file .cs (KHÔNG Designer)
- Liệt kê controls đang dùng
- Tìm controls tự tạo mà UC đã có sẵn:
  - 3 ComboBox Tỉnh/Huyện/Xã → thay bằng HIS.UC.AddressCombo
  - DateEdit tự tạo → thay bằng HIS.UC.DateEditor
  - TextBox ICD tự tạo → thay bằng HIS.UC.Icd + SecondaryIcd
  - Grid bệnh nhân tự tạo → thay bằng HIS.UC.PatientSelect

## Bước 3: Tra cứu API từ uc_guide.md

Với mỗi UC gợi ý:
- Processor methods: Run, GetValue, SetValue, Reload, Validate, Reset
- InitADO properties quan trọng
- Delegates cần truyền
- Output type

## Bước 4: Sinh code tích hợp

### 4a. Declare fields
```csharp
private UserControl uc{UCShortName};
private {UC}Processor {ucShortName}Proc;
```

### 4b. Init trong Load (hoặc method riêng)
```csharp
private void Init{UCShortName}()
{
    try
    {
        {ucShortName}Proc = new {UC}Processor(new CommonParam());
        var ado = new {UC}InitADO();
        ado.Property1 = value1;
        ado.DelegateNextFocus = () => nextControl.Focus();
        uc{UCShortName} = (UserControl){ucShortName}Proc.Run(ado);
        panel{UCShortName}.Controls.Add(uc{UCShortName});
        uc{UCShortName}.Dock = DockStyle.Fill;
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

### 4c. GetValue trong Save
```csharp
var {ucShortName}Data = ({OutputADO}){ucShortName}Proc.GetValue(uc{UCShortName});
if ({ucShortName}Data != null)
{
    dto.FIELD = {ucShortName}Data.PROPERTY;
}
```

### 4d. Validate trong SaveProcess
```csharp
if (!(bool){ucShortName}Proc.ValidationXxx(uc{UCShortName})) return;
```

### 4e. Reload khi cần
```csharp
{ucShortName}Proc.Reload(uc{UCShortName}, newData);
```

### 4f. ReadOnly khi view mode
```csharp
{ucShortName}Proc.ReadOnly(uc{UCShortName}, ActionType == GlobalVariables.ActionView);
```

## Bước 5: Output

```
FORM: {path hoặc mô tả}

UC GỢI Ý:
1. HIS.UC.Icd (105 plugins dùng)
   Lý do: Form cần nhập chẩn đoán chính
   Panel: panelIcd (tạo trong Designer, Dock=Fill)
   Code: [Init + GetValue + Validate đầy đủ]

2. HIS.UC.SecondaryIcd (124 plugins dùng)
   Lý do: Luôn đi kèm Icd chính
   Panel: panelSubIcd
   Code: [Init + GetValue đầy đủ]

THAY THẾ:
- Line {n}: Tự tạo 3 ComboBox Tỉnh/Huyện/Xã → HIS.UC.AddressCombo
- Line {n}: Tự tạo DateEdit → HIS.UC.DateEditor
- Line {n}: Tự tạo Grid bệnh nhân → HIS.UC.PatientSelect
```
