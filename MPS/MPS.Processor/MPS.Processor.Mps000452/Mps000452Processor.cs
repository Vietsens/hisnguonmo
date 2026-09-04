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
using MPS.Processor.Mps000452.ADO;
using MPS.Processor.Mps000452.PDO;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000452
{
    public class Mps000452Processor : AbstractProcessor
    {
        List<KskDriverDityADO> lstADO = new List<KskDriverDityADO>();
        List<KskDriverDityADO> lstFullADO = new List<KskDriverDityADO>();
        Mps000452PDO rdo;
        TreatmentAdo TreatmentAdos { get; set; }
        public Mps000452Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000452PDO)rdoBase;
        }
        public override bool ProcessData() 
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                SetSingleKey();
                // Kết luận theo bệnh (ICD-10) của lượt khám — lấy từ HIS_KSK_GENERAL
                SetConclusionIcdKeysFromGeneral();
                SetSignatureKeyImageByCFG();
                SetImageKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                //objectTag.AddObjectData(store, "ServiceReq", new List<V_HIS_SERVICE_REQ>() { rdo.HisServiceReq });
                objectTag.AddObjectData(store, "KskOverEighteen", new List<HIS_KSK_OVER_EIGHTEEN>() { rdo.HisKskOverEighteen });
                objectTag.AddObjectData(store, "Treatment", new List<TreatmentAdo>() { TreatmentAdos });
                objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst });
                objectTag.AddRelationship(store, "KskOverEighteen", "Dhst", "DHST_ID", "ID");

                SetData();
                objectTag.AddObjectData(store, "KskDriverDity", lstADO);
                objectTag.AddObjectData(store, "KskDriverDityFull", lstFullADO);
                objectTag.AddObjectData(store, "DiseaseType", rdo.DiseaseType);

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
        private void SetData()
        {
            try
            {
                int rowContTable1 = 0;

                if (rdo.PeriodDriverDity.Count % 2 == 0)
                {
                    rowContTable1 = rdo.PeriodDriverDity.Count / 2;

                }
                else
                {
                    rowContTable1 = rdo.PeriodDriverDity.Count / 2 + 1;
                }

                for (int i = 0; i < rdo.PeriodDriverDity.Count; i++)
                {
                    KskDriverDityADO ado = new KskDriverDityADO();
                    var name = rdo.DiseaseType.Where(o => o.ID == rdo.PeriodDriverDity[i].DISEASE_TYPE_ID).First().DISEASE_TYPE_NAME;
                    string y = "";
                    string n = "";
                    if (rdo.PeriodDriverDity[i].IS_YES_NO == "1")
                    {
                        y = "X";
                        if (rdo.DiseaseType[i].DISEASE_TYPE_CODE == "22")
                        {
                            name += ":" + rdo.HisKskOverEighteen.PATHOLOGICAL_HISTORY;
                        }
                    }
                    else if (rdo.PeriodDriverDity[i].IS_YES_NO == "0")
                    {
                        n = "X";
                    }
                    ado.NAME_DITY = name;
                    ado.IS_YES = y;
                    ado.IS_NO = n;

                    int indexr = i % rowContTable1;

                    if (lstADO.Count == indexr)
                    {
                        ado.NAME_DITY_1 = name;
                        ado.IS_YES_1 = y;
                        ado.IS_NO_1 = n;
                        lstADO.Add(ado);
                    }
                    else
                    {
                        lstADO[indexr].NAME_DITY_2 = name;
                        lstADO[indexr].IS_YES_2 = y;
                        lstADO[indexr].IS_NO_2 = n;
                    }
                    lstFullADO.Add(ado);

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                SetSingleKey(new KeyValue(Mps000452ExtendSingleKey.CONCLUSION_ICD_NONE_X, icdType == 1 ? "x" : ""));
                SetSingleKey(new KeyValue(Mps000452ExtendSingleKey.CONCLUSION_ICD_PRELIM_X, icdType == 2 ? "x" : ""));
                SetSingleKey(new KeyValue(Mps000452ExtendSingleKey.CONCLUSION_ICD_FINAL_X, icdType == 3 ? "x" : ""));
                SetSingleKey(new KeyValue(Mps000452ExtendSingleKey.CONCLUSION_ICD_CODE,
                    (kskGeneral != null ? kskGeneral.CONCLUSION_ICD_CODE : null) ?? ""));
                SetSingleKey(new KeyValue(Mps000452ExtendSingleKey.CONCLUSION_ICD_NAME,
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
                TreatmentAdos = new TreatmentAdo();
                if (rdo.Treatment != null)
                {
                    TreatmentAdo ado = new TreatmentAdo();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentAdo>(ado, rdo.Treatment);
                    TreatmentAdos = ado;
                }

                if (rdo.HisKskOverEighteen != null)
                {
                    AddObjectKeyIntoListkey<HIS_KSK_OVER_EIGHTEEN>(rdo.HisKskOverEighteen, false);
                }
                // Số thứ tự KSK -> key chung {KSK_NUMBER} (ngoài key thô {KSK_OVER_EIGHTEEN_CODE})
                SetSingleKey(new KeyValue(Mps000452ExtendSingleKey.KSK_NUMBER,
                    rdo.HisKskOverEighteen != null ? rdo.HisKskOverEighteen.KSK_OVER_EIGHTEEN_CODE : ""));
                 if (rdo.HisServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                } if (rdo.HisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.HisDhst, false);
                }
                if (rdo.HisDhst != null)
                {
                    SetSingleKey((new KeyValue(Mps000452ExtendSingleKey.DHST_LOGINNAME, rdo.HisDhst.EXECUTE_LOGINNAME)));
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

                if (rdo.HisServiceReq != null && !string.IsNullOrEmpty(rdo.HisServiceReq.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(Mps000452ExtendSingleKey.IMG_AVATAR, rdo.HisServiceReq.TDL_PATIENT_AVATAR_URL);
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

        public void SetSingleImage(string key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                if (stream != null)
                {
                    SetSingleKey(new KeyValue(key, stream.ToArray()));
                }
                else
                {
                    SetSingleKey(new KeyValue(key, ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
