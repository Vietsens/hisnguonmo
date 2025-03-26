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
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Core;
using DevExpress.Utils;
using Inventec.UC.ListReports.Base;
using Inventec.Desktop.Common.Message;

namespace Inventec.UC.ListReports.Form
{
    public partial class frmUpdate : DevExpress.XtraEditors.XtraForm
    {
        private SAR.EFMODEL.DataModels.V_SAR_REPORT Report;
        private ProcessHasException _HasException;

        public frmUpdate(SAR.EFMODEL.DataModels.V_SAR_REPORT report, ProcessHasException hasException)
        {
            InitializeComponent();
            try
            {
                SetIconForm();
                this.Report = report;
                this._HasException = hasException;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIconForm()
        {
            try
            {
                string filePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                string pathFileIcon = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), Config.GlobalStore.pathFileIcon);
                System.Drawing.Icon icon = new Icon(pathFileIcon);
                this.Icon = icon;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                if (Report != null)
                {
                    lblReportCode.Text = Report.REPORT_CODE;
                    lblReportStatus.Text = Report.REPORT_STT_NAME;
                    txtReportName.Text = Report.REPORT_NAME;
                    txtDescription.Text = Report.DESCRIPTION;
                    txtReportName.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (Report != null)
                {
                    WaitingManager.Show();
                    Report.DESCRIPTION = txtDescription.Text;
                    Report.REPORT_NAME = txtReportName.Text;
                    SAR.EFMODEL.DataModels.SAR_REPORT upReport = new SAR.EFMODEL.DataModels.SAR_REPORT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<SAR.EFMODEL.DataModels.SAR_REPORT>(upReport, Report);
                    var result = new Sar.SarReport.Update.SarReportUpdateFactory(param).createFactory(upReport).Update();
                    if (result != null)
                    {
                        success = true;
                    }
                    WaitingManager.Hide();

                    #region Show message
                    MessageManager.Show(this.ParentForm, new CommonParam(), true);
                    #endregion ;
                    #region has exception
                    if (_HasException != null) _HasException(param);
                    #endregion

                    if (success)
                        this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void txtReportName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
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
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
    }
}
