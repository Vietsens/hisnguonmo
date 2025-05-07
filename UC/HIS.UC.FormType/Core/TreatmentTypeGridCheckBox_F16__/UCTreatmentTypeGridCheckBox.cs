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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using HIS.UC.FormType.HisMultiGetString;
using HIS.UC.FormType.Base;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;

namespace HIS.UC.FormType.TreatmentTypeGridCheckBox
{
    public partial class UCTreatmentTypeGridCheckBox : DevExpress.XtraEditors.XtraUserControl
    {
        string FDO = null;
        string[] limitCodes = null;
        string valueSend = "";

        private List<long> DEPARTMENT_IDs = new List<long>();
        private List<string> DEPARTMENT_CODEs = new List<string>();
        private List<DataGet> selectedFilters = new List<DataGet>();
        private List<DataGet> dataSource = new List<DataGet>();
        TreatmentTypeGridCheckBoxFDO generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        bool isValidData = true;
        int positionHandleControl = -1;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        string Output0 = "";
        string JsonOutput = "";

        public UCTreatmentTypeGridCheckBox(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 125;

                this.config = config;

                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Init()
        {
            try
            {
                SetTitle();
                FormTypeConfig.ReportTypeCode = this.config.REPORT_TYPE_CODE;

                FDO = FilterConfig.HisFilterTypes(this.config.JSON_OUTPUT);
                limitCodes = FilterConfig.GetLimitCodes(this.config.JSON_OUTPUT);
                FilterConfig.GetValueOutput0(this.config.JSON_OUTPUT, ref Output0);
                dataSource = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0);

                JsonOutput = this.config.JSON_OUTPUT;
                FilterConfig.RemoveStrOutput0(ref JsonOutput);
                gridViewTreatmentTypes.GridControl.DataSource = dataSource;
                this.gridColumn5.Caption = "Mã " + this.config.DESCRIPTION;
                this.gridColumn6.Caption = "Tên " + this.config.DESCRIPTION;

                InitGridCheckMarksSelection();

                //if (FDO == "THE_BRANCH" && this.config.JSON_OUTPUT.Contains("ADMIN"))
                //{
                //    if (dataSource != null && dataSource.Count == 1)
                //    {
                //        selectedFilters.Add(dataSource.First());
                //        this.Enabled = false;
                //    }
                //}
                LoadDefault();
                LoadBranch();
                if (this.report != null)
                {
                    SetValue();
                }

                SelectFilter(this.config.JSON_OUTPUT, Output0, dataSource, ref selectedFilters);
                List<long> Ids = selectedFilters.Select(o => o.ID).ToList();
                ReSelectGridView(Ids);
                if (isValidData)
                {
                    //lciTitleName.AppearanceItemCaption.ForeColor = Color.Maroon;
                    //lblTitleName.ForeColor = Color.Maroon;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefault()
        {
            try
            {
                if (dataSource.Count == 1 && this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    selectedFilters.Add(dataSource.First());
                }

            }
            catch (Exception ex)
            {   
                LogSystem.Error(ex);
            }
        }

        private void LoadBranch()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.config.JSON_OUTPUT) && (this.config.JSON_OUTPUT.Contains("BRANCH_ID") || this.config.JSON_OUTPUT.Contains("BRANCH__ID")) && this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    ReSelectGridView(new List<long>() { FormTypeConfig.BranchId });
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        /// <summary>
        /// Chon cac dieu kien loc mac dinh
        /// </summary>
        /// <param name="JSON_OUTPUT"></param>
        /// <param name="Output0"></param>
        /// <param name="dataSource"></param>
        /// <param name="selectedFilters"></param>
        private void SelectFilter(string JSON_OUTPUT, string Output0, List<DataGet> dataSource, ref List<DataGet> selectedFilters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(JSON_OUTPUT)) return;
                if (string.IsNullOrWhiteSpace(Output0)) return;
                if (dataSource == null || dataSource.Count == 0) return;
                //Khong select neu da tung select truoc do
                if (selectedFilters != null && selectedFilters.Count > 0) return;

                List<string> Codes = null;
                StringToListCode(Output0, ref Codes);
                SelectFilterByCode(Codes, dataSource, ref selectedFilters);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void StringToListCode(string Output0, ref List<string> Codes)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Output0)) return;
                Codes = Output0.Split(',').ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void SelectFilterById(List<long> Ids, List<DataGet> dataSource, ref List<DataGet> selectedFilters)
        {
            try
            {
                if (Ids == null || Ids.Count == 0) return;
                if (dataSource == null || dataSource.Count == 0) return;
                //Khong select neu da tung select truoc do
                if (selectedFilters != null && selectedFilters.Count > 0) return;
                selectedFilters = dataSource.Where(o => Ids.Contains(o.ID)).ToList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            };
        }

