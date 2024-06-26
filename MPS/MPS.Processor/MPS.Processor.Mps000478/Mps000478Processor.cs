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
using MPS.Processor.Mps000478.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000478
{
    public partial class Mps000478Processor : AbstractProcessor
    {
        Mps000478PDO rdo;
        List<V_HIS_EXP_MEST_MEDICINE> lstServiceMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_SERE_SERV> listSereServ = new List<V_HIS_SERE_SERV>();
        List<V_HIS_SERE_SERV> listSereServBlood = new List<V_HIS_SERE_SERV>();
        public Mps000478Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            if (rdoBase != null && rdoBase is Mps000478PDO)
            {
                rdo = (Mps000478PDO)rdoBase;
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
                objectTag.AddObjectData(store, "SereServ", this.listSereServ);
                objectTag.AddObjectData(store, "Bloods", this.listSereServBlood);
                objectTag.AddObjectData(store, "ServiceMedicines", this.lstServiceMedicines);


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

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.Treatment != null && !String.IsNullOrEmpty(rdo.Treatment.TDL_PATIENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Treatment.TDL_PATIENT_CODE);
                    barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodePatientCode.IncludeLabel = false;
                    barcodePatientCode.Width = 120;
                    barcodePatientCode.Height = 40;
                    barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodePatientCode.IncludeLabel = true;

                    dicImage.Add(Mps000478ExtendSingleKey.TDL_PATIENT_CODE_BAR, barcodePatientCode);
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
                if (rdo.Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TDL_PATIENT_CODE, rdo.Treatment.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TREATMENT_CODE, rdo.Treatment.TREATMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TDL_PATIENT_NAME, rdo.Treatment.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TDL_PATIENT_DOB, rdo.Treatment.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TDL_PATIENT_GENDER_NAME, rdo.Treatment.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TDL_PATIENT_ETHNIC_NAME, rdo.Treatment.TDL_PATIENT_ETHNIC_NAME));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TDL_PATIENT_ADDRESS, rdo.Treatment.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TDL_PATIENT_CAREER_NAME, rdo.Treatment.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TDL_HEIN_CARD_NUMBER, rdo.Treatment.TDL_HEIN_CARD_NUMBER));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TDL_HEIN_CARD_FROM_TIME, rdo.Treatment.TDL_HEIN_CARD_FROM_TIME));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.TDL_HEIN_CARD_TO_TIME, rdo.Treatment.TDL_HEIN_CARD_TO_TIME));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.IN_TIME, rdo.Treatment.IN_TIME));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.OUT_TIME, rdo.Treatment.OUT_TIME));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.ICD_CODE, rdo.Treatment.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.ICD_NAME, rdo.Treatment.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.ICD_SUB_CODE, rdo.Treatment.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000478ExtendSingleKey.ICD_TEXT, rdo.Treatment.ICD_TEXT));
                }
                if (rdo.lstExpMestMedicine != null && rdo.lstExpMestMedicine.Count() > 0)
                {
                    this.lstServiceMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                    var groupExpMestMedicine = rdo.lstExpMestMedicine.GroupBy(o => o.MEDICINE_TYPE_ID);
                    foreach (var itemGroup in groupExpMestMedicine)
                    {
                        V_HIS_EXP_MEST_MEDICINE serviceMedicines = new V_HIS_EXP_MEST_MEDICINE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO>(serviceMedicines, itemGroup.First());
                        serviceMedicines.AMOUNT = itemGroup.Sum(o => o.AMOUNT) - itemGroup.Sum(o => o.TH_AMOUNT ?? 0);
                        serviceMedicines.TDL_INTRUCTION_TIME = itemGroup.OrderBy(o => o.TDL_INTRUCTION_TIME).First().TDL_INTRUCTION_TIME;
                        decimal totalOfDay = 0;
                        var itemGroupByDay = itemGroup.GroupBy(o => o.TDL_INTRUCTION_TIME);
                        foreach (var item in itemGroupByDay)
                        {
                            var dt = item.Sum(o => Inventec.Common.TypeConvert.Parse.ToDecimal(o.EVENING ?? "0")) + item.Sum(o => Inventec.Common.TypeConvert.Parse.ToDecimal(o.MORNING ?? "0")) + item.Sum(o => Inventec.Common.TypeConvert.Parse.ToDecimal(o.AFTERNOON ?? "0")) + item.Sum(o => Inventec.Common.TypeConvert.Parse.ToDecimal(o.NOON ?? "0"));
                            totalOfDay += dt > 0 ? (item.Sum(o => o.AMOUNT) - item.Sum(o => o.TH_AMOUNT ?? 0)) / dt : 1;
                        }
                        serviceMedicines.DAY_COUNT = (long?)Math.Ceiling(totalOfDay);
                        this.lstServiceMedicines.Add(serviceMedicines);
                    }
                }
                if(rdo.lstSereServ !=null && rdo.lstSereServ.Count > 0)
                {
                    listSereServ = rdo.lstSereServ.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT).ToList();
                    listSereServBlood = rdo.lstSereServ.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM).ToList();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
