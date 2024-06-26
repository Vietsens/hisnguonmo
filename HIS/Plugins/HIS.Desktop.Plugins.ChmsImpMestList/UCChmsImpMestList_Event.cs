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
using HIS.Desktop.ADO;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.ChmsImpMestList
{
    public partial class UCChmsImpMestList : UserControl
    {
        private void ButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var ViewImport = (MOS.EFMODEL.DataModels.V_HIS_CHMS_IMP_MEST)gridView.GetFocusedRow();
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
                        HIS.Desktop.ADO.ImpMestViewDetailADO impMestView = new HIS.Desktop.ADO.ImpMestViewDetailADO(ViewImport.IMP_MEST_ID, ViewImport.IMP_MEST_TYPE_ID, ViewImport.IMP_MEST_STT_ID);

                        List<object> listArgs = new List<object>();
                        listArgs.Add(impMestView);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
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
                WaitingManager.Hide();
            }
        }

        private void ButtonEditEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                //CommonParam param = new CommonParam();
                //var expMestData = (MOS.EFMODEL.DataModels.V_HIS_CHMS_IMP_MEST)gridView.GetFocusedRow();
                //if (expMestData.IMP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.IMP_MEST_TYPE_ID__LOST || expMestData.IMP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.IMP_MEST_TYPE_ID__LIQU || expMestData.IMP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.IMP_MEST_TYPE_ID__EXPE)
                //{
                //    //SessionManager.GetFormMain().AllocateExportMedicineForEditClick(expMestData);
                //    WaitingManager.Hide();
                //}
                //else if (expMestData.IMP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.IMP_MEST_TYPE_ID__CHMS || expMestData.IMP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.IMP_MEST_TYPE_ID__SALE || expMestData.IMP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.IMP_MEST_TYPE_ID__DEPA)
                //{
                //    //SessionManager.GetFormMain().ExportMedicineForEditClick(expMestData);
                //    WaitingManager.Hide();
                //}
                //else
                //{
                //    WaitingManager.Hide();
                //    DevExpress.XtraEditors.XtraMessageBox.Show(
                //        "",
                //        Resources.ResourceMessage.ThongBao,
                //        MessageBoxButtons.OK);
                //}
                MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex); 
                WaitingManager.Hide();
            }
        }

        private void ButtonDiscardEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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
                    var row = (MOS.EFMODEL.DataModels.V_HIS_CHMS_IMP_MEST)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                        data.ID = row.IMP_MEST_ID;
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                        if (apiresul)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonApprovalEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_CHMS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_CHMS_IMP_MEST)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(EVImportMest, VImportMest);

                EVImportMest.IMP_MEST_STT_ID = Base.HisImpMestSttCFG.IMP_MEST_STT_ID__APPROVED;
                EVImportMest.ID = VImportMest.IMP_MEST_ID;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_UPDATE_STATUS, ApiConsumers.MosConsumer, EVImportMest, param);
                if (apiresul != null && apiresul.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__APPROVED)
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

        private void ButtonDisApprovalEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_CHMS_IMP_MEST)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                data.ID = row.IMP_MEST_ID;
                data.IMP_MEST_STT_ID = Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                if (apiresul != null && apiresul.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED)
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
                WaitingManager.Hide();
            }
        }

        private void ButtonActualExportEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_CHMS_IMP_MEST)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                data.ID = row.IMP_MEST_ID;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_IMPORT, ApiConsumer.ApiConsumers.MosConsumer, data, param);
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
                WaitingManager.Hide();
            }
        }

        private void ButtonChmsImpEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();

                var row = (MOS.EFMODEL.DataModels.V_HIS_CHMS_IMP_MEST)gridView.GetFocusedRow();
                if (row != null)
                {
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                    data.ID = row.IMP_MEST_ID;
                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.SDO.HisChmsImpMestResultSDO>(ApiConsumer.HisRequestUriStore.HIS_CHMS_IMP_MEST_CREATE, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (result != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
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
                WaitingManager.Hide();
            }
        }

        private void ButtonRequest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_CHMS_IMP_MEST)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);
                data.ID = row.IMP_MEST_ID;
                data.IMP_MEST_STT_ID = Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REQUEST;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
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

        private void FillDataApterSave(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
