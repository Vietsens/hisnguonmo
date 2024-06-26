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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using HIS.UC.ExamTreatmentFinish.Config;
using Inventec.Common.WebApiClient;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.UC.ExamTreatmentFinish.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;

namespace HIS.UC.ExamTreatmentFinish.Run
{
    public partial class UCExamTreatmentFinish : UserControl
    {
        private void LoadComboTreatmentEndType()
        {
            try
            {
                treatmentEndTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().Where(o => o.IS_FOR_OUT_PATIENT == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_END_TYPE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("TREATMENT_END_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_END_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboTreatmentEndType, treatmentEndTypes, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboTreatmentResult()
        {
            try
            {
                if (!BackendDataWorker.IsExistsKey<HIS_TREATMENT_RESULT>())
                {
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisTreatmentResultFilter filter = new MOS.Filter.HisTreatmentResultFilter();
                    filter.IS_ACTIVE = 1;
                    treatmentResults = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT>>("api/HisTreatmentResult/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (treatmentResults != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT), treatmentResults, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                else
                {
                    treatmentResults = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_RESULT>().Where(o => o.IS_ACTIVE == 1).ToList();
                }
                treatmentResults = treatmentResults != null ? treatmentResults.OrderBy(o => o.TREATMENT_RESULT_NAME).ToList() : treatmentResults;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_RESULT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("TREATMENT_RESULT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_RESULT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboTreatmentResult, treatmentResults, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboCareer()
        {
            try
            {
                var careers = BackendDataWorker.Get<HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CAREER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("CAREER_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CAREER_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboCareer, careers, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void UpdateProgramData(List<HIS_PATIENT_PROGRAM> patientPrograms, List<V_HIS_DATA_STORE> dataStores)
        {
            try
            {
                this.ExamTreatmentFinishInitADO.PatientPrograms = patientPrograms;
                this.ExamTreatmentFinishInitADO.DataStores = dataStores;
                LoadComboProgram(patientPrograms, dataStores);
                SetDafaultComboProram();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboProgram(List<HIS_PATIENT_PROGRAM> patientPrograms, List<V_HIS_DATA_STORE> dataStores)
        {
            try
            {
                if (chkCapSoLuuTruBA.CheckState != CheckState.Checked)
                    return;
                var programs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PROGRAM>().Where(o => o.IS_ACTIVE == 1 && (o.TREATMENT_TYPE_ID == null || o.TREATMENT_TYPE_ID == this.ExamTreatmentFinishInitADO.Treatment.TDL_TREATMENT_TYPE_ID)).ToList();

                this.ProgramADOList = new List<ProgramADO>();
                foreach (var item in programs)
                {
                    ProgramADO program = new ProgramADO();
                    program.ID = item.ID;
                    program.PROGRAM_NAME = item.PROGRAM_NAME;
                    program.PROGRAM_CODE = item.PROGRAM_CODE;
                    program.DATA_STORE_ID = item.DATA_STORE_ID;

                    var check = patientPrograms != null
                        ? patientPrograms.FirstOrDefault(o => o.PROGRAM_ID == item.ID)
                        : null;

                    if (check != null)
                        program.SelectPatient = true;
                    else
                        program.SelectPatient = false;

                    ProgramADOList.Add(program);
                }

                ProgramADOList = ProgramADOList.Where(o => !o.DATA_STORE_ID.HasValue
                    || (o.DATA_STORE_ID.HasValue && dataStores != null && dataStores.Select(p => p.ID).Contains(o.DATA_STORE_ID.Value))).ToList();

                ProgramADOList = ProgramADOList != null
                    ? ProgramADOList.OrderByDescending(o => o.SelectPatient).ThenBy(p => p.PROGRAM_NAME).ToList()
                    : ProgramADOList;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PROGRAM_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PROGRAM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PROGRAM_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboProgram, ProgramADOList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboTreatmentEndTypeExt()
        {
            try
            {
                //bệnh nhân giới tính khác nữ sẽ không chọn nghỉ dưỡng thai
                List<HIS_TREATMENT_END_TYPE_EXT> lstEndTypeExts = new List<HIS_TREATMENT_END_TYPE_EXT>();
                lstEndTypeExts.AddRange(this.treatmentEndTypeExts);
                if (this.ExamTreatmentFinishInitADO != null && this.ExamTreatmentFinishInitADO.Treatment != null)
                {
                    if (this.ExamTreatmentFinishInitADO.Treatment.TDL_PATIENT_GENDER_ID != IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        lstEndTypeExts = lstEndTypeExts.Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI).ToList();
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_END_TYPE_EXT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("TREATMENT_END_TYPE_EXT_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_END_TYPE_EXT_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboTreatmentEndTypeExt, lstEndTypeExts, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlAppoinment(bool enable)
        {
            try
            {
                //dtTimeAppointment.Enabled = enable;
                //spDay.Enabled = enable;
                chkPrintAppoinment.Enabled = enable;
               
                if (!enable)
                //{
                //    chkPrintAppoinment.CheckState = CheckState.Checked;
                //}
                //else
                {
                    chkPrintAppoinment.CheckState = CheckState.Unchecked;
                }
                chkSignAppoinment.Enabled = enable;
                if (!enable)
                //{
                //    chkSignAppoinment.CheckState = CheckState.Checked;
                //}
                //else
                {
                    chkSignAppoinment.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToControl()
        {
            try
            {
                if (ExamTreatmentFinishInitADO != null)
                {
                    txtAdviseNew.Text = ExamTreatmentFinishInitADO.Advise;
                    txtConclusionNew.Text = ExamTreatmentFinishInitADO.Conclusion;
                    memNote.Text = ExamTreatmentFinishInitADO.Note;
                }
                else
                {
                    txtAdviseNew.Text = "";
                    txtConclusionNew.Text = "";
                    memNote.Text = "";
                }

                if (ExamTreatmentFinishInitADO != null && ExamTreatmentFinishInitADO.Treatment != null)
                {
                    dtTimeIn.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ExamTreatmentFinishInitADO.Treatment.IN_TIME) ?? DateTime.Now;
                    if (ExamTreatmentFinishInitADO.Treatment.OUT_TIME.HasValue)
                    {
                        dtEndTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ExamTreatmentFinishInitADO.Treatment.OUT_TIME.Value) ?? DateTime.Now;
                    }
                    else
                    {
                        dtEndTime.DateTime = DateTime.Now;
                    }
                }

                if (IsAutoCheckBKTheoDoiTuongBN())
                    chkPrintBordereau.CheckState = CheckState.Checked;

                if (ExamTreatmentFinishInitADO != null && ExamTreatmentFinishInitADO.MediRecord != null)
                {
                    lblSoLuuTruBA.Text = ExamTreatmentFinishInitADO.MediRecord.STORE_CODE;
                }
                else
                {
                    lblSoLuuTruBA.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public bool IsAutoCheckBKTheoDoiTuongBN()
        {
            bool result = false;
            try
            {
                string autoCheckBKTheoDoiTuongBenhNhan = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfig.AUTO_CHECK_PRINT_BORDEREAU_BY_PATIENT_TYPE);
                if (!String.IsNullOrEmpty(autoCheckBKTheoDoiTuongBenhNhan))
                {
                    //Lay doi tuong benh nhan hien tai
                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = ExamTreatmentFinishInitADO.Treatment.ID;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);

                    string[] autoCheckBKTheoDoiTuongArr = autoCheckBKTheoDoiTuongBenhNhan.Split(',');
                    if (autoCheckBKTheoDoiTuongArr != null
                        && autoCheckBKTheoDoiTuongArr.Length > 0
                        && PatientTypeAlter != null)
                    {

                        foreach (var item in autoCheckBKTheoDoiTuongArr)
                        {
                            if (item.ToLower().Trim() == PatientTypeAlter.PATIENT_TYPE_CODE.ToLower())
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        //private void SetAppoinmentTime()
        //{
        //    try
        //    {
        //        long priorityAppoinmentTime = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfig.PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY));
        //        if (priorityAppoinmentTime == 1)
        //        {
        //            MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
        //            filter.TREATMENT_ID = ExamTreatmentFinishInitADO.Treatment.ID;
        //            filter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
        //            filter.ORDER_DIRECTION = "DESC";
        //            filter.ORDER_FIELD = "CREATE_TIME";
        //            var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, null);

        //            var serviceReq = serviceReqs.Where(o => o.USE_TIME_TO.HasValue).OrderByDescending(o => o.USE_TIME_TO).FirstOrDefault();
        //            if (serviceReq != null)
        //            {
        //                DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.USE_TIME_TO.Value) ?? DateTime.MinValue;
        //                dtTimeAppointment.DateTime = dtUseTime.AddDays((double)1);
        //                return;
        //            }
        //        }

        //        long appoinmentTimeDefault = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfig.TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY));
        //        if (appoinmentTimeDefault > 0)
        //        {
        //            long endTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
        //            if (endTime > 0)
        //            {
        //                dtTimeAppointment.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Calculation.Add(endTime, appoinmentTimeDefault - 1, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) ?? 0) ?? DateTime.Now;
        //            }
        //            else
        //            {
        //                dtTimeAppointment.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
        //                    Inventec.Common.DateTime.Calculation.Add(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0,
        //                    appoinmentTimeDefault - 1,
        //                    Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY
        //                    ) ?? 0) ?? DateTime.Now;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void LoadTreatmentEndTypeDefault()
        {
            try
            {
                //if (this.ExamTreatmentFinishInitADO.Treatment.TREATMENT_END_TYPE_ID == null || this.ExamTreatmentFinishInitADO.Treatment.TREATMENT_END_TYPE_ID == 0)
                //{
                    long treatmentEndTypeSda = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfig.TREATMENT_END___TREATMENT_END_TYPE_DEFAULT));
                    if (treatmentEndTypeSda == 2)
                    {
                        HIS_TREATMENT_END_TYPE treatmentEndType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV);
                        cboTreatmentEndType.EditValue = treatmentEndType.ID;
                        cboTreatmentEndTypeExt.Enabled = true;
                    }
                    else if (treatmentEndTypeSda == 1)
                    {
                        HIS_TREATMENT_END_TYPE treatmentEndType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN);
                        if (treatmentEndType != null)
                        {
                            cboTreatmentEndType.EditValue = treatmentEndType.ID;
                            EnableControlAppoinment(true);
                            cboTreatmentEndTypeExt.Enabled = true;

                        }
                    }
                    else if (treatmentEndTypeSda == 3)
                    {
                        if (!string.IsNullOrEmpty(this.ExamTreatmentFinishInitADO.Treatment.TDL_HEIN_CARD_NUMBER) && !string.IsNullOrEmpty(this.ExamTreatmentFinishInitADO.Treatment.TDL_HEIN_MEDI_ORG_CODE) && (string.IsNullOrEmpty(BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE) || BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE != this.ExamTreatmentFinishInitADO.Treatment.TDL_HEIN_MEDI_ORG_CODE) && (string.IsNullOrEmpty(BranchDataWorker.Branch.ACCEPT_HEIN_MEDI_ORG_CODE) || (!BranchDataWorker.Branch.ACCEPT_HEIN_MEDI_ORG_CODE.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList().Exists(o => o.Equals(this.ExamTreatmentFinishInitADO.Treatment.TDL_HEIN_MEDI_ORG_CODE)))) && (string.IsNullOrEmpty(BranchDataWorker.Branch.SYS_MEDI_ORG_CODE) || (!BranchDataWorker.Branch.SYS_MEDI_ORG_CODE.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList().Exists(o => o.Equals(this.ExamTreatmentFinishInitADO.Treatment.TDL_HEIN_MEDI_ORG_CODE)))))
                            cboTreatmentEndType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN;
                        else
                            cboTreatmentEndType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV;
                        cboTreatmentEndTypeExt.Enabled = true;
                    }
                    else if (treatmentEndTypeSda == 4)
                    {
                        var lastPatientTypeAlter = new BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, this.ExamTreatmentFinishInitADO.Treatment.ID, null);
                        if ((!string.IsNullOrEmpty(this.ExamTreatmentFinishInitADO.Treatment.TRANSFER_IN_MEDI_ORG_CODE) && lastPatientTypeAlter != null && lastPatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE) || (!string.IsNullOrEmpty(this.ExamTreatmentFinishInitADO.Treatment.TDL_HEIN_CARD_NUMBER) && !string.IsNullOrEmpty(this.ExamTreatmentFinishInitADO.Treatment.TDL_HEIN_MEDI_ORG_CODE) && (string.IsNullOrEmpty(BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE) || BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE != this.ExamTreatmentFinishInitADO.Treatment.TDL_HEIN_MEDI_ORG_CODE) && (string.IsNullOrEmpty(BranchDataWorker.Branch.ACCEPT_HEIN_MEDI_ORG_CODE) || (!BranchDataWorker.Branch.ACCEPT_HEIN_MEDI_ORG_CODE.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList().Exists(o => o.Equals(this.ExamTreatmentFinishInitADO.Treatment.TDL_HEIN_MEDI_ORG_CODE)))) && (string.IsNullOrEmpty(BranchDataWorker.Branch.SYS_MEDI_ORG_CODE) || (!BranchDataWorker.Branch.SYS_MEDI_ORG_CODE.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList().Exists(o => o.Equals(this.ExamTreatmentFinishInitADO.Treatment.TDL_HEIN_MEDI_ORG_CODE))))))
                            cboTreatmentEndType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN;
                        else
                            cboTreatmentEndType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV;
                        cboTreatmentEndTypeExt.Enabled = true;
                    }
                //}
                cboTreatmentResult.EditValue = ExamTreatmentFinishInitADO.Treatment.TREATMENT_RESULT_ID;
                string treatmentResultCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfig.TREATMENT_RESULT__TREATMENT_RESULT_CODE_DEFAULT);
                if (!String.IsNullOrWhiteSpace(treatmentResultCode) && cboTreatmentResult.EditValue == null)
                {
                    var treatmentResult = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.TREATMENT_RESULT_CODE.ToUpper() == treatmentResultCode.ToUpper());
                    if (treatmentResult != null)
                    {
                        cboTreatmentResult.EditValue = treatmentResult.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_DOCUMENT_BOOK> LoadDocumentBook()
        {
            List<V_HIS_DOCUMENT_BOOK> rs = null;
            try
            {
                HisDocumentBookViewFilter dBookFilter = new HisDocumentBookViewFilter();
                dBookFilter.FOR_SICK_BHXH = true;
                dBookFilter.IS_OUT_NUM_ORDER = false;
                dBookFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DOCUMENT_BOOK>>("api/HisDocumentBook/GetView", ApiConsumers.MosConsumer, dBookFilter, null);

                long year = Convert.ToInt64((this.dtEndTime.EditValue != null && this.dtEndTime.DateTime != DateTime.MinValue) ? this.dtEndTime.DateTime.ToString("yyyy") : DateTime.Now.ToString("yyyy"));
                LogSystem.Debug("LoadDocumentBook.Year: " + year);
                rs = rs != null ? rs.Where(o => !o.YEAR.HasValue || o.YEAR.Value == year).ToList() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                rs = null;
            }
            return rs;
        }
    }
}
