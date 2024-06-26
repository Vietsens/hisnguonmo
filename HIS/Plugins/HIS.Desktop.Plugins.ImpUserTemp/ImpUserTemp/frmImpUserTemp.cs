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
using Inventec.Common.Logging;
using Inventec.UC.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;

using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System.Resources;
using MOS.Filter;

namespace HIS.Desktop.Plugins.ImpUserTemp.ImpUserTemp
{
    public partial class frmImpUserTemp : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        List<long> listID = new List<long>();
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        HIS_IMP_USER_TEMP currentData;
        List<string> arrImpUserTempName = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion
        #region Construct
        public frmImpUserTemp(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;


                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon
                        (System.IO.Path.Combine
                        (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                        System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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

        #region Private Method
        private void frmImpUserTemp_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void Show()
        {
            //Gan gia tri mac dinh
            SetDefaultValue();
            //Set enable control default
            EnableControlChanged(this.ActionType);
            //Set tabindex control
            InitTabIndex();
            //set ngon ngu
            SetCaptionByLanguagekey();

            //Focus default
            SetDefaultFocus();
            //Load du lieu
            FillDataToGridControl();

        }
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP>> apiResult = null;
                HisImpUserTempFilter filter = new HisImpUserTempFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyword.Text.Trim(); 
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP>>(ImpRequestUriStore.IMP_USER_TEMP_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {

                    var data = (List<MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP>)apiResult.Data;
                    if (data != null)
                    {
                    
                        gridView1.GridControl.DataSource = data;
                        arrImpUserTempName = data.Select(o => o.IMP_USER_TEMP_NAME).ToList();
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
      


        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetCaptionByLanguagekey()
        {
            try
            {

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ImpUserTemp.Resources.Lang", typeof(HIS.Desktop.Plugins.ImpUserTemp.ImpUserTemp.frmImpUserTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmImpUserTemp.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmImpUserTemp.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmImpUserTemp.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmImpUserTemp.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmImpUserTemp.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmImpUserTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmImpUserTemp.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmImpUserTemp.grdColEdit.Tooltip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelete.ToolTip = Inventec.Common.Resource.Get.Value("frmImpUserTemp.grdColDelete.Tooltip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColumn2.Caption = Inventec.Common.Resource.Get.Value("frmImpUserTemp.grdColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColumn3.Caption = Inventec.Common.Resource.Get.Value("frmImpUserTemp.grdColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSTT.Caption = Inventec.Common.Resource.Get.Value("frmImpUserTemp.grdColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmImpUserTemp.grdColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmImpUserTemp.grdColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmImpUserTemp.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSTT.Caption = Inventec.Common.Resource.Get.Value("frmImpUserTemp.grdColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColumn4.Caption = Inventec.Common.Resource.Get.Value("frmImpUserTemp.grdColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmImpUserTemp.grdColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void EnableControlChanged(int action)
        {
            try
            {
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
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
                dicOrderTabIndexControl.Add("txtSysStse39t5m941tzg576s0uhca7", 0);
                dicOrderTabIndexControl.Add("btnSearch", 1);
                dicOrderTabIndexControl.Add("btnAdd", 2);

                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, layoutControl1);
                    }
                }
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
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }
        #endregion
        #region Button Handle

        private void SetFocusEditor()
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
        private void ChangedDataRow(HIS_IMP_USER_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    positionHandle = -1;
                    //                   Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dx, dxErrorProvider1);
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDatatoImpTempUser(MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    CommonParam paramCommon = new CommonParam();
                    HisImpUserTempDtFilter filter = new HisImpUserTempDtFilter();

                    filter.IMP_USER_TEMP_ID = data.ID;

                    var apiResult = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP_DT>>(ImpRequestUriStore.IMP_USER_TEMP_DT_GET, ApiConsumers.MosConsumer, filter, paramCommon);

                    if (apiResult != null)
                    {
                        var dataDt = (List<HIS_IMP_USER_TEMP_DT>)apiResult;
                        if (dataDt != null)
                        {
                            gridView2.GridControl.DataSource = dataDt;
                            listID = dataDt.Select(o => o.ID).ToList();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_IMP_USER_TEMP)gridView1.GetFocusedRow();
                if (rowData != null)
                {

                    ChangedDataRow(rowData);
                    FillDatatoImpTempUser(rowData);

                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (HIS_IMP_USER_TEMP)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    var rowData = (HIS_IMP_USER_TEMP)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                      
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(ImpRequestUriStore.IMP_USER_TEMP_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
                    
                        if (success)
                        {
                            FillDataToGridControl();
                            currentData = ((List<HIS_IMP_USER_TEMP>)gridControl1.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                        refeshData();
                    }
                }

            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                frmInforImpUserTemp frm = new frmInforImpUserTemp(refeshData, moduleData, arrImpUserTempName);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    HIS_IMP_USER_TEMP pData = (MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IMP_USER_TEMP_NAME_STR")
                    {
                        try
                        {
                            e.Value = pData.IMP_USER_TEMP_NAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot IMP_USER_TEMP_NAME", ex);
                        }
                    }
                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_IMP_USER_TEMP)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    HIS_IMP_USER_TEMP_DT pData = (MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP_DT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "grdColSTT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "EXECUTE_ROLE_ID_STR")
                    {
                        try
                        {

                            if (pData.EXECUTE_ROLE_ID != null)
                            {
                                CommonParam paramCommon = new CommonParam();
                                HisExecuteRoleFilter filter = new HisExecuteRoleFilter();
                                filter.ORDER_FIELD = "MODIFY_TIME";
                                filter.ORDER_DIRECTION = "DESC";
                                filter.ID = pData.EXECUTE_ROLE_ID;

                                var apiResult = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>>(ExecuteRoleRequestUriStore.EXECUTE_ROLE_GET, ApiConsumers.MosConsumer, filter, paramCommon);

                                if (apiResult != null)
                                {
                                    var data = (List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>)apiResult;
                                    if (data != null)
                                    {
                                        e.Value = data.FirstOrDefault().EXECUTE_ROLE_NAME;
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot EXECUTE_ROLE_ID", ex);
                        }
                    }
                    else if (e.Column.FieldName == "USERNAME_STR")
                    {
                        try
                        {
                            e.Value = pData.USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot USERNAME", ex);
                        }
                    }
                    gridControl2.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    var rowData = (HIS_IMP_USER_TEMP)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        frmInforImpUserTemp frm = new frmInforImpUserTemp(refeshData, moduleData, rowData.ID, rowData.IMP_USER_TEMP_NAME, arrImpUserTempName);
                        frm.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void refeshData()
        {
            txtKeyword.Text = "";
            txtKeyword.Focus();
            FillDataToGridControl();
            this.currentData = (MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP)gridView1.GetFocusedRow();
            if (this.currentData != null)
            {

                ChangedDataRow(this.currentData);
                SetFocusEditor();
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
