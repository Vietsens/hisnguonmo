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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.HisManuImportMestMedicine
{
    public partial class UCHisManuImportMestMedicine : UserControl
    {
        private void txtImpCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtImpCode.Text)) FillDataToGrid();
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                if (e.KeyCode == Keys.Tab)
                {
                    dtCreateTimeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeFrom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtCreateTimeFrom.EditValue != null)
                    {
                        dtCreateTimeTo.Focus();
                        dtCreateTimeTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDateFrom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtDocumentDateFrom.EditValue != null)
                    {
                        dtDocumentDateTo.Focus();
                        dtDocumentDateTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDateTo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEditHasDocumentNumber_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEditHasDocumentNumber.Checked) checkEditNoDocumentNumber.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEditNoDocumentNumber_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEditNoDocumentNumber.Checked) checkEditHasDocumentNumber.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditViewDetail_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var ViewImport = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU)gridView.GetFocusedRow();
                //hien thi popup chi tiet
                if (ViewImport != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ImpMestViewDetail").FirstOrDefault();
                    if (moduleData == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ImpMestViewDetail");
                        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        HIS.Desktop.ADO.ImpMestViewDetailADO impMestView = new HIS.Desktop.ADO.ImpMestViewDetailADO(ViewImport.ID, ViewImport.IMP_MEST_TYPE_ID, ViewImport.IMP_MEST_STT_ID);

                        List<object> listArgs = new List<object>();
                        listArgs.Add(impMestView);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditEditEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var ViewImport = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU)gridView.GetFocusedRow();
                //hien thi popup chi tiet
                if (ViewImport != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ManuImpMestEdit").FirstOrDefault();
                    if (moduleData == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ManuImpMestEdit");
                        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ViewImport.ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                        FillDataApterClose(ViewImport);
                    }
                    else
                    {
                        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditDiscardEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                        data.ID = row.ID;
                        var valid = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            (HisRequestUriStore.HIS_IMP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                        if (valid)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEditApprovalEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(EVImportMest, VImportMest);

                EVImportMest.IMP_MEST_STT_ID = Base.HisImpMestSttCFG.IMP_MEST_STT_ID__APPROVED;
                EVImportMest.ID = VImportMest.ID;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_UPDATE_STATUS, ApiConsumers.MosConsumer, EVImportMest, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataToGrid();
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEditDisApprovalEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(EVImportMest, VImportMest);
                EVImportMest.IMP_MEST_STT_ID = Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED;
                EVImportMest.ID = VImportMest.ID;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_UPDATE_STATUS, ApiConsumers.MosConsumer, EVImportMest, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataToGrid();
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEditActualImportEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonThucNhapDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                        data.IMP_MEST_STT_ID = Base.HisImpMestSttCFG.IMP_MEST_STT_ID__IMPORTED;
                        data.ID = row.ID;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                            (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_IMPORT, ApiConsumers.MosConsumer, data, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEditExpSupplierEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                //mở module Tạo yêu cầu xuất trả nhà cung cấp
                var ExpMestData = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU)gridView.GetFocusedRow();

                if (ExpMestData != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ManuExpMestCreate").FirstOrDefault();
                    if (moduleData == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ManuExpMestCreate");
                        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ExpMestData);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                        FillDataApterClose(ExpMestData);
                    }
                    else
                    {
                        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterClose(MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU ExpMestData)
        {
            try
            {
                MOS.Filter.HisManuImpMestViewFilter filter = new MOS.Filter.HisManuImpMestViewFilter();
                filter.ID = ExpMestData.ID;
                var listTreat = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU>>(HisRequestUriStore.HIS_MANU_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, filter, null);
                if (listTreat != null && listTreat.Count == 1)
                {
                    dataGrid[dataGrid.IndexOf(ExpMestData)] = listTreat.First();
                    gridControl.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditReApproval_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(EVImportMest, VImportMest);

                EVImportMest.IMP_MEST_STT_ID = Base.HisImpMestSttCFG.IMP_MEST_STT_ID__UNAPPROVED;
                EVImportMest.ID = VImportMest.ID;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_UPDATE_STATUS, ApiConsumers.MosConsumer, EVImportMest, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataToGrid();
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
