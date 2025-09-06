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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using HIS.UC.DateEditor;
using HIS.UC.PatientSelect;
using HIS.UC.PeriousExpMestList;
using HIS.UC.SecondaryIcd;
using HIS.UC.TreatmentFinish;
using IMSys.DbConfig.HIS_RS;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void InitMultipleThread()
        {
            try
            {
                this.InitUcIcd();
                this.InitUcCauseIcd();
                this.InitUcSecondaryIcd();
                this.InitUcDate();
                this.InitUCPeriousExpMestList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                this.currentPrescriptionFilter.IS_EXECUTE_KIDNEY_PRES = true;
                this.currentPrescriptionFilter.ORDER_FIELD = "INTRUCTION_TIME";
                this.currentPrescriptionFilter.TDL_PATIENT_ID = VHistreatment.PATIENT_ID;
                ado.ServiceReqView7Filter = this.currentPrescriptionFilter;              
                this.ucPeriousExpMestList = (UserControl)periousExpMestListProcessor.Run(ado);
                if (this.ucPeriousExpMestList != null)
                {
                    //if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)// 
                    //    this.pnlUCPanelRightBottom.Controls.Add(this.ucPeriousExpMestList);
                    //else
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
                ado.IsVisibleMultiDate = GlobalStore.IsTreatmentIn;
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

        private void InitUcIcd()
        {
            try
            {
                this.icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.DelegateRequiredCause = DelegateRequiredCause;
                ado.Width = 330;
                ado.Height = 24;
                ado.IsColor = (HisConfigCFG.ObligateIcd == GlobalVariables.CommonStringTrue);
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
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
                    .Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && p.IS_CAUSE == 1).OrderBy(o => o.ICD_CODE).ToList();
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
                this.subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList());
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.Width = 660;
                ado.Height = 24;
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
                //Lay gio server
                TimerSDO timeSync = new BackendAdapter(new CommonParam()).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam());
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeSync), timeSync));
                ;
                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                if (timeSync != null)
                {

                    dateInputADO.Time = timeSync.DateNow;
                    dateInputADO.Dates = new List<DateTime?>();
                    dateInputADO.Dates.Add(dateInputADO.Time);
                }

                this.ucDateProcessor.Reload(this.ucDate, dateInputADO);
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                this.isMultiDateState = false;

                this.FillAllPatientInfoSelectedInForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
