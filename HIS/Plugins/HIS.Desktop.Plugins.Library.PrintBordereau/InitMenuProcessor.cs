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
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using HIS.UC.MenuPrint.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau
{
    public partial class PrintBordereauProcessor
    {
        public void InitMenu(PrintOption.Value? printOption = null)
        {
            try
            {
                bool isBHYT = false;
                bool isVienPhi = false;
                CheckBordereauType(ref isBHYT, ref isVienPhi);

                if (this.ReLoadMenuOption != null && printOption.HasValue && (printOption.Value == PrintOption.Value.INIT_MENU
                    || printOption.Value == PrintOption.Value.PRINT_NOW_AND_INIT_MENU))
                {

                    if (this.ReLoadMenuOption.Type == ReloadMenuOption.MenuType.NORMAL)
                    {
                        this.InitMenuNormal(isBHYT, isVienPhi);
                    }
                    else if (this.ReLoadMenuOption.Type == ReloadMenuOption.MenuType.DYNAMIC)
                    {
                        this.InitMenuDynamic(isBHYT, isVienPhi);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMenuOption(bool isBHYT, bool isVienPhi, BordereauPrint.Type bordereauPrintType)
        {
            try
            {
                if (bordereauPrintType == BordereauPrint.Type.MPS_BASE)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMenuNormal(bool isBHYT, bool isVienPhi)
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();

                #region MenuOption
                if (this.ReLoadMenuOption != null && this.ReLoadMenuOption.BordereauPrint.HasValue && this.ReLoadMenuOption.BordereauPrint.Value == BordereauPrint.Type.MPS_BASE)
                {
                    if (this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        if (isBHYT)
                        {
                            this.MenuNormalReplaceMps("Bảng kê BHYT nội trú", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT);
                        }
                        if (isVienPhi)
                        {
                            this.MenuNormalReplaceMps("Bảng kê viện phí nội trú", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI);
                        }
                    }
                    else if (this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        if (isBHYT)
                        {
                            this.MenuNormalReplaceMps("Bảng kê BHYT ngoại trú", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT);
                        }
                        if (isVienPhi)
                        {
                            this.MenuNormalReplaceMps("Bảng kê viện phí ngoại trú", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI);
                        }
                    }
                    return;
                }
                #endregion

                if (this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                {
                    if (isBHYT)
                    {
                        this.CreateMenuNormal("Bảng kê BHYT nội trú", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT);
                        this.CreateMenuNormal("Bảng kê BHYT nội trú - Hao phí", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__HAO_PHI);
                        this.CreateMenuNormal("Bảng kê BHYT nội trú - TPTB", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__BHYT__TPTB);
                        this.CreateMenuNormal("Bảng kê BHYT nội trú - chưa thanh toán", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__CHUA_THANH_TOAN);
                        this.CreateMenuNormal("Bảng kê BHYT nội trú (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT);
                        this.CreateMenuNormal("Bảng kê BHYT nội trú theo khoa (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT__THEO_KHOA);
                        this.CreateMenuNormal("Bảng kê tổng hợp nội trú BHYT ", PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP__NOI_TRU__BHYT);
                        this.CreateMenuNormal("Bảng kê theo khoa BHYT", PrintTypeCodeWorker.PRINT_TYPE_CODE___THEO_KHOA___BHYT);
                        this.CreateMenuNormal("Bảng kê BHYT nội trú (6556/QĐ-BYT) tách stent", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT_STENT_2);
                    }
                    if (isVienPhi)
                    {
                        this.CreateMenuNormal("Bảng kê viện phí nội trú", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI);
                        this.CreateMenuNormal("Bảng kê viện phí nội trú - Hao phí", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI__HAO_PHI);
                        this.CreateMenuNormal("Bảng kê viện phí nội trú - TPTB", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__VIEN_PHI__TPTB);
                        this.CreateMenuNormal("Bảng kê viện phí nội trú - chưa thanh toán", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__CHUA_THANH_TOAN);
                        this.CreateMenuNormal("Bảng kê viện phí nội trú (100%) - chưa thanh toán", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI_100__CHUA_THANH_TOAN);
                        this.CreateMenuNormal("Bảng kê viện phí nội trú (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__6556_QĐ_BYT);
                        this.CreateMenuNormal("Bảng kê viện phí nội trú theo khoa (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__6556_QĐ_BYT__THEO_KHOA);

                    }
                    this.CreateMenuNormal("Bảng kê tổng hợp nội trú - Hao phí ", PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_NOI_TRU__HAO_PHI);
                    this.CreateMenuNormal("Bảng kê nội trú - Hao phí", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__HAO_PHI);
                    this.CreateMenuNormal("Bảng kê nội trú thuốc, vật tư chương trình", PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_THUOC_VATTU_CHUONG_TRINH__6556_QĐ_BYT);
                    this.CreateMenuNormal("Bảng kê nội trú- Hao phí theo khoa (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI_THEO_KHOA__6556_QĐ_BYT);
                }
                else //if (this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM || this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                {
                    if (isBHYT)
                    {
                        this.CreateMenuNormal("Bảng kê BHYT ngoại trú", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT);
                        this.CreateMenuNormal("Bảng kê BHYT ngoại trú - Hao phí", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__HAO_PHI);
                        this.CreateMenuNormal("Bảng kê BHYT ngoại trú - TPTB", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__BHYT__TPTB);
                        this.CreateMenuNormal("Bảng kê BHYT ngoại trú - chưa thanh toán", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__CHUA_THANH_TOAN);
                        this.CreateMenuNormal("Bảng kê BHYT ngoại trú (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__6556_QĐ_BYT);
                        this.CreateMenuNormal("Bảng kê BHYT ngoại trú theo khoa (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__6556_QĐ_BYT__THEO_KHOA);
                        this.CreateMenuNormal("Bảng kê tổng hợp BHYT ngoại trú", PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP__NGOAI_TRU__BHYT);
                        this.CreateMenuNormal("Bảng kê theo khoa BHYT", PrintTypeCodeWorker.PRINT_TYPE_CODE___THEO_KHOA___BHYT);
                    }
                    if (isVienPhi)
                    {
                        this.CreateMenuNormal("Bảng kê viện phí ngoại trú", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI);
                        this.CreateMenuNormal("Bảng kê viện phí ngoại trú - Hao phí", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI__HAO_PHI);
                        this.CreateMenuNormal("Bảng kê viện phí ngoại trú - TPTB", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__VIEN_PHI__TPTB);
                        this.CreateMenuNormal("Bảng kê viện phí ngoại trú - chưa thanh toán", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__CHUA_THANH_TOAN);
                        this.CreateMenuNormal("Bảng kê viện phí ngoại trú (100%) - chưa thanh toán", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI_100__CHUA_THANH_TOAN);
                        this.CreateMenuNormal("Bảng kê viện phí ngoại trú (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__6556_QĐ_BYT);
                        this.CreateMenuNormal("Bảng kê viện phí ngoại trú theo khoa (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__6556_QĐ_BYT);
                    }

                    this.CreateMenuNormal("Bảng kê tổng hợp ngoại trú - Hao phí ", PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_NGOAI_TRU__HAO_PHI);
                    this.CreateMenuNormal("Bảng kê ngoại trú - Hao phí", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI);
                    this.CreateMenuNormal("Bảng kê ngoại trú thuốc, vật tư chương trình", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_THUOC_VATTU_CHUONG_TRINH__6556_QĐ_BYT);
                    this.CreateMenuNormal("Bảng kê ngoại trú - Hao phí theo khoa (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI_THEO_KHOA__6556_QĐ_BYT);
                }

                this.CreateMenuNormal("Bảng kê tổng hợp", PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP);
                this.CreateMenuNormal("Bảng kê tổng hợp có giá gói", PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_GOI);
                this.CreateMenuNormal("Bảng kê viện phí tổng hợp", PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_VIEN_PHI_TONG_HOP);
                this.CreateMenuNormal("Bảng kê gói kỹ thuật cao", PrintTypeCodeWorker.PRINT_TYPE_CODE___TRONG_GOI_KY_THUAT_CAO);
                this.CreateMenuNormal("Bảng kê theo khoa", PrintTypeCodeWorker.PRINT_TYPE_CODE___THEO_KHOA);
                this.CreateMenuNormal("In giấy phụ thu", PrintTypeCodeWorker.PRINT_TYPE_CODE___IN_GIAY_PHU_THU);
                this.CreateMenuNormal("Bảng kê 6556 tổng hợp", PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_6556_TONG_HOP);
                this.CreateMenuNormal("Bảng kê tổng hợp có giá gói (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_6556_TONG_HOP_GOI);
                this.CreateMenuNormal("Bảng kê tổng hợp theo khoa (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_6556__THEO_KHOA);
                this.CreateMenuNormal("Bảng kê tổng hợp theo khoa phòng thanh toán (6556/QĐ-BYT)", PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_6556__THEO_KHOA_PHONG_THANH_TOAN);
                this.CreateMenuNormal("Bảng kê đối tượng khác", PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_DOI_TUONG_KHAC);
                this.CreateMenuNormal("Yêu cầu thanh toán", PrintTypeCodeWorker.PRINT_TYPE_CODE___YEU_CAU_THANH_TOAN);
                this.CreateMenuNormal("Bảng kê 6556 theo loại dịch vụ", PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_6556_THEO_LOAI_DICH_VU);

                if (this.Treatment.FUND_ID.HasValue)
                {
                    this.CreateMenuNormal("Bảng kê tổng hợp chi phí dịch vụ của đơn vị cùng chi trả", PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_CCT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitMenuDynamic(bool isBHYT, bool isVienPhi)
        {
            try
            {
                List<MenuPrintADO> menuPrints = new List<MenuPrintADO>();
                Inventec.Common.Logging.LogSystem.Debug("InitMenuDynamic. 1");
                #region MenuOption
                if (this.ReLoadMenuOption != null && this.ReLoadMenuOption.BordereauPrint.HasValue && this.ReLoadMenuOption.BordereauPrint.Value == BordereauPrint.Type.MPS_BASE)
                {
                    Inventec.Common.Logging.LogSystem.Debug("InitMenuDynamic. 2");
                    if (this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("InitMenuDynamic. 3");
                        if (isBHYT)
                        {
                            menuPrints.Add(this.MenuDynamicReplaceMps(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT));
                        }
                        if (isVienPhi)
                        {
                            menuPrints.Add(this.MenuDynamicReplaceMps(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI));
                        }
                    }
                    else if (this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("InitMenuDynamic. 4");
                        if (isBHYT)
                        {
                            menuPrints.Add(this.MenuDynamicReplaceMps(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT));
                        }
                        if (isVienPhi)
                        {
                            menuPrints.Add(this.MenuDynamicReplaceMps(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI));
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug("InitMenuDynamic. 5");
                    if (this.ReLoadMenuOption.ReloadMenu != null)
                    {
                        this.ReLoadMenuOption.ReloadMenu(menuPrints);
                    }
                    Inventec.Common.Logging.LogSystem.Debug("InitMenuDynamic. 6");
                    return;
                }
                #endregion
                Inventec.Common.Logging.LogSystem.Debug("InitMenuDynamic. 7");
                if (this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                {
                    if (isBHYT)
                    {
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__HAO_PHI));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__BHYT__TPTB));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__CHUA_THANH_TOAN));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT__THEO_KHOA));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP__NOI_TRU__BHYT));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___THEO_KHOA___BHYT));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT_STENT_2));
                    }
                    if (isVienPhi)
                    {
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI__HAO_PHI));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__VIEN_PHI__TPTB));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__CHUA_THANH_TOAN));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI_100__CHUA_THANH_TOAN));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__6556_QĐ_BYT));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__6556_QĐ_BYT__THEO_KHOA));

                    }
                    menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_NOI_TRU__HAO_PHI));
                    menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__HAO_PHI));
                    menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_THUOC_VATTU_CHUONG_TRINH__6556_QĐ_BYT));
                    menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__HAO_PHI_THEO_KHOA__6556_QĐ_BYT));
                }
                else //if (this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM || this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                {
                    if (isBHYT)
                    {
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__HAO_PHI));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__BHYT__TPTB));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__CHUA_THANH_TOAN));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__6556_QĐ_BYT));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__6556_QĐ_BYT__THEO_KHOA));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP__NGOAI_TRU__BHYT));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___THEO_KHOA___BHYT));
                    }
                    if (isVienPhi)
                    {
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI__HAO_PHI));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__VIEN_PHI__TPTB));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__CHUA_THANH_TOAN));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI_100__CHUA_THANH_TOAN));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__6556_QĐ_BYT));
                        menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__6556_QĐ_BYT__THEO_KHOA));
                    }

                    menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_NGOAI_TRU__HAO_PHI));
                    menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI));
                    menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_THUOC_VATTU_CHUONG_TRINH__6556_QĐ_BYT));
                    menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI_THEO_KHOA__6556_QĐ_BYT));
                }
                Inventec.Common.Logging.LogSystem.Debug("InitMenuDynamic. 8");
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_GOI));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_VIEN_PHI_TONG_HOP));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___TRONG_GOI_KY_THUAT_CAO));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___THEO_KHOA));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___IN_GIAY_PHU_THU));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_6556_TONG_HOP));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_6556_TONG_HOP_GOI));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_6556__THEO_KHOA));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_6556__THEO_KHOA_PHONG_THANH_TOAN));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_DOI_TUONG_KHAC));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___YEU_CAU_THANH_TOAN));
                menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_6556_THEO_LOAI_DICH_VU));
                Inventec.Common.Logging.LogSystem.Debug("InitMenuDynamic. 9");
                if (this.Treatment != null && this.Treatment.FUND_ID.HasValue)
                {
                    menuPrints.Add(this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_CCT));
                }

                if (this.ReLoadMenuOption.ReloadMenu != null)
                {
                    this.ReLoadMenuOption.ReloadMenu(menuPrints);
                }
                Inventec.Common.Logging.LogSystem.Debug("InitMenuDynamic. 10");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MenuNormalReplaceMps(string name, string mpsCode)
        {
            try
            {
                if (this.dicMpsReplace != null && this.dicMpsReplace.ContainsKey(mpsCode))
                    this.CreateMenuNormal(name, this.dicMpsReplace[mpsCode]);
                else
                    this.CreateMenuNormal(name, mpsCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private MenuPrintADO MenuDynamicReplaceMps(string mpsCode)
        {
            MenuPrintADO menuPrintADO = null;
            try
            {
                if (this.dicMpsReplace != null && this.dicMpsReplace.ContainsKey(mpsCode))
                    menuPrintADO = this.CreateMenuDynamic(this.dicMpsReplace[mpsCode]);
                else
                    menuPrintADO = this.CreateMenuDynamic(mpsCode);
            }
            catch (Exception ex)
            {
                menuPrintADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return menuPrintADO;
        }


        private MenuPrintADO CreateMenuDynamic(string PrintTypeCode)
        {
            MenuPrintADO menuPrintADO = null;
            try
            {
                menuPrintADO = new MenuPrintADO();
                menuPrintADO.EventHandler = new EventHandler(this.PrintEventFromMenu);
                menuPrintADO.ItemClickEventHandler = new ItemClickEventHandler(PrintEventFromBar);
                menuPrintADO.PrintTypeCode = PrintTypeCode;
                menuPrintADO.Tag = PrintTypeCode;
            }
            catch (Exception ex)
            {
                menuPrintADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return menuPrintADO;
        }

        private void CreateMenuNormal(string namePrint, string printTypeCode)
        {
            try
            {
                DXMenuItem itemMenu = new DXMenuItem(namePrint,
                                new EventHandler(this.PrintEventFromMenu));
                itemMenu.Tag = printTypeCode;
                if (this.ReLoadMenuOption.ReloadMenu != null)
                {
                    this.ReLoadMenuOption.ReloadMenu(itemMenu);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void PrintEventFromMenu(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();

                string mpsCode = null;
                if (sender is DXMenuItem)
                {
                    var btn = sender as DXMenuItem;
                    mpsCode = (string)(btn.Tag);
                }
                else if (sender is DevExpress.XtraEditors.SimpleButton)
                {
                    var btn = sender as DevExpress.XtraEditors.SimpleButton;
                    mpsCode = (string)(btn.Tag);
                }
                if (String.IsNullOrEmpty(mpsCode))
                    throw new Exception("MpsCode is null");

                GlobalDataStore.CURRENT_PRINT_OPTION = PrintOption.Value.SHOW_DIALOG;

                RunPrint(mpsCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintEventFromBar(object sender, ItemClickEventArgs e)
        {
            try
            {
                this.LoadData();

                string mpsCode = e.Item.Tag.ToString();

                if (String.IsNullOrEmpty(mpsCode))
                    throw new Exception("MpsCode is null");
                GlobalDataStore.CURRENT_PRINT_OPTION = PrintOption.Value.SHOW_DIALOG;
                RunPrint(mpsCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
