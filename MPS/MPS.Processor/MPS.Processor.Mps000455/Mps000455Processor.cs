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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000455.PDO;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000455
{
    public class Mps000455Processor : AbstractProcessor
    {
         Mps000455PDO rdo;
         public Mps000455Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000455PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                //objectTag.AddObjectData(store, "ServiceReq", new List<V_HIS_SERVICE_REQ>() { rdo.HisServiceReq });
                //objectTag.AddObjectData(store, "KskDriverCar", new List<HIS_KSK_DRIVER_CAR>() { rdo.HisKskDriverCar });
                SetSingleKey();
                // Kết luận theo bệnh (ICD-10) của lượt khám — lấy từ HIS_KSK_GENERAL
                SetConclusionIcdKeysFromGeneral();
                SetSignatureKeyImageByCFG();
                objectTag.AddObjectData(store, "ExamRank",  rdo.examRank );
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
                SetSingleKey(new KeyValue(Mps000455ExtendSingleKey.CONCLUSION_ICD_NONE_X, icdType == 1 ? "x" : ""));
                SetSingleKey(new KeyValue(Mps000455ExtendSingleKey.CONCLUSION_ICD_PRELIM_X, icdType == 2 ? "x" : ""));
                SetSingleKey(new KeyValue(Mps000455ExtendSingleKey.CONCLUSION_ICD_FINAL_X, icdType == 3 ? "x" : ""));
                SetSingleKey(new KeyValue(Mps000455ExtendSingleKey.CONCLUSION_ICD_CODE,
                    (kskGeneral != null ? kskGeneral.CONCLUSION_ICD_CODE : null) ?? ""));
                SetSingleKey(new KeyValue(Mps000455ExtendSingleKey.CONCLUSION_ICD_NAME,
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
                if (rdo.HisKskDriverCar != null)
                {
                    AddObjectKeyIntoListkey<HIS_KSK_DRIVER_CAR>(rdo.HisKskDriverCar, false);
                }
                if (rdo.HisServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
