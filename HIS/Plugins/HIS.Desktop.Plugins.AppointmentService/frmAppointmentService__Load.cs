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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.AppointmentService.Base;
using HIS.Desktop.Plugins.AppointmentService.Config;
using HIS.Desktop.Plugins.AppointmentService.Resources;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AppointmentService
{
    public partial class frmAppointmentService : FormBase
    {
        public enum ACTION
        {
            CREATE,
            UPDATE
        }

        private void LoadCurrentPatyAlter()
        {
            try
            {
                CommonParam param = new CommonParam();

                HisTreatmentFilter tFilter = new HisTreatmentFilter();
                tFilter.ID = this.treatmentId;
                List<HIS_TREATMENT> hisTreatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, tFilter, param);
                this.treatment = hisTreatments != null ? hisTreatments.FirstOrDefault() : null;

                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatmentId;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                this.PatientTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);

                this.listPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                patientTypeIdAls = this.listPatientType != null ? listPatientType.Where(o => o.IS_ACTIVE == 1).Select(s => s.ID).ToList() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToComboPriviousServiceReq(V_HIS_PATIENT_TYPE_ALTER patyAlter)
        {
            try
            {
                if (patyAlter == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("FillDataToComboPriviousServiceReq patyAlteris null");
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam(0, 10);
                MOS.Filter.HisServiceReqView6Filter serviceReqFilter = new MOS.Filter.HisServiceReqView6Filter();
                serviceReqFilter.TDL_PATIENT_ID = patyAlter.TDL_PATIENT_ID;
                serviceReqFilter.ORDER_DIRECTION = "DESC";
                serviceReqFilter.ORDER_FIELD = "CREATE_TIME";
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long>();
                //Nếu thêm một loại yêu cầu dv khác thì phải vào đây bổ sung
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN);
                Inventec.Common.Logging.LogSystem.Debug("begin call HisServiceReq/GetView6");

                List<V_HIS_SERVICE_REQ_6> currentPreServiceReqs = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6>>("api/HisServiceReq/GetView6", ApiConsumers.MosConsumer, serviceReqFilter, param);
                Inventec.Common.Logging.LogSystem.Debug("end call HisServiceReq/GetView6");
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_REQ_TYPE_NAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("RENDERER_INTRUCTION_TIME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_INTRUCTION_TIME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(this.cboPriviousServiceReq, currentPreServiceReqs, controlEditorADO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitComboRepositoryPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(this.repositoryItemcboPatientType_TabService, data, controlEditorADO);
                }
                else
                    ControlEditorLoader.Load(this.repositoryItemcboPatientType_TabService, this.listPatientType, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void LoadConfig()
        {
            try
            {
                HisConfigCFG.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGrid(bool isAutoSetPaty)
        {
            try
            {
                this.gridViewServiceProcess.BeginUpdate();
                this.gridViewServiceProcess.ClearGrouping();
                var allDatas = this.ServiceIsleafADOs.AsQueryable();
                List<SereServADO> listSereServADO = null;
                if (this.toggleSwitchDataChecked.EditValue != null && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true")
                {
                    listSereServADO = allDatas.Where(o => o.IsChecked).ToList();
                }
                else
                {
                    //Lay tat ca cac node duoc check tren tree

                    var nodeCheckeds = this.treeService.GetAllCheckedNodes();
                    if (nodeCheckeds != null && nodeCheckeds.Count > 0)
                    {
                        List<ServiceADO> parentNodes = new List<ServiceADO>();
                        listSereServADO = new List<SereServADO>();
                        List<long?> parentIds = new List<long?>();
                        List<long> serviceTypeIds = new List<long>();

                        //lay data cua cac dong tuong ung voi cac node duoc check
                        foreach (var node in nodeCheckeds)
                        {
                            var data = this.treeService.GetDataRecordByNode(node) as ServiceADO;
                            if (data != null)
                            {
                                parentNodes.Add(data);
                            }
                        }
                        if (parentNodes.Count > 0)
                        {
                            var checkPtttSelected = parentNodes.Any(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);                           
                            var parentIdAllows = parentNodes.Select(o => o.ID).ToList();

                            //Lay tat ca cac dich vụ khong co cha cua tat ca cac loai dich vụ duoc check tren tree
                            var parentRootSetys = parentNodes.Where(o => String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                            if (parentRootSetys != null && parentRootSetys.Count > 0)
                            {
                                foreach (var item in parentRootSetys)
                                {
                                    if (item != null)
                                    {
                                        var childOfParentNodeNoParents = allDatas.Where(o =>
                                        (o.PARENT_ID == null || item.ID == o.PARENT_ID)
                                        && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID
                                        && parentIdAllows.Contains(o.PARENT_ID ?? 0)
                                        ).ToList();
                                        if (childOfParentNodeNoParents != null && childOfParentNodeNoParents.Count > 0)
                                        {
                                            listSereServADO.AddRange(childOfParentNodeNoParents);
                                        }
                                    }
                                }

                                //Inventec.Common.Logging.LogSystem.Warn(" listSereServADO after foreach (var item in parentRootSetys)" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listSereServADO), listSereServADO));
                            }

                            //Lay ra tat ca cac dich vụ con cua dich vu cha va cac dich vu con cua con cua no neu co -> duyet de quy cho den het cay dich vu,..
                            var parentRoots = parentNodes.Where(o => !String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                            //Inventec.Common.Logging.LogSystem.Warn(" parentRoots " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => parentRoots), parentRoots));
                            if (parentRoots != null && parentRoots.Count > 0)
                            {
                                foreach (var item in parentRoots)
                                {
                                    var childs = GetChilds(item);
                                    if (childs != null && childs.Count > 0)
                                    {
                                        listSereServADO.AddRange(childs);
                                    }
                                }
                            }
                            listSereServADO = listSereServADO.Distinct().ToList();
                        }
                    }
                    else
                    {
                        listSereServADO = allDatas.ToList();
                    }
                }
                //Inventec.Common.Logging.LogSystem.Warn(" listSereServADO end  " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listSereServADO), listSereServADO));

                if (!String.IsNullOrWhiteSpace(txtServiceName_Search.Text) && listSereServADO != null && listSereServADO.Count() > 0)
                {
                    listSereServADO = listSereServADO.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(txtServiceName_Search.Text.ToLower().Trim())).ToList();
                }
                if (!String.IsNullOrWhiteSpace(txtServiceCode_Search.Text) && listSereServADO != null && listSereServADO.Count() > 0)
                {
                    listSereServADO = listSereServADO.Where(o => o.SERVICE_CODE_HIDDEN.ToLower().Contains(txtServiceCode_Search.Text.ToLower().Trim())).ToList();
                }

                this.gridViewServiceProcess.GridControl.DataSource = listSereServADO.Distinct().OrderBy(o => o.SERVICE_TYPE_ID).ThenByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                this.gridViewServiceProcess.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<SereServADO> GetChilds(ServiceADO parentNode)
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                if (parentNode != null)
                {
                    var childs = ServiceIsleafADOs.Where(o => o.PARENT_ID == parentNode.ID && o.SERVICE_TYPE_ID == parentNode.SERVICE_TYPE_ID).ToList();
                    if (childs != null && childs.Count > 0)
                    {
                        result.AddRange(childs);
                    }

                    var childOfParents = dicService[SERVICE_ENUM.PARENT].Where(o => o.PARENT_ID == parentNode.ID && o.SERVICE_TYPE_ID == parentNode.SERVICE_TYPE_ID).ToList();
                    if (childOfParents != null && childOfParents.Count > 0)
                    {
                        foreach (var item in childOfParents)
                        {
                            var childOfChilds = GetChilds(item);
                            if (childOfChilds != null && childOfChilds.Count > 0)
                            {
                                result.AddRange(childOfChilds);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void EnableControlSave(bool enabled)
        {
            try
            {
                btnSave.Enabled = enabled;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadAppointmentService(long treatmentId)
        {
            try
            {
                if (this.ServiceIsleafADOs == null || this.ServiceIsleafADOs.Count == 0)
                    return;

                CommonParam param = new CommonParam();
                HisAppointmentServFilter appointmentServFilter = new HisAppointmentServFilter();
                appointmentServFilter.TREATMENT_ID = treatmentId;
                List<HIS_APPOINTMENT_SERV> appoinmentServs = new BackendAdapter(param)
                    .Get<List<HIS_APPOINTMENT_SERV>>("api/HisAppointmentServ/Get", ApiConsumers.MosConsumer, appointmentServFilter, param);
                if (appoinmentServs != null && appoinmentServs.Count > 0)
                {
                    foreach (var item in appoinmentServs)
                    {
                        SereServADO sereServADO = this.ServiceIsleafADOs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
                        sereServADO.IsChecked = true;
                        sereServADO.AMOUNT = item.AMOUNT;
                        sereServADO.AppointmentSereServId = item.ID;
                        sereServADO.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID ?? 0;
                    }
                    toggleSwitchDataChecked.EditValue = true;
                    this.action = ACTION.UPDATE;
                    this.LoadDataToGrid(true);
                }
                else
                {
                    action = ACTION.CREATE;
                }
                List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                gridControlAppointmentServ.BeginUpdate();
                gridControlAppointmentServ.DataSource = serviceCheckeds__Send;
                gridControlAppointmentServ.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCboServiceGroup()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var serviceGroups = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>();

                serviceGroups = (serviceGroups != null && serviceGroups.Count > 0) ?
                    serviceGroups.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE &&
                        ((o.IS_PUBLIC ?? -1) == 1 || o.CREATOR.ToLower() == loginName.ToLower()))
                        .ToList()
                : serviceGroups;

                // order tăng dần theo num_order
                if (serviceGroups != null && serviceGroups.Count > 0)
                {
                    var serviceGroup1s = serviceGroups.Where(u => u.NUM_ORDER != null).OrderBy(u => u.NUM_ORDER).ThenBy(o => o.SERVICE_GROUP_NAME);
                    var serviceGroup2s = serviceGroups.Where(u => u.NUM_ORDER == null).OrderBy(o => o.SERVICE_GROUP_NAME);
                    var serviceGroupConcats = serviceGroup1s.Concat(serviceGroup2s);
                    cboServiceGroup.Properties.DataSource = serviceGroupConcats.ToList();
                }
                else
                {
                    cboServiceGroup.Properties.DataSource = null;
                }

                cboServiceGroup.Properties.DisplayMember = "SERVICE_GROUP_NAME";
                cboServiceGroup.Properties.ValueMember = "ID";
                //DevExpress.XtraGrid.Columns.GridColumn col2 = cboServiceGroup.Properties.View.Columns.AddField("SERVICE_GROUP_CODE");
                //col2.VisibleIndex = 1;
                //col2.Width = 100;
                //col2.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboServiceGroup.Properties.View.Columns.AddField("SERVICE_GROUP_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "Tên";
                cboServiceGroup.Properties.PopupFormWidth = 320;
                cboServiceGroup.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboServiceGroup.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitGridCheckMarksSelectionServiceGroup()
        {
            try
            {
                this.cboServiceGroup.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboServiceGroup_CustomDisplayText);
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboServiceGroup.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(cboServiceGroup__SelectionChange);
                cboServiceGroup.Properties.Tag = gridCheck;
                cboServiceGroup.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboServiceGroup.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //    gridCheckMark.ClearSelection(cboServiceGroup.Properties.View);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectOneServiceGroupProcess(List<HIS_SERVICE_GROUP> svgrs)
        {
            try
            {
                List<SereServADO> services = null;
                StringBuilder strMessage = new StringBuilder();
                StringBuilder strMessageTemp__CoDichVuKhongCauHinh = new StringBuilder();
                StringBuilder strMessageTemp__KhongDichVu = new StringBuilder();
                bool hasMessage = false;
                this.ResetServiceGroupSelected();
                if (svgrs != null && svgrs.Count > 0)
                {
                    var idSelecteds = svgrs.Select(o => o.ID).Distinct().ToList();
                    var servSegrAllows = BackendDataWorker.Get<V_HIS_SERV_SEGR>().Where(o => idSelecteds.Contains(o.SERVICE_GROUP_ID)).ToList();
                    if (servSegrAllows != null && servSegrAllows.Count > 0)
                    {
                        var serviceOfGroupsInGroupbys = servSegrAllows.GroupBy(o => o.SERVICE_GROUP_ID).ToDictionary(o => o.Key, o => o.ToList());
                        foreach (var item in serviceOfGroupsInGroupbys)
                        {
                            List<V_HIS_SERV_SEGR> servSegrErrors = new List<V_HIS_SERV_SEGR>();
                            foreach (var svInGr in serviceOfGroupsInGroupbys[item.Key])
                            {
                                var service = this.ServiceIsleafADOs.FirstOrDefault(o => svInGr.SERVICE_ID == o.SERVICE_ID);
                                if (service != null)
                                {
                                    service.IsChecked = true;
                                    service.IsKHBHYT = false;
                                    service.SERVICE_GROUP_ID_SELECTEDs = idSelecteds;
                                    var searchServiceOfGroups = servSegrAllows.Where(o => o.SERVICE_ID == service.SERVICE_ID).ToList();
                                    if (searchServiceOfGroups != null)
                                    {
                                        service.AMOUNT = searchServiceOfGroups.Sum(o => o.AMOUNT);
                                        service.IsExpend = (searchServiceOfGroups[0].IS_EXPEND == 1);
                                    }
                                }
                                else
                                {
                                    servSegrErrors.Add(svInGr);
                                }
                            }

                            if (servSegrErrors != null && servSegrErrors.Count > 0)
                            {
                                if (String.IsNullOrEmpty(strMessageTemp__CoDichVuKhongCauHinh.ToString()))
                                {
                                    strMessageTemp__CoDichVuKhongCauHinh.Append("; ");
                                }
                                strMessageTemp__CoDichVuKhongCauHinh.Append(String.Format(ResourceMessage.NhomDichVuChiTiet, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(servSegrErrors[0].SERVICE_GROUP_NAME, Color.Red), FontStyle.Bold), String.Join(",", servSegrErrors.Select(o => Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(o.SERVICE_CODE, Color.Black), FontStyle.Bold)))));

                                hasMessage = true;
                            }
                            servSegrErrors = new List<V_HIS_SERV_SEGR>();
                        }

                        services = this.ServiceIsleafADOs.Where(o => o.IsChecked).OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                    }
                    var sgNotIn = servSegrAllows.Select(o => o.SERVICE_GROUP_ID).Distinct().ToArray();
                    var searchServiceOfGroups__NoService = svgrs.Where(o => !sgNotIn.Contains(o.ID)).ToList();
                    if (searchServiceOfGroups__NoService != null && searchServiceOfGroups__NoService.Count > 0)
                    {
                        strMessageTemp__KhongDichVu.Append(String.Join(",", searchServiceOfGroups__NoService.Select(o => Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(o.SERVICE_GROUP_NAME, Color.Red), FontStyle.Bold))));
                        hasMessage = true;
                    }

                    this.toggleSwitchDataChecked.EditValue = true;

                    if (hasMessage)
                    {
                        strMessage.Append(ResourceMessage.NhomDichVuCoDichVuDuocCauHinhTrongNhomNhungKhongCoCauHinhChinhSach);
                        if (!String.IsNullOrEmpty(strMessageTemp__CoDichVuKhongCauHinh.ToString()))
                        {
                            strMessage.Append("\r\n" + String.Format(ResourceMessage.NhomDichVuCoDichVuKhongCoCauHinh, strMessageTemp__CoDichVuKhongCauHinh.ToString()));
                        }
                        if (!String.IsNullOrEmpty(strMessageTemp__KhongDichVu.ToString()))
                        {
                            strMessage.Append("\r\n" + String.Format(ResourceMessage.NhomDichVuKhongCoDichVu, strMessageTemp__KhongDichVu.ToString()));
                        }
                        strMessage.Append("\r\n" + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.CacDichVuKhongCoChinhSachGiaHoacKhongCoCauHinhSeKhongDuocChon, Color.Maroon), FontStyle.Italic));
                        WaitingManager.Hide();
                        MessageManager.Show(strMessage.ToString());
                    }
                    else
                    {
                        WaitingManager.Hide();
                    }
                    //this.VerifyWarningOverCeiling();
                }
                else
                {
                    services = this.ServiceIsleafADOs;
                    this.toggleSwitchDataChecked.EditValue = false;
                }
                this.gridControlServiceProcess.DataSource = services;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetServiceGroupSelected()
        {
            try
            {
                if (this.ServiceIsleafADOs.Any(o => o.IsChecked == true))
                {
                    foreach (var item in this.ServiceIsleafADOs)
                    {
                        if (item.SERVICE_GROUP_ID_SELECTEDs != null && item.SERVICE_GROUP_ID_SELECTEDs.Count > 0)
                        {
                            item.IsChecked = false;
                            item.SERVICE_GROUP_ID_SELECTEDs = null;
                            item.TDL_EXECUTE_ROOM_ID = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessChoiceServiceReqPrevious(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6 serviceReq)
        {
            try
            {
                if (serviceReq != null)
                {
                    var allDatas = this.ServiceIsleafADOs.AsQueryable();
                    List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServs = SereServGet.GetByServiceReqId(serviceReq.ID);
                    if (sereServs != null && sereServs.Count > 0)
                    {
                        this.gridViewServiceProcess.BeginUpdate();
                        treeService.UncheckAll();
                        if (sereServs != null && sereServs.Count > 0)
                        {
                            var serviceIds = sereServs.Select(o => o.SERVICE_ID).Distinct().ToArray();
                            allDatas = allDatas.Where(o => serviceIds.Contains(o.SERVICE_ID));
                        }
                        var resultData = allDatas.ToList();

                        if (resultData != null && resultData.Count > 0)
                        {
                            foreach (var sereServADO in resultData)
                            {
                                sereServADO.IsChecked = true;
                                //this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
                                //this.ValidServiceDetailProcessing(sereServADO);
                            }
                            this.toggleSwitchDataChecked.EditValue = true;
                        }
                        this.gridViewServiceProcess.GridControl.DataSource = resultData.OrderByDescending(o => o.SERVICE_NUM_ORDER).ToList();
                        this.gridViewServiceProcess.EndUpdate();

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(SereServADO data, GridLookUpEdit patientTypeCombo)
        {
            try
            {
                if (patientTypeCombo != null)
                {
                    List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = null;

                    if (BranchDataWorker.HasServicePatyWithListPatientType(data.SERVICE_ID, patientTypeIdAls))
                    {
                        var arrPatientTypeCode = BranchDataWorker.DicServicePatyInBranch[data.SERVICE_ID].Select(o => o.PATIENT_TYPE_CODE).Distinct().ToList();
                        dataCombo = (arrPatientTypeCode != null && arrPatientTypeCode.Count > 0 ? this.listPatientType.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE)).ToList() : null);
                    }

                    this.InitComboPatientType(patientTypeCombo, dataCombo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType(GridLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SetDefaultSerServTotalPrice()
        {
            try
            {
                decimal totalPrice = 0;
                decimal totalBhytPrice = 0;
                if (ServiceIsleafADOs != null && ServiceIsleafADOs.Count > 0)
                {
                    foreach (var item in ServiceIsleafADOs)
                    {
                        if (item.IsChecked && item.PATIENT_TYPE_ID != 0 && (item.IsExpend ?? false) == false)
                        {
                            var servicePaties = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.ServicePatyWithListPatientType(item.SERVICE_ID, this.patientTypeIdAls);
                            V_HIS_SERVICE_PATY data_ServicePrice = null;
                            V_HIS_SERVICE_PATY bhyt_ServicePrice = null;
                            if (servicePaties != null && servicePaties.Count > 0)
                            {
                                data_ServicePrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, this.branch.ID, null, null, null, instructionTime, this.treatment.IN_TIME, item.SERVICE_ID, item.PATIENT_TYPE_ID, null);
                                if (item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                {
                                    bhyt_ServicePrice = data_ServicePrice;
                                }
                                else
                                {
                                    bhyt_ServicePrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, this.branch.ID, null, null, null, instructionTime, this.treatment.IN_TIME, item.SERVICE_ID, HisConfigCFG.PatientTypeId__BHYT, null);
                                }
                            }

                            if (data_ServicePrice != null)
                            {
                                totalPrice += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO));
                            }

                            if (item.HEIN_LIMIT_PRICE.HasValue)
                            {
                                totalBhytPrice += (item.AMOUNT * item.HEIN_LIMIT_PRICE.Value);
                            }
                            else if (bhyt_ServicePrice != null)
                            {
                                totalBhytPrice += (item.AMOUNT * bhyt_ServicePrice.PRICE * (1 + bhyt_ServicePrice.VAT_RATIO));
                            }
                        }
                    }
                }
                this.lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);
                this.lblTotalPriceBHYT.Text = Inventec.Common.Number.Convert.NumberToString(totalBhytPrice, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
