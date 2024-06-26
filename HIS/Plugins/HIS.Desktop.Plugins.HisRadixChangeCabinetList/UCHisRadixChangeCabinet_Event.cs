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
using HIS.Desktop.Plugins.HisRadixChangeCabinetList.Base;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.HisRadixChangeCabinetList
{
    public partial class UCHisRadixChangeCabinet : UserControlBase
    {
        private void repositoryItemButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                V_HIS_EXP_MEST ExpMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(ExpMestData, rowDataExpMest);

                if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL)
                {
                    HIS.Desktop.ADO.ApproveAggrExpMestSDO exeMestView = new HIS.Desktop.ADO.ApproveAggrExpMestSDO(ExpMestData.ID, ExpMestData.EXP_MEST_STT_ID);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(exeMestView);
                    CallModule callModule = new CallModule(CallModule.ApproveAggrExpMest, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.AggrExpMestDetail, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    new CallModule("HIS.Desktop.Plugins.ExpMestDetailBCS", this.roomId, this.roomTypeId, listArgs);
                    WaitingManager.Hide();
                }
                else
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.ExpMestViewDetail, this.roomId, this.roomTypeId, listArgs);

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
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                if (rowDataExpMest != null)
                {

                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentModule);
                    listArgs.Add(rowDataExpMest);
                    CallModule callModule = new CallModule(CallModule.RadixChangeCabinet, this.roomId, this.roomTypeId, listArgs);
                    RefreshData();

                }
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
                    var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                    if (rowDataExpMest != null)
                    {
                        WaitingManager.Show();
                        if (rowDataExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            HisExpMestSDO sdo = new HisExpMestSDO();
                            sdo.ExpMestId = rowDataExpMest.ID;
                            sdo.ReqRoomId = this.roomId;
                            var apiresul = new Inventec.Common.Adapter.BackendAdapter
                                (param).Post<bool>
                                ("api/HisExpMest/BaseDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresul)
                            {
                                success = true;
                                RefreshData();
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
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                if (rowDataExpMest != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(rowDataExpMest);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.BrowseExportTicket, this.roomId, this.roomTypeId, listArgs);
                    btnSearch_Click(null, null);
                    WaitingManager.Hide();
                }
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
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                CommonParam param = new CommonParam();
                bool success = false;
                if (rowDataExpMest != null)
                {
                    WaitingManager.Show();

                    HisExpMestSDO ado = new HisExpMestSDO();
                    ado.ExpMestId = rowDataExpMest.ID;
                    ado.ReqRoomId = this.roomId;
                    if (gridControl.DataSource != null)
                    {
                        var datagridcontrol = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/BaseUnapprove", ApiConsumer.ApiConsumers.MosConsumer, ado, param);
                        if (apiresul != null)
                        {
                            btnSearch_Click(null, null);
                        }
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

        private void ButtonEnableActualExport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                if (row != null)
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
                            IsFinish = true;
                        }

                    }

                    HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                    sdo.ExpMestId = row.ID;
                    sdo.ReqRoomId = this.roomId;
                    sdo.IsFinish = IsFinish;

                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<CabinetBaseResultSDO>
                        ("api/HisExpMest/BaseExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                    if (apiresult != null)
                    {
                        success = true;
                        btnSearch_Click(null, null);
                    }
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

        private void ButtonCopyExportMest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var expMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
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
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                V_HIS_EXP_MEST ExpMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(ExpMestData, rowDataExpMest);

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
                FillDataToControl();
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
                    if (gridControl.DataSource != null)
                    {
                        HisExpMestResultSDO expMestSdo = null;
                        CabinetBaseResultSDO cabinetSdo = null;

                        if (prescription is HisExpMestResultSDO)
                        {

                            expMestSdo = (HisExpMestResultSDO)prescription;
                        }
                        else if (prescription is CabinetBaseResultSDO)
                        {
                            cabinetSdo = (CabinetBaseResultSDO)prescription;
                        }

                        List<V_HIS_EXP_MEST_4> datagridcontrol = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                        foreach (var item in datagridcontrol)
                        {
                            if (expMestSdo != null && item.ID == expMestSdo.ExpMest.ID)
                            {
                                var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == expMestSdo.ExpMest.EXP_MEST_STT_ID);
                                item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                break;
                            }
                            else if (cabinetSdo != null && item.ID == cabinetSdo.ExpMest.ID)
                            {
                                var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == cabinetSdo.ExpMest.EXP_MEST_STT_ID);
                                item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                item.FINISH_TIME = cabinetSdo.ExpMest.FINISH_TIME;
                                item.MODIFY_TIME = cabinetSdo.ExpMest.MODIFY_TIME;
                                item.IMP_MEST_CODE = cabinetSdo.ImpMest.IMP_MEST_CODE;
                                break;
                            }
                        }
                        gridView.BeginUpdate();
                        datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                        gridControl.DataSource = datagridcontrol;
                        gridView.EndUpdate();
                    }
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
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                if (rowDataExpMest != null)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();

                    HisExpMestSDO data = new HisExpMestSDO();
                    data.ExpMestId = rowDataExpMest.ID;
                    data.ReqRoomId = this.roomId;
                    if (gridControl.DataSource != null)
                    {
                        var datagridcontrol = (List<V_HIS_EXP_MEST_4>)gridControl.DataSource;
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/BaseUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                        if (apiresul != null)
                        {
                            foreach (var item in datagridcontrol)
                            {
                                if (item.ID == apiresul.ID)
                                {
                                    var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                    item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                    item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                    item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                    item.IMP_MEST_CODE = "";
                                    item.FINISH_TIME = null;
                                    item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                    break;
                                }
                            }
                            success = true;
                            gridView.BeginUpdate();
                            datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                            gridControl.DataSource = datagridcontrol;
                            gridView.EndUpdate();
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
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                V_HIS_EXP_MEST expMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMestData, rowDataExpMest);
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

        private void Btn_Bill_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                if (rowDataExpMest != null && !rowDataExpMest.BILL_ID.HasValue && rowDataExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(rowDataExpMest.ID);
                    DelegateSelectData dl = new DelegateSelectData(this.ProcessRefressDelefate);
                    listArgs.Add(dl);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.MedicineSaleBill", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_CancelBill_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_4)gridView.GetFocusedRow();
                if (rowDataExpMest != null
                    && rowDataExpMest.BILL_ID.HasValue
                    && rowDataExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                {
                    if (!HisConfigCFG.EXPORT_SALE__MUST_BILL || rowDataExpMest.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {

                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowDataExpMest.BILL_ID.Value);
                        listArgs.Add(rowDataExpMest);
                        DelegateSelectData dl = new DelegateSelectData(this.ProcessRefressDelefate);
                        listArgs.Add(dl);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TransactionCancel", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ProcessRefressDelefate(object data)
        {
            try
            {
                if (data != null)
                {
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
