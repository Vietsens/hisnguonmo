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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ListDepositRequest.ADO;
using HIS.UC.ListDepositRequest;
using DevExpress.XtraEditors;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.DepositRequest;
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.DepositRequest
{
    public partial class UCDepositRequest : UserControlBase
    {

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;

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
                ucPaging1.Init(GridPaging, param, pageSize);
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

                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_DEPOSIT_REQ>> apiResult = null;
                HisDepositReqViewFilter filter = new HisDepositReqViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                filter.BRANCH_ID = WorkPlace.GetBranchId();
                if (!String.IsNullOrEmpty(txtReqCode.Text))
                {
                    string code = txtReqCode.Text.Trim();
                    if (code.Length < 8 && checkDigit(code))
                    {
                        code = string.Format("{0:00000000}", Convert.ToInt64(code));
                        txtReqCode.Text = code;
                    }
                    filter.DEPOSIT_REQ_CODE__EXACT = code;
                }
                else if (txtKeyWord.Text != null)
                {
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                }

                if (cboStatus.SelectedIndex == 0)
                {
                    filter.HAS_DEPOSIT = null;
                }
                else if (cboStatus.SelectedIndex == 1)
                {
                    filter.HAS_DEPOSIT = false;
                }
                //else if (cboStatus.SelectedIndex == 2)
                else
                {
                    filter.HAS_DEPOSIT = true;
                }

                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_DEPOSIT_REQ>>(HisRequestUriStore.HIS_DEPOSIT_REQ_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    listDepositReq = apiResult.Data;
                    if (listDepositReq != null && listDepositReq.Count > 0)
                    {
                        if (ucRequestDeposit != null)
                        {
                            listDepositReqProcessor.Reload(ucRequestDeposit, listDepositReq);
                            //listDepositReqProcessor.GetSelectRow(ucRequestDeposit);
                        }
                        rowCount = (listDepositReq == null ? 0 : listDepositReq.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        if (ucRequestDeposit != null)
                        {
                            listDepositReqProcessor.Reload(ucRequestDeposit, null);
                            //listDepositReqProcessor.GetSelectRow(ucRequestDeposit);
                        }
                    }
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void getDataDepositReq(long treatmentid)
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        HisDepositReqViewFilter filter = new HisDepositReqViewFilter();
        //        filter.BRANCH_ID = WorkPlace.GetBranchId();
        //        listDepositReq = new BackendAdapter(param).Get<List<V_HIS_DEPOSIT_REQ>>(HisRequestUriStore.HIS_DEPOSIT_REQ_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, null);


        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}


        //private void SetIcon()
        //{
        //    try
        //    {
        //        this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        private void InitListDepositReqGrid()
        {
            try
            {
                //var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //var langManager = Base.ResourceLangManager.LanguageUCExpMestChmsCreate;

                this.listDepositReqProcessor = new ListDepositRequestProcessor();
                ListDepositRequestInitADO ado = new ListDepositRequestInitADO();
                ado.ListDepositReqGrid_CustomUnboundColumnData = depositReqGrid__CustomUnboundColumnData;
                ado.ListDepositReqGrid_RowCellClick = Grid_RowCellClick;
                ado.ListDepositReqGrid_RowCellStyle = gridView_RowCellStyle;

                //ado.ListDepositReqGrid_KeyUp = Grid_KeyUp;
                ado.IsShowSearchPanel = false;
                ado.ListDepositReqColumn = new List<ListDepositRequestColumn>();

                //ListDepositRequestColumn colSTT = new ListDepositRequestColumn("STT", "STT", 40, false, true);
                //colSTT.VisibleIndex = 0;
                //colSTT.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListDepositReqColumn.Add(colSTT);

                //ListDepositRequestColumn colDepositReqCode = new ListDepositRequestColumn("Mã yêu cầu", "DEPOSIT_REQ_CODE", 100, false, true);
                //colDepositReqCode.VisibleIndex = 1;

                //ado.ListDepositReqColumn.Add(colDepositReqCode);
                ListDepositRequestColumn colTreatmentCode = new ListDepositRequestColumn("Mã điều trị", "TREATMENT_CODE", 80, false, true);
                colTreatmentCode.VisibleIndex = 2;
                colTreatmentCode.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListDepositReqColumn.Add(colTreatmentCode);
                ListDepositRequestColumn colPatientCode = new ListDepositRequestColumn("Mã bệnh nhân", "TDL_PATIENT_CODE", 80, false, true);
                colPatientCode.VisibleIndex = 3;
                colPatientCode.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListDepositReqColumn.Add(colPatientCode);

                ListDepositRequestColumn colPatientName = new ListDepositRequestColumn("Tên bệnh nhân", "TDL_PATIENT_NAME", 150, false, true);
                colPatientName.VisibleIndex = 4;
                colPatientName.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListDepositReqColumn.Add(colPatientName);

                ListDepositRequestColumn colAmount = new ListDepositRequestColumn("Số tiền", "AMOUNT_DISPLAY", 120, false, true);
                colAmount.VisibleIndex = 5;
                //colAmount.ColumnEdit.="btnDelete"
                //colAmount.ColumnEdit = "btnDelete";
                colAmount.Format = new DevExpress.Utils.FormatInfo();
                colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAmount.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colAmount.Format.FormatString = "#,##0.00";
                ado.ListDepositReqColumn.Add(colAmount);

                ListDepositRequestColumn colDOB = new ListDepositRequestColumn("Ngày sinh", "DOB_DISPLAY", 80, false, true);
                colDOB.VisibleIndex = 6;
                colDOB.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListDepositReqColumn.Add(colDOB);

                ListDepositRequestColumn colGender = new ListDepositRequestColumn("Giới tính", "TDL_PATIENT_GENDER_NAME", 80, false, true);
                colGender.VisibleIndex = 7;
                ado.ListDepositReqColumn.Add(colGender);

                ListDepositRequestColumn colRoomName = new ListDepositRequestColumn("Người yêu cầu", "REQUEST_USERNAME", 120, false, true);
                colRoomName.VisibleIndex = 8;
                ado.ListDepositReqColumn.Add(colRoomName);

                ListDepositRequestColumn colDepartName = new ListDepositRequestColumn("Khoa yêu cầu", "DEPARTMENT_NAME", 120, false, true);
                colDepartName.VisibleIndex = 9;
                ado.ListDepositReqColumn.Add(colDepartName);

                ListDepositRequestColumn colCreateTime = new ListDepositRequestColumn("Thời gian tạo", "CREATE_TIME_DISPLAY", 120, false, true);
                colCreateTime.VisibleIndex = 10;
                colCreateTime.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListDepositReqColumn.Add(colCreateTime);

                ListDepositRequestColumn colCreator = new ListDepositRequestColumn("Người tạo", "CREATOR", 80, false, true);
                colCreator.VisibleIndex = 11;
                ado.ListDepositReqColumn.Add(colCreator);

                ListDepositRequestColumn colModifyTime = new ListDepositRequestColumn("Thời gian sửa", "MODIFY_TIME_DISPLAY", 120, false, true);
                colModifyTime.VisibleIndex = 12;
                colModifyTime.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListDepositReqColumn.Add(colModifyTime);

                ListDepositRequestColumn colModifier = new ListDepositRequestColumn("Người sửa", "MODIFIER", 80, false, true);
                colModifier.VisibleIndex = 13;
                ado.ListDepositReqColumn.Add(colModifier);

                ado.ListDepositReq = listDepositReq;

                this.ucRequestDeposit = (UserControl)this.listDepositReqProcessor.Run(ado);
                if (this.ucRequestDeposit != null)
                {
                    this.panelControl1.Controls.Add(this.ucRequestDeposit);
                    this.ucRequestDeposit.Dock = DockStyle.Fill;
                }
                //if (listDepositReq != null)
                //{
                //    listDepositReq.FirstOrDefault();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void depositReqGrid__CustomUnboundColumnData(V_HIS_DEPOSIT_REQ data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        try
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                            //e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data.AMOUNT, ConfigApplications.NumberSeperator);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCare_CustomRowCellEdit(V_HIS_DEPOSIT_REQ data, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                //GridView View = sender as GridView;
                //if (e.RowHandle >= 0)
                //{
                //    //long careSumId = 0;
                //    //long careSumId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewCare.GetRowCellValue(e.RowHandle, "CARE_SUM_ID") ?? 0).ToString());
                //    var creator = data.CREATOR;
                //    var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //    if (e.Column.FieldName == "DELETE")
                //    {
                //        if (loginName.Equals(creator))
                //        {
                //            e.RepositoryItem = btnDeleteE;
                //        }
                //        else
                //        {
                //            e.RepositoryItem = btnDeleteD;
                //        }
                //    }
                //    if (e.Column.FieldName == "btn")
                //    {
                //        if (loginName.Equals(creator))
                //        {
                //            e.RepositoryItem = btnEditE;
                //        }
                //        else
                //        {
                //            e.RepositoryItem = btnEditD;
                //        }
                //    }

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void Grid_RowCellClick(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                LoadDataToForm(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToForm(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                if (data != null)
                {
                    currentdepositReq = data;
                    dtTransactionTime.DateTime = DateTime.Now;
                    txtAmount.Text = Inventec.Common.Number.Convert.NumberToString(data.AMOUNT, ConfigApplications.NumberSeperator);
                    txtDescription.Text = data.DESCRIPTION;
                    txtEditReqCode.Text = data.DEPOSIT_REQ_CODE;
                    this.action = GlobalVariables.ActionEdit;
                    //EnableControlChanged(action);
                    if (data.DEPOSIT_ID != null)
                    {
                        btnSave.Enabled = false;
                        btnSavePrint.Enabled = false;
                        //btnCancel.Enabled = true;
                        btnPrint.Enabled = true;
                        btnCreateQR.Enabled = false;
                    }
                    else if (data.DEPOSIT_ID == null)
                    {
                        //btnCancel.Enabled = false;
                        btnSave.Enabled = true;
                        btnSavePrint.Enabled = true;
                        btnPrint.Enabled = false;
                        btnCreateQR.Enabled = true;
                    }

                    //chọn lại sổ để update
                    SetDefaultAccountBookForUser();
                }
                else
                {
                    currentdepositReq = null;
                    txtAmount.Text = "";
                    txtDescription.Text = "";
                    txtEditReqCode.Text = "";
                    SpNumOrder.EditValue = null;
                    this.action = GlobalVariables.ActionAdd;
                    btnSave.Enabled = true;
                    btnSavePrint.Enabled = true;
                    btnPrint.Enabled = false;
                }
              
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_RowCellStyle(V_HIS_DEPOSIT_REQ data, RowCellStyleEventArgs e)
        {
            try
            {
                if (data != null && data.DEPOSIT_ID == null)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                else
                {
                    e.Appearance.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Save()
        {
            if (!btnSave.Enabled)
                return;
            btnSave_Click(null, null);
        }
        public void Search()
        {
            if (!btnSearch.Enabled)
                return;
            btnSearch_Click(null, null);
        }
        public void Print()
        {
            if (!btnPrint.Enabled)
                return;
            btnPrint_Click(null, null);
        }
        public void SavePrint()
        {
            if (!btnSavePrint.Enabled)
                return;
            btnSavePrint_Click(null, null);
        }
        public void Cancel()
        {
            //if (!btnCancel.Enabled)
            //    return;
            //btnCancel_Click(null, null);
        }
        public void HotkeyF2()
        {
            if (!txtReqCode.Enabled)
                return;
            txtReqCode_Click(null, null);
        }
    }
}
