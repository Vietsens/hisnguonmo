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
using MOS.Filter;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Drawing;
using HIS.Desktop.Plugins.ExpMestBCSCreate.ADO;
using HIS.Desktop.Plugins.ExpMestAggregate;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Controls;

namespace HIS.Desktop.Plugins.ExpMestBCSCreate.Run
{
    public partial class UCExpMestBCSCreate : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.Desktop.Common.Modules.Module _Module;
        V_HIS_MEDI_STOCK _MediStock { get; set; }
        List<V_HIS_MEDI_STOCK> _lstMediStock { get; set; }
        List<ExpMestTTADO> dataExpMestTTs { get; set; }
        List<ExpMestTTADO> dataExpMestEquals { get; set; }
		public bool IsReasonRequired { get; private set; }

		bool isCheckAll = true;
        bool isCheckAllEqual = true;

        bool _IsReadOnly;
        int positionHandleControl = -1;
        public UCExpMestBCSCreate()
        {
            InitializeComponent();
        }

        public UCExpMestBCSCreate(Inventec.Desktop.Common.Modules.Module _module, V_HIS_MEDI_STOCK _mediStock)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this._MediStock = _mediStock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCExpMestBCSCreate(Inventec.Desktop.Common.Modules.Module _module, bool _isReadOnly)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this._IsReadOnly = _isReadOnly;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestBCSCreate_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._IsReadOnly)
                {
                    btnRefesh.Enabled = false;
                    btnSearch.Enabled = false;
                    btnTaoPhieu.Enabled = false;

                    DevExpress.XtraEditors.XtraMessageBox.Show("Không phải là tủ trực", "Thông báo");
                    return;
                }
                this.gridControlExpMestTT.ToolTipController = this.toolTipController1;
                this.gridControlImpMestTH.ToolTipController = this.toolTipController1;
                this.gridControlExpMestBcs.ToolTipController = this.toolTipController1;

                _lstMediStock = new List<V_HIS_MEDI_STOCK>();
                _lstMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => p.IS_ACTIVE == 1).ToList();
                IsReasonRequired = LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__IS_REASON_REQUIRED) == "1";
                SetDefaultValue();
                LoadExpMestTT();
                LoadImpMestTT_TL();
                LoadDataExpMestBcs();
                InitComboMediStock(this.cboMediStock);
                LoadDataToComboReasonRequired();
                lciReasonRequired.AppearanceItemCaption.ForeColor = Color.Black;
                if (IsReasonRequired)
                {
                    lciReasonRequired.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationSingleControl(cboReasonRequired);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
				Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboReasonRequired()
        {
            try
            {
                var reason = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboReasonRequired, reason, controlEditorADO);
                cboReasonRequired.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtKeyword.Text = "";
                // Load thoi gian mac dinh len control datetime
               dtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
               dtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
                chkChuaBu.Checked = true;
                chkDaBu.Checked = false;
                gridControlExpMestTT.DataSource = null;
                gridControlBscDetail.DataSource = null;
                gridControlExpMestBcs.DataSource = null;
                gridControlExpMestDetail.DataSource = null;
                gridControlImpMestTH.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestTT()
        {

            try
            {
                WaitingManager.Show();
                //gridViewExpMestTT.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.False;

                gridControlExpMestTT.DataSource = null;
                #region Filter
                HisExpMestView5Filter filter = new HisExpMestView5Filter();
                filter.KEY_WORD = txtKeyword.Text.Trim();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.IS_HAS_BCS_REQ_AMOUNT = false;
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT;//La Don TT
                filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;

                if (chkChuaBu.Checked == true && chkDaBu.Checked == false)
                {
                    filter.HAS_XBTT_EXP_MEST_ID = false;
                }
                else if (chkChuaBu.Checked == false && chkDaBu.Checked == true)
                {
                    filter.HAS_XBTT_EXP_MEST_ID = true;
                }
                #endregion
                filter.MEDI_STOCK_ID = this._MediStock.ID;

                #region DateTime
                if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.FINISH_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                }
                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.FINISH_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                }
                #endregion

                filter.ColumnParams = new List<string>()
                {
                    "ID",
                    "EXP_MEST_STT_ID",
                    "EXP_MEST_TYPE_ID",
                    "IMP_MEDI_STOCK_ID",
                    "IS_ACTIVE",
                    "IS_DELETE",
                    "IS_EXECUTE_KIDNEY_PRES",
                    "IS_EXPORT_EQUAL_APPROVE",
                    "IS_EXPORT_EQUAL_REQUEST",
                    "IS_HAS_BCS_REQ_AMOUNT",
                    "IS_NOT_FULL_BCS_REQ_AMOUNT",
                    "IS_OLD_BCS",
                    "MEDI_STOCK_ID",
                    "MEDI_STOCK_CODE",
                    "REQ_DEPARTMENT_ID",
                    "REQ_LOGINNAME",
                    "REQ_USERNAME",
                    "SERVICE_REQ_ID",
                    "TDL_INTRUCTION_TIME",
                    "XBTT_EXP_MEST_ID",
                    "MEDI_STOCK_NAME",
                    "FINISH_TIME",
                    "FINISH_DATE",
                    "EXP_MEST_CODE",
                    "CREATE_TIME",
                    "TDL_TREATMENT_CODE",
                    "TDL_SERVICE_REQ_CODE",
                    "TDL_PATIENT_NAME"
                };

                CommonParam param = new CommonParam();

                List<V_HIS_EXP_MEST_5> rs = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_5>>("api/HisExpMest/GetView5Dynamic", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                dataExpMestTTs = new List<ExpMestTTADO>();
                if (rs != null && rs.Count > 0)
                {
                    dataExpMestTTs.AddRange((from r in rs select new ExpMestTTADO(r)).ToList());
                }
                gridControlExpMestTT.DataSource = dataExpMestTTs;
                WaitingManager.Hide();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadImpMestTT_TL()
        {

            try
            {
                WaitingManager.Show();
                gridControlImpMestTH.DataSource = null;
                #region Filter
                HisExpMestView5Filter filter = new HisExpMestView5Filter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS;
                filter.IS_EXPORT_EQUAL_REQUEST = false;
                filter.IMP_MEDI_STOCK_ID = this._MediStock.ID;
                filter.IS_HAS_BCS_REQ_AMOUNT = false;
                filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                if (chkChuaBu.Checked == true && chkDaBu.Checked == false)
                {
                    filter.HAS_XBTT_EXP_MEST_ID = false;
                }
                else if (chkChuaBu.Checked == false && chkDaBu.Checked == true)
                {
                    filter.HAS_XBTT_EXP_MEST_ID = true;
                }
                #endregion

                #region DateTime
                if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.FINISH_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                }
                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.FINISH_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                }
                #endregion

                filter.ColumnParams = new List<string>()
                {
                    "ID",
                    "EXP_MEST_STT_ID",
                    "EXP_MEST_TYPE_ID",
                    "IMP_MEDI_STOCK_ID",
                    "IS_ACTIVE",
                    "IS_DELETE",
                    "IS_EXECUTE_KIDNEY_PRES",
                    "IS_EXPORT_EQUAL_APPROVE",
                    "IS_EXPORT_EQUAL_REQUEST",
                    "IS_HAS_BCS_REQ_AMOUNT",
                    "IS_NOT_FULL_BCS_REQ_AMOUNT",
                    "IS_OLD_BCS",
                    "MEDI_STOCK_ID",
                    "MEDI_STOCK_CODE",
                    "REQ_DEPARTMENT_ID",
                    "REQ_LOGINNAME",
                    "REQ_USERNAME",
                    "SERVICE_REQ_ID",
                    "TDL_INTRUCTION_TIME",
                    "XBTT_EXP_MEST_ID",
                    "MEDI_STOCK_NAME",
                    "FINISH_TIME",
                    "FINISH_DATE",
                    "CREATE_TIME",
                    "EXP_MEST_CODE"
                };

                CommonParam param = new CommonParam();

                List<V_HIS_EXP_MEST_5> rs = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_5>>("api/HisExpMest/GetView5Dynamic", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                dataExpMestEquals = new List<ExpMestTTADO>();
                if (rs != null && rs.Count > 0)
                {
                    dataExpMestEquals.AddRange((from r in rs select new ExpMestTTADO(r)).ToList());
                }
                gridControlImpMestTH.DataSource = dataExpMestEquals;

                WaitingManager.Hide();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSearch()
        {
            Inventec.Common.Logging.LogSystem.Debug("bbtnSearch()");
            btnSearch_Click(null, null);
        }

        public void bbtnRefesh()
        {
            Inventec.Common.Logging.LogSystem.Debug("btnRefesh_Click()");
            btnRefesh_Click(null, null);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadExpMestTT();
                LoadImpMestTT_TL();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                LoadExpMestTT();
                LoadImpMestTT_TL();
                LoadDataExpMestBcs();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void gridViewExpMestTT_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    ExpMestTTADO data = (ExpMestTTADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStock_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    dtTimeFrom.Focus();
                    dtTimeFrom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestTT_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var row = (ExpMestTTADO)gridViewExpMestTT.GetFocusedRow();
                if (row != null)
                {
                    var data = LoadDataDetailExpMest(row.ID);
                    gridControlExpMestDetail.DataSource = null;
                    gridControlExpMestDetail.DataSource = data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTaoPhieu_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnTaoPhieu.Enabled)
                    return;

                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                List<ExpMestTTADO> _ExpMestChecks = new List<ExpMestTTADO>();
                if (this.dataExpMestTTs != null && this.dataExpMestTTs.Count > 0)
                {
                    _ExpMestChecks = this.dataExpMestTTs.Where(p => p.IsCheck == true && p.XBTT_EXP_MEST_ID == null).ToList();

                }
                List<ExpMestTTADO> _ImpMestChecks = new List<ExpMestTTADO>();
                if (this.dataExpMestEquals != null && this.dataExpMestEquals.Count > 0)
                {
                    _ImpMestChecks = this.dataExpMestEquals.Where(p => p.IsCheck == true && p.XBTT_EXP_MEST_ID == null).ToList();
                }

                if ((_ExpMestChecks != null && _ExpMestChecks.Count > 0) || (_ImpMestChecks != null && _ImpMestChecks.Count > 0))
                {
                    HisExpMestBcsSDO sdo = new HisExpMestBcsSDO();
                    if (cboReasonRequired.EditValue != null)
                        sdo.ExpMestReasonId = Int64.Parse(cboReasonRequired.EditValue.ToString());
                    sdo.ExpMestDttIds = new List<long>();
                    sdo.ExpMestBcsIds = new List<long>();

                    if (_ExpMestChecks != null && _ExpMestChecks.Count > 0)
                    {
                        sdo.ExpMestDttIds = _ExpMestChecks.Select(p => p.ID).ToList();
                        Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("ExpMestDttIds----", sdo.ExpMestDttIds));
                    }
                    if (_ImpMestChecks != null && _ImpMestChecks.Count > 0)
                    {
                        sdo.ExpMestBcsIds = _ImpMestChecks.Select(p => p.ID).ToList();
                        Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("ExpMestBcsIds----", sdo.ExpMestBcsIds));
                    }
                    sdo.ReqRoomId = this._Module.RoomId;
                    if (this.cboMediStock.EditValue != null)
                    {
                        sdo.MediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboMediStock.EditValue.ToString());
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn kho xuất", "Thông báo");
                        cboMediStock.Focus();
                        cboMediStock.SelectAll();
                        return;
                    }

                    sdo.ImpMediStockId = this._MediStock.ID;

                    bool success = false;
                    CommonParam param = new CommonParam();
                    var result = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/BcsCreate", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (result != null)
                    {
                        success = true;
                        btnRefesh_Click(null, null);
                    }
                    else
                        success = false;
                    MessageManager.Show(this.ParentForm, param, success);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestTH_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    ExpMestTTADO data = (ExpMestTTADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "IMP_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestTH_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var row = (ExpMestTTADO)gridViewImpMestTH.GetFocusedRow();
                if (row != null)
                {
                    HIS_EXP_MEST ado = new HIS_EXP_MEST();
                    ado.ID = row.ID;
                    ado.EXP_MEST_STT_ID = row.EXP_MEST_STT_ID;
                    var data = LoadDataDetailExpMestBcs(ado);
                    gridControlExpMestDetail.DataSource = null;
                    gridControlExpMestDetail.DataSource = data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestBcs_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    HIS_EXP_MEST data = (HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXP_MEST_STT_DISPLAY")
                    {
                        //Status
                        long expMestSttId = data.EXP_MEST_STT_ID;
                        //trang: dang gui YC : màu vàng
                        if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        //Trang thai: da duyet màu xanh
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        //trang thai: da hoan thanh-da xuat: màu xanh
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        //trang thai: tu choi duyet mau den
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                    }
                    else if (e.Column.FieldName == "TDL_INTRUCTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MEDI_STOCK_NAME")
                    {
                        var medi = _lstMediStock.FirstOrDefault(p => p.ID == data.MEDI_STOCK_ID);
                        if (medi != null)
                            e.Value = medi.MEDI_STOCK_NAME;
                        else
                            e.Value = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestBcs_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var row = (HIS_EXP_MEST)gridViewExpMestBcs.GetFocusedRow();
                if (row != null)
                {
                    var data = LoadDataDetailExpMestBcs(row);
                    gridControlBscDetail.DataSource = null;
                    gridControlBscDetail.DataSource = data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBcsDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    //V_HIS_EXP_MEST data = (V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    //V_HIS_EXP_MEST data = (V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlExpMestTT)
                {
                    ToolTipDetail(gridControlExpMestTT, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlImpMestTH)
                {
                    ToolTipDetail(gridControlImpMestTH, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlExpMestBcs)
                {
                    ToolTipDetail(gridControlExpMestBcs, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        private void ToolTipDetail(DevExpress.XtraGrid.GridControl gridControl, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                if (info.InRowCell)
                {
                    if (lastRowHandle != info.RowHandle)
                    {
                        lastRowHandle = info.RowHandle;
                        string text = "";
                        if (info.Column.FieldName == "EXP_MEST_STT_DISPLAY")
                        {
                            long stt = (long)view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_ID");
                            var name = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(p => p.ID == stt);
                            if (name != null)
                                text = name.EXP_MEST_STT_NAME;
                            else
                                text = "";//(view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                        }
                        lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                    }
                    e.Info = lastInfo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnTongHop()
        {
            try
            {
                btnTaoPhieu_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestTT_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    //Review
                    var data = (ExpMestTTADO)gridViewExpMestTT.GetRow(e.RowHandle);
                    if (data != null && e.Column.FieldName == "IsCheck")
                    {
                        if (data.XBTT_EXP_MEST_ID > 0)
                            e.RepositoryItem = repositoryItemCheck__D;
                        else
                            e.RepositoryItem = repositoryItemCheck__E;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestTT_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnCheck.Image = imageListIcon.Images[5];
                            gridViewExpMestTT.BeginUpdate();
                            if (this.dataExpMestTTs == null)
                                this.dataExpMestTTs = new List<ExpMestTTADO>();
                            if (isCheckAll == true)
                            {
                                foreach (var item in this.dataExpMestTTs)
                                {
                                    item.IsCheck = true;
                                    if (item.XBTT_EXP_MEST_ID != null)
                                    {
                                        item.IsCheck = false;
                                    }
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumnCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this.dataExpMestTTs)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAll = true;
                            }
                            gridViewExpMestTT.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestTH_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    //Review
                    var data = (ExpMestTTADO)gridViewExpMestTT.GetRow(e.RowHandle);
                    if (data != null && e.Column.FieldName == "IsCheck")
                    {
                        if (data.XBTT_EXP_MEST_ID > 0)
                            e.RepositoryItem = repositoryItemCheckEdit_Equal__D;
                        else
                            e.RepositoryItem = repositoryItemCheckEdit_Equal__E;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestTH_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnCheckkk.Image = imageListIcon.Images[5];
                            gridViewImpMestTH.BeginUpdate();
                            if (this.dataExpMestEquals == null)
                                this.dataExpMestEquals = new List<ExpMestTTADO>();
                            if (isCheckAllEqual == true)
                            {
                                foreach (var item in this.dataExpMestEquals)
                                {
                                    item.IsCheck = true;
                                    if (item.XBTT_EXP_MEST_ID != null)
                                    {
                                        item.IsCheck = false;
                                    }
                                }
                                isCheckAllEqual = false;
                            }
                            else
                            {
                                gridColumnCheckkk.Image = imageListIcon.Images[6];
                                foreach (var item in this.dataExpMestEquals)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAllEqual = true;
                            }
                            gridViewImpMestTH.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Print_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this._ExpMestPrint = new HIS_EXP_MEST();
                this._ExpMestPrint = (HIS_EXP_MEST)gridViewExpMestBcs.GetFocusedRow();
                if (this._ExpMestPrint != null)
                {
                    MOS.Filter.HisExpMestViewFilter filter = new HisExpMestViewFilter();
                    filter.ID = this._ExpMestPrint.ID;
                    var expMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    PrintAggregateExpMest(expMests.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_EXP_MEST currentAggExpMest = null;
        internal void PrintAggregateExpMest(V_HIS_EXP_MEST currentAggExpMest)
        {
            try
            {
                //Review
                this.currentAggExpMest = currentAggExpMest;
                DevExpress.XtraBars.BarManager barManager1 = new DevExpress.XtraBars.BarManager();
                barManager1.Form = this;

                ExpMestBCSPopupMenuProcessor processor = new ExpMestBCSPopupMenuProcessor(this.currentAggExpMest, ExpMestBCSMouseRightClick, barManager1);
                processor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExpMestBCSMouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Tag is ExpMestBCSPopupMenuProcessor.PrintType)
                {
                    var moduleType = (ExpMestBCSPopupMenuProcessor.PrintType)e.Item.Tag;
                    switch (moduleType)
                    {
                        case ExpMestBCSPopupMenuProcessor.PrintType.InTraDoiThuoc:
                            ShowFormFilter(Convert.ToInt64(6));
                            break;
                        case ExpMestBCSPopupMenuProcessor.PrintType.PhieuBuCoSo:
                            onClickPrintPhieuXuatBCS(null, null);
                            break;
                       
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFormFilter(long printType)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    //Review
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentAggExpMest);
                    listArgs.Add(printType);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this._Module.RoomId, this._Module.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this._Module.RoomId, this._Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    if (extenceInstance.GetType() == typeof(bool))
                    {
                        return;
                    }
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var dataRow = (HIS_EXP_MEST)gridViewExpMestBcs.GetFocusedRow();
                if (dataRow != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Bạn có chắc muốn hủy dữ liệu?",
                        "Thông báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        HisExpMestSDO sdo = new HisExpMestSDO();
                        sdo.ExpMestId = dataRow.ID;
                        sdo.ReqRoomId = this._Module.RoomId;
                        if (dataRow.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES_DETAIL)
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>("/api/HisExpMest/BaseCompensationDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresul)
                            {
                                success = true;
                                LoadDataExpMestBcs();
                            }
                        }
                        else if (dataRow.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__BASE)
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>("/api/HisExpMest/CompensationByBaseDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresul)
                            {
                                success = true;
                                LoadDataExpMestBcs();
                            }
                        }
                        else
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresul)
                            {
                                success = true;
                                LoadDataExpMestBcs();
                            }
                        }

                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBcs_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    //Review
                    var data = (HIS_EXP_MEST)gridViewExpMestBcs.GetRow(e.RowHandle);
                    if (data != null && e.Column.FieldName == "DELETE")
                    {
                        if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                            || data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            || (_MediStock.CABINET_MANAGE_OPTION == IMSys.DbConfig.HIS_RS.HIS_MEDI_STOCK.CABINET_MANAGE_OPTION__PRES && data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT))
                            e.RepositoryItem = repositoryItemButton__Delete;
                        else
                            e.RepositoryItem = repositoryItemButton__Delete_D;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
		{
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
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
