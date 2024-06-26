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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MedicalStoreV2.ADO;
using HIS.Desktop.Plugins.MedicalStoreV2.Config;
using HIS.Desktop.Plugins.MedicalStoreV2.Popup;
using HIS.Desktop.Plugins.MedicalStoreV2.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.MedicalStoreV2.ChooseStore
{
    public partial class frmSaveStore : Form
    {

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        internal List<TreatmentADO> currentTreatment { get; set; }
        HIS.Desktop.Common.RefeshReference refeshData;
        //internal List<L_HIS_TREATMENT_3> lstTreatment { get; set; }
        Inventec.Desktop.Common.Modules.Module currentModule;
        int positionHandle = -1;
        List<HIS_DATA_STORE> dataStores = null;
        bool isInit = true;
        private List<HIS_LOCATION_STORE> lstLocationStore;

        public frmSaveStore(Inventec.Desktop.Common.Modules.Module currentModule, List<TreatmentADO> currentMediRecord, RefeshReference refeshData, List<HIS.Desktop.Library.CacheClient.ControlStateRDO> controlStateRDO,
        HIS.Desktop.Library.CacheClient.ControlStateWorker ctrlStateWorker)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentTreatment = currentMediRecord;
                this.refeshData = refeshData;
                this.controlStateWorker = ctrlStateWorker;
                this.currentControlStateRDO = controlStateRDO;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmSaveStore(Inventec.Desktop.Common.Modules.Module currentModule, List<L_HIS_TREATMENT_3> lstTreatments, RefeshReference refeshData)
        {
            //InitializeComponent();
            //try
            //{
            //    //this.lstTreatment = lstTreatments;
            //    this.currentModule = currentModule;
            //    this.refeshData = refeshData;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDataStore_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                ValidateForm();
                LoadComboLocationStore();
                InitControlState();
                SetDefaultData();
                FocusButtonSave();
                Inventec.Common.Logging.LogSystem.Info("currentTreatment: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTreatment), currentTreatment));

                if (this.currentTreatment != null && this.currentTreatment.Count > 1)
                {
                    chkAutoSetStoreCode.Checked = false;
                    chkAutoSetStoreCode.Enabled = false;

                }

                var check = this.currentTreatment.FirstOrDefault();
                if (!string.IsNullOrEmpty(check.RECORD_INSPECTION_REJECT_NOTE) || !string.IsNullOrEmpty(check.REJECT_STORE_REASON))
                {
                    Inventec.Common.Logging.LogSystem.Warn("Visible true");
                }
                else
                {
                    linkLabel1.Text = "";
                    Inventec.Common.Logging.LogSystem.Warn("Visible false");
                }

                isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboLocationStore()
        {

            try
            {
                lstLocationStore = BackendDataWorker.Get<HIS_LOCATION_STORE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOCATION_STORE_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("LOCATION_STORE_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("LOCATION_STORE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboLoacationStore, lstLocationStore, controlEditorADO);
                cboLoacationStore.Properties.ImmediatePopup = true;
                cboLoacationStore.Properties.PopupFormSize = new Size(350, cboLoacationStore.Properties.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InitControlState()
        {
            try
            {
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_FILTER_DATA_STORE_BY_END_INFO)
                        {
                            checkFilterDataStoreByEndInfo.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_FILTER_DATA_STORE_BY_TREATMENT_END_TYPE)
                        {
                            checkFilterDataStoreByTreatmentEndType.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkAutoSetStoreCode.Name)
                        {
                            chkAutoSetStoreCode.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkIsTreatmentType.Name)
                        {
                            chkIsTreatmentType.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkUseEndCode.Name)
                        {
                            chkUseEndCode.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultData()
        {
            try
            {
                var programs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PROGRAM>().Where(o => o.IS_ACTIVE == 1).ToList();
                LoadDataTocboProram(programs);
                dtSaveTime.DateTime = DateTime.Now;

                GetDataStore();
                LoadDataTocboDataStore(dataStores);
                SetDefaultDataStore();
                LoadDataTocboMediRecordType(BackendDataWorker.Get<HIS_MEDI_RECORD_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList());
                SetDefaultMediRecordType();
                lciProgram.Visibility = chkBANgoaiTru.Checked ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                linkLabel1.Text = "Trước đó, hồ sơ đã bị từ chối duyệt. Bạn có thể click vào đây để xem nội dung từ chối trước khi lưu trữ bệnh án";
                linkLabel1.LinkArea = new LinkArea(58, 3);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataStore()
        {
            try
            {
                string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var userRooms = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == loginname && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                if (userRooms != null && userRooms.Count > 0)
                {
                    List<long> roomIds = userRooms.Select(o => o.ROOM_ID).Distinct().ToList();
                    dataStores = BackendDataWorker.Get<HIS_DATA_STORE>().Where(o => roomIds.Contains(o.ROOM_ID) && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                if (checkFilterDataStoreByEndInfo.Checked && dataStores != null && dataStores.Count > 0)
                {
                    List<long> departmentIds = this.currentTreatment.Where(o => o.END_DEPARTMENT_ID.HasValue).Select(s => s.END_DEPARTMENT_ID.Value).Distinct().ToList();
                    List<long> roomIds = this.currentTreatment.Where(o => o.END_ROOM_ID.HasValue).Select(s => s.END_ROOM_ID.Value).Distinct().ToList();
                    dataStores = dataStores.Where(o =>
                        (!o.STORED_DEPARTMENT_ID.HasValue || !departmentIds.Any(a => a != o.STORED_DEPARTMENT_ID.Value))
                        && (!o.STORED_ROOM_ID.HasValue || !roomIds.Any(a => a != o.STORED_ROOM_ID.Value))
                        ).ToList();
                }

                if (checkFilterDataStoreByTreatmentEndType.Checked && dataStores != null && dataStores.Count > 0)
                {
                    if (this.currentTreatment.Exists(e => !e.TREATMENT_END_TYPE_ID.HasValue))
                    {
                        dataStores = new List<HIS_DATA_STORE>();
                    }
                    else
                    {
                        List<long> TreatmentEndTypeIds = this.currentTreatment.Select(s => s.TREATMENT_END_TYPE_ID.Value).Distinct().ToList();
                        dataStores = dataStores.Where(o => String.IsNullOrWhiteSpace(o.TREATMENT_END_TYPE_IDS) || !TreatmentEndTypeIds.Any(a => !this.CheckContainsTreatmentEndType(o.TREATMENT_END_TYPE_IDS, a))).ToList();
                    }
                }
                if (chkIsTreatmentType.Checked && dataStores != null && dataStores.Count > 0)
                {
                    List<string> treatmentTypeIds = this.currentTreatment.Where(o => o.TDL_TREATMENT_TYPE_ID.HasValue).Select(s => s.TDL_TREATMENT_TYPE_ID.Value.ToString()).Distinct().ToList();

                    dataStores = dataStores.Where(o =>
                        (string.IsNullOrEmpty(o.TREATMENT_TYPE_IDS)) || o.TREATMENT_TYPE_IDS.Split(',').ToList().Exists(p => treatmentTypeIds.Contains(p))).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckContainsTreatmentEndType(string treatmentEndTypeIds, long treatmentEndTypeId)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(treatmentEndTypeIds) && String.Format(",{0},", treatmentEndTypeIds).Contains(String.Format(",{0},", treatmentEndTypeId)))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private void SetDefaultDataStore()
        {
            try
            {
                if (dataStores != null && dataStores.Count == 1)
                {
                    cboDataStore.EditValue = dataStores[0].ID;
                    return;
                }
                if (dataStores != null && cboDataStore.EditValue != null && dataStores.Any(a => a.ID == Convert.ToInt64(cboDataStore.EditValue)))
                {
                    return;
                }
                cboDataStore.EditValue = null;
                if (GlobalVariables.ListDataStoreUseTime != null && (GlobalVariables.ListDataStoreUseTime is List<LocalStoreADO>) && dataStores != null)
                {
                    List<LocalStoreADO> ados = ((List<LocalStoreADO>)GlobalVariables.ListDataStoreUseTime).OrderByDescending(o => o.LastTime).ToList();
                    LocalStoreADO ado = ados.FirstOrDefault(o => dataStores.Any(a => a.ID == o.DataStoreId));
                    if (ado != null)
                    {
                        cboDataStore.EditValue = ado.DataStoreId;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultMediRecordType()
        {
            try
            {
                List<long> mediRecordTypeIds = this.currentTreatment.Where(o => o.MEDI_RECORD_TYPE_ID.HasValue).Select(s => s.MEDI_RECORD_TYPE_ID.Value).Distinct().ToList();
                List<long> treatmentEndTypeIds = this.currentTreatment.Select(s => (s.TREATMENT_END_TYPE_ID ?? 0)).Distinct().ToList();
                if (mediRecordTypeIds != null && mediRecordTypeIds.Count == 1)
                {
                    cboMediRecordType.EditValue = mediRecordTypeIds[0];
                    return;
                }
                else if (treatmentEndTypeIds != null && treatmentEndTypeIds.Count == 1 && treatmentEndTypeIds[0] == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                {
                    cboMediRecordType.EditValue = IMSys.DbConfig.HIS_RS.HIS_MEDI_RECORD_TYPE.ID__TuVong;
                    return;
                }
                else
                {
                    cboMediRecordType.EditValue = IMSys.DbConfig.HIS_RS.HIS_MEDI_RECORD_TYPE.ID__Khac;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidateGridLookupWithTextEdit(cboDataStore, txtDataStoreCode);
                ValidateThoiGian();
                ValidateStoreCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateStoreCode()
        {
            try
            {
                ControlMaxLengthValidationRule rule = new ControlMaxLengthValidationRule();
                rule.editor = txtStoreCode;
                rule.IsRequired = false;
                rule.maxLength = 20;
                rule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtStoreCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateThoiGian()
        {
            try
            {
                DateTimeValidationRule validRule = new DateTimeValidationRule();
                validRule.dtTime = dtSaveTime;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtSaveTime, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataTocboProram(List<HIS_PROGRAM> ProgramADOList)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PROGRAM_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PROGRAM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PROGRAM_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboProgram, ProgramADOList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboMediRecordType(List<HIS_MEDI_RECORD_TYPE> ProgramADOList)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_RECORD_TYPE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MEDI_RECORD_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_RECORD_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboMediRecordType, ProgramADOList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboDataStore(List<HIS_DATA_STORE> ProgramADOList)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DATA_STORE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DATA_STORE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DATA_STORE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboDataStore, ProgramADOList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource_frmSaveStore = new ResourceManager("HIS.Desktop.Plugins.MedicalStoreV2.Resources.Lang", typeof(frmSaveStore).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.linkLabel1.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.linkLabel1.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.chkAutoSetStoreCode.Properties.Caption = Inventec.Common.Resource.Get.Value("frmSaveStore.chkAutoSetStoreCode.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.bar1.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.barButtonItem_Save.Caption = Inventec.Common.Resource.Get.Value("frmSaveStore.barButtonItem_Save.Caption", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.barButtonItem__Print.Caption = Inventec.Common.Resource.Get.Value("frmSaveStore.barButtonItem__Print.Caption", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.checkFilterDataStoreByTreatmentEndType.Properties.Caption = Inventec.Common.Resource.Get.Value("frmSaveStore.checkFilterDataStoreByTreatmentEndType.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.checkFilterDataStoreByEndInfo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmSaveStore.checkFilterDataStoreByEndInfo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.cboMediRecordType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSaveStore.cboMediRecordType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.cboProgram.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSaveStore.cboProgram.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.chkBANgoaiTru.Properties.Caption = Inventec.Common.Resource.Get.Value("frmSaveStore.chkBANgoaiTru.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.cboDataStore.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSaveStore.cboDataStore.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.btnSaveData.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.btnSaveData.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmSaveStore.layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.lciProgram.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.lciProgram.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.lciFilterDataStoreByEndInfo.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.lciFilterDataStoreByEndInfo.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.lciShowDataStoreByEndTreatmentType.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.lciShowDataStoreByEndTreatmentType.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.lciStoreCode.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.lciStoreCode.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.chkIsTreatmentType.Properties.Caption = Inventec.Common.Resource.Get.Value("frmSaveStore.chkIsTreatmentType.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSaveStore.Text", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.chkUseEndCode.Properties.Caption = Inventec.Common.Resource.Get.Value("frmSaveStore.chkUseEndCode.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
                this.chkUseEndCode.ToolTip = Inventec.Common.Resource.Get.Value("frmSaveStore.chkUseEndCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource_frmSaveStore, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusButtonSave()
        {
            try
            {
                if (cboMediRecordType.EditValue != null && cboDataStore.EditValue != null)
                {
                    this.ActiveControl = btnSaveData;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                CommonParam param = new CommonParam();

                if (!dxValidationProvider1.Validate())
                    return;

                HisTreatmentStoreSDO treatmentStoreSDO = new HisTreatmentStoreSDO();
                if (cboDataStore.EditValue != null)
                {
                    treatmentStoreSDO.DataStoreId = Convert.ToInt32(cboDataStore.EditValue.ToString());
                }

                if (cboMediRecordType.EditValue != null)
                {
                    treatmentStoreSDO.MediRecordTypeId = Convert.ToInt32(cboMediRecordType.EditValue.ToString());
                }

                treatmentStoreSDO.IsOutPatient = chkBANgoaiTru.CheckState == CheckState.Checked;

                if (chkBANgoaiTru.CheckState == CheckState.Checked && cboProgram.EditValue != null)
                {
                    treatmentStoreSDO.ProgramId = Convert.ToInt32(cboProgram.EditValue.ToString());
                }
                if (dtSaveTime != null && dtSaveTime.DateTime != DateTime.MinValue)
                {
                    treatmentStoreSDO.StoreTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtSaveTime.DateTime.ToString("yyyyMMddHHmm") + "59");
                }

                if (txtStoreCode.Enabled && !String.IsNullOrEmpty(txtStoreCode.Text))
                {
                    treatmentStoreSDO.StoreCode = txtStoreCode.Text.Trim();
                }
                if (chkUseEndCode.CheckState == CheckState.Checked)
                {
                    treatmentStoreSDO.IsUseEndCode = true;
                }
                if (cboLoacationStore.EditValue != null)
                    treatmentStoreSDO.LocationStoreId = Int64.Parse(cboLoacationStore.EditValue.ToString());
                bool result = false;
                string MessageSuccess = "";
                List<HIS_TREATMENT> treatmentResults = null;
                if (currentTreatment != null && currentTreatment.Count > 0)
                {
                    treatmentStoreSDO.TreatmentIds = new List<long>();
                    treatmentStoreSDO.TreatmentIds = currentTreatment.Select(o => o.ID).Distinct().ToList();

                    treatmentResults = new BackendAdapter(param).Post<List<HIS_TREATMENT>>("api/HisTreatment/Store", ApiConsumers.MosConsumer, treatmentStoreSDO, param);
                    if (treatmentResults != null && treatmentResults.Count > 0)
                    {
                        result = true;
                        foreach (var item in treatmentResults)
                        {
                            MessageSuccess += item.STORE_CODE + ", ";
                        }

                        if (treatmentResults.Count == 1)
                        {
                            txtStoreCode.Text = treatmentResults.First().STORE_CODE;
                        }

                        this.refeshData();
                        this.SetLastTimeToLocal(treatmentStoreSDO.DataStoreId);
                    }
                }

                #region Show message
                if (treatmentResults != null && treatmentResults.Count > 0)
                {
                    MessageManager.Show(ResourceMessage.XuLyThanhCong + " " + ResourceMessage.SoluuTruCuHoSoLa + MessageSuccess);
                }
                else
                {
                    MessageManager.Show(this.ParentForm, param, result);
                }
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (result)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //PrintProcessByMediRecordCode(PrintTypeMediRecord.IN_BARCODE_MEDI_RECORD_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveData_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                lciProgram.Visibility = chkBANgoaiTru.CheckState == CheckState.Checked
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                if (chkBANgoaiTru.CheckState == CheckState.Unchecked)
                {
                    cboProgram.EditValue = null;
                }
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

        private void cboDataStore_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDataStore.EditValue != null)
                    {
                        var dataStore = dataStores.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDataStore.EditValue ?? "").ToString()));
                        if (dataStore != null)
                        {
                            txtDataStoreCode.Text = dataStore.DATA_STORE_CODE;
                            cboDataStore.Properties.Buttons[1].Visible = true;
                            chkBANgoaiTru.Focus();
                            FocusButtonSave();
                        }
                        else
                        {
                            cboDataStore.Focus();
                            cboDataStore.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDataStore_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboDataStore.Text))
                    {
                        string key = cboDataStore.Text.ToLower();
                        var listData = this.dataStores.Where(o => o.DATA_STORE_CODE.ToLower().Contains(key) || o.DATA_STORE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboDataStore.EditValue = listData.First().ID;
                            txtDataStoreCode.Text = listData.First().DATA_STORE_CODE;
                        }
                    }
                    if (!valid)
                    {
                        cboDataStore.Focus();
                        cboDataStore.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDataStoreCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDataStore(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataStore(string _DataStoreCode)
        {
            try
            {
                List<HIS_DATA_STORE> listResult = new List<HIS_DATA_STORE>();
                listResult = this.dataStores.Where(o => (!String.IsNullOrWhiteSpace(o.DATA_STORE_CODE) && o.DATA_STORE_CODE.StartsWith(_DataStoreCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DATA_STORE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DATA_STORE_NAME", "", 250, 2));
                if (listResult.Count == 1)
                {
                    cboDataStore.EditValue = listResult[0].ID;
                    txtDataStoreCode.Text = listResult[0].DATA_STORE_CODE;
                    chkBANgoaiTru.Focus();

                }
                else if (listResult.Count > 1)
                {
                    cboDataStore.EditValue = null;
                    cboDataStore.Focus();
                    cboDataStore.ShowPopup();
                }
                else
                {
                    cboDataStore.EditValue = null;
                    cboDataStore.Focus();
                    cboDataStore.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSaveTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDataStoreCode.Focus();
                    txtDataStoreCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkBANgoaiTru_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboProgram.Focus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProgram_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProgram_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMediRecordType.Focus();
                    cboMediRecordType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediRecordType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSaveData.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkFilterDataStoreByEndInfo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.GetDataStore();
                this.LoadDataTocboDataStore(dataStores);
                this.SetDefaultDataStore();
                if (isInit)
                {
                    return;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_FILTER_DATA_STORE_BY_END_INFO && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (checkFilterDataStoreByEndInfo.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_FILTER_DATA_STORE_BY_END_INFO;
                    csAddOrUpdate.VALUE = (checkFilterDataStoreByEndInfo.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetLastTimeToLocal(long dataStoreId)
        {
            try
            {
                if (dataStoreId > 0)
                {
                    long time = Inventec.Common.DateTime.Get.Now().Value;
                    List<LocalStoreADO> storeADOs = null;
                    if (GlobalVariables.ListDataStoreUseTime != null && GlobalVariables.ListDataStoreUseTime is List<LocalStoreADO>)
                    {
                        storeADOs = (List<LocalStoreADO>)GlobalVariables.ListDataStoreUseTime;
                    }
                    else
                    {
                        storeADOs = new List<LocalStoreADO>();
                        GlobalVariables.ListDataStoreUseTime = storeADOs;
                    }

                    LocalStoreADO exist = storeADOs.FirstOrDefault(o => o.DataStoreId == dataStoreId);
                    if (exist != null)
                    {
                        exist.LastTime = time;
                    }
                    else
                    {
                        exist = new LocalStoreADO();
                        exist.DataStoreId = dataStoreId;
                        exist.LastTime = time;
                        storeADOs.Add(exist);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDataStore_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtDataStoreCode.Text = "";
                if (cboDataStore.EditValue != null)
                {
                    HIS_DATA_STORE data = dataStores.FirstOrDefault(o => o.ID == Convert.ToInt64(cboDataStore.EditValue));
                    if (data != null)
                    {
                        txtDataStoreCode.Text = data.DATA_STORE_CODE;
                        cboLoacationStore.Properties.DataSource = lstLocationStore.Where(o => o.DATA_STORE_ID == null || o.DATA_STORE_ID == data.ID).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkFilterDataStoreByTreatmentEndType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.GetDataStore();
                this.LoadDataTocboDataStore(dataStores);
                this.SetDefaultDataStore();
                if (isInit)
                {
                    return;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_FILTER_DATA_STORE_BY_TREATMENT_END_TYPE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (checkFilterDataStoreByTreatmentEndType.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_FILTER_DATA_STORE_BY_TREATMENT_END_TYPE;
                    csAddOrUpdate.VALUE = (checkFilterDataStoreByTreatmentEndType.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediRecordType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    FocusButtonSave();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoSetStoreCode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAutoSetStoreCode.Checked)
                {
                    txtStoreCode.Enabled = true;
                    chkUseEndCode.Checked = false;
                    chkUseEndCode.Enabled = false;
                }
                else
                {
                    txtStoreCode.Enabled = false;
                    txtStoreCode.Text = "";
                    chkUseEndCode.Enabled = true;
                }

                if (isInit)
                {
                    return;
                }
                //WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoSetStoreCode.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoSetStoreCode.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoSetStoreCode.Name;
                    csAddOrUpdate.VALUE = (chkAutoSetStoreCode.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                frmContentRefused ContentRefused = new frmContentRefused(this.currentModule, this.currentTreatment.FirstOrDefault());
                ContentRefused.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsTreatmentType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.GetDataStore();
                this.LoadDataTocboDataStore(dataStores);
                this.SetDefaultDataStore();
                if (isInit)
                {
                    return;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkIsTreatmentType.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkIsTreatmentType.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkIsTreatmentType.Name;
                    csAddOrUpdate.VALUE = (chkIsTreatmentType.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkUseEndCode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkUseEndCode.Checked)
                {
                    txtStoreCode.Enabled = false;
                    txtStoreCode.Text = "";
                    chkAutoSetStoreCode.Checked = false;
                    chkAutoSetStoreCode.Enabled = false;
                }
                else
                {
                    chkAutoSetStoreCode.Enabled = true;
                }

                if (isInit)
                {
                    return;
                }
                //WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkUseEndCode.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkUseEndCode.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkUseEndCode.Name;
                    csAddOrUpdate.VALUE = (chkUseEndCode.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLoacationStore_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboLoacationStore.EditValue = null;
                    txtLocationStoreCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLocationStoreCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtLocationStoreCode.Text.Trim()))
                    {
                        var data = lstLocationStore.FirstOrDefault(o => o.LOCATION_STORE_CODE == txtLocationStoreCode.Text.Trim());
                        if (data != null)
                        {
                            cboLoacationStore.EditValue = data.ID;
                            chkBANgoaiTru.Focus();
                            return;
                        }
                    }
                    cboLoacationStore.Focus();
                    cboLoacationStore.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLoacationStore_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (cboLoacationStore.EditValue != null)
                {
                    var data = lstLocationStore.FirstOrDefault(o => o.ID == Int64.Parse(cboLoacationStore.EditValue.ToString()));
                    if (data != null)
                    {
                        txtLocationStoreCode.Text = data.LOCATION_STORE_CODE;
                    }
                }
                else { txtLocationStoreCode.Text = null; }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
