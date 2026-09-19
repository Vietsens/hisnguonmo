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
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Library.CheckServiceExclusive.ADO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.CheckServiceExclusive.Form
{
    /// <summary>
    /// Form hien thi danh sach cap dich vu vi pham quy tac "khong duoc chi dinh dong thoi".
    /// Dung chung cho ca 2 muc xu ly (viec 57452):
    ///  - Muc CHAN  : khoi tao KHONG truyen actionContinue -> chi co nut Dong, ham goi tra ve false.
    ///  - Muc CANH BAO: truyen actionContinue -> hien them nut "Tiep tuc"; bam Tiep tuc thi cho luu.
    /// Nguoi dung bam dau X cung duoc coi la khong dong y (giong frmContraindicated).
    /// </summary>
    public partial class frmServiceExclusive : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private List<ServiceExclusiveViolationADO> violations;
        private Action<bool> actionContinue;
        private bool isClickBtnY = false;

        public frmServiceExclusive()
        {
            InitializeComponent();
        }

        public frmServiceExclusive(Inventec.Desktop.Common.Modules.Module moduleData,
            List<ServiceExclusiveViolationADO> violations,
            Action<bool> actionContinue = null)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.moduleData = moduleData;
                this.violations = violations;
                this.actionContinue = actionContinue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(
                    ApplicationStoreLocation.ApplicationDirectory,
                    ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmServiceExclusive_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControlProperties();
                LoadDataToGrid();

                // Chi hien nut "Tiep tuc" khi la muc canh bao (co callback)
                if (this.actionContinue != null)
                {
                    this.btnN.Text = "Hủy";
                    this.layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }

                this.btnN.Select();
                this.btnN.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGrid()
        {
            try
            {
                gridControl1.DataSource = null;
                List<ServiceExclusiveViolationADO> data = this.violations ?? new List<ServiceExclusiveViolationADO>();

                // Muc Chan len truoc de nguoi dung nhin thay ngay cai chan viec luu
                data = data.OrderByDescending(o => o.HANDLE_TYPE_ID)
                           .ThenBy(o => o.ASSIGNED_SERVICE_NAME)
                           .ToList();

                gridControl1.BeginUpdate();
                gridControl1.DataSource = data;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControlProperties()
        {
            try
            {
                this.layoutControlRoot.MinimumSize = new System.Drawing.Size(this.layoutControlRoot.Width, 240);

                this.layoutControlRoot.AutoSize = true;
                this.layoutControlRoot.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                this.AutoSize = true;
                this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>To do cac dong muc Chan de phan biet voi dong muc Canh bao</summary>
        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                {
                    return;
                }

                ServiceExclusiveViolationADO row = gridView1.GetRow(e.RowHandle) as ServiceExclusiveViolationADO;
                if (row != null && row.HANDLE_TYPE_ID == (short)HandleType.Block)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.actionContinue != null)
                {
                    this.actionContinue(false);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnY_Click(object sender, EventArgs e)
        {
            try
            {
                this.isClickBtnY = true;
                if (this.actionContinue != null)
                {
                    this.actionContinue(true);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmServiceExclusive_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // Bam dau X = khong dong y tiep tuc
                if (this.actionContinue != null && !this.isClickBtnY)
                {
                    this.actionContinue(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
