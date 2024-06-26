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
using FlexCel.Report;
using Inventec.Common.Logging;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000033.PDO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000033.ADO;
using FlexCel.Core;

namespace MPS.Processor.Mps000033
{
    class Mps000033Processor : AbstractProcessor
    {
        Mps000033PDO rdo;
        List<ADO.FileImageADO> listFile = new List<FileImageADO>();

        public Mps000033Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000033PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                List<MannerADO> listManner = new List<MannerADO>();

                if (rdo.Manner != null && rdo.Manner.Count > 0)
                {
                    foreach (var item in rdo.Manner)
                    {
                        listManner.Add(new MannerADO(item));
                    }
                }

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ekipUser", rdo.ekipUsers);
                objectTag.AddObjectData(store, "listManner", listManner);
                objectTag.AddObjectData(store, "ImageFile", listFile);
                SetSingleKey();
                SetBarcodeKey();

                singleTag.ProcessData(store, singleValueDictionary);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void SetSingleKey()
        {
            try
            {
                if (rdo.ServiceReqPrint != null)
                {
                    //keyValues.Add(new KeyValue(Mps000033ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.LOCK_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000033ExtendSingleKey.START_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.START_TIME ?? 0)));
                    if (rdo.ServiceReqPrint.FINISH_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000033ExtendSingleKey.FINISH_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.FINISH_TIME ?? 0)));
                }
                else
                {
                    //keyValues.Add(new KeyValue(Mps000033ExtendSingleKey.OPEN_TIME_SEPARATE_STR, ""));
                    SetSingleKey(new KeyValue(Mps000033ExtendSingleKey.START_TIME_STR, ""));
                }

                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000033ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.treatment.IN_TIME)));
                }

                foreach (var ekipUser in rdo.ekipUsers)
                {
                    SetSingleKey(new KeyValue("USERNAME_EXECUTE_ROLE_" + ekipUser.EXECUTE_ROLE_CODE, ekipUser.USERNAME));
                    SetSingleKey(new KeyValue("LOGIN_NAME_EXECUTE_ROLE_" + ekipUser.EXECUTE_ROLE_CODE, ekipUser.LOGINNAME));
                }

                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.treatment, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_PTTT>(rdo.sereServsPttt, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_5>(rdo.sereServ, false);
                AddObjectKeyIntoListkey<HIS_SERE_SERV_EXT>(rdo.SereServExt, false);
                AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo.departmentTran, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReqPrint, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.Patient, false);
                AddObjectKeyIntoListkey<V_HIS_BED_LOG>(rdo.BedLog);
                if (rdo.LastBedLog != null)
                {
                    SetSingleKey(new KeyValue(Mps000033ExtendSingleKey.LAST_BED_CODE, rdo.LastBedLog.BED_CODE));
                    SetSingleKey(new KeyValue(Mps000033ExtendSingleKey.LAST_BED_NAME, rdo.LastBedLog.BED_NAME));
                }
                if(rdo.sesePtttMethod != null && rdo.sesePtttMethod.Count > 0)
                {
                    List<string> lstCode = new List<string>();
                    List<string> lstName = new List<string>();
                    if (rdo.sereServsPttt != null)
                    {
                        if (!string.IsNullOrEmpty(rdo.sereServsPttt.REAL_PTTT_METHOD_CODE))
                            lstCode.Add(rdo.sereServsPttt.REAL_PTTT_METHOD_CODE);
                        if (!string.IsNullOrEmpty(rdo.sereServsPttt.REAL_PTTT_METHOD_NAME))
                            lstName.Add(rdo.sereServsPttt.REAL_PTTT_METHOD_NAME);
                    }                   
                    lstCode.AddRange(rdo.sesePtttMethod.Select(o=>o.PTTT_METHOD_CODE));
                    lstName.AddRange(rdo.sesePtttMethod.Select(o => o.PTTT_METHOD_NAME));
                    SetSingleKey(new KeyValue(Mps000033ExtendSingleKey.REAL_PTTT_METHOD_CODE, string.Join("; ", lstCode)));
                    SetSingleKey(new KeyValue(Mps000033ExtendSingleKey.REAL_PTTT_METHOD_NAME, string.Join("; ", lstName)));
                }
                if (rdo.SkinSurgeryDesc != null)
                {
                    AddObjectKeyIntoListkey<HIS_SKIN_SURGERY_DESC>(rdo.SkinSurgeryDesc, false);
                }

                if (rdo.SereServFile != null && rdo.SereServFile.Count > 0)
                {
                    var lstFile = rdo.SereServFile.OrderBy(o => o.ID).ToList();
                    int count = 1;
                    foreach (var item in lstFile)
                    {
                        if (!String.IsNullOrWhiteSpace(item.URL))
                        {
                            ADO.FileImageADO ado = new FileImageADO();
                            ado.FILE_NAME = item.SERE_SERV_FILE_NAME;
                            SetSingleImage("IMAGE_" + count, item.URL, ref ado);
                            count++;
                            listFile.Add(ado);
                        }
                    }
                }
                SetSingleKey(new KeyValue(Mps000033ExtendSingleKey.AGE_STRING, Inventec.Common.DateTime.Calculation.AgeString(rdo.treatment.TDL_PATIENT_DOB,"","","","",rdo.treatment.IN_TIME)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetSingleImage(string key, string imageUrl, ref ADO.FileImageADO ado)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                if (stream != null)
                {
                    SetSingleKey(new KeyValue(key, stream.ToArray()));
                    stream.Position = 0;
                    ado.IMAGE_DATA = stream.ToArray();
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

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã điều trị: " + rdo.ServiceReqPrint.TREATMENT_CODE;
                log += " , Mã yêu cầu: " + rdo.ServiceReqPrint.SERVICE_REQ_CODE;
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.ServiceReqPrint != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.ServiceReqPrint.TREATMENT_CODE;
                    string serviceReqCode = "SERVICE_REQ_CODE:" + rdo.ServiceReqPrint.SERVICE_REQ_CODE;
                    string requestDepartmentName = rdo.ServiceReqPrint.REQUEST_DEPARTMENT_NAME;
                    string intructionTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.INTRUCTION_TIME);
                    long sereServId = rdo.sereServ.ID;

                    result = String.Format("{0} {1} {2} - {3} - {4} - {5}", printTypeCode, treatmentCode, serviceReqCode, requestDepartmentName, intructionTime, sereServId);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
