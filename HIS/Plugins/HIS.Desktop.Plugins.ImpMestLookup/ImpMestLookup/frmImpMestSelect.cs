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
using HIS.Desktop.Plugins.ImpMestLookup.ADO;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestLookup.ImpMestLookup
{
    /// <summary>
    /// Màn chọn phiếu nhập - hiển thị khi tra cứu theo Số hóa đơn trả về nhiều phiếu nhập
    /// (số hóa đơn không có ràng buộc duy nhất trên dữ liệu phiếu nhập).
    /// </summary>
    public partial class frmImpMestSelect : XtraForm
    {
        private List<V_HIS_IMP_MEST> impMests;

        /// <summary>
        /// Phiếu nhập người dùng đã chọn. null nếu người dùng đóng màn mà không chọn.
        /// </summary>
        public V_HIS_IMP_MEST SelectedImpMest { get; private set; }

        public frmImpMestSelect(List<V_HIS_IMP_MEST> _impMests)
        {
            try
            {
                InitializeComponent();
                this.impMests = _impMests;
                this.SelectedImpMest = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmImpMestSelect_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                this.AcceptButton = btnSelect;
                this.CancelButton = btnClose;

                List<ImpMestSelectADO> data = new List<ImpMestSelectADO>();
                if (this.impMests != null)
                {
                    data = this.impMests.Select(o => new ImpMestSelectADO(o)).ToList();
                }
                gridControlImpMest.DataSource = data;
                gridControlImpMest.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (Resources.ResourceLanguageManager.LanguageResource == null)
                {
                    Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ImpMestLookup.Resources.Lang", typeof(frmImpMestSelect).Assembly);
                }

                this.Text = Inventec.Common.Resource.Get.Value("frmImpMestSelect.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSelect.Text = Inventec.Common.Resource.Get.Value("frmImpMestSelect.btnSelect.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnClose.Text = Inventec.Common.Resource.Get.Value("frmImpMestSelect.btnClose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnImpMestCode.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.lblImpMestCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMediStockName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.lblImpMedistock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnImpTime.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.lblImpTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnImpUserName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.lblImpUserName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDocumentPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestSelect.gridColumnDocumentPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMest_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = gridViewImpMest.CalcHitInfo(gridControlImpMest.PointToClient(Control.MousePosition));
                if (hitInfo == null || !hitInfo.InRow) return;
                SelectCurrentRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                SelectCurrentRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.SelectedImpMest = null;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lấy phiếu nhập trên dòng đang chọn và đóng màn với kết quả OK.
        /// </summary>
        private void SelectCurrentRow()
        {
            try
            {
                ImpMestSelectADO row = gridViewImpMest.GetFocusedRow() as ImpMestSelectADO;
                if (row == null || row.ImpMest == null) return;
                this.SelectedImpMest = row.ImpMest;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
