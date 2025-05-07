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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Validate.ValidateRule;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.Desktop.Utility.ValidateRule;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using StringUtil = HIS.Desktop.Utility.StringUtil;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        List<MediMatyTypeADO> mediMatyTypeADOAdds = new List<MediMatyTypeADO>();
        public void ReloadModuleByInputData(HIS.Desktop.ADO.AssignPrescriptionADO data, MOS.Filter.HisTreatmentBedRoomLViewFilter treatmentBedRoomLViewFilter, Inventec.Desktop.Common.Modules.Module module)
		{
			try
            {
                LogSystem.Debug("ReloadModuleByInputData Start___"+ IsUseApplyFormClosingOption);
                this.IsUseApplyFormClosingOption = true;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                pnlUCPanelRightTop.Controls.Clear();
                pnlUCPanelRightBottom.Controls.Clear();
                layoutControlPrintAssignPrescription.Clear();
                this.layoutControlPrintAssignPrescription.Root.Clear();
                layoutControlPrintAssignPrescriptionExt.Clear();
                this.layoutControlPrintAssignPrescriptionExt.Root.Clear();
                GridCheckMarksSelection gridCheckMark = cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboMediStockExport.Properties.View);
                }
                rdOpionGroup.SelectedIndex = 0;
                this.txtMediMatyForPrescription.Text = "";
                gridViewMediMaty.ActiveFilter.Clear();
                this.treatmentBedRoomLViewFilterInput = treatmentBedRoomLViewFilter;
                this.actionType = data.AssignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd;
                this.actionTypePrint = data.AssignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd;
                this.currentModule = module;
                ReloadRoomRam();
                InitServiceConditionData();
                this.processDataResult = data.DgProcessDataResult;
                this.processRefeshIcd = data.DgProcessRefeshIcd;
                this.processWhileAutoTreatmentEnd = data.DlgWhileAutoTreatmentEnd;
                this.treatmentId = data.TreatmentId;
                this.expMestTemplateId = data.ExpMestTemplateId;
                this.treatmentCode = data.TreatmentCode;
                if (data.ServiceReqId > 0)
                    this.serviceReqParentId = data.ServiceReqId;
                this.isInKip = data.IsInKip;
                GlobalStore.IsCabinet = data.IsCabinet;
                GlobalStore.IsTreatmentIn = data.IsExecutePTTT;
                GlobalStore.IsExecutePTTT = data.IsExecutePTTT;
                this.patientName = data.PatientName;
                this.patientDob = data.PatientDob;
                this.genderName = data.GenderName;

                if (data.Tracking != null)
                {
                    this.Listtrackings = new List<HIS_TRACKING>();
                    this.Listtrackings.Add(data.Tracking);
                }

                if (this.isInKip)
                    this.currentSereServInEkip = data.SereServ;
                else
                    this.currentSereServ = data.SereServ;
                this.isAutoCheckExpend = data.IsAutoCheckExpend;
                this.assignPrescriptionEditADO = data.AssignPrescriptionEditADO;
                this.icdExam = data.IcdExam;
                this.currentDhst = data.Dhst;
                this.sereServsInTreatmentRaw = data.SereServsInTreatment;
                this.provisionalDiagnosis = data.ProvisionalDiagnosis;
                this.ContructorIntructionTime = data.IntructionTime;
                this.InitAssignPresctiptionType();
                this.InitDataForPrescriptionEdit();
                HisConfigCFG.LoadConfig();
                ReloadInitUC();
                InitControlState();
                ReloadValidate();
                SetDataText();
                if (!GlobalStore.IsCabinet && (HisConfigCFG.icdServiceHasCheck == 1 || HisConfigCFG.icdServiceHasCheck == 2))
                {
                    this.lciPDDT.Enabled = true;
                }
                else
                {
                    this.lciPDDT.Enabled = false;
                }
                LogSystem.Debug("ReloadModuleByInputData InitializeComponent.2");


                LogSystem.Debug("ReloadModuleByInputData Starting.... 1");
                WaitingManager.Show();
                this.LoadVHisTreatment();

                this.isNotLoadWhileChangeInstructionTimeInFirst = true;
                this.isInitTracking = true;
                this.gridControlServiceProcess.ToolTipController = this.tooltipService;
                this.ResetDataForm();
                ReloadDataDefault();
                this.SetDefaultUC();
                LogSystem.Debug("ReloadModuleByInputData 1...");
                this.LoadDataToPatientInfo();
                LogSystem.Debug("ReloadModuleByInputData 1.1");
                this.isNotLoadMediMatyByMediStockInitForm = true;
                this.ReSetDataInputAfterAdd__MedicinePage();
                LogSystem.Debug("ReloadModuleByInputData. 2");


                    Inventec.Common.Logging.LogSystem.Debug("ReloadModuleByInputData .DEBUG true");
                    this.ValidateForm();
                    this.ValidateBosung();
                    this.VisibleColumnInGridControlService();
            
                LogSystem.Debug("ReloadModuleByInputData. 4");
                this.FillDataToControlsFormRebuild();
                LogSystem.Debug("ReloadModuleByInputData. 5");
                this.VisibleButton(this.actionBosung);
                this.LoadDefaultTabpageMedicine();
                this.InitDataByServicePackage();
                this.InitDataByServiceMetyMaty();
                this.InitDataByExpMestTemplate();
                LogSystem.Debug("ReloadModuleByInputData. 6");
                this.LoadPrescriptionForEdit();
                this.SetEnableButtonControl(this.actionType);
                this.LoadDataDhstToControl();
                this.isNotLoadMediMatyByMediStockInitForm = false;
                this.IsHandlerWhileOpionGroupSelectedIndexChanged = false;
                this.isNotLoadWhileChangeInstructionTimeInFirst = false;

                this.InitMenuToButtonPrint();
                LogSystem.Debug("ReloadModuleByInputData. 7");

                WaitingManager.Hide();
                this.InitDefaultFocus();

                this.timerInitForm.Interval = 2000;//Fix 5s
                this.timerInitForm.Enabled = true;
                this.timerInitForm.Start();

                LogSystem.Debug("ReloadModuleByInputData. 8");

                VisibleColumnPreAmount();
                this.LoadSubPrescription();
                LogSystem.Debug("ReloadModuleByInputData. 9");
                watch.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), this.ModuleLink, "OpenModule", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}       

        private void ReloadDataDefault()
		{
			try
			{
                //Load so ngay theo cau hinh tai khoan ke don phong kham
                if (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && !GlobalStore.IsExecutePTTT)
                {
                    int numOfDay = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__DEFAULT_NUM_OF_DAY);
                    if (numOfDay > 0)
                    {
                        this.spinSoNgay.Value = numOfDay;
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultData => kiem tra co cau hình CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__DEFAULT_NUM_OF_DAY= " + numOfDay + ", lay gan gia tri vao spinSoNgay ");
                    }
                }

                this.btnCreateVBA.Enabled = false;

                this.lblPhatSinh.Text = "";
                this.lblPhatSinh__BHYT.Text = "";
                this.lblPhatSinh__KhacBHYT.Text = "";
                this.lblPhatSinh__MuaNgoai.Text = "";
                this.idRow = 1;

                this.btnSave.Enabled = false;
                this.btnSaveAndPrint.Enabled = false;
                this.lciPrintAssignPrescription.Enabled = false;
                this.btnAdd.Enabled = false;
                this.btnAddTutorial.Enabled = false;

                this.txtLadder.Text = "";
                this.txtAdvise.Text = "";

                this.spinSoNgay.Enabled = true;
                this.spinTocDoTruyen.EditValue = null;
                this.chkHomePres.Checked = false;
                this.chkTemporayPres.Checked = false;
                this.spnPresPhaseNum.EditValue = null;
                this.txtProvisionalDiagnosis.Text = this.provisionalDiagnosis;
                this.txtPreviousUseDay.Text = "";

                this.actionType = (this.assignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd);
                this.actionBosung = GlobalVariables.ActionAdd;

                this.gridControlServiceProcess.DataSource = null;
                this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                this.cboExpMestTemplate.EditValue = null;
                this.cboExpMestTemplate.Properties.Buttons[1].Visible = false;
                this.txtExpMestTemplateCode.Text = "";
                this.spinKidneyCount.EditValue = null;
                this.chkPreKidneyShift.Checked = false;

                this.cboPhieuDieuTri.EditValue = null;

                txtInteractionReason.Enabled = false;

                this.ResetComboNhaThuoc();

                GlobalStore.ClientSessionKey = Guid.NewGuid().ToString();

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl, dxErrorProvider1);

                this.serviceReqMetyInDay = null;
                this.serviceReqMatyInDay = null;

                this.resultDataPrescription = null;

                //Ẩn số thang theo cấu hình
                long prescriptionIsVisiableRemedyCount = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__ISVISIBLE_REMEDY_COUNT);
                if (prescriptionIsVisiableRemedyCount == 1)
                {
                    lciLadder.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                this.layoutControlPrintAssignPrescriptionExt.Root.Clear();
                this.lciPrintAssignPrescriptionExt.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                this.lciPrintAssignPrescriptionExt.MinSize = new System.Drawing.Size(2, 1);
                this.lciPrintAssignPrescriptionExt.MaxSize = new System.Drawing.Size(2, 40);
                this.lciPrintAssignPrescriptionExt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lciPrintAssignPrescription.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                this.cboPatientType.EditValue = null;
                this.cboPatientType.Properties.DataSource = null;

                this.lblChiPhiBNPhaiTra.Text = "";
                this.lblDaDong.Text = "";
                this.lblConThua.Text = "";
                this.lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForlblConThua.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciForlblConThua.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                this.lciExpMestReason.Visibility = actionType == GlobalVariables.ActionEdit ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private async Task ReloadRoomRam()
		{
			try
			{
                this.requestRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private async Task ReloadInitUC()
        {
            try
            {

                Action myaction = () => {
                    this.isAutoCheckIcd = (HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue);
                    this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_TRADITIONAL != 1).OrderBy(o => o.ICD_CODE).ToList();

                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                VisibleLayoutSubIcd(HisConfigCFG.OptionSubIcdWhenFinish == "3");
                UCIcdInit();
                UCIcdCauseInit();
                UcDateInit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task ReloadValidate()
        {
            try
            {
                int heightUCTop = 0;
                int heightUCBottom = 0;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();

                var allowedModules = HisConfigCFG.MODULELINKS.Split(',');

                if ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) || GlobalStore.IsExecutePTTT)
                {
                    this.InitUCPatientSelect();
                    this.InitUCPeriousExpMestList();
                    heightUCBottom = lciUCBottomPanel.Height + 20;
                    heightUCTop = lciUCTopPanel.Height - 20;

                    if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS) && moduleData != null && allowedModules.Contains(moduleData.ModuleLink))
                    {
                        layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else
                    {
                        layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciForchkSignForDPK.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciForchkSignForDTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciForchkSignForDDT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                }
                else
                {
                    this.InitUCPeriousExpMestList();
                    this.InitUcTreatmentFinish();
                    heightUCBottom = lciUCBottomPanel.Height + 20;
                    heightUCTop = lciUCTopPanel.Height - 20;

                    if (GlobalStore.IsCabinet)
                    {
                        if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS) && moduleData != null && allowedModules.Contains(moduleData.ModuleLink))
                        {
                            layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }
                        else
                        {
                            layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lciForchkSignForDPK.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            lciForchkSignForDTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lciForchkSignForDDT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS) && moduleData != null && allowedModules.Contains(moduleData.ModuleLink))
                        {
                            layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }
                        else
                        {
                            layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lciForchkSignForDPK.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            lciForchkSignForDTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lciForchkSignForDDT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }
                    }
                }
                this.lciUCBottomPanel.Height = heightUCBottom;
                this.lciUCTopPanel.Height = heightUCTop;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
