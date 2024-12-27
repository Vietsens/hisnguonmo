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
using DevExpress.XtraEditors;
using DevExpress.Utils;
using HIS.UC.FormType.Loader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.FormType.MultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment
{
    public partial class UCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment : DevExpress.XtraEditors.XtraUserControl
    {
        private List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> selectedDepartments;
        private List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> selectedMedicines = new List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
        bool isValidData = false;
        private WaitDialogForm waitLoad;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 300;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                
                this.config = config;
                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //Nạp dữ liệu lên các UC:
        void Init()
        {
            try
            {
                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = Config.HisFormTypeConfig.VHisMedicineTypes;
                gridView1.EndUpdate();
                InitGridCheckMarksSelection();
                if (this.isValidData)
                {
                    //lblTitleName.ForeColor = Color.Maroon;
                }

                SetTitle();
                DepartmentLoader.LoadDataToCombo(comboBoxEdit1);
                MediStockLoader.LoadDataToCombo(comboBoxEdit2);
                MediStockPeriodLoader.LoadDataToCombo(comboBoxEdit3);

                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    //Validation();
                    //lciTitleName.AppearanceItemCaption.ForeColor = Color.Maroon;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void InitGridCheckMarksSelection()
        {
            try
            {
                DevExpress.XtraGrid.Selection.GridCheckMarksSelection gridCheckMarksSA = new DevExpress.XtraGrid.Selection.GridCheckMarksSelection(gridView1);
                gridCheckMarksSA.SelectionChanged += new DevExpress.XtraGrid.Selection.GridCheckMarksSelection.SelectionChangedEventHandler(gridCheckMarks_SelectionChanged);
                gridView1.Tag = gridCheckMarksSA;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void gridCheckMarks_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                selectedMedicines.Clear();
                foreach (V_HIS_MEDICINE_TYPE rv in (sender as DevExpress.XtraGrid.Selection.GridCheckMarksSelection).Selection)
                {
                    selectedMedicines.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    //lciTitleName.Text = this.config.DESCRIPTION;
                    //lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    //lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string GetValue()
        {
            string value = "";
            List<long> MEDICINE_TYPE_IDs = new List<long>();
            try
            {
                if (selectedMedicines.Count > 0)
                {
                    MEDICINE_TYPE_IDs = selectedMedicines.Select(s => s.ID).ToList();
                }

                value = String.Format(this.config.JSON_OUTPUT, Newtonsoft.Json.JsonConvert.SerializeObject(MEDICINE_TYPE_IDs));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }

        private void SetValue()
        {
            try
            {
                if (this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,this.config.JSON_OUTPUT, this.report.JSON_FILTER);
                    List<long> Ids = null;
                    if (value != null && value != "null" && value != "")
                    {
                        try
                        {
                            Ids = (value.Substring(1)).Substring(0, value.Substring(1).Length - 1).Split(new string[] { "," }, StringSplitOptions.None)
                                .Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o)).ToList();
                        }
                        catch (Exception)
                        {

                            Ids = null;
                        }
                        if (Ids != null)
                        {

                            var listView = Config.HisFormTypeConfig.VHisMedicineTypes;
                            gridView1.GridControl.DataSource = listView;
                            selectedMedicines = listView.Where(o => Ids.Contains(o.ID)).ToList();
                            DevExpress.XtraGrid.Selection.GridCheckMarksSelection gridCheckMark = gridView1.Tag as DevExpress.XtraGrid.Selection.GridCheckMarksSelection;
                            gridCheckMark.Selection.AddRange(selectedMedicines);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (this.isValidData)
                {
                    // var rows = gridViewDepartment.GetSelectedRows();
                    var rowRooms = gridView1.GetSelectedRows();
                    //if (rows == null || rows.Count() == 0 || rowRooms == null || rowRooms.Count() == 0)
                    //{
                    //    result = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }
        // Đặt giá trị mã kho và tên kho khi kết thúc chọn kho:
        private void comboBoxEdit2_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (comboBoxEdit2.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.VHisMediStock.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit2.EditValue.ToString()));
                        if (department != null)
                        {
                            textEdit2.Text = department.MEDI_STOCK_CODE;
                            textEdit3.Text = "";
                            comboBoxEdit3.EditValue = null;
                            comboBoxEdit3.Properties.DataSource = Config.HisFormTypeConfig.VHisMediStockPeriod.Where(o => o.MEDI_STOCK_ID == department.ID).ToList();
                        }
                    }
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void comboBoxEdit1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (comboBoxEdit1.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.HisDepartments.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit1.EditValue.ToString()));
                        if (department != null)
                        {
                            textEdit1.Text = department.DEPARTMENT_CODE;
                            textEdit2.Text = "";
                            comboBoxEdit2.EditValue = null;
                            comboBoxEdit2.Properties.DataSource = Config.HisFormTypeConfig.VHisMediStock.Where(o => o.DEPARTMENT_ID == department.ID).ToList();
                        }
                    }
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void comboBoxEdit3_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (comboBoxEdit3.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.VHisMediStockPeriod.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit3.EditValue.ToString()));
                        if (department != null)
                        {
                            textEdit3.Text = department.MEDI_STOCK_PERIOD_NAME;
                            gridView1.BeginUpdate();
                            //gridView1.GridControl.DataSource = FormTypeConfig.VHisMedicineTypes.Where(o => o.MEDI_STOCK_PERIOD_ID == department.ID).ToList();
                            gridView1.EndUpdate();
                            //textEdit2.Text = "";
                            //comboBoxEdit2.EditValue = null;
                            //comboBoxEdit2.Properties.DataSource = FormTypeConfig.VHisMediStocks.Where(o => o.DEPARTMENT_ID == department.ID).ToList();
                        }
                    }
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment_Load(object sender, EventArgs e)
        {
            try
            {
                layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MULTI_MEDICINE_GRID_CHECK_FILTER_BY_MEDISTOCK_PERIOD_BY_MEDISTOCK_BY_DEPARTMENT_LAYOUT_CONTROL_ITEM5", Resources.ResourceLanguageManager.LanguageUCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment, Base.LanguageManager.GetCulture());
                layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MULTI_MEDICINE_GRID_CHECK_FILTER_BY_MEDISTOCK_PERIOD_BY_MEDISTOCK_BY_DEPARTMENT_LAYOUT_CONTROL_ITEM6", Resources.ResourceLanguageManager.LanguageUCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment, Base.LanguageManager.GetCulture());
                layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MULTI_MEDICINE_GRID_CHECK_FILTER_BY_MEDISTOCK_PERIOD_BY_MEDISTOCK_BY_DEPARTMENT_LAYOUT_CONTROL_ITEM7", Resources.ResourceLanguageManager.LanguageUCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment, Base.LanguageManager.GetCulture());
                gridColumn1.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MULTI_MEDICINE_GRID_CHECK_FILTER_BY_MEDISTOCK_PERIOD_BY_MEDISTOCK_BY_DEPARTMENT_GRID_COLUMN1", Resources.ResourceLanguageManager.LanguageUCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment, Base.LanguageManager.GetCulture());
                gridColumn2.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MULTI_MEDICINE_GRID_CHECK_FILTER_BY_MEDISTOCK_PERIOD_BY_MEDISTOCK_BY_DEPARTMENT_GRID_COLUMN2", Resources.ResourceLanguageManager.LanguageUCMultiMedicineGridCheckFilterByMediStockPeriodByMediStockByDepartment, Base.LanguageManager.GetCulture());
                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void textEdit1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }
    }
}
