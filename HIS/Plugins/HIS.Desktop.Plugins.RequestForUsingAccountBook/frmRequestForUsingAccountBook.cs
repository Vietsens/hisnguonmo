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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.RequestForUsingAccountBook.Validate;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RequestForUsingAccountBook
{
    public partial class frmRequestForUsingAccountBook : FormBase
    {
        AuthorityAccountBookSDO myRequest = null;
        private int positionHandleControl = -1;

        public frmRequestForUsingAccountBook(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                foreach (var scrn in Screen.AllScreens)
                {
                    if (scrn.Bounds.Contains(this.Location))
                    {
                        this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmRequestForUsingAccountBook_Load(object sender, EventArgs e)
        {
            try
            {

                RegisterTimer(GetModuleLink(), "timerCheckAuthority", timerCheckAuthority.Interval, timerCheckAuthority_Tick);
                WaitingManager.Show();
                this.TopLevel = true;
                this.ValidControl();
                this.LoadData();
                cboCashierUser.Enabled = !(this.myRequest != null);
                btnRequest.Enabled = !(this.myRequest != null);
                btnCancelUsing.Enabled = (this.myRequest != null);
                GlobalVariables.RefreshUsingAccountBookModule = this.DelegateRefreshData;
                this.InitComboCashierUser();
                this.InitPayform();
                if (GlobalVariables.DefaultPayformRequest.HasValue && GlobalVariables.DefaultPayformRequest.Value > 0)
                    cboPayform.EditValue = GlobalVariables.DefaultPayformRequest.Value;
                else
                    cboPayform.EditValue = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM;

                if (this.myRequest != null && !this.myRequest.AccountBookId.HasValue)
                    StartTimer(GetModuleLink(), "timerCheckAuthority");
                timerCheckAuthority.Interval = 3000;//Fix
                timerCheckAuthority.Enabled = true;
                
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                AuthorityAccountBookSDO requests = new BackendAdapter(new CommonParam()).Get<AuthorityAccountBookSDO>("api/HisAccountBook/MyRequest", ApiConsumers.MosConsumer, this.currentModuleBase.RoomId, null);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("INPUT HisAccountBook/MyRequest this.currentModuleBase.RoomId ", this.currentModuleBase.RoomId));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("OUT HisAccountBook/MyRequest requests ", requests));
                this.myRequest = requests;
                GlobalVariables.AuthorityAccountBook = this.myRequest;

                if (this.myRequest != null && this.myRequest.AccountBookId.HasValue)
                {
                    HisAccountBookViewFilter accFilter = new HisAccountBookViewFilter();
                    accFilter.ID = this.myRequest.AccountBookId.Value;
                    List<V_HIS_ACCOUNT_BOOK> accountBooks = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, accFilter, null);

                    if (accountBooks != null && accountBooks.Count > 0)
                    {
                        lblAccountBook.Text = accountBooks[0].ACCOUNT_BOOK_NAME;
                        lblNumOrder.Text = ((long)(accountBooks[0].CURRENT_NUM_ORDER ?? 0)).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Load người chỉ định
        private async Task InitComboCashierUser()
        {
            try
            {
                List<ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS_USER>())
                {
                    datas = BackendDataWorker.Get<ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new BackendAdapter(paramCommon).GetAsync<List<ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

                List<V_HIS_USER_ROOM> urs = BackendDataWorker.Get<V_HIS_USER_ROOM>();

                HisUserRoomViewFilter uRFilter = new HisUserRoomViewFilter();
                uRFilter.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN;
                uRFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                List<V_HIS_USER_ROOM> userRooms = new BackendAdapter(new CommonParam()).Get<List<V_HIS_USER_ROOM>>("api/HisUserRoom/GetView", ApiConsumers.MosConsumer, uRFilter, null);

                datas = datas != null ? datas.Where(o => userRooms != null && userRooms.Any(a => a.LOGINNAME == o.LOGINNAME)).ToList() : null;

                //Nguoi chi dinh
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboCashierUser, datas, controlEditorADO);
                if (this.myRequest != null)
                {
                    cboCashierUser.EditValue = this.myRequest.CashierLoginName;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitPayform()
        {
            try
            {
                var payFormList = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == 1
                    && o.ID != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE
                    && o.ID != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__KEYPAY).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboPayform, payFormList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task CheckApprove()
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlCashierUser();
                ValidControlPayform();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlCashierUser()
        {
            try
            {
                CashierUserValidationRule rule = new CashierUserValidationRule();
                rule.cboCashierUser = this.cboCashierUser;

                dxValidationProvider1.SetValidationRule(this.cboCashierUser, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlPayform()
        {
            try
            {
                CashierUserValidationRule rule = new CashierUserValidationRule();
                rule.cboCashierUser = this.cboPayform;

                dxValidationProvider1.SetValidationRule(this.cboPayform, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCashierUser_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboCashierUser.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierUser_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
                else
                {
                    cboCashierUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRequest.Enabled || this.myRequest != null) return;
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate()) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                AuthorityAccountBookSDO sdo = new AuthorityAccountBookSDO();
                sdo.RequestLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                sdo.RequestRoomId = this.currentModuleBase.RoomId;
                sdo.RequestTime = Inventec.Common.DateTime.Get.Now().Value;
                sdo.CashierLoginName = (cboCashierUser.EditValue ?? "").ToString();
                sdo.CashierUserName = cboCashierUser.Text;

                LogSystem.Debug(LogUtil.TraceData("Input Request: ", sdo));

                AuthorityAccountBookSDO rs = new BackendAdapter(param).Post<AuthorityAccountBookSDO>("api/HisAccountBook/Request", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    this.myRequest = rs;
                    GlobalVariables.AuthorityAccountBook = this.myRequest;
                    cboCashierUser.Enabled = false;

                    btnCancelUsing.Enabled = true;
                    btnRequest.Enabled = false;
                    StartTimer(GetModuleLink(), "timerCheckAuthority"); 
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelUsing_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCancelUsing.Enabled || this.myRequest == null) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                bool rs = new BackendAdapter(param).Post<bool>("api/HisAccountBook/Cancel", ApiConsumers.MosConsumer, this.myRequest, param);
                if (rs)
                {
                    success = true;
                    this.myRequest = null;
                    GlobalVariables.AuthorityAccountBook = this.myRequest;
                    cboCashierUser.Enabled = true;
                    cboPayform.Enabled = true;
                    btnCancelUsing.Enabled = false;
                    btnRequest.Enabled = true;
                    lblAccountBook.Text = "";
                    lblNumOrder.Text = "";
                    cboCashierUser.EditValue = null;
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerCheckAuthority_Tick()
        {
            try
            {
                LogSystem.Debug("timerCheckAuthority_Tick => 1...");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.myRequest ", this.myRequest));
                if (this.myRequest == null || this.myRequest.AccountBookId.HasValue)
                {
                    StopTimer(GetModuleLink(), "timerCheckAuthority");
                }
                this.CheckApprove();
                LogSystem.Debug("timerCheckAuthority_Tick => 2...");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        

        private void DelegateRefreshData(object data)
        {
            try
            {
                if (data != null && (data is long || data is decimal))
                {
                    lblNumOrder.Text = ((long)(data)).ToString();
                }
                else
                {
                    lblNumOrder.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmRequestForUsingAccountBook_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GlobalVariables.RefreshUsingAccountBookModule = null;
                this.Dispose();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayform_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPayform.EditValue != null)
                    GlobalVariables.DefaultPayformRequest = Convert.ToInt64((cboPayform.EditValue ?? "").ToString());
                else
                    GlobalVariables.DefaultPayformRequest = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
