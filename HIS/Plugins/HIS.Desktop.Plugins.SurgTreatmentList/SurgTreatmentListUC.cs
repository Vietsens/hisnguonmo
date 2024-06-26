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
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.ConfigApplication;

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
                    GridViewSereServ.SetRowCellValue(GridViewSereServ.FocusedRowHandle, GvSS_GcFee, chk.Checked);
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
                    GridViewSereServ.SetRowCellValue(GridViewSereServ.FocusedRowHandle, GvSS_GcGatherData, chk.Checked);
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
                var sereServADO = (ADO.SereServADO)GridViewSereServ.GetRow(e.RowHandle);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sereServADO___:", sereServADO));

                if (sereServADO != null)
                {
                    if (e.Column.FieldName == GvSS_GcFee.FieldName)
                    {
                        if (sereServADO.Fee && !CheckFeeAndGather(sereServADO, true)) return;
                        UpdateRowData(sereServADO, true);
                    }
                    else if (e.Column.FieldName == GvSS_GcGatherData.FieldName)
                    {
                        if (sereServADO.GatherData && !CheckFeeAndGather(sereServADO, false)) return;
                        UpdateRowData(sereServADO, false);
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
    }
}
