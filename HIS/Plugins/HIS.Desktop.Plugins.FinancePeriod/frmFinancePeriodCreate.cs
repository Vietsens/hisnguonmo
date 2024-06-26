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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LocalStorage.Location;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.FinancePeriod
{
    public partial class frmFinancePeriodCreate : Form
    {

        public int positionHandle = -1;
        DelegateRefreshData refeshData;
        long branchId;

        public frmFinancePeriodCreate(long branchId, DelegateRefreshData refeshData)
        {
            InitializeComponent();
            try
            {
                this.refeshData = refeshData;
                this.branchId = branchId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {


                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                CommonParam param = new CommonParam();
                bool success = false;
                if (CheckPeriodTime())
                {
                    long periodTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPeriodTime.DateTime) ?? 0;
                    HIS_FINANCE_PERIOD financePeriod = new HIS_FINANCE_PERIOD();
                    financePeriod.BRANCH_ID = branchId;
                    financePeriod.PERIOD_TIME = periodTime;
                    WaitingManager.Show();
                    var result = new BackendAdapter(param)
                    .Post<MOS.EFMODEL.DataModels.HIS_FINANCE_PERIOD>("api/HisFinancePeriod/Create", ApiConsumers.MosConsumer, financePeriod, param);
                    WaitingManager.Hide();
                    if (result != null)
                    {
                        success = true;
                        if (refeshData != null)
                        {
                            refeshData();
                            this.Close();
                        }
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool CheckPeriodTime()
        {
            bool result = true;
            try
            {
                //Lay thoi gian hien tai
                HisFinancePeriodViewFilter filter = new HisFinancePeriodViewFilter();
                filter.BRANCH_ID = branchId;
                filter.ORDER_FIELD = "PERIOD_TIME";
                filter.ORDER_DIRECTION = "DESC";
                CommonParam param = new CommonParam();
                List<V_HIS_FINANCE_PERIOD> financePeriods = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_FINANCE_PERIOD>>("api/HisFinancePeriod/GetView", ApiConsumers.MosConsumer, filter, param);
                if (financePeriods != null && financePeriods.Count > 0)
                {
                    long periodTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPeriodTime.DateTime) ?? 0;
                    if (periodTime < financePeriods[0].PERIOD_TIME)
                    {
                        MessageBox.Show(String.Format("Thời gian cuối kỳ nhỏ hơn kỳ trước đó {0}", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(financePeriods[0].PERIOD_TIME)), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        result = false;
                    }
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

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmFinancePeriodCreate_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                dtPeriodTime.DateTime = DateTime.Now;
                ValiPeriodTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValiPeriodTime()
        {
            try
            {
                ControlEditValidationRule icdMainRule = new ControlEditValidationRule();
                icdMainRule.editor = dtPeriodTime;
                icdMainRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtPeriodTime, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
