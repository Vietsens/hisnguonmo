# JsonExtractor — Hướng dẫn cho Designer

Tool standalone để **phân tích file Excel template MPS** và sinh ra file `.json` skeleton sẵn sàng dùng.

## Cài đặt

1. Yêu cầu: **Windows + .NET Framework 4.5** trở lên (đã có sẵn trên Windows 8+).
2. Copy nguyên folder `bin/Release/` về máy. Folder chứa các file:
   - `JsonExtractor.exe`
   - `Inventec.Common.JsonExport.dll`
   - `FlexCel.dll`
   - `Newtonsoft.Json.dll`
   - `Inventec.Common.Logging.dll`
   - `log4net.dll`
3. Mở Command Prompt hoặc PowerShell, `cd` vào folder.

## Sử dụng cơ bản

```
JsonExtractor.exe <đường-dẫn-file-xlsx>
```

Ví dụ:
```
> JsonExtractor.exe C:\Templates\Mps000312.xlsx
[OK] Mps000312.xlsx -> C:\Templates\Mps000312.json
     8 single keys, 2 list bindings (sheets scanned: 1, cells with placeholder: 17)
```

Tool đọc file Excel → quét tất cả cell có placeholder `<#KEY;>` → sinh file `.json` cạnh file `.xlsx`.

## Tùy chọn

| Cờ | Tác dụng |
|---|---|
| `-o <path>` hoặc `--output <path>` | Chỉ định nơi save file JSON khác đường dẫn mặc định |
| `-v` hoặc `--verbose` | In chi tiết từng key tìm được |
| `--stdout` | In JSON ra màn hình thay vì save file |
| `--force` | Ghi đè file output đã tồn tại |
| `-h` hoặc `--help` | Xem trợ giúp |

## Ví dụ verbose mode

```
> JsonExtractor.exe Mps000312.xlsx -v
[OK] Mps000312.xlsx -> Mps000312.json
     8 single keys, 2 list bindings (sheets scanned: 1, cells with placeholder: 17)

Single keys:
  - PARENT_ORGANIZATION_NAME
  - ORGANIZATION_NAME
  - REQUEST_ROOM_NAME
  - TDL_PATIENT_DOB
  ...

List bindings:
  - SereServs (5 properties)
      .SERVICE_NAME
      .AMOUNT
      .SERVICE_UNIT_NAME
      .TOTAL_PRICE
      .INSTRUCTION_NOTE
  - ServiceReqs (1 properties)
      .EXECUTE_ROOM_NAME
```

## Output JSON skeleton

File output đã sẵn cú pháp `<#KEY;>` để renderer có thể chạy. Designer chỉ cần:

1. **Đổi tên key bên trái** (JSON field name) thành tên mong muốn cho output schema.
2. **Thêm pipe fallback** nếu cần: `"<#KEY_A;>|<#KEY_B;>"`.
3. **Thêm function** nếu cần: `"<#fmt(<#PRICE;>, \"N0\");>"`, `"<#date(<#TIME;>, \"dd/MM/yyyy\");>"`.
4. **Đánh dấu omit-if-empty** bằng suffix `?`: `"ghichu?": "<#NOTE;>"`.

Xem cú pháp đầy đủ ở [TEMPLATE_SYNTAX.md](..\Inventec.Common.JsonExport\TEMPLATE_SYNTAX.md).

### Ví dụ chuyển đổi

**Skeleton extract ra**:
```json
{
  "TDL_PATIENT_NAME": "<#TDL_PATIENT_NAME;>",
  "TDL_PATIENT_DOB": "<#TDL_PATIENT_DOB;>",
  "TOTAL_PRICE": "<#TOTAL_PRICE;>",
  "SereServs": [
    { "SERVICE_NAME": "<#SERVICE_NAME;>", "AMOUNT": "<#AMOUNT;>" }
  ]
}
```

**Designer chỉnh thành**:
```json
{
  "hoten": "<#TDL_PATIENT_NAME;>|<#VIR_PATIENT_NAME;>",
  "ngaysinh": "<#date(<#TDL_PATIENT_DOB;>, \"dd/MM/yyyy\");>",
  "tongtien": "<#fmt(<#TOTAL_PRICE;>, \"N0\");>",
  "danhsachdichvu": [
    {
      "tendichvu": "<#SERVICE_NAME;>",
      "soluong": "<#AMOUNT;>"
    }
  ]
}
```

**Lưu ý quan trọng**: nếu designer đổi tên list cấp 1 (vd `SereServs` → `danhsachdichvu`), thì developer cũng phải đổi tên trong code C#:
```csharp
RegisterListForJson("danhsachdichvu", rdo.SereServs.Cast<object>());
```

## Exit codes

- `0` — thành công
- `1` — sai cú pháp lệnh / hiển thị help
- `2` — không tìm thấy file xlsx
- `3` — lỗi khi extract
- `4` — không ghi được file output

## FAQ

**Q: Tool có sinh thiếu key không?**  
A: Tool quét tất cả cell có chứa `<#...;>`. Nếu Excel template có key đúng convention thì tool tìm hết.

**Q: Tool có nhầm tag điều khiển (như `<#delete row>`, `<#FlFunc...>`) thành key không?**  
A: Không. Strict regex chỉ accept identifier thuần (chữ + số + underscore), tự loại các tag có space/paren/special chars.

**Q: Inner placeholder bên trong FlFunc có được extract không?**  
A: Có. Ví dụ trong cell `<#FlFuncCalculateAge(<#DOB;>;tuoi)>` — `DOB` được extract, FlFunc bị skip.

**Q: Tool có handle sheet ẩn không?**  
A: Skip sheet tên `Template_Key` và `Config_Image` (do FlexCel code-generate). Sheet ẩn khác vẫn quét.

**Q: Output có giữ thứ tự key như trong Excel không?**  
A: Có — thứ tự đầu tiên xuất hiện trong khi quét (sheet→row→col).

**Q: List property có thứ tự thế nào?**  
A: Theo thứ tự xuất hiện đầu tiên cho mỗi tên list.
