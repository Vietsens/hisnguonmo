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
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using His.UC.UCHein;
using His.UC.UCHein.Data;
using His.UC.UCHein.Get.ExpriedTimeHeinCardBhyt;
using His.UC.UCHein.Init;
using His.UC.UCHein.Set.ResetValueControl;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using His.UC.UCHein.Base;
using System.Threading;
using Inventec.Common.Logging;
using System.Threading.Tasks;
using His.UC.UCHein.Core.UpdateDataFormIntoPatientProfile;
using His.UC.UCHein.Core.ResetValidationControl;
using His.UC.UCHein.Set.FocusHeinCardFromTime;
using His.UC.UCHein.Set.InitValidationControl;
using His.UC.UCHein.Set.DefaultFocusUserControl;
using His.UC.UCHein.Core.UpdateDataFormIntoPatientTypeAlter;
using His.UC.UCHein.Set.SetFocusUserByLiveAreaCode;
using His.UC.UCHein.Dispose;
using His.UC.UCHein.Core.SetResultDataADO;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;

namespace His.UC.UCHein
{
    public partial class MainHisHeinBhyt : BusinessBase
    {
        public System.Windows.Forms.UserControl InitUC(object data, Inventec.Common.WebApiClient.ApiConsumer mosConsumer, string language)
        {
            System.Windows.Forms.UserControl result = null;
            TokenStore.language = language;
            try
            {
                ApiConsumerStore.MosConsumer = mosConsumer;
                result = (System.Windows.Forms.UserControl)(InitFactory.MakeIInitUC(param, data).Run());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public void ResetValue(UserControl UC)
        {
            try
            {
                ResetValueControlFactory.MakeIResetValueControl(param, UC).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void SetResultDataADOBhyt(UserControl UC, ResultDataADO ResultDataADO)
        {
            try
            {
                SetResultDataADOFactory.MakeISetResultDataADO(param, UC, ResultDataADO).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void UpdateDataFormIntoPatientProfile(UserControl UC, HisPatientProfileSDO patientProfileSDO)
        {
            try
            {
                UpdateDataFormIntoPatientProfileFactory.MakeIUpdateDataFormIntoPatientProfile(param, UC, patientProfileSDO).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UpdateDataFormIntoPatientTypeAlter(UserControl UC, HisPatientProfileSDO patientProfileSDO)
        {
            try
            {
                UpdateDataFormIntoPatientTypeAlterFactory.MakeIUpdateDataFormIntoPatientProfile(param, UC, patientProfileSDO).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ResetValidationControl(UserControl uc)
        {
            try
            {
                ResetValidationControlFactory.MakeIResetValidationControl(param, uc).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool FillDataHeinInsuranceInfoByPatientTypeAlter(UserControl uc, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER patientTypeAlter)
        {
            bool result = false;
            try
            {
                if (uc == null) throw new ArgumentNullException("ucHein__BHYT is null");
                if (patientTypeAlter == null) throw new ArgumentNullException("patientTypeAlter is null");

                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).ChangeDataHeinInsuranceInfoByPatientTypeAlter(patientTypeAlter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public bool GoiUCTuManHinhTiepDon(UserControl uc, bool _isCallByRegistor)
        {
            bool result = false;
            try
            {
                if (uc == null) throw new ArgumentNullException("ucHein__BHYT is null");
                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).CoPhaiUCDuocGoiTuModuleTiepDonHayKhong(_isCallByRegistor);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public void DefaultFocusUserControl(UserControl uc)
        {
            try
            {
                DefaultFocusUserControlFactory.MakeIDefaultFocusUserControl(param, uc).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void DisposeControl(UserControl uc)
        {
            try
            {
                DisposeFactory.MakeIDispose(param, uc).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool GetInvalidControls(UserControl uc)
        {
            try
            {
                return InitValidationControlFactory.MakeIInitValidationControl(param, uc).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        public void SetFocusUserByLiveAreaCode(UserControl uc)
        {
            try
            {
                SetFocusUserByLiveAreaCodeFactory.MakeISetFocusUserByLiveAreaCode(param, uc).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void AutoCheckRightRoute(UserControl uc, bool IsDungTuyenCapCuu)
        {
            try
            {
                // AutoCheckRightRouteFactory.MakeIAutoCheckRightRoute(param, uc).Run();
                if (uc == null) throw new ArgumentNullException("ucHein__BHYT is null");

                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).AutoCheckRightRoute(IsDungTuyenCapCuu);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ChangeRoomNotEmergency(UserControl uc)
        {
            try
            {
                // AutoCheckRightRouteFactory.MakeIAutoCheckRightRoute(param, uc).Run();
                if (uc == null) throw new ArgumentNullException("ucHein__BHYT is null");

                if (uc is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)uc).ChangeRoomNotEmergency();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void PatientOldUnder6(UserControl UC, bool IsChild)
        {
            try
            {
                PatientOldUnder6Factory.MakeIPatientOldUnder6(param, UC, IsChild).Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
