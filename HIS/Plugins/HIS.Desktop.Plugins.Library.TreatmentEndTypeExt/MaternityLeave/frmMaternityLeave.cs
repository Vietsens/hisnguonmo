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
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.MaternityLeave
{
    public partial class frmMaternityLeave : FormBase
    {
        long treatmentId;
        DelegateSelectData ReloadDataTreatmentEndTypeExt;
        List<MaternityLeaveData> MaternityLeaveDatas;
        public frmMaternityLeave(long _treatmentId, List<MaternityLeaveData> _maternityLeaveDatas, DelegateSelectData _reloadDataTreatmentEndTypeExt)
        {
            InitializeComponent();
            try
            {
                this.treatmentId = _treatmentId;
                this.ReloadDataTreatmentEndTypeExt = _reloadDataTreatmentEndTypeExt;
                this.MaternityLeaveDatas = _maternityLeaveDatas;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMaternityLeave_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                InitComboGender();
                InitMaternityLeaveGrid();
                LoadDataDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaternityLeave_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ACTION_DELETE")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    List<MaternityLeaveData> temps = gridControlMaternityLeave.DataSource as List<MaternityLeaveData>;
                    if (rowSelected > 0 || (temps != null && temps.Count > 1))
                    {
                        e.RepositoryItem = repositoryItemButtonEditActionDelete;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditActionAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var maternityLeaveDatas = gridControlMaternityLeave.DataSource as List<MaternityLeaveData>;
                MaternityLeaveData maternityLeaveData = new MaternityLeaveData();
                maternityLeaveDatas.Add(maternityLeaveData);
                gridControlMaternityLeave.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditActionDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var maternityLeaveDatas = gridControlMaternityLeave.DataSource as List<MaternityLeaveData>;
                var maternityLeaveData = gridViewMaternityLeave.GetFocusedRow() as MaternityLeaveData;
                if (maternityLeaveData != null)
                {
                    maternityLeaveDatas.Remove(maternityLeaveData);
                    gridControlMaternityLeave.RefreshDataSource();
                    gridViewMaternityLeave.FocusedColumn = gridViewMaternityLeave.Columns[1];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Check()) return;

                List<MaternityLeaveData> maternityLeaveDatas = gridControlMaternityLeave.DataSource as List<MaternityLeaveData>;
                if (this.ReloadDataTreatmentEndTypeExt != null)
                    this.ReloadDataTreatmentEndTypeExt(maternityLeaveDatas);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }
    }
}
