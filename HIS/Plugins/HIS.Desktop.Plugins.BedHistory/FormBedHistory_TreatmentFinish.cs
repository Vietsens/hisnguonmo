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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedHistory
{
    public partial class FormBedHistory : HIS.Desktop.Utility.FormBase
    {
        private void InitCboTreatmentFinish()
        {
            var listEndType = new List<HIS_TREATMENT_END_TYPE>();
            if (CurrentTreatment != null)
            {
                if (CurrentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    listEndType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().Where(o => o.IS_FOR_OUT_PATIENT == 1).ToList();
                }
                else
                {
                    listEndType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().Where(o => o.IS_FOR_IN_PATIENT == 1).ToList();
                }
            }

            LoadDataGridLookUpEdit(CboTreatmentEndType, "TREATMENT_END_TYPE_CODE", "TREATMENT_END_TYPE_NAME", "ID", listEndType);

            LoadDataGridLookUpEdit(CboTreatmentResult, "TREATMENT_RESULT_CODE", "TREATMENT_RESULT_NAME", "ID", BackendDataWorker.Get<HIS_TREATMENT_RESULT>());
        }

        private void LoadDataGridLookUpEdit(DevExpress.XtraEditors.GridLookUpEdit comboEdit, string code, string name, string value, object data)
        {
            try
            {
                comboEdit.Properties.DataSource = data;
                comboEdit.Properties.DisplayMember = name;
                comboEdit.Properties.ValueMember = value;
                comboEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                comboEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                comboEdit.Properties.ImmediatePopup = true;
                comboEdit.ForceInitialize();
                comboEdit.Properties.View.Columns.Clear();
                comboEdit.Properties.PopupFormSize = new System.Drawing.Size(300, 250);

                GridColumn aColumnCode = comboEdit.Properties.View.Columns.AddField(code);
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = comboEdit.Properties.View.Columns.AddField(name);
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long? CountTreatmentDay()
        {
            long? result = null;
            try
            {
                if (CboTreatmentEndType.EditValue != null && CboTreatmentResult.EditValue != null && DtOutTime.EditValue != null)
                {
                    long? outTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DtOutTime.DateTime);
                    long treatmentEndTypeId = Inventec.Common.TypeConvert.Parse.ToInt32(CboTreatmentEndType.EditValue.ToString());
                    long treatmentResultId = Inventec.Common.TypeConvert.Parse.ToInt32(CboTreatmentResult.EditValue.ToString());
                    long inTime = CurrentTreatment.CLINICAL_IN_TIME.HasValue ? CurrentTreatment.CLINICAL_IN_TIME.Value : CurrentTreatment.IN_TIME;

                    var dataBhyt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.HIS_CONFIG__PATIENT_TYPE_CODE__BHYT));
                    if (CurrentTreatment.TDL_PATIENT_TYPE_ID == (dataBhyt != null ? dataBhyt.ID : 0))
                    {
                        result = HIS.Common.Treatment.Calculation.DayOfTreatment(inTime, outTime, treatmentEndTypeId, treatmentResultId, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                    }
                    else
                    {
                        result = HIS.Common.Treatment.Calculation.DayOfTreatment(inTime, outTime, treatmentEndTypeId, treatmentResultId, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private decimal SplitBedServiceByCircular(DateTime startTime,DateTime finishTime)
        {
            decimal result = 0;
            try
            {
                if (CboTreatmentEndType.EditValue != null && CboTreatmentResult.EditValue != null && DtOutTime.EditValue != null)
                {
                    long? outTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(finishTime);
                    long treatmentEndTypeId = Inventec.Common.TypeConvert.Parse.ToInt32(CboTreatmentEndType.EditValue.ToString());
                    long treatmentResultId = Inventec.Common.TypeConvert.Parse.ToInt32(CboTreatmentResult.EditValue.ToString());
                    long inTime = (long)Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(startTime);
                    result = (decimal)HIS.Common.Treatment.Calculation.DayOfTreatment(inTime, outTime, treatmentEndTypeId, treatmentResultId, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
