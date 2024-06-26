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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisExamServiceTypeList.Entity;
using HIS.Desktop.Plugins.HisExamServiceTypeList.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisExamServiceTypeList
{
    public partial class frmHisExamServiceTypeList : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module Module;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        internal long serviceId;
        internal long serviceTypeId;
        V_HIS_EXAM_SERVICE_TYPE currentData;

        public frmHisExamServiceTypeList(Inventec.Desktop.Common.Modules.Module _module)
		:base(_module)
        {
            InitializeComponent();
            this.Module = _module;
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

        private void frmHisExamServiceTypeList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //FillDataToGridControl();
                SetCaptionByLanguageKey();
                LoaddataToTreeList(this);
                initComboDVT();
                //initComboDichVuBHYT();
                InitComboCha();
                initComboLoaiDVBH();
                initComboBill();
                initComboLoaiBH();
                InitComboLoaiDTTT();
                ValidateForm();
                spGia.EditValue = null;
                spTime.EditValue = null;
                spSTT.EditValue = null;
                cboBill.EditValue = 1;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisExamServiceTypeViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExamServiceTypeList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisExamServiceTypeList.frmHisExamServiceTypeList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcRight.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.lcRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkOutPack.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.chkOutPack.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem5.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.barButtonItem5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem6.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.barButtonItem6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem7.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.barButtonItem7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem8.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.barButtonItem8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.barButtonItem4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBill.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.cboBill.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLDvBH.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.cboLDvBH.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCha.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.cboCha.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefreh.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.btnRefreh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCheck.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.chkCheck.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDVT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.cboDVT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkCode.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.lkCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkName.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.lkName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkDvi.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.lkDvi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkGiaVon.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.lkGiaVon.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkTime.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.lkTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkSTT.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.lkSTT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkBill.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.lkBill.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkPatientType.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.lkPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLoaiBH.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.cboLoaiBH.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.treeListColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTypeList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExam_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    //var rowData = (V_HIS_EXAM_SERVICE_TYPE)treeList1.GetFocusedRow();
            //    if (rowData != null)
            //    {
            //        FillDataToEditorControl(rowData);
            //        this.ActionType = GlobalVariables.ActionEdit;
            //        EnableControlChanged(this.ActionType);

            //        //Disable nút sửa nếu dữ liệu đã bị khóa
            //        btnEdit.Enabled = (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
            //        //if (this.currentData != null)
            //        //{
            //            //btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

            //        //}


            //        positionHandle = -1;
            //        Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }
        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_EXAM_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    serviceId = data.SERVICE_ID;
                    serviceTypeId = data.ID;
                    txtCode.Text = data.EXAM_SERVICE_TYPE_CODE;
                    txtName.Text = data.EXAM_SERVICE_TYPE_NAME;
                    txtMaCK.Text = data.SPECIALITY_CODE;
                    cboDVT.EditValue = data.SERVICE_UNIT_ID;
                    cboCha.EditValue = data.PARENT_ID;
                    cboPatientType.EditValue = data.BILL_PATIENT_TYPE_ID;
                    cboLDvBH.EditValue = data.HEIN_SERVICE_BHYT_ID;
                    cboLoaiBH.EditValue = data.HEIN_SERVICE_TYPE_ID;
                    if (data.BILL_OPTION == null)
                    {
                        cboBill.EditValue = 1;
                    }
                    else if (data.BILL_OPTION == 1)
                    {
                        cboBill.EditValue = 2;
                    }
                    else if (data.BILL_OPTION == 2)
                    {
                        cboBill.EditValue = 3;
                    }
                    spGia.EditValue = data.COGS;
                    spTime.EditValue = data.ESTIMATE_DURATION;
                    chkCheck.Checked = (data.IS_MULTI_REQUEST == 1 ? true : false);
                    chkOutPack.Checked = (data.IS_OUT_PARENT_FEE == 1 ? true : false);
                    spTime.EditValue = data.NUM_ORDER;
                }
                else
                {
                    serviceId = 0;
                    serviceTypeId = 0;
                }
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
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoaddataToTreeList(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void initComboDVT()
        {
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 150, 1));
            columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 250);
            ControlEditorLoader.Load(cboDVT, BackendDataWorker.Get<HIS_SERVICE_UNIT>(), controlEditorADO);
        }
        private void InitComboCha()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExamServiceTypeViewFilter filter = new HisExamServiceTypeViewFilter();
                filter.IS_ACTIVE = 1;
                var serviceTypeParents = new BackendAdapter(param)
                    .Get<List<V_HIS_EXAM_SERVICE_TYPE>>("api/HisExamServiceType/GetView", ApiConsumers.MosConsumer, filter, param);
                //var serviceTypeParents = BackendDataWorker.Get<V_HIS_DIIM_SERVICE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXAM_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXAM_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXAM_SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCha, serviceTypeParents, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //private void initComboDichVuBHYT()
        //{
        //    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //    columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_CODE", "", 150, 1));
        //    columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_NAME", "", 250, 2));
        //    ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_BHYT_NAME", "ID", columnInfos, false, 250);
        //    ControlEditorLoader.Load(cboDichVuBHYT, BackendDataWorker.Get<HIS_HEIN_SERVICE_BHYT>(), controlEditorADO);
        //}
        private void initComboLoaiDVBH()
        {
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_CODE", "", 150, 1));
            columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_BHYT_NAME", "ID", columnInfos, false, 250);
            ControlEditorLoader.Load(cboLDvBH, BackendDataWorker.Get<HIS_HEIN_SERVICE_BHYT>(), controlEditorADO);
        }
        private void initComboLoaiBH()
        {
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_CODE", "", 150, 1));
            columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_TYPE_NAME", "ID", columnInfos, false, 250);
            ControlEditorLoader.Load(cboLoaiBH, BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(), controlEditorADO);
        }
        private void InitComboLoaiDTTT()
        {
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 150, 1));
            columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
            ControlEditorLoader.Load(cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), controlEditorADO);
        }
        private void initComboBill()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Hóa đơn thường"));
                status.Add(new Status(2, "Tách chênh lệch vào hóa đơn dịch vụ"));
                status.Add(new Status(3, "Hóa đơn dịch vụ"));

                List<Inventec.Common.Controls.EditorLoader.ColumnInfo> columnInfos = new List<Inventec.Common.Controls.EditorLoader.ColumnInfo>();
                columnInfos.Add(new Inventec.Common.Controls.EditorLoader.ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBill, status, controlEditorADO);
                cboBill.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
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
        private void ResetFormData()
        {
            try
            {
                if (!lcRight.IsInitialized) return;
                lcRight.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcRight.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcRight.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtCode);
                ValidationSingleControl(txtName);
                ValidationSingleControl(cboDVT);
                //ValidationSingleControl(cboPatientType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = String.Format(ResourceMessage.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
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
                validRule.ErrorText = String.Format(ResourceMessage.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format(ResourceMessage.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnRefreh_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                txtCode.Focus();
                cboBill.EditValue = 1;
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

        private void txtCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtName.Focus();
                    txtName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaCK.Focus();
                    txtMaCK.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDVT_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDVT.EditValue == null)
                    {
                        cboDVT.ShowPopup();
                    }
                    else
                    {
                        if (cboPatientType.EditValue == null)
                        {
                            cboPatientType.Focus();
                            cboPatientType.ShowPopup();
                        }
                        else
                        {
                            cboPatientType.Focus();
                        }
                    }
                    //if (cboDVT.EditValue != null && cboDVT.EditValue != cboDVT.OldEditValue)
                    //{
                    //    MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDVT.EditValue.ToString()));
                    //    if (gt != null)
                    //    {
                    //        cboDichVuBHYT.Focus();
                    //        cboDichVuBHYT.ShowPopup();
                    //    }
                    //}
                    //else
                    //{
                    //    cboDVT.ShowPopup();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDVT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cboDVT.EditValue == null)
                {
                    cboDVT.Focus();
                    cboDVT.ShowPopup();
                }
                else
                {
                    if (cboPatientType.EditValue == null)
                    {
                        cboPatientType.Focus();
                        cboPatientType.ShowPopup();
                    }
                    else
                    {
                        cboPatientType.Focus();
                    }
                }
            }
        }

        //private void cboLoaiDV_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.CloseMode == PopupCloseMode.Normal)
        //        {
        //            cboDichVuBHYT.Focus();
        //            cboDichVuBHYT.ShowPopup();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void cboLoaiDV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboDichVuBHYT_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboLDvBH.Focus();
                    cboLDvBH.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spGia_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCheck.Properties.FullFocusRect = true;
                    chkCheck.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spSTT.Focus();
                    spSTT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCheck_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkOutPack.Focus();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spSTT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled == true)
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

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefreh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void cboDVT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //private void cboDichVuBHYT_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            cboDichVuBHYT.ShowPopup();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtMaCK_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDVT.Focus();
                    cboDVT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoaddataToTreeList(frmHisExamServiceTypeList control)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExamServiceTypeViewFilter filter = new MOS.Filter.HisExamServiceTypeViewFilter();
                filter.KEY_WORD = txtKeyword.Text;
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_EXAM_SERVICE_TYPE>>("api/HisExamServiceType/GetView", ApiConsumers.MosConsumer, filter, param).ToList();
                if (currentDataStore != null)
                {

                    treeList1.KeyFieldName = "ID";
                    treeList1.ParentFieldName = "PARENT_ID";
                    treeList1.DataSource = currentDataStore;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_Click(object sender, EventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_EXAM_SERVICE_TYPE rowData = data as V_HIS_EXAM_SERVICE_TYPE;
                if (rowData != null)
                {

                    ChangedDataRow(rowData);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_EXAM_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void treeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_EXAM_SERVICE_TYPE)
                {
                    V_HIS_EXAM_SERVICE_TYPE rowData = data as V_HIS_EXAM_SERVICE_TYPE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (rowData.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnlock : btnLock);

                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IS_ACTIVE_TRUE)
                                e.RepositoryItem = btnDelete;
                            else
                                e.RepositoryItem = btnDelete_Disable;
                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    V_HIS_EXAM_SERVICE_TYPE pData = e.Row as V_HIS_EXAM_SERVICE_TYPE;
                    if (pData == null || this.treeList1 == null) return;
                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.CREATE_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.MODIFY_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_MULTI_REQUEST_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_MULTI_REQUEST == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi cho phep nhap chi dinh lon hon 1 IS_MULTI_REQUEST", ex);
                        }
                    }

                    else if (e.Column.FieldName == "CPNG")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_OUT_PARENT_FEE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BILL_OTPION_STR")
                    {
                        try
                        {
                            e.Value = pData.BILL_OPTION;
                            if (pData.BILL_OPTION == null)
                            {
                                e.Value = "Hóa đơn thường";
                            }
                            else if (pData.BILL_OPTION == 1)
                            {
                                e.Value = "Tách tiền chênh lệch vào hóa đơn dịch vụ";
                            }
                            else if (pData.BILL_OPTION == 2)
                            {
                                e.Value = "Hóa đơn dịch vụ";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("BILL_OTPION_STR", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_EXAM_SERVICE_TYPE hisMediStock = new V_HIS_EXAM_SERVICE_TYPE();
            bool notHandler = false;
            try
            {
                WaitingManager.Show();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_EXAM_SERVICE_TYPE dataMediStock = data as V_HIS_EXAM_SERVICE_TYPE;
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonKhoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_EXAM_SERVICE_TYPE data1 = new V_HIS_EXAM_SERVICE_TYPE();
                    data1.ID = dataMediStock.ID;
                    WaitingManager.Show();
                    hisMediStock = new BackendAdapter(param).Post<V_HIS_EXAM_SERVICE_TYPE>("api/HisExamServiceType/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisMediStock != null)
                    {
                        LoaddataToTreeList(this);
                        notHandler = true;
                    }

                    btnEdit.Enabled = false;
                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_EXAM_SERVICE_TYPE hisMediStock = new V_HIS_EXAM_SERVICE_TYPE();
            bool notHandler = false;
            try
            {
                WaitingManager.Show();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_EXAM_SERVICE_TYPE dataMediStock = data as V_HIS_EXAM_SERVICE_TYPE;
                if (MessageBox.Show(String.Format(ResourceMessage.MoKhoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_EXAM_SERVICE_TYPE data1 = new V_HIS_EXAM_SERVICE_TYPE();
                    data1.ID = dataMediStock.ID;
                    WaitingManager.Show();
                    hisMediStock = new BackendAdapter(param).Post<V_HIS_EXAM_SERVICE_TYPE>("api/HisExamServiceType/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisMediStock != null)
                    {
                        LoaddataToTreeList(this);
                        notHandler = true;
                    }
                    btnEdit.Enabled = false;
                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_EXAM_SERVICE_TYPE rowData = data as V_HIS_EXAM_SERVICE_TYPE;
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonXoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisExamServiceType/Delete", ApiConsumers.MosConsumer, rowData, param);
                        if (success)
                        {
                            LoaddataToTreeList(this);
                        }
                        MessageManager.Show(this, param, success);
                    }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCha_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCha.Text == txtName.Text)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không được chọn chính nó làm cha.Vui lòng chọn lại", "Thông báo");
                        cboCha.ShowPopup();
                        return;
                    }
                    cboBill.SelectAll();
                    cboBill.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void cboDichVuBHYT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        if (cboDichVuBHYT.EditValue != null)
        //        {
        //            var a = BackendDataWorker.Get<HIS_HEIN_SERVICE_BHYT>();
        //            var dvBHYT = a.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDichVuBHYT.EditValue.ToString()));
        //            if (dvBHYT == null)
        //            {
        //                cboDichVuBHYT.EditValue = null;
        //                cboDichVuBHYT.Focus();
        //                cboDichVuBHYT.ShowPopup();
        //                return;
        //            }
        //            if (cboDichVuBHYT.EditValue == "")
        //            {
        //                cboDichVuBHYT.EditValue = null;
        //                cboLDvBH.Focus();
        //                cboLDvBH.ShowPopup();
        //            }

        //            else
        //            {
        //                cboLDvBH.Focus();
        //                cboLDvBH.ShowPopup();
        //            }
        //        }
        //        else
        //        {
        //            cboDichVuBHYT.ShowPopup();
        //        }
        //    }
        //}

        private void cboCha_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cboCha.EditValue != null)
                {
                    if (cboCha.EditValue == "")
                    {
                        cboCha.EditValue = null;
                        cboCha.ShowPopup();
                        //spGia.Focus();
                        //spGia.SelectAll();
                    }
                    else
                    {
                        cboBill.SelectAll();
                        cboBill.ShowPopup();
                    }
                }
                else
                {
                    cboCha.ShowPopup();
                }
            }
        }

        private void cboDichVuBHYT_KeyUp_1(object sender, KeyEventArgs e)
        {

        }

        private void cboLDvBH_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboCha.Focus();
                    cboCha.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLDvBH_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cboLDvBH.EditValue != null)
                {
                    if (cboLDvBH.EditValue == "")
                    {
                        cboLDvBH.EditValue = null;
                        cboLDvBH.ShowPopup();
                        //spGia.Focus();
                        //spGia.SelectAll();
                    }
                    else
                    {
                        cboCha.Focus();
                        cboCha.SelectAll();
                    }
                }
                else
                {
                    cboCha.Focus();
                    cboCha.ShowPopup();
                }
            }
        }

        private void cboBill_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    spGia.Focus();
                    spGia.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBill_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBill.EditValue != null)
                    {
                        if (cboBill.EditValue == "")
                        {
                            cboBill.EditValue = null;
                            cboBill.ShowPopup();
                            //spGia.Focus();
                            //spGia.SelectAll();
                        }
                        else
                        {
                            spGia.Focus();
                            spGia.SelectAll();
                        }
                    }
                    else
                    {
                        cboBill.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLoaiBH.EditValue == null)
                    {
                        cboLoaiBH.Focus();
                        cboLoaiBH.ShowPopup();
                    }
                    else
                    {
                        cboLoaiBH.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPatientType.EditValue == null)
                    {
                        cboPatientType.Focus();
                        cboPatientType.ShowPopup();
                    }
                    else
                    {
                        cboLoaiBH.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboPatientType.Properties.Buttons[1].Visible = true;
                    cboPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLDvBH_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboLDvBH.Properties.Buttons[1].Visible = true;
                    cboLDvBH.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCha_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboCha.Properties.Buttons[1].Visible = true;
                    cboCha.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBill_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboBill.Properties.Buttons[1].Visible = true;
                    cboBill.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkOutPack_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spTime.Focus();
                    spTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiBH_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboLoaiBH.Properties.Buttons[1].Visible = true;
                    cboLoaiBH.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiBH_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLDvBH.EditValue == null)
                    {
                        cboLDvBH.Focus();
                        cboLDvBH.ShowPopup();
                    }
                    else
                    {
                        cboLDvBH.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiBH_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiBH.EditValue == null)
                    {
                        cboLoaiBH.Focus();
                        cboLoaiBH.ShowPopup();
                    }
                    else
                    {
                        cboLDvBH.Focus();
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
