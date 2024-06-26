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
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using SDA.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using SDA.Filter;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using System.Resources;
using SDA.Desktop.Plugins.SdaExecuteSql.UC;
using MOS.SDO;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;

namespace SDA.Desktop.Plugins.SdaExecuteSql.SdaExecuteSql
{
    public partial class frmSdaExecuteSql : FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module currentModule;
        long idSqlSelect = 0;

        bool checkThrough = true;
        List<SDA_SQL_PARAM> lstSqlParam = new List<SDA_SQL_PARAM>();
        List<SDA_SQL> lstSdaSqlData = new List<SDA_SQL>();
        //const string moduleLink = "HIS.Desktop.Plugins.ServiceReqList";
        #endregion

        #region Construct
        public frmSdaExecuteSql(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                // this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                // this.gridControlServiceReq.ToolTipController = this.tooltipServiceRequest;

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("SDA.Desktop.Plugins.SdaExecuteSql.Resources.Lang", typeof(SDA.Desktop.Plugins.SdaExecuteSql.SdaExecuteSql.frmSdaExecuteSql).Assembly);

                this.currentModule = module;
                this.Text = module.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        #endregion

        private void frmSdaExecuteSql_Load(object sender, EventArgs e)
        {
            try
            {
                LoadComboRequired();
                btnExecute.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboRequired()
        {
            try
            {
                CommonParam param = new CommonParam();
                SdaSqlFilter filter = new SdaSqlFilter();
                filter.IS_ACTIVE = 1;
                lstSdaSqlData = new BackendAdapter(param).Get<List<SDA_SQL>>("api/SdaSql/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer, filter, param);
                if (lstSdaSqlData != null && lstSdaSqlData.Count > 0)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("SQL_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("SQL_NAME", "", 200, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("SQL_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(cboRequired, lstSdaSqlData, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRequired_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                xtraScrollableControl1.Controls.Clear();
                if (cboRequired.EditValue != null)
                {
                    xtraScrollableControl1.Controls.Clear();
                    btnExecute.Enabled = true;
                    idSqlSelect = Inventec.Common.TypeConvert.Parse.ToInt64(cboRequired.EditValue.ToString());
                    DynamicGenScreen(idSqlSelect);
                }
                else
                {
                    btnExecute.Enabled = false;
                    idSqlSelect = 0;
                    labelDescription.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DynamicGenScreen(long sqlId)
        {
            try
            {
                CommonParam param = new CommonParam();
                SdaSqlParamFilter filter = new SdaSqlParamFilter();
                filter.IS_ACTIVE = 1;
                filter.SQL_ID = Inventec.Common.TypeConvert.Parse.ToInt64(sqlId.ToString());
                var data = new BackendAdapter(param).Get<List<SDA_SQL_PARAM>>("api/SdaSqlParam/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer, filter, param);
                if (data != null && data.Count > 0)
                {
                    lstSqlParam = data;
                    foreach (var item in data)
                    {
                        UserControl dynamicUC = new UserControl();
                        switch (item.SQL_PARAM_TYPE)
                        {
                            case 1:
                                dynamicUC = new UCText(item);
                                break;
                            case 2:
                                dynamicUC = new UCTextNumberOnly(item);
                                break;
                            case 3:
                                dynamicUC = new UCDateTimeWithHour(item);
                                break;
                            case 4:
                                dynamicUC = new UCDateNoHour(item);
                                break;
                        }
                        xtraScrollableControl1.Controls.Add(dynamicUC);
                        dynamicUC.Dock = DockStyle.Top;
                        labelDescription.Text = lstSdaSqlData.Where(o => o.ID == sqlId).FirstOrDefault().DESCRIPTION ?? "";
                    }

                    //this.Size = new System.Drawing.Size(this.Size.Width, data.Count * 30 + 120);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnExecute.Enabled == false) return;
                ExecuteSqlSDO sdo = GetValueInput();
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                if (checkThrough == false)
                    return;

                var lstData = lstSdaSqlData.FirstOrDefault(o => o.ID == idSqlSelect);
                if (lstData == null)
                    return;


                if (sdo == null)
                {
                    return;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstData), lstData));

                CommonParam param = new CommonParam();
                bool success = false;
                switch (lstData.SCHEMA_CODE)
                {
                    case "HIS_RS":
                        {
                            success = new BackendAdapter(param).Post<bool>
("api/SqlExecute/Run", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            break;
                        }
                    case "SDA_RS":
                        {
                            success = new BackendAdapter(param).Post<bool>
("api/SqlExecute/Run", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer, sdo, param);
                            break;
                        }
                    case "ACS_RS":
                        {
                            success = new BackendAdapter(param).Post<bool>
("api/SqlExecute/Run", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, sdo, param);
                            break;
                        }
                }
                MessageManager.Show(this, param, success);
                // idSqlSelect = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private ExecuteSqlSDO GetValueInput()
        {
            try
            {
                if (lstSqlParam != null && lstSqlParam.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    ExecuteSqlSDO sdo = new ExecuteSqlSDO();
                    sdo.SqlParams = new List<SqlParamSDO>();
                    sdo.SqlId = Inventec.Common.TypeConvert.Parse.ToInt64(cboRequired.EditValue.ToString());
                    bool valid = true;
                    foreach (Control item in xtraScrollableControl1.Controls)
                    {

                        if (item.GetType() == typeof(UCText))
                        {
                            var uc = (UCText)item;
                            SqlParamSDO sdoSqlParams = new SqlParamSDO();
                            valid = uc.validate() && valid;

                        }
                        else if (item.GetType() == typeof(UCTextNumberOnly))
                        {
                            var uc = (UCTextNumberOnly)item;
                            SqlParamSDO sdoSqlParams = new SqlParamSDO();
                            valid = uc.validate() && valid;

                        }
                        else if (item.GetType() == typeof(UCDateTimeWithHour))
                        {
                            var uc = (UCDateTimeWithHour)item;
                            SqlParamSDO sdoSqlParams = new SqlParamSDO();
                            valid = uc.validate() && valid;

                        }
                        else if (item.GetType() == typeof(UCDateNoHour))
                        {
                            var uc = (UCDateNoHour)item;
                            SqlParamSDO sdoSqlParams = new SqlParamSDO();
                            valid = uc.validate() && valid;
                        }
                    }
                    if (valid)
                    {
                        foreach (Control item in xtraScrollableControl1.Controls)
                        {
                            if (item.GetType() == typeof(UCText))
                            {
                                var uc = (UCText)item;
                                SqlParamSDO sdoSqlParams = new SqlParamSDO();
                                var data = uc.GetValue();
                                if (data != null)
                                {
                                    sdoSqlParams.SqlParamId = data.ID;
                                    sdoSqlParams.Value = data.value;
                                    sdo.SqlParams.Add(sdoSqlParams);
                                }
                            }
                            else if (item.GetType() == typeof(UCTextNumberOnly))
                            {
                                var uc = (UCTextNumberOnly)item;
                                SqlParamSDO sdoSqlParams = new SqlParamSDO();

                                var data = uc.GetValue();
                                if (data != null)
                                {
                                    sdoSqlParams.SqlParamId = data.ID;
                                    sdoSqlParams.Value = data.value;
                                    sdo.SqlParams.Add(sdoSqlParams);
                                }
                            }
                            else if (item.GetType() == typeof(UCDateTimeWithHour))
                            {
                                var uc = (UCDateTimeWithHour)item;
                                SqlParamSDO sdoSqlParams = new SqlParamSDO();
                                var data = uc.GetValue();
                                if (data != null)
                                {
                                    sdoSqlParams.SqlParamId = data.ID;
                                    sdoSqlParams.Value = data.value;
                                    sdo.SqlParams.Add(sdoSqlParams);
                                }
                            }
                            else if (item.GetType() == typeof(UCDateNoHour))
                            {
                                var uc = (UCDateNoHour)item;
                                SqlParamSDO sdoSqlParams = new SqlParamSDO();
                                var data = uc.GetValue();
                                if (data != null)
                                {
                                    sdoSqlParams.SqlParamId = data.ID;
                                    sdoSqlParams.Value = data.value;
                                    sdo.SqlParams.Add(sdoSqlParams);
                                }
                            }
                        }
                        return sdo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return null;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnExecute_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
