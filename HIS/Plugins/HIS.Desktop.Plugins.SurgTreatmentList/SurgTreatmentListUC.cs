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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Data.Helpers.ExpressiveSortInfo;

namespace HIS.Desktop.Plugins.SurgTreatmentList
{
    public partial class SurgTreatmentListUC : HIS.Desktop.Utility.UserControlBase
    {
        #region ---Declare variable---
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private List<long> ClsServiceType = new List<long>() {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL,
        };

        private int lastRowHandle = -1;
        private DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        private DevExpress.Utils.ToolTipControlInfo lastInfo = null;

        List<ADO.SereServADO> listData;
        private List<HIS_PTTT_PRIORITY> ListPtttPriority;
        Dictionary<long, int> DicMapData = new Dictionary<long, int>();
        List<ADO.SearchADO> SelectedGatherdatas = new List<ADO.SearchADO>();
        List<ADO.SearchADO> SelectedFees = new List<ADO.SearchADO>();
        private bool isNotLoadWhileChangeControlStateInFirst;
        private List<ControlStateRDO> currentControlStateRDO;
        private Inventec.Desktop.Common.Modules.Module ModuleData;
        private ControlStateWorker controlStateWorker;
        private bool isInternalChange = false;
        bool isCheckAllGatherData = true;
        bool isCheckAllFee = true;
        #endregion
        public SurgTreatmentListUC()
        {
            InitializeComponent();
        }

        public SurgTreatmentListUC(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this.moduleData = moduleData;
        }

        private void SurgTreatmentListUC_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                InitControlState();

                //danh sach phong
                InitCboExecuteRoom();

                InitPtttPriorityCheck();
                InitPtttPriority();

                //load combo lay du lieu
                InitCheck(cboIs_Gather_Data, SelectionGridGathers);
                InitCombo(cboIs_Gather_Data);

                //load combo huong chi phi
                InitCheck(cboIs_Fee, SelectionGridFees);
                InitCombo(cboIs_Fee);

                ProcessColumnRole();

                SetFormatColumn();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToCotrol();

                //focus truong du lieu dau tien
                TxtKeyword.Focus();

