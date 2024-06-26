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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000371.PDO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000371.ADO;

namespace MPS.Processor.Mps000371
{
    class Mps000371Processor : AbstractProcessor
    {
        Mps000371PDO rdo;

        List<SereServADO> SereServADOs = new List<SereServADO>();

        public Mps000371Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000371PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    List<SereServADO> listData0 = new List<SereServADO>();
                    List<SereServADO> listData1 = new List<SereServADO>();
                    List<SereServADO> listData2 = new List<SereServADO>();
                    List<SereServADO> listData3 = new List<SereServADO>();

                    for (int i = 0; i < SereServADOs.Count; i++)
                    {
                        int d = i % 4;
                        switch (d)
                        {
                            case 0:
                                listData0.Add(SereServADOs[i]);
                                break;
                            case 1:
                                listData1.Add(SereServADOs[i]);
                                break;
                            case 2:
                                listData2.Add(SereServADOs[i]);
                                break;
                            case 3:
                                listData3.Add(SereServADOs[i]);
                                break;
                            default:
                                break;
                        }
                    }

                    //luôn tạo danh sách để không lỗi mẫu
                    objectTag.AddObjectData(store, "listData0", listData0);
                    objectTag.AddObjectData(store, "listData1", listData1);
                    objectTag.AddObjectData(store, "listData2", listData2);
                    objectTag.AddObjectData(store, "listData3", listData3);

                    objectTag.AddObjectData(store, "ListDataTotal", SereServADOs);
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo._ListServiceReq != null && rdo._ListServiceReq.Count > 0 && rdo._ListSereServ != null && rdo._ListSereServ.Count > 0)
                {
                    int keyNum = 0;
                    foreach (var itemSere in rdo._ListSereServ)
                    {
                        keyNum++;
                        SetSingleKey(new KeyValue("REQUEST_DEPARTMENT_NAME" + keyNum, itemSere.REQUEST_DEPARTMENT_NAME));
                        SetSingleKey(new KeyValue("TDL_SERVICE_NAME" + keyNum, itemSere.TDL_SERVICE_NAME));
                        SetSingleKey(new KeyValue("AMOUNT" + keyNum, itemSere.AMOUNT));
                        SetSingleKey(new KeyValue("INSTRUCTION_NOTE" + keyNum, itemSere.INSTRUCTION_NOTE));
                        SetSingleKey(new KeyValue("VIR_PRICE" + keyNum, itemSere.VIR_PRICE));
                        SetSingleKey(new KeyValue("PRICE" + keyNum, itemSere.PRICE));
                        SetSingleKey(new KeyValue("VAT_RATIO" + keyNum, itemSere.VAT_RATIO));
                        var serviceUnit = rdo.ListServiceUnit.FirstOrDefault(o => o.ID == itemSere.TDL_SERVICE_UNIT_ID);
                        if (serviceUnit != null)
                            SetSingleKey(new KeyValue("TDL_SERVICE_UNIT_NAME" + keyNum, serviceUnit.SERVICE_UNIT_NAME));
                        else
                            SetSingleKey(new KeyValue("TDL_SERVICE_UNIT_NAME" + keyNum, ""));

                        if (itemSere.SERVICE_REQ_ID != null && itemSere.SERVICE_REQ_ID > 0)
                        {
                            var dataServiceReqBySere = rdo._ListServiceReq.FirstOrDefault(o => o.ID == itemSere.SERVICE_REQ_ID);
                            if (dataServiceReqBySere != null)
                            {
                                SetSingleKey(new KeyValue("REQUEST_ROOM_NAME" + keyNum, dataServiceReqBySere.REQUEST_ROOM_NAME));
                                SetSingleKey(new KeyValue("TDL_PATIENT_NAME" + keyNum, dataServiceReqBySere.TDL_PATIENT_NAME));
                                SetSingleKey(new KeyValue("TDL_PATIENT_DOB" + keyNum, dataServiceReqBySere.TDL_PATIENT_DOB));
                                SetSingleKey(new KeyValue("DESCRIPTION" + keyNum, dataServiceReqBySere.DESCRIPTION));
                                SetSingleKey(new KeyValue("RATION_TIME_NAME" + keyNum, dataServiceReqBySere.RATION_TIME_NAME));
                                SetSingleKey(new KeyValue("RATION_TIME_CODE" + keyNum, dataServiceReqBySere.RATION_TIME_CODE));

                                var treatmentBedRoom = rdo.TreatmentBedRoom.FirstOrDefault(o => o.TREATMENT_ID == (itemSere.TDL_TREATMENT_ID ?? 0));
                                var bedLog = rdo.HisBedLog.Where(o => o.TREATMENT_BED_ROOM_ID == treatmentBedRoom.ID && o.FINISH_TIME != null).ToList();
                                if (bedLog != null && bedLog.Count > 0)
                                {
                                    var bed = bedLog.OrderByDescending(o => o.START_TIME).FirstOrDefault().BED_NAME;
                                    SetSingleKey(new KeyValue("BED_NAME" + keyNum, bed));
                                }
                                else
                                    SetSingleKey(new KeyValue("BED_NAME" + keyNum, ""));
                            }
                        }
                    }

                    SereServADOs.AddRange(from r in rdo._ListSereServ select new SereServADO(r, rdo._ListServiceReq, rdo.ListServiceUnit, rdo.TreatmentBedRoom, rdo.HisBedLog));
                }
                long DATE_TIME_NOW = Inventec.Common.TypeConvert.Parse.ToInt64((DateTime.Now.ToString("yyyyMMddHHmmss")));
                SetSingleKey(new KeyValue("DATE_TIME_NOW", DATE_TIME_NOW));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
