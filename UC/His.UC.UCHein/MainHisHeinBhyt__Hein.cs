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
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using His.UC.UCHein;
using His.UC.UCHein.Data;
using His.UC.UCHein.Init;
using His.UC.UCHein.Set.ResetValueControl;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using His.UC.UCHein.FillDataToHeinInsuranceInfoByOldPatient;
using His.UC.UCHein.Base;
using His.UC.UCHein.Set.FocusHeinCardFromTime;
using His.UC.UCHein.Get.ExpriedTimeHeinCardBhyt;
using His.UC.UCHein.FillDataTranPatiInForm;

namespace His.UC.UCHein
{
    public partial class MainHisHeinBhyt : BusinessBase
    {
        public static string TEMPLATE__BHYT1 = "TemplateBHYT1";
        public void SetValueTreatmentType(UserControl uc, long TreatmentTypeId)
        {
            try
            {
                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).SetTreatmentType(TreatmentTypeId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void ShowComboSoThe(UserControl uc)
        {
            try
            {
                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).ShowComboSoThe();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void SetValueLogTime(UserControl uc, long ltime)
        {
            try
            {
                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).SetLogTime(ltime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void SetValueAddress(UserControl uc, string address)
        {
            try
            {
                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).SetValueAddress(address);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FillDataAfterFindQrCode(UserControl uc, Inventec.Common.QrCodeBHYT.HeinCardData heinCardData)
        {
            try
            {
                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).FillDataAfterFindQrCode(heinCardData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //FIll data ToDateTime BHYT
        public void FillDataAfterCheckBHYT(UserControl uc, Inventec.Common.QrCodeBHYT.HeinCardData heinCardData)
        {
            try
            {
                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).FillDataAfterCheckBHYT(heinCardData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UpdateHasDobCertificateEnable(UserControl uc, bool hasDobCertificate)
        {
            try
            {
                LogSystem.Debug("UpdateHasDobCertificateEnable t1. begin ep kieu");
                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    LogSystem.Debug("UpdateHasDobCertificateEnable t1. end ep kieu");
                    LogSystem.Debug("UpdateHasDobCertificateEnable t2. begin update checkbox");
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).UpdateHasDobCertificateEnable(hasDobCertificate);
                    LogSystem.Debug("UpdateHasDobCertificateEnable t2. end update checkbox");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public CheckEdit GetchkHasDobCertificate(UserControl uc)
        {
            try
            {
                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    return ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).chkHasDobCertificate;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        public void FillDataToHeinInsuranceInfoByOldPatient(UserControl uc, HisPatientSDO patient)
        {
            try
            {
                IRun behavior = FillDataToHeinInsuranceInfoByOldPatientFactory.MakeIFillDataToHeinInsuranceInfoByOldPatient(param, patient, uc);
                var result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FillDataTranPatiInForm(UserControl uc, long treatmentId)
        {
            try
            {
                IRun behavior = FillDataTranPatiInFormFactory.MakeIFillDataTranPatiInForm(param, treatmentId, uc);
                var result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SelectMediOrgForSearch(UserControl uc, bool isSearch)
        {
            try
            {
                if (uc.GetType() == typeof(Design.TemplateHeinBHYT1.Template__HeinBHYT1))
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).SelectMediOrgForSearch(isSearch);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusHeinCardFromTime(UserControl uc)
        {
            try
            {
                FocusHeinCardFromTimeFactory.MakeIFocusHeinCardFromTime(param, uc).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public long AlertExpriedTimeHeinCardBhyt(UserControl uc, long alertExpriedTimeHeinCardBhyt, ref long resultDayAlert)
        {
            try
            {
                return ExpriedTimeHeinCardBhytFactory.MakeIExpriedTimeHeinCardBhyt(param, uc, alertExpriedTimeHeinCardBhyt, ref resultDayAlert).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return 0;
        }

        public void InitOldPatientData(UserControl uc, long? patientId, string heinCardNumber)
        {
            try
            {
                if (uc.GetType() == typeof(Design.TemplateHeinBHYT1.Template__HeinBHYT1))
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).InitOldPatientData(patientId ?? 0, heinCardNumber);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
