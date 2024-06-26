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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.Core.RelaytionType;
using HIS.UC.SurgeryAppointment.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.UC.SurgeryAppointment
{
    public partial class UCSurgeryAppointment : UserControl
    {
        ControlStateWorker controlStateWorker;
        List<ControlStateRDO> currentControlStateRDO;
        private string UC_LINK = "HIS.UC.SurgeryAppointment";
        private string CHECK_PRINT_APPOINT_SURG = "chkPrintAppointSurg";
        private bool isInit = true;
        private string SocialNumber="";

        private MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment;
        private DelegateNextFocus NextFocus;
        private int positionHandle = -1;
        public bool isInvisiblePrint { get; set; }

        public UCSurgeryAppointment()
        {
            InitializeComponent();
        }

        public UCSurgeryAppointment(ADO.SurgeryAppointmentInitADO data)
            : this()
        {
            try
            {
                if (data != null)
                {
                    this.hisTreatment = data.CurrentHisTreatment;

                    this.isInvisiblePrint = data.InvisibleCheckPrint;

                    FillDataToForm(this.hisTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCSurgeryAppointment_Load(object sender, EventArgs e)
        {
            try
            {
                LoadKeysFromlanguage();
                if (this.isInvisiblePrint)
                {
                    lciPrintAppointSurg.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciPrintAppointSurg.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                this.InitControlState();
                ValidateForm();
                this.isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.UC_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == this.CHECK_PRINT_APPOINT_SURG)
                        {
                            chkPrintAppointSurg.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetStringFromKey(string key)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(key))
                {
                    result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceMessage.LanguageUCSurgeryAppointment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void ValidateForm()
        {
            try
            {
                ValidationMaxLength(txtDichVuMo, 500);
                ValidationMaxLength(txtGhiChu, 500);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationMaxLength(Control control, int? maxLength)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule valid = new ControlMaxLengthValidationRule();
                valid.editor = control;
                valid.maxLength = maxLength;
                valid.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToForm(HIS_TREATMENT data)
        {
            try
            {
                if (data != null)
                {
                    if (data.SURGERY_APPOINTMENT_TIME.HasValue)
                    {
                        dtSurgeryTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.SURGERY_APPOINTMENT_TIME.Value) ?? new DateTime();
                    }
                    else
                    {
                        dtSurgeryTime.EditValue = null;
                    }
                    txtDichVuMo.Text = data.APPOINTMENT_SURGERY;
                    txtGhiChu.Text = data.ADVISE;
                    
                    MOS.Filter.HisPatientFilter patientFilter = new MOS.Filter.HisPatientFilter();
                    patientFilter.ID = data.PATIENT_ID;
                    var patient = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                    if (patient != null && patient.Count > 0)
                    {
                        SocialNumber = patient.FirstOrDefault().SOCIAL_INSURANCE_NUMBER;
                    }
                }
                else
                {
                    dtSurgeryTime.EditValue = null;
                    txtDichVuMo.Text = "";
                    txtGhiChu.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FocusControl()
        {
            try
            {
                dtSurgeryTime.Focus();
                dtSurgeryTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void Reload(MOS.EFMODEL.DataModels.HIS_TREATMENT input)
        {
            try
            {
                FillDataToForm(input);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillCurrentData(ADO.SurgeryAppointmentInitADO input)
        {
            try
            {
                if (input != null)
                {
                    this.NextFocus = input.DelegateNextFocus;

                    if (input.CurrentHisTreatment != null)
                    {
                        this.hisTreatment = input.CurrentHisTreatment;
                        FillDataToForm(this.hisTreatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ReadOnly(bool isReadOnly)
        {
            try
            {
                dtSurgeryTime.ReadOnly = isReadOnly;
                txtDichVuMo.ReadOnly = isReadOnly;
                txtGhiChu.ReadOnly = isReadOnly;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValue(object data)
        {
            try
            {
                if (data is ADO.SurgeryAppointmentInitADO)
                {

                }
                else
                {
                    FillDataToForm(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal object GetValue()
        {
            object result = null;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate()) return null;

                MOS.SDO.HisTreatmentFinishSDO outPut = new MOS.SDO.HisTreatmentFinishSDO();

                outPut.TreatmentId = hisTreatment.ID;

                if (dtSurgeryTime.EditValue != null && dtSurgeryTime.DateTime != DateTime.MinValue)
                {
                    outPut.SurgeryAppointmentTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtSurgeryTime.DateTime) ?? 0;
                }
                else
                    outPut.SurgeryAppointmentTime = null;

                outPut.AppointmentSurgery = txtDichVuMo.Text;
                outPut.Advise = txtGhiChu.Text;
                outPut.SocialInsuranceNumber = SocialNumber;


                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

        internal object GetValuePlus()
        {
            object result = null;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate()) return null;

                SurgAppointmentADO outPut = new SurgAppointmentADO();

                outPut.TreatmentId = hisTreatment.ID;

                if (dtSurgeryTime.EditValue != null && dtSurgeryTime.DateTime != DateTime.MinValue)
                {
                    outPut.SurgeryAppointmentTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtSurgeryTime.DateTime) ?? 0;
                }
                else
                    outPut.SurgeryAppointmentTime = null;

                outPut.AppointmentSurgery = txtDichVuMo.Text;
                outPut.Advise = txtGhiChu.Text;
                outPut.IsPrintSurgAppoint = chkPrintAppointSurg.Checked;
                LogSystem.Debug("SurgAppointmentADO: \n" + LogUtil.TraceData("outPut", outPut));
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

        private void dtSurgeryTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtDichVuMo.Focus();
                    txtDichVuMo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtSurgeryTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (dtSurgeryTime != null)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        txtDichVuMo.Focus();
                        txtDichVuMo.SelectAll();
                    }
                }
                else
                {
                    dtSurgeryTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintAppointSurg_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                {
                    return;
                }
                ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == this.CHECK_PRINT_APPOINT_SURG && o.MODULE_LINK == this.UC_LINK).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintAppointSurg.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = this.CHECK_PRINT_APPOINT_SURG;
                    csAddOrUpdate.VALUE = (chkPrintAppointSurg.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = this.UC_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
