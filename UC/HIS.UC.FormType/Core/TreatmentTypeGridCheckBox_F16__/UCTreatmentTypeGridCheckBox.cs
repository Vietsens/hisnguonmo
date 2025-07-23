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
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Office.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSpreadsheet.Model;
using HIS.UC.FormType.Base;
using HIS.UC.FormType.HisMultiGetString;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HIS.UC.FormType.TreatmentTypeGridCheckBox
{
    public partial class UCTreatmentTypeGridCheckBox : DevExpress.XtraEditors.XtraUserControl
    {
        private const string VALUE_MEMBER = "VALUEMEMBER";
        private const string DISPLAY_CODE_MEMBER = "DISPLAYCODEMEMBER";
        private const string DISPLAY_NAME_MEMBER = "DISPLAYNAMEMEMBER";
        private const string PARENT_ID_MEMBER = "PARENTID";
        private const string PARENT_CODE_MEMBER = "PARENTCODE";
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
        DynamicFilterRDO DynamicFilter;
        PropetiesRDO PropetiesFilter;
        string Output0 = "";
        string JsonOutput = "";
        HIS.Desktop.Common.DelegateSelectDatas delegateSelectDatas;
        public UCTreatmentTypeGridCheckBox(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO, HIS.Desktop.Common.DelegateSelectDatas delegateSelectDatas)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 125;

                this.config = config;

                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                    this.DynamicFilter = (paramRDO as GenerateRDO).DynamicFilter;
                    if (this.DynamicFilter != null)
                    {
                        this.config = this.DynamicFilter.Fofi;
                    }
                }
                this.delegateSelectDatas = delegateSelectDatas;
                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                if (this.DynamicFilter != null)
                {
                    InitPropeties();
                }
                else
                    Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitPropeties()
        {
            try
            {
                FormTypeConfig.ReportTypeCode = this.config.REPORT_TYPE_CODE;
                JsonOutput = this.config.JSON_OUTPUT;
                PropetiesFilter = DynamicFilter.Propeties;
                limitCodes = PropetiesFilter != null && !string.IsNullOrEmpty(PropetiesFilter.LimitCode) ? PropetiesFilter.LimitCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : null;
                Output0 = PropetiesFilter != null ? (PropetiesFilter.DefaultValue as string) : null;
                dataSource = ProcessDataSource();
                gridViewTreatmentTypes.GridControl.DataSource = dataSource;
                this.gridColumn5.Caption = "Mã " + this.config.DESCRIPTION;
                this.gridColumn6.Caption = "Tên " + this.config.DESCRIPTION;
                InitGridCheckMarksSelection();
                LoadDefault();
                if (this.report != null)
                {
                    SetValue();
                }
                if (!string.IsNullOrEmpty(Output0))
                {
                    selectedFilters = dataSource.Where(o => Output0.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Contains(o.CODE)).ToList();
                    List<long> Ids = selectedFilters.Select(o => o.ID).ToList();
                    SetGridCheckMark(Ids, dataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private List<DataGet> ProcessDataSource()
        {
            List<DataGet> result = new List<DataGet>();

            try
            {
                if (DynamicFilter.DATA_TABLE != null && DynamicFilter.DATA_TABLE.Count > 0)
                {
                    result = ConvertDataTableToDataGet(DynamicFilter.DATA_TABLE);
                }
                else if (DynamicFilter.DATA_CACHE != null && DynamicFilter.DATA_CACHE.Length > 0)
                {
                    string tableName, valueMember, displayCodeMember, displayNameMember, parentId, parentCode;
                    ParseTableConfigString(DynamicFilter.DATA_CACHE, out tableName, out valueMember, out displayCodeMember, out displayNameMember, out parentId, out parentCode);
                    Type TableValid = IsTableInMOSEFModel(tableName);
                    if (TableValid != null)
                    {
                        Type backendType = typeof(HIS.Desktop.LocalStorage.BackendData.BackendDataWorker);
                        var method = backendType.GetMethod("IsExistsKey", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                        if (method != null)
                        {
                            var genericMethod = method.MakeGenericMethod(TableValid);
                            var isExists = genericMethod.Invoke(null, null);
                            if ((bool)isExists)
                            {
                                var dicObject = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.GetAll()[TableValid];
                                result = ConvertDataTableToDataGet(AddListDataCache(TableValid, dicObject, valueMember, displayCodeMember, displayNameMember, parentId, parentCode));
                            }
                            else
                            {
                                //Trường hợp không sử dụng bảng, kiểm tra có sử dụng V_ không
                                if (tableName.ToUpper().StartsWith("V_"))
                                    tableName = tableName.Replace("V_", "");
                                else

                                    tableName = "V_" + tableName;
                                var TableValidCv = IsTableInMOSEFModel(tableName);
                                backendType = typeof(HIS.Desktop.LocalStorage.BackendData.BackendDataWorker);
                                method = backendType.GetMethod("IsExistsKey", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                                if (method != null)
                                {
                                    if (TableValidCv != null)
                                    {
                                        genericMethod = method.MakeGenericMethod(TableValidCv);
                                        isExists = genericMethod.Invoke(null, null);
                                    }
                                    else
                                        isExists = false;
                                    if ((bool)isExists)
                                    {
                                        var dicObject = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.GetAll()[TableValidCv];
                                        result = ConvertDataTableToDataGet(AddListDataCache(TableValidCv, dicObject, valueMember, displayCodeMember, displayNameMember, parentId, parentCode));
                                    }
                                    else
                                    {
                                        var methodGet = typeof(HIS.Desktop.LocalStorage.BackendData.BackendDataWorker)
                                    .GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                                    .FirstOrDefault(m => m.Name == "Get" && m.IsGenericMethod && m.GetParameters().Length == 0);
                                        object dicObject = null;
                                        if (methodGet != null)
                                        {
                                            var genericGet = methodGet.MakeGenericMethod(TableValid);
                                            dicObject = genericGet.Invoke(null, null);
                                            result = ConvertDataTableToDataGet(AddListDataCache(TableValid, dicObject, valueMember, displayCodeMember, displayNameMember, parentId, parentCode));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private List<DataTable> AddListDataCache(Type TableValid, object dicObject, string valueMember, string displayCodeMember, string displayNameMember, string parentId, string parentCode)
        {
            List < DataTable > dataTables = new List<DataTable>();
            try
            {
                DataTable table = new DataTable();
                if (!string.IsNullOrEmpty(valueMember)) table.Columns.Add(VALUE_MEMBER, typeof(object));
                if (!string.IsNullOrEmpty(displayCodeMember)) table.Columns.Add(DISPLAY_CODE_MEMBER, typeof(object));
                if (!string.IsNullOrEmpty(displayNameMember)) table.Columns.Add(DISPLAY_NAME_MEMBER, typeof(object));
                if (!string.IsNullOrEmpty(parentId)) table.Columns.Add(PARENT_ID_MEMBER, typeof(object));
                if (!string.IsNullOrEmpty(parentCode)) table.Columns.Add(PARENT_CODE_MEMBER, typeof(object));

                var props = TableValid.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var item in (IEnumerable<object>)dicObject)
                {
                    DataRow row = table.NewRow();
                    if (!string.IsNullOrEmpty(valueMember))
                    {
                        var prop = props.FirstOrDefault(p => p.Name == valueMember);
                        row[VALUE_MEMBER] = prop != null ? prop.GetValue(item) ?? DBNull.Value : DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(displayCodeMember))
                    {
                        var prop = props.FirstOrDefault(p => p.Name == displayCodeMember);
                        row[DISPLAY_CODE_MEMBER] = prop != null ? prop.GetValue(item) ?? DBNull.Value : DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(displayNameMember))
                    {
                        var prop = props.FirstOrDefault(p => p.Name == displayNameMember);
                        row[DISPLAY_NAME_MEMBER] = prop != null ? prop.GetValue(item) ?? DBNull.Value : DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(parentId))
                    {
                        var prop = props.FirstOrDefault(p => p.Name == parentId);
                        row[PARENT_ID_MEMBER] = prop != null ? prop.GetValue(item) ?? DBNull.Value : DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(parentCode))
                    {
                        var prop = props.FirstOrDefault(p => p.Name == parentCode);
                        row[PARENT_CODE_MEMBER] = prop != null ? prop.GetValue(item) ?? DBNull.Value : DBNull.Value;
                    }
                    table.Rows.Add(row);
                }
                dataTables.Add(table);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return dataTables;

        }
        private List<DataGet> ConvertDataTableToDataGet(List<DataTable> dt)
        {
            List<DataGet> result = new List<DataGet>();
            try
            {
                foreach (var table in dt)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        DataGet dataGett = new DataGet();
                        if (table.Columns.Contains(VALUE_MEMBER) && row[VALUE_MEMBER] != DBNull.Value)
                            dataGett.ID = Convert.ToInt64(row[VALUE_MEMBER]);
                        if (table.Columns.Contains(DISPLAY_CODE_MEMBER) && row[DISPLAY_CODE_MEMBER] != DBNull.Value)
                            dataGett.CODE = row[DISPLAY_CODE_MEMBER].ToString();
                        if (table.Columns.Contains(DISPLAY_NAME_MEMBER) && row[DISPLAY_NAME_MEMBER] != DBNull.Value)
                            dataGett.NAME = row[DISPLAY_NAME_MEMBER].ToString();
                        if (table.Columns.Contains(PARENT_ID_MEMBER) && row[PARENT_ID_MEMBER] != DBNull.Value)
                            dataGett.PARENT = Convert.ToInt64(row[PARENT_ID_MEMBER]);
                        if (table.Columns.Contains(PARENT_CODE_MEMBER) && row[PARENT_CODE_MEMBER] != DBNull.Value)
                            dataGett.PARENT_CODE = row[PARENT_CODE_MEMBER].ToString();
                        result.Add(dataGett);
                    }
                }
                result = result.Where(o => limitCodes != null && limitCodes.Count() > 0 ? limitCodes.Contains(o.CODE) : true).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        Type IsTableInMOSEFModel(string tableName)
        {
            // Pseudocode plan:
            // 1. Get all types in MOS.EFMODEL.DataModels namespace that are classes.
            // 2. For each type, get its public properties' names (case-insensitive).
            // 3. Exclude types that have any property named "TREATMENT_ID", "PATIENT_ID", "SERVICE_REQ_ID", or "EXP_MEST_ID".
            // 4. Return or process the filtered list as needed.

            var mosAssembly = typeof(MOS.EFMODEL.DataModels.HIS_BRANCH).Assembly;
            var types = mosAssembly.GetTypes()
                .Where(t => t.Namespace == "MOS.EFMODEL.DataModels" && t.IsClass)
                .Where(t =>
                {
                    var propNames = t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                     .Select(p => p.Name.ToUpperInvariant())
                                     .ToList();
                    return !propNames.Contains("TREATMENT_ID")
                        && !propNames.Contains("PATIENT_ID")
                        && !propNames.Contains("SERVICE_REQ_ID")
                        && !propNames.Contains("EXP_MEST_ID");
                })
                .ToList();

            return types.FirstOrDefault(o => o.Name == tableName);
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
            }
            ;
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
            }
            ;
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
                if (delegateSelectDatas != null && PropetiesFilter != null && !string.IsNullOrEmpty(PropetiesFilter.ValueTransfer) && PropetiesFilter.IdReference > 0)
                {
                    string content = null;
                    if (PropetiesFilter.ValueTransfer.ToUpper().Contains(VALUE_MEMBER))
                    {
                        content = string.Join(",", selectedFilters.Select(o => o.ID));
                    }
                    else if (PropetiesFilter.ValueTransfer.ToUpper().Contains(DISPLAY_CODE_MEMBER))
                    {
                        content = string.Join(",", selectedFilters.Select(o => o.CODE));
                    }
                    else if (PropetiesFilter.ValueTransfer.ToUpper().Contains(DISPLAY_NAME_MEMBER))
                    {
                        content = string.Join(",", selectedFilters.Select(o => o.NAME));
                    }
                    delegateSelectDatas(PropetiesFilter.IdReference, string.Join(",", selectedFilters.Select(o => o.ID)));
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
                        if (PropetiesFilter != null)
                        {
                            SetGridCheckMark(Ids, dataSource);
                        }
                        else
                        {
                            FDO = FilterConfig.HisFilterTypes(this.JsonOutput);
                            ReSelectGridView(Ids);
                        }
                            //gridViewTreatmentTypes.GridControl.DataSource = null;

                    }

                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public bool GetValueReceive(object data)
        {
            bool result = false;
            try
            {
                var dataSource = this.dataSource;
                if (data != null && PropetiesFilter != null && !string.IsNullOrEmpty(PropetiesFilter.ValueReceive) && data != null && !string.IsNullOrEmpty(data.ToString()))
                {
                    var convertData = data.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (PropetiesFilter.ValueReceive.ToUpper().Contains(PARENT_ID_MEMBER))
                    {
                        dataSource = dataSource.Where(o => convertData.Contains(o.PARENT.ToString())).ToList();
                    }
                    else if (PropetiesFilter.ValueReceive.ToUpper().Contains(PARENT_CODE_MEMBER))
                    {
                        dataSource = dataSource.Where(o => convertData.Contains(o.PARENT_CODE)).ToList();
                    }
                    else if (PropetiesFilter.ValueReceive.ToUpper().Contains(DISPLAY_CODE_MEMBER))
                    {
                        dataSource = dataSource.Where(o => convertData.Contains(o.CODE)).ToList();
                    }
                    else if (PropetiesFilter.ValueReceive.ToUpper().Contains(DISPLAY_NAME_MEMBER))
                    {
                        dataSource = dataSource.Where(o => convertData.Contains(o.NAME)).ToList();
                    }
                    result = true;
                }
                gridViewTreatmentTypes.GridControl.DataSource = dataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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
                SetGridCheckMark(Ids, listView);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetGridCheckMark(List<long> Ids, List<DataGet> listView)
        {

            try
            {
                gridViewTreatmentTypes.GridControl.DataSource = listView.OrderBy(o => !Ids.Contains(o.ID)).ToList();
                selectedFilters = listView.Where(o => Ids.Contains(o.ID)).ToList();
                DevExpress.XtraGrid.Selection.GridCheckMarksSelection gridCheckMark = gridViewTreatmentTypes.Tag as DevExpress.XtraGrid.Selection.GridCheckMarksSelection;
                gridCheckMark.Selection.Clear();
                gridCheckMark.Selection.AddRange(selectedFilters);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        // Hàm tách chuỗi theo yêu cầu: 
        // Input: [HIS_DEPARTMENT]{"ValueMember":"ID", "DisplayCodeMember":"DEPARTMENT_CODE","DisplayNameMember":"DEPARTMENT_NAME"}
        // Output: 
        //   - tableName = "HIS_DEPARTMENT"
        //   - valueMember = "ID"
        //   - displayCodeMember = "DEPARTMENT_CODE"
        //   - displayNameMember = "DEPARTMENT_NAME"
        private void ParseTableConfigString(string input, out string tableName, out string valueMember, out string displayCodeMember, out string displayNameMember, out string parentId, out string parentCode)
        {
            tableName = null;
            valueMember = null;
            displayCodeMember = null;
            displayNameMember = null;
            parentId = null;
            parentCode = null;
            try
            {
                int startBracket = input.IndexOf('[');
                int endBracket = input.IndexOf(']');
                if (startBracket >= 0 && endBracket > startBracket)
                {
                    tableName = input.Substring(startBracket + 1, endBracket - startBracket - 1).ToUpper();
                }

                int jsonStart = input.IndexOf('{', endBracket);
                int jsonEnd = input.LastIndexOf('}');
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    string json = input.Substring(jsonStart, jsonEnd - jsonStart + 1);
                    string[] pairs = json.Trim('{', '}').Split(',');
                    foreach (var pair in pairs)
                    {
                        var kv = pair.Split(':');
                        if (kv.Length == 2)
                        {
                            string key = kv[0].Trim(' ', '"');
                            string value = kv[1].Trim(' ', '"');
                            switch (key.ToUpper())
                            {
                                case VALUE_MEMBER:
                                    valueMember = value;
                                    break;
                                case DISPLAY_CODE_MEMBER:
                                    displayCodeMember = value;
                                    break;
                                case DISPLAY_NAME_MEMBER:
                                    displayNameMember = value;
                                    break;
                                case PARENT_ID_MEMBER:
                                    parentId = value;
                                    break;
                                case PARENT_CODE_MEMBER:
                                    parentCode = value;
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                tableName = null;
                valueMember = null;
                displayCodeMember = null;
                displayNameMember = null;
                parentId = null;
                parentCode = null;

                Inventec.Common.Logging.LogSystem.Error("Input loi " + input);
            }
        }

    }
}
