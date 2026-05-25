# JSON Template Syntax — Hướng dẫn cho người tạo template

Tài liệu này dành cho **template designer** (người thiết kế file Excel + JSON cho từng mã in MPS).

## 1. Vị trí file template

- Excel template: `Tmp/Mps/Mps000312.xlsx`, `Mps0031201.xlsx`, ...
- JSON template: `Tmp/Mps/Mps000312.json`

Cách engine tìm file JSON theo thứ tự:
1. **Cùng tên Excel, đổi extension**: `Mps0031201.xlsx` → tìm `Mps0031201.json`.
2. **Prefix base print code** (cắt theo regex `^Mps\d{6}`): `Mps0031201` → tìm `Mps000312.json`.

→ Một file JSON có thể dùng chung cho nhiều variant Excel cùng prefix.

## 2. Cú pháp cơ bản — placeholder `<#KEY;>`

Mỗi giá trị JSON kiểu chuỗi có thể chứa placeholder `<#KEY;>`. Engine sẽ tra dictionary key đơn (`SetSingleKey(...)` trong processor) và thay thế.

```json
{
  "hoten": "<#patient_name;>"
}
```

## 3. Pipe fallback — `<#a;>|<#b;>|<#c;>`

Khi cùng 1 trường JSON có thể được fill bởi nhiều key khác nhau tùy processor, dùng dấu `|` để liệt kê các option theo thứ tự ưu tiên:

```json
{
  "hoten": "<#patient_name;>|<#vir_patient_name;>"
}
```

- Engine thử option 1 (`<#patient_name;>`); nếu có giá trị → dùng.
- Nếu null/empty → thử option 2.
- Hết option → output chuỗi rỗng.

## 4. Function — `<#FN(args);>`

Mỗi function nhận tham số phân tách bằng dấu phẩy. Tham số có thể là:
- **Quoted string**: `"text"`
- **Number**: `123`, `1.5`
- **Boolean**: `true`, `false`
- **Placeholder**: `<#KEY;>`
- **Mixed**: `Mã: <#code;>`
- **Raw name** (cho list/field): `services` (không cần quotes)

### Bảng function

| Function | Cú pháp | Mục đích |
|---|---|---|
| `evaluate` | `<#evaluate(<#a;>+<#b;>);>` | Biểu thức số học |
| `if` | `<#if(COND, THEN, ELSE);>` | Điều kiện — COND truthy thì THEN, không thì ELSE |
| `ifnull` | `<#ifnull(VALUE, DEFAULT);>` | VALUE rỗng → DEFAULT |
| `concat` | `<#concat(A, B, C);>` | Nối chuỗi |
| `sum` | `<#sum(LIST, FIELD);>` | Tổng property FIELD của list ADO |
| `count` | `<#count(LIST);>` | Đếm phần tử |
| `avg`/`min`/`max` | `<#avg(LIST, FIELD);>` | Trung bình/min/max |
| `fmt` | `<#fmt(<#price;>, "N0");>` | Format số (vi-VN) |
| `date` | `<#date(<#in_time;>, "dd/MM/yyyy");>` | Format ngày (input: long yyyyMMddHHmmss) |
| `substr` | `<#substr(<#code;>, 0, 3);>` | Cắt chuỗi |
| `upper`/`lower`/`trim` | `<#upper(<#name;>);>` | String transform |
| `cell` | `<#cell("Sheet1!E10");>` | Đọc cell sau khi FlexCel render (formula đã tính) |
| `named` | `<#named("total_price");>` | Đọc named range trên workbook |

### Shorthand `<#@name;>`

Tương đương `<#named("name");>`:

```json
{ "tongtien": "<#@total_price;>" }
```

### Shorthand `[[Sheet!A1]]`

Tương đương `<#cell("Sheet!A1");>`:

```json
{ "tongtien": "[[Sheet1!E10]]" }
```

## 5. Array loop

Array có **đúng 1 phần tử mẫu** sẽ được engine expand theo list ADO trùng tên.

```json
{
  "danhsachdichvu": [
    { "tendichvu": "<#name;>", "gia": "<#price;>" }
  ]
}
```

