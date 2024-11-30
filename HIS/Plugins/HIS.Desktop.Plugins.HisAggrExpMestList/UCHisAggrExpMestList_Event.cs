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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Common;
using MOS.SDO;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisAggrExpMestList
{
    public partial class UCHisAggrExpMestList : HIS.Desktop.Utility.UserControlBase
    {
        //DelegateSelectData delegateSelectData = null;
        private void ButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST ExpMestData = new MOS.EFMODEL.DataModels.V_HIS_EXP_MEST();
                var ExpMestData3 = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3)gridView.GetFocusedRow();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>(ExpMestData, ExpMestData3);
                if (ExpMestData != null)
                {
                    WaitingManager.Show();

                    HIS.Desktop.ADO.ApproveAggrExpMestSDO exeMestView = new HIS.Desktop.ADO.ApproveAggrExpMestSDO(ExpMestData.ID, ExpMestData.EXP_MEST_STT_ID);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((DelegateSelectData)delegateSelectData);
                    CallModule callModule = new CallModule(CallModule.AggrExpMestDetail, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void delegateSelectData(object data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                //CommonParam param = new CommonParam();
                //var expMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                //if (expMestData.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__LOST || expMestData.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__LIQU || expMestData.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__EXPE)
                //{
                //    //SessionManager.GetFormMain().AllocateExportMedicineForEditClick(expMestData);
                //    WaitingManager.Hide();
                //}
                //else if (expMestData.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__CHMS || expMestData.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__SALE || expMestData.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__DEPA)
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
                    var row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            ("api/HisExpMest/AggrDelete", ApiConsumer.ApiConsumers.MosConsumer, row.ID, param);
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
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3 row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3)gridView.GetFocusedRow();
                if (row != null)
                {
                    //WaitingManager.Show();

                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrApprove").FirstOrDefault();
                    //if (moduleData == null)
                    //{
                    //    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrApprove");
                    //    MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    //}
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{

                    //    List<object> listArgs = new List<object>();
                    //    listArgs.Add(row.ID);
                    //    listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId));
                    //    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    //    WaitingManager.Hide();
                    //    ((Form)extenceInstance).ShowDialog();
                    //    FillDataToGrid();
                    //}
                    //else
                    //{
                    //    MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    //}
                    WaitingManager.Show();
                    MOS.Filter.HisExpMestMedicineFilter filter = new MOS.Filter.HisExpMestMedicineFilter();
                    filter.EXP_MEST_ID = row.ID;

                    HisExpMestSDO sdo = new HisExpMestSDO();
                    sdo.ExpMestId = row.ID;
                    sdo.ReqRoomId = this.roomId;
                    //sdo.IsFinish = true;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>
                        ("api/HisExpMest/AggrApprove", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                    if (apiresult != null && apiresult.Count > 0)
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3 row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3)gridView.GetFocusedRow();
                HisExpMestSDO ado = new HisExpMestSDO();
                ado.ExpMestId = row.ID;
                ado.ReqRoomId = this.roomId;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Decline", ApiConsumer.ApiConsumers.MosConsumer, ado, param);
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

        private void ButtonExportEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3 row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3)gridView.GetFocusedRow();
                if (row != null)
                {
                    //if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)//Review //row.BLOOD_TYPE_COUNT > 0 && row.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__PRES)
                    //{
                    //    WaitingManager.Show();

                    //    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExportBlood").FirstOrDefault();
                    //    if (moduleData == null)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ExportBlood");
                    //        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    //    }
                    //    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //    {

                    //        List<object> listArgs = new List<object>();
                    //        listArgs.Add(row.ID);
                    //        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    //        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    //        WaitingManager.Hide();
                    //        ((Form)extenceInstance).ShowDialog();
                    //        FillDataApterClose(row);
                    //    }
                    //    else
                    //    {
                    //        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    //    }
                    //}
                    //else
                    //{
                    WaitingManager.Show();
                    HisExpMestSDO sdo = new HisExpMestSDO();
                    sdo.ExpMestId = row.ID;
                    sdo.ReqRoomId = this.roomId;
                    //sdo.IsFinish = true;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                        (RequestUriStore.HIS_EXP_MEST_AGGREXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
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

                    //}
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonCopyExport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var expMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_1)gridView.GetFocusedRow();
                //SessionManager.GetFormMain().ExportMedicineForCopyExpMestClick(expMestData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonReApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3 row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3)gridView.GetFocusedRow();
                HisExpMestSDO data = new HisExpMestSDO();
                data.ExpMestId = row.ID;
                data.ReqRoomId = this.roomId;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/AggrUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
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

        private void Btn_HuyThucXuat_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3 row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3)gridView.GetFocusedRow();
                if (row != null)
                {

                    WaitingManager.Show();
                    HisExpMestSDO sdo = new HisExpMestSDO();
                    sdo.ExpMestId = row.ID;
                    sdo.ReqRoomId = this.roomId;
                    //sdo.IsFinish = true;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                        ("api/HisExpMest/AggrUnexport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
    }
}
