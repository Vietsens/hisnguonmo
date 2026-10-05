/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Template
{
    /// <summary>
    /// Mẫu 14 - Cấu hình chi tiết theo BVND 115 (key HIS.Desktop.Plugins.Library.ElectronicBill.Template = 14).
    /// Gom dịch vụ đã thanh toán thành tối đa 12 dòng theo loại dịch vụ HIS (HIS_SERVICE_TYPE), thứ tự cố định:
    ///   Khám bệnh; Xét nghiệm; Chẩn đoán hình ảnh; Thăm dò chức năng; Nội soi; Siêu âm; Giải phẫu bệnh;
    ///   Phẫu thuật - Thủ thuật (PT, TT, PHCN); Giường; Thuốc, dịch truyền; Vật tư y tế; Dịch vụ khác (các loại còn lại).
    /// Mỗi dòng: đơn vị "khoản", số lượng 1, đơn giá = thành tiền = tổng tiền bệnh nhân phải trả (PRICE) của nhóm.
    /// Nhóm không có dịch vụ thì không sinh dòng. Không phụ thuộc đối tượng BHYT cuối và không gọi API
    /// (luồng in hóa đơn nháp gọi thẳng TemplateFactory nên dữ liệu đó có thể chưa có).
    /// Giao dịch xuất bán nhà thuốc không đi qua mẫu này: ElectronicBillProcessor chuyển sang TemplateNhaThuoc khi key = 14.
    /// Khuôn lấy từ Template6 (gom 6 nhóm) - sửa lỗi ở Template6 cần soát lại mẫu này.
    /// </summary>
    class Template14 : IRunTemplate
    {
        private const string UNIT_NAME = "khoản";
        private const string OTHER_CODE = "DVKH";
        private const string OTHER_NAME = "Dịch vụ khác";

        private class GroupDef
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public long[] ServiceTypeIds { get; set; }
            /// <summary>true: dòng là hàng thuốc (ProductBase.Type = 1) - theo cách đánh dấu của Template6</summary>
            public bool IsMedicine { get; set; }
        }

        /// <summary>
        /// 11 nhóm có tên; mọi loại dịch vụ không nằm trong bảng (suất ăn, khác, máu, loại 0 của thanh toán khác...) -> "Dịch vụ khác".
        /// Thứ tự phần tử = thứ tự dòng trên hóa đơn.
        /// </summary>
        private static readonly GroupDef[] GROUPS = new GroupDef[]
        {
            new GroupDef { Code = "KB",    Name = "Khám bệnh",              ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH } },
            new GroupDef { Code = "XN",    Name = "Xét nghiệm",             ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN } },
            new GroupDef { Code = "CDHA",  Name = "Chẩn đoán hình ảnh",     ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA } },
            new GroupDef { Code = "TDCN",  Name = "Thăm dò chức năng",      ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN } },
            new GroupDef { Code = "NS",    Name = "Nội soi",                ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS } },
            new GroupDef { Code = "SA",    Name = "Siêu âm",                ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA } },
            new GroupDef { Code = "GPB",   Name = "Giải phẫu bệnh",         ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL } },
            new GroupDef { Code = "PTTT",  Name = "Phẫu thuật - Thủ thuật", ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN } },
            new GroupDef { Code = "G",     Name = "Giường",                 ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G } },
            new GroupDef { Code = "THUOC", Name = "Thuốc, dịch truyền",     ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC }, IsMedicine = true },
            new GroupDef { Code = "VTYT",  Name = "Vật tư y tế",            ServiceTypeIds = new long[] { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT } },
        };

        private Base.ElectronicBillDataInput DataInput;

        public Template14(Base.ElectronicBillDataInput dataInput)
        {
            this.DataInput = dataInput;
        }

        public object Run()
        {
            List<ProductBase> result = new List<ProductBase>();
            try
            {
                if (DataInput.SereServBill != null && DataInput.SereServBill.Count > 0)
                {
                    //Xếp từng dòng dịch vụ vào nhóm theo loại dịch vụ; không khớp nhóm nào -> Dịch vụ khác
                    Dictionary<string, List<HIS_SERE_SERV_BILL>> dicGroupLines = new Dictionary<string, List<HIS_SERE_SERV_BILL>>();
                    foreach (var sereServ in DataInput.SereServBill)
                    {
                        long serviceTypeId = sereServ.TDL_SERVICE_TYPE_ID ?? 0;
                        GroupDef group = GROUPS.FirstOrDefault(g => g.ServiceTypeIds.Contains(serviceTypeId));
                        string groupCode = group != null ? group.Code : OTHER_CODE;
                        if (!dicGroupLines.ContainsKey(groupCode))
                        {
                            dicGroupLines[groupCode] = new List<HIS_SERE_SERV_BILL>();
                        }
                        dicGroupLines[groupCode].Add(sereServ);
                    }

                    //Sinh dòng theo thứ tự cố định, nhóm không có dịch vụ thì bỏ
                    foreach (GroupDef group in GROUPS)
                    {
                        if (dicGroupLines.ContainsKey(group.Code))
                        {
                            result.Add(CreateProduct(group.Code, group.Name, dicGroupLines[group.Code], group.IsMedicine));
                        }
                    }

                    if (dicGroupLines.ContainsKey(OTHER_CODE))
                    {
                        result.Add(CreateProduct(OTHER_CODE, OTHER_NAME, dicGroupLines[OTHER_CODE], false));
                    }
                }

                if (result.Count > 0)
                {
                    foreach (var product in result)
                    {
                        //Làm tròn theo cấu hình - số lượng luôn 1 nên giữ đơn giá = thành tiền
                        if (HisConfigCFG.RoundTransactionAmountOption == "1" || HisConfigCFG.RoundTransactionAmountOption == "2")
                        {
                            product.Amount = Math.Round(product.Amount, 0, MidpointRounding.AwayFromZero);
                            product.ProdPrice = product.Amount;
                        }

                        if (HisConfigCFG.IsHidePrice)
                        {
                            product.ProdPrice = null;
                        }

                        if (HisConfigCFG.IsHideQuantity)
                        {
                            product.ProdQuantity = null;
                        }

                        if (HisConfigCFG.IsHideUnitName)
                        {
                            product.ProdUnit = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private static ProductBase CreateProduct(string code, string name, List<HIS_SERE_SERV_BILL> lines, bool isMedicine)
        {
            decimal amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(lines.Sum(s => s.PRICE));

            ProductBase product = new ProductBase();
            product.ProdCode = code;
            product.ProdName = name;
            product.ProdUnit = UNIT_NAME;
            product.ProdQuantity = 1;
            product.ProdPrice = amount;
            product.Amount = amount;
            product.TaxRateID = Base.ProviderType.tax_KCT;
            product.Type = isMedicine ? 1 : 0;
            return product;
        }
    }
}
