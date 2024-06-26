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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.MediStockPeriod.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Plugins.MediStockPeriod;
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

namespace HIS.Desktop.Plugins.MediStockPeriod.CreatePeriod
{
    public partial class frmCreatePeriod : HIS.Desktop.Utility.FormBase
    {
        int positionHandleControlBedInfo = -1;
        RefeshData refeshData;
        Inventec.Desktop.Common.Modules.Module currentModule;
        MediStockPeriodADO _MediStockPeriod = new MediStockPeriodADO();
        int Action = 0;
        List<MediStockADO> _MediStockProcess = new List<MediStockADO>();
        List<MediStockADO> MediStock__Seleced;

        public frmCreatePeriod()
        {
            InitializeComponent();
        }

        public frmCreatePeriod(Inventec.Desktop.Common.Modules.Module _module, List<MediStockADO> mediStockProcess, RefeshData refeshData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _module;
                this.refeshData = refeshData;
                this._MediStockProcess = mediStockProcess;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmCreatePeriod(Inventec.Desktop.Common.Modules.Module _module, List<MediStockADO> mediStockProcess, MediStockPeriodADO dataEdit, RefeshData refeshData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _module;
                this.refeshData = refeshData;
                this._MediStockPeriod = dataEdit;
                this._MediStockProcess = mediStockProcess;
                this.Action = GlobalVariables.ActionEdit;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmCreatePeriod_Load(object sender, EventArgs e)
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                ValidateBedForm();
                LoadDataToComboMedistock();
                if (this._MediStockPeriod != null && this._MediStockPeriod.ID > 0)
                {
                    txtMediStockPeriodName.Text = this._MediStockPeriod.MEDI_STOCK_PERIOD_NAME;
                    //cboMediStock.EditValue = this._MediStockPeriod.MEDI_STOCK_ID;
                    cboMediStock.Focus();
                    var data = _MediStockProcess.FirstOrDefault(o => o.ID == _MediStockPeriod.MEDI_STOCK_ID);
                    if (data != null)
                    {
                        GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            List<MediStockADO> selectData = new List<MediStockADO>() { data };
                            gridCheckMark.SelectAll(selectData);
                        }
                    }
                    txtMediStockPeriodName.Focus();
                    cboMediStock.Enabled = false;

                    dtTimePeriod.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this._MediStockPeriod.TO_TIME ?? 0) ?? DateTime.Now;
                    dtTimePeriod.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboMedistock()
        {
            CommonParam param = new CommonParam();
            try
            {
                //List<V_HIS_MEDI_STOCK> mediStockImp = new List<V_HIS_MEDI_STOCK>();
                //var _WorkPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId);
                //if (_WorkPlace != null)
                //{
                //    var datas = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(p => p.LOGINNAME.Trim() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim() && _WorkPlace.BranchId == p.BRANCH_ID).ToList();
                //    if (datas != null && datas.Count > 0)
                //    {
                //        List<long> roomIds = datas.Select(p => p.ROOM_ID).ToList();
                //        mediStockImp = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => p.DEPARTMENT_ID == _WorkPlace.DepartmentId && roomIds.Contains(p.ROOM_ID) && p.IS_ACTIVE == 1).OrderBy(p => p.MEDI_STOCK_NAME).ToList();
                //    }
                //}

                //cboMediStock.Properties.DataSource = mediStockImp;
                //cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                //cboMediStock.Properties.ValueMember = "ID";
                //cboMediStock.Properties.ForceInitialize();
                //cboMediStock.Properties.Columns.Clear();
                //cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 100));
                //cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                //cboMediStock.Properties.ShowHeader = false;
                //cboMediStock.Properties.ImmediatePopup = true;
                //cboMediStock.Properties.PopupWidth = 300;
                //if (mediStockImp != null && mediStockImp.Count > 0)
                //{
                //    var data = mediStockImp.FirstOrDefault(p => p.ROOM_ID == this.currentModule.RoomId);
                //    if (data != null)
                //    {
                //        this.cboMediStock.EditValue = data.ID;
                //        this.txtMediStockCode.Text = data.MEDI_STOCK_CODE;
                //    }
                //}

                InitCheck(cboMediStock, SelectionGrid__MediStock);
                InitCombo(cboMediStock, _MediStockProcess, "MEDI_STOCK_NAME", "ID");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__MediStock(object sender, EventArgs e)
        {
            try
            {
                MediStock__Seleced = new List<MediStockADO>();
                foreach (MediStockADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        MediStock__Seleced.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 350;
                col2.Caption = "Tất cả";
                col2.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                cbo.Properties.PopupFormWidth = 450;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                cbo.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;

                //GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //{
                //    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMediStockCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(strValue))
                    {
                        cboMediStock.EditValue = null;
                        cboMediStock.Focus();
                        cboMediStock.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    //if (cboMediStock.EditValue != null)
                    //{
                    //    var mediStockImp = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>()
                    //        .FirstOrDefault(o =>
                    //            o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStock.EditValue ?? 0).ToString()));
                    //    if (mediStockImp != null)
                    //    {
                    //        txtMediStockCode.Text = mediStockImp.MEDI_STOCK_CODE;
                    //    }
                    //}
                    dtTimePeriod.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void cboMediStock_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (cboMediStock.EditValue != null)
        //        {
        //            var mediStockImp = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>()
        //                    .FirstOrDefault(o => o.ID == (long)cboMediStock.EditValue);
        //            if (mediStockImp != null)
        //            {
        //                txtMediStockCode.Text = mediStockImp.MEDI_STOCK_CODE;
        //                dtTimePeriod.Focus();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

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

                if (positionHandleControlBedInfo == -1)
                {
                    positionHandleControlBedInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlBedInfo > edit.TabIndex)
                {
                    positionHandleControlBedInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateBedForm()
        {
            ValidateWithTextEdit(txtMediStockPeriodName);
            //ValidateComboWithTextEdit(cboMediStock, txtMediStockCode);
            ValidateComboWithTextEdit(cboMediStock);
        }

        private void ValidateWithTextEdit(TextEdit txtTextEdit)
        {
            try
            {
                TextEditValidationRule validRule = new TextEditValidationRule();
                validRule.txtTextEdit = txtTextEdit;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTextEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateComboWithTextEdit(GridLookUpEdit cbo)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.cbo = cbo;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cbo, validRule);
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
                this.positionHandleControlBedInfo = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                if (this.Action == GlobalVariables.ActionEdit && this._MediStockPeriod != null && this._MediStockPeriod.ID > 0)
                {
                    HIS_MEDI_STOCK_PERIOD dataCreate = new HIS_MEDI_STOCK_PERIOD();
                    dataCreate.MEDI_STOCK_PERIOD_NAME = txtMediStockPeriodName.Text;
                    //dataCreate.MEDI_STOCK_ID = (long)cboMediStock.EditValue;
                    //if (dtTimePeriod.EditValue != null)
                    //    dataCreate.TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimePeriod.DateTime);
                    //else
                    //    dataCreate.TO_TIME = null;
                    dataCreate.ID = this._MediStockPeriod.ID;
                    dataCreate.MEDI_STOCK_ID = this._MediStockPeriod.MEDI_STOCK_ID;
                    dataCreate.TO_TIME = this._MediStockPeriod.TO_TIME;
                    HIS_MEDI_STOCK_PERIOD result = new BackendAdapter(param).Post<HIS_MEDI_STOCK_PERIOD>("/api/HisMediStockPeriod/Update", ApiConsumers.MosConsumer, dataCreate, param);
                    if (result != null)
                    {
                        success = true;
                    }
                }
                else
                {
                    if (MediStock__Seleced == null || MediStock__Seleced.Count <= 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn kho", "Thông báo");
                        return;
                    }

                    bool result = true;
                    foreach (var item in MediStock__Seleced)
                    {
                        CommonParam paramCreate = new CommonParam();
                        HIS_MEDI_STOCK_PERIOD dataCreate = new HIS_MEDI_STOCK_PERIOD();
                        dataCreate.MEDI_STOCK_PERIOD_NAME = txtMediStockPeriodName.Text;
                        dataCreate.MEDI_STOCK_ID = item.ID;
                        if (dtTimePeriod.EditValue != null)
                            dataCreate.TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimePeriod.DateTime);
                        else
                            dataCreate.TO_TIME = null;

                        var apiResult = new BackendAdapter(paramCreate).Post<HIS_MEDI_STOCK_PERIOD>(HisRequestUriStore.HIS_MEST_STOCK_PERIOD_CREATE, ApiConsumers.MosConsumer, dataCreate, paramCreate);
                        result = result && (apiResult != null);
                        if (apiResult == null)
                        {
                            string messageError = item.MEDI_STOCK_NAME;

                            if (paramCreate.BugCodes != null && paramCreate.BugCodes.Count > 0)
                            {
                                messageError += string.Format("({0})", string.Join(",", paramCreate.BugCodes));
                            }

                            if (paramCreate.Messages != null && paramCreate.Messages.Count > 0)
                            {
                                messageError += string.Format("({0})", string.Join(",", paramCreate.Messages));
                            }

                            param.Messages.Add(messageError);
                        }
                    }

                    success = result;
                }

                if (success)
                {
                    if (refeshData != null)
                    {
                        this.refeshData();
                    }

                    this.Close();
                    success = true;
                }

                WaitingManager.Hide();
                #region ShowMessager
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimePeriod_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (dtTimePeriod.EditValue != null)
                    {
                        btnSave.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimePeriod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtTimePeriod.EditValue != null)
                    {
                        btnSave.Focus();
                    }
                    else
                        dtTimePeriod.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStock_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string display = "";
                foreach (var item in this.MediStock__Seleced)
                {
                    if (display.Trim().Length > 0)
                    {
                        display += ", " + item.MEDI_STOCK_NAME;
                    }
                    else
                        display = item.MEDI_STOCK_NAME;
                }
                e.DisplayText = display;
                cboMediStock.ToolTip = display;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediStockPeriodName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMediStock.Focus();
                    cboMediStock.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
