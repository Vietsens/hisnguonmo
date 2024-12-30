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
using Inventec.Common.FlexCellExport;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000247.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000247
{
    public class Mps000247Processor : AbstractProcessor
    {
        Mps000247PDO rdo;

        List<ExpMestMedicineADO> _ExpMestMedicineADOs = new List<ExpMestMedicineADO>();
        List<ExpMestMaterialADO> _ExpMestMaterialADOs = new List<ExpMestMaterialADO>();
        List<ExpMestADO> _ExpMestAdos = new List<ExpMestADO>();
        List<V_HIS_EXP_MEST> _ExpMestIntructionDates = new List<V_HIS_EXP_MEST>();

        public Mps000247Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000247PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                //Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrExpMest.EXP_MEST_CODE);
                //barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodePatientCode.IncludeLabel = false;
                //barcodePatientCode.Width = 120;
                //barcodePatientCode.Height = 40;
                //barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodePatientCode.IncludeLabel = true;

                //dicImage.Add(Mps000247ExtendSingleKey.EXP_MEST_CODE_BAR, barcodePatientCode);
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
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "IntructionDates", this._ExpMestIntructionDates);
                objectTag.AddObjectData(store, "ExpMests", this._ExpMestAdos);
                objectTag.AddObjectData(store, "Medicines", this._ExpMestMedicineADOs);
                objectTag.AddObjectData(store, "Materials", this._ExpMestMaterialADOs);

                objectTag.AddRelationship(store, "IntructionDates", "ExpMests", "TDL_INTRUCTION_DATE", "TDL_INTRUCTION_DATE");
                objectTag.AddRelationship(store, "IntructionDates", "Medicines", "TDL_INTRUCTION_DATE", "TDL_INTRUCTION_DATE");
                objectTag.AddRelationship(store, "IntructionDates", "Materials", "TDL_INTRUCTION_DATE", "TDL_INTRUCTION_DATE");
                objectTag.AddRelationship(store, "ExpMests", "Medicines", "TDL_PATIENT_ID", "TDL_PATIENT_ID");    
                objectTag.AddRelationship(store, "ExpMests", "Materials", "TDL_PATIENT_ID", "TDL_PATIENT_ID");

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

        //Các bước lấy ra dữ liệu buồng như sau:
//- B1: Lấy ra dữ liệu buồng của hồ sơ điều trị đó và tương ứng với khoa (V_HIS_TREATMENT_BED_ROOM có DEPARTMENT_ID tương ứng với khoa, TREATMENT_ID tương ứng với hồ sơ điều trị)
//- B2: Kiểm tra, trong dữ liệu buồng có được ở B1, nếu tồn tại V_HIS_TREATMENT_BED_ROOM có REMOVE_TIME null thì lấy V_HIS_TREATMENT_BED_ROOM có ADD_TIME lớn nhất mà REMOVE_TIME null
        //- B3: Nếu không tồn tại V_HIS_TREATMENT_BED_ROOM nào có REMOVE_TIME null thì lấy V_HIS_TREATMENT_BED_ROOM có REMOVE_TIME lớn nhất.
        string GetBedRoomByPatient(long treatmentId)
        {
            string result = String.Empty;
            try
            {
                if (rdo.vHisTreatmentBedRooms == null || rdo.vHisTreatmentBedRooms.Count == 0)
                    return result;

                var treatmentBedRooms = rdo.vHisTreatmentBedRooms.Where(o => o.TREATMENT_ID == treatmentId && o.DEPARTMENT_ID == rdo.Department.ID).ToList();

                List<V_HIS_TREATMENT_BED_ROOM> lsttreatmentBedRoom = new List<V_HIS_TREATMENT_BED_ROOM>();

                if (treatmentBedRooms != null && treatmentBedRooms.Count > 0)
                {
                    var checkFinishTime = treatmentBedRooms.Where(o => o.REMOVE_TIME == null).ToList();

                    if (checkFinishTime != null && checkFinishTime.Count > 0)
                    {
                        lsttreatmentBedRoom = checkFinishTime.OrderByDescending(o => o.ADD_TIME).ToList();
                    }
                    else
                    {
                        lsttreatmentBedRoom = treatmentBedRooms.OrderByDescending(o => o.REMOVE_TIME).ToList();
                    }


                    if (lsttreatmentBedRoom != null && lsttreatmentBedRoom.Count > 0)
                    {
                        result = lsttreatmentBedRoom[0].BED_ROOM_NAME;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

//        Các bước lấy ra dữ liệu giường như sau:
//- B1: Lấy ra dữ liệu lịch sử giường của hồ sơ điều trị đó và tương ứng với khoa (V_HIS_BED_LOG có DEPARTMENT_ID tương ứng với khoa, TREATMENT_ID tương ứng với hồ sơ điều trị)
//- B2: Kiểm tra, trong dữ liệu "Lịch sử giường" có được ở B1, nếu tồn tại V_HIS_BED_LOG có FINISH_TIME null thì lấy V_HIS_BED_LOG có START_TIME lớn nhất mà FINISH_TIME null
//- B3: Nếu không tồn tại V_HIS_BED_LOG nào có FINISH_TIME null thì lấy V_HIS_BED_LOG có FINISH_TIME lớn nhất.
        void GetBedByTreatment(long treatmentId, ref string BedCode, ref string BedName)
        {
            if (rdo._listBedLog == null || rdo._listBedLog.Count == 0)
                return;
            try
            {
                var bedLodDepartment = rdo._listBedLog.Where(o => o.TREATMENT_ID == treatmentId && o.DEPARTMENT_ID == rdo.Department.ID).ToList();

                List<V_HIS_BED_LOG> bedLog = new List<V_HIS_BED_LOG>();

                var checkFinishTime = (bedLodDepartment != null && bedLodDepartment.Count > 0) ? bedLodDepartment.Where(o => o.FINISH_TIME == null).ToList() : null;

                if (checkFinishTime != null && checkFinishTime.Count > 0)
                {
                    bedLog = checkFinishTime.OrderByDescending(o => o.START_TIME).ToList();
                }
                else 
                {
                    bedLog = bedLodDepartment.OrderByDescending(o => o.FINISH_TIME).ToList();
                }


                if (bedLog != null && bedLog.Count > 0)
                {
                    BedCode = bedLog[0].BED_CODE;
                    BedName = bedLog[0].BED_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        void SetSingleKey()
        {
            try
            {
                if (rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {

                    SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.TIME_FILTER_OPTION, rdo._TimeFilterOption));
                    

                    if (rdo._TimeFilterOption == 1)
                    {
                        long minTime = rdo._ExpMests_Print.Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                        long maxTime = rdo._ExpMests_Print.Max(p => p.TDL_INTRUCTION_TIME ?? 0);

                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));

                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));
                    }
                    else if (rdo._TimeFilterOption == 2)
                    {
                        var min = rdo._ExpMests_Print.Min(s => s.TDL_USE_TIME ?? 0);
                        var max = rdo._ExpMests_Print.Max(s => s.TDL_USE_TIME ?? 0);
                           
                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MIN_USE_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(min)));
                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(min)));
                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(min)));

                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MAX_USE_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(max)));
                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(max)));
                        SetSingleKey(new KeyValue(Mps000247ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(max)));
                    }    
                    
                    

                    
                }

                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

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
                this._ExpMestMaterialADOs = new List<ExpMestMaterialADO>();
                this._ExpMestMedicineADOs = new List<ExpMestMedicineADO>();
                this._ExpMestAdos = new List<ExpMestADO>();
                this._ExpMestIntructionDates = new List<V_HIS_EXP_MEST>();

                if (rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {
                    var dataGroupByIntructionTimes = rdo._ExpMests_Print.GroupBy(p => p.TDL_INTRUCTION_DATE).Select(p => p.ToList()).ToList();
                    foreach (var itemTime in dataGroupByIntructionTimes)
                    {
                        this._ExpMestIntructionDates.Add(itemTime.FirstOrDefault());

                        var dataExpMestGroups = rdo._ExpMests_Print.Where(p => p.TDL_INTRUCTION_DATE == itemTime.FirstOrDefault().TDL_INTRUCTION_DATE).GroupBy(p => p.TDL_PATIENT_ID).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in dataExpMestGroups)
                        {
                            ExpMestADO ado = new ExpMestADO(itemGr.FirstOrDefault());

                            if (itemGr.FirstOrDefault().TDL_TREATMENT_ID.HasValue)
                            {
                                ado.BED_ROOM_NAMEs = GetBedRoomByPatient(itemGr.FirstOrDefault().TDL_TREATMENT_ID.Value);
                                string bedCode = "", bedName = "";
                                GetBedByTreatment(itemGr.FirstOrDefault().TDL_TREATMENT_ID.Value, ref bedCode, ref bedName);
                                ado.BED_CODE = bedCode;
                                ado.BED_NAME = bedName;
                            }

                            this._ExpMestAdos.Add(ado);
                            //this._ExpMestAdos.Add(itemGr.FirstOrDefault());

                            if (rdo._ExpMestMedicines != null && rdo._ExpMestMedicines.Count > 0)
                            {
                                List<long> _ExpMestIds = itemGr.Select(p => p.ID).ToList();
                                var query = rdo._ExpMestMedicines.Where(p => _ExpMestIds.Contains(p.EXP_MEST_ID ??0)).ToList();
                                //if (rdo._ConfigKeyMERGER_DATA == 1)
                                //{
                                //    var Groups = query.GroupBy(g => new
                                //    {
                                //        g.MEDICINE_ID,
                                //        g.CONCENTRA
                                //    }).Select(p => p.ToList()).ToList();
                                //    this._ExpMestMedicineADOs.AddRange(from r in Groups
                                //                                       select new ExpMestMedicineADO(r, itemGr.FirstOrDefault().TDL_PATIENT_ID ?? 0));
                                //}
                                //else
                                //{
                                var Groups = query.GroupBy(g => new
                                {
                                    g.MEDICINE_TYPE_ID,
                                    g.CONCENTRA
                                }).Select(p => p.ToList()).ToList();
                                this._ExpMestMedicineADOs.AddRange(from r in Groups
                                                                   select new ExpMestMedicineADO(r, itemGr.FirstOrDefault().TDL_PATIENT_ID ?? 0, itemGr.FirstOrDefault().TDL_INTRUCTION_DATE ?? 0));
                                //}
                            }



                            //TODO VT
                            if (rdo._ExpMestMaterials != null && rdo._ExpMestMaterials.Count > 0)
                            {
                                List<long> _ExpMestIds = itemGr.Select(p => p.ID).ToList();
                                var query = rdo._ExpMestMaterials.Where(p => _ExpMestIds.Contains(p.EXP_MEST_ID ??0)).ToList();


                                //if (rdo._ConfigKeyMERGER_DATA == 1)
                                //{
                                //    var Groups = query.GroupBy(g => new
                                //    {
                                //        g.MATERIAL_ID,
                                //        g.IS_CHEMICAL_SUBSTANCE
                                //    }).Select(p => p.ToList()).ToList();
                                //    this._ExpMestMaterialADOs.AddRange(from r in Groups
                                //                                       select new ExpMestMaterialADO(r, itemGr.FirstOrDefault().TDL_PATIENT_ID ?? 0));
                                //}
                                //else
                                //{
                                var Groups = query.GroupBy(g => new
                                {
                                    g.MATERIAL_TYPE_ID,
                                    g.IS_CHEMICAL_SUBSTANCE
                                }).Select(p => p.ToList()).ToList();
                                this._ExpMestMaterialADOs.AddRange(from r in Groups
                                                                   select new ExpMestMaterialADO(r, itemGr.FirstOrDefault().TDL_PATIENT_ID ?? 0, itemGr.FirstOrDefault().TDL_INTRUCTION_DATE ?? 0));
                                // }
                            }
                        }
                    }
                }
                if (this._ExpMestIntructionDates != null && this._ExpMestIntructionDates.Count > 0)
                {
                    this._ExpMestIntructionDates = this._ExpMestIntructionDates.OrderBy(p => p.TDL_INTRUCTION_DATE).ToList();
                }
                if (this._ExpMestAdos != null && this._ExpMestAdos.Count > 0)
                {
                    this._ExpMestAdos = this._ExpMestAdos.OrderBy(p => p.TDL_PATIENT_NAME).ToList();
                }

                if (this._ExpMestMedicineADOs != null && this._ExpMestMedicineADOs.Count > 0)
                {
                    this._ExpMestMedicineADOs = this._ExpMestMedicineADOs.OrderBy(p => p.MEDICINE_TYPE_NAME).ToList();
                }
                if (this._ExpMestMaterialADOs != null && this._ExpMestMaterialADOs.Count > 0)
                {
                    this._ExpMestMaterialADOs = this._ExpMestMaterialADOs.OrderBy(p => p.MATERIAL_TYPE_NAME).ToList();
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
                if (rdo != null && rdo._ExpMests_Print != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", printTypeCode, "", rdo._ExpMests_Print.FirstOrDefault().EXP_MEST_CODE, rdo._ExpMests_Print.Count(), "Phiếu lĩnh", this._ExpMestAdos.FirstOrDefault().TDL_INTRUCTION_DATE, this._ExpMestAdos.Count());
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
