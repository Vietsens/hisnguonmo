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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Save;
using HIS.Desktop.Plugins.Library.CheckIcd;
using HIS.Desktop.Utility;
using HIS.UC.DateEditor;
using HIS.UC.PatientSelect;
using HIS.UC.PeriousExpMestList;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using HIS.UC.TreatmentFinish;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void InitMultipleThread()
        {
            try
            {
                LogSystem.Debug("Construct => 1");
                int heightUCTop = 0;
                int heightUCBottom = 0;
                List<Action> methods = new List<Action>();
                InitUcIcd();
                InitUcIcdYHCT();
                InitUcCauseIcd();
                InitUcSecondaryIcd();
                InitUcSecondaryIcdYHCT();
                InitUcDate();
                //methods.Add(this.InitUcIcd);
                //methods.Add(this.InitUcCauseIcd);
                //methods.Add(this.InitUcSecondaryIcd);
                //methods.Add(this.InitUcDate);

                if ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) || GlobalStore.IsExecutePTTT)// 
                {
                    InitUCPatientSelect();
                    InitUCPeriousExpMestList();
                    heightUCTop = lciUCTopPanel.Height + 97;
                    heightUCBottom = lciUCBottomPanel.Height - 97;
                }
                else
                {
                    InitUCPeriousExpMestList();
                    InitUcTreatmentFinish();
                    heightUCTop = lciUCTopPanel.Height - 97;
                    heightUCBottom = lciUCBottomPanel.Height + 97;
                }

                //ThreadCustomManager.MultipleThreadWithJoin(methods);
                lciUCBottomPanel.Height = heightUCBottom;
                lciUCTopPanel.Height = heightUCTop;
                //popupControlContainerMediMaty.CloseOnLostFocus = false;
                //popupControlContainerMediMaty.CloseOnOuterMouseClick = false;                
                LogSystem.Debug("Construct => 2");
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
                ado.DelegateGetDateADO = GetDateADO;
                ado.Height = 153;
                ado.Width = 275;
                ado.IsValidate = true;
                ado.NotAutoInitData = true;
                ado.UseCapSoBABNCT = true;
                ado.AutoFinishServiceIds = HisConfigCFG.autoFinishServiceIds;
                ado.IsCheckBedService = HisConfigCFG.isCheckBedService;
                ado.IsCheckFinishTime = HisConfigCFG.IsCheckFinishTime;
                ado.treatmentId = this.treatmentId;
                ado.MustFinishAllServicesBeforeFinishTreatment = HisConfigCFG.mustFinishAllServicesBeforeFinishTreatment;
                ado.TreatmentEndAppointmentTimeDefault = HisConfigCFG.TreatmentEndAppointmentTimeDefault;
                ado.TreatmentEndHasAppointmentTimeDefault = HisConfigCFG.TreatmentEndHasAppointmentTimeDefault;
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

                this.ucTreatmentFinish = (UserControl)this.treatmentFinishProcessor.Run(ado);
                if (this.ucTreatmentFinish != null)
                {
                    this.pnlUCPanelRightBottom.Controls.Add(this.ucTreatmentFinish);
                    this.ucTreatmentFinish.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                ado.LanguageInputADO.ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac = ResourceMessage.ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac;
                ado.LanguageInputADO.CanhBaoBenhNhanDaKeThuocTrongNgay = ResourceMessage.CanhBaoBenhNhanDaKeThuocTrongNgay;
                this.ucPatientSelect = (UserControl)this.patientSelectProcessor.Run(ado);
                if (this.ucPatientSelect != null)
                {
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

                this.currentPrescriptionFilter.PRESCRIPTION_TYPE_ID = 2;
                this.currentPrescriptionFilter.ORDER_DIRECTION = "DESC";
                this.currentPrescriptionFilter.ORDER_FIELD = "INTRUCTION_TIME";
                this.currentPrescriptionFilter.TDL_PATIENT_ID = Histreatment.PATIENT_ID;
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

        private void InitUcDate()
        {
            try
            {
                this.ucDateProcessor = new UCDateProcessor();
                HIS.UC.DateEditor.ADO.DateInitADO ado = new HIS.UC.DateEditor.ADO.DateInitADO();
                ado.DelegateNextFocus = NextForcusUCDate;
                ado.DelegateChangeIntructionTime = ChangeIntructionTime;
                ado.DelegateSelectMultiDate = DelegateSelectMultiDate;
                ado.DelegateMultiDateChanged = DelegateMultiDateChanged;
                ado.Height = 24;
                ado.Width = 364;
                ado.IsVisibleMultiDate = (GlobalStore.IsTreatmentIn && (this.actionType != GlobalVariables.ActionEdit)
                    && !GlobalStore.IsCabinet );
                ado.IsValidate = true;
                ado.LanguageInputADO = new UC.DateEditor.ADO.LanguageInputADO();
                ado.LanguageInputADO.TruongDuLieuBatBuoc = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                ado.LanguageInputADO.UCDate__CaptionlciDateEditor = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTimeAssign.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.UCDate__CaptionchkMultiIntructionTime = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkMultiIntructionTime.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.ChuaChonNgayChiDinh = ResourceMessage.ChuaChonNgayChiDinh;
                ado.LanguageInputADO.FormMultiChooseDate__CaptionCalendaInput = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionCalendaInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.FormMultiChooseDate__CaptionText = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.FormMultiChooseDate__CaptionTimeInput = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionTimeInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.FormMultiChooseDate__CaptionBtnChoose = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionBtnChoose", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.ucDate = (UserControl)this.ucDateProcessor.Run(ado);

                if (this.ucDate != null)
                {
                    this.pnlUCDate.Controls.Add(this.ucDate);
                    this.ucDate.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegateMultiDateChanged()
        {
            this.InitComboTracking(cboPhieuDieuTri);
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

        private void NextForcusUCDate()
        {
            try
            {
                cboMediStockExport.Focus();
                cboMediStockExport.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcCauseIcd()
        {
            try
            {
                this.icdCauseProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcdCause;
                ado.Width = 330;
                ado.Height = 24;
                ado.IsColor = false;
                ado.IsUCCause = true;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>()
                    .Where(p => p.IS_CAUSE == 1 && p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue;
                ado.LblIcdMain = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdCause.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.ToolTipsIcdMain = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdCause.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.ucIcdCause = (UserControl)this.icdCauseProcessor.Run(ado);

                if (this.ucIcdCause != null)
                {
                    this.panelControlCauseIcd.Controls.Add(this.ucIcdCause);
                    this.ucIcdCause.Dock = DockStyle.Fill;
                }
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
                this.icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.DelegateRequiredCause = DelegateRequiredCause;
                ado.DelegateRefreshSubIcd = LoadSubIcd;
                //ado.delegateCheckICD = CheckICDCode;
                ado.Width = 330;
                ado.Height = 24;
                ado.hisTreatment = this.Histreatment;
                ado.IsColor = (HisConfigCFG.ObligateIcd == GlobalVariables.CommonStringTrue);
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_TRADITIONAL != 1).OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue;
                this.ucIcd = (UserControl)this.icdProcessor.Run(ado);

                if (this.ucIcd != null)
                {
                    this.panelControlIcd.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadSubIcd(string icdCodes, string icdNames)
        {
            try
            {
                SecondaryIcdDataADO data = new SecondaryIcdDataADO();
                data.ICD_SUB_CODE = icdCodes;
                data.ICD_TEXT = icdNames;
                if (this.subIcdProcessor != null && this.ucSecondaryIcd != null)
                {
                    this.subIcdProcessor.SetAttachIcd(this.ucSecondaryIcd, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadSubIcdYHCT(string icdCodes, string icdNames)
        {
            try
            {
                SecondaryIcdDataADO data = new SecondaryIcdDataADO();
                data.ICD_SUB_CODE = icdCodes;
                data.ICD_TEXT = icdNames;
                if (this.subIcdProcessorYHCT != null && this.ucSecondaryIcdYHCT != null)
                {
                    this.subIcdProcessorYHCT.SetAttachIcd(this.ucSecondaryIcdYHCT, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private void InitUcIcdYHCT()
        {
            try
            {
                this.icdProcessorYHCT = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                //ado.DelegateRequiredCause = DelegateRequiredCause;
                ado.DelegateRefreshSubIcd = LoadSubIcdYHCT;
                ado.LblIcdMain = "CĐ YHCT:";
                ado.ToolTipsIcdMain = "Chẩn đoán y học cổ truyền";
                ado.Width = 220;
                ado.Height = 30;
                ado.hisTreatment = this.Histreatment;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_TRADITIONAL == 1).OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue;
                this.ucIcdYHCT = (UserControl)this.icdProcessorYHCT.Run(ado);

                if (this.ucIcdYHCT != null)
                {
                    this.panelControlIcdYHCT.Controls.Add(this.ucIcdYHCT);
                    this.ucIcdYHCT.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void DelegateRequiredCause(bool isRequired)
        {
            try
            {
                if (this.icdCauseProcessor != null && this.ucIcdCause != null)
                {
                    this.icdCauseProcessor.SetRequired(this.ucIcdCause, isRequired);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                if (icdCauseProcessor != null && ucIcdCause != null)
                {
                    icdCauseProcessor.FocusControl(ucIcdCause);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusSubIcdCause()
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

        private void InitUcSecondaryIcd()
        {
            try
            {
                var dataICD = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_TRADITIONAL != 1).OrderBy(o => o.ICD_CODE).ToList();
                this.subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), dataICD);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                //ado.delegateCheckICD = CheckICDSecond;
                ado.Width = 660;
                ado.Height = 24;
                ado.hisTreatment = this.Histreatment;
                ado.TextLblIcd = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdText.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.TextNullValue = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtIcdExtraNames.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                this.ucSecondaryIcd = (UserControl)this.subIcdProcessor.Run(ado);

                if (this.ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(this.ucSecondaryIcd);
                    this.ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
       
        private void InitUcSecondaryIcdYHCT()
        {
            try
            {
                var dataICD = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_TRADITIONAL == 1).OrderBy(o => o.ICD_CODE).ToList();
                this.subIcdProcessorYHCT = new SecondaryIcdProcessor(new CommonParam(), dataICD);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.DelegateGetIcdMain = GetIcdMainCodeYHCT;
                ado.Width = 440;
                ado.Height = 30;
                ado.TextLblIcd = "CĐ YHCT Phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán y học cổ truyền phụ";
                ado.hisTreatment = this.Histreatment;
                //  ado.TextLblIcd = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdText.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.TextNullValue = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtIcdExtraNames.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                this.ucSecondaryIcdYHCT = (UserControl)this.subIcdProcessorYHCT.Run(ado);

                if (this.ucSecondaryIcdYHCT != null)
                {
                    this.panelControlUcSubIcdYHCT.Controls.Add(this.ucSecondaryIcdYHCT);
                    this.ucSecondaryIcdYHCT.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //viec 178289 thinhdt1
        private bool CheckICD(string icd_code,string icd_sub_code,ref string mess,ref string error_code)
        {
            bool valid = false;
            
            try
            {
                HIS.Desktop.Plugins.Library.CheckIcd.CheckIcdManager mana = new CheckIcdManager(DlgIcdSubCode, this.Histreatment);
                string messError = "";
                if (!mana.ProcessCheckIcd(icd_code, icd_sub_code, ref messError,false,true))
                {

                    LogSystem.Debug("icd: " + icd_code + " va icd_phu: " + icd_sub_code + "co trung nhom ICD10");
                    valid = true;
                }
                mess += messError;
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
        private void DlgIcdSubCode(string icdCodes, string icdNames)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("DlgIcdSubCode.1");
                
                Inventec.Common.Logging.LogSystem.Debug("DlgIcdSubCode.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.icdProcessor != null && this.ucIcd != null)
                {
                    var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }
        private string GetIcdMainCodeYHCT()
        {
            string mainCode = "";
            try
            {
                if (this.icdProcessorYHCT != null && this.ucIcdYHCT != null)
                {
                    var icdValue = this.icdProcessorYHCT.GetValue(this.ucIcdYHCT);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private void NextForcusOut()
        {
            try
            {
                txtMediMatyForPrescription.Focus();
                txtMediMatyForPrescription.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultUC()
        {
            try
            {
                DateTime now = DateTime.Now;
                if (ContructorIntructionTime > 0)
                {
                    now = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ContructorIntructionTime) ?? DateTime.Now;
                }

                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();

                dateInputADO.Time = now;
                dateInputADO.Dates = new List<DateTime?>();
                dateInputADO.Dates.Add(now);

                this.ucDateProcessor.Reload(this.ucDate, dateInputADO);
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                this.isMultiDateState = false;

                this.LoadDataToPatientInfo();

                if (this.treatmentFinishProcessor != null && this.ucTreatmentFinish != null)
                    this.treatmentFinishProcessor.Reload(this.ucTreatmentFinish, this.GetDateADO());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
