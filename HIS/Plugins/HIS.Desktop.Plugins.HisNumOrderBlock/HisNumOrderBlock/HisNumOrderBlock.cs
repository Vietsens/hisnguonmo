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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Common.WebApiClient;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Resources;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.HisNumOrderBlock.HisNumOrderBlock
{
    public partial class frmHisNumOrderBlock : HIS.Desktop.Utility.FormBase
    {
        #region ---Decalre---
        Module Currentmodule;
        RefeshReference refeshReference;
        int ActionType = -1;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        HIS_NUM_ORDER_BLOCK currentData;
        HIS_NUM_ORDER_BLOCK currentData_;
        List<HIS_ROOM_TIME> HisRoomTime = new List<HIS_ROOM_TIME>();
        HIS_ROOM_TIME HisRoomTime_ = new HIS_ROOM_TIME();
        List<string> ListBuoi = new List<string>();
        #endregion
        public frmHisNumOrderBlock(Module module)
            : this(null, null)
        {

        }
        public frmHisNumOrderBlock(Module module, HIS_ROOM_TIME HisRoomTime_)

            : base(module)
        {
            InitializeComponent();
            //this.refeshReference = reference;
            this.HisRoomTime_ = HisRoomTime_;
            this.Currentmodule = module;
            try
            {
                if (this.Currentmodule != null && !String.IsNullOrEmpty(this.Currentmodule.text))
                {
                    this.Text = this.Currentmodule.text;
                }
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.AddBarManager(this.barManager1);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisRoomTime_), HisRoomTime_));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmHisNumOrderBlock_Load(object sender, EventArgs e)
        {
            try
            {
                Validate();
                cboBuoi.Enabled = false;
                SetDataDefaut();
                EnableControlChange(this.ActionType);
                LoadDataToGridControl();
                SetCapitionByLanguageKey();
                if (HisRoomTime == null && HisRoomTime.Count() < 1)
                {
                    HisRoomTime = BackendDataWorker.Get<HIS_ROOM_TIME>();
                }
                InitComboCommon();
                if (chkBlock.Checked == true)
                {
                    this.lcFromTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lcToTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lcSTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lcSTTtu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lcBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lbBlocks.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lcEmBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                if (chkTuDen.Checked == true)
                {
                    this.lcFromTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lcToTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lcSTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lcSTTtu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lcBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lbBlocks.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lcEmBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #region ---PreviewKeyDown---
        private void txtHisNumOrderBlockCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //txtHisNumOrderBlockName.Focus();
                    // txtHisNumOrderBlockName.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtHisNumOrderBlockName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFromTime.Focus();
                    txtFromTime.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtFromTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtToTime.Focus();
                    txtToTime.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtToTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                        btnAdd.Focus();
                    else if (btnEdit.Enabled)
                        btnEdit.Focus();
                    else
                        btnCancel.Focus();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---Validate---
        private void Validate()
        {
            try
            {
                if (chkBlock.Checked == true)
                {
                    ValidateMaxlength(txtBlock, true, 2);
                    ValidateMaxlength(txtSTTtu, true, 100);
                    dxValidationProvider1.SetValidationRule(txtSTT, null);
                    //dxValidationProvider1.SetValidationRule(txtFromTime, null);
                    //dxValidationProvider1.SetValidationRule(txtToTime, null);
                }
                if (chkTuDen.Checked == true)
                {
                    ValidateMaxlength(txtSTT, true, 1000);
                    // ValidateMaxlength(txtHisNumOrderBlockName, true, 100);
                    //ValidateMaxlength(txtFromTime, false, 6);
                    //ValidateMaxlength(txtToTime, false, 6);
                    dxValidationProvider1.SetValidationRule(txtSTTtu, null);
                    dxValidationProvider1.SetValidationRule(txtBlock, null);
                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void ValidateMaxlength(DevExpress.XtraEditors.BaseEdit control, bool IsRequired, int maxlength)
        {
            try
            {
                ControlMaxLengthValidationRule valie = new ControlMaxLengthValidationRule();
                valie.editor = control;
                valie.maxLength = maxlength;
                valie.IsRequired = IsRequired;
                valie.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                valie.ErrorText = "Nhập quá ký tự cho phép (" + maxlength + ")";
                dxValidationProvider1.SetValidationRule(control, valie);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---SetData---
        private void SetCapitionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisNumOrderBlock.Resources.Lang", typeof(HIS.Desktop.Plugins.HisNumOrderBlock.HisNumOrderBlock.frmHisNumOrderBlock).Assembly);
                this.lcFromTime.Text = Setlanguage("txtHisNumOrderBlockCode.Text");
                this.lcToTime.Text = Setlanguage("txtHisNumOrderBlockName.Text");
                this.lcFromTime.Text = Setlanguage("txtFromTime.Text");
                this.lcToTime.Text = Setlanguage("txtToTime.Text");
                //this.grdSTT.Caption = Setlanguage("HisNumOrderBlock.grdSTT.ToolTip");
                //this.grdSTT.ToolTip = Setlanguage("HisNumOrderBlock.grdSTT.ToolTip");
                //this.grdTu.Caption = Setlanguage("HisNumOrderBlock.grdColFromTo.Caption");
                //this.grdTu.ToolTip = Setlanguage("HisNumOrderBlock.grdColFromTo.ToolTip");
                //this.grdDen.Caption = Setlanguage("HisNumOrderBlock.grdColToTime.Caption");
                //this.grdDen.ToolTip = Setlanguage("HisNumOrderBlock.grdColToTime.ToolTip");
                //this.grdColToTime.Caption = Setlanguage("grdColToTime.Caption");
                //this.grdColToTime.ToolTip = Setlanguage("grdColToTime.ToolTip");
                this.grdColIsActive.Caption = Setlanguage("grdColIsActive.Caption");
                //this.grdTu.Caption = Setlanguage("grdTu.Caption");
                //this.grdDen.Caption = Setlanguage("grdDen.Caption");
                this.grdColIsActive.ToolTip = Setlanguage("grdColIsActive.ToolTip");
                this.grdColCreateTime.Caption = Setlanguage("grdColCreateTime.Caption");
                this.grdColCreateTime.ToolTip = Setlanguage("grdColCreateTime.ToolTip");
                this.grdColCreator.Caption = Setlanguage("grdColCreator.Caption");
                this.grdColCreator.ToolTip = Setlanguage("grdColCreator.ToolTip");
                this.grdColModifyTime.Caption = Setlanguage("grdColModifyTime.Caption");
                this.grdColModifyTime.ToolTip = Setlanguage("grdColModifyTime.ToolTip");
                this.grdColModifier.Caption = Setlanguage("grdColModifier.Caption");
                this.grdColModifier.ToolTip = Setlanguage("grdColModifier.ToolTip");
                this.btnAdd.Text = Setlanguage("btnAdd.Text");
                this.btnEdit.Text = Setlanguage("btnEdit.Text");
                this.btnCancel.Text = Setlanguage("btnCancel.Text");

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private String Setlanguage(string KeyCaption)
        {
            string keycaption = "";
            try
            {
                keycaption = Inventec.Common.Resource.Get.Value("HisNumOrderBlock." + KeyCaption, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                keycaption = "";
                LogSystem.Warn(ex);
            }
            return keycaption;
        }
        private void SetDataDefaut()
        {
            try
            {
                txtSTT.Text = "";
                txtSTTtu.Text = "";
                txtBlock.Text = "";
                txtFromTime.Text = "";
                txtToTime.Text = "";
                this.ActionType = GlobalVariables.ActionAdd;

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void EnableControlChange(int action)
        {
            try
            {
                this.btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
                this.btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        //Load data to gridcontrol
        private void LoadDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPaging, param, numPageSize, this.GridControlHisNumOrderBlock);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {

                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<HIS_NUM_ORDER_BLOCK>> apiResuilt = null;

                HisNumOrderBlockFilter filter = new HisNumOrderBlockFilter();
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "NUM_ORDER";
                filter.ROOM_TIME_ID = HisRoomTime_.ID;
                filter.KEY_WORD = txtTimKiem.Text;
                
                GridViewHisNumOrderBlock.BeginUpdate();
                apiResuilt = new BackendAdapter(paramCommon).GetRO<List<HIS_NUM_ORDER_BLOCK>>(HisRequestUriStore.HisNumOrderBlock_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResuilt), apiResuilt));
                if (apiResuilt != null)
                {
                    var data = apiResuilt.Data;
                    if (data != null && data.Count > 0)
                    {
                        GridControlHisNumOrderBlock.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResuilt.Param == null ? 0 : apiResuilt.Param.Count ?? 0);
                    }
                    else
                    {
                        GridControlHisNumOrderBlock.DataSource = null;
                    }
                }
                else
                {
                    GridControlHisNumOrderBlock.DataSource = null;
                }
                GridViewHisNumOrderBlock.EndUpdate();
                GridViewHisNumOrderBlock.RefreshData();
                this.Refresh();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        //
        // Update data
        private void ProcessorSave()
        {
            try
            {
                List<HIS_NUM_ORDER_BLOCK> hisblock_ = new List<HIS_NUM_ORDER_BLOCK>();
                CommonParam param = new CommonParam();
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;

                if (txtFromTime.EditValue != null && txtToTime.EditValue != null && txtFromTime.DateTime > txtToTime.DateTime)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian từ không được lớn hơn thời gian đến", "Thông báo");
                    return;
                }
                
                //WaitingManager.Show();
                HIS_NUM_ORDER_BLOCK UpdateDTO = new HIS_NUM_ORDER_BLOCK();
                if (chkTuDen.Checked == true)
                {
                    TimeSpan start = DateTime.Parse(from).TimeOfDay;
                    TimeSpan end = DateTime.Parse(to).TimeOfDay;

                    UpDataDTOFromDataForm(ref UpdateDTO);
                }

                if (chkBlock.Checked == true)
                {
                    if (txtBlock.Text.ToString() == "0")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Block không được có giá trị = 0", "Thông báo");
                        return;
                    }
                    if (!string.IsNullOrEmpty(txtBlock.Text.ToString()) && txtBlock.Text.ToString() != "0")
                    {
                        TimeSpan start = DateTime.Parse(from).TimeOfDay;
                        TimeSpan end = DateTime.Parse(to).TimeOfDay;
                        

                        int time = int.Parse(txtBlock.Text.ToString());
                        TimeSpan interval = new TimeSpan(0, time, 0);
                        // If Start is bigger than end, Add a day to the end.
                        if (start > end)
                            end = end.Add(new TimeSpan(1, 0, 0, 0));
                        
                        Int64 num = Int64.Parse(txtSTTtu.Text.ToString());
                        while (true)
                        {
                            if (start >= end)
                            {
                                break;
                            }

                            HIS_NUM_ORDER_BLOCK hisblock = new HIS_NUM_ORDER_BLOCK();
                            hisblock.FROM_TIME = start.ToString().Replace(":", "");
                            start = start.Add(interval);
                            
                            if (start.ToString().Replace(":", "") == "1.000000")
                            {
                                hisblock.TO_TIME = "235959";
                            }
                            else
                            {
                                if (start > end)
                                {
                                    hisblock.TO_TIME = end.ToString().Replace(":", "");
                                }
                                else
                                {
                                    TimeSpan startk = start - DateTime.Parse("00:01:00").TimeOfDay;
                                    hisblock.TO_TIME = startk.ToString().Replace(":", "");
                                }
                            }

                            hisblock.NUM_ORDER = num;
                            hisblock.ROOM_TIME_ID = HisRoomTime_.ID;
                            num = num + 1;
                            hisblock_.Add(hisblock);



                        }
                    }
                }

                if (chkTuDen.Checked == true)
                {
                    if (checkTimeOut()) 
                    
                    {
                        if (this.ActionType == GlobalVariables.ActionAdd)
                        {
                            UpdateDTO.ROOM_TIME_ID = this.HisRoomTime_.ID;
                            var Result = new BackendAdapter(param).Post<HIS_NUM_ORDER_BLOCK>(HisRequestUriStore.HisNumOrderBlock_Create, ApiConsumers.MosConsumer, UpdateDTO, param);
                            if (Result != null)
                            {
                                BackendDataWorker.Reset<HIS_NUM_ORDER_BLOCK>();
                                success = true;
                                //MessageManager.Show(this, param, success);
                                LoadDataToGridControl();
                                btnCancel_Click(null, null);

                            }
                        }
                        else
                        {
                            if (this.currentData != null)
                            {
                                UpdateDTO.ID = this.currentData.ID;
                                UpdateDTO.ROOM_TIME_ID = this.currentData.ROOM_TIME_ID;
                                var Resutl = new BackendAdapter(param).Post<HIS_NUM_ORDER_BLOCK>(HisRequestUriStore.HisNumOrderBlock_UPDATE, ApiConsumers.MosConsumer, UpdateDTO, param);
                                if (Resutl != null)
                                {
                                    BackendDataWorker.Reset<HIS_NUM_ORDER_BLOCK>();
                                    success = true;
                                    // MessageManager.Show(this, param, success);
                                    LoadDataToGridControl();

                                }
                            }
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    
                }
                if (chkBlock.Checked == true)
                {
                    if (hisblock_ != null && hisblock_.Count() > 0)
                    {
                        //  foreach (var item in hisblock_)
                        //{
                        if (this.ActionType == GlobalVariables.ActionAdd)
                        {

                            var Result = new BackendAdapter(param).Post<List<HIS_NUM_ORDER_BLOCK>>(HisRequestUriStore.HisNumOrderBlock_CreateList, ApiConsumers.MosConsumer, hisblock_, param);
                            if (Result != null)
                            {
                                BackendDataWorker.Reset<HIS_NUM_ORDER_BLOCK>();
                                success = true;
                                //MessageManager.Show(this, param, success);
                                LoadDataToGridControl();
                                btnCancel_Click(null, null);

                            }
                        }
                        else
                        {
                            if (this.currentData != null)
                            {
                                UpdateDTO.ID = this.currentData.ID;
                                UpdateDTO.ROOM_TIME_ID = this.currentData.ROOM_TIME_ID;
                                var Resutl = new BackendAdapter(param).Post<HIS_NUM_ORDER_BLOCK>(HisRequestUriStore.HisNumOrderBlock_UPDATE, ApiConsumers.MosConsumer, UpdateDTO, param);
                                if (Resutl != null)
                                {
                                    BackendDataWorker.Reset<HIS_NUM_ORDER_BLOCK>();
                                    success = true;
                                    // MessageManager.Show(this, param, success);
                                    LoadDataToGridControl();

                                }
                            }
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                        //}
                    }
                }
                #region ---Thong bao---
                
                #endregion
                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        private void UpDataDTOFromDataForm(ref HIS_NUM_ORDER_BLOCK data)
        {
            try
            {
                if (chkTuDen.Checked == true)
                {
                    string text = txtSTT.Text.ToString().Replace(",", "");
                    if (text.Contains("."))
                    {
                        text = text.Replace(".", "");
                    }

                    data.NUM_ORDER = long.Parse(txtSTT.Text);

                    if (txtFromTime.EditValue != null)
                        data.FROM_TIME = txtFromTime.DateTime.ToString("HHmmsss");
                    else
                        data.FROM_TIME = "000000";

                    if (txtToTime.EditValue != null)
                        data.TO_TIME = txtToTime.DateTime.ToString("HHmmsss");
                    else
                        data.TO_TIME = "235959";

                }

                if (chkBlock.Checked == true)
                {



                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        //
        //Set data defaut to control 
        private void RestFormData()
        {
            try
            {
                if (!lcInfor.IsInitialized)
                    return;
                lcInfor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcInfor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            cboBuoi.Focus();
                            cboBuoi.SelectAll();
                        }
                    }

                }
                   
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcInfor.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedataRow(HIS_NUM_ORDER_BLOCK data)
        {
            try
            {


                if (data != null)
                {
                    FillDatatoControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChange(this.ActionType);
                    this.btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void FillDatatoControl(HIS_NUM_ORDER_BLOCK data)
        {
            try
            {
                if (data != null)
                {
                    //if (chkTuDen.Checked == true)
                    //{

                    txtSTT.Text = data.NUM_ORDER.ToString();

                    string nowStrTo = Inventec.Common.DateTime.Get.Now().ToString();

                    if (data.TO_TIME.Length < 6)
                    {
                        string timeTo = nowStrTo.Substring(0, 8) + data.TO_TIME + "00";
                        DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.TypeConvert.Parse.ToInt64(timeTo)));

                        if (dt != null)
                        {

                            txtToTime.EditValue = dt.Value.ToString("HH:mm:ss");
                        }
                    }
                    else
                    {
                        string timeTo = nowStrTo.Substring(0, 8) + data.TO_TIME;
                        DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.TypeConvert.Parse.ToInt64(timeTo)));

                        if (dt != null)
                        {
                            txtToTime.EditValue = dt.Value.ToString("HH:mm:ss");
                        }
                    }


                    if (data.TO_TIME.Length < 6)
                    {
                        string timeTo = nowStrTo.Substring(0, 8) + data.FROM_TIME + "00";
                        DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.TypeConvert.Parse.ToInt64(timeTo)));

                        if (dt != null)
                        {
                            txtFromTime.EditValue = dt.Value.ToString("HH:mm:ss");
                        }
                    }
                    else
                    {
                        string timeTo = nowStrTo.Substring(0, 8) + data.FROM_TIME;
                        DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.TypeConvert.Parse.ToInt64(timeTo)));

                        if (dt != null)
                        {
                            txtFromTime.EditValue = dt.Value.ToString("HH:mm:ss");
                        }
                    }

                    // }
                    //if (chkBlock.Checked == true)
                    //{
                    //    TimeSpan start = DateTime.Parse(txtFromTime.Text).TimeOfDay;
                    //    TimeSpan end = DateTime.Parse(txtToTime.Text).TimeOfDay;

                    //    TimeSpan interval = new TimeSpan(0, 60, 0);

                    //    // If Start is bigger than end, Add a day to the end.
                    //    if (start > end)
                    //        end = end.Add(new TimeSpan(1, 0, 0, 0));

                    //    while (true)
                    //    {
                    //        Console.WriteLine((new DateTime() + start).ToString("hh:mm tt"));

                    //        start = start.Add(interval);

                    //        if (start > end)
                    //            break;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---Even Button---
        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        private void F2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //txtHisNumOrderBlockCode.Focus();
                //txtHisNumOrderBlockCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {

                chkTuDen.Enabled = false;
                chkBlock.Enabled = false;
                this.ProcessorSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProcessorSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChange(this.ActionType);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                bool checkBlock = false;
                if (chkBlock.Checked)
                {
                    checkBlock = true;
                }
                bool checkTuDen = false;
                if (chkTuDen.Checked)
                {
                    checkTuDen = true;
                }

                RestFormData();
                chkTuDen.Enabled = true;
                chkBlock.Enabled = true;
                if (checkBlock)
                {
                    chkBlock.Checked = true;
                }
                if (checkTuDen)
                {
                    chkTuDen.Checked = true;
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
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region ---Even GridControl---
        private void GridViewHisNumOrderBlock_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_NUM_ORDER_BLOCK datarow = (HIS_NUM_ORDER_BLOCK)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (datarow != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + this.startPage;
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = (datarow.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa");
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(datarow.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(datarow.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "FROM_TIME_STR")
                        {
                            string nowStrFr = Inventec.Common.DateTime.Get.Now().ToString();
                            if (datarow.FROM_TIME.Length < 6)
                            {
                                string timeFr = nowStrFr.Substring(0, 8) + datarow.FROM_TIME + "00";
                                DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(timeFr));
                                if (dt != null)
                                {
                                    e.Value = dt.Value.ToString("HH:mm:ss");
                                }
                            }
                            else
                            {
                                string timeFr = nowStrFr.Substring(0, 8) + datarow.FROM_TIME;
                                DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(timeFr));
                                if (dt != null)
                                {
                                    e.Value = dt.Value.ToString("HH:mm:ss");
                                }
                            }






                        }
                        else if (e.Column.FieldName == "TO_TIME_STR")
                        {
                            string nowStrTo = Inventec.Common.DateTime.Get.Now().ToString();

                            if (datarow.TO_TIME.Length < 6)
                            {
                                string timeTo = nowStrTo.Substring(0, 8) + datarow.TO_TIME + "00";
                                DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.TypeConvert.Parse.ToInt64(timeTo)));

                                if (dt != null)
                                {
                                    e.Value = dt.Value.ToString("HH:mm:ss");
                                }
                            }
                            else
                            {
                                string timeTo = nowStrTo.Substring(0, 8) + datarow.TO_TIME;
                                DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.TypeConvert.Parse.ToInt64(timeTo)));

                                if (dt != null)
                                {
                                    e.Value = dt.Value.ToString("HH:mm:ss");
                                }
                            }



                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void GridViewHisNumOrderBlock_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_NUM_ORDER_BLOCK datarow = (HIS_NUM_ORDER_BLOCK)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (datarow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnLock : btnUnLock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (datarow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDelete : btnVisibleDetele);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void GridViewHisNumOrderBlock_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views
                    .Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_NUM_ORDER_BLOCK dataRow = (HIS_NUM_ORDER_BLOCK)GridViewHisNumOrderBlock.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        e.Appearance.ForeColor = (dataRow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Color.Green : Color.Red);
                    }
                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void GridViewHisNumOrderBlock_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_NUM_ORDER_BLOCK datarow = (HIS_NUM_ORDER_BLOCK)GridViewHisNumOrderBlock.GetFocusedRow();
                if (datarow != null)
                {
                    chkTuDen.Checked = true;
                    this.currentData = datarow;
                    ChangedataRow(datarow);
                    chkTuDen.Enabled = false;
                    chkBlock.Enabled = false;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        #endregion
        #region ---btn Lock and Delete
        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_NUM_ORDER_BLOCK datarow = (HIS_NUM_ORDER_BLOCK)GridViewHisNumOrderBlock.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var Result = new BackendAdapter(param).Post<HIS_NUM_ORDER_BLOCK>(HisRequestUriStore.HisNumOrderBlock_CHANGELOCK, ApiConsumers.MosConsumer, datarow.ID, param);
                        if (Result != null)
                        {
                            LoadDataToGridControl();
                            success = true;
                            btnCancel_Click(null, null);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_NUM_ORDER_BLOCK datarow = (HIS_NUM_ORDER_BLOCK)GridViewHisNumOrderBlock.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var Result = new BackendAdapter(param).Post<HIS_NUM_ORDER_BLOCK>(HisRequestUriStore.HisNumOrderBlock_CHANGELOCK, ApiConsumers.MosConsumer, datarow.ID, param);
                        if (Result != null)
                        {
                            LoadDataToGridControl();
                            success = true;
                            btnCancel_Click(null, null);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();

                HIS_NUM_ORDER_BLOCK datarow = (HIS_NUM_ORDER_BLOCK)GridViewHisNumOrderBlock.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        bool success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HisNumOrderBlock_DELETE, ApiConsumers.MosConsumer, datarow.ID, param);
                        if (success)
                        {

                            LoadDataToGridControl();
                            btnCancel_Click(null, null);
                            currentData = ((List<HIS_NUM_ORDER_BLOCK>)GridViewHisNumOrderBlock.DataSource).FirstOrDefault();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    LoadDataToGridControl();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        string from;
        string to;
        private void InitComboCommon()
        {
            try
            {
                // ListBuoi = new List<string>();
                //foreach (var item in HisRoomTime)
                //{
                string nowStrTo = Inventec.Common.DateTime.Get.Now().ToString();

                if (HisRoomTime_.FROM_TIME.Length < 6)
                {
                    string timeFr = nowStrTo.Substring(0, 8) + HisRoomTime_.FROM_TIME + "00";
                    DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.TypeConvert.Parse.ToInt64(timeFr)));

                    if (dt != null)
                    {
                        from = dt.Value.ToString("HH:mm:ss");
                    }
                }
                else
                {
                    string timeFr = nowStrTo.Substring(0, 8) + HisRoomTime_.FROM_TIME;
                    DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.TypeConvert.Parse.ToInt64(timeFr)));

                    if (dt != null)
                    {
                        from = dt.Value.ToString("HH:mm:ss");
                    }
                }

                if (HisRoomTime_.TO_TIME.Length < 6)
                {
                    string timeTo = nowStrTo.Substring(0, 8) + HisRoomTime_.TO_TIME + "00";
                    DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.TypeConvert.Parse.ToInt64(timeTo)));

                    if (dt != null)
                    {
                        to = dt.Value.ToString("HH:mm:ss");
                    }
                }
                else
                {
                    string timeTo = nowStrTo.Substring(0, 8) + HisRoomTime_.TO_TIME;
                    DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.TypeConvert.Parse.ToInt64(timeTo)));

                    if (dt != null)
                    {
                        to = dt.Value.ToString("HH:mm:ss");
                    }
                }
                cboBuoi.Text = HisRoomTime_.ROOM_TIME_NAME + "(" + from + "-" + to + ")";

                //  ListBuoi.Add(buoi);
                //}
                //cboBuoi.Properties.DataSource = ListBuoi;
                ////cboBuoi.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                ////cboBuoi.Properties.ValueMember = "ID";
                //cboBuoi.Properties.ForceInitialize();
                //cboBuoi.Properties.Columns.Clear();
                ////cboBuoi.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_CODE", "", 50));
                ////cboBuoi.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_NAME", "", 200));
                //cboBuoi.Properties.ShowHeader = false;
                //cboBuoi.Properties.ImmediatePopup = true;
                //cboBuoi.Properties.DropDownRows = 10;
                //cboBuoi.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkBlock_CheckedChanged(object sender, EventArgs e)
        {

            if (chkBlock.Checked == true)
            {
                chkTuDen.Checked = false;
                this.lcFromTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lcToTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lcSTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lcSTTtu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lcBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lbBlocks.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lcEmBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

            }
            if (chkTuDen.Checked == true)
            {
                chkBlock.Checked = false;
                this.lcFromTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lcToTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lcSTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lcSTTtu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lcBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lbBlocks.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lcEmBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            }
            Validate();
        }

        private void chkTuDen_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBlock.Checked == true)
            {
                chkTuDen.Checked = false;
                this.lcFromTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lcToTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lcSTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lcSTTtu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lcBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lbBlocks.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lcEmBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

            }
            if (chkTuDen.Checked == true)
            {
                chkBlock.Checked = false;
                this.lcFromTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lcToTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lcSTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lcSTTtu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lcBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lbBlocks.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lcEmBlock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            }
            Validate();
        }

        private void txtSTT_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                        btnAdd_Click(null, null);
                    if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                        btnEdit_Click(null, null);
                }
                if (e.KeyChar == '\r')
                {
                    txtSTT.Focus();
                    txtSTT.SelectAll();
                }
                else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtSTTtu_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                        btnAdd_Click(null, null);
                    if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                        btnEdit_Click(null, null);
                }
                if (e.KeyChar == '\r')
                {
                    txtSTTtu.Focus();
                    txtSTTtu.SelectAll();
                }
                else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBlock_KeyPress(object sender, KeyPressEventArgs e)
        {

            
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    txtSTTtu.Focus();
                    txtSTTtu.SelectAll();
                }
                if (e.KeyChar == '\r')
                {
                    txtSTTtu.Focus();
                    txtSTTtu.SelectAll();
                }
                else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void txtFromTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtToTime.SelectAll();
                    txtToTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtToTime_KeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSTT.SelectAll();
                    txtSTT.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkTimeOut()
        {
            try
            {
                if (chkTuDen.Checked == true)
                {
                    TimeSpan start = DateTime.Parse(from).TimeOfDay;
                    TimeSpan end = DateTime.Parse(to).TimeOfDay;
                   
                    
                    if (txtFromTime.EditValue != null )
                    {
                        TimeSpan start_ = DateTime.Parse(txtFromTime.DateTime.ToString()).TimeOfDay;
                        if (start_ < start ) 
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Ngoài khoảng thời gian làm việc của 'Buổi'", "Thông báo");
                            return false;
                        }
                    }
                    if (txtToTime.EditValue != null)
                    {
                        TimeSpan end_ = DateTime.Parse(txtToTime.DateTime.ToString()).TimeOfDay;
                        if (end < end_ )
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Ngoài khoảng thời gian làm việc của 'Buổi'", "Thông báo");
                            return false;
                        }
                    }
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                 return false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

 

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                //BaseEdit edit = e.InvalidControl as BaseEdit;
                //if (edit == null)
                //    return;

                //DevExpress.XtraEditors.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.BaseEditViewInfo;
                //if (viewInfo == null)
                //    return;

                //if (positionHandleControlPatientInfo == -1)
                //{
                //    positionHandleControlPatientInfo = edit.TabIndex;
                //    if (edit.Visible)
                //    {
                //        edit.SelectAll();
                //        edit.Focus();
                //    }
                //}
                //if (positionHandleControlPatientInfo > edit.TabIndex)
                //{
                //    positionHandleControlPatientInfo = edit.TabIndex;
                //    if (edit.Visible)
                //    {
                //        edit.SelectAll();
                //        edit.Focus();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }

}
