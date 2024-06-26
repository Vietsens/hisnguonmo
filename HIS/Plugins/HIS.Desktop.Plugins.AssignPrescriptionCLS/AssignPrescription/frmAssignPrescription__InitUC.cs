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
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
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
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void InitMultipleThread()
        {
            try
            {
                LogSystem.Debug("InitMultipleThread => 1");
                int heightUCTop = 0;
                int heightUCBottom = 0;

                this.isAutoCheckIcd = (HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue);
                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();

                UCIcdInit();
                //UCIcdCauseInit();
                UcDateInit();

                if (HisConfigCFG.IsUsingServiceTime
                    && !GlobalStore.IsTreatmentIn
                    && !GlobalStore.IsExecutePTTT)
                {

                    this.pnlUCDate.Enabled = false;
                }
                //this.InitUcIcd();
                //this.InitUcCauseIcd();
                //this.InitUcSecondaryIcd();
                //this.InitUcDate();

                //if ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) || GlobalStore.IsExecutePTTT)
                //{
                //    this.InitUCPatientSelect();
                //    this.InitUCPeriousExpMestList();
                //    heightUCTop = lciUCTopPanel.Height + 97;
                //    heightUCBottom = lciUCBottomPanel.Height - 97;
                //}
                //else
                //{
                //    this.InitUCPeriousExpMestList();
                //    this.InitUcTreatmentFinish();
                //    heightUCTop = lciUCTopPanel.Height - 97;
                //    heightUCBottom = lciUCBottomPanel.Height + 97;
                //}

                //lciUCBottomPanel.Height = heightUCBottom;
                //lciUCTopPanel.Height = heightUCTop;

                LogSystem.Debug("InitMultipleThread => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void InitUcTreatmentFinish()
        //{
        //    try
        //    {
        //        this.treatmentFinishProcessor = new TreatmentFinishProcessor();
        //        HIS.UC.TreatmentFinish.ADO.TreatmentFinishInitADO ado = new HIS.UC.TreatmentFinish.ADO.TreatmentFinishInitADO();
        //        ado.DelegateNextFocus = NextForcusUCTreatmentFinish;
        //        ado.AutoTreatmentFinish__Checked = AutoTreatmentFinish__Checked;
        //        ado.DelegateGetDateADO = GetDateADO;
        //        ado.Height = 153;
        //        ado.Width = 275;
        //        ado.IsValidate = true;
        //        ado.NotAutoInitData = true;
        //        ado.AutoFinishServiceIds = HisConfigCFG.autoFinishServiceIds;
        //        ado.IsCheckBedService = HisConfigCFG.isCheckBedService;
        //        ado.IsCheckFinishTime = HisConfigCFG.IsCheckFinishTime;
        //        ado.MustFinishAllServicesBeforeFinishTreatment = HisConfigCFG.mustFinishAllServicesBeforeFinishTreatment;
        //        ado.TreatmentEndAppointmentTimeDefault = HisConfigCFG.TreatmentEndAppointmentTimeDefault;
        //        ado.TreatmentEndHasAppointmentTimeDefault = HisConfigCFG.TreatmentEndHasAppointmentTimeDefault;
        //        ado.treatmentId = this.treatmentId;

        //        ado.LanguageInputADO = new UC.TreatmentFinish.ADO.LanguageInputADO();
        //        ado.LanguageInputADO.gBoxTreatmentFinishInfo__Text = Inventec.Common.Resource.Get.Value("gBoxTreatmentFinishInfo__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.lciAppointmentTime__Text = Inventec.Common.Resource.Get.Value("lciAppointmentTime__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.lciForchkAutoBK__Text = Inventec.Common.Resource.Get.Value("lciForchkAutoBK__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.lciForchkAutoPrintGHK__Text = Inventec.Common.Resource.Get.Value("lciForchkAutoPrintGHK__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.lciFordtEndTime__Text = Inventec.Common.Resource.Get.Value("lciFordtEndTime__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.lciTreatmentEndType__Text = Inventec.Common.Resource.Get.Value("lciTreatmentEndType__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());


        //        this.ucTreatmentFinish = (UserControl)this.treatmentFinishProcessor.Run(ado);
        //        if (this.ucTreatmentFinish != null)
        //        {
        //            this.pnlUCPanelRightBottom.Controls.Add(this.ucTreatmentFinish);
        //            this.ucTreatmentFinish.Dock = DockStyle.Fill;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void NextForcusUCTreatmentFinish()
        //{
        //    try
        //    {
        //        btnSave.Focus();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void InitUCPatientSelect()
        //{
        //    try
        //    {
        //        this.patientSelectProcessor = new PatientSelectProcessor();
        //        HIS.UC.PatientSelect.ADO.PatientSelectInitADO ado = new HIS.UC.PatientSelect.ADO.PatientSelectInitADO();
        //        ado.SelectedSingleRow = PatientSelectedChange;
        //        ado.CheckChangeSelectedPatientWhileHasPrescription = CheckChangeSelectedPatientWhileHasPrescription;
        //        ado.IsInitForm = true;
        //        ado.RoomId = GetRoomId();
        //        ado.TreatmentId = treatmentId;
        //        //ado.IsAutoWidth = false;
        //        ado.LanguageInputADO = new UC.PatientSelect.ADO.LanguageInputADO();
        //        ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColBedName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColBedName__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColDob__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColDob__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColPatientName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColPatientName__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColSTT__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColSTT__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColTreatmentCode__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColTreatmentCode__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac = ResourceMessage.ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac;
        //        ado.LanguageInputADO.CanhBaoBenhNhanDaKeThuocTrongNgay = ResourceMessage.CanhBaoBenhNhanDaKeThuocTrongNgay;
        //        this.ucPatientSelect = (UserControl)this.patientSelectProcessor.Run(ado);
        //        if (this.ucPatientSelect != null)
        //        {
        //            this.pnlUCPanelRightTop.Controls.Add(this.ucPatientSelect);
        //            this.ucPatientSelect.Dock = DockStyle.Fill;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void InitUCPeriousExpMestList()
        //{
        //    try
        //    {
        //        this.periousExpMestListProcessor = new PeriousExpMestListProcessor();
        //        HIS.UC.PeriousExpMestList.ADO.PeriousExpMestInitADO ado = new HIS.UC.PeriousExpMestList.ADO.PeriousExpMestInitADO();
        //        ado.btnSelected_Click = ProcessChoicePrescriptionPrevious;
        //        ado.btnView_Click = ViewPrescriptionPreviousButtonClick;
        //        ado.IsAutoWidth = true;
        //        this.currentPrescriptionFilter = new MOS.Filter.HisServiceReqView7Filter();
        //        //this.currentPrescriptionFilter.TDL_PATIENT_ID = this.currentTreatmentWithPatientType.PATIENT_ID;
        //        this.currentPrescriptionFilter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs = new List<long>() { 
        //                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN, 
        //                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS, 
        //                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL, 
        //                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK, 
        //                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM, 
        //                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP, 
        //                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC, 
        //                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL, 
        //                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC };

        //        this.currentPrescriptionFilter.PRESCRIPTION_TYPE_ID = 1;
        //        this.currentPrescriptionFilter.ORDER_DIRECTION = "DESC";
        //        this.currentPrescriptionFilter.ORDER_FIELD = "INTRUCTION_TIME";

        //        ado.ServiceReqView7Filter = this.currentPrescriptionFilter;
        //        ado.LanguageInputADO = new UC.PeriousExpMestList.ADO.LanguageInputADO();
        //        ado.LanguageInputADO.btnSelectPrescriptionPrevious__ToolTip = Inventec.Common.Resource.Get.Value("btnSelectPrescriptionPrevious__ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.btnShowPrescriptionPrevious__ToolTip = Inventec.Common.Resource.Get.Value("btnShowPrescriptionPrevious__ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.gridControlPreviousprescription__gcolIntructionTime__Caption = Inventec.Common.Resource.Get.Value("gridControlPreviousprescription__gcolIntructionTime__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.gridControlPreviousprescription__gcolIntructionUser__Caption = Inventec.Common.Resource.Get.Value("gridControlPreviousprescription__gcolIntructionUser__Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.lciPrePrescription__Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPriviousExpMest.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.BenhChinh__Text = Inventec.Common.Resource.Get.Value("UCPeriousExpMestList.BenhChinh__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.BenhPhu__Text = Inventec.Common.Resource.Get.Value("UCPeriousExpMestList.BenhPhu__Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        this.ucPeriousExpMestList = (UserControl)periousExpMestListProcessor.Run(ado);
        //        if (this.ucPeriousExpMestList != null)
        //        {
        //            if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)// 
        //                this.pnlUCPanelRightBottom.Controls.Add(this.ucPeriousExpMestList);
        //            else
        //                this.pnlUCPanelRightTop.Controls.Add(this.ucPeriousExpMestList);
        //            this.ucPeriousExpMestList.Dock = DockStyle.Fill;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

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

        //private void InitUcDate()
        //{
        //    try
        //    {
        //        this.ucDateProcessor = new UCDateProcessor();
        //        HIS.UC.DateEditor.ADO.DateInitADO ado = new HIS.UC.DateEditor.ADO.DateInitADO();
        //        ado.DelegateNextFocus = NextForcusUCDate;
        //        ado.DelegateChangeIntructionTime = ChangeIntructionTime;
        //        ado.DelegateSelectMultiDate = DelegateSelectMultiDate;
        //        ado.DelegateMultiDateChanged = DelegateMultiDateChanged;
        //        ado.IsManyDayPrescriptionOption = (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet));
        //        ado.Height = 24;
        //        ado.Width = 364;
        //        ado.IsVisibleMultiDate = (GlobalStore.IsTreatmentIn && (this.actionType != GlobalVariables.ActionEdit)
        //            && !GlobalStore.IsCabinet
        //            );
        //        ado.IsValidate = true;
        //        ado.LanguageInputADO = new UC.DateEditor.ADO.LanguageInputADO();
        //        ado.LanguageInputADO.TruongDuLieuBatBuoc = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
        //        ado.LanguageInputADO.UCDate__CaptionlciDateEditor = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTimeAssign.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.UCDate__CaptionchkMultiIntructionTime = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkMultiIntructionTime.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.ChuaChonNgayChiDinh = ResourceMessage.ChuaChonNgayChiDinh;
        //        ado.LanguageInputADO.FormMultiChooseDate__CaptionCalendaInput = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionCalendaInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.FormMultiChooseDate__CaptionText = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.FormMultiChooseDate__CaptionTimeInput = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionTimeInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.LanguageInputADO.FormMultiChooseDate__CaptionBtnChoose = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionBtnChoose", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());


        //        this.ucDate = (UserControl)this.ucDateProcessor.Run(ado);

        //        if (this.ucDate != null)
        //        {
        //            this.pnlUCDate.Controls.Add(this.ucDate);
        //            this.ucDate.Dock = DockStyle.Fill;
        //        }

        //        if (HisConfigCFG.IsUsingServiceTime
        //            && !GlobalStore.IsTreatmentIn
        //            && !GlobalStore.IsExecutePTTT)
        //        {

        //            this.pnlUCDate.Enabled = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void InitUcDateForMedi()
        //{
        //    try
        //    {
        //        if (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
        //        {
        //            this.ucDateForMediProcessor = new UCDateProcessor();
        //            HIS.UC.DateEditor.ADO.DateInitADO adoForMedi = new HIS.UC.DateEditor.ADO.DateInitADO();
        //            adoForMedi.DelegateNextFocus = NextForcusUCDateForMedi;
        //            adoForMedi.DelegateChangeIntructionTime = ChangeIntructionTimeForMedi;
        //            adoForMedi.DelegateSelectMultiDate = DelegateSelectMultiDateForMedi;
        //            adoForMedi.DelegateMultiDateChanged = DelegateMultiDateForMediChanged;
        //            adoForMedi.Height = 24;
        //            adoForMedi.Width = 364;
        //            adoForMedi.IsValidate = true;
        //            adoForMedi.IsVisibleMultiDate = true;
        //            adoForMedi.IsHideCheck = true;
        //            adoForMedi.IsHideTimeEdit = true;
        //            adoForMedi.LanguageInputADO = new UC.DateEditor.ADO.LanguageInputADO();
        //            adoForMedi.LanguageInputADO.TruongDuLieuBatBuoc = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
        //            adoForMedi.LanguageInputADO.UCDate__CaptionlciDateEditor = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTimeAssign.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //            adoForMedi.LanguageInputADO.UCDate__CaptionchkMultiIntructionTime = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkMultiIntructionTime.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //            adoForMedi.LanguageInputADO.ChuaChonNgayChiDinh = ResourceMessage.ChuaChonNgayChiDinh;
        //            adoForMedi.LanguageInputADO.FormMultiChooseDate__CaptionCalendaInput = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionCalendaInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //            adoForMedi.LanguageInputADO.FormMultiChooseDate__CaptionText = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //            adoForMedi.LanguageInputADO.FormMultiChooseDate__CaptionTimeInput = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionTimeInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //            adoForMedi.LanguageInputADO.FormMultiChooseDate__CaptionBtnChoose = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionBtnChoose", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

        //            this.ucDateForMedi = (UserControl)this.ucDateForMediProcessor.Run(adoForMedi);
        //            if (this.ucDateForMedi != null)
        //            {
        //                this.pnlUCDateForMedi.Controls.Add(this.ucDateForMedi);
        //                this.ucDateForMedi.Dock = DockStyle.Fill;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void InitUcIcd()
        //{
        //    try
        //    {
        //        this.icdProcessor = new HIS.UC.Icd.IcdProcessor();
        //        HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
        //        ado.DelegateNextFocus = NextForcusSubIcd;
        //        ado.DelegateRequiredCause = DelegateRequiredCause;
        //        ado.Width = 330;
        //        ado.Height = 24;
        //        ado.IsColor = (HisConfigCFG.ObligateIcd == GlobalVariables.CommonStringTrue);
        //        ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
        //        ado.AutoCheckIcd = HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue;
        //        this.ucIcd = (UserControl)this.icdProcessor.Run(ado);

        //        if (this.ucIcd != null)
        //        {
        //            this.panelControlIcd.Controls.Add(this.ucIcd);
        //            this.ucIcd.Dock = DockStyle.Fill;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void DelegateRequiredCause(bool isRequired)
        //{
        //    try
        //    {
        //        if (this.icdCauseProcessor != null && this.ucIcdCause != null)
        //        {
        //            this.icdCauseProcessor.SetRequired(this.ucIcdCause, isRequired);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void InitUcCauseIcd()
        //{
        //    try
        //    {
        //        this.icdCauseProcessor = new HIS.UC.Icd.IcdProcessor();
        //        HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
        //        ado.DelegateNextFocus = NextForcusSubIcdCause;
        //        ado.Width = 330;
        //        ado.Height = 24;
        //        ado.IsColor = false;
        //        ado.IsUCCause = true;
        //        ado.DataIcds = BackendDataWorker.Get<HIS_ICD>()
        //            .Where(p => p.IS_CAUSE == 1).OrderBy(o => o.ICD_CODE).ToList();
        //        ado.AutoCheckIcd = HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue;
        //        ado.LblIcdMain = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdCause.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.ToolTipsIcdMain = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdCause.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        this.ucIcdCause = (UserControl)this.icdCauseProcessor.Run(ado);

        //        if (this.ucIcdCause != null)
        //        {
        //            this.panelControlCauseIcd.Controls.Add(this.ucIcdCause);
        //            this.ucIcdCause.Dock = DockStyle.Fill;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void NextForcusSubIcd()
        //{
        //    try
        //    {
        //        if (icdCauseProcessor != null && ucIcdCause != null)
        //        {
        //            icdCauseProcessor.FocusControl(ucIcdCause);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void NextForcusSubIcdCause()
        //{
        //    try
        //    {
        //        if (ucSecondaryIcd != null)
        //        {
        //            subIcdProcessor.FocusControl(ucSecondaryIcd);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void InitUcSecondaryIcd()
        //{
        //    try
        //    {
        //        this.subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList());
        //        HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
        //        ado.DelegateNextFocus = NextForcusOut;
        //        ado.DelegateGetIcdMain = GetIcdMainCode;
        //        ado.Width = 660;
        //        ado.Height = 24;
        //        ado.TextLblIcd = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdText.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.TextNullValue = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtIcdExtraNames.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
        //        this.ucSecondaryIcd = (UserControl)this.subIcdProcessor.Run(ado);

        //        if (this.ucSecondaryIcd != null)
        //        {
        //            this.panelControlSubIcd.Controls.Add(this.ucSecondaryIcd);
        //            this.ucSecondaryIcd.Dock = DockStyle.Fill;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

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

        private void SetDefaultUC()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDefaultUC. 1");
                //Lay gio server
                //TimerSDO timeSync = new BackendAdapter(new CommonParam()).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam());
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeSync), timeSync));
                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                //if (timeSync != null)
                //{
                //    dateInputADO.Time = timeSync.DateNow;
                //    dateInputADO.Dates = new List<DateTime?>();
                //    dateInputADO.Dates.Add(dateInputADO.Time);
                //}
                UcDateReload(dateInputADO);
                Inventec.Common.Logging.LogSystem.Debug("SetDefaultUC. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