- Tên `danhsachdichvu` phải khớp tên list mà processor đã `RegisterListForJson("danhsachdichvu", listAdo)`.
- Trong fragment lặp, `<#name;>` và `<#price;>` được tra **property của từng item** (reflection, case-insensitive).
- Nếu list rỗng → array `[]`.

### Nested loop (list trong list)

Phần tử cha có property là 1 list → nested loop tự động hoạt động:

```json
{
  "khoa": [
    {
      "tenKhoa": "<#deptName;>",
      "danhSachBN": [
        { "tenBN": "<#patientName;>" }
      ]
    }
  ]
}
```

- `khoa` bind với list ADO `khoa` (top-level register).
- `danhSachBN` bind với property `DanhSachBN` (hoặc `Patients`...) trên từng item của list `khoa`.

## 6. Conditional omit — suffix `?`

Property có tên kết thúc bằng `?` sẽ bị **xoá** nếu giá trị render là rỗng. Nếu có giá trị → suffix `?` bị strip, property xuất hiện với tên gốc.

```json
{
  "hoten": "Nguyễn Văn A",
  "ghichu?": "<#note;>"
}
```

- Có `note` → output `"ghichu": "..."`.
- Không có `note` → output không có field `ghichu`.

Hữu ích cho field optional như BHYT, ghi chú, mã thẻ phụ...

## 7. Cell formula Excel — 3 cách lấy

Cell trong Excel có formula `=SUM(B5:B19)` không nằm trong `singleValueDictionary`. 3 cách đưa vào JSON:

**Cách 1 (khuyên dùng)**: Processor tự tính trong C# rồi `SetSingleKey("TOTAL", value)`. JSON ref:
```json
"tongtien": "<#TOTAL;>"
```

**Cách 2**: Dùng function tương đương:
```json
"tongtien": "<#sum(services, price);>"
```

**Cách 3**: Đọc cell trực tiếp sau FlexCel render (chỉ khi cell có địa chỉ cố định):
```json
"tongtien": "[[Sheet1!E10]]"
```
hoặc named range:
```json
"tongtien": "<#@total_price;>"
```

Có thể kết hợp qua pipe fallback (tốt nhất):
```json
"tongtien": "<#TOTAL;>|<#sum(services, price);>|<#@total_price;>|[[Sheet1!E10]]"
```

## 8. Type coercion

Nếu placeholder trả về **số/boolean**, output JSON sẽ là number/boolean (không có dấu nháy):

| Template | Dictionary | Output |
|---|---|---|
| `"gia": "<#price;>"` | `price = 200000` | `"gia": 200000` |
| `"flag": "<#active;>"` | `active = true` | `"flag": true` |
| `"name": "<#patient;>"` | `patient = "A"` | `"name": "A"` |

→ Template designer **không cần quan tâm** raw vs string context, engine tự xử lý.

## 9. Escape JSON

Engine tự động escape:
- `"` → `\"`
- `\` → `\\`
- newline, tab, control char

Bạn KHÔNG cần escape trong value — engine làm tự động.

## 10. Validation

Engine gọi `JToken.Parse(output)` sau khi render. Nếu output không phải JSON valid → log error, KHÔNG sinh file `.json` rỗng.

Nguyên nhân thường gặp khi validation fail:
- Quote chưa đóng trong template.
- Placeholder có ký tự lạ.
- Loop template không phải JObject.

## 11. Ví dụ template hoàn chỉnh

```json
{
  "hoten": "<#patient_name;>|<#vir_patient_name;>",
  "tuoi": "<#age;>",
  "gioitinh": "<#if(<#gender;>, \"Nam\", \"Nữ\");>",
  "ngaykham": "<#date(<#in_time;>, \"dd/MM/yyyy HH:mm\");>",
  "khoa": "<#dept_name;>",
  "bhyt?": "<#hein_card_no;>",
  "danhsachdichvu": [
    {
      "tendichvu": "<#name;>",
      "gia": "<#fmt(<#price;>, \"N0\");>",
      "soluong": "<#amount;>"
    }
  ],
  "tongtien": "<#total_price;>|<#sum(danhsachdichvu, price);>",
  "thanhtien": "<#evaluate(<#total_price_patient;>+<#total_price_bhyt;>);>",
  "hangbn": "<#if(<#evaluate(<#total_price;>>5000000);>, \"VIP\", \"Thường\");>",
  "ghichu?": "<#note;>"
}
```
