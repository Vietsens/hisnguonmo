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
//using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraTreeList.Data;
using His.UC.LibraryMessage;
using HIS.UC.FormType.HisMultiGetString;
using HIS.UC.FormType.Loader;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.GridLookUpTextEdit
{
    public partial class UCGridLookUpTextEdit : DevExpress.XtraEditors.XtraUserControl
    {
        private const string VALUE_MEMBER = "VALUEMEMBER";
        private const string DISPLAY_CODE_MEMBER = "DISPLAYCODEMEMBER";
        private const string DISPLAY_NAME_MEMBER = "DISPLAYNAMEMEMBER";
        private const string PARENT_ID_MEMBER = "PARENTID";
        private const string PARENT_CODE_MEMBER = "PARENTCODE";
        private const string REF_FILTER_1 = "REFFILTER1";
        private const string REF_FILTER_2 = "REFFILTER2";
        private const string REF_FILTER_3 = "REFFILTER3";
        private const string REF_FILTER_4 = "REFFILTER4";
        private const string REF_FILTER_5 = "REFFILTER5";
        GridLookUpTextEditFDO generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        string FDO = null;
        string[] limitCodes = null;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        DynamicFilterRDO DynamicFilter;
        PropetiesRDO PropetiesFilter;
        string Output0 = "";
        string JsonOutput = "";
        HIS.Desktop.Common.DelegateSelectDatas delegateSelectDatas;
        public UCGridLookUpTextEdit(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO, HIS.Desktop.Common.DelegateSelectDatas delegateSelectDatas)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 25;

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
                cboDepartment.Properties.Buttons[1].Visible = true;
                if (this.DynamicFilter != null)
                {
                    InitPropeties();
                }
                else
                    Init();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        List<DataGet> dataSource = null;
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
                cboDepartment.Properties.DataSource = dataSource;
                cboDepartment.Properties.DisplayMember = "NAME";

                var listData = cboDepartment.Properties.DataSource as List<DataGet>;
                DataGet defaultValue = null;
                if (!string.IsNullOrWhiteSpace(Output0) && listData != null)
                {
                    defaultValue = listData.FirstOrDefault(o => o.CODE == Output0);
                }
                string[] Filters = null;
                FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);
                if (!(Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0])))
                {
                    cboDepartment.Properties.ValueMember = "ID";
                    if (defaultValue != null && defaultValue.ID > 0)
                    {
                        cboDepartment.EditValue = defaultValue.ID;
                        txtDepartmentCode.Text = defaultValue.CODE;
                    }
                }
                else
                {
                    cboDepartment.Properties.ValueMember = "CODE";
                    if (defaultValue != null)
                    {
                        cboDepartment.EditValue = defaultValue.CODE;
                        txtDepartmentCode.Text = defaultValue.CODE;
                    }
                }
                cboDepartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboDepartment.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboDepartment.Properties.ImmediatePopup = true;
                cboDepartment.ForceInitialize();
                cboDepartment.Properties.View.Columns.Clear();
                cboDepartment.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboDepartment.Properties.View.Columns.AddField("CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = false;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboDepartment.Properties.View.Columns.AddField("NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = false;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
                aColumnCode.Visible = true;
                aColumnName.Visible = true;
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    Validation();
                    lciTitleName.AppearanceItemCaption.ForeColor = Color.Maroon;
                }

                if (defaultValue == null && cboDepartment.Properties.DataSource != null && (cboDepartment.Properties.DataSource as List<DataGet>).Count > 0)
                {
                    if (listData.Count == 1)
                    {
                        txtDepartmentCode.Text = listData.First().CODE;
                        cboDepartment.EditValue = listData.First().ID;
                    }
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
                                TableValid = IsTableInMOSEFModel(tableName);
                                backendType = typeof(HIS.Desktop.LocalStorage.BackendData.BackendDataWorker);
                                method = backendType.GetMethod("IsExistsKey", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                                if (method != null)
                                {
                                    genericMethod = method.MakeGenericMethod(TableValid);
                                    isExists = genericMethod.Invoke(null, null);
                                    if ((bool)isExists)
                                    {
                                        var dicObject = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.GetAll()[TableValid];
                                        result = ConvertDataTableToDataGet(AddListDataCache(TableValid, dicObject, valueMember, displayCodeMember, displayNameMember, parentId, parentCode));
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
            List<DataTable> dataTables = new List<DataTable>();
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
                        if (table.Columns.Contains(REF_FILTER_1) && row[REF_FILTER_1] != DBNull.Value)
                            dataGett.REF_FILTER_1 = row[REF_FILTER_1];
                        if (table.Columns.Contains(REF_FILTER_2) && row[REF_FILTER_2] != DBNull.Value)
                            dataGett.REF_FILTER_2 = row[REF_FILTER_2];
                        if (table.Columns.Contains(REF_FILTER_3) && row[REF_FILTER_3] != DBNull.Value)
                            dataGett.REF_FILTER_3 = row[REF_FILTER_3];
                        if (table.Columns.Contains(REF_FILTER_4) && row[REF_FILTER_4] != DBNull.Value)
                            dataGett.REF_FILTER_4 = row[REF_FILTER_4];
                        if (table.Columns.Contains(REF_FILTER_5) && row[REF_FILTER_5] != DBNull.Value)
                            dataGett.REF_FILTER_5 = row[REF_FILTER_5];
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
        private void LoadDefault(List<DataGet> listData)
        {
            try
            {
                if (listData != null && listData.Count == 1 && this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    txtDepartmentCode.Text = listData.First().CODE;
                    cboDepartment.EditValue = listData.First().ID;
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadBranch(List<DataGet> listData)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.config.JSON_OUTPUT) && (this.config.JSON_OUTPUT.Contains("BRANCH_ID") || this.config.JSON_OUTPUT.Contains("BRANCH__ID")) && this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    var currentBranch = listData.FirstOrDefault(o => o.ID == FormTypeConfig.BranchId);
                    if (currentBranch != null)
                    {
                        txtDepartmentCode.Text = currentBranch.CODE;
                        cboDepartment.EditValue = currentBranch.ID;
                    }
                    else
                    {
                        txtDepartmentCode.Text = listData.First().CODE;
                        cboDepartment.EditValue = listData.First().ID;
                    }
                }
                else
                {
                    cboDepartment.Focus();
                    //cboDepartment.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void Init()
        {
            try
            {
                FormTypeConfig.ReportTypeCode = config.REPORT_TYPE_CODE;

                FilterConfig.GetValueOutput0(this.config.JSON_OUTPUT, ref Output0);
                JsonOutput = this.config.JSON_OUTPUT;
                FilterConfig.RemoveStrOutput0(ref JsonOutput);
                //this.config.JSON_OUTPUT = JsonOutput;

                FDO = FilterConfig.HisFilterTypes(this.config.JSON_OUTPUT);
                limitCodes = FilterConfig.GetLimitCodes(this.config.JSON_OUTPUT);
                cboDepartment.Properties.DataSource = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0);
                cboDepartment.Properties.DisplayMember = "NAME";
                var dataCombo = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0);
                var defaultValue = new DataGet();
                if (!string.IsNullOrWhiteSpace(Output0) && dataCombo != null)
                {
                    defaultValue = dataCombo.FirstOrDefault(o => o.CODE == Output0);
                }
                string[] Filters = null;
                FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);
                if (!(Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0])))
                {
                    cboDepartment.Properties.ValueMember = "ID";
                    if (defaultValue != null && defaultValue.ID > 0)
                    {
                        cboDepartment.EditValue = defaultValue.ID;
                        txtDepartmentCode.Text = defaultValue.CODE;
                    }
                }
                else
                {
                    cboDepartment.Properties.ValueMember = "CODE";
                    if (defaultValue != null)
                    {
                        cboDepartment.EditValue = defaultValue.CODE;
                        txtDepartmentCode.Text = defaultValue.CODE;
                    }
                }

                cboDepartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboDepartment.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboDepartment.Properties.ImmediatePopup = true;
                cboDepartment.ForceInitialize();
                cboDepartment.Properties.View.Columns.Clear();
                cboDepartment.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboDepartment.Properties.View.Columns.AddField("CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = false;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboDepartment.Properties.View.Columns.AddField("NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = false;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;


                LoadDefault(HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0));
                LoadBranch(HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0));

                aColumnCode.Visible = true;
                aColumnName.Visible = true;
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    Validation();
                    lciTitleName.AppearanceItemCaption.ForeColor = Color.Maroon;
                }

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                    lciTitleName.Text = this.config.DESCRIPTION ?? " ";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtDepartmentCode.Text.Trim()))
                    {
                        string code = txtDepartmentCode.Text.Trim().ToLower();
                        var listData = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0).Where(o => o.CODE.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.CODE.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtDepartmentCode.Text = result.First().CODE;

                            string[] Filters = null;
                            FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);
                            if (!(Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0])))
                            {
                                cboDepartment.EditValue = result.First().ID;
                            }
                            else
                                cboDepartment.EditValue = result.First().CODE;
                        }
                    }
                    if (showCbo)
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
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
                        var department = new DataGet();
                        var lst = cboDepartment.Properties.DataSource as List<DataGet>;
                        DataGet obj = null;
                        if (lst != null && lst.Count > 0 && cboDepartment.EditValue != null)
                        {
                            obj = lst.FirstOrDefault(o => cboDepartment.Properties.ValueMember == "ID" ? o.ID == Int64.Parse(cboDepartment.EditValue.ToString()) : o.CODE == cboDepartment.EditValue.ToString());
                        }
                        if (obj != null)
                        {
                            txtDepartmentCode.Text = obj.CODE;
                        }
                        if (this.config != null && this.config.IS_REQUIRE != IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            cboDepartment.Properties.Buttons[1].Visible = true;
                        }
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var department = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0).FirstOrDefault(f => f.ID == long.Parse(cboDepartment.EditValue.ToString()));
                        if (department != null)
                        {
                            txtDepartmentCode.Text = department.CODE;
                        }
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetValue()
        {
            string value = "";
            try
            {
                string[] Filters = null;
                FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);
                if (!(Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0])))
                {
                    long? departmentId = (long?)cboDepartment.EditValue;
                    value = String.Format(this.JsonOutput, ConvertUtils.ConvertToObjectFilter(departmentId));
                }
                else
                {
                    string departmentId = "\"" + (string)cboDepartment.EditValue + "\"";
                    if (departmentId == "\"\"") departmentId = "null";
                    value = String.Format(this.JsonOutput, ConvertUtils.ConvertToObjectFilter(departmentId));
                }
            }
            catch (Exception ex)
            {
                value = null;
                LogSystem.Warn(ex);
            }
            return value;
        }

        public void SetValue()
        {
            try
            {
                if (this.JsonOutput != null && this.report.JSON_FILTER != null)
                {

                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, this.JsonOutput, this.report.JSON_FILTER);

                    string[] Filters = null;
                    FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);

                    if (Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0]) && value != null && value != "null")
                    {
                        txtDepartmentCode.Text = value.Replace("\"", "");
                        cboDepartment.EditValue = (HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0).FirstOrDefault(f => f.CODE == txtDepartmentCode.Text) ?? new DataGet()).CODE;
                    }
                    else if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                    {
                        //cboDepartment.Properties.DataSource = HisMultiGetByString.GetByStringLimit(FDO, limitCodes);
                        txtDepartmentCode.Text = (HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0).FirstOrDefault(f => f.ID == Inventec.Common.TypeConvert.Parse.ToInt64(value)) ?? new DataGet()).CODE;
                        cboDepartment.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
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
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    this.positionHandleControl = -1;
                    result = dxValidationProvider1.Validate();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #region Validation
        private void ValidateDepartment()
        {
            try
            {
                HIS.UC.FormType.GridLookUpTextEdit.Validation.DepartmentValidationRule validRule = new HIS.UC.FormType.GridLookUpTextEdit.Validation.DepartmentValidationRule();
                validRule.cboDepartment = cboDepartment;
                validRule.txtDepartmentCode = txtDepartmentCode;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtDepartmentCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Validation()
        {
            try
            {
                ValidateDepartment();
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
        #endregion

        private void UCDepartmentCombo_Load(object sender, EventArgs e)
        {
            try
            {
                lciTitleName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_DEPARTMENT_COMBO_LCI_TITLE_NAME", Resources.ResourceLanguageManager.LanguageUCDepartmentCombo, Base.LanguageManager.GetCulture());

                SetTitle();
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

        //private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataGet data = null;
        //        if (cboDepartment.EditValue != null && cboDepartment.EditValue.GetType() == typeof(string))
        //        {
        //            data = HisMultiGetByString.GetByStringLimit(FDO, limitCodes).FirstOrDefault(o => o.CODE == cboDepartment.EditValue.ToString());
        //        }
        //        else
        //        {
        //            data = HisMultiGetByString.GetByStringLimit(FDO, limitCodes).FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? 0).ToString()));
        //        }
        //        if (data != null)
        //        {
        //            txtDepartmentCode.Text = data.CODE;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                    cboDepartment.Properties.Buttons[1].Visible = false;
                    txtDepartmentCode.Text = null;
                    txtDepartmentCode.Focus();
                    txtDepartmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var lst = cboDepartment.Properties.DataSource as List<DataGet>;
                DataGet obj = null;
                if (lst != null && lst.Count > 0 && cboDepartment.EditValue != null)
                {
                    obj = lst.FirstOrDefault(o => cboDepartment.Properties.ValueMember == "ID" ? o.ID == Int64.Parse(cboDepartment.EditValue.ToString()) : o.CODE == cboDepartment.EditValue.ToString());
                }
                if (delegateSelectDatas != null && PropetiesFilter != null && PropetiesFilter.ValueTransfer != null && PropetiesFilter.ValueTransfer.Length > 0 && PropetiesFilter.IdReference > 0)
                {
                    var arrGoc = PropetiesFilter.ValueTransfer.ToArray();
                    if (obj != null)
                    {
                        for (int i = 0; i < arrGoc.Length; i++)
                        {
                            var arrItem = arrGoc[i];
                            string propertyName = arrItem.ToString();
                            // Check if the property exists in the DataGet class
                            var propertyInfo = typeof(DataGet).GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                            if (propertyInfo != null)
                            {
                                // Select only the value of the matching property
                                var propertyValue = propertyInfo.GetValue(obj);

                                arrGoc = ReplaceValueMember(arrGoc, propertyName, propertyValue);

                                // Log or process the property values as needed
                                Inventec.Common.Logging.LogSystem.Info($"Values for property '{propertyName}': {propertyValue}");
                            }
                            else
                            {
                                // Nếu tên trường không có trong DataGet thì bỏ qua không xử lý
                                Inventec.Common.Logging.LogSystem.Warn($"Property '{propertyName}' does not exist in DataGet.");
                            }
                        }
                    }
                    else
                        arrGoc = null;

                    delegateSelectDatas(PropetiesFilter.IdReference, arrGoc);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public object[] ReplaceValueMember(object[] arr, string member, object value)
        {
            if (arr == null || arr.Length == 0 || string.IsNullOrEmpty(member))
                return arr;

            var result = arr.ToArray();
            for (int i = 0; i < result.Length; i++)
            {
                if (string.Equals(result[i].ToString().ToUpper(), member, StringComparison.OrdinalIgnoreCase))
                {
                    result[i] = value;
                }
            }
            return result;
        }
        public bool GetValueReceive(object data)
        {
            bool result = false;
            try
            {
                var dataSource = this.dataSource;
                if (data != null)
                {
                    var arrStr = data as object[];
                    if (arrStr != null && arrStr.Length > 0)
                    {
                        var filteredData = dataSource;
                        for (int i = 0; i < arrStr.Length; i++)
                        {
                            object filterValue = arrStr[i];
                            if (filterValue != null)
                            {
                                switch (i)
                                {
                                    case 0:
                                        filteredData = filteredData.Where(o => o.REF_FILTER_1 != null && o.REF_FILTER_1 == filterValue).ToList();
                                        break;
                                    case 1:
                                        filteredData = filteredData.Where(o => o.REF_FILTER_2 != null && o.REF_FILTER_2 == filterValue).ToList();
                                        break;
                                    case 2:
                                        filteredData = filteredData.Where(o => o.REF_FILTER_3 != null && o.REF_FILTER_3 == filterValue).ToList();
                                        break;
                                    case 3:
                                        filteredData = filteredData.Where(o => o.REF_FILTER_4 != null && o.REF_FILTER_4 == filterValue).ToList();
                                        break;
                                    case 4:
                                        filteredData = filteredData.Where(o => o.REF_FILTER_5 != null && o.REF_FILTER_5 == filterValue).ToList();
                                        break;
                                }
                            }
                        }
                        dataSource = filteredData;
                    }
                    result = true;
                }
                cboDepartment.Properties.DataSource = dataSource;
                if (cboDepartment.Properties.DataSource != null && (cboDepartment.Properties.DataSource as List<DataGet>).Count > 0)
                {
                    if (dataSource.Count == 1)
                    {
                        if (cboDepartment.Properties.ValueMember == "ID")
                        {
                            txtDepartmentCode.Text = dataSource.First().CODE;
                            cboDepartment.EditValue = dataSource.First().ID;
                        }
                        else
                        {
                            txtDepartmentCode.Text = dataSource.First().CODE;
                            cboDepartment.EditValue = dataSource.First().CODE;
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
    }
}
