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
using DevExpress.XtraLayout.Utils;
using His.UC.UCHein;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CallPatientTypeAlter.Config;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using Inventec.Common.QrCodeBHYT;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    public partial class frmPatientTypeAlter : HIS.Desktop.Utility.FormBase
    {
        List<HisPatientSDO> listResult = new List<HisPatientSDO>();
        internal void ChoiceTemplateHeinCard(string patientTypeCode, bool focusMoveOut)
        {
            try
            {
                His.UC.UCHein.Base.ResouceManager.ResourceLanguageManager();
                Inventec.Common.Logging.LogSystem.Info("t3.1: begin process ChoiceTemplateHeinCard");
                xclHeinCardInformation.Controls.Clear();
                xclHeinCardInformation.Update();
                xclHeinCardInformation.Enabled = true;
                ucHein__BHYT = new UserControl();
                uCMainHein = new His.UC.UCHein.MainHisHeinBhyt();
                if (patientTypeCode == HisConfigCFG.PatientTypeCode__BHYT || patientTypeCode == HisConfigCFG.PatientTypeCode__QN)
                {
                    emptySpaceItem3.Visibility = LayoutVisibility.Always;
                    layoutControlItem8.Visibility = LayoutVisibility.Always;
                    btnSave.Size = new Size(110, btnSave.Height);
                    this.Size = new Size(this.Width, 410);

                    panelControlImageBHYT.Controls.Clear();
                    panelControlImageBHYT.Update();
                    ucImageBHYT = new UC_ImageBHYT();

                    ucImageBHYT.Dock = DockStyle.Fill;
                    panelControlImageBHYT.Controls.Add(ucImageBHYT);
                    ucImageBHYT.pictureEditImageBHYT.Tag = "NoImage";

                    Inventec.Common.Logging.LogSystem.Info("t3.1.1: set default data to control hein");
                    His.UC.UCHein.Data.DataInitHeinBhyt dataHein = new His.UC.UCHein.Data.DataInitHeinBhyt();
                    dataHein.Template = His.UC.UCHein.MainHisHeinBhyt.TEMPLATE__BHYT1;
                    if (this.currentHisTreatment.TDL_PATIENT_DOB > 0)
                    {
                        dataHein.IsChild = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisTreatment.TDL_PATIENT_DOB).Value);
                    }
                    dataHein.HEIN_LEVEL_CODE__CURRENT = HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                    dataHein.HeinRightRouteTypes = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeStore.Get();
                    dataHein.Icds = BackendDataWorker.Get<HIS_ICD>();
                    dataHein.Genders = BackendDataWorker.Get<HIS_GENDER>();
                    dataHein.BhytBlackLists = BackendDataWorker.Get<HIS_BHYT_BLACKLIST>();
                    dataHein.BhytWhiteLists = BackendDataWorker.Get<HIS_BHYT_WHITELIST>();
                    dataHein.LiveAreas = BackendDataWorker.Get<MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData>();
                    dataHein.MEDI_ORG_CODE__CURRENT = HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT;
                    dataHein.SYS_MEDI_ORG_CODE = HisMediOrgCFG.SYS_MEDI_ORG_CODE;
                    dataHein.MEDI_ORG_CODES__ACCEPTs = HisMediOrgCFG.MEDI_ORG_CODES__ACCEPT;
                    dataHein.MediOrgs = BackendDataWorker.Get<HIS_MEDI_ORG>();
                    dataHein.PATIENT_TYPE_ID__BHYT = HisConfigCFG.PatientTypeId__BHYT;
                    dataHein.PatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                    dataHein.TranPatiForms = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>();
                    dataHein.TranPatiReasons = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>();
                    dataHein.TREATMENT_TYPE_ID__EXAM = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                    dataHein.TreatmentTypes = BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                    dataHein.PatientTypeId = (long)(cboPatientType.EditValue ?? 0);
                    dataHein.isVisibleControl = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__TIEP_DON_HIEN_THI_THONG_TIN_THEM);
                    dataHein.IsDefaultRightRouteType = (HisConfigCFG.IsDefaultRightRouteType == "1" ? true : false);
                    dataHein.IsShowCheckKhongKTHSD = HisConfigCFG.IsShowCheckExpired;
                    dataHein.IsNotRequiredRightTypeInCaseOfHavingAreaCode = HisConfigCFG.IsNotRequiredRightTypeInCaseOfHavingAreaCode;
                    dataHein.AutoCheckIcd = HisConfigCFG.AutoCheckIcd;
                    dataHein.IsEdit = this.isEdit;
                    dataHein.CheckExamHistory = this.CheckTT;
                    dataHein.ExceedDayAllow = HisConfigs.Get<long>("MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT");
                    dataHein.HisTreatment = this.currentHisTreatment;

                    var WorkPlaceSDO = WorkPlace.WorkPlaceSDO.Where(o => o.RoomId == module.RoomId).FirstOrDefault();
                    dataHein.IsSampleDepartment = currentHisTreatment !=null && this.currentHisTreatment.LAST_DEPARTMENT_ID == (WorkPlaceSDO != null ? WorkPlaceSDO.DepartmentId : -1);

                    if (patientTypeCode != HisConfigCFG.PatientTypeCode__BHYT && !string.IsNullOrEmpty(HisConfigCFG.CheckTempQN))
                    {
                        dataHein.IsTempQN = HisConfigCFG.CheckTempQN.Contains(patientTypeCode);
                    }

                    dataHein.SetFocusMoveOut = FocusDelegate;
                    dataHein.IsObligatoryTranferMediOrg = HisConfigCFG.IsObligatoryTranferMediOrg;
                    dataHein.ObligatoryTranferMediOrg = HisConfigCFG.ObligatoryTranferMediOrg;
                    dataHein.IsAutoSelectEmergency = (currentHisTreatment != null && currentHisTreatment.IS_EMERGENCY == (short?)1) || IsHasEmergency;
                    dataHein.AutoCheckCC = CheckCCToEnablePrint;
                    dataHein.currentModule = module;
                    dataHein.IsInitFromCallPatientTypeAlter = true;
                    dataHein.PatientId = currentHisTreatment != null ? currentHisTreatment.PATIENT_ID : 0;
                    dataHein.SetShortcutKeyDown = ShortcutDelegate;
                    Inventec.Common.Logging.LogSystem.Info("t3.1.2: uCMainHein init");
                    ucHein__BHYT = uCMainHein.InitUC(dataHein, ApiConsumers.MosConsumer, null);
                    ucHein__BHYT.Dock = DockStyle.Fill;
                    xclHeinCardInformation.Controls.Add(ucHein__BHYT);
                    Inventec.Common.Logging.LogSystem.Info("t3.1.3: set delegate");
                    uCMainHein.DefaultFocusUserControl(ucHein__BHYT);
                    Inventec.Common.Logging.LogSystem.Info("t3.1.4: end");
                }

                else
                {
                    ucImageBHYT = null;
                    emptySpaceItem3.Visibility = LayoutVisibility.Never;
                    layoutControlItem8.Visibility = LayoutVisibility.Never;
                    btnSave.Size = new Size(110, btnSave.Height);
                    this.Size = new Size(this.Width, 100);

                    xclHeinCardInformation.Controls.Clear();
                    xclHeinCardInformation.Update();
                    xclHeinCardInformation.Enabled = false;
                    if (focusMoveOut)
                    {
                        btnSave.Focus();
                    }
                    ucHein__BHYT = null;
                }

                if (uCMainHein != null && ucHein__BHYT != null)
                {
                    uCMainHein.ResetValidationControl(ucHein__BHYT);
                }
                Inventec.Common.Logging.LogSystem.Info("t3.2: end process ChoiceTemplateHeinCard");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckCCToEnablePrint(bool value)
		{
			try
			{
                if(value || currentHisTreatment.IS_EMERGENCY == (short?) 1)
                    btnPrint.Enabled = true;
                else
                    btnPrint.Enabled = false;
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

		private void FocusDelegate()
        {
            try
            {
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShortcutDelegate(Keys key)
        {
            try
            {
                switch (key)
                {
                    case Keys.S:
                        btnSave.PerformClick();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataAfterFindQrCodeNoExistsCard(HeinCardData dataHein)
        {
            try
            {
                if (uCMainHein != null && ucHein__BHYT != null)
                    uCMainHein.FillDataAfterFindQrCode(ucHein__BHYT, dataHein);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetPatyAlterBhyt(V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, HisPatientSDO patientSdo)
        {
            try
            {
                if (patyAlterBhyt == null) throw new ArgumentNullException("patyAlterBhyt");
                if (patientSdo == null) throw new ArgumentNullException("patientSdo");

                patyAlterBhyt.ADDRESS = patientSdo.HeinAddress;
                patyAlterBhyt.HEIN_CARD_FROM_TIME = (patientSdo.HeinCardFromTime ?? 0);
                patyAlterBhyt.HEIN_CARD_TO_TIME = (patientSdo.HeinCardToTime ?? 0);
                patyAlterBhyt.HEIN_CARD_NUMBER = patientSdo.HeinCardNumber;
                patyAlterBhyt.HEIN_MEDI_ORG_CODE = patientSdo.HeinMediOrgCode;
                patyAlterBhyt.HEIN_MEDI_ORG_NAME = patientSdo.HeinMediOrgName;
                patyAlterBhyt.JOIN_5_YEAR = patientSdo.Join5Year;
                patyAlterBhyt.LIVE_AREA_CODE = patientSdo.LiveAreaCode;
                patyAlterBhyt.PAID_6_MONTH = patientSdo.Paid6Month;
                patyAlterBhyt.RIGHT_ROUTE_CODE = patientSdo.RightRouteCode;
                patyAlterBhyt.RIGHT_ROUTE_TYPE_CODE = patientSdo.RightRouteTypeCode;
                patyAlterBhyt.TDL_PATIENT_ID = patientSdo.ID;
                patyAlterBhyt.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                //patyAlterBhyt.HAS_BIRTH_CERTIFICATE = p
                patyAlterBhyt.IS_NO_CHECK_EXPIRE = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
