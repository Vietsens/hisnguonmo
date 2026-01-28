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
using DevExpress.Skins.Info;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.WCF.DCO;
using His.Bhyt.ExportXml.Base;
using His.Bhyt.ExportXml.XML3220;

//using His.Bhyt.ExportXml.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExportXml3220.ADO;
using HIS.Desktop.Plugins.ExportXml3220.Base;
using HIS.Desktop.Plugins.ExportXml3220.Config;
using HIS.Desktop.Plugins.ExportXml3220.Resources;
using HIS.Desktop.Utility;
using HIS.UC.SettingSignInfo;
using IMSys.DbConfig.HIS_RS;

//using IMSys.DbConfig.HIS_RS;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.ServiceSign;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using GlobalConfigStore = His.Bhyt.ExportXml.XML3220.GlobalConfigStore;
using HIS_TREATMENT_TYPE = MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE;
using InputADO = His.Bhyt.ExportXml.XML3220.InputADO;
using SyncResultADO = His.Bhyt.ExportXml.XML3220.SyncResultADO;

namespace HIS.Desktop.Plugins.ExportXml3220
{
    public partial class UCExportXml3220 : UserControlBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private HIS_BRANCH currentBranch = null;
        private V_HIS_TREATMENT_10 currentTreatment = null;
        private bool isInit = true;
        public SavePathADO savePathADO;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        bool isNotLoadWhileChangeControlStateInFirst;
        bool isAutoSync = false;
        bool btnAutoSyncClick = false;
        bool isNotFileSign;
        bool timerTickIsRunning = false;
        bool isSendCollinearXml;
        bool callSyncSuccess;
        bool isExportXml;
        bool showMessSusscess;
        SettingSignADO SettingSignADO;
        ConfigSyncADO configSync;
        CommonParam paramUpdateXml130;
        List<HIS_BRANCH> branchSelecteds;
        List<HIS_PATIENT_TYPE> patientTypeSelecteds;
        List<HIS_PATIENT_TYPE> patientTypeTTSelecteds;
        List<HIS_TREATMENT_TYPE> treatmentTypeSelecteds;
        List<V_HIS_TREATMENT_10> listTreatmentSync;
        List<string> listMessageError;
        List<V_HIS_TREATMENT_10> listSelection = new List<V_HIS_TREATMENT_10>();
        List<HIS_CONFIG> NewConfig;
        List<V_HIS_PATIENT_TYPE_ALTER> ListPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
        List<V_HIS_BABY> ListBaby = new List<V_HIS_BABY>();
        List<V_HIS_TREATMENT_10> HisTreatments = new List<V_HIS_TREATMENT_10>();

        public UCExportXml3220(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            this.currentModule = module;
            InitializeComponent();
            HisConfigCFG.LoadConfig();
        }

