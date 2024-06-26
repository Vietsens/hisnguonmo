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
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PatientUpdateCard
{
    public partial class frmPatientUpdateCard : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module module;
        internal HisPatientSDO currentPatientInRegisterForm { get; set; }
        CommonParam param = new CommonParam();
        V_HIS_CARD hisCard = null;
        HisPatientSDO patient = null;
        List<HisPatientSDO> lstPatient = null;
        public frmPatientUpdateCard(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            this.module = _module;

        }
        void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmPatientUpdateCard_Load(object sender, EventArgs e)
        {
            InitWCFReadCard();
            SetIcon();
            LoadComboGender();
            SetCaptionByLanguageKey();
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PatientUpdateCard.Resources.Lang", typeof(HIS.Desktop.Plugins.PatientUpdateCard.frmPatientUpdateCard).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefesh.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.btnRefesh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGender.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.cboGender.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtDob.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.dtDob.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkPatient.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.lkPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdateCard.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDobPatientToForm(MOS.EFMODEL.DataModels.HIS_PATIENT patientDTO)
        {
            try
            {
                if (patientDTO != null)
                {
                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDTO.DOB) ?? DateTime.MinValue;
                    dtDob.EditValue = dtNgSinh;
                    txtDob.Text = dtNgSinh.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FillDataPatientToControl(HisPatientSDO patientDTO)
        {
            try
            {
                if (patientDTO == null)
                {
                    txtPatientCode.Text = null;
                    txtPatientName.Text = null;
                    txtTreatmentCode.Text = null;
                    txtHisCard.Text = null;
                    txtBHYTCard.Text = null;
                    txtPatientName.Text = null;
                    txtDob.Text = null;
                    dtDob.EditValue = null;
                    txtGender.Text = null;
                    cboGender.EditValue = null;
                    txtAddress.Text = null;
                }
                else
                {
                    patient.ID = patientDTO.ID;
                    txtPatientCode.Text = patientDTO.PATIENT_CODE;
                    txtPatientName.Text = patientDTO.VIR_PATIENT_NAME;
                    if (patientDTO.DOB > 0 && patientDTO.DOB.ToString().Length >= 6)
                    {
                        LoadDobPatientToForm(patientDTO);
                    }

                    MOS.EFMODEL.DataModels.HIS_GENDER gioitinh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == patientDTO.GENDER_ID);
                    if (gioitinh != null)
                    {
                        txtGender.Text = gioitinh.GENDER_CODE;
                        cboGender.EditValue = gioitinh.ID;
                    }
                    //txtTreatmentCode.Text = patientDTO.HIS_TREATMENT.FirstOrDefault().TREATMENT_CODE;
                    txtAddress.Text = patientDTO.VIR_ADDRESS;
                    //txtHisCard.Text = patientDTO.HIS_CARD.FirstOrDefault().CARD_NUMBER;
                    //HIS_PATIENT_TYPE_ALTER patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    //HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                    //patientTypeAlterFilter.TDL_PATIENT_ID = patientDTO.ID;
                    //var listPatientTypeAlter = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                    //if (listPatientTypeAlter != null && listPatientTypeAlter.Count > 0)
                    //{
                    //    patientTypeAlter = listPatientTypeAlter.FirstOrDefault();
                    //}
                    txtBHYTCard.Text = patientDTO.HeinCardNumber;
                    txtHisCard.Text = patientDTO.CardCode;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtPatientCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientName.Text = null;
                    txtTreatmentCode.Text = null;
                    txtHisCard.Text = null;
                    txtBHYTCard.Text = null;
                    txtPatientName.Text = null;
                    txtDob.Text = null;
                    dtDob.EditValue = null;
                    txtGender.Text = null;
                    cboGender.EditValue = null;
                    txtAddress.Text = null;
                    patient = new HisPatientSDO();
                    gridControlPatient.DataSource = null;
                    MOS.Filter.HisPatientAdvanceFilter patientFilter = new HisPatientAdvanceFilter();
                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        var codeFind = string.Format("{0:0000000000}", Convert.ToInt64(txtPatientCode.Text));
                        patientFilter.PATIENT_CODE__EXACT = codeFind;
                        var listPatient = new BackendAdapter(param)
                    .Get<List<HisPatientSDO>>("api/HisPatient/GetSdoAdvance", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                        if (listPatient != null && listPatient.Count > 0)
                        {
                            if (listPatient.Count == 1)
                            {
                                patient = listPatient.First();
                            }
                            else
                            {
                                gridControlPatient.DataSource = listPatient;
                            }
                        }
                    }
                    FillDataPatientToControl(patient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public static void LoadGenderCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cbogd, DevExpress.XtraEditors.TextEdit txtgd, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbogd.EditValue = null;
                    cbogd.Focus();
                    cbogd.ShowPopup();
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().Where(o => o.GENDER_CODE.Equals(searchCode)).ToList();
                    List<HIS_GENDER> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.GENDER_CODE == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cbogd.EditValue = result[0].ID;
                        txtgd.Text = result[0].GENDER_CODE;
                        focusControl.Focus();
                        focusControl.SelectAll();
                    }
                    else
                    {
                        cbogd.EditValue = null;
                        cbogd.Focus();
                        cbogd.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtTreatmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientCode.Text = null;
                    txtPatientName.Text = null;
                    txtHisCard.Text = null;
                    txtBHYTCard.Text = null;
                    txtPatientName.Text = null;
                    txtDob.Text = null;
                    dtDob.EditValue = null;
                    txtGender.Text = null;
                    cboGender.EditValue = null;
                    txtAddress.Text = null;
                    patient = new HisPatientSDO();
                    gridControlPatient.DataSource = null;
                    MOS.Filter.HisPatientAdvanceFilter patientFilter = new HisPatientAdvanceFilter();
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        var codeFind = string.Format("{0:000000000000}", Convert.ToInt64(txtTreatmentCode.Text));
                        patientFilter.TREATMENT_CODE__EXACT = codeFind;
                        var PatientCode = new BackendAdapter(param)
                    .Get<List<HisPatientSDO>>("api/HisPatient/GetSdoAdvance", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                        if (PatientCode != null && PatientCode.Count > 0)
                        {
                            if (PatientCode.Count == 1)
                            {
                                patient = PatientCode.First();
                                txtTreatmentCode.Text = codeFind;
                            }
                            else
                            {
                                gridControlPatient.DataSource = PatientCode;
                            }
                        }
                        else
                        {
                            txtTreatmentCode.Text = codeFind;
                            DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy bệnh nhân tương ứng với mã điều trị này", "Thông báo");
                            return;
                        }
                    }
                    FillDataPatientToControl(patient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHisCard_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientCode.Text = null;
                    txtPatientName.Text = null;
                    txtTreatmentCode.Text = null;
                    txtBHYTCard.Text = null;
                    txtPatientName.Text = null;
                    txtDob.Text = null;
                    dtDob.EditValue = null;
                    txtGender.Text = null;
                    cboGender.EditValue = null;
                    txtAddress.Text = null;
                    patient = new HisPatientSDO();
                    gridControlPatient.DataSource = null;
                    MOS.Filter.HisPatientAdvanceFilter patientFilter = new HisPatientAdvanceFilter();
                    if (!String.IsNullOrEmpty(txtHisCard.Text))
                    {
                        patientFilter.CARD_CODE__EXACT = txtHisCard.Text;
                        var PatientCode = new BackendAdapter(param)
                    .Get<List<HisPatientSDO>>("api/HisPatient/GetSdoAdvance", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                        if (PatientCode != null && PatientCode.Count > 0)
                        {
                            if (PatientCode.Count == 1)
                            {
                                patient = PatientCode.First();
                            }
                            else
                            {
                                gridControlPatient.DataSource = PatientCode;
                            }
                        }
                    }
                    FillDataPatientToControl(patient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBHYTCard_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientCode.Text = null;
                    txtPatientName.Text = null;
                    txtTreatmentCode.Text = null;
                    txtHisCard.Text = null;
                    txtPatientName.Text = null;
                    txtDob.Text = null;
                    dtDob.EditValue = null;
                    txtGender.Text = null;
                    cboGender.EditValue = null;
                    txtAddress.Text = null;
                    patient = new HisPatientSDO();
                    gridControlPatient.DataSource = null;
                    MOS.Filter.HisPatientAdvanceFilter patientFilter = new HisPatientAdvanceFilter();
                    if (!String.IsNullOrEmpty(txtBHYTCard.Text))
                    {
                        patientFilter.HEIN_CARD_NUMBER__EXACT = txtBHYTCard.Text;
                        var PatientCode = new BackendAdapter(param)
                    .Get<List<HisPatientSDO>>("api/HisPatient/GetSdoAdvance", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                        if (PatientCode != null && PatientCode.Count > 0)
                        {
                            if (PatientCode.Count == 1)
                            {
                                patient = PatientCode.First();
                            }
                            else
                            {
                                gridControlPatient.DataSource = PatientCode;
                            }
                        }
                    }
                    FillDataPatientToControl(patient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    patient = new HisPatientSDO();
                    gridControlPatient.DataSource = null;
                    MOS.Filter.HisPatientAdvanceFilter patientFilter = new HisPatientAdvanceFilter();
                    //if ((!String.IsNullOrEmpty(txtPatientName.Text)) && (!String.IsNullOrEmpty(dtDob.EditValue.ToString())) && (!String.IsNullOrEmpty(cboGender.EditValue.ToString())))
                    if ((!String.IsNullOrEmpty(txtPatientName.Text)) && (!String.IsNullOrEmpty(txtDob.Text)) && (cboGender.EditValue != null))
                    {
                        txtPatientCode.Text = null;
                        txtTreatmentCode.Text = null;
                        txtHisCard.Text = null;
                        txtBHYTCard.Text = null;
                        //txtPatientName.Text = null;
                        txtAddress.Text = null;
                        patientFilter.VIR_PATIENT_NAME__EXACT = txtPatientName.Text;
                        patientFilter.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDob.DateTime);
                        patientFilter.GENDER_ID = (long)cboGender.EditValue;
                        patientFilter.VIR_PATIENT_NAME__EXACT = txtPatientName.Text;
                        var PatientCode = new BackendAdapter(param)
                    .Get<List<HisPatientSDO>>("api/HisPatient/GetSdoAdvance", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                        if (PatientCode != null && PatientCode.Count > 0)
                        {
                            if (PatientCode.Count == 1)
                            {
                                patient = PatientCode.First();
                                FillDataPatientToControl(patient);
                            }
                            else if (PatientCode.Count > 1)
                            {
                                gridControlPatient.DataSource = PatientCode;
                                gridViewPatient.GetFocusedDataRow();
                            }
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy thông tin bệnh nhân tương ứng", "Thông báo");
                            return;
                        }
                    }
                    else
                    {
                        txtDob.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDob_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGender.Focus();
                    //MOS.Filter.HisPatientAdvanceFilter patientFilter = new HisPatientAdvanceFilter();
                    //gridControlPatient.DataSource = null;
                    //if ((!String.IsNullOrEmpty(txtPatientCode.Text)) && (!String.IsNullOrEmpty(dtDob.EditValue.ToString())) && (!String.IsNullOrEmpty(txtGender.Text)))
                    //{
                    //    patientFilter.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDob.DateTime);
                    //    var PatientCode = new BackendAdapter(param)
                    //.Get<List<HIS_PATIENT>>("api/HisPatient/GetAdvance", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                    //}
                    //else
                    //{
                    //    txtGender.Focus();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGender_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtGender.Text))
                    {
                        cboGender.EditValue = null;
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                    else
                    {
                        List<HIS_GENDER> searchs = null;
                        var listData1 = BackendDataWorker.Get<HIS_GENDER>().Where(o => o.GENDER_CODE.ToUpper().Contains(txtGender.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.GENDER_CODE.ToUpper() == txtGender.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtGender.Text = searchs[0].GENDER_CODE;
                            cboGender.EditValue = searchs[0].ID;
                            txtPatientName_KeyDown(null, new KeyEventArgs(Keys.Enter));
                        }
                        else
                        {
                            cboGender.Focus();
                            cboGender.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadComboGender()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_GENDER>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboGender, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
            //        LoadGenderCombo(strValue, false, cboGender, txtGender, txtPatientName);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboGender.EditValue != null)
                        txtPatientName.Focus();
                    else
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDob_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                dtDob.Visible = false;
                dtDob.Update();
                txtDob.Text = dtDob.DateTime.ToString("dd/MM/yyyy");

                txtDob.Focus();
                txtDob.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDob_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = ConvertDateStringToSystemDate(txtDob.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtDob.EditValue = dt;
                        dtDob.Update();
                    }

                    dtDob.Visible = true;
                    dtDob.ShowPopup();
                    dtDob.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal static DateTime? ConvertDateStringToSystemDate(string date)
        {
            DateTime? result = DateTime.MinValue;
            try
            {
                if (!String.IsNullOrEmpty(date))
                {
                    date = date.Replace(" ", "");
                    int day = Int16.Parse(date.Substring(0, 2));
                    int month = Int16.Parse(date.Substring(3, 2));
                    int year = Int16.Parse(date.Substring(6, 4));
                    result = new DateTime(year, month, day);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                txtPatientCode.Text = null;
                txtPatientName.Text = null;
                txtTreatmentCode.Text = null;
                txtHisCard.Text = null;
                txtBHYTCard.Text = null;
                txtPatientName.Text = null;
                txtDob.Text = null;
                dtDob.EditValue = null;
                txtGender.Text = null;
                cboGender.EditValue = null;
                txtAddress.Text = null;
                gridControlPatient.DataSource = null;
                txtPatientCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatient_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        try
                        {
                            string dob = (view.GetRowCellValue(e.ListSourceRowIndex, "DOB") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(dob));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "GENDER_STR")
                    {
                        try
                        {
                            string gender = (view.GetRowCellValue(e.ListSourceRowIndex, "GENDER_ID") ?? "").ToString();
                            if (gender == "1")
                                e.Value = "Nữ";
                            else if (gender == "2")
                                e.Value = "Nam";
                            else
                                e.Value = "Không xác định";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    gridViewPatient.RefreshData();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatient_Click(object sender, EventArgs e)
        {

        }

        private void gridControlPatient_Enter(object sender, EventArgs e)
        {
            try
            {
                HisPatientSDO patient = null;
                //MOS.Filter.HisPatientAdvanceFilter patientFilter = new HisPatientAdvanceFilter();
                //patientFilter.VIR_PATIENT_NAME = txtPatientName.Text;
                patient = gridViewPatient.GetFocusedRow() as HisPatientSDO;
                if (patient != null)
                    FillDataPatientToControl(patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlPatient_Click(object sender, EventArgs e)
        {
            try
            {
                HisPatientSDO patient = gridViewPatient.GetFocusedRow() as HisPatientSDO;
                if (patient != null)
                    FillDataPatientToControl(patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void getCard(string serviceCode)
        {
            try
            {
                WaitingManager.Show();
                hisCard = new V_HIS_CARD();
                CommonParam param = new CommonParam();
                hisCard = new BackendAdapter(param).Get<V_HIS_CARD>("api/HisCard/GetViewByCode", ApiConsumer.ApiConsumers.MosConsumer, serviceCode, param);

                WaitingManager.Hide();

                if (hisCard != null)
                {
                    if (patient == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn thông tin bệnh nhân", "Thông báo");
                        txtPatientCode.Focus();
                        return;
                    }
                    if (hisCard.PATIENT_ID == null)
                    {
                        txtHisCard.Text = "";
                        txtHisCard.Text = hisCard.CARD_CODE;
                    }
                    else
                    {
                        if (hisCard.PATIENT_ID != patient.ID)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Thẻ của bệnh nhân " + hisCard.VIR_PATIENT_NAME + "(" + "Mã :" + hisCard.PATIENT_CODE + "). \nBạn có muốn tiếp tục thực hiện không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                txtHisCard.Text = hisCard.CARD_CODE;
                            }
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Trong thẻ đã có thông tin của bệnh nhân " + hisCard.VIR_PATIENT_NAME + "(" + "Mã :" + hisCard.PATIENT_CODE + ")");
                            txtHisCard.Text = "";
                            txtHisCard.Text = hisCard.CARD_CODE;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitWCFReadCard()
        {
            try
            {
                if (CARD.WCF.Service.TapCardService.TapCardServiceManager.OpenHost())
                    CARD.WCF.Service.TapCardService.TapCardServiceManager.SetDelegate(CheckServiceCodeDelegate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool CheckServiceCodeDelegate(string serviceCode)
        {
            bool success = false;
            try
            {
                getCard(serviceCode);
                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();

                if (hisCard != null)
                {
                    if (this.patient != null && this.patient.ID > 0)
                    {
                        HIS_CARD resultHisCard = new HIS_CARD();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_CARD>(resultHisCard, hisCard);
                        resultHisCard.PATIENT_ID = patient.ID;

                        var UpdateHisCard = new BackendAdapter(param)
                            .Post<HIS_CARD>("api/HisCard/CreateOrUpdate", ApiConsumers.MosConsumer, resultHisCard, param);
                        if (UpdateHisCard != null)
                        {
                            success = true;
                        }
                    }
                    else
                    {
                        param.Messages.Add("Không có thông tin bệnh nhân");
                    }
                }
                else
                {
                    param.Messages.Add("Không tìm thấy thông tin thẻ");
                }
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboGender.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_GENDER rh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString()));
                        if (rh != null)
                        {
                            txtGender.Text = rh.GENDER_CODE;
                            cboGender.EditValue = rh.ID;
                            txtPatientName_KeyDown(null, new KeyEventArgs(Keys.Enter));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDob_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DateTime? dt = ConvertDateStringToSystemDate(txtDob.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtDob.EditValue = dt;
                        dtDob.Update();
                        txtGender.Focus();
                    }
                    else
                    {
                        dtDob.Visible = true;
                        dtDob.ShowPopup();
                        dtDob.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void emptySpaceItem1_Click(object sender, EventArgs e)
        {
            getCard("8695438d168354ea3034ea36106b9cd64a9a223e");
        }

        private void frmPatientUpdateCard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                CARD.WCF.Service.TapCardService.TapCardServiceManager.CloseHost();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