        private void SelectFilterByCode(List<string> Codes, List<DataGet> dataSource, ref List<DataGet> selectedFilters)
        {
            try
            {
                if (Codes == null || Codes.Count == 0) return;
                if (dataSource == null || dataSource.Count == 0) return;
                //Khong select neu da tung select truoc do
                if (selectedFilters != null && selectedFilters.Count > 0) return;
                selectedFilters = dataSource.Where(o => Codes.Contains(o.CODE)).ToList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            };
        }

        void InitGridCheckMarksSelection()
        {
            try
            {
                DevExpress.XtraGrid.Selection.GridCheckMarksSelection gridCheckMarksSA = new DevExpress.XtraGrid.Selection.GridCheckMarksSelection(gridViewTreatmentTypes);
                gridCheckMarksSA.SelectionChanged += new DevExpress.XtraGrid.Selection.GridCheckMarksSelection.SelectionChangedEventHandler(gridCheckMarks_SelectionChanged);
                gridViewTreatmentTypes.Tag = gridCheckMarksSA;
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
                selectedFilters.Clear();
                foreach (DataGet rv in (sender as DevExpress.XtraGrid.Selection.GridCheckMarksSelection).Selection)
                {
                    selectedFilters.Add(rv);
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
                //if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                //{
                //    lciTitleName.Text = this.config.DESCRIPTION;
                //    lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //}
                //else
                //{
                //    lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //}
                if (this.config != null)
                {
                    //lciTitleName.Text = this.config.DESCRIPTION ?? " ";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRooms_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //try
            //{
            //    selectedFilters.Clear();
            //    var rows = gridViewTreatmentTypes.GetSelectedRows();
            //    for (int i = 0; i < rows.Length; i++)
            //    {
            //        selectedFilters.Add((DataGet)gridViewTreatmentTypes.GetRow(rows[i]));
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        /// <summary>
        /// Get value in form return in object
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            DEPARTMENT_IDs.Clear();
            DEPARTMENT_CODEs.Clear();
            try
            {
                string filterType = FilterConfig.HisFilterTypes(this.config.JSON_OUTPUT);
                var dataGet = new List<DataGet>();
                if (filterType != null)
                {
                    dataGet = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0);
                }
                if (selectedFilters != null && selectedFilters.Count > 0)
                {
                    DEPARTMENT_IDs = new List<long>();
                    DEPARTMENT_CODEs = new List<string>();
                    foreach (var rv in selectedFilters)
                    {
                        DEPARTMENT_CODEs.Add(rv.CODE);
                        DEPARTMENT_IDs.Add(rv.ID);
                    }
                }

                if (this.isValidData && DEPARTMENT_IDs.Count == 0)
                {
                    WaitingManager.Hide();
                    System.Windows.Forms.MessageBox.Show("Chưa chọn " + this.config.DESCRIPTION);
                    //HIS.UC.FormType.Medicin.UCMedicin.exitclick = true;
                    valueSend = "error";
                }
                else
                {

                    string[] Filters = null;
                    FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);
                    if (!(Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0])))
                    {
                        if (DEPARTMENT_IDs.Count > ManagerConstant.MAX_REQUEST_LENGTH_PARAM)
                        {
                            DEPARTMENT_IDs = new List<long>();
                        }
                        valueSend = String.Format(this.JsonOutput, Newtonsoft.Json.JsonConvert.SerializeObject(DEPARTMENT_IDs));
                    }    
                        
                    else
                    {
                        if (DEPARTMENT_CODEs.Count > ManagerConstant.MAX_REQUEST_LENGTH_PARAM)
                        {
                            DEPARTMENT_CODEs = new List<string>();
                        }
                        valueSend = String.Format(this.JsonOutput, Newtonsoft.Json.JsonConvert.SerializeObject(DEPARTMENT_CODEs));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valueSend;
        }
        public void SetValue()
        {
            try
            {
                if (this.JsonOutput != null && this.report.JSON_FILTER != null)
                {
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, this.JsonOutput, this.report.JSON_FILTER);
                    List<long> Ids = null;
                    string[] Filters = null;
                    FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);

                    if ((Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0])) && value != null && value != "null")
                    {
                        List<string> codes = new List<string>();
                        try
                        {
                            codes = (value.Substring(1)).Substring(0, value.Substring(1).Length - 1).Split(new string[] { "," }, StringSplitOptions.None)
                               .ToList();
                        }
                        catch (Exception)
                        {

                            codes = new List<string>();
                        }
                        var listView = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0);
                        if (listView != null)
                        {
                            Ids = listView.Where(f => codes.Exists(p => f.CODE == p.Replace("\"", "").Trim())).Select(o => o.ID).ToList();
                        }
                    }
                    else if (value != null && value != "null")
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
                    }
                    if (Ids != null)
                    {
                        FDO = FilterConfig.HisFilterTypes(this.JsonOutput);
                        ReSelectGridView(Ids);
                        //gridViewTreatmentTypes.GridControl.DataSource = null;

                    }

                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        /// Chu y: Sau khi chon selectedFilters phai select lai grid view theo ham nay
        /// </summary>
        /// <param name="Ids"></param>
        private void ReSelectGridView(List<long> Ids)
        {
            try
            {
                var listView = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0);
                gridViewTreatmentTypes.GridControl.DataSource = listView.OrderBy(o => !Ids.Contains(o.ID)).ToList();
                selectedFilters = listView.Where(o => Ids.Contains(o.ID)).ToList();
                DevExpress.XtraGrid.Selection.GridCheckMarksSelection gridCheckMark = gridViewTreatmentTypes.Tag as DevExpress.XtraGrid.Selection.GridCheckMarksSelection;
                gridCheckMark.Selection.Clear();
                gridCheckMark.Selection.AddRange(selectedFilters);
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
                    result = (valueSend == null || valueSend == "") ? false : true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
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

        private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    List<DataGet> listView = new List<DataGet>();
                    if (selectedFilters != null && selectedFilters.Count > 0)
                        listView.AddRange(selectedFilters);

                    listView.AddRange(dataSource.Where(o => checkdata(o, txtFind.Text)).ToList());
                    listView = listView.Distinct().ToList();
                    gridViewTreatmentTypes.GridControl.DataSource = listView;

                    //sau khi set select đầu tiên thì selectedFilters sẽ thay đổi nên cần dùng biến phụ
                    List<DataGet> listSelected = new List<DataGet>();
                    if (selectedFilters != null && selectedFilters.Count > 0)
                    {
                        listSelected.AddRange(selectedFilters);
                        foreach (var item in listSelected)
                        {
                            var i = listView.IndexOf(item);
                            gridViewTreatmentTypes.SelectRow(gridViewTreatmentTypes.GetRowHandle(i));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkdata(DataGet data, string seach)
        {
            bool result = false;
            try
            {
                if (data != null && !String.IsNullOrEmpty(data.CODE))
                {
                    if (String.IsNullOrEmpty(seach))
                        return true;

                    result = (!selectedFilters.Select(p => p.CODE).Contains(data.CODE)) &&
                    ((data.CODE.ToLower().Contains(seach.ToLower())) || (data.NAME != null && data.NAME.ToLower().Contains(seach.ToLower())));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }
    }
}
