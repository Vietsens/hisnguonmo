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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using HIS.Desktop.Plugins.Library.PrintOtherForm.RunPrintTemplate;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000227
{
    public partial class Mps000227Behavior : MpsDataBase, ILoad
    {
        V_HIS_PATIENT patient { get; set; }
        V_HIS_SERVICE_REQ serviceReq { get; set; }
        V_HIS_SERE_SERV_PTTT sereServPTTT { get; set; }
        HIS_SERE_SERV_EXT sereServExt { get; set; }
        HIS_SERE_SERV sereServ { get; set; }
        V_HIS_TREATMENT treatment { get; set; }
        List<V_HIS_EKIP_USER> ekipUsers { get; set; }
        List<V_HIS_TREATMENT_BED_ROOM> treatmentBedRooms { get; set; }
        List<HIS_SERE_SERV_FILE> SereServFiles { get; set; }

        public Mps000227Behavior(long? _serviceReqId, long? _sereServId, long _treatmentId, long _patientId)
            : base(_serviceReqId, _sereServId, _treatmentId, _patientId)
        {

        }

        bool ILoad.Load(string printTypeCode, UpdateType.TYPE updateType)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                this.LoadData();

                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);
                if (printTemplate == null)
                {
                    throw new Exception("Khong tim thay template");
                }

                this.SetDicParamPatient(ref dicParamPlus);
                this.SetDicParamServiceReq(ref dicParamPlus);
                this.SetDicParamBedAndBedRoomFromTreatment(ref dicParamPlus);
                this.SetDicParamEkipUser(ref dicParamPlus);
                this.SetDicParamSereServPTTT(ref dicParamPlus);
                this.SetDicParamSereServExt(ref dicParamPlus);
                this.SetDicImagePlusSereServFile(ref dicImagePlus);

                //Thoi gian vao vien
                if (this.treatment != null)
                {
                    dicParamPlus.Add("CLINICAL_IN_TIME_STR", this.treatment.CLINICAL_IN_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.treatment.CLINICAL_IN_TIME.Value) : "");
                    dicParamPlus.Add("IN_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.treatment.IN_TIME));
                    dicParamPlus.Add("TDL_HEIN_CARD_FROM_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.treatment.TDL_HEIN_CARD_FROM_TIME ?? 0));
                    dicParamPlus.Add("TDL_HEIN_CARD_TO_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.treatment.TDL_HEIN_CARD_TO_TIME ?? 0));
                    dicParamPlus.Add("AGE_STRING", Inventec.Common.DateTime.Calculation.AgeString(this.treatment.TDL_PATIENT_DOB, "", "", "", "", this.treatment.IN_TIME));
                }

                //Thoi gian bat dau
                dicParamPlus.Add("START_TIME_STR", this.serviceReq.START_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.serviceReq.START_TIME.Value) : "");
                // thoi gian ket thuc
                dicParamPlus.Add("FINISH_TIME_STR", this.serviceReq.FINISH_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.serviceReq.FINISH_TIME.Value) : "");
                // thoi gian hien tai
                dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentTime());
                dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR_1", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentDateSeparateFullTime());
                dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR_2", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentDateSeparate());
                //so vao vien
                dicParamPlus.Add("IN_CODE", this.treatment.IN_CODE);



                List<string> keys = new List<string>();
                foreach (var item in dicParamPlus)
                {
                    if (item.Value == null)
                    {
                        keys.Add(item.Key);
                    }
                }

                if (keys.Count > 0)
                {
                    foreach (var key in keys)
                    {
                        dicParamPlus[key] = "";
                    }
                }

                WaitingManager.Hide();
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.treatment != null ? this.treatment.TREATMENT_CODE : ""), printTemplate.PRINT_TYPE_CODE);
                IRunTemp behavior = RunUpdateFactory.RunTemplateByUpdateType(this.serviceReq, updateType);
                result = behavior != null ? (behavior.Run(printTemplate, dicParamPlus, dicImagePlus, richEditorMain, inputADO)) : false;
            }
            catch (Exception ex)
            {
                result = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
