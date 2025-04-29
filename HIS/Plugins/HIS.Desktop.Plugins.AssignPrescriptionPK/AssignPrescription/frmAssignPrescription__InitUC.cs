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
using ACS.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.UC.PatientSelect;
using HIS.UC.PeriousExpMestList;
using HIS.UC.TreatmentFinish;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void InitMultipleThread()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();

                var allowedModules = HisConfigCFG.MODULELINKS.Split(',');
                LogSystem.Debug("InitMultipleThread => 1");
                int heightUCTop = 0;
                int heightUCBottom = 0;

                this.isAutoCheckIcd = (HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue);
                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_TRADITIONAL != 1).OrderBy(o => o.ICD_CODE).ToList();
                this.currentTranditionalIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_TRADITIONAL == 1).OrderBy(o => o.ICD_CODE).ToList();
                VisibleLayoutSubIcd(HisConfigCFG.OptionSubIcdWhenFinish == "3");
                LogSystem.Debug("InitMultipleThread => 2");
                UCIcdInit();
                UCIcdCauseInit();
                UcDateInit();
                InitUcIcdYhct();
                InitUcSecondaryIcdYhct();
                LogSystem.Debug("InitMultipleThread => 3");
                if (HisConfigCFG.IsUsingServiceTime
                    && !GlobalStore.IsTreatmentIn
                    && !GlobalStore.IsExecutePTTT)
                {

                    this.pnlUCDate.Enabled = false;
                }

                InitControlState();
                LogSystem.Debug("InitMultipleThread => 4");
                if ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) || GlobalStore.IsExecutePTTT)
                {
                    this.InitUCPatientSelect();
                    LogSystem.Debug("InitMultipleThread => 5");
                    this.InitUCPeriousExpMestList();
                    LogSystem.Debug("InitMultipleThread => 6");
                    heightUCBottom = lciUCBottomPanel.Height + 20;
                    heightUCTop = lciUCTopPanel.Height - 20;
                    //heightUCTop = lciUCTopPanel.Height - 200;
                    //heightUCBottom = lciUCBottomPanel.Height + 200;
                    if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS) && moduleData != null && allowedModules.Contains(moduleData.ModuleLink))
                    {
                        layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        chkSignForDPK.Checked = false;
                        chkSignForDTT.Checked = false;
                        chkSignForDDT.Checked = false;
                    }
                    else
                    {
                        lciForchkSignForDPK.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciForchkSignForDTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciForchkSignForDDT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                }
                else
                {
                    this.InitUCPeriousExpMestList();
                    LogSystem.Debug("InitMultipleThread => 7");
                    this.InitUcTreatmentFinish();
                    LogSystem.Debug("InitMultipleThread => 8");
                    heightUCBottom = lciUCBottomPanel.Height + 20;
                    heightUCTop = lciUCTopPanel.Height - 20;
                    if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS) && moduleData != null && allowedModules.Contains(moduleData.ModuleLink))
                    {
                        layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        chkSignForDPK.Checked = false;
                        chkSignForDTT.Checked = false;
                        chkSignForDDT.Checked = false;
                    }
                    else
                    {
                        if (GlobalStore.IsCabinet)
                        {
                            lciForchkSignForDPK.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            lciForchkSignForDTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lciForchkSignForDDT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }
                        else
                        {
                            lciForchkSignForDPK.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lciForchkSignForDTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            lciForchkSignForDDT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }
                    }
                }
                //this.lciUCBottomPanel.MinSize = new System.Drawing.Size(this.lciUCBottomPanel.Width, heightUCBottom);//110
                //this.lciUCBottomPanel.Height = heightUCBottom;
                //this.lciUCTopPanel.Height = heightUCTop;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heightUCTop), heightUCTop) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heightUCBottom), heightUCBottom));
                LogSystem.Debug("InitMultipleThread => 9");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcTreatmentFinish()
        {
            try
            {
                this.treatmentFinishProcessor = new TreatmentFinishProcessor();
                HIS.UC.TreatmentFinish.ADO.TreatmentFinishInitADO ado = new HIS.UC.TreatmentFinish.ADO.TreatmentFinishInitADO();
                ado.DelegateNextFocus = NextForcusUCTreatmentFinish;
                ado.AutoTreatmentFinish__Checked = AutoTreatmentFinish__Checked;
                ado.DelegateTreatmentFinishCheckChange = TreatmentFinishCheckChanged;
                ado.DelegateGetDateADO = GetDateADO;
                ado.Height = 153;
                ado.Width = 275;
                ado.IsValidate = true;
                ado.NotAutoInitData = true;
                ado.UseCapSoBABNCT = true;
                ado.AutoFinishServiceIds = HisConfigCFG.autoFinishServiceIds;
                ado.IsCheckBedService = HisConfigCFG.isCheckBedService;
                ado.IsCheckFinishTime = HisConfigCFG.IsCheckFinishTime;
                ado.MustFinishAllServicesBeforeFinishTreatment = HisConfigCFG.mustFinishAllServicesBeforeFinishTreatment;
                ado.TreatmentEndAppointmentTimeDefault = HisConfigCFG.TreatmentEndAppointmentTimeDefault;
                ado.TreatmentEndHasAppointmentTimeDefault = HisConfigCFG.TreatmentEndHasAppointmentTimeDefault;
                ado.treatmentId = this.treatmentId;
                ado.WorkingRoomId = (this.currentModule != null ? this.currentModule.RoomId : 0);
                ado.WorkingDepartmentId = this.currentWorkPlace != null ? this.currentWorkPlace.DepartmentId : 0;
                var dataRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                ado.IsBlockOrder = dataRoom.IS_BLOCK_NUM_ORDER == 1 ? true : false;
                ado.LanguageInputADO = new UC.TreatmentFinish.ADO.LanguageInputADO();
                ado.LanguageInputADO.gBoxTreatmentFinishInfo__Text = Inventec.Common.Resource.Get.Value("gBoxTreatmentFinishInfo__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.lciAppointmentTime__Text = Inventec.Common.Resource.Get.Value("lciAppointmentTime__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.lciForchkAutoBK__Text = Inventec.Common.Resource.Get.Value("lciForchkAutoBK__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.lciForchkAutoPrintGHK__Text = Inventec.Common.Resource.Get.Value("lciForchkAutoPrintGHK__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.lciFordtEndTime__Text = Inventec.Common.Resource.Get.Value("lciFordtEndTime__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.lciTreatmentEndType__Text = Inventec.Common.Resource.Get.Value("lciTreatmentEndType__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.licPrintNHBHXH_Text = Inventec.Common.Resource.Get.Value("licPrintNHBHXH_Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.licPrintTrichLuc_Text = Inventec.Common.Resource.Get.Value("licPrintTrichLuc_Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.lciHisCareer__Text = Inventec.Common.Resource.Get.Value("lciHisCareer__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                ado.DelegateGetStoreStateValue = GetStateTratmentFinishCheck;
                ado.DelegateSetStoreStateValue = SetStateTratmentFinishCheck;

                this.ucTreatmentFinish = (UserControl)this.treatmentFinishProcessor.Run(ado);
                if (this.ucTreatmentFinish != null)
                {
                    this.pnlUCPanelRightBottom.Controls.Clear();
                    this.pnlUCPanelRightBottom.Controls.Add(this.ucTreatmentFinish);
                    this.ucTreatmentFinish.Dock = DockStyle.Fill;
                }
                TreatmentFinishCheckChanged(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetStateTratmentFinishCheck(string key)
        {
            string value = "";
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetStateTratmentFinishCheck.1");
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == key && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    value = csAddOrUpdate.VALUE;
                }
                Inventec.Common.Logging.LogSystem.Debug("GetStateTratmentFinishCheck.2" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }

        private bool SetStateTratmentFinishCheck(string key, string value)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetStateTratmentFinishCheck.0");
                Inventec.Common.Logging.LogSystem.Debug("SetStateTratmentFinishCheck.1");
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return true;
                }

                Inventec.Common.Logging.LogSystem.Debug("SetStateTratmentFinishCheck.2");
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == key && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = key;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                bool success = this.controlStateWorker.SetData(this.currentControlStateRDO);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
                Inventec.Common.Logging.LogSystem.Debug("SetStateTratmentFinishCheck.3");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
        }

        private void NextForcusUCTreatmentFinish()
        {
            try
            {
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCPatientSelect()
        {
            try
            {
                this.patientSelectProcessor = new PatientSelectProcessor();
                HIS.UC.PatientSelect.ADO.PatientSelectInitADO ado = new HIS.UC.PatientSelect.ADO.PatientSelectInitADO();
                ado.SelectedSingleRow = PatientSelectedChange;
                ado.CheckChangeSelectedPatientWhileHasPrescription = CheckChangeSelectedPatientWhileHasPrescription;
                ado.IsInitForm = true;
                ado.RoomId = GetRoomId();
                ado.TreatmentId = treatmentId;
                //ado.IsAutoWidth = false;
                ado.LanguageInputADO = new UC.PatientSelect.ADO.LanguageInputADO();
                ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColBedName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColBedName__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColDob__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColDob__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColPatientName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColPatientName__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColSTT__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColSTT__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColTreatmentCode__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColTreatmentCode__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColClassifyName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColClassifyName__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac = ResourceMessage.ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac;
                ado.LanguageInputADO.CanhBaoBenhNhanDaKeThuocTrongNgay = ResourceMessage.CanhBaoBenhNhanDaKeThuocTrongNgay;
                ado.LanguageInputADO.SearchNull_Text = Inventec.Common.Resource.Get.Value("SearchNullText__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.ucPatientSelect = (UserControl)this.patientSelectProcessor.Run(ado);
                if (this.ucPatientSelect != null)
                {
                    this.pnlUCPanelRightTop.Controls.Clear();
                    this.pnlUCPanelRightTop.Controls.Add(this.ucPatientSelect);
                    this.ucPatientSelect.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCPeriousExpMestList()
        {
            try
            {
                this.periousExpMestListProcessor = new PeriousExpMestListProcessor();
                HIS.UC.PeriousExpMestList.ADO.PeriousExpMestInitADO ado = new HIS.UC.PeriousExpMestList.ADO.PeriousExpMestInitADO();
                ado.btnSelected_Click = ProcessChoicePrescriptionPrevious;
                ado.btnView_Click = ViewPrescriptionPreviousButtonClick;
                ado.ActNotShowExpMestTypeDTTCheckedChanged = SaveStateNotShowExpMestTypeDTTCheckedChanged;
                ado.IsNotShowExpMestTypeDTT = this.chkNotShowExpMestTypeDTT;
                ado.IsAutoWidth = true;
                this.currentPrescriptionFilter = new MOS.Filter.HisServiceReqView7Filter();
                //this.currentPrescriptionFilter.TDL_PATIENT_ID = this.currentTreatmentWithPatientType.PATIENT_ID;
                this.currentPrescriptionFilter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs = new List<long>() { 
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN, 
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS, 
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL, 
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK, 
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM, 
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP, 
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC, 
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL, 
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC };

                this.currentPrescriptionFilter.PRESCRIPTION_TYPE_ID = 1;
                this.currentPrescriptionFilter.ORDER_DIRECTION = "DESC";
                this.currentPrescriptionFilter.ORDER_FIELD = "INTRUCTION_TIME";
                this.currentPrescriptionFilter.TDL_PATIENT_ID = VHistreatment.PATIENT_ID;
                ado.ServiceReqView7Filter = this.currentPrescriptionFilter;
                ado.LanguageInputADO = new UC.PeriousExpMestList.ADO.LanguageInputADO();
                ado.LanguageInputADO.btnSelectPrescriptionPrevious__ToolTip = Inventec.Common.Resource.Get.Value("btnSelectPrescriptionPrevious__ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.btnShowPrescriptionPrevious__ToolTip = Inventec.Common.Resource.Get.Value("btnShowPrescriptionPrevious__ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.gridControlPreviousprescription__gcolIntructionTime__Caption = Inventec.Common.Resource.Get.Value("gridControlPreviousprescription__gcolIntructionTime__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.gridControlPreviousprescription__gcolIntructionUser__Caption = Inventec.Common.Resource.Get.Value("gridControlPreviousprescription__gcolIntructionUser__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.lciPrePrescription__Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPriviousExpMest.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.BenhChinh__Text = Inventec.Common.Resource.Get.Value("UCPeriousExpMestList.BenhChinh__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.BenhPhu__Text = Inventec.Common.Resource.Get.Value("UCPeriousExpMestList.BenhPhu__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.lciCheckBox__Text = Inventec.Common.Resource.Get.Value("UCPeriousExpMestList.lciCheckBox__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.cboItem1_Text = Inventec.Common.Resource.Get.Value("UCPeriousExpMestList.cboItem1__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.cboItem2_Text = Inventec.Common.Resource.Get.Value("UCPeriousExpMestList.cboItem2__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.cboItem3_Text = Inventec.Common.Resource.Get.Value("UCPeriousExpMestList.cboItem3__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.cboItem4_Text = Inventec.Common.Resource.Get.Value("UCPeriousExpMestList.cboItem4__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.gridControlPreviousprescription__gcol4__Caption = Inventec.Common.Resource.Get.Value("UCPeriousExpMestList.gridControlPreviousprescription__gcol4__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.IsPresPK = (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet);

                this.ucPeriousExpMestList = (UserControl)periousExpMestListProcessor.Run(ado);
                if (this.ucPeriousExpMestList != null)
                {
                    if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)// 
                        this.pnlUCPanelRightBottom.Controls.Add(this.ucPeriousExpMestList);                      
                    else
                        this.pnlUCPanelRightTop.Controls.Add(this.ucPeriousExpMestList);
                    this.ucPeriousExpMestList.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long GetTreatmentTypeIdFromCode(string code)
        {
            long result = 0;
            try
            {
                HIS_TREATMENT_TYPE treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == code);
                if (treatmentType != null)
                    result = treatmentType.ID;
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                var icdValue = this.UcIcdGetValue() as UC.Icd.ADO.IcdInputADO;
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

        private async Task SetDefaultUC()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDefaultUC. 1");
                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                if (!HisConfigCFG.IsShowServerTimeByDefault)
                {
                    if (HisConfigCFG.IsUsingServiceTime)
                    {
                        //Lay gio server
                        TimerSDO timeSync = await new BackendAdapter(new CommonParam()).GetAsync<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam());
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeSync), timeSync));
                        if (timeSync != null)
                        {
                            dateInputADO.Time = timeSync.DateNow;
                            dateInputADO.Dates = new List<DateTime?>();
                            dateInputADO.Dates.Add(dateInputADO.Time);
                        }
                    }
                }
                else
                {
                    dateInputADO.Time = dteCommonParam;
                    dateInputADO.Dates = new List<DateTime?>();
                    dateInputADO.Dates.Add(dateInputADO.Time);
                }
               
               
                UcDateReload(dateInputADO);
                Inventec.Common.Logging.LogSystem.Debug("SetDefaultUC. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadUcTreatmentFinish()
        {
            try
            {
                if (this.treatmentFinishProcessor != null)
                    this.treatmentFinishProcessor.Reload(this.ucTreatmentFinish, this.GetDateADO());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
