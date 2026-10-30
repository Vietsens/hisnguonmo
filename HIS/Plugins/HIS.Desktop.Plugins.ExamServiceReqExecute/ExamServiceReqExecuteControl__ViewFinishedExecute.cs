/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Xem lai thong tin xu tri kham khi y lenh da ket thuc
 *
 *   - Mo y lenh da ket thuc: chi mo nhom "Xu tri kham", cac phan khac van khoa nhu cu
 *   - 4 radio Kham them / Nhap vien / Ket thuc kham / Ket thuc dieu tri van chon duoc,
 *     chon radio nao thi hien UC tuong ung voi du lieu da luu, chi xem khong sua
 *   - Tu tich radio theo HIS_SERVICE_REQ.EXAM_END_TYPE
 *   - Khong validate, khong luu, khong goi cac su kien cua UC lam thay doi man hinh trai
 */
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.UC.ExamFinish;
using HIS.UC.ExamFinish.ADO;
using HIS.UC.ExamServiceAdd;
using HIS.UC.ExamServiceAdd.ADO;
using HIS.UC.ExamTreatmentFinish;
using HIS.UC.Hospitalize;
using HIS.UC.Hospitalize.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl
    {
        bool isViewFinishedExecute = false;

        private void InitViewFinishedExecute()
        {
            try
            {
                this.isViewFinishedExecute = this.HisServiceReqView != null
                    && this.HisServiceReqView.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT;
                if (!this.isViewFinishedExecute)
                    return;

                groupControlTreatmentFinish.Enabled = true;
                chkExamServiceAdd.Enabled = true;
                chkHospitalize.Enabled = true;
                chkExamFinish.Enabled = true;
                chkTreatmentFinish.Enabled = true;

                long? examEndType = this.HisServiceReqView.EXAM_END_TYPE;
                if (examEndType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ.EXAM_END_TYPE__KHAM_THEM)
                    chkExamServiceAdd.Checked = true;
                else if (examEndType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ.EXAM_END_TYPE__NHAP_VIEN)
                    chkHospitalize.Checked = true;
                else if (examEndType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ.EXAM_END_TYPE__KET_THUC_DT)
                    chkTreatmentFinish.Checked = true;
                else if (examEndType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ.EXAM_END_TYPE__KET_THUC)
                    chkExamFinish.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ViewFinishedExamServiceAdd()
        {
            try
            {
                if (!chkExamServiceAdd.Checked)
                {
                    LoadUCToPanelExecuteExt(null, chkExamServiceAdd);
                    return;
                }
                SetCheckExecute(true, chkExamServiceAdd, chkHospitalize, chkTreatmentFinish, chkExamFinish);

                ExamServiceAddInitADO examServiceAddADO = new ExamServiceAddInitADO();
                examServiceAddADO.ServiceReqId = this.HisServiceReqView.ID;
                examServiceAddADO.treatmentId = this.HisServiceReqView.TREATMENT_ID;
                examServiceAddADO.roomId = moduleData.RoomId;
                examServiceAddADO.DefaultIdRoom = moduleData.RoomId;
                examServiceAddADO.FinishTime = this.HisServiceReqView.FINISH_TIME;
                examServiceAddADO.OutTime = this.treatment.OUT_TIME;
                examServiceAddADO.InTime = this.treatment.IN_TIME;
                examServiceAddADO.StartTime = this.HisServiceReqView.START_TIME;
                examServiceAddADO.AppointmentDesc = this.HisServiceReqView.APPOINTMENT_DESC;
                examServiceAddADO.AppointmentTime = this.HisServiceReqView.APPOINTMENT_TIME;
                examServiceAddADO.AppointmentExamRoomId = this.HisServiceReqView.APPOINTMENT_EXAM_ROOM_ID;
                examServiceAddADO.AppointmentExamServiceId = this.HisServiceReqView.APPOINTMENT_EXAM_SERVICE_ID;
                examServiceAddADO.IsMainExam = this.HisServiceReqView.IS_MAIN_EXAM == 1;
                examServiceAddADO.Note = CurrentPatient != null ? CurrentPatient.NOTE : null;

                CommonParam param = new CommonParam();
                HisServiceReqFilter addedReqFilter = new HisServiceReqFilter();
                addedReqFilter.PREVIOUS_SERVICE_REQ_ID = this.HisServiceReqView.ID;
                List<HIS_SERVICE_REQ> addedReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, addedReqFilter, param);
                examServiceAddADO.AddedServiceReq = addedReqs != null ? addedReqs.Where(o => o.IS_DELETE != 1).OrderByDescending(o => o.ID).FirstOrDefault() : null;
                if (examServiceAddADO.AddedServiceReq != null)
                {
                    HisSereServFilter sereServFilter = new HisSereServFilter();
                    sereServFilter.SERVICE_REQ_ID = examServiceAddADO.AddedServiceReq.ID;
                    List<HIS_SERE_SERV> sereServs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);
                    examServiceAddADO.AddedSereServ = sereServs != null ? sereServs.FirstOrDefault() : null;
                }

                examServiceAddProcessor = new ExamServiceAddProcessor();
                this.ucExamAddition = (UserControl)examServiceAddProcessor.Run(examServiceAddADO);
                LoadUCToPanelViewFinished(this.ucExamAddition, chkExamServiceAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ViewFinishedHospitalize()
        {
            try
            {
                if (!chkHospitalize.Checked)
                {
                    LoadUCToPanelExecuteExt(null, chkHospitalize);
                    return;
                }
                SetCheckExecute(true, chkHospitalize, chkExamServiceAdd, chkTreatmentFinish, chkExamFinish);

                // Hien thi giong luong chua ket thuc, chi khoa khong cho sua
                GetUcIcdYHCT();
                HospitalizeInitADO hospitalizeADO = CreateHospitalizeInitADO();
                hospitalizeProcessor = new HospitalizeProcessor();
                this.ucHospitalize = (UserControl)hospitalizeProcessor.Run(hospitalizeADO);
                LoadUCToPanelViewFinished(this.ucHospitalize, chkHospitalize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ViewFinishedExamFinish()
        {
            try
            {
                if (!chkExamFinish.Checked)
                {
                    LoadUCToPanelExecuteExt(null, chkExamFinish);
                    return;
                }
                SetCheckExecute(true, chkExamFinish, chkHospitalize, chkExamServiceAdd, chkTreatmentFinish);

                ExamFinishInitADO examFinishInitADO = new ExamFinishInitADO();
                examFinishInitADO.TreatmentId = this.HisServiceReqView.TREATMENT_ID;
                examFinishInitADO.FinishTime = this.HisServiceReqView.FINISH_TIME;
                examFinishInitADO.InTime = this.treatment.IN_TIME;
                examFinishInitADO.OutTime = this.treatment.OUT_TIME;
                examFinishInitADO.StartTime = this.HisServiceReqView.START_TIME;
                examFinishInitADO.AppointmentDesc = this.HisServiceReqView.APPOINTMENT_DESC;
                examFinishInitADO.AppointmentTime = this.HisServiceReqView.APPOINTMENT_TIME;
                examFinishInitADO.AppointmentExamRoomId = this.HisServiceReqView.APPOINTMENT_EXAM_ROOM_ID;
                examFinishInitADO.AppointmentExamServiceId = this.HisServiceReqView.APPOINTMENT_EXAM_SERVICE_ID;
                examFinishInitADO.IsMainExam = this.HisServiceReqView.IS_MAIN_EXAM == 1;
                examFinishInitADO.Note = CurrentPatient != null ? CurrentPatient.NOTE : null;

                examFinishProcessor = new ExamFinishProcessor();
                this.ucExamFinish = (UserControl)examFinishProcessor.Run(examFinishInitADO);
                LoadUCToPanelViewFinished(this.ucExamFinish, chkExamFinish);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ViewFinishedTreatmentFinish()
        {
            try
            {
                if (!chkTreatmentFinish.Checked)
                {
                    LoadUCToPanelExecuteExt(null, chkTreatmentFinish);
                    return;
                }
                SetCheckExecute(true, chkTreatmentFinish, chkExamServiceAdd, chkHospitalize, chkExamFinish);

                // Dung ban sao ho so da luu, khong lay gio server / ICD / phuong phap dieu tri tren man hinh de ghi de
                HIS_TREATMENT treatmentView = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatmentView, this.treatment);

                HIS.UC.ExamTreatmentFinish.ADO.TreatmentFinishInitADO treatmentFinishInitADO = new HIS.UC.ExamTreatmentFinish.ADO.TreatmentFinishInitADO();
                treatmentFinishInitADO.Treatment = treatmentView;
                treatmentFinishInitADO.IsEmergencyRoom = this.IsEmergencyClassifyRoom();
                treatmentFinishInitADO.EmergencyClassifyId2 = treatmentView.EMERGENCY_CLASSIFY_ID_2;
                treatmentFinishInitADO.BranchName = BranchDataWorker.Branch.BRANCH_NAME;
                treatmentFinishInitADO.moduleData = this.moduleData;
                if (this.patient == null || this.patient.ID == 0)
                {
                    LoadPatient();
                }
                if (this.patient != null)
                {
                    if (!string.IsNullOrEmpty(this.patient.CMND_NUMBER))
                    {
                        treatmentFinishInitADO.CmndDate = this.patient.CMND_DATE;
                        treatmentFinishInitADO.CmndNumber = this.patient.CMND_NUMBER;
                        treatmentFinishInitADO.CmndPlace = this.patient.CMND_PLACE;
                        treatmentFinishInitADO.DocumentType = "CMND";
                    }
                    else if (!string.IsNullOrEmpty(this.patient.CCCD_NUMBER))
                    {
                        treatmentFinishInitADO.CmndDate = this.patient.CCCD_DATE;
                        treatmentFinishInitADO.CmndNumber = this.patient.CCCD_NUMBER;
                        treatmentFinishInitADO.CmndPlace = this.patient.CCCD_PLACE;
                        treatmentFinishInitADO.DocumentType = "CCCD";
                    }
                    treatmentFinishInitADO.CareerId = this.patient.CAREER_ID;
                }
                treatmentFinishInitADO.TraditionalIcdCode = treatmentView.TRADITIONAL_ICD_CODE;
                treatmentFinishInitADO.TraditionalIcdName = treatmentView.TRADITIONAL_ICD_NAME;
                treatmentFinishInitADO.TraditionalIcdText = treatmentView.TRADITIONAL_ICD_TEXT;
                treatmentFinishInitADO.TraditionalIcdSubCode = treatmentView.TRADITIONAL_ICD_SUB_CODE;
                treatmentFinishInitADO.IcdCode = treatmentView.ICD_CODE;
                treatmentFinishInitADO.IcdName = treatmentView.ICD_NAME;

                if (MediRecode == null)
                {
                    LoadMediRecord();
                }
                treatmentFinishInitADO.PatientPrograms = PatientProgramList;
                treatmentFinishInitADO.DataStores = DataStoreList;
                treatmentFinishInitADO.MediRecord = MediRecode;
                treatmentFinishInitADO.TreatmentEndTypeExts = GetTreatmentEndTypeExt();
                var dataRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                treatmentFinishInitADO.IsBlockNumOrder = dataRoom != null && dataRoom.IS_BLOCK_NUM_ORDER == 1;
                treatmentFinishInitADO.dlgGetIcdSubCode = GetIcdSubCode;
                treatmentFinishInitADO.Note = CurrentPatient != null ? CurrentPatient.NOTE : null;
                treatmentFinishInitADO.Advise = this.HisServiceReqView.ADVISE;
                treatmentFinishInitADO.Conclusion = this.HisServiceReqView.CONCLUSION;
                treatmentFinishInitADO.PATHOLOGICAL_PROCESS = txtPathologicalProcess.Text;

                CommonParam param = new CommonParam();
                HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                filter.TREATMENT_ID = treatmentView.ID;
                var dtSevere = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param);
                if (dtSevere != null && dtSevere.Count > 0)
                {
                    treatmentFinishInitADO.SevereIllNessInfo = dtSevere[0];
                    HisEventsCausesDeathFilter filterChild = new HisEventsCausesDeathFilter();
                    filterChild.SEVERE_ILLNESS_INFO_ID = dtSevere[0].ID;
                    treatmentFinishInitADO.ListEventsCausesDeath = new BackendAdapter(param).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, filterChild, param);
                }

                treatmentFinishProcessor = new ExamTreatmentFinishProcessor();
                this.ucTreatmentFinish = (UserControl)treatmentFinishProcessor.Run(treatmentFinishInitADO, this.currentTreatmentExt);
                LoadUCToPanelViewFinished(this.ucTreatmentFinish, chkTreatmentFinish);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadUCToPanelViewFinished(UserControl uc, CheckEdit checkEdit)
        {
            try
            {
                if (uc == null)
                    return;
                // UC set gia tri/Enabled trong su kien Load (co the chay sau khi tab duoc hien) nen khoa lai sau Load
                uc.Load += (sender, e) =>
                {
                    if (this.IsHandleCreated)
                        this.BeginInvoke(new Action(() => SetReadOnlyExecuteExt(uc)));
                };
                LoadUCToPanelExecuteExt(uc, checkEdit);
                SetReadOnlyExecuteExt(uc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khoa nhap lieu trong UC xu tri: o nhap ReadOnly, nut bam disable, luoi khong sua.
        /// Khong disable container (tab, panel, layout) de van chuyen tab Loi dan / Ket luan / Ghi chu va cuon xem duoc.
        /// </summary>
        private void SetReadOnlyExecuteExt(Control control)
        {
            try
            {
                if (control == null)
                    return;

                if (control is BaseEdit)
                {
                    BaseEdit edit = (BaseEdit)control;
                    edit.Properties.ReadOnly = true;
                    ButtonEdit buttonEdit = edit as ButtonEdit;
                    if (buttonEdit != null)
                    {
                        foreach (EditorButton button in buttonEdit.Properties.Buttons)
                        {
                            if (button.Kind != ButtonPredefines.Combo && button.Kind != ButtonPredefines.DropDown)
                                button.Enabled = false;
                        }
                    }
                }
                else if (control is SimpleButton)
                {
                    control.Enabled = false;
                }
                else if (control is GridControl)
                {
                    foreach (BaseView view in ((GridControl)control).Views)
                    {
                        if (view is ColumnView)
                            ((ColumnView)view).OptionsBehavior.Editable = false;
                    }
                }

                // Control them vao sau (vd UC con hien theo hinh thuc ra vien) cung phai khoa
                control.ControlAdded -= ExecuteExtControlAdded;
                control.ControlAdded += ExecuteExtControlAdded;
                foreach (Control child in control.Controls)
                {
                    SetReadOnlyExecuteExt(child);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ExecuteExtControlAdded(object sender, ControlEventArgs e)
        {
            if (this.isViewFinishedExecute)
                SetReadOnlyExecuteExt(e.Control);
        }
    }
}
