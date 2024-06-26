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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.SDO;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisCoTreatmentReceive.ValidationRule;
using System.Resources;

namespace HIS.Desktop.Plugins.HisCoTreatmentReceive.HisCoTreatmentReceive
{
    public partial class frmHisCoTreatmentReceive : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        DelegateSelectData delegateSelect;
        Inventec.Desktop.Common.Modules.Module moduleData;
        int positionHandle = -1;
        long CotreatmentId;
        #endregion

        #region Construct
        public frmHisCoTreatmentReceive(long _cotreatmentId, Inventec.Desktop.Common.Modules.Module _moduleData, DelegateSelectData _delegateSelect)
            : base(_moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = _moduleData;
                this.delegateSelect = _delegateSelect;
                this.CotreatmentId = _cotreatmentId;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmHisCoTreatmentCreate_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguage();
                InitComboBedRoom();
                ValidateGridLookupWithTextEdit(this.cboBedRoom, this.txtBedRoomCode);
                ValidControlStartTime();
                dtStartTime.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBedRoom()
        {
            try
            {
                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                if (room != null)
                {
                    var bedRooms = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.DEPARTMENT_ID == room.DEPARTMENT_ID && o.IS_ACTIVE == 1).ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("BED_ROOM_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("BED_ROOM_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("BED_ROOM_NAME", "ID", columnInfos, false, 350);
                    ControlEditorLoader.Load(this.cboBedRoom, bedRooms, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisCoTreatmentReceive.Resources.Lang", typeof(HIS.Desktop.Plugins.HisCoTreatmentReceive.HisCoTreatmentReceive.frmHisCoTreatmentReceive).Assembly);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisCoTreatmentCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBedRoomCode.Text = Inventec.Common.Resource.Get.Value("frmHisCoTreatmentReceive.lciBedRoomCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStartTime.Text = Inventec.Common.Resource.Get.Value("frmHisCoTreatmentReceive.lciStartTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !string.IsNullOrEmpty(moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlStartTime()
        {
            try
            {
                StartTimeValidationRule reasonRule = new StartTimeValidationRule();
                reasonRule.dtCancelTime = dtStartTime;
                dxValidationProviderEditorInfo.SetValidationRule(dtStartTime, reasonRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                WaitingManager.Show();
                HisCoTreatmentReceiveSDO hisCoTreatmentSDO = new HisCoTreatmentReceiveSDO();
                hisCoTreatmentSDO.BedRoomId = Inventec.Common.TypeConvert.Parse.ToInt32((cboBedRoom.EditValue ?? "").ToString());
                hisCoTreatmentSDO.Id = this.CotreatmentId;
                hisCoTreatmentSDO.RequestRoomId = this.moduleData.RoomId;
                if (dtStartTime.DateTime != DateTime.MinValue)
                {
                    hisCoTreatmentSDO.StartTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStartTime.DateTime) ?? 0;
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CO_TREATMENT>(HisRequestUriStore.MOSHIS_CO_TREATMENT_RECEIVE, ApiConsumers.MosConsumer, hisCoTreatmentSDO, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;

                }
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    if (this.delegateSelect != null)
                    {
                        this.delegateSelect(result);
                    }

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        void LoadDepartmentCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboBedRoom.EditValue = null;
                    cboBedRoom.Focus();
                    cboBedRoom.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboAccountBook);
                }
                else
                {
                    var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboBedRoom.EditValue = data[0].ID;
                            txtBedRoomCode.Text = data[0].DEPARTMENT_CODE;
                            btnSave.Focus();
                        }
                        else if (data.Count > 1)
                        {
                            cboBedRoom.EditValue = null;
                            cboBedRoom.Focus();
                            cboBedRoom.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboAccountBook);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBedRoom.EditValue != null && cboBedRoom.EditValue != cboBedRoom.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_BED_ROOM accountBook = BackendDataWorker.Get<V_HIS_BED_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBedRoom.EditValue.ToString()));
                        if (accountBook != null)
                        {
                            txtBedRoomCode.Text = accountBook.BED_ROOM_CODE;
                            btnSave.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDepartmentCombo(strValue, false);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion


        #region Public method
        #endregion

        #region Shortcut
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

    }
}