        private void UCExportXml3220_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //SetCaptionByLanguageKey();
                this.InitUser();
                this.InitControlState();
                this.InitComboBranch();
                this.InitComboTreatmentType();
                this.InitCheckUSBToken();
                this.SetDefaultValueControl();
                this.FillDataToGridTreatment();
                this.isInit = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                txtPathSave.Text = "";
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.TXT_PATH_SAVE)
                        {
                            txtPathSave.Text = item.VALUE;
                        }
                        else if (item.KEY == chkSignFileCertUtil.Name)
                        {
                            SettingSignADO = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingSignADO>(item.VALUE);
                            chkSignFileCertUtil.Checked = SettingSignADO != null && !string.IsNullOrEmpty(SettingSignADO.SerialNumber);
                        }
                        else if (item.KEY == btnSettingConfigSync.Name)
                        {
                            configSync = !String.IsNullOrWhiteSpace(item.VALUE) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigSyncADO>(item.VALUE) : null;
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitComboTreatmentType()
        {
            try
            {
                List<HIS_TREATMENT_TYPE> listAll = new List<HIS_TREATMENT_TYPE>();

                HIS_TREATMENT_TYPE all = new HIS_TREATMENT_TYPE();
                all.ID = 0;
                all.TREATMENT_TYPE_CODE = "00";
                all.TREATMENT_TYPE_NAME = "Tất cả";
                listAll.Add(all);

                listAll.AddRange(BackendDataWorker.Get<HIS_TREATMENT_TYPE>());

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboTreatmentType, listAll, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitComboBranch()
        {
            try
            {
                List<HIS_BRANCH> listAll = new List<HIS_BRANCH>();

                HIS_BRANCH all = new HIS_BRANCH();
                all.ID = 0;
                all.BRANCH_CODE = "00";
                all.BRANCH_NAME = "Tất cả";

                listAll.Add(all);

                listAll.AddRange(BackendDataWorker.Get<HIS_BRANCH>());

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBranch, listAll, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitUser()
        {
            try
            {
                if (!BackendDataWorker.IsExistsKey<ACS_USER>())
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    List<ACS_USER> datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                dtOutTimeFrom.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                dtOutTimeTo.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                cboTreatmentType.EditValue = (long)0;
                txtTreatmentCode.Text = "";
                txtPatientCode.Text = "";
                txtKeyword.Text = "";
                this.currentBranch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
                //cboBranch.EditValue = this.currentBranch.ID;
                cboStatus.SelectedIndex = 0;
                cboXmlType.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitCheckUSBToken()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                //btnXML3176.Visible = !chkXML3176.Checked;
                //Inventec.Common.Logging.LogSystem.Info("Load form - checkbox XML3176: " + chkXML3176.Checked);
                if (SettingSignADO != null && !String.IsNullOrWhiteSpace(SettingSignADO.SerialNumber))
                {
                    chkSignFileCertUtil.Checked = !String.IsNullOrWhiteSpace(SettingSignADO.SerialNumber);
                    if (chkSignFileCertUtil.Checked && HisConfigCFG.BHXH__XML_SIGN_OPTION == "1")
                    {
                        chkSignFileCertUtil.Properties.ReadOnly = true;
                        chkSignFileCertUtil.Enabled = false;
                    }
                }
                else
                {
                    chkSignFileCertUtil.Checked = false;
                    chkSignFileCertUtil.Properties.ReadOnly = false;
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment()
        {
            try
            {
                int pageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                if (pageSize <= 0) pageSize = 50;
                FillDataToGridTreatment(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(FillDataToGridTreatment, param, pageSize, this.gridControlTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment(object param)
        {
            try
            {

                currentTreatment = null;
                this.FillDataByTreatmentSelect();
                List<V_HIS_TREATMENT_10> listData = new List<V_HIS_TREATMENT_10>();

                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisTreatmentView10Filter filter = new HisTreatmentView10Filter();
                filter.ORDER_DIRECTION = "ACS";
                filter.ORDER_FIELD = "OUT_TIME";
                //filter.TDL_PATIENT_TYPE_IDs = new List<long>()
                //{
                //    HisConfigCFG.PATIENT_TYPE_ID__BHYT
                //};
                if (cboBranch.EditValue != null && Convert.ToInt64(cboBranch.EditValue) > 0)
                    filter.BRANCH_ID = Convert.ToInt64(cboBranch.EditValue);

                filter.IS_PAUSE = true;
                filter.XML3220_TYPE = ENUM_XML3220_TYPE.ALL;

                if (cboXmlType.SelectedIndex == 1)
                {
                    filter.XML3220_TYPE = ENUM_XML3220_TYPE.CT03;
                }
                else if (cboXmlType.SelectedIndex == 2)
                {
                    filter.XML3220_TYPE = ENUM_XML3220_TYPE.CT04;
                }
                else if (cboXmlType.SelectedIndex == 3)
                {
                    filter.XML3220_TYPE = ENUM_XML3220_TYPE.CT05;
                }
                else if (cboXmlType.SelectedIndex == 4)
                {
                    filter.XML3220_TYPE = ENUM_XML3220_TYPE.CT06;
                }
                else if (cboXmlType.SelectedIndex == 5)
                {
                    filter.XML3220_TYPE = ENUM_XML3220_TYPE.CT07;
                }
                else if (cboXmlType.SelectedIndex == 6)
                {
                    filter.XML3220_TYPE = ENUM_XML3220_TYPE.CTGiayDieuTriNoiTru;
                }

                if (!String.IsNullOrEmpty(txtTreatmentCode.Text.Trim()))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (Char.IsDigit(code.FirstOrDefault()))
                    {
                        if (code.Length < 12)
                        {
                            try
                            {
                                code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                                txtTreatmentCode.Text = code;
                                filter.TREATMENT_CODE__EXACT = code;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else
                        {
                            filter.TREATMENT_CODE__EXACT = code;
                        }
                    }
                }
                else if (!String.IsNullOrEmpty(txtPatientCode.Text.Trim()))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (Char.IsDigit(code.FirstOrDefault()))
                    {
                        if (code.Length < 10)
                        {
                            try
                            {
                                code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                                txtPatientCode.Text = code;
                                filter.PATIENT_CODE__EXACT = code;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else
                        {
                            filter.PATIENT_CODE__EXACT = code;
                        }
                    }
                }

                if (String.IsNullOrEmpty(filter.TREATMENT_CODE__EXACT) && String.IsNullOrEmpty(filter.PATIENT_CODE__EXACT))
                {
                    if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                    {
                        filter.KEY_WORD = txtKeyword.Text.Trim();
                    }

                    if (cboTreatmentType.EditValue != null && (long)cboTreatmentType.EditValue > 0)
                    {
                        filter.TREATMENT_TYPE_ID = (long)cboTreatmentType.EditValue;
                    }

                    if (cboStatus.SelectedIndex == 1)
                    {
                        filter.HAS_TREAT_XML_RESULT = true;
                    }
                    else if (cboStatus.SelectedIndex == 2)
                    {
                        filter.HAS_TREAT_XML_RESULT = false;
                    }
                    else if (cboStatus.SelectedIndex == 3)
                    {
                        filter.TREATMENT_XML_RESULT = 1;
                    }
                    else if (cboStatus.SelectedIndex == 4)
                    {
                        filter.TREATMENT_XML_RESULT = 2;
                    }

                    if (dtOutTimeFrom.EditValue != null && dtOutTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_TIME_FROM = Convert.ToInt64(dtOutTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                    }
                    if (dtOutTimeTo.EditValue != null && dtOutTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_TIME_TO = Convert.ToInt64(dtOutTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");
                    }
                }

                var result = new BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_10>>("api/HisTreatment/GetView10", ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    listData = result.Data;
                    rowCount = (listData == null ? 0 : listData.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }
                gridControlTreatment.BeginUpdate();
                gridControlTreatment.DataSource = listData;
                gridControlTreatment.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataByTreatmentSelect()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    lblAddress.Text = this.currentTreatment.TDL_PATIENT_ADDRESS ?? "";
                    lblCarrer.Text = this.currentTreatment.TDL_PATIENT_CAREER_NAME ?? "";
                    if (this.currentTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1)
                    {
                        lblDob.Text = this.currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatment.TDL_PATIENT_DOB);
                    }
                    lblEndDepartment.Text = this.currentTreatment.END_DEPARTMENT_NAME ?? "";
                    lblEthnicName.Text = this.currentTreatment.ETHNIC_NAME ?? "";
                    lblGenderName.Text = this.currentTreatment.TDL_PATIENT_GENDER_NAME ?? "";
                    lblHeinCardNumber.Text = this.currentTreatment.TDL_HEIN_CARD_NUMBER ?? "";
                    lblIcdName.Text = this.currentTreatment.ICD_NAME ?? "";
                    lblIcdText.Text = this.currentTreatment.ICD_TEXT ?? "";
                    lblInTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.currentTreatment.IN_TIME) ?? "";
                    lblOutTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.currentTreatment.OUT_TIME ?? 0) ?? "";
                    lblPatientCode.Text = this.currentTreatment.TDL_PATIENT_CODE ?? "";
                    lblPatientName.Text = this.currentTreatment.TDL_PATIENT_NAME ?? "";
                    lblRelativeName.Text = this.currentTreatment.TDL_PATIENT_RELATIVE_NAME ?? "";
                    lblTreatmentCode.Text = this.currentTreatment.TREATMENT_CODE ?? "";
                    lblTreatmentEndType.Text = this.currentTreatment.TREATMENT_END_TYPE_NAME ?? "";
                    lblTreatmentEndTypeExt.Text = this.currentTreatment.TREATMENT_END_TYPE_EXT_NAME ?? "";
                    lblTreatmentResult.Text = this.currentTreatment.TREATMENT_RESULT_NAME ?? "";
                    lblTreatmentType.Text = this.currentTreatment.TREATMENT_TYPE_NAME ?? "";
                    lblWorkPlace.Text = this.currentTreatment.TDL_PATIENT_WORK_PLACE ?? (this.currentTreatment.TDL_PATIENT_WORK_PLACE_NAME ?? "");
                    lblAdvise.Text = this.currentTreatment.ADVISE ?? "";
                    txtClinicalNote.Text = this.currentTreatment.CLINICAL_NOTE ?? "";
                    txtSubclinicalResult.Text = this.currentTreatment.SUBCLINICAL_RESULT ?? "";
                    txtTreatmentMethod.Text = this.currentTreatment.TREATMENT_METHOD ?? "";

                    if (this.currentTreatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM
                        || this.currentTreatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                    {
                        lblSickRelativeName.Text = this.currentTreatment.TDL_PATIENT_RELATIVE_NAME ?? "";
                        lblSickTime.Text = String.Format("{0} - {1}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatment.SICK_LEAVE_FROM ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatment.SICK_LEAVE_TO ?? 0));
                        lblSickUser.Text = String.Format("{0} - {1}", this.currentTreatment.SICK_LOGINNAME, this.currentTreatment.SICK_USERNAME);
                        lblExtraCode.Text = this.currentTreatment.EXTRA_END_CODE ?? "";
                        if (!String.IsNullOrEmpty(this.currentTreatment.TDL_PATIENT_WORK_PLACE_NAME))
                            lblSickWorkPlace.Text = this.currentTreatment.TDL_PATIENT_WORK_PLACE_NAME;
                        else
                            lblSickWorkPlace.Text = this.currentTreatment.TDL_PATIENT_WORK_PLACE ?? "";
                    }
                    else
                    {
                        lblSickRelativeName.Text = "";
                        lblSickTime.Text = "";
                        lblSickUser.Text = "";
                        lblExtraCode.Text = "";
                        lblSickWorkPlace.Text = "";
                    }

                    HisBabyViewFilter babyFilter = new HisBabyViewFilter();
                    babyFilter.TREATMENT_ID = this.currentTreatment.ID;
                    List<V_HIS_BABY> listBabys = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, babyFilter, null);
                    listBabys = listBabys != null ? listBabys.Where(o => o.BIRTH_CERT_BOOK_ID.HasValue).ToList() : null;
                    gridControlBabys.BeginUpdate();
                    gridControlBabys.DataSource = listBabys;
                    gridControlBabys.EndUpdate();
                }
                else
                {
                    lblAddress.Text = "";
                    lblCarrer.Text = "";
                    lblDob.Text = "";
                    lblEndDepartment.Text = "";
                    lblEthnicName.Text = "";
                    lblGenderName.Text = "";
                    lblHeinCardNumber.Text = "";
                    lblIcdName.Text = "";
                    lblIcdText.Text = "";
                    lblInTime.Text = "";
                    lblOutTime.Text = "";
                    lblPatientCode.Text = "";
                    lblPatientName.Text = "";
                    lblRelativeName.Text = "";
                    lblTreatmentCode.Text = "";
                    lblTreatmentEndType.Text = "";
                    lblTreatmentEndTypeExt.Text = "";
                    lblTreatmentResult.Text = "";
                    lblTreatmentType.Text = "";
                    lblWorkPlace.Text = "";
                    lblAdvise.Text = "";

                    txtClinicalNote.Text = "";
                    txtSubclinicalResult.Text = "";
                    txtTreatmentMethod.Text = "";

                    lblSickRelativeName.Text = "";
                    lblSickTime.Text = "";
                    lblSickUser.Text = "";
                    lblExtraCode.Text = "";
                    lblSickWorkPlace.Text = "";

                    gridControlBabys.BeginUpdate();
                    gridControlBabys.DataSource = null;
                    gridControlBabys.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBranch_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtOutTimeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBranch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtOutTimeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtOutTimeTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutTimeFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtOutTimeTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboTreatmentType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutTimeTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTreatmentType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtTreatmentCode.Focus();
                    txtTreatmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatmentCode.Focus();
                    txtTreatmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                    {
                        btnFind_Click(null, null);
                    }
                    else
                    {
                        txtPatientCode.Focus();
                        txtPatientCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtPatientCode.Text))
                    {
                        btnFind_Click(null, null);
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
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
                    cboStatus.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatus_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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

        private void cboStatus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled) return;
                WaitingManager.Show();
                FillDataToGridTreatment();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex < 0 || !e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound)
                    return;
                //var data = (V_HIS_TREATMENT_10)gridViewTreatment.GetRow(e.ListSourceRowIndex);
                var data = (V_HIS_TREATMENT_10)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging.pagingGrid.CurrentPage - 1) * ucPaging.pagingGrid.PageSize;
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1)
                        {
                            e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                        else
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                    }

                    else if (e.Column.FieldName == "IN_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.IN_TIME);
                    }
                    else if (e.Column.FieldName == "OUT_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.OUT_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "STATUS")
                    {
                        if (data.TREAT_IS_EXPORTED_XML == (short)1 || !String.IsNullOrWhiteSpace(data.TREATMENT_XML_URL))
                        {
                            e.Value = "Đã xuất";
                        }
                        else
                        {
                            e.Value = "Chưa xuất";
                        }
                    }
                    else if (e.Column.FieldName == "SICK_HEIN_CARD_NUMBER_STR")
                    {
                        if (!string.IsNullOrEmpty(data.SICK_HEIN_CARD_NUMBER))
                            e.Value = data.SICK_HEIN_CARD_NUMBER;
                        else
                            e.Value = data.TDL_HEIN_CARD_NUMBER;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBabys_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex < 0 || !e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound)
                    return;
                var data = (V_HIS_BABY)gridViewBabys.GetRow(e.ListSourceRowIndex);
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "BORN_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.BORN_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPathSave_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        if (this.MakeFolderSave(fbd.SelectedPath))
                        {
                            txtPathSave.Text = fbd.SelectedPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool MakeFolderSave(string folderPath)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrWhiteSpace(folderPath))
                {
                    if (System.IO.Directory.Exists(folderPath))
                    {
                        return true;
                    }
                    var dicInfo = System.IO.Directory.CreateDirectory(folderPath);
                    if (dicInfo == null)
                    {
                        LogSystem.Warn("Tao folderPath luu file that bai");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void txtPathSave_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (isInit)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.TXT_PATH_SAVE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = txtPathSave.Text ?? "";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.TXT_PATH_SAVE;
                    csAddOrUpdate.VALUE = txtPathSave.Text ?? "";
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportXml.Enabled || listSelection == null || listSelection.Count == 0) return;
                CommonParam param = new CommonParam();
                MemoryStream memoryStream = new MemoryStream();
                bool success = false;
                bool xuatXml12 = true;
                if (String.IsNullOrWhiteSpace(txtPathSave.Text))
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        txtPathSave.Text = fbd.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                }
                //if (string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK))
                //{
                //    if (XtraMessageBox.Show("Chưa chọn thư mục lưu file chỉ tiêu dữ liệu giám định y khoa. Bạn có muốn chọn đường dẫn không?", Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                //        btnSavePath_Click(null, null);
                //}
                //xuatXml12 = !string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK);
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click Begin");
                success = this.GenerateXml(ref param, ref memoryStream, false, false, xuatXml12, listSelection);
                Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click End");
                WaitingManager.Hide();
                if (success && param.Messages.Count == 0)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                }
                else if (param.Messages.Count >= 1)
                {
                    MessageManager.Show(param, success);
                }
                else
                {
                    param.Messages.Add("Xuất XML thất bại. Vui lòng kiểm tra lại.");
                    MessageManager.Show(this.ParentForm, param, false);
                }
                this.gridControlTreatment.RefreshDataSource();

                //if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathXml))
                //{
                //    btnSavePath_Click(null, null);
                //}
                //if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                //{
                //    if (string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK))
                //    {
                //        if (XtraMessageBox.Show("Chưa chọn thư mục lưu file chỉ tiêu dữ liệu giám định y khoa. Bạn có muốn chọn đường dẫn không?", Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                //            btnSavePath_Click(null, null);
                //    }
                //    xuatXml12 = !string.IsNullOrEmpty(this.savePathADO.pathXmlGDYK);
                //    WaitingManager.Show();
                //    Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click Begin");
                //    success = this.GenerateXml(ref param, ref memoryStream, false, false, xuatXml12, listSelection);
                //    Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click End");
                //    WaitingManager.Hide();
                //    if (success && param.Messages.Count == 0)
                //    {
                //        MessageManager.Show(this.ParentForm, param, success);
                //    }
                //    else if (param.Messages.Count >= 1)
                //    {
                //        MessageManager.Show(param, success);
                //    }
                //    else
                //    {
                //        param.Messages.Add("Xuất XML thất bại. Vui lòng kiểm tra lại.");
                //        MessageManager.Show(this.ParentForm, param, false);
                //    }
                //    this.gridControlTreatment.RefreshDataSource();
                //}
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnSavePath_Click(object sender, EventArgs e)
        {
            try
            {
                frmSettingSavePath frmSettingSavePath = new frmSettingSavePath(savePathADO, UpdateSavePath);
                frmSettingSavePath.ShowDialog(this.ParentForm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool GenerateXml3220(ref CommonParam paramExport, List<V_HIS_TREATMENT_10> listSelected)
        {
            bool result = false;
            try
            {
                if (listSelected.Count > 0)
                {
                    if (!GlobalConfigStore.IsInit)
                        if (!this.SetDataToLocalXml())
                        {
                            paramExport.Messages.Add(ResourceMessage.KhongThieLapDuocCauHinhDuLieuXuatXml);
                            return result;
                        }
                    GlobalConfigStore.PathSaveXml = txtPathSave.Text;

                    int skip = 0;
                    while (listSelected.Count - skip > 0)
                    {
                        List<V_HIS_TREATMENT_10> limit = listSelected.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        string message = "";
                        HisBabyViewFilter babyFilter = new HisBabyViewFilter();
                        babyFilter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                        List<V_HIS_BABY> babys = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, babyFilter, null);
                        //babys = babys != null ? babys.Where(o => o.BIRTH_CERT_BOOK_ID.HasValue).ToList() : null;
                        babys = babys != null ? babys.ToList() : null;
                        //message = ProcessExportXmlDetail(ref result, limit, babys);

                        if (!String.IsNullOrEmpty(message))
                        {
                            paramExport.Messages.Add(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        //Xử lý Xuất XML
        private string ProcessExportXmlDetail(ref bool isSuccess, ref MemoryStream memoryStream, bool viewXml, List<V_HIS_TREATMENT_10> listTreatment, List<V_HIS_PATIENT_TYPE_ALTER> listPatientTypeAlter, List<V_HIS_BABY> listBaby)
        {
            string result = "";
            List<V_HIS_BABY> babys = listBaby != null ? listBaby.Where(o => o.BIRTH_CERT_BOOK_ID.HasValue).ToList() : null;
            Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
            try
            {
                List<V_HIS_TREATMENT_10> listSuccess = new List<V_HIS_TREATMENT_10>();

                Dictionary<long, List<V_HIS_BABY>> dicBaby = new Dictionary<long, List<V_HIS_BABY>>();

                if (babys != null && babys.Count > 0)
                {
                    foreach (V_HIS_BABY item in babys)
                    {
                        if (!dicBaby.ContainsKey(item.TREATMENT_ID))
                            dicBaby[item.TREATMENT_ID] = new List<V_HIS_BABY>();
                        dicBaby[item.TREATMENT_ID].Add(item);
                    }
                }
                string xmlTypeCode = null;
                if (cboXmlType.SelectedIndex == 1)
                {
                    xmlTypeCode = "CT03";
                }
                else if (cboXmlType.SelectedIndex == 2)
                {
                    xmlTypeCode = "CT04";
                }
                else if (cboXmlType.SelectedIndex == 3)
                {
                    xmlTypeCode = "CT05";
                }
                else if (cboXmlType.SelectedIndex == 4)
                {
                    xmlTypeCode = "CT06";
                }
                else if (cboXmlType.SelectedIndex == 5)
                {
                    xmlTypeCode = "CT07";
                }
                List<string> lstTreatmentCode = new List<string>();
                foreach (V_HIS_TREATMENT_10 treat in listTreatment)
                {
                    Inventec.Common.Logging.LogSystem.Error(treat.ADVISE + "   START #######################_______________________________________");
                    var lstBabyNotHasBirthCertBook = listBaby.Where(o => !o.BIRTH_CERT_BOOK_ID.HasValue && o.TREATMENT_ID == treat.ID).FirstOrDefault();
                    if (lstBabyNotHasBirthCertBook != null)
                    {
                        lstTreatmentCode.Add(treat.TREATMENT_CODE);
                    }
                    else
                    {
                        InputADO ado = new InputADO();
                        ado.Treatment = treat;
                        Inventec.Common.Logging.LogSystem.Error(treat.ADVISE + "  END  #######################_______________________________________");
                        ado.Employees = BackendDataWorker.Get<HIS_EMPLOYEE>();
                        ado.Branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == treat.BRANCH_ID);
                        ado.AcsUsers = BackendDataWorker.Get<ACS_USER>();
                        if (dicBaby.ContainsKey(treat.ID))
                        {
                            ado.Babys = dicBaby[treat.ID];
                        }
                        //ado.XML2076__DOC_CODE = xmlTypeCode;
                        ado.ConfigData = BackendDataWorker.Get<HIS_CONFIG>().ToList();
                        ado.PatientTypeAlter = listPatientTypeAlter
                            .Where(p => p.TREATMENT_ID == treat.ID)
                            .OrderByDescending(p => p.LOG_TIME)
                            .ThenByDescending(p => p.ID)
                            .FirstOrDefault();
                        His.Bhyt.ExportXml.XML3220.CreateXmlMain xmlMain = new His.Bhyt.ExportXml.XML3220.CreateXmlMain(ado);

                        string errorMess = "";
                        //string errorCode = "";
                        var fullFileName = xmlMain.RunXml3220Path(ref errorMess);

                        if (String.IsNullOrWhiteSpace(fullFileName))
                        {
                            LogSystem.Info("Khong tao duoc XML3220 cho Ho so duyet bhyt TreatmentCode: " + LogUtil.TraceData("TREATMENT_CODE", treat.TREATMENT_CODE));
                            if (!DicErrorMess.ContainsKey(errorMess))
                            {
                                DicErrorMess[errorMess] = new List<string>();
                            }
                        }
                        else
                        {
                            isSuccess = true;
                            listSuccess.Add(treat);
                        }
                        His.Bhyt.ExportXml.XML3220.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML3220.CreateXmlProcessor(ado);
                        string saveFilePath = "";
                        string fullFileNameSign = "";
                        if (!viewXml)
                        {
                            fullFileNameSign = xmlProcessor.GetFileName();
                            saveFilePath = String.Format("{0}/{1}", GlobalConfigStore.PathSaveXml, fullFileNameSign);
                        }
                        //Xử lý view
                        //if (viewXml)
                        //{

                            //string saveFilePath = "";
                            var rs = xmlProcessor.Run(ref errorMess);
                            if (!String.IsNullOrWhiteSpace(errorMess))
                            {
                                Inventec.Common.Logging.LogSystem.Error("Run3220: " + errorMess);
                            }
                            if (rs != null)
                            {
                                if (viewXml)
                                {
                                    memoryStream = rs;

                                //xử lý tạm chỉ nếu xem thì xóa file đã xuất
                                if (File.Exists(fullFileName))
                                {
                                    File.Delete(fullFileName);
                                }
                            }
                                else if(isNotFileSign == false && SettingSignADO != null)
                                {
                                    using (FileStream file = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write))
                                    {
                                        rs.WriteTo(file);
                                    }
                                    rs.Close();
                                }
                                isSuccess = true;
                            }
                            else
                            {
                                if (!DicErrorMess.ContainsKey(errorMess))
                                {
                                    DicErrorMess[errorMess] = new List<string>();
                                }

                                DicErrorMess[errorMess].Add(treat.TREATMENT_CODE);
                            }
                        //}

                        //Xuất xml có check Ký số
                        if (isNotFileSign == false && SettingSignADO != null)
                        {
                            if (File.Exists(fullFileName))
                            {
                                File.Delete(fullFileName);
                            }
                            string currentDirectory = Directory.GetCurrentDirectory();

                            string tempFolderPath = Path.Combine(currentDirectory, "Temp");
                            Directory.CreateDirectory(tempFolderPath);
                            fullFileName = xmlProcessor.GetFileName();
                            string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                            File.Create(tempFilePath).Close();

                            string pathAfterFileSign = "";
                            WcfSignDCO wcfSignDCO = null;
                            if (SettingSignADO.IsHsm)
                            {
                                var xmlBase64 = SourceFileSignApi(ReadFileContent(saveFilePath));
                                if (!string.IsNullOrEmpty(xmlBase64))
                                {
                                    try
                                    {
                                        var xmlBytes = Convert.FromBase64String(xmlBase64);
                                        File.WriteAllBytes(tempFilePath, xmlBytes);
                                        pathAfterFileSign = tempFilePath;
                                    }
                                    catch (Exception ex)
                                    {
                                        Inventec.Common.Logging.LogSystem.Error("Error saving xmlBase64 to file: " + ex);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        if (File.Exists(saveFilePath))
                                        {
                                            File.Delete(saveFilePath);
                                        }
                                    }
                                    catch (IOException ioEx)
                                    {
                                        Inventec.Common.Logging.LogSystem.Error("File đang bị khóa, chưa xóa được: " + ioEx);
                                    }
                                }
                            }
                            else
                            {
                                wcfSignDCO = new WcfSignDCO();
                                wcfSignDCO.SerialNumber = SettingSignADO.SerialNumber;
                                wcfSignDCO.OutputFile = tempFilePath;
                                wcfSignDCO.PIN = "";
                                //if (!string.IsNullOrEmpty(saveFilePathCollinearXml))
                                //{
                                //    wcfSignDCO.SourceFile = saveFilePathCollinearXml;
                                //}
                                //else
                                //{
                                    wcfSignDCO.SourceFile = saveFilePath;
                                //}
                                wcfSignDCO.fieldSigned = "CHUKYDONVI";
                                string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                                SignProcessorClient signProcessorClient = new SignProcessorClient();
                                if (VerifyServiceSignProcessorIsRunning())
                                {
                                    var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                                    if (wcfSignResultDCO != null && wcfSignResultDCO.Success)
                                    {
                                        pathAfterFileSign = wcfSignResultDCO.OutputFile;
                                        if (!File.Exists(pathAfterFileSign) || new FileInfo(pathAfterFileSign).Length == 0)
                                        {
                                            XtraMessageBox.Show("Ký số thất bại: file output không tồn tại hoặc rỗng.");
                                            isSuccess = false;
                                            return "";
                                        }
                                        Inventec.Common.Logging.LogSystem.Debug("wcfSignResultDCO.OutputFile: " + Inventec.Common.Logging.LogUtil.TraceData("output file", wcfSignResultDCO.OutputFile));
                                    }
                                }
                            }
                            //if (this.savePathADO == null || string.IsNullOrEmpty(this.savePathADO.pathCollinearXml))
                            //{
                            //    XtraMessageBox.Show("Vui lòng thiết lập thư mục lưu trữ trước khi xuất dữ liệu.", Resources.ResourceMessageLang.ThongBao);
                            //    btnSavePath_Click(null, null);
                            //}
                            //if (this.savePathADO != null && !string.IsNullOrEmpty(this.savePathADO.pathXml))
                            //{
                            //    if (!string.IsNullOrEmpty(pathAfterFileSign))
                            //    {
                            //        if (wcfSignDCO != null)
                            //        {
                            //            if (wcfSignDCO.SourceFile.Trim() != pathAfterFileSign.Trim())
                            //            {
                            //                if (File.Exists(wcfSignDCO.SourceFile))
                            //                {
                            //                    File.Delete(wcfSignDCO.SourceFile);
                            //                }
                            //            }
                            //            File.Copy(pathAfterFileSign, wcfSignDCO.SourceFile);
                            //        }
                            //        else if (SettingSignADO.IsHsm)
                            //        {
                            //            var sourceFile = saveFilePath;
                            //            if (sourceFile.Trim() != pathAfterFileSign.Trim())
                            //            {
                            //                if (File.Exists(sourceFile))
                            //                {
                            //                    File.Delete(sourceFile);
                            //                }
                            //            }
                            //            File.Copy(pathAfterFileSign, sourceFile);
                            //        }
                            //    }
                            //}
                            if (!string.IsNullOrEmpty(saveFilePath))
                            {
                                if (wcfSignDCO != null)
                                {
                                    File.Copy(pathAfterFileSign, wcfSignDCO.SourceFile, true);
                                }
                                else if (SettingSignADO.IsHsm)
                                {
                                    File.Copy(pathAfterFileSign, saveFilePath, true);
                                }
                            }
                        }
                    }
                }
                if (lstTreatmentCode != null && lstTreatmentCode.Count > 0)
                {
                    WaitingManager.Hide();
                    string treatmentCode = String.Join(",", lstTreatmentCode);
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongCoSoChungSinh + ": " + treatmentCode, ResourceMessage.Thongbao, System.Windows.Forms.MessageBoxButtons.OK);
                }
                if (DicErrorMess.Count > 0)
                {
                    foreach (var item in DicErrorMess)
                    {
                        result += String.Format("{0}:{1}. ", item.Key, String.Join(",", item.Value));
                    }
                }

                List<V_HIS_TREATMENT_10> listUpdate = listSuccess.Where(o => o.TREAT_IS_EXPORTED_XML != (short)1).ToList();

                //this.UpdateIsExported(listUpdate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void UpdateIsExported(List<V_HIS_TREATMENT_10> listUpdate)
        {
            try
            {
                if (listUpdate != null && listUpdate.Count > 0)
                {
                    CommonParam upParam = new CommonParam();
                    bool rs = new BackendAdapter(upParam).Post<bool>("api/HisTreatment/UpdateExportedXml3220", ApiConsumers.MosConsumer, listUpdate.Select(s => s.ID).ToList(), upParam);
                    if (!rs)
                    {
                        LogSystem.Warn("Update IS_EXPORTED_XML3220 cho Treatment that bai: \n" + String.Join(", ", listUpdate.Select(s => s.TREATMENT_CODE).ToList()));
                    }
                    else
                    {
                        listUpdate.ForEach(o => o.TREAT_IS_EXPORTED_XML = (short)1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool SetDataToLocalXml()
        {
            bool result = false;
            try
            {
                if (this.currentBranch == null)
                {
                    return result;
                }

                GlobalConfigStore.Branch = this.currentBranch;

                GlobalConfigStore.IsInit = true;
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void BtnFind()
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

        public void BtnExportXml()
        {
            try
            {
                btnExportXml_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        // Export đổi thành View
        private void repositoryItemBtnExport_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var rowData = (V_HIS_TREATMENT_10)view.GetRow(e.RowHandle);
                    if (rowData == null) return;

                    if (e.Column.FieldName == "STATUS_ICON")
                    {
                        if (rowData.TREATMENT_XML_RESULT == 1) // Lỗi đồng bộ
                        {
                            if (rowData.TREATMENT_XML_DESC != null)
                            {
                                e.RepositoryItem = btnError;
                            }
                            else
                            {
                                e.RepositoryItem = btnFail;
                            }
                        }
                        else if (rowData.TREATMENT_XML_RESULT == 2) // Gửi thành công
                        {
                            e.RepositoryItem = btnSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_TREATMENT_10)gridViewTreatment.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.TREATMENT_XML_DESC))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.TREATMENT_XML_DESC, "Thông tin lỗi");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_RowClick(object sender, RowClickEventArgs e)
        {
            currentTreatment = gridViewTreatment.GetRow(e.RowHandle) as V_HIS_TREATMENT_10;
            FillDataByTreatmentSelect();
        }

        private void chkSignFileCertUtil_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                    return;

                isChkSignFileCertUtil();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void isChkSignFileCertUtil()
        {
            try
            {
                if (chkSignFileCertUtil.Checked == true)
                {
                    frmSetting frm = new frmSetting(SettingSignADO, (result) =>
                    {
                        SettingSignADO = (SettingSignADO)result;
                    });
                    frm.ShowDialog();
                    if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                        chkSignFileCertUtil.Checked = false;
                }
                else
                {
                    SettingSignADO = null;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignFileCertUtil.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignFileCertUtil.Name;
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                    csAddOrUpdate.MODULE_LINK = this.currentModule.ModuleLink;
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

        private void btnSettingConfigSync_Click(object sender, EventArgs e)
        {
            try
            {
                if (configSync == null)
                {
                    ConfigSyncADO tempConfigSync = new ConfigSyncADO();
                    //tempConfigSync.branchIds = this.branchSelecteds?.Select(o => o.ID).ToList();
                    //tempConfigSync.patientTypeIds = this.patientTypeSelecteds?.Select(o => o.ID).ToList();
                    //tempConfigSync.patientTypeTTIds = this.patientTypeTTSelecteds?.Select(o => o.ID).ToList();
                    //tempConfigSync.treatmentTypeIds = this.treatmentTypeSelecteds?.Select(o => o.ID).ToList();
                    tempConfigSync.statusId = 0;
                    tempConfigSync.period = 10;
                    //tempConfigSync.isCheckOutTime = false;
                    //tempConfigSync.isCheckCollinearXml = false;
                    //tempConfigSync.isXML3176 = false;
                    frmSettingConfigSync frmSettingConfigSync = new frmSettingConfigSync(tempConfigSync, isAutoSync, UpdateConfigSign);
                    frmSettingConfigSync.ShowDialog(this.ParentForm);
                }
                else
                {
                    frmSettingConfigSync frmSettingConfigSync = new frmSettingConfigSync(configSync, isAutoSync, UpdateConfigSign);
                    frmSettingConfigSync.ShowDialog(this.ParentForm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UpdateConfigSign(ConfigSyncADO config)
        {
            try
            {
                if (config != null)
                {
                    this.configSync = config;

                    string value = Newtonsoft.Json.JsonConvert.SerializeObject(configSync);
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == btnSettingConfigSync.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = value;
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = btnSettingConfigSync.Name;
                        csAddOrUpdate.VALUE = value;
                        csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAutoSync_Click(object sender, EventArgs e)
        {
            try
            {
                btnAutoSyncClick = true;
                //if (configSync.isXML3176)
                //{
                //    isXML130 = false;
                //}
                //else
                //{
                //    isXML130 = true;
                //}
                if (configSync == null)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.VuiLongThietLapDieuKienGuiHoSoTruocKhiThucHien, Resources.ResourceMessageLang.ThongBao);
                    ConfigSyncADO tempConfigSync = new ConfigSyncADO();
                    //tempConfigSync.branchIds = this.branchSelecteds?.Select(o => o.ID).ToList();
                    //tempConfigSync.patientTypeIds = this.patientTypeSelecteds?.Select(o => o.ID).ToList();
                    //tempConfigSync.patientTypeTTIds = this.patientTypeTTSelecteds?.Select(o => o.ID).ToList();
                    //tempConfigSync.treatmentTypeIds = this.treatmentTypeSelecteds?.Select(o => o.ID).ToList();
                    tempConfigSync.statusId = 0;
                    tempConfigSync.period = 10;
                    //tempConfigSync.isCheckOutTime = false;
                    //tempConfigSync.isCheckCollinearXml = false;
                    //tempConfigSync.isXML3176 = false;
                    frmSettingConfigSync frmSettingConfigSync = new frmSettingConfigSync(tempConfigSync, isAutoSync, UpdateConfigSign);
                    frmSettingConfigSync.ShowDialog(this.ParentForm);
                }
                if (chkSignFileCertUtil.Checked == false)
                {
                    if (!isAutoSync && this.configSync != null && this.configSync.period > 0)
                    {
                        isNotFileSign = true;
                        isAutoSync = true;
                        btnAutoSync.Text = Resources.ResourceMessageLang.DangDongBo;
                        btnAutoSync.ToolTip = Resources.ResourceMessageLang.DangChayTienTrinhDongBoDuLieuXml130LenCongBHYT;
                        this.StartTimer();
                    }
                    else
                    {
                        isAutoSync = false;
                        autoSync.Stop();
                        btnAutoSync.Text = Resources.ResourceMessageLang.DongBoTD;
                        btnAutoSync.ToolTip = Resources.ResourceMessageLang.DongBoTuDong;
                    }
                }
                else
                {
                    if (SettingSignADO == null || (SettingSignADO != null && string.IsNullOrEmpty(SettingSignADO.SerialNumber)))
                    {
                        MessageBox.Show("Không có thông tin HSM server/Usb Token ký số");
                        return;
                    }
                    else
                    {
                        //isNotFileSign = false;
                        //isAutoSync = true;
                        //btnAutoSync.Text = Resources.ResourceMessageLang.DangDongBo;
                        //btnAutoSync.ToolTip = Resources.ResourceMessageLang.DangChayTienTrinhDongBoDuLieuXml130LenCongBHYT;
                        //this.StartTimer();
                        if (!isAutoSync && this.configSync != null && this.configSync.period > 0)
                        {
                            isNotFileSign = false;
                            isAutoSync = true;
                            btnAutoSync.Text = Resources.ResourceMessageLang.DangDongBo;
                            btnAutoSync.ToolTip = Resources.ResourceMessageLang.DangChayTienTrinhDongBoDuLieuXml130LenCongBHYT;
                            this.StartTimer();
                        }
                        else
                        {
                            isAutoSync = false;
                            autoSync.Stop();
                            btnAutoSync.Text = Resources.ResourceMessageLang.DongBoTD;
                            btnAutoSync.ToolTip = Resources.ResourceMessageLang.DongBoTuDong;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void StartTimer()
        {
            try
            {
                autoSync.Interval = (int)(configSync.period * 60000);
                autoSync.Enabled = true;
                this.autoSync_Tick(null, null);
                autoSync.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void autoSync_Tick(object sender, EventArgs e)
        {
            try
            {
                if (timerTickIsRunning)
                {
                    LogSystem.Info("Tien trinh tu dong dong bo dang chay. Khong cho phep khoi tao tien trinh khac");
                    return;
                }
                timerTickIsRunning = true;

                LogSystem.Info("Begin Run Thread Auto Sync");

                if (this.configSync.isCheckCollinearXml)
                {
                    listTreatmentSync = new List<V_HIS_TREATMENT_10>();
                    //lay ho so khoa bhyt
                    this.configSync.isCheckOutTime = true;
                    var listTreatmentLockBHYT = this.GetTreatment();
                    listTreatmentSync.AddRange(listTreatmentLockBHYT);
                    //lay ho so ket thuc dieu tri
                    this.configSync.isCheckOutTime = false;
                    var listTreatmentEnd = this.GetTreatment();
                    listTreatmentSync.AddRange(listTreatmentEnd);
                }
                else
                {
                    listTreatmentSync = this.GetTreatment();
                }

                if (this.configSync.isXML3176 && !backgroundWorker1.IsBusy)
                {
                    LogSystem.Info("Thread Auto Sync. isXML3176 ");
                    backgroundWorker1.RunWorkerAsync();
                }

                if (listTreatmentSync != null && listTreatmentSync.Count > 0 && !backgroundWorker1.IsBusy)
                {
                    LogSystem.Info("Thread Auto Sync. TreatmentCount: " + listTreatmentSync.Count);
                    backgroundWorker1.RunWorkerAsync();
                }
                else
                {
                    LogSystem.Info("Khong co ho so dieu tri nao. Khong thuc hien tu dong dong bo");
                }
                LogSystem.Info("End Run Thread Auto Auto Sync");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            timerTickIsRunning = false;
        }

        private List<V_HIS_TREATMENT_10> GetTreatment()
        {
            List<V_HIS_TREATMENT_10> result = null;
            try
            {
                if (configSync != null)
                {
                    HisTreatmentView10Filter filter = new HisTreatmentView10Filter();
                    if (configSync.branchIds != null && configSync.branchIds.Count > 0)
                        filter.BRANCH_IDs = configSync.branchIds;
                    if (configSync.patientTypeIds != null && configSync.patientTypeIds.Count > 0)
                        filter.TDL_PATIENT_TYPE_IDs = configSync.patientTypeIds;
                    if (configSync.treatmentTypeIds != null && configSync.treatmentTypeIds.Count > 0)
                        filter.TREATMENT_TYPE_IDs = configSync.treatmentTypeIds;
                    if (configSync.statusId != null)
                    {
                        if (configSync.statusId == 1)
                        {
                            filter.IS_LOCK_HEIN = true;
                        }
                        else if (configSync.statusId == 2)
                        {
                            filter.IS_PAUSE = true;
                        }
                        else if (configSync.statusId == 3)
                        {
                            filter.HAS_IN_CODE = true;
                        }
                    }
                    if (configSync.isCheckOutTime)
                    {
                        filter.OUT_TIME_FROM = Convert.ToInt64(DateTime.Today.ToString("yyyyMMdd") + "000000");
                        filter.OUT_TIME_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        filter.IS_PAUSE = true;
                    }
                    else
                    {
                        filter.FEE_LOCK_TIME_FROM = Convert.ToInt64(DateTime.Today.ToString("yyyyMMdd") + "000000");
                        filter.FEE_LOCK_TIME_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    }

                    filter.HAS_TREAT_XML_RESULT = false;
                    LogSystem.Debug("Treatment Filter: " + LogUtil.TraceData("Filter", filter));
                    result = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_10>>("api/HisTreatment/GetView10", ApiConsumers.MosConsumer, filter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Xong");
                FillDataToGridTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
             {
                List<Task> lst = new List<Task>();
                lst.Add(ProcessSyncTreatment(listTreatmentSync));
                Task.WaitAll(lst.ToArray());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Xử lý Gửi tự động
        private async Task ProcessSyncTreatment(List<V_HIS_TREATMENT_10> listTreatmentSync)
        {
            try
            {
                listMessageError = new List<string>();
                string connect_infor = HisConfigCFG.QD_130_BYT__CONNECTION_INFO;
                string username = null, password = null, address = null, typeXml = null;
                string xml130Api = null, xmlGdykApi = null;
                List<string> connectInfors = new List<string>();
                if (string.IsNullOrEmpty(connect_infor))
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("01 - Lỗi cấu hình hệ thống");
                    return;
                }
                else
                {
                    connectInfors = connect_infor.Split('|').ToList();
                    if (connectInfors.Count < 3 || string.IsNullOrEmpty(connectInfors[0]) || string.IsNullOrEmpty(connectInfors[1]) || string.IsNullOrEmpty(connectInfors[2]))
                    {
                        WaitingManager.Hide();
                        XtraMessageBox.Show("01 - Lỗi cấu hình hệ thống");
                        return;
                    }
                }
                address = connectInfors[0];
                username = connectInfors[1];
                password = connectInfors[2];

                try
                {
                    typeXml = connectInfors[3];
                    xml130Api = connectInfors[4];
                    xmlGdykApi = connectInfors[5];
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error("Key cấu hình hệ thống chỉ thiết lập 3 giá trị");
                }


                Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
                // Lấy danh sách list ID của listTreatmentSync

                if (listTreatmentSync != null && listTreatmentSync.Count > 0)
                {
                    listTreatmentSync = listTreatmentSync.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();

                    //------------------------------test do BackendDataWorker ko có dữ liệu---------------------------------------//
                    var listIds = listTreatmentSync.Select(x => x.ID).ToList();
                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                    filter.TREATMENT_IDs = listIds;
                    var resultPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, param);
                    //------------------------------test--------------------------------------------------------------------------//

                    foreach (var treat in listTreatmentSync)
                    {
                        paramUpdateXml130 = new CommonParam();
                        #region
                        bool sendXml12 = true;
                        var ado = new InputADO
                        {
                            Treatment = treat,
                            Employees = BackendDataWorker.Get<HIS_EMPLOYEE>(), 
                            Branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(b => b.ID == treat.BRANCH_ID),
                            AcsUsers = BackendDataWorker.Get<ACS_USER>(), 
                            Babys = BackendDataWorker.Get<V_HIS_BABY>().Where(b => b.TREATMENT_ID == treat.ID).ToList(),
                            PatientTypeAlter = resultPatientTypeAlter
                                .Where(p => p.TREATMENT_ID == treat.ID)
                                .OrderByDescending(p => p.LOG_TIME)
                                .ThenByDescending(p => p.ID)
                                .FirstOrDefault()
                        };
                        #endregion
                        His.Bhyt.ExportXml.XML3220.CreateXmlMain xmlMain = new His.Bhyt.ExportXml.XML3220.CreateXmlMain(ado);

                        string errorMessPlus = "";
                        MemoryStream resultSyncPlus = null;
                        SyncResultADO syncResult = null;
                        string saveFilePathXml = "";
                        //string errMessage = "";
                        //bool success = false;
                        bool signSuccess = true;
                        int count = 0;
                        resultSyncPlus = xmlMain.RunXml3220Plus(ref errorMessPlus);
                        bool isSuccess = (resultSyncPlus != null && resultSyncPlus.Length > 0);
                        var updateInfo = new
                        {
                            TreatmentId = ado.Treatment.ID,
                            XmlType = 1,
                            XmlResult = isSuccess ? 2 : 1,
                            Description = errorMessPlus,
                            CheckCode = ""
                        };
                        var updateList = new List<object> { updateInfo };
                        //new BackendAdapter(new CommonParam()).Post<bool>("api/HisTreatmentXml/UpdateXmlInfo", ApiConsumers.MosConsumer, updateList, null);
                        His.Bhyt.ExportXml.XML3220.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML3220.CreateXmlProcessor(ado);

                        if (resultSyncPlus != null)
                        {
                            if (this.configSync != null && !string.IsNullOrEmpty(this.configSync.folderPath))
                            {
                                string fullFileName = xmlProcessor.GetFileName();
                                saveFilePathXml = string.Format("{0}/{1}{2}", this.configSync.folderPath, "XML", fullFileName);

                                using (FileStream fs = new FileStream(saveFilePathXml, FileMode.Create, FileAccess.Write))
                                {
                                    resultSyncPlus.WriteTo(fs);
                                }
                                resultSyncPlus.Close();

                                Inventec.Common.Logging.LogSystem.Debug("__Luu XML vao client folder thanh cong. Path: " + saveFilePathXml);

                                if (!isNotFileSign)
                                {
                                    signSuccess = sendXMLSign(xmlProcessor, saveFilePathXml, ref syncResult, xmlMain);

                                    if (!signSuccess)
                                    {
                                        if (File.Exists(saveFilePathXml))
                                        {
                                            try
                                            {
                                                File.Delete(saveFilePathXml);
                                                Inventec.Common.Logging.LogSystem.Warn("Ky so that bai -> da xoa file XML: " + saveFilePathXml);
                                            }
                                            catch (Exception ex)
                                            {
                                                Inventec.Common.Logging.LogSystem.Error("Khong xoa duoc file XML khi ky so loi: " + ex);
                                            }
                                        }
                                        return;
                                    }
                                }
                            }

                            //Nếu có check ký thì ký thành công mới cập nhật trạng thái
                            new BackendAdapter(new CommonParam()).Post<bool>("api/HisTreatmentXml/UpdateXmlInfo", ApiConsumers.MosConsumer, updateList, null);
                        }
                        count++;
                    }
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool sendXMLSign(His.Bhyt.ExportXml.XML3220.CreateXmlProcessor xmlProcessor, string sourceFile, ref SyncResultADO syncResult, His.Bhyt.ExportXml.XML3220.CreateXmlMain xmlMain)
        {
            try
            {
                if (SettingSignADO == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "Không có thông tin cài đặt ký số sendXMLSign"
                    );
                    return false;
                }

                string currentDirectory = Directory.GetCurrentDirectory();
                string tempFolderPath = Path.Combine(currentDirectory, "Temp");
                Directory.CreateDirectory(tempFolderPath);

                string fullFileName = xmlProcessor.GetFileName();
                string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                File.Create(tempFilePath).Close();

                string pathAfterFileSign = null;
                WcfSignDCO wcfSignDCO = null;

                // ====== KÝ FILE ======
                if (SettingSignADO.IsHsm)
                {
                    var xmlBase64 = SourceFileSignApi(ReadFileContent(sourceFile));
                    if (string.IsNullOrEmpty(xmlBase64))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Ký HSM thất bại");
                        return false;
                    }

                    var xmlBytes = Convert.FromBase64String(xmlBase64);
                    File.WriteAllBytes(tempFilePath, xmlBytes);
                    pathAfterFileSign = tempFilePath;
                }
                else
                {
                    wcfSignDCO = new WcfSignDCO
                    {
                        SerialNumber = SettingSignADO.SerialNumber,
                        OutputFile = tempFilePath,
                        PIN = "",
                        SourceFile = sourceFile,
                        fieldSigned = "CHUKYDONVI"
                    };

                    string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                    SignProcessorClient signProcessorClient = new SignProcessorClient();

                    if (!VerifyServiceSignProcessorIsRunning())
                    {
                        return false;
                    }

                    var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                    Inventec.Common.Logging.LogSystem.Debug("wcfSignResultDCO.OutputFile Ky USB: " + Inventec.Common.Logging.LogUtil.TraceData("output file", wcfSignResultDCO.OutputFile));
                    if (wcfSignResultDCO == null || !wcfSignResultDCO.Success)
                    {
                        return false;
                    }

                    pathAfterFileSign = wcfSignResultDCO.OutputFile;
                }

                // ====== GỬI FILE ======
                if (configSync != null && !configSync.dontSend)
                {
                    Inventec.Common.Logging.LogSystem.Info("Gửi file His.Bhyt.ExportXml.XML3220.CreateXmlMain.SendXml3220(pathAfterFileSign): " + pathAfterFileSign);
                    SyncResultADO syncResultADO = null;
                    Task task = Task.Run(async () =>
                        //syncResultADO = await xmlProcessor.SendFileSign(pathAfterFileSign));
                        syncResultADO = await xmlMain.SendXml3220(pathAfterFileSign)
                    );
                    task.Wait();

                    syncResult = syncResultADO;

                    if (syncResult == null || !syncResult.Success)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "Gửi file ký số thất bại: " + syncResult?.Message
                        );
                        return false;
                    }
                }

                // ====== COPY FILE VỀ THƯ MỤC ======
                if (configSync != null && !string.IsNullOrEmpty(configSync.folderPath))
                {
                    if (wcfSignDCO != null)
                    {
                        File.Copy(pathAfterFileSign, wcfSignDCO.SourceFile, true);
                    }
                    else if (SettingSignADO.IsHsm)
                    {
                        File.Copy(pathAfterFileSign, sourceFile, true);
                    }
                }

                // ====== CLEAN TEMP ======
                foreach (string file in Directory.GetFiles(tempFolderPath))
                {
                    File.Delete(file);
                }

                return true; // 🎯 QUAN TRỌNG
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
        private string SourceFileSignApi(string xmlBase64Source)
        {
            string result = null;
            try
            {
                CommonParam param = new CommonParam();
                EMR.SDO.SignXmlBhytSDO signXmlBhytSDO = new EMR.SDO.SignXmlBhytSDO();
                signXmlBhytSDO.XmlBase64 = xmlBase64Source;
                signXmlBhytSDO.TagStoreSignatureValue = "CHUKYDONVI";
                signXmlBhytSDO.ConfigData = new EMR.SDO.XmlConfigDataSDO() { HsmSerialNumber = SettingSignADO.SerialNumber, HsmType = SettingSignADO.Id, HsmUserCode = SettingSignADO.Name, Password = SettingSignADO.Password, SecretKey = SettingSignADO.SercetKey, IdentityNumber = SettingSignADO.CccdNumber };
                Inventec.Common.Logging.LogSystem.Info("Input HSM: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => signXmlBhytSDO), signXmlBhytSDO));
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<string>("api/EmrSign/SignXmlBhyt", ApiConsumer.ApiConsumers.EmrConsumer, signXmlBhytSDO, SessionManager.ActionLostToken, param);
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                {
                    string message = string.Join(Environment.NewLine, param.Messages);
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Inventec.Common.Logging.LogSystem.Warn(message);
                }
                Inventec.Common.Logging.LogSystem.Info("Out HSM: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.1");
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.2");
                        valid = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.3");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeSignPath), exeSignPath));
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {

                            Process.Start(startInfo);
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.4");
                            Thread.Sleep(500);
                            valid = true;
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.5");
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        public string AppFilePathSignService()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }
        private string ReadFileContent(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    try
                    {
                        // 1. Load trực tiếp từ file, .NET sẽ tự xử lý BOM và Encoding chuẩn nhất
                        xmlDocument.Load(filePath);

                        // 2. Đọc tất cả byte một lần duy nhất để chuyển sang Base64
                        byte[] fileBytes = File.ReadAllBytes(filePath);

                        // Bạn nên cân nhắc việc có cần thiết phải dùng hàm StringToBytes trung gian không
                        // Nếu fileBytes đã là chuẩn rồi thì dùng luôn:
                        return Convert.ToBase64String(fileBytes);
                    }
                    catch (XmlException ex)
                    {
                        // Log lỗi tại đây để biết tại sao file XML bị hỏng
                        Console.WriteLine("Lỗi cấu trúc XML: " + ex.Message);
                        throw;
                    }

                    //byte[] fileBytes = File.ReadAllBytes(filePath);
                    //XmlDocument xmlDocument = new XmlDocument();
                    //try
                    //{
                    //    xmlDocument.LoadXml(RemoveByteOrderMark(Encoding.UTF8.GetString(File.ReadAllBytes(filePath))));
                    //    return Convert.ToBase64String(StringToBytes(RemoveByteOrderMark(Encoding.UTF8.GetString(fileBytes))));
                    //}
                    //catch (Exception)
                    //{
                    //    xmlDocument.LoadXml(Encoding.UTF8.GetString(File.ReadAllBytes(filePath)));
                    //    return Convert.ToBase64String(StringToBytes(Encoding.UTF8.GetString(fileBytes)));
                    //}
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        private string RemoveByteOrderMark(string XML)
        {
            string byteOrderMark = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (XML.StartsWith(byteOrderMark))
            {
                XML = XML.Replace(byteOrderMark, "");
                XML = XML.Replace(Environment.NewLine, "");
                XML = XML.Replace(" ", "");
            }
            return XML;
        }
        public byte[] StringToBytes(string input)
        {
            if (input == null) return null;
            return Encoding.UTF8.GetBytes(input);
        }

        private void gridViewTreatment_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                listSelection = new List<V_HIS_TREATMENT_10>();
                var listIndex = gridViewTreatment.GetSelectedRows();
                foreach (var index in listIndex)
                {
                    var treatment = (V_HIS_TREATMENT_10)gridViewTreatment.GetRow(index);
                    if (treatment != null)
                    {
                        listSelection.Add(treatment);
                    }
                }

                gridViewTreatment.BeginDataUpdate();
                gridViewTreatment.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool GenerateXml(ref CommonParam paramExport, ref MemoryStream memoryStream, bool viewXml, bool xuatXmlTT, bool xuatXml12, List<V_HIS_TREATMENT_10> listSelection)
        {
            bool result = false;
            try
            {
                if (listSelection.Count > 0)
                {
                    if (!GlobalConfigStore.IsInit)
                        if (!this.SetDataToLocalXml())
                        {
                            paramExport.Messages.Add(ResourceMessage.KhongThieLapDuocCauHinhDuLieuXuatXml);
                            return result;
                        }
                    GlobalConfigStore.PathSaveXml = txtPathSave.Text;
                    listSelection = listSelection.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();
                    this.NewConfig = GetNewConfig();
                    int skip = 0;
                    while (listSelection.Count - skip > 0)
                    {
                        var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        ListPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
                        HisTreatments = new List<V_HIS_TREATMENT_10>();
                        ListBaby = new List<V_HIS_BABY>();

                        string message = "";
                        isExportXml = true;

                        CreateThreadGetData(limit);
                        isExportXml = false;
                        if (chkSignFileCertUtil.Checked == false)
                        {
                            isNotFileSign = true;
                            //qtcode
                            message = ProcessExportXmlDetail(ref result, ref memoryStream, viewXml, HisTreatments, ListPatientTypeAlter, ListBaby);
                        }
                        else
                        {
                            if (SettingSignADO != null && string.IsNullOrEmpty(SettingSignADO.SerialNumber) || SettingSignADO == null)
                            {
                                if (XtraMessageBox.Show("Không có thông tin Serial chứng thư ký số. Bạn có muốn tiếp tục xuất xml?", Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    message = "";
                                }
                                else
                                {
                                    isNotFileSign = true;
                                    message = ProcessExportXmlDetail(ref result, ref memoryStream, viewXml, HisTreatments, ListPatientTypeAlter, ListBaby);
                                }
                            }
                            else
                            {
                                isNotFileSign = false;
                                message = ProcessExportXmlDetail(ref result, ref memoryStream, viewXml, HisTreatments, ListPatientTypeAlter, ListBaby);
                            }
                        }
                        if (!String.IsNullOrEmpty(message))
                        {
                            paramExport.Messages.Add(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        private List<HIS_CONFIG> GetNewConfig()
        {
            List<HIS_CONFIG> result = null;
            try
            {
                CommonParam paramGet = new CommonParam();
                MOS.Filter.HisConfigFilter configFilter = new MOS.Filter.HisConfigFilter();
                configFilter.IS_ACTIVE = 1;
                result = new BackendAdapter(paramGet).Get<List<MOS.EFMODEL.DataModels.HIS_CONFIG>>("/api/HisConfig/Get", ApiConsumers.MosConsumer, configFilter, paramGet);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void CreateThreadGetData(List<V_HIS_TREATMENT_10> listSelection)
        {
            try
            {
                System.Threading.Thread PatientTypeAlter = new System.Threading.Thread(ThreadGetPatientTypeAlter);
                System.Threading.Thread Baby = new System.Threading.Thread(ThreadGetBaby);
                System.Threading.Thread Treatment12 = new System.Threading.Thread(ThreadGetTreatment12);
                try
                {
                    PatientTypeAlter.Start(listSelection);
                    Baby.Start(listSelection);
                    Treatment12.Start(listSelection);
                    PatientTypeAlter.Join();
                    Baby.Join();
                    Treatment12.Join();
                }
                catch (Exception ex)
                {
                    PatientTypeAlter.Abort();
                    Baby.Abort();
                    Treatment12.Abort();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetPatientTypeAlter(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_10> listSelection = (List<V_HIS_TREATMENT_10>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                    filter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resultPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (resultPatientTypeAlter != null && resultPatientTypeAlter.Count > 0)
                    {
                        ListPatientTypeAlter.AddRange(resultPatientTypeAlter);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ThreadGetBaby(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_10> listSelection = (List<V_HIS_TREATMENT_10>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();
                    HisBabyViewFilter filter = new HisBabyViewFilter();
                    filter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resultBaby = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (resultBaby != null && resultBaby.Count > 0)
                    {
                        ListBaby.AddRange(resultBaby);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ThreadGetTreatment12(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_10> listSelection = (List<V_HIS_TREATMENT_10>)obj;

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();
                    HisTreatmentView12Filter treatmentFilter = new HisTreatmentView12Filter();
                    treatmentFilter.IDs = limit.Select(o => o.ID).ToList();
                    var resultTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_10>>("api/HisTreatment/GetView10", ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (resultTreatment != null && resultTreatment.Count > 0)
                    {
                        HisTreatments.AddRange(resultTreatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UpdateSavePath(SavePathADO savePathADO)
        {
            try
            {
                if (savePathADO == null)
                    savePathADO = new SavePathADO();
                this.savePathADO = savePathADO;
                string value = Newtonsoft.Json.JsonConvert.SerializeObject(this.savePathADO);
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == txtPathSave.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = txtPathSave.Name;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
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
        private async Task XML130()
        {
            try
            {
                if (btnAutoSyncClick == true && configSync.isXML3176 == true)
                {
                    listSelection = this.GetTreatment();
                }
                if ((listSelection == null || listSelection.Count == 0))
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.BanChuaChonHoSoDeDongBo, Resources.ResourceMessageLang.ThongBao);
                    return;
                }
                var listTreatmentSynced = listSelection.Where(o => o.TREATMENT_XML_TYPE == 1).ToList();
                if (listTreatmentSynced != null && listTreatmentSynced.Count > 0 && listTreatmentSynced.Any(o => o.TREATMENT_XML_RESULT == 2))
                {
                    if (XtraMessageBox.Show(String.Format(Resources.ResourceMessageLang.CacHoSoDaDongBoThanhCongBanCoMuonDongBoLai, String.Join(", ", listTreatmentSynced.Where(o => o.TREATMENT_XML_RESULT == 2).Select(o => o.TREATMENT_CODE).ToList())), Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }

                var listTreatmentxml3176 = listSelection.Where(o => o.XML130_RESULT == 1).ToList();
                if (listTreatmentxml3176 != null && listTreatmentxml3176.Count > 0 && showMessSusscess == true)
                {
                    if (XtraMessageBox.Show(String.Format("Các hồ sơ {0} đã gửi thành công bạn có muốn gửi lại?", String.Join(", ", listTreatmentxml3176.Select(o => o.TREATMENT_CODE).ToList())), Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }
                if (chkSignFileCertUtil.Checked == true)
                {
                    if (SettingSignADO == null || (SettingSignADO != null && string.IsNullOrEmpty(SettingSignADO.SerialNumber)))
                    {
                        MessageBox.Show("Không có thông tin Usb Token ký số");
                        return;
                    }
                    else
                    {
                        WaitingManager.Show();
                        callSyncSuccess = false;
                        isSendCollinearXml = false;
                        await ProcessSyncTreatment(listSelection);
                    }
                }
                else
                {
                    //qtcode
                    isNotFileSign = true;
                    //qtcode
                    WaitingManager.Show();
                    callSyncSuccess = false;
                    isSendCollinearXml = false;
                    await ProcessSyncTreatment(listSelection);
                }
                if (callSyncSuccess)
                {
                    if (listMessageError != null && listMessageError.Count > 0)
                    {
                        listMessageError = listMessageError.Distinct().ToList();
                        if (paramUpdateXml130.Messages != null && paramUpdateXml130.Messages.Count > 0)
                        {
                            listMessageError.AddRange(paramUpdateXml130.Messages);
                        }
                        XtraMessageBox.Show(Resources.ResourceMessage.XuLyThatBai + String.Join("\r\n", listMessageError), Resources.ResourceMessage.ThongBao);
                    }
                    else if (paramUpdateXml130.Messages != null && paramUpdateXml130.Messages.Count > 0)
                    {
                        MessageManager.Show(this.ParentForm, paramUpdateXml130, false);
                    }
                    else
                        MessageManager.Show(this.ParentForm, paramUpdateXml130, true);

                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Thay chức năng xuất thành chức năng xem xml
        private void gridViewTreatment_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        var treatment1 = (V_HIS_TREATMENT_10)gridViewTreatment.GetRow(hi.RowHandle);
                        if (treatment1 != null)
                        {
                            if (hi.Column.FieldName == "EXPORT_XML")
                            {
                                isNotFileSign = true;
                                CommonParam param = new CommonParam();
                                MemoryStream memoryStream = new MemoryStream();
                                MemoryStream memoryStreamXml12 = new MemoryStream();
                                bool success = false;
                                WaitingManager.Show();
                                List<V_HIS_TREATMENT_10> listTreatments = new List<V_HIS_TREATMENT_10>();
                                listTreatments.Add(treatment1);
                                Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click Begin");
                                success = this.GenerateXml(ref param, ref memoryStream, true, false, false, listTreatments);
                                isNotFileSign = false;
                                Inventec.Common.Logging.LogSystem.Info("btnExportXml_Click End");
                                WaitingManager.Hide();
                                if (success && param.Messages.Count == 0)
                                {
                                    MessageManager.Show(this.ParentForm, param, success);
                                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer130").FirstOrDefault();
                                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer130'");
                                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                    {
                                        moduleData.RoomId = this.currentModule.RoomId;
                                        moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                                        List<object> listArgs = new List<object>();
                                        long dataType = 3;
                                        if (memoryStream != null)
                                            listArgs.Add(memoryStream);
                                        else
                                        {
                                            DevExpress.XtraEditors.XtraMessageBox.Show("Lỗi tạo xml");
                                            return;
                                        }
                                        listArgs.Add(moduleData);
                                        listArgs.Add(dataType);
                                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                                        if (extenceInstance == null)
                                        {
                                            throw new ArgumentNullException("moduleData is null");
                                        }

                                        ((Form)extenceInstance).ShowDialog();
                                    }
                                    else
                                    {
                                        MessageManager.Show(Resources.ResourceMessageLang.ChucNangChuaHoTroPhienBanHienTai);
                                    }
                                }
                                else if (!success && param.Messages.Count > 0)
                                {
                                    MessageManager.Show(param, success);
                                }

                                this.gridControlTreatment.RefreshDataSource();

                                SessionManager.ProcessTokenLost(param);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                await XML130();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
