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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.PublicMedicineByPhased.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.PublicMedicineByPhased
{
    public partial class frmPublicMedicineByPhased : HIS.Desktop.Utility.FormBase
    {
        long _treatmentId;
        internal L_HIS_TREATMENT_BED_ROOM _TreatmentBedRoom { get; set; }
        internal List<ExpMestMediAndMateADO> _Datas { get; set; }
        internal List<ExpMestMediAndMateADO> ExpMestMediAndMateADOPrint { get; set; }
        List<HisPatientTypeADO> lstHisPatientTypeADO;
        List<HisPatientTypeADO> lstHisPatientTypeSelecteds;
        internal bool RoomTypeBed;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        Inventec.Desktop.Common.Modules.Module currentModule;
        bool isNotLoadWhileChangeControlStateInFirst;
        int dayCountData = 0;
        public frmPublicMedicineByPhased()
        {
            InitializeComponent();
        }

        public frmPublicMedicineByPhased(Inventec.Desktop.Common.Modules.Module currentModule, L_HIS_TREATMENT_BED_ROOM curentTreatment)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this._TreatmentBedRoom = curentTreatment;
            this._treatmentId = curentTreatment.TREATMENT_ID;
        }

        public frmPublicMedicineByPhased(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_TREATMENT_4 _treatment4)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            if (_treatment4 != null)
            {
                this._TreatmentBedRoom = new L_HIS_TREATMENT_BED_ROOM();
                Inventec.Common.Mapper.DataObjectMapper.Map<L_HIS_TREATMENT_BED_ROOM>(this._TreatmentBedRoom, _treatment4);
            }

            this._treatmentId = _treatment4.ID;
            this.RoomTypeBed = true;
        }

        private void frmPublicMedicineByPhased_Load(object sender, EventArgs e)
        {
            try
            {
                Config.Config.LoadConfig();
                SetCaptionByLanguageKey();

                InitControlState();

                dtFromTime.EditValue = Inventec.Common.DateTime.Get.StartWeekSystemDateTime();
                dtToTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                if (this._TreatmentBedRoom != null)
                {
                    LoadViewLabel();
                }
                
                GetDataCombo();

                InitCombo(checkedCboHisPatientType, lstHisPatientTypeADO, "NAME", "ID");

                InitCheck(checkedCboHisPatientType, SelectionGrid__ServiceReqFunt);

                LoadDataCboDepartment(null);
                SetDefaultValue();
                GetAllData();

                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                checkedCboHisPatientType.Text = "Tất cả";
                this.cboDepartment.EditValue = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetDataCombo()
        {
            try
            {
                lstHisPatientTypeADO = new List<HisPatientTypeADO>();
                var listTemp = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                var lstHisPatientTypeADO1 = from s in listTemp
                                            select new HisPatientTypeADO { ID = s.ID, NAME = s.PATIENT_TYPE_NAME };
                lstHisPatientTypeADO = lstHisPatientTypeADO1.ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
                //this.checkedCboHisPatientType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.checkedCboHisPatientType_CustomDisplayText);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {

            cbo.Properties.DataSource = data;
            cbo.Properties.DisplayMember = DisplayValue;
            cbo.Properties.ValueMember = ValueMember;
            DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

            col2.VisibleIndex = 1;
            col2.Width = 200;
            col2.Caption = "Tất cả";
            cbo.Properties.PopupFormWidth = 200;
            cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
            cbo.Properties.View.OptionsSelection.MultiSelect = true;

            GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
            if (gridCheckMark != null)
            {
                gridCheckMark.SelectAll(cbo.Properties.DataSource);
            }
        }

        private void SelectionGrid__ServiceReqFunt(object sender, EventArgs e)
        {
            try
            {
                lstHisPatientTypeSelecteds = new List<HisPatientTypeADO>();
                foreach (HisPatientTypeADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        lstHisPatientTypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PublicMedicineByPhased.Resources.Lang", typeof(HIS.Desktop.Plugins.PublicMedicineByPhased.frmPublicMedicineByPhased).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lciTreatmentCode.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciTreatmentCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewSereServ.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.gridViewSereServ.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSTT.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.grdColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceCode.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.grdColServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceName.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.grdColServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.grdColServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAmount.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.grdColAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientTypeName.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.grdColPatientTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPrice.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.grdColPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColVirTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.grdColVirTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupControlFilterCondition.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.groupControlFilterCondition.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHoaChat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.chkHoaChat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkBlood.Properties.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.chkBlood.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMaterial.Properties.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.chkMaterial.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMedicine.Properties.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.chkMedicine.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicine.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaterial.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFromTime.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciFromTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciToTime.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciToTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBlood.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciBlood.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreate.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.btnCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDOB.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciDOB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTachHDSD.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciTachHDSD.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTachHDSD.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.lciTachHDSD.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkTachHDSD.ToolTip = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.chkTachHDSD.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPublicMedicineByPhased.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetIconFrm()
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

        private void LoadViewLabel()
        {
            lblTreatmentCode.Text = this._TreatmentBedRoom.TREATMENT_CODE;
            lblPatientCode.Text = this._TreatmentBedRoom.TDL_PATIENT_CODE;
            lblPatientName.Text = this._TreatmentBedRoom.TDL_PATIENT_NAME;
            lblDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._TreatmentBedRoom.TDL_PATIENT_DOB);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                //Lọc các sereServ theo từ, đến

                long timeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFromTime.DateTime) ?? 0;
                long timeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtToTime.DateTime) ?? 0;

                List<ExpMestMediAndMateADO> sereServTemp = new List<ExpMestMediAndMateADO>();
                GetAllData();
                sereServTemp = _Datas;
                //Máu
                List<ExpMestMediAndMateADO> bloods = sereServTemp.Where(o =>
                    o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList();
                if (bloods != null && bloods.Count > 0)
                {
                    sereServTemp = sereServTemp.Except(bloods).ToList();
                }


                //Thuốc
                List<ExpMestMediAndMateADO> medicines = sereServTemp.Where(o =>
                    o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).ToList();
                if (medicines != null && medicines.Count > 0)
                {
                    sereServTemp = sereServTemp.Except(medicines).ToList();
                }

                //Vật tư
                List<ExpMestMediAndMateADO> materials = sereServTemp.Where(o =>
                    o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                    && o.IS_CHEMICAL_SUBSTANCE != 1).ToList();
                if (materials != null && materials.Count > 0)
                {
                    sereServTemp = sereServTemp.Except(materials).ToList();
                }

                //Hóa chất
                List<ExpMestMediAndMateADO> chemicals = sereServTemp.Where(o =>
                    o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                    && o.IS_CHEMICAL_SUBSTANCE == 1).ToList();
                if (chemicals != null && chemicals.Count > 0)
                {
                    sereServTemp = sereServTemp.Except(chemicals).ToList();
                }


                List<ExpMestMediAndMateADO> sereServResults = new List<ExpMestMediAndMateADO>();
                if (chkMedicine.Checked)
                {
                    if (medicines != null && medicines.Count > 0)
                        sereServResults.AddRange(medicines);
                }
                if (chkMaterial.Checked)
                {
                    if (materials != null && materials.Count > 0)
                        sereServResults.AddRange(materials);
                }
                if (chkBlood.Checked)
                {
                    if (bloods != null && bloods.Count > 0)
                        sereServResults.AddRange(bloods);
                }
                if (chkHoaChat.Checked)
                {
                    if (chemicals != null && chemicals.Count > 0)
                        sereServResults.AddRange(chemicals);
                }
                if (sereServResults != null && sereServResults.Count > 0)
                {
                    if (chkIsExpend.Checked)
                    {
                        sereServResults = sereServResults.OrderBy(p => p.TYPE).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        
                    }
                    else {
                        sereServResults = sereServResults.Where(p => p.IS_EXPEND != 1).ToList();
                    }
                    //sereServResults = sereServResults.OrderBy(p => p.TYPE).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();

                    //sereServResults = sereServResults.Where(o => o.PATIENT_TYPE_ID)
                    //sereServResults = sereServResults.OrderBy(p => p.TYPE).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                }
                gridControlSereServ.DataSource = null;
                gridControlSereServ.DataSource = sereServResults;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridViewSereServ.RowCount > 0)
                {
                    ExpMestMediAndMateADOPrint = new List<ExpMestMediAndMateADO>();
                    for (int i = 0; i < gridViewSereServ.SelectedRowsCount; i++)
                    {
                        if (gridViewSereServ.GetSelectedRows()[i] >= 0)
                        {
                            ExpMestMediAndMateADOPrint.Add((ExpMestMediAndMateADO)gridViewSereServ.GetRow(gridViewSereServ.GetSelectedRows()[i]));
                        }
                    }
                    if (ExpMestMediAndMateADOPrint != null && ExpMestMediAndMateADOPrint.Count > 0)
                    {
                        PrintProcess(PrintTypeVotesMedicines.IN_PHIEU_CONG_KHAI_THUOC);
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServ_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (gridViewSereServ.SelectedRowsCount > 0)
                {
                    btnCreate.Enabled = true;
                }
                else
                {
                    btnCreate.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long TimeNumberToDateNumber(long? timeNumber)
        {
            long result = 0;
            try
            {
                string dateNumberString = timeNumber.ToString().Substring(0, 8);
                result = Inventec.Common.TypeConvert.Parse.ToInt64(dateNumberString);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void gridViewSereServ_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExpMestMediAndMateADO dataRow = (ExpMestMediAndMateADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    //SereServADO dataRow = (SereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "TOTAL_PRICE")
                    {
                        e.Value = dataRow.PRICE * dataRow.AMOUNT;
                    }
                    else if (e.Column.FieldName == "Service_Type_Display")
                    {
                        if (dataRow.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            e.Value = "Thuốc";
                        }
                        else if (dataRow.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && dataRow.IS_CHEMICAL_SUBSTANCE != 1)
                        {
                            e.Value = "Vật tư";
                        }
                        else if (dataRow.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU)
                        {
                            e.Value = "Máu";
                        }
                        else if (dataRow.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && dataRow.IS_CHEMICAL_SUBSTANCE == 1)
                        {
                            e.Value = "Hóa chất";
                        }
                    }
                    else if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
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
                btnCreate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        cboDepartment.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                    cboDepartment.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(Config.ControlStateConstan.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == Config.ControlStateConstan.chkSign)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == Config.ControlStateConstan.chkPrintDocumentSigned)
                        {
                            chkPrintDocumentSigned.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == Config.ControlStateConstan.chkTachHDSD)
                        {
                            chkTachHDSD.Checked = item.VALUE == "1";
                        }
                        else if(item.KEY == Config.ControlStateConstan.chkSortEstablish)
                        {
                            chkSortEstablish.Checked = item.VALUE == "1";
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
                if (chkSign.Checked == false)
                {
                    chkPrintDocumentSigned.Checked = false;
                    chkPrintDocumentSigned.Enabled = false;
                }
                if (!chkSortEstablish.Checked)
                {
                    chkSortName.Checked = true;
                }
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
                chkPrintDocumentSigned.Enabled = chkSign.Checked;
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                if (chkSign.Checked == false)
                {
                    chkPrintDocumentSigned.Checked = false;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == Config.ControlStateConstan.chkSign && o.MODULE_LINK == Config.ControlStateConstan.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = Config.ControlStateConstan.chkSign;
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = Config.ControlStateConstan.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintDocumentSigned_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO addOrUpdate = ((this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.FirstOrDefault(o => o.KEY == Config.ControlStateConstan.chkPrintDocumentSigned && o.MODULE_LINK == Config.ControlStateConstan.ModuleLink) : null);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => addOrUpdate), addOrUpdate));
                if (addOrUpdate != null)
                {
                    addOrUpdate.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                }
                else
                {
                    addOrUpdate = new Desktop.Library.CacheClient.ControlStateRDO();
                    addOrUpdate.KEY = Config.ControlStateConstan.chkPrintDocumentSigned;
                    addOrUpdate.MODULE_LINK = Config.ControlStateConstan.ModuleLink;
                    addOrUpdate.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(addOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkedCboHisPatientType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender
                    is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection :
                    (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag
                    as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HisPatientTypeADO rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.NAME.ToString());
                }
                if (lstHisPatientTypeADO.Count == gridCheckMark.Selection.Count)
                {
                    e.DisplayText = "Tất cả";
                }
                else
                {
                    e.DisplayText = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkedCboHisPatientType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                    if (gridCheckMark == null) return;
                    gridCheckMark.ClearSelection(checkedCboHisPatientType.Properties.View);
                    chkIsExpend.Focus();
                    //checkedCboHisPatientType.EditValue = null;
                    //checkedCboHisPatientType.Text = "";
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTachHDSD_CheckedChanged(object sender, EventArgs e)
        {
            HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == Config.ControlStateConstan.chkTachHDSD && o.MODULE_LINK == Config.ControlStateConstan.ModuleLink).FirstOrDefault() : null;
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
            if (csAddOrUpdate != null)
            {
                csAddOrUpdate.VALUE = (chkTachHDSD.Checked ? "1" : "");
            }
            else
            {
                csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                csAddOrUpdate.KEY = Config.ControlStateConstan.chkTachHDSD;
                csAddOrUpdate.VALUE = (chkTachHDSD.Checked ? "1" : "");
                csAddOrUpdate.MODULE_LINK = Config.ControlStateConstan.ModuleLink;
                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                this.currentControlStateRDO.Add(csAddOrUpdate);
            }
            this.controlStateWorker.SetData(this.currentControlStateRDO);
        }

        private void chkSortEstablish_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == Config.ControlStateConstan.chkSortEstablish && o.MODULE_LINK == Config.ControlStateConstan.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSortEstablish.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = Config.ControlStateConstan.chkSortEstablish;
                    csAddOrUpdate.VALUE = (chkSortEstablish.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = Config.ControlStateConstan.ModuleLink;
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
