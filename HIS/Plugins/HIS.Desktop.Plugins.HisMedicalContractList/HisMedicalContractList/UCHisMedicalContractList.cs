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
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.HisMedicalContractList.DetailForm;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisMedicalContractList.Base;
using HIS.Desktop.IsAdmin;

namespace HIS.Desktop.Plugins.HisMedicalContractList.HisMedicalContractList
{
    public partial class UCHisMedicalContractList : UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int lastRowHandle = -1;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        internal DetailForm.frmDetail FormDetail { get; set; }
        V_HIS_MEDICAL_CONTRACT currentData = null;
        List<V_HIS_MEDICAL_CONTRACT> LstHisMedicalContractList = new List<V_HIS_MEDICAL_CONTRACT>();
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        string loginName = null;

        public UCHisMedicalContractList()
        {
            InitializeComponent();
        }

        public UCHisMedicalContractList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            currentModule = module;
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisMedicalContractList_Load(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
                SetCaptionByLanguageKey();
                SetDefaultValue();
                InitComboSupplier();
                InitComboBid();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMedicalContractList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMedicalContractList.HisMedicalContractList.UCHisMedicalContractList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControl1.Text = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.navBarControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup1.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.navBarGroup1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSupplier.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.cboSupplier.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup2.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.navBarGroup2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCHisMedicalContractList.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                int pageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                //gridControlTreatmentList.DataSource = null;
                FillDataToGridContract(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridContract, param, pageSize, gridControlHisMedicalContractList);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGridContract(object param)
        {
            try
            {
                start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisMedicalContractViewFilter filter = new HisMedicalContractViewFilter();

                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!string.IsNullOrEmpty(txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text;
                }
                if ((dtFrom.EditValue != null && dtFrom.EditValue.ToString() != ""))
                {
                    filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                         Convert.ToDateTime(dtFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if ((dtTo.EditValue != null && dtTo.EditValue.ToString() != ""))
                {
                    filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                         Convert.ToDateTime(dtTo.EditValue).ToString("yyyyMMdd") + "000000");
                }

                if ((cboSupplier.EditValue != null && cboSupplier.EditValue.ToString() != ""))
                {
                    filter.SUPPLIER_ID = (long)cboSupplier.EditValue;
                }

                if ((cboBid.EditValue != null && cboBid.EditValue.ToString() != ""))
                {
                    filter.BID_ID = (long?)cboBid.EditValue;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                if (result != null)
                {
                    LstHisMedicalContractList = (List<V_HIS_MEDICAL_CONTRACT>)result.Data;
                    this.LstHisMedicalContractList = LstHisMedicalContractList.OrderBy(o => o.BID_NUMBER).ToList();


                    gridControlHisMedicalContractList.BeginUpdate();
                    gridControlHisMedicalContractList.DataSource = null;
                    gridControlHisMedicalContractList.DataSource = LstHisMedicalContractList;
                    rowCount = (LstHisMedicalContractList == null ? 0 : LstHisMedicalContractList.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    gridControlHisMedicalContractList.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSupplier_EditValueChanged(object sender, EventArgs e)
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

        private void InitComboSupplier()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSupplierFilter filter = new HisSupplierFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "SUPPLIER_CODE";
                var listSupplier = new BackendAdapter(param).Get<List<HIS_SUPPLIER>>("api/HisSupplier/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SUPPLIER_CODE", "Mã nhà thầu", 100, 1));
                columnInfos.Add(new ColumnInfo("SUPPLIER_NAME", "Tên nhà thầu", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SUPPLIER_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboSupplier, listSupplier, controlEditorADO);
                cboSupplier.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBid()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBidFilter filter = new HisBidFilter();
                filter.IS_ACTIVE = 1;
                //filter.ORDER_DIRECTION = "DESC";
                //filter.ORDER_FIELD = "BID_YEAR";                
                var listBid = new BackendAdapter(param).Get<List<HIS_BID>>("api/HisBid/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BID_NUMBER", "Số QĐ", 200, 1));
                columnInfos.Add(new ColumnInfo("BID_NAME", "Tên", 300, 2));
                columnInfos.Add(new ColumnInfo("BID_YEAR", "Năm thầu", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BID_NUMBER", "ID", columnInfos, true, 350);
                var listSort = listBid.OrderByDescending(o => o.BID_YEAR).ThenByDescending(o => o.CREATE_TIME).ToList();
                ControlEditorLoader.Load(cboBid, listSort, controlEditorADO);
                cboBid.Properties.ImmediatePopup = true;
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
                txtKeyword.Text = "";
                dtFrom.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtTo.EditValue = DateTime.Now;
                cboSupplier.EditValue = null;
                cboBid.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHisMedicalContractList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_HIS_MEDICAL_CONTRACT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + start;
                        }
                        else if (e.Column.FieldName == "VALID_FROM_DATE_STR")
                        {
                            if (data.VALID_FROM_DATE.HasValue)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.VALID_FROM_DATE.Value);
                            }
                        }
                        else if (e.Column.FieldName == "VALID_TO_DATE_STR")
                        {
                            if (data.VALID_TO_DATE.HasValue)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.VALID_TO_DATE.Value);
                            }
                        }
                        else if (e.Column.FieldName == "CREAT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonViewDetail_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDICAL_CONTRACT)gridViewHisMedicalContractList.GetFocusedRow();
                FormDetail = new frmDetail(currentModule, rowData);
                FormDetail.StartPosition = FormStartPosition.CenterScreen;
                FormDetail.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDICAL_CONTRACT)gridViewHisMedicalContractList.GetFocusedRow();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisMedicalContractCreate").FirstOrDefault();
                //var row = (PSS.EFMODEL.DataModels.PSS_ACCOUNT)gridViewAccount.GetFocusedRow();
                //if (row != null)
                //{
                //    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MediReactCreate").FirstOrDefault();
                //    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.MediReactCreate'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();

                    listArgs.Add(rowData.ID);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                    //SetDefaultValue();
                }
                else
                {
                    //MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisMedicalContractList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewHisMedicalContractList.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    short isDelete = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewHisMedicalContractList.GetRowCellValue(e.RowHandle, "IS_DELETE") ?? "").ToString());
                    string creator = (gridViewHisMedicalContractList.GetRowCellValue(e.RowHandle, "CREATOR") ?? "0").ToString();
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        if (creator == this.loginName)
                        {
                            e.RepositoryItem = ((isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) ? repositoryItemButtonUnLock : repositoryItemButtonLock);
                        }
                        else
                        {
                            e.RepositoryItem = ((isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) ? repositoryItemButtonUnLock_Disable : repositoryItemButtonLock_Disable);
                        }
                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = ((isDelete != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE && creator == this.loginName && isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) ? repositoryItemButtonDelete_Enable : repositoryItemButtonDelete_Disable);
                    }

                    if (e.Column.FieldName == "Edit")
                    {
                        e.RepositoryItem = ((isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && creator == this.loginName || controlAcs != null && (controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.repositoryItemButtonEdit) != null)) ?
                        repositoryItemButtonEdit_Enable : repositoryItemButtonEdit_Disable);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDICAL_CONTRACT)gridViewHisMedicalContractList.GetFocusedRow();
                CommonParam param = new CommonParam();
                if (rowData != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>("api/HisMedicalContract/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGrid();
                            currentData = ((List<V_HIS_MEDICAL_CONTRACT>)gridControlHisMedicalContractList.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDICAL_CONTRACT result = new HIS_MEDICAL_CONTRACT();
            bool success = false;
            try
            {

                V_HIS_MEDICAL_CONTRACT data = (V_HIS_MEDICAL_CONTRACT)gridViewHisMedicalContractList.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDICAL_CONTRACT>("api/HisMedicalContract/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGrid();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDICAL_CONTRACT result = new HIS_MEDICAL_CONTRACT();
            bool success = false;
            try
            {

                V_HIS_MEDICAL_CONTRACT data = (V_HIS_MEDICAL_CONTRACT)gridViewHisMedicalContractList.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDICAL_CONTRACT>("api/HisMedicalContract/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGrid();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
