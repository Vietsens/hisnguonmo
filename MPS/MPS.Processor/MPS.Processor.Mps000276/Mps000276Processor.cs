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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000276.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using MOS.EFMODEL.DataModels;
using System.Text;
using System.Linq;

namespace MPS.Processor.Mps000276
{
    class Mps000276Processor : AbstractProcessor
    {
        Mps000276PDO rdo;

        private List<Mps000276ADO> _ListCashierRoom = new List<Mps000276ADO>();
        private List<Mps000276ADO> _ListSereServ = new List<Mps000276ADO>();
        private List<Mps000276ADO> _ListSereServGroup = new List<Mps000276ADO>();
        private List<Mps000276ADO> _ListSereServKsk = new List<Mps000276ADO>();
        private List<Mps000276ADO> _ListSereServNonKsk = new List<Mps000276ADO>();
        private List<Mps000276ADO> _ListSereServGroupKsk;
        private List<Mps000276ADO> _ListSereServGroupNonKsk;
        private List<ServiceNumOderAdo> _ListServiceNumOder = new List<ServiceNumOderAdo>();
        private List<TreatmentADO> _ListTreatment = new List<TreatmentADO>();
        public Mps000276Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000276PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo != null)
                {
                    if (rdo._vServiceReqs != null && rdo._vServiceReqs.Count > 0)
                    {
                        if (!String.IsNullOrWhiteSpace(rdo._vServiceReqs.First().TDL_PATIENT_CODE))
                        {
                            Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(rdo._vServiceReqs.First().TDL_PATIENT_CODE);
                            barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                            barcode.IncludeLabel = false;
                            barcode.Width = 120;
                            barcode.Height = 40;
                            barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                            barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                            barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                            barcode.IncludeLabel = true;

                            dicImage.Add(Mps000276ExtendSingleKey.BARCODE_PATIENT_CODE, barcode);
                        }

                        if (!String.IsNullOrWhiteSpace(rdo._vServiceReqs.First().TDL_TREATMENT_CODE))
                        {
                            Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(rdo._vServiceReqs.First().TDL_TREATMENT_CODE);
                            barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                            barcode.IncludeLabel = false;
                            barcode.Width = 120;
                            barcode.Height = 40;
                            barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                            barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                            barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                            barcode.IncludeLabel = true;

                            dicImage.Add(Mps000276ExtendSingleKey.BARCODE_TREATMENT_CODE, barcode);
                        }
                    }
                }
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
                SetBarcodeKey();
                SetSingleKeyQrCode();
                SetSingleKey();
                this.ProcessListData();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                rdo._vServiceReqs = rdo._vServiceReqs.OrderBy(o => o.ID).ToList();
                rdo._SereServs = (rdo._SereServs != null && rdo._SereServs.Count > 0) ? rdo._SereServs.OrderBy(p => p.ID).ToList() : rdo._SereServs;
                objectTag.AddObjectData(store, "ServiceReqs", rdo._vServiceReqs);
                objectTag.AddObjectData(store, "CashierRooms", this._ListCashierRoom);
                objectTag.AddObjectData(store, "SereServs", this._ListSereServ);
                objectTag.AddObjectData(store, "SereServsGroup", this._ListSereServGroup);
                objectTag.AddRelationship(store, "SereServsGroup", "SereServs", "ParentServiceCode", "ParentServiceCode");

                objectTag.AddObjectData(store, "Treatments", _ListTreatment ?? new List<TreatmentADO>());
                //objectTag.AddObjectData(store, "PatientTypeAlters", rdo._PatientTypeAlterList ?? new List<V_HIS_PATIENT_TYPE_ALTER>());
                objectTag.AddObjectData(store, "SereServKsks", this._ListSereServKsk);
                objectTag.AddObjectData(store, "SereServNonKsks", this._ListSereServNonKsk);

                objectTag.AddObjectData(store, "SereServGroupKsks", this._ListSereServGroupKsk);
                objectTag.AddObjectData(store, "SereServGroupNonKsks", this._ListSereServGroupNonKsk);

