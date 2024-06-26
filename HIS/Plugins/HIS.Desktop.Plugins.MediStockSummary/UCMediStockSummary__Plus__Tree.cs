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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MediStockSummary.CreateReport;
using HIS.UC.HisMaterialInStock.ADO;
using HIS.UC.HisMedicineInStock.ADO;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.SDO;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using MOS.Filter;

namespace HIS.Desktop.Plugins.MediStockSummary
{
    public partial class UCMediStockSummary : HIS.Desktop.Utility.UserControlBase
    {
        Dictionary<long, HisMedicineInStockSDO> dicMedicines = new Dictionary<long, HisMedicineInStockSDO>();
        Dictionary<long, HisMaterialInStockSDO> dicMaterials = new Dictionary<long, HisMaterialInStockSDO>();
        //Thuốc
        private void medicineType_GetSelectImage(HisMedicineInStockSDO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                if (this.mediStockIds.Count == 1)
                {
                    if (data != null)
                    {
                        if (data.IS_LEAF == 1)//&& data.isTypeNode)
                        {
                            e.NodeImageIndex = 1;
                        }
                        else
                        {
                            e.NodeImageIndex = -1;
                        }
                    }
                    else
                        e.NodeImageIndex = -1;
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineType_SelectImageClick(HisMedicineInStockSDO noteData)
        {
            try
            {
                if (chkMedicine.Checked)
                {
                    if (noteData != null && noteData.IS_LEAF == 1 && this.mediStockIds.Count == 1)
                    {
                        //chọn loại thì ID = TYPE_ID
                        frmMediCardByDateReport frm = new frmMediCardByDateReport(this.RoomId, KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_THUOC_THEO_NGAY, noteData, BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.mediStockIds.FirstOrDefault()), noteData.ID != noteData.MEDICINE_TYPE_ID);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineType_GetStateImage(HisMedicineInStockSDO data, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                if (this.mediStockIds.Count == 1)
                {
                    if (data != null)
                    {
                        if (data.IS_LEAF == 1)// && data.isTypeNode)
                        {
                            e.NodeImageIndex = 0;
                        }
                        else
                        {
                            e.NodeImageIndex = -1;
                        }
                    }
                    else
                        e.NodeImageIndex = -1;
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineType_StateImageClick(HisMedicineInStockSDO data)
        {
            try
            {
                if (chkMedicine.Checked)
                {
                    if (data != null && data.IS_LEAF == 1 && this.mediStockIds.Count == 1)
                    {
                        //chọn loại thì ID = TYPE_ID
                        frmMediCardByDateReport frm = new frmMediCardByDateReport(this.RoomId, KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_THUOC_THEO_NGAY, data, BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.mediStockIds.FirstOrDefault()), data.ID != data.MEDICINE_TYPE_ID);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    HisMedicineInStockADO data = e.Row as HisMedicineInStockADO;
                    if (data != null)
                    {
                        if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY" && !e.Node.HasChildren)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(data.EXPIRED_DATE ?? 0));
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY" && !e.Node.HasChildren)
                        {
                            e.Value = (data.IMP_VAT_RATIO * 100);
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE" && !e.Node.HasChildren)
                        {
                            decimal SUM_PRICE = data.IMP_PRICE * data.TotalAmount * (1 + data.IMP_VAT_RATIO) ?? 0;
                            e.Value = SUM_PRICE;
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY" && !e.Node.HasChildren)
                        {
                            e.Value = data.IMP_PRICE;
                        }
                        else if (e.Column.FieldName == "PRICE_AFTER_VAT" && !e.Node.HasChildren)
                        {
                            e.Value = data.IMP_PRICE * (data.IMP_VAT_RATIO + 1);
                        }
                        else if (e.Column.FieldName == "TotalAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                        {
                            e.Value = data.TotalAmount;
                        }
                        else if (e.Column.FieldName == "AvailableAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                        {
                            e.Value = data.AvailableAmount;
                        }
                        else if (e.Column.FieldName == "TotalAmountConvert_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                        {
                            ///e.Value = (data.TotalAmount * data.CONVERT_RATIO);//TODO
                        }
                        else if (e.Column.FieldName == "AvailableAmountConvert_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                        {
                            //e.Value = (data.AvailableAmount * data.CONVERT_RATIO);//TODO
                        }
                        else if (e.Column.FieldName == "CompensationBaseAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                        {
                            if (data.RealBaseAmount.HasValue)
                            {
                                e.Value = data.RealBaseAmount.Value - (data.TotalAmount ?? 0);
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        else if (e.Column.FieldName == "TOTAL_EXP_PRICE" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                        {
                            if (data.ExpPrice.HasValue)
                            {
                                e.Value = data.ExpPrice.Value * ((data.ExpVatRatio ?? 0) + 1);
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        else if (e.Column.FieldName == "BaseAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                        {
                            e.Value = data.BaseAmount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void medicineType_NodeCellStyle(HisMedicineInStockADO data, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (data.ALERT_MIN_IN_STOCK.HasValue && e.Node.HasChildren)
                    {
                        if (data.TotalAmount < data.ALERT_MIN_IN_STOCK)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineType_CustomDrawNodeCell(HisMedicineInStockSDO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (this.HisMediStockMetyByStocks == null && this.HisMediStockMetyByStocks.Count == 0)
                        return;
                    foreach (var item in this.HisMediStockMetyByStocks)
                    {
                        if (data.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID)
                        {
                            if (item.ALERT_MIN_IN_STOCK != null)
                            {
                                if (data.TotalAmount < item.ALERT_MIN_IN_STOCK)
                                {
                                    e.Appearance.ForeColor = Color.Red;
                                }
                            }
                            if (item.ALERT_MAX_IN_STOCK != null)
                            {
                                if (data.TotalAmount > item.ALERT_MAX_IN_STOCK)
                                {
                                    e.Appearance.ForeColor = Color.Blue;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //Máu
        private void bloodType_GetSelectImage(HisBloodInStockSDO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                if (this.mediStockIds.Count == 1)
                {
                    if (data != null)
                    {
                        if (string.IsNullOrEmpty(data.ParentNodeId))// && data.isTypeNode)
                        {
                            e.NodeImageIndex = 1;
                        }
                        else
                        {
                            e.NodeImageIndex = -1;
                        }
                    }
                    else
                        e.NodeImageIndex = -1;
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bloodType_GetStateImage(HisBloodInStockSDO data, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                if (this.mediStockIds.Count == 1)
                {
                    if (data != null)
                    {
                        if (string.IsNullOrEmpty(data.ParentNodeId))// && data.isTypeNode)
                        {
                            e.NodeImageIndex = 0;
                        }
                        else
                        {
                            e.NodeImageIndex = -1;
                        }
                    }
                    else
                        e.NodeImageIndex = -1;
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bloodType_SelectImageClick(HisBloodInStockSDO noteData)
        {
            try
            {
                if (chkBlood.Checked)
                {
                    if (noteData != null && string.IsNullOrEmpty(noteData.ParentNodeId) && this.mediStockIds.Count == 1)
                    {
                        frmMediCardByDateReport frm = new frmMediCardByDateReport(this.RoomId, KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_MAU_THEO_NGAY, noteData, BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.mediStockIds.FirstOrDefault()), true);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bloodType_StateImageClick(HisBloodInStockSDO data)
        {
            try
            {
                if (chkBlood.Checked)
                {
                    if (data != null && string.IsNullOrEmpty(data.ParentNodeId) && this.mediStockIds.Count == 1)
                    {
                        //chọn loại thì ID = TYPE_ID
                        frmMediCardByDateReport frm = new frmMediCardByDateReport(this.RoomId, KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_MAU_THEO_NGAY, data, BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.mediStockIds.FirstOrDefault()), true);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //Vật tư
        private void materialType_GetSelectImage(HisMaterialInStockSDO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                if (this.mediStockIds.Count == 1)
                {
                    if (data != null)
                    {
                        if (data.IS_LEAF == 1)// && data.isTypeNode)
                        {
                            e.NodeImageIndex = 1;
                        }
                        else
                        {
                            e.NodeImageIndex = -1;
                        }
                    }
                    else
                        e.NodeImageIndex = -1;
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialType_SelectImageClick(HisMaterialInStockADO noteData)
        {
            try
            {
                if (chkMaterial.Checked)
                {
                    if (noteData != null && noteData.IS_LEAF == 1 && this.mediStockIds.Count == 1)
                    {
                        frmMediCardByDateReport frm = new frmMediCardByDateReport(this.RoomId, KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_VAT_TU_THEO_NGAY, noteData, BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.mediStockIds.FirstOrDefault()), noteData.ID != noteData.MATERIAL_TYPE_ID);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialType_GetStateImage(HisMaterialInStockSDO data, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                if (this.mediStockIds.Count == 1)
                {
                    if (data != null)
                    {
                        if (data.IS_LEAF == 1)// && data.isTypeNode)
                        {
                            e.NodeImageIndex = 0;
                        }
                        else
                        {
                            e.NodeImageIndex = -1;
                        }
                    }
                    else
                        e.NodeImageIndex = -1;
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialType_StateImageClick(HisMaterialInStockADO noteData)
        {
            try
            {
                if (chkMaterial.Checked)
                {
                    if (noteData != null && noteData.IS_LEAF == 1 && this.mediStockIds.Count == 1)
                    {
                        frmMediCardByDateReport frm = new frmMediCardByDateReport(this.RoomId, KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_VAT_TU_THEO_NGAY, noteData, BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.mediStockIds.FirstOrDefault()), noteData.ID != noteData.MATERIAL_TYPE_ID);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    HisMaterialInStockADO data = e.Row as HisMaterialInStockADO;
                    //if (data.ParentNodeId != null)
                    //{
                    if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY" && !e.Node.HasChildren)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(data.EXPIRED_DATE ?? 0));
                    }
                    else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY" && !e.Node.HasChildren)
                    {
                        e.Value = (data.IMP_VAT_RATIO * 100);
                    }
                    else if (e.Column.FieldName == "TOTAL_PRICE" && !e.Node.HasChildren)
                    {
                        decimal SUM_PRICE = data.IMP_PRICE * data.TotalAmount * (1 + data.IMP_VAT_RATIO) ?? 0;
                        e.Value = SUM_PRICE;
                    }
                    else if (e.Column.FieldName == "PRICE_AFTER_VAT" && !e.Node.HasChildren)
                    {
                        e.Value = data.IMP_PRICE * (data.IMP_VAT_RATIO + 1);
                    }
                    else if (e.Column.FieldName == "IMP_PRICE_DISPLAY" && !e.Node.HasChildren)
                    {
                        e.Value = data.IMP_PRICE;
                    }
                    else if (e.Column.FieldName == "TotalAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        e.Value = data.TotalAmount;
                    }
                    else if (e.Column.FieldName == "AvailableAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        e.Value = data.AvailableAmount;
                    }
                    else if (e.Column.FieldName == "CompensationBaseAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        if (data.RealBaseAmount.HasValue)
                        {
                            e.Value = data.RealBaseAmount.Value - (data.TotalAmount ?? 0);
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "BaseAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        e.Value = data.BaseAmount;
                    }
                    else if (e.Column.FieldName == "TOTAL_EXP_PRICE" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        if (data.ExpPrice.HasValue)
                        {
                            e.Value = data.ExpPrice.Value * ((data.ExpVatRatio ?? 0) + 1);
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void materialType_NodeCellStyle(HisMaterialInStockADO data, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (data.ALERT_MIN_IN_STOCK.HasValue && e.Node.HasChildren)
                    {
                        if (data.TotalAmount < data.ALERT_MIN_IN_STOCK)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialType_CustomDrawNodeCell(HisMaterialInStockADO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (this.HisMediStockMatyByStocks == null && this.HisMediStockMatyByStocks.Count == 0)
                        return;
                    foreach (var item in this.HisMediStockMatyByStocks)
                    {
                        if (data.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID)
                        {
                            if (item.ALERT_MIN_IN_STOCK != null)
                            {
                                if (data.TotalAmount < item.ALERT_MIN_IN_STOCK)
                                {
                                    e.Appearance.ForeColor = Color.Red;
                                }
                            }
                            if (item.ALERT_MAX_IN_STOCK != null)
                            {
                                if (data.TotalAmount > item.ALERT_MAX_IN_STOCK)
                                {
                                    e.Appearance.ForeColor = Color.Blue;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineType_DoubleClick(HisMedicineInStockADO data)
        {
            try
            {
                if (data != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(data.MEDICINE_TYPE_ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HistoryMedicine", this.RoomId, this.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void materialType_DoubleClick(HisMaterialInStockADO data)
        {
            try
            {
                if (data != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(data.MATERIAL_TYPE_ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HistoryMaterial", this.RoomId, this.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<DevExpress.Utils.Menu.DXMenuItem> menuItemMedicine(HisMedicineInStockADO data, TreeListNode node)
        {
            try
            {
                mediStockAdo = new HisMedicineInStockADO();
                dXmenuItem = new List<DevExpress.Utils.Menu.DXMenuItem>();
                var dXmenu = new DevExpress.Utils.Menu.DXMenuItem();
                if (data != null)
                {
                    mediStockAdo = data;
                    if (mediStockAdo.IS_LEAF == 1 && node != null && !node.HasChildren) mediStockAdo.NotHasChildren = true;
                    dXmenu.Click += Medicine_RightMouseClick;
                    dXmenu.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY_XEM_LICH_SU_XUAT_NHAP_THUOC", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    dXmenuItem.Add(dXmenu);
                    if (this.mediStockIds != null && mediStockIds.Count == 1)
                    {
                        var dxmenu1 = new DevExpress.Utils.Menu.DXMenuItem();
                        dxmenu1.Click += Medicine_Pay_Available_RightMouseClick;
                        dxmenu1.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY_TRA_KHA_DUNG", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        dXmenuItem.Add(dxmenu1);
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dXmenuItem;

        }
        private void Medicine_RightMouseClick(object sender, EventArgs e)
        {
            try
            {
                if (mediStockAdo != null)
                {
                    List<object> listArgs = new List<object>();

                    if (mediStockAdo.NotHasChildren == true)
                    {
                        if (XtraMessageBox.Show("Bạn có muốn hiển thị lịch sử nhập thuốc/vt theo lô không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            listArgs.Add(mediStockAdo.PACKAGE_NUMBER);
                        }
                    }
                    listArgs.Add(mediStockAdo.MEDICINE_TYPE_ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HistoryMedicine", this.RoomId, this.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                // WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Medicine_Pay_Available_RightMouseClick(object sender, EventArgs e)
        {
            try
            {
                if (mediStockAdo != null)
                {
                    //So luong kha dung --AvailableAmount
                    //So luong ton ---TotalAmount
                    if ((mediStockAdo.AvailableAmount ?? 0) < (mediStockAdo.TotalAmount ?? 0))
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        MOS.SDO.HisMedicineReturnAvailableSDO updateSDO = new MOS.SDO.HisMedicineReturnAvailableSDO();
                        if (mediStockAdo.isTypeNode == false)
                            updateSDO.MedicineId = mediStockAdo.ID;
                        else
                            updateSDO.MedicineTypeId = mediStockAdo.MEDICINE_TYPE_ID;
                        updateSDO.MediStockId = (this.mediStockIds != null && this.mediStockIds.Count > 0 ? this.mediStockIds.FirstOrDefault() : 0);
                        success = new BackendAdapter(param).Post<bool>(RequestUriStore.RETURN_AVAILABLE_MEDICINE, ApiConsumer.ApiConsumers.MosConsumer, updateSDO, param);
                        if (success)
                        {
                            ShowUCControl();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                        MessageBox.Show(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY_KHA_DUNG_BANG_TON_KHO", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private List<DevExpress.Utils.Menu.DXMenuItem> menuItemMaterial(HisMaterialInStockADO data, TreeListNode node)
        {
            try
            {
                mateStockAdo = new HisMaterialInStockADO();
                dXmenuItem = new List<DevExpress.Utils.Menu.DXMenuItem>();
                var dXmenu = new DevExpress.Utils.Menu.DXMenuItem();
                var dxMenu1 = new DevExpress.Utils.Menu.DXMenuItem();
                if (data != null)
                {
                    mateStockAdo = data;
                    if (mateStockAdo.IS_LEAF == 1 && node != null && !node.HasChildren) mateStockAdo.NotHasChildren = true;
                    dXmenu.Click += Material_RightMouseClick;
                    dXmenu.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY_XEM_LICH_SU_XUAT_NHAP_VAT_TU", HIS.Desktop.Plugins.MediStockSummary.Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    dXmenuItem.Add(dXmenu);
                    if (this.mediStockIds != null && mediStockIds.Count == 1)
                    {
                        dxMenu1.Click += Material_RightMouseClick_TraKD;
                        dxMenu1.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY_TRA_KHA_DUNG_VT", HIS.Desktop.Plugins.MediStockSummary.Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        dXmenuItem.Add(dxMenu1);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dXmenuItem;

        }

        private void Material_RightMouseClick_TraKD(object sender, EventArgs e)
        {
            try
            {
                if (mateStockAdo != null)
                {
                    //So luong kha dung --AvailableAmount
                    //So luong ton ---TotalAmount
                    if ((mateStockAdo.AvailableAmount ?? 0) < (mateStockAdo.TotalAmount ?? 0))
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        MOS.SDO.HisMaterialReturnAvailableSDO updateSDO = new MOS.SDO.HisMaterialReturnAvailableSDO();
                        if (mateStockAdo.isTypeNode == false)
                            updateSDO.MaterialId = mateStockAdo.ID;
                        else
                            updateSDO.MaterialTypeId = mateStockAdo.MATERIAL_TYPE_ID;
                        updateSDO.MediStockId = (this.mediStockIds != null && this.mediStockIds.Count > 0 ? this.mediStockIds.FirstOrDefault() : 0);
                        success = new BackendAdapter(param).Post<bool>(RequestUriStore.RETURN_AVAILABLE_MATERIAL, ApiConsumer.ApiConsumers.MosConsumer, updateSDO, param);
                        if (success)
                        {
                            ShowUCControl();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                        MessageBox.Show(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY_KHA_DUNG_BANG_TON_KHO", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Material_RightMouseClick(object sender, EventArgs e)
        {
            try
            {
                if (mateStockAdo != null)
                {
                    List<object> listArgs = new List<object>();
                    if (mateStockAdo.NotHasChildren == true)
                    {
                        if (XtraMessageBox.Show("Bạn có muốn hiển thị lịch sử nhập thuốc/vt theo lô không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            listArgs.Add(mateStockAdo.PACKAGE_NUMBER);
                        }
                    }
                    listArgs.Add(mateStockAdo.MATERIAL_TYPE_ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HistoryMaterial", this.RoomId, this.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                // WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnLock_buttonClick(HisMedicineInStockADO data)
        {
            try
            {
                if (data != null)
                {
                    currentRowMedicine = data;
                    frmReasonLock frmLock = new frmReasonLock(data, ActionSuccessMedi, this.mediStockIds[0], RoomId);
                    frmLock.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ActionSuccessMedi(bool success)
        {
            try
            {
                if (!success)
                    return;
                var lstData = hisMediInStockProcessor.GetListAll(ucMedicineInfo);
                parentAvailableAmount = 0;
                //foreach (var item in lstData)
                //{
                //if(item.ID == currentRowMedicine.ID)
                {
                        CommonParam param = new CommonParam();
                        HisMedicineStockViewFilter mediFilter = new HisMedicineStockViewFilter();
                        mediFilter.MEDI_STOCK_IDs = this.mediStockIds;
                        mediFilter.INCLUDE_EMPTY = false;
                        mediFilter.GROUP_BY_MEDI_STOCK = false;
                        mediFilter.INCLUDE_BASE_AMOUNT = false;
                        mediFilter.INCLUDE_EXP_PRICE = false;
                        mediFilter.ID = currentRowMedicine.ID;
                        var ListMedicineInStockSDO = new BackendAdapter(param).Get<List<HisMedicineInStockSDO>>("/api/HisMedicine/GetInStockMedicineWithTypeTree", ApiConsumer.ApiConsumers.MosConsumer, mediFilter, param);
                        if(ListMedicineInStockSDO != null && ListMedicineInStockSDO.Count > 0)
                        {
                            var medi = ListMedicineInStockSDO.FirstOrDefault(o => o.ID == currentRowMedicine.ID);
                            if (medi != null)
                            {
                                var oldAmount = currentRowMedicine.AvailableAmount;
                                currentRowMedicine.AvailableAmount = medi.AvailableAmount;
                                var parent = lstData.FirstOrDefault(o => o.NodeId == currentRowMedicine.ParentNodeId);
                                if (parent != null)
                                    parentAvailableAmount = parent.AvailableAmount = currentRowMedicine.AvailableAmount != 0 ? parent.AvailableAmount + currentRowMedicine.AvailableAmount : parent.AvailableAmount - oldAmount;
                            }
                        //    break;
                        }    
                    }    
                //}
                hisMediInStockProcessor.RefreshData(ucMedicineInfo, currentRowMedicine, parentAvailableAmount);
                //hisMediInStockProcessor.FocusRowTree(ucMedicineInfo);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnLock_buttonClick(HisMedicineInStockADO data)
        {
            try
            {
                if (data != null)
                {
                    HisMedicineChangeLockSDO dataChange = new HisMedicineChangeLockSDO();
                    dataChange.MedicineId = data.ID;
                    dataChange.MediStockId = this.mediStockIds[0];
                    dataChange.WorkingRoomId = RoomId;
                    bool succes = false;
                    CommonParam param = new CommonParam();
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        succes = new BackendAdapter(param).Post<bool>("/api/HisMedicine/Unlock", ApiConsumer.ApiConsumers.MosConsumer, dataChange, param);
                        if (succes)
                        {
                            currentRowMedicine = data;
                            ActionSuccessMedi(succes);
                            hisMediInStockProcessor.FocusRowTree(ucMedicineInfo);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, succes);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_buttonClickMaterial(HisMaterialInStockADO data)
        {
            try
            {
                if (data != null)
                {
                    currentRowMaterial = data;
                    frmReasonLock frmLock = new frmReasonLock(data,ActionSuccessMate, this.mediStockIds[0],RoomId);
                    frmLock.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ActionSuccessMate(bool success)
        {
            try
            {
                if (!success)
                    return;
                parentAvailableAmount = 0;
                var lstData = hisMateInStockProcessor.GetListAll(ucMaterialInfo);
                {
                    CommonParam param = new CommonParam();
                    HisMaterialStockViewFilter mateFilter = new HisMaterialStockViewFilter();
                    mateFilter.MEDI_STOCK_IDs = this.mediStockIds;
                    mateFilter.INCLUDE_EMPTY = false;
                    mateFilter.GROUP_BY_MEDI_STOCK = false;
                    mateFilter.INCLUDE_BASE_AMOUNT = false;
                    mateFilter.INCLUDE_EXP_PRICE = false;
                    mateFilter.ID = currentRowMaterial.ID;
                    var ListMaterialInStockSDO = new BackendAdapter(param).Get<List<HisMaterialInStockSDO>>("/api/HisMaterial/GetInStockMaterialWithTypeTree", ApiConsumer.ApiConsumers.MosConsumer, mateFilter, param);
                    if (ListMaterialInStockSDO != null && ListMaterialInStockSDO.Count > 0)
                    {
                        var medi = ListMaterialInStockSDO.FirstOrDefault(o => o.ID == currentRowMaterial.ID);
                        if (medi != null)
                        {
                            var oldAmount = currentRowMaterial.AvailableAmount;
                            currentRowMaterial.AvailableAmount = medi.AvailableAmount;
                            var parent = lstData.FirstOrDefault(o => o.NodeId == currentRowMaterial.ParentNodeId);
                            if (parent != null)
                                parentAvailableAmount = parent.AvailableAmount = currentRowMaterial.AvailableAmount != 0 ? parent.AvailableAmount + currentRowMaterial.AvailableAmount : parent.AvailableAmount - oldAmount;
                        }
                    }
                }
                hisMateInStockProcessor.RefreshData(ucMaterialInfo, currentRowMaterial, parentAvailableAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnLock_buttonClickMaterial(HisMaterialInStockADO data)
        {
            try
            {
                if (data != null)
                {
                    bool succes = false;
                    HisMaterialChangeLockSDO dataChange = new HisMaterialChangeLockSDO();
                    dataChange.MaterialId = data.ID;
                    dataChange.MediStockId = this.mediStockIds[0];
                    dataChange.WorkingRoomId = RoomId;
                    CommonParam param = new CommonParam();
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        succes = new BackendAdapter(param).Post<bool>("/api/HisMaterial/Unlock", ApiConsumer.ApiConsumers.MosConsumer, dataChange, param);
                        if (succes)
                        {
                            currentRowMaterial = data;
                            ActionSuccessMate(succes);
                            hisMateInStockProcessor.FocusRowTree(ucMaterialInfo);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, succes);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
