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
using MPS.Processor.Mps000141.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000141
{
    public class Mps000141Processor : AbstractProcessor
    {
        Mps000141PDO rdo;
        List<RecordingTransaction> recordTranList = new List<RecordingTransaction>();
        public Mps000141Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000141PDO)rdoBase;
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

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "RecordingTransaction1", this.recordTranList);
                objectTag.AddObjectData(store, "RecordingTransaction2", this.recordTranList);
                objectTag.AddObjectData(store, "RecordingTransaction3", this.recordTranList);
                objectTag.AddObjectData(store, "mediMaties1", rdo._ListAdo);
                objectTag.AddObjectData(store, "mediMaties2", rdo._ListAdo);
                objectTag.AddObjectData(store, "mediMaties3", rdo._ListAdo);
                objectTag.AddRelationship(store, "RecordingTransaction1", "mediMaties1", "IMP_MEST_MEDI_MATE_ID", "ID");
                objectTag.AddRelationship(store, "RecordingTransaction2", "mediMaties2", "IMP_MEST_MEDI_MATE_ID", "ID");
                objectTag.AddRelationship(store, "RecordingTransaction3", "mediMaties3", "IMP_MEST_MEDI_MATE_ID", "ID");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                this.recordTranList = new List<RecordingTransaction>();
                decimal sumPrice = 0;
                if (rdo._ImpMestMedicines != null && rdo._ImpMestMedicines.Count > 0)
                {
                    var groupMedicine = rdo._ImpMestMedicines.GroupBy(o => o.RECORDING_TRANSACTION).ToList();

                    foreach (var group in groupMedicine)
                    {
                        RecordingTransaction parent = new RecordingTransaction();
                        parent.IMP_MEST_MEDI_MATE_ID = group.FirstOrDefault().ID + "TH";
                        parent.NAME = group.FirstOrDefault().RECORDING_TRANSACTION;
                        parent.TOTAL = group.Sum(o => o.AMOUNT * o.PRICE.Value * (1 + (o.VAT_RATIO ?? 0)));
                        parent.TOTAL_DOCUMENT_PRICE = group.Sum(o => o.DOCUMENT_PRICE ?? 0);
                        this.recordTranList.Add(parent);
                        foreach (var item in group)
                        {
                            MPS.Processor.Mps000141.PDO.Mps000141PDO.Mps000141ADO Mps000141ADO = new MPS.Processor.Mps000141.PDO.Mps000141PDO.Mps000141ADO(item, this.rdo._Medicines, this.rdo._listImpSource);

                            if (rdo._ListMedicalContract != null && rdo._ListMedicalContract.Count > 0)
                            {
                                V_HIS_MEDICAL_CONTRACT MedicalContract = rdo._ListMedicalContract.FirstOrDefault(o => o.MEDICINE_ID == item.MEDICINE_ID);
                                if (MedicalContract != null)
                                {
                                    Mps000141ADO.MEDICAL_CONTRACT_CODE = MedicalContract.MEDICAL_CONTRACT_CODE;
                                    Mps000141ADO.MEDICAL_CONTRACT_NAME = MedicalContract.MEDICAL_CONTRACT_NAME;
                                    Mps000141ADO.DOCUMENT_SUPPLIER_NAME = MedicalContract.DOCUMENT_SUPPLIER_NAME;
                                    Mps000141ADO.VENTURE_AGREENING = MedicalContract.VENTURE_AGREENING;
                                }
                            }

                            rdo._ListAdo.Add(Mps000141ADO);
                            //rdo._ListAdo.Add(new MPS.Processor.Mps000141.PDO.Mps000141PDO.Mps000141ADO(item, this.rdo._Medicines, this.rdo._listImpSource));
                            if (!item.PRICE.HasValue)
                                continue;
                            sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.VAT_RATIO ?? 0));
                        }
                    }
                }
                if (rdo._ImpMestMaterials != null && rdo._ImpMestMaterials.Count > 0)
                {

                    var groupMaterial = rdo._ImpMestMaterials.GroupBy(o => o.RECORDING_TRANSACTION).ToList();

                    foreach (var group in groupMaterial)
                    {
                        RecordingTransaction parent = new RecordingTransaction();
                        parent.IMP_MEST_MEDI_MATE_ID = group.FirstOrDefault().ID + "VT";
                        parent.NAME = group.FirstOrDefault().RECORDING_TRANSACTION;
                        parent.TOTAL = group.Sum(o => o.AMOUNT * o.PRICE.Value * (1 + (o.VAT_RATIO ?? 0)));
                        parent.TOTAL_DOCUMENT_PRICE = group.Sum(o => o.DOCUMENT_PRICE ?? 0);
                        this.recordTranList.Add(parent);
                        foreach (var item in group)
                        {
                            MPS.Processor.Mps000141.PDO.Mps000141PDO.Mps000141ADO Mps000141ADO = new MPS.Processor.Mps000141.PDO.Mps000141PDO.Mps000141ADO(item, this.rdo._Materials, this.rdo._listImpSource);

                            if (rdo._ListMedicalContract != null && rdo._ListMedicalContract.Count > 0)
                            {
                                V_HIS_MEDICAL_CONTRACT MedicalContract = rdo._ListMedicalContract.FirstOrDefault(o => o.MATERIAL_ID == item.MATERIAL_ID);
                                if (MedicalContract != null)
                                {
                                    Mps000141ADO.MEDICAL_CONTRACT_CODE = MedicalContract.MEDICAL_CONTRACT_CODE;
                                    Mps000141ADO.MEDICAL_CONTRACT_NAME = MedicalContract.MEDICAL_CONTRACT_NAME;
                                    Mps000141ADO.DOCUMENT_SUPPLIER_NAME = MedicalContract.DOCUMENT_SUPPLIER_NAME;
                                    Mps000141ADO.VENTURE_AGREENING = MedicalContract.VENTURE_AGREENING;
                                }
                            }

                            rdo._ListAdo.Add(Mps000141ADO);

                            //rdo._ListAdo.Add(new MPS.Processor.Mps000141.PDO.Mps000141PDO.Mps000141ADO(item, this.rdo._Materials, this.rdo._listImpSource));
                            if (!item.PRICE.HasValue)
                                continue;
                            sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.VAT_RATIO ?? 0));
                        }
                    }

                }

                if (rdo._ManuImpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000141ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ManuImpMest.IMP_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000141ExtendSingleKey.DOCUMENT_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ManuImpMest.DOCUMENT_DATE ?? 0)));
                    SetSingleKey(new KeyValue(Mps000141ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ManuImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000141ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ManuImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000141ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._ManuImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000141ExtendSingleKey.SUM_PRICE, Inventec.Common.Number.Convert.NumberToString(sumPrice)));
                    string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumPrice));
                    SetSingleKey(new KeyValue(Mps000141ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                    decimal sumAfterDiscount = sumPrice - rdo._ManuImpMest.DISCOUNT ?? 0;
                    SetSingleKey(new KeyValue(Mps000141ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT, sumAfterDiscount));
                    string sumAfterDiscountString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumAfterDiscount));
                    SetSingleKey(new KeyValue(Mps000141ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumAfterDiscountString)));

                    AddObjectKeyIntoListkey(rdo._ManuImpMest, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
