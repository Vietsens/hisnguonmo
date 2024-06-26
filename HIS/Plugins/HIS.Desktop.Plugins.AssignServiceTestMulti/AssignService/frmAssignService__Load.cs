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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignServiceTestMulti.ADO;
using HIS.Desktop.Plugins.AssignServiceTestMulti.Resources;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignServiceTestMulti.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private void ValidServiceDetailProcessing(SereServADO sereServADO, bool isValidExecuteRoom)
        {
            try
            {
                if (sereServADO != null)
                {
                    bool vlPatientTypeId = (sereServADO.IsChecked && sereServADO.PATIENT_TYPE_ID <= 0);
                    sereServADO.ErrorMessagePatientTypeId = (vlPatientTypeId ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
                    sereServADO.ErrorTypePatientTypeId = (vlPatientTypeId ? ErrorType.Warning : ErrorType.None);

                    bool vlAmount = (sereServADO.IsChecked && sereServADO.AMOUNT <= 0);
                    sereServADO.ErrorMessageAmount = (vlAmount ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
                    sereServADO.ErrorTypeAmount = (vlAmount ? ErrorType.Warning : ErrorType.None);

                    long intructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((this.dtInstructionTime.EditValue ?? "").ToString()).ToString("yyyyMMdd"));
                    var existsSereServInDate = this.sereServWithTreatment.Any(o => o.SERVICE_ID == sereServADO.SERVICE_ID && o.INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTime.ToString());
                    sereServADO.ErrorMessageIsAssignDay = (existsSereServInDate ? ResourceMessage.CanhBaoDichVuDaChiDinhTrongNgay : "");
                    sereServADO.ErrorTypeIsAssignDay = (existsSereServInDate ? ErrorType.Warning : ErrorType.None);

                    bool vlExecuteRoom = (isValidExecuteRoom && sereServADO.TDL_EXECUTE_ROOM_ID <= 0);
                    sereServADO.ErrorMessageExecuteRoom = (vlExecuteRoom ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
                    sereServADO.ErrorTypeExecuteRoom = (vlExecuteRoom ? ErrorType.Warning : ErrorType.None);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidServiceDetailProcessing(SereServADO sereServADO)
        {
            try
            {
                this.ValidServiceDetailProcessing(sereServADO, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                        this.treeService.UncheckAll();
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
                                this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
                                this.ValidServiceDetailProcessing(sereServADO);
                            }
                            this.toggleSwitchDataChecked.EditValue = true;
                        }
                        this.gridViewServiceProcess.GridControl.DataSource = resultData.OrderByDescending(o => o.SERVICE_NUM_ORDER).ToList();
                        this.gridViewServiceProcess.EndUpdate();

                        this.SetEnableButtonControl(this.actionType);
                        this.SetDefaultSerServTotalPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToComboPriviousServiceReq(HisTreatmentWithPatientTypeInfoSDO currentHisTreatment)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam(0, 10);
                MOS.Filter.HisServiceReqView6Filter serviceReqFilter = new MOS.Filter.HisServiceReqView6Filter();
                serviceReqFilter.TDL_PATIENT_ID = currentHisTreatment.PATIENT_ID;
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
                this.currentPreServiceReqs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6>>(RequestUriStore.HIS_SERVICE_REQ_GETVIEW_6, ApiConsumers.MosConsumer, serviceReqFilter, ProcessLostToken, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_REQ_TYPE_NAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("RENDERER_INTRUCTION_TIME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_INTRUCTION_TIME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboPriviousServiceReq, currentPreServiceReqs, controlEditorADO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan gia trị mac dinh cho cac control can khoi tao san gia tri
        /// </summary>
        private void SetDefaultData()
        {
            try
            {
                this.btnSave.Enabled = false;
                this.btnSaveAndPrint.Enabled = false;
                this.btnShowDetail.Enabled = false;
                this.pnlPrintAssignService.Enabled = false;
                this.cboServiceGroup.EditValue = null;
                this.cboServiceGroup.Properties.Buttons[1].Visible = false;
                this.lblTotalServicePrice.Text = "0";
                this.actionType = GlobalVariables.ActionAdd;
                this.dtInstructionTime.EditValue = DateTime.Now;
                //this.chkPriority.Checked = false;
                //this.chkEmergency.Checked = false;
                //this.txtDescription.Text = "";
                //this.chkExpendAll.Checked = false;
                //this.dtInstructionTime.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitConfig()
        {
            try
            {
                this.DisablecheckEmergencyByConfig();
                this.VisibleColumnInGridControlService();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra nếu có cấu hình 
        /// </summary>
        void DisablecheckEmergencyByConfig()
        {
            try
            {
                //if (this.isAutoEnableEmergency)
                //{
                //    lciEmergency.Enabled = true;
                //}

                //var executeRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>()
                //     .FirstOrDefault(o =>
                //     o.IS_EMERGENCY == IMSys.DbConfig.HIS_RS.HIS_EXECUTE_ROOM.IS_EMERGENCY__TRUE
                //     && o.ROOM_ID == currentModule.RoomId);
                //if (executeRoom != null)
                //{
                //    lciEmergency.Enabled = true;
                //    chkEmergency.CheckState = CheckState.Checked;
                //    chkPriority.CheckState = CheckState.Checked;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void VisibleColumnInGridControlService()
        {
            try
            {
                //An hien cot cp ngoai goi
                long isVisibleColumnCPNgoaiGoi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_CP_NGOAI_GOI);
                if (isVisibleColumnCPNgoaiGoi == 1)
                {
                    gridColumnChiPhiNgoaiGoi_TabService.Visible = false;
                }

                //An hien cot hao phi
                long isVisibleColumnHaoPhi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_HAO_PHI);
                if (isVisibleColumnHaoPhi == 1)
                {
                    grcExpend_TabService.Visible = false;
                }

                //An hien cot gia goi
                long isVisibleColumnGiaGoi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_GIA_GOI);
                if (isVisibleColumnGiaGoi == 1)
                {
                    grcPrice_ServicePatyPrpo.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadLoadDataByPackageService(object param)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataByPackageServiceNewThread));
            try
            {
                thread.Start(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void LoadDataByPackageServiceNewThread(object data)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { LoadDataByPackageService((V_HIS_SERE_SERV)data); }));
                }
                else
                {
                    LoadDataByPackageService((V_HIS_SERE_SERV)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataByPackageService(V_HIS_SERE_SERV currentSereServ)
        {
            try
            {
                if (currentSereServ != null)
                {
                    CommonParam param = new CommonParam();
                    //Lấy list service package
                    HisServicePackageViewFilter filter = new HisServicePackageViewFilter();
                    filter.SERVICE_ID = currentSereServ.SERVICE_ID;
                    var servicePackageByServices = new BackendAdapter(param).Get<List<V_HIS_SERVICE_PACKAGE>>(HisRequestUriStore.HIS_SERVICE_PACKAGE_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    if (servicePackageByServices != null && servicePackageByServices.Count > 0)
                    {
                        List<long> serviceIds = servicePackageByServices.Select(o => o.SERVICE_ATTACH_ID).Distinct().ToList();

                        MOS.Filter.HisServiceViewFilter filterMedicine = new HisServiceViewFilter();
                        filterMedicine.IDs = serviceIds;
                        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceInPackages = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, filterMedicine, param);

                        //Load data for service package
                        LoadPageServiceInServicePackage(serviceInPackages);

                        //Tính lại tổng số tiền đã thanh toán là hao phí trong gói
                        SetTotalPriceInPackage(currentSereServ);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTotalPriceInPackage(V_HIS_SERE_SERV sereServ)
        {
            try
            {
                CommonParam param = new CommonParam();
                //Lấy list service package
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.IS_EXPEND = true;
                sereServFilter.PARENT_ID = sereServ.ID;
                var serviceInPackage__Expends = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, sereServFilter, ProcessLostToken, param);
                if (serviceInPackage__Expends != null && serviceInPackage__Expends.Count > 0)
                {
                    this.currentExpendInServicePackage = serviceInPackage__Expends.Sum(o => (o.AMOUNT * o.PRICE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPageServiceInServicePackage(List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceInPackages)
        {
            try
            {
                this.gridViewServiceProcess.BeginUpdate();
                var allDatas = this.ServiceIsleafADOs.AsQueryable();
                this.treeService.UncheckAll();
                if (serviceInPackages != null && serviceInPackages.Count > 0)
                {
                    var serviceIds = serviceInPackages.Select(o => o.ID).Distinct().ToArray();
                    allDatas = allDatas.Where(o => serviceIds.Contains(o.ID));
                }
                var resultData = allDatas.ToList();
                if (resultData != null && resultData.Count > 0)
                {
                    foreach (var sereServADO in resultData)
                    {
                        sereServADO.IsChecked = true;
                        this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
                        this.ValidServiceDetailProcessing(sereServADO);
                    }
                    this.toggleSwitchDataChecked.EditValue = true;
                }
                this.gridViewServiceProcess.GridControl.DataSource = resultData.OrderByDescending(o => o.SERVICE_NUM_ORDER).ToList();
                this.gridViewServiceProcess.EndUpdate();

                this.SetEnableButtonControl(this.actionType);
                this.SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ LoadDataToCurrentServiceReqData(long treatmentId)
        {
            MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serq = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqViewFilter filter = new MOS.Filter.HisServiceReqViewFilter();
                filter.ID = treatmentId;

                var listSerq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                if (listSerq != null && listSerq.Count > 0)
                {
                    serq = listSerq[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return serq;
        }

        private HisTreatmentWithPatientTypeInfoSDO LoadDataToCurrentTreatmentData(long treatmentId, long intructionTime)
        {
            HisTreatmentWithPatientTypeInfoSDO treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.INTRUCTION_TIME = intructionTime;
                treatment = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>(RequestUriStore.HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO, ApiConsumers.MosConsumer, filter, ProcessLostToken, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        private void ProcessDataWithTreatmentWithPatientTypeInfo()
        {
            try
            {
                var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                if (patientTypeAllows != null && patientTypeAllows.Count > 0 && patientTypes != null)
                {
                    if (this.currentHisTreatment != null && !String.IsNullOrEmpty(this.currentHisTreatment.PATIENT_TYPE_CODE))
                    {
                        var patientType = patientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == this.currentHisTreatment.PATIENT_TYPE_CODE);
                        if (patientType == null) throw new AggregateException("Khong lay duoc thong tin PatientType theo ma doi tuong (PATIENT_TYPE trong HisTreatmentWithPatientTypeInfoSDO).");

                        this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_ID = patientType.ID;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE = this.currentHisTreatment.TREATMENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE = this.currentHisTreatment.HEIN_MEDI_ORG_CODE;
                        this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME = this.currentHisTreatment.HEIN_CARD_FROM_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME = this.currentHisTreatment.HEIN_CARD_TO_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER = this.currentHisTreatment.HEIN_CARD_NUMBER;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = this.currentHisTreatment.RIGHT_ROUTE_TYPE_CODE;
                        this.currentHisPatientTypeAlter.LEVEL_CODE = this.currentHisTreatment.LEVEL_CODE;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_CODE = this.currentHisTreatment.RIGHT_ROUTE_CODE;

                        var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == patientType.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        if (patientTypeAllow != null && patientTypeAllow.Count > 0)
                            this.currentPatientTypeWithPatientTypeAlter = patientTypes.Where(o => patientTypeAllow.Contains(o.ID)).ToList();
                    }
                    else
                        throw new AggregateException("currentHisTreatment.PATIENT_TYPE_CODE is null");
                }
                else
                    throw new AggregateException("patientTypeAllows is null");
            }
            catch (AggregateException ex)
            {
                this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                this.currentPatientTypeWithPatientTypeAlter = new List<HIS_PATIENT_TYPE>();
                MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                Inventec.Common.Logging.LogSystem.Info("LoadDataToCurrentTreatmentData => khong lay duoc doi tuong benh nhan. Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTime), intructionTime) + "____Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisTreatment), currentHisTreatment));
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentPatientTypeAlter(long treatmentId, long intructionTime, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatmentId;
                if (intructionTime > 0)
                    filter.InstructionTime = intructionTime;
                else
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControlsForm()
        {
            try
            {
                this.InitComboRepositoryPatientType(currentPatientTypeWithPatientTypeAlter);
                this.InitGridCheckMarksSelectionServiceGroup();
                this.InitComboServiceGroup();
                this.InitDefaultDataByPatientType();
                this.InitComboExecuteRoom();
                this.ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdDefault()
        {
            try
            {
                //if (currentHisTreatment != null)
                //{
                //    //Nếu hồ sơ chưa có thông tin ICD, và là hồ sơ đến khám theo loại là hẹn khám thì khi chỉ định dịch vụ, tự động hiển thị ICD của đợt điều trị trước, tương ứng với mã hẹn khám
                //    if (currentHisTreatment.ICD_ID == null && currentHisTreatment.APPOINTMENT_ID != null && currentHisTreatment.PREVIOUS_ICD_ID != null)
                //    {
                //        txtIcdMainCode.Text = currentHisTreatment.PREVIOUS_ICD_CODE;
                //        cboIcdServiceReq.EditValue = currentHisTreatment.PREVIOUS_ICD_ID;
                //        if (!string.IsNullOrEmpty(currentHisTreatment.PREVIOUS_ICD_MAIN_TEXT))
                //        {
                //            chkIcdServiceReq.Checked = true;
                //            txtIcdServiceReq.Text = currentHisTreatment.PREVIOUS_ICD_MAIN_TEXT;
                //        }
                //        chkIcdServiceReq.Enabled = true;
                //        if (IcdGeneraCFG.AutoCheckIcd == "1")
                //        {
                //            chkIcdServiceReq.Checked = true;
                //        }
                //        txtIcdExtraNames.Text = currentHisTreatment.PREVIOUS_ICD_NAME;
                //        txtIcdExtraCodes.Text = currentHisTreatment.PREVIOUS_ICD_SUB_CODE;
                //    }
                //    else if (currentHisTreatment.ICD_ID != null)
                //    {
                //        txtIcdMainCode.Text = currentHisTreatment.ICD_CODE;
                //        cboIcdServiceReq.EditValue = currentHisTreatment.ICD_ID;
                //        if (!string.IsNullOrEmpty(currentHisTreatment.ICD_MAIN_TEXT))
                //        {
                //            chkIcdServiceReq.Checked = true;
                //            txtIcdServiceReq.Text = currentHisTreatment.ICD_MAIN_TEXT;
                //        }
                //        chkIcdServiceReq.Enabled = true;
                //        if (IcdGeneraCFG.AutoCheckIcd == "1")
                //        {
                //            chkIcdServiceReq.Checked = true;
                //        }
                //        txtIcdExtraNames.Text = currentHisTreatment.ICD_TEXT;
                //        txtIcdExtraCodes.Text = currentHisTreatment.ICD_SUB_CODE;
                //    }
                //    else
                //    {
                //        chkIcdServiceReq.Enabled = false;
                //        txtIcdExtraNames.Text = currentHisTreatment.ICD_TEXT;
                //        txtIcdExtraCodes.Text = currentHisTreatment.ICD_SUB_CODE;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatmentInfo__PatientType()
        {
            try
            {
                //decimal totalPrice = 0;
                //if (this.dSereServ1WithTreatment != null && this.dSereServ1WithTreatment.Count > 0)
                //{
                //    totalPrice = this.dSereServ1WithTreatment.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);              
                //}
                this.lblPatientName.Text = this.patientName;
                if (this.patientDob > 0)
                    this.lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.patientDob);
                this.lblGenderName.Text = this.genderName;
                if (this.currentHisPatientTypeAlter != null)
                    this.lblPatientTypeName.Text = this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void LoadDSereServ1WithTreatment(long treatmentId, long patientTypeId)
        //{
        //    try
        //    {
        //        LogSystem.Info("Load LoadDSereServ1WithTreatment start");
        //        CommonParam param = new CommonParam();
        //        HisSereServView1Filter hisSereServFilter = new HisSereServView1Filter();
        //        hisSereServFilter.TREATMENT_ID = treatmentId;
        //        hisSereServFilter.PATIENT_TYPE_ID = patientTypeId;
        //        this.dSereServ1WithTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.D_HIS_SERE_SERV_1>>(RequestUriStore.HIS_SERE_SERV_GETD_1, ApiConsumers.MosConsumer, hisSereServFilter, param);
        //        LogSystem.Info("Loaded LoadDSereServ1WithTreatment end");
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void CreateThreadLoadDataSereServWithTreatment(object param)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataSereServWithTreatmentNewThread));
            try
            {
                thread.Start(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void LoadDataSereServWithTreatmentNewThread(object param)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.LoadDataSereServWithTreatment((HisTreatmentWithPatientTypeInfoSDO)param, null); }));
                }
                else
                {
                    this.LoadDataSereServWithTreatment((HisTreatmentWithPatientTypeInfoSDO)param, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataSereServWithTreatment(HisTreatmentWithPatientTypeInfoSDO treatment, DateTime? intructionTime)
        {
            try
            {
                if (treatment != null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServView1Filter hisSereServFilter = new HisSereServView1Filter();
                    hisSereServFilter.TREATMENT_ID = treatment.ID;
                    if (intructionTime != null && intructionTime.Value != DateTime.MinValue)
                    {
                        hisSereServFilter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(intructionTime.Value.ToString("yyyyMMdd") + "000000");
                        hisSereServFilter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(intructionTime.Value.ToString("yyyyMMdd") + "235959");
                    }
                    else
                    {
                        hisSereServFilter.INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Get.StartDay();
                        hisSereServFilter.INTRUCTION_TIME_TO = Inventec.Common.DateTime.Get.EndDay();
                    }
                    hisSereServFilter.INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Get.StartDay();
                    hisSereServFilter.INTRUCTION_TIME_TO = Inventec.Common.DateTime.Get.EndDay();
                    hisSereServFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                    this.sereServWithTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_1, ApiConsumers.MosConsumer, hisSereServFilter, ProcessLostToken, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDefaultDataByPatientType()
        {
            try
            {
                if (this.currentSereServ == null)
                {
                    //this.gridViewServiceProcess.Columns["IsOutKtcFee"].Visible = false;
                }

                this.SetPatientInfoToControl(); //thong tin ve BN                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetPatientInfoToControl()
        {
            try
            {
                if (this.currentHisTreatment != null)
                {
                    //this.lblTreatmentCode_TabBlood.Text = currentHisTreatment.TREATMENT_CODE;
                    //this.lblPatientName_TabBlood.Text = currentHisTreatment.VIR_PATIENT_NAME;
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
                if (patientTypeCombo != null && this.servicePatyInBranchs != null && this.servicePatyInBranchs.Count > 0)
                {
                    var arrPatientTypeCode = this.servicePatyInBranchs[data.SERVICE_ID].Select(o => o.PATIENT_TYPE_CODE).Distinct().ToList();
                    if (arrPatientTypeCode != null && arrPatientTypeCode.Count > 0)
                    {
                        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE)).ToList();
                        this.InitComboPatientType(patientTypeCombo, dataCombo);
                    }
                    else
                    {
                        this.InitComboPatientType(patientTypeCombo, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoExcuteRoomCombo(SereServADO data, DevExpress.XtraEditors.GridLookUpEdit excuteRoomCombo)
        {
            try
            {
                var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                var executeRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                if (excuteRoomCombo != null && executeRoomViews != null && serviceRoomViews != null && serviceRoomViews.Count > 0)
                {
                    var arrExcuteRoomCode = serviceRoomViews.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).Select(o => o.ROOM_ID).ToList();
                    if (arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0)
                    {
                        List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = executeRoomViews.Where(o => arrExcuteRoomCode.Contains(o.ROOM_ID)).ToList();
                        this.InitComboExecuteRoom(excuteRoomCombo, dataCombo);
                        var roomCheck = dataCombo.Any(o => o.ROOM_ID == currentModule.RoomId);
                        if (roomCheck)
                        {
                            data.TDL_EXECUTE_ROOM_ID = currentModule.RoomId;
                        }
                    }
                    else
                    {
                        this.InitComboExecuteRoom(excuteRoomCombo, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