                objectTag.AddRelationship(store, "Treatments", "SereServKsks", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "Treatments", "SereServNonKsks", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "Treatments", "SereServGroupKsks", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "Treatments", "SereServGroupNonKsks", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "SereServGroupKsks", "SereServKsks", "ParentServiceCode", "ParentServiceCode");
                objectTag.AddRelationship(store, "SereServGroupNonKsks", "SereServNonKsks", "ParentServiceCode", "ParentServiceCode");

                objectTag.AddObjectData(store, "ServiceNumOrder", this._ListServiceNumOder.Distinct().ToList());

                objectTag.AddRelationship(store, "CashierRooms", "SereServs", "CashierRoomId", "CashierRoomId");
                objectTag.AddRelationship(store, "CashierRooms", "SereServKsks", "CashierRoomId", "CashierRoomId");
                objectTag.AddRelationship(store, "CashierRooms", "SereServNonKsks", "CashierRoomId", "CashierRoomId");
                objectTag.AddRelationship(store, "ServiceNumOrder", "SereServs", "SERVICE_CODE", "ParentServiceCode");
                objectTag.AddRelationship(store, "ServiceNumOrder", "SereServKsks", "SERVICE_CODE", "ParentServiceCode");
                objectTag.AddRelationship(store, "ServiceNumOrder", "SereServNonKsks", "SERVICE_CODE", "ParentServiceCode");

