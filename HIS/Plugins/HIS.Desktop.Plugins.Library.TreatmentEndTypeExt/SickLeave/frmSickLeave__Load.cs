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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.Core.RelaytionType;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using HIS.Desktop.Utility;
using HIS.UC.WorkPlace;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.SickLeave
{
    public partial class frmSickLeave : FormBase
    {
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetConfigSda()
        {
            try
            {
                this.CheDoHienThiNoiLamViecManHinhDangKyTiepDon = ConfigApplicationWorker.Get<long>("CONFIG_KEY__HIEN_THI_NOI_LAM_VIEC_THEO_DINH_DANG_MAN_HINH_DANG_KY");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataDefault()
        {
            try
            {
                spinSickLeaveDay.EditValue = null;
                spinSickLeaveDay.Focus();
                if (treatment != null)
                    txtExtraEndCode.Text = treatment.EXTRA_END_CODE;

                if (TreatmentEndTypeExtData != null)
                {
                    if (TreatmentEndTypeExtData.SickLeaveDay.HasValue)
                        spinSickLeaveDay.EditValue = TreatmentEndTypeExtData.SickLeaveDay;
                    if (TreatmentEndTypeExtData.SickLeaveFrom.HasValue)
                    {
                        DateTime FromTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(TreatmentEndTypeExtData.SickLeaveFrom.Value) ?? DateTime.Now;
                        dtSickLeaveFromTime.DateTime = FromTime;
                    }

                    if (TreatmentEndTypeExtData.SickLeaveTo.HasValue)
                    {
                        DateTime ToTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(TreatmentEndTypeExtData.SickLeaveTo.Value) ?? DateTime.Now;
                        dtSickLeaveToTime.DateTime = ToTime;
                    }


                    txtPatientMotherName.Text = TreatmentEndTypeExtData.MotherName;
                    txtPatientFatherName.Text = TreatmentEndTypeExtData.FatherName;
                    txtRelativeName.Text = TreatmentEndTypeExtData.PatientRelativeName;//TODO
                    cboRelativeType.EditValue = TreatmentEndTypeExtData.PatientRelativeType;//TODO
                    txtPatientWorkPlace.Text = TreatmentEndTypeExtData.PatientWorkPlace;
                    if (TreatmentEndTypeExtData.WorkPlaceId.HasValue && TreatmentEndTypeExtData.WorkPlaceId > 0)
                    {
                        cboWorkPlace.EditValue = TreatmentEndTypeExtData.WorkPlaceId;
                    }
                    else
                        cboWorkPlace.EditValue = null;

                    txtSoThe.Text = TreatmentEndTypeExtData.SickHeinCardNumber;
                    txtLoginName.Text = TreatmentEndTypeExtData.Loginname;
                    cboUser.EditValue = TreatmentEndTypeExtData.Loginname;
                    txtEndTypeExtNote.Text = TreatmentEndTypeExtData.EndTypeExtNote;
                    chkIsPregnancyTermination.Checked = TreatmentEndTypeExtData.IsPregnancyTermination == true;
                    txtGestationAge.Text = TreatmentEndTypeExtData.GestationalAge != null ? TreatmentEndTypeExtData.GestationalAge.ToString() : "";
                    memPregnancyTerminationReason.Text = TreatmentEndTypeExtData.PregnancyTerminationReason;
                    memTreatmentMethod.Text = TreatmentEndTypeExtData.TreatmentMethod;
                    txtBhxhCode.Text = TreatmentEndTypeExtData.SocialInsuranceNumber;
                    if (TreatmentEndTypeExtData.PregnancyTerminationTime.HasValue)
                    {
                        DateTime pregnancyTerminationTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(TreatmentEndTypeExtData.PregnancyTerminationTime.Value) ?? DateTime.Now;
                        dtPregnancyTerminationTime.DateTime = pregnancyTerminationTime;
                    }
                    else
                        dtPregnancyTerminationTime.EditValue = null;
                }
                else if (this.treatment != null)
                {
                    txtSoThe.Text = this.treatment.SICK_HEIN_CARD_NUMBER;
                    if (!String.IsNullOrEmpty(this.treatment.SICK_HEIN_CARD_NUMBER))
                    {
                        txtSoThe.Enabled = false;
                    }
                    if (String.IsNullOrWhiteSpace(this.treatment.SICK_HEIN_CARD_NUMBER))
                    {
                        txtSoThe.Text = this.treatment.TDL_HEIN_CARD_NUMBER;
                    }

                    if (!string.IsNullOrEmpty(this.treatment.TDL_SOCIAL_INSURANCE_NUMBER))
                    {
                        txtBhxhCode.Text = this.treatment.TDL_SOCIAL_INSURANCE_NUMBER;
                    }
                    else if (!string.IsNullOrEmpty(txtSoThe.Text.Trim()))
                    {
                        var soThe = txtSoThe.Text.Replace("-", "");
                        if (soThe.Length > 10) txtBhxhCode.Text = soThe.Substring(soThe.Length - 10, 10);
                    }

                    spinSickLeaveDay.EditValue = this.treatment.SICK_LEAVE_DAY;
                    if (this.treatment.SICK_LEAVE_FROM != null)
                    {
                        DateTime FromTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatment.SICK_LEAVE_FROM.Value) ?? DateTime.Now;
                        dtSickLeaveFromTime.DateTime = FromTime;
                    }
                    else
                    {
                        DateTime inTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatment.IN_TIME) ?? DateTime.Now;
                        dtSickLeaveFromTime.DateTime = inTime;
                    }

                    if (this.treatment.SICK_LEAVE_TO != null)
                    {
                        DateTime ToTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatment.SICK_LEAVE_TO.Value) ?? DateTime.Now;
                        dtSickLeaveToTime.DateTime = ToTime;
                    }

                    txtPatientMotherName.Text = this.treatment.TDL_PATIENT_MOTHER_NAME;
                    txtPatientFatherName.Text = this.treatment.TDL_PATIENT_FATHER_NAME;
                    txtRelativeName.Text = this.treatment.TDL_PATIENT_RELATIVE_NAME;//TODO
                    cboRelativeType.EditValue = this.treatment.TDL_PATIENT_RELATIVE_TYPE;//TODO
                    txtPatientWorkPlace.Text = this.treatment.TDL_PATIENT_WORK_PLACE;

                    if (!String.IsNullOrWhiteSpace(this.treatment.TDL_PATIENT_WORK_PLACE_NAME))
                    {
                        var workPlace = BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.WORK_PLACE_NAME == this.treatment.TDL_PATIENT_WORK_PLACE_NAME);
                        if (workPlace != null)
                        {
                            cboWorkPlace.EditValue = workPlace.ID;
                        }
                    }

                    if (treatmentFinishSDO != null && !String.IsNullOrEmpty(treatmentFinishSDO.SickLoginname))
                    {
                        txtLoginName.Text = treatmentFinishSDO.SickLoginname;
                        cboUser.EditValue = treatmentFinishSDO.SickLoginname;
                    }
                    txtEndTypeExtNote.Text = this.treatment.END_TYPE_EXT_NOTE;
                    chkIsPregnancyTermination.Checked = treatment.IS_PREGNANCY_TERMINATION == 1;
                    txtGestationAge.Text = treatment.GESTATIONAL_AGE != null ? treatment.GESTATIONAL_AGE.ToString() : "";
                    memPregnancyTerminationReason.Text = treatment.PREGNANCY_TERMINATION_REASON;
                    memTreatmentMethod.Text = treatment.TREATMENT_METHOD;
                    if (this.treatment.PREGNANCY_TERMINATION_TIME != null)
                    {
                        DateTime pregnancyTerminationTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatment.PREGNANCY_TERMINATION_TIME.Value) ?? DateTime.Now;
                        dtPregnancyTerminationTime.DateTime = pregnancyTerminationTime;
                    }
                    else
                        dtPregnancyTerminationTime.EditValue = null;
                }

                if (type == FormEnum.TYPE.NGHI_DUONG_THAI)
                {
                    LoadBabyInfor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusMoveout()
        {
            try
            {
                txtRelativeName.Focus();
                txtRelativeName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = this.treatmentId;
                var treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                this.treatment = treatments != null ? treatments.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatmentByPatient()
        {
            try
            {
                if (this.treatment != null)
                {
                    this.lstTreatmentByPatient = new List<HIS_TREATMENT>();
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.PATIENT_ID = this.treatment.PATIENT_ID;
                    filter.TREATMENT_END_TYPE_EXT_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM;
                    filter.TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                    var treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);

                    DateTime? outTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatmentFinishSDO.TreatmentFinishTime);

                    this.lstTreatmentByPatient = treatments != null ? treatments.Where(o => o.VIR_OUT_MONTH == Inventec.Common.TypeConvert.Parse.ToInt64(outTime.Value.ToString("yyyyMM00000000")) && o.ID != this.treatment.ID).ToList() : null;
                }
                else
                {
                    this.lstTreatmentByPatient = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboUser.EditValue = null;
                        this.FocusShowpopup(cboUser, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboUser.EditValue = searchResult[0].LOGINNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                            this.FocusWhileSelectedUser();
                        }
                        else
                        {
                            this.cboUser.EditValue = null;
                            this.FocusShowpopup(cboUser, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRelaytionType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Name", columnInfos, false, 250);
                ControlEditorLoader.Load(cboRelativeType, RelaytionTypeDataWorker.RelaytionTypeADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboWorkPlace()
        {
            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_WORK_PLACE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WORK_PLACE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboWorkPlace, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitComboUser()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

                //Nguoi chi dinh
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboUser, datas, controlEditorADO);

                //if (TreatmentEndTypeExtData != null && !String.IsNullOrEmpty(TreatmentEndTypeExtData.Loginname))
                //{
                //    txtLoginName.Text = TreatmentEndTypeExtData.Loginname;
                //    cboUser.EditValue = TreatmentEndTypeExtData.Loginname;
                //}
                //else if (treatmentFinishSDO != null && !String.IsNullOrEmpty(treatmentFinishSDO.SickLoginname))
                //{
                //    txtLoginName.Text = treatmentFinishSDO.SickLoginname;
                //    cboUser.EditValue = treatmentFinishSDO.SickLoginname;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDocumentBook()
        {
            try
            {
                long yearOutDate = this.treatmentFinishSDO.TreatmentFinishTime > 0 ? Inventec.Common.TypeConvert.Parse.ToInt64(this.treatmentFinishSDO.TreatmentFinishTime.ToString().Substring(0, 4)) : 0;//TODO
                CommonParam param = new CommonParam();
                HisDocumentBookViewFilter documentBookViewFilter = new HisDocumentBookViewFilter();
                documentBookViewFilter.IS_ACTIVE = 1;
                documentBookViewFilter.FOR_SICK_BHXH = true;
                documentBookViewFilter.IS_OUT_NUM_ORDER = false;
                var documentBooks = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_DOCUMENT_BOOK>>("api/HisDocumentBook/GetView", ApiConsumers.MosConsumer, documentBookViewFilter, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => yearOutDate), yearOutDate)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => documentBookViewFilter), documentBookViewFilter)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => documentBooks), documentBooks));

                documentBooks = documentBooks != null && documentBooks.Count > 0 ? documentBooks.Where(o => o.YEAR == null || o.YEAR == yearOutDate).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_BOOK_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_BOOK_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(cboDocumentBook, documentBooks, controlEditorADO);

                if (documentBooks != null && documentBooks.Count == 1)
                {
                    cboDocumentBook.EditValue = documentBooks[0].ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUIByFormType()
        {
            try
            {
                HIS_TREATMENT_END_TYPE_EXT typeExt = new HIS_TREATMENT_END_TYPE_EXT();
                lciIsPregnancyTermination.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciPregnancyTerminationReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciPregnancyTerminationTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciGestationalAge.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciTreatmentMethod.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => type), type));
                if (type == FormEnum.TYPE.NGHI_OM)
                {
                    //this.Height = this.Height - lciMaternityLeave.Height - lciBtnBaby.Height + 2;
                    lciMaternityLeave.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciBtnBaby.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    typeExt = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE_EXT>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM);

                    lciTreatmentMethod.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciIsPregnancyTermination.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciPregnancyTerminationReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciPregnancyTerminationTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciGestationalAge.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciPregnancyTerminationReason.MinSize = new Size(lciIsPregnancyTermination.MinSize.Width, 50);
                    lciTreatmentMethod.MinSize = new Size(lciIsPregnancyTermination.MinSize.Width, 50);
                    lciIsPregnancyTermination.MinSize = new Size(lciIsPregnancyTermination.MinSize.Width, 50);
                    layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    ValidationRequired(memTreatmentMethod);
                }
                else if (type == FormEnum.TYPE.NGHI_DUONG_THAI)
                {
                    lciLaySoTheBHYT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciBhxhCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    typeExt = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE_EXT>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI);
                    dxValidationProvider1.SetValidationRule(txtBhxhCode, null);
                }
                else if (type == FormEnum.TYPE.NGHI_VIEC_HUONG_BHXH)
                {
                    this.Height = this.Height - lciMaternityLeave.Height - lciBtnBaby.Height + 2;
                    lciMaternityLeave.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciBtnBaby.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    typeExt = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE_EXT>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO);
                }

                if (typeExt != null)
                {
                    this.Text = typeExt.TREATMENT_END_TYPE_EXT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
