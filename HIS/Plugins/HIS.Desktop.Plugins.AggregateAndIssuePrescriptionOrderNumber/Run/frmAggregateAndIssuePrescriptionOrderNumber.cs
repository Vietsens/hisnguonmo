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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AggregateAndIssuePrescriptionOrderNumber.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggregateAndIssuePrescriptionOrderNumber.Run
{
    public partial class frmAggregateAndIssuePrescriptionOrderNumber : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module _moduleData;
        long counterReloadScreen = 0;
        long counterResetThongBao = 0;
        long DefaultSeccondReloadScreen = 120;
        long DefaultSeccondResetThongBao = 10;
        bool isResetThongBao = false;
        HIS_EXP_MEST _expMest_ForPrint;
        MOS.SDO.WorkPlaceSDO WorkPlaceSDO;

        const string timerApplicationRuntime = "timerApplicationRuntime";
        const string timerAutoFocusTxtTreatmentCode = "timerAutoFocusTxtTreatmentCode";
        #endregion

        #region Construct
        public frmAggregateAndIssuePrescriptionOrderNumber()
        {
            InitializeComponent();
        }

        public frmAggregateAndIssuePrescriptionOrderNumber(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this._moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmAggregateAndIssuePrescriptionOrderNumber_Load(object sender, EventArgs e)
        {
            try
            {
                RegisterTimer(this._moduleData.ModuleLink, timerApplicationRuntime, 1000, timerApplicationRuntime_Tick);
                RegisterTimer(this._moduleData.ModuleLink, timerAutoFocusTxtTreatmentCode, 20000, timerAutoFocusTxtTreatmentCode_Tick);
                StartTimer(this._moduleData.ModuleLink, timerApplicationRuntime);
                StartTimer(this._moduleData.ModuleLink, timerAutoFocusTxtTreatmentCode);
                lciBranchImage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LoadCurrentBranch();
                txtTreatmentCode.Focus();
                MoveCursor(txtTreatmentCode.Location);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentBranch()
        {
            try
            {
                this.WorkPlaceSDO = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this._moduleData.RoomId);
                var branch = BackendDataWorker.Get<HIS_BRANCH>().Where(o => o.ID == WorkPlaceSDO.BranchId).FirstOrDefault();
                if (branch != null)
                {
                    if (!string.IsNullOrEmpty(branch.LOGO_URL))
                    {
                        lciBranchImage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        MemoryStream fs = null;
                        try
                        {
                            fs = Inventec.Fss.Client.FileDownload.GetFile(branch.LOGO_URL);
                            pictureEditBranchImage.Image = System.Drawing.Image.FromStream(fs);
                            lciBranchImage.Size = pictureEditBranchImage.Image.Size;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        lciBranchImage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    lblWellcomeText.Text = String.Format(ResourceMessage.WellcomeText, branch.BRANCH_NAME);
                }
                else
                {
                    lciBranchImage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lblWellcomeText.Text = "";
                }
                //Căn chữ ở giữa
                var oldPadding = lblDefault01.Padding;
                lblDefault01.Padding = new Padding(oldPadding.Left, oldPadding.Top, lciBranchImage.Width, oldPadding.Bottom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerApplicationRuntime_Tick()
        {
            try
            {
                this.counterReloadScreen += 1;
                if (this.counterReloadScreen >= this.DefaultSeccondReloadScreen)
                {
                    FillDataExpMest(new HIS_EXP_MEST());
                    lblThongBao.Text = "";
                }
                this.counterResetThongBao += 1;
                if (this.counterResetThongBao >= this.DefaultSeccondResetThongBao)
                {
                    ResetThongBao();
                }
                CreateThreadUpdateApplicationTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadUpdateApplicationTime()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(UpdateApplicationTimeNewThread));
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void UpdateApplicationTimeNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { UpdateApplicationTime(); }));
                }
                else
                {
                    UpdateApplicationTime();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateApplicationTime()
        {
            try
            {
                lblApplicationRuntime.Text = GetCurrentDatetime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetCurrentDatetime()
        {
            try
            {
                var now = DateTime.Now;
                string ngay = string.Format("{0:00}", now.Day);
                string thang = string.Format("{0:00}", now.Month);
                string nam = now.Year.ToString();
                string gio = string.Format("{0:00}", now.Hour);
                string phut = string.Format("{0:00}", now.Minute);
                //string giay = string.Format("{0:00}", DateTime.Now.Second);
                //return "" + ngay + "/" + thang + "/" + nam + " " + gio + ":" + phut + ":" + giay + "";
                return "" + ngay + "/" + thang + "/" + nam + " " + gio + ":" + phut + "";
            }
            catch { }

            return "";
        }

        private void timerAutoFocusTxtTreatmentCode_Tick()
        {
            try
            {
                txtTreatmentCode.Focus();
                //MoveCursor(txtTreatmentCode.Location);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MoveCursor(Point position)
        {
            try
            {
                // Set the Current cursor, move the cursor's Position,

                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point(position.X + 20, position.Y + 20);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ProcessAggregate();
                    txtTreatmentCode.Text = "";
                    txtTreatmentCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                var str = e.NewValue != null ? e.NewValue.ToString() : "";
                if (!String.IsNullOrEmpty(str))
                {
                    if (!IsDigitsOnly(str))
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private void txtTreatmentCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.counterReloadScreen = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lblThongBao_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.counterResetThongBao = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
