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
using Bartender.PrintClient;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout.Utils;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ServiceReqList.ADO;
using HIS.Desktop.Plugins.ServiceReqList.Base;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using HIS.Desktop.Plugins.ServiceReqList.Reason;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmServiceReqList : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        string loginName = null;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;

        V_HIS_ROOM currentRoom;

        List<HIS_SERVICE_REQ_TYPE> serviceReqTypeSelecteds;
        List<HIS_SERVICE_REQ_STT> serviceReqSttSelecteds;
        ADO.ServiceReqADO currentServiceReqPrint;
        ADO.ServiceReqADO currentServiceReq;
        V_HIS_SERVICE_REQ serviceReqPrintRaw;
        V_HIS_PATIENT currentPatient = null;
        HIS_TREATMENT treatment = null;
        HIS_EXP_MEST prescriptionPrint;
        HIS_EXP_MEST currentPrescription;

        Inventec.Desktop.Common.Modules.Module currentModule;
        MOS.SDO.WorkPlaceSDO currentWorkPlace;

        PrintPopupMenuProcessor PrintPopupMenuProcessor;

        List<ADO.ServiceReqADO> listServiceReq;
        List<ADO.ListMedicineADO> _listMedicine;

        bool isCheckAll = true;
        List<HIS_RATION_TIME> lsRationTime;
        HIS_SERE_SERV sereServPrint;
        ListMedicineADO rightClickData;
        string treatmentCode = "";

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.ServiceReqList";
        private HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeAlter;
        Dictionary<string, object> dicParam;
        Dictionary<string, Image> dicImage;

        internal List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders;
        List<V_HIS_SERE_SERV_TEIN> lstSereServTein;
        HIS_SERE_SERV_EXT sereServExtPrint = null;
        internal V_HIS_SERE_SERV_4 sereServ;
        List<string> keyPrint = new List<string>() { "<#CONCLUDE_PRINT;>", "<#NOTE_PRINT;>", "<#DESCRIPTION_PRINT;>", "<#CURRENT_USERNAME_PRINT;>" };
        List<long> ConfigIds = new List<long>();
        List<ConfigADO> lstConfig;
        #endregion

        #region Construct

        public frmServiceReqList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.gridControlServiceReq.ToolTipController = this.tooltipServiceRequest;

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmServiceReqList = new ResourceManager("HIS.Desktop.Plugins.ServiceReqList.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceReqList.frmServiceReqList).Assembly);

                this.currentModule = module;
                this.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmServiceReqList(Inventec.Desktop.Common.Modules.Module module, HIS_TREATMENT data)
            : this(module)
        {
            try
            {
                this.treatment = data;
                this.currentModule = module;
                this.Text = module.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmServiceReqList(Inventec.Desktop.Common.Modules.Module module, V_HIS_PATIENT data)
            : this(module)
        {
            try
            {
                this.currentPatient = data;
                this.currentModule = module;
                this.Text = module.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        #region Load
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

        private void frmServiceReqList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                isNotLoadWhileChangeControlStateInFirst = true;
                SetCaptionByLanguageKey();
                HisConfigCFG.LoadConfig();
                this.currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                if (treatment != null)
                {
                    this.treatment = LoadDataToCurrentTreatmentData(this.treatment.ID);
                }
                LoadDataCboFilterType();
                SetPrintTypeToMps();
                LoadComboExcuteRoom();
                InitControlState();
                SetDefaultValueControl();
                FillDataToGrid();
                GeneratePopupMenu();
                InitListConfig();
                Gc_HisSendOldSystem.Visible = HisConfigCFG.IsOldSystemIntegration;
                LoadDataRationTime();
                isNotLoadWhileChangeControlStateInFirst = false;
                VisibleColumnPresAmount();
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Info("end load");

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void VisibleColumnPresAmount()
        {
            try
            {
                if (!HisConfigCFG.IsShowPresAmount)
                {
                    gridColSerSevPresAmount.VisibleIndex = -1;
                }
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
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPK.Name)
                        {
                            chkPK.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == gridConfig.Name)
                        {
                            if (!string.IsNullOrEmpty(item.VALUE))
                            {
                                var lstStr = item.VALUE.Split(';').ToList();
                                foreach (var str in lstStr)
                                {
                                    ConfigIds.Add(Int64.Parse(str));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadComboExcuteRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExecuteRoomFilter filter = new HisExecuteRoomFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboExecuteRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboExcuteRoom(bool check)
        {
            try
            {
                if (check)
                {
                    CommonParam param = new CommonParam();
                    HisExecuteRoomFilter filter = new HisExecuteRoomFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IS_EXAM = check;
                    var data = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null).ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 200, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(cboExecuteRoom, data, controlEditorADO);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataRationTime()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisRationTimeFilter filter = new HisRationTimeFilter();
                this.lsRationTime = new BackendAdapter(param).Get<List<HIS_RATION_TIME>>("api/HisRationTime/Get", ApiConsumers.MosConsumer, filter, param);
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
                ////Caption Frm
                //if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                //{
                //    this.Text = this.currentModule.text;
                //}

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.cboServiceReqType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmServiceReqList.cboServiceReqType.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.cboServiceReqStt.Properties.NullText = Inventec.Common.Resource.Get.Value("frmServiceReqList.cboServiceReqStt.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevSTT.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevSTT.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevView.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevView.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevPrint.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevPrint.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gcolServiceTypeName.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gcolServiceTypeName.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevName.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevName.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevAmount.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevAmount.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevUnitName.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevUnitName.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevTypeName.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevTypeName.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColOtherPrintForm.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColOtherPrintForm.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.txtServiceReqCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmServiceReqList.txtServiceReqCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmServiceReqList.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.btnFind.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Stt.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_Stt.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_ServiceReq_Edit.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_ServiceReq_Edit.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_ServiceReq_Delete.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_ServiceReq_Delete.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_ServiceReq_Print.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_ServiceReq_Print.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_ServiceReq_Stt.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_ServiceReq_Stt.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.repositoryItempicServiceReqStatus.NullText = Inventec.Common.Resource.Get.Value("frmServiceReqList.repositoryItempicServiceReqStatus.NullText", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransactionCode.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_TransactionCode.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Amount.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_Amount.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Request_Username.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_Cashier.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_CashierRoomName.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_CashierRoomName.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_PayFormName.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_PayFormName.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Dob.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_Dob.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());

                //qtcode
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                //qtcode

                this.gridColumn_Transaction_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_TreatmentCode.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_VirPatientName.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_VirPatientName.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_GenderName.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_GenderName.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Execute_Username.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Execute_Username.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_CreateTime.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_CreateTime.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Creator.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_Creator.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_ModifyTime.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Modifier.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_Modifier.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.grdColRationTime.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.grdColRationTime.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciServiceReqCode.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciServiceReqCode.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciKeyword.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciKeyword.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.bar1.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.bbtnRCFind.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.bbtnRCFind.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.bbtnRCRefresh.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.bbtnRCRefresh.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciIntructionTimeFrom.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciIntructionTimeFrom.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciIntructionTimeTo.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciIntructionTimeTo.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());

                this.lciAggrExpMestCode.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciAggrExpMestCode.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnAggrExpMest.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciBtnAggrExpMest.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciExpMestCode.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciExpMestCode.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciExpMestRoom.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciExpMestRoom.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciGender.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciGender.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciPatientName.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciTreatmentCode.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciTreatmentCode.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnMobaCreate.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.btnMobaCreate.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.groupControlInfo.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.groupControlInfo.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciExpMestStt.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciExpMestStt.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciExcuteDepartment.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciExcuteDepartment.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciSoTheTM.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciSoTheTM.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());

                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmServiceReqList.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.repositoryItemBtnBieuMauKhac.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.repositoryItemBtnBieuMauKhac.Buttons[0].ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.repositoryItemBtnDeleteServiceReq.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.repositoryItemBtnDeleteServiceReq.Buttons[0].ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                //this.repositoryItemBtnDeleteServiceReqDisable.Buttons[0].ToolTip = this.repositoryItemBtnDeleteServiceReq.Buttons[0].ToolTip;
                this.repositoryItemBtnEditServiceReq.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.repositoryItemBtnEditServiceReq.Buttons[0].ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                //this.repositoryItemBtnEditServiceReqDisable.Buttons[0].ToolTip = this.repositoryItemBtnEditServiceReq.Buttons[0].ToolTip;
                this.repositoryItemBtnPrintServiceReq.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.repositoryItemBtnPrintServiceReq.Buttons[0].ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                //this.repositoryItemBtnPrintServiceReqDisable.Buttons[0].ToolTip = this.repositoryItemBtnPrintServiceReq.Buttons[0].ToolTip;
                this.repositoryItemButtonEditIntructionTime.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.repositoryItemButtonEditIntructionTime.Buttons[0].ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.repositoryItemButtonPrint.Buttons[0].ToolTip = this.repositoryItemBtnPrintServiceReq.Buttons[0].ToolTip;
                this.repositoryItemButtonView.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.repositoryItemButtonView.Buttons[0].ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());

                this.lciReqDepartment.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciReqDepartment.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciRationTime.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciRationTime.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                //
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciReqSended.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciReqSended.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciReqSended.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciReqSended.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciExpMestRoom.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciExpMestRoom.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciIsKidney.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciIsKidney.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciIsHomePres.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciIsHomePres.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciRationSumCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciRationSumCode.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciRationSumCode.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciRationSumCode.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnDropDownPrint.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.btnDropDownPrint.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnPrintMedicine.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.btnPrintMedicine.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnPrintTotal.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.btnPrintTotal.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnPrintTemBarcode.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.btnPrintTemBarcode.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Transaction_PatientCode.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Transaction_PatientCode.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmServiceReqList.txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.txtStoreCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmServiceReqList.txtStoreCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());

                this.lciAssignTurnCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciAssignTurnCode.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciAssignTurnCode.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciAssignTurnCode.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevCode.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevCode.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevPatientTypeName.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevPatientTypeName.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevPatientTypeName.ToolTip = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevPatientTypeName.ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevConvertRatio.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevConvertRatio.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevConvertAmount.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevConvertAmount.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColSerSevConvertName.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColSerSevConvertName.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.Gc_HisSendOldSystem.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.Gc_HisSendOldSystem.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciTestSampleTypeName.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciTestSampleTypeName.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.gridColumn_Pttt_Group_Name.Caption = Inventec.Common.Resource.Get.Value("frmServiceReqList.gridColumn_Pttt_Group_Name.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboFilterType()
        {
            try
            {
                List<ADO.FilterTypeADO> listFilterType = new List<ADO.FilterTypeADO>();

                listFilterType.Add(new ADO.FilterTypeADO(0, new Base.GlobalStore().TOI_TAO));
                listFilterType.Add(new ADO.FilterTypeADO(1, new Base.GlobalStore().PHONG_CHI_DINH));
                listFilterType.Add(new ADO.FilterTypeADO(2, new Base.GlobalStore().KHOA_CHI_DINH));
                listFilterType.Add(new ADO.FilterTypeADO(3, new Base.GlobalStore().KHOA_THUC_HIEN));
                listFilterType.Add(new ADO.FilterTypeADO(4, new Base.GlobalStore().TAT_CA));

                cboFilter.Properties.DataSource = listFilterType;
                cboFilter.Properties.DisplayMember = "FilterTypeName";
                cboFilter.Properties.ValueMember = "ID";
                cboFilter.Properties.ForceInitialize();
                cboFilter.Properties.Columns.Clear();
                cboFilter.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FilterTypeName", "", 200));
                cboFilter.Properties.ShowHeader = false;
                cboFilter.Properties.ImmediatePopup = true;
                cboFilter.Properties.DropDownRows = 5;
                cboFilter.Properties.PopupWidth = 220;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                InitServiceReqTypeCheck();
                InitServiceReqSttCheck();
                InitComboServiceReqType();
                InitComboServiceReqStt();

                cboFilter.EditValue = (long)0;
                txtKeyword.Text = "";
                dtIntructionTimeFrom.DateTime = DateTime.Now;
                dtIntructionTimeTo.DateTime = DateTime.Now;

                //load mặc định tôi tạo, phòng chỉ định
                string filterType = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Base.ConfigKey.Filter_Type_For_Treatment_Patient);
                long filterCboValue = Inventec.Common.TypeConvert.Parse.ToInt64(filterType);
                if (this.treatment != null)
                {
                    txtTreatmentCode.Text = this.treatment.TREATMENT_CODE;
                    cboFilter.EditValue = filterCboValue > 0 && filterCboValue <= 5 ? filterCboValue - 1 : (long)2;
                    dtIntructionTimeFrom.EditValue = null;
                    dtIntructionTimeTo.EditValue = null;
                }
                if (this.currentPatient != null)
                {
                    cboFilter.EditValue = filterCboValue > 0 && filterCboValue <= 5 ? filterCboValue - 1 : (long)2;
                    dtIntructionTimeFrom.EditValue = null;
                }

                currentWorkPlace = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == currentModule.RoomId && o.RoomTypeId == currentModule.RoomTypeId);

                SetVisibleControl(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_TREATMENT LoadDataToCurrentTreatmentData(long treatmentId)
        {
            MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                filter.ID = treatmentId;

                var listTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    treatment = listTreatment[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        private void InitServiceReqTypeCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboServiceReqType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ServiceReqType);
                cboServiceReqType.Properties.Tag = gridCheck;
                cboServiceReqType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboServiceReqType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboServiceReqType.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__ServiceReqType(object sender, EventArgs e)
        {
            try
            {
                serviceReqTypeSelecteds = new List<HIS_SERVICE_REQ_TYPE>();
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        serviceReqTypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitServiceReqSttCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboServiceReqStt.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ServiceReqStt);
                cboServiceReqStt.Properties.Tag = gridCheck;
                cboServiceReqStt.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboServiceReqStt.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboServiceReqStt.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__ServiceReqStt(object sender, EventArgs e)
        {
            try
            {
                serviceReqSttSelecteds = new List<HIS_SERVICE_REQ_STT>();
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        serviceReqSttSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetPrintTypeToMps()
        {
            try
            {
                if (MPS.PrintConfig.PrintTypes == null || MPS.PrintConfig.PrintTypes.Count == 0)
                {
                    MPS.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR_PRINT_TYPE>();
                }
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
                int pagingSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                FillDataToGridTransaction(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTransaction, param, pagingSize, this.gridControlServiceReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTransaction(object param)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 1");
                WaitingManager.Show();
                List<ADO.ServiceReqADO> listData = new List<ADO.ServiceReqADO>();
                gridControlServiceReq.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisServiceReqFilter filter = new HisServiceReqFilter();
                SetFilter(ref filter);
                Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 2");
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<HIS_SERVICE_REQ>>((HisConfigCFG.IsUseGetDynamic ? RequestUriStore.HIS_SERVICE_REQ_GET_DYNAMIC : RequestUriStore.HIS_SERVICE_REQ_GET), ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 3");
                if (result != null)
                {
                    rowCount = (result.Data == null ? 0 : result.Data.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    if (result.Data != null && result.Data.Count > 0)
                    {
                        foreach (var item in result.Data)
                        {
                            ADO.ServiceReqADO ado = new ADO.ServiceReqADO(item);
                            listData.Add(ado);
                        }
                    }
                    else
                    {
                        listData = null;
                    }
                    this.gridColumn_ServiceReq_Choose.Image = this.imageListCheck.Images[4];
                }

                gridControlServiceReq.BeginUpdate();
                gridControlServiceReq.DataSource = listData;
                gridControlServiceReq.EndUpdate();

                grdSereServServiceReq.BeginUpdate();
                grdSereServServiceReq.DataSource = null;
                grdSereServServiceReq.EndUpdate();
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 4");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisServiceReqFilter filter)
        {
            try
            {
                bool IsNotDate = false;
                bool IsNotServiceReq = false;
                if (filter == null) filter = new HisServiceReqFilter();
                filter.ORDER_FIELD = "INTRUCTION_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD1 = "SERVICE_REQ_CODE";
                filter.ORDER_DIRECTION1 = "DESC";
                if (HisConfigCFG.IsUseGetDynamic)
                {
                    filter.ColumnParams = new List<string>()
                    {
                        "BARCODE",
                        "SESSION_CODE",
                        "CALL_COUNT",
                        "CALL_SAMPLE_ORDER",
                        "CREATE_TIME",
                        "CREATOR",
                        "DESCRIPTION",
                        "DHST_ID",
                        "REMEDY_COUNT",
                        "USE_TIME",
                        "USE_TIME_TO",
                        "TRACKING_ID",
                        "KIDNEY_TIMES",
                        "EXE_SERVICE_MODULE_ID",
                        "EXECUTE_DEPARTMENT_ID",
                        "EXECUTE_GROUP_ID",
                        "EXECUTE_LOGINNAME",
                        "EXECUTE_USERNAME",
                        "EXECUTE_ROOM_ID",
                        "EXP_MEST_TEMPLATE_ID",
                        "FINISH_TIME",
                        "ADVISE",
                        "JSON_PRINT_ID",
                        "ECG_AFTER",
                        "ECG_BEFORE",
                        "ICD_CAUSE_NAME",
                        "ICD_CAUSE_CODE",
                        "ICD_CODE",
                        "ICD_NAME",
                        "ICD_SUB_CODE",
                        "ICD_TEXT",
                        "ID",
                        "INTRUCTION_DATE",
                        "INTRUCTION_TIME",
                        "IS_ACTIVE",
                        "IS_EMERGENCY",
                        "IS_EXECUTE_KIDNEY_PRES",
                        "IS_HOME_PRES",
                        "IS_KIDNEY",
                        "IS_NO_EXECUTE",
                        "IS_NOT_REQUIRE_FEE",
                        "IS_WAIT_CHILD",
                        "MACHINE_ID",
                        "MODIFIER",
                        "MODIFY_TIME",
                        "NUM_ORDER",
                        "PRIORITY",
                        "REQUEST_DEPARTMENT_ID",
                        "REQUEST_LOGINNAME",
                        "REQUEST_USERNAME",
                        "REQUEST_ROOM_ID",
                        "SERVICE_REQ_CODE",
                        "SERVICE_REQ_STT_ID",
                        "SERVICE_REQ_TYPE_ID",
                        "START_TIME",
                        "IS_INTEGRATE_HIS_SENT",
                        "TREATMENT_ID",
                        "TREATMENT_TYPE_ID",
                        "TDL_TREATMENT_CODE",
                        "TDL_PATIENT_NAME",
                        "SERVICE_REQ_TYPE_NAME",
                        "EXECUTE_ROOM_NAME",
                        "REQUEST_ROOM_NAME",
                        "IS_MAIN_EXAM",
                        "TDL_PATIENT_GENDER_ID",
                        "TDL_PATIENT_DOB",
                        "TDL_PATIENT_ID",
                        "TDL_PATIENT_GENDER_NAME",
                        "ATTACHMENT_FILE_URL",
                        "LIS_STT_ID",
                        "IS_SENT_EXT",
                        "PRESCRIPTION_TYPE_ID",
                        "TDL_SERVICE_TYPE_ID",
                        "IS_ANTIBIOTIC_RESISTANCE",
                        "RATION_TIME_ID"
                    };
                    filter.ColumnParams = filter.ColumnParams.Distinct().ToList();
                }


                if (!String.IsNullOrEmpty(txtStoreCode.Text.Trim()))
                {
                    if (!String.IsNullOrEmpty(txtServiceReqCode.Text.Trim()) && !String.IsNullOrEmpty(txtTreatmentCode.Text.Trim()))
                    {
                        IsNotServiceReq = true;
                        string codeServiceReq = txtServiceReqCode.Text.Trim();
                        string codeTreatment = txtTreatmentCode.Text.Trim();
                        if (codeServiceReq.Length < 12 && checkDigit(codeServiceReq))
                        {
                            codeServiceReq = string.Format("{0:000000000000}", Convert.ToInt64(codeServiceReq));
                            txtServiceReqCode.Text = codeServiceReq;
                        }
                        filter.SERVICE_REQ_CODE__EXACT = codeServiceReq;

                        if (codeTreatment.Length < 12 && checkDigit(codeTreatment))
                        {
                            codeTreatment = string.Format("{0:000000000000}", Convert.ToInt64(codeTreatment));
                            txtTreatmentCode.Text = codeTreatment;
                        }
                        filter.TREATMENT_CODE__EXACT = codeTreatment;

                    }
                    else
                    {
                        IsNotDate = true;
                        CommonParam param = new CommonParam();
                        HisTreatmentFilter filterTreatment = new HisTreatmentFilter();
                        filterTreatment.STORE_CODE__EXACT = txtStoreCode.Text.Trim();
                        var result = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filterTreatment, param);
                        List<long> lstTreatmentId = new List<long>();
                        foreach (var item in result)
                        {
                            lstTreatmentId.Add(item.ID);
                        }
                        filter.TREATMENT_IDs = lstTreatmentId;
                    }
                }
                if (!String.IsNullOrEmpty(txtServiceReqCode.Text.Trim()))
                {
                    string codeServiceReq = txtServiceReqCode.Text.Trim();
                    if (codeServiceReq.Length < 12 && checkDigit(codeServiceReq))
                    {
                        codeServiceReq = string.Format("{0:000000000000}", Convert.ToInt64(codeServiceReq));
                        txtServiceReqCode.Text = codeServiceReq;
                    }
                    filter.SERVICE_REQ_CODE__EXACT = codeServiceReq;
                }
                else
                {
                    IsNotDate = true;
                    #region
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text.Trim()))
                    {
                        string codeTreatment = txtTreatmentCode.Text.Trim();
                        if (codeTreatment.Length < 12 && checkDigit(codeTreatment))
                        {
                            codeTreatment = string.Format("{0:000000000000}", Convert.ToInt64(codeTreatment));
                            txtTreatmentCode.Text = codeTreatment;
                        }
                        filter.TREATMENT_CODE__EXACT = codeTreatment;

                    }
                    if (!string.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        string patientCode = txtPatientCode.Text.Trim();
                        if (patientCode.Length < 10)
                        {
                            patientCode = string.Format("{0:0000000000}", Convert.ToInt64(patientCode));
                            txtPatientCode.Text = patientCode;
                        }
                        filter.TDL_PATIENT_CODE__EXACT = patientCode;
                    }
                    #endregion

                }

                if (IsNotDate)
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                    if (serviceReqTypeSelecteds != null && serviceReqTypeSelecteds.Count > 0)
                    {
                        filter.SERVICE_REQ_TYPE_IDs = serviceReqTypeSelecteds.Select(o => o.ID).ToList();
                    }

                    if (serviceReqSttSelecteds != null && serviceReqSttSelecteds.Count > 0)
                    {
                        filter.SERVICE_REQ_STT_IDs = serviceReqSttSelecteds.Select(o => o.ID).ToList();
                    }

                    int value = Convert.ToInt32(cboFilter.EditValue);

                    if (value == 0)//tôi tạo
                    {
                        filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    }
                    else if (value == 1) // phòng làm việc
                    {
                        if (currentModule != null && currentModule.RoomId > 0)
                            filter.REQUEST_ROOM_ID = currentModule.RoomId;
                    }
                    else if (value == 2) //Khoa chỉ định
                    {
                        filter.REQUEST_DEPARTMENT_ID = currentWorkPlace.DepartmentId;
                    }
                    else if (value == 3) // Khoa thực hiện
                    {
                        filter.EXECUTE_DEPARTMENT_ID = currentWorkPlace.DepartmentId;
                    }
                    if (currentPatient != null && currentPatient.ID > 0)
                    {
                        filter.TDL_PATIENT_ID = currentPatient.ID;
                    }


                    if (dtIntructionTimeFrom.EditValue != null && dtIntructionTimeFrom.DateTime != DateTime.MinValue)
                        filter.INTRUCTION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtIntructionTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtIntructionTimeTo.EditValue != null && dtIntructionTimeTo.DateTime != DateTime.MinValue)
                        filter.INTRUCTION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtIntructionTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                    if (cboExecuteRoom.EditValue != null && cboExecuteRoom.EditValue.ToString() != "")
                    {
                        filter.EXECUTE_ROOM_ID = (long)cboExecuteRoom.EditValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = true;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == false) return false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void InitComboServiceReqStt()
        {
            try
            {
                var datas = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>();
                if (datas != null)
                {
                    cboServiceReqStt.Properties.DataSource = datas;
                    cboServiceReqStt.Properties.DisplayMember = "SERVICE_REQ_STT_NAME";
                    cboServiceReqStt.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboServiceReqStt.Properties.View.Columns.AddField("SERVICE_REQ_STT_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    cboServiceReqStt.Properties.PopupFormWidth = 200;
                    cboServiceReqStt.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboServiceReqStt.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboServiceReqStt.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboServiceReqStt.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboServiceReqType()
        {
            try
            {
                var datas = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>();
                if (datas != null)
                {
                    cboServiceReqType.Properties.DataSource = datas;
                    cboServiceReqType.Properties.DisplayMember = "SERVICE_REQ_TYPE_NAME";
                    cboServiceReqType.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboServiceReqType.Properties.View.Columns.AddField("SERVICE_REQ_TYPE_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    cboServiceReqType.Properties.PopupFormWidth = 200;
                    cboServiceReqType.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboServiceReqType.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboServiceReqType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboServiceReqType.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillDataGridDetail(HIS_EXP_MEST dataExpMest, ServiceReqADO serviceClick)
        {
            List<ADO.ListMedicineADO> listMedicine = new List<ADO.ListMedicineADO>();
            try
            {
                CommonParam paramCommon = new CommonParam();
                #region mau
                if (serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM)
                {
                    if (dataExpMest.ID > 0)
                    {
                        HisExpMestBltyReqFilter bltyFilter = new HisExpMestBltyReqFilter();
                        bltyFilter.EXP_MEST_ID = dataExpMest.ID;
                        var bloods = await new BackendAdapter(paramCommon).GetAsync<List<HIS_EXP_MEST_BLTY_REQ>>(RequestUriStore.HIS_EXP_MEST_BLTY_REQ_GET, ApiConsumers.MosConsumer, bltyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (bloods != null && bloods.Count > 0)
                        {
                            var expMestBltyGroups = bloods.GroupBy(o => o.BLOOD_TYPE_ID).ToList();
                            foreach (var expMestBltyGroup in expMestBltyGroups)
                            {
                                ADO.ListMedicineADO bltyExpmestTypeADO = new ADO.ListMedicineADO();

                                bltyExpmestTypeADO.NUM_ORDER = expMestBltyGroup.First().NUM_ORDER ?? 999999;
                                bltyExpmestTypeADO.AMOUNT = expMestBltyGroup.Sum(o => o.AMOUNT);
                                bltyExpmestTypeADO.CREATE_TIME = expMestBltyGroup.First().CREATE_TIME;
                                bltyExpmestTypeADO.type = 2;
                                //bltyExpmestTypeADO.kind = 0;

                                var blty = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(o => o.ID == expMestBltyGroup.First().BLOOD_TYPE_ID);
                                if (blty != null)
                                {
                                    bltyExpmestTypeADO.TDL_SERVICE_NAME = blty.BLOOD_TYPE_NAME;
                                    bltyExpmestTypeADO.TDL_SERVICE_CODE = blty.BLOOD_TYPE_CODE;
                                    bltyExpmestTypeADO.SERVICE_UNIT_NAME = blty.SERVICE_UNIT_NAME;
                                }
                                listMedicine.Add(bltyExpmestTypeADO);
                            }
                        }
                    }
                }
                #endregion
                #region don pk, tt, exp_mest
                else if (serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                        serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                        serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                {
                    if (dataExpMest.ID > 0)
                    {
                        paramCommon = new CommonParam();
                        HisExpMestMedicineFilter mediFilter = new HisExpMestMedicineFilter();
                        mediFilter.EXP_MEST_ID = dataExpMest.ID;
                        var medicines = await new BackendAdapter(paramCommon).GetAsync<List<HIS_EXP_MEST_MEDICINE>>(RequestUriStore.HIS_EXP_MEST_MEDICINE_GET, ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (medicines != null && medicines.Count > 0)
                        {
                            var expMestMetyGroups = medicines.GroupBy(o => new { o.TDL_MEDICINE_TYPE_ID, o.TUTORIAL, o.HTU_TEXT}).ToList();
                            foreach (var expMestMetyGroup in expMestMetyGroups)
                            {
                                ADO.ListMedicineADO metyExpmestTypeADO = new ADO.ListMedicineADO();

                                metyExpmestTypeADO.NUM_ORDER = expMestMetyGroup.First().NUM_ORDER ?? 999999;
                                metyExpmestTypeADO.AMOUNT = expMestMetyGroup.Sum(o => o.AMOUNT);
                                metyExpmestTypeADO.CREATE_TIME = expMestMetyGroup.First().CREATE_TIME;
                                metyExpmestTypeADO.kind = 0;
                                metyExpmestTypeADO.type = 1;
                                metyExpmestTypeADO.HuongDanSuDung = expMestMetyGroup.First().TUTORIAL;
                                metyExpmestTypeADO.CachDung = expMestMetyGroup.First().HTU_TEXT;
                                metyExpmestTypeADO.TocDoTruyen = expMestMetyGroup.First().SPEED;
                                metyExpmestTypeADO.ExpMestMedicineId = expMestMetyGroup.First().ID;
                                metyExpmestTypeADO.TDL_INTRUCTION_TIME = dataExpMest.TDL_INTRUCTION_TIME.Value;
                                metyExpmestTypeADO.USE_TIME_TO = expMestMetyGroup.First().USE_TIME_TO;
                                metyExpmestTypeADO.PRES_AMOUNT = expMestMetyGroup.Sum(o => o.PRES_AMOUNT ?? o.AMOUNT);
                                metyExpmestTypeADO.USE_TIME = serviceClick.USE_TIME;

                                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == expMestMetyGroup.First().TDL_MEDICINE_TYPE_ID);
                                if (mety != null)
                                {
                                    metyExpmestTypeADO.TDL_SERVICE_NAME = mety.MEDICINE_TYPE_NAME;
                                    metyExpmestTypeADO.TDL_SERVICE_CODE = mety.MEDICINE_TYPE_CODE;
                                    metyExpmestTypeADO.SERVICE_UNIT_NAME = mety.SERVICE_UNIT_NAME;
                                    metyExpmestTypeADO.CONVERT_RATIO = mety.CONVERT_RATIO;
                                    metyExpmestTypeADO.CONVERT_UNIT_NAME = mety.CONVERT_UNIT_NAME;
                                    if (mety.CONVERT_RATIO.HasValue)
                                    {
                                        metyExpmestTypeADO.CONVERT_AMOUNT = metyExpmestTypeADO.AMOUNT * mety.CONVERT_RATIO.Value;
                                    }
                                }
                                if (expMestMetyGroup.First().PATIENT_TYPE_ID != null)
                                //if (serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)   //Với loại “Đơn điều trị” thì hiển thị 'Đối tượng thanh toán'
                                {
                                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == expMestMetyGroup.First().PATIENT_TYPE_ID);
                                    metyExpmestTypeADO.PATIENT_TYPE_ID = patientType != null ? patientType.ID : 0;
                                    metyExpmestTypeADO.PATIENT_TYPE_NAME = patientType != null ? patientType.PATIENT_TYPE_NAME : null;
                                    metyExpmestTypeADO.IS_RATION = patientType != null ? patientType.IS_RATION : null;
                                }

                                listMedicine.Add(metyExpmestTypeADO);
                            }
                        }

                        paramCommon = new CommonParam();
                        HisExpMestMaterialFilter mateFilter = new HisExpMestMaterialFilter();
                        mateFilter.EXP_MEST_ID = dataExpMest.ID;
                        var materials = await new BackendAdapter(paramCommon).GetAsync<List<HIS_EXP_MEST_MATERIAL>>(RequestUriStore.HIS_EXP_MEST_MATERIAL_GET, ApiConsumer.ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (materials != null && materials.Count > 0)
                        {
                            var expMestMatyGroups = materials.GroupBy(o => new { o.TDL_MATERIAL_TYPE_ID, o.TUTORIAL, o.HTU_TEXT }).ToList();
                            foreach (var expMestMatyGroup in expMestMatyGroups)
                            {
                                ADO.ListMedicineADO matyExpmestTypeADO = new ADO.ListMedicineADO();

                                matyExpmestTypeADO.NUM_ORDER = expMestMatyGroup.First().NUM_ORDER ?? 999999;
                                matyExpmestTypeADO.AMOUNT = expMestMatyGroup.Sum(o => o.AMOUNT);
                                matyExpmestTypeADO.CREATE_TIME = expMestMatyGroup.First().CREATE_TIME;
                                matyExpmestTypeADO.kind = 0;
                                matyExpmestTypeADO.type = 0;
                                matyExpmestTypeADO.HuongDanSuDung = expMestMatyGroup.First().TUTORIAL;
                                matyExpmestTypeADO.CachDung = expMestMatyGroup.First().HTU_TEXT;
                                matyExpmestTypeADO.PRES_AMOUNT = expMestMatyGroup.Sum(o => o.PRES_AMOUNT ?? o.AMOUNT);
                                matyExpmestTypeADO.USE_TIME = serviceClick.USE_TIME;

                                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == expMestMatyGroup.First().TDL_MATERIAL_TYPE_ID);
                                if (maty != null)
                                {
                                    matyExpmestTypeADO.TDL_SERVICE_CODE = maty.MATERIAL_TYPE_CODE;
                                    matyExpmestTypeADO.TDL_SERVICE_NAME = maty.MATERIAL_TYPE_NAME;
                                    matyExpmestTypeADO.SERVICE_UNIT_NAME = maty.SERVICE_UNIT_NAME;
                                    matyExpmestTypeADO.CONVERT_RATIO = maty.CONVERT_RATIO;
                                    matyExpmestTypeADO.CONVERT_UNIT_NAME = maty.CONVERT_UNIT_NAME;
                                    if (maty.CONVERT_RATIO.HasValue)
                                    {
                                        matyExpmestTypeADO.CONVERT_AMOUNT = matyExpmestTypeADO.AMOUNT * maty.CONVERT_RATIO.Value;
                                    }
                                }
                                if (expMestMatyGroup.First().PATIENT_TYPE_ID != null)
                                //if (serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)   //Với loại “Đơn điều trị” thì hiển thị 'Đối tượng thanh toán'
                                {
                                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == expMestMatyGroup.First().PATIENT_TYPE_ID);
                                    matyExpmestTypeADO.PATIENT_TYPE_ID = patientType != null ? patientType.ID : 0;
                                    matyExpmestTypeADO.PATIENT_TYPE_NAME = patientType != null ? patientType.PATIENT_TYPE_NAME : null;
                                    matyExpmestTypeADO.IS_RATION = patientType != null ? patientType.IS_RATION : null;
                                }
                                listMedicine.Add(matyExpmestTypeADO);
                            }
                        }
                    }

                    #region ngoai dm
                    //Thuoc vat tu ngoai danh muc
                    //Thuoc
                    paramCommon = new CommonParam();
                    HisServiceReqMetyFilter metyFilter = new HisServiceReqMetyFilter();

                    metyFilter.SERVICE_REQ_ID = dataExpMest.SERVICE_REQ_ID;
                    List<HIS_SERVICE_REQ_METY> metys = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_SERVICE_REQ_METY>>(RequestUriStore.HIS_SERVICE_REQ_METY_GET, ApiConsumers.MosConsumer, metyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (metys != null && metys.Count > 0)
                    {


                        if (!metys.Exists(p => p.MEDICINE_TYPE_ID == null))
                        {
                            var expMestMetyGroups = metys.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.TUTORIAL}).ToList();
                            foreach (var expMestMetyGroup in expMestMetyGroups)
                            {
                                ADO.ListMedicineADO metyExpmestTypeADO = new ADO.ListMedicineADO();

                                metyExpmestTypeADO.TDL_SERVICE_NAME = expMestMetyGroup.First().MEDICINE_TYPE_NAME;
                                metyExpmestTypeADO.NUM_ORDER = expMestMetyGroup.First().NUM_ORDER ?? 999999;
                                metyExpmestTypeADO.AMOUNT = expMestMetyGroup.Sum(o => o.AMOUNT);
                                metyExpmestTypeADO.CREATE_TIME = expMestMetyGroup.First().CREATE_TIME;
                                metyExpmestTypeADO.HuongDanSuDung = expMestMetyGroup.First().TUTORIAL;
                                metyExpmestTypeADO.TocDoTruyen = expMestMetyGroup.First().SPEED;
                                metyExpmestTypeADO.subPress = expMestMetyGroup.First().IS_SUB_PRES;
                                metyExpmestTypeADO.kind = 1;
                                metyExpmestTypeADO.type = 1;
                                metyExpmestTypeADO.serviceReqMety = expMestMetyGroup.First();
                                metyExpmestTypeADO.USE_TIME = serviceClick.USE_TIME;

                                
                                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == expMestMetyGroup.First().MEDICINE_TYPE_ID);
                                if (mety != null)
                                {
                                    metyExpmestTypeADO.isStartMark = mety.IS_STAR_MARK;
                                    metyExpmestTypeADO.TDL_SERVICE_CODE = mety.MEDICINE_TYPE_CODE;
                                    metyExpmestTypeADO.SERVICE_UNIT_NAME = mety.SERVICE_UNIT_NAME;
                                    metyExpmestTypeADO.CONVERT_RATIO = mety.CONVERT_RATIO;
                                    metyExpmestTypeADO.CONVERT_UNIT_NAME = mety.CONVERT_UNIT_NAME;
                                    if (mety.CONVERT_RATIO.HasValue)
                                    {
                                        metyExpmestTypeADO.CONVERT_AMOUNT = metyExpmestTypeADO.AMOUNT * mety.CONVERT_RATIO.Value;
                                    }
                                }
                                listMedicine.Add(metyExpmestTypeADO);
                            }
                           
                        }
                        else
                        {
                            var notNull = metys.Where(o => o.MEDICINE_TYPE_ID != null).ToList();
                            if (notNull != null && notNull.Count > 0)
                            {
                                var expMestMetyGroups = notNull.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.TUTORIAL, o.HTU_TEXT }).ToList();
                                foreach (var expMestMetyGroup in expMestMetyGroups)
                                {
                                    ADO.ListMedicineADO metyExpmestTypeADO = new ADO.ListMedicineADO();

                                    metyExpmestTypeADO.TDL_SERVICE_NAME = expMestMetyGroup.First().MEDICINE_TYPE_NAME;
                                    metyExpmestTypeADO.NUM_ORDER = expMestMetyGroup.First().NUM_ORDER ?? 999999;
                                    metyExpmestTypeADO.AMOUNT = expMestMetyGroup.Sum(o => o.AMOUNT);
                                    metyExpmestTypeADO.CREATE_TIME = expMestMetyGroup.First().CREATE_TIME;
                                    metyExpmestTypeADO.HuongDanSuDung = expMestMetyGroup.First().TUTORIAL;
                                    metyExpmestTypeADO.TocDoTruyen = expMestMetyGroup.First().SPEED;
                                    metyExpmestTypeADO.subPress = expMestMetyGroup.First().IS_SUB_PRES;
                                    metyExpmestTypeADO.kind = 1;
                                    metyExpmestTypeADO.type = 1;
                                    metyExpmestTypeADO.serviceReqMety = expMestMetyGroup.First();
                                    metyExpmestTypeADO.USE_TIME = serviceClick.USE_TIME;

                                    var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == expMestMetyGroup.First().MEDICINE_TYPE_ID);
                                    if (mety != null)
                                    {
                                        metyExpmestTypeADO.isStartMark = mety.IS_STAR_MARK;
                                        metyExpmestTypeADO.TDL_SERVICE_CODE = mety.MEDICINE_TYPE_CODE;
                                        metyExpmestTypeADO.SERVICE_UNIT_NAME = mety.SERVICE_UNIT_NAME;
                                        metyExpmestTypeADO.CONVERT_RATIO = mety.CONVERT_RATIO;
                                        metyExpmestTypeADO.CONVERT_UNIT_NAME = mety.CONVERT_UNIT_NAME;
                                        if (mety.CONVERT_RATIO.HasValue)
                                        {
                                            metyExpmestTypeADO.CONVERT_AMOUNT = metyExpmestTypeADO.AMOUNT * mety.CONVERT_RATIO.Value;
                                        }

                                    }
                                    listMedicine.Add(metyExpmestTypeADO);
                                }
                            }

                            var Null = metys.Where(o => o.MEDICINE_TYPE_ID == null).ToList();
                            if (Null != null && Null.Count > 0)
                            {
                                var expMestMetyGroups = Null.GroupBy(o => new { o.MEDICINE_TYPE_NAME, o.MEDICINE_USE_FORM_ID, o.TUTORIAL}).ToList();
                                foreach (var expMestMetyGroup in expMestMetyGroups)
                                {
                                    ADO.ListMedicineADO metyExpmestTypeADO = new ADO.ListMedicineADO();

                                    metyExpmestTypeADO.TDL_SERVICE_NAME = expMestMetyGroup.First().MEDICINE_TYPE_NAME;
                                    metyExpmestTypeADO.NUM_ORDER = expMestMetyGroup.First().NUM_ORDER ?? 999999;
                                    metyExpmestTypeADO.AMOUNT = expMestMetyGroup.Sum(o => o.AMOUNT);
                                    metyExpmestTypeADO.CREATE_TIME = expMestMetyGroup.First().CREATE_TIME;
                                    metyExpmestTypeADO.HuongDanSuDung = expMestMetyGroup.First().TUTORIAL;
                                    metyExpmestTypeADO.TocDoTruyen = expMestMetyGroup.First().SPEED;
                                    metyExpmestTypeADO.subPress = expMestMetyGroup.First().IS_SUB_PRES;
                                    metyExpmestTypeADO.type = 1;
                                    metyExpmestTypeADO.kind = 1;
                                    metyExpmestTypeADO.serviceReqMety = expMestMetyGroup.First();
                                    metyExpmestTypeADO.USE_TIME = serviceClick.USE_TIME;
                                    metyExpmestTypeADO.SERVICE_UNIT_NAME = expMestMetyGroup.First().UNIT_NAME;
                                    listMedicine.Add(metyExpmestTypeADO);
                                }
                            }
                        }
                    }
                    //vat tu
                    paramCommon = new CommonParam();
                    HisServiceReqMatyFilter matyFilter = new HisServiceReqMatyFilter();
                    matyFilter.SERVICE_REQ_ID = dataExpMest.SERVICE_REQ_ID;
                    List<HIS_SERVICE_REQ_MATY> matys = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_SERVICE_REQ_MATY>>(RequestUriStore.HIS_SERVICE_REQ_MATY_GET, ApiConsumers.MosConsumer, matyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (matys != null && matys.Count > 0)
                    {
                        if (!matys.Exists(p => p.MATERIAL_TYPE_ID == null))
                        {
                            var expMestMatyGroups = matys.GroupBy(o => new { o.MATERIAL_TYPE_ID, o.TUTORIAL}).ToList();
                            foreach (var expMestMatyGroup in expMestMatyGroups)
                            {
                                ADO.ListMedicineADO matyExpmestTypeADO = new ADO.ListMedicineADO();
                                matyExpmestTypeADO.NUM_ORDER = expMestMatyGroup.First().NUM_ORDER ?? 999999;
                                matyExpmestTypeADO.AMOUNT = expMestMatyGroup.Sum(o => o.AMOUNT);
                                matyExpmestTypeADO.CREATE_TIME = expMestMatyGroup.First().CREATE_TIME;
                                matyExpmestTypeADO.subPress = expMestMatyGroup.First().IS_SUB_PRES;
                                matyExpmestTypeADO.kind = 1;
                                matyExpmestTypeADO.type = 0;
                                matyExpmestTypeADO.HuongDanSuDung = expMestMatyGroup.First().TUTORIAL;
                                matyExpmestTypeADO.USE_TIME = serviceClick.USE_TIME;

                                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == expMestMatyGroup.First().MATERIAL_TYPE_ID);
                                if (maty != null)
                                {
                                    matyExpmestTypeADO.TDL_SERVICE_CODE = maty.MATERIAL_TYPE_CODE;
                                    matyExpmestTypeADO.TDL_SERVICE_NAME = maty.MATERIAL_TYPE_NAME;
                                    matyExpmestTypeADO.SERVICE_UNIT_NAME = maty.SERVICE_UNIT_NAME;
                                    matyExpmestTypeADO.CONVERT_RATIO = maty.CONVERT_RATIO;
                                    matyExpmestTypeADO.CONVERT_UNIT_NAME = maty.CONVERT_UNIT_NAME;
                                    if (maty.CONVERT_RATIO.HasValue)
                                    {
                                        matyExpmestTypeADO.CONVERT_AMOUNT = matyExpmestTypeADO.AMOUNT * maty.CONVERT_RATIO.Value;
                                    }
                                }
                                listMedicine.Add(matyExpmestTypeADO);
                            }
                        }
                        else
                        {
                            var notNull = matys.Where(o => o.MATERIAL_TYPE_ID != null).ToList();
                            if (notNull != null && notNull.Count > 0)
                            {
                                var expMestMatyGroups = notNull.GroupBy(o => new { o.MATERIAL_TYPE_ID, o.TUTORIAL }).ToList();
                                foreach (var expMestMatyGroup in expMestMatyGroups)
                                {
                                    ADO.ListMedicineADO matyExpmestTypeADO = new ADO.ListMedicineADO();
                                    matyExpmestTypeADO.NUM_ORDER = expMestMatyGroup.First().NUM_ORDER ?? 999999;
                                    matyExpmestTypeADO.AMOUNT = expMestMatyGroup.Sum(o => o.AMOUNT);
                                    matyExpmestTypeADO.CREATE_TIME = expMestMatyGroup.First().CREATE_TIME;
                                    matyExpmestTypeADO.subPress = expMestMatyGroup.First().IS_SUB_PRES;
                                    matyExpmestTypeADO.kind = 1;
                                    matyExpmestTypeADO.type = 0;
                                    matyExpmestTypeADO.HuongDanSuDung = expMestMatyGroup.First().TUTORIAL;
                                    matyExpmestTypeADO.USE_TIME = serviceClick.USE_TIME;

                                    var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == expMestMatyGroup.First().MATERIAL_TYPE_ID);
                                    if (maty != null)
                                    {
                                        matyExpmestTypeADO.TDL_SERVICE_CODE = maty.MATERIAL_TYPE_CODE;
                                        matyExpmestTypeADO.TDL_SERVICE_NAME = maty.MATERIAL_TYPE_NAME;
                                        matyExpmestTypeADO.SERVICE_UNIT_NAME = maty.SERVICE_UNIT_NAME;
                                        matyExpmestTypeADO.CONVERT_RATIO = maty.CONVERT_RATIO;
                                        matyExpmestTypeADO.CONVERT_UNIT_NAME = maty.CONVERT_UNIT_NAME;
                                        if (maty.CONVERT_RATIO.HasValue)
                                        {
                                            matyExpmestTypeADO.CONVERT_AMOUNT = matyExpmestTypeADO.AMOUNT * maty.CONVERT_RATIO.Value;
                                        }
                                    }
                                    listMedicine.Add(matyExpmestTypeADO);
                                }
                            }

                            var Null = matys.Where(o => o.MATERIAL_TYPE_ID == null).ToList();
                            if (Null != null && Null.Count > 0)
                            {
                                var expMestMatyGroups = Null.GroupBy(o => new { o.MATERIAL_TYPE_ID, o.TUTORIAL, o.HTU_TEXT}).ToList();
                                foreach (var expMestMatyGroup in expMestMatyGroups)
                                {
                                    ADO.ListMedicineADO matyExpmestTypeADO = new ADO.ListMedicineADO();
                                    matyExpmestTypeADO.NUM_ORDER = expMestMatyGroup.First().NUM_ORDER ?? 999999;
                                    matyExpmestTypeADO.AMOUNT = expMestMatyGroup.Sum(o => o.AMOUNT);
                                    matyExpmestTypeADO.CREATE_TIME = expMestMatyGroup.First().CREATE_TIME;
                                    matyExpmestTypeADO.subPress = expMestMatyGroup.First().IS_SUB_PRES;
                                    matyExpmestTypeADO.kind = 1;
                                    matyExpmestTypeADO.type = 0;
                                    matyExpmestTypeADO.SERVICE_UNIT_NAME = expMestMatyGroup.First().UNIT_NAME;
                                    matyExpmestTypeADO.HuongDanSuDung = expMestMatyGroup.First().TUTORIAL;
                                  
                                    matyExpmestTypeADO.USE_TIME = serviceClick.USE_TIME;
                                    listMedicine.Add(matyExpmestTypeADO);
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion
                else if (serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN)
                {
                    HisSereServRationFilter rationFilter = new HisSereServRationFilter();
                    rationFilter.SERVICE_REQ_ID = serviceClick.ID;
                    rationFilter.ORDER_DIRECTION = "DESC";
                    rationFilter.ORDER_FIELD = "ID";
                    paramCommon = new CommonParam();
                    var listRation = await new BackendAdapter(paramCommon).GetAsync<List<V_HIS_SERE_SERV_RATION>>(RequestUriStore.HIS_SERE_SERV_RATION_GETVIEW, ApiConsumers.MosConsumer, rationFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (listRation != null && listRation.Count > 0)
                    {
                        foreach (var item in listRation)
                        {
                            ListMedicineADO ado = new ListMedicineADO();
                            ado.IS_RATION = item.IS_RATION;
                            ado.AMOUNT = item.AMOUNT;
                            ado.DISCOUNT = item.DISCOUNT;
                            ado.HuongDanSuDung = item.INSTRUCTION_NOTE;
                            ado.CachDung = item.INSTRUCTION_NOTE;
                            ado.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID;
                            ado.PATIENT_TYPE_NAME = item.PATIENT_TYPE_NAME;
                            ado.PRICE = item.PRICE;
                            ado.DISCOUNT = item.DISCOUNT;
                            ado.VAT_RATIO = item.VAT_RATIO ?? 0;
                            ado.SERVICE_ID = item.SERVICE_ID;
                            //ado.TDL_INTRUCTION_TIME = dataExpMest.TDL_INTRUCTION_TIME.Value;
                            var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                            if (service != null)
                            {
                                ado.TDL_SERVICE_CODE = service.SERVICE_CODE;
                                ado.TDL_SERVICE_NAME = service.SERVICE_NAME;
                                //ado.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                                ado.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                            }

                            listMedicine.Add(ado);
                        }
                    }
                }
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    listMedicine = listMedicine.OrderBy(o => o.NUM_ORDER).ToList();
                }
                else
                {
                    HisSereServFilter filter = new HisSereServFilter();
                    filter.SERVICE_REQ_ID = serviceClick.ID;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "ID";
                    paramCommon = new CommonParam();
                    listMedicine = await new BackendAdapter(paramCommon).GetAsync<List<ADO.ListMedicineADO>>(RequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (listMedicine != null)
                    {
                        foreach (var item in listMedicine)
                        {
                            var unit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                            if (unit != null)
                            {
                                item.SERVICE_UNIT_NAME = unit.SERVICE_UNIT_NAME;
                                item.CONVERT_RATIO = unit.CONVERT_RATIO;

                                if (unit.CONVERT_ID.HasValue && unit.CONVERT_RATIO.HasValue)
                                {
                                    var conv = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == unit.CONVERT_ID);
                                    if (conv != null)
                                    {
                                        item.CONVERT_UNIT_NAME = conv.SERVICE_UNIT_NAME;
                                        item.CONVERT_AMOUNT = item.AMOUNT * unit.CONVERT_RATIO.Value;
                                    }
                                }
                            }
                            var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == item.PATIENT_TYPE_ID);
                            item.PATIENT_TYPE_ID = patientType != null ? patientType.ID : 0;
                            item.PATIENT_TYPE_NAME = patientType != null ? patientType.PATIENT_TYPE_NAME : null;
                            item.IS_RATION = patientType != null ? patientType.IS_RATION : null;
                            item.USE_TIME = serviceClick.USE_TIME;
                            var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                            if (service != null)
                            {
                                item.PTTT_GROUP_NAME = service.PTTT_GROUP_NAME;
                            }

                        }
                    }
                }

                _listMedicine = new List<ListMedicineADO>();
                _listMedicine.AddRange(listMedicine);
                grdViewSereServServiceReq.BeginUpdate();
                grdViewSereServServiceReq.GridControl.DataSource = listMedicine;
                grdViewSereServServiceReq.EndUpdate();
            }
            catch (Exception ex)
            {
                listMedicine = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ControlServiceReqClick(ServiceReqADO data)
        {
            try
            {
                if (data != null)
                {
                    var serviceClick = data;
                    if (serviceClick != null && serviceClick.ID > 0)
                    {
                        //object dataSource = null;
                        WaitingManager.Show();
                        CommonParam paramCommon = new CommonParam();
                        HIS_EXP_MEST expMest = null;

                        if (serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                            serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM ||
                            serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                            serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                        {
                            this.currentPrescription = null;
                            HisExpMestFilter expMestFilter = new HisExpMestFilter();
                            expMestFilter.SERVICE_REQ_ID = serviceClick.ID;
                            var result = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>(RequestUriStore.HIS_EXP_MEST_GET, ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                            if (result != null && result.Count > 0)
                            {
                                currentPrescription = result.FirstOrDefault();
                                expMest = result.FirstOrDefault();
                            }
                            else
                            {
                                expMest = new HIS_EXP_MEST();
                                expMest.SERVICE_REQ_ID = serviceClick.ID;
                            }
                        }

                        this.FillDataGridDetail(expMest, serviceClick);
                        this.FillDataToControl(expMest, serviceClick);
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task GetSereServ(ServiceReqADO serviceClick)
        {

            try
            {
                if (serviceClick != null && serviceClick.ID > 0)
                {
                    CommonParam paramCommon = new CommonParam();
                    HIS_EXP_MEST expMest = null;

                    if (serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                        serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM ||
                        serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                        serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                    {
                        this.currentPrescription = null;
                        HisExpMestFilter expMestFilter = new HisExpMestFilter();
                        expMestFilter.SERVICE_REQ_ID = serviceClick.ID;
                        var result = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>(RequestUriStore.HIS_EXP_MEST_GET, ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (result != null && result.Count > 0)
                        {
                            currentPrescription = result.FirstOrDefault();
                            expMest = result.FirstOrDefault();
                        }
                        else
                        {
                            expMest = new HIS_EXP_MEST();
                            expMest.SERVICE_REQ_ID = serviceClick.ID;
                        }
                    }

                    await this.FillDataGridDetail(expMest, serviceClick);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region event
        private void gridControlServiceReq_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var serviceClick = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("serviceClick___", serviceClick));
                    if (serviceClick != null && serviceClick.ID != 0)
                    {
                        this.currentServiceReq = serviceClick;
                        this.ControlServiceReqClick(this.currentServiceReq);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ADO.ServiceReqADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (((ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.CurrentPage) - 1) * (ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.PageSize));
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMG")
                        {
                            try
                            {
                                long statusId = data.SERVICE_REQ_STT_ID;
                                if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && data.SAMPLE_TIME != null)
                                {
                                    e.Value = imageListIcon.Images[6];
                                }
                                else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL && data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && data.RECEIVE_SAMPLE_TIME != null)
                                {
                                    e.Value = imageListIcon.Images[2];
                                }
                                else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                {
                                    e.Value = imageListIcon.Images[0];
                                }
                                else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                {
                                    e.Value = imageListIcon.Images[1];
                                }
                                else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                {
                                    e.Value = imageListIcon.Images[3];
                                }
                                else
                                {
                                    e.Value = imageListIcon.Images[0];
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot icon trang thai yeu cau dich vu IMG", ex);
                            }
                        }
                        else if (e.Column.FieldName == "PRIORITY_DISPLAY")
                        {
                            long priority = (data.PRIORITY ?? 0);
                            if ((priority == 1))
                            {
                                e.Value = imageListPriority.Images[0];
                            }
                        }
                        else if (e.Column.FieldName == "REQUEST_USERNAME_DISPLAY")
                        {
                            try
                            {
                                e.Value = data.REQUEST_LOGINNAME + (String.IsNullOrEmpty(data.REQUEST_USERNAME) ? "" : " - " + data.REQUEST_USERNAME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXECUTE_USERNAME_DISPLAY")
                        {
                            try
                            {
                                e.Value = data.EXECUTE_LOGINNAME + (String.IsNullOrEmpty(data.EXECUTE_USERNAME) ? "" : " - " + data.EXECUTE_USERNAME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME);
                            }
                            catch (Exception ex) 
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        //qtcode
                        else if (e.Column.FieldName == "USE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.USE_TIME.Value);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        //qtcode
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
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
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "YEAR")
                        {
                            try
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MAIN_EXAM")
                        {
                            try
                            {
                                if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                                    e.Value = data.IS_MAIN_EXAM == 1 ? "Khám chính" : "";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS_INTEGRATE_HIS_SENT_STR")
                        {
                            try
                            {
                                e.Value = data.IS_INTEGRATE_HIS_SENT == 1 ? "Đã gửi" : "Chưa gửi";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "RATION_TIME_STR")
                        {
                            try
                            {
                                var rs = this.lsRationTime.FirstOrDefault(o => o.ID == (data.RATION_TIME_ID ?? 0));
                                if (rs != null)
                                    e.Value = rs.RATION_TIME_NAME;
                                //e.Value = this.lsRationTime.FirstOrDefault(o => o.ID == (data.RATION_TIME_ID ?? 0)).RATION_TIME_NAME;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void gridViewServiceReq_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    ServiceReqADO data = (ServiceReqADO)gridViewServiceReq.GetRow(e.RowHandle);
                    var workingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    string creator = (gridViewServiceReq.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    string reqLoginName = (gridViewServiceReq.GetRowCellValue(e.RowHandle, "REQUEST_LOGINNAME") ?? "").ToString().Trim();
                    long reqSttId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceReq.GetRowCellValue(e.RowHandle, "SERVICE_REQ_STT_ID") ?? "").ToString().Trim());
                    long serReqTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceReq.GetRowCellValue(e.RowHandle, "SERVICE_REQ_TYPE_ID") ?? "").ToString().Trim());
                    short isNoExecute = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewServiceReq.GetRowCellValue(e.RowHandle, "IS_NO_EXECUTE") ?? "").ToString());
                    short isCofirm = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewServiceReq.GetRowCellValue(e.RowHandle, "IS_CONFIRM_NO_EXCUTE") ?? "").ToString());
                    string jsonPrint = (gridViewServiceReq.GetRowCellValue(e.RowHandle, "JSON_PRINT_ID") ?? "").ToString().Trim();
                    long requestDepartmentId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceReq.GetRowCellValue(e.RowHandle, "REQUEST_DEPARTMENT_ID") ?? "").ToString().Trim());
                    long requestRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceReq.GetRowCellValue(e.RowHandle, "REQUEST_ROOM_ID") ?? "").ToString().Trim());
                    long executeRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceReq.GetRowCellValue(e.RowHandle, "EXECUTE_ROOM_ID") ?? "").ToString().Trim());

                    if (e.Column.FieldName == "ServiceReqDelete")
                    {
                        bool accountCanDelete = this.loginName == creator || CheckLoginAdmin.IsAdmin(this.loginName) || this.loginName == reqLoginName;
                        bool roomCanDelete = this.currentRoom != null && (currentRoom.ID == executeRoomId || currentRoom.ID == requestRoomId);
                        if (reqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && (accountCanDelete || (serReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && requestDepartmentId == this.currentRoom.DEPARTMENT_ID && roomCanDelete)))
                        {
                            e.RepositoryItem = repositoryItemBtnDeleteServiceReq;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnDeleteServiceReqDisable;
                        }
                        if (data.CARER_CARD_BORROW_ID != null)
                        {
                            e.RepositoryItem = repositoryItemBtnDeleteServiceReqDisable;
                        }
                    }
                    else if (e.Column.FieldName == "BieuMauKhac")
                    {
                        if (!String.IsNullOrEmpty(jsonPrint))
                            e.RepositoryItem = repositoryItemBtnBieuMauKhac;
                        else
                            e.RepositoryItem = repositoryItemReadOnly;
                    }
                    else if (e.Column.FieldName == "ServiceReqEdit")
                    {
                        //if (serReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONNT)
                        //{
                        //    e.RepositoryItem = repositoryItemReadOnly;
                        //}
                        if (creator == this.loginName || reqLoginName == this.loginName || CheckLoginAdmin.IsAdmin(this.loginName))
                        {
                            if (isNoExecute != Base.GlobalStore.IS_TRUE)
                            {
                                if ((reqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                             || HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1" ||
                             HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "2" && serReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                {
                                    e.RepositoryItem = repositoryItemBtnEditServiceReq;
                                }
                            }
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemReadOnly;
                        }
                        if (data.CARER_CARD_BORROW_ID != null)
                        {
                            e.RepositoryItem = repositoryItemBtnEditServiceReqDisable;
                        }
                    }
                    else if (e.Column.FieldName == "AllowNotExecute")
                    {
                        if (//data.REQUEST_ROOM_ID == this.currentModule.RoomId && 
                            //workingRoom.IS_EXAM == 1
                            //&& (data.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                            //&& data.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                            //&& data.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                            //&& data.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                            //&& 
                            data.IS_ACCEPTING_NO_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            //&& data.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            )
                        {
                            e.RepositoryItem = repositoryItemButtonEditAllowNotExecute_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEditAllowNotExecute_Disable;
                        }
                    }
                    //else if (e.Column.FieldName == "IsConfirmNoExcute")
                    //{
                    //    if (isCofirm == 1)
                    //    {
                    //        e.RepositoryItem = repositoryItemButtonEditIsConfirm_Ena;
                    //    }
                    //    else
                    //    {
                    //        e.RepositoryItem = repositoryItemButtonEditIsConfirm_Dis;
                    //    }
                    //}
                    else if (e.Column.FieldName == "ServiceReqPrint")
                    {
                        if (data.CARER_CARD_BORROW_ID != null)
                        {
                            e.RepositoryItem = repositoryItemBtnPrintServiceReqDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnPrintServiceReq;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetRow(e.RowHandle);
                    //var data = (ADO.ServiceReqADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IS_NO_EXECUTE == Base.GlobalStore.IS_TRUE)
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
                        if (data.IS_TEMPORARY_PRES == 1)
                        {
                            e.Appearance.ForeColor = Color.Orange;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                var listData = (List<ADO.ServiceReqADO>)gridControlServiceReq.DataSource;
                if (listData.Count > 0)
                {
                    listServiceReq = new List<ADO.ServiceReqADO>();

                    foreach (var item in listData)
                    {
                        if (item.isCheck)
                        {
                            listServiceReq.Add(item);
                        }
                    }

                    if (listServiceReq != null && listServiceReq.Count > 0)
                    {

                        var PopupMenuProcessor = new PopupMenuProcessorCheck(barManager1, RightMenuCheck_Click, listServiceReq, this.loginName, this.currentRoom, listServiceReq);
                        PopupMenuProcessor.InitMenu();
                    }
                    else
                    {
                        var row = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                        if (row != null)
                        {
                            row.DeleteCheck = CheckLoginAdmin.IsAdmin(this.loginName) || (this.currentRoom != null && row.REQUEST_DEPARTMENT_ID == this.currentRoom.DEPARTMENT_ID && row.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
                            row.AddInforPTTT = row.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT
                                && (this.currentRoom != null
                                && row.EXECUTE_DEPARTMENT_ID == this.currentRoom.DEPARTMENT_ID
                                && (row.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                                || row.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT));
                            PrintPopupMenuProcessor = new PrintPopupMenuProcessor(RightMenu_Click, barManager1, row, this.loginName, this.currentRoom);
                            PrintPopupMenuProcessor.currentDepartmentId = this.currentRoom != null ? currentRoom.DEPARTMENT_ID : 0;
                            PrintPopupMenuProcessor.RightMenu();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RightMenuCheck_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    PopupMenuProcessorCheck.ItemType type = (PopupMenuProcessorCheck.ItemType)(e.Item.Tag);
                    switch (type)
                    {
                        case PopupMenuProcessorCheck.ItemType.In:
                            ProcessDataCheckedToPrint();
                            break;
                        case PopupMenuProcessorCheck.ItemType.Xoa:
                            ProcessDataCheckedToDelete();
                            break;
                        case PopupMenuProcessorCheck.ItemType.ChuyenPhong:
                            ProcessDataCheckedToChangeRoom();
                            break;
                        case PopupMenuProcessorCheck.ItemType.InKemKetQua:
                            ProcessDataInKemKetQua();
                            break;
                        case PopupMenuProcessorCheck.ItemType.KetQuaHeThongBenhAnhDienTu:
                            PrintKetQuaHeThongBenhAnhDienTu();
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataInKemKetQua()
        {
            try
            {
                try
                {
                    if (listServiceReq != null && listServiceReq.Count > 0)
                    {

                        foreach (var item in listServiceReq)
                        {


                            if (item != null && item.IS_NO_EXECUTE != 1)
                            {
                                this.currentServiceReqPrint = new ServiceReqADO();
                                ExecuteBefPrint(item);
                                if (item.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                {
                                    this.currentServiceReqPrint = item;
                                    this.serviceReqPrintRaw = GetServiceReqForPrint(item.ID);
                                    WaitingManager.Hide();

                                    if (currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA ||
                                        currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL ||
                                        currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS ||
                                        currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA ||
                                        currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN)
                                    {

                                        HisSereServExtFilter hfilter = new HisSereServExtFilter();
                                        hfilter.TDL_SERVICE_REQ_ID = currentServiceReqPrint.ID;
                                        var sereServExt = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, hfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                                        if (sereServExt != null && sereServExt.Count > 0)
                                        {
                                            sereServExtPrint = sereServExt.FirstOrDefault();
                                            SAR.EFMODEL.DataModels.SAR_PRINT currentSarPrint = GetListPrintByDescriptionPrint(sereServExtPrint);

                                            if (currentSarPrint != null && currentSarPrint.ID > 0)
                                            {

                                                LoadTreatmentWithPaty();
                                                ProcessDicParamForPrint();

                                                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.PrintOption") == "1")
                                                {
                                                    PrintOption1(false, Utility.TextLibHelper.BytesToStringConverted(currentSarPrint.CONTENT));
                                                }
                                                else
                                                {
                                                    PrintOption2(false, Utility.TextLibHelper.BytesToStringConverted(currentSarPrint.CONTENT));
                                                }
                                            }
                                        }

                                    }
                                    else if (currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                                    {

                                        Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                                        richEditorMain.RunPrintTemplate("Mps000014", InKetQuaXetNghiem);
                                    }

                                    WaitingManager.Hide();
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void LoadTreatmentWithPaty()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("1. Begin LoadTreatmentWithPaty");
                CommonParam param = new CommonParam();

                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = currentServiceReqPrint.TREATMENT_ID;
                filter.INTRUCTION_TIME = currentServiceReqPrint.INTRUCTION_TIME;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    TreatmentWithPatientTypeAlter = apiResult.FirstOrDefault();
                }
                Inventec.Common.Logging.LogSystem.Info("1. End LoadTreatmentWithPaty");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static string GetCurrentTimeSeparateBeginTime(System.DateTime now)
        {
            string result = "";
            try
            {
                if (now != DateTime.MinValue)
                {
                    string month = string.Format("{0:00}", now.Month);
                    string day = string.Format("{0:00}", now.Day);
                    string hour = string.Format("{0:00}", now.Hour);
                    string hours = string.Format("{0:00}", now.Hour);
                    string minute = string.Format("{0:00}", now.Minute);
                    string strNgay = "ngày";
                    string strThang = "tháng";
                    string strNam = "năm";
                    result = string.Format("{0}" + ":" + "{1} " + strNgay + " {2} " + strThang + " {3} " + strNam + " {4}", hours, minute, now.Day, now.Month, now.Year);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        internal static string GetCurrentTimeSeparateBeginTime(long time)
        {
            string result = "";
            try
            {
                if (time > 0)
                {
                    string temp = time.ToString();
                    string year = string.Format("{0:00}", temp.Substring(0, 4));
                    string month = string.Format("{0:00}", temp.Substring(4, 2));
                    string day = string.Format("{0:00}", temp.Substring(6, 2));
                    string hours = string.Format("{0:00}", temp.Substring(8, 2));
                    string minute = string.Format("{0:00}", temp.Substring(10, 2));
                    result = string.Format("{0} giờ {1} phút ngày {2} tháng {3} năm {4}", hours, minute, day, month, year);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private string CalculatorAge(long ageYearNumber, bool isHl7)
        {
            string tuoi = "";
            try
            {
                string caption__Tuoi = "Tuổi";
                string caption__ThangTuoi = "Tháng tuổi";
                string caption__NgayTuoi = "Ngày tuổi";
                string caption__GioTuoi = "Giờ tuổi";

                if (isHl7)
                {
                    caption__Tuoi = "T";
                    caption__ThangTuoi = "TH";
                    caption__NgayTuoi = "NT";
                    caption__GioTuoi = "GT";
                }

                if (ageYearNumber > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ageYearNumber).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    TimeSpan diff__hour = (System.DateTime.Now - dtNgSinh);
                    TimeSpan diff__month = (System.DateTime.Now.Date - dtNgSinh.Date);

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;
                    if (hour < 24)
                    {
                        tuoi = ((int)hour + " " + caption__GioTuoi);
                    }
                    else
                    {
                        long tongsogiay__hour = diff__hour.Ticks;
                        System.DateTime newDate__hour = new System.DateTime(tongsogiay__hour);
                        int month__hour = ((newDate__hour.Year - 1) * 12 + newDate__hour.Month - 1);
                        if (month__hour == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            tuoi = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                        }
                        else
                        {
                            long tongsogiay = diff__month.Ticks;
                            System.DateTime newDate = new System.DateTime(tongsogiay);
                            int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                            if (month == 0)
                            {
                                //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                                tuoi = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                            }
                            else
                            {
                                //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                                if (month < 72)
                                {
                                    tuoi = (month + " " + caption__ThangTuoi);
                                }
                                //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                                else
                                {
                                    int year = System.DateTime.Now.Year - dtNgSinh.Year;
                                    tuoi = (year + " " + caption__Tuoi);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                tuoi = "";
            }
            return tuoi;
        }

        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus, bool autoOveride)
        {
            try
            {
                if (data != null)
                {
                    PropertyInfo[] pis = typeof(T).GetProperties();
                    if (pis != null && pis.Length > 0)
                    {
                        foreach (var pi in pis)
                        {
                            if (pi.GetGetMethod().IsVirtual) continue;

                            var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);
                            if (String.IsNullOrEmpty(searchKey.Key))
                            {
                                dicParamPlus.Add(pi.Name, pi.GetValue(data));
                            }
                            else
                            {
                                if (autoOveride)
                                    dicParamPlus[pi.Name] = pi.GetValue(data);
                                else if (dicParamPlus[pi.Name] == null)
                                    dicParamPlus[pi.Name] = pi.GetValue(data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDicParamForPrint()
        {
            try
            {
                ProcessDicParam();

                //bổ sung các key nhóm cha của dv
                var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                if (service.PARENT_ID.HasValue)
                {
                    var serviceParent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == service.PARENT_ID);
                    if (serviceParent != null)
                    {
                        this.dicParam.Add("SERVICE_CODE_PARENT", serviceParent.SERVICE_CODE);
                        this.dicParam.Add("SERVICE_NAME_PARENT", serviceParent.SERVICE_NAME);
                        this.dicParam.Add("HEIN_SERVICE_BHYT_CODE_PARENT", serviceParent.HEIN_SERVICE_BHYT_CODE);
                        this.dicParam.Add("HEIN_SERVICE_BHYT_NAME_PARENT", serviceParent.HEIN_SERVICE_BHYT_NAME);
                    }
                }

                dicParam["IS_COPY"] = "BẢN SAO";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDicParam()
        {
            try
            {
                // chế biến dữ liệu thành các key đơn thêm vào biểu mẫu tương tự như mps excel
                this.dicParam = new Dictionary<string, object>();
                this.dicImage = new Dictionary<string, Image>();

                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(this.dicParam);//commonkey
                if (currentServiceReqPrint != null)
                {
                    dicParam.Add("INTRUCTION_TIME_FULL_STR",
                            GetCurrentTimeSeparateBeginTime(
                            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            currentServiceReqPrint.INTRUCTION_TIME) ?? DateTime.Now));

                    dicParam.Add("INTRUCTION_DATE_FULL_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        currentServiceReqPrint.INTRUCTION_TIME));

                    dicParam.Add("INTRUCTION_TIME_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReqPrint.INTRUCTION_TIME));
                    //qtcode
                    dicParam.Add("USE_TIME",
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                            currentServiceReqPrint.USE_TIME.Value));
                    //qtcode
                    dicParam.Add("START_TIME_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReqPrint.START_TIME ?? 0));

                    dicParam.Add("START_TIME_FULL_STR",
                            GetCurrentTimeSeparateBeginTime(
                            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            currentServiceReqPrint.START_TIME ?? 0) ?? DateTime.Now));

                    dicParam.Add("ICD_MAIN_TEXT", currentServiceReqPrint.ICD_NAME);

                    dicParam.Add("NATIONAL_NAME", currentServiceReqPrint.TDL_PATIENT_NATIONAL_NAME);
                    dicParam.Add("WORK_PLACE", currentServiceReqPrint.TDL_PATIENT_WORK_PLACE_NAME);
                    dicParam.Add("ADDRESS", currentServiceReqPrint.TDL_PATIENT_ADDRESS);
                    dicParam.Add("CAREER_NAME", currentServiceReqPrint.TDL_PATIENT_CAREER_NAME);
                    dicParam.Add("PATIENT_CODE", currentServiceReqPrint.TDL_PATIENT_CODE);
                    dicParam.Add("DISTRICT_CODE", currentServiceReqPrint.TDL_PATIENT_DISTRICT_CODE);
                    dicParam.Add("GENDER_NAME", currentServiceReqPrint.TDL_PATIENT_GENDER_NAME);
                    dicParam.Add("MILITARY_RANK_NAME", currentServiceReqPrint.TDL_PATIENT_MILITARY_RANK_NAME);
                    dicParam.Add("VIR_ADDRESS", currentServiceReqPrint.TDL_PATIENT_ADDRESS);
                    dicParam.Add("AGE", CalculatorAge(currentServiceReqPrint.TDL_PATIENT_DOB, false));
                    dicParam.Add("STR_YEAR", currentServiceReqPrint.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    dicParam.Add("VIR_PATIENT_NAME", currentServiceReqPrint.TDL_PATIENT_NAME);

                    var executeRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReqPrint.EXECUTE_ROOM_ID);
                    if (executeRoom != null)
                    {
                        dicParam.Add("EXECUTE_DEPARTMENT_CODE", executeRoom.DEPARTMENT_CODE);
                        dicParam.Add("EXECUTE_DEPARTMENT_NAME", executeRoom.DEPARTMENT_NAME);
                        dicParam.Add("EXECUTE_ROOM_CODE", executeRoom.ROOM_CODE);
                        dicParam.Add("EXECUTE_ROOM_NAME", executeRoom.ROOM_NAME);
                    }

                    var reqRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReqPrint.REQUEST_ROOM_ID);
                    if (reqRoom != null)
                    {
                        dicParam.Add("REQUEST_DEPARTMENT_CODE", reqRoom.DEPARTMENT_CODE);
                        dicParam.Add("REQUEST_DEPARTMENT_NAME", reqRoom.DEPARTMENT_NAME);
                        dicParam.Add("REQUEST_ROOM_CODE", reqRoom.ROOM_CODE);
                        dicParam.Add("REQUEST_ROOM_NAME", reqRoom.ROOM_NAME);
                    }
                }

                if (TreatmentWithPatientTypeAlter != null)
                {
                    if (!String.IsNullOrEmpty(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        dicParam.Add("HEIN_CARD_NUMBER_SEPARATE",
                            HeinCardHelper.SetHeinCardNumberDisplayByNumber(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER));
                        dicParam.Add("STR_HEIN_CARD_FROM_TIME",
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(
                            TreatmentWithPatientTypeAlter.HEIN_CARD_FROM_TIME));
                        dicParam.Add("STR_HEIN_CARD_TO_TIME",
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(
                            TreatmentWithPatientTypeAlter.HEIN_CARD_TO_TIME));
                        dicParam.Add("HEIN_CARD_ADDRESS", TreatmentWithPatientTypeAlter.HEIN_CARD_ADDRESS);
                    }
                    else
                    {
                        dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                        dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                        dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                        dicParam.Add("HEIN_CARD_ADDRESS", "");
                    }

                    var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == TreatmentWithPatientTypeAlter.PATIENT_TYPE_CODE);
                    if (patientType != null)
                        dicParam.Add("PATIENT_TYPE_NAME", patientType.PATIENT_TYPE_NAME);
                    else
                        dicParam.Add("PATIENT_TYPE_NAME", "");

                    var treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == TreatmentWithPatientTypeAlter.TREATMENT_TYPE_CODE);
                    if (treatmentType != null)
                        dicParam.Add("TREATMENT_TYPE_NAME", treatmentType.TREATMENT_TYPE_NAME);
                    else
                        dicParam.Add("TREATMENT_TYPE_NAME", "");

                    dicParam.Add("TREATMENT_ICD_CODE", TreatmentWithPatientTypeAlter.ICD_CODE);
                    dicParam.Add("TREATMENT_ICD_NAME", TreatmentWithPatientTypeAlter.ICD_NAME);
                    dicParam.Add("TREATMENT_ICD_SUB_CODE", TreatmentWithPatientTypeAlter.ICD_SUB_CODE);
                    dicParam.Add("TREATMENT_ICD_TEXT", TreatmentWithPatientTypeAlter.ICD_TEXT);

                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(TreatmentWithPatientTypeAlter, this.dicParam, false);

                    int AGE_NUM = Inventec.Common.DateTime.Calculation.Age(TreatmentWithPatientTypeAlter.TDL_PATIENT_DOB, TreatmentWithPatientTypeAlter.IN_TIME);
                    dicParam.Add("AGE_NUM", AGE_NUM);
                }
                else
                {
                    dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                    dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                    dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                    dicParam.Add("HEIN_CARD_ADDRESS", "");
                    var typeAlter = new HisTreatmentWithPatientTypeInfoSDO();
                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(typeAlter, this.dicParam, false);
                }

                //if (patient != null)
                //    AddKeyIntoDictionaryPrint<ADO.PatientADO>(patient, this.dicParam, false);
                CommonParam paramCommon = new CommonParam();



                MOS.Filter.HisSereServView4Filter filter = new MOS.Filter.HisSereServView4Filter();
                filter.ID = sereServExtPrint.SERE_SERV_ID;
                var rs = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<V_HIS_SERE_SERV_4>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GETVIEW_4, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (rs != null && rs.Count > 0)
                {
                    sereServ = rs[0];

                }

                HIS_SERE_SERV sereS = new HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereS, this.sereServ);
                AddKeyIntoDictionaryPrint<HIS_SERVICE_REQ>(this.currentServiceReq, this.dicParam, true);
                AddKeyIntoDictionaryPrint<HIS_SERE_SERV>(sereS, this.dicParam, false);
                AddKeyIntoDictionaryPrint<HIS_SERE_SERV_EXT>(this.sereServExtPrint, this.dicParam, true);

                if (this.sereServExtPrint != null)
                {
                    if (!dicParam.ContainsKey("END_TIME_FULL_STR"))
                        dicParam.Add("END_TIME_FULL_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExtPrint.END_TIME ?? 0));
                    else
                        dicParam["END_TIME_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExtPrint.END_TIME ?? 0);
                    if (!dicParam.ContainsKey("BEGIN_TIME_FULL_STR"))
                        dicParam.Add("BEGIN_TIME_FULL_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExtPrint.BEGIN_TIME ?? 0));
                    else
                        dicParam["BEGIN_TIME_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
                                this.sereServExtPrint.BEGIN_TIME ?? 0);

                    if (this.sereServExtPrint.MACHINE_ID.HasValue)
                    {
                        var machine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == this.sereServExtPrint.MACHINE_ID.Value);
                        if (machine != null)
                        {
                            dicParam["MACHINE_NAME"] = machine.MACHINE_NAME;
                        }
                    }

                    if (sereServExtPrint.END_TIME.HasValue)
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(sereServExtPrint.END_TIME.Value);
                        dicParam["EXECUTE_TIME_FULL_STR"] = GetCurrentTimeSeparateBeginTime(sereServExtPrint.END_TIME.Value);
                    }
                    else if (sereServExtPrint.MODIFY_TIME.HasValue)
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(sereServExtPrint.MODIFY_TIME.Value);
                        dicParam["EXECUTE_TIME_FULL_STR"] = GetCurrentTimeSeparateBeginTime(sereServExtPrint.MODIFY_TIME.Value);
                    }
                    else
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = "";
                        dicParam["EXECUTE_TIME_FULL_STR"] = "";
                    }
                }
                else
                {
                    dicParam["EXECUTE_DATE_FULL_STR"] = "";
                    dicParam["EXECUTE_TIME_FULL_STR"] = "";
                    dicParam["MACHINE_NAME"] = "";
                }

                dicParam.Add("USER_NAME", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());

                //bỏ key để phục vụ đổ dữ liệu khi in
                foreach (var item in keyPrint)
                {
                    dicParam.Remove(item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintOption1(bool printNow, string content)
        {
            try
            {
                Dictionary<string, string> dicRtfText = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(content))
                {
                    dicRtfText["DESCRIPTION_WORD"] = content;

                    SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == "Mps000354");

                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.TreatmentWithPatientTypeAlter != null ? TreatmentWithPatientTypeAlter.TREATMENT_CODE : ""), "Mps000354", currentModule != null ? currentModule.RoomId : 0);

                    richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, sereServ.TDL_SERVICE_NAME, null, null, dicParam, dicImage, inputADO, dicRtfText, printNow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintOption2(bool printNow, string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content)) return;
                var printDocument = ProcessDocumentBeforePrint(content);
                if (printDocument == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("printDocument is null");
                    return;
                }

                if (printNow)
                {
                    printDocument.Print();
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printDocument.Print();
                }
                else
                {
                    printDocument.ShowPrintPreview();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private RichEditControl ProcessDocumentBeforePrint(string document)
        {
            RichEditControl result = null;
            try
            {
                if (document != null)
                {
                    result = new RichEditControl();
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = sereServ.SERVICE_REQ_ID;
                    var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    long? finishTime = null;
                    if (lstServiceReq != null && lstServiceReq.Count > 0)
                    {
                        finishTime = lstServiceReq.FirstOrDefault().FINISH_TIME;
                    }


                    result.RtfText = document;
                    if (string.IsNullOrEmpty(result.Text)) return null;
                    var tgkt = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.ThoiGianKetThuc");
                    string HideTimePrint = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.HideTimePrint");

                    if (!String.IsNullOrWhiteSpace(tgkt))
                    {
                        foreach (var section in result.Document.Sections)
                        {
                            if (HideTimePrint != "1")
                            {
                                section.Margins.HeaderOffset = 50;
                                section.Margins.FooterOffset = 50;
                                var myHeader = section.BeginUpdateHeader(DevExpress.XtraRichEdit.API.Native.HeaderFooterType.Odd);
                                //xóa header nếu có dữ liệu
                                myHeader.Delete(myHeader.Range);

                                myHeader.InsertText(myHeader.CreatePosition(0),
                                    String.Format(Inventec.Common.Resource.Get.Value("NgayIn",
                                    Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                                myHeader.Fields.Update();
                                section.EndUpdateHeader(myHeader);
                            }

                            string finishTimeStr = "";
                            if (finishTime.HasValue)
                            {
                                finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(finishTime.Value);
                            }

                            var rangeSeperators = result.Document.FindAll(tgkt, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                            if (rangeSeperators != null && rangeSeperators.Length > 0)
                            {
                                for (int i = 0; i < rangeSeperators.Length; i++)
                                    result.Document.Replace(rangeSeperators[i], finishTimeStr);
                            }
                        }
                    }

                    //key hiển thị màu trắng khi in sẽ thay key
                    if (sereServExtPrint != null)
                    {
                        //đổi về màu đen để hiển thị.
                        foreach (var key in keyPrint)
                        {
                            var rangeSeperators = result.Document.FindAll(key, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                            foreach (var rang in rangeSeperators)
                            {
                                CharacterProperties cp = result.Document.BeginUpdateCharacters(rang);
                                cp.ForeColor = Color.Black;
                                result.Document.EndUpdateCharacters(cp);
                            }
                        }

                        result.Document.ReplaceAll("<#CONCLUDE_PRINT;>", sereServExtPrint.CONCLUDE, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#NOTE_PRINT;>", sereServExtPrint.NOTE, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#DESCRIPTION_PRINT;>", sereServExtPrint.DESCRIPTION, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                        result.Document.ReplaceAll("<#CURRENT_USERNAME_PRINT;>", lstServiceReq.FirstOrDefault().EXECUTE_USERNAME, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);

                        foreach (var item in dicParam)
                        {
                            if (item.Value != null && CheckType(item.Value))
                            {
                                string key = string.Format("<#{0}_PRINT;>", item.Key);
                                var rangeSeperators = result.Document.FindAll(key, DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                                foreach (var rang in rangeSeperators)
                                {
                                    CharacterProperties cp = result.Document.BeginUpdateCharacters(rang);
                                    cp.ForeColor = Color.Black;
                                    result.Document.EndUpdateCharacters(cp);
                                }
                                result.Document.ReplaceAll(key, item.Value.ToString(), DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckType(object value)
        {
            bool result = false;
            try
            {
                result = value.GetType() == typeof(long) || value.GetType() == typeof(int) || value.GetType() == typeof(string) || value.GetType() == typeof(short) || value.GetType() == typeof(decimal) || value.GetType() == typeof(double) || value.GetType() == typeof(float);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private SAR.EFMODEL.DataModels.SAR_PRINT GetListPrintByDescriptionPrint(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt)
        {
            SAR.EFMODEL.DataModels.SAR_PRINT result = null;
            try
            {
                List<long> printIds = GetListPrintIdBySereServ(sereServExt);
                if (printIds != null && printIds.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    SAR.Filter.SarPrintFilter filter = new SAR.Filter.SarPrintFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IDs = printIds;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(ApiConsumer.SarRequestUriStore.SAR_PRINT_GET, ApiConsumer.ApiConsumers.SarConsumer, filter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<long> GetListPrintIdBySereServ(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT item)
        {
            List<long> result = new List<long>();
            try
            {
                if (!String.IsNullOrEmpty(item.DESCRIPTION_SAR_PRINT_ID))
                {
                    var arrIds = item.DESCRIPTION_SAR_PRINT_ID.Split(',', ';');
                    if (arrIds != null && arrIds.Length > 0)
                    {
                        foreach (var id in arrIds)
                        {
                            long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                            if (printId > 0)
                            {
                                result.Add(printId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool InKetQuaXetNghiem(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {

                WaitingManager.Show();
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                long treatmentId = currentServiceReqPrint.TREATMENT_ID;

                //Loai Patient_type_name
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ID = currentServiceReqPrint.ID;
                var _ServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                //Load Data Treatment
                HIS_TREATMENT _Treatment = new HIS_TREATMENT();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatmentId;
                _Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                patientTypeAlterFilter.TREATMENT_ID = treatmentId;
                patientTypeAlterFilter.ORDER_FIELD = "LOG_TIME";
                patientTypeAlterFilter.ORDER_DIRECTION = "DESC";
                patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, param).FirstOrDefault();

                //Mức hưởng BHYT
                decimal ratio_text = 0;
                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                if (patientTypeAlter != null)
                {
                    ratio_text = GetDefaultHeinRatioForView(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patientTypeAlter.RIGHT_ROUTE_CODE);
                }

                List<object> obj = new List<object>();
                obj.Add(patientTypeAlter);
                obj.Add(_ServiceReq);
                obj.Add(_Treatment);

                MOS.Filter.HisSereServViewFilter SereServfilter = new MOS.Filter.HisSereServViewFilter();

                SereServfilter.SERVICE_REQ_ID = this.currentServiceReqPrint.ID;
                SereServfilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                var lstSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, SereServfilter, param);

                _SereServNumOders = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                if (lstSereServ != null && lstSereServ.Count > 0)
                {
                    foreach (var itemss in lstSereServ)
                    {
                        MPS.Processor.Mps000014.PDO.SereServNumOder sereServNumOder = new MPS.Processor.Mps000014.PDO.SereServNumOder(itemss, BackendDataWorker.Get<V_HIS_SERVICE>());
                        _SereServNumOders.Add(sereServNumOder);
                    }
                    _SereServNumOders = _SereServNumOders.OrderByDescending(p => p.SERVICE_NUM_ODER).ThenBy(p => p.TDL_SERVICE_NAME).ToList();

                    HisSereServTeinViewFilter sereSerTeinFilter = new HisSereServTeinViewFilter();
                    sereSerTeinFilter.SERE_SERV_IDs = lstSereServ.Select(o => o.ID).ToList();
                    sereSerTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    this.lstSereServTein = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, sereSerTeinFilter, param);

                }
                //  AutoMapper.Mapper.CreateMap<ADO.HisSereServTeinSDO, V_HIS_SERE_SERV_TEIN>();
                var sereServTeins = AutoMapper.Mapper.Map<List<V_HIS_SERE_SERV_TEIN>>(lstSereServTein);

                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders2 = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders4 = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                Dictionary<long, List<MPS.Processor.Mps000014.PDO.SereServNumOder>> _SereServNumOderss = new Dictionary<long, List<MPS.Processor.Mps000014.PDO.SereServNumOder>>();


                foreach (var item in this._SereServNumOders)
                {
                    if (item.ServiceParentId == null)
                    {
                        if (!_SereServNumOderss.ContainsKey(0))
                        {
                            _SereServNumOderss[0] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                        }
                        _SereServNumOderss[0].Add(item);
                    }
                    else
                    {
                        _SereServNumOders2.Add(item);
                    }
                }
                foreach (var item in _SereServNumOders2)
                {
                    var parent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.ServiceParentId);
                    if (parent.PARENT_ID == null)
                    {
                        if (!_SereServNumOderss.ContainsKey(parent.ID))
                        {
                            _SereServNumOderss[parent.ID] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                        }
                        _SereServNumOderss[parent.ID].Add(item);
                    }
                    else
                    {
                        _SereServNumOders4.Add(item);
                    }
                    _SereServNumOders4.GroupBy(o => o.GrandParentID);

                }
                foreach (var item in _SereServNumOders4)
                {
                    if (!_SereServNumOderss.ContainsKey(item.GrandParentID.Value))
                    {
                        _SereServNumOderss[item.GrandParentID.Value] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                    }
                    _SereServNumOderss[item.GrandParentID.Value].Add(item);
                }


                foreach (var item in _SereServNumOderss.Keys)
                {

                    MPS.Processor.Mps000014.PDO.Mps000014PDO mps000014RDO = new MPS.Processor.Mps000014.PDO.Mps000014PDO(
                        obj.ToArray(),
                        _SereServNumOderss[item],
                        sereServTeins,
                        ratio_text,
                        BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                        _Treatment.TDL_PATIENT_GENDER_ID,
                        BackendDataWorker.Get<V_HIS_SERVICE>()
                        );

                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_Treatment != null ? _Treatment.TREATMENT_CODE : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                    Inventec.Common.Logging.LogSystem.Info(_Treatment.TREATMENT_CODE);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public decimal GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            decimal result = 0;
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessDataCheckedToChangeRoom()
        {
            try
            {
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    var checkFinishStatus = listServiceReq.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT).ToList();
                    if (checkFinishStatus != null && checkFinishStatus.Count > 0)
                    {
                        string serviceCode = "";
                        List<String> lstCode = new List<string>();
                        foreach (var item in checkFinishStatus)
                        {
                            string code = item.SERVICE_REQ_CODE;
                            lstCode.Add(code);
                        }
                        serviceCode = String.Join(",", lstCode);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Chỉ định " + serviceCode + " đã kết thúc, không cho phép chuyển phòng", Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (listServiceReq.Count == 1)
                        {
                            Btn_ChangeRoom_ButtonClick();
                        }
                        else
                        {
                            frmChangeRoom changeRoom = new frmChangeRoom(currentModule, listServiceReq);
                            changeRoom.ShowDialog();
                            btnFind_Click(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataCheckedToDelete()
        {
            try
            {
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    if (MessageBox.Show(Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        bool checkParent = false;
                        foreach (var item in listServiceReq)
                        {
                            if (CheckParentBeforeDelete(item.ID))
                            {
                                checkParent = true;
                                break;
                            }
                        }

                        if (checkParent && DevExpress.XtraEditors.XtraMessageBox.Show("Đã có y lệnh đính kèm (CLS). Bạn có chắc chắn muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }

                        CommonParam param = new CommonParam();
                        bool success = true;
                        WaitingManager.Show();

                        foreach (var item in listServiceReq)
                        {
                            MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                            sdo.Id = item.ID;
                            sdo.RequestRoomId = this.currentModule.RoomId;
                            success = success && new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SERVICE_REQ_DELETE, ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        }

                        FillDataToGrid();

                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this, param, success);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataCheckedToPrint()
        {
            try
            {
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    //if (HisConfigCFG.IsmergeOptionMergePrint)
                    //{
                    //    MPS.PrintStorage.Clear();
                    //}
                    foreach (var item in listServiceReq)
                    {
                        ExecuteBefPrint(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.HitTest == GridHitTest.Column)
                {
                    if (hi.Column.FieldName == "isCheck")
                    {
                        var listData = gridControlServiceReq.DataSource as List<ADO.ServiceReqADO>;
                        if (listData != null && listData.Count > 0)
                        {
                            gridViewServiceReq.BeginUpdate();
                            if (isCheckAll)
                            {
                                isCheckAll = false;
                                foreach (var item in listData)
                                {
                                    item.isCheck = true;
                                }
                                this.gridColumn_ServiceReq_Choose.Image = this.imageListCheck.Images[3];
                            }
                            else
                            {
                                isCheckAll = true;
                                foreach (var item in listData)
                                {
                                    item.isCheck = false;
                                }
                                this.gridColumn_ServiceReq_Choose.Image = this.imageListCheck.Images[4];
                            }
                            gridViewServiceReq.EndUpdate();
                        }
                    }
                }

                if (hi.InRowCell)
                {
                    if (hi.Column.FieldName == "SERVICE_REQ_CODE" || hi.Column.FieldName == "TDL_TREATMENT_CODE" || hi.Column.FieldName == "TDL_PATIENT_CODE" || hi.Column.FieldName == "TDL_PATIENT_NAME")
                    {
                        gridViewServiceReq.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDownFocused;
                    }
                    else
                    {
                        gridViewServiceReq.OptionsBehavior.EditorShowMode = EditorShowMode.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdViewSereServServiceReq_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ADO.ListMedicineADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "IS_ACCEPTING_NO_EXECUTE_STR")
                        {
                            if (data.IS_ACCEPTING_NO_EXECUTE == 1)
                                e.Value = "true";
                        }
                        else if (e.Column.FieldName == "PRES_AMOUNT_DISPLAY")
                        {
                            e.Value = data.PRES_AMOUNT.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdViewSereServServiceReq_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                this.rightClickData = null;
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = grdViewSereServServiceReq.GetVisibleRowHandle(hi.RowHandle);

                    var row = (ListMedicineADO)grdViewSereServServiceReq.GetRow(rowHandle);
                    if (row != null)
                    {
                        this.rightClickData = row;

                        //grdViewSereServServiceReq.OptionsSelection.EnableAppearanceFocusedCell = true;
                        //grdViewSereServServiceReq.OptionsSelection.EnableAppearanceFocusedRow = true;

                        BarManager barManager = new BarManager();
                        barManager.Form = this;

                        PopupMenuProcessorMedicine popupMenuProcessor = new PopupMenuProcessorMedicine(row, barManager, MouseRight_Click, this.currentWorkPlace);
                        popupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewSereServServiceReq_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var subPress = Inventec.Common.TypeConvert.Parse.ToInt32((view.GetRowCellValue(e.RowHandle, "subPress") ?? "0").ToString());
                    var kind = Inventec.Common.TypeConvert.Parse.ToInt32((view.GetRowCellValue(e.RowHandle, "kind") ?? "").ToString());

                    if (subPress == 1 && kind == 1)
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Italic;
                        var isStartMark = (view.GetRowCellValue(e.RowHandle, "isStartMark") ?? "").ToString();

                        if (!string.IsNullOrEmpty(isStartMark))
                            e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold;
                    }
                    else if (kind == 1)
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Green;
                    }


                    var IS_NO_EXECUTE = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(e.RowHandle, "IS_NO_EXECUTE") ?? "").ToString());

                    if (IS_NO_EXECUTE == Base.GlobalStore.IS_TRUE)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdViewSereServServiceReq_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                string rowValue = Convert.ToString(grdViewSereServServiceReq.GetGroupRowValue(e.RowHandle, GridColumnInReqExeute));
                info.GroupText = rowValue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdViewSereServServiceReq_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var dataserviceReqSelect = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    var data = (ListMedicineADO)grdViewSereServServiceReq.GetRow(e.RowHandle);
                    var hisExecuteRoomData = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == data.TDL_EXECUTE_ROOM_ID);
                    string creator = (grdViewSereServServiceReq.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();

                    long typeIdCheckForButtonEdit = long.Parse((grdViewSereServServiceReq.GetRowCellValue(e.RowHandle, "TDL_SERVICE_TYPE_ID") ?? "").ToString());
                    if (e.Column.FieldName == "btnView_Tab")
                    {
                        if (dataserviceReqSelect.EXE_SERVICE_MODULE_ID == null ||
                            (dataserviceReqSelect.EXE_SERVICE_MODULE_ID != IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__KHAM &&
                            dataserviceReqSelect.EXE_SERVICE_MODULE_ID != IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__XN &&
                            dataserviceReqSelect.EXE_SERVICE_MODULE_ID != IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__XULYXN &&
                            dataserviceReqSelect.EXE_SERVICE_MODULE_ID != IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__PHCN &&
                            dataserviceReqSelect.EXE_SERVICE_MODULE_ID != IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__XULYDV &&
                               dataserviceReqSelect.EXE_SERVICE_MODULE_ID != IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__PTTT) ||
                            (HisConfigCFG.ShowResultWhenReqComplete == "1" && dataserviceReqSelect.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT) || (HisConfigCFG.ShowResultWhenReqComplete == "2" && dataserviceReqSelect.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && dataserviceReqSelect.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT))
                        {

                            e.RepositoryItem = repositoryItemTextEditDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonView;
                        }
                    }
                    else if (e.Column.FieldName == "IS_ACCEPTING_NO_EXECUTE_STR")
                    {
                        if (e.CellValue == "true")
                        {
                            e.RepositoryItem = repositoryItemButtonIsAcceptNoExecute;
                        }
                    }
                    if (e.Column.FieldName == "btnPrint_Tab")
                    {
                        if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT ||
                            typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT ||
                            typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN)
                        {
                            e.RepositoryItem = repositoryItemButtonPrint;
                        }
                        else
                            e.RepositoryItem = repositoryItemTextEditDisable;
                    }
                    else if (e.Column.FieldName == "SereSerDeleteDQ")
                    {
                        if ((creator == this.loginName || CheckLoginAdmin.IsAdmin(this.loginName))
                        && data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                        && currentServiceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT
                        && hisExecuteRoomData.ALLOW_NOT_CHOOSE_SERVICE == (short)1
                        )
                        {
                            e.RepositoryItem = repositoryItemButtonEditDeleteEna;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEditDeleteDis;
                        }

                    }
                    if (e.Column.FieldName == "IS_CONFIRM_NO_EXCUTE")
                    {
                        if (data.IS_CONFIRM_NO_EXCUTE == 1)
                        {
                            e.RepositoryItem = repositoryItemButtonEditServiceConfirmEna;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEditServiceConfirmDis;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceReqStt_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.SERVICE_REQ_STT_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceReqStt_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboServiceReqType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceReqType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    btnFind.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit1View_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceReqType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.SERVICE_REQ_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEditChoose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                if (row != null)
                {
                    row.isCheck = !row.isCheck;
                    var listData = gridControlServiceReq.DataSource as List<ADO.ServiceReqADO>;
                    bool hasCheck = false;
                    bool isAll = false;

                    var listCheck = listData.Where(o => o.isCheck).ToList();
                    if (listCheck != null && listCheck.Count > 0)
                    {
                        if (listCheck.Count == listData.Count)
                        {
                            isAll = true;
                        }
                        else
                        {
                            hasCheck = true;
                        }
                    }

                    if (isAll)
                    {
                        isCheckAll = false;
                        this.gridColumn_ServiceReq_Choose.Image = this.imageListCheck.Images[3];
                    }
                    else if (hasCheck)
                    {
                        this.gridColumn_ServiceReq_Choose.Image = this.imageListCheck.Images[5];
                    }
                    else
                    {
                        isCheckAll = true;
                        this.gridColumn_ServiceReq_Choose.Image = this.imageListCheck.Images[4];
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region click
        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                    {
                        FillDataToGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        FillDataToGrid();
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
                    if (!String.IsNullOrEmpty(txtKeyword.Text))
                    {
                        FillDataToGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void repositoryItemBtnServiceReqDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                bool IsBreak = false;
                List<string> lstServiceName = new List<string>();
                if (!CheckLoginAdmin.IsAdmin(this.loginName))
                {
                    await GetSereServ(data);
                    var sereServ = grdSereServServiceReq.DataSource as List<ListMedicineADO>;
                    if (sereServ != null && sereServ.Count > 0)
                    {
                        foreach (var item in sereServ)
                        {
                            var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == item.PATIENT_TYPE_ID);
                            if (patientType != null && patientType.IS_NOT_EDIT_ASSIGN_SERVICE == 1)
                            {
                                lstServiceName.Add(item.TDL_SERVICE_NAME);
                                IsBreak = true;
                            }
                        }
                    }
                }
                if (IsBreak)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Dịch vụ {0} có đối tượng thanh toán được tích \"Không cho phép sửa xóa dịch vụ đã chỉ định\", vui lòng liên hệ với quản trị hệ thống", string.Join(", ", lstServiceName)), "Thông báo");
                    return;
                }
                if (gridViewServiceReq.FocusedRowHandle >= 0 && !IsBreak)
                {
                    CommonParam paramEmr = new CommonParam();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    if (data == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Data thuc hien huy yeu cau dich vu null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                        return;
                    }
                    if (data != null)
                    {
                        EmrDocumentFilter filter = new EmrDocumentFilter();
                        Inventec.Common.Logging.LogSystem.Debug("TDL_TREATMENT_CODE_______________________________________" + data.TDL_TREATMENT_CODE);
                        filter.TREATMENT_CODE__EXACT = data.TDL_TREATMENT_CODE;
                        filter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN;
                        var resultEmrDocument = new BackendAdapter(paramEmr).Get<List<EMR_DOCUMENT>>("api/EmrDocument/Get", ApiConsumers.EmrConsumer, filter, paramEmr);
                        if (resultEmrDocument != null && resultEmrDocument.Count() > 0)
                        {
                            resultEmrDocument = resultEmrDocument.Where(o => o.IS_DELETE != 1).ToList();
                            var checkServiceReqCode = "SERVICE_REQ_CODE:" + data.SERVICE_REQ_CODE;

                            var resultEmrDocumentLast = new List<EMR_DOCUMENT>();
                            foreach (var item in resultEmrDocument)
                            {
                                if (item.HIS_CODE != null && item.HIS_CODE.Contains(checkServiceReqCode))
                                {
                                    resultEmrDocumentLast.Add(item);
                                }
                            }
                            if (resultEmrDocumentLast.Count() > 0 && resultEmrDocumentLast != null)
                            {
                                #region
                                if (MessageBox.Show("Y lệnh này đã tồn tại văn bản ký, bạn có muốn hủy dữ liệu?", Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    WaitingManager.Show();
                                    MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                                    sdo.Id = data.ID;
                                    sdo.RequestRoomId = this.currentModule.RoomId;
                                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SERVICE_REQ_DELETE, ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                                    WaitingManager.Hide();
                                    if (success == true)
                                    {
                                        var result = false;
                                        foreach (var item in resultEmrDocumentLast)
                                        {
                                            result = new BackendAdapter(paramEmr).Post<bool>("api/EmrDocument/Delete", ApiConsumers.EmrConsumer, item.ID, paramEmr);
                                            if (result)
                                            {
                                                FillDataToGrid();
                                            }
                                        }
                                        MessageManager.Show(this, paramEmr, result);
                                    }
                                    #region Show message
                                    if (success == false)
                                    {
                                        MessageManager.Show(this, param, success);
                                    }
                                    #endregion

                                    #region Process has exception
                                    SessionManager.ProcessTokenLost(param);
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region
                                if (MessageBox.Show(Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    if (CheckParentBeforeDelete(data.ID) && DevExpress.XtraEditors.XtraMessageBox.Show("Đã có y lệnh đính kèm (CLS). Bạn có chắc chắn muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                                    {
                                        return;
                                    }
                                    WaitingManager.Show();
                                    MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                                    sdo.Id = data.ID;
                                    sdo.RequestRoomId = this.currentModule.RoomId;
                                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SERVICE_REQ_DELETE, ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                                    WaitingManager.Hide();
                                    if (success)
                                    {
                                        FillDataToGrid();
                                    }
                                    #region Show message
                                    MessageManager.Show(this, param, success);
                                    #endregion
                                    #region Process has exception
                                    SessionManager.ProcessTokenLost(param);
                                    #endregion
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            #region
                            if (MessageBox.Show(Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                if (CheckParentBeforeDelete(data.ID) && DevExpress.XtraEditors.XtraMessageBox.Show("Đã có y lệnh đính kèm (CLS). Bạn có chắc chắn muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    return;
                                }
                                WaitingManager.Show();
                                MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                                sdo.Id = data.ID;
                                sdo.RequestRoomId = this.currentModule.RoomId;
                                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SERVICE_REQ_DELETE, ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                                WaitingManager.Hide();
                                if (success)
                                {
                                    FillDataToGrid();
                                }
                                #region Show message
                                MessageManager.Show(this, param, success);
                                #endregion
                                #region Process has exception
                                SessionManager.ProcessTokenLost(param);
                                #endregion
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }



        private bool CheckParentBeforeDelete(long _serviceReqId)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.PARENT_ID = _serviceReqId;
                var serviceReqParent = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param);
                if (serviceReqParent != null && serviceReqParent.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
        }

        private async void repositoryItemBtnServiceReqEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    bool IsBreak = false;
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    List<string> lstServiceName = new List<string>();
                    if (!CheckLoginAdmin.IsAdmin(this.loginName))
                    {
                        await GetSereServ(data);
                        var sereServ = grdSereServServiceReq.DataSource as List<ListMedicineADO>;
                        if (sereServ != null && sereServ.Count > 0)
                        {
                            foreach (var item in sereServ)
                            {
                                var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == item.PATIENT_TYPE_ID);
                                if (patientType != null && patientType.IS_NOT_EDIT_ASSIGN_SERVICE == 1)
                                {
                                    lstServiceName.Add(item.TDL_SERVICE_NAME);
                                    IsBreak = true;
                                }
                            }
                        }
                    }
                    if (IsBreak)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Dịch vụ {0} có đối tượng thanh toán được tích \"Không cho phép sửa xóa dịch vụ đã chỉ định\", vui lòng liên hệ với quản trị hệ thống", string.Join(", ", lstServiceName)), "Thông báo");
                        return;
                    }
                    if (data != null && !IsBreak)// && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    {
                        //65930
                        if (HisConfigCFG.AutoDeleteEmrDocumentWhenEditReq == "1")
                        {
                            EmrDocumentViewFilter emrDocumentFilter = new EmrDocumentViewFilter();
                            emrDocumentFilter.TREATMENT_CODE__EXACT = data.TDL_TREATMENT_CODE;
                            emrDocumentFilter.IS_DELETE = false;
                            var documents = new BackendAdapter(new CommonParam()).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrDocumentFilter, null);
                            if (documents != null && documents.Count() > 0)
                            {
                                var checkServiceReqCode = "SERVICE_REQ_CODE:" + data.SERVICE_REQ_CODE;
                                var resultEmrDocumentLast = documents.Where(o => o.DOCUMENT_TYPE_ID != IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT && !string.IsNullOrEmpty(o.HIS_CODE) && o.HIS_CODE.Contains(checkServiceReqCode));
                                if (resultEmrDocumentLast != null && resultEmrDocumentLast.Count() > 0)
                                {
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Y lệnh đã tồn tại văn bản ký, tiếp tục sẽ tự động Xóa văn bản ký hiện tại. Bạn có muốn tiếp tục?", Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                        return;

                                    WaitingManager.Show();
                                    foreach (var item in resultEmrDocumentLast)
                                    {
                                        var result = new BackendAdapter(new CommonParam()).Post<bool>("api/EmrDocument/Delete", ApiConsumers.EmrConsumer, item.ID, null);
                                    }
                                    WaitingManager.Hide();
                                }
                            }
                        }
                        /////

                        var paramCommon = new CommonParam();
                        var treatment = new HIS_TREATMENT();
                        HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                        treatFilter.ID = data.TREATMENT_ID;
                        var currentTreats = new BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (currentTreats != null && currentTreats.Count == 1)
                        {
                            var treat = currentTreats.FirstOrDefault();
                            if (treat.IS_PAUSE == Base.GlobalStore.IS_TRUE || treat.IS_ACTIVE != Base.GlobalStore.IS_TRUE)
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Resources.ResourceMessage.HoSoDieuTriDangTamKhoa);
                                MessageBox.Show(Resources.ResourceMessage.HoSoDieuTriDangTamKhoa);
                                return;
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Resources.ResourceMessage.KhongTimThayHoSoDieuTri);
                            MessageBox.Show(Resources.ResourceMessage.KhongTimThayHoSoDieuTri);
                            return;
                        }

                        this.serviceReqPrintRaw = GetServiceReqForPrint(data.ID);

                        if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                        {
                            WaitingManager.Show();
                            List<object> sendObj = new List<object>() { this.serviceReqPrintRaw.ID };
                            CallModule("HIS.Desktop.Plugins.UpdateExamServiceReq", sendObj);
                            WaitingManager.Hide();
                        }
                        else if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                            data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT ||
                            data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                        {
                            WaitingManager.Show();
                            AssignPrescriptionEditADO assignEditADO = null;
                            var serviceReq = new HIS_SERVICE_REQ();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(serviceReq, this.serviceReqPrintRaw);
                            HisExpMestFilter expfilter = new HisExpMestFilter();
                            expfilter.SERVICE_REQ_ID = this.serviceReqPrintRaw.ID;
                            var expMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                            if (expMests != null && expMests.Count == 1)
                            {
                                var expMest = expMests.FirstOrDefault();
                                if (expMest.IS_NOT_TAKEN.HasValue && expMest.IS_NOT_TAKEN.Value == 1)
                                {
                                    WaitingManager.Hide();
                                    MessageBox.Show(Resources.ResourceMessage.DonKhongLayKhongChoPhepSua);
                                    return;
                                }
                                assignEditADO = new AssignPrescriptionEditADO(serviceReq, expMest, FillDataApterSave);
                            }
                            else
                            {
                                assignEditADO = new AssignPrescriptionEditADO(serviceReq, null, FillDataApterSave);
                            }

                            if (data.IS_EXECUTE_KIDNEY_PRES == 1)
                            {
                                AssignPrescriptionKidneyADO assignPrescriptionKidneyADO = new AssignPrescriptionKidneyADO();
                                assignPrescriptionKidneyADO.AssignPrescriptionEditADO = assignEditADO;
                                List<object> sendObj = new List<object>() { assignPrescriptionKidneyADO };

                                CallModule("HIS.Desktop.Plugins.AssignPrescriptionKidney", sendObj);
                            }
                            else
                            {
                                var assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(data.TREATMENT_ID, 0, serviceReq.ID);
                                assignServiceADO.GenderName = data.TDL_PATIENT_GENDER_NAME;
                                assignServiceADO.PatientDob = data.TDL_PATIENT_DOB;
                                assignServiceADO.PatientName = data.TDL_PATIENT_NAME;

                                assignServiceADO.AssignPrescriptionEditADO = assignEditADO;

                                List<object> sendObj = new List<object>() { assignServiceADO };

                                if (data.PRESCRIPTION_TYPE_ID == 1)
                                {
                                    if (data.PARENT_ID.HasValue)
                                    {
                                        HisSereServViewFilter ssfilter = new HisSereServViewFilter();
                                        ssfilter.SERVICE_REQ_ID = data.PARENT_ID;
                                        var listSereServ = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, ssfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                                        if (listSereServ != null && listSereServ.Count > 0 && listSereServ.FirstOrDefault().TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH && listSereServ.FirstOrDefault().TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                                            assignServiceADO.SereServ = listSereServ.FirstOrDefault();
                                    }
                                    CallModule("HIS.Desktop.Plugins.AssignPrescriptionPK", sendObj);
                                }
                                else if (data.PRESCRIPTION_TYPE_ID == 2)
                                {
                                    CallModule("HIS.Desktop.Plugins.AssignPrescriptionYHCT", sendObj);
                                }
                                else if (data.PRESCRIPTION_TYPE_ID == 3)
                                {
                                    CallModule("HIS.Desktop.Plugins.AssignPrescriptionCLS", sendObj);
                                }
                            }

                            WaitingManager.Hide();
                        }
                        else if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM)
                        {
                            // MessageManager.Show(Resources.ResourceMessage.DonMauKhongChoPhepSua);
                            var serviceReq = new HIS_SERVICE_REQ();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(serviceReq, this.serviceReqPrintRaw);

                            HIS.Desktop.ADO.AssignBloodADO assignBloodADO = new HIS.Desktop.ADO.AssignBloodADO(data.TREATMENT_ID, 0, 0);
                            assignBloodADO.PatientDob = data.TDL_PATIENT_DOB;
                            assignBloodADO.DgProcessDataResult = FillDataApterSave;
                            assignBloodADO.PatientName = data.TDL_PATIENT_NAME;
                            assignBloodADO.GenderName = data.TDL_PATIENT_GENDER_NAME;
                            List<object> sendObj = new List<object>() { assignBloodADO, serviceReq };
                            CallModule("HIS.Desktop.Plugins.HisAssignBlood", sendObj);
                        }
                        else if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN)
                        {
                            AssignServiceEditADO ado = new AssignServiceEditADO(data.ID, data.INTRUCTION_TIME, RefreshClick);
                            List<object> sendObj = new List<object>() { ado, currentModuleBase };
                            CallModule("HIS.Desktop.Plugins.AssignNutritionEdit", sendObj);
                        }
                        else
                        {
                            if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            {
                                if (data.SAMPLE_TIME != null)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Y lệnh đã thực hiện lấy mẫu. Bạn phải thực hiện bỏ tích lấy mẫu.", Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK);
                                    return;
                                }
                            }
                            AssignServiceEditADO assignServiceEditADO = new AssignServiceEditADO(data.ID, data.INTRUCTION_TIME, (HIS.Desktop.Common.RefeshReference)RefreshClick);
                            List<object> sendObj = new List<object>() { assignServiceEditADO };
                            CallModule("HIS.Desktop.Plugins.AssignServiceEdit", sendObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnServiceReqPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    WaitingManager.Hide();
                    this.currentServiceReqPrint = (ADO.ServiceReqADO)gridViewServiceReq.GetRow(gridViewServiceReq.FocusedRowHandle);
                    if (this.currentServiceReqPrint != null)
                    {
                        this.serviceReqPrintRaw = GetServiceReqForPrint(this.currentServiceReqPrint.ID);
                        WaitingManager.Hide();
                        if (currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                            currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                            currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                        {
                            this.prescriptionPrint = null;
                            HisExpMestFilter expfilter = new HisExpMestFilter();
                            expfilter.SERVICE_REQ_ID = currentServiceReqPrint.ID;
                            var expMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                            if (expMest != null && expMest.Count > 0)
                            {
                                this.prescriptionPrint = expMest.FirstOrDefault();
                                if (this.prescriptionPrint.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                                {
                                    PrintPopupMenuProcessor = new PrintPopupMenuProcessor(PrintMedicine_Click, barManager1, this.loginName);
                                    PrintPopupMenuProcessor.InitMenu();
                                }
                            }
                            else
                            {
                                PrintPopupMenuProcessor = new PrintPopupMenuProcessor(PrintMedicine_Click, barManager1, this.loginName);
                                PrintPopupMenuProcessor.InitMenu();
                            }
                        }
                        else if (currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM)
                        {
                            PrintBlood();
                        }
                        else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                        {
                            PrintPopupMenuProcessor = new PrintPopupMenuProcessor(PrintExam_Click, barManager1, this.loginName);
                            PrintPopupMenuProcessor.InitMenuKham(this.currentServiceReqPrint);
                        }
                        else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                        {
                            PrintPopupMenuProcessor = new PrintPopupMenuProcessor(PrintTest_Click, barManager1, this.loginName);
                            PrintPopupMenuProcessor.InitMenuXetNghiem();
                        }
                        else
                            ProcessingPrint();
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                var sereServRow = (ADO.ListMedicineADO)grdViewSereServServiceReq.GetFocusedRow();


                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServRow), sereServRow));
                if (data != null)// && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                {
                    if (sereServRow != null)
                    {
                        List<object> sendObj = new List<object>();//{ sereServRow.ID }

                        if (data.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__KHAM)
                        {
                            sendObj.Add(sereServRow.ID);
                            CallModule("HIS.Desktop.Plugins.ExamServiceReqResult", sendObj);
                        }
                        else if (data.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__XN)
                        {
                            if (data.IS_ANTIBIOTIC_RESISTANCE == 1)
                            {
                                AutoMapper.Mapper.CreateMap<ADO.ListMedicineADO, HIS_SERE_SERV>();
                                HIS_SERE_SERV sereserv = AutoMapper.Mapper.Map<ADO.ListMedicineADO, HIS_SERE_SERV>(sereServRow);
                                sendObj.Add(sereserv);
                                CallModule("HIS.Desktop.Plugins.SereServTeinBacterium", sendObj);
                            }
                            else
                            {
                                AutoMapper.Mapper.CreateMap<ADO.ListMedicineADO, HIS_SERE_SERV>();
                                HIS_SERE_SERV sereserv = AutoMapper.Mapper.Map<ADO.ListMedicineADO, HIS_SERE_SERV>(sereServRow);
                                sendObj.Add(sereserv);
                                CallModule("HIS.Desktop.Plugins.SereServTein", sendObj);
                            }
                        }
                        else if (data.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__XULYXN ||
                            data.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__PHCN ||
                            data.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__XULYDV ||
                            (data.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__PTTT && sereServRow.IS_SENT_EXT == 1))
                        {
                            sendObj.Add(sereServRow.ID);
                            CallModule("HIS.Desktop.Plugins.ServiceReqResultView", sendObj);
                        }
                        //else if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                        //{
                        //    AutoMapper.Mapper.CreateMap<ADO.ListMedicineADO, HIS_SERE_SERV>();
                        //    HIS_SERE_SERV sereserv = AutoMapper.Mapper.Map<ADO.ListMedicineADO, HIS_SERE_SERV>(sereServRow);
                        //    sendObj.Add(sereserv);
                        //    if (data.IS_ANTIBIOTIC_RESISTANCE == 1)
                        //    {
                        //        CallModule("HIS.Desktop.Plugins.SereServTeinBacterium", sendObj);
                        //    }
                        //    else
                        //    {
                        //        CallModule("HIS.Desktop.Plugins.SereServTein", sendObj);
                        //    }
                        //}
                        //else if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                        //    data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM ||
                        //    data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                        //    data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                        //{
                        //    MessageBox.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                        //}
                        //else
                        //{
                        //    sendObj.Add(sereServRow.ID);
                        //    CallModule("HIS.Desktop.Plugins.ServiceReqResultView", sendObj);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnBieuMauKhac_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    this.currentServiceReqPrint = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (this.currentServiceReqPrint != null)
                    {
                        WaitingManager.Show();
                        if (this.currentServiceReqPrint != null && this.currentServiceReqPrint.JSON_PRINT_ID != null)
                        {
                            HIS.Desktop.ADO.SarPrintADO ado = new SarPrintADO();
                            ado.JSON_PRINT_ID = this.currentServiceReqPrint.JSON_PRINT_ID;
                            ado.JsonPrintResult = JsonPrintResult;
                            List<object> sendObj = new List<object>() { ado };
                            CallModule("SAR.Desktop.Plugins.SarPrintList", sendObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterSave(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_SERVICE_REQ GetServiceReqForPrint(long serviceReqId)
        {
            V_HIS_SERVICE_REQ result = new V_HIS_SERVICE_REQ();
            try
            {
                if (serviceReqId > 0)
                {
                    HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                    serviceReqFilter.ID = serviceReqId;
                    serviceReqFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var serviceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (serviceReq != null && serviceReq.Count > 0)
                    {
                        result = serviceReq.FirstOrDefault();
                        //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(result, serviceReq.FirstOrDefault());

                        //result.TREATMENT_CODE = serviceReq.FirstOrDefault().TDL_TREATMENT_CODE;

                        //var executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.FirstOrDefault().EXECUTE_ROOM_ID);
                        //if (executeRoom != null)
                        //{
                        //    result.EXECUTE_DEPARTMENT_CODE = executeRoom.DEPARTMENT_CODE;
                        //    result.EXECUTE_DEPARTMENT_ID = executeRoom.DEPARTMENT_ID;
                        //    result.EXECUTE_DEPARTMENT_NAME = executeRoom.DEPARTMENT_NAME;
                        //    result.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                        //    result.EXECUTE_ROOM_ID = executeRoom.ID;
                        //    result.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                        //    result.EXECUTE_ROOM_ADDRESS = executeRoom.ADDRESS;
                        //}
                        //var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.FirstOrDefault().REQUEST_ROOM_ID);
                        //if (reqRoom != null)
                        //{
                        //    result.REQUEST_DEPARTMENT_CODE = reqRoom.DEPARTMENT_CODE;
                        //    result.REQUEST_DEPARTMENT_ID = reqRoom.DEPARTMENT_ID;
                        //    result.REQUEST_DEPARTMENT_NAME = reqRoom.DEPARTMENT_NAME;
                        //    result.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                        //    result.REQUEST_ROOM_ID = reqRoom.ID;
                        //    result.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                        //}

                        //var reqStt = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().FirstOrDefault(o => o.ID == serviceReq.FirstOrDefault().SERVICE_REQ_STT_ID);
                        //if (reqStt != null)
                        //{
                        //    result.SERVICE_REQ_STT_CODE = reqStt.SERVICE_REQ_STT_CODE;
                        //    result.SERVICE_REQ_STT_NAME = reqStt.SERVICE_REQ_STT_NAME;
                        //    result.SERVICE_REQ_STT_ID = reqStt.ID;
                        //}
                        //var reqType = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(o => o.ID == serviceReq.FirstOrDefault().SERVICE_REQ_TYPE_ID);
                        //if (reqType != null)
                        //{
                        //    result.SERVICE_REQ_TYPE_CODE = reqType.SERVICE_REQ_TYPE_CODE;
                        //    result.SERVICE_REQ_TYPE_NAME = reqType.SERVICE_REQ_TYPE_NAME;
                        //    result.SERVICE_REQ_TYPE_ID = reqType.ID;
                        //}

                        //if (result.SAMPLE_ROOM_ID.HasValue)
                        //{
                        //    var sampleRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == result.SAMPLE_ROOM_ID.Value);
                        //    if (sampleRoom != null)
                        //    {
                        //        result.SAMPLE_ROOM_CODE = sampleRoom.ROOM_CODE;
                        //        result.SAMPLE_ROOM_NAME = sampleRoom.ROOM_NAME;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new V_HIS_SERVICE_REQ();
            }
            return result;
        }

        private void JsonPrintResult(object data)
        {
            try
            {
                if (data != null)
                {
                    var listData = (List<ADO.ServiceReqADO>)gridControlServiceReq.DataSource;
                    listData[listData.IndexOf(this.currentServiceReqPrint)].JSON_PRINT_ID = data.ToString();
                    gridControlServiceReq.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tooltipServiceRequest_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlServiceReq)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlServiceReq.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "IMG")
                            {
                                long sttId = (long)view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_ID");
                                long serviceReqTypeId = (long)view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_TYPE_ID");
                                if (sttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && view.GetRowCellValue(lastRowHandle, "SAMPLE_TIME") != null)
                                {
                                    text = "Đã lấy mẫu";
                                }
                                else if (sttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL && serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && view.GetRowCellValue(lastRowHandle, "RECEIVE_SAMPLE_TIME") != null)
                                {
                                    text = "Đã nhận mẫu";
                                }
                                else
                                    text = (view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "PRIORITY_DISPLAY")
                            {
                                string priority = Inventec.Common.Resource.Get.Value("His.Desktop.Plugins.ServiceReqlist.PriorityName", ResourceLangManager.LanguageFrmServiceReqList, LanguageManager.GetCulture());
                                text = priority.ToString();
                            }
                            else if (info.Column.FieldName == "ServiceReqEdit")
                            {
                                if (view.GetRowCellValue(lastRowHandle, "CARER_CARD_BORROW_ID") != null)
                                    text = "Chỉ định dịch vụ mượn thẻ chỉ cho phép sửa/xóa thông qua chức năng \"Quản lý mượn thẻ\"";
                                else
                                    text = "Sửa";
                            }
                            else if (info.Column.FieldName == "ServiceReqDelete")
                            {
                                if (view.GetRowCellValue(lastRowHandle, "CARER_CARD_BORROW_ID") != null)
                                    text = "Chỉ định dịch vụ mượn thẻ chỉ cho phép sửa/xóa thông qua chức năng \"Quản lý mượn thẻ\"";
                                else
                                    text = "Xóa";
                            }
                            else if (info.Column.FieldName == "ServiceReqPrint")
                            {
                                if (view.GetRowCellValue(lastRowHandle, "CARER_CARD_BORROW_ID") != null)
                                    text = "Chỉ định dịch vụ mượn thẻ chỉ cho phép sửa/xóa thông qua chức năng \"Quản lý mượn thẻ\"";
                                else
                                    text = "In";
                            }
                            else if (view.GetRowCellValue(lastRowHandle, "IS_TEMPORARY_PRES") != null && (view.GetRowCellValue(lastRowHandle, "IS_TEMPORARY_PRES") ?? "").ToString() == "1")
                            {
                                text = "Là đơn tạm";
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
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
                if (string.IsNullOrWhiteSpace(txtServiceReqCode.Text) && string.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                {
                    if (dtIntructionTimeFrom.EditValue != null)
                    {
                        if (dtIntructionTimeTo.EditValue != null)
                        {
                            if ((dtIntructionTimeTo.DateTime.Date - dtIntructionTimeFrom.DateTime.Date).TotalDays > 90)
                            {
                                XtraMessageBox.Show("Khoảng thời gian tìm kiếm quá dài, vui lòng chọn tối đa 3 tháng!",
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            WaitingManager.Show();
                            FillDataToGrid();
                            WaitingManager.Hide();
                        }
                        else
                        {
                            dtIntructionTimeTo.EditValue = DateTime.Now;
                            if ((dtIntructionTimeTo.DateTime.Date - dtIntructionTimeFrom.DateTime.Date).TotalDays > 90)
                            {
                                XtraMessageBox.Show("Khoảng thời gian tìm kiếm quá dài, vui lòng chọn tối đa 3 tháng!",
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            WaitingManager.Show();
                            FillDataToGrid();
                            WaitingManager.Hide();
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Thời gian từ không được bỏ trống",
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    WaitingManager.Show();
                    FillDataToGrid();
                    WaitingManager.Hide();
                }
                
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void FillDataToControl(HIS_EXP_MEST data, ADO.ServiceReqADO serviceReq2)
        {
            try
            {
                if (serviceReq2 != null)
                {
                    lblPatientName.Text = serviceReq2.TDL_PATIENT_NAME;
                    lblTreatmentCode.Text = serviceReq2.TDL_TREATMENT_CODE;
                    lblGender.Text = serviceReq2.TDL_PATIENT_GENDER_NAME;
                    if (serviceReq2.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        lbDOB.Text = serviceReq2.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        lbDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq2.TDL_PATIENT_DOB);
                    }

                    lblSoTT.Text = serviceReq2.NUM_ORDER.HasValue ? serviceReq2.NUM_ORDER.ToString() : "";//TODO
                    var hiscard = GetHisCard(serviceReq2.TDL_PATIENT_ID);
                    lblSoTheTM.Text = hiscard != null ? hiscard.CARD_CODE : "";

                    if (serviceReq2.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        lblExpMestStt.Text = "Chưa xử lý";
                    else if (serviceReq2.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                    {
                        lblExpMestStt.Text = "Đang xử lý";
                    }
                    else if (serviceReq2.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        lblExpMestStt.Text = "Hoàn thành";
                    }

                    var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == serviceReq2.REQUEST_DEPARTMENT_ID);
                    if (department != null)
                    {
                        lblReqDepartment.Text = department.DEPARTMENT_NAME;
                    }

                    var excuteDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == serviceReq2.EXECUTE_DEPARTMENT_ID);
                    if (excuteDepartment != null)
                    {
                        lbExcuteDepartment.Text = excuteDepartment.DEPARTMENT_NAME;
                    }

                    if (data != null)
                    {
                        SetVisibleControl(true);

                        this.lciAggrExpMestCode.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciAggrExpMestCode.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                        this.lciExpMestCode.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciExpMestCode.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                        this.lciExpMestRoom.Text = Inventec.Common.Resource.Get.Value("frmServiceReqList.lciExpMestRoom.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                    }
                    else
                    {
                        SetVisibleControl(false);
                    }

                    if (serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                        || serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL
                        || serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA
                        || serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                        || serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                        || serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA)
                    {
                        SetVisibleControlRegion2(true);
                        lblBarcode.Text = serviceReq2.BARCODE;
                        chkReqSended.Checked = (bool)(serviceReq2.IS_SENT_EXT == 1) ? true : false;
                        lblAssignTurnCode.Text = serviceReq2.ASSIGN_TURN_CODE;
                    }
                    else
                    {
                        SetVisibleControlRegion2(false);
                    }

                    if (serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                        || serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                        || serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                        || serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM)
                    {
                        SetVisibleControlRegion3(true);
                        chkIsKidney.ReadOnly = true;
                        chkIsHomePres.ReadOnly = true;
                        var aData = (HIS_EXP_MEST)data;
                        this.lblAggrExpMestCode.Text = aData.TDL_AGGR_EXP_MEST_CODE;
                        this.lblExpMestCode.Text = aData.EXP_MEST_CODE;
                        var expMestStt = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == aData.EXP_MEST_STT_ID);
                        var medistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == aData.MEDI_STOCK_ID);
                        this.lblExpMestRoom.Text = (medistock != null) ? medistock.MEDI_STOCK_NAME : "";
                        chkIsKidney.Checked = (bool)(serviceReq2.IS_KIDNEY == 1) ? true : false;
                        chkIsHomePres.Checked = (bool)(serviceReq2.IS_HOME_PRES == 1) ? true : false;
                        lblSoThang.Text = serviceReq2.REMEDY_COUNT != null ? serviceReq2.REMEDY_COUNT.ToString() : "";
                        if (serviceReq2.IS_TEMPORARY_PRES == 1)// ẩn với đơn tạm
                        {
                            this.btnMobaCreate.Enabled = false;
                        }
                        else if (this.currentWorkPlace == null || this.currentWorkPlace.DepartmentId != department.ID)
                        {
                            this.btnMobaCreate.Enabled = false;
                        }
                        else if (aData.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            this.btnMobaCreate.Enabled = false;
                        }
                        else if (aData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK)
                        //ẩn với đơn phòng khám
                        {
                            this.btnMobaCreate.Enabled = false;
                        }
                        else if (medistock != null && medistock.DEPARTMENT_ID == currentWorkPlace.DepartmentId)
                        {
                            this.btnMobaCreate.Enabled = true;
                        }
                        else
                            this.btnMobaCreate.Enabled = true;
                    }
                    else
                    {
                        SetVisibleControlRegion3(false);
                    }

                    if (serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN)
                    {
                        lciRationTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        var RationTime = this.lsRationTime.FirstOrDefault(o => o.ID == (serviceReq2.RATION_TIME_ID ?? 0));
                        if (RationTime != null)
                        {
                            lblRationTime.Text = RationTime.RATION_TIME_NAME;
                        }
                        //Mã phiếu TH
                        lciRationSumCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        HIS_RATION_SUM rationSum = GetRationSumByID(serviceReq2.RATION_SUM_ID);
                        lblRationSumCode.Text = rationSum != null ? rationSum.RATION_SUM_CODE : "";
                    }
                    else
                    {
                        lciRationTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciRationSumCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    if (serviceReq2.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                    {
                        lciSamplerName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciReceiveSampleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lblSamplerName.Text = serviceReq2.SAMPLER_LOGINNAME + " - " + serviceReq2.SAMPLER_USERNAME;
                        lblReceiveSampleName.Text = serviceReq2.RECEIVE_SAMPLE_LOGINNAME + " - " + serviceReq2.RECEIVE_SAMPLE_USERNAME;
                        //loai mau
                        lciTestSampleTypeName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        var testSampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().FirstOrDefault(o => o.ID == serviceReq2.TEST_SAMPLE_TYPE_ID);
                        lblTestSampleTypeName.Text = testSampleType != null ? testSampleType.TEST_SAMPLE_TYPE_NAME : "";
                    }
                    else
                    {
                        lciSamplerName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciReceiveSampleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciTestSampleTypeName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                }
                else
                {
                    lblPatientName.Text = "";
                    lblRationTime.Text = "";
                    lblTreatmentCode.Text = "";
                    lblGender.Text = "";
                    lblReqDepartment.Text = "";
                    lblSoTT.Text = "";
                    lblSoTheTM.Text = "";
                    lblBarcode.Text = "";
                    lblSamplerName.Text = "";
                    lblReceiveSampleName.Text = "";
                    lblAssignTurnCode.Text = "";
                    lblTestSampleTypeName.Text = "";
                    SetVisibleControl(false);
                    SetVisibleControlRegion2(false);
                    SetVisibleControlRegion3(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_RATION_SUM GetRationSumByID(long? ID)
        {
            HIS_RATION_SUM result = null;
            try
            {
                if (ID == null || ID < 0)
                    return null;
                HisRationSumFilter filter = new HisRationSumFilter();
                filter.ID = ID;
                var apiResult = new BackendAdapter(new CommonParam()).Get<List<HIS_RATION_SUM>>(RequestUriStore.HIS_RATION_SUM_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, new CommonParam());
                if (apiResult != null)
                    result = apiResult.FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        HIS_CARD GetHisCard(long patientId)
        {
            try
            {
                HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.PATIENT_ID = patientId;
                cardFilter.ORDER_DIRECTION = "DESC";
                cardFilter.ORDER_FIELD = "MODIFY_TIME";
                var result = new BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>(RequestUriStore.HIS_CARD_GET, ApiConsumer.ApiConsumers.MosConsumer, cardFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (result != null && result.Count > 0)
                {
                    return result[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private void SetVisibleControl(bool p)
        {
            try
            {
                var visibility = p ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //lciAggrExpMestCode.Visibility = visibility;
                lciBtnAggrExpMest.Visibility = visibility;
                //lciExpMestCode.Visibility = visibility;
                //lciExpMestRoom.Visibility = visibility;
                emptySpaceItem2.Visibility = visibility;
                lciBtnMobaCreate.Visibility = visibility;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetVisibleControlRegion2(bool p)
        {
            try
            {
                var visibility = p ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciReqSended.Visibility = visibility;
                chkReqSended.ReadOnly = true;
                lciBarcode.Visibility = visibility;
                lciAssignTurnCode.Visibility = visibility;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetVisibleControlRegion3(bool p)
        {
            try
            {
                var visibility = p ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciExpMestRoom.Visibility = visibility;
                lciExpMestCode.Visibility = visibility;
                layoutControlItem11.Visibility = visibility;
                lciIsKidney.Visibility = visibility;
                lciAggrExpMestCode.Visibility = visibility;
                lciIsHomePres.Visibility = visibility;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAggrExpMest_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.currentServiceReq != null && this.currentPrescription != null)
                {
                    HIS.Desktop.ADO.MobaImpMestListADO data = new MobaImpMestListADO(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM, this.currentPrescription.EXP_MEST_CODE, this.currentServiceReq.TDL_TREATMENT_CODE);

                    List<object> sendObj = new List<object>() { data };
                    CallModule("HIS.Desktop.Plugins.MobaImpMestList", sendObj);
                }
                //else
                //{
                //    MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMobaCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnMobaCreate.Enabled) return;

                if (this.currentServiceReq != null && this.currentPrescription != null &&
                    (this.currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                    this.currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                    this.currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT ||
                    this.currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM))
                {
                    if (currentPrescription.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        List<object> sendObj = new List<object>() { currentPrescription.ID };
                        List<object> sendObjDPK = new List<object>() { this.currentServiceReq.ID };

                        if (currentPrescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            CallModule("HIS.Desktop.Plugins.MobaPrescriptionCreate", sendObj);
                        }
                        //else if (currentPrescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK)
                        //{
                        //    CallModule("HIS.Desktop.Plugins.MobaExamPresCreate", sendObjDPK);
                        //}
                        else if (currentPrescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                        {
                            CallModule("HIS.Desktop.Plugins.MobaCabinetCreate", sendObj);
                        }
                        else if (currentPrescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                        {
                            CallModule("HIS.Desktop.Plugins.MobaBloodCreate", sendObj);
                        }
                        else
                        {
                            MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintTotal_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridControlServiceReq.DataSource != null)
                {
                    var listData = (List<ADO.ServiceReqADO>)gridControlServiceReq.DataSource;
                    if (listData.Count > 0)
                    {
                        listServiceReq = new List<ADO.ServiceReqADO>();

                        foreach (var item in listData)
                        {
                            if (item.isCheck)
                            {
                                listServiceReq.Add(item);
                            }
                        }

                        if (listServiceReq != null && listServiceReq.Count > 0)
                        {
                            CommonParam param = new CommonParam();
                            if (CheckListServiceReq(listServiceReq, param))
                            {
                                MessageManager.Show(this, param, false);
                                return;
                            }

                            InPhieuYeuCauChiDinhTongHop(MPS000037);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.BanChuaChonDichVu);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckListServiceReq(List<ADO.ServiceReqADO> listServiceReq, CommonParam param)
        {
            bool result = false;
            try
            {
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    param.Messages = new List<string>();
                    Dictionary<long, List<ADO.ServiceReqADO>> dicServiceReqByTreatment = new Dictionary<long, List<ADO.ServiceReqADO>>();

                    //Dictionary<long, List<V_HIS_SERVICE_REQ_2>> dicServiceReqByTime = new Dictionary<long, List<V_HIS_SERVICE_REQ_2>>();
                    string serviceCode = "";

                    foreach (var item in listServiceReq)
                    {
                        if (item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                            item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM ||
                            item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                            item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                        {
                            serviceCode += String.Format("{0}, ", item.SERVICE_REQ_CODE);
                        }
                        if (!dicServiceReqByTreatment.ContainsKey(item.TREATMENT_ID))
                            dicServiceReqByTreatment[item.TREATMENT_ID] = new List<ADO.ServiceReqADO>();
                        dicServiceReqByTreatment[item.TREATMENT_ID].Add(item);

                        //if (!dicServiceReqByTime.ContainsKey(item.INTRUCTION_TIME))
                        //    dicServiceReqByTime[item.INTRUCTION_TIME] = new List<V_HIS_SERVICE_REQ_2>();
                        //dicServiceReqByTime[item.INTRUCTION_TIME].Add(item);
                    }

                    string isNotCheck = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Base.ConfigKey.MpsTotalToBordereau);

                    if (!String.IsNullOrEmpty(serviceCode) && !(isNotCheck == "1"))
                    {
                        param.Messages.Add(String.Format(Resources.ResourceMessage.DichVuLaThuoc, serviceCode));
                    }
                    else if (dicServiceReqByTreatment.Count > 1)
                    {
                        param.Messages.Add(Resources.ResourceMessage.DichVuKhongCungHoSoDieuTri);
                    }
                    //else if (dicServiceReqByTime.Count > 1)
                    //{
                    //    param.Messages.Add(Resources.ResourceMessage.DichVuKhongCungThoiGianChiDinh);
                    //}

                    if (param.Messages.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckListServiceReqMedicine(List<ADO.ServiceReqADO> listServiceReq, CommonParam param)
        {
            bool result = false;
            try
            {
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    param.Messages = new List<string>();
                    Dictionary<long, List<ADO.ServiceReqADO>> dicServiceReqByTreatment = new Dictionary<long, List<ADO.ServiceReqADO>>();

                    //Dictionary<long, List<V_HIS_SERVICE_REQ_2>> dicServiceReqByTime = new Dictionary<long, List<V_HIS_SERVICE_REQ_2>>();
                    string serviceCode = "";

                    foreach (var item in listServiceReq)
                    {
                        if (item.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK)
                        {
                            serviceCode += String.Format("{0}, ", item.SERVICE_REQ_CODE);
                        }
                        if (!dicServiceReqByTreatment.ContainsKey(item.TREATMENT_ID))
                            dicServiceReqByTreatment[item.TREATMENT_ID] = new List<ADO.ServiceReqADO>();
                        dicServiceReqByTreatment[item.TREATMENT_ID].Add(item);

                        //if (!dicServiceReqByTime.ContainsKey(item.INTRUCTION_TIME))
                        //    dicServiceReqByTime[item.INTRUCTION_TIME] = new List<V_HIS_SERVICE_REQ_2>();
                        //dicServiceReqByTime[item.INTRUCTION_TIME].Add(item);
                    }

                    if (!String.IsNullOrEmpty(serviceCode))
                    {
                        param.Messages.Add(string.Format("Các yêu cầu sau không là đơn phòng khám : {0}", serviceCode));
                    }
                    else if (dicServiceReqByTreatment.Count > 1)
                    {
                        param.Messages.Add(Resources.ResourceMessage.DichVuKhongCungHoSoDieuTri);
                    }
                    //else if (dicServiceReqByTime.Count > 1)
                    //{
                    //    param.Messages.Add(Resources.ResourceMessage.DichVuKhongCungThoiGianChiDinh);
                    //}

                    if (param.Messages.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void repositoryItemButtonEditIntructionTime_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data.ID);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)RefreshClick);
                        CallModule("HIS.Desktop.Plugins.ServiceReqUpdateInstruction", listArgs);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (HIS_SERE_SERV)grdViewSereServServiceReq.GetFocusedRow();
                    if (data != null)
                    {
                        this.sereServPrint = data;
                        this.treatmentCode = (!String.IsNullOrEmpty(this.sereServPrint.TDL_TREATMENT_CODE) ? this.sereServPrint.TDL_TREATMENT_CODE : "");

                        if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT ||
                            data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                        {
                            MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1 sereServ1 = new V_HIS_SERE_SERV_1();
                            MOS.Filter.HisSereServView1Filter sersereView1Filter = new HisSereServView1Filter();
                            sersereView1Filter.ID = this.sereServPrint.ID;
                            var sereServ1s = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_1>>("api/HisSereServ/GetView1", ApiConsumer.ApiConsumers.MosConsumer, sersereView1Filter, null);
                            if (sereServ1s != null && sereServ1s.Count > 0)
                            {
                                sereServ1 = sereServ1s.FirstOrDefault();
                            }

                            if (sereServ1 != null && sereServ1.ID > 0)
                            {
                                PrintPopupMenuProcessor = new PrintPopupMenuProcessor(PrintPttt_Click, barManager1, sereServ1.SERVICE_REQ_STT_ID, data.TDL_SERVICE_TYPE_ID, this.loginName);
                                PrintPopupMenuProcessor.InitMenuPttt();
                            }
                            else
                            {
                                PrintPopupMenuProcessor = new PrintPopupMenuProcessor(PrintPttt_Click, barManager1, data.TDL_SERVICE_TYPE_ID, this.loginName);
                                PrintPopupMenuProcessor.InitMenuPttt();
                            }
                        }
                        else if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN)
                        {
                            ProcessPrintResult();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallModule(string moduleLink, List<object> data)
        {
            try
            {
                CallModule callModule = new CallModule(moduleLink, currentModule.RoomId, currentModule.RoomTypeId, data);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region public method
        public void RefreshClick()
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Button handler
        private void Btn_EvenLog_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();

                    if (data != null)
                    {
                        WaitingManager.Show();
                        List<object> listArgs = new List<object>();
                        Inventec.UC.EventLogControl.Data.DataInit dataInit = new Inventec.UC.EventLogControl.Data.DataInit(ConfigApplications.NumPageSize, "", "", "", "", "", data.SERVICE_REQ_CODE);
                        KeyCodeADO ado = new KeyCodeADO();
                        ado.serviceRequestCode = data.SERVICE_REQ_CODE;
                        listArgs.Add(ado);
                        listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);

                        var moduleWK = PluginInstance.GetModuleWithWorkingRoom(this.currentModule, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleWK, listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                        WaitingManager.Hide();
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.EventLog", moduleWK.RoomId, moduleWK.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void RightMenu_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintPopupMenuProcessor.ModuleType type = (PrintPopupMenuProcessor.ModuleType)(e.Item.Tag);
                    var serviceClick = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    switch (type)
                    {
                        case PrintPopupMenuProcessor.ModuleType.Edit:
                            repositoryItemBtnServiceReqEdit_ButtonClick(null, null);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.Delete:
                            repositoryItemBtnServiceReqDelete_ButtonClick(null, null);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.Print:
                            repositoryItemBtnServiceReqPrint_ButtonClick(null, null);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.EditIntruction:
                            repositoryItemButtonEditIntructionTime_ButtonClick(null, null);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.EvenLog:
                            Btn_EvenLog_ButtonClick(null, null);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.BieuMauKhac:
                            repositoryItemBtnBieuMauKhac_ButtonClick(null, null);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.ExamMain:
                            Btn_ExamMain_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.GuiLaiXN:
                            {
                                if (serviceClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                                    GuiLaiXNSangLIS(null, null);
                                else
                                    SendResultToPacs(null, null);
                                break;
                            }
                        case PrintPopupMenuProcessor.ModuleType.BieuMauKhacV2:
                            Btn_BieuMauKhacV2_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.SendOldSystemIntegration:
                            Btn_SendOldSystemIntegration_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.OpenAttachFile:
                            Btn_OpenAttachFile_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.EnterInforBeforeSurgery:
                            Btn_EnterInforBeforeSurgery_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.Execute:
                            Btn_SeqUpdateNoExecute_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.NoExecute:
                            Btn_SeqUpdateNoExecute_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.SampleInfo:
                            Btn_LayMauBenhPham_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.SampleType:
                            UpdateSampleType();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.ChangeRoom:
                            Btn_ChangeRoom_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.DrugInterventionInfo:
                            Btn_DrugInterventionInfo_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.AllowNotExecute:
                            Btn_AllowNotExecute_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.DisposeAllowNotExecute:
                            Btn_DisposeAllowNotExecute_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.DanhSachVanBanDaKy:
                            ShowEmrDocumentList();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.KetQuaHeThongBenhAnhDienTu:
                            PrintKetQuaHeThongBenhAnhDienTu();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.GiayDeNghiDoiTraDichVu:
                            GiayDeNghiDoiTraDichVu();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.TaoPhieuYeuCauSuDungKhangSinh:
                            TaoPhieuYeuCauSuDungKhangSinh();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.HuyLayMau:
                            Btn_HuyLayMau_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.ChuyenThanhDonTam:
                            Btn_ChuyenDonTam_ButtonClick();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.InVatTuTSD:
                            Btn_InVatTuTSD();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_InVatTuTSD()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate("Mps000494", DelegateRunPrinter);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        void RunPrint(string printTypeCode, string fileName, object data, Inventec.Common.FlexCelPrint.DelegateEventLog EventLogPrint, bool result, long? roomId)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, EventLogPrint));
                }
                else
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(((ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow()).TDL_TREATMENT_CODE, printTypeCode, roomId);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName, EventLogPrint) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Btn_ChuyenDonTam_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    bool IsDelEmr = false;
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null && data.ID > 0)
                    {
                        List<V_EMR_DOCUMENT> documentDel = null;
                        if (data.TRACKING_ID == null)
                        {

                        }
                        else
                        {
                            EmrDocumentViewFilter emrDocumentFilter = new EmrDocumentViewFilter();
                            emrDocumentFilter.TREATMENT_CODE__EXACT = data.TDL_TREATMENT_CODE;
                            var documents = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrDocumentFilter, null);
                            if (documents != null && documents.Count() > 0)
                            {
                                documentDel = documents.Where(o => !string.IsNullOrEmpty(o.HIS_CODE) && o.HIS_CODE.Equals(String.Format("Mps000062 TREATMENT_CODE:{0} HIS_TRACKING:{1}", data.TDL_TREATMENT_CODE, data.TRACKING_ID))).ToList();
                                if (documentDel != null && documentDel.Count() > 0)
                                {
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Y lệnh {0} đã được gắn tờ điều trị và tờ điều trị đã được ký. Bạn có muốn bỏ gắn tờ điều trị và hủy văn bản ký của tờ điều trị không?", data.SERVICE_REQ_CODE), Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                                    {
                                        return;
                                    }
                                    IsDelEmr = true;
                                }
                                else
                                {
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Y lệnh {0} đã được gắn tờ điều trị. Bạn có muốn bỏ gắn tờ điều trị không?", data.SERVICE_REQ_CODE), Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                                    {
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Y lệnh {0} đã được gắn tờ điều trị. Bạn có muốn bỏ gắn tờ điều trị không?", data.SERVICE_REQ_CODE), Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                                {
                                    return;
                                }
                            }
                        }


                        bool success = false;
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                        reqFilter.ID = data.ID;
                        List<HIS_SERVICE_REQ> sReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, reqFilter, null);
                        HIS_SERVICE_REQ req = sReqs != null ? sReqs.FirstOrDefault() : null;
                        if (req != null)
                        {
                            var resultData = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisServiceReq/UpdateToTemporaryPres", ApiConsumers.MosConsumer, req, param);
                            if (resultData != null)
                            {
                                success = true;
                                if (IsDelEmr)
                                {
                                    bool apiResult = new BackendAdapter(param).Post<bool>("api/EmrDocument/DeleteList", ApiConsumers.EmrConsumer, documentDel.Select(o => o.ID).ToList(), param);
                                }
                                FillDataToGrid();
                            }
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_HuyLayMau_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null && data.ID > 0)
                    {
                        bool success = false;
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        ServiceReqSampleInfoSDO sdo = new ServiceReqSampleInfoSDO();
                        sdo.ServiceReqId = data.ID;
                        sdo.ReqRoomId = currentModule.RoomId;
                        sdo.IsCancel = true;
                        var resultData = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisServiceReq/UpdateSampleInfo", ApiConsumers.MosConsumer, sdo, param);
                        if (resultData != null)
                        {

                            success = true;
                            FillDataToGrid();

                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TaoPhieuYeuCauSuDungKhangSinh()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    HisExpMestFilter expfilter = new HisExpMestFilter();
                    expfilter.SERVICE_REQ_ID = data.ID;
                    var lstExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (lstExpMest != null && lstExpMest.Count() > 0)
                    {
                        var expMest = lstExpMest.FirstOrDefault();
                        if (expMest.ANTIBIOTIC_REQUEST_ID == null)
                        {
                            HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                            expMestMedicineFilter.TDL_SERVICE_REQ_ID = data.ID;
                            var lstExpMestMedicine = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, null);
                            if (lstExpMestMedicine != null && lstExpMestMedicine.Count() > 0)
                            {
                                HisMedicineTypeAcinViewFilter medicineTypeAcinViewFilter = new HisMedicineTypeAcinViewFilter();
                                medicineTypeAcinViewFilter.MEDICINE_TYPE_IDs = lstExpMestMedicine.Select(o => (long)o.TDL_MEDICINE_TYPE_ID).ToList();
                                var lstMedicineTypeAcin = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICINE_TYPE_ACIN>>("api/HisMedicineTypeAcin/GetView", ApiConsumers.MosConsumer, medicineTypeAcinViewFilter, null);
                                if (lstMedicineTypeAcin == null && lstMedicineTypeAcin.Count() == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Đơn không chứa hoạt chất nào cần tạo phiếu yêu cầu sử dụng", Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK);
                                    return;
                                }
                                lstMedicineTypeAcin = lstMedicineTypeAcin.Where(o => o.IS_APPROVAL_REQUIRED == 1).ToList();
                                List<object> listArgs = new List<object>();
                                AntibioticRequestADO ado = new AntibioticRequestADO();
                                ado.ExpMestId = expMest.ID;
                                ado.InstructionDate = data.INTRUCTION_DATE;
                                ProcessDataToSend(data, lstMedicineTypeAcin, lstExpMestMedicine, ref ado);
                                listArgs.Add(ado);
                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AntibioticRequest", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                            }
                        }
                        else
                        {
                            HisAntibioticRequestFilter antibioticReqFilter = new HisAntibioticRequestFilter();
                            antibioticReqFilter.ID = expMest.ANTIBIOTIC_REQUEST_ID;
                            var antibioticReq = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ANTIBIOTIC_REQUEST>>("api/HisAntibioticRequest/GetView", ApiConsumers.MosConsumer, antibioticReqFilter, null).FirstOrDefault();

                            List<object> listArgs = new List<object>();
                            AntibioticRequestADO ado = new AntibioticRequestADO();
                            ado.AntibioticRequest = antibioticReq;
                            ado.ExpMestId = expMest.ID;
                            ado.InstructionDate = data.INTRUCTION_DATE;
                            HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                            expMestMedicineFilter.TDL_SERVICE_REQ_ID = data.ID;
                            var lstExpMestMedicine = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, null);
                            List<V_HIS_MEDICINE_TYPE_ACIN> lstMedicineTypeAcin = new List<V_HIS_MEDICINE_TYPE_ACIN>();
                            if (true)
                            {
                                HisMedicineTypeAcinViewFilter medicineTypeAcinViewFilter = new HisMedicineTypeAcinViewFilter();
                                medicineTypeAcinViewFilter.MEDICINE_TYPE_IDs = lstExpMestMedicine.Select(o => (long)o.TDL_MEDICINE_TYPE_ID).ToList();
                                lstMedicineTypeAcin = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICINE_TYPE_ACIN>>("api/HisMedicineTypeAcin/Get", ApiConsumers.MosConsumer, medicineTypeAcinViewFilter, null);
                                lstMedicineTypeAcin = lstMedicineTypeAcin.Where(o => o.IS_APPROVAL_REQUIRED == 1).ToList();
                            }
                            ProcessDataToSend(data, lstMedicineTypeAcin, lstExpMestMedicine, ref ado);
                            listArgs.Add(ado);
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AntibioticRequest", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataToSend(ServiceReqADO data, List<V_HIS_MEDICINE_TYPE_ACIN> lstMedicineTypeAcin, List<V_HIS_EXP_MEST_MEDICINE> lstExpMestMedicine, ref AntibioticRequestADO ado)
        {
            try
            {
                ado.PatientCode = data.TDL_PATIENT_CODE;
                ado.PatientName = data.TDL_PATIENT_NAME;
                ado.Dob = data.TDL_PATIENT_DOB;
                ado.IsHasNotDayDob = data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1;
                ado.GenderName = data.TDL_PATIENT_GENDER_NAME;
                ado.IcdSubCode = data.ICD_SUB_CODE;
                ado.IcdText = data.ICD_TEXT;
                HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_ID = data.TREATMENT_ID;
                var lstDhst = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, null);
                if (lstDhst != null && lstDhst.Count() > 0)
                {
                    var dhst = lstDhst.OrderByDescending(o => o.EXECUTE_TIME ?? 0).FirstOrDefault();
                    ado.Temperature = dhst.TEMPERATURE;
                    ado.Weight = dhst.WEIGHT;
                    ado.Height = dhst.HEIGHT;
                }
                ado.NewRegimen = new List<HIS_ANTIBIOTIC_NEW_REG>();
                foreach (var item in lstMedicineTypeAcin)
                {
                    HIS_ANTIBIOTIC_NEW_REG antibioticNewReg = new HIS_ANTIBIOTIC_NEW_REG();
                    var expMestMedicine = lstExpMestMedicine.Where(o => o.TDL_MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID).FirstOrDefault();
                    antibioticNewReg.DOSAGE = expMestMedicine.TUTORIAL;
                    antibioticNewReg.USE_FORM = expMestMedicine.MEDICINE_USE_FORM_NAME;
                    antibioticNewReg.USE_DAY = expMestMedicine.USE_TIME_TO - data.INTRUCTION_TIME;
                    antibioticNewReg.ACTIVE_INGREDIENT_ID = item.ACTIVE_INGREDIENT_ID;
                    antibioticNewReg.CONCENTRA = expMestMedicine.CONCENTRA;
                    ado.NewRegimen.Add(antibioticNewReg);
                }
                ado.processType = HIS.Desktop.ADO.AntibioticRequestADO.ProcessType.Request;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GiayDeNghiDoiTraDichVu()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    printChangeServiceId = data.ID;

                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate(MPS000433, DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ShowEmrDocumentList()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null && data.ID > 0)
                    {


                        WaitingManager.Show();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "EMR.Desktop.Plugins.SignedDocument").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'EMR.Desktop.Plugins.SignedDocument'");
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            EmrDocumentInfoADO ado = new EmrDocumentInfoADO();
                            ado.TreatmentCode = data.TDL_TREATMENT_CODE;
                            ado.HisCode = "SERVICE_REQ_CODE:" + data.SERVICE_REQ_CODE;
                            listArgs.Add(ado);
                            listArgs.Add((HIS.Desktop.Common.RefeshReference)FillDataToGrid);
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                            if (extenceInstance == null)
                            {
                                throw new ArgumentNullException("extenceInstance is null");
                            }
                            WaitingManager.Hide();
                            ((Form)extenceInstance).ShowDialog();

                        }
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UpdateSampleType()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null && data.ID > 0)
                    {
                        HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(req, data);
                        List<object> listArgs = new List<object>();
                        listArgs.Add(req);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)RefreshClick);
                        CallModule("HIS.Desktop.Plugins.ServiceReqUpdateSampleType", listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_LayMauBenhPham_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null && data.ID > 0)
                    {
                        if (HisConfigCFG.IsUseInventecLis)
                        {
                            if (data.LIS_STT_ID.HasValue || data.IS_SENT_EXT.HasValue)
                            {
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = LIS.Desktop.Plugins.SampleInfo");
                                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                {
                                    List<object> listArgs = new List<object>();
                                    listArgs.Add(data.SERVICE_REQ_CODE);
                                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                                    ((Form)extenceInstance).ShowDialog();
                                }
                            }
                            else
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân đang nợ viện phí cần thực hiện thanh toán hoặc tạm ứng dịch vụ mới được lấy mẫu bệnh phẩm");
                            }
                        }
                        else
                        {
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqSampleInfo").FirstOrDefault();
                            if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ServiceReqSampleInfo");
                            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(data.ID);
                                listArgs.Add((RefeshReference)FillDataToGrid);
                                listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                                ((Form)extenceInstance).ShowDialog();
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
        private void Btn_DrugInterventionInfo_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null && data.ID > 0)
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DrugInterventionInfo").FirstOrDefault();
                        if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.DrugInterventionInfo");
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            // listArgs.Add(data.SERVICE_REQ_CODE);
                            listArgs.Add(data.ID);
                            listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                            ((Form)extenceInstance).ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void Btn_ChangeRoom_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    AutoMapper.Mapper.CreateMap<ServiceReqADO, L_HIS_SERVICE_REQ>();
                    L_HIS_SERVICE_REQ result = AutoMapper.Mapper.Map<ServiceReqADO, L_HIS_SERVICE_REQ>(data);
                    if (data != null && data.ID > 0)
                    {
                        if (result.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Chỉ định " + result.SERVICE_REQ_CODE + " đã kết thúc, không cho phép chuyển phòng", Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                            {
                                return;
                            }
                        }
                        else
                        {
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ChangeExamRoomProcess").FirstOrDefault();
                            if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ChangeExamRoomProcess");
                            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(result);
                                listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                                ((Form)extenceInstance).ShowDialog();
                                btnFind_Click(null, null);
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



        private void Btn_AllowNotExecute_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    frmReason frm = new frmReason(currentModule, data);
                    frm.ShowDialog();

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_DisposeAllowNotExecute_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null && data.ID > 0)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        HisServiceReqAcceptNoExecuteSDO UnAcceptNoExecuteSDO = new HisServiceReqAcceptNoExecuteSDO();
                        UnAcceptNoExecuteSDO.ServiceReqId = data.ID;
                        UnAcceptNoExecuteSDO.WorkingRoomId = this.currentModule.RoomId;
                        var result = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisSereServ/UnacceptNoExecuteByServiceReq", ApiConsumers.MosConsumer, UnAcceptNoExecuteSDO, param);

                        if (result != null)
                        {
                            success = true;
                        }
                        MessageManager.Show(this, param, success);
                        btnFind_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_SeqUpdateNoExecute_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null && data.ID > 0)
                    {
                        HisSereServNoExecuteSDO sereServNoExecuteSDO = new HisSereServNoExecuteSDO();
                        sereServNoExecuteSDO.IsNoExecute = data.IS_NO_EXECUTE == Base.GlobalStore.IS_TRUE ? false : true;
                        sereServNoExecuteSDO.RequestRoomId = currentModule.RoomId;
                        sereServNoExecuteSDO.TreatmentId = data.TREATMENT_ID;
                        sereServNoExecuteSDO.ServiceReqIds = new List<long>();
                        sereServNoExecuteSDO.ServiceReqIds.Add(data.ID);
                        var dataUpdate = new BackendAdapter(param).Post<List<HIS_SERE_SERV>>("api/HisSereServ/UpdateNoExecute", ApiConsumers.MosConsumer, sereServNoExecuteSDO, param);
                        if (dataUpdate != null && dataUpdate.Count > 0)
                        {
                            success = true;
                            btnFind_Click(null, null);
                        }
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Btn_EnterInforBeforeSurgery_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null)
                    {
                        HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                        req = data;
                        List<object> listArgs = new List<object>();
                        listArgs.Add(req);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)RefreshClick);
                        CallModule("HIS.Desktop.Plugins.EnterInforBeforeSurgery", listArgs);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Btn_SendOldSystemIntegration_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null)
                    {
                        //TODO
                        CommonParam param = new CommonParam();
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>("api/HisServiceReq/SendToOldSystem", ApiConsumers.MosConsumer, data.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        #region Show message
                        MessageManager.Show(this, param, success);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_OpenAttachFile_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null)
                    {
                        AttachFileADO attachFile = new AttachFileADO() { AttachFileUrl = data.ATTACHMENT_FILE_URL, FileType = "Pdf" };
                        List<object> sendObj = new List<object>() { attachFile };
                        CallModule("HIS.Desktop.Plugins.AttachFileViewer", sendObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_BieuMauKhacV2_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null)
                    {
                        List<object> sendObj = new List<object>() { data.ID };
                        CallModule("HIS.Desktop.Plugins.OtherFormAssService", sendObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void SendResultToPacs(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Data thuc hien gui lai yeu cau sang PACS: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                        return;
                    }

                    WaitingManager.Show();

                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisServiceReq/RequestOrder", ApiConsumers.MosConsumer, data.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    WaitingManager.Hide();
                    if (success)
                    {
                        FillDataToGrid();
                    }

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GuiLaiXNSangLIS(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Data thuc hien gui lai yeu cau XN sang LIS: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                        return;
                    }

                    WaitingManager.Show();

                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTestServiceReq/RequestOrder", ApiConsumers.MosConsumer, data.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    WaitingManager.Hide();
                    if (success)
                    {
                        FillDataToGrid();
                    }

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void btnPrintMedicine_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (gridControlServiceReq.DataSource != null)
                    {
                        var listData = (List<ADO.ServiceReqADO>)gridControlServiceReq.DataSource;
                        if (listData.Count > 0)
                        {
                            listServiceReq = new List<ADO.ServiceReqADO>();

                            foreach (var item in listData)
                            {
                                if (item.isCheck)
                                {
                                    listServiceReq.Add(item);
                                }
                            }

                            if (listServiceReq != null && listServiceReq.Count > 0)
                            {
                                CommonParam param = new CommonParam();
                                if (CheckListServiceReqMedicine(listServiceReq, param))
                                {
                                    MessageManager.Show(this, param, false);
                                    return;
                                }

                                InDonThuocTongHop(MPS000234);
                            }
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.BanChuaChonDichVu);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_ExamMain_ButtonClick()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null && data.ID > 0)
                    {
                        var dataUpdate = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisServiceReq/ChangeMain", ApiConsumers.MosConsumer, data.ID, param);
                        if (dataUpdate != null && dataUpdate.ID > 0)
                        {
                            success = true;
                            btnFind_Click(null, null);
                        }
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void MouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.rightClickData != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessorMedicine.ItemType type = (PopupMenuProcessorMedicine.ItemType)(e.Item.Tag);
                    switch (type)
                    {
                        case PopupMenuProcessorMedicine.ItemType.SuaHuongDanSuDung:
                            SuaHuongDanSuDung(this.rightClickData);
                            Inventec.Common.Logging.LogSystem.Warn("log 1");
                            break;
                        case PopupMenuProcessorMedicine.ItemType.AssignPres:
                            AssignPreClick();
                            break;
                        case PopupMenuProcessorMedicine.ItemType.AssignPresCabinet:
                            AssignPreCabinetClick();
                            break;
                        case PopupMenuProcessorMedicine.ItemType.PrintServiceReq:
                            PrintServiceReqBySelectedService();
                            break;
                        //case PopupMenuProcessorMedicine.ItemType.KetQuaHeThongBenhAnhDienTu:
                        //    PrintKetQuaHeThongBenhAnhDienTu();
                        //    break;
                        case PopupMenuProcessorMedicine.ItemType.AssignInKip:
                            AssignInKip();
                            break;
                        case PopupMenuProcessorMedicine.ItemType.AssignOutKip:
                            AssignOutKip();
                            break;
                        case PopupMenuProcessorMedicine.ItemType.AcceptNoExecute:
                            AcceptNoExecute();
                            break;
                        case PopupMenuProcessorMedicine.ItemType.UnacceptNoExecute:
                            UnacceptNoExecute(this.rightClickData);
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UnacceptNoExecute(ListMedicineADO data)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();

                HisSereServAcceptNoExecuteSDO AcceptNoExecuteSDO = new HisSereServAcceptNoExecuteSDO();
                AcceptNoExecuteSDO.SereServId = data.ID;
                AcceptNoExecuteSDO.WorkingRoomId = this.currentModule.RoomId;

                var result = new BackendAdapter(param).Post<HIS_SERE_SERV>("api/HisSereServ/UnacceptNoExecute", ApiConsumers.MosConsumer, AcceptNoExecuteSDO, param);

                if (result != null && result.IS_ACCEPTING_NO_EXECUTE != 1)
                {
                    success = true;
                    foreach (var item in _listMedicine)
                    {
                        var check = _listMedicine.FirstOrDefault(o => o.ID == result.ID);
                        if (check != null)
                        {
                            _listMedicine.FirstOrDefault(o => o.ID == result.ID).IS_ACCEPTING_NO_EXECUTE = result.IS_ACCEPTING_NO_EXECUTE;
                        }
                    }

                    grdViewSereServServiceReq.BeginUpdate();
                    grdViewSereServServiceReq.GridControl.DataSource = _listMedicine;
                    grdViewSereServServiceReq.EndUpdate();
                }
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AcceptNoExecute()
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    var dataSereServServiceReq = (ADO.ListMedicineADO)grdViewSereServServiceReq.GetFocusedRow();
                    if (dataSereServServiceReq != null)
                    {
                        frmReason frm = new frmReason(currentModule, data, dataSereServServiceReq);
                        frm.ShowDialog();
                    }
                    else
                    {
                        frmReason frm = new frmReason(currentModule, data);
                        frm.ShowDialog();
                    }

                    //bool success = false;
                    //CommonParam param = new CommonParam();
                    //HisServiceReqAcceptNoExecuteSDO AcceptNoExecuteSDO = new HisServiceReqAcceptNoExecuteSDO();
                    //AcceptNoExecuteSDO.ServiceReqId = data.ID;
                    //AcceptNoExecuteSDO.WorkingRoomId = this.currentModule.RoomId;
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AcceptNoExecuteSDO), AcceptNoExecuteSDO));
                    //var result = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("

                    //if (result != null)
                    //{
                    //    success = true;
                    //}
                    //MessageManager.Show(this, param, success);
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshHuongDanSuDung()
        {
            try
            {
                ControlServiceReqClick(this.currentServiceReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void AssignPreClick()
        {
            try
            {
                bool isExecuteKidneyShift = this.currentServiceReq.IS_EXECUTE_KIDNEY_PRES == 1;
                string moduleLinkAssignPres = isExecuteKidneyShift ? "HIS.Desktop.Plugins.AssignPrescriptionKidney" : "HIS.Desktop.Plugins.AssignPrescriptionPK";

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == moduleLinkAssignPres).FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + moduleLinkAssignPres);
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AutoMapper.Mapper.CreateMap<ListMedicineADO, V_HIS_SERE_SERV>();
                    V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<ListMedicineADO, V_HIS_SERE_SERV>(this.rightClickData);

                    if (isExecuteKidneyShift)
                    {
                        AssignPrescriptionKidneyADO assignPrescription = new AssignPrescriptionKidneyADO();
                        assignPrescription.AssignPrescriptionEditADO = new AssignPrescriptionEditADO(this.currentServiceReq, null);

                        HisExpMestFilter expfilter = new HisExpMestFilter();
                        expfilter.SERVICE_REQ_ID = currentServiceReq.ID;
                        var expMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                        if (expMest != null && expMest.Count > 0)
                        {
                            assignPrescription.AssignPrescriptionEditADO.ExpMest = expMest.First();
                        }

                        //xuandv new
                        CommonParam param = new CommonParam();
                        HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.ID = this.currentServiceReq.TREATMENT_ID;

                        var rsApi = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        if (rsApi != null && rsApi.Count > 0)
                        {
                            if (rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                            {
                                assignPrescription.IsAutoCheckExpend = true;
                            }
                            if (rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                            {
                                assignPrescription.IsAutoCheckExpend = true;
                            }
                        }
                        listArgs.Add(assignPrescription);
                    }
                    else
                    {
                        AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(this.currentServiceReq.TREATMENT_ID, intructionTime, this.currentServiceReq.ID, sereServInput, null);
                        assignPrescription.GenderName = this.currentServiceReq.TDL_PATIENT_GENDER_NAME;
                        assignPrescription.PatientName = this.currentServiceReq.TDL_PATIENT_NAME;
                        assignPrescription.PatientDob = this.currentServiceReq.TDL_PATIENT_DOB;
                        //assignPrescription.IsAutoCheckExpend = true;
                        // assignPrescription.IsExecutePTTT = true;


                        //xuandv new
                        CommonParam param = new CommonParam();
                        HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.ID = this.currentServiceReq.TREATMENT_ID;

                        var rsApi = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        //if (rsApi != null && rsApi.Count > 0 && rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                        //{
                        //    assignPrescription.IsExecutePTTT = true;
                        //    assignPrescription.IsAutoCheckExpend = true;
                        //}
                        if (rsApi != null && rsApi.Count > 0)
                        {
                            if (rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                            {
                                assignPrescription.IsExecutePTTT = true;
                                assignPrescription.IsAutoCheckExpend = true;
                            }
                            if (rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                            {
                                //issue 13620
                                //var data = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(p => p.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU);
                                //if (data != null && data.HEIN_TREATMENT_TYPE_CODE == "DT")
                                //{
                                assignPrescription.IsExecutePTTT = true;
                                assignPrescription.IsAutoCheckExpend = true;
                                //}
                            }
                        }
                        listArgs.Add(assignPrescription);
                    }

                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AssignPreCabinetClick()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AutoMapper.Mapper.CreateMap<ListMedicineADO, V_HIS_SERE_SERV>();
                    V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<ListMedicineADO, V_HIS_SERE_SERV>(this.rightClickData);

                    AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(this.currentServiceReq.TREATMENT_ID, intructionTime, this.currentServiceReq.ID, sereServInput, null);
                    assignPrescription.GenderName = this.currentServiceReq.TDL_PATIENT_GENDER_NAME;
                    assignPrescription.PatientName = this.currentServiceReq.TDL_PATIENT_NAME;
                    assignPrescription.PatientDob = this.currentServiceReq.TDL_PATIENT_DOB;
                    assignPrescription.IsCabinet = true;
                    assignPrescription.IsAutoCheckExpend = true;

                    listArgs.Add(assignPrescription);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SuaHuongDanSuDung(ListMedicineADO data)
        {
            try
            {
                frmTutorial frm = new frmTutorial((HIS.Desktop.Common.DelegateRefreshData)RefreshHuongDanSuDung, data);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AssignOutKip()
        {
            try
            {
                var serviceReq = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                var sereServRow = (ADO.ListMedicineADO)grdViewSereServServiceReq.GetFocusedRow();
                HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager manager = new Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager();
                if (manager.Run(serviceReq.TREATMENT_ID, serviceReq.TDL_PATIENT_TYPE_ID ?? 0, this.currentModule.RoomId))
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                        AutoMapper.Mapper.CreateMap<ADO.ListMedicineADO, V_HIS_SERE_SERV>();
                        V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<ADO.ListMedicineADO, V_HIS_SERE_SERV>(sereServRow);
                        AssignServiceADO assignServiceADO = new AssignServiceADO(serviceReq.TREATMENT_ID, intructionTime, serviceReq.ID, sereServInput, LoadSereServOutKipResult, false);
                        assignServiceADO.GenderName = serviceReq.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.PatientName = serviceReq.TDL_PATIENT_NAME;
                        assignServiceADO.PatientDob = serviceReq.TDL_PATIENT_DOB;
                        assignServiceADO.IsNotUseBhyt = serviceReq.IS_NOT_USE_BHYT == 1;

                        listArgs.Add(assignServiceADO);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HisEkipUserADO> GetEkipUserADO()
        {
            List<HisEkipUserADO> result = new List<HisEkipUserADO>();
            try
            {
                var serviceReq = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                var sereServRow = (ADO.ListMedicineADO)grdViewSereServServiceReq.GetFocusedRow();
                if (sereServRow != null)
                {
                    CommonParam param = new CommonParam();

                    if (sereServRow.EKIP_ID.HasValue)
                    {
                        HisEkipUserViewFilter hisEkipUserFilter = new HisEkipUserViewFilter();
                        hisEkipUserFilter.EKIP_ID = sereServRow.EKIP_ID;
                        var lst = new BackendAdapter(param)
                .Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, hisEkipUserFilter, param);

                        if (lst.Count > 0)
                        {
                            foreach (var item in lst)
                            {
                                var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                                if (dataCheck == null || dataCheck.ID == 0)
                                    continue;
                                Mapper.CreateMap<V_HIS_EKIP_USER, HisEkipUserADO>();
                                var HisEkipUserProcessing = Mapper.Map<V_HIS_EKIP_USER, HisEkipUserADO>(item);
                                result.Add(HisEkipUserProcessing);
                            }
                        }
                    }
                    else if (serviceReq.EKIP_PLAN_ID.HasValue)
                    {
                        HisEkipPlanUserViewFilter hisEkipPlanUserFilter = new HisEkipPlanUserViewFilter();
                        hisEkipPlanUserFilter.EKIP_PLAN_ID = serviceReq.EKIP_PLAN_ID;
                        var lst = new BackendAdapter(param)
                .Get<List<V_HIS_EKIP_PLAN_USER>>("api/HisEkipPlanUser/GetView", ApiConsumers.MosConsumer, hisEkipPlanUserFilter, param);

                        if (lst.Count > 0)
                        {
                            foreach (var item in lst)
                            {
                                var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                                if (dataCheck == null || dataCheck.ID == 0)
                                    continue;
                                Mapper.CreateMap<V_HIS_EKIP_PLAN_USER, HisEkipUserADO>();
                                var HisEkipUserProcessing = Mapper.Map<V_HIS_EKIP_PLAN_USER, HisEkipUserADO>(item);
                                result.Add(HisEkipUserProcessing);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                result = new List<HisEkipUserADO>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void AssignInKip()
        {
            try
            {
                var serviceReq = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                var sereServRow = (ADO.ListMedicineADO)grdViewSereServServiceReq.GetFocusedRow();
                var ekipUsers = GetEkipUserADO();
                bool hasInvalid = ekipUsers != null && ekipUsers.Count > 0 ? true : false;
                if (!hasInvalid)
                {
                    MessageBox.Show("Chưa có thông tin kíp thực hiện", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager manager = new Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager();
                if (manager.Run(serviceReq.TREATMENT_ID, serviceReq.TDL_PATIENT_TYPE_ID ?? 0, this.currentModule.RoomId))
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                        AutoMapper.Mapper.CreateMap<ADO.ListMedicineADO, V_HIS_SERE_SERV>();
                        V_HIS_SERE_SERV sereServInput = AutoMapper.Mapper.Map<ADO.ListMedicineADO, V_HIS_SERE_SERV>(sereServRow);

                        AssignServiceADO assignServiceADO = new AssignServiceADO(serviceReq.TREATMENT_ID, intructionTime, serviceReq.ID, sereServInput, LoadSereServInKipResult, true);
                        assignServiceADO.GenderName = serviceReq.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.PatientName = serviceReq.TDL_PATIENT_NAME;
                        assignServiceADO.PatientDob = serviceReq.TDL_PATIENT_DOB;
                        assignServiceADO.IsNotUseBhyt = serviceReq.IS_NOT_USE_BHYT == 1;

                        listArgs.Add(assignServiceADO);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServInKipResult(object data)
        {

        }

        private void LoadSereServOutKipResult(object data)
        {
            try
            {
                if (data != null)
                {
                    //LoadAfterAssign();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrintNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridControlServiceReq.DataSource != null)
                {
                    var listData = (List<ADO.ServiceReqADO>)gridControlServiceReq.DataSource;
                    if (listData.Count > 0)
                    {
                        listServiceReq = new List<ADO.ServiceReqADO>();

                        foreach (var item in listData)
                        {
                            if (item.isCheck)
                            {
                                listServiceReq.Add(item);
                            }
                        }

                        if (listServiceReq != null && listServiceReq.Count > 0)
                        {
                            CommonParam param = new CommonParam();
                            if (CheckListServiceReqV2(listServiceReq, param))
                            {
                                MessageManager.Show(this, param, false);
                                return;
                            }

                            var serviceReqDv = listServiceReq.Where(o =>
                                o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK &&
                                o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT &&
                                o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT &&
                                o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM).ToList();

                            var serviceReqDon = listServiceReq.Where(o => !serviceReqDv.Select(s => s.ID).Contains(o.ID)).ToList();

                            if (serviceReqDon != null && serviceReqDon.Count > 0)
                            {
                                foreach (var item in serviceReqDon)
                                {
                                    ExecuteBefPrint(item);
                                }
                            }

                            if (serviceReqDv != null && serviceReqDv.Count > 0)
                            {
                                List<V_HIS_BED_LOG> listBedLogs = new List<V_HIS_BED_LOG>();
                                HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                                bedLogFilter.TREATMENT_ID = serviceReqDv.First().TREATMENT_ID;
                                var resultBedlog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
                                if (resultBedlog != null)
                                {
                                    listBedLogs = resultBedlog;
                                }

                                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                                filter.TREATMENT_ID = serviceReqDv.First().TREATMENT_ID;
                                filter.INTRUCTION_TIME = serviceReqDv.Min(o => o.INTRUCTION_TIME);
                                var hisTreatments = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);

                                HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();

                                HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                                serviceReqFilter.IDs = serviceReqDv.Select(s => s.ID).ToList();
                                serviceReqFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                                HisServiceReqSDO.ServiceReqs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);


                                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                                sereServFilter.SERVICE_REQ_IDs = serviceReqDv.Select(s => s.ID).ToList();
                                sereServFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                                HisServiceReqSDO.SereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                                var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, hisTreatments.FirstOrDefault(), listBedLogs, currentModule != null ? currentModule.RoomId : 0);
                                PrintServiceReqProcessor.Print("Mps000340", false);
                            }
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.BanChuaChonDichVu);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.BanChuaChonDichVu);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GeneratePopupMenu()
        {
            try
            {
                DevExpress.Utils.Menu.DXPopupMenu menu = new DevExpress.Utils.Menu.DXPopupMenu();

                menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Phiếu in tổng hợp", new EventHandler(btnPrintNew_Click)));
                menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Phiếu hướng dẫn bệnh nhân thực hiện CLS", new EventHandler(onClickPhieuHuongDan)));

                btnDropDownPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuHuongDan(object sender, EventArgs e)
        {
            try
            {
                if (gridControlServiceReq.DataSource != null)
                {
                    var listData = (List<ADO.ServiceReqADO>)gridControlServiceReq.DataSource;
                    if (listData.Count > 0)
                    {
                        listServiceReq = new List<ADO.ServiceReqADO>();
                        List<V_HIS_SERVICE_REQ> _V_HIS_SERVICE_REQs = new List<V_HIS_SERVICE_REQ>();
                        foreach (var item in listData)
                        {
                            if (item.isCheck)
                            {
                                V_HIS_SERVICE_REQ ado = new V_HIS_SERVICE_REQ();

                                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(ado, item);
                                _V_HIS_SERVICE_REQs.Add(ado);
                                listServiceReq.Add(item);
                            }
                        }

                        if (listServiceReq != null && listServiceReq.Count > 0)
                        {
                            CommonParam param = new CommonParam();
                            if (CheckListServiceReqV2(listServiceReq, param))
                            {
                                MessageManager.Show(this, param, false);
                                return;
                            }


                            var PrintServiceReqProcessor = new HIS.Desktop.Plugins.Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor(_V_HIS_SERVICE_REQs, this.currentModule != null ? this.currentModule.RoomId : 0);
                            PrintServiceReqProcessor.Print("Mps000276", false);
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.BanChuaChonDichVu);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.BanChuaChonDichVu);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDropDownPrint_Click(object sender, EventArgs e)
        {
            btnDropDownPrint.ShowDropDown();
        }

        private void repositoryItemButton__BieuMauKhac_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                Btn_BieuMauKhacV2_ButtonClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnPrintTemBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrintTemBarcode.Enabled) return;

                List<ServiceReqADO> datas = gridControlServiceReq.DataSource as List<ServiceReqADO>;
                if (datas == null || datas.Count <= 0)
                {
                    XtraMessageBox.Show("Khong co Yêu cầu xét nghiệm nào ở trạng thái chưa xử lý", "Thông báo", DefaultBoolean.True);
                    return;
                }
                WaitingManager.Show();
                ServiceReqADO ado = datas.FirstOrDefault();
                HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                reqFilter.ID = ado.ID;
                List<HIS_SERVICE_REQ> sReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, reqFilter, null);
                HIS_SERVICE_REQ req = sReqs != null ? sReqs.FirstOrDefault() : null;
                if (req != null)
                {
                    this.PrintBarcodeByBartender(req);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintBarcodeByBartender(HIS_SERVICE_REQ req)
        {
            try
            {
                if (StartAppPrintBartenderProcessor.OpenAppPrintBartender())
                {
                    ClientPrintADO ado = new ClientPrintADO();

                    ado.DobYear = req.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    ado.DobAge = MPS.AgeUtil.CalculateFullAge(req.TDL_PATIENT_DOB);

                    ado.GenderName = req.TDL_PATIENT_GENDER_NAME ?? "";
                    ado.GenderName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.GenderName);
                    ado.GenderCode = req.TDL_PATIENT_GENDER_ID.HasValue ? (req.TDL_PATIENT_GENDER_ID == 1 ? "F" : "M") : "";

                    ado.PatientCode = req.TDL_PATIENT_CODE ?? "";
                    ado.PatientName = req.TDL_PATIENT_NAME ?? "";
                    ado.PatientName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.PatientName);
                    ado.ServiceReqCode = req.SERVICE_REQ_CODE ?? "";
                    ado.TreatmentCode = req.TDL_TREATMENT_CODE ?? "";
                    V_HIS_ROOM reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == req.REQUEST_ROOM_ID);
                    if (reqRoom != null)
                    {
                        ado.RequestRoomCode = reqRoom.ROOM_CODE;
                        ado.RequestRoomName = reqRoom.ROOM_NAME ?? "";
                        ado.RequestRoomName_Unsign = Inventec.Common.String.Convert.UnSignVNese(reqRoom.ROOM_NAME ?? "");
                        ado.RequestDepartmentCode = reqRoom.DEPARTMENT_CODE ?? "";
                        ado.RequestDepartmentName = reqRoom.DEPARTMENT_NAME ?? "";
                        ado.RequestDepartmentName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.RequestDepartmentName);
                    }
                    V_HIS_ROOM exeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == req.EXECUTE_ROOM_ID);
                    if (exeRoom != null)
                    {
                        ado.ExecuteRoomCode = exeRoom.ROOM_CODE;
                        ado.ExecuteRoomName = exeRoom.ROOM_NAME ?? "";
                        ado.ExecuteRoomName_Unsign = Inventec.Common.String.Convert.UnSignVNese(exeRoom.ROOM_NAME ?? "");
                        ado.ExecuteDepartmentCode = exeRoom.DEPARTMENT_CODE ?? "";
                        ado.ExecuteDepartmentName = exeRoom.DEPARTMENT_NAME ?? "";
                        ado.ExecuteDepartmentName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.ExecuteDepartmentName);
                    }
                    BartenderPrintClientManager client = new BartenderPrintClientManager();
                    bool success = client.BartenderPrint(ado);
                    if (!success)
                    {
                        LogSystem.Error("In barcode Bartender that bai. Check log BartenderPrint");
                    }
                }
                else
                {
                    LogSystem.Warn("Khong mo duoc APP Print Bartender");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonPrintTemBarcode_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                btnPrintTemBarcode_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        FillDataToGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPK_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPK.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPK.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPK.Name;
                    csAddOrUpdate.VALUE = (chkPK.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                if (chkPK.Checked)
                {
                    LoadComboExcuteRoom(true);
                }
                else { LoadComboExcuteRoom(); }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExecuteRoom.EditValue = null;
                    cboExecuteRoom.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExecuteRoom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExecuteRoom.EditValue != null)
                    {
                        cboExecuteRoom.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditAllowNotExecute_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                //if (gridViewServiceReq.FocusedRowHandle >= 0)
                //{
                //    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                //bool success = false;
                //CommonParam param = new CommonParam();
                //HisServiceReqAcceptNoExecuteSDO AcceptNoExecuteSDO = new HisServiceReqAcceptNoExecuteSDO();
                //AcceptNoExecuteSDO.ServiceReqId = data.ID;
                //AcceptNoExecuteSDO.WorkingRoomId = this.currentModule.RoomId;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AcceptNoExecuteSDO), AcceptNoExecuteSDO));
                //var result = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisSereServ/AcceptNoExecuteByServiceReq", ApiConsumers.MosConsumer, AcceptNoExecuteSDO, param);

                //if (result != null)
                //{
                //    success = true;
                //}
                //MessageManager.Show(this, param, success);
                //    btnFind_Click(null, null);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStoreCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtStoreCode.Text))
                    {
                        FillDataToGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditDeleteEna_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (grdViewSereServServiceReq.FocusedRowHandle >= 0)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var data = (ADO.ListMedicineADO)grdViewSereServServiceReq.GetFocusedRow();
                    if (data == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Data thuc hien huy yeu cau dich vu null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                        return;
                    }
                    if (!CheckLoginAdmin.IsAdmin(this.loginName) && BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == data.PATIENT_TYPE_ID).IS_NOT_EDIT_ASSIGN_SERVICE == 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Dịch vụ {0} có đối tượng thanh toán được tích \"Không cho phép sửa xóa dịch vụ đã chỉ định\", vui lòng liên hệ với quản trị hệ thống", data.TDL_SERVICE_NAME), "Thông báo");
                        return;
                    }
                    
                    if (MessageBox.Show(string.Format("Có chắc muốn xóa dịch vụ {0} không?", data.TDL_SERVICE_NAME), Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        WaitingManager.Show();


                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisSereServ/ExamDelete", ApiConsumers.MosConsumer, data.ID, param);

                        if (success)
                        {
                            this.ControlServiceReqClick(this.currentServiceReq);
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this, param, success);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdSereServServiceReq_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                List<ListMedicineADO> dataSource = grdSereServServiceReq.DataSource != null ? (grdSereServServiceReq.DataSource as IEnumerable<ListMedicineADO>).ToList() : null;
                if (dataSource != null)
                {
                    if (dataSource.All(o => o.IS_RATION == 1))
                    {
                        gridColSerSevPatientTypeName.Caption = "Mức ăn";
                        gridColSerSevPatientTypeName.ToolTip = "";
                    }
                    else if (dataSource.All(o => o.IS_RATION == null))
                    {
                        gridColSerSevPatientTypeName.Caption = "ĐTTT";
                        gridColSerSevPatientTypeName.ToolTip = "Đối tượng thanh toán";
                    }
                    else
                    {
                        gridColSerSevPatientTypeName.Caption = "Mức ăn/ĐTTT";
                        gridColSerSevPatientTypeName.ToolTip = "Mức ăn/Đối tượng thanh toán";
                    }
                }
                else
                {
                    gridColSerSevPatientTypeName.Caption = "Mức ăn/ĐTTT";
                    gridColSerSevPatientTypeName.ToolTip = "Mức ăn/Đối tượng thanh toán";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemTextEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var serviceClick = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (this.currentServiceReq == null || (serviceClick != null && serviceClick.ID != 0 && serviceClick.ID != this.currentServiceReq.ID))
                    {
                        this.currentServiceReq = serviceClick;
                        this.ControlServiceReqClick(this.currentServiceReq);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            try
            {
                ShowPopupContainerForConfig(btnConfig);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ShowPopupContainerForConfig(SimpleButton editor)
        {
            try
            {
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainer1.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom - 100));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitListConfig()
        {
            try
            {
                List<string> ListName = new List<string>() { "Không hiển thị đơn không lấy ở đơn thuốc TH" };
                lstConfig = new List<ConfigADO>();
                for (int i = 0; i < ListName.Count; i++)
                {
                    lstConfig.Add(new ConfigADO() { ID = i + 1, NAME = ListName[i], IsChecked = ConfigIds.Exists(o => o == (i + 1)) });
                }
                gridConfig.DataSource = null;
                gridConfig.DataSource = lstConfig;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void repCheckConfig_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit chk = sender as CheckEdit;
                foreach (var item in lstConfig)
                {
                    if (item.ID == ((ConfigADO)gvConfig.GetFocusedRow()).ID)
                        item.IsChecked = chk.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainer1_CloseUp(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == gridConfig.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                lstConfig = gridConfig.DataSource as List<ConfigADO>;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstConfig), lstConfig));
                var lstSelect = lstConfig.Where(o => o.IsChecked).Select(o => o.ID);
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = String.Join(";", lstSelect);
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = gridConfig.Name;
                    csAddOrUpdate.VALUE = String.Join(";", lstSelect);
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
