/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.XMLViewer130.Bhxh
{
    public class CategoryTypeADO
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string LogName { get; set; }
        public string LoaiHs { get; set; }
        public string EndpointPath { get; set; }
        public bool RequireKyQT { get; set; }

        public static List<CategoryTypeADO> GetAll()
        {
            return new List<CategoryTypeADO>
            {
                new CategoryTypeADO
                {
                    Code = "MAU01_DM",
                    DisplayName = "Mẫu 01/DM – Bộ phận chuyên môn khám bệnh, chữa bệnh BHYT",
                    LogName = "Mẫu 01/DM Bộ phận chuyên môn khám bệnh, chữa bệnh bảo hiểm y tế",
                    LoaiHs = "70",
                    EndpointPath = "/api/DanhMucGW/GuiDanhMuc01_BPCMKBCB",
                    RequireKyQT = false
                },
                new CategoryTypeADO
                {
                    Code = "MAU02_DM",
                    DisplayName = "Mẫu 02/DM – Nhân lực thực hiện khám bệnh, chữa bệnh BHYT",
                    LogName = "Mẫu 02/DM Nhân lực thực hiện khám bệnh, chữa bệnh bảo hiểm y tế",
                    LoaiHs = "71",
                    EndpointPath = "/api/DanhMucGW/GuiDanhMuc02_NLKCB",
                    RequireKyQT = false
                },
                new CategoryTypeADO
                {
                    Code = "MAU03_DM",
                    DisplayName = "Mẫu 03/DM – Thuốc, máu, chế phẩm máu",
                    LogName = "Mẫu 03/DM Thuốc, máu, chế phẩm máu",
                    LoaiHs = "10",
                    EndpointPath = "/api/DanhMucGW/GuiDanhMuc03_DMTHUOC",
                    RequireKyQT = false
                },
                new CategoryTypeADO
                {
                    Code = "MAU04_DM",
                    DisplayName = "Mẫu 04/DM – Vật tư y tế",
                    LogName = "Mẫu 04/DM Vật tư y tế",
                    LoaiHs = "11",
                    EndpointPath = "/api/DanhMucGW/GuiDanhMuc04_DMVTYT",
                    RequireKyQT = false
                },
                new CategoryTypeADO
                {
                    Code = "MAU05_DM",
                    DisplayName = "Mẫu 05/DM – Dịch vụ kỹ thuật",
                    LogName = "Mẫu 05/DM Dịch vụ kỹ thuật",
                    LoaiHs = "12",
                    EndpointPath = "/api/DanhMucGW/GuiDanhMuc05_DMDVKT",
                    RequireKyQT = false
                },
                new CategoryTypeADO
                {
                    Code = "MAU06_DM",
                    DisplayName = "Mẫu 06/DM – Trang thiết bị y tế",
                    LogName = "Mẫu 06/DM Trang thiết bị y tế",
                    LoaiHs = "72",
                    EndpointPath = "/api/DanhMucGW/GuiDanhMuc06_DMTBYT",
                    RequireKyQT = false
                },
                new CategoryTypeADO
                {
                    Code = "HSTH_01BH",
                    DisplayName = "Hồ sơ tổng hợp mẫu 01/BH",
                    LogName = "Hồ sơ tổng hợp mẫu 01/BH",
                    LoaiHs = "5",
                    EndpointPath = "/api/HoSoTongHop7980/GuiHoSoTongHop01BH",
                    RequireKyQT = true
                }
            };
        }
    }
}
