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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignNutrition.ADO;
using HIS.Desktop.Plugins.AssignNutrition.Config;
using HIS.Desktop.Plugins.AssignNutrition.Resources;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utilities.Extentions;
using HIS.Desktop.Utility;
using HIS.UC.DateEditor;
using HIS.UC.Icd;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignNutrition.AssignNutrition
{
    public partial class frmAssignNutrition : HIS.Desktop.Utility.FormBase
    {
        #region Reclare variable
        Dictionary<long, long?> DicCapacity = new Dictionary<long, long?>();
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();

        string[] periodSeparators = new string[] { "," };
        string[] icdSeparators = new string[] { ",", ";" };
        const string commonString__true = "1";
        int positionHandleControl = -1;
        int actionType = 0;
        long treatmentId = 0;
        long? serviceReqParentId;
        long previusTreatmentId = 0;

        internal bool isMultiDateState = false;
        internal List<long> intructionTimeSelecteds = new List<long>();
        internal IcdProcessor icdProcessor;
        internal UserControl ucIcd;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;
        internal UCDateProcessor ucDateProcessor;
        internal UserControl ucDate;
        
        List<V_HIS_TREATMENT_BED_ROOM> ListTreatmentBedRooms { get; set; }
        List<V_HIS_TREATMENT_BED_ROOM> treatmentBedRoomSelecteds;

        V_HIS_SERE_SERV currentSereServ { get; set; }

        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ currentHisServiceReq { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        List<HIS_PATIENT_TYPE> currentPatientTypes;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6> currentPreServiceReqs;
        HisServiceReqListResultSDO serviceReqComboResultSDO;

        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1> sereServWithTreatment = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>();
        Inventec.Desktop.Common.Modules.Module currentModule;

        Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs { get; set; }
        Dictionary<long, V_HIS_SERVICE> dicServices;

        decimal currentExpendInServicePackage = 0;
        bool isSaveAndPrint = false;
        int notSearch = 0;

        List<HIS_SERVICE_GROUP> selectedSeviceGroups;
        bool isYes = false;
        decimal totalHeinByTreatment = 0;
        internal HIS_ICD icdChoose { get; set; }
        List<HIS_ROOM_TIME> roomTimes;
        List<SSServiceADO> ServiceIsleafADOs { get; set; }
        List<ServiceADO> ServiceAllADOs { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_ROOM requestRoom;
        List<ServiceADO> ServiceParentADOs;
        HideCheckBoxHelper hideCheckBoxHelper__Service;
        BindingList<ServiceADO> records;
        List<HIS_RATION_TIME> selectedRationTimes;
        List<HIS_RATION_TIME> __curentRationTimes;
        List<HIS_SERVICE_RATI> __curentServiceRatis;
        bool IscheckAllTreeService = false;
        bool isInitForm = true;
        SSServiceADO currentRowSereServADO;
        HisTreatmentBedRoomLViewFilter treatmentBedRoomLViewFilterInput;

        List<V_HIS_TRACKING> trackings;
        V_HIS_TRACKING tracking;
        bool IsVisibleColumn = true;
        #endregion

        #region Construct

        public frmAssignNutrition(Inventec.Desktop.Common.Modules.Module module, long _treatmentId, HisTreatmentBedRoomLViewFilter treatmentBedRoomLViewFilter)
            : base(module)
        {
            try
            {
                InitializeComponent();

                #region ---- Icon ----
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                    if (module != null)
                    {
                        this.Text = module.text;
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                #endregion

                this.actionType = GlobalVariables.ActionAdd;
                this.currentModule = module;
                this.treatmentId = _treatmentId;
                this.treatmentBedRoomLViewFilterInput = treatmentBedRoomLViewFilter;

                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignNutrition.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignNutrition.AssignNutrition.frmAssignNutrition).Assembly);
                HisConfigCFG.LoadConfig();
                Task.Run(() => GetRoomTimes()).Wait();
                Task.Run(() => GetRationTimes()).Wait();
                Task.Run(() => GetServiceRations()).Wait();
                List<Action> methods = new List<Action>();
                methods.Add(this.InitUcIcd);
                methods.Add(this.InitUcSecondaryIcd);
                methods.Add(this.InitUcDate);
                ThreadCustomManager.MultipleThreadWithJoin(methods);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load data
        private void frmAssignNutrition_Load(object sender, EventArgs e)
        {
            try
            {

                this.isInitForm = true;
                this.CheckOverTotalPatientPrice();
                WaitingManager.Show();
                this.SetDefaultData();
                this.gridControlService.ToolTipController = this.tooltipService;
                this.FillAllPatientInfoSelectedInForm();

                this.requestRoom = GetRequestRoom(this.currentModule.RoomId);

                this.InitTabIndex();
                this.FillDataToControlsForm();
                this.LoadIcdDefault();
                this.LoadDefaultUser();
                this.InitDefaultFocus();
                this.LoadDataToCashierRoom();

                InitCboServiceGroupCheck();


                this.FillDataToGridTreatment();
                this.AddBarManager(this.barManager1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        V_HIS_ROOM GetRequestRoom(long requestRoomId)
        {
            V_HIS_ROOM result = new V_HIS_ROOM();
            try
            {
                if (requestRoomId > 0)
                {
                    result = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == requestRoomId);
                }
            }
            catch (Exception ex)
            {
                result = new V_HIS_ROOM();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private async Task GetRoomTimes()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisRoomTimeFilter roomTimeFilter = new HisRoomTimeFilter();
                roomTimeFilter.IS_ACTIVE = 1;
                Inventec.Common.Logging.LogSystem.Debug("begin call HisRoomTime/Get");
                roomTimes = await new BackendAdapter(param).GetAsync<List<HIS_ROOM_TIME>>("api/HisRoomTime/Get", ApiConsumer.ApiConsumers.MosConsumer, roomTimeFilter, param);
                Inventec.Common.Logging.LogSystem.Debug("end call HisRoomTime/Get");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task GetServiceRations()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceRatiFilter ratiFilter = new HisServiceRatiFilter();
                ratiFilter.IS_ACTIVE = 1;
                __curentServiceRatis = await new BackendAdapter(param).GetAsync<List<HIS_SERVICE_RATI>>("api/HisServiceRati/Get", ApiConsumer.ApiConsumers.MosConsumer, ratiFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task GetRationTimes()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisRationTimeFilter roomTimeFilter = new HisRationTimeFilter();
                roomTimeFilter.IS_ACTIVE = 1;
                __curentRationTimes = await new BackendAdapter(param).GetAsync<List<HIS_RATION_TIME>>("api/HisRationTime/Get", ApiConsumer.ApiConsumers.MosConsumer, roomTimeFilter, param);

                if (__curentRationTimes != null && __curentRationTimes.Count == 1)
                {
                    this.lciHalfInFirstDay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task CheckOverTotalPatientPrice()
        {
            try
            {
                if (HisConfigCFG.WarningOverTotalPatientPrice__IsCheck == "10000")
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFeeViewFilter hisTreatmentFeeViewFilter = new HisTreatmentFeeViewFilter();
                    hisTreatmentFeeViewFilter.IS_ACTIVE = 1;
                    hisTreatmentFeeViewFilter.ID = this.treatmentId;
                    Inventec.Common.Logging.LogSystem.Debug("begin call HisTreatment/GetFeeView");
                    var treatmentFees = await new BackendAdapter(param).GetAsync<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFeeViewFilter, param);

                    if (treatmentFees != null && treatmentFees.Count > 0)
                    {
                        var treatmentFee = treatmentFees.FirstOrDefault();
                        if (treatmentFee.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                        {
                            return;
                        }
                        decimal warningOverTotalCGF = Convert.ToInt64(HisConfigCFG.WarningOverTotalPatientPrice);
                        decimal totalReceiveMore = (treatmentFee.TOTAL_PATIENT_PRICE ?? 0) - (treatmentFee.TOTAL_DEPOSIT_AMOUNT ?? 0) - (treatmentFee.TOTAL_BILL_AMOUNT ?? 0) + (treatmentFee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) + (treatmentFee.TOTAL_REPAY_AMOUNT ?? 0);
                        if (totalReceiveMore > warningOverTotalCGF)
                        {
                            if (MessageBox.Show(String.Format(ResourceMessage.BenhNhanDangThieuVienPhi, Inventec.Common.Number.Convert.NumberToString(totalReceiveMore, ConfigApplications.NumberSeperator)), "Cảnh báo",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
        MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                            {
                                this.Close();
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

        private void InitUcDate()
        {
            try
            {
                ucDateProcessor = new UCDateProcessor();
                HIS.UC.DateEditor.ADO.DateInitADO ado = new HIS.UC.DateEditor.ADO.DateInitADO();
                ado.DelegateNextFocus = NextForcusUCDate;
                ado.DelegateChangeIntructionTime = ChangeIntructionTime;
                ado.DelegateSelectMultiDate = DelegateSelectMultiDate;
                ado.DelegateMultiDateChanged = DelegateMultiDateChanged;
                ado.DelegateCheckMultiDate = DelegateCheckMultiDated;
                ado.Height = 24;
                ado.Width = 364;//284//TODO
                ado.IsVisibleMultiDate = true;
                ado.IsValidate = true;
                ado.LanguageInputADO = new UC.DateEditor.ADO.LanguageInputADO();
                ado.LanguageInputADO.TruongDuLieuBatBuoc = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                ado.LanguageInputADO.UCDate__CaptionlciDateEditor = "Thời gian chỉ định:";//Inventec.Common.Resource.Get.Value("frmAssignNutrition.lciTimeAssign.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.UCDate__CaptionchkMultiIntructionTime = "Nhiều ngày";// Inventec.Common.Resource.Get.Value("frmAssignNutrition.chkMultiIntructionTime.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.ChuaChonNgayChiDinh = ResourceMessage.ChuaChonNgayChiDinh;
                ado.LanguageInputADO.FormMultiChooseDate__CaptionCalendaInput = "Ngày y lệnh:";// Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionCalendaInput", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.FormMultiChooseDate__CaptionText = "Chọn nhiều ngày y lệnh";// Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.FormMultiChooseDate__CaptionTimeInput = "Giờ y lệnh:";// Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionTimeInput", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.FormMultiChooseDate__CaptionBtnChoose = "Chọn";// Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionBtnChoose", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                ucDate = (UserControl)ucDateProcessor.Run(ado);

                if (ucDate != null)
                {
                    this.pnlUCDate.Controls.Add(ucDate);
                    ucDate.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void DelegateCheckMultiDated(bool isCheck)
		{
			try
			{
                if(isCheck && chkAutoEat.Checked)
				{
                   ucDateProcessor.EnableCheckBoxMultiIntructionTime(ucDate, false);
                   DevExpress.XtraEditors.XtraMessageBox.Show("Không cho phép chỉ định nhiều ngày khi báo ăn tự động", "Thông báo");
                   return;                    
                }                    
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}
        private void DelegateMultiDateChanged()
        {
            try
            {
                ChangeIntructionTime(DateTime.Now);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusUCDate()
        {
            try
            {
                //SendKeys.Send("{TAB}");
                this.FocusShowpopup(this.cboServiceGroup, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.Width = 660;
                ado.Height = 24;
                ado.IsColor = (HisConfigCFG.ObligateIcd == commonString__true);
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = HisConfigCFG.AutoCheckIcd == commonString__true;
                ucIcd = (UserControl)icdProcessor.Run(ado);

                if (ucIcd != null)
                {
                    this.panelControlIcd.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.FocusControl(ucSecondaryIcd);
                }
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
                var icdValue = icdProcessor.GetValue(ucIcd) as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null)
                {
                    mainCode = icdValue.ICD_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList());
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.Width = 660;
                ado.Height = 24;
                ado.TextLblIcd = "Chẩn đoán phụ:"; //Inventec.Common.Resource.Get.Value("frmAssignNutrition.lcitxtIcdExtraCodes.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.TextNullValue = "Chẩn đoán phụ:";// Inventec.Common.Resource.Get.Value("frmAssignNutrition.txtIcdExtraNames.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
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
                icdProcessor.FocusControl(ucDate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDefaultFocus()
        {
            try
            {
                //if ((this.currentHisPatientTypeAlter != null && this.currentHisPatientTypeAlter.PATIENT_TYPE_ID > 0) || HisConfigCFG.AutoCheckIcd != commonString__true)
                //    ucDateProcessor.FocusControl(ucDate);
                //else
                //    icdProcessor.FocusControl(ucIcd);
                icdProcessor.FocusControl(ucIcd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Control editor
        private void toggleSwitchDataChecked_Toggled(object sender, EventArgs e)
        {
            try
            {
                ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                if (toggleSwitch != null)
                {
                    this.LoadDataToGrid(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                // DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    long _serviceId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewService.GetRowCellValue(e.RowHandle, "ID") ?? "").ToString().Trim());
                    if (e.Column.FieldName == "AMOUNT")
                    {
                        short isMultiRequest = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewService.GetRowCellValue(e.RowHandle, "IS_MULTI_REQUEST") ?? "").ToString().Trim());
                        if (isMultiRequest == 1)
                        {
                            e.RepositoryItem = this.repositoryItemSpinEdit__E;
                        }
                        else
                        {
                            e.RepositoryItem = this.repositoryItemSpinEdit__D;
                        }
                    }
                    else if (e.Column.FieldName.Contains("cot"))
                    {
                        DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
                        string dem = e.Column.FieldName.Substring(3);
                        //TODO CheckEdit enable hay k ?
                        if (!IsVisibleColumn)
                        {
                            repositoryItemCheckEdit.GlyphAlignment = HorzAlignment.Near;
                        }
                        var strCaption = (gridViewService.GetRowCellValue(e.RowHandle, "CaptionRationTime") ?? "").ToString();
                        repositoryItemCheckEdit.Caption = null;
                        if (!string.IsNullOrEmpty(strCaption))
                        {
                            var splt = strCaption.Split(new string[] { "____", ";" }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < splt.Count(); i++)
                            {
                                if (!string.IsNullOrEmpty(splt[i]) && splt[i] == _serviceId.ToString() && splt[i + 1] == dem) {
                                    repositoryItemCheckEdit.Caption = splt[i + 2];
                                }
                            }
                        }          
                        if (!ProcessRationTime(_serviceId, dem))
                        {
                            repositoryItemCheckEdit.ReadOnly = true;
                            repositoryItemCheckEdit.CheckStyle = CheckStyles.Style2;
                        }
                        e.RepositoryItem = repositoryItemCheckEdit;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ProcessRationTime(long _serviceId, string _indext)
        {
            bool result = false;
            try
            {
                var rationTimeId = this.__curentRationTimes[Inventec.Common.TypeConvert.Parse.ToInt32(_indext)].ID;
                if (this.__curentServiceRatis != null && this.__curentServiceRatis.Count > 0)
                {
                    var data = this.__curentServiceRatis.FirstOrDefault(p => p.SERVICE_ID == _serviceId && p.RATION_TIME_ID == rationTimeId);
                    if (data != null)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void gridViewServiceProcess_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SSServiceADO data = view.GetFocusedRow() as SSServiceADO;
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        this.FillDataIntoPatientTypeCombo(data, editor);
                        editor.EditValue = data.PATIENT_TYPE_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "PRIMARY_PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null && !data.IsNotChangePrimaryPaty)
                    {
                        this.FillDataIntoPrimaryPatientTypeCombo(data, editor);
                        editor.EditValue = data.PRIMARY_PATIENT_TYPE_ID;
                    }
                    else
                    {
                        editor.ReadOnly = true;
                    }
                }
                else if (view.FocusedColumn.FieldName == "ROOM_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        this.FillDataIntoExcuteRoomCombo(data, editor);
                        editor.EditValue = data.ROOM_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "BUA_AN_NAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        this.InitCboBuaAnCheck(editor);
                        this.InitComboRationTime(editor, data);
                        editor.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.repositoryItemGridLookUp__BuaAn_CustomDisplayText);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        SSServiceADO oneServiceSDO = (SSServiceADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
                        if (oneServiceSDO != null)
                        {
                            if (e.Column.FieldName == "PRICE_DISPLAY" && oneServiceSDO.IsChecked && IsVisibleColumn)
                            {
                                if (oneServiceSDO.PATIENT_TYPE_ID != 0 && this.servicePatyInBranchs.ContainsKey(oneServiceSDO.ID) && instructionTime > 0)
                                {
                                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();
                                    var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                                    var executeRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                                    List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> arrExcuteRoomCode = new List<V_HIS_SERVICE_ROOM>();
                                    if (executeRoomViews != null && executeRoomViews.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
                                    {
                                        arrExcuteRoomCode = serviceRoomViews.Where(o => oneServiceSDO != null && o.SERVICE_ID == oneServiceSDO.ID).ToList();
                                        dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0 && executeRoomViews != null) ? executeRoomViews.Where(o => arrExcuteRoomCode.Select(p => p.ROOM_ID).Contains(o.ROOM_ID)).ToList() : null);
                                    }
                                    var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault(o => o.BRANCH_ID == this.requestRoom.BRANCH_ID) : null;
                                    if (checkExecuteRoom != null)
                                    {
                                        oneServiceSDO.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
                                    }
                                    else
                                    {
                                        oneServiceSDO.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                                    }
                                    List<V_HIS_SERVICE_PATY> servicePaties = this.servicePatyInBranchs[oneServiceSDO.ID];
                                    var oneServicePatyPrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(
                                        servicePaties,
                                        oneServiceSDO.TDL_EXECUTE_BRANCH_ID,
                                        null,
                                        this.requestRoom.ID,
                                        this.requestRoom.DEPARTMENT_ID,
                                        instructionTime,
                                        this.currentHisTreatment.IN_TIME,
                                        oneServiceSDO.ID,
                                        oneServiceSDO.PATIENT_TYPE_ID,
                                        null,null,null,null,
                                         this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID,
                                         null
                                        );

                                    if (oneServicePatyPrice != null)
                                    {
                                        e.Value = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO));
                                    }
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("oneServiceSDO.PATIENT_TYPE_ID else continued");
                                }
                            }
                            else if (e.Column.FieldName == "ACTUAL_PRICE_DISPLAY" && oneServiceSDO.IsChecked && IsVisibleColumn)
                            {
                                if (oneServiceSDO.PATIENT_TYPE_ID != 0 && this.servicePatyInBranchs.ContainsKey(oneServiceSDO.ID) && instructionTime > 0)
                                {
                                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();
                                    var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                                    var executeRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                                    List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> arrExcuteRoomCode = new List<V_HIS_SERVICE_ROOM>();
                                    if (executeRoomViews != null && executeRoomViews.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
                                    {
                                        arrExcuteRoomCode = serviceRoomViews.Where(o => oneServiceSDO != null && o.SERVICE_ID == oneServiceSDO.ID).ToList();
                                        dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0 && executeRoomViews != null) ? executeRoomViews.Where(o => arrExcuteRoomCode.Select(p => p.ROOM_ID).Contains(o.ROOM_ID)).ToList() : null);
                                    }
                                    var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault(o => o.BRANCH_ID == this.requestRoom.BRANCH_ID) : null;
                                    if (checkExecuteRoom != null)
                                    {
                                        oneServiceSDO.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
                                    }
                                    else
                                    {
                                        oneServiceSDO.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                                    }
                                    List<V_HIS_SERVICE_PATY> servicePaties = this.servicePatyInBranchs[oneServiceSDO.ID];
                                    var oneServicePatyPrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(
                                        servicePaties,
                                        oneServiceSDO.TDL_EXECUTE_BRANCH_ID,
                                        null,
                                        this.requestRoom.ID,
                                        this.requestRoom.DEPARTMENT_ID,
                                        instructionTime,
                                        this.currentHisTreatment.IN_TIME,
                                        oneServiceSDO.ID,
                                        oneServiceSDO.PATIENT_TYPE_ID,
                                        null, null, null, null,
                                         this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID,
                                         null
                                        );
                                    if (oneServicePatyPrice != null)
                                    {
                                        e.Value = oneServicePatyPrice.ACTUAL_PRICE;
                                    }
                                }
                            }
                            if (e.Column.FieldName == "PRICE_PRPO_DISPLAY")
                            {
                                if (oneServiceSDO.PATIENT_TYPE_ID != 0 && this.servicePatyInBranchs.ContainsKey(oneServiceSDO.ID))
                                {
                                    if (this.dicServices.ContainsKey(oneServiceSDO.ID))
                                    {
                                        var oneServicePrice = this.dicServices[oneServiceSDO.ID];
                                        if (oneServicePrice != null)
                                        {
                                            e.Value = (oneServicePrice.PACKAGE_PRICE);
                                        }
                                    }
                                }
                            }
                            else if (e.Column.FieldName == "ESTIMATE_DURATION_DISPLAY")
                            {

                            }
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var sereServADO = (SSServiceADO)this.gridViewService.GetFocusedRow();
                    if (sereServADO != null)
                    {
                        //sereServADO.IsChecked = true;
                        if (sereServADO.IsChecked)
                        {
                            this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.ID, sereServADO);
                            this.ChooseExecuteRoomDefaultlService(sereServADO.ID, sereServADO);
                            this.ChooseCotDefaultlService(sereServADO.ID, sereServADO);
                            this.ValidServiceDetailProcessing(sereServADO);
                            //this.ProcessNoDifferenceHeinServicePrice(sereServADO);
                            //this.VerifyWarningOverCeiling();
                            //if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                            //{
                            //    MessageBox.Show("Dịch vụ cận lâm sàng có giới hạn chỉ định thanh toán BHYT. Đề nghị BS xem xét trước khi chỉ định", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //}
                        }
                        else
                        {
                            this.ResetOneService(sereServADO);
                        }

                        this.gridControlService.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);
                        this.SetDefaultSerServTotalPrice();
                        // focus vào ô tìm kiếm (Auto filter)
                        // gridViewService.FocusedRowHandle = -2147483646;
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    var sereServADO = (SSServiceADO)this.gridViewService.GetFocusedRow();
                    if (sereServADO != null)
                    {
                        UpdateCurrentFocusRow(sereServADO);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewServiceProcess_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (!this.ValidPatientTypeForAdd())
                    return;

                GridView view = (GridView)sender;
                Point pt = view.GridControl.PointToClient(Control.MousePosition);
                GridHitInfo info = view.CalcHitInfo(pt);
                if ((info.InRow || info.InRowCell)
                    && info.Column.FieldName != this.grcChecked_TabService.FieldName
                    && info.Column.FieldName != this.gridColumnPatientTypeName__TabService.FieldName
                    && info.Column.FieldName != this.grcAmount_TabService.FieldName
                    && info.Column.FieldName != this.grcExecuteRoomName_TabService.FieldName)
                {
                    var sereServADO = (SSServiceADO)this.gridViewService.GetFocusedRow();
                    if (sereServADO != null)
                    {
                        UpdateCurrentFocusRow(sereServADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var sereServADO = (SSServiceADO)this.gridViewService.GetFocusedRow();
                if (sereServADO != null)
                {
                    if (e.Column.FieldName == this.grcChecked_TabService.FieldName
                        )
                    {
                        if (sereServADO.IsChecked)
                        {
                            this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.ID, sereServADO);    
                            this.ChooseExecuteRoomDefaultlService(sereServADO.ID, sereServADO);
                            this.ChooseCotDefaultlService(sereServADO.ID, sereServADO);

                            this.FillDataOtherPaySourceDataRow(sereServADO);

                            this.ValidServiceDetailProcessing(sereServADO);
                            if (DicCapacity.ContainsKey(sereServADO.ID))
                                sereServADO.CAPACITY = DicCapacity[sereServADO.ID];
                        }
                        else
                        {
                            this.ResetOneService(sereServADO);
                        }
                        this.gridControlService.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);
                    }
                    else if (e.Column.FieldName.Contains("cot"))
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        CheckEdit chk = view.ActiveEditor as CheckEdit;
                        if (chk != null && chk.Properties.CheckStyle == CheckStyles.Style2)
                        {
                            string dem = e.Column.FieldName.Substring(3);
                            chk.Checked = false;
                            ResetCheckByIndext(sereServADO, Inventec.Common.TypeConvert.Parse.ToInt32(dem));
                            return;
                        }
                        //string dem = e.Column.FieldName.Substring(3);
                        CheckProcess(sereServADO);
                        if (sereServADO.RationTimeIds != null && sereServADO.RationTimeIds.Count > 0)
                        {
                            int count = 0;
                            System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<SSServiceADO>();
                            foreach (var s in pi)
                            {
                                if (s.Name.Contains("cot") && (bool)s.GetValue(sereServADO))
                                {
                                    count++;
                                }
                            }
                            if(count == 1 && !sereServADO.IsChecked)
                                this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.ID, sereServADO);
                            sereServADO.IsChecked = true;
                            this.ChooseExecuteRoomDefaultlService(sereServADO.ID, sereServADO);
                            this.ChooseCotDefaultlService(sereServADO.ID, sereServADO);
                            this.ValidServiceDetailProcessing(sereServADO);
                        }
                        else
                        {
                            sereServADO.IsChecked = false;
                            this.ResetOneService(sereServADO);
                        }

                        this.gridControlService.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);
                    }

                    this.SetDefaultSerServTotalPrice();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_ColumnWidthChanged(object sender, ColumnEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "SERVICE_NAME")
                {
                    //txtServiceName_Search.Width = e.Column.Width - 2;
                }
                else if (e.Column.FieldName == "SERVICE_CODE")
                {
                    //txtServiceCode_Search.Width = e.Column.Width;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetCheckByIndext(SSServiceADO item, int indext)
        {
            try
            {
                switch (indext)
                {
                    case 0:
                        item.cot0 = false;
                        break;
                    case 1:
                        item.cot1 = false;
                        break;
                    case 2:
                        item.cot2 = false;
                        break;
                    case 3:
                        item.cot3 = false;
                        break;
                    case 4:
                        item.cot4 = false;
                        break;
                    case 5:
                        item.cot5 = false;
                        break;
                    case 6:
                        item.cot6 = false;
                        break;
                    case 7:
                        item.cot7 = false;
                        break;
                    case 8:
                        item.cot8 = false;
                        break;
                    case 9:
                        item.cot9 = false;
                        break;
                    case 10:
                        item.cot10 = false;
                        break;
                    case 11:
                        item.cot11 = false;
                        break;
                    case 12:
                        item.cot12 = false;
                        break;
                    case 13:
                        item.cot13 = false;
                        break;
                    case 14:
                        item.cot14 = false;
                        break;
                    case 15:
                        item.cot15 = false;
                        break;
                    case 16:
                        item.cot16 = false;
                        break;
                    case 17:
                        item.cot17 = false;
                        break;
                    case 18:
                        item.cot18 = false;
                        break;
                    case 19:
                        item.cot19 = false;
                        break;
                    case 20:
                        item.cot20 = false;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ResetOneService(SSServiceADO item)
        {
            try
            {
                item.IsChecked = false;
                //item.NOTE = null;
                item.CAPACITY = null;
                item.AMOUNT = 1;
                item.PATIENT_TYPE_ID = 0;
                item.PATIENT_TYPE_CODE = null;
                item.PATIENT_TYPE_NAME = null;
                item.ROOM_ID = 0;
                item.PRICE = 0;
                item.RationTimeIds = null;
                item.BUA_AN_NAME = "";
                item.cot0 = false;
                item.cot1 = false;
                item.cot2 = false;
                item.cot3 = false;
                item.cot4 = false;
                item.cot5 = false;
                item.cot6 = false;
                item.cot7 = false;
                item.cot8 = false;
                item.cot9 = false;
                item.cot10 = false;
                item.cot11 = false;
                item.cot12 = false;
                item.cot13 = false;
                item.cot14 = false;
                item.cot15 = false;
                item.cot16 = false;
                item.cot17 = false;
                item.cot18 = false;
                item.cot19 = false;
                item.cot20 = false;

                item.ErrorMessageAmount = "";
                item.ErrorTypeAmount = ErrorType.None;
                item.ErrorMessagePatientTypeId = "";
                item.ErrorTypePatientTypeId = ErrorType.None;
                item.ErrorMessageIsAssignDay = "";
                item.ErrorTypeIsAssignDay = ErrorType.None;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void tooltipService_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == this.gridControlService)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = this.gridControlService.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        //if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        //{
                        //    lastColumn = info.Column;
                        //    lastRowHandle = info.RowHandle;
                        //    string text = text = (view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_NAME") ?? "").ToString();
                        //    if (info.Column.FieldName == "IMG")
                        //    {
                        //        text = (view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_NAME") ?? "").ToString();
                        //    }
                        //    if (info.Column.FieldName == "PRIORITY_DISPLAY")
                        //    {
                        //        string priority = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCSERVICE_REQUEST_LIST_PRIORITY", EXE.APP.Resources.ResourceLanguageManager.LanguageUCServiceRequestList, EXE.LOGIC.Base.LanguageManager.GetCulture());
                        //        text = priority.ToString();
                        //    }
                        //    lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        //}
                        //e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableButtonControl(int actionType)
        {
            try
            {
                List<SSServiceADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0);
                }
                else
                {
                    this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ValidPatientTypeForAdd()
        {
            bool valid = true;
            try
            {
                if (this.currentHisPatientTypeAlter == null || this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == 0)
                {
                    MessageManager.Show(String.Format(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh, Inventec.Common.DateTime.Convert.TimeNumberToDateString(intructionTimeSelecteds.First())));
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboUser.EditValue = null;
                        this.FocusShowpopup(cboUser, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboUser.EditValue = searchResult[0].LOGINNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                            this.FocusWhileSelectedUser();
                        }
                        else
                        {
                            this.cboUser.EditValue = null;
                            this.FocusShowpopup(cboUser, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusWhileSelectedUser()
        {
            try
            {
                //    this.gridControlServiceProcess.Focus();
                //    this.gridViewServiceProcess.FocusedRowHandle = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }

                    this.FocusWhileSelectedUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.FocusWhileSelectedUser();
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }
                }
                else
                {
                    this.cboUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task FillAllPatientInfoSelectedInForm()
        {
            try
            {
                DateTime itime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);
                ChangeIntructionTime(itime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DelegateSelectMultiDate(List<DateTime?> datas, DateTime time)
        {
            try
            {
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate).OrderByDescending(o => o).ToList();
                this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(this.ucDate);

                ChangeIntructionTime(time);                   
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeIntructionTime(DateTime intructTime)
        {
            try
            {
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(ucDate);
                this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(ucDate);
                this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.FirstOrDefault());
                this.ProcessDataWithTreatmentWithPatientTypeInfo();
                //this.LoadTotalSereServByHeinWithTreatment(this.treatmentId);
                // this.LoadDataSereServWithTreatment(this.currentHisTreatment, intructTime);
                //LogSystem.Info("ChangeIntructionTime => Loaded PatientType With ProcessDataWithTreatmentWithPatientTypeInfo info");
                this.chkHalfInFirstDay.ReadOnly = this.intructionTimeSelecteds.Count == 1;
                this.chkHalfInFirstDay.Enabled = this.intructionTimeSelecteds.Count > 1;
                this.chkHalfInFirstDay.Checked = false;
                this.LoadServicePaty();
                LogSystem.Debug("ChangeIntructionTime => LoadServicePaty...");
                this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
                this.LoadTreatmentInfo__PatientType();
                this.InitComboRepositoryServiceRoom(BackendDataWorker.Get<V_HIS_SERVICE_ROOM>());
                this.BindTree();
                LoadDataToGrid(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region -----Cbo Groups ----
        private void SelectOneServiceGroupProcess(List<HIS_SERVICE_GROUP> svgrs)
        {
            try
            {
                List<SSServiceADO> services = null;
                StringBuilder strMessage = new StringBuilder();
                StringBuilder strMessageTemp__CoDichVuKhongCauHinh = new StringBuilder();
                StringBuilder strMessageTemp__KhongDichVu = new StringBuilder();
                bool hasMessage = false;
                // this.ResetServiceGroupSelected();
                if (svgrs != null && svgrs.Count > 0)
                {
                    var idSelecteds = svgrs.Select(o => o.ID).Distinct().ToList();
                    var servSegrAllows = BackendDataWorker.Get<V_HIS_SERV_SEGR>().Where(o => idSelecteds.Contains(o.SERVICE_GROUP_ID)).ToList();
                    if (servSegrAllows != null && servSegrAllows.Count > 0)
                    {
                        var serviceOfGroupsInGroupbys = servSegrAllows.GroupBy(o => o.SERVICE_GROUP_ID).ToDictionary(o => o.Key, o => o.ToList());
                        foreach (var item in serviceOfGroupsInGroupbys)
                        {
                            List<V_HIS_SERV_SEGR> servSegrErrors = new List<V_HIS_SERV_SEGR>();
                            foreach (var svInGr in serviceOfGroupsInGroupbys[item.Key])
                            {
                                var service = this.ServiceIsleafADOs.FirstOrDefault(o => svInGr.SERVICE_ID == o.ID);
                                if (service != null)
                                {
                                    service.NOTE = svInGr.NOTE;
                                    service.IsChecked = true;
                                    service.SERVICE_GROUP_ID_SELECTEDs = idSelecteds;
                                    var searchServiceOfGroups = servSegrAllows.Where(o => o.SERVICE_ID == service.ID).ToList();
                                    if (searchServiceOfGroups != null)
                                    {
                                        service.AMOUNT = (long)searchServiceOfGroups.Sum(o => o.AMOUNT);
                                    }
                                    this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, service.ID, service);
                                    this.ChooseExecuteRoomDefaultlService(service.ID, service);
                                    this.ChooseCotDefaultlService(service.ID, service);
                                    this.ValidServiceDetailProcessing(service);
                                }
                                else
                                {
                                    servSegrErrors.Add(svInGr);
                                }
                            }

                            if (servSegrErrors != null && servSegrErrors.Count > 0)
                            {
                                if (String.IsNullOrEmpty(strMessageTemp__CoDichVuKhongCauHinh.ToString()))
                                {
                                    strMessageTemp__CoDichVuKhongCauHinh.Append("; ");
                                }
                                strMessageTemp__CoDichVuKhongCauHinh.Append(String.Format(ResourceMessage.NhomDichVuChiTiet, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(servSegrErrors[0].SERVICE_GROUP_NAME, Color.Red), FontStyle.Bold), String.Join(",", servSegrErrors.Select(o => Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(o.SERVICE_CODE, Color.Black), FontStyle.Bold)))));

                                hasMessage = true;
                            }
                            servSegrErrors = new List<V_HIS_SERV_SEGR>();
                        }

                        services = this.ServiceIsleafADOs.Where(o => o.IsChecked).OrderByDescending(o => o.NUM_ORDER ?? 0).ThenBy(o => o.SERVICE_NAME).ToList();
                    }
                    var sgNotIn = servSegrAllows.Select(o => o.SERVICE_GROUP_ID).Distinct().ToArray();
                    var searchServiceOfGroups__NoService = svgrs.Where(o => !sgNotIn.Contains(o.ID)).ToList();
                    if (searchServiceOfGroups__NoService != null && searchServiceOfGroups__NoService.Count > 0)
                    {
                        strMessageTemp__KhongDichVu.Append(String.Join(",", searchServiceOfGroups__NoService.Select(o => Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(o.SERVICE_GROUP_NAME, Color.Red), FontStyle.Bold))));
                        hasMessage = true;
                    }

                    this.toggleSwitchDataChecked.EditValue = true;

                    if (hasMessage)
                    {
                        strMessage.Append(ResourceMessage.NhomDichVuCoDichVuDuocCauHinhTrongNhomNhungKhongCoCauHinhChinhSach);
                        if (!String.IsNullOrEmpty(strMessageTemp__CoDichVuKhongCauHinh.ToString()))
                        {
                            strMessage.Append("\r\n" + String.Format(ResourceMessage.NhomDichVuCoDichVuKhongCoCauHinh, strMessageTemp__CoDichVuKhongCauHinh.ToString()));
                        }
                        if (!String.IsNullOrEmpty(strMessageTemp__KhongDichVu.ToString()))
                        {
                            strMessage.Append("\r\n" + String.Format(ResourceMessage.NhomDichVuKhongCoDichVu, strMessageTemp__KhongDichVu.ToString()));
                        }
                        strMessage.Append("\r\n" + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.CacDichVuKhongCoChinhSachGiaHoacKhongCoCauHinhSeKhongDuocChon, Color.Maroon), FontStyle.Italic));
                        WaitingManager.Hide();
                        MessageManager.Show(strMessage.ToString());
                    }
                    else
                    {
                        WaitingManager.Hide();
                    }
                    // this.VerifyWarningOverCeiling();
                }
                else
                {
                    services = this.ServiceIsleafADOs;
                    this.toggleSwitchDataChecked.EditValue = false;
                }
                this.gridControlService.DataSource = services;
                this.gridControlService.RefreshDataSource();
                this.SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = this.cboServiceGroup.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                        gridCheckMark.ClearSelection(this.cboServiceGroup.Properties.View);
                    this.cboServiceGroup.EditValue = null;
                    this.cboServiceGroup.Properties.Buttons[1].Visible = false;
                    this.gridControlService.DataSource = null;
                    this.selectedSeviceGroups = null;
                    this.ResetServiceGroupSelected();
                    this.toggleSwitchDataChecked.EditValue = false;
                    this.SetEnableButtonControl(this.actionType);
                    this.SetDefaultSerServTotalPrice();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetServiceGroupSelected()
        {
            try
            {
                foreach (var item in this.ServiceIsleafADOs)
                {
                    item.IsChecked = false;
                    item.SERVICE_GROUP_ID_SELECTEDs = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.SERVICE_GROUP_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCboServiceGroupCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboServiceGroup.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(cboServiceGroup__SelectionChange);
                cboServiceGroup.Properties.Tag = gridCheck;
                cboServiceGroup.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboServiceGroup.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboServiceGroup.Properties.View);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceGroup__SelectionChange(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_SERVICE_GROUP> sgSelectedNews = new List<HIS_SERVICE_GROUP>();
                    foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.SERVICE_GROUP_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.selectedSeviceGroups = new List<HIS_SERVICE_GROUP>();
                    this.selectedSeviceGroups.AddRange(sgSelectedNews);
                    SelectOneServiceGroupProcess(this.selectedSeviceGroups);
                }

                this.cboServiceGroup.Text = sb.ToString();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    //if (this.cboServiceGroup.EditValue != null)
                    //{
                    //    MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboServiceGroup.EditValue ?? "0").ToString()));
                    //    if (data != null)
                    //    {
                    //        this.cboServiceGroup.Properties.Buttons[1].Visible = true;
                    //        this.SelectOneServiceGroupProcess(data);
                    //    }
                    //}
                    //if (this.lciExecuteGroup.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    //{
                    //    this.FocusShowpopup(this.cboExecuteGroup, false);
                    //}
                    //else
                    //{
                    //    this.txtDescription.Focus();
                    //    this.txtDescription.SelectAll();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (this.cboServiceGroup.EditValue != null)
                    //{
                    //    //MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboServiceGroup.EditValue ?? "0").ToString()));
                    //    //if (data != null)
                    //    //{
                    //    //    this.cboServiceGroup.Properties.Buttons[1].Visible = true;
                    //    //    this.SelectOneServiceGroupProcess(data);
                    //    //}

                    //    if (this.lciExecuteGroup.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    //    {
                    //        this.FocusShowpopup(this.cboExecuteGroup, false);
                    //    }
                    //    else
                    //    {
                    //        this.txtDescription.Focus();
                    //        this.txtDescription.SelectAll();
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLoginName.Focus();
                    txtLoginName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboPriviousServiceReq.Properties.Buttons[1].Visible = false;
                    this.cboPriviousServiceReq.EditValue = null;
                    this.gridControlService.DataSource = null;
                    foreach (var item in this.ServiceIsleafADOs)
                    {
                        item.IsChecked = false;
                    }
                    this.toggleSwitchDataChecked.EditValue = false;
                    this.SetEnableButtonControl(this.actionType);
                    this.SetDefaultSerServTotalPrice();
                }
                else if (e.Button.Kind == ButtonPredefines.Search)
                {
                    WaitingManager.Show();
                    LogSystem.Debug("Begin FillDataToComboPriviousServiceReq");
                    this.FillDataToComboPriviousServiceReq(this.currentHisTreatment);
                    LogSystem.Debug("End FillDataToComboPriviousServiceReq");
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboPriviousServiceReq.EditValue != null && this.currentPreServiceReqs != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6 data = this.currentPreServiceReqs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPriviousServiceReq.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.cboPriviousServiceReq.Properties.Buttons[1].Visible = true;
                            this.ProcessChoiceServiceReqPrevious(data);
                            this.btnSave.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_INTRUCTION_TIME")
                {
                    var item = ((List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6>)this.cboPriviousServiceReq.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.INTRUCTION_TIME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboPriviousServiceReq.EditValue != null && this.currentPreServiceReqs != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6 data = this.currentPreServiceReqs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPriviousServiceReq.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.cboPriviousServiceReq.Properties.Buttons[1].Visible = true;
                            //   this.ProcessChoiceServiceReqPrevious(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_Leave(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.cboPriviousServiceReq.Text) && this.cboPriviousServiceReq.EditValue != null)
                {
                    this.cboPriviousServiceReq.EditValue = null;
                    this.cboPriviousServiceReq.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceName_Search_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("txtServiceName_Search_EditValueChanged. 1");
                if (this.notSearch > 0)
                {
                    this.notSearch--;
                    return;
                }
                var nodeCheckeds = this.treeService.GetAllCheckedNodes();
                List<SSServiceADO> listSereServADO = new List<SSServiceADO>();
                if (nodeCheckeds != null && nodeCheckeds.Count > 0 && !HisConfigCFG.IsSearchAll)
                {
                    Inventec.Common.Logging.LogSystem.Debug("txtServiceName_Search_EditValueChanged => 1");
                    var allDatas = this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0 ? this.ServiceIsleafADOs.AsQueryable() : null;

                    List<ServiceADO> parentNodes = new List<ServiceADO>();
                    foreach (var node in nodeCheckeds)
                    {
                        var data = this.treeService.GetDataRecordByNode(node) as ServiceADO;
                        if (data != null)
                        {
                            parentNodes.Add(data);
                        }
                    }
                    if (parentNodes.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("txtServiceName_Search_EditValueChanged => 2");
                        //var checkPtttSelected = parentNodes.Any(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                        //if (checkPtttSelected)
                        //{
                        //    this.ChangeStateGroupInGrid(groupType__PtttGroupName);
                        //}
                        //else
                        //{
                        //    this.ChangeStateGroupInGrid(groupType__ServiceTypeName);
                        //}
                        var parentIdAllows = parentNodes.Select(o => o.ID).ToArray();
                        //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => parentIdAllows), parentIdAllows));
                        //Lay tat ca cac dich vụ khong co cha cua tat ca cac loai dich vụ duoc check tren tree
                        var parentRootSetys = parentNodes.Where(o => String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                        if (parentRootSetys != null && parentRootSetys.Count > 0)
                        {
                            foreach (var item in parentRootSetys)
                            {
                                if (item != null)
                                {
                                    var childOfParentNodeNoParents = allDatas.Where(o =>
                                    (o.PARENT_ID == null || item.ID == o.PARENT_ID)
                                    && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID
                                    && parentIdAllows.Contains(o.PARENT_ID ?? 0)
                                    ).ToList();
                                    if (childOfParentNodeNoParents != null && childOfParentNodeNoParents.Count > 0)
                                    {
                                        listSereServADO.AddRange(childOfParentNodeNoParents);
                                    }
                                }
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug("txtServiceName_Search_EditValueChanged => 3");
                        //Lay ra tat ca cac dich vụ con cua dich vu cha va cac dich vu con cua con cua no neu co -> duyet de quy cho den het cay dich vu,..
                        var parentRoots = parentNodes.Where(o => !String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                        Inventec.Common.Logging.LogSystem.Debug("txtServiceName_Search_EditValueChanged => 4");
                        if (parentRoots != null && parentRoots.Count > 0)
                        {
                            foreach (var item in parentRoots)
                            {
                                var childs = GetChilds(item);
                                if (childs != null && childs.Count > 0)
                                {
                                    listSereServADO.AddRange(childs);
                                }
                            }
                        }
                        listSereServADO = listSereServADO.Distinct().ToList();
                        Inventec.Common.Logging.LogSystem.Debug("txtServiceName_Search_EditValueChanged => 5__listSereServADO.count = " + listSereServADO.Count);
                        this.gridControlService.DataSource = null;
                        if (!String.IsNullOrWhiteSpace(txtServiceName_Search.Text))
                        {
                            var listResult = listSereServADO.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(txtServiceName_Search.Text.ToLower().Trim())).ToList();
                            this.gridControlService.DataSource = listResult;
                        }
                        else
                        {
                            this.gridControlService.DataSource = listSereServADO;
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("txtServiceName_Search_EditValueChanged => 6__listDatasFix.count = " + ServiceIsleafADOs.Count);
                    this.gridControlService.DataSource = null;
                    if (!String.IsNullOrWhiteSpace(txtServiceName_Search.Text))
                    {
                        var listResult = (this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count() > 0) ? this.ServiceIsleafADOs.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(txtServiceName_Search.Text.ToLower().Trim())).ToList() : null;
                        this.gridControlService.DataSource = listResult;
                    }
                    else
                    {
                        this.gridControlService.DataSource = this.ServiceIsleafADOs;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("txtServiceName_Search_EditValueChanged. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceCode_Search_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("txtServiceCode_Search_EditValueChanged. 1");
                if (this.notSearch > 0)
                {
                    this.notSearch--;
                    return;
                }
                var nodeCheckeds = this.treeService.GetAllCheckedNodes();
                List<SSServiceADO> listSereServADO = new List<SSServiceADO>();
                if (nodeCheckeds != null && nodeCheckeds.Count > 0 && !HisConfigCFG.IsSearchAll)
                {
                    var allDatas = this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0 ? this.ServiceIsleafADOs.AsQueryable() : null;

                    List<ServiceADO> parentNodes = new List<ServiceADO>();
                    foreach (var node in nodeCheckeds)
                    {
                        var data = this.treeService.GetDataRecordByNode(node) as ServiceADO;
                        if (data != null)
                        {
                            parentNodes.Add(data);
                        }
                    }
                    if (parentNodes.Count > 0)
                    {
                        //var checkPtttSelected = parentNodes.Any(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                        //if (checkPtttSelected)
                        //{
                        //    this.ChangeStateGroupInGrid(groupType__PtttGroupName);
                        //}
                        //else
                        //{
                        //    this.ChangeStateGroupInGrid(groupType__ServiceTypeName);
                        //}
                        var parentIdAllows = parentNodes.Select(o => o.ID).ToArray();

                        //Lay tat ca cac dich vụ khong co cha cua tat ca cac loai dich vụ duoc check tren tree
                        var parentRootSetys = parentNodes.Where(o => String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                        if (parentRootSetys != null && parentRootSetys.Count > 0)
                        {
                            foreach (var item in parentRootSetys)
                            {
                                if (item != null)
                                {
                                    var childOfParentNodeNoParents = allDatas.Where(o =>
                                    (o.PARENT_ID == null || item.ID == o.PARENT_ID)
                                    && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID
                                    && parentIdAllows.Contains(o.PARENT_ID ?? 0)
                                    ).ToList();
                                    if (childOfParentNodeNoParents != null && childOfParentNodeNoParents.Count > 0)
                                    {
                                        listSereServADO.AddRange(childOfParentNodeNoParents);
                                    }
                                }
                            }
                        }

                        //Lay ra tat ca cac dich vụ con cua dich vu cha va cac dich vu con cua con cua no neu co -> duyet de quy cho den het cay dich vu,..
                        var parentRoots = parentNodes.Where(o => !String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                        if (parentRoots != null && parentRoots.Count > 0)
                        {
                            foreach (var item in parentRoots)
                            {
                                var childs = GetChilds(item);
                                if (childs != null && childs.Count > 0)
                                {
                                    listSereServADO.AddRange(childs);
                                }
                            }
                        }
                        listSereServADO = listSereServADO.Distinct().ToList();
                        this.gridControlService.DataSource = null;
                        if (!String.IsNullOrWhiteSpace(txtServiceCode_Search.Text))
                        {
                            var listResult = listSereServADO.Where(o => o.SERVICE_CODE_HIDDEN.ToLower().Contains(txtServiceCode_Search.Text.ToLower().Trim())).ToList();
                            this.gridControlService.DataSource = listResult;
                        }
                        else
                        {
                            this.gridControlService.DataSource = listSereServADO;
                        }
                    }
                }
                else
                {
                    this.gridControlService.DataSource = null;
                    if (!String.IsNullOrWhiteSpace(txtServiceCode_Search.Text))
                    {
                        var listResult = (this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count() > 0) ? this.ServiceIsleafADOs.Where(o => o.SERVICE_CODE_HIDDEN.ToLower().Contains(txtServiceCode_Search.Text.ToLower().Trim())).ToList() : null;
                        this.gridControlService.DataSource = listResult;
                    }
                    else
                    {
                        this.gridControlService.DataSource = this.ServiceIsleafADOs;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("txtServiceCode_Search_EditValueChanged. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceName_Search_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var sereServADO = (SSServiceADO)this.gridViewService.GetFocusedRow();
                    if (sereServADO != null)
                    {
                        //sereServADO.IsChecked = true;
                        if (sereServADO.IsChecked)
                        {
                            this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.ID, sereServADO);
                            this.ChooseCotDefaultlService(sereServADO.ID, sereServADO);
                            this.ValidServiceDetailProcessing(sereServADO);
                            //this.ProcessNoDifferenceHeinServicePrice(sereServADO);
                            //this.VerifyWarningOverCeiling();
                            if (CheckExistServicePaymentLimit(sereServADO.SERVICE_CODE))
                            {
                                MessageBox.Show("Dịch vụ cận lâm sàng có giới hạn chỉ định thanh toán BHYT. Đề nghị BS xem xét trước khi chỉ định", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            this.ResetOneService(sereServADO);
                            //sereServADO.IsNoDifference = false;
                        }

                        this.gridControlService.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);
                        this.SetDefaultSerServTotalPrice();
                        // focus vào ô tìm kiếm (Auto filter)
                        //gridViewServiceProcess.FocusedRowHandle = -2147483646;
                        txtServiceName_Search.Focus();
                        txtServiceName_Search.SelectAll();
                    }
                }
                //else if (e.KeyCode == Keys.Space)
                //{
                //    var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                //    if (sereServADO != null)
                //    {
                //        UpdateCurrentFocusRow(sereServADO);
                //    }
                //}
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewService.Focus();
                    gridViewService.FocusedRowHandle = 0;
                    gridViewService.FocusedColumn = grcServiceName_TabService;
                }
                //GridView view = sender as GridView;
                //if (e.KeyCode == Keys.Enter && view.IsFilterRow(view.FocusedRowHandle))
                //{
                //    CriteriaOperator op = view.ActiveFilterCriteria;
                //}
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtServiceCode_Search_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var sereServADO = (SSServiceADO)this.gridViewService.GetFocusedRow();
                    if (sereServADO != null)
                    {
                        //sereServADO.IsChecked = true;
                        if (sereServADO.IsChecked)
                        {
                            this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.ID, sereServADO);
                            this.ChooseCotDefaultlService(sereServADO.ID, sereServADO);
                            this.ValidServiceDetailProcessing(sereServADO);
                            //this.ProcessNoDifferenceHeinServicePrice(sereServADO);
                            //this.VerifyWarningOverCeiling();
                            if (CheckExistServicePaymentLimit(sereServADO.SERVICE_CODE))
                            {
                                MessageBox.Show("Dịch vụ cận lâm sàng có giới hạn chỉ định thanh toán BHYT. Đề nghị BS xem xét trước khi chỉ định", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            this.ResetOneService(sereServADO);
                            //sereServADO.IsNoDifference = false;
                        }

                        this.gridControlService.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);
                        this.SetDefaultSerServTotalPrice();
                        // focus vào ô tìm kiếm (Auto filter)
                        //gridViewServiceProcess.FocusedRowHandle = -2147483646;
                        txtServiceCode_Search.Focus();
                        txtServiceCode_Search.SelectAll();
                    }
                }
                //else if (e.KeyCode == Keys.Space)
                //{
                //    var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                //    if (sereServADO != null)
                //    {
                //        UpdateCurrentFocusRow(sereServADO);
                //    }
                //}
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewService.Focus();
                    gridViewService.FocusedRowHandle = 0;
                    gridViewService.FocusedColumn = grcServiceCode_TabService;
                }
                //GridView view = sender as GridView;
                //if (e.KeyCode == Keys.Enter && view.IsFilterRow(view.FocusedRowHandle))
                //{
                //    CriteriaOperator op = view.ActiveFilterCriteria;
                //}
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region Button
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProcessSaveData(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProcessSaveData(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeLockButtonWhileProcess(bool isLock)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                    return;

                this.btnSave.Enabled = isLock;
                this.btnSaveAndPrint.Enabled = isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnSaveShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnSave.Enabled)
                    this.btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnSaveAndPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnSaveAndPrint.Enabled)
                    this.btnSaveAndPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!this.btnSave.Enabled)
                {
                    // DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetDefaultData();
                this.LoadIcdDefault();

                await LoadTotalSereServByHeinWithTreatment(this.treatmentId);

                this.SetEnableButtonControl(this.actionType);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void barbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShowDetail_Click(object sender, EventArgs e)
        {
            try
            {
                frmDetail frmDetail = new frmDetail(this.serviceReqComboResultSDO, this.currentHisPatientTypeAlter, currentHisTreatment, this.currentModule);
                frmDetail.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSereservInTreatmentPreview_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SereservInTreatment").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SereservInTreatment'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SereservInTreatment' is not plugins");
                HIS.Desktop.ADO.SereservInTreatmentADO sereservInTreatmentADO = new HIS.Desktop.ADO.SereservInTreatmentADO(this.treatmentId, intructionTimeSelecteds.First(), serviceReqParentId ?? 0);
                List<object> listArgs = new List<object>();
                listArgs.Add(sereservInTreatmentADO);
                listArgs.Add(this.currentModule);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnServiceReqList_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ServiceReqList' is not plugins");

                MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                treatment.ID = this.treatmentId;
                List<object> listArgs = new List<object>();
                listArgs.Add(treatment);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTomLuocVienPhi_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.AggrHospitalFees' is not plugins");

                List<object> listArgs = new List<object>();
                listArgs.Add(this.treatmentId);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void getSereServForBill()
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LoadDataToCashierRoom()
        {
            try
            {
                List<V_HIS_CASHIER_ROOM> cashierRooms;
                if (WorkPlace.GetRoomIds() != null && WorkPlace.GetRoomIds().Count > 0)
                {
                    cashierRooms = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().Where(o => WorkPlace.GetRoomIds().Contains(o.ROOM_ID)).ToList();
                }
                else
                {
                    cashierRooms = new List<V_HIS_CASHIER_ROOM>();
                }
                cboCashierRoom.Properties.DataSource = cashierRooms;
                cboCashierRoom.Properties.DisplayMember = "CASHIER_ROOM_NAME";
                cboCashierRoom.Properties.ValueMember = "ID";
                cboCashierRoom.Properties.ForceInitialize();
                cboCashierRoom.Properties.Columns.Clear();
                cboCashierRoom.Properties.Columns.Add(new LookUpColumnInfo("CASHIER_ROOM_CODE", "", 50));
                cboCashierRoom.Properties.Columns.Add(new LookUpColumnInfo("CASHIER_ROOM_NAME", "", 200));
                cboCashierRoom.Properties.ShowHeader = false;
                cboCashierRoom.Properties.ImmediatePopup = true;
                cboCashierRoom.Properties.DropDownRows = 10;
                cboCashierRoom.Properties.PopupWidth = 250;
                // đặt giá trị mặc định cho phòng thu ngân
                if (cashierRooms != null && cashierRooms.Count > 0)
                {
                    cboCashierRoom.EditValue = cashierRooms.FirstOrDefault().ID;
                }
                else
                {
                    cboCashierRoom.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCreateBill_Click(object sender, EventArgs e)
        {
            try
            {
                // get treatment
                CommonParam param = new CommonParam();
                V_HIS_TREATMENT_FEE currentTreatment = null;
                MOS.Filter.HisTreatmentFeeViewFilter treatmentViewFilter = new HisTreatmentFeeViewFilter();
                treatmentViewFilter.ID = this.treatmentId;
                var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, treatmentViewFilter, param);
                if (treatments != null && treatments.Count > 0)
                {
                    currentTreatment = treatments.FirstOrDefault();
                }

                // get sereServs
                //- Lấy các dịch vụ đã chỉ định mà chưa thanh toán (ko thuộc sere_SErv_bill).
                //- Áp dụng cho các dịch vụ viện phí (Không load các dịch vụ có đối tượng thanh toán là BHYT)
                //- Lấy các dịch vụ có creator là người đăng nhập.
                //- Mở form thanh toán như của thu ngân.
                //- Phòng thanh toán là phòng thu ngân mà người dùng đang mở cùng với phòng xử lý (giải pháp như tiếp đón).
                MOS.Filter.HisSereServView5Filter sereServViewFilter = new HisSereServView5Filter();
                sereServViewFilter.TDL_TREATMENT_ID = this.treatmentId;
                var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumer.ApiConsumers.MosConsumer, sereServViewFilter, param);
                // get sereServBills
                if (sereServs == null || sereServs.Count == 0)
                {
                    return;
                }
                MOS.Filter.HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                sereServBillFilter.SERE_SERV_IDs = sereServs.Select(p => p.ID).Distinct().ToList();
                var sereServBills = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServBillFilter, param);
                if (sereServBills != null && sereServBills.Count > 0)
                {
                    sereServs = sereServs.Where(o => !sereServBills.Select(p => p.SERE_SERV_ID).Distinct().ToList().Contains(o.ID)).ToList();
                }
                // lọc các dịch vụ viện phí, các dịch vụ có creator là người đăng nhập
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                sereServs = sereServs.Where(o => o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT && o.CREATOR == loginName).ToList();

                if (!btnCreateBill.Enabled || currentTreatment == null)
                    return;
                if (cboCashierRoom.EditValue == null)
                {
                    MessageBox.Show(ResourceMessage.ChuaChonPhongThuNgan, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                var cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboCashierRoom.EditValue.ToString()));
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBill").FirstOrDefault();
                if (sereServs == null || sereServs.Count == 0)
                {
                    MessageBox.Show(ResourceMessage.HSDTKhongCoHoacDaThanhToanDichVu, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionBill'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = cashierRoom.ROOM_ID;
                    moduleData.RoomTypeId = cashierRoom.ROOM_TYPE_ID;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentTreatment);
                    listArgs.Add(moduleData);
                    listArgs.Add(sereServs);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, cashierRoom.ROOM_ID, cashierRoom.ROOM_TYPE_ID), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    //FillDataToControlBySelectTreatment(true);
                    //txtFindTreatmentCode.Focus();
                    //txtFindTreatmentCode.SelectAll();
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private async Task LoadTotalSereServByHeinWithTreatment(long treatmentId)
        {
            try
            {
                //LogSystem.Info("LoadTotalSereServByHeinWithTreatment => start");
                CommonParam param = new CommonParam();
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = treatmentId;
                hisSereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;

                var totalHeinByTreatments = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilter, param);
                totalHeinByTreatment = totalHeinByTreatments != null && totalHeinByTreatments.Count > 0 ? totalHeinByTreatments.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0) : 0;
                //LogSystem.Info("LoadTotalSereServByHeinWithTreatment => end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string GetTreatmentTypeNameByCode(string code)
        {
            string name = "";
            try
            {
                name = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == code).TREATMENT_TYPE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return name;
        }

        private decimal GetTotalPriceServiceSelected(long patientTypeId)
        {
            decimal totalPrice = 0;
            try
            {
                //List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                //foreach (var item in serviceCheckeds__Send)
                //{
                //    if (item.IsChecked
                //        && ((patientTypeId > 0 && item.PATIENT_TYPE_ID == patientTypeId) || (patientTypeId <= 0 && item.PATIENT_TYPE_ID > 0))
                //        && (item.IsExpend ?? false) == false)
                //    {
                //        if (this.servicePatyInBranchs.ContainsKey(item.SERVICE_ID))
                //        {
                //            var data_ServicePrice = this.servicePatyInBranchs[item.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                //            //var data_ServicePrice = this.servicePatyInBranchs.Where(o => o == item.SERVICE_ID && o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                //            if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                //            {
                //                totalPrice += item.AMOUNT * (data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO));
                //            }
                //        }

                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return totalPrice;
        }

        private void cboCashierRoom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboCashierRoom.EditValue != null)
                    {
                        var account = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboCashierRoom.EditValue));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnBoSungPhacDo_Click(object sender, EventArgs e)
        {

            try
            {
                //Lay danh sach icd
                List<HIS_ICD> icdCodeArr = getIcdCodeListFromUcIcd();

                if (icdCodeArr == null || icdCodeArr.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy thông tin ICD", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                //List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                //if (serviceCheckeds__Send == null || serviceCheckeds__Send.Count == 0)
                //{
                //    MessageBox.Show("Vui lòng chọn dịch vụ", "Thông báo",
                //                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                //List<long> serviceIds = serviceCheckeds__Send.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).ToList();
                //CommonParam param = new CommonParam();
                //HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                //icdServiceFilter.SERVICE_IDs = serviceIds;
                //icdServiceFilter.ICD_CODE__EXACTs = icdCodeArr.Select(o => o.ICD_CODE).Distinct().ToList();
                //List<HIS_ICD_SERVICE> icdServices = new BackendAdapter(param)
                //.Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);
                //List<long> icdServiceIds = icdServices.Select(o => o.SERVICE_ID).Distinct().ToList();
                //List<long> serviceNotConfigIds = new List<long>();
                //foreach (var item in serviceIds)
                //{
                //    if (!icdServiceIds.Contains(item))
                //    {
                //        serviceNotConfigIds.Add(item);
                //    }
                //}

                //if (serviceNotConfigIds == null || serviceNotConfigIds.Count == 0)
                //{
                //    MessageBox.Show("Không tìm thấy dịch vụ chưa được cấu hình", "Thông báo",
                //                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}

                //List<HIS_ICD> icds = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().Where(o => icdCodeArr.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).Distinct().ToList();
                //if (icds == null || icds.Count == 0)
                //{
                //    LogSystem.Debug("Khong tim thay ICD");
                //    return;
                //}


                // List<object> listObj = new List<object>();
                // listObj.Add(icdCodeArr);
                // listObj.Add(serviceNotConfigIds);
                //  HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceIcd", currentModule.RoomId, currentModule.RoomTypeId, listObj);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemMemoExEdit_IntructionNote_Popup(object sender, EventArgs e)
        {
            MemoExPopupForm form = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as MemoExPopupForm;
            form.OkButton.Text = Inventec.Common.Resource.Get.Value("frmAssignNutrition.InstructionNoteMemoEx.OkButtion", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            form.CloseButton.Text = Inventec.Common.Resource.Get.Value("frmAssignNutrition.InstructionNoteMemoEx.CloseButtion", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }

        private void gridViewServiceProcess_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            //try
            //{
            //    MyGridView view = (MyGridView)sender;
            //    if (!view.OptionsView.ShowAutoFilterRow || !view.IsDataRow(e.RowHandle))
            //        return;

            //    string filterCellText = view.GetRowCellDisplayText(GridControl.AutoFilterRowHandle, e.Column);

            //    if (String.IsNullOrEmpty(filterCellText))
            //        return;

            //    string temp = MyGridView.RemoveDiacritics(e.DisplayText, true);
            //    int filterTextIndex = temp.IndexOf(filterCellText, StringComparison.CurrentCultureIgnoreCase);
            //    if (filterTextIndex == -1)
            //        return;

            //    XPaint.Graphics.DrawMultiColorString(e.Cache, e.Bounds, e.DisplayText, filterCellText, e.Appearance, Color.Black, Color.Gold, false, filterTextIndex);
            //    e.Handled = true;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void gridControlServiceProcess_ProcessGridKey(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    if (gridViewServiceProcess.FocusedRowHandle < 0)
            //    {
            //        gridViewServiceProcess.FocusedRowHandle = 0;
            //    }
            //    else
            //    {
            //        gridViewServiceProcess.FocusedRowHandle++;
            //    }

            //    //gridViewServiceProcess.FocusedColumn = grcChecked_TabService;
            //    e.Handled = true;
            //}
        }

        private void gridViewServiceProcess_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                //if (gridViewServiceProcess.RowCount == 2)
                //{
                //    var sereServADO = (SereServADO)this.gridViewServiceProcess.GetRow(0);
                //    if (sereServADO != null)
                //    {
                //        sereServADO.IsChecked = true;
                //        if (sereServADO.IsChecked)
                //        {
                //            this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
                //            this.ValidServiceDetailProcessing(sereServADO);
                //            this.ProcessNoDifferenceHeinServicePrice(sereServADO);
                //            this.VerifyWarningOverCeiling();
                //        }
                //        else
                //        {
                //            this.ResetOneService(sereServADO);
                //            sereServADO.IsNoDifference = false;
                //        }

                //        this.gridControlServiceProcess.RefreshDataSource();
                //        this.SetEnableButtonControl(this.actionType);
                //        this.SetDefaultSerServTotalPrice();

                //        gridViewServiceProcess.ActiveEditor.SelectAll();

                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region ---BuaAn ----
        private void repositoryItemGridLookUp__BuaAn_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = this.repositoryItemGridLookUp__BuaAn.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                        gridCheckMark.ClearSelection(this.repositoryItemGridLookUp__BuaAn.View);
                    this.repositoryItemGridLookUp__BuaAn.View.EditingValue = null;
                    this.repositoryItemGridLookUp__BuaAn.Buttons[1].Visible = false;
                    this.gridControlService.DataSource = null;
                    this.selectedSeviceGroups = null;
                    // this.ResetServiceGroupSelected();
                    this.toggleSwitchDataChecked.EditValue = false;
                    this.SetEnableButtonControl(this.actionType);
                    this.SetDefaultSerServTotalPrice();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetServiceGroupSelected123()
        {
            try
            {
                //foreach (var item in this.ServiceIsleafADOs)
                //{
                //    if (item.SERVICE_GROUP_ID_SELECTEDs != null && item.SERVICE_GROUP_ID_SELECTEDs.Count > 0)
                //    {
                //        item.IsChecked = false;
                //        item.SERVICE_GROUP_ID_SELECTEDs = null;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUp__BuaAn_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                //GridLookUpEdit grrrrr = gridCheckMark.CurrentRepository.OwnerEdit as GridLookUpEdit;
                foreach (MOS.EFMODEL.DataModels.HIS_RATION_TIME rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.RATION_TIME_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCboBuaAnCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(this.repositoryItemGridLookUp__BuaAn);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(repositoryItemGridLookUp__BuaAn__SelectionChange);
                this.repositoryItemGridLookUp__BuaAn.Tag = gridCheck;
                this.repositoryItemGridLookUp__BuaAn.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = this.repositoryItemGridLookUp__BuaAn.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(this.repositoryItemGridLookUp__BuaAn.View);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboBuaAnCheck(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(repositoryItemGridLookUp__BuaAn__SelectionChange);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    // gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridLookUp__BuaAn__SelectionChange(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                StringBuilder sb = new StringBuilder();
                this.selectedRationTimes = new List<HIS_RATION_TIME>();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    GridLookUpEdit grrrrr = gridCheckMark.CurrentRepository.OwnerEdit as GridLookUpEdit;
                    List<HIS_RATION_TIME> sgSelectedNews = new List<HIS_RATION_TIME>();
                    foreach (MOS.EFMODEL.DataModels.HIS_RATION_TIME rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.RATION_TIME_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    grrrrr.Text = sb.ToString();
                    this.selectedRationTimes.AddRange(sgSelectedNews);
                    var data = (SSServiceADO)gridViewService.GetFocusedRow();
                    SelectOneRationGroupProcess(this.selectedRationTimes, data, sb);
                }
                // this.repositoryItemGridLookUp__BuaAn.DisplayMember = sb.ToString();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectOneRationGroupProcess(List<HIS_RATION_TIME> svgrs, SSServiceADO data, StringBuilder str)
        {
            try
            {
                // var data = (SSServiceADO)gridViewService.GetFocusedRow();
                if (data != null && svgrs != null && svgrs.Count > 0)
                {
                    if (!data.IsChecked)
                        data.IsChecked = true;
                    data.RationTimeIds = svgrs.Select(p => p.ID).ToList();
                    data.BUA_AN_NAME = str.ToString();
                }
                else
                {
                    if (data.IsChecked)
                        data.IsChecked = false;
                    data.BUA_AN_NAME = "";
                }
                this.gridControlService.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUp__BuaAn_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    //if (this.repositoryItemGridLookUp__BuaAn.EditValue != null)
                    //{
                    //    MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.repositoryItemGridLookUp__BuaAn.EditValue ?? "0").ToString()));
                    //    if (data != null)
                    //    {
                    //        this.repositoryItemGridLookUp__BuaAn.Properties.Buttons[1].Visible = true;
                    //        this.SelectOneServiceGroupProcess(data);
                    //    }
                    //}
                    //if (this.lciExecuteGroup.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    //{
                    //    this.FocusShowpopup(this.cboExecuteGroup, false);
                    //}
                    //else
                    //{
                    //    this.txtDescription.Focus();
                    //    this.txtDescription.SelectAll();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUp__BuaAn_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (this.repositoryItemGridLookUp__BuaAn.EditValue != null)
                    //{
                    //    //MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.repositoryItemGridLookUp__BuaAn.EditValue ?? "0").ToString()));
                    //    //if (data != null)
                    //    //{
                    //    //    this.repositoryItemGridLookUp__BuaAn.Properties.Buttons[1].Visible = true;
                    //    //    this.SelectOneServiceGroupProcess(data);
                    //    //}

                    //    if (this.lciExecuteGroup.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    //    {
                    //        this.FocusShowpopup(this.cboExecuteGroup, false);
                    //    }
                    //    else
                    //    {
                    //        this.txtDescription.Focus();
                    //        this.txtDescription.SelectAll();
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion


        private void UpdateCurrentFocusRow(SSServiceADO sereServADO)
        {
            try
            {
                if (sereServADO == null)
                    return;

                sereServADO.IsChecked = !sereServADO.IsChecked;
                if (sereServADO.IsChecked)
                {
                    this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.ID, sereServADO);
                    this.ChooseExecuteRoomDefaultlService(sereServADO.ID, sereServADO);
                    this.ChooseCotDefaultlService(sereServADO.ID, sereServADO);
                    this.ValidServiceDetailProcessing(sereServADO);
                    //this.ProcessNoDifferenceHeinServicePrice(sereServADO);
                    //if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                    //{
                    //    MessageBox.Show("Dịch vụ cận lâm sàng có giới hạn chỉ định thanh toán BHYT. Đề nghị BS xem xét trước khi chỉ định", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}
                }
                else
                {
                    this.ResetOneService(sereServADO);
                    //sereServADO.IsNoDifference = false;
                }

                this.gridControlService.RefreshDataSource();
                //if (sereServADO.IsChecked)
                //{
                //    this.VerifyWarningOverCeiling();
                //}
                this.SetEnableButtonControl(this.actionType);
                this.SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_BuaAn_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Down)
                {
                    this.repositoryItemGridLookUp__BuaAn.View.ShowPopupEditForm();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_ShowingEditor(object sender, CancelEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled) return;

                //TODO
                ProcessingPrintV2("Mps000275");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnExpendAll_Click(object sender, EventArgs e)
        {
            try
            {
                this.treeService.CollapseAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExpendAll_Click(object sender, EventArgs e)
        {
            try
            {
                IscheckAllTreeService = true;
                this.treeService.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void treeService_Click(object sender, EventArgs e)
        {
            try
            {
                IscheckAllTreeService = false;
                if (HisConfigCFG.IsSingleCheckservice != commonString__true)
                {
                    // Nếu đang chọn huyết học của xét nghiệm mà bấm vào chẩn đoán hình ảnh (check ô vuông) thì chưa hủy tick huyết học => MM: tự động mở cây của chẩn đoán hình ảnh và tick các dịch vụ con của chẩn đoán hình ảnh đồng thời hủy tick huyết học, thu lại cây xét nghiệm.

                    foreach (TreeListNode treeListNode in treeService.GetAllCheckedNodes())
                    {
                        List<TreeListNode> allParentNodes = new List<TreeListNode>();
                        GetParent(treeListNode, allParentNodes);
                        var checkParentNode = allParentNodes.FirstOrDefault(o => o.CheckState == CheckState.Unchecked);
                        if (checkParentNode != null)
                        {
                            treeListNode.CheckState = CheckState.Unchecked;
                        }
                    }

                }
                if (this.treeService.FocusedNode != null)
                {
                    //Process expand node
                    var parent = this.treeService.FocusedNode.ParentNode;
                    //Trường hợp node đang chọn có cha
                    if (parent != null)
                    {
                        this.ProcessExpandTree(this.treeService.FocusedNode);
                    }
                    //Trường hợp node đang chọn không có cha
                    else
                    {
                        this.treeService.CollapseAll();
                        this.treeService.FocusedNode.Expanded = true;
                        bool checkState = this.treeService.FocusedNode.Checked;

                        if (HisConfigCFG.IsSingleCheckservice == commonString__true)
                        {
                            this.treeService.UncheckAll();
                        }

                        if (checkState)
                            this.treeService.FocusedNode.CheckAll();
                    }

                    //Process check state node is leaf
                    var data = this.treeService.GetDataRecordByNode(this.treeService.FocusedNode);
                    if (this.treeService.FocusedNode != null
                        && !this.treeService.FocusedNode.HasChildren
                        && data != null
                        && data is ServiceADO)
                    {
                        //Cấu hình cho phép chọn một/nhiều nhóm dịch vụ trên cây là node lá
                        //Nếu không có cấu hình thì mặc định là chọn nhiều
                        //Nếu có cấu hình thì xử lý theo cấu hình
                        if (HisConfigCFG.IsSingleCheckservice == commonString__true)
                        {
                            if (parent != null)
                            {
                                parent.UncheckAll();
                            }
                            this.treeService.FocusedNode.Checked = true;
                        }
                        else
                        {
                            this.treeService.FocusedNode.Checked = !this.treeService.FocusedNode.Checked;
                        }
                    }

                    this.toggleSwitchDataChecked.EditValue = false;
                    if (!this.treeService.FocusedNode.HasChildren)
                    {
                        this.LoadDataToGrid(true);
                        this.SetDefaultSerServTotalPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                var data = this.treeService.GetDataRecordByNode(e.Node) as ServiceADO;
                if (HisConfigCFG.IsSingleCheckservice == commonString__true)
                {
                    this.treeService.FocusedNode = e.Node;
                    //if (e.Node.ParentNode != null)
                    //{
                    //    e.Node.ParentNode.UncheckAll();
                    //}
                    //e.Node.Checked = true;
                }
                else
                {
                    // Nếu đang chọn huyết học của xét nghiệm mà bấm vào chẩn đoán hình ảnh (check ô vuông) thì chưa hủy tick huyết học => MM: tự động mở cây của chẩn đoán hình ảnh và tick các dịch vụ con của chẩn đoán hình ảnh đồng thời hủy tick huyết học, thu lại cây xét nghiệm.

                    foreach (TreeListNode treeListNode in treeService.GetAllCheckedNodes())
                    {
                        List<TreeListNode> allParentNodes = new List<TreeListNode>();
                        GetParent(treeListNode, allParentNodes);
                        var checkParentNode = allParentNodes.FirstOrDefault(o => o.CheckState == CheckState.Unchecked);
                        if (checkParentNode != null)
                        {
                            treeListNode.CheckState = CheckState.Unchecked;
                        }
                    }
                }
                this.toggleSwitchDataChecked.EditValue = false;
                this.LoadDataToGrid(true);
                this.SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                //issue 13991
                if (HisConfigCFG.IsSingleCheckservice == commonString__true)
                {
                    treeService.UncheckAll();
                }
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
                treeService.FocusedNode = e.Node;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.treeService.FocusedNode != null)
                    {
                        this.treeService.FocusedNode.Checked = true;
                        this.gridControlService.Focus();
                        this.gridViewService.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    var node = this.treeService.FocusedNode;
                    var data = this.treeService.GetDataRecordByNode(node);
                    if (node != null && node.HasChildren && data != null && data is ServiceADO)
                    {
                        node.Expanded = !node.Expanded;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_BeforeExpand(object sender, DevExpress.XtraTreeList.BeforeExpandEventArgs e)
        {
            try
            {
                if (e.Node != null && !IscheckAllTreeService)
                {

                    //Process expand node
                    var parent = e.Node.ParentNode;
                    //Trường hợp node đang chọn có cha
                    if (parent != null)
                    {
                        this.ProcessExpandTree(e.Node);
                    }
                    //Trường hợp node đang chọn không có cha
                    else
                    {
                        this.treeService.CollapseAll();
                        e.Node.Expanded = true;
                        //bool checkState = this.treeService.FocusedNode.Checked;
                        //this.treeService.UncheckAll();
                        //if (checkState)
                        //    this.treeService.FocusedNode.CheckAll();
                    }
                    // bỏ focus node tránh trường hợp sang hàm click tree
                    this.treeService.FocusedNode = e.Node;
                    //Process check state node is leaf TODO
                    //var data = this.treeService.GetDataRecordByNode(this.treeService.FocusedNode);
                    //if (this.treeService.FocusedNode != null
                    //    //&& !this.treeService.FocusedNode.HasChildren
                    //    && data != null
                    //    && data is ServiceADO)
                    //{
                    //    //Cấu hình cho phép chọn một/nhiều nhóm dịch vụ trên cây là node lá
                    //    //Nếu không có cấu hình thì mặc định là chọn nhiều
                    //    //Nếu có cấu hình thì xử lý theo cấu hình
                    //    if (HisConfigCFG.IsSingleCheckservice == commonString__true)
                    //    {
                    //        if (parent != null)
                    //        {
                    //            parent.UncheckAll();
                    //        }
                    //        this.treeService.FocusedNode.Checked = true;
                    //    }
                    //    else
                    //    {
                    //        this.treeService.FocusedNode.Checked = !this.treeService.FocusedNode.Checked;
                    //    }
                    //}

                    //this.toggleSwitchDataChecked.EditValue = false;
                    //this.LoadDataToGrid(true);
                    //this.SetDefaultSerServTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessExpandTree(TreeListNode focusedNode)
        {
            try
            {
                TreeListNode parent = focusedNode.ParentNode;
                //bool checkState = treeService.FocusedNode.Checked;
                if (parent != null)
                {
                    this.treeService.CollapseAll();
                    List<TreeListNode> allParentNodes = new List<TreeListNode>();
                    this.GetParent(focusedNode, allParentNodes);
                    if (allParentNodes != null && allParentNodes.Count > 0)
                    {
                        var nodes = this.treeService.GetNodeList();
                        foreach (var item in nodes)
                        {
                            //item.Checked = false;
                            if (focusedNode == item)
                            {
                                focusedNode.Expanded = true;
                                //var childNodes = nodes.Where(o => o.ParentNode == focusedNode).ToList();
                                //if (childNodes != null && childNodes.Count > 0)
                                //{
                                //    foreach (var childOfChild in childNodes)
                                //    {
                                //        //childOfChild.Expanded = true;
                                //    }
                                //}
                            }
                            else if (allParentNodes.ToArray().Contains(item))
                            {
                                item.Expanded = true;
                            }
                            else
                            {
                                item.Expanded = false;
                            }
                        }
                    }

                    //treeService.FocusedNode.Expanded = true;

                    //treeService.UncheckAll();
                    //if (checkState)
                    //    treeService.FocusedNode.CheckAll();
                }
                this.treeService.FocusedNode = focusedNode;
                //parent.ExpandAll();
                //else
                //{
                //    treeService.CollapseAll();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetParent(TreeListNode focusedNode, List<TreeListNode> parentNodes)
        {
            try
            {
                if (focusedNode != null && focusedNode.ParentNode != null)
                {
                    parentNodes.Add(focusedNode.ParentNode);
                    this.GetParent(focusedNode.ParentNode, parentNodes);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentBedRoom_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_TREATMENT_BED_ROOM treatmentBedRoom = (V_HIS_TREATMENT_BED_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        if (treatmentBedRoom != null && treatmentBedRoom.TDL_PATIENT_DOB.ToString().Length >= 4)
                        {
                            e.Value = treatmentBedRoom.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    treatmentIds.AddRange(GetSelectedRows().Select(o => o.TREATMENT_ID).ToList());
                    treatmentIds = treatmentIds.Distinct().ToList();
                    FillDataToGridTreatment();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditOtherPaySource_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                this.currentRowSereServADO = (SSServiceADO)gridViewService.GetFocusedRow();
                if (this.currentRowSereServADO != null && this.currentRowSereServADO.IsChecked)
                {
                    if (e.Button.Kind == ButtonPredefines.Down || e.Button.Kind == ButtonPredefines.DropDown)
                    {
                        ButtonEdit editor = sender as ButtonEdit;
                        Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                        popupControlContainerOtherPaySource.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 160));

                        var dataOtherPaySources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                        List<HIS_OTHER_PAY_SOURCE> dataOtherPaySourceTmps = new List<HIS_OTHER_PAY_SOURCE>();
                        dataOtherPaySources = (dataOtherPaySources != null && dataOtherPaySources.Count > 0) ? dataOtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                        if (dataOtherPaySources != null && dataOtherPaySources.Count > 0)
                        {
                            var workingPatientType = currentPatientTypes.Where(t => t.ID == this.currentRowSereServADO.PATIENT_TYPE_ID).FirstOrDefault();

                            if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS))
                            {
                                dataOtherPaySourceTmps = dataOtherPaySources.Where(o => ("," + workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList();
                            }
                            else
                            {
                                dataOtherPaySourceTmps.AddRange(dataOtherPaySources);
                            }

                        }

                        gridControlOtherPaySource.DataSource = null;
                        gridControlOtherPaySource.DataSource = dataOtherPaySourceTmps;
                        gridControlOtherPaySource.Focus();

                        int focusRow = 0;
                        if (this.currentRowSereServADO.OTHER_PAY_SOURCE_ID > 0 && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Count > 0)
                        {

                            for (int i = 0; i < dataOtherPaySourceTmps.Count; i++)
                            {
                                if (dataOtherPaySourceTmps[i].ID == this.currentRowSereServADO.OTHER_PAY_SOURCE_ID)
                                {
                                    focusRow = i;
                                }
                            }
                        }
                        gridViewOtherPaySource.FocusedRowHandle = focusRow;
                    }
                    else if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        this.currentRowSereServADO.OTHER_PAY_SOURCE_ID = null;
                        this.currentRowSereServADO.OTHER_PAY_SOURCE_CODE = "";
                        this.currentRowSereServADO.OTHER_PAY_SOURCE_NAME = "";
                        this.gridControlService.RefreshDataSource();

                        if (this.gridViewService.IsEditing)
                            this.gridViewService.CloseEditor();

                        if (this.gridViewService.FocusedRowModified)
                            this.gridViewService.UpdateCurrentRow();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlOtherPaySource_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_OTHER_PAY_SOURCE conditionRow = (HIS_OTHER_PAY_SOURCE)gridViewOtherPaySource.GetFocusedRow();
                if (conditionRow != null)
                {
                    popupControlContainerOtherPaySource.HidePopup();
                    this.currentRowSereServADO = (SSServiceADO)gridViewService.GetFocusedRow();
                    if (this.currentRowSereServADO != null)
                    {
                        this.currentRowSereServADO.OTHER_PAY_SOURCE_ID = conditionRow.ID;
                        this.currentRowSereServADO.OTHER_PAY_SOURCE_NAME = conditionRow.OTHER_PAY_SOURCE_NAME;
                        this.gridControlService.RefreshDataSource();

                        if (this.gridViewService.IsEditing)
                            this.gridViewService.CloseEditor();

                        if (this.gridViewService.FocusedRowModified)
                            this.gridViewService.UpdateCurrentRow();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewOtherPaySource_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var conditionRow = (HIS_OTHER_PAY_SOURCE)this.gridViewOtherPaySource.GetFocusedRow();
                    if (conditionRow != null)
                    {
                        popupControlContainerOtherPaySource.HidePopup();
                        this.currentRowSereServADO = (SSServiceADO)gridViewService.GetFocusedRow();
                        if (this.currentRowSereServADO != null)
                        {
                            this.currentRowSereServADO.OTHER_PAY_SOURCE_ID = conditionRow.ID;
                            this.currentRowSereServADO.OTHER_PAY_SOURCE_NAME = conditionRow.OTHER_PAY_SOURCE_NAME;
                            this.gridControlService.RefreshDataSource();

                            if (this.gridViewService.IsEditing)
                                this.gridViewService.CloseEditor();

                            if (this.gridViewService.FocusedRowModified)
                                this.gridViewService.UpdateCurrentRow();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewOtherPaySource_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        SSServiceADO ssADO = (SSServiceADO)gridViewService.GetFocusedRow();
                        if (ssADO != null)
                        {
                            List<HIS_OTHER_PAY_SOURCE> servicePatieDatas = ((List<HIS_OTHER_PAY_SOURCE>)((BaseView)sender).DataSource);

                            HIS_OTHER_PAY_SOURCE oneServiceSDO = (HIS_OTHER_PAY_SOURCE)servicePatieDatas[e.ListSourceRowIndex];
                            if (oneServiceSDO != null)
                            {
                                if (e.Column.FieldName == "HEIN_RATIO_DISPLAY")
                                {
                                    //e.Value = oneServiceSDO.HEIN_RATIO.HasValue ? (decimal?)Inventec.Common.Number.Convert.NumberToNumberRoundMax4((decimal)((oneServiceSDO.HEIN_RATIO ?? 0) * 100)) : null;
                                }
                            }
                            else
                            {
                                e.Value = null;
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

        private void cboTracking_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTracking.EditValue != null)
                {
                    this.tracking = this.trackings != null && this.trackings.Count > 0 ? this.trackings.FirstOrDefault(o => o.ID == (long)cboTracking.EditValue) : new V_HIS_TRACKING();
                }
                else
                {
                    this.tracking = new V_HIS_TRACKING();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTracking_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //cboTracking.Properties.Buttons[1].Visible = true;
                    cboTracking.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.treatmentId);

                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));

                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                        //Load lại tracking
                        InitComboTracking(this.treatmentId);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void chkAutoEat_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
                this.chkHalfInFirstDay.ReadOnly = this.intructionTimeSelecteds.Count == 1;
                this.chkHalfInFirstDay.Enabled = this.intructionTimeSelecteds.Count > 1;
                this.chkHalfInFirstDay.Checked = false;
                if (chkAutoEat.Checked)
                {
                    chkHalfInFirstDay.ReadOnly = false;
                    chkHalfInFirstDay.Enabled = true;
                    ucDateProcessor.EnableCheckBoxMultiIntructionTime(ucDate, false);
                }
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

        private void gridViewTreatmentBedRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int[] selectRows = gridViewTreatmentBedRoom.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0 && selectRows.Count() == 1)
                {
                    var data = (V_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetRow(selectRows[0]);
                    HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                    icd.ICD_CODE = data.ICD_CODE;
                    icd.ICD_NAME = data.ICD_NAME;
                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }
                    HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = data.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = data.ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