                objectTag.SetUserFunction(store, "FuncPerviousRowNum", new TFlexCelUFPerviousRowNum());
                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData13", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData14", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData21", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData22", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData23", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData24", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData31", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData32", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData33", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData34", new CalculateMergerData());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessListData()
        {
            try
            {
                if (rdo._SereServs == null || rdo._SereServs.Count <= 0)
                {
                    return;
                }
                var Groups = rdo._vServiceReqs.GroupBy(g => g.CASHIER_ROOM_ID ?? 0).ToList();
                var ServiceTypeS = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                long maxStt = (rdo._ServiceNumOrder != null && rdo._ServiceNumOrder.Count > 0) ? rdo._ServiceNumOrder.Max(m => m.NUM_ORDER) : 0;
                foreach (var group in Groups)
                {
                    List<V_HIS_SERVICE_REQ> list = group.ToList();
                    Mps000276ADO parentAdo = new Mps000276ADO();
                    parentAdo.CashierRoomId = group.Key;
                    V_HIS_CASHIER_ROOM cashierRoom = rdo._CashierRooms != null ? rdo._CashierRooms.FirstOrDefault(o => o.ID == group.Key) : null;
                    if (cashierRoom != null)
                    {
                        parentAdo.CashierRoomCode = cashierRoom.CASHIER_ROOM_CODE;
                        parentAdo.CashierRoomName = cashierRoom.CASHIER_ROOM_NAME;
                        parentAdo.CashierRoomAddress = cashierRoom.ADDRESS;
                    }
                    else
                    {
                        parentAdo.CashierRoomCode = String.Format("CashierRoomCode_{0}", group.Key);
                    }

                    // Gom theo SERVICE_REQ_ID: mỗi y lệnh xử lý thành 1 block riêng,
                    // không trộn lẫn dịch vụ giữa các y lệnh
                    var lstSereServByReq = rdo._SereServs
                        .Where(o => list.Any(a => a.ID == o.SERVICE_REQ_ID))
                        .GroupBy(o => o.SERVICE_REQ_ID)
                        .ToList();
                    long minStt = maxStt;
                    foreach (var reqGroup in lstSereServByReq)
                    foreach (HIS_SERE_SERV ss in reqGroup)
                    {
                        V_HIS_SERVICE_REQ sr = list.FirstOrDefault(o => o.ID == ss.SERVICE_REQ_ID);
                        V_HIS_SERVICE service = rdo._Services.FirstOrDefault(o => o.ID == ss.SERVICE_ID);
                        V_HIS_SERVICE parent = null;
                        HIS_SERE_SERV_EXT ext = rdo._ext != null ? rdo._ext.FirstOrDefault(ex => ex.SERE_SERV_ID == ss.ID) : null;
                        V_HIS_ROOM resultRoom = null;
                        V_HIS_DESK resultDesk = null;
                        ServiceNumOderAdo serviceNumOderAdo = null;

                        HIS_SERVICE_NUM_ORDER exeNumOrder = null;

                        if (service.PARENT_ID.HasValue)
                        {
                            parent = rdo._Services.FirstOrDefault(o => o.ID == service.PARENT_ID.Value);
                            exeNumOrder = rdo._ServiceNumOrder != null ? rdo._ServiceNumOrder.FirstOrDefault(o => o.REQUEST_ROOM_ID == sr.REQUEST_ROOM_ID && o.SERVICE_ID == service.PARENT_ID.Value) : null;
                        }

                        if (sr.RESULT_DESK_ID.HasValue)
                        {
                            resultDesk = rdo._Desks != null ? rdo._Desks.FirstOrDefault(o => o.ID == sr.RESULT_DESK_ID.Value) : null;
                        }
                        if (sr.RESULT_ROOM_ID.HasValue)
                        {
                            resultRoom = rdo._Rooms != null ? rdo._Rooms.FirstOrDefault(o => o.ID == sr.RESULT_ROOM_ID.Value) : null;
                        }

                        Mps000276ADO ado = new Mps000276ADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000276ADO>(ado, ss);
                        ado.CashierRoomAddress = parentAdo.CashierRoomAddress;
                        ado.CashierRoomCode = parentAdo.CashierRoomCode;
                        ado.CashierRoomId = parentAdo.CashierRoomId;
                        ado.CashierRoomName = parentAdo.CashierRoomName;

                        ado.ExecuteRoomAddress = sr.EXECUTE_ROOM_ADDRESS;
                        ado.ExecuteRoomCode = sr.EXECUTE_ROOM_CODE;
                        ado.ExecuteRoomId = sr.EXECUTE_ROOM_ID;
                        ado.ExecuteRoomName = sr.EXECUTE_ROOM_NAME;

                        ado.InstructionDate = sr.INTRUCTION_DATE;
                        ado.InstructionTime = sr.INTRUCTION_TIME;
                        ado.IsResultInDiffDay = (sr.IS_RESULT_IN_DIFF_DAY == (short)1);

                        ado.CallSampleOrder = sr.CALL_SAMPLE_ORDER;
                        ado.SampleRoomCode = sr.SAMPLE_ROOM_CODE;
                        ado.SampleRoomName = sr.SAMPLE_ROOM_NAME;
                        ado.ASSIGN_TURN_CODE = sr.ASSIGN_TURN_CODE;
                        ado.Note = !string.IsNullOrWhiteSpace(ext?.INSTRUCTION_NOTE) ? ext.INSTRUCTION_NOTE : "";

                        var serviceType = ServiceTypeS.FirstOrDefault(o => o.ID == ss.TDL_SERVICE_TYPE_ID);
                        if (serviceType != null)
                        {
                            ado.ServiceTypeNumOrder = serviceType.NUM_ORDER;
                        }
                        if (parent != null)
                        {

                            ado.ParentServiceCode = parent.SERVICE_CODE;
                            ado.ParentServiceId = parent.ID;
                            ado.ParentServiceNumOrder = parent.NUM_ORDER;
                            ado.ParentServiceName = parent.SERVICE_NAME;
                            if (exeNumOrder != null)
                            {
                                serviceNumOderAdo = new ServiceNumOderAdo();
                                serviceNumOderAdo.SERVICE_ID = parent.ID;
                                serviceNumOderAdo.SERVICE_NAME = parent.SERVICE_NAME;
                                serviceNumOderAdo.SERVICE_CODE = parent.SERVICE_CODE;
                                serviceNumOderAdo.SERVICE_TYPE_CODE = parent.SERVICE_TYPE_CODE;
                                serviceNumOderAdo.ASSIGN_TURN_CODE = sr.ASSIGN_TURN_CODE;
                                serviceNumOderAdo.SERVICE_REQ_CODE = sr.SERVICE_REQ_CODE;
                            }
                        }
                        else
                        {
                            if (service != null)
                            {
                                ado.ParentServiceNumOrder = ado.ServiceTypeNumOrder;
                                ado.ParentServiceCode = service.SERVICE_TYPE_CODE;
                                ado.ParentServiceName = service.SERVICE_TYPE_NAME;
                            }
                        }
                        ado.RequestRoomAddress = sr.REQUEST_ROOM_ADDRESS;
                        ado.RequestRoomCode = sr.REQUEST_ROOM_CODE;
                        ado.RequestRoomId = sr.REQUEST_ROOM_ID;
                        ado.RequestRoomName = sr.REQUEST_ROOM_NAME;

                        if (resultRoom != null)
                        {
                            ado.ResultRoomAddress = resultRoom.ADDRESS;
                            ado.ResultRoomCode = resultRoom.ROOM_CODE;
                            ado.ResultRoomId = resultRoom.ID;
                            ado.ResultRoomName = resultRoom.ROOM_NAME;
                        }
                        else if (resultDesk != null)
                        {
                            ado.ResultDeskCode = resultDesk.DESK_CODE;
                            ado.ResultDeskId = resultDesk.ID;
                            ado.ResultDeskName = resultDesk.DESK_NAME;
                        }

                        ado.SereServId = ss.ID;
                        ado.ServiceCode = ss.TDL_SERVICE_CODE;
                        ado.ServiceId = ss.SERVICE_ID;
                        ado.ServiceName = ss.TDL_SERVICE_NAME;
                        ado.ServiceReqId = sr.ID;
                        ado.ServiceReqNumOrder = sr.NUM_ORDER ?? 99999999;
                        ado.ServiceTypeId = ss.TDL_SERVICE_TYPE_ID;
                        if (service != null)
                        {
                            ado.ServiceNumOrder = service.NUM_ORDER;
                            ado.ServiceTypeCode = service.SERVICE_TYPE_CODE;
                            ado.ServiceTypeName = service.SERVICE_TYPE_NAME;
                        }

                        if (exeNumOrder != null)
                        {
                            ado.ExecuteNumOrder = exeNumOrder.NUM_ORDER;
                            if (serviceNumOderAdo != null) serviceNumOderAdo.NUM_ORDER = exeNumOrder.NUM_ORDER;
                            minStt = (exeNumOrder.NUM_ORDER < minStt) ? exeNumOrder.NUM_ORDER : minStt;
                        }
                        else
                        {
                            ado.ExecuteNumOrder = maxStt++;
                        }

                        if (serviceNumOderAdo != null)
                        {
                            serviceNumOderAdo.CALL_SAMPLE_ORDER = !string.IsNullOrWhiteSpace(ado.CallSampleOrder.ToString()) ? ado.CallSampleOrder : null;

                            serviceNumOderAdo.EXECUTE_ROOM_NAME = ado.ExecuteRoomName;
                        }

                        //serviceNumOderAdo.CALL_SAMPLE_ORDER = !string.IsNullOrWhiteSpace((ado.CallSampleOrder).ToString()) ? ado.CallSampleOrder : null;
                        this._ListSereServ.Add(ado);
                        if (serviceNumOderAdo != null && (_ListServiceNumOder.Count() == 0 || !_ListServiceNumOder.Exists(o => o.SERVICE_CODE == serviceNumOderAdo.SERVICE_CODE)))
                            this._ListServiceNumOder.Add(serviceNumOderAdo);
                    }
                    parentAdo.ExecuteNumOrder = minStt;
                    this._ListCashierRoom.Add(parentAdo);
                }

                this._ListCashierRoom = this._ListCashierRoom.OrderBy(o => o.ExecuteNumOrder).ToList();
                // Giữ mỗi y lệnh thành 1 block liền mạch (không bị trộn theo nhóm cha
                // chung ExecuteNumOrder); thứ tự giữa các y lệnh theo ExecuteNumOrder
                // nhỏ nhất của y lệnh đó, trong 1 y lệnh thì theo ExecuteNumOrder
                var reqMinOrder = this._ListSereServ
                    .GroupBy(t => t.ServiceReqId)
                    .ToDictionary(g => g.Key, g => g.Min(x => x.ExecuteNumOrder ?? 0));
                this._ListSereServ = this._ListSereServ
                    .OrderBy(t => reqMinOrder[t.ServiceReqId])
                    .ThenBy(t => t.ServiceReqId)
                    .ThenBy(t => t.ExecuteNumOrder)
                    .ToList();

                int currStt = 0;
                foreach (var item in this._ListSereServ)
                {
                    item.RowNum = currStt;
                    currStt++;
                }

                // minhnq
                if (_ListServiceNumOder.Count() > 0)
                {
                    this._ListServiceNumOder = this._ListServiceNumOder.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.SERVICE_ID).ToList();
                    Dictionary<long, int> numRanks = _ListServiceNumOder
                        .GroupBy(i => i.NUM_ORDER)
                        .OrderBy(g => g.First().NUM_ORDER)
                        .Select((g, index) => new { num = g.First().NUM_ORDER, rank = index + 1 })
                        .ToDictionary(x => x.num, x => x.rank);

                    foreach (var item in _ListServiceNumOder)
                    {
                        item.SEQUENCE = numRanks.FirstOrDefault(o => o.Key == item.NUM_ORDER).Value.ToString();
                    }
                }

                // Tách danh sách theo KSK
                SplitSereServByKsk();

                // Gắn dữ liệu RowNum, NextRowNum cho _ListTreatment
                ProcessTreatmentRowNum();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SplitSereServByKsk()
        {
            try
            {
                if (this._ListSereServ == null || this._ListSereServ.Count <= 0)
                    return;

                // Lấy danh sách SERVICE_ID thuộc KSK dựa vào Treatment.TDL_KSK_CONTRACT_ID
                List<long> kskServiceIds = new List<long>();
                if (rdo._TreatmentList != null && rdo._TreatmentList.Count > 0
                    && rdo._TreatmentList.Exists(o=>o.TDL_KSK_CONTRACT_ID.HasValue)
                    && rdo._Ksk != null && rdo._Ksk.Count > 0
                    && rdo._KskServices != null && rdo._KskServices.Count > 0)
                {
                    var kskIds = rdo._Ksk
                        .Where(k => rdo._TreatmentList.Exists(o=>(o.TDL_KSK_CONTRACT_ID ?? 0) == k.KSK_CONTRACT_ID))
                        .Select(k => k.ID)
                        .ToList();

                    kskServiceIds = rdo._KskServices
                        .Where(ks => kskIds.Contains(ks.KSK_ID))
                        .Select(ks => ks.SERVICE_ID)
                        .Distinct()
                        .ToList();
                }

                this._ListSereServKsk = this._ListSereServ.Where(o => kskServiceIds.Contains(o.SERVICE_ID)).ToList();
                this._ListSereServNonKsk = this._ListSereServ.Where(o => !kskServiceIds.Contains(o.SERVICE_ID)).ToList();

                // Gom nhóm theo parent service và thêm parent vào danh sách
                this._ListSereServGroup = GroupByParentService(this._ListSereServ);
                this._ListSereServGroupKsk = GroupByParentService(this._ListSereServKsk);
                this._ListSereServGroupNonKsk = GroupByParentService(this._ListSereServNonKsk);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessTreatmentRowNum()
        {
            try
            {
                if (rdo._TreatmentList == null || rdo._TreatmentList.Count <= 0)
                    return;

                _ListTreatment = new List<TreatmentADO>();
                foreach (var treatment in rdo._TreatmentList)
                {
                    TreatmentADO ado = new TreatmentADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentADO>(ado, treatment);
                    _ListTreatment.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<Mps000276ADO> GroupByParentService(List<Mps000276ADO> listSereServ)
        {
            try
            {
                if (listSereServ == null || listSereServ.Count <= 0)
                    return listSereServ;

                List<Mps000276ADO> result = new List<Mps000276ADO>();

                // Nhóm theo ParentServiceCode + y lệnh (ServiceReqId) để 2 y lệnh
                // cùng nhóm cha không bị gom chung vào một block
                var groupedByParent = listSereServ
                    .Where(o => !string.IsNullOrWhiteSpace(o.ParentServiceCode))
                    .GroupBy(o => new { o.ParentServiceCode,o.TDL_TREATMENT_ID,o.ServiceReqId })
                    .ToList();

                // Danh sách các dịch vụ không có parent - sẽ gom theo ServiceTypeName
                var noParentServices = listSereServ
                    .Where(o => string.IsNullOrWhiteSpace(o.ParentServiceCode))
                    .ToList();

                // Xử lý các dịch vụ không có parent - gom theo ServiceTypeName + y lệnh

                var groupedByServiceType = noParentServices
                    .GroupBy(o => new { o.ServiceTypeName,o.TDL_TREATMENT_ID,o.ServiceReqId })
                    .ToList();

                foreach (var group in groupedByServiceType)
                {
                    var services = group.ToList();

                    var firstService = services.First();
                    Mps000276ADO serviceTypeAdo = new Mps000276ADO();
                    serviceTypeAdo.TDL_TREATMENT_ID = firstService.TDL_TREATMENT_ID;
                    serviceTypeAdo.ServiceReqId = firstService.ServiceReqId;
                    serviceTypeAdo.ParentServiceName = firstService.ServiceTypeName;
                    serviceTypeAdo.ParentServiceCode = firstService.ServiceTypeCode;
                    serviceTypeAdo.ParentServiceNumOrder = firstService.ServiceNumOrder;
                    serviceTypeAdo.ServiceTypeName = firstService.ServiceTypeName;
                    serviceTypeAdo.ServiceTypeCode = firstService.ServiceTypeCode;

                    serviceTypeAdo.ExecuteRoomId = firstService.ExecuteRoomId;
                    serviceTypeAdo.ExecuteRoomCode = firstService.ExecuteRoomCode;
                    serviceTypeAdo.ExecuteRoomName = firstService.ExecuteRoomName;
                    serviceTypeAdo.ExecuteRoomAddress = firstService.ExecuteRoomAddress;

                    serviceTypeAdo.CashierRoomId = firstService.CashierRoomId;
                    serviceTypeAdo.CashierRoomCode = firstService.CashierRoomCode;
                    serviceTypeAdo.CashierRoomName = firstService.CashierRoomName;
                    serviceTypeAdo.CashierRoomAddress = firstService.CashierRoomAddress;

                    serviceTypeAdo.RequestRoomId = firstService.RequestRoomId;
                    serviceTypeAdo.RequestRoomCode = firstService.RequestRoomCode;
                    serviceTypeAdo.RequestRoomName = firstService.RequestRoomName;
                    serviceTypeAdo.RequestRoomAddress = firstService.RequestRoomAddress;

                    serviceTypeAdo.InstructionDate = firstService.InstructionDate;
                    serviceTypeAdo.InstructionTime = firstService.InstructionTime;
                    serviceTypeAdo.ExecuteNumOrder = firstService.ExecuteNumOrder;
                    serviceTypeAdo.ServiceReqNumOrder = firstService.ServiceReqNumOrder;

                    // Thêm header ServiceType vào đầu nhóm
                    result.Add(serviceTypeAdo);

                }

                // Xử lý từng nhóm parent
                foreach (var group in groupedByParent)
                {
                    var childServices = group.ToList();

                    // Nếu cùng 1 phòng xử lý, tạo ADO cho parent service
                    var firstChild = childServices.First();
                    Mps000276ADO parentAdo = new Mps000276ADO();

                    parentAdo.TDL_TREATMENT_ID = firstChild.TDL_TREATMENT_ID;
                    parentAdo.ServiceReqId = firstChild.ServiceReqId;
                    parentAdo.ParentServiceCode = firstChild.ParentServiceCode;
                    parentAdo.ParentServiceId = firstChild.ParentServiceId;
                    parentAdo.ParentServiceName = firstChild.ParentServiceName;
                    parentAdo.ParentServiceNumOrder = firstChild.ParentServiceNumOrder;
                    parentAdo.ServiceCode = firstChild.ParentServiceCode;
                    parentAdo.ServiceName = firstChild.ParentServiceName;
                    parentAdo.ServiceId = firstChild.ParentServiceId ?? 0;

                    parentAdo.ExecuteRoomId = firstChild.ExecuteRoomId;
                    parentAdo.ExecuteRoomCode = firstChild.ExecuteRoomCode;
                    parentAdo.ExecuteRoomName = firstChild.ExecuteRoomName;
                    parentAdo.ExecuteRoomAddress = firstChild.ExecuteRoomAddress;

                    parentAdo.CashierRoomId = firstChild.CashierRoomId;
                    parentAdo.CashierRoomCode = firstChild.CashierRoomCode;
                    parentAdo.CashierRoomName = firstChild.CashierRoomName;
                    parentAdo.CashierRoomAddress = firstChild.CashierRoomAddress;

                    parentAdo.RequestRoomId = firstChild.RequestRoomId;
                    parentAdo.RequestRoomCode = firstChild.RequestRoomCode;
                    parentAdo.RequestRoomName = firstChild.RequestRoomName;
                    parentAdo.RequestRoomAddress = firstChild.RequestRoomAddress;

                    parentAdo.InstructionDate = firstChild.InstructionDate;
                    parentAdo.InstructionTime = firstChild.InstructionTime;
                    parentAdo.ExecuteNumOrder = firstChild.ExecuteNumOrder;
                    parentAdo.ServiceReqNumOrder = firstChild.ServiceReqNumOrder;

                    // Thêm parent vào đầu nhóm
                    result.Add(parentAdo);

                }

                // Sắp xếp lại theo thứ tự
                result = result.OrderBy(t => t.ParentServiceNumOrder).ThenBy(t => t.ServiceNumOrder).ToList();

                // Cập nhật lại RowNum
                int currStt = 0;
                foreach (var item in result)
                {
                    item.RowNum = currStt;
                    currStt++;
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return listSereServ;
            }
        }
        private void SetSingleKeyQrCode()
        {
            try
            {
                if (rdo.transReq != null && rdo.lstConfig != null && rdo.lstConfig.Count > 0)
                {
                    var data = HIS.Desktop.Common.BankQrCode.QrCodeProcessor.CreateQrImage(rdo.transReq, rdo.lstConfig);
                    foreach (var item in data)
                    {
                        SetSingleKey(new KeyValue(item.Key, item.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo._Treatment);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatientTypeAlter, false);
                AddObjectKeyIntoListkey<V_HIS_ROOM>(rdo._vHisRoom, false);
                if (rdo._vServiceReqs != null && rdo._vServiceReqs.Count > 0)
                {
                    this.SetSingleKey(new KeyValue(Mps000276ExtendSingleKey.REQUEST_ROOM_ADDRESS, rdo._vServiceReqs.FirstOrDefault().REQUEST_ROOM_ADDRESS));
                    this.SetSingleKey(new KeyValue("REQUEST_DEPARTMENT", rdo._vServiceReqs.FirstOrDefault().REQUEST_DEPARTMENT_CODE));
                    this.SetSingleKey(new KeyValue("REQUEST_ROOM", rdo._vServiceReqs.FirstOrDefault().REQUEST_ROOM_CODE));
                    this.SetSingleKey(new KeyValue("INTRUCTION_TIME", rdo._SereServs.FirstOrDefault().TDL_INTRUCTION_TIME));
                    DateTime dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo._vServiceReqs.FirstOrDefault().INTRUCTION_TIME).Value;
                    switch (dt.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            this.SetSingleKey(new KeyValue(Mps000276ExtendSingleKey.INTRUCTION_DATE_DAY_OF_WEEK, "Thứ sáu"));
                            break;
                        case DayOfWeek.Monday:
                            this.SetSingleKey(new KeyValue(Mps000276ExtendSingleKey.INTRUCTION_DATE_DAY_OF_WEEK, "Thứ hai"));
                            break;
                        case DayOfWeek.Saturday:
                            this.SetSingleKey(new KeyValue(Mps000276ExtendSingleKey.INTRUCTION_DATE_DAY_OF_WEEK, "Thứ bảy"));
                            break;
                        case DayOfWeek.Sunday:
                            this.SetSingleKey(new KeyValue(Mps000276ExtendSingleKey.INTRUCTION_DATE_DAY_OF_WEEK, "Chủ nhật"));
                            break;
                        case DayOfWeek.Thursday:
                            this.SetSingleKey(new KeyValue(Mps000276ExtendSingleKey.INTRUCTION_DATE_DAY_OF_WEEK, "Thứ năm"));
                            break;
                        case DayOfWeek.Tuesday:
                            this.SetSingleKey(new KeyValue(Mps000276ExtendSingleKey.INTRUCTION_DATE_DAY_OF_WEEK, "Thứ ba"));
                            break;
                        case DayOfWeek.Wednesday:
                            this.SetSingleKey(new KeyValue(Mps000276ExtendSingleKey.INTRUCTION_DATE_DAY_OF_WEEK, "Thứ tư"));
                            break;
                        default:
                            break;
                    }

                    int isOnlyCdha = (rdo._vServiceReqs.Any(a => a.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA) && !rdo._vServiceReqs.Any(a => a.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)) ? 1 : 0;
                    this.SetSingleKey(new KeyValue(Mps000276ExtendSingleKey.IS_ONLY_CDHA, isOnlyCdha));
                }
                if (rdo.transReq != null)
                    SetSingleKey(new KeyValue(Mps000276ExtendSingleKey.PAYMENT_AMOUNT, rdo.transReq.AMOUNT));

                if (rdo.aDO != null)
                {
                    SetSingleKey(new KeyValue("WEIGHT", rdo.aDO.WEIGHT));
                    SetSingleKey(new KeyValue("REQUEST_DOCTOR_MOBILE", rdo.aDO.REQUEST_DOCTOR_MOBILE));
                    SetSingleKey(new KeyValue("EXECUTE_DOCTOR_MOBILE", rdo.aDO.EXECUTE_DOCTOR_MOBILE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class TFlexCelUFPerviousRowNum : TFlexCelUserFunction
        {
            public TFlexCelUFPerviousRowNum()
            {
            }
            public override object Evaluate(object[] parameters)
            {
                int result = 0;
                try
                {
                    if (parameters == null || parameters.Length < 1)
                        throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                    int rownumber = Convert.ToInt32(parameters[0]);
                    result = (rownumber - 1);
                }
                catch (Exception ex)
                {
                    result = 0;
                    LogSystem.Debug(ex);
                }

                return result;
            }
        }

        class CalculateMergerData : TFlexCelUserFunction
        {
            string sampleRoom = null;

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    string serviceName = parameters[0].ToString();
                    if (serviceName != null)
                    {
                        if (this.sampleRoom == serviceName)
                        {
                            return true;
                        }
                        else
                        {
                            this.sampleRoom = serviceName;
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    result = false;
                }
                return result;
            }
        }
    }
}
