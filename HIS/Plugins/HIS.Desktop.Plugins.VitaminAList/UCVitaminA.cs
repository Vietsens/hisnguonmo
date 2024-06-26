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
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.VitaminAList
{
    public partial class UCVitaminAList : UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        System.Globalization.CultureInfo cultureLang;
        List<MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A> listVitaminA;
        Inventec.Desktop.Common.Modules.Module currentModule;
        #endregion

        #region Construct
        public UCVitaminAList(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.roomId = currentModule.RoomId;
                this.roomTypeId = currentModule.RoomTypeId;
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCVitaminAList_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGrid();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void LoadKeysFromlanguage()
        {
            try
            {
                //Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.VitaminAList.Resources.Lang", typeof(HIS.Desktop.Plugins.VitaminAList.UCVitaminAList).Assembly);

                //this.gridColumn12.ToolTip = Inventec.Common.Resource.Get.Value("UCVitaminAList.gridColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtKeyWord.Text = "";
                //Focus
                txtKeyWord.Focus();
                dtCreateTimeFrom.DateTime = DateTime.Now;
                dtCreateTimeTo.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControl);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A>> apiResult = null;
                MOS.Filter.HisVitaminAViewFilter filter = new MOS.Filter.HisVitaminAViewFilter();
                SetFilter(ref filter);

                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A>>
                    ("api/HisVitaminA/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    listVitaminA = apiResult.Data.OrderByDescending(o => o.MODIFY_TIME).ThenByDescending(o => o.ID).ToList();
                    if (listVitaminA != null && listVitaminA.Count > 0)
                    {
                        gridControl.DataSource = listVitaminA;
                        rowCount = (listVitaminA == null ? 0 : listVitaminA.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (listVitaminA == null ? 0 : listVitaminA.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView.EndUpdate();
            }
        }

        private void SetFilter(ref MOS.Filter.HisVitaminAViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                filter.BRANCH_ID = BranchDataWorker.GetCurrentBranchId();

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A data = (MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "REQUEST_NAME")
                        {
                            if (!string.IsNullOrEmpty(data.REQUEST_LOGINNAME))
                            {
                                e.Value = data.REQUEST_LOGINNAME + " - " + data.REQUEST_USERNAME;
                            }
                        }
                        else if (e.Column.FieldName == "EXECUTE_NAME")
                        {
                            if (!string.IsNullOrEmpty(data.EXECUTE_LOGINNAME))
                            {
                                e.Value = data.EXECUTE_LOGINNAME + " - " + data.EXECUTE_USERNAME;
                            }
                        }
                        //else if (e.Column.FieldName == "IS_SICK_STR")
                        //{
                        //    e.Value = data.IS_SICK == 1 ? true : false;
                        //}
                        //else if (e.Column.FieldName == "IS_ONE_MONTH_BORN_STR")
                        //{
                        //    e.Value = data.IS_ONE_MONTH_BORN == 1 ? true : false;
                        //}
                        else if (e.Column.FieldName == "REQUEST_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.REQUEST_TIME);
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_VITAMIN_A)gridView.GetRow(e.RowHandle);
                    long isActive = long.Parse((gridView.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DELETE_DISPLAY")
                        {
                            if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && creator == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                                e.RepositoryItem = Btn_Delete_Enable;
                            else
                                e.RepositoryItem = Btn_Delete_Disable;
                        }
                        else if (e.Column.FieldName == "Edit")
                        {
                            if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && creator == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                                e.RepositoryItem = ButtonEditEnable;
                            else
                                e.RepositoryItem = ButtonEditDisable;
                        }
                        else if (e.Column.FieldName == "IS_ONE_MONTH_BORN_STR")
                        {
                            e.RepositoryItem = data.IS_ONE_MONTH_BORN == 1 ? OneMonth : null;
                        }
                        else if (e.Column.FieldName == "IS_SICK_STR")
                        {
                            e.RepositoryItem = data.IS_SICK == 1 ? Sick : null;
                        }
                        else if (e.Column.FieldName == "CASE_TYPE_STR")
                        {
                            if (data.CASE_TYPE == 1)
                                e.RepositoryItem = CaseTypeRed;
                            else if (data.CASE_TYPE == 2)
                                e.RepositoryItem = CaseTypeBlue;
                            else if (data.CASE_TYPE == 3)
                                e.RepositoryItem = CaseTypeBlack;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Event
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var row = (MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        var data = new MOS.EFMODEL.DataModels.HIS_VITAMIN_A();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_VITAMIN_A>(data, row);
                        var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A>("api/HisVitaminA/ChangeLock", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (result != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var row = (MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        var data = new MOS.EFMODEL.DataModels.HIS_VITAMIN_A();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_VITAMIN_A>(data, row);
                        var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A>("api/HisVitaminA/ChangeLock", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (result != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //try
            //{
            //    var ExpMestData = (MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A)gridView.GetFocusedRow();

            //    if (ExpMestData != null)
            //    {
            //        frmVitaminAUpdate frm = new frmVitaminAUpdate(ExpMestData);
            //        frm.ShowDialog();
            //        FillDataToGrid();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void FillDataApterClose(MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A ExpMestData)
        {
            try
            {
                MOS.Filter.HisVitaminAViewFilter filter = new MOS.Filter.HisVitaminAViewFilter();
                filter.ID = ExpMestData.ID;
                var listTreat = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A>>("api/HisVitaminA/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (listTreat != null && listTreat.Count == 1)
                {
                    listVitaminA[listVitaminA.IndexOf(ExpMestData)] = listTreat.First();
                    gridControl.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion
        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Refreshs()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusCode()
        {
            try
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void Btn_Delete_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Bạn có muốn xóa dữ liệu không?", "Thông báo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var row = (MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        var data = new MOS.EFMODEL.DataModels.HIS_VITAMIN_A();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_VITAMIN_A>(data, row);
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisVitaminA/Delete", ApiConsumer.ApiConsumers.MosConsumer, data.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (success)
                        {
                            FillDataToGrid();
                        }
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridControl_Click(object sender, EventArgs e)
        {

        }

        private void gridView_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_VITAMIN_A)gridView.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "CASE_TYPE_STR")
                        {
                            if (data.CASE_TYPE == 1)
                                e.Appearance.BackColor = Color.Red;
                            else if (data.CASE_TYPE == 2)
                                e.Appearance.BackColor = Color.Blue;
                            else if (data.CASE_TYPE == 3)
                                e.Appearance.BackColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
