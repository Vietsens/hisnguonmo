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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Core;
using HIS.Desktop.Utility;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.TreatmentFinish.Config;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.SecondaryIcd.ADO;
using System.Drawing;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    public partial class FormTreatmentFinish : HIS.Desktop.Utility.FormBase
    {
        int popupHeight = 400;
        private void SetupPrintConfig()
        {
            try
            {
                DevExpress.Utils.Menu.DXPopupMenu menu_print = new DevExpress.Utils.Menu.DXPopupMenu();
                printConfigADOs = new List<PrintConfigADO>();

                PrintConfigADO printConfigADOForGiayRaVien = new PrintConfigADO()
                {
                    ModuleTypePrint = FormTreatmentFinish.ModuleTypePrint.IN_GIAY_RA_VIEN,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_RA_VIEN"),
                    Visible = false
                };

                printConfigADOs.Add(printConfigADOForGiayRaVien);

                PrintConfigADO printConfigADOForGiayHenKhamLai = new PrintConfigADO()
                {
                    ModuleTypePrint = FormTreatmentFinish.ModuleTypePrint.HEN_KHAM_LAI,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_HEN_KHAM_LAI"),
                    Visible = false
                };

                PrintConfigADO printConfigADOForGiayChuyenVien = new PrintConfigADO()
                {
                    ModuleTypePrint = ModuleTypePrint.IN_GIAY_CHUYEN_VIEN,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_CHUYEN_VIEN"),
                    Visible = false
                };

                PrintConfigADO printConfigADOForPhieuBanGiaoNguoiBenhCV = new PrintConfigADO()
                {
                    ModuleTypePrint = ModuleTypePrint.Mps000382,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_PHIEU_BAN_GIAO_NGUOI_BENH_CV"),
                    Visible = false
                };

                PrintConfigADO printConfigADOForGiayBaoTu = new PrintConfigADO()
                {
                    ModuleTypePrint = ModuleTypePrint.GIAY_BAO_TU,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_BAO_TU"),
                    Visible = false
                };

                PrintConfigADO printConfigADOForGiayNghiOm = new PrintConfigADO()
                {
                    ModuleTypePrint = ModuleTypePrint.GIAY_NGHI_OM,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_NGHI_OM"),
                    Visible = false
                };

                printConfigADOs.Add(printConfigADOForGiayHenKhamLai);
                printConfigADOs.Add(printConfigADOForGiayChuyenVien);
                printConfigADOs.Add(printConfigADOForPhieuBanGiaoNguoiBenhCV);
                printConfigADOs.Add(printConfigADOForGiayBaoTu);
                printConfigADOs.Add(printConfigADOForGiayNghiOm);

                PrintConfigADO printConfigADOForBangKeThanhToan = new PrintConfigADO()
                {
                    ModuleTypePrint = ModuleTypePrint.BANG_KE_THANH_TOAN,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_BANG_KE_THANH_TOAN"),
                    Visible = false
                };
                printConfigADOs.Add(printConfigADOForBangKeThanhToan);

                PrintConfigADO printConfigADOForBieuMauKhac = new PrintConfigADO()
                {
                    ModuleTypePrint = ModuleTypePrint.BIEU_MAU_KHAC,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_BIEU_MAU_KHAC"),
                    Visible = false
                };
                printConfigADOs.Add(printConfigADOForBieuMauKhac);

                PrintConfigADO printConfigADOForGiayPTTT = new PrintConfigADO()
                {
                    ModuleTypePrint = ModuleTypePrint._IN_GIAY_CHUNG_NHAN_PTTT,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_CHUNG_NHAN_PTTT"),
                    Visible = false
                };
                printConfigADOs.Add(printConfigADOForGiayPTTT);

                PrintConfigADO printConfigADOForTomTatBenhAn = new PrintConfigADO()
                {
                    ModuleTypePrint = ModuleTypePrint.TOM_TAT_BENH_AN,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_TOM_TAT_BENH_AN"),
                    Visible = false
                };
                printConfigADOs.Add(printConfigADOForTomTatBenhAn);

                PrintConfigADO printConfigADOForXacNhanDieuTriNoiTru = new PrintConfigADO()
                {
                    ModuleTypePrint = ModuleTypePrint.Mps000399,
                    PrintTypeName = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_XAC_NHAN_DIEU_TRI_NOI_TRU"),
                    Visible = false
                };

                printConfigADOs.Add(printConfigADOForXacNhanDieuTriNoiTru);

                if (printConfigADOLocalStores != null && printConfigADOLocalStores.Count > 0)
                {
                    foreach (var item in printConfigADOs)
                    {
                        var printStore = printConfigADOLocalStores.FirstOrDefault(o => o.ModuleTypePrint == item.ModuleTypePrint);
                        if (printStore != null)
                        {
                            item.IsAutoPrint = printStore.IsAutoPrint;
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printConfigADOs), printConfigADOs)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printConfigADOLocalStores), printConfigADOLocalStores));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Sau khi lưu thành công, thì tự động in các mẫu được check chọn nhưng cần đảm bảo:
        ///+ Nếu loại kết thúc không phải là "chuyển viện" (HIS_TREATMENT có TREATMENT_END_TYPE_ID khác IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN), thì không in mẫu "Phiếu chuyển viện" (Mps000011), "Phiếu bàn giao người bệnh chuyển viện" (Mps000382)
        ///+ Nếu loại kết thúc không phải là "tử vong" (HIS_TREATMENT có TREATMENT_END_TYPE_ID khác IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET), thì không in mẫu "Giấy báo tử" (Mps000268)
        ///+ Nếu nếu hồ sơ không có thông tin "ngày hẹn khám" ( HIS_TREATMENT có APPOINTMENT_TIME null) thì không in "Giấy hẹn khám" (Mps000010)
        ///+ Nếu hồ sơ không có thông tin bổ sung là "Nghỉ ốm" (his_treatment có TREATMENT_END_TYPE_EXT_ID khác IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM) thì không in "Giấy nghỉ việc hưởng BHXH" (Mps000269)
        ///+ Nếu loại kết thúc là "trốn viện" (HIS_TREATMENT có TREATMENT_END_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__TRON), thì không in mẫu "Giấy ra viện" (Mps000008)
        /// </summary>
        private void RunAutoPrintByPrintConfig()
        {
            try
            {              
                var printConfigAutoPrints = (printConfigADOs != null && printConfigADOs.Count > 0 && printConfigADOs.Exists(o => o.IsAutoPrint)) ? printConfigADOs.Where(o => o.IsAutoPrint).ToList() : null;               
                if (printConfigAutoPrints != null && printConfigAutoPrints.Count > 0)
                {
                    foreach (var itemPrint in printConfigAutoPrints)
                    {
                        if (hisTreatmentResult.TREATMENT_END_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN
                            && (itemPrint.ModuleTypePrint == FormTreatmentFinish.ModuleTypePrint.IN_GIAY_CHUYEN_VIEN
                            || itemPrint.ModuleTypePrint == FormTreatmentFinish.ModuleTypePrint.Mps000382))
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Nếu loại kết thúc không phải là \"chuyển viện\" (HIS_TREATMENT có TREATMENT_END_TYPE_ID khác IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN), thì không in mẫu \"Phiếu chuyển viện\" (Mps000011)____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentResult), hisTreatmentResult) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemPrint), itemPrint));
                            continue;
                        }
                        else if (hisTreatmentResult.TREATMENT_END_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET
                            && itemPrint.ModuleTypePrint == FormTreatmentFinish.ModuleTypePrint.GIAY_BAO_TU)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Nếu loại kết thúc không phải là \"tử vong\" (HIS_TREATMENT có TREATMENT_END_TYPE_ID khác IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET), thì không in mẫu \"Giấy báo tử\" (Mps000268)____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentResult), hisTreatmentResult) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemPrint), itemPrint));
                            continue;
                        }
                        else if (hisTreatmentResult.APPOINTMENT_TIME == null
                            && itemPrint.ModuleTypePrint == FormTreatmentFinish.ModuleTypePrint.HEN_KHAM_LAI)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Nếu nếu hồ sơ không có thông tin \"ngày hẹn khám\" ( HIS_TREATMENT có APPOINTMENT_TIME null) thì không in \"Giấy hẹn khám\" (Mps000010)____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentResult), hisTreatmentResult) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemPrint), itemPrint));
                            continue;
                        }
                        else if (hisTreatmentResult.TREATMENT_END_TYPE_EXT_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM
                           && itemPrint.ModuleTypePrint == FormTreatmentFinish.ModuleTypePrint.GIAY_NGHI_OM)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Nếu hồ sơ không có thông tin bổ sung là \"Nghỉ ốm\" (his_treatment có TREATMENT_END_TYPE_EXT_ID khác IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM) thì không in \"Giấy nghỉ việc hưởng BHXH\" (Mps000269)____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentResult), hisTreatmentResult) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemPrint), itemPrint));
                            continue;
                        }

                        DevExpress.Utils.Menu.DXMenuItem menuItem = new DevExpress.Utils.Menu.DXMenuItem();
                        menuItem.Tag = itemPrint.ModuleTypePrint;
                        PrintCloseTreatment_Click(menuItem, null);
                    }
                }               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessStorePrintConfigIntoLocal()
        {
            try
            {
                string jsonData = (printConfigADOs != null && printConfigADOs.Count > 0) ? Newtonsoft.Json.JsonConvert.SerializeObject(printConfigADOs) : "";

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.PrintConfigData && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = jsonData;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.PrintConfigData;
                    csAddOrUpdate.VALUE = jsonData;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                bool success = this.controlStateWorker.SetData(this.currentControlStateRDO);
                Inventec.Common.Logging.LogSystem.Debug("ProcessStorePrintConfigIntoLocal____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessShowpopupControlContainerPrintConfig()
        {
            int heightPlus = 0;
            Rectangle bounds = GetClientRectangle(this, ref heightPlus);
            Rectangle bounds1 = GetAllClientRectangle(this, ref heightPlus);
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bounds), bounds) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bounds1), bounds1) + "|bounds1.Height=" + bounds1.Height + "|popupHeight=" + popupHeight);
            if (bounds == null)
            {
                bounds = btnPrintConfig.Bounds;
            }

            //xử lý tính toán lại vị trí hiển thị popup tương đối phụ thuộc theo chiều cao của popup, kích thước màn hình, đối tượng bệnh nhân(bhyt/...)
            if (bounds1.Height <= 768)
            {
                if (popupHeight == 400)
                {
                    heightPlus = bounds.Y >= 650 ? -125 : (bounds.Y > 410 ? (-262) : (bounds.Y < 230 ? (-bounds.Y - 227) : -276));
                }
                else
                    heightPlus = bounds.Y >= 650 ? -60 : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 27 : -78)));
            }
            else
            {
                if (popupHeight == 400)
                {
                    heightPlus = bounds.Y >= 650 ? -327 : (bounds.Y > 410 ? -260 : (bounds.Y < 230 ? (-bounds.Y - 225) : -608));
                }
                else
                    heightPlus = bounds.Y >= 650 ? (-122) : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 25 : -180)));
            }

            Rectangle buttonBounds = new Rectangle(btnPrintConfig.Bounds.X + 10, bounds.Y + heightPlus, bounds.Width, bounds.Height);
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("buttonBounds", buttonBounds)
                + Inventec.Common.Logging.LogUtil.TraceData("heightPlus", heightPlus));
            popupControlContainerPrintConfig.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom));
            gridViewContainerPrintConfig.Focus();
            gridViewContainerPrintConfig.FocusedRowHandle = 0;
        }

        private Rectangle GetClientRectangle(Control control, ref int heightPlus)
        {
            Rectangle bounds = default(Rectangle);
            if (control != null)
            {
                bounds = control.Bounds;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("GetClientRectangle:" + Inventec.Common.Logging.LogUtil.GetMemberName(() => bounds), bounds) + "|control.name=" + control.Name + "|bounds.Y=" + bounds.Y);
                if (control.Parent != null && !(control is UserControl))
                {
                    heightPlus += bounds.Y;
                    return GetClientRectangle(control.Parent, ref  heightPlus);
                }
            }
            return bounds;
        }

        private Rectangle GetAllClientRectangle(Control control, ref int heightPlus)
        {
            Rectangle bounds = default(Rectangle);
            if (control != null)
            {
                bounds = control.Bounds;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("GetAllClientRectangle:" + Inventec.Common.Logging.LogUtil.GetMemberName(() => bounds), bounds) + "|control.name=" + control.Name + "|bounds.Y=" + bounds.Y);
                if (control.Parent != null)
                {
                    heightPlus += bounds.Y;
                    return GetAllClientRectangle(control.Parent, ref  heightPlus);
                }
            }
            return bounds;
        }

        private void InitPopupPrintConfig()
        {
            try
            {
                popupHeight = 300;
                gridViewContainerPrintConfig.BeginUpdate();
                gridViewContainerPrintConfig.Columns.Clear();
                popupControlContainerPrintConfig.Width = 400;
                popupControlContainerPrintConfig.Height = popupHeight;
                int columnIndex = 1;
                AddFieldColumnIntoComboExt(gridViewContainerPrintConfig, "PrintTypeName", "Tên", 350, columnIndex++, true);
                AddFieldColumnIntoComboExt(gridViewContainerPrintConfig, "IsAutoPrint", " ", 30, columnIndex++, true, null, true);

                gridViewContainerPrintConfig.GridControl.DataSource = printConfigADOs;

                gridViewContainerPrintConfig.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewContainerPrintConfig.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddFieldColumnIntoComboExt(Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn gridView, string FieldName, string Caption, int Width, int VisibleIndex, bool FixedWidth, DevExpress.Data.UnboundColumnType? UnboundType = null, bool allowEdit = false)
        {
            DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
            col2.FieldName = FieldName;
            col2.Caption = Caption;
            col2.Width = Width;
            col2.VisibleIndex = VisibleIndex;
            col2.OptionsColumn.FixedWidth = FixedWidth;
            if (UnboundType != null)
                col2.UnboundType = UnboundType.Value;
            col2.OptionsColumn.AllowEdit = allowEdit;
            if (FieldName == "IsChecked")
            {
                col2.ColumnEdit = GenerateRepositoryItemCheckEdit();
                col2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                col2.OptionsFilter.AllowFilter = false;
                col2.OptionsFilter.AllowAutoFilter = false;
                col2.OptionsColumn.AllowEdit = false;
            }

            gridView.Columns.Add(col2);
        }

        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit GenerateRepositoryItemCheckEdit()
        {
            DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            return repositoryItemCheckEdit1;
        }
    }
}
