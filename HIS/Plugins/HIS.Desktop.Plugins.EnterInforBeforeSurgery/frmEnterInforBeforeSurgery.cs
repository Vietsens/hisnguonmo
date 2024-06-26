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
using AutoMapper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EnterInforBeforeSurgery
{
    public partial class frmEnterInforBeforeSurgery : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        internal HIS_SERVICE_REQ _ServiceReq { get; set; }

        internal HIS.UC.Icd.IcdProcessor IcdMainProcessor { get; set; }
        internal HIS.UC.Icd.IcdProcessor IcdBeforeProcessor { get; set; }
        internal HIS.UC.Icd.IcdProcessor IcdAfterProcessor { get; set; }
        internal UserControl ucIcdMain;
        internal UserControl ucIcdBefore;
        internal UserControl ucIcdAfter;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;

        internal HIS_SERE_SERV sereServ { get; set; }
        internal V_HIS_SERE_SERV_PTTT sereServPTTT { get; set; }
        internal HIS_SERE_SERV_EXT SereServExt { get; set; }

        public frmEnterInforBeforeSurgery()
        {
            InitializeComponent();
        }

        public frmEnterInforBeforeSurgery(Inventec.Desktop.Common.Modules.Module currentModule, HIS_SERVICE_REQ data)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this._ServiceReq = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmEnterInforBeforeSurgery_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                InitIcd();

                LoadDataToCbo();

                LoadData();

                FillDataDefaultToControl();

                FillDataToInformationSurg();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                var ekipUsers = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                if (ekipUsers == null || ekipUsers.Count < 1)
                {
                    HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                    ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    grdControlInformationSurg.DataSource = null;
                    grdControlInformationSurg.DataSource = ekipUserAdoTemps;
                }
                else
                {
                    HisEkipUserADO participant = new HisEkipUserADO();
                    participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUsers.Add(participant);
                    grdControlInformationSurg.DataSource = null;
                    grdControlInformationSurg.DataSource = ekipUsers;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var ekipUsers = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                var ekipUser = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();
                if (ekipUser != null)
                {
                    if (ekipUsers.Count > 0)
                    {
                        ekipUsers.Remove(ekipUser);
                        grdControlInformationSurg.DataSource = null;
                        grdControlInformationSurg.DataSource = ekipUsers;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbo_UseName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

        }

        public void ComboMethodPTTT(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                List<HIS_PTTT_METHOD> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboEmotionlessMothod(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                List<HIS_EMOTIONLESS_METHOD> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(p => p.IS_ACTIVE == 1
                   && (p.IS_FIRST == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cbo)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 50, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboAcsUser(MultiColumnFilterTest.RepositoryItemCustomGridLookUpEditNew cbo)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }
                else
                {
                    acsUserAlows = data;
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 50, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit cbo)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                cbo.DataSource = data;
                cbo.DisplayMember = "USERNAME";
                cbo.ValueMember = "LOGINNAME";

                cbo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.ImmediatePopup = true;
                cbo.View.Columns.Clear();

                GridColumn aColumnCode = cbo.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }
                else
                {
                    acsUserAlows = data;
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboAcsUser(SearchLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }
                else
                {
                    acsUserAlows = data;
                }

                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);

                cbo.Properties.DataSource = acsUserAlows;
                cbo.Properties.DisplayMember = "USERNAME";
                cbo.Properties.ValueMember = "LOGINNAME";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboAcsUser(LookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboExecuteRole(MultiColumnFilterTest.RepositoryItemCustomGridLookUpEditNew cbo)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboPhuongPhap2()
        {
            try
            {
                List<HIS_EMOTIONLESS_METHOD> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(p => p.IS_ACTIVE == 1
                    && (p.IS_SECOND == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboPhuongPhap2, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCbo()
        {
            try
            {
                ComboMethodPTTT(cboMethod);
                ComboEmotionlessMothod(cbbEmotionlessMethod);//Phương pháp vô cảm
                ComboPhuongPhap2();
                ComboAcsUser(cbo_UseName);//Họ và tên
                ComboExecuteRole(cboPosition);//Vai trò
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitIcd()
        {
            try
            {
                InitIcdMain();
                InitUcSecondaryIcd();
                InitIcdBefore();
                InitIcdAfter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitIcdMain()
        {
            var listIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == 1).OrderBy(o => o.ICD_CODE).ToList();
            IcdMainProcessor = new HIS.UC.Icd.IcdProcessor();
            HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
            ado.DelegateNextFocus = NextFocusFromIcdMain;
            ado.DataIcds = listIcd;
            ado.Width = 430;
            ado.Height = 29;
            this.ucIcdMain = (UserControl)this.IcdMainProcessor.Run(ado);
            if (this.ucIcdMain != null)
            {
                this.panelICDMain.Controls.Add(this.ucIcdMain);
                this.ucIcdMain.Dock = DockStyle.Fill;
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                this.subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == 1).OrderBy(o => o.ICD_CODE).ToList());
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                //ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.Width = 430;
                ado.Height = 29;
                ado.TextLblIcd = "CĐ phụ:";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                this.ucSecondaryIcd = (UserControl)this.subIcdProcessor.Run(ado);

                if (this.ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(this.ucSecondaryIcd);
                    this.ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusOut()
        {
            try
            {
                this.IcdBeforeProcessor.FocusControl(this.ucIcdBefore);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.IcdMainProcessor != null && this.ucIcdMain != null)
                {
                    var icdValue = this.IcdMainProcessor.GetValue(this.ucIcdMain);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private void InitIcdBefore()
        {
            var listIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == 1).OrderBy(o => o.ICD_CODE).ToList();
            IcdBeforeProcessor = new HIS.UC.Icd.IcdProcessor();
            HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
            ado.DelegateNextFocus = NextFocusFromIcdBefore;
            ado.DataIcds = listIcd;
            ado.Width = 430;
            ado.Height = 29;
            ado.LblIcdMain = "CĐ trước:";
            ado.ToolTipsIcdMain = "Chẩn đoán trước khi phẫu thuật thủ thuật";
            this.ucIcdBefore = (UserControl)this.IcdBeforeProcessor.Run(ado);
            if (this.ucIcdBefore != null)
            {
                this.panelICDBefore.Controls.Add(this.ucIcdBefore);
                this.ucIcdBefore.Dock = DockStyle.Fill;
            }
        }

        private void InitIcdAfter()
        {
            var listIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == 1).OrderBy(o => o.ICD_CODE).ToList();
            IcdAfterProcessor = new HIS.UC.Icd.IcdProcessor();
            HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
            ado.DelegateNextFocus = NextFocusFromIcdAfer;
            ado.DataIcds = listIcd;
            ado.Width = 471;
            ado.Height = 29;
            ado.LblIcdMain = "CĐ sau:";
            ado.ToolTipsIcdMain = "Chẩn đoán sau khi phẫu thuật thủ thuật";
            this.ucIcdAfter = (UserControl)this.IcdAfterProcessor.Run(ado);
            if (this.ucIcdAfter != null)
            {
                this.panelICDAfter.Controls.Add(this.ucIcdAfter);
                this.ucIcdAfter.Dock = DockStyle.Fill;
            }
        }

        private void NextFocusFromIcdAfer()
        {
            try
            {
                txtMethodCode.Focus();
                txtMethodCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextFocusFromIcdBefore()
        {
            try
            {
                this.IcdAfterProcessor.FocusControl(this.ucIcdAfter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextFocusFromIcdMain()
        {
            try
            {
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.FocusControl(ucSecondaryIcd);
                }
                //this.IcdBeforeProcessor.FocusControl(this.ucIcdBefore);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMethodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadMethod(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadMethod(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMethod.Focus();
                    cboMethod.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.PTTT_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMethod.EditValue = data[0].ID;
                            cboMethod.Properties.Buttons[1].Visible = true;
                            txtPhuongPhap2.Focus();
                            txtPhuongPhap2.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cboMethod.EditValue = search.ID;
                                cboMethod.Properties.Buttons[1].Visible = true;
                                txtPhuongPhap2.Focus();
                                txtPhuongPhap2.SelectAll();
                            }
                            else
                            {
                                cboMethod.EditValue = null;
                                cboMethod.Focus();
                                cboMethod.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboMethod.EditValue = null;
                        cboMethod.Focus();
                        cboMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMethod.Properties.Buttons[1].Visible = false;
                    cboMethod.EditValue = null;
                    txtMethodCode.Text = "";
                    txtMethodCode.Focus();
                    txtMethodCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMethod.EditValue.ToString()));
                        {
                            txtMethodCode.Text = data.PTTT_METHOD_CODE;
                            cboMethod.Properties.Buttons[1].Visible = true;
                            txtPhuongPhap2.Focus();
                            txtPhuongPhap2.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMethodCode.Text = data.PTTT_METHOD_CODE;
                            cboMethod.Properties.Buttons[1].Visible = true;
                            txtPhuongPhap2.Focus();
                            txtPhuongPhap2.SelectAll();
                        }
                    }
                }
                else
                {
                    cboMethod.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhuongPhap2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadPhuongPhap2(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadPhuongPhap2(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPhuongPhap2.Focus();
                    cboPhuongPhap2.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o =>
                        o.IS_ACTIVE == 1
                        && (o.IS_SECOND == 1 || (o.IS_FIRST != 1 && o.IS_SECOND != 1))
                        && o.EMOTIONLESS_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPhuongPhap2.EditValue = data[0].ID;
                            cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                            txtEmotionlessMethod.Focus();
                            txtEmotionlessMethod.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cboPhuongPhap2.EditValue = search.ID;
                                cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                                txtEmotionlessMethod.Focus();
                                txtEmotionlessMethod.SelectAll();
                            }
                            else
                            {
                                cboPhuongPhap2.EditValue = null;
                                cboPhuongPhap2.Focus();
                                cboPhuongPhap2.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboPhuongPhap2.EditValue = null;
                        cboPhuongPhap2.Focus();
                        cboPhuongPhap2.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhap2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPhuongPhap2.Properties.Buttons[1].Visible = false;
                    cboPhuongPhap2.EditValue = null;
                    txtPhuongPhap2.Text = "";
                    txtPhuongPhap2.Focus();
                    txtPhuongPhap2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhap2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhuongPhap2.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPhuongPhap2.EditValue.ToString()));
                        {
                            txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                            cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                            txtEmotionlessMethod.Focus();
                            txtEmotionlessMethod.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhap2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPhuongPhap2.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhuongPhap2.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                            cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                            txtEmotionlessMethod.Focus();
                            txtEmotionlessMethod.SelectAll();
                        }
                    }
                }
                else
                {
                    cboPhuongPhap2.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmotionlessMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadEmotionlessMethod(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadEmotionlessMethod(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbEmotionlessMethod.Focus();
                    cbbEmotionlessMethod.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o =>
                        o.IS_ACTIVE == 1
                        && (o.IS_FIRST == 1 || (o.IS_FIRST != 1 && o.IS_SECOND != 1))
                        && o.EMOTIONLESS_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbEmotionlessMethod.EditValue = data[0].ID;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            txtResultNote.Focus();
                            txtResultNote.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cbbEmotionlessMethod.EditValue = search.ID;
                                cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                                txtResultNote.Focus();
                                txtResultNote.SelectAll();
                            }
                            else
                            {
                                cbbEmotionlessMethod.EditValue = null;
                                cbbEmotionlessMethod.Focus();
                                cbbEmotionlessMethod.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cbbEmotionlessMethod.EditValue = null;
                        cbbEmotionlessMethod.Focus();
                        cbbEmotionlessMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbEmotionlessMethod.Properties.Buttons[1].Visible = false;
                    cbbEmotionlessMethod.EditValue = null;
                    txtEmotionlessMethod.Text = "";
                    txtEmotionlessMethod.Focus();
                    txtEmotionlessMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbEmotionlessMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            txtResultNote.Focus();
                            txtResultNote.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbEmotionlessMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            txtResultNote.Focus();
                            txtResultNote.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            grdViewInformationSurg.PostEditor();
            btnSave.Focus();

            bool success = false;
            try
            {
                HisSurgServiceReqUpdateSDO hisSurgResultSDO = new MOS.SDO.HisSurgServiceReqUpdateSDO();

                hisSurgResultSDO.SereServPttt = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT();
                hisSurgResultSDO.SereServExt = new HIS_SERE_SERV_EXT();
                hisSurgResultSDO.EkipUsers = new List<HIS_EKIP_USER>();


                if (this.sereServ != null)
                {
                    ProcessEkipUser(hisSurgResultSDO);
                    ProcessSereServPttt(hisSurgResultSDO);
                    ProcessSereServExt(hisSurgResultSDO);
                    SaveSurgServiceReq(hisSurgResultSDO, ref success);
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        void ProcessSereServPttt(HisSurgServiceReqUpdateSDO hisSurgResultSDO)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT HisSereServPttt = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT();
                if (this.sereServPTTT != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_PTTT>(HisSereServPttt, this.sereServPTTT);
                }

                HisSereServPttt.SERE_SERV_ID = sereServ.ID;
                if (this._ServiceReq != null)
                    HisSereServPttt.TDL_TREATMENT_ID = this._ServiceReq.TREATMENT_ID;


                if (this.ucIcdMain != null)
                {
                    var icdValue = this.IcdMainProcessor.GetValue(this.ucIcdMain);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        HisSereServPttt.ICD_CODE = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        HisSereServPttt.ICD_NAME = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }
                if (this.ucSecondaryIcd != null)
                {
                    var subIcd = this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        HisSereServPttt.ICD_SUB_CODE = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        HisSereServPttt.ICD_TEXT = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }


                if (this.ucIcdBefore != null)
                {
                    var icdValue = this.IcdBeforeProcessor.GetValue(this.ucIcdBefore);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        HisSereServPttt.BEFORE_PTTT_ICD_CODE = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        HisSereServPttt.BEFORE_PTTT_ICD_NAME = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }

                if (this.ucIcdAfter != null)
                {
                    var icdValue = this.IcdAfterProcessor.GetValue(this.ucIcdAfter);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        HisSereServPttt.AFTER_PTTT_ICD_CODE = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        HisSereServPttt.AFTER_PTTT_ICD_NAME = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }


                //Phuong phap vô cảm
                if (cbbEmotionlessMethod.EditValue != null)
                {
                    HisSereServPttt.EMOTIONLESS_METHOD_ID = (long)cbbEmotionlessMethod.EditValue;
                }
                else
                {
                    HisSereServPttt.EMOTIONLESS_METHOD_ID = null;
                }

                //Phuong phap PTTT
                if (cboMethod.EditValue != null)
                {
                    HisSereServPttt.PTTT_METHOD_ID = (long)cboMethod.EditValue;
                }
                else
                {
                    HisSereServPttt.PTTT_METHOD_ID = null;
                }


                if (cboPhuongPhap2.EditValue != null)
                {
                    HisSereServPttt.EMOTIONLESS_METHOD_SECOND_ID = (long)cboPhuongPhap2.EditValue;
                }
                else
                {
                    HisSereServPttt.EMOTIONLESS_METHOD_SECOND_ID = null;
                }


                hisSurgResultSDO.SereServPttt = HisSereServPttt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessEkipUser(HisSurgServiceReqUpdateSDO hisSurgResultSDO)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
                var sereServPTTTADOs = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                if (sereServPTTTADOs != null && sereServPTTTADOs.Count > 0)
                {
                    foreach (var item in sereServPTTTADOs)
                    {
                        MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, item);

                        var acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == ekipUser.LOGINNAME);
                        if (acsUser != null)
                        {
                            ekipUser.USERNAME = acsUser.USERNAME;
                        }

                        if (sereServ.EKIP_ID.HasValue)
                        {
                            ekipUser.EKIP_ID = sereServ.EKIP_ID.Value;
                        }

                        ekipUsers.Add(ekipUser);
                    }
                }
                hisSurgResultSDO.EkipUsers = ekipUsers;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSereServExt(HisSurgServiceReqUpdateSDO hisSurgResultSDO)
        {
            try
            {
                if (this.sereServ != null)
                {
                    if (SereServExt == null)
                    {
                        SereServExt = new HIS_SERE_SERV_EXT();
                    }
                    SereServExt.NOTE = txtResultNote.Text.Trim();
                    SereServExt.SERE_SERV_ID = this.sereServ.ID;
                    hisSurgResultSDO.SereServExt = SereServExt;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SaveSurgServiceReq(HisSurgServiceReqUpdateSDO hisSuimResultSDO, ref bool success, [Optional] bool notShowMess)
        {
            CommonParam param = new CommonParam();
            success = false;
            try
            {
                if (this.sereServ == null)
                    throw new Exception("Khong tim thay dich vu");
                hisSuimResultSDO.SereServId = this.sereServ.ID;

                WaitingManager.Show();
                var currentHisSurgResultSDO = new BackendAdapter(param)
                  .Post<MOS.SDO.HisSurgServiceReqUpdateSDO>("api/HisServiceReq/SurgUpdate", ApiConsumers.MosConsumer, hisSuimResultSDO, param);
                if (currentHisSurgResultSDO != null)
                {
                    success = true;
                }
                WaitingManager.Hide();


                #region Show message
                MessageManager.Show(this.ParentForm, param, success);

                if (success)
                {
                    this.Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void grdViewInformationSurg_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((grdViewInformationSurg.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = btnAdd;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = btnDelete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadData()
        {
            GetSereServByServiceReq();
            GetSereServPTTT();
            GetSereServEXT();
        }

        private void GetSereServByServiceReq()
        {
            try
            {
                CommonParam param = new Inventec.Core.CommonParam();
                this.sereServ = new HIS_SERE_SERV();
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.SERVICE_REQ_ID = this._ServiceReq.ID;
                List<HIS_SERE_SERV> sereServs = new BackendAdapter(param)
                    .Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.sereServ = sereServs.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetSereServPTTT()
        {
            try
            {
                CommonParam param = new Inventec.Core.CommonParam();
                this.sereServPTTT = new V_HIS_SERE_SERV_PTTT();
                HisSereServPtttViewFilter sereServFilter = new HisSereServPtttViewFilter();
                sereServFilter.SERE_SERV_ID = this.sereServ.ID;
                List<V_HIS_SERE_SERV_PTTT> sereServs = new BackendAdapter(param)
                    .Get<List<V_HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, sereServFilter, param);
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.sereServPTTT = sereServs.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetSereServEXT()
        {
            try
            {
                CommonParam param = new Inventec.Core.CommonParam();
                this.SereServExt = new HIS_SERE_SERV_EXT();
                HisSereServExtFilter sereServFilter = new HisSereServExtFilter();
                sereServFilter.SERE_SERV_ID = this.sereServ.ID;
                List<HIS_SERE_SERV_EXT> sereServs = new BackendAdapter(param)
                    .Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, sereServFilter, param);
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.SereServExt = sereServs.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataDefaultToControl()
        {
            try
            {
                if (sereServ != null)
                {
                    CommonParam param = new CommonParam();

                    List<HisEkipUserADO> ekipUserAdos = new List<HisEkipUserADO>();
                    if (sereServ.EKIP_ID.HasValue)
                    {
                        HisEkipUserViewFilter hisEkipUserFilter = new HisEkipUserViewFilter();
                        hisEkipUserFilter.EKIP_ID = sereServ.EKIP_ID;
                        var lst = new BackendAdapter(param)
                .Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, hisEkipUserFilter, param);

                        if (lst.Count > 0)
                        {
                            foreach (var item in lst)
                            {
                                Mapper.CreateMap<V_HIS_EKIP_USER, HisEkipUserADO>();
                                var HisEkipUserProcessing = Mapper.Map<V_HIS_EKIP_USER, HisEkipUserADO>(item);
                                if (item != lst[0])
                                {
                                    HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                                }
                                ekipUserAdos.Add(HisEkipUserProcessing);
                            }
                            grdControlInformationSurg.DataSource = null;
                            grdControlInformationSurg.DataSource = ekipUserAdos;
                        }
                    }
                    else if (this._ServiceReq.EKIP_PLAN_ID.HasValue) //tiennv
                    {
                        HisEkipPlanUserViewFilter hisEkipPlanUserFilter = new HisEkipPlanUserViewFilter();
                        hisEkipPlanUserFilter.EKIP_PLAN_ID = this._ServiceReq.EKIP_PLAN_ID;
                        var lst = new BackendAdapter(param)
                .Get<List<V_HIS_EKIP_PLAN_USER>>("api/HisEkipPlanUser/GetView", ApiConsumers.MosConsumer, hisEkipPlanUserFilter, param);

                        if (lst.Count > 0)
                        {
                            foreach (var item in lst)
                            {
                                Mapper.CreateMap<V_HIS_EKIP_PLAN_USER, HisEkipUserADO>();
                                var HisEkipUserProcessing = Mapper.Map<V_HIS_EKIP_PLAN_USER, HisEkipUserADO>(item);
                                if (item != lst[0])
                                {
                                    HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                                }
                                ekipUserAdos.Add(HisEkipUserProcessing);
                            }
                            grdControlInformationSurg.DataSource = null;
                            grdControlInformationSurg.DataSource = ekipUserAdos;
                        }
                    }

                    if (SereServExt != null)
                    {
                        txtResultNote.Text = SereServExt.NOTE;
                    }
                    else
                    {
                        txtResultNote.Text = "";
                    }


                    List<HIS_ICD> icds = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>();

                    if (this.sereServPTTT != null)
                    {

                        LoadIcdToControl(this.IcdMainProcessor, this.sereServPTTT.ICD_CODE, this.sereServPTTT.ICD_NAME, this.ucIcdMain);
                        LoadIcdToControl(this.IcdBeforeProcessor, this.sereServPTTT.BEFORE_PTTT_ICD_CODE, this.sereServPTTT.BEFORE_PTTT_ICD_NAME, this.ucIcdBefore);
                        LoadIcdToControl(this.IcdAfterProcessor, this.sereServPTTT.AFTER_PTTT_ICD_CODE, this.sereServPTTT.AFTER_PTTT_ICD_NAME, this.ucIcdAfter);

                        SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                        subIcd.ICD_SUB_CODE = this.sereServPTTT.ICD_SUB_CODE;
                        subIcd.ICD_TEXT = this.sereServPTTT.ICD_TEXT;
                        if (ucSecondaryIcd != null)
                        {
                            subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                        }

                        txtMethodCode.Text = this.sereServPTTT.PTTT_METHOD_CODE;
                        cboMethod.EditValue = this.sereServPTTT.PTTT_METHOD_ID;
                        txtEmotionlessMethod.Text = this.sereServPTTT.EMOTIONLESS_METHOD_CODE;
                        cbbEmotionlessMethod.EditValue = this.sereServPTTT.EMOTIONLESS_METHOD_ID;


                        if (this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID > 0)
                        {
                            var data = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(p => p.ID == this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID);
                            if (data != null)
                            {
                                txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                                cboPhuongPhap2.EditValue = data.ID;
                            }
                            else
                            {
                                txtPhuongPhap2.Text = "";
                                cboPhuongPhap2.EditValue = null;
                            }
                        }

                    }
                }
                else
                {
                    txtResultNote.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToInformationSurg(bool? isClick = null)
        {
            try
            {
                if (grdControlInformationSurg == null
                    || grdControlInformationSurg.DataSource == null)
                {
                    List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                    string executeRoleDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.PLUGINS.SURGSERVICEREQEXECUTE.EXECUTE_ROLE_DEFAULT");
                    List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> executeRoles = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();

                    if (!String.IsNullOrEmpty(executeRoleDefault))
                    {
                        string[] str = executeRoleDefault.Split(',');
                        List<string> executeRoleCodes = new List<string>(str);
                        var executeRoleDefaults = executeRoles.Where(o => executeRoleCodes.Contains(o.EXECUTE_ROLE_CODE))
                            .OrderBy(o => o.EXECUTE_ROLE_CODE).ToList();
                        foreach (var executeRole in executeRoleDefaults)
                        {
                            HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                            ekipUserAdoTemp.EXECUTE_ROLE_ID = executeRole.ID;

                            if (ekipUserAdoTemps.Count == 0)
                            {
                                ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            }
                            else
                            {
                                ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                            }
                            ekipUserAdoTemps.Add(ekipUserAdoTemp);
                        }
                    }
                    else
                    {
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }

                    if (ekipUserAdoTemps == null || ekipUserAdoTemps.Count == 0)
                    {
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                    grdControlInformationSurg.DataSource = ekipUserAdoTemps;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdToControl(HIS.UC.Icd.IcdProcessor IcdProcessor, string icdCode, string icdName, UserControl ucIcd)
        {
            try
            {
                UC.Icd.ADO.IcdInputADO icd = new UC.Icd.ADO.IcdInputADO();
                icd.ICD_CODE = icdCode;
                icd.ICD_NAME = icdName;
                if (ucIcd != null)
                {
                    IcdProcessor.Reload(ucIcd, icd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

    }
}
