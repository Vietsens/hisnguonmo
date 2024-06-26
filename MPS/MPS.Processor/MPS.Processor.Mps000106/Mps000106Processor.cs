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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000106.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000106
{
    public class Mps000106Processor : AbstractProcessor
    {
        List<Mps000106SDO> listSereServ;
        List<Mps000106SDO> listSereServType;

        List<Mps000106SDO> ExecuteRoomGroup;

        Mps000106PDO rdo;
        public Mps000106Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000106PDO)rdoBase;
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
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                SetBarcodeKey();
                ProcessListSereServ();
                SetSingleKey();
                if (listSereServType != null && listSereServType.Count > 0)
                {
                    listSereServType = listSereServType.OrderBy(o => o.Type).ToList();
                }

                listSereServ = listSereServ.OrderBy(o => o.Type).ThenBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.TDL_SERVICE_CODE).ToList();
                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                ExecuteRoomGroup = new List<Mps000106SDO>();
                ExecuteRoomGroup = listSereServ.GroupBy(g => new { g.TDL_EXECUTE_ROOM_ID, g.TDL_SERVICE_TYPE_ID }).Select(s => s.First()).ToList();

                var ServiceTypeGroups = new List<Mps000106SDO>();
                ServiceTypeGroups = ExecuteRoomGroup.GroupBy(o => o.TDL_SERVICE_TYPE_ID).Select(s => s.First()).ToList();

                objectTag.AddObjectData(store, "SereServ", listSereServ);
                objectTag.AddObjectData(store, "SereServType", listSereServType);
                objectTag.AddRelationship(store, "SereServType", "SereServ", "Type", "Type");

                var listSereServGroupServiceType = listSereServ.GroupBy(g => g.TDL_SERVICE_TYPE_ID).Select(s => new Mps000106SDO { TDL_SERVICE_TYPE_ID = s.First().TDL_SERVICE_TYPE_ID, SERVICE_TYPE_NAME = s.First().SERVICE_TYPE_NAME }).ToList();
                listSereServGroupServiceType = listSereServGroupServiceType.OrderBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                objectTag.AddObjectData(store, "SereServData", listSereServ);
                objectTag.AddObjectData(store, "SereServTypeGroup", listSereServGroupServiceType);
                objectTag.AddRelationship(store, "SereServTypeGroup", "SereServData", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");

                objectTag.AddObjectData(store, "ExecuteRoomGroup", ExecuteRoomGroup);
                objectTag.AddObjectData(store, "ServiceTypeGroup", ServiceTypeGroups);
                objectTag.AddRelationship(store, "ServiceTypeGroup", "ExecuteRoomGroup", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceTypeGroup", "SereServ", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceTypeGroup", "SereServData", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ExecuteRoomGroup", "SereServ", "TDL_EXECUTE_ROOM_ID", "TDL_EXECUTE_ROOM_ID");
                objectTag.AddRelationship(store, "ExecuteRoomGroup", "SereServData", "TDL_EXECUTE_ROOM_ID", "TDL_EXECUTE_ROOM_ID");

                List<Mps000106SDO> ListSereServTotal = new List<Mps000106SDO>();
                List<Mps000106SDO> ServiceReqGroupTotals = new List<Mps000106SDO>();
                List<Mps000106SDO> ServiceTypeGroupTotals = new List<Mps000106SDO>();

                if (listSereServ != null && listSereServ.Count > 0)
                {
                    ListSereServTotal.AddRange(from r in listSereServ select new Mps000106SDO(r));
                    ListSereServTotal = ListSereServTotal.OrderBy(o => o.SERVICE_TYPE_NUM_ORDER ?? 9999).ThenBy(o => o.TDL_SERVICE_NAME).ThenBy(o => o.SERE_SERV_ID).ToList();

                    List<Mps000106SDO> SereServTotal = new List<Mps000106SDO>();

                    var serviceReqs = ListSereServTotal.GroupBy(o => new { o.SERVICE_REQ_ID, o.TDL_SERVICE_TYPE_ID }).ToList();
                    var serviceTypes = serviceReqs.GroupBy(o => o.Key.TDL_SERVICE_TYPE_ID).ToList();

                    long countType = 1;
                    long count = 1;
                    foreach (var sType in serviceTypes)
                    {
                        Mps000106SDO sTypeAdo = new Mps000106SDO(sType.First().First());
                        sTypeAdo.TOTAL_PRICE_DV = sType.Sum(o => o.Sum(s => s.TOTAL_PRICE_DV));
                        sTypeAdo.TOTAL_PRICE_FEE = sType.Sum(o => o.Sum(s => s.TOTAL_PRICE_FEE));
                        sTypeAdo.TOTAL_PRICE_VP = sType.Sum(o => o.Sum(s => s.TOTAL_PRICE_VP));
                        sTypeAdo.TOTAL_PRICE_SERVICE = sType.Sum(o => o.Sum(s => s.TOTAL_PRICE_SERVICE));
                        sTypeAdo.STT = countType;
                        ServiceTypeGroupTotals.Add(sTypeAdo);
                        countType++;

                        var reqs = serviceReqs.Where(o => o.Key.TDL_SERVICE_TYPE_ID == sType.Key).ToList();
                        if (sType.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)//sắp xếp lại với xét nghiệm
                        {
                            reqs = reqs.OrderBy(o => o.First().SERVICE_TYPE_NUM_ORDER ?? 9999).ThenBy(o => o.First().TDL_EXECUTE_DEPARTMENT_ID).ToList();
                        }

                        foreach (var req in reqs)
                        {
                            Mps000106SDO reqAdo = new Mps000106SDO(req.First());
                            reqAdo.TOTAL_PRICE_DV = req.Sum(o => o.TOTAL_PRICE_DV);
                            reqAdo.TOTAL_PRICE_FEE = req.Sum(o => o.TOTAL_PRICE_FEE);
                            reqAdo.TOTAL_PRICE_VP = req.Sum(o => o.TOTAL_PRICE_VP);
                            reqAdo.TOTAL_PRICE_SERVICE = req.Sum(o => o.TOTAL_PRICE_SERVICE);
                            reqAdo.STT = count;
                            ServiceReqGroupTotals.Add(reqAdo);
                            count++;

                            //sắp xếp lại với xét nghiệm
                            if (req.Key.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                            {
                                SereServTotal.AddRange(req.OrderByDescending(o => o.SERVICE_PARENT_NUM_ORDER).ThenByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERE_SERV_ID).ToList());
                            }
                            else
                            {
                                SereServTotal.AddRange(req);
                            }
                        }
                    }

                    if (ListSereServTotal.Count == SereServTotal.Count)
                    {
                        ListSereServTotal = SereServTotal;
                    }
                }

                objectTag.AddObjectData(store, "ServiceTotals", ListSereServTotal);
                objectTag.AddObjectData(store, "ServiceReqGroupTotals", ServiceReqGroupTotals);
                objectTag.AddObjectData(store, "ServiceTypeGroupTotals", ServiceTypeGroupTotals);

                objectTag.AddRelationship(store, "ServiceTypeGroupTotals", "ServiceTotals", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceTypeGroupTotals", "ServiceReqGroupTotals", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceReqGroupTotals", "ServiceTotals", "SERVICE_REQ_ID", "SERVICE_REQ_ID");

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessListSereServ()
        {
            try
            {
                if (rdo.listSereServBill != null && rdo.listSereServBill.Count > 0 && rdo.listSereServBill.Any(a => a.TDL_AMOUNT.HasValue))
                {
                    Inventec.Common.Logging.LogSystem.Info("ProcessListSereServ - listSereServBill");
                    listSereServ = new List<Mps000106SDO>();
                    listSereServType = new List<Mps000106SDO>();
                    foreach (var item in rdo.listSereServBill)
                    {
                        var sdo = new Mps000106SDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000106SDO>(sdo, item);
                        sdo.VIR_PRICE = sdo.TDL_REAL_PRICE;
                        sdo.VIR_PATIENT_PRICE = sdo.TDL_REAL_PATIENT_PRICE;
                        sdo.VIR_HEIN_PRICE = sdo.TDL_REAL_HEIN_PRICE;
                        sdo.AMOUNT = (sdo.TDL_AMOUNT ?? 0);

                        if (item.MEDICINE_ID.HasValue)
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 2);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000106SDO() { Type = 2, SERVICE_TYPE_NAME = item.SERVICE_TYPE_NAME });
                            }
                            sdo.Type = 2;
                        }
                        else if (item.MATERIAL_ID.HasValue)
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 3);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000106SDO() { Type = 3, SERVICE_TYPE_NAME = item.SERVICE_TYPE_NAME });
                            }
                            sdo.Type = 3;
                        }
                        else
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 1);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000106SDO() { Type = 1, SERVICE_TYPE_NAME = "Dịch vụ" });
                            }
                            sdo.Type = 1;
                        }

                        sdo.PRICE_DV = item.TDL_REAL_PRICE;

                        if (item.TDL_LIMIT_PRICE.HasValue)
                        {
                            sdo.PRICE_VP = item.TDL_LIMIT_PRICE;
                        }
                        else
                        {
                            //bắt buộc truyền vào ko là ko in ra
                            if (item.TDL_PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeBHYT || item.TDL_PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeVP)
                            {
                                sdo.PRICE_VP = item.TDL_REAL_PRICE;
                            }
                            else
                            {
                                sdo.PRICE_VP = 0;
                            }
                        }

                        sdo.PRICE_DV = item.TDL_REAL_PRICE - sdo.PRICE_VP;

                        //có chênh lệch
                        if (sdo.PRICE_DV == 0 && sdo.TDL_HEIN_LIMIT_PRICE.HasValue && sdo.TDL_REAL_PRICE > sdo.TDL_HEIN_LIMIT_PRICE)
                        {
                            sdo.PRICE_VP = sdo.TDL_HEIN_LIMIT_PRICE;
                            sdo.PRICE_DV = (sdo.TDL_REAL_PRICE ?? 0) - sdo.PRICE_VP;
                        }

                        if (item.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && (item.TDL_TOTAL_PATIENT_PRICE ?? 0) > (item.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0) && item.TDL_PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeBHYT)
                        {
                            //2 là hóa đơn dv
                            if (rdo._Transaction.BILL_TYPE_ID == 2)
                            {
                                sdo.PRICE_VP = 0;
                                sdo.TDL_TOTAL_HEIN_PRICE = 0;
                            }
                            else
                            {
                                sdo.PRICE_DV = 0;
                            }
                        }

                        sdo.TOTAL_PRICE_DV = (sdo.PRICE_DV ?? 0) * sdo.AMOUNT;
                        sdo.TOTAL_PRICE_VP = (sdo.PRICE_VP ?? 0) * sdo.AMOUNT;

                        if (item.TDL_HEIN_LIMIT_PRICE.HasValue && (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH))
                        {
                            sdo.PRICE_FEE = item.TDL_HEIN_LIMIT_PRICE;
                        }
                        else if (item.TDL_LIMIT_PRICE.HasValue)
                        {
                            sdo.PRICE_FEE = item.TDL_LIMIT_PRICE;
                        }
                        else if (item.TDL_PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeBHYT
                            || item.TDL_PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeVP)
                        {
                            sdo.PRICE_FEE = item.TDL_REAL_PRICE ?? 0;
                        }
                        else
                        {
                            sdo.PRICE_FEE = 0;
                        }

                        sdo.PRICE_SERVICE = (item.TDL_REAL_PRICE ?? 0) - (sdo.PRICE_FEE ?? 0);

                        //Y nghia la co chech lech thi tach ra
                        if (sdo.PRICE_SERVICE == 0 && item.TDL_HEIN_LIMIT_PRICE.HasValue && (item.TDL_REAL_PRICE ?? 0) > item.TDL_HEIN_LIMIT_PRICE)
                        {
                            //khi có chênh lệch thì phần chênh lệch chỉ dồn sang khi là dịch vụ khám hoặc giường.
                            if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                            {
                                sdo.PRICE_FEE = item.TDL_HEIN_LIMIT_PRICE;
                                sdo.PRICE_SERVICE = (item.TDL_REAL_PRICE ?? 0) - sdo.PRICE_FEE;
                            }
                        }

                        sdo.TOTAL_PRICE_SERVICE = (sdo.PRICE_SERVICE ?? 0) * item.TDL_AMOUNT;
                        sdo.TOTAL_PRICE_FEE = (sdo.PRICE_FEE ?? 0) * item.TDL_AMOUNT;

                        if (rdo.listSereServ != null && rdo.listSereServ.Count > 0)
                        {
                            var ss = rdo.listSereServ.FirstOrDefault(o => o.ID == item.SERE_SERV_ID);
                            if (ss != null)
                            {
                                sdo.REQUEST_ROOM_NAME = ss.REQUEST_ROOM_NAME;
                                sdo.EXECUTE_ROOM_NAME = ss.EXECUTE_ROOM_NAME;
                                sdo.TDL_EXECUTE_ROOM_ID = ss.TDL_EXECUTE_ROOM_ID;
                            }
                        }

                        V_HIS_SERVICE service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (service != null)
                        {
                            sdo.TDL_SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                            sdo.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                            sdo.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                            sdo.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                            sdo.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                            sdo.HEIN_SERVICE_TYPE_NUM_ORDER = service.HEIN_SERVICE_TYPE_NUM_ORDER;

                            var serviceType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == service.SERVICE_TYPE_ID);
                            if (serviceType != null)
                            {
                                sdo.SERVICE_TYPE_NUM_ORDER = serviceType.NUM_ORDER;
                            }

                            if (service.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                            {
                                sdo.SERVICE_NUM_ORDER = service.NUM_ORDER;
                                if (service.PARENT_ID.HasValue)
                                {
                                    V_HIS_SERVICE parent = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == service.PARENT_ID.Value);
                                    if (parent != null)
                                    {
                                        sdo.SERVICE_PARENT_NUM_ORDER = parent.NUM_ORDER ?? -1;
                                    }
                                }
                            }
                            else
                            {
                                sdo.SERVICE_NUM_ORDER = -1;
                                sdo.SERVICE_PARENT_NUM_ORDER = -1;
                            }
                        }
                        else
                        {
                            sdo.SERVICE_NUM_ORDER = -1;
                            sdo.SERVICE_PARENT_NUM_ORDER = -1;
                        }

                        listSereServ.Add(sdo);
                    }

                    var mediMateNotBill = rdo.listSereServ != null ? rdo.listSereServ.Where(o => (o.VIR_TOTAL_PATIENT_PRICE ?? 0) == 0).ToList() : null;
                    if (mediMateNotBill != null && mediMateNotBill.Count > 0)
                    {
                        var Groups = mediMateNotBill.GroupBy(p => new { p.SERVICE_ID, p.TDL_SERVICE_CODE, p.TDL_SERVICE_NAME, p.PRICE, p.VAT_RATIO, p.IS_EXPEND }).Select(p => p.ToList()).ToList();
                        foreach (var item in Groups)
                        {
                            if (!rdo.ShowExpend && item.FirstOrDefault().IS_EXPEND == 1) continue;

                            //if (rdo.listSereServBill.Any(a => a.SERE_SERV_ID == item.ID)) continue;

                            var sdo = new Mps000106SDO(item.FirstOrDefault(), rdo.Mps000106ADO);
                            sdo.AMOUNT = item.Sum(p => p.AMOUNT);
                            sdo.TDL_AMOUNT = item.Sum(p => p.AMOUNT);
                            sdo.TDL_TOTAL_HEIN_PRICE = item.Sum(p => p.VIR_TOTAL_HEIN_PRICE);
                            sdo.TDL_TOTAL_PATIENT_PRICE = item.Sum(p => p.VIR_TOTAL_PATIENT_PRICE);

                            //Inventec.Common.Mapper.DataObjectMapper.Map<Mps000106SDO>(sdo, item);
                            if (item.FirstOrDefault().MEDICINE_ID.HasValue)
                            {
                                var type = listSereServType.FirstOrDefault(o => o.Type == 2);
                                if (type == null)
                                {
                                    listSereServType.Add(new Mps000106SDO() { Type = 2, SERVICE_TYPE_NAME = item.FirstOrDefault().SERVICE_TYPE_NAME });
                                }
                                sdo.Type = 2;
                            }
                            else if (item.FirstOrDefault().MATERIAL_ID.HasValue)
                            {
                                var type = listSereServType.FirstOrDefault(o => o.Type == 3);
                                if (type == null)
                                {
                                    listSereServType.Add(new Mps000106SDO() { Type = 3, SERVICE_TYPE_NAME = item.FirstOrDefault().SERVICE_TYPE_NAME });
                                }
                                sdo.Type = 3;
                            }
                            else
                            {
                                var type = listSereServType.FirstOrDefault(o => o.Type == 1);
                                if (type == null)
                                {
                                    listSereServType.Add(new Mps000106SDO() { Type = 1, SERVICE_TYPE_NAME = "Dịch vụ" });
                                }
                                sdo.Type = 1;
                            }

                            sdo.PRICE_DV = item.FirstOrDefault().VIR_PRICE;

                            if (item.FirstOrDefault().LIMIT_PRICE.HasValue)
                            {
                                sdo.PRICE_VP = item.FirstOrDefault().LIMIT_PRICE;
                            }
                            else
                            {
                                //bắt buộc truyền vào ko là ko in ra
                                if (item.FirstOrDefault().PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeBHYT || item.FirstOrDefault().PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeVP)
                                {
                                    sdo.PRICE_VP = item.FirstOrDefault().VIR_PRICE;
                                }
                                else
                                {
                                    sdo.PRICE_VP = 0;
                                }
                            }

                            sdo.PRICE_DV = item.FirstOrDefault().VIR_PRICE - sdo.PRICE_VP;

                            //có chênh lệch
                            if (sdo.PRICE_DV == 0 && sdo.TDL_HEIN_LIMIT_PRICE.HasValue && sdo.TDL_REAL_PRICE > sdo.TDL_HEIN_LIMIT_PRICE)
                            {
                                sdo.PRICE_VP = sdo.TDL_HEIN_LIMIT_PRICE;
                                sdo.PRICE_DV = (sdo.TDL_REAL_PRICE ?? 0) - sdo.PRICE_VP;
                            }

                            if (item.FirstOrDefault().TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH &&
                                (item.FirstOrDefault().VIR_TOTAL_PATIENT_PRICE ?? 0) > (item.FirstOrDefault().VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0) &&
                                item.FirstOrDefault().PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeBHYT)
                            {
                                //2 là hóa đơn dv
                                if (rdo._Transaction.BILL_TYPE_ID == 2)
                                {
                                    sdo.PRICE_VP = 0;
                                    sdo.TDL_TOTAL_HEIN_PRICE = 0;
                                }
                                else
                                {
                                    sdo.PRICE_DV = 0;
                                }
                            }

                            sdo.TOTAL_PRICE_DV = (sdo.PRICE_DV ?? 0) * sdo.AMOUNT;
                            sdo.TOTAL_PRICE_VP = (sdo.PRICE_VP ?? 0) * sdo.AMOUNT;

                            listSereServ.Add(sdo);
                        }
                    }
                }
                else if (rdo.listSereServ != null && rdo.listSereServ.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("ProcessListSereServ - listSereServ");
                    listSereServ = new List<Mps000106SDO>();
                    listSereServType = new List<Mps000106SDO>();
                    foreach (var item in rdo.listSereServ)
                    {
                        //if (item.IS_EXPEND == 1) continue;
                        if (!rdo.ShowExpend && item.IS_EXPEND == 1) continue;

                        var sdo = new Mps000106SDO(item, rdo.Mps000106ADO);
                        //Inventec.Common.Mapper.DataObjectMapper.Map<Mps000106SDO>(sdo, item);
                        if (item.MEDICINE_ID.HasValue)
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 2);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000106SDO() { Type = 2, SERVICE_TYPE_NAME = item.SERVICE_TYPE_NAME });
                            }
                            sdo.Type = 2;
                        }
                        else if (item.MATERIAL_ID.HasValue)
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 3);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000106SDO() { Type = 3, SERVICE_TYPE_NAME = item.SERVICE_TYPE_NAME });
                            }
                            sdo.Type = 3;
                        }
                        else
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 1);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000106SDO() { Type = 1, SERVICE_TYPE_NAME = "Dịch vụ" });
                            }
                            sdo.Type = 1;
                        }

                        sdo.PRICE_DV = item.VIR_PRICE;

                        if (item.LIMIT_PRICE.HasValue)
                        {
                            sdo.PRICE_VP = item.LIMIT_PRICE;
                        }
                        else
                        {
                            //bắt buộc truyền vào ko là ko in ra
                            if (item.PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeBHYT || item.PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeVP)
                            {
                                sdo.PRICE_VP = item.VIR_PRICE;
                            }
                            else
                            {
                                sdo.PRICE_VP = 0;
                            }
                        }

                        sdo.PRICE_DV = item.VIR_PRICE - sdo.PRICE_VP;

                        //có chênh lệch
                        if (sdo.PRICE_DV == 0 && sdo.TDL_HEIN_LIMIT_PRICE.HasValue && sdo.TDL_REAL_PRICE > sdo.TDL_HEIN_LIMIT_PRICE)
                        {
                            sdo.PRICE_VP = sdo.TDL_HEIN_LIMIT_PRICE;
                            sdo.PRICE_DV = (sdo.TDL_REAL_PRICE ?? 0) - sdo.PRICE_VP;
                        }

                        if (item.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && (item.VIR_TOTAL_PATIENT_PRICE ?? 0) > (item.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0) && item.PATIENT_TYPE_ID == rdo.Mps000106ADO.PatientTypeBHYT)
                        {
                            //2 là hóa đơn dv
                            if (rdo._Transaction.BILL_TYPE_ID == 2)
                            {
                                sdo.PRICE_VP = 0;
                                sdo.TDL_TOTAL_HEIN_PRICE = 0;
                            }
                            else
                            {
                                sdo.PRICE_DV = 0;
                            }
                        }

                        sdo.TOTAL_PRICE_DV = (sdo.PRICE_DV ?? 0) * sdo.AMOUNT;
                        sdo.TOTAL_PRICE_VP = (sdo.PRICE_VP ?? 0) * sdo.AMOUNT;

                        listSereServ.Add(sdo);
                    }
                }

                if (rdo.BillGoods != null && rdo.BillGoods.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("ProcessListSereServ - BillGoods");
                    if (listSereServ == null) listSereServ = new List<Mps000106SDO>();
                    if (listSereServType == null) listSereServType = new List<Mps000106SDO>();
                    var type = listSereServType.FirstOrDefault(o => o.Type == 1);
                    if (type == null)
                    {
                        listSereServType.Add(new Mps000106SDO() { Type = 1, SERVICE_TYPE_NAME = "Dịch vụ" });
                    }
                    var goods = (from r in rdo.BillGoods select new Mps000106SDO(r)).ToList();
                    listSereServ.AddRange(goods);
                }

                listSereServ = listSereServ.Where(o => o.TDL_AMOUNT > 0).ToList();
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
                if (rdo._Transaction != null)
                {
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    string temp = rdo._Transaction.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    //SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_TEXT, amountText));
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));
                    decimal ratio = ((rdo._Transaction.EXEMPTION ?? 0) * 100) / rdo._Transaction.AMOUNT;
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    //Ket Chuyen, Can Thu
                    if (rdo._Transaction.KC_AMOUNT.HasValue)
                    {
                        string kcAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.KC_AMOUNT.Value));
                        SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.KC_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(kcAmountText)));
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._CanThu_Amount));
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._CanThu_Amount)));
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(ctAmountText)));
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.DESCRIPTION, rdo._Transaction.DESCRIPTION));
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.EXEMPTION_REASON, rdo._Transaction.EXEMPTION_REASON));

                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Transaction.CREATE_TIME ?? 0)));

                    string depositAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo.depositAmpount));
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.TU_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo.depositAmpount)));
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.TU_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(depositAmountText)));

                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.HisTreatment, false);
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.RATIO_TEXT, rdo.RatioText));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo._patient, false);
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatientTypeAlter, false);

                    if (rdo.HisTreatment != null)
                    {
                        var dayOftreatment = HIS.Common.Treatment.Calculation.DayOfTreatment(rdo.HisTreatment.IN_TIME, rdo.HisTreatment.OUT_TIME, rdo.HisTreatment.TREATMENT_END_TYPE_ID, rdo.HisTreatment.TREATMENT_RESULT_ID, rdo._PatientTypeAlter != null && rdo._PatientTypeAlter.PATIENT_TYPE_ID == 1 ? HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT : HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);

                        SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.SO_NGAY_DIEU_TRI, dayOftreatment));
                    }

                    if (rdo.ListDepartment != null && rdo.ListDepartment.Count > 0 && rdo.HisTreatment != null)
                    {
                        HIS_DEPARTMENT department = null;
                        if (rdo.HisTreatment.END_DEPARTMENT_ID.HasValue)
                        {
                            department = rdo.ListDepartment.FirstOrDefault(o => o.ID == rdo.HisTreatment.END_DEPARTMENT_ID.Value);
                        }
                        else if (rdo.HisTreatment.LAST_DEPARTMENT_ID.HasValue)
                        {
                            department = rdo.ListDepartment.FirstOrDefault(o => o.ID == rdo.HisTreatment.LAST_DEPARTMENT_ID.Value);
                        }

                        if (department != null)
                        {
                            SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.TREAT_DEPARTMENT_NAME, department.DEPARTMENT_NAME));
                        }
                    }
                }

                if (listSereServ != null && listSereServ.Count > 0)
                {
                    List<string> req = listSereServ.Select(s => s.REQUEST_ROOM_NAME).Distinct().ToList();
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.TOTAL_REQUEST_ROOM_NAME, string.Join(",", req)));
                }

                if (rdo.HisTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000106ExtendSingleKey.IN_TIME_FULL_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo.HisTreatment.IN_TIME)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.HisTreatment != null)
                {
                    if (!String.IsNullOrEmpty(rdo.HisTreatment.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTreatment.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000106ExtendSingleKey.BARCODE_TREATMENT_CODE, barcodeTreatment);
                    }

                    if (!string.IsNullOrWhiteSpace(rdo.HisTreatment.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTreatment.TDL_PATIENT_CODE);
                        barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatient.IncludeLabel = false;
                        barcodePatient.Width = 120;
                        barcodePatient.Height = 40;
                        barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatient.IncludeLabel = true;

                        dicImage.Add(Mps000106ExtendSingleKey.BARCODE_PATIENT_CODE, barcodePatient);
                    }
                }

                if (rdo._Transaction != null)
                {
                    if (!String.IsNullOrWhiteSpace(rdo._Transaction.TRANSACTION_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTransaction = new Inventec.Common.BarcodeLib.Barcode(rdo._Transaction.TRANSACTION_CODE);
                        barcodeTransaction.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTransaction.IncludeLabel = false;
                        barcodeTransaction.Width = 120;
                        barcodeTransaction.Height = 40;
                        barcodeTransaction.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTransaction.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTransaction.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTransaction.IncludeLabel = true;

                        dicImage.Add(Mps000106ExtendSingleKey.BARCODE_TRANSACTION_CODE, barcodeTransaction);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = LogDataTransaction(rdo._Transaction.TREATMENT_CODE, rdo._Transaction.TRANSACTION_CODE, "");
                log += "CanThu: " + rdo._CanThu_Amount + " TamUng: " + rdo.depositAmpount;
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
                if (rdo != null && rdo._Transaction != null)
                    result = String.Format("{0}_{1}_{2}_{3}", this.printTypeCode, rdo._Transaction.TREATMENT_CODE, rdo._Transaction.TRANSACTION_CODE, rdo._Transaction.ACCOUNT_BOOK_CODE);
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
