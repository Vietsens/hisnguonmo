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
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    /// <summary>
    /// Chọn phiếu xét nghiệm khi lần khám có nhiều chỉ định, trước khi mở chức năng
    /// xem kết quả liên thông EMRToolkit. Thuần UI — không gọi API.
    /// </summary>
    public partial class frmChooseServiceReqEmrToolkit : HIS.Desktop.Utility.FormBase
    {
        private readonly List<HIS_SERVICE_REQ> listServiceReq;

        /// <summary>Phiếu người dùng chọn. Chỉ có giá trị khi DialogResult là OK.</summary>
        public HIS_SERVICE_REQ SelectedServiceReq { get; private set; }

        public frmChooseServiceReqEmrToolkit(List<HIS_SERVICE_REQ> listServiceReq)
        {
            InitializeComponent();
            try
            {
                this.listServiceReq = listServiceReq ?? new List<HIS_SERVICE_REQ>();
                SetIcon();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmChooseServiceReqEmrToolkit_Load(object sender, EventArgs e)
        {
            try
            {
                this.gridViewServiceReq.CustomUnboundColumnData += gridViewServiceReq_CustomUnboundColumnData;

                this.gridViewServiceReq.BeginUpdate();
                try
                {
                    this.grdServiceReq.DataSource = this.listServiceReq;
                }
                finally
                {
                    this.gridViewServiceReq.EndUpdate();
                }

                if (this.listServiceReq.Count > 0) this.gridViewServiceReq.FocusedRowHandle = 0;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Chỉ định dạng thời gian — cột nhẹ, không truy vấn gì thêm</summary>
        private void gridViewServiceReq_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "INTRUCTION_TIME_STR" || !e.IsGetData) return;

                HIS_SERVICE_REQ data = e.Row as HIS_SERVICE_REQ;
                if (data == null) return;

                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                ChooseProcess();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ChooseProcess();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ChooseProcess()
        {
            try
            {
                HIS_SERVICE_REQ selected = this.gridViewServiceReq.GetFocusedRow() as HIS_SERVICE_REQ;
                if (selected == null) return;

                this.SelectedServiceReq = selected;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.SelectedServiceReq = null;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
