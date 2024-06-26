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
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraEditors.ViewInfo;

namespace HIS.Desktop.Plugins.ContactDeclaration.UcObject
{
    public partial class UcPatient : UserControl
    {
        public UpdateVContactPoint updateVContactPoint;
        //List<V_HIS_PATIENT_1> lstPatient = new List<V_HIS_PATIENT_1>();
        V_HIS_PATIENT_1 Patient = new V_HIS_PATIENT_1();
        //Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_CONTACT_POINT CurrentContactPoint = new V_HIS_CONTACT_POINT();
        long? PatientsID;
        int positionHandle = -1;

        public UcPatient() { }

        public UcPatient(long? _PatientsID, UpdateVContactPoint _updateVContactPoint)
        {
            //this.lstPatient = Patients;
            this.PatientsID = _PatientsID;
            this.updateVContactPoint = _updateVContactPoint;
            InitializeComponent();
        }

        private void UcPatient_Load(object sender, EventArgs e)
        {
            //LoadComboboxPatient();
            loadPading(this.PatientsID);
            //ValidateForm();
            ValidateUcPatient();
        }

        public void ValidateUcPatient() 
        {
            try
            {
                ValidationSingleControl(dxValidationProvider1, txtPatientName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        public void loadPading(long? patientId)
        {
            try
            {
                if (patientId != null)
                {
                    HisPatientView1Filter filter = new HisPatientView1Filter();
                    filter.ID = patientId;
                    var Patients = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_PATIENT_1>>("api/HisPatient/GetView1", ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                    if (Patients != null && Patients.Count > 0)
                    {
                        Patient = Patients.FirstOrDefault();
                        txtPatientCode.Text = Patients.FirstOrDefault().PATIENT_CODE;
                        txtPatientName.Text = Patients.FirstOrDefault().VIR_PATIENT_NAME;
                        lblDepartmentPatient.Text = Patients.FirstOrDefault().DEPARTMENT_NAME;
                        //txtPatientName.EditValue = Patients.FirstOrDefault().ID;
                    }
                }
                else
                {
                    Patient = new V_HIS_PATIENT_1();
                    txtPatientCode.Text = "";
                    txtPatientName.Text = "";
                    lblDepartmentPatient.Text = "";
                    //txtPatientName.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool ValidateForm()
        {
            positionHandle = -1;
            return dxValidationProvider1.Validate();
        }

        private void ValidationSingleControl(DXValidationProvider validate, BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                validRule.ErrorType = ErrorType.Warning;
                validate.SetValidationRule(txtPatientCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void LoadComboboxPatient()
        //{
        //    try
        //    {
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("PATIENT_CODE", "", 100, 1));
        //        columnInfos.Add(new ColumnInfo("VIR_PATIENT_NAME", "", 250, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("VIR_PATIENT_NAME", "ID", columnInfos, false, 350);
        //        ControlEditorLoader.Load(cboPatient, lstPatient, controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void txtPatient_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Patient = new V_HIS_PATIENT_1();
                    //if (!String.IsNullOrEmpty(txtPatient.Text) && lstPatient != null && lstPatient.Count > 0)
                    //{
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }

                    HisPatientView1Filter filter = new HisPatientView1Filter();
                    filter.PATIENT_CODE__EXACT = code;
                    var Patients = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_PATIENT_1>>("api/HisPatient/GetView1", ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                    if (Patients != null && Patients.Count > 0)
                    {
                        Patient = Patients.FirstOrDefault();
                    }

                    //var Search = lstPatient.FirstOrDefault(o => o.PATIENT_CODE == code);
                    if (Patient != null)
                    {
                        txtPatientName.Text = Patient.VIR_PATIENT_NAME;
                        //txtPatientName.EditValue = Patient.ID;
                        lblDepartmentPatient.Text = Patient.DEPARTMENT_NAME;

                        CommonParam paramCommon = new CommonParam();
                        HisContactPointFilter filterContactPoint = new HisContactPointFilter();

                        filterContactPoint.PATIENT_ID = Patient.ID;

                        var ContactPoint = new BackendAdapter(paramCommon).Get<List<HIS_CONTACT_POINT>>("/api/HisContactPoint/Get", ApiConsumers.MosConsumer, filterContactPoint, paramCommon);

                        //CurrentContactPoint = ContactPoint.FirstOrDefault();
                        SetCurrentContactPoint(ref CurrentContactPoint, ContactPoint.FirstOrDefault());

                        this.updateVContactPoint(CurrentContactPoint);
                    }
                    else
                    {
                        Patient = new V_HIS_PATIENT_1();
                        //txtPatientName.EditValue = null;
                        lblDepartmentPatient.Text = "";
                        txtPatientCode.Focus();
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatient_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (cboPatient.EditValue != null)
            //    {
            //        var Search = lstPatient.FirstOrDefault(o => o.ID == (long)cboPatient.EditValue);
            //        if (Search != null)
            //        {
            //            WaitingManager.Show();
            //            Patient = Search;
            //            txtPatient.Text = Search.PATIENT_CODE;
            //            //var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Search.LAST_DEPARTMENT_ID);
            //            lblDepartmentPatient.Text = Search.DEPARTMENT_NAME;
            //            cboPatient.Properties.Buttons[1].Visible = true;

            //            CommonParam paramCommon = new CommonParam();
            //            HisContactPointFilter filter = new HisContactPointFilter();

            //            filter.PATIENT_ID = Search.ID;

            //            var ContactPoint = new BackendAdapter(paramCommon).Get<List<HIS_CONTACT_POINT>>("/api/HisContactPoint/Get", ApiConsumers.MosConsumer, filter, paramCommon);

            //            //CurrentContactPoint = ContactPoint.FirstOrDefault();
            //            SetCurrentContactPoint(ref CurrentContactPoint, ContactPoint.FirstOrDefault());

            //            this.updateVContactPoint(CurrentContactPoint);

            //            WaitingManager.Hide();
            //        }
            //        else
            //        {
            //            Patient = new V_HIS_PATIENT_1();
            //            txtPatient.Text = "";
            //            lblDepartmentPatient.Text = "";
            //            cboPatient.Properties.Buttons[1].Visible = false;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    WaitingManager.Hide();
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void SetCurrentContactPoint(ref V_HIS_CONTACT_POINT VContactPoint, HIS_CONTACT_POINT ConTactPoint)
        {
            try
            {
                if (ConTactPoint != null)
                {
                    VContactPoint = new V_HIS_CONTACT_POINT();
                    VContactPoint.CONTACT_LEVEL = ConTactPoint.CONTACT_LEVEL;
                    VContactPoint.CONTACT_POINT_OTHER_TYPE_NAME = ConTactPoint.CONTACT_POINT_OTHER_TYPE_NAME;
                    VContactPoint.CONTACT_TYPE = ConTactPoint.CONTACT_TYPE;
                    VContactPoint.CREATE_TIME = ConTactPoint.CREATE_TIME;
                    VContactPoint.CREATOR = ConTactPoint.CREATOR;
                    VContactPoint.DOB = ConTactPoint.DOB;
                    VContactPoint.EMPLOYEE_ID = ConTactPoint.EMPLOYEE_ID;
                    VContactPoint.FIRST_NAME = ConTactPoint.FIRST_NAME;
                    VContactPoint.FULL_NAME = ConTactPoint.VIR_FULL_NAME;
                    VContactPoint.GENDER_ID = ConTactPoint.GENDER_ID;
                    VContactPoint.ID = ConTactPoint.ID;
                    VContactPoint.LAST_NAME = ConTactPoint.LAST_NAME;
                    VContactPoint.MODIFIER = ConTactPoint.MODIFIER;
                    VContactPoint.MODIFY_TIME = ConTactPoint.MODIFY_TIME;
                    VContactPoint.NOTE = ConTactPoint.NOTE;
                    VContactPoint.PATIENT_ID = ConTactPoint.PATIENT_ID;
                    VContactPoint.PHONE = ConTactPoint.PHONE;
                    VContactPoint.TEST_RESULT_1 = ConTactPoint.TEST_RESULT_1;
                    VContactPoint.TEST_RESULT_2 = ConTactPoint.TEST_RESULT_2;
                    VContactPoint.TEST_RESULT_3 = ConTactPoint.TEST_RESULT_3;
                    VContactPoint.VIR_ADDRESS = ConTactPoint.VIR_ADDRESS;
                }
                else
                {
                    VContactPoint = new V_HIS_CONTACT_POINT();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboPatient_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //try
            //{
            //    if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
            //    {
            //        txtPatient.Text = "";
            //        cboPatient.EditValue = null;
            //        lblDepartmentPatient.Text = "";
            //        cboPatient.Properties.Buttons[1].Visible = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        public void FocusTxtPatient()
        {
            try
            {
                txtPatientCode.Focus();
                txtPatientCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public V_HIS_PATIENT_1 GetValueHisPatient()
        {
            try
            {
                if (this.Patient != null && this.Patient.ID > 0)
                {
                    return this.Patient;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new V_HIS_PATIENT_1();
            }
        }

        public V_HIS_CONTACT_POINT GetCurrentContactPoint()
        {
            try
            {
                return CurrentContactPoint;
            }
            catch (Exception ex)
            {
                return new V_HIS_CONTACT_POINT();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetValueeCboPatient(long? PatientId)
        {
            try
            {
                loadPading(PatientId);
                //cboPatient.EditValue = PatientId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void reset() 
        {
            try
            {
                this.txtPatientCode.Text = "";
                this.txtPatientName.Text = "";
                this.lblDepartmentPatient.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }
    }
}
