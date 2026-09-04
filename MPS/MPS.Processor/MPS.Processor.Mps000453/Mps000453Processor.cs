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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000453.ADO;
using MPS.Processor.Mps000453.PDO;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000453
{
    public class Mps000453Processor : AbstractProcessor
    {
        Mps000453PDO rdo;
        TreatmentAdo TreatmentAdos { get; set; }
        public Mps000453Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000453PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                TreatmentAdos = new TreatmentAdo();
                if (rdo.treatment != null)
                {
                    TreatmentAdo ado = new TreatmentAdo();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentAdo>(ado, rdo.treatment);
                    TreatmentAdos = ado;
                }
                SetImageKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                //objectTag.AddObjectData(store, "ServiceReq", new List<V_HIS_SERVICE_REQ>() { rdo.HisServiceReq });
                objectTag.AddObjectData(store, "KskUnderEighteen", new List<HIS_KSK_UNDER_EIGHTEEN>() { rdo.HisKskUnderEighteen });
                objectTag.AddObjectData(store, "Treatment", new List<TreatmentAdo>() { TreatmentAdos });
                objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst });
                objectTag.AddRelationship(store, "KskUnderEighteen", "Dhst", "DHST_ID", "ID");
                SetSingleKey();
                // Kết luận theo bệnh (ICD-10) của lượt khám — lấy từ HIS_KSK_GENERAL
                SetConclusionIcdKeysFromGeneral();
                SetSignatureKeyImageByCFG();

                if (rdo.lstUneiVaty == null)
                {
                    rdo.lstUneiVaty = new List<HIS_KSK_UNEI_VATY>();
                }
           
                objectTag.AddObjectData(store, "VaccineType", rdo.lstVaccineType);
                objectTag.AddObjectData(store, "KskUneiVaty", rdo.lstUneiVaty);
                objectTag.AddRelationship(store, "KskUneiVaty", "VaccineType", "VACCINE_TYPE_ID", "ID");
                objectTag.AddObjectData(store, "ExamRank", rdo.examRank);

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
      
        /// <summary>
        /// Kết luận theo bệnh (ICD-10) — dữ liệu nằm ở HIS_KSK_GENERAL cùng SERVICE_REQ_ID
        /// (UC "Kết luận theo bệnh ICD-10" dùng chung cho mọi tab KSK), KHÔNG nằm ở bảng khám của mẫu này.
        /// Khuôn theo Mps000516: sinh 3 cờ "x" theo CONCLUSION_ICD_TYPE
        /// (1=Chưa phát hiện bất thường, 2=Chẩn đoán sơ bộ, 3=Chẩn đoán xác định) + mã/tên ICD.
        /// Ngoài ra đổ toàn bộ cột HIS_KSK_GENERAL với prefix GENERAL_ để biểu mẫu dùng thêm
        /// các trường kết luận khác ({GENERAL_DISEASES}, {GENERAL_CONCLUDER_USERNAME}...) mà không đụng key.
        /// </summary>
        private void SetConclusionIcdKeysFromGeneral()
        {
            try
            {
                HIS_KSK_GENERAL kskGeneral = rdo.HisKskGeneral;
                if (kskGeneral != null)
                {
                    AddObjectKeyIntoListkeyWithPrefix<HIS_KSK_GENERAL>(kskGeneral, "GENERAL_", false);
                }
                long? icdType = (kskGeneral != null && kskGeneral.CONCLUSION_ICD_TYPE != null)
                    ? (long?)kskGeneral.CONCLUSION_ICD_TYPE.Value : null;
                SetSingleKey(new KeyValue(Mps000453ExtendSingleKey.CONCLUSION_ICD_NONE_X, icdType == 1 ? "x" : ""));
                SetSingleKey(new KeyValue(Mps000453ExtendSingleKey.CONCLUSION_ICD_PRELIM_X, icdType == 2 ? "x" : ""));
                SetSingleKey(new KeyValue(Mps000453ExtendSingleKey.CONCLUSION_ICD_FINAL_X, icdType == 3 ? "x" : ""));
                SetSingleKey(new KeyValue(Mps000453ExtendSingleKey.CONCLUSION_ICD_CODE,
                    (kskGeneral != null ? kskGeneral.CONCLUSION_ICD_CODE : null) ?? ""));
                SetSingleKey(new KeyValue(Mps000453ExtendSingleKey.CONCLUSION_ICD_NAME,
                    (kskGeneral != null ? kskGeneral.CONCLUSION_ICD_NAME : null) ?? ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                // Y lệnh KSK (entity HIS_SERVICE_REQ) + bệnh nhân (HIS_PATIENT) — key prefix SREQ_ / PATIENT_
                if (rdo.KskServiceReq != null)
                {
                    AddObjectKeyIntoListkeyWithPrefix<HIS_SERVICE_REQ>(rdo.KskServiceReq, "SREQ_", false);
                }
                if (rdo.KskPatient != null)
                {
                    AddObjectKeyIntoListkeyWithPrefix<HIS_PATIENT>(rdo.KskPatient, "PATIENT_", false);
                }
                if (rdo.HisKskUnderEighteen != null)
                {
                    AddObjectKeyIntoListkey<HIS_KSK_UNDER_EIGHTEEN>(rdo.HisKskUnderEighteen, false);
                }
                // Số thứ tự KSK -> key chung {KSK_NUMBER} (ngoài key thô {KSK_UNDER_EIGHTEEN_CODE})
                SetSingleKey(new KeyValue(Mps000453ExtendSingleKey.KSK_NUMBER,
                    rdo.HisKskUnderEighteen != null ? rdo.HisKskUnderEighteen.KSK_UNDER_EIGHTEEN_CODE : ""));
                if (rdo.HisServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                } if (rdo.HisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.HisDhst, false);
                }
                if (rdo.HisDhst != null)
                {
                    SetSingleKey((new KeyValue(Mps000453ExtendSingleKey.DHST_LOGINNAME, rdo.HisDhst.EXECUTE_LOGINNAME)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetImageKey()
        {
            try
            {
                if (TreatmentAdos != null && !string.IsNullOrEmpty(TreatmentAdos.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(TreatmentAdos, TreatmentAdos.TDL_PATIENT_AVATAR_URL);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetSingleImage(TreatmentAdo key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                key.AVATAR = stream != null ? stream.ToArray() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