                repositoryItemChkDisable.Enabled = false;
                SetEnableCombo();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region ---Click---
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToCotrol();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                RestCombo(cboIs_Fee);
                RestCombo(cboIs_Gather_Data);
                FillDataToCotrol();
                SetEnableCombo();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboExecuteRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboExecuteRoom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void TxtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region ---CheckedChanged---
        private void ChkOutTreat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChkOutTreat.Checked)
                {
                    ChkInTreat.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkInTreat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChkInTreat.Checked)
                {
                    ChkOutTreat.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkPT_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (ChkPT.Checked)
            //    {
            //        ChkTT.Checked = false;
            //        ChkCDHA.Checked = false;
            //        ChkNS.Checked = false;
            //        ChkSA.Checked = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void ChkTT_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (ChkTT.Checked)
            //    {
            //        ChkPT.Checked = false;
            //        ChkCDHA.Checked = false;
            //        ChkNS.Checked = false;
            //        ChkSA.Checked = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void ChkCDHA_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (ChkCDHA.Checked)
            //    {
            //        ChkPT.Checked = false;
            //        ChkTT.Checked = false;
            //        ChkNS.Checked = false;
            //        ChkSA.Checked = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void ChkNS_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (ChkNS.Checked)
            //    {
            //        ChkPT.Checked = false;
            //        ChkCDHA.Checked = false;
            //        ChkTT.Checked = false;
            //        ChkSA.Checked = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void ChkSA_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (ChkSA.Checked)
            //    {
            //        ChkPT.Checked = false;
            //        ChkCDHA.Checked = false;
            //        ChkNS.Checked = false;
            //        ChkTT.Checked = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void repositoryItemChkFee_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var chk = sender as DevExpress.XtraEditors.CheckEdit;
                if (chk == null) return;
                var row = (ADO.SereServADO)GridViewSereServ.GetFocusedRow();

                if (row != null)
                {
                    if (toggleSwitch1.IsOn)
                    {
                        row.Fee = chk.Checked;
                        toggleOnFee();
                    }
                    else
                    {
                        GridViewSereServ.SetRowCellValue(GridViewSereServ.FocusedRowHandle, GvSS_GcFee, chk.Checked);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemChkGatherData_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var chk = sender as DevExpress.XtraEditors.CheckEdit;
                if (chk == null) return;
                var row = (ADO.SereServADO)GridViewSereServ.GetFocusedRow();

                if (row != null)
                {
                    if (toggleSwitch1.IsOn)
                    {
                        row.GatherData = chk.Checked;
                        toggleOnGatherData();
                    }
                    else
                    {
                        GridViewSereServ.SetRowCellValue(GridViewSereServ.FocusedRowHandle, GvSS_GcGatherData, chk.Checked);
                    }                        
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void GridViewSereServ_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.SereServADO data = (ADO.SereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        //else if (e.Column.FieldName == GvSS_GcEndTime.FieldName)
                        //{
                        //    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.END_TIME ?? 0);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewEkip_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewSereServ_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (isInternalChange) return;
                var sereServADO = (ADO.SereServADO)GridViewSereServ.GetRow(e.RowHandle);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sereServADO___:", sereServADO));

                if (sereServADO != null)
                {
                    if (e.Column.FieldName == GvSS_GcFee.FieldName)
                    {
                        if (sereServADO.Fee && !CheckFeeAndGather(sereServADO, true)) return;
                        
                        bool success = UpdateRowData(sereServADO, true);
                        if (!success)
                        {
                            isInternalChange = true;
                            sereServADO.Fee = !sereServADO.Fee; 
                            GridViewSereServ.SetRowCellValue(e.RowHandle, GvSS_GcFee, sereServADO.Fee);
                            isInternalChange = false;
                        }
                    }
                    else if (e.Column.FieldName == GvSS_GcGatherData.FieldName)
                    {
                        if (sereServADO.GatherData && !CheckFeeAndGather(sereServADO, false)) return;

                        bool success = UpdateRowData(sereServADO, false);
                        if (!success)
                        {
                            isInternalChange = true;
                            sereServADO.GatherData = !sereServADO.GatherData;
                            GridViewSereServ.SetRowCellValue(e.RowHandle, GvSS_GcGatherData, sereServADO.GatherData);
                            isInternalChange = false;
                        }
                    }
                    GridControlSereServ.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool CheckFeeAndGather(ADO.SereServADO sereServADO, bool isFee)
        {
            bool rs = true;
            try
            {
                if (sereServADO.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT && XtraMessageBox.Show("Trạng thái y lệnh chưa Hoàn thành. Bạn có muốn Lấy dữ liệu?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    rs = isFee ? sereServADO.Fee = false : sereServADO.GatherData = false;

                else if (sereServADO.EKIP_ID == null && XtraMessageBox.Show("Y lệnh không có thông tin kíp thực hiện. Hệ thống sẽ tự tạo kíp thực hiện với vai trò là phẫu thuật viên chính. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    rs = isFee ? sereServADO.Fee = false : sereServADO.GatherData = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return rs;
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == GridControlSereServ)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = GridControlSereServ.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            ADO.SereServADO dataRow = (ADO.SereServADO)GridViewSereServ.GetRow(info.RowHandle);
                            if (dataRow == null) dataRow = new ADO.SereServADO();

                            string text = "";
                            if (info.Column.FieldName == GvSS_GcFee.FieldName)
                                text = Inventec.Common.Resource.Get.Value(
                                    "IVT_LANGUAGE_KEY_UC_SURG_TREATMENT_LIST__TOOLTIP__FEE",
                                    Resources.ResourceLanguageManager.LanguageResource,
                                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                            else if (info.Column.FieldName == GvSS_GcGatherData.FieldName)
                                text = Inventec.Common.Resource.Get.Value(
                                    "IVT_LANGUAGE_KEY_UC_SURG_TREATMENT_LIST__TOOLTIP__GATHER_DATA",
                                    Resources.ResourceLanguageManager.LanguageResource,
                                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            else if (info.Column.FieldName == GvSS_GcExecuteRoleName1.FieldName)
                            {
                                text = dataRow.REMUNERATION_PRICE_1;
                            }
                            else if (info.Column.FieldName == GvSS_GcExecuteRoleName2.FieldName)
                            {
                                text = dataRow.REMUNERATION_PRICE_2;
                            }
                            else if (info.Column.FieldName == GvSS_GcExecuteRoleName3.FieldName)
                            {
                                text = dataRow.REMUNERATION_PRICE_3;
                            }
                            else if (info.Column.FieldName == GvSS_GcExecuteRoleName4.FieldName)
                            {
                                text = dataRow.REMUNERATION_PRICE_4;
                            }
                            else if (info.Column.FieldName == GvSS_GcExecuteRoleName5.FieldName)
                            {
                                text = dataRow.REMUNERATION_PRICE_5;
                            }
                            else if (info.Column.FieldName == GvSS_GcExecuteRoleName6.FieldName)
                            {
                                text = dataRow.REMUNERATION_PRICE_6;
                            }
                            else if (info.Column.FieldName == GvSS_GcExecuteRoleName7.FieldName)
                            {
                                text = dataRow.REMUNERATION_PRICE_7;
                            }
                            else if (info.Column.FieldName == GvSS_GcExecuteRoleName8.FieldName)
                            {
                                text = dataRow.REMUNERATION_PRICE_8;
                            }
                            else if (info.Column.FieldName == GvSS_GcExecuteRoleName9.FieldName)
                            {
                                text = dataRow.REMUNERATION_PRICE_9;
                            }
                            else if (info.Column.FieldName == GvSS_GcExecuteRoleName10.FieldName)
                            {
                                text = dataRow.REMUNERATION_PRICE_10;
                            }

                            lastInfo = new DevExpress.Utils.ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewSereServ_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (ADO.SereServADO)GridViewSereServ.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == GvSS_GcFee.FieldName)
                        {
                            if (GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null && GlobalVariables.AcsAuthorizeSDO.ControlInRoles.Exists(o => o.CONTROL_CODE == "HIS000019"))
                            {
                                e.RepositoryItem = repositoryItemChkFee;
                            }
                            else
                                e.RepositoryItem = repositoryItemChkDisable;
                        }
                        else if (e.Column.FieldName == GvSS_GcGatherData.FieldName)
                        {
                            if (GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null && GlobalVariables.AcsAuthorizeSDO.ControlInRoles.Exists(o => o.CONTROL_CODE == "HIS000020"))
                            {
                                e.RepositoryItem = repositoryItemChkGatherData;
                            }
                            else
                                e.RepositoryItem = repositoryItemChkDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPtttPriorityName_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {

            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_PTTT_PRIORITY rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.PTTT_PRIORITY_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboPtttPriorityName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboIs_Gather_Data.Focus();
                    cboIs_Gather_Data.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region ---Even Combo cboIs_Fee and cboIs_Gather
        private void SelectionGridFees(object sender, EventArgs e)
        {
            try
            {
                SelectedFees = new List<ADO.SearchADO>();
                foreach (ADO.SearchADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        SelectedFees.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGridGathers(object sender, EventArgs e)
        {
            try
            {
                SelectedGatherdatas = new List<ADO.SearchADO>();
                foreach (ADO.SearchADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        SelectedGatherdatas.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RestCombo(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection grid = cbo.Properties.Tag as GridCheckMarksSelection;
                if (grid != null)
                {
                    grid.SelectAll(cbo.Properties.DataSource);
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIs_Gather_Data_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string display = "";
                foreach (var item in SelectedGatherdatas)
                {
                    if (display.Trim().Length > 0)
                    {
                        display += ", " + item.Display;
                    }
                    else
                        display = item.Display;
                }
                e.DisplayText = display;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIs_Fee_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string display = "";
                foreach (var item in SelectedFees)
                {
                    if (display.Trim().Length > 0)
                    {
                        display += ", " + item.Display;
                    }
                    else
                        display = item.Display;
                }
                e.DisplayText = display;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIs_Gather_Data_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboIs_Fee.Focus();
                    cboIs_Fee.SelectAll();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIs_Fee_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    DtIntructionTimeFrom.Focus();
                    DtIntructionTimeFrom.SelectAll();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableCombo()
        {
            try
            {
                cboIs_Gather_Data.Enabled = false;
                cboIs_Fee.Enabled = false;
                cboIs_Fee.Enabled = true;
                cboIs_Gather_Data.Enabled = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkStatus_CheckedChanged()
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPending.Name && o.MODULE_LINK == moduleData.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { chkPending.Checked, chkInProgress.Checked, chkCompleted.Checked });
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPending.Name;
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { chkPending.Checked, chkInProgress.Checked, chkCompleted.Checked });
                    csAddOrUpdate.MODULE_LINK = moduleData.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleData.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPending.Name)
                        {
                            var check = Newtonsoft.Json.JsonConvert.DeserializeObject<bool[]>(item.VALUE);
                            chkPending.Checked = check[0];
                            chkInProgress.Checked = check[1];
                            chkCompleted.Checked = check[2];
                        }
                        else if (item.KEY == toggleSwitch1.Name)
                        {
                            toggleSwitch1.IsOn = item.VALUE == "1";
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPending_CheckedChanged(object sender, EventArgs e)
        {
            chkStatus_CheckedChanged();
        }

        private void chkInProgress_CheckedChanged(object sender, EventArgs e)
        {
            chkStatus_CheckedChanged();
        }

        private void chkCompleted_CheckedChanged(object sender, EventArgs e)
        {
            chkStatus_CheckedChanged();
        }

        private void GridControlSereServ_Click(object sender, EventArgs e)
        {

        }

        private void GridViewSereServ_MouseDown(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            GridHitInfo hi = view.CalcHitInfo(e.Location);
            var listData = GridControlSereServ.DataSource as List<ADO.SereServADO>;
            if (!toggleSwitch1.IsOn)
            {
                this.GvSS_GcGatherData.Image = null;
                this.GvSS_GcFee.Image = null;
                return;
            }
            if (hi.HitTest == GridHitTest.Column)
            {
                if (hi.Column.FieldName == GvSS_GcGatherData.FieldName)
                {
                    if (listData != null && listData.Count > 0)
                    {
                        GridViewSereServ.BeginUpdate();
                        if (isCheckAllGatherData)
                        {
                            isCheckAllGatherData = false;
                            foreach (var item in listData)
                            {
                                item.GatherData = true;
                            }
                            this.GvSS_GcGatherData.Image = this.imageListCheck.Images[3];
                        }
                        else
                        {
                            isCheckAllGatherData = true;
                            foreach (var item in listData)
                            {
                                item.GatherData = false;
                            }
                            this.GvSS_GcGatherData.Image = this.imageListCheck.Images[4];
                        }
                        GridViewSereServ.EndUpdate();
                    }
                }else if (hi.Column.FieldName == GvSS_GcFee.FieldName)
                {
                    if (listData != null && listData.Count > 0)
                    {
                        GridViewSereServ.BeginUpdate();
                        if (isCheckAllFee)
                        {
                            isCheckAllFee = false;
                            foreach (var item in listData)
                            {
                                item.Fee = true;
                            }
                            this.GvSS_GcFee.Image = this.imageListCheck.Images[3];
                        }
                        else
                        {
                            isCheckAllFee = true;
                            foreach (var item in listData)
                            {
                                item.Fee = false;
                            }
                            this.GvSS_GcFee.Image = this.imageListCheck.Images[4];
                        }
                        GridViewSereServ.EndUpdate();
                    }
                }
            }
        }

        private void toggleSwitch1_Toggled(object sender, EventArgs e)
        {
            try
            {
                toggleChanged();
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == toggleSwitch1.Name && o.MODULE_LINK == moduleData.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null) 
                { 
                    csAddOrUpdate.VALUE = (toggleSwitch1.IsOn ? "1" : "0");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.MODULE_LINK = moduleData.ModuleLink;
                    csAddOrUpdate.KEY = toggleSwitch1.Name;
                    csAddOrUpdate.VALUE = (toggleSwitch1.IsOn ? "1" : "");
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool success = false;
            CommonParam param = new CommonParam();
            //var listData = GridControlSereServ.DataSource as List<ADO.SereServADO>;
            var gatherDatas = listData.Where(x => x.GatherData).Select(x => x.ID).ToList();
            var unGatherDatas = listData.Where(x => !x.GatherData).Select(x => x.ID).ToList();
            var fees = listData.Where(x => x.Fee).Select(x => x.ID).ToList();
            var unFees = listData.Where(x => !x.Fee).Select(x => x.ID).ToList();

            SetFeeAndGatherDataSDO sdo = new SetFeeAndGatherDataSDO
            {
                GatherDatas = gatherDatas,
                Fees = fees,
                UnGatherDatas = unGatherDatas,
                UnFees = unFees
            };

            var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERE_SERV_EXT>("api/HisSereServExt/SetFeeAndGatherData", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
            if (apiResult != null)
            {
                success = true;
            }
            else
            {
                success = false;
            }
            MessageManager.Show(this.ParentForm, param, success);
        }
        private void toggleChanged()
        {
            if (toggleSwitch1.IsOn)
            {
                toggleSwitch1.ToolTip = "Cập nhật nhiều dịch vụ";
                btnSave.Enabled = true;
                toggleOnGatherData();
                toggleOnFee();
            }
            else
            {
                toggleSwitch1.ToolTip = "Cập nhật từng dịch vụ";
                this.GvSS_GcGatherData.Image = null;
                this.GvSS_GcFee.Image = null;
                btnSave.Enabled = false;
                btnSave.AppearanceDisabled.BackColor = Color.LightGray;
                FillDataToCotrol();
            }
        }
        private void toggleOnFee()
        {
            var listData = GridControlSereServ.DataSource as List<ADO.SereServADO>;
            bool hasCheck = false;
            bool isAll = false;

            if (listData != null && listData.Count > 0)
            {
                var listCheck = listData.Where(o => o.Fee).ToList();
                if (listCheck != null && listCheck.Count > 0)
                {
                    if (listCheck.Count == listData.Count)
                    {
                        isAll = true;
                    }
                    else
                    {
                        hasCheck = true;
                    }
                } 
            }

            if (isAll)
            {
                isCheckAllFee = false;
                this.GvSS_GcFee.Image = this.imageListCheck.Images[3];
            }
            else if (hasCheck)
            {
                //this.GvSS_GcFee.Image = this.imageListCheck.Images[5];
                this.GvSS_GcFee.Image = this.imageListCheck.Images[4];
            }
            else
            {
                isCheckAllFee = true;
                this.GvSS_GcFee.Image = this.imageListCheck.Images[4];
            }
        }
        private void toggleOnGatherData()
        {
            var listData = GridControlSereServ.DataSource as List<ADO.SereServADO>;
            bool hasCheck = false;
            bool isAll = false;
            if (listData != null && listData.Count > 0)
            {
                var listCheck = listData.Where(o => o.GatherData).ToList();
                if (listCheck != null && listCheck.Count > 0)
                {
                    if (listCheck.Count == listData.Count)
                    {
                        isAll = true;
                    }
                    else
                    {
                        hasCheck = true;
                    }
                }
            }
            if (isAll)
            {
                isCheckAllGatherData = false;
                this.GvSS_GcGatherData.Image = this.imageListCheck.Images[3];
            }
            else if (hasCheck)
            {
                //this.GvSS_GcGatherData.Image = this.imageListCheck.Images[5];
                this.GvSS_GcGatherData.Image = this.imageListCheck.Images[4];
            }
            else
            {
                isCheckAllGatherData = true;
                this.GvSS_GcGatherData.Image = this.imageListCheck.Images[4];
            }
        }
    }
}
