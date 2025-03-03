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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using MOS.SDO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using MOS.KskSignData;
using DevExpress.Data;
using System.Diagnostics;
using System.Threading;
using EMR.WCF.DCO;
using Inventec.Common.SignLibrary.ServiceSign;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Xml;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.ConsultationRegList
{
    public partial class UCConsultationRegList : UserControlBase
    {

        #region Derlare
        string datetime;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        List<HIS_EXECUTE_ROOM> executeRoomSelecteds;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        RefeshReference refeshReference { get; set; }
        int lastRowHandle = -1;
        private bool isNotLoadWhileChangeControlStateInFirst;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private X509Certificate2 certificate;
        private string SerialNumber;
        #endregion
        #region ConstructorLoad
        public UCConsultationRegList(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
        }
        private void UCConsultationRegList_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                FillDataToGrid();
                txtKeyWord.Focus();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region Private Method
        private void FillDataToGrid()
        {
            try
            {
                gridControl1.DataSource = null;
                WaitingManager.Show();
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadGridData(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_HIS_CONSULTATION_REG>> apiResult = null;
                HisConsultationRegViewFilter filter = new HisConsultationRegViewFilter();
                SetFilter(ref filter);
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_CONSULTATION_REG>>("api/HisConsultationReg/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl1.DataSource = data;
                    }
                    else
                    {
                        gridControl1.DataSource = null;

                    }
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        private void SetFilter(ref HisConsultationRegViewFilter filter)
        {
            try
            {
                filter.BRANCH_ID = WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId;
                if (!string.IsNullOrEmpty(txtPhoneNumber.Text))
                {
                    filter.MOBILE = txtPhoneNumber.Text;
                }
                else if (!string.IsNullOrEmpty(txtCccd.Text))
                {
                    filter.CMND_OR_CCCD = txtCccd.Text;

                }
                else if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE = txtPatientCode.Text;
                }
                else if (!string.IsNullOrEmpty(txtConsultationRegCode.Text))
                {
                    string code = txtConsultationRegCode.Text.Trim();
                    if (code.Length < 7 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000}", Convert.ToInt64(code));
                        txtConsultationRegCode.Text = code;
                    }
                    filter.CONSULTATION_REG_CODE = txtConsultationRegCode.Text;
                }
                else
                {
                    filter.ORDER_FIELD = "NUM_ORDER";
                    filter.ORDER_DIRECTION = "ASC";
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.REGISTER_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.REGISTER_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                    if (cboStatus.EditValue != null)
                    {
                        switch (cboStatus.SelectedIndex)
                        {
                            case 0:
                                filter.REGISTER_STT_ID = null;
                                break;
                            case 1:
                                filter.REGISTER_STT_IDs= new List<long>() { 1, 2 };
                                break;
                            case 2:
                                filter.REGISTER_STT_ID = 1;
                                break;
                            case 3:
                                filter.REGISTER_STT_ID = 2;
                                break;
                            default:
                                filter.REGISTER_STT_ID = 3;
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultControl()
        {
            try
            {

                dtCreateTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartMonth() ?? 0));
                dtCreateTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                txtKeyWord.Text = "";
                txtConsultationRegCode.Text = "";
                txtPatientCode.Text = "";
                txtCccd.Text = "";
                txtPhoneNumber.Text = "";
                cboStatus.SelectedIndex = 1;
                txtKeyWord.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        #region EvenGridView
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_CONSULTATION_REG data = (V_HIS_CONSULTATION_REG)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging.pagingGrid.CurrentPage - 1) * (ucPaging.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "Status")
                    {
                        if (data.REGISTER_STT_ID == 1) // Yeu cau
                        {
                            e.Value = imageList1.Images[0];
                        }
                        else if (data.REGISTER_STT_ID == 2)// dang xu ly
                        {
                            e.Value = imageList1.Images[1];
                        }
                        else if (data.REGISTER_STT_ID == 5)//huy
                        {
                            e.Value = imageList1.Images[2];
                        }
                        else if (data.REGISTER_STT_ID == 4)//khong lien lac duoc
                        {
                            e.Value = imageList1.Images[3];
                        }
                        else//hoan thanh
                        {
                        }


                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        try
                        {
                            if (data.IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.DOB.ToString().Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DOB", ex);
                        }
                    }
                    else if (e.Column.FieldName == "REGISTER_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.REGISTER_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao REGISTER_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "EXECUTE_STR")
                    {
                        try
                        {
                            e.Value = !string.IsNullOrEmpty(data.EXECUTE_LOGINNAME) ? data.EXECUTE_LOGINNAME + " - " + data.EXECUTE_USERNAME : "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao EXECUTE_STR", ex);

                        }
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.EXECUTE_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao EXECUTE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CCCD_CMND_STR")
                    {
                        try
                        {
                            e.Value = data.CCCD_NUMBER ?? data.CMND_NUMBER;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CCCD_CMND", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #region EventClickPress
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnRefresh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnRefresh.Focus();
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        private void btnSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtKskDriverCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
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
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        #region EvenBarManager
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region EventItemButton
        private void repositoryItemButtonEdit_EDIT_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool rs = false;
                var row = (V_HIS_CONSULTATION_REG)gridView1.GetFocusedRow();
                if (row != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn Xử lý tư vấn khám?", "Thông báo", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        HIS_CONSULTATION_REG obj = new HIS_CONSULTATION_REG();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_CONSULTATION_REG>(obj, row);

                        obj.REGISTER_STT_ID = 2;
                        obj.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        obj.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                        obj.EXECUTE_TIME = Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => obj), obj));
                        var rsObj = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_CONSULTATION_REG>("api/HisConsultationReg/Update", ApiConsumers.MosConsumer, obj, param);
                        rs = rsObj != null;
                        if (rs)
                        {
                            FillDataToGrid();
                            WaitingManager.Hide();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, rs);
                    }

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ASYN_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool rs = false;
                List<KskDriverSyncSDO> listKskDriveSync = new List<KskDriverSyncSDO>();
                var row = (V_HIS_CONSULTATION_REG)gridView1.GetFocusedRow();
                if (row != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn Hoàn thành tư vấn khám?", "Thông báo", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        HIS_CONSULTATION_REG obj = new HIS_CONSULTATION_REG();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_CONSULTATION_REG>(obj, row);
                        obj.REGISTER_STT_ID = 3;

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => obj), obj));
                        var rsObj = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_CONSULTATION_REG>("api/HisConsultationReg/Update", ApiConsumers.MosConsumer, obj, param);
                        rs = rsObj != null;
                        if (rs)
                        {
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, rs);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        public void Refesh()
        {
            btnRefresh_Click(null, null);
        }
        public void Search()
        {
            btnSearch_Click(null, null);
        }
        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl1)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl1.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "Status")
                            {
                                long sttId = (long)view.GetRowCellValue(lastRowHandle, "REGISTER_STT_ID");
                                if (sttId == 1)
                                {
                                    text = "Yêu cầu";
                                }
                                else if (sttId == 2)
                                {
                                    text = "Đang xử lý";
                                }
                                else if (sttId == 3)
                                    text = "Hoàn thành";
                                else if(sttId == 4)
                                    text = "Không liên lạc được";
                                else if (sttId == 5)
                                    text = "Hủy";
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_CONSULTATION_REG)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "Execute")
                        {
                            try
                            {
                                if (data.REGISTER_STT_ID == 1)
                                    e.RepositoryItem = repExecute;
                                else
                                    e.RepositoryItem = repExecuteDis;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "Finish")
                        {
                            try
                            {
                                if (data.REGISTER_STT_ID == 2)
                                    e.RepositoryItem = repFinish;
                                else
                                    e.RepositoryItem = repFinishDis;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DONT_CONTACT")
                        {
                            try
                            {
                                if (data.REGISTER_STT_ID == 2)
                                    e.RepositoryItem = repoNotContactE;
                                else
                                    e.RepositoryItem = repoNotContactD;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repoNotContactE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool rs = false;
                
                var row = (V_HIS_CONSULTATION_REG)gridView1.GetFocusedRow();
                if (MessageBox.Show(this, "Bạn muốn cập nhật tư vấn \"Không liên lạc được\"?",
                    "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    WaitingManager.Show();
                    HIS_CONSULTATION_REG obj = new HIS_CONSULTATION_REG();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_CONSULTATION_REG>(obj, row);
                    obj.REGISTER_STT_ID = 4;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => obj), obj));
                    var rsObj = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_CONSULTATION_REG>("api/HisConsultationReg/Update", ApiConsumers.MosConsumer, obj, param);
                    rs = rsObj != null;
                    if (rs)
                    {
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, rs);
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
