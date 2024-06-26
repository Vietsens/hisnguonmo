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
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.HisExportChmsList.Base;

namespace HIS.Desktop.Plugins.HisExportChmsList
{
    public partial class UCHisExportChmsList : UserControlBase
    {
        private void repositoryItemButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var ExpMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();

                if (ExpMestData.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.ExpMestViewDetail, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                else
                {
                    HIS.Desktop.ADO.ApproveAggrExpMestSDO exeMestView = new HIS.Desktop.ADO.ApproveAggrExpMestSDO(ExpMestData.ID, ExpMestData.EXP_MEST_STT_ID);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(exeMestView);
                    CallModule callModule = new CallModule(CallModule.ApproveAggrExpMest, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEnableEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var expMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                if (expMestData != null)
                {
                    if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData.ID);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                        CallModule callModule = new CallModule(CallModule.ExpMestOtherExport, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData.ID);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                        CallModule callModule = new CallModule(CallModule.ExpMestSaleCreate, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData);
                        CallModule callModule = new CallModule(CallModule.ExpMestChmsUpdate, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData);
                        CallModule callModule = new CallModule(CallModule.ExpMestDepaUpdate, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData);
                        CallModule callModule = new CallModule(CallModule.ManuExpMestCreate, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else
                        MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                }
                else
                    MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEnableDiscard_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        HisExpMestSDO sdo = new HisExpMestSDO();
                        sdo.ExpMestId = row.ID;
                        sdo.ReqRoomId = this.roomId;
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresul)
                        {
                            success = true;
                            RefreshData();
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

        private void ButtonEnableApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                if (row != null)
                {
                    if ((row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK&&row.IS_REQUEST_BY_PACKAGE!=1)
                             || row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                             || row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP
                             || row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.ID);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                        CallModule callModule = new CallModule(CallModule.BrowseExportTicket, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                        //FillDataApterSave(true);
                    }
                    else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                        hisExpMestApproveSDO.ExpMestId = row.ID;
                        hisExpMestApproveSDO.IsFinish = true;
                        hisExpMestApproveSDO.ReqRoomId = this.roomId;

                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
                   "api/HisExpMest/InPresApprove", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                        if (rs != null)
                        {
                            success = true;
                            RefreshData();
                        }

                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion

                    }
                    else
                    {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                        hisExpMestApproveSDO.ExpMestId = row.ID;
                        hisExpMestApproveSDO.IsFinish = true;
                        hisExpMestApproveSDO.ReqRoomId = this.roomId;

                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
                   "api/HisExpMest/Approve", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                        if (rs != null)
                        {
                            success = true;
                            RefreshData();
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





                //WaitingManager.Show();
                //bool success = false;
                //CommonParam param = new CommonParam();
                //MOS.EFMODEL.DataModels.V_HIS_EXP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                ////if (row.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__CHMS)
                ////{
                ////    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BrowseExportTicket").FirstOrDefault();
                ////    if (moduleData == null)
                ////    {
                ////        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.BrowseExportTicket");
                ////        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                ////    }
                ////    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                ////    {
                ////        List<object> listArgs = new List<object>();
                ////        listArgs.Add(row.ID);
                ////        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                ////        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                ////        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                ////        WaitingManager.Hide();
                ////        ((Form)extenceInstance).ShowDialog();
                ////    }
                ////    else
                ////    {
                ////        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                ////    }
                ////}
                ////else
                ////{
                //MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                //Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, row);
                //data.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                //var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                //if (apiresul != null)
                //{
                //    success = true;
                //    RefreshData();
                //}
                //WaitingManager.Hide();
                //#region Show message
                //Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                //#endregion

                //#region Process has exception
                //HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                //#endregion
                ////}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEnableDisApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HisExpMestSDO ado = new HisExpMestSDO();
                    ado.ExpMestId = row.ID;
                    ado.ReqRoomId = this.roomId;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Decline", ApiConsumer.ApiConsumers.MosConsumer, ado, param);
                    if (apiresul != null)
                    {
                        success = true;
                        RefreshData();
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

        private void ButtonEnableActualExport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                if (row != null)
                {
                    if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                    {
                        bool IsFinish = false;
                        if (row.IS_EXPORT_EQUAL_APPROVE == 1)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Đã xuất hết số lượng duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else if (row.IS_EXPORT_EQUAL_APPROVE == null || row.IS_EXPORT_EQUAL_APPROVE != 1)
                        {
                            HisExpMestMetyReqFilter expMestMetyReqFilter = new HisExpMestMetyReqFilter();
                            expMestMetyReqFilter.EXP_MEST_ID = row.ID;

                            var listExpMestMetyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, expMestMetyReqFilter, param);

                            HisExpMestMatyReqFilter expMestMatyReqFilter = new HisExpMestMatyReqFilter();
                            expMestMatyReqFilter.EXP_MEST_ID = row.ID;

                            var listExpMestMatyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, expMestMatyReqFilter, param);

                            List<AmountADO> amountAdo = new List<AmountADO>();

                            if (listExpMestMetyReq != null && listExpMestMetyReq.Count > 0)
                            {
                                foreach (var item in listExpMestMetyReq)
                                {
                                    var ado = new AmountADO(item);
                                    amountAdo.Add(ado);
                                }
                            }

                            if (listExpMestMatyReq != null && listExpMestMatyReq.Count > 0)
                            {
                                foreach (var item in listExpMestMatyReq)
                                {
                                    var ado = new AmountADO(item);
                                    amountAdo.Add(ado);
                                }
                            }

                            if (amountAdo != null && amountAdo.Count > 0)
                            {
                                var dataAdo = amountAdo.Where(o => o.Amount > o.Dd_Amount || o.Dd_Amount == null).ToList();
                                if (dataAdo != null && dataAdo.Count > 0)
                                {
                                    if (XtraMessageBox.Show("Phiếu chưa duyệt đủ số lượng yêu cầu. Bạn có muốn hoàn thành phiếu xuất?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        IsFinish = true;
                                    }
                                }
                                else
                                    IsFinish = true;
                            }

                        }

                        HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                        sdo.ExpMestId = row.ID;
                        sdo.ReqRoomId = this.roomId;
                        sdo.IsFinish = IsFinish;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresult != null)
                        {
                            success = true;
                            RefreshData();
                        }
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion

                    }
                    else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                    {
                        HisExpMestSDO sdo = new HisExpMestSDO();
                        sdo.ExpMestId = row.ID;
                        sdo.ReqRoomId = this.roomId;
                        //sdo.IsFinish = true;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            ("api/HisExpMest/InPresExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresult != null)
                        {
                            success = true;
                            RefreshData();
                        }
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion

                    }

                    else
                    {
                        HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                        sdo.ExpMestId = row.ID;
                        sdo.ReqRoomId = this.roomId;
                        sdo.IsFinish = true;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresult != null)
                        {
                            success = true;
                            RefreshData();
                        }
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

        private void ButtonCopyExportMest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var expMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                //SessionManager.GetFormMain().ExportMedicineForCopyExpMestClick(expMestData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEnableMobaImpCreate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var ExpMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();

                if (ExpMestData != null)
                {
                    if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ExpMestData.ID);
                        CallModule callModule = new CallModule(CallModule.MobaDepaCreate, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                        FillDataApterClose(ExpMestData);
                    }
                    else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ExpMestData.ID);
                        CallModule callModule = new CallModule(CallModule.MobaBloodCreate, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                        FillDataApterClose(ExpMestData);
                    }
                    else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ExpMestData.ID);
                        CallModule callModule = new CallModule(CallModule.MobaSaleCreate, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                        FillDataApterClose(ExpMestData);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterClose(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST ExpMestData)
        {
            try
            {
                MOS.Filter.HisExpMestView1Filter filter = new MOS.Filter.HisExpMestView1Filter();
                filter.ID = ExpMestData.ID;
                var listTreat = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_1_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (listTreat != null && listTreat.Count == 1)
                {
                    listExpMest[listExpMest.IndexOf(ExpMestData)] = listTreat.First();
                    gridControl.RefreshDataSource();
                }
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

        private void ButtonRequest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy duyệt không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        if (row.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();

                            HisExpMestSDO data = new HisExpMestSDO();
                            data.ExpMestId = row.ID;
                            data.ReqRoomId = this.roomId;
                            var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                            if (apiresul != null)
                            {
                                success = true;
                                RefreshData();
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                        else
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();

                            HisExpMestSDO data = new HisExpMestSDO();
                            data.ExpMestId = row.ID;
                            data.ReqRoomId = this.roomId;
                            var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/InPresUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                            if (apiresul != null)
                            {
                                success = true;
                                RefreshData();
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonAssignTest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var expMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridView.GetFocusedRow();
                if (expMestData != null)
                {
                    List<object> listArgs = new List<object>();
                    AssignServiceTestADO assignBloodADO = new AssignServiceTestADO(0, 0, 0, null);
                    GetTreatmentIdFromResultData(expMestData, ref assignBloodADO);
                    listArgs.Add(assignBloodADO);
                    CallModule callModule = new CallModule(CallModule.AssignServiceTest, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatmentIdFromResultData(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST resultExpMest, ref AssignServiceTestADO assignBloodADO)
        {
            try
            {
                long __expMestId = ((resultExpMest != null && resultExpMest.ID > 0) ? resultExpMest.ID : 0);
                MOS.Filter.HisExpMestViewFilter expFilter = new MOS.Filter.HisExpMestViewFilter();
                expFilter.ID = __expMestId;
                var listExp = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expFilter, null);
                if (resultExpMest != null && listExp.Count == 1)
                {
                    MOS.Filter.HisTreatmentView2Filter treatmentView2Filter = new MOS.Filter.HisTreatmentView2Filter();
                    treatmentView2Filter.PATIENT_ID = listExp.First().TDL_PATIENT_ID;
                    var listTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW_2, ApiConsumer.ApiConsumers.MosConsumer, treatmentView2Filter, null);
                    if (listTreatment != null && listTreatment.Count == 1)
                    {
                        assignBloodADO.TreatmentId = listTreatment.First().ID;
                        assignBloodADO.GenderName = listTreatment.First().TDL_PATIENT_GENDER_NAME;
                        assignBloodADO.PatientDob = (listTreatment.First().TDL_PATIENT_DOB);
                        assignBloodADO.PatientName = listTreatment.First().TDL_PATIENT_NAME;
                        assignBloodADO.ExpMestId = __expMestId;
                        assignBloodADO.ServiceReqId = listExp.First().SERVICE_REQ_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Import


        #endregion
    }
}
