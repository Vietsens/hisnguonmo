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
using MPS.Processor.Mps000049.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000049
{
    public class Mps000049Processor : AbstractProcessor
    {
        Mps000049PDO rdo;
        List<ExpMestADO> ExpMestADOs;
        List<MedicineUseFormADO> medicineUseForms;
        List<ExpMestADO> listMedicineType = new List<ExpMestADO>();
        List<ExpMestADO> listMedicineParent = new List<ExpMestADO>();

        public Mps000049Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000049PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            { 
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrExpMest.EXP_MEST_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000049ExtendSingleKey.EXP_MEST_CODE_BAR, barcodePatientCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                ProcessListADO();
                ProcessListMedicineUse();

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetBarcodeKey();
                SetSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);



                if (ExpMestADOs != null && ExpMestADOs.Count > 0)
                {
                    ExpMestADOs = ExpMestADOs.OrderBy(o => o.TDL_PATIENT_FIRST_NAME).ToList();
                }

                barCodeTag.ProcessData(store, dicImage);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("rdo.keyName____", rdo.keyName));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("rdo.listAdo__", rdo.listAdo));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("medicineUseForms__", medicineUseForms));

                GetMedicineGroup();
                GetMedicineParent();

                objectTag.AddObjectData(store, "ExpMestAggregates", rdo.listAdo);
                objectTag.AddObjectData(store, "ExpMests", this.ExpMestADOs);
                objectTag.AddObjectData(store, "MedicineUseForms", medicineUseForms);
                objectTag.AddObjectData(store, "MedicineGroup", listMedicineType);
                objectTag.AddObjectData(store, "MedicineParent", listMedicineParent);

                objectTag.AddRelationship(store, "ExpMestAggregates", "ExpMests", new string[] { "MEDI_MATE_TYPE_ID", "TYPE_ID" }, new string[] { "MEDI_MATE_TYPE_ID", "TYPE_ID" });

                objectTag.AddRelationship(store, "MedicineUseForms", "ExpMests", "MEDICINE_USE_FORM_CODE", "MEDICINE_USE_FORM_CODE");
                objectTag.AddRelationship(store, "MedicineUseForms", "ExpMestAggregates", "MEDICINE_USE_FORM_CODE", "MEDICINE_USE_FORM_CODE");
                objectTag.AddRelationship(store, "MedicineGroup", "ExpMestAggregates", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "ExpMests", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ExpMestAggregates", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ExpMests", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");

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

        private void ProcessListMedicineUse()
        {
            try
            {
                medicineUseForms = new List<MedicineUseFormADO>();
                Inventec.Common.Logging.LogSystem.Info("rdo.keyName:" + rdo.keyName + "  keyTitles.phieuLinhThuocThuong:" + keyTitles.phieuLinhThuocThuong);

                if (rdo.keyName == keyTitles.phieuLinhThuocThuong)
                {
                    Inventec.Common.Logging.LogSystem.Info("true__");

                    var lstAdo = rdo.listAdo.Distinct();

                    foreach (var item in rdo.listAdo)
                    {

                        if (!medicineUseForms.Exists(o => o.MEDICINE_USE_FORM_ID == item.MEDICINE_USE_FORM_ID))
                        {
                            MedicineUseFormADO medicineUseForm = new MedicineUseFormADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MedicineUseFormADO>(medicineUseForm, item);
                            medicineUseForms.Add(medicineUseForm);
                        }
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
                
                if (rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {
                    var minTime = rdo._ExpMests_Print.Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._ExpMests_Print.Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));

                }

                switch (rdo.keyName)
                {
                    case keyTitles.phieuLinhTongHop:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "TỔNG HỢP"));
                        break;
                    case keyTitles.phieuLinhThuocThuong:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC THƯỜNG"));
                        break;
                    case keyTitles.Corticoid:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC CORTICOID"));
                        break;
                    case keyTitles.KhangSinh:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC KHÁNG SINH"));
                        break;
                    case keyTitles.Lao:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC LAO"));
                        break;
                    case keyTitles.DichTruyen:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC DỊCH TRUYỀN"));
                        break;
                    case keyTitles.TienChat:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC TIỀN CHẤT"));
                        break;
                    default:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, ""));
                        break;
                }

                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>(rdo.AggrExpMest, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));

                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.PARENT_TYPE_CODE, rdo.ConfigMps49.PARENT_TYPE_CODE));
                if (this.rdo.ListAggrExpMest != null && this.rdo.ListAggrExpMest.Count > 0)
                {

                    string expMestCodes = string.Join(",", rdo.ListAggrExpMest.Select(s => s.EXP_MEST_CODE).Distinct().ToList());
                    SetSingleKey((new KeyValue(Mps000049ExtendSingleKey.EXP_MEST_CODEs, expMestCodes)));

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
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.CREATE_TIME_AGGR_STR, createTimeStr));
                }

                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.TOTAL_REQ_ROOM_NAME_DISPLAY, totalReqRoomName));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string totalReqRoomName = "";
        private void GetMedicineGroup()
        {
            try
            {
                if (ExpMestADOs != null && ExpMestADOs.Count > 0)
                {
                    var group = ExpMestADOs.GroupBy(o => new { o.MEDICINE_GROUP_ID, o.MEDICINE_GROUP_CODE, o.MEDICINE_GROUP_NAME });
                    foreach (var item in group)
                    {
                        listMedicineType.Add(item.ToList().First());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetMedicineParent()
        {
            try
            {
                if (ExpMestADOs != null && ExpMestADOs.Count > 0)
                {
                    var group = ExpMestADOs.GroupBy(o => new { o.MEDICINE_PARENT_ID });
                    foreach (var item in group)
                    {
                        listMedicineParent.Add(item.ToList().First());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        void ProcessListADO()
        {
            try
            {
                List<long> reqRoomIds = new List<long>();
                ExpMestADOs = new List<ExpMestADO>();
                if (rdo.IsMedicine && rdo._ExpMestMedicines != null && rdo._ExpMestMedicines.Count > 0)
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

                    List<Mps000049ADO> dataMedis = new List<Mps000049ADO>();
                    if (rdo.ConfigMps49._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new { g.MEDICINE_ID, g.CONCENTRA }).Select(p => p.ToList()).ToList();
                        dataMedis.AddRange(from r in Groups
                                           select new Mps000049ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._MedicineTypes,
                                               rdo.ConfigMps49._ExpMestSttId__Approved,
                                               rdo.ConfigMps49._ExpMestSttId__Exported,
                                               rdo.ConfigMps49.PatientTypeId__BHYT));
                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr .Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();

                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._MedicineTypes, rdo.ConfigMps49._ExpMestSttId__Approved, rdo.ConfigMps49._ExpMestSttId__Exported, rdo.ConfigMps49.PatientTypeId__BHYT, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.CONCENTRA }).Select(p => p.ToList()).ToList();
                        dataMedis.AddRange(from r in Groups
                                           select new Mps000049ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._MedicineTypes,
                                               rdo.ConfigMps49._ExpMestSttId__Approved,
                                               rdo.ConfigMps49._ExpMestSttId__Exported,
                                               rdo.ConfigMps49.PatientTypeId__BHYT));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {

                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();

                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._MedicineTypes, rdo.ConfigMps49._ExpMestSttId__Approved, rdo.ConfigMps49._ExpMestSttId__Exported, rdo.ConfigMps49.PatientTypeId__BHYT, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }

                    if (dataMedis != null && dataMedis.Count > 0)
                    {
                        switch (rdo.ConfigMps49._ConfigKeyOderOption)
                        {
                            case 1:
                                dataMedis = dataMedis.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 2:
                                dataMedis = dataMedis.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 3:
                                dataMedis = dataMedis.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 4:
                                dataMedis = dataMedis.OrderBy(p => p.SERVICE_UNIT_NAME).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                        }
                        rdo.listAdo.AddRange(dataMedis);
                    }


                }

                //TODO VT
                if (rdo._ExpMestMaterials != null && rdo._ExpMestMaterials.Count > 0)
                {
                    var query = rdo._ExpMestMaterials.ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0 && rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                    {
                        query = query.Where(p => rdo.RoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                    }

                    query = query.Where(p => Check(p)).ToList();

                    if (query != null && query.Count > 0)
                    {
                        reqRoomIds.AddRange(query.Select(s => s.REQ_ROOM_ID).ToList());
                    }

                    List<Mps000049ADO> dataMates = new List<Mps000049ADO>();
                    if (rdo.ConfigMps49._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new { g.MATERIAL_ID, g.IS_CHEMICAL_SUBSTANCE }).Select(p => p.ToList()).ToList();
                        dataMates.AddRange(from r in Groups
                                           select new Mps000049ADO(rdo.AggrExpMest,
                                               r,
                                               rdo.ConfigMps49._ExpMestSttId__Approved,
                                               rdo.ConfigMps49._ExpMestSttId__Exported,
                                               rdo.ConfigMps49.PatientTypeId__BHYT));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo.ConfigMps49._ExpMestSttId__Approved, rdo.ConfigMps49._ExpMestSttId__Exported, rdo.ConfigMps49.PatientTypeId__BHYT, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.IS_CHEMICAL_SUBSTANCE }).Select(p => p.ToList()).ToList();
                        dataMates.AddRange(from r in Groups
                                           select new Mps000049ADO(rdo.AggrExpMest,
                                               r,
                                               rdo.ConfigMps49._ExpMestSttId__Approved,
                                               rdo.ConfigMps49._ExpMestSttId__Exported,
                                               rdo.ConfigMps49.PatientTypeId__BHYT));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo.ConfigMps49._ExpMestSttId__Approved, rdo.ConfigMps49._ExpMestSttId__Exported, rdo.ConfigMps49.PatientTypeId__BHYT, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }

                    if (dataMates != null && dataMates.Count > 0)
                    {
                        dataMates = dataMates.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        rdo.listAdo.AddRange(dataMates);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        bool Check(V_HIS_EXP_MEST_MATERIAL _expMestMaterial)
        {
            bool result = false;
            try
            {
                if (_expMestMaterial != null)
                {
                    if (rdo.ServiceUnitIds != null && rdo.ServiceUnitIds.Count > 0 && (rdo.Ismaterial || rdo.IsChemicalSustance))
                    {
                        if (rdo.ServiceUnitIds.Contains(_expMestMaterial.SERVICE_UNIT_ID))
                            result = true;
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
    }
}
