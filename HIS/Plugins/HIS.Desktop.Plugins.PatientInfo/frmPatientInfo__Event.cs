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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PatientInfo.Resources;
using HIS.Desktop.Utility;
using HIS.UC.WorkPlace;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PatientInfo
{
    public partial class frmPatientInfo : HIS.Desktop.Utility.FormBase
    {
        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandleControlPatientInfo = -1;
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }

                MOS.SDO.HisPatientUpdateSDO sdo = new MOS.SDO.HisPatientUpdateSDO();
                MOS.EFMODEL.DataModels.HIS_PATIENT patientUpdateForm = new HIS_PATIENT();

                //if (this.currentPatient != null)
                //{
                //    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT>(patientUpdateForm, this.currentPatient);
                //}

                UpdateDataForm(ref patientUpdateForm);

                //if (!string.IsNullOrEmpty(patientUpdateForm.TDL_HEIN_CARD_NUMBER) && CheckHeinCardExist(patientUpdateForm.TDL_HEIN_CARD_NUMBER))
                //{
                //    DevExpress.XtraEditors.XtraMessageBox.Show("Thẻ BHYT đã được sử dụng bởi bệnh nhân khác");
                //    return;
                //}

                WaitingManager.Show();

                if (this.currentPatient != null)
                {
                    sdo.HisPatient = patientUpdateForm;
                    sdo.HisPatient.ID = this.currentPatient.ID;
                    //Inventec.Common.Logging.LogSystem.Info("sdo.HisPatient.COMMUNE_NAME: " + sdo.HisPatient.COMMUNE_NAME + "- " + sdo.HisPatient.COMMUNE_CODE);
                    var resultApi = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, sdo, param);
                    if (resultApi != null)
                    {
                        success = true;
                        btnSave.Enabled = false;
                    }
                }
                else
                {
                    //Inventec.Common.Logging.LogSystem.Info("patientUpdateForm.COMMUNE_NAME: " + patientUpdateForm.DISTRICT_NAME + "-" + patientUpdateForm.COMMUNE_CODE);
                    var resultApi = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/Create", ApiConsumers.MosConsumer, patientUpdateForm, param);
                    if (resultApi != null)
                    {
                        success = true;
                        btnSave.Enabled = false;
                        //this.currentPatient = new V_HIS_PATIENT();
                        //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_PATIENT>(this.currentPatient, resultApi);
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
            MessageManager.Show(this, param, success);
        }

        private void UpdateDataForm(ref HIS_PATIENT updateData)
        {
            try
            {
                var tinhKhaiSinhUpdate = BackendDataWorker.Get<SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == GetStringByCombo(cboTinhKhaiSinh));
                updateData.ADDRESS = txtAddress.Text;
                updateData.BLOOD_ABO_CODE = GetStringByCombo(cboNhomMau);
                updateData.BLOOD_RH_CODE = GetStringByCombo(cboRh);
                updateData.BORN_PROVINCE_CODE = GetStringByCombo(cboTinhKhaiSinh);
                updateData.BORN_PROVINCE_NAME = tinhKhaiSinhUpdate != null ? tinhKhaiSinhUpdate.PROVINCE_NAME : "";

                updateData.CAREER_ID = GetIdByCombo(cboCareer);
                var carrer = BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.ID == GetIdByCombo(cboCareer));
                if (carrer != null)
                {
                    updateData.CAREER_CODE = carrer.CAREER_CODE;
                    updateData.CAREER_NAME = carrer.CAREER_NAME;
                }

                if (txtCMND.Text.Length == 9)
                {
                    updateData.CMND_NUMBER = txtCMND.Text;
                    updateData.CMND_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNgayCap.DateTime);
                    updateData.CMND_PLACE = txtNoiCap.Text;
                }
                else if (txtCMND.Text.Length == 12)
                {
                    updateData.CCCD_NUMBER = txtCMND.Text;
                    updateData.CCCD_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNgayCap.DateTime);
                    updateData.CCCD_PLACE = txtNoiCap.Text;
                }
                else
                {
                    updateData.CMND_NUMBER = "";
                    updateData.CMND_DATE = null;
                    updateData.CMND_PLACE = "";
                    updateData.CCCD_NUMBER = "";
                    updateData.CCCD_DATE = null;
                    updateData.CCCD_PLACE = "";
                }
                updateData.COMMUNE_CODE = GetStringByCombo(cboCommune);
                updateData.COMMUNE_NAME = cboCommune.Text;
                updateData.DISTRICT_CODE = GetStringByCombo(cboDistricts);
                updateData.DISTRICT_NAME = cboDistricts.Text;

                Inventec.Common.Logging.LogSystem.Info(updateData.COMMUNE_NAME);
                try
                {
                    if (dtDOB.DateTime != null)
                    {
                        if (txtPatientDOB.Text.Length == 4)
                        {
                            updateData.IS_HAS_NOT_DAY_DOB = 1;
                            string dateDob = txtPatientDOB.Text.Substring(0, 4) + "0101";
                            string timeDob = "00";
                            updateData.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
                        }
                        else
                        {
                            txtPatientDOB.Text = dtDOB.Text;
                            string dateDob = dtDOB.DateTime.ToString("yyyyMMdd");
                            string timeDob = "00";
                            updateData.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
                            updateData.IS_HAS_NOT_DAY_DOB = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                try
                {
                    int idx = txtPatientName.Text.Trim().LastIndexOf(" ");
                    if (idx > -1)
                    {
                        updateData.FIRST_NAME = txtPatientName.Text.Trim().Substring(idx).Trim();
                        updateData.LAST_NAME = txtPatientName.Text.Trim().Substring(0, idx).Trim();
                    }
                    else
                    {
                        updateData.FIRST_NAME = txtPatientName.Text.Trim();
                        updateData.LAST_NAME = "";
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                
                updateData.EMAIL = txtEmail.Text;
                updateData.ETHNIC_CODE = txtEthnic.Text;
                updateData.ETHNIC_NAME = cboEthnic.Text;
                updateData.FATHER_NAME = txtHoTenBo.Text;
                updateData.GENDER_ID = GetIdByCombo(cboGender1) ?? 0;
                updateData.HOUSEHOLD_CODE = txtSoHoKhau.Text;
                updateData.HOUSEHOLD_RELATION_NAME = cboQuanHeChuHo.Text;
                updateData.HT_ADDRESS = txtDiaChiHienTai.Text;
                updateData.HT_COMMUNE_NAME = cboXaHienTai.Text;
                updateData.HT_DISTRICT_NAME = cboHuyenHienTai.Text;
                updateData.HT_PROVINCE_NAME = cboTinhHienTai.Text;
                updateData.IS_CHRONIC = chkBNManTinh.Checked ? (short?)1 : null;
                updateData.MILITARY_RANK_ID = GetIdByCombo(cboMilitaryRank);
                updateData.MOTHER_NAME = txtHoTenMe.Text;
                updateData.NATIONAL_CODE = GetStringByCombo(cboNation);
                updateData.NATIONAL_NAME = cboNation.Text;
                updateData.PROVINCE_CODE = GetStringByCombo(cboProvince);
                updateData.PROVINCE_NAME = cboProvince.Text;
                updateData.PHONE = txtPhone.Text;
                updateData.RELATIVE_ADDRESS = txtContact.Text;
                updateData.RELATIVE_TYPE = txtRelation.Text;
                updateData.RELATIVE_CMND_NUMBER = txtCMNDRelative.Text;
                updateData.RELATIVE_NAME = txtPersonFamily.Text;
                updateData.FATHER_EDUCATIIONAL_LEVEL = this.currentPatient.FATHER_EDUCATIIONAL_LEVEL;
                updateData.MOTHER_EDUCATIIONAL_LEVEL = this.currentPatient.MOTHER_EDUCATIIONAL_LEVEL;
                updateData.MOTHER_CAREER = this.currentPatient.MOTHER_CAREER;
                updateData.FATHER_CAREER = this.currentPatient.FATHER_CAREER;
                updateData.RELATIVE_MOBILE = this.currentPatient.RELATIVE_MOBILE;
                updateData.RELATIVE_PHONE = this.currentPatient.RELATIVE_PHONE;
                updateData.UUID = this.currentPatient.UUID;
                updateData.PT_PATHOLOGICAL_HISTORY = this.currentPatient.PT_PATHOLOGICAL_HISTORY;
                updateData.PT_PATHOLOGICAL_HISTORY_FAMILY = this.currentPatient.PT_PATHOLOGICAL_HISTORY_FAMILY;
                updateData.UUID_BHYT_NUMBER = this.currentPatient.UUID_BHYT_NUMBER;
                updateData.ACCOUNT_NUMBER = this.currentPatient.ACCOUNT_NUMBER;
                updateData.TAX_CODE = this.currentPatient.TAX_CODE;
                updateData.PATIENT_STORE_CODE = this.currentPatient.PATIENT_STORE_CODE;
                updateData.RELIGION_NAME = this.currentPatient.RELIGION_NAME;

                //if (!string.IsNullOrEmpty(this.txtSoTheBHYT.Text))
                //    updateData.TDL_HEIN_CARD_NUMBER = HeinUtils.TrimHeinCardNumber(this.txtSoTheBHYT.Text);
                //else
                //    updateData.TDL_HEIN_CARD_NUMBER = "";
                if (workPlaceTemplate == WorkPlaceProcessor.Template.Combo)
                {
                    updateData.WORK_PLACE_ID = (long?)workPlaceProcessor.GetValue(ucWorkPlace, workPlaceTemplate);
                    updateData.WORK_PLACE = "";
                }
                else
                {
                    updateData.WORK_PLACE = (string)workPlaceProcessor.GetValue(ucWorkPlace, workPlaceTemplate);
                    updateData.WORK_PLACE_ID = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool CheckHeinCardExist(string heinCardNumber)
        {
            bool rs = false;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                filter.HEIN_CARD_NUMBER__EXACT = heinCardNumber;
                var patients = (new BackendAdapter(param).Get<List<HisPatientSDO>>("api/HisPatient/GetSdoAdvance", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));
                if (patients != null && patients.Count > 0)
                {
                    rs = true;
                }
            }
            catch (Exception ex)
            {
                rs = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private long? GetIdByCombo(LookUpEdit cbo)
        {
            long? Id = null;
            try
            {
                if (cbo.EditValue != null)
                {
                    Id = Inventec.Common.TypeConvert.Parse.ToInt64(cbo.EditValue.ToString());
                }
                else
                {
                    Id = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return Id;
        }

        private string GetStringByCombo(LookUpEdit cbo)
        {
            string edit = "";
            try
            {
                if (cbo.EditValue != null)
                {
                    edit = cbo.EditValue.ToString();
                }
                else
                {
                    edit = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }

            return edit;
        }

        private void txtGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadGioiTinhCombo(strValue, cboGender1, txtGender, txtPatientDOB);
                    SearchPatientByFilterCombo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEthnic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadDanTocCombo(strValue, false, cboEthnic, txtEthnic, txtCareer);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtNation.Text))
                    {
                        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                        LoadQuocTichCombo(strValue, false, cboNation, txtNation, pnlWorkPlace);
                    }
                    else
                    {
                        cboNation.Focus();
                        cboNation.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMilitaryRankCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        cboMilitaryRank.EditValue = null;
                        FocusShowPopup(this.cboMilitaryRank);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().Where(o => o.MILITARY_RANK_CODE.ToUpper().Contains(searchCode)).ToList();
                        List<HIS_MILITARY_RANK> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.MILITARY_RANK_CODE.ToUpper() == searchCode).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            txtMilitaryRankCode.Text = result[0].MILITARY_RANK_CODE;
                            cboMilitaryRank.EditValue = result[0].ID;
                            FocusMoveText(this.txtPhone);
                        }
                        else
                        {
                            cboMilitaryRank.EditValue = null;
                            FocusShowPopup(this.cboMilitaryRank);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender1_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboGender1.EditValue != null && cboGender1.EditValue != cboGender1.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_GENDER gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboGender1.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtGender.Text = gt.GENDER_CODE;
                            txtPatientDOB.Focus();
                            txtPatientDOB.SelectAll();
                            SearchPatientByFilterCombo();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEthnic_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEthnic.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_ETHNIC ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == ((cboEthnic.EditValue ?? "").ToString()));
                        if (ethnic != null)
                        {
                            txtEthnic.Text = ethnic.ETHNIC_CODE;
                        }
                    }
                    FocusMoveText(this.txtCareer);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCareer_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCareer.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_CAREER career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCareer.EditValue ?? 0).ToString()));
                        if (career != null)
                        {
                            txtCareer.Text = career.CAREER_CODE;
                        }
                    }
                    FocusMoveText(this.txtAddress);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboProvince.EditValue != null )
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtProvince.Text = province.PROVINCE_CODE;
                        }
                    }
                    //txtDistricts.Text = "";
                    txtDistricts.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDistricts.EditValue != null )
                    {
                        string str = cboDistricts.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == cboDistricts.EditValue.ToString()
                                && (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()))
                            {
                                cboProvince.EditValue = district.PROVINCE_CODE;
                                txtProvince.Text = district.PROVINCE_CODE;
                            }
                            txtDistricts.Text = district.DISTRICT_CODE;
                            cboCommune.EditValue = null;
                            txtCommune.Text = "";
                            LoadCommuneCombo("", district.DISTRICT_CODE, false);
                        }
                    }
                    FocusMoveText(this.txtCommune);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCommune.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == cboCommune.EditValue.ToString()
                                    && (String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (cboDistricts.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            txtCommune.Text = commune.COMMUNE_CODE;
                            if (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()) || String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()))
                            {
                                cboDistricts.EditValue = commune.DISTRICT_CODE;
                                txtDistricts.Text = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    cboProvince.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusMoveText(this.txtNation);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMilitaryRank.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MILITARY_RANK commune = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMilitaryRank.EditValue.ToString()));
                        if (commune != null)
                        {
                            txtMilitaryRankCode.Text = commune.MILITARY_RANK_CODE;
                        }
                    }
                    FocusMoveText(this.txtPhone);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodAbo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
                btnSave_Click(null, null);
        }

        private void cboProvince_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboProvince.EditValue != null)
                    {
                        string str = cboProvince.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtProvince.Text = province.SEARCH_CODE;
                            txtDistricts.Text = "";
                            txtDistricts.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            GetNotInListValue(sender, e, cboDistricts);
        }

        private void cboCommune_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            GetNotInListValue(sender, e, cboCommune);
        }

        private void txtIB_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNation_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboNation.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_NATIONAL data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_CODE == ((cboNation.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtNation.Text = data.NATIONAL_CODE;
                            if (workPlaceProcessor != null && workPlaceTemplate != null)
                                workPlaceProcessor.FocusControl(workPlaceTemplate);
                        }
                    }
                    else
                    {
                        if (workPlaceProcessor != null && workPlaceTemplate != null)
                            workPlaceProcessor.FocusControl(workPlaceTemplate);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusMoveText(this.txtProvince);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtContact_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCMNDRelative.Focus();
                    txtCMNDRelative.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //txtGender.Focus();
                //e.Handled = true;
                //SearchPatientByFilterCombo();
            }
        }

        private void txtPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGender.Focus();
                    txtGender.SelectAll();
                    SearchPatientByFilterCombo();
                }
                if (e.KeyCode == Keys.Tab)
                {
                    e = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCareer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadNgheNghiepCombo(strValue, false, cboCareer, txtCareer, txtAddress);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDOB_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEthnic.Focus();
                    SearchPatientByFilterCombo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEmail.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvince_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadProvinceCombo(strValue.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtDistricts_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string provinceCode = "";
                    if (cboProvince.EditValue != null)
                    {
                        provinceCode = cboProvince.EditValue.ToString();
                        LoadDistrictsCombo(strValue.ToUpper(), provinceCode, true);
                    }
                    else {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin tỉnh", "Thông báo");
                    }        
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommune_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string districtCode = "";
                    if (cboDistricts.EditValue != null)
                    {
                        districtCode = cboDistricts.EditValue.ToString();
                        LoadCommuneCombo(strValue.ToUpper(), districtCode, true);
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin huyện", "Thông báo");
                    }
                    
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmail_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPersonFamily.Focus();
                txtPersonFamily.SelectAll();
            }
        }

        private void txtDR_Enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

            }

        }

        private void txtPersonFamily_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRelation.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtContact.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtProvince.Text))
                {
                    cboProvince.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMilitaryRank.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MILITARY_RANK commune = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMilitaryRank.EditValue.ToString()));
                        if (commune != null)
                        {
                            txtMilitaryRankCode.Text = commune.MILITARY_RANK_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkBNManTinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTinhKhaiSinh.Focus();
                    txtTinhKhaiSinh.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDOB_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtPatientDOB.Visible = true;

                    txtPatientDOB.Text = dtDOB.DateTime.ToString("dd/MM/yyyy");
                    string strDob = txtPatientDOB.Text;
                    SearchPatientByFilterCombo();

                    FocusMoveText(this.txtEthnic);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDOB_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDOB.Visible = true;
                    dtDOB.Update();
                    txtPatientDOB.Text = dtDOB.DateTime.ToString("dd/MM/yyyy");

                    //TimBenhNhanTheoDieuKien(true);
                    SearchPatientByFilterCombo();

                    System.Threading.Thread.Sleep(100);
                    FocusMoveText(this.txtEthnic);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDOB_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!txtPatientDOB.ReadOnly && e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtPatientDOB.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtDOB.EditValue = dt;
                        dtDOB.Update();
                    }
                    dtDOB.Visible = true;
                    dtDOB.ShowPopup();
                    dtDOB.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDOB_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtPatientDOB.Text))
                {
                    if (!txtPatientDOB.ReadOnly)
                    {
                        txtPatientDOB.Text = PatientDobUtil.PatientDobToDobRaw(txtPatientDOB.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDOB_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void txtPatientDOB_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPatientDOB.Text))
                    {
                        txtPatientDOB.Text = PatientDobUtil.PatientDobToDobRaw(txtPatientDOB.Text);
                    }

                    if (!String.IsNullOrEmpty(txtPatientDOB.Text))
                    {
                        string strDob = "";

                        if (txtPatientDOB.Text.Length == 2 || txtPatientDOB.Text.Length == 1)
                        {
                            int patientDob = Int32.Parse(txtPatientDOB.Text);
                            if (patientDob < 7)
                            {
                                MessageBox.Show(ResourceLanguageManager.NgaySinhKhongDuocNhoHon7);
                                FocusMoveText(this.txtPatientDOB);
                                return;
                            }
                            else
                            {
                                txtPatientDOB.Text = (DateTime.Now.Year - patientDob).ToString();
                            }
                        }
                        else if (txtPatientDOB.Text.Length == 4)
                        {
                            if (Inventec.Common.TypeConvert.Parse.ToInt64(txtPatientDOB.Text) <= DateTime.Now.Year)
                            {
                                strDob = "01/01/" + txtPatientDOB.Text;
                                isNotPatientDayDob = true;
                            }
                            else
                            {
                                MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                                FocusMoveText(this.txtPatientDOB);
                                return;
                            }

                        }
                        else if (txtPatientDOB.Text.Length == 8)
                        {
                            var year = txtPatientDOB.Text.Substring(4);
                            var day = txtPatientDOB.Text.Substring(0, 2);
                            var month = txtPatientDOB.Text.Substring(2, 2);
                            if (Inventec.Common.DateTime.Check.IsValidTime(year + month + day + "000000"))
                            {
                                strDob = txtPatientDOB.Text.Substring(0, 2) + "/" + txtPatientDOB.Text.Substring(2, 2) + "/" + txtPatientDOB.Text.Substring(4, 4);
                                if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date)
                                {
                                    strDob = txtPatientDOB.Text.Substring(0, 2) + "/" + txtPatientDOB.Text.Substring(2, 2) + "/" + txtPatientDOB.Text.Substring(4, 4);
                                    txtPatientDOB.Text = strDob;
                                    isNotPatientDayDob = false;
                                }
                                else
                                {
                                    MessageBox.Show(ResourceLanguageManager.ThongTinNgaySinhPhaiNhoHonNgayHienTai);
                                    FocusMoveText(this.txtPatientDOB);
                                    return;
                                }
                            }
                            else
                            {
                                MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                                FocusMoveText(this.txtPatientDOB);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                            FocusMoveText(this.txtPatientDOB);
                            return;
                        }


                        if (String.IsNullOrWhiteSpace(strDob))
                        {
                            strDob = txtPatientDOB.Text;
                        }
                        SearchPatientByFilterCombo();
                        isTxtPatientDobPreviewKeyDown = true;

                        FocusMoveText(this.txtEthnic);
                    }
                    else
                    {
                        FocusMoveText(this.txtPatientDOB);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtPatientDOB.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtDOB.EditValue = dt;
                        dtDOB.Update();
                    }

                    dtDOB.Visible = true;
                    dtDOB.ShowPopup();
                    dtDOB.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
