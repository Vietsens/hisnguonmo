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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Common.Logging;
using Inventec.UC.Paging;
using System.Resources;
using DevExpress.XtraEditors.ViewInfo;
namespace HIS.Desktop.Plugins.HisExpiredDateCFG
{
    public partial class frmHisExpiredDateCFG : HIS.Desktop.Utility.FormBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        int positionHandleControlPatientInfo = -1;
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_EXPIRED_DATE_CFG currentData;
        public frmHisExpiredDateCFG(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
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
        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                spnEXPIRED_DAY.EditValue = null;
                spnLIFESPAN_MONTH_FROM.EditValue = null;
                spnLIFESPAN_MONTH_TO.EditValue = null;
                spnEXPIRED_DAY_RATIO.EditValue = null;
                chkThuoc.Checked = true;
                chkVatTu.Checked = false;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void SetCaptionByLanguagekey()
        {
            try
            {

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExpiredDateCFG.Resources.Lang", typeof(HIS.Desktop.Plugins.HisExpiredDateCFG.frmHisExpiredDateCFG).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt
                 this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
              
                 this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisLanguage.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisLanguage.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisLanguage.btnAdd.Text",
Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisLanguage.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
              

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void frmHisExpiredDateCFG_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                EnableControlChanged(this.ActionType);
                FillDataToGrid();
                SetCaptionByLanguagekey();
                ValidateForm();
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
                WaitingManager.Show();


                int pageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
            //txtLANGUAGECODE.ReadOnly = !(action == GlobalVariables.ActionAdd);
        }
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_EXPIRED_DATE_CFG>> apiResult = null;
                HisExpiredDateCfgFilter filter = new HisExpiredDateCfgFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_EXPIRED_DATE_CFG>>("api/HisExpiredDateCfg/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<HIS_EXPIRED_DATE_CFG>)apiResult.Data;
                    if (data != null)
                    {

                        gridView1.GridControl.DataSource = data;
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

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_EXPIRED_DATE_CFG pData = (MOS.EFMODEL.DataModels.HIS_EXPIRED_DATE_CFG)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IS_MADICINE_STR")
                    {
                        e.Value = pData.IS_MATERIAL == null ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_MATERIAL_STR")
                    {
                        e.Value = pData.IS_MATERIAL == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {

                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }

                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_EXPIRED_DATE_CFG data = (HIS_EXPIRED_DATE_CFG)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? repositoryItemButtonEditLOCK : repositoryItemButtonEditUNLOCK);

                    }

                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemButtonEditENADELETE : repositoryItemButtonEditDISDELETE);

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_EXPIRED_DATE_CFG)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ChangedDataRow(HIS_EXPIRED_DATE_CFG data)
        {
            try
            {
                if (data != null)
                {

                    spnEXPIRED_DAY.EditValue = currentData.EXPIRED_DAY;
                    spnLIFESPAN_MONTH_TO.EditValue = currentData.LIFESPAN_MONTH_TO;
                    spnLIFESPAN_MONTH_FROM.EditValue = currentData.LIFESPAN_MONTH_FROM;
                    spnEXPIRED_DAY_RATIO.EditValue = currentData.EXPIRED_DAY_RATIO;
                    if (currentData.IS_MATERIAL == 1)
                        chkVatTu.Checked = true;
                    else
                        chkThuoc.Checked = true;
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditLOCK_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_EXPIRED_DATE_CFG success = new HIS_EXPIRED_DATE_CFG();
            bool notHandler = false;
            try
            {

                HIS_EXPIRED_DATE_CFG data = (HIS_EXPIRED_DATE_CFG)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXPIRED_DATE_CFG>("api/HisExpiredDateCfg/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGrid();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditUNLOCK_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_EXPIRED_DATE_CFG success = new HIS_EXPIRED_DATE_CFG();
            bool notHandler = false;
            try
            {

                HIS_EXPIRED_DATE_CFG data = (HIS_EXPIRED_DATE_CFG)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXPIRED_DATE_CFG>("api/HisExpiredDateCfg/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGrid();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditENADELETE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit.Enabled = false;
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_EXPIRED_DATE_CFG)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {


                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>("api/HisExpiredDateCfg/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGrid();
                            currentData = ((List<HIS_EXPIRED_DATE_CFG>)gridControl1.DataSource).FirstOrDefault();


                        }

                        MessageManager.Show(this, param, success);
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
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SaveProcess()
        {
            CommonParam param = new CommonParam();

            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (spnEXPIRED_DAY.EditValue == null && spnEXPIRED_DAY_RATIO.EditValue == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                "Chưa nhập hạn sử dụng cảnh báo",
                "Thông báo");
                    return;
                }
                if (spnEXPIRED_DAY.EditValue != null && spnEXPIRED_DAY_RATIO.EditValue != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                "Chỉ cho phép nhập một trong hai trường \"Hạn sử dụng cảnh báo theo ngày\" và \"Hạn sử dụng cảnh báo theo tuổi thọ\"",
                "Thông báo");
                    return;
                }
                if (spnLIFESPAN_MONTH_FROM.EditValue != null && spnLIFESPAN_MONTH_TO.EditValue != null)
                {
                    if (Int32.Parse(spnLIFESPAN_MONTH_FROM.EditValue.ToString()) >= Int32.Parse(spnLIFESPAN_MONTH_TO.EditValue.ToString()))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                                        "Tuổi thọ từ đang lớn hơn hoặc bằng tuổi thọ đến",
                                        "Thông báo");
                        return;
                    }
                }

                WaitingManager.Show();
                HIS_EXPIRED_DATE_CFG updateDTO = new HIS_EXPIRED_DATE_CFG();


                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                if (spnEXPIRED_DAY.EditValue!=null )
                updateDTO.EXPIRED_DAY = Inventec.Common.TypeConvert.Parse.ToInt64(spnEXPIRED_DAY.Value.ToString()) ;
                else
                    updateDTO.EXPIRED_DAY = null;
                if (spnEXPIRED_DAY_RATIO.EditValue != null)
                    updateDTO.EXPIRED_DAY_RATIO = Inventec.Common.Number.Get.RoundCurrency(spnEXPIRED_DAY_RATIO.Value, 2);
                else
                    updateDTO.EXPIRED_DAY_RATIO = null;
                if (spnLIFESPAN_MONTH_FROM.EditValue != null)
                    updateDTO.LIFESPAN_MONTH_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(spnLIFESPAN_MONTH_FROM.Value.ToString());
                else
                    updateDTO.LIFESPAN_MONTH_FROM = null;
                if (spnLIFESPAN_MONTH_TO.EditValue != null )
                    updateDTO.LIFESPAN_MONTH_TO = Inventec.Common.TypeConvert.Parse.ToInt64(spnLIFESPAN_MONTH_TO.Value.ToString());
                else
                    updateDTO.LIFESPAN_MONTH_TO = null;
           
                if (chkThuoc.Checked)
                    updateDTO.IS_MATERIAL = null;
                else
                    updateDTO.IS_MATERIAL = 1;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<HIS_EXPIRED_DATE_CFG>("api/HisExpiredDateCfg/Create", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGrid();
                        SetDefaultValue();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<HIS_EXPIRED_DATE_CFG>("api/HisExpiredDateCfg/Update", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;

                        FillDataToGrid();
                    }
                }



                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadCurrent(long currentId, ref HIS_EXPIRED_DATE_CFG currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpiredDateCfgFilter filter = new HisExpiredDateCfgFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<HIS_EXPIRED_DATE_CFG>>("api/HisExpiredDateCfg/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
      

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                btnEdit_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                btnAdd_Click(null, null);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnReset_Click(null, null);
        }

        private void chkThuoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkThuoc.Checked)
                    chkVatTu.Checked = false;
            }
            catch (Exception ex)
            {
                 Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkVatTu_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkVatTu.Checked)
                    chkThuoc.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                validControl(spnEXPIRED_DAY);
                validControl(spnEXPIRED_DAY_RATIO);
                validControl(spnLIFESPAN_MONTH_FROM);
                validControl(spnLIFESPAN_MONTH_TO);
            }
            catch (Exception ex)
            {
                
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void validControl(DevExpress.XtraEditors.SpinEdit spn)
        {
           ValidateNumber val = new ValidateNumber();
           val.spn = spn;
           dxValidationProvider1.SetValidationRule(spn, val);
        }

        private void spnLIFESPAN_MONTH_FROM_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                 if (e.KeyCode == Keys.Enter)
                {
                    spnLIFESPAN_MONTH_TO.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnLIFESPAN_MONTH_TO_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnEXPIRED_DAY.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnEXPIRED_DAY_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnEXPIRED_DAY_RATIO.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnEXPIRED_DAY_RATIO_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlPatientInfo == -1)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlPatientInfo > edit.TabIndex)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
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

    }
}
