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
using Inventec.Common.Adapter;
using MOS.Filter;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.DepaExpMestList
{
    public partial class UCDepaExpMestList : UserControl
    {
        private void ButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show(); ;
                var ExpMestData = (MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST)gridView.GetFocusedRow();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpMestViewDetail").FirstOrDefault();
                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ExpMestViewDetail");
                    MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    ExpMestViewDetailADO exeMestView = new ExpMestViewDetailADO(ExpMestData.EXP_MEST_ID, ExpMestData.EXP_MEST_TYPE_ID, ExpMestData.EXP_MEST_STT_ID);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(exeMestView);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
                else
                {
                    MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                }
                WaitingManager.Hide();
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
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var data = (MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST)gridView.GetFocusedRow();

                if (data != null)
                {
                    HisExpMestView1Filter filter = new HisExpMestView1Filter();
                    filter.ID = data.EXP_MEST_ID;
                    var listData = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_1>>("api/HisExpMest/GetView1", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (listData != null && listData.Count > 0)
                    {
                        var expMestData = listData.FirstOrDefault();
                        if (expMestData.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__CHMS)
                        {
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpMestDepaUpdate").FirstOrDefault();
                            if (moduleData == null)
                            {
                                Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ExpMestDepaUpdate");
                                MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                            }
                            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(expMestData);
                                listArgs.Add(moduleData);
                                //listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                                var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                                WaitingManager.Hide();
                                ((Form)extenceInstance).ShowDialog();
                            }
                            else
                            {
                                MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                            }
                        }
                        else
                            MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                    }
                    else
                        MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                }
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
                    var row = (MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, row);
                        data.ID = row.EXP_MEST_ID;
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, data, param);
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
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST)gridView.GetFocusedRow();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BrowseExportTicket").FirstOrDefault();
                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.BrowseExportTicket");
                    MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                }
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.EXP_MEST_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).ShowDialog();
                }
                else
                {
                    MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                }
                //MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                //Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, row);
                //data.ID = row.EXP_MEST_ID;
                //data.EXP_MEST_STT_ID = Base.HisExpMestSttCFG.EXP_MEST_STT_ID__APPROVED;
                //var apiresul = new Inventec.Common.Adapter.BackendAdapter
                //    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                //    (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                //if (apiresul != null && apiresul.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__APPROVED)
                //{
                //    success = true;
                //    FillDataToGrid();
                //}
                //WaitingManager.Hide();
                //#region Show message
                //Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                //#endregion

                //#region Process has exception
                //HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                //#endregion
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
                var row = (MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, row);
                data.ID = row.EXP_MEST_ID;
                data.EXP_MEST_STT_ID = Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
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

        private void ButtonActualExportEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, row);
                data.ID = row.EXP_MEST_ID;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, data, param);
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

        private void ButtonDepaImpEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

                //Chưa có API + SDO


                //WaitingManager.Show();
                //bool success = false;
                //CommonParam param = new CommonParam();

                //var row = (MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST)gridView.GetFocusedRow();
                //if (row != null)
                //{
                //    MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                //    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, row);
                //    data.ID = row.EXP_MEST_ID;
                //    var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.SDO.HisDepaImpMestResultSDO>("/api/HisChmsImpMest/Create", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                //    if (result != null)
                //    {
                //        success = true;
                //        FillDataToGrid();
                //    }
                //}
                //WaitingManager.Hide();
                //#region Show message
                //Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                //#endregion

                //#region Process has exception
                //HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                //#endregion
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
                var row = (MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, row);
                data.ID = row.EXP_MEST_ID;
                data.EXP_MEST_STT_ID = Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
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
