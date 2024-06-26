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
using AutoMapper;
using FlexCel.Report;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000169.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000169
{
    public class Mps000169Processor : AbstractProcessor
    {
        Mps000169PDO rdo;
        public Mps000169Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000169PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode expMestCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrExpMest.EXP_MEST_CODE);
                expMestCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                expMestCodeBar.IncludeLabel = false;
                expMestCodeBar.Width = 120;
                expMestCodeBar.Height = 40;
                expMestCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                expMestCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                expMestCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                expMestCodeBar.IncludeLabel = true;

                dicImage.Add(Mps000169ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBar);
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
                ProcessListADO();
                SetSingleKey();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "ExpMestAggregates", rdo.listAdo);
                objectTag.AddObjectData(store, "ExpMests", this._ExpMestADOs);
                if (rdo.ConfigMps169._ConfigKeyMERGER_DATA == 1)
                    objectTag.AddRelationship(store, "ExpMestAggregates", "ExpMests", new string[] { "MEDICINE_ID", "ACTIVE_INGR_BHYT_CODE", "MANUFACTURER_ID", "EXPIRED_DATE", "CONCENTRA" }, new string[] { "MEDICINE_ID", "ACTIVE_INGR_BHYT_CODE", "MANUFACTURER_ID", "EXPIRED_DATE", "CONCENTRA" });
                else
                    objectTag.AddRelationship(store, "ExpMestAggregates", "ExpMests", new string[] { "MEDI_MATE_TYPE_ID", "ACTIVE_INGR_BHYT_CODE", "MANUFACTURER_ID", "EXPIRED_DATE", "CONCENTRA" }, new string[] { "MEDICINE_TYPE_ID", "ACTIVE_INGR_BHYT_CODE", "MANUFACTURER_ID", "EXPIRED_DATE", "CONCENTRA" });

                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData13", new CalculateMergerData());

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
                
                if (rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {
                    var minTime = rdo._ExpMests_Print.Where(p => p.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT).Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._ExpMests_Print.Where(p => p.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT).Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));

                    SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));

                }
                string keyNameTitles = "";
                switch (rdo.keyName)
                {
                    case keyTitles.phieuLinh_GN_HT:
                        keyNameTitles = "GÂY NGHIỆN, THUỐC HƯỚNG THẦN";
                        break;
                    case keyTitles.phieuLinhGN:
                        keyNameTitles = "GÂY NGHIỆN";
                        break;
                    case keyTitles.phieuLinhHT:
                        keyNameTitles = "HƯỚNG THẦN";
                        break;
                    case keyTitles.TienChat:
                        keyNameTitles = "TIỀN CHẤT";
                        break;
                    case keyTitles.phieuLinhHCHT:
                        keyNameTitles = "THUỐC CÓ CHỨA DƯỢC CHẤT HƯỚNG THẦN";
                        break;
                    case keyTitles.phieuLinhHCGN_HCHT:
                        keyNameTitles = "THUỐC CÓ CHỨA DƯỢC CHẤT GÂY NGHIỆN HOẶC DƯỢC CHẤT HƯỚNG THẦN";
                        break;
                        
                    default:
                        break;
                }
                SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.KEY_NAME_TITLES, keyNameTitles));
                AddObjectKeyIntoListkey<V_HIS_EXP_MEST>(rdo.AggrExpMest, false);
                AddObjectKeyIntoListkey<HIS_DEPARTMENT>(rdo.Department, false);
                SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                if (this.rdo.ListAggrExpMest != null && this.rdo.ListAggrExpMest.Count > 0)
                {
                    string expMestCodes = string.Join(",", rdo.ListAggrExpMest.Select(s => s.EXP_MEST_CODE).Distinct().ToList());
                    SetSingleKey((new KeyValue(Mps000169ExtendSingleKey.EXP_MEST_CODEs, expMestCodes)));

                    List<string> createTime = new List<string>();
                    var list = this.rdo._ExpMestMedicines.Select(o => o.AGGR_EXP_MEST_ID).ToList();
                    this.rdo.ListAggrExpMest = this.rdo.ListAggrExpMest.OrderBy(o => o.CREATE_TIME).Where(o => o.MEDI_STOCK_CODE == rdo.AggrExpMest.MEDI_STOCK_CODE).ToList();
                    foreach (var item in this.rdo.ListAggrExpMest)
                    {
                        if (list != null && list.Count > 0 && list.Contains(item.ID))
                        {
                            createTime.Add(Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.CREATE_DATE));
                        }
                    }
                    createTime = createTime.Distinct().ToList();
                    var createTimeStr = String.Join(",", createTime);
                    SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.CREATE_TIME_AGGR_STR, createTimeStr));

                }

                SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.TOTAL_REQ_ROOM_NAME_DISPLAY, totalReqRoomName));

                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string totalReqRoomName = "";
        void ProcessListADO()
        {
            try
            {
                List<long> reqRoomIds = new List<long>();
                if (rdo._ExpMestMedicines != null && rdo._ExpMestMedicines.Count > 0)
                {
                    var query = rdo._ExpMestMedicines.ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        query = query.Where(p => rdo.RoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                    }
                    query = query.Where(p => Check(p)).ToList();

                    if (query != null && query.Count > 0)
                    {
                        reqRoomIds.AddRange(query.Select(s => s.REQ_ROOM_ID).ToList());
                    }
                    if (rdo.ConfigMps169._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new
                    {
                        g.MEDICINE_ID,
                        g.ACTIVE_INGR_BHYT_CODE,
                        g.MANUFACTURER_ID,
                        g.EXPIRED_DATE,
                        g.CONCENTRA
                    }).Select(p => p.ToList()).ToList();
                        rdo.listAdo.AddRange(from r in Groups
                                             select new Mps000169ADO(rdo.AggrExpMest,
                                                 r,
                                                 rdo.ConfigMps169._ExpMestSttId__Approved,
                                                 rdo.ConfigMps169._ExpMestSttId__Exported,
                                                 rdo.ConfigMps169.PatientTypeId__BHYT));
                    }
                    else
                    {

                        var Groups = query.GroupBy(g => new
                        {
                            g.MEDICINE_TYPE_ID,
                            g.ACTIVE_INGR_BHYT_CODE,
                            g.MANUFACTURER_ID,
                            g.EXPIRED_DATE,
                            g.CONCENTRA
                        }).Select(p => p.ToList()).ToList();
                        rdo.listAdo.AddRange(from r in Groups
                                             select new Mps000169ADO(rdo.AggrExpMest,
                                                 r,
                                                 rdo.ConfigMps169._ExpMestSttId__Approved,
                                                 rdo.ConfigMps169._ExpMestSttId__Exported,
                                                 rdo.ConfigMps169.PatientTypeId__BHYT));
                    }
                }

                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    if (rdo.ConfigMps169._ConfigKeyOderOption == 1)
                    {
                        rdo.listAdo = rdo.listAdo.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                    }
                    else if (rdo.ConfigMps169._ConfigKeyOderOption == 2)
                    {
                        rdo.listAdo = rdo.listAdo.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                    }
                    else if (rdo.ConfigMps169._ConfigKeyOderOption == 3)
                    {
                        rdo.listAdo = rdo.listAdo.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                    }
                    SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.TOTAL_PRICE, rdo.listAdo.Sum(o => o.PRICE ?? 0)));
                    SetSingleKey(new KeyValue(Mps000169ExtendSingleKey.TOTAL_PRICE_VAT, rdo.listAdo.Sum(o => (o.PRICE ?? 0) * (1 + (o.VAT_RATIO ?? 0)))));
                }

                _ExpMestADOs = new List<ExpMestADO>();
                if (rdo != null && rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {
                    var dataExpMestGrs = rdo._ExpMests_Print.GroupBy(p => p.TDL_PATIENT_ID).Select(p => p.ToList()).ToList();
                    foreach (var itemGrs in dataExpMestGrs)
                    {

                        List<long> _expMestIds = itemGrs.Select(p => p.ID).ToList();
                        var dataMedis = rdo._ExpMestMedicines.Where(p => _expMestIds.Contains(p.EXP_MEST_ID ?? 0)).ToList();

                        List<List<V_HIS_EXP_MEST_MEDICINE>> dataMedisGrs = new List<List<V_HIS_EXP_MEST_MEDICINE>>();
                        if (rdo.ConfigMps169._ConfigKeyMERGER_DATA == 1)
                            dataMedisGrs = dataMedis.GroupBy(p => new { p.MEDICINE_ID, p.ACTIVE_INGR_BHYT_CODE, p.MANUFACTURER_ID, p.EXPIRED_DATE, p.CONCENTRA }).Select(p => p.ToList()).ToList();
                        else
                            dataMedisGrs = dataMedis.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.ACTIVE_INGR_BHYT_CODE, p.MANUFACTURER_ID, p.EXPIRED_DATE, p.CONCENTRA }).Select(p => p.ToList()).ToList();
                        foreach (var item in dataMedisGrs)
                        {
                            ExpMestADO ado = new ExpMestADO(itemGrs.FirstOrDefault());
                            ado.MEDICINE_TYPE_ID = item.FirstOrDefault().MEDICINE_TYPE_ID;
                            ado.AMOUNT = item.Sum(p => p.AMOUNT);
                            ado.MANUFACTURER_ID = item.FirstOrDefault().MANUFACTURER_ID;
                            ado.ACTIVE_INGR_BHYT_CODE = item.FirstOrDefault().ACTIVE_INGR_BHYT_CODE;
                            ado.CONCENTRA = item.FirstOrDefault().CONCENTRA;
                            ado.EXPIRED_DATE = item.FirstOrDefault().EXPIRED_DATE;
                            ado.MEDICINE_ID = item.FirstOrDefault().MEDICINE_ID;
                            if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.ConfigMps169._ExpMestSttId__Approved
                                || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.ConfigMps169._ExpMestSttId__Exported)
                            {
                                ado.AMOUNT_EXCUTE = ado.AMOUNT;

                                if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.ConfigMps169._ExpMestSttId__Exported)
                                {
                                    ado.AMOUNT_EXPORTED = ado.AMOUNT_EXCUTE;
                                }
                            }
                            var dataBHYTs = item.Where(p => p.PATIENT_TYPE_ID == rdo.ConfigMps169.PatientTypeId__BHYT).ToList();
                            if (dataBHYTs != null && dataBHYTs.Count > 0)
                            {
                                ado.HEIN_AMOUNT = dataBHYTs.Sum(p => p.AMOUNT);
                            }
                            else
                            {
                                ado.HEIN_AMOUNT = null;
                            }
                            ado.AMOUNT_REQUEST = ado.AMOUNT;
                            ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_REQUEST)));
                            ado.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXCUTE)));
                            ado.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXPORTED)));
                            ado.HEIN_AMOUNT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.HEIN_AMOUNT ?? 0)));

                            _ExpMestADOs.Add(ado);
                        }
                    }
                }

                if (reqRoomIds.Count > 0)
                {
                    var reqrooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => reqRoomIds.Contains(o.ID)).ToList();
                    if (reqrooms != null && reqrooms.Count > 0)
                    {
                        totalReqRoomName = String.Join("; ", reqrooms.Select(s => s.ROOM_NAME).OrderBy(o => o));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<ExpMestADO> _ExpMestADOs { get; set; }

        bool Check(V_HIS_EXP_MEST_MEDICINE _expMestMedicine)
        {
            bool result = false;
            try
            {
                var data = rdo._MedicineTypes.FirstOrDefault(p => p.ID == _expMestMedicine.MEDICINE_TYPE_ID);
                if (data != null)
                {
                    if (rdo.ServiceUnitIds != null
                        && rdo.ServiceUnitIds.Count > 0)
                    {
                        if (rdo.ServiceUnitIds.Contains(data.SERVICE_UNIT_ID))
                            result = true;
                    }
                    if (data.MEDICINE_USE_FORM_ID > 0)
                    {
                        if (rdo.UseFormIds != null
                    && rdo.UseFormIds.Count > 0 && rdo.UseFormIds.Contains(data.MEDICINE_USE_FORM_ID ?? 0))
                        {
                            result = result && true;
                        }
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

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (rdo != null && rdo._ExpMests_Print != null)
                {
                    List<string> treatmentCodes = rdo._ExpMests_Print.Select(s => s.TDL_TREATMENT_CODE).Distinct().ToList();
                    List<string> expMestCodes = rdo._ExpMests_Print.Select(s => s.EXP_MEST_CODE).Distinct().ToList();
                    log = LogDataExpMest(string.Join(";", treatmentCodes), string.Join(";", expMestCodes), "");
                }
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
                if (rdo != null && rdo.AggrExpMest != null && rdo._ExpMests_Print != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", printTypeCode, rdo.AggrExpMest.EXP_MEST_CODE, rdo.AggrExpMest.MEDI_STOCK_CODE, rdo._ExpMests_Print.Count(), "Phiếu lĩnh", rdo.listAdo.FirstOrDefault().MEDICINE_TYPE_CODE, rdo.listAdo.Count());
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        class CalculateMergerData : TFlexCelUserFunction
        {
            long mediMateTypeId = 0;

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    long mediMateId = Convert.ToInt64(parameters[0]);

                    if (mediMateId > 0)
                    {
                        if (this.mediMateTypeId == mediMateId)
                        {
                            return true;
                        }
                        else
                        {
                            this.mediMateTypeId = mediMateId;
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

        public class ExpMestADO : HIS_EXP_MEST
        {
            public long MEDICINE_TYPE_ID { get; set; }
            public decimal AMOUNT { get; set; }
            public decimal AMOUNT_EXPORTED { get; set; }
            public decimal AMOUNT_EXCUTE { get; set; }
            public decimal AMOUNT_REQUEST { get; set; }
            public long? MEDICINE_ID { get; set; }
            public decimal? HEIN_AMOUNT { get; set; }

            public string AMOUNT_EXPORT_STRING { get; set; }
            public string AMOUNT_EXECUTE_STRING { get; set; }
            public string AMOUNT_REQUEST_STRING { get; set; }
            public string HEIN_AMOUNT_STRING { get; set; }
            public string CREATE_TIME_AGGR_STR { get; set; }
            public long? MANUFACTURER_ID { get; set; }
            public string ACTIVE_INGR_BHYT_CODE { get; set; }
            public string MEDICINE_USE_FORM_CODE { get; set; }
            public long? MEDICINE_USE_FORM_ID { get; set; }
            public string MEDICINE_USE_FORM_NAME { get; set; }
            public long? MEDICINE_USE_FORM_NUM_ORDER { get; set; }
            public long? EXPIRED_DATE { get; set; }
            public string CONCENTRA { get; set; }
            public ExpMestADO() { }

            public ExpMestADO(HIS_EXP_MEST data)
            {
                try
                {
                    if (data != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestADO>(this, data);
                        this.CREATE_TIME_AGGR_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.CREATE_TIME ?? 0);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }
    }
}
