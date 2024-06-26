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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LocalStorage.Location;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.PtttMethod
{
    public partial class FormPtttMethod : FormBase
    {
        Action<List<PtttMethodADO>> ChooseData;

        List<PtttMethodADO> DataSource;
        V_HIS_SERE_SERV_5 CurrentSereServ;
        List<PtttMethodADO> SelectedData = new List<PtttMethodADO>();
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO = new List<Desktop.Library.CacheClient.ControlStateRDO>();
        private string MODULELINK = "FormEkipUser";
        PtttMethodADO currentPtttEkip;
        Inventec.Desktop.Common.Modules.Module CurrentModuleData;
        List<HIS_PTTT_GROUP> lstGroup;
        List<PtttMethodADO> lstSelect = new List<PtttMethodADO>();
        public FormPtttMethod(Action<List<PtttMethodADO>> chooseData, V_HIS_SERE_SERV_5 sereServ, List<PtttMethodADO> oldSelect, Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            this.ChooseData = chooseData;
            this.CurrentSereServ = sereServ;
            this.CurrentModuleData = moduleData;
            if (oldSelect != null && oldSelect.Count > 0)
            {
                SelectedData.AddRange(oldSelect);
            }
        }

        private void FormPtttMethod_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                this.SetCaptionByLanguageKey();
                pictureEdit1.Image = imageCollection1.Images[1];
                lstGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>();
                ComboPTMethod();
                CreateDataSource();
                FillDataToGrid();
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
                DataSource = lstSelect;
                //var query = DataSource.AsQueryable();
                string keyword = txtKeyword.Text.Trim();
                keyword = keyword.Trim().ToLower();
                if (!String.IsNullOrEmpty(keyword))
                {
                    DataSource = DataSource.Where(o =>
                        o.PTTT_METHOD_CODE.ToLower().Contains(keyword)
                        || o.PTTT_METHOD_NAME.ToLower().Contains(keyword)
                        ).ToList();
                }

                DataSource = DataSource.OrderBy(o => o.PTTT_METHOD_CODE).ToList();

                gridControlPtttMethod.BeginUpdate();
                gridControlPtttMethod.DataSource = DataSource;
                gridControlPtttMethod.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateDataSource()
        {
            try
            {
                this.DataSource = new List<PtttMethodADO>();
                List<HIS_PTTT_METHOD> listData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>();

                foreach (var item in listData)
                {
                    if (item.IS_ACTIVE == 1)
                    {
                        PtttMethodADO data = new PtttMethodADO();
                        data.ID = item.ID;
                        data.PTTT_METHOD_CODE = item.PTTT_METHOD_CODE;
                        data.PTTT_METHOD_NAME = item.PTTT_METHOD_NAME;
                        data.PTTT_GROUP_ID = item.PTTT_GROUP_ID;
                        data.SERE_SERV_ID = CurrentSereServ.ID;
                        if (this.SelectedData != null && this.SelectedData.Count > 0)
                        {
                            var old = this.SelectedData.FirstOrDefault(o => o.ID == item.ID);
                            if (old != null)
                            {
                                data.IS_SELECTION = true;
                                data.PTTT_GROUP_ID = old.PTTT_GROUP_ID;
                                data.EkipUsersADO = old.EkipUsersADO;
                                data.AMOUNT = old.AMOUNT;
                                data.EKIP_ID = old.EKIP_ID;
                                data.SERVICE_REQ_ID = old.SERVICE_REQ_ID;
                                if (data.PTTT_GROUP_ID == null)
                                {
                                    data.PTTT_GROUP_NAME = old.PTTT_GROUP_NAME;
                                }
                                else
                                {
                                    data.PTTT_GROUP_NAME = old.PTTT_GROUP_ID != null ? lstGroup.Where(o => o.ID == old.PTTT_GROUP_ID).FirstOrDefault().PTTT_GROUP_NAME : "";
                                }
                            }
                        }

                        this.DataSource.Add(data);
                    }
                }
                lstSelect = DataSource;
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
                    FillDataToGrid();
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

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstSelect != null && lstSelect.Count > 0)
                {
                    var listCheck = lstSelect.Where(o => o.IS_SELECTION).ToList();
                    if (listCheck == null || listCheck.Count <= 0)
                    {
                        if (XtraMessageBox.Show(ResourceMessage.BanChuaChonPhuongPhapNao, ResourceMessage.ThongBao, MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                            return;
                    }


                    if (this.ChooseData != null)
                    {
                        listCheck.ForEach(o => o.SERVICE_REQ_ID = this.CurrentSereServ.SERVICE_REQ_ID ?? 0);
                        this.ChooseData(listCheck);
                        this.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }
        private void gridViewPtttMethod_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                PtttMethodADO data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewPtttMethod.GetDataSourceRowIndex(e.RowHandle);
                    data = (PtttMethodADO)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        if (e.Column.FieldName == "PTTT_GROUP_NAME")
                        {
                            if (data.IS_SELECTION && data.PTTT_GROUP_ID == null)
                            {
                                e.RepositoryItem = GridLU_PTTT;
                            }
                            else
                            {
                                e.RepositoryItem = null;
                            }
                        }
                        else if (e.Column.FieldName == "IS_SELECTION")
                        {
                            if (data.IS_SELECTION)
                            {
                                e.RepositoryItem = BbtnCheck;
                            }
                            else
                            {
                                e.RepositoryItem = BbtnUnCheck;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void gridViewPtttMethod_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view.FocusedColumn.FieldName == "PTTT_GROUP_NAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;

                    ComboPTTT(editor);

                    gridViewPtttMethod.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ComboPTTT(GridLookUpEdit cbo)
        {
            try
            {
                List<HIS_PTTT_GROUP> lstGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>();
                cbo.Properties.DataSource = lstGroup;
                cbo.Properties.DisplayMember = "PTTT_GROUP_NAME";
                cbo.Properties.ValueMember = "ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("PTTT_GROUP_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("PTTT_GROUP_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 150;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboPTMethod()
        {
            try
            {
                List<HIS_PTTT_GROUP> lstGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_CODE", "Mã", 50, 1));
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_NAME", "Tên", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_GROUP_NAME", "ID", columnInfos, true, 150);
                ControlEditorLoader.Load(GridLU_PTTT, lstGroup, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BbtnUser_PTTT_Click(object sender, EventArgs e)
        {
            try
            {
                var currentSelect = (PtttMethodADO)gridViewPtttMethod.GetFocusedRow();
                if (currentSelect.IS_SELECTION)
                {
                    currentPtttEkip = currentSelect;
                    FormEkipUser frm = new FormEkipUser(SetEkipUser, currentSelect, CurrentModuleData);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEkipUser(EkipUsersADO data)
        {
            try
            {
                var Alls = this.gridControlPtttMethod.DataSource as List<PtttMethodADO>;
                int currentIndex = 0;
                for (int i = 0; i < Alls.Count; i++)
                {
                    var item = Alls[i];
                    if (item.ID == currentPtttEkip.ID)
                    {
                        currentIndex = i;
                        item.EkipUsersADO = data;
                    }
                }
                gridControlPtttMethod.DataSource = new List<PtttMethodADO>();
                gridControlPtttMethod.DataSource = Alls;
                gridViewPtttMethod.FocusedRowHandle = currentIndex;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BbtnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                var currentCheck = (PtttMethodADO)gridViewPtttMethod.GetFocusedRow();
                var Alls = gridControlPtttMethod.DataSource as List<PtttMethodADO>;
                int currentIndex = 0;
                for (int i = 0; i < Alls.Count; i++)
                {
                    var item = Alls[i];
                    if (item.ID == currentCheck.ID)
                    {
                        currentIndex = i;
                        item.IS_SELECTION = false;
                        item.PTTT_GROUP_NAME = null;
                        item.AMOUNT = null;
                        item.EkipUsersADO = null;
                        break;
                    }
                }

                gridControlPtttMethod.DataSource = new List<PtttMethodADO>();
                gridControlPtttMethod.DataSource = Alls;
                gridViewPtttMethod.FocusedRowHandle = currentIndex;

                foreach (var item in lstSelect)
                {
                    if (item.ID == currentCheck.ID)
                    {
                        item.IS_SELECTION = false;
                        item.PTTT_GROUP_NAME = null;
                        item.AMOUNT = null;
                        item.EkipUsersADO = null;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BbtnUnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                List<HIS_PTTT_GROUP> lstGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>();
                var currentUnCheck = (PtttMethodADO)gridViewPtttMethod.GetFocusedRow();
                var Alls = gridControlPtttMethod.DataSource as List<PtttMethodADO>;
                int currentIndex = 0;
                for (int i = 0; i < Alls.Count; i++)
                {
                    var item = Alls[i];
                    if (item.ID == currentUnCheck.ID)
                    {
                        currentIndex = i;
                        item.IS_SELECTION = true;
                        item.PTTT_GROUP_NAME = item.PTTT_GROUP_ID != null ? lstGroup.Where(o => o.ID == item.PTTT_GROUP_ID).FirstOrDefault().PTTT_GROUP_NAME : "";
                        item.AMOUNT = 1;
                        break;
                    }
                }

                gridControlPtttMethod.DataSource = new List<PtttMethodADO>();
                gridControlPtttMethod.DataSource = Alls;
                gridViewPtttMethod.FocusedRowHandle = currentIndex;

                foreach (var item in lstSelect)
                {
                    if (item.ID == currentUnCheck.ID)
                    {
                        item.IS_SELECTION = true;
                        item.PTTT_GROUP_NAME = item.PTTT_GROUP_ID != null ? lstGroup.Where(o => o.ID == item.PTTT_GROUP_ID).FirstOrDefault().PTTT_GROUP_NAME : "";
                        item.AMOUNT = 1;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void pictureEdit1_Click(object sender, EventArgs e)
        {
            try
            {

                var Alls = this.gridControlPtttMethod.DataSource as List<PtttMethodADO>;
                if (pictureEdit1.Image == imageCollection1.Images[1])
                {
                    pictureEdit1.Image = imageCollection1.Images[0];
                    foreach (var item in Alls)
                    {
                        item.IS_SELECTION = true;
                        item.PTTT_GROUP_NAME = item.PTTT_GROUP_ID != null ? lstGroup.Where(o => o.ID == item.PTTT_GROUP_ID).FirstOrDefault().PTTT_GROUP_NAME : "";
                        item.AMOUNT = 1;
                    }
                }
                else
                {
                    pictureEdit1.Image = imageCollection1.Images[1];

                    foreach (var item in Alls)
                    {
                        item.IS_SELECTION = false;
                        item.PTTT_GROUP_NAME = null;
                        item.AMOUNT = null;
                    }
                }

                gridControlPtttMethod.DataSource = new List<PtttMethodADO>();
                gridControlPtttMethod.DataSource = Alls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormPtttMethod
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(FormPtttMethod).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormPtttMethod.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormPtttMethod.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.barBtnSearch.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.barBtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.gc_icon.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_icon.Caption", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.gc_MethodCode.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_MethodCode.Caption", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.gc_MethodName.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_MethodName.Caption", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.gc_PTTT.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_PTTT.Caption", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.gc_PTTT.ToolTip = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_PTTT.ToolTip", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.gc_amount.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_amount.Caption", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEdit_PTTT.NullText = Inventec.Common.Resource.Get.Value("FormPtttMethod.repositoryItemGridLookUpEdit_PTTT.NullText", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.GridLU_PTTT.NullText = Inventec.Common.Resource.Get.Value("FormPtttMethod.GridLU_PTTT.NullText", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("FormPtttMethod.btnChoose.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FormPtttMethod.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormPtttMethod.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormPtttMethod.Text", Resources.ResourceLanguageManager.LanguageResource__FormPtttMethod, LanguageManager.GetCulture());
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
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisPtttMethod").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisPtttMethod'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.CurrentModuleData.RoomId;
                    moduleData.RoomTypeId = this.CurrentModuleData.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    if (!String.IsNullOrEmpty(txtKeyword.Text.Trim()))
                        listArgs.Add(txtKeyword.Text.Trim());
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                }
                BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_PTTT_METHOD>();

                CreateDataSource();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchClick(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SearchClick(string keyword)
        {
            try
            {   
                List<PtttMethodADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<PtttMethodADO> searchResult = new List<PtttMethodADO>();

                    searchResult = lstSelect.Where(o => o.PTTT_METHOD_CODE.ToLower().Contains(keyword.Trim().ToLower()) || (o.PTTT_METHOD_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())).Distinct().ToList();
                    listResult = searchResult;
                }
                else
                {
                    listResult = lstSelect;
                }
                gridControlPtttMethod.BeginUpdate();
                gridControlPtttMethod.DataSource = listResult;
                gridControlPtttMethod.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }
    }
}
