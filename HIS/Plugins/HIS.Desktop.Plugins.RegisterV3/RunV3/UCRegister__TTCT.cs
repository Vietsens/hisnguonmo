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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using HIS.UC.UCTransPati.ADO;
using MOS.SDO;
using Inventec.Core;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using His.UC.UCHein.Base;

namespace HIS.Desktop.Plugins.RegisterV3.Run3
{
    public partial class UCRegister : UserControlBase
    {
        private void ShowFormThongTinChuyenTuyen(bool _isValidate)
        {
            try
            {
                frm = new frmTransPati(_isValidate, this.transPatiADO, UpdateSelectedTranPati);
                frm.SetValidForTTCT(this.CheckValidateFormTTCT);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void UpdateSelectedTranPati(UCTransPatiADO transpatiADO)
        {
            try
            {
                this.transPatiADO = transpatiADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void UpdateTranPatiDataByPatientOld(HIS_PATIENT_TYPE_ALTER patientTypeAlter)
        {
            try
            {
                if (patientTypeAlter == null) throw new ArgumentNullException("patientTypeAlter");
                if (this.transPatiADO == null) this.transPatiADO = new UCTransPatiADO();

                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                treatmentFilter.ID = patientTypeAlter.TREATMENT_ID;
                var treatmentByPatient = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();

                this.transPatiADO.HINHTHUCHUYEN_ID = treatmentByPatient.TRAN_PATI_FORM_ID;
                this.transPatiADO.LYDOCHUYEN_ID = treatmentByPatient.TRAN_PATI_REASON_ID;
                this.transPatiADO.ICD_CODE = treatmentByPatient.TRANSFER_IN_ICD_CODE;
                if (!String.IsNullOrEmpty(treatmentByPatient.TRANSFER_IN_ICD_CODE))
                {
                    var icd = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE == treatmentByPatient.TRANSFER_IN_ICD_CODE);
                    this.transPatiADO.ICD_TEXT = (treatmentByPatient.TRANSFER_IN_ICD_NAME == icd.ICD_NAME ? "" : treatmentByPatient.TRANSFER_IN_ICD_NAME);
                }
                this.transPatiADO.ICD_NAME = treatmentByPatient.TRANSFER_IN_ICD_NAME;
                this.transPatiADO.NOICHUYENDEN_CODE = treatmentByPatient.TRANSFER_IN_MEDI_ORG_CODE;
                this.transPatiADO.NOICHUYENDEN_NAME = treatmentByPatient.TRANSFER_IN_MEDI_ORG_NAME;
                this.transPatiADO.SOCHUYENVIEN = treatmentByPatient.TRANSFER_IN_CODE;
                this.transPatiADO.TRANSFER_IN_CMKT = treatmentByPatient.TRANSFER_IN_CMKT;
                this.transPatiADO.HINHTHUCHUYEN_ID = treatmentByPatient.TRANSFER_IN_FORM_ID;
                this.transPatiADO.LYDOCHUYEN_ID = treatmentByPatient.TRANSFER_IN_REASON_ID;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void CheckValidateFormTTCT(bool isValidated)
        {
            try
            {
                this.ValidatedTTCT = isValidated;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
