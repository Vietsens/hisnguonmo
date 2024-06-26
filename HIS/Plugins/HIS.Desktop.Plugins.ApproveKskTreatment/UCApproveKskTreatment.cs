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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Threading;
using System.IO;
using Inventec.Common.RichEditor.Base;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.SDO;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ApproveKskTreatment.Base;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Common;
using DevExpress.XtraBars;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.ApproveKskTreatment
{
    public partial class UCApproveKskTreatment : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private string LoggingName = "";
        MOS.EFMODEL.DataModels.V_HIS_ROOM _Room = null;
        long roomId = 0;
        long roomTypeId = 0;
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo;
        GridColumn lastColumn = null;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;

        BarManager baManager = null;
        PopupMenuProcessor popupMenuProcessor = null;
        List<V_HIS_KSK_CONTRACT> kskContractList;
        List<TreatmentADO> treatmentAdoList = null;
        bool isCheckAll = true;
        #endregion

        #region Construct
        public UCApproveKskTreatment()
        {
            InitializeComponent();
            try
            {
                gridColumnCheckTreatment.Image = imageListIcon.Images[6];
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                gridControl.ToolTipController = this.toolTipController;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCApproveKskTreatment(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                gridColumnCheckTreatment.Image = imageListIcon.Images[6];
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                gridControl.ToolTipController = this.toolTipController;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.roomId = _moduleData.RoomId;
                this.roomTypeId = _moduleData.RoomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisAggrExpMestList_Load(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
                //Gan ngon ngu
                //LoadKeysFromlanguage();

                LoadDataKskContract();

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
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //filter
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.btnUnApporval.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                //gridView
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcExpMestCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__EXP_MEST_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__MODIFIER",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.GcUseTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__USE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                //grid button
                this.ButtonApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonApprovalEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonDisApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonDisApprovalEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonDiscardDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonDiscardEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonEditDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonEditEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonExportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_EXPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonExportEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_EXPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonReApproval.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonReApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
                this.ButtonViewDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__GR_BUTTON_VIEW",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataKskContract()
        {
            try
            {
                MOS.Filter.HisKskContractViewFilter filter = new HisKskContractViewFilter();
                filter.IS_ACTIVE = 1;
                kskContractList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("HisKskContract/GetView kskContractList: ", kskContractList));
                kskContractList = kskContractList != null && kskContractList.Count > 0 ? kskContractList.Where(o => o.IS_REQUIRED_APPROVAL == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboKskContract, kskContractList, controlEditorADO);

                var contractDefault = kskContractList.FirstOrDefault();
                cboKskContract.EditValue = contractDefault.ID;
                txtKskContractCode.Text = contractDefault.KSK_CONTRACT_CODE;

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
                _Room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                txtKeyWord.Text = "";
                txtKeyWord.Focus();
                cboStatus.SelectedIndex = 1;

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
                if (cboKskContract.EditValue == null)
                {
                    MessageBox.Show("Bạn chưa chọn hợp đồng khám sức khỏe", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                WaitingManager.Show();
                gridColumnCheckTreatment.Image = imageListIcon.Images[6];
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
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4>> apiResult = null;
                MOS.Filter.HisTreatmentView4Filter filter = new MOS.Filter.HisTreatmentView4Filter();
                SetFilter(ref filter);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("HisTreatment/GetView4 filter ", filter));
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4>>
                    ("api/HisTreatment/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        treatmentAdoList = new List<TreatmentADO>();
                        foreach (var item in data)
                        {
                            TreatmentADO treatment = new TreatmentADO(item);
                            treatmentAdoList.Add(treatment);
                        }

                        gridControl.DataSource = treatmentAdoList;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
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

        private void SetFilter(ref MOS.Filter.HisTreatmentView4Filter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (cboKskContract.EditValue != null)
                {
                    filter.TDL_KSK_CONTRACT_ID = (long)cboKskContract.EditValue;
                }

                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter = new HisTreatmentView4Filter();
                    filter.TREATMENT_CODE__EXACT = code;
                }
                else if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }
                else
                {
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                    filter.PATIENT_NAME = txtPatientName.Text.Trim();
                    if (cboStatus.SelectedIndex == 0)
                    {
                        filter.IS_KSK_APPROVE = true;
                    }
                    else if (cboStatus.SelectedIndex == 1)
                    {
                        filter.IS_KSK_APPROVE = false;
                    }

                }
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

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
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

        private void btnUnApproval_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                List<TreatmentADO> dataSource = (List<TreatmentADO>)gridControl.DataSource;
                if (dataSource != null && dataSource.Count > 0)
                {
                    var treatmentUnAprovalList = dataSource.Where(o => o.CheckTreatment && o.IS_PAUSE != 1 && o.IS_LOCK_HEIN != 1 && o.IS_LOCK_FEE != 1 && o.IS_KSK_APPROVE == 1).ToList();
                    if (treatmentUnAprovalList != null && treatmentUnAprovalList.Count > 0)
                    {
                        WaitingManager.Show();
                        List<long> treatmentIdList = new List<long>();
                        treatmentIdList.AddRange(treatmentUnAprovalList.Select(o => o.ID).Distinct().ToList());
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<List<HIS_TREATMENT>>
                            ("api/HisTreatment/KskUnapprove", ApiConsumer.ApiConsumers.MosConsumer, treatmentIdList, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                    else
                    {
                        MessageBox.Show("Không có hồ sơ để thực hiện hủy duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Không có hồ sơ để thực hiện hủy duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    V_HIS_TREATMENT_4 treatment = (V_HIS_TREATMENT_4)gridView.GetRow(e.RowHandle);
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();

                    if (e.Column.FieldName == "APPROVAL_DISPLAY") //duyet
                    {
                        if (treatment != null && (treatment.IS_PAUSE == 1 || treatment.IS_LOCK_HEIN == 1 || treatment.IS_LOCK_FEE == 1))
                        {
                            e.RepositoryItem = ButtonApprovalDisable;
                        }
                        else
                            if (treatment != null && treatment.IS_KSK_APPROVE != 1)// TODO
                            {
                                e.RepositoryItem = ButtonApprovalEnable;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonDisApprovalEnable;
                            }
                    }
                    else if (e.Column.FieldName == "CheckTreatment")
                    {
                        if (treatment != null && (treatment.IS_PAUSE == 1 || treatment.IS_LOCK_HEIN == 1 || treatment.IS_LOCK_FEE == 1))
                        {
                            e.RepositoryItem = repositoryItemCheckEditTreatment_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheckEditTreatment_Enable;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4 data = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "EXP_MEST_STT_ICON")// trạng thái
                        {
                            if (data.IS_KSK_APPROVE == 1)
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            else
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "EXP_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FUND_FROM_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                        {
                            //e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "USE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.Column.FieldName != "EXP_MEST_STT_ICON")
                    {
                        return;
                    }

                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            string isKskApproval = (view.GetRowCellValue(lastRowHandle, "IS_KSK_APPROVE") ?? "").ToString();
                            if (isKskApproval == "1")
                            {
                                text = "Đã duyệt";
                            }
                            else
                            {
                                text = "Chưa duyệt";
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

        private string DisplayName(string loginname, string username)
        {
            string value = "";
            try
            {
                if (String.IsNullOrEmpty(loginname) && String.IsNullOrEmpty(username))
                {
                    value = "";
                }
                else if (loginname != "" && username == "")
                {
                    value = loginname;
                }
                else if (loginname == "" && username != "")
                {
                    value = username;
                }
                else if (loginname != "" && username != "")
                {
                    value = string.Format("{0} - {1}", loginname, username);
                }
                return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return value;
            }
        }
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

        public void ApprovalKskContract()
        {
            try
            {
                btnApproval_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UnApprovalKskContract()
        {
            try
            {
                btnUnApproval_Click(null, null);
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Report
        private void btnExportCodeList_Click(object sender, EventArgs e)
        {
            try
            {
                Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CreateReport));
                thread.Priority = ThreadPriority.Normal;
                thread.IsBackground = true;
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                try
                {
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateReport()
        {
            try
            {
                List<string> expCode = new List<string>();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachCacMaPhieuLinh.xlsx");

                //chọn đường dẫn
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayBieuMauIn, templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    //getdata
                    GetDataProcessor(ref expCode);

                    ProcessData(expCode, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.TaiThanhCong);

                                if (MessageBox.Show(Resources.ResourceMessage.BanCoMuonMoFile,
                                    Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.XuLyThatBai);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<string> expCode, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                List<Base.ExportListCodeRDO> listRdo = new List<Base.ExportListCodeRDO>();
                Dictionary<string, object> singleValueDictionary = new Dictionary<string, object>();

                if (expCode != null && expCode.Count > 0)
                {
                    Dictionary<int, List<string>> dicExpCode = new Dictionary<int, List<string>>();

                    int count = expCode.Count;
                    int max = count / 6;
                    int size = count % 6;
                    string emty = "";

                    if (count > 31)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (i != 5)
                            {
                                dicExpCode[i] = new List<string>();
                                dicExpCode[i].AddRange(expCode.GetRange(0, (size <= 0 ? max : max + 1)));
                                expCode.RemoveRange(0, (size <= 0 ? max : max + 1));
                            }
                            else
                                dicExpCode.Add(i, expCode);

                            if (dicExpCode[i].Count < dicExpCode[0].Count)
                            {
                                int loop = dicExpCode[0].Count - dicExpCode[i].Count;
                                for (int j = 0; j < loop; j++)
                                {
                                    dicExpCode[i].Add(emty);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (i != 5)
                            {
                                dicExpCode[i] = new List<string>();
                                dicExpCode[i].AddRange(expCode.GetRange(0, (size <= 0 ? max : max + 1)));
                                expCode.RemoveRange(0, (size <= 0 ? max : max + 1));
                                size--;
                            }
                            else
                                dicExpCode.Add(i, expCode);

                            if (dicExpCode[i].Count < dicExpCode[0].Count)
                            {
                                dicExpCode[i].Add(emty);
                            }
                        }
                    }

                    for (int i = 0; i < dicExpCode[0].Count; i++)
                    {
                        Base.ExportListCodeRDO a = new Base.ExportListCodeRDO();
                        a.EXPORT_CODE1 = dicExpCode[0][i];
                        a.EXPORT_CODE2 = dicExpCode[1][i];
                        a.EXPORT_CODE3 = dicExpCode[2][i];
                        a.EXPORT_CODE4 = dicExpCode[3][i];
                        a.EXPORT_CODE5 = dicExpCode[4][i];
                        a.EXPORT_CODE6 = dicExpCode[5][i];

                        listRdo.Add(a);
                    }
                }
                singleTag.AddSingleKey(store, "TYPE", "THỰC XUẤT");
                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(singleValueDictionary);
                singleTag.ProcessData(store, singleValueDictionary);

                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "List", listRdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }

        private void GetDataProcessor(ref List<string> expCode)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentView4Filter expFilter = new MOS.Filter.HisTreatmentView4Filter();
                expFilter.DATA_DOMAIN_FILTER = true;
                expFilter.WORKING_ROOM_ID = roomId;

                var exportList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumer.ApiConsumers.MosConsumer, expFilter, param);
            }
            catch (Exception ex)
            {
                expCode = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        //private void Btn_HuyThucXuat_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        bool success = false;
        //        CommonParam param = new CommonParam();
        //        MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4 row = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)gridView.GetFocusedRow();
        //        if (row != null)
        //        {

        //            WaitingManager.Show();
        //            HisExpMestSDO sdo = new HisExpMestSDO();
        //            sdo.ExpMestId = row.ID;
        //            sdo.ReqRoomId = this.roomId;
        //            //sdo.IsFinish = true;
        //            var apiresult = new Inventec.Common.Adapter.BackendAdapter
        //                (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
        //                ("api/HisExpMest/AggrUnexport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
        //            if (apiresult != null)
        //            {
        //                success = true;
        //                FillDataToGrid();
        //            }
        //            WaitingManager.Hide();
        //            #region Show message
        //            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
        //            #endregion

        //            #region Process has exception
        //            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
        //            #endregion

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //        WaitingManager.Hide();
        //    }
        //}

        private void btnInTraDoiTongHop_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_TREATMENT_4> _ExpMestTraDoiChecks = new List<V_HIS_TREATMENT_4>();
                if (gridView.RowCount > 0)
                {
                    for (int i = 0; i < gridView.SelectedRowsCount; i++)
                    {
                        if (gridView.GetSelectedRows()[i] >= 0)
                        {
                            _ExpMestTraDoiChecks.Add((V_HIS_TREATMENT_4)gridView.GetRow(gridView.GetSelectedRows()[i]));
                        }
                    }
                }
                if (_ExpMestTraDoiChecks != null && _ExpMestTraDoiChecks.Count > 0)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(_ExpMestTraDoiChecks);
                        listArgs.Add((long)5);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        if (extenceInstance.GetType() == typeof(bool))
                        {
                            return;
                        }
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var treatmentData = (V_HIS_TREATMENT_4)gridView.GetRow(hi.RowHandle);
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (treatmentData != null)
                        {
                            if (hi.Column.FieldName == "APPROVAL_DISPLAY")
                            {
                                //#region ----- APPROVAL_DISPLAY -----
                                //try
                                //{
                                //    List<V_HIS_TREATMENT_4> expMestChilds = this.GetChildExpMestFromAggExpMest(ExpMestData.ID);
                                //    List<long> expMestGetIds = new List<long>();
                                //    expMestGetIds.Add(ExpMestData.ID);
                                //    if (expMestChilds != null && expMestChilds.Count() > 0)
                                //    {
                                //        expMestGetIds.AddRange(expMestChilds.Select(o => o.ID).ToList());
                                //    }

                                //    // get expMestMedicine
                                //    MOS.Filter.HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                                //    filter.EXP_MEST_IDs = expMestGetIds;
                                //    var expMestMedicines = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MEDICINE>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                                //    if (expMestMedicines != null && expMestMedicines.Count() > 0)
                                //    {
                                //        string message = "";
                                //        foreach (var item in expMestMedicines)
                                //        {
                                //            var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.TDL_MEDICINE_TYPE_ID && o.IS_STAR_MARK == 1);
                                //            if (medicineType != null)
                                //            {
                                //                message += medicineType.MEDICINE_TYPE_NAME + " số lượng: " + item.AMOUNT + "; ";
                                //            }
                                //        }

                                //        if (!String.IsNullOrEmpty(message))
                                //        {
                                //            message = String.Format("Phiếu lĩnh có thuốc * gồm: {0} \nBạn có đồng ý duyệt?", message);
                                //            if (MessageBox.Show(message, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                //                return;
                                //        }
                                //    }
                                //    bool success = false;
                                //    CommonParam param = new CommonParam();
                                //    WaitingManager.Show();
                                //    HisExpMestSDO sdo = new HisExpMestSDO();
                                //    sdo.ExpMestId = ExpMestData.ID;
                                //    sdo.ReqRoomId = this.roomId;
                                //    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                //        (param).Post<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>
                                //        ("api/HisExpMest/AggrApprove", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                //    if (apiresult != null && apiresult.Count > 0)
                                //    {
                                //        success = true;
                                //        FillDataToGrid();
                                //    }
                                //    WaitingManager.Hide();
                                //    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                //}
                                //catch (Exception ex)
                                //{
                                //    Inventec.Common.Logging.LogSystem.Error(ex);
                                //    WaitingManager.Hide();
                                //}
                                //#endregion
                            }
                        }
                    }

                    //if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                    //{
                    //    view.FocusedRowHandle = hi.RowHandle;
                    //    view.FocusedColumn = hi.Column;
                    //    view.ShowEditor();
                    //    DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                    //    DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                    //    Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                    //    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    //    Rectangle gridGlyphRect =
                    //        new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                    //         viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                    //         glyphRect.Width,
                    //         glyphRect.Height);
                    //    if (!gridGlyphRect.Contains(e.Location))
                    //    {
                    //        view.CloseEditor();
                    //        if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                    //        {
                    //            view.SelectCell(hi.RowHandle, hi.Column);
                    //        }
                    //        else
                    //        {
                    //            view.UnselectCell(hi.RowHandle, hi.Column);
                    //        }
                    //    }
                    //else
                    //{
                    //    var dataRow = (TreatmentADO)gridView.GetRow(hi.RowHandle);
                    //    if (dataRow != null && dataRow.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST && (dataRow.AGGR_IMP_MEST_ID == null || dataRow.AGGR_IMP_MEST_ID <= 0))
                    //    {
                    //        checkEdit.Checked = !checkEdit.Checked;
                    //        view.CloseEditor();
                    //        if (this._ImpMestADOs != null && this._ImpMestADOs.Count > 0)
                    //        {
                    //            var dataChecks = this._ImpMestADOs.Where(p => p.IsCheck != true && p.AGGR_IMP_MEST_ID == null && p.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST).ToList();
                    //            if (dataChecks != null && dataChecks.Count > 0)
                    //            {
                    //                gridColumnCheck.Image = imageListIcon.Images[6];
                    //                isCheckAll = false;
                    //            }
                    //            else
                    //            {
                    //                gridColumnCheck.Image = imageListIcon.Images[5];
                    //                isCheckAll = true;
                    //            }
                    //        }
                    //    }
                    //}
                    //(e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                    //}
                }

                if (hi.HitTest == GridHitTest.Column)
                {
                    if (hi.Column.FieldName == "CheckTreatment")
                    {
                        gridColumnCheckTreatment.Image = imageListIcon.Images[5];
                        gridView.BeginUpdate();
                        if (this.treatmentAdoList == null)
                            this.treatmentAdoList = new List<TreatmentADO>();
                        if (isCheckAll == true)
                        {
                            foreach (var item in this.treatmentAdoList)
                            {
                                item.CheckTreatment = true;
                            }
                            isCheckAll = false;
                        }
                        else
                        {
                            gridColumnCheckTreatment.Image = imageListIcon.Images[6];
                            foreach (var item in this.treatmentAdoList)
                            {
                                item.CheckTreatment = false;
                            }
                            isCheckAll = true;
                        }
                        gridView.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    var expMest = (V_HIS_TREATMENT_4)gridView.GetFocusedRow();
                    if (this.baManager == null)
                    {
                        this.baManager = new BarManager();
                        this.baManager.Form = this;
                    }
                    if (expMest != null)
                    {
                        this.popupMenuProcessor = new PopupMenuProcessor(expMest, this.baManager, MouseRightItemClick);
                        this.popupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MouseRightItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem))
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.PhieuCongKhaiTheoBN:
                            this.OnClickInPhieuCongKhaiTheoBenhNhan();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OnClickInPhieuCongKhaiTheoBenhNhan()
        {
            try
            {
                InPhieuCongKhaiTheoBN();
                //Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                //richEditorMain.RunPrintTemplate("Mps000262", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        //private bool DelegateRunPrinter(string printTypeCode, string fileName)
        //{
        //    bool result = false;
        //    try
        //    {
        //        switch (printTypeCode)
        //        {
        //            case "Mps000262":
        //                InPhieuCongKhaiTheoBN(printTypeCode, fileName, ref result);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }

        //    return result;
        //}

        // lấy các phiếu con từ phiếu lĩnh được chọn
        List<V_HIS_TREATMENT_4> GetChildExpMestFromAggExpMest(long AggExpMestId)
        {
            List<V_HIS_TREATMENT_4> result = new List<V_HIS_TREATMENT_4>();
            try
            {
                if (AggExpMestId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentView4Filter expMestViewFilter = new HisTreatmentView4Filter();

                    result = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param);
                }
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_TREATMENT_4>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InPhieuCongKhaiTheoBN()
        {
            try
            {
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineTemps = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialTemps = new List<V_HIS_EXP_MEST_MATERIAL>();
                var AggExpMest = (V_HIS_TREATMENT_4)gridView.GetFocusedRow();

                List<V_HIS_TREATMENT_4> expMestCheckeds = new List<V_HIS_TREATMENT_4>();
                CommonParam param = new CommonParam();
                expMestCheckeds = GetChildExpMestFromAggExpMest(AggExpMest.ID);
                if (expMestCheckeds == null || expMestCheckeds.Count == 0)
                {
                    return;
                }
                //List<long> expMestIds = expMestCheckeds.Select(o => o.ID).ToList();
                //// nếu là load lên mặc định (check all các phiếu xuất)
                //MOS.Filter.HisExpMestMedicineViewFilter filterMedicine = new HisExpMestMedicineViewFilter();
                //filterMedicine.EXP_MEST_IDs = expMestIds;

                //expMestMedicineTemps = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filterMedicine, param);

                //MOS.Filter.HisExpMestMedicineViewFilter filterMaterial = new HisExpMestMedicineViewFilter();
                //filterMaterial.EXP_MEST_IDs = expMestIds;
                //expMestMaterialTemps = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filterMaterial, param);

                String message = "";

                //foreach (var item in expMestCheckeds)
                //{
                //    if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL)
                //    {
                //        message += item.EXP_MEST_CODE + "; ";
                //    }
                //}

                //if (expMestCheckeds == null || expMestCheckeds.Count() == 0)
                //{
                //    return;
                //}

                //if (!String.IsNullOrWhiteSpace(message))
                //{
                //    MessageBox.Show("Không cho phép chọn phiếu bù lẻ để in phiếu công khai [mã phiếu xuất: " + message + "]");
                //}

                //WaitingManager.Show();

                //var groupPatient = expMestCheckeds.Where(o => o.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL).ToList();

                //var mps262 = new HIS.Desktop.Plugins.Library.PrintAggrExpMest.PrintAggrExpMestProcessor(groupPatient);
                //if (mps262 != null)
                //{
                //    mps262.Print("Mps000262");
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKskContractCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtKskContractCode.Text))
                    {
                        var contract = this.kskContractList.FirstOrDefault(o => o.KSK_CONTRACT_CODE.ToLower() == txtKskContractCode.Text.Trim
                       ().ToLower());
                        if (contract != null)
                        {
                            cboKskContract.EditValue = contract.ID;
                            txtKskContractCode.Text = contract.KSK_CONTRACT_CODE;
                            txtTreatmentCode.Focus();
                            txtTreatmentCode.SelectAll();
                        }
                        else
                        {
                            cboKskContract.Focus();
                            cboKskContract.ShowPopup();
                        }
                    }
                    else
                    {
                        txtKskContractCode.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboKskContract_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboKskContract.EditValue != null)
                    {
                        var contract = this.kskContractList.FirstOrDefault(o => o.ID == (long)cboKskContract.EditValue);
                        if (contract != null)
                        {
                            txtKskContractCode.Text = contract.KSK_CONTRACT_CODE;
                            txtTreatmentCode.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboKskContract_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboKskContract.EditValue = null;
                    txtKskContractCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonApprovalEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                TreatmentADO row = (TreatmentADO)gridView.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    List<long> treatmentIdList = new List<long>();
                    treatmentIdList.Add(row.ID);
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<List<HIS_TREATMENT>>
                        ("api/HisTreatment/KskApprove", ApiConsumer.ApiConsumers.MosConsumer, treatmentIdList, param);
                    if (apiresult != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonDisApprovalEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                TreatmentADO row = (TreatmentADO)gridView.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    List<long> treatmentIdList = new List<long>();
                    treatmentIdList.Add(row.ID);
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<List<HIS_TREATMENT>>
                        ("api/HisTreatment/KskUnapprove", ApiConsumer.ApiConsumers.MosConsumer, treatmentIdList, param);
                    if (apiresult != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                List<TreatmentADO> dataSource = (List<TreatmentADO>)gridControl.DataSource;
                if (dataSource != null && dataSource.Count > 0)
                {
                    var treatmentUnAprovalList = dataSource.Where(o => o.CheckTreatment && o.IS_PAUSE != 1 && o.IS_LOCK_HEIN != 1 && o.IS_LOCK_FEE != 1 && o.IS_KSK_APPROVE != 1).ToList();
                    if (treatmentUnAprovalList != null && treatmentUnAprovalList.Count > 0)
                    {
                        WaitingManager.Show();
                        List<long> treatmentIdList = new List<long>();
                        treatmentIdList.AddRange(treatmentUnAprovalList.Select(o => o.ID).Distinct().ToList());
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<List<HIS_TREATMENT>>
                            ("api/HisTreatment/KskApprove", ApiConsumer.ApiConsumers.MosConsumer, treatmentIdList, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                    else
                    {
                        MessageBox.Show("Không có hồ sơ để thực hiện duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Không có hồ sơ để thực hiện duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
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
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
