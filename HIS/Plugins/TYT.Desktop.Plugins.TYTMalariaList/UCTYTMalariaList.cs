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
using Inventec.Core;
using DevExpress.Utils;
using Inventec.Desktop.Common.Message;
using TYT.Desktop.Plugins.TYTMalariaList;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Logging;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Common.Adapter;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.ConfigSystem;
using System.IO;
using DevExpress.XtraEditors.Controls;
using TYT.Filter;
using TYT.EFMODEL.DataModels;

namespace TYT.Desktop.Plugins.TYTMalariaList
{
    public partial class UCTYTMalariaList : HIS.Desktop.Utility.UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        string loginName = "";
        bool IsAddmin = true;

        public UCTYTMalariaList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.currentModule = module;
            SetCaptionByLanguageKey();
        }

        private void gridTYTFetusExamListList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (TYT_MALARIA)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        try
                        {
                            string dob = (view.GetRowCellValue(e.ListSourceRowIndex, "DOB") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(dob));
                            if (data.IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = dob.Substring(0, 4);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                    gridViewTYTFetusBornList.RefreshData();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditHeinCardInfo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setDefaultControl()
        {
            try
            {

                dtCreateTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartDay() ?? 0));
                dtCreateTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                txtKeyWord.Text = "";
                txtMaYTeCode.Text = "";
                txtTYTFetusExamListCode.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        int pageSize;
        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                GridPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(GridPaging, param, pageSize, gridControlTYTFetusBornList);
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

                ApiResultObject<List<TYT.EFMODEL.DataModels.TYT_MALARIA>> apiResult = null;
                TytMalariaFilter filter = new TytMalariaFilter();

                SetFilter(ref filter);

                gridViewTYTFetusBornList.BeginUpdate();

                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<TYT_MALARIA>>
                    ("api/TytMalaria/Get", HIS.Desktop.ApiConsumer.ApiConsumers.TytConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlTYTFetusBornList.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlTYTFetusBornList.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewTYTFetusBornList.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
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
                setDefaultControl();
                FillDataToGrid();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                Search();
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetFilter(ref TytMalariaFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtTYTFetusExamListCode.Text))
                {
                    string code = txtTYTFetusExamListCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtTYTFetusExamListCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }
                else if (!string.IsNullOrEmpty(txtMaYTeCode.Text))
                {
                    string code = txtMaYTeCode.Text.Trim();
                    if (code.Length < 9 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000}", Convert.ToInt64(code));
                        txtMaYTeCode.Text = code;
                    }
                    filter.PERSON_CODE__EXACT = code;
                }
                else
                {
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.KEY_WORD = txtKeyWord.Text.Trim();


                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGTYTFetusExamListEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (TYT.EFMODEL.DataModels.TYT_MALARIA)gridViewTYTFetusBornList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    listArgs.Add((RefeshReference)Refesh);
                    CallModule callModule = new CallModule(CallModule.TYTFetusBornListUpdate, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Search()
        {
            if (btnSearch.Enabled)
            {
                btnSearch.Focus();
                btnSearch_Click(null, null);
            }
        }

        public void Refesh()
        {
            try
            {
                if (btnRefesh.Enabled)
                {
                    btnRefesh.Focus();
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtKeyWord.Text != "")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCListTYTFetusExamList_Load(object sender, EventArgs e)
        {
            try
            {
                setDefaultControl();
                FillDataToGrid();
                loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CheckEmployeeIsAdmin();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckEmployeeIsAdmin()
        {
            try
            {
                MOS.Filter.HisEmployeeFilter employeeFilter = new MOS.Filter.HisEmployeeFilter();
                employeeFilter.LOGINNAME__EXACT = this.loginName;
                var employees = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>("api/HisEmployee/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, employeeFilter, null);
                IsAddmin = (employees != null && employees.Count() > 0) ? (employees.FirstOrDefault().IS_ADMIN == 1) : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTYTFetusExamListCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtTYTFetusExamListCode.Text != "")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaYTeCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtMaYTeCode.Text != "")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTYTFetusExamList_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string creator = (gridViewTYTFetusBornList.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();

                    if (e.Column.FieldName == "DELETE")
                    {
                        if (creator == loginName || this.IsAddmin)
                            e.RepositoryItem = repositoryItemButton__Delete;
                        else
                            e.RepositoryItem = repositoryItemButton__Delete_D;
                    }
                    if (e.Column.FieldName == "EDIT")
                    {
                        if (creator == loginName || this.IsAddmin)
                            e.RepositoryItem = btnGTYTFetusExamListEdit;
                        else
                            e.RepositoryItem = repositoryItemButtonEdit_Update_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Bạn có muốn hủy dữ liệu không",
                    "Thông báo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var row = (TYT_MALARIA)gridViewTYTFetusBornList.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            ("/api/TytMalaria/Delete", HIS.Desktop.ApiConsumer.ApiConsumers.TytConsumer, row.ID, param);
                        if (apiresul)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();

                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

    }
}
