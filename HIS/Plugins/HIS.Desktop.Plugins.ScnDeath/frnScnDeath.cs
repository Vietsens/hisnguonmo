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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ScnDeath;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using SCN.EFMODEL.DataModels;
using SCN.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ScnDeath
{
    public partial class frmScnDeath : HIS.Desktop.Utility.FormBase
    {
        string _PersonCode = "";
        Inventec.Desktop.Common.Modules.Module currentModule;

        List<SCN_DEATH> _ScnDeaths { get; set; }
        SCN_DEATH _DeathCurrent { get; set; }
        int action = 0;

        int dem = 0;

        int cout = 0;

        int positionHandleControl = -1;

        public frmScnDeath()
        {
            InitializeComponent();
        }

        public frmScnDeath(Inventec.Desktop.Common.Modules.Module currentModule, string _personCode)
            : base(currentModule)
        {
            InitializeComponent();
            this.SetIcon();
            this.currentModule = currentModule;
            this._PersonCode = _personCode;
            if (this.currentModule != null)
            {
                this.Text = currentModule.text;
            }
        }

        private void SetIcon()
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

        private void frmScnDeath_Load(object sender, EventArgs e)
        {
            try
            {
                this.action = 1;
                this.dtTime.DateTime = DateTime.Now;
                this.LoadDataToCombo();
                this.LoadDataDeath();
                this.ValidationControlMaxLength(this.txtDeathCause, 500);
                this.ValidationControlMaxLength(this.txtDescription, 500);
                this.ValidationControlMaxLength(this.txtCollector, 100);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDeath()
        {
            try
            {
                this._ScnDeaths = new List<SCN_DEATH>();

                SCN.Filter.ScnDeathFilter filter = new SCN.Filter.ScnDeathFilter();
                filter.PERSON_CODE__EXACT = this._PersonCode;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                CommonParam param = new CommonParam();
                this._ScnDeaths = ApiConsumers.ScnWrapConsumer.Get<List<SCN_DEATH>>(true, "api/ScnDeath/Get", param, filter);
                if (this._ScnDeaths != null && this._ScnDeaths.Count > 0)
                {
                    this.SetDataToForm(this._ScnDeaths[0]);
                    this.cout = this._ScnDeaths.Count;
                    if (cout > 1)
                    {
                        btnBack.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToForm(SCN_DEATH data)
        {
            try
            {
                if (data != null)
                {
                    this._DeathCurrent = data;
                    this.action = 2;
                    this.dtTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.DEATH_TIME ?? 0) ?? DateTime.Now;
                    if (data.DEATH_LOCATION_TYPE_ID.HasValue)
                        this.cboDeathLocationType.EditValue = data.DEATH_LOCATION_TYPE_ID;
                    else
                        this.cboDeathLocationType.EditValue = null;
                    if (data.IS_CBYT_CARE == 1)
                        chkCBYTCare.Checked = true;
                    else
                        chkCBYTCare.Checked = false;
                    this.txtDeathCause.Text = data.DEATH_CAUSE;
                    this.txtDescription.Text = data.DESCRIPTION;
                    this.txtCollector.Text = data.COLLECTOR;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                List<HIS.Desktop.Plugins.ScnDeath.ComboADO> status = new List<HIS.Desktop.Plugins.ScnDeath.ComboADO>();
                status.Add(new HIS.Desktop.Plugins.ScnDeath.ComboADO(1, ResourceMessage.TaiCoSoYTe));
                status.Add(new HIS.Desktop.Plugins.ScnDeath.ComboADO(2, ResourceMessage.TaiNha));
                status.Add(new HIS.Desktop.Plugins.ScnDeath.ComboADO(3, ResourceMessage.Khac));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", ResourceMessage.TrangThai, 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(this.cboDeathLocationType, status, controlEditorADO);

                this.cboDeathLocationType.EditValue = status[0].id;
                this.cboDeathLocationType.Properties.Buttons[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                if (!this.dxValidationProvider1.Validate())
                    return;

                HID.EFMODEL.DataModels.HID_PERSON _HidPerson = new HID.EFMODEL.DataModels.HID_PERSON();
                CommonParam param = new CommonParam();
                _HidPerson = new Inventec.Common.Adapter.BackendAdapter(param).Get<HID.EFMODEL.DataModels.HID_PERSON>(
                    "/api/HidPerson/GetByPersonCode",
                HIS.Desktop.ApiConsumer.ApiConsumers.HidConsumer,
                this._PersonCode,
                param);

                SCN_DEATH _death = new SCN_DEATH();

                if (_HidPerson != null)
                {
                    HID.Filter.HidGenderFilter filterGender = new HID.Filter.HidGenderFilter();
                    filterGender.ID = _HidPerson.GENDER_ID;
                    var _HidGender = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HID.EFMODEL.DataModels.HID_GENDER>>(
                           "/api/HidGender/Get",
                       HIS.Desktop.ApiConsumer.ApiConsumers.HidConsumer,
                       filterGender,
                       param);
                    _death.GENDER_NAME = (_HidGender != null && _HidGender.Count > 0) ? _HidGender[0].GENDER_NAME : "";
                    _death.FIRST_NAME = _HidPerson.FIRST_NAME;
                    _death.LAST_NAME = _HidPerson.LAST_NAME;
                    _death.DOB = _HidPerson.DOB;
                    _death.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                    _death.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                    _death.CAREER_NAME = _HidPerson.CAREER_NAME;
                    _death.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                }

                _death.PERSON_CODE = this._PersonCode;
                _death.DEATH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime) ?? 0;
                if (this.cboDeathLocationType.EditValue != null)
                    _death.DEATH_LOCATION_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboDeathLocationType.EditValue.ToString());
                if (chkCBYTCare.Checked)
                    _death.IS_CBYT_CARE = 1;
                _death.DEATH_CAUSE = this.txtDeathCause.Text;
                _death.DESCRIPTION = this.txtDescription.Text;
                _death.COLLECTOR = this.txtCollector.Text;

                // CommonParam param = new CommonParam();
                bool succes = false;
                SCN_DEATH result = null;

                if (this.action == 1)
                {
                    result = ApiConsumers.ScnWrapConsumer.Post<SCN_DEATH>(true, "api/ScnDeath/Create", param, _death);
                }
                else if (this.action == 2 && this._DeathCurrent != null)
                {
                    _death.ID = this._DeathCurrent.ID;
                    result = ApiConsumers.ScnWrapConsumer.Post<SCN_DEATH>(true, "api/ScnDeath/Update", param, _death);
                }
                if (result != null)
                {
                    this.action = 2;
                    succes = true;
                    this._DeathCurrent = new SCN_DEATH();
                    this.LoadDataDeath();
                }
                MessageManager.Show(this.ParentForm, param, succes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {

            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();

            validate.editor = control;

            validate.maxLength = maxLength;

            validate.ErrorText = ResourceMessage.TruongDuLieuVuotQuaKyTu;

            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;

            this.dxValidationProvider1.SetValidationRule(control, validate);

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnBack.Enabled)
                    return;
                dxValidationProvider1.RemoveControlError(txtCollector);
                dxValidationProvider1.RemoveControlError(txtDeathCause);
                dxValidationProvider1.RemoveControlError(txtDescription);

                this.dem++;

                SetDataToForm(this._ScnDeaths[this.dem]);
                btnNext.Enabled = true;
                if (this.dem == (cout - 1))
                {
                    btnBack.Enabled = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNext.Enabled)
                    return;
                dxValidationProvider1.RemoveControlError(txtCollector);
                dxValidationProvider1.RemoveControlError(txtDeathCause);
                dxValidationProvider1.RemoveControlError(txtDescription);
                this.dem--;
                SetDataToForm(this._ScnDeaths[this.dem]);
                btnBack.Enabled = true;
                if (this.dem == 0)
                {
                    btnNext.Enabled = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtTime.EditValue != null)
                    {
                        cboDeathLocationType.Focus();
                        cboDeathLocationType.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                cboDeathLocationType.Focus();
                cboDeathLocationType.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDeathLocationType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDeathLocationType.EditValue != null)
                    {
                        cboDeathLocationType.Properties.Buttons[1].Visible = true;
                        chkCBYTCare.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDeathLocationType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    if (cboDeathLocationType.EditValue == null || cboDeathLocationType.EditValue == "")
                    {
                        cboDeathLocationType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCBYTCare_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    txtDeathCause.Focus();
                    txtDeathCause.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDeathCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Tab)
                //{
                //    txtDescription.Focus();
                //    txtDescription.SelectAll();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Tab)
                //{
                //    txtCollector.Focus();
                //    txtCollector.SelectAll();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCollector_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDeathLocationType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDeathLocationType.Properties.Buttons[1].Visible = false;
                    cboDeathLocationType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonI__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
