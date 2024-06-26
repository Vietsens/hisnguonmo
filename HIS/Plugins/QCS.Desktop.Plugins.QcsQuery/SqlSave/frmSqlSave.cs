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
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using QCS.Desktop.Plugins.QcsQuery;
using HIS.Desktop.Utility;
using QCS.EFMODEL.DataModels;
using QCS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QCS.Desktop.Plugins.QcsQuery.SqlSave
{
    public partial class frmSqlSave : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int actionType = 0;// No action     
        QCS.EFMODEL.DataModels.QCS_QUERY currentUpdateDTO = null;
        long currentQcsQueryId = 0;
        int positionHandleControlInfo = -1;
        RefeshReference refeshData;
        List<string> arrControlEnableNotChange = new List<string>();
        //Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        #endregion

        #region Contructor
        public frmSqlSave(QCS.EFMODEL.DataModels.QCS_QUERY dataSave)
            : this(0, null)
        {
            this.currentUpdateDTO = dataSave;
        }
        public frmSqlSave(QCS.EFMODEL.DataModels.QCS_QUERY dataSave, RefeshReference _refeshData)
            : this(0, null)
        {
            this.currentUpdateDTO = dataSave;
            this.refeshData = _refeshData;
        }

        public frmSqlSave(RefeshReference _refeshData)
            : this(0, _refeshData)
        {

        }

        public frmSqlSave(long QcsQueryId)
            : this(QcsQueryId, null)
        {

        }

        public frmSqlSave(long QcsQueryId, RefeshReference _refeshData)
        {
            try
            {
                InitializeComponent();

                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                this.actionType = (QcsQueryId > 0 ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd);
                this.currentQcsQueryId = QcsQueryId;
                this.refeshData = _refeshData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmEditInfoQcsQuery_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultData();
                SetCaptionByLanguageKey();
                InitTabIndex();
                FillDataToControlsForm();
                if (this.currentQcsQueryId > 0)
                {
                    LoadCurrentView(this.currentQcsQueryId, ref currentUpdateDTO);
                    FillDataToEditorControl(this.currentUpdateDTO);
                }
                //ValidateForm();
                WaitingManager.Hide();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Set lai tat cac cac label/caption/tooltip hien thi tren giao dien doc tu file resource ngon ngu
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                //this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMSqlSave_FRM_PATIENT_UPDATE", EXE.APP.Resources.ResourceLanguageManager.LanguageFrmSqlSave, EXE.LOGIC.Base.LanguageManager.GetCulture());
                //btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMSqlSave_BTN_SAVE", EXE.APP.Resources.ResourceLanguageManager.LanguageFrmSqlSave, EXE.LOGIC.Base.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        /// <summary>
        /// Gan gia trị mac dinh cho cac control can khoi tao san gia tri
        /// </summary>
        private void SetDefaultData()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Do du lieu tu doi tuong du lieu vao cac control editor
        /// </summary>
        /// <param name="data"></param>
        private void FillDataToEditorControl(QCS.EFMODEL.DataModels.QCS_QUERY data)
        {
            try
            {
                txtDescription.Text = data.DESCRIPTION;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khoi tao va do du lieu vao cac control dang combo/loolup/gridlookup,...
        /// </summary>
        private void FillDataToControlsForm()
        {
            try
            {

                //InitComboExecuteRole();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lay doi tuong du lieu theo id
        /// </summary>
        /// <param name="QcsQueryId"></param>
        /// <param name="QcsQueryDTO"></param>
        private void LoadCurrent(long QcsQueryId, ref QCS.EFMODEL.DataModels.QCS_QUERY QcsQueryDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                QcsQueryFilter filter = new QcsQueryFilter();
                filter.ID = QcsQueryId;
                QcsQueryDTO = new BackendAdapter(param).Get<List<QCS_QUERY>>("api/QcsQuery/Get", ApiConsumers.QcsConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lay view cua doi tuong du lieu theo id
        /// </summary>
        /// <param name="QcsQueryId"></param>
        /// <param name="QcsQueryDTO"></param>
        private void LoadCurrentView(long QcsQueryId, ref QCS.EFMODEL.DataModels.QCS_QUERY QcsQueryDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                QcsQueryFilter filter = new QcsQueryFilter();
                filter.ID = QcsQueryId;
                QcsQueryDTO = new BackendAdapter(param).Get<List<QCS_QUERY>>("api/QcsQuery/Get", ApiConsumers.QcsConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void InitTabIndex()
        {
            try
            {
                //dicOrderTabIndexControl.Add("txtSql", 0);
                //dicOrderTabIndexControl.Add("txtDescription", 1);

                ////dicOrderTabIndexControl.Add("txtPatientCode", 0);
                ////dicOrderTabIndexControl.Add("txtPatientName", 1);

                //if (dicOrderTabIndexControl != null)
                //{
                //    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                //    {
                //        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                //if (!layoutControlEditor.IsInitialized) return success;
                //layoutControlEditor.BeginUpdate();
                //try
                //{
                //    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                //    {
                //        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                //        if (lci != null && lci.Control != null)
                //        {
                //            BaseEdit be = lci.Control as BaseEdit;
                //            if (be != null)
                //            {
                //                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                //                if (itemOrderTab.Key.Contains(be.Name))
                //                {
                //                    be.TabIndex = itemOrderTab.Value;
                //                }
                //            }
                //        }
                //    }
                //}
                //finally
                //{
                //    layoutControlEditor.EndUpdate();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        /// <summary>
        /// Gan gia tri cac control tren form vao doi tuong can xu ly
        /// </summary>
        /// <param name="QcsQueryDTO"></param>
        private void UpdateDTOFromDataForm(ref QCS.EFMODEL.DataModels.QCS_QUERY currentDTO)
        {
            try
            {
                currentDTO.DESCRIPTION = txtDescription.Text.Trim();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ham kiem tra neu ben ngoai truyen vao delegate thi goi thuc hien goi delegate
        /// </summary>
        private void RefeshDataDelegate()
        {
            try
            {
                if (this.refeshData != null)
                {
                    this.refeshData();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham cap nhat du lieu
        /// lay du lieu moi nhat tren form gan vao doi tuong can xu ly
        /// sau do goi api gui len doi tuong do
        /// tra ket qua tuong ung ve cho client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandleControlInfo = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                if (currentQcsQueryId > 0)
                {
                    LoadCurrent(this.currentUpdateDTO.ID, ref currentUpdateDTO);
                }
                UpdateDTOFromDataForm(ref currentUpdateDTO);

                var resultData = new BackendAdapter(param).Post<QCS_QUERY>((currentQcsQueryId > 0 ? "api/QcsQuery/Update" : "api/QcsQuery/Create"), ApiConsumers.QcsConsumer, currentUpdateDTO, param);

                if (resultData != null)
                {
                    success = true;
                    WaitingManager.Hide();
                    RefeshDataDelegate();
                    this.Close();
                }

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        #region Validation Provider ValidationFailed
        /// <summary>
        /// Ham ValidationProvider cung cap: tu dong quet tat ca cac control editor can validate
        /// neu tim thay control editor vao khong hop le se hien thi icon canh bao ben canh
        /// dong thoi focus vao control dau tien dang loi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlInfo == -1)
                {
                    positionHandleControlInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlInfo > edit.TabIndex)
                {
                    positionHandleControlInfo = edit.TabIndex;
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
        #endregion

        #region Shortcut button handler
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event editor control handler

        //TODO bo xung key
        //private void txtEmployeeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtEmployeeName.Focus();
        //            txtEmployeeName.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}        

        #endregion

        #endregion

        #region Public method
        //Cac ham can chia ra ben ngoai se khai bao tap trung o day

        /// <summary>
        /// Ham public cho ben ngoai module goi, thuong su dung cho shortcut ben ngoai
        /// </summary>
        public void Save()
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

    }
}
