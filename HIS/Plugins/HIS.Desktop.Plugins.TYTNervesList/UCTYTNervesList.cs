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
using DevExpress.Data;
using TYT.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.Common;
using HIS.Desktop.IsAdmin;

namespace TYT.Desktop.Plugins.NervesList
{
    public partial class UCTYTNervesList : HIS.Desktop.Utility.UserControlBase
    {
        int numPageSize;
        int rowCount = 0;
        int dataTotal = 0;
        Inventec.Desktop.Common.Modules.Module moduleData;

        public UCTYTNervesList(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            this.moduleData = _module;
        }

        private void gridViewNerves_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TYT_NERVES dataRow = (TYT_NERVES)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        if (dataRow.IS_HAS_NOT_DAY_DOB.HasValue)
                            if (dataRow.IS_HAS_NOT_DAY_DOB == 1)
                                e.Value = dataRow.DOB.ToString().Substring(0, 4);
                            else
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.DOB);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.MODIFY_TIME ?? 0);
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCTYTNervesList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataToDefault();
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
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
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dtTimeFrom.EditValue != null)
                {
                    dtTimeTo.Focus();
                    dtTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            if (dtTimeTo.EditValue != null)
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                TYT_NERVES nerves = gridViewNerves.GetFocusedRow() as TYT_NERVES;
                List<TYT_NERVES> listNervesTemp = gridControlNerves.DataSource as List<TYT_NERVES>;
                if (nerves != null && listNervesTemp != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool result = new BackendAdapter(param)
                    .Post<bool>("api/TytNerves/Delete", ApiConsumers.TytConsumer, nerves.ID, param);
                    WaitingManager.Hide();
                    if (result)
                    {
                        listNervesTemp.Remove(nerves);
                        gridControlNerves.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                TYT_NERVES nerves = gridViewNerves.GetFocusedRow() as TYT_NERVES;
                if (nerves != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "TYT.Desktop.Plugins.Nerves").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = TYT.Desktop.Plugins.Nerves");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(nerves.ID);
                        listArgs.Add((DelegateSelectData)RefeshDataEdit);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataEdit(object data)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToDefault();
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
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

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void txtPersonCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void gridViewNerves_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                TYT_NERVES data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewNerves.GetDataSourceRowIndex(e.RowHandle);
                    data = (TYT_NERVES)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (e.Column.FieldName == "ACTION_EDIT")
                        {
                            if (data.CREATOR == loginName || CheckLoginAdmin.IsAdmin(loginName))
                            {
                                e.RepositoryItem = repositoryItemButtonEdit;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_Disable;
                            }
                        }
                        else if (e.Column.FieldName == "ACTION_DELETE")
                        {
                            if (data.CREATOR == loginName || CheckLoginAdmin.IsAdmin(loginName))
                            {
                                e.RepositoryItem = repositoryItemButtonDelete;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonDelete_Disable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
