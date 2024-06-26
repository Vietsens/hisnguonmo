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
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;

namespace HIS.Desktop.Plugins.HisMedicalContractCreate.Run
{
    public partial class UC_LoadEdit : UserControl
    {
        Action<ADO.MetyMatyADO> DeleteButton_Click;
        Action<ADO.MetyMatyADO> EditButton_Click;
        List<ADO.MetyMatyADO> ListAdo;

        public UC_LoadEdit(Action<ADO.MetyMatyADO> editClick, Action<ADO.MetyMatyADO> deleteClick, bool isMedicine)
        {
            InitializeComponent();
            try
            {
                this.DeleteButton_Click = deleteClick;
                this.EditButton_Click = editClick;

                if (!isMedicine)
                {
                    gridColumn8.VisibleIndex = -1;
                    gridColumn12.VisibleIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.MetyMatyADO)gridViewEdit.GetFocusedRow();
                if (row != null && this.DeleteButton_Click != null)
                {
                    this.DeleteButton_Click(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEdit_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var data = (ADO.MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (e.Column.FieldName == "ImpVatRatio")
                    {
                        data.IMP_VAT_RATIO = data.ImpVatRatio / 100;
                    }
                    gridControlEdit.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(List<ADO.MetyMatyADO> data)
        {
            try
            {
                gridControlEdit.BeginUpdate();
                this.ListAdo = data;
                gridControlEdit.DataSource = null;
                gridControlEdit.DataSource = data;
                gridControlEdit.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UC_LoadEdit_Load(object sender, EventArgs e)
        {
        }

        public void PostEditor()
        {
            try
            {
                gridViewEdit.PostEditor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                    {
                        Search(txtSearch.Text.Trim().ToUpper());
                    }
                    else
                    {
                        Reload(this.ListAdo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Search(string keyWord)
        {
            try
            {
                if (ListAdo != null && ListAdo.Count > 0)
                {
                    List<ADO.MetyMatyADO> ados = new List<ADO.MetyMatyADO>();
                    ados = ListAdo.Where(o =>
                        o.MEDICINE_TYPE_CODE.ToUpper().Contains(keyWord)
                        || o.MEDICINE_TYPE_NAME.ToUpper().Contains(keyWord)
                        ).ToList();

                    gridControlEdit.BeginUpdate();
                    gridControlEdit.DataSource = null;
                    gridControlEdit.DataSource = ados;
                    gridControlEdit.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                {
                    Search(txtSearch.Text.Trim().ToUpper());
                }
                else
                {
                    Reload(this.ListAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEdit_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.MetyMatyADO data = (ADO.MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data == null)
                        return;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_STRING")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                    }
                    else if (e.Column.FieldName == "IMP_EXPIRED_DATE_STRING")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.IMP_EXPIRED_DATE ?? 0);
                    }
                    else if (e.Column.FieldName == "BID_GROUP_CODE")
                    {
                        if (!string.IsNullOrEmpty(data.BID_GROUP_CODE))
                        {
                            e.Value = data.BID_GROUP_CODE;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewEdit_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var row = (ADO.MetyMatyADO)gridViewEdit.GetFocusedRow();
                if (row != null && this.EditButton_Click != null)
                {
                    this.EditButton_Click(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
