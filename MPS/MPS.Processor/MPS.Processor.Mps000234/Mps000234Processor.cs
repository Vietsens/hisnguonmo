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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000234.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000234
{
    public class Mps000234Processor : AbstractProcessor
    {
        List<ExpMestsGroupADO> ExpMests { get; set; }

        List<ExpMestsGroupADO> GroupByStock = new List<ExpMestsGroupADO>();
        List<ExpMestsGroupADO> GroupByType = new List<ExpMestsGroupADO>();
        List<ExpMestsGroupADO> GroupByReqRoom = new List<ExpMestsGroupADO>();
        List<ExpMestsGroupADO> GroupDetail = new List<ExpMestsGroupADO>();
        List<ExpMestsGroupADO> GroupByTypeStock = new List<ExpMestsGroupADO>();
        List<ExpMestsGroupADO> GroupByTypeStockRoom = new List<ExpMestsGroupADO>();
        List<ExpMestsGroupADO> GroupByTypeStockRoomDetail = new List<ExpMestsGroupADO>();
        Mps000234PDO rdo;
        public Mps000234Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000234PDO)rdoBase;
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
                this.ExpMestMedicineGroup();
                ProcessBarcodeKey();
                ProcessSingleKey();
                this.SetSignatureKeyImageByCFG();
                ProcessListMedicineGroupByStock();

                if (store.ReadTemplate(System.IO.Path.GetFullPath(fileName)))
                {
                    singleTag.ProcessData(store, singleValueDictionary);
                    barCodeTag.ProcessData(store, dicImage);

                    if (this.ExpMests == null)
                        return false;

                    this.ExpMests = this.ExpMests.OrderBy(o => o.ID).ToList();
                    SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                    objectTag.AddObjectData(store, "MedicineExpmest", rdo.expMestMedicines);
                    objectTag.AddObjectData(store, "expMestMedicineIncludeOutStock", rdo.expMestMedicineIncludeOutStock);
                    objectTag.AddObjectData(store, "ExpMest", this.ExpMests);
                    objectTag.AddRelationship(store, "ExpMest", "MedicineExpmest", "EXP_MEST_ID", "EXP_MEST_ID");
                    objectTag.AddRelationship(store, "ExpMest", "expMestMedicineIncludeOutStock", "EXP_MEST_ID", "EXP_MEST_ID");

                    objectTag.AddObjectData(store, "MediMateDetail", this.GroupDetail);
                    objectTag.AddObjectData(store, "GroupByReqRoom", this.GroupByReqRoom);
                    objectTag.AddObjectData(store, "GroupByType", this.GroupByType);
                    objectTag.AddObjectData(store, "GroupByStock", this.GroupByStock);
                    objectTag.AddRelationship(store, "GroupByStock", "GroupByType", "MEDI_STOCK_ID", "MEDI_STOCK_ID");
                    objectTag.AddRelationship(store, "GroupByStock", "GroupByReqRoom", "MEDI_STOCK_ID", "MEDI_STOCK_ID");
                    objectTag.AddRelationship(store, "GroupByStock", "MediMateDetail", "MEDI_STOCK_ID", "MEDI_STOCK_ID");
                    objectTag.AddRelationship(store, "GroupByType", "GroupByReqRoom", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                    objectTag.AddRelationship(store, "GroupByType", "MediMateDetail", "PATIENT_TYPE_ID", "PATIENT_TYPE_ID");
                    objectTag.AddRelationship(store, "GroupByReqRoom", "MediMateDetail", "REQ_ROOM_ID", "REQ_ROOM_ID");

                    // Gom nhom xuat thuoc theo loai kho va phong chi dinh
                    objectTag.AddObjectData(store, "MediStockTypeRoomDetail", this.GroupByTypeStockRoomDetail);
                    objectTag.AddObjectData(store, "RequestRoom", this.GroupByTypeStockRoom);
                    objectTag.AddObjectData(store, "MediStockType", this.GroupByTypeStock);
                    objectTag.AddRelationship(store, "MediStockType", "RequestRoom", "MEDI_STOCK_TYPE", "MEDI_STOCK_TYPE");
                    objectTag.AddRelationship(store, "MediStockType", "MediStockTypeRoomDetail", "MEDI_STOCK_TYPE", "MEDI_STOCK_TYPE");
                    objectTag.AddRelationship(store, "RequestRoom", "MediStockTypeRoomDetail", "REQUEST_ROOM_CODE", "REQUEST_ROOM_CODE");
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessListMedicineGroupByStock()
        {
            try
            {
                if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                {
                    var detail = new List<ExpMestsGroupADO>();
                    GroupDetail = new List<ExpMestsGroupADO>();
                    GroupByType = new List<ExpMestsGroupADO>();
                    GroupByStock = new List<ExpMestsGroupADO>();
                    GroupByReqRoom = new List<ExpMestsGroupADO>();

                    //expMestMedicines ko có thông tin MEDI_STOCK_ID do map từ HIS_MEDICINE_TYPE
                    foreach (var item in rdo.expMestMedicines)
                    {
                        ExpMestsGroupADO ado = new ExpMestsGroupADO(item);
                        if (rdo.ListServiceReq != null && rdo.ListServiceReq.Count > 0)
                        {
                            var serviceReq = rdo.ListServiceReq.FirstOrDefault(o => o.ID == item.TDL_SERVICE_REQ_ID);
                            if (serviceReq != null)
                            {
                                ado.REQUEST_LOGINNAME = serviceReq.REQUEST_LOGINNAME;
                                ado.REQUEST_USER_TITLE = serviceReq.REQUEST_USER_TITLE;
                                ado.REQUEST_USERNAME = serviceReq.REQUEST_USERNAME;
                                ado.ICD_SUB_CODE = serviceReq.ICD_SUB_CODE;
                                ado.ICD_TEXT = serviceReq.ICD_TEXT;

                                if (rdo.ListAcsUser != null && rdo.ListAcsUser.Count > 0)
                                {
                                    var user = rdo.ListAcsUser.FirstOrDefault(o => o.LOGINNAME == serviceReq.REQUEST_LOGINNAME);
                                    if (user != null)
                                    {
                                        ado.MOBILE = user.MOBILE;
                                    }
                                }

                                if (rdo.ListRoom != null && rdo.ListRoom.Count > 0)
                                {
                                    var room = rdo.ListRoom.FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                                    if (room != null)
                                    {
                                        ado.REQUEST_ROOM_CODE = room.ROOM_CODE;
                                        ado.REQUEST_ROOM_NAME = room.ROOM_NAME;
                                    }
                                }

                                if (rdo.ListRoom != null && rdo.ListRoom.Count > 0)
                                {
                                    var room = rdo.ListRoom.FirstOrDefault(o => o.ID == serviceReq.EXECUTE_ROOM_ID);
                                    if (room != null)
                                    {
                                        ado.MEDI_STOCK_CODE = room.ROOM_CODE;
                                        ado.MEDI_STOCK_ID = room.ID;
                                        ado.MEDI_STOCK_NAME = room.ROOM_NAME;
                                    }
                                }
                            }
                        }

                        detail.Add(ado);
                    }
                    //
                    string totalICDSubCode = "";
                    string totalICDText = "";
                    var listICDSubCode = detail.Select(o => o.ICD_SUB_CODE).ToList() ?? new List<string>();
                    var listICDText = detail.Select(o => o.ICD_TEXT).ToList() ?? new List<string>();
                    List<string> listICDSubCode_Seperate = SeperateListString(listICDSubCode);
                    List<string> listICDText_Seperate = SeperateListString(listICDText);

                    totalICDSubCode = string.Join("; ", listICDSubCode_Seperate);
                    totalICDText = string.Join("; ", listICDText_Seperate);
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.TOTAL_ICD_SUB_CODE, totalICDSubCode));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.TOTAL_ICD_TEXT, totalICDText));

                    var grDetail = detail.GroupBy(o => new { o.MEDI_STOCK_ID, o.PATIENT_TYPE_ID, o.MEDICINE_TYPE_ID, o.PRICE, o.REQ_ROOM_ID }).ToList();
                    foreach (var item in grDetail)
                    {
                        ExpMestsGroupADO ado = new ExpMestsGroupADO();

                        Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestsGroupADO>(ado, item.First());
                        ado.AMOUNT = item.Sum(s => s.AMOUNT);
                        ado.PRES_AMOUNT = item.Sum(s => s.PRES_AMOUNT ?? 0);

                        GroupDetail.Add(ado);
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.expMestMedicines), rdo.expMestMedicines));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GroupDetail), GroupDetail));

                    var groupbByTypeNStockReq = GroupDetail.GroupBy(o => new { o.MEDI_STOCK_ID, o.PATIENT_TYPE_ID, o.REQ_ROOM_ID }).ToList();
                    foreach (var item in groupbByTypeNStockReq)
                    {
                        ExpMestsGroupADO ado = new ExpMestsGroupADO();

                        Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestsGroupADO>(ado, item.First());
                        ado.AMOUNT = item.Sum(s => s.AMOUNT);
                        ado.PRES_AMOUNT = item.Sum(s => s.PRES_AMOUNT ?? 0);

                        GroupByReqRoom.Add(ado);
                    }

                    var groupbByTypeNStock = GroupByReqRoom.GroupBy(o => new { o.MEDI_STOCK_ID, o.PATIENT_TYPE_ID }).ToList();
                    foreach (var item in groupbByTypeNStock)
                    {
                        ExpMestsGroupADO ado = new ExpMestsGroupADO();

                        Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestsGroupADO>(ado, item.First());
                        ado.AMOUNT = item.Sum(s => s.AMOUNT);
                        ado.PRES_AMOUNT = item.Sum(s => s.PRES_AMOUNT ?? 0);

                        GroupByType.Add(ado);
                    }

                    var groupbByStock = GroupByType.GroupBy(o => new { o.MEDI_STOCK_ID }).ToList();
                    foreach (var item in groupbByStock)
                    {
                        ExpMestsGroupADO ado = new ExpMestsGroupADO();

                        Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestsGroupADO>(ado, item.First());
                        ado.AMOUNT = item.Sum(s => s.AMOUNT);
                        ado.PRES_AMOUNT = item.Sum(s => s.PRES_AMOUNT ?? 0);

                        GroupByStock.Add(ado);
                    }
                }
                // Xu gom thuoc theo loai thuoc trong kho, thuoc ngoai kho, thuoc khac
                if (rdo.expMestMedicineIncludeOutStock != null && rdo.expMestMedicineIncludeOutStock.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("rdo.expMestMedicineIncludeOutStock", rdo.expMestMedicineIncludeOutStock));
                    foreach (var item in rdo.expMestMedicineIncludeOutStock)
                    {
                        ExpMestsGroupADO ado = new ExpMestsGroupADO(item);
                        ado.MEDI_STOCK_TYPE = ado.GetTypeGroup();
                        if (rdo.ListRoom != null && rdo.ListRoom.Count > 0)
                        {
                            var reqRoom = rdo.ListRoom.FirstOrDefault(o => o.ID == item.REQ_ROOM_ID);
                            if (reqRoom != null)
                            {
                                ado.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                                ado.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                            }
                        }

                        var stock = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == item.MEDI_STOCK_ID);
                        if (stock != null)
                        {
                            ado.MEDI_STOCK_ID = stock.ID;
                            ado.MEDI_STOCK_CODE = stock.MEDI_STOCK_CODE;
                            ado.MEDI_STOCK_NAME = stock.MEDI_STOCK_NAME;
                        }

                        GroupByTypeStockRoomDetail.Add(ado);
                    }

                    // Gom theo loai kho va phong chi dinh
                    GroupByTypeStockRoom = GroupByTypeStockRoomDetail.GroupBy(o => new { o.MEDI_STOCK_TYPE, o.REQUEST_ROOM_CODE }).Select(s => s.First()).ToList();

                    // Gom theo loai kho
                    GroupByTypeStock = GroupByTypeStockRoom.GroupBy(o => o.MEDI_STOCK_TYPE).Select(s => s.First()).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<string> SeperateListString(List<string> listString)
        {
            List<string> result = new List<string>();
            try
            {
                foreach (var item in listString)
                {
                    if (String.IsNullOrEmpty(item))
                        continue;
                    string[] arrListStr = item.Split(';');
                    foreach (var str in arrListStr)
                    {
                        if (!String.IsNullOrWhiteSpace(str))
                        {
                            var newStr = str.Trim();
                            if (result.Contains(newStr) == false)
                            {
                                result.Add(newStr);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.HisPrescription != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.HisPrescription.TDL_TREATMENT_CODE;
                    //string serviceReqCode = "SERVICE_REQ_CODE:" + rdo.HisPrescription.SERVICE_REQ_CODE;
                    string serviceCode = "";
                    if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                    {
                        var serviceFirst = rdo.expMestMedicines.OrderBy(o => o.MEDICINE_TYPE_CODE).First();
                        serviceCode = "SERVICE_CODE:" + serviceFirst.MEDICINE_TYPE_CODE;
                    }

                    List<string> serviceReqCodes = new List<string>();
                    if (rdo.ListServiceReq != null && rdo.ListServiceReq.Count > 0)
                    {
                        foreach (var item in rdo.ListServiceReq.Select(s => s.SERVICE_REQ_CODE).Distinct().ToList())
                        {
                            serviceReqCodes.Add("SERVICE_REQ_CODE:" + item);
                        }
                    }

                    string serviceReqCode = string.Join(",", serviceReqCodes);
                    result = String.Format("{0} {1} {2} {3}", printTypeCode, treatmentCode, serviceReqCode, serviceCode);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ExpMestMedicineGroup()
        {
            try
            {
                ExpMests = new List<ExpMestsGroupADO>();
                if (rdo.expMestMedicines == null || rdo.expMestMedicines.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("rdo.expMestMedicines == null");
                    return;
                }

                var expMestMedicineGroups = rdo.expMestMedicines.GroupBy(o => o.EXP_MEST_ID);
                foreach (var item in expMestMedicineGroups)
                {
                    ExpMestsGroupADO ado = new ExpMestsGroupADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestsGroupADO>(ado, item.First());

                    if (rdo.ListServiceReq != null && rdo.ListServiceReq.Count > 0)
                    {
                        var serviceReq = rdo.ListServiceReq.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_REQ_ID);
                        if (serviceReq != null)
                        {
                            ado.REQUEST_LOGINNAME = serviceReq.REQUEST_LOGINNAME;
                            ado.REQUEST_USER_TITLE = serviceReq.REQUEST_USER_TITLE;
                            ado.REQUEST_USERNAME = serviceReq.REQUEST_USERNAME;

                            if (rdo.ListAcsUser != null && rdo.ListAcsUser.Count > 0)
                            {
                                var user = rdo.ListAcsUser.FirstOrDefault(o => o.LOGINNAME == serviceReq.REQUEST_LOGINNAME);
                                if (user != null)
                                {
                                    ado.MOBILE = user.MOBILE;
                                }
                            }

                            if (rdo.ListRoom != null && rdo.ListRoom.Count > 0)
                            {
                                var room = rdo.ListRoom.FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                                if (room != null)
                                {
                                    ado.REQUEST_ROOM_CODE = room.ROOM_CODE;
                                    ado.REQUEST_ROOM_NAME = room.ROOM_NAME;
                                }
                            }
                        }
                    }

                    ExpMests.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSingleKey()
        {
            try
            {

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.hisServiceReq_CurentExam), rdo.hisServiceReq_CurentExam));
                if (rdo.Mps000234ADO != null)
                {
                    AddObjectKeyIntoListkey<Mps000234ADO>(rdo.Mps000234ADO, false);
                    SetSingleKey(new KeyValue(("TITLE"), rdo.Mps000234ADO.TITLE));
                    SetSingleKey(new KeyValue(("NUMBER_ORDER_OF_DAY"), rdo.Mps000234ADO.NUMBER_ORDER_OF_DAY));
                }
                if (rdo.hisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.hisDhst, false);
                }

                if (rdo.HisPrescription != null)
                {
                    if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                    {
                        decimal tong = 0;
                        foreach (var item in rdo.expMestMedicines)
                        {
                            tong += item.AMOUNT * ((item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0)));
                        }
                        SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.TOTAL_PRICE_PRESCRIPTION, tong));

                        //nếu không có use_time_to trong V_HIS_PRESCRIPTION thì lấy trong ds thuốc 
                        if (rdo.HisPrescription != null && !rdo.HisPrescription.USE_TIME_TO.HasValue)
                        {
                            var maxUseTimeTo = rdo.expMestMedicines.Where(o => o.USE_TIME_TO.HasValue).ToList();
                            if (maxUseTimeTo != null && maxUseTimeTo.Count > 0)
                            {
                                var useTimeTo = maxUseTimeTo.Max(m => m.USE_TIME_TO).Value;
                                SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(useTimeTo)));
                            }
                        }
                        else
                            SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.USE_TIME_TO ?? 0)));

                        if (rdo.HisPrescription != null && rdo.HisPrescription.USE_TIME.HasValue)
                        {
                            SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.USE_TIME ?? 0)));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.INTRUCTION_TIME)));
                        }
                    }

                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.INTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.INTRUCTION_TIME_FULL_SRT, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.HisPrescription.INTRUCTION_TIME)));

                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.HisPrescription.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.D_O_B, rdo.HisPrescription.TDL_PATIENT_DOB.ToString().Substring(0, 4))));

                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.NATIONAL_NAME, rdo.HisPrescription.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.WORK_PLACE, rdo.HisPrescription.TDL_PATIENT_WORK_PLACE_NAME));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.ADDRESS, rdo.HisPrescription.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.CAREER_NAME, rdo.HisPrescription.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.PATIENT_CODE, rdo.HisPrescription.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.DISTRICT_CODE, rdo.HisPrescription.TDL_PATIENT_DISTRICT_CODE));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.GENDER_NAME, rdo.HisPrescription.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.MILITARY_RANK_NAME, rdo.HisPrescription.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.DOB, rdo.HisPrescription.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.VIR_ADDRESS, rdo.HisPrescription.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.VIR_PATIENT_NAME, rdo.HisPrescription.TDL_PATIENT_NAME));


                    string title = "C";
                    var expMestMedicine = rdo.expMestMedicines != null && rdo.expMestMedicines.Count() > 0 ? rdo.expMestMedicines.FirstOrDefault() : null;
                    if (expMestMedicine != null && expMestMedicine.IS_NEUROLOGICAL == 1) title = "H";
                    else if (expMestMedicine != null && expMestMedicine.IS_ADDICTIVE == 1) title = "N";

                    string serviceReqCode = rdo.HisPrescription != null ? (rdo.HisPrescription.SERVICE_REQ_CODE) : null;
                    Inventec.Common.Logging.LogSystem.Debug("serviceReqCode___:" + serviceReqCode);
                    string electronicExpMestCode = string.Format("{0}{1}-{2}", MPS.ProcessorBase.PrintConfig.MediOrgCode, HIS.ERXConnect.ERXCode.Encode(Convert.ToInt64(serviceReqCode)), title);
                    Inventec.Common.Logging.LogSystem.Debug("electronicExpMestCode___:" + electronicExpMestCode);
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.ELECTRONIC_EXP_MEST_CODE, electronicExpMestCode));

                }
                if (rdo.vHisPatientTypeAlter != null)
                {
                    if (!String.IsNullOrEmpty(rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                if (rdo.Mps000234ADO != null && rdo.Mps000234ADO.HisTreatment != null)
                {
                    SetSingleKey((new KeyValue("APPOINTMENT_CODE", rdo.Mps000234ADO.HisTreatment.APPOINTMENT_CODE)));
                    SetSingleKey((new KeyValue("APPOINTMENT_DATE", rdo.Mps000234ADO.HisTreatment.APPOINTMENT_DATE)));
                    SetSingleKey((new KeyValue("APPOINTMENT_DESC", rdo.Mps000234ADO.HisTreatment.APPOINTMENT_DESC)));
                    SetSingleKey((new KeyValue("APPOINTMENT_SURGERY", rdo.Mps000234ADO.HisTreatment.APPOINTMENT_SURGERY)));
                    SetSingleKey((new KeyValue("APPOINTMENT_TIME", rdo.Mps000234ADO.HisTreatment.APPOINTMENT_TIME)));
                    SetSingleKey((new KeyValue("APPOINTMENT_EXAM_ROOM_IDS", rdo.Mps000234ADO.HisTreatment.APPOINTMENT_EXAM_ROOM_IDS)));
                }

                AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.HisPrescription, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.vHisPatientTypeAlter);
                //AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.vHisPatient);
                if (rdo.hisServiceReq_Exam != null)
                {
                    AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.hisServiceReq_Exam, false);
                }
                if (rdo.Mps000234ADO != null && rdo.Mps000234ADO.HisTreatment != null)
                {
                    AddObjectKeyIntoListkey(rdo.Mps000234ADO.HisTreatment, false);

                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.ICD_SUB_CODE_TREATMENT, rdo.Mps000234ADO.HisTreatment.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.ICD_TEXT_TREATMENT, rdo.Mps000234ADO.HisTreatment.ICD_TEXT));
                }

                if (rdo.hisServiceReq_CurentExam != null)
                {
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.ICD_CODE_EXAM, rdo.hisServiceReq_CurentExam.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.ICD_NAME_EXAM, rdo.hisServiceReq_CurentExam.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.ICD_SUB_CODE_EXAM, rdo.hisServiceReq_CurentExam.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000234ExtendSingleKey.ICD_TEXT_EXAM, rdo.hisServiceReq_CurentExam.ICD_TEXT));

                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.PART_EXAM_EYE_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYE))));
                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.PART_EXAM_EYE_TENSION_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYE_TENSION_LEFT))));
                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.PART_EXAM_EYE_TENSION_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYE_TENSION_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.PART_EXAM_EYESIGHT_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYESIGHT_LEFT))));
                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.PART_EXAM_EYESIGHT_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYESIGHT_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.PART_EXAM_EYESIGHT_GLASS_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYESIGHT_GLASS_LEFT))));
                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.PART_EXAM_EYESIGHT_GLASS_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYESIGHT_GLASS_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.PART_EXAM_HOLE_GLASS_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_HOLE_GLASS_LEFT))));
                    SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.PART_EXAM_HOLE_GLASS_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_HOLE_GLASS_RIGHT))));

                }

                string ttServiceNames = "";
                string clsServiceNames = "";
                if (rdo.ListSereServCls != null && rdo.ListSereServCls.Count > 0)
                {
                    var listTt = rdo.ListSereServCls.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && o.IS_NO_EXECUTE != 1).ToList();
                    var listOther = rdo.ListSereServCls.Where(o => o.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && o.IS_NO_EXECUTE != 1).ToList();

                    if (listTt != null && listTt.Count() > 0)
                    {
                        ttServiceNames = string.Join(",", listTt.Select(s => s.TDL_SERVICE_NAME).Distinct().ToList());
                    }

                    if (listOther != null && listOther.Count() > 0)
                    {
                        clsServiceNames = string.Join(",", listOther.Select(s => s.TDL_SERVICE_NAME).Distinct().ToList());
                    }
                }

                SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.TT_SERVICE_NAME, ttServiceNames)));
                SetSingleKey((new KeyValue(Mps000234ExtendSingleKey.CLS_SERVICE_NAME, clsServiceNames)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string ProcessDataEye(string p)
        {
            string result = p;
            try
            {
                if (!String.IsNullOrEmpty(p))
                {
                    bool addText = true;
                    foreach (var item in p)
                    {
                        if (Char.IsLetter(item))
                        {
                            addText = false;
                            break;
                        }
                    }

                    if (addText)
                    {
                        result += "/10";
                    }
                }
            }
            catch (Exception ex)
            {
                result = p;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        void ProcessBarcodeKey()
        {
            try
            {
                if (rdo.HisPrescription != null)
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.HisPrescription.TDL_TREATMENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(Mps000234ExtendSingleKey.TREATMENT_CODE_BARCODE, barcodeTreatmentCode);

                    Inventec.Common.BarcodeLib.Barcode expMestCodeBarCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Mps000234ADO.EXP_MEST_CODE);
                    expMestCodeBarCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    expMestCodeBarCode.IncludeLabel = false;
                    expMestCodeBarCode.Width = 120;
                    expMestCodeBarCode.Height = 40;
                    expMestCodeBarCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    expMestCodeBarCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    expMestCodeBarCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    expMestCodeBarCode.IncludeLabel = true;

                    dicImage.Add(Mps000234ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBarCode);
                    Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.HisPrescription.TDL_PATIENT_CODE);
                    barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodePatient.IncludeLabel = false;
                    barcodePatient.Width = 120;
                    barcodePatient.Height = 40;
                    barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodePatient.IncludeLabel = true;

                    dicImage.Add(Mps000234ExtendSingleKey.PATIENT_CODE_BARCODE, barcodePatient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class HeinCardHelper
    {
        public static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
                {
                    string separateSymbol = "-";
                    result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        public static string TrimHeinCardNumber(string chucodau)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
