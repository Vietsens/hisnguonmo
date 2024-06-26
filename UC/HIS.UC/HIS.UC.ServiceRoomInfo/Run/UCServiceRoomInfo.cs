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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using HIS.UC.ServiceRoomInfo.ADO;
using HIS.UC.ServiceRoomInfo.Delegate;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;

namespace HIS.UC.ServiceRoomInfo.Run
{
    public partial class UCServiceRoomInfo : DevExpress.XtraEditors.XtraUserControl
    {
        internal MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter;
        List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE> examServiceTypeByPatys;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY> servicePatyInBranchs;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> serviceRooms { get; set; }
        V_HIS_SERE_SERV sereServExam;
        RemoveServiceRoomInfo deleteServiceRoomInfo;
        FoucusMoveOutServiceRoomInfo foucusMoveOutServiceRoomInfo;
        long HIS_ROOM_TYPE_ID__DV;
        bool isInit;
        string lciName;
        int positionHandleControl = -1;

        public enum ControlType
        {
            Assign,
            Register
        }
        public UCServiceRoomInfo(ServiceRoomInfoInitADO serviceRoomInfoInitADO)
        {
            try
            {
                InitializeComponent();
                if (serviceRoomInfoInitADO != null)
                {
                    //this.currentHisTreatment = serviceRoomInfoInitADO.currentHisTreatment;
                    this.currentHisPatientTypeAlter = serviceRoomInfoInitADO.currentHisPatientTypeAlter;
                    this.currentPatientTypeWithPatientTypeAlter = serviceRoomInfoInitADO.currentPatientTypeWithPatientTypeAlter;
                    this.examServiceTypeByPatys = serviceRoomInfoInitADO.HisExamServiceTypes;
                    this.servicePatyInBranchs = serviceRoomInfoInitADO.HisVServicePatyInBranchs;
                    this.serviceRooms = serviceRoomInfoInitADO.VHisServiceRooms;
                    this.HIS_ROOM_TYPE_ID__DV = serviceRoomInfoInitADO.HIS_ROOM_TYPE_ID__DV;

                    this.deleteServiceRoomInfo = serviceRoomInfoInitADO.DeleteServiceRoomInfo;
                    this.foucusMoveOutServiceRoomInfo = serviceRoomInfoInitADO.FoucusMoveOutServiceRoomInfo;

                    if (serviceRoomInfoInitADO.ServiceRoomInfoGenerateADO != null)
                    {
                        this.isInit = serviceRoomInfoInitADO.ServiceRoomInfoGenerateADO.IsInit;
                        this.lciName = serviceRoomInfoInitADO.ServiceRoomInfoGenerateADO.LayoutControlItemName;
                        this.sereServExam = serviceRoomInfoInitADO.ServiceRoomInfoGenerateADO.sereServExam;
                    }

                    LogSystem.Info("Du lieu dich vu kham: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => examServiceTypeByPatys), examServiceTypeByPatys));
                    LogSystem.Info("Du lieu doi tuong chap nhan thanh toan: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientTypeWithPatientTypeAlter), this.currentPatientTypeWithPatientTypeAlter));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeysFromLanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageUCServiceRoomInfo = new ResourceManager("HIS.Desktop.Plugins.AssignPrescription.Resources.Lang", typeof(HIS.UC.ServiceRoomInfo.Run.UCServiceRoomInfo).Assembly);

                //lciExamServiceType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCSERVICEROOMINFO_LCI_EXAM_SERVICE_TYPE", Resources.ResourceLanguageManager.LanguageUCServiceRoomInfo, ServiceRoomConfig.Culture);
                //lciRoom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCSERVICEROOMINFO_LCI_ROOM", Resources.ResourceLanguageManager.LanguageUCServiceRoomInfo, ServiceRoomConfig.Culture);
                //lciObjectofpaymentCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCSERVICEROOMINFO_LCI_OBJECTOFPAYMENT_CODE", Resources.ResourceLanguageManager.LanguageUCServiceRoomInfo, ServiceRoomConfig.Culture);
                //lciPrioritize.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCSERVICEROOMINFO_LCI_PRIORITIZE", Resources.ResourceLanguageManager.LanguageUCServiceRoomInfo, ServiceRoomConfig.Culture);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCServiceRoomInfo_Load(object sender, EventArgs e)
        {
            try
            {
                LoadKeysFromLanguage();
                VisibilityControl();
                ValidateControl();
                LoadCurrentExamServiceByPatientTypeId();
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void VisibilityControl()
        {
            try
            {
                if (this.isInit)
                {
                    lciBtnDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciBtnDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Init()
        {
            try
            {
                if (sereServExam != null)
                {
                    var examSety = examServiceTypeByPatys.FirstOrDefault(o => o.SERVICE_ID == sereServExam.SERVICE_ID);
                    if (examSety != null)
                    {
                        cboExamServiceType.EditValue = examSety.ID;
                        txtExamServiceType.Text = examSety.EXAM_SERVICE_TYPE_CODE;
                        cboExamServiceType.Properties.Buttons[1].Visible = true;
                    }

                    LoadPhongKhamComboByServicePackage(sereServExam.SERVICE_ID, cboRoom);
                }
                InitComboExamServiceType(cboExamServiceType, examServiceTypeByPatys);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadPhongKhamComboByServicePackage(long serviceId, GridLookUpEdit cbo)
        {
            try
            {
                if (serviceId > 0)
                {
                    //Load phong kham theo examServiceTypeId
                    var examSVDTO = examServiceTypeByPatys.FirstOrDefault(o => o.SERVICE_ID == serviceId);
                    if (examSVDTO == null) throw new ArgumentNullException("ExamServiceType is null");

                    var dataRoom = serviceRooms.Where(o => (examSVDTO != null && o.SERVICE_ID == examSVDTO.SERVICE_ID) && o.ROOM_TYPE_ID == HIS_ROOM_TYPE_ID__DV).ToList();
                    cboRoom.Properties.View.Columns.Clear();
                    InitComboRoom(cboRoom, dataRoom);
                    if (dataRoom != null && dataRoom.Count == 1)
                    {
                        cboRoom.Properties.Buttons[1].Visible = true;
                        cboRoom.EditValue = dataRoom[0].ROOM_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadCurrentExamServiceByPatientTypeId()
        {
            try
            {
                if (examServiceTypeByPatys != null && examServiceTypeByPatys.Count > 0)
                {
                    if (servicePatyInBranchs != null && servicePatyInBranchs.Count > 0)
                    {
                        var arrServiceIds = servicePatyInBranchs.Where(o => this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == o.PATIENT_TYPE_ID).Select(o => o.SERVICE_ID).Distinct().ToArray();
                        if (arrServiceIds != null && arrServiceIds.Count() > 0)
                        {
                            LogSystem.Info("Du lieu arrServiceIds: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => arrServiceIds), arrServiceIds));
                            examServiceTypeByPatys = examServiceTypeByPatys.Where(o => arrServiceIds.Contains(o.SERVICE_ID)).ToList();
                            LogSystem.Info("Du lieu examServiceTypeByPatys sau khi loc: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => examServiceTypeByPatys), examServiceTypeByPatys));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.deleteServiceRoomInfo != null)
                    this.deleteServiceRoomInfo(this.lciName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool ValidationControl()
        {
            bool valid = true;
            try
            {
                this.positionHandleControl = -1;
                valid = dxValidationProvider1.Validate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        public void FocusInControl()
        {
            try
            {
                txtExamServiceType.Focus();
                txtExamServiceType.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue(V_HIS_PATIENT patient)
        {
            try
            {
                if (patient != null)
                {
                    var examServiceType = examServiceTypeByPatys.FirstOrDefault(o => o.SERVICE_ID == (patient.RECENT_SERVICE_ID ?? 0));
                    if (examServiceType != null)
                    {
                        txtExamServiceType.Text = examServiceType.EXAM_SERVICE_TYPE_CODE;
                        cboExamServiceType.EditValue = examServiceType.SERVICE_ID;
                        LoadPhongKhamComboByServicePackage(examServiceType.SERVICE_ID, cboRoom);
                        cboRoom.EditValue = patient.RECENT_ROOM_ID;
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
