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
using MPS.Processor.Mps000480.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000480
{
    public partial class Mps000480Processor : AbstractProcessor
    {
        Mps000480PDO rdo;
        List<HIS_EXP_MEST> lstExpMest = new List<HIS_EXP_MEST>();
        List<MEDICINE_MATERIAL> lstMedicineMaterials = new List<MEDICINE_MATERIAL>();
        public Mps000480Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            if (rdoBase != null && rdoBase is Mps000480PDO)
            {
                rdo = (Mps000480PDO)rdoBase;
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                SetBarcodeKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                SetSingleKey();
                ProcessListKey();

                objectTag.AddObjectData(store, "ExpMests", this.lstExpMest);
                objectTag.AddObjectData(store, "MedicineMaterials", this.lstMedicineMaterials);
                objectTag.AddRelationship(store, "ExpMests", "MedicineMaterials", "ID", "EXP_MEST_ID");


                this.SetSignatureKeyImageByCFG();
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

        private void ProcessListKey()
        {
            try
            {
                if (rdo.lstMedicine != null && rdo.lstMedicine.Count() > 0)
                {
                    rdo.lstMedicine = rdo.lstMedicine.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.ID).ToList();
                    var medicineGroup = rdo.lstMedicine.GroupBy(x => new { x.EXP_MEST_ID, x.EXP_MEST_CODE });
                    foreach (var item in medicineGroup)
                    {
                        HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                        expMest.ID = (long)item.First().EXP_MEST_ID;
                        expMest.EXP_MEST_CODE = item.First().EXP_MEST_CODE;
                        lstExpMest.Add(expMest);
                    }
                    var medicineGroupByType = rdo.lstMedicine.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.TDL_INTRUCTION_TIME, o.USE_TIME_TO, o.EXP_MEST_ID });
                    foreach (var item in medicineGroupByType)
                    {
                        MEDICINE_MATERIAL medimate = new MEDICINE_MATERIAL();
                        medimate.EXP_MEST_ID = item.First().EXP_MEST_ID;
                        medimate.NAME = item.First().MEDICINE_TYPE_NAME;
                        medimate.ACTIVE_INGR_BHYT_NAME = item.First().ACTIVE_INGR_BHYT_NAME;
                        medimate.CONCENTRA = item.First().CONCENTRA;
                        medimate.AMOUNT = item.Sum(o => o.AMOUNT);
                        medimate.SERVICE_UNIT_NAME = item.First().SERVICE_UNIT_NAME;
                        medimate.TUTORIAL = item.First().TUTORIAL;
                        medimate.REQ_LOGINNAME = item.First().REQ_LOGINNAME;
                        medimate.REQ_USERNAME = item.First().REQ_USERNAME;
                        medimate.MORNING = item.First().MORNING;
                        medimate.NOON = item.First().NOON;
                        medimate.AFTERNOON = item.First().AFTERNOON;
                        medimate.EVENING = item.First().EVENING;
                        if (item.First().USE_TIME_TO != null && item.First().TDL_INTRUCTION_TIME != null)
                        {
                            DateTime use_time_to = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)item.First().USE_TIME_TO) ?? DateTime.MinValue;
                            DateTime intruction_time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)item.First().TDL_INTRUCTION_TIME) ?? DateTime.MinValue;
                            TimeSpan Time = use_time_to - intruction_time;
                            medimate.NUM_OF_DAYS = (Time.Days + 1).ToString();
                        }
                        lstMedicineMaterials.Add(medimate);
                    }
                }
                if (rdo.lstMaterial != null && rdo.lstMaterial.Count() > 0)
                {
                    rdo.lstMaterial = rdo.lstMaterial.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.ID).ToList();
                    var materialGroup = rdo.lstMaterial.GroupBy(x => new { x.EXP_MEST_ID, x.EXP_MEST_CODE });
                    foreach (var item in materialGroup)
                    {
                        // them vao DS xuat
                        HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                        expMest.ID = (long)item.First().EXP_MEST_ID;
                        expMest.EXP_MEST_CODE = item.First().EXP_MEST_CODE;
                        lstExpMest.Add(expMest);
                    }
                    var materialGroupByType = rdo.lstMaterial.GroupBy(o => new { o.MATERIAL_TYPE_ID });
                    foreach (var item in materialGroupByType)
                    {
                        MEDICINE_MATERIAL medimate = new MEDICINE_MATERIAL();
                        medimate.EXP_MEST_ID = item.First().EXP_MEST_ID;
                        medimate.NAME = item.First().MATERIAL_TYPE_NAME;
                        medimate.AMOUNT = item.Sum(o => o.AMOUNT);
                        medimate.SERVICE_UNIT_NAME = item.First().SERVICE_UNIT_NAME;
                        medimate.TUTORIAL = item.First().TUTORIAL;
                        medimate.REQ_LOGINNAME = item.First().REQ_LOGINNAME;
                        medimate.REQ_USERNAME = item.First().REQ_USERNAME;
                        lstMedicineMaterials.Add(medimate);
                    }
                }
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
                if (rdo.expMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000480ExtendSingleKey.TDL_TREATMENT_CODE, rdo.expMest.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000480ExtendSingleKey.TDL_PATIENT_NAME, rdo.expMest.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000480ExtendSingleKey.TDL_PATIENT_DOB, rdo.expMest.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000480ExtendSingleKey.TDL_PATIENT_GENDER_NAME, rdo.expMest.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000480ExtendSingleKey.NUM_ORDER, rdo.expMest.NUM_ORDER));
                    
                }
                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000480ExtendSingleKey.ICD_CODE, rdo.treatment.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000480ExtendSingleKey.ICD_NAME, rdo.treatment.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000480ExtendSingleKey.ICD_SUB_CODE, rdo.treatment.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000480ExtendSingleKey.ICD_TEXT, rdo.treatment.ICD_TEXT));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.expMest != null && !String.IsNullOrEmpty(rdo.expMest.TDL_TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.expMest.TDL_TREATMENT_CODE);
                    barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodePatientCode.IncludeLabel = false;
                    barcodePatientCode.Width = 120;
                    barcodePatientCode.Height = 40;
                    barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodePatientCode.IncludeLabel = true;

                    dicImage.Add(Mps000480ExtendSingleKey.TREATMENT_CODE_BAR, barcodePatientCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
