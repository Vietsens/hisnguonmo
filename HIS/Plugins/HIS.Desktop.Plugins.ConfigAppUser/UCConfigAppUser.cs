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
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using SDA.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ConfigAppUser.ADO;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using ACS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ConfigAppUser
{
    public partial class UCConfigAppUser : HIS.Desktop.Utility.UserControlBase
    {
        int start = 0;
        SDA_CONFIG_APP currentData;
        SDA_CONFIG_APP_USER updateDTO = new SDA_CONFIG_APP_USER();
        List<SDA_CONFIG_APP_USER> currentConfigAppUser = new List<SDA_CONFIG_APP_USER>();
        internal List<HisConfigAppUserADO> _dataGrids { get; set; }
        Inventec.Desktop.Common.Modules.Module currentModule;
        Action delegateRefresh;
        string loginName;
        string workingModulelink = "";

        public UCConfigAppUser(Inventec.Desktop.Common.Modules.Module moduleData, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Action delegateRefresh, long numPageSize, string applicationCode)
            : base(moduleData)
        {
            InitializeComponent();
            moduleData.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__UC;
            this.currentModule = moduleData;
            this.delegateRefresh = delegateRefresh;
            ConfigApplications.NumPageSize = numPageSize;
            GlobalVariables.APPLICATION_CODE = applicationCode;
            ApiConsumers.SdaConsumer = sdaConsumer;
            //txtModuleLinks.Visible = false;
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public UCConfigAppUser( Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Action delegateRefresh, long numPageSize, string applicationCode, string _workingModuleLink)
            //: base(moduleData)
        {
            InitializeComponent();
            //this.currentModule = moduleData;
            this.delegateRefresh = delegateRefresh;
            ConfigApplications.NumPageSize = numPageSize;
            GlobalVariables.APPLICATION_CODE = applicationCode;
            ApiConsumers.SdaConsumer = sdaConsumer;
            this.workingModulelink = _workingModuleLink;
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UCConfigAppUser_Load(object sender, EventArgs e)
        {
            try
            {
                if (workingModulelink != null)
                    txtModuleLinks.Text = this.workingModulelink;
                FillDataToGridControl();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load Data GridControl
        /// </summary>
        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                gridControlConfigApplication.DataSource = null;

                List<string> glstModuleLink=new List<string>();
                

                _dataGrids = new List<HisConfigAppUserADO>();

                SDA.Filter.SdaConfigAppFilter configAppFilter = new SDA.Filter.SdaConfigAppFilter();
                configAppFilter.APP_CODE_ACCEPT = GlobalVariables.APPLICATION_CODE;
                configAppFilter.KEY_WORD = txtKeyWord.Text;
                if (!String.IsNullOrWhiteSpace(txtModuleLinks.Text))
                {
                    string[] textSplit = txtModuleLinks.Text.Split(new char[] { ','});
                    foreach (string moduleLink in textSplit)
                        glstModuleLink.Add(moduleLink);
                    configAppFilter.MODULE_LINKSs = glstModuleLink;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => configAppFilter), configAppFilter));
                var _currentConfigApps = new BackendAdapter(param).Get<List<SDA_CONFIG_APP>>("api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, configAppFilter, param);

                if (_currentConfigApps != null && _currentConfigApps.Count > 0)
                {
                    //if (!String.IsNullOrWhiteSpace(txtModuleLinks.Text))
                    //{
                    //    _currentConfigApps = _currentConfigApps
                    //       .Where(o => !String.IsNullOrWhiteSpace(o.MODULE_LINKS)
                    //           && o.MODULE_LINKS.ToLower().Contains(txtModuleLinks.Text.Trim().ToLower())
                    //       ).OrderByDescending(o => o.MODIFY_TIME).ToList();
                    //}
                    _dataGrids = (from n in _currentConfigApps select new HisConfigAppUserADO(n)).ToList();
                }

                SDA.Filter.SdaConfigAppUserFilter appUserFilter = new SDA.Filter.SdaConfigAppUserFilter();
                appUserFilter.LOGINNAME = loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                currentConfigAppUser = new BackendAdapter(param).Get<List<SDA_CONFIG_APP_USER>>("api/SdaConfigAppUser/Get", ApiConsumers.SdaConsumer, appUserFilter, param);
                if (currentConfigAppUser != null && currentConfigAppUser.Count > 0)
                {
                    foreach (var item in _dataGrids)
                    {
                        SDA_CONFIG_APP_USER ado = currentConfigAppUser.FirstOrDefault(p => p.CONFIG_APP_ID == item.ID);
                        if (ado != null)
                        {
                            item.LOGINNAME = ado.LOGINNAME;
                            item.VALUE = ado.VALUE;
                        }
                    }
                }

                gridControlConfigApplication.DataSource = _dataGrids;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewConfigApplication_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SDA_CONFIG_APP data = (SDA_CONFIG_APP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + start;
                            }
                            catch (Exception ex)
                            {

                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri STT", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //xử lý khi thay đổi giá trị
        private void gridViewConfigApplication_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                currentData = (SDA_CONFIG_APP)gridViewConfigApplication.GetFocusedRow();
                var ListConfigAppUser = currentConfigAppUser.Where(o => o.CONFIG_APP_ID == currentData.ID).ToList();

                updateDTO.VALUE = gridViewConfigApplication.FocusedValue.ToString();
                if (updateDTO.VALUE != null && updateDTO.VALUE != "")
                {
                    if (!CheckValue())
                        return;
                    List<SDA_CONFIG_APP_USER> configAppUsers = new List<SDA_CONFIG_APP_USER>();
                    SDA_CONFIG_APP_USER configAppUser = new SDA_CONFIG_APP_USER();
                    configAppUser.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    configAppUser.VALUE = updateDTO.VALUE;
                    configAppUser.CONFIG_APP_ID = currentData.ID;
                    if (ListConfigAppUser != null && ListConfigAppUser.Count > 0)
                    {
                        configAppUser.ID = ListConfigAppUser.FirstOrDefault().ID;
                    }
                    configAppUsers.Add(configAppUser);
                    if (ListConfigAppUser.Count == 0 || ListConfigAppUser == null)
                    {
                        var createResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                                            "api/SdaConfigAppUser/CreateList",
                                            ApiConsumers.SdaConsumer,
                                            configAppUsers,
                                            param);
                        if (createResult != null)
                        {
                            success = true;
                            FillDataToGridControl();
                        }
                    }
                    else
                    {
                        var createResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                            "api/SdaConfigAppUser/UpdateList", ApiConsumers.SdaConsumer, configAppUsers, param);
                        if (createResult != null)
                        {
                            success = true;
                        }
                    }
                }
                else
                {
                    List<long> deleteIds = ListConfigAppUser.Select(o => o.ID).ToList();
                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                              "api/SdaConfigAppUser/DeleteList",
                              ApiConsumers.SdaConsumer,
                              deleteIds,
                              param);
                    if (deleteResult)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                MessageManager.Show(this.ParentForm, param, success);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //vaildate giá trị
        private bool CheckValue()
        {
            bool result = true;
            try
            {
                string valueType = currentData.VALUE_TYPE != null ? currentData.VALUE_TYPE.ToString() : "";

                string strValueAllowMin = currentData.VALUE_ALLOW_MIN != null ? currentData.VALUE_ALLOW_MIN.ToString() : "";
                string strValueAllowMax = currentData.VALUE_ALLOW_MAX != null ? currentData.VALUE_ALLOW_MAX.ToString() : "";
                string strValueAllowIn = currentData.VALUE_ALLOW_IN != null ? currentData.VALUE_ALLOW_IN.ToString() : "";
                switch (valueType)
                {
                    case "long":
                        long lValue = Convert.ToInt64(updateDTO.VALUE);
                        if (!String.IsNullOrEmpty(strValueAllowMin))
                        {
                            long valueMin = Convert.ToInt64(strValueAllowMin);
                            if (lValue < valueMin)
                            {
                                result = false;
                                //e.Valid = false;
                                //e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép", "Thông báo");
                                return result;
                            }
                        }
                        if (!String.IsNullOrEmpty(strValueAllowMax))
                        {
                            long valueMax = Convert.ToInt64(strValueAllowMax);
                            if (lValue > valueMax)
                            {
                                result = false;
                                //e.Valid = false;
                                //if (!String.IsNullOrEmpty(e.ErrorText))
                                //    e.ErrorText += "\r\n";
                                //e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào vượt quá giá trị lớn nhất cho phép", "Thông báo");
                                return result;
                            }
                        }
                        if (!String.IsNullOrEmpty(strValueAllowIn))
                        {
                            if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                strValueAllowIn = "," + strValueAllowIn;
                            if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                strValueAllowIn = strValueAllowIn + ",";
                            if (!strValueAllowIn.Contains("," + updateDTO.VALUE.ToString() + ",") && !strValueAllowIn.Contains(";" + updateDTO.VALUE.ToString() + ";"))
                            {
                                result = false;
                                //e.Valid = false;
                                //if (!String.IsNullOrEmpty(e.ErrorText))
                                //    e.ErrorText += "\r\n";
                                //e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào không nằm trong danh sách các giá trị cho phép", "Thông báo");
                                return result;
                            }
                        }
                        break;

                    case "int":
                        long IValue = Convert.ToInt64(updateDTO.VALUE);
                        if (!String.IsNullOrEmpty(strValueAllowMin))
                        {
                            long valueMin = Convert.ToInt64(strValueAllowMin);
                            if (IValue < valueMin)
                            {
                                result = false;
                                //e.Valid = false;
                                //e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép", "Thông báo");
                                return result;
                            }
                        }
                        if (!String.IsNullOrEmpty(strValueAllowMax))
                        {
                            long valueMax = Convert.ToInt64(strValueAllowMax);
                            if (IValue > valueMax)
                            {
                                result = false;
                                //e.Valid = false;
                                //if (!String.IsNullOrEmpty(e.ErrorText))
                                //    e.ErrorText += "\r\n";
                                //e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào vượt quá giá trị lớn nhất cho phép", "Thông báo");
                                return result;
                            }
                        }
                        if (!String.IsNullOrEmpty(strValueAllowIn))
                        {
                            if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                strValueAllowIn = "," + strValueAllowIn;
                            if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                strValueAllowIn = strValueAllowIn + ",";
                            if (!strValueAllowIn.Contains("," + updateDTO.VALUE.ToString() + ",") && !strValueAllowIn.Contains(";" + updateDTO.VALUE.ToString() + ";"))
                            {
                                result = false;
                                //e.Valid = false;
                                //if (!String.IsNullOrEmpty(e.ErrorText))
                                //    e.ErrorText += "\r\n";
                                //e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào không nằm trong danh sách các giá trị cho phép", "Thông báo");
                                return result;
                            }
                        }
                        break;

                    case "string":
                        if (!String.IsNullOrEmpty(strValueAllowIn))
                        {
                            if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                strValueAllowIn = "," + strValueAllowIn;
                            if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                strValueAllowIn = strValueAllowIn + ",";
                            if (!strValueAllowIn.Contains("," + updateDTO.VALUE.ToString() + ",") && !strValueAllowIn.Contains(";" + updateDTO.VALUE.ToString() + ";"))
                            {
                                result = false;
                                //e.Valid = false;
                                //if (!String.IsNullOrEmpty(e.ErrorText))
                                //    e.ErrorText += "\r\n";
                                //e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào không nằm trong danh sách các giá trị cho phép", "Thông báo");
                                return result;
                            }
                        }
                        break;
                    case "short":
                        short shValue = Convert.ToInt16(updateDTO.VALUE);
                        if (!String.IsNullOrEmpty(strValueAllowMin))
                        {
                            long valueMin = Convert.ToInt16(strValueAllowMin);
                            if (shValue < valueMin)
                            {
                                result = false;
                                //e.Valid = false;
                                //e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép", "Thông báo");
                                return result;
                            }
                        }
                        if (!String.IsNullOrEmpty(strValueAllowMax))
                        {
                            long valueMax = Convert.ToInt16(strValueAllowMax);
                            if (shValue > valueMax)
                            {
                                result = false;
                                //e.Valid = false;
                                //if (!String.IsNullOrEmpty(e.ErrorText))
                                //    e.ErrorText += "\r\n";
                                //e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào vượt quá giá trị lớn nhất cho phép", "Thông báo");
                                return result;
                            }
                        }
                        if (!String.IsNullOrEmpty(strValueAllowIn))
                        {
                            if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                strValueAllowIn = "," + strValueAllowIn;
                            if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                strValueAllowIn = strValueAllowIn + ",";
                            if (!strValueAllowIn.Contains("," + updateDTO.VALUE.ToString() + ",") && !strValueAllowIn.Contains(";" + updateDTO.VALUE.ToString() + ";"))
                            {
                                result = false;
                                //e.Valid = false;
                                //if (!String.IsNullOrEmpty(e.ErrorText))
                                //    e.ErrorText += "\r\n";
                                //e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào không nằm trong danh sách các giá trị cho phép", "Thông báo");
                                return result;
                            }
                        }
                        break;
                    case "decimal":
                        decimal decValue = Convert.ToDecimal(updateDTO.VALUE);
                        if (!String.IsNullOrEmpty(strValueAllowMin))
                        {
                            decimal valueMin = Convert.ToDecimal(strValueAllowMin);
                            if (decValue < valueMin)
                            {
                                result = false;
                                //e.Valid = false;
                                //e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép", "Thông báo");
                                return result;
                            }
                        }
                        if (!String.IsNullOrEmpty(strValueAllowMax))
                        {
                            decimal valueMax = Convert.ToDecimal(strValueAllowMax);
                            if (decValue > valueMax)
                            {
                                result = false;
                                //e.Valid = false;
                                //if (!String.IsNullOrEmpty(e.ErrorText))
                                //    e.ErrorText += "\r\n";
                                //e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào vượt quá giá trị lớn nhất cho phép", "Thông báo");
                                return result;
                            }
                        }
                        if (!String.IsNullOrEmpty(strValueAllowIn))
                        {
                            if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                strValueAllowIn = "," + strValueAllowIn;
                            if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                strValueAllowIn = strValueAllowIn + ",";
                            if (!strValueAllowIn.Contains("," + updateDTO.VALUE.ToString() + ",") && !strValueAllowIn.Contains(";" + updateDTO.VALUE.ToString() + ";"))
                            {
                                result = false;
                                //e.Valid = false;
                                //if (!String.IsNullOrEmpty(e.ErrorText))
                                //    e.ErrorText += "\r\n";
                                //e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào không nằm trong danh sách các giá trị cho phép", "Thông báo");
                                return result;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = false;
                //e.Valid = false;
                //e.ErrorText += "Giá trị nhập vào không đúng định dạng";
                DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào không đúng định dạng", "Thông báo");
                return result;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                //lấy danh sách cấu hình
                var listConfigs = _dataGrids as List<HisConfigAppUserADO>;
                if (listConfigs != null && listConfigs.Count > 0)
                {
                    List<SDA_CONFIG_APP_USER> configAppUserCreats = new List<SDA_CONFIG_APP_USER>();
                    List<SDA_CONFIG_APP_USER> configAppUserUpdates = new List<SDA_CONFIG_APP_USER>();
                    List<SDA_CONFIG_APP_USER> configAppUserDeletes = new List<SDA_CONFIG_APP_USER>();
                    List<long> deleteIds = new List<long>();
                    foreach (var item in listConfigs)
                    {
                        var currentConfigAppUsers = currentConfigAppUser.Where(o => o.CONFIG_APP_ID == item.ID).ToList();
                        if (item.VALUE != null && item.VALUE != "")
                        {
                            if (currentConfigAppUsers == null || currentConfigAppUsers.Count == 0)
                            {
                                SDA_CONFIG_APP_USER configAppUserCreat = new SDA_CONFIG_APP_USER();
                                configAppUserCreat.LOGINNAME = loginName;
                                configAppUserCreat.VALUE = item.VALUE;
                                configAppUserCreat.CONFIG_APP_ID = item.ID;
                                configAppUserCreats.Add(configAppUserCreat);
                            }
                            else
                            {
                                SDA_CONFIG_APP_USER configAppUserUpdate = new SDA_CONFIG_APP_USER();
                                configAppUserUpdate.LOGINNAME = loginName;
                                configAppUserUpdate.VALUE = item.VALUE;
                                configAppUserUpdate.CONFIG_APP_ID = item.ID;
                                configAppUserUpdate.ID = currentConfigAppUsers.FirstOrDefault().ID;
                                configAppUserUpdates.Add(configAppUserUpdate);
                            }
                        }
                        else
                        {
                            if (currentConfigAppUsers != null && currentConfigAppUsers.Count > 0)
                            {
                                SDA_CONFIG_APP_USER configAppUserDelete = new SDA_CONFIG_APP_USER();
                                configAppUserDelete.ID = currentConfigAppUsers.FirstOrDefault().ID;
                                configAppUserDeletes.Add(configAppUserDelete);
                            }
                        }
                    }
                    if (configAppUserCreats != null && configAppUserCreats.Count > 0)
                    {
                        var createResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                                                                   "api/SdaConfigAppUser/CreateList",
                                                                   ApiConsumers.SdaConsumer,
                                                                   configAppUserCreats,
                                                                   param);
                        if (createResult != null)
                        {
                            success = true;
                        }
                    }
                    if (configAppUserUpdates != null && configAppUserUpdates.Count > 0)
                    {
                        var createResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                            "api/SdaConfigAppUser/UpdateList", ApiConsumers.SdaConsumer, configAppUserUpdates, param);
                        if (createResult != null)
                        {
                            success = true;
                        }
                    }
                    if (configAppUserDeletes.Count > 0 && configAppUserDeletes != null)
                    {
                        deleteIds = configAppUserDeletes.Select(o => o.ID).ToList();
                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                              "api/SdaConfigAppUser/DeleteList",
                              ApiConsumers.SdaConsumer,
                              deleteIds,
                              param);
                        if (deleteResult)
                        {
                            success = true;
                        }
                    }
                }
                MessageManager.Show(this.ParentForm, param, success);

                if (success == true)
                {
                    FillDataToGridControl();
                    if (this.delegateRefresh != null)
                        this.delegateRefresh();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //vaildate giá trị
        private void gridViewConfigApplication_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                currentData = (SDA_CONFIG_APP)gridViewConfigApplication.GetFocusedRow();
                if (view.FocusedColumn.FieldName == "VALUE")
                {
                    string valueType = currentData.VALUE_TYPE != null ? currentData.VALUE_TYPE.ToString() : "";
                    string strValueAllowMin = currentData.VALUE_ALLOW_MIN != null ? currentData.VALUE_ALLOW_MIN.ToString() : "";
                    string strValueAllowMax = currentData.VALUE_ALLOW_MAX != null ? currentData.VALUE_ALLOW_MAX.ToString() : "";
                    string strValueAllowIn = currentData.VALUE_ALLOW_IN != null ? currentData.VALUE_ALLOW_IN.ToString() : "";
                    if (e.Value != null && e.Value.ToString().Trim() != "")
                    {
                        switch (valueType)
                        {
                            case "long":
                                long lValue = Convert.ToInt64(e.Value);
                                if (!String.IsNullOrEmpty(strValueAllowMin))
                                {
                                    long valueMin = Convert.ToInt64(strValueAllowMin);
                                    if (lValue < valueMin)
                                    {
                                        e.Valid = false;
                                        e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                        return;
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowMax))
                                {
                                    long valueMax = Convert.ToInt64(strValueAllowMax);
                                    if (lValue > valueMax)
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                        return;
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowIn))
                                {
                                    if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                        strValueAllowIn = "," + strValueAllowIn;
                                    if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                        strValueAllowIn = strValueAllowIn + ",";
                                    if (!strValueAllowIn.Contains("," + e.Value.ToString() + ",") && !strValueAllowIn.Contains(";" + e.Value.ToString() + ";"))
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                        return;
                                    }
                                }
                                break;

                            case "int":
                                long IValue = Convert.ToInt64(e.Value);
                                if (!String.IsNullOrEmpty(strValueAllowMin))
                                {
                                    long valueMin = Convert.ToInt64(strValueAllowMin);
                                    if (IValue < valueMin)
                                    {
                                        //result = false;
                                        e.Valid = false;
                                        e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                        return;
                                        // DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép", "Thông báo");
                                        //return result;
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowMax))
                                {
                                    long valueMax = Convert.ToInt64(strValueAllowMax);
                                    if (IValue > valueMax)
                                    {
                                        //result = false;
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                        return;
                                        //DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào vượt quá giá trị lớn nhất cho phép", "Thông báo");
                                        //return result;
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowIn))
                                {
                                    if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                        strValueAllowIn = "," + strValueAllowIn;
                                    if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                        strValueAllowIn = strValueAllowIn + ",";
                                    if (!strValueAllowIn.Contains("," + e.Value.ToString() + ",") && !strValueAllowIn.Contains(";" + e.Value.ToString() + ";"))
                                    {
                                        //result = false;
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                        return;
                                        //DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị nhập vào không nằm trong danh sách các giá trị cho phép", "Thông báo");
                                        //return result;
                                    }
                                }
                                break;

                            case "string":
                                if (!String.IsNullOrEmpty(strValueAllowIn))
                                {
                                    if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                        strValueAllowIn = "," + strValueAllowIn;
                                    if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                        strValueAllowIn = strValueAllowIn + ",";
                                    if (!strValueAllowIn.Contains("," + e.Value.ToString() + ",") && !strValueAllowIn.Contains(";" + e.Value.ToString() + ";"))
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                        return;
                                    }
                                }
                                break;
                            case "short":
                                short shValue = Convert.ToInt16(e.Value);
                                if (!String.IsNullOrEmpty(strValueAllowMin))
                                {
                                    long valueMin = Convert.ToInt16(strValueAllowMin);
                                    if (shValue < valueMin)
                                    {
                                        e.Valid = false;
                                        e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                        return;
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowMax))
                                {
                                    long valueMax = Convert.ToInt16(strValueAllowMax);
                                    if (shValue > valueMax)
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                        return;
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowIn))
                                {
                                    if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                        strValueAllowIn = "," + strValueAllowIn;
                                    if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                        strValueAllowIn = strValueAllowIn + ",";
                                    if (!strValueAllowIn.Contains("," + e.Value.ToString() + ",") && !strValueAllowIn.Contains(";" + e.Value.ToString() + ";"))
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                        return;
                                    }
                                }
                                break;
                            case "decimal":
                                decimal decValue = Convert.ToDecimal(e.Value);
                                if (!String.IsNullOrEmpty(strValueAllowMin))
                                {
                                    decimal valueMin = Convert.ToDecimal(strValueAllowMin);
                                    if (decValue < valueMin)
                                    {
                                        e.Valid = false;
                                        e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                        return;
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowMax))
                                {
                                    decimal valueMax = Convert.ToDecimal(strValueAllowMax);
                                    if (decValue > valueMax)
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                        return;
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowIn))
                                {
                                    if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                        strValueAllowIn = "," + strValueAllowIn;
                                    if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                        strValueAllowIn = strValueAllowIn + ",";
                                    if (!strValueAllowIn.Contains("," + e.Value.ToString() + ",") && !strValueAllowIn.Contains(";" + e.Value.ToString() + ";"))
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                        return;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                e.Valid = false;
                e.ErrorText += "Giá trị nhập vào không đúng định dạng";
                return;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewConfigApplication_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                //Do not perform any default action 
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.DisplayError;
                //Show the message with the error text specified 
                // MessageBox.Show(e.ErrorText);
                return;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGridControl();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //restore 1 dòng
        private void repositoryItemButtonEdit_Restore_Click(object sender, EventArgs e)
        {
            try
            {

                bool success = false;
                CommonParam param = new CommonParam();
                currentData = (SDA_CONFIG_APP)gridViewConfigApplication.GetFocusedRow();
                if (currentData != null)
                {
                    WaitingManager.Show();
                    var RestoreConfigAppUsers = currentConfigAppUser.Where(o => o.CONFIG_APP_ID == currentData.ID).ToList();
                    if (currentData.DEFAULT_VALUE != null && currentData.DEFAULT_VALUE != "")
                    {
                        List<SDA_CONFIG_APP_USER> ListconfigAppUserRestore = new List<SDA_CONFIG_APP_USER>();
                        SDA_CONFIG_APP_USER configAppUserRestore = new SDA_CONFIG_APP_USER();
                        configAppUserRestore.LOGINNAME = loginName;
                        configAppUserRestore.VALUE = currentData.DEFAULT_VALUE;
                        configAppUserRestore.CONFIG_APP_ID = currentData.ID;
                        if (RestoreConfigAppUsers.Count > 0 && RestoreConfigAppUsers != null)
                        {
                            configAppUserRestore.ID = RestoreConfigAppUsers.FirstOrDefault().ID;
                        }
                        ListconfigAppUserRestore.Add(configAppUserRestore);
                        if (RestoreConfigAppUsers.Count > 0 && RestoreConfigAppUsers != null)
                        {
                            var restoreResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                             "api/SdaConfigAppUser/UpdateList", ApiConsumers.SdaConsumer, ListconfigAppUserRestore, param);
                            if (restoreResult != null)
                            {
                                success = true;
                            }
                        }
                        else
                        {
                            var restoreResultCreate = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                                                   "api/SdaConfigAppUser/CreateList", ApiConsumers.SdaConsumer, ListconfigAppUserRestore, param);
                            if (restoreResultCreate != null)
                            {
                                success = true;
                            }
                        }
                    }
                    else
                    {
                        List<long> deleteIds = new List<long>();
                        List<SDA_CONFIG_APP_USER> configAppUserDeletes = new List<SDA_CONFIG_APP_USER>();
                        if (RestoreConfigAppUsers != null && RestoreConfigAppUsers.Count > 0)
                        {
                            SDA_CONFIG_APP_USER configAppUserDelete = new SDA_CONFIG_APP_USER();
                            configAppUserDelete.ID = RestoreConfigAppUsers.FirstOrDefault().ID;
                            configAppUserDeletes.Add(configAppUserDelete);

                            if (configAppUserDeletes.Count > 0 && configAppUserDeletes != null)
                            {
                                deleteIds = configAppUserDeletes.Select(o => o.ID).ToList();
                                bool deleteResult = new BackendAdapter(param).Post<bool>(
                                      "api/SdaConfigAppUser/DeleteList",
                                      ApiConsumers.SdaConsumer,
                                      deleteIds,
                                      param);
                                if (deleteResult)
                                {
                                    success = true;
                                }
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            return;
                        }
                    }


                }
                MessageManager.Show(this.ParentForm, param, success);
                if (success == true)
                {
                    FillDataToGridControl();
                    if (this.delegateRefresh != null)
                        this.delegateRefresh();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                //lấy danh sách cấu hình
                var listConfigs = _dataGrids as List<HisConfigAppUserADO>;
                if (listConfigs != null && listConfigs.Count > 0)
                {
                    List<SDA_CONFIG_APP_USER> configAppUserCreats = new List<SDA_CONFIG_APP_USER>();
                    List<SDA_CONFIG_APP_USER> configAppUserUpdates = new List<SDA_CONFIG_APP_USER>();
                    List<SDA_CONFIG_APP_USER> configAppUserDeletes = new List<SDA_CONFIG_APP_USER>();
                    List<long> deleteIds = new List<long>();
                    foreach (var item in listConfigs)
                    {
                        var currentConfigAppUsers = currentConfigAppUser.Where(o => o.CONFIG_APP_ID == item.ID).ToList();
                        if (item.DEFAULT_VALUE != null && item.DEFAULT_VALUE != "")
                        {
                            if (currentConfigAppUsers == null || currentConfigAppUsers.Count == 0)
                            {
                                SDA_CONFIG_APP_USER configAppUserCreat = new SDA_CONFIG_APP_USER();
                                configAppUserCreat.LOGINNAME = loginName;
                                configAppUserCreat.VALUE = item.DEFAULT_VALUE;
                                configAppUserCreat.CONFIG_APP_ID = item.ID;
                                configAppUserCreats.Add(configAppUserCreat);
                            }
                            else
                            {
                                SDA_CONFIG_APP_USER configAppUserUpdate = new SDA_CONFIG_APP_USER();
                                configAppUserUpdate.LOGINNAME = loginName;
                                configAppUserUpdate.VALUE = item.DEFAULT_VALUE;
                                configAppUserUpdate.CONFIG_APP_ID = item.ID;
                                configAppUserUpdate.ID = currentConfigAppUsers.FirstOrDefault().ID;
                                configAppUserUpdates.Add(configAppUserUpdate);
                            }
                        }
                        else
                        {
                            if (currentConfigAppUsers != null && currentConfigAppUsers.Count > 0)
                            {
                                SDA_CONFIG_APP_USER configAppUserDelete = new SDA_CONFIG_APP_USER();
                                configAppUserDelete.ID = currentConfigAppUsers.FirstOrDefault().ID;
                                configAppUserDeletes.Add(configAppUserDelete);
                            }
                        }
                    }
                    if (configAppUserCreats != null && configAppUserCreats.Count > 0)
                    {
                        var createResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                                                                   "api/SdaConfigAppUser/CreateList",
                                                                   ApiConsumers.SdaConsumer,
                                                                   configAppUserCreats,
                                                                   param);
                        if (createResult != null)
                        {
                            success = true;
                        }
                    }

                    if (configAppUserUpdates != null && configAppUserUpdates.Count > 0)
                    {
                        var updateResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                            "api/SdaConfigAppUser/UpdateList", ApiConsumers.SdaConsumer, configAppUserUpdates, param);
                        if (updateResult != null)
                        {
                            success = true;
                        }
                    }

                    if (configAppUserDeletes.Count > 0 && configAppUserDeletes != null)
                    {
                        deleteIds = configAppUserDeletes.Select(o => o.ID).ToList();
                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                              "api/SdaConfigAppUser/DeleteList",
                              ApiConsumers.SdaConsumer,
                              deleteIds,
                              param);
                        if (deleteResult)
                        {
                            success = true;
                        }
                    }
                }
                MessageManager.Show(this.ParentForm, param, success);

                FillDataToGridControl();
                if (this.delegateRefresh != null)
                    this.delegateRefresh();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void RestoreShortcut()
        {
            try
            {
                if (!gridViewConfigApplication.PostEditor())
                {
                    return;
                }
                btnRestore_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut()
        {
            try
            {
                if (!gridViewConfigApplication.PostEditor())
                {
                    return;
                }
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveShortcut()
        {
            try
            {
                btnSave.Focus();
                if (!gridViewConfigApplication.PostEditor())
                {
                    return;
                }
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtModuleLinks_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
