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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PublicMedicineMultiPatient.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient
{
    public partial class frmPublicMedicineMultiPatient : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        L_HIS_TREATMENT_BED_ROOM focusedTreatmentBedRoom;
        HisTreatmentBedRoomLViewFilter treatmentBedRoomFilter;
        List<HisPatientTypeADO> patientTypeADOs;
        List<HisPatientTypeADO> patientTypeSelecteds;

        #region ControlState
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isNotLoadWhileChangeControlStateInFirst;
        #endregion

        public frmPublicMedicineMultiPatient()
        {
            InitializeComponent();
        }

        public frmPublicMedicineMultiPatient(Inventec.Desktop.Common.Modules.Module currentModule, L_HIS_TREATMENT_BED_ROOM focusedTreatmentBedRoom, HisTreatmentBedRoomLViewFilter treatmentBedRoomFilter)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this.focusedTreatmentBedRoom = focusedTreatmentBedRoom;
            this.treatmentBedRoomFilter = treatmentBedRoomFilter;
        }

        private void frmPublicMedicineMultiPatient_Load(object sender, EventArgs e)
        {
            try
            {
                Config.Config.LoadConfig();
                SetIcon();
                SetCaptionByLanguageKey();
                InitComboPatientType();
                LoadDataCboDepartment();
                SetDefaultValue();
                InitControlState();
                this.cboDepartment.EditValueChanged += new System.EventHandler(this.cboDepartment_EditValueChanged);
                LoadPatientList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageValue(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PublicMedicineMultiPatient.Resources.Lang", typeof(HIS.Desktop.Plugins.PublicMedicineMultiPatient.frmPublicMedicineMultiPatient).Assembly);

                this.Text = GetLanguageValue("frmPublicMedicineMultiPatient.Text");
                this.groupControlPatient.Text = GetLanguageValue("frmPublicMedicineMultiPatient.groupControlPatient.Text");
                this.gridViewPatient.OptionsFind.FindNullPrompt = GetLanguageValue("frmPublicMedicineMultiPatient.gridViewPatient.OptionsFind.FindNullPrompt");
                this.gcCheck.ToolTip = GetLanguageValue("frmPublicMedicineMultiPatient.gcCheck.ToolTip");
                this.gcStt.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.gcStt.Caption");
                this.gcBedRoomName.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.gcBedRoomName.Caption");
                this.gcBedName.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.gcBedName.Caption");
                this.gcTreatmentCode.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.gcTreatmentCode.Caption");
                this.gcPatientCode.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.gcPatientCode.Caption");
                this.gcPatientName.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.gcPatientName.Caption");
                this.gcDob.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.gcDob.Caption");
                this.gcGender.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.gcGender.Caption");
                this.groupControlFilterCondition.Text = GetLanguageValue("frmPublicMedicineMultiPatient.groupControlFilterCondition.Text");
                this.lciMedicine.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciMedicine.Text");
                this.lciMaterial.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciMaterial.Text");
                this.lciBlood.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciBlood.Text");
                this.lciChemical.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciChemical.Text");
                this.lciIsExpend.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciIsExpend.Text");
                this.lciDepartment.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciDepartment.Text");
                this.lciRoom.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciRoom.Text");
                this.lciPatientType.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciPatientType.Text");
                this.lciPatientType.OptionsToolTip.ToolTip = GetLanguageValue("frmPublicMedicineMultiPatient.lciPatientType.OptionsToolTip.ToolTip");
                this.lciFromTime.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciFromTime.Text");
                this.lciToTime.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciToTime.Text");
                this.lciSortName.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciSortName.Text");
                this.chkSortName.Properties.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.chkSortName.Properties.Caption");
                this.chkSortName.ToolTip = GetLanguageValue("frmPublicMedicineMultiPatient.chkSortName.ToolTip");
                this.chkSortEstablish.Properties.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.chkSortEstablish.Properties.Caption");
                this.chkSortEstablish.ToolTip = GetLanguageValue("frmPublicMedicineMultiPatient.chkSortEstablish.ToolTip");
                this.lciTachHDSD.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciTachHDSD.Text");
                this.lciTachHDSD.OptionsToolTip.ToolTip = GetLanguageValue("frmPublicMedicineMultiPatient.lciTachHDSD.OptionsToolTip.ToolTip");
                this.lciSign.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciSign.Text");
                this.lciPrintDocumentSigned.Text = GetLanguageValue("frmPublicMedicineMultiPatient.lciPrintDocumentSigned.Text");
                this.btnCreate.Text = GetLanguageValue("frmPublicMedicineMultiPatient.btnCreate.Text");
                this.barButtonItem1.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.btnCreate.Text");

                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Defaults: Tạo từ 00:00 - Tạo đến 23:59 of today, Khoa yêu cầu = department of the working room, ĐTTT = all.
        /// Phòng yêu cầu stays empty: the ticked patients may lie in other bed rooms whose prescriptions are requested from those rooms.
        /// </summary>
        private void SetDefaultValue()
        {
            try
            {
                DateTime today = DateTime.Today;
                this.dtFromTime.EditValue = today;
                this.dtToTime.EditValue = today.AddDays(1).AddMinutes(-1);

                var workplace = WorkPlace.WorkPlaceSDO != null && this.currentModule != null
                    ? WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId)
                    : null;
                if (workplace != null)
                {
                    this.cboDepartment.EditValue = workplace.DepartmentId;
                    this.cboDepartment.Properties.Buttons[1].Visible = true;
                    LoadDataCboRoom(workplace.DepartmentId);
                    this.cboRoom.EditValue = null;
                    this.cboRoom.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    LoadDataCboRoom(null);
                    this.cboDepartment.Properties.Buttons[1].Visible = false;
                    this.cboRoom.Properties.Buttons[1].Visible = false;
                }

                this.chkPrintDocumentSigned.Enabled = this.chkSign.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Department, room, patient type
        private void LoadDataCboDepartment()
        {
            try
            {
                List<HIS_DEPARTMENT> departments = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", GetLanguageValue("frmPublicMedicineMultiPatient.cboDepartment.DEPARTMENT_CODE"), 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", GetLanguageValue("frmPublicMedicineMultiPatient.cboDepartment.DEPARTMENT_NAME"), 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboDepartment, departments, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboRoom(long? departmentId)
        {
            try
            {
                List<V_HIS_ROOM> rooms = BackendDataWorker.Get<V_HIS_ROOM>()
                    .Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (!departmentId.HasValue || p.DEPARTMENT_ID == departmentId.Value))
                    .ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", GetLanguageValue("frmPublicMedicineMultiPatient.cboRoom.ROOM_CODE"), 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", GetLanguageValue("frmPublicMedicineMultiPatient.cboRoom.ROOM_NAME"), 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboRoom, rooms, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                long? departmentId = null;
                if (this.cboDepartment.EditValue != null && this.cboDepartment.EditValue.ToString() != "")
                {
                    departmentId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboDepartment.EditValue.ToString());
                }
                LoadDataCboRoom(departmentId);
                this.cboRoom.EditValue = null;
                this.cboRoom.Properties.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal && this.cboDepartment.EditValue != null)
                {
                    this.cboDepartment.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboDepartment.EditValue = null;
                    this.cboDepartment.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal && this.cboRoom.EditValue != null)
                {
                    this.cboRoom.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboRoom.EditValue = null;
                    this.cboRoom.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboPatientType()
        {
            try
            {
                this.patientTypeADOs = BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .Select(o => new HisPatientTypeADO(o.ID, o.PATIENT_TYPE_NAME))
                    .ToList();

                this.checkedCboHisPatientType.Properties.DataSource = this.patientTypeADOs;
                this.checkedCboHisPatientType.Properties.DisplayMember = "NAME";
                this.checkedCboHisPatientType.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn column = this.checkedCboHisPatientType.Properties.View.Columns.AddField("NAME");
                column.VisibleIndex = 1;
                column.Width = 200;
                column.Caption = GetLanguageValue("frmPublicMedicineMultiPatient.checkedCboHisPatientType.All");
                this.checkedCboHisPatientType.Properties.PopupFormWidth = 200;
                this.checkedCboHisPatientType.Properties.View.OptionsView.ShowColumnHeaders = true;
                this.checkedCboHisPatientType.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(this.checkedCboHisPatientType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(this.SelectionGrid__PatientType);
                this.checkedCboHisPatientType.Properties.Tag = gridCheck;
                gridCheck.SelectAll(this.checkedCboHisPatientType.Properties.DataSource);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__PatientType(object sender, EventArgs e)
        {
            try
            {
                this.patientTypeSelecteds = new List<HisPatientTypeADO>();
                foreach (HisPatientTypeADO patientType in (sender as GridCheckMarksSelection).Selection)
                {
                    if (patientType != null)
                        this.patientTypeSelecteds.Add(patientType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkedCboHisPatientType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = this.checkedCboHisPatientType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark == null)
                    return;
                if (this.patientTypeADOs != null && gridCheckMark.Selection.Count == this.patientTypeADOs.Count)
                {
                    e.DisplayText = GetLanguageValue("frmPublicMedicineMultiPatient.checkedCboHisPatientType.All");
                }
                else
                {
                    List<string> names = new List<string>();
                    foreach (HisPatientTypeADO patientType in gridCheckMark.Selection)
                    {
                        names.Add(patientType.NAME);
                    }
                    e.DisplayText = String.Join(", ", names);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkedCboHisPatientType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = this.checkedCboHisPatientType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark == null)
                        return;
                    gridCheckMark.ClearSelection(this.checkedCboHisPatientType.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region ControlState
        private void InitControlState()
        {
            try
            {
                this.isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = this.controlStateWorker.GetData(Config.ControlStateConstan.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == this.chkSign.Name)
                        {
                            this.chkSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == this.chkPrintDocumentSigned.Name)
                        {
                            this.chkPrintDocumentSigned.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == this.chkTachHDSD.Name)
                        {
                            this.chkTachHDSD.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == this.chkSortEstablish.Name)
                        {
                            this.chkSortEstablish.Checked = item.VALUE == "1";
                        }
                    }
                }
                if (!this.chkSign.Checked)
                {
                    this.chkPrintDocumentSigned.Checked = false;
                }
                this.chkPrintDocumentSigned.Enabled = this.chkSign.Checked;
                if (!this.chkSortEstablish.Checked)
                {
                    this.chkSortName.Checked = true;
                }
                this.isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                this.isNotLoadWhileChangeControlStateInFirst = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveControlState(string key, bool isChecked)
        {
            try
            {
                if (this.isNotLoadWhileChangeControlStateInFirst || this.controlStateWorker == null)
                    return;

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = this.currentControlStateRDO != null
                    ? this.currentControlStateRDO.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == Config.ControlStateConstan.ModuleLink)
                    : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (isChecked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = key;
                    csAddOrUpdate.VALUE = (isChecked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = Config.ControlStateConstan.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.chkPrintDocumentSigned.Enabled = this.chkSign.Checked;
                if (this.isNotLoadWhileChangeControlStateInFirst)
                    return;
                if (!this.chkSign.Checked)
                {
                    this.chkPrintDocumentSigned.Checked = false;
                }
                SaveControlState(this.chkSign.Name, this.chkSign.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintDocumentSigned_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isNotLoadWhileChangeControlStateInFirst)
                    return;
                SaveControlState(this.chkPrintDocumentSigned.Name, this.chkPrintDocumentSigned.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTachHDSD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isNotLoadWhileChangeControlStateInFirst)
                    return;
                SaveControlState(this.chkTachHDSD.Name, this.chkTachHDSD.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSortEstablish_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isNotLoadWhileChangeControlStateInFirst)
                    return;
                SaveControlState(this.chkSortEstablish.Name, this.chkSortEstablish.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnCreate.Enabled)
                    btnCreate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                this.treatmentBedRoomADOs = null;
                this.printItems = null;
                this.patientTypeADOs = null;
                this.patientTypeSelecteds = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
