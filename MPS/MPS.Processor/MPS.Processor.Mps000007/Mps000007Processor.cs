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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000007.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000007.ADO;

namespace MPS.Processor.Mps000007
{
    public partial class Mps000007Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        private PatyAlterBhytADO patyAlter { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> ExpMestMaterials { get; set; }
        List<ExpMestBloods> ExpMestBloods { get; set; }

        Mps000007PDO rdo;
        public Mps000007Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000007PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.Treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000007ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                DataInputProcess();
                ProcessSingleKey();
                SetBarcodeKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ExpMestMedicines", this.ExpMestMedicines);
                objectTag.AddObjectData(store, "ExpMestMaterials", this.ExpMestMaterials);
                objectTag.AddObjectData(store, "ExpMesBloods", this.ExpMestBloods);
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
                SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.RATIO_STR, rdo.SingleKeyValue.RatioText));
                SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.EXECUTE_DEPARTMENT_NAME, rdo.SingleKeyValue.ExecuteDepartmentName));
                SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.EXECUTE_ROOM_NAME, rdo.SingleKeyValue.ExecuteRoomName));
                SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.HOSPITALIZE_DEPARTMENT_CODE, rdo.SingleKeyValue.HospitalizeDepartmentCode));
                SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.HOSPITALIZE_DEPARTMENT_NAME, rdo.SingleKeyValue.HospitalizeDepartmentName));

                if (rdo.SereServs != null)
                {
                    List<HIS_SERE_SERV> clsName = new List<HIS_SERE_SERV>();
                    clsName.AddRange(rdo.SereServs);
                    if (rdo.Treatment != null && rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        clsName = clsName.Where(o => o.TDL_INTRUCTION_TIME <= rdo.Treatment.CLINICAL_IN_TIME.Value).ToList();
                    }

                    SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.CLS_NAMES, string.Join("; ", clsName.Select(o => o.TDL_SERVICE_NAME).Distinct().ToList())));
                }

                if (rdo.Treatment != null)
                {
                    if (rdo.Treatment.CLINICAL_IN_TIME != null)
                    {

                        SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value))));

                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.IN_TIME))));
                    }
                }

                if (rdo.ExamServiceReq != null)
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ExamServiceReq.INTRUCTION_TIME))));
                if (!String.IsNullOrEmpty(rdo.Treatment.TRANSFER_IN_MEDI_ORG_CODE))
                {
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ICD_NGT_TEXT, rdo.Treatment.TRANSFER_IN_ICD_NAME)));
                }
                SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ICD_DEPARTMENT_TRAN, rdo.SingleKeyValue.Icd_Name)));

                if (rdo.PatyAlter != null)
                {
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlter.HEIN_CARD_FROM_TIME.ToString()))));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.HEIN_CARD_TO_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlter.HEIN_CARD_TO_TIME.ToString()))));
                    SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.PatyAlter.ADDRESS));
                }

                SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.LOGIN_USER_NAME, rdo.SingleKeyValue.Username)));
                SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.LOGIN_NAME, rdo.SingleKeyValue.LoginName)));


                if (rdo._currentPatient != null)
                {
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._currentPatient.DOB))));
                }

                if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                {
                    rdo.DepartmentTrans = rdo.DepartmentTrans.OrderByDescending(o => o.DEPARTMENT_IN_TIME).ToList();
                    V_HIS_DEPARTMENT_TRAN departmentTran = null;
                    if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                    {
                        var departmentTranTimeNull = rdo.DepartmentTrans.FirstOrDefault(o => o.DEPARTMENT_IN_TIME == null);
                        if (departmentTranTimeNull != null)
                            departmentTran = departmentTranTimeNull;
                        else
                            departmentTran = rdo.DepartmentTrans[0];
                    }
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.NEXT_DEPARTMENT_CODE, departmentTran.DEPARTMENT_CODE)));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.NEXT_DEPARTMENT_NAME, departmentTran.DEPARTMENT_NAME)));

                }

                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment, false);
                AddObjectKeyIntoListkey<PatientADO>(patientADO, false);

                if (rdo.ExamServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ExamServiceReq, false);
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.PROVISIONAL_DIAGNOSIS, rdo.ExamServiceReq.PROVISIONAL_DIAGNOSIS)));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ORIGINAL_ICD_CODE, rdo.ExamServiceReq.ICD_CODE)));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ORIGINAL_ICD_NAME, rdo.ExamServiceReq.ICD_NAME)));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ORIGINAL_ICD_SUB_CODE, rdo.ExamServiceReq.ICD_SUB_CODE)));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ORIGINAL_ICD_TEXT, rdo.ExamServiceReq.ICD_TEXT)));

                }
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlter, false);
                AddObjectKeyIntoListkey<HIS_DHST>(rdo.DHST, false);
                if (rdo.DHST != null)
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.DHST_NOTE, rdo.DHST.NOTE)));
                //AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(rdo.TranPati, false);
                //AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo.DepartmentTran,false);
                ExpMestBloods = new List<ExpMestBloods>();
                if (rdo.ExpMestBloodList != null && rdo.ExpMestBloodList.Count > 0)
                {
                    string blood = "";
                    var groupBlood = rdo.ExpMestBloodList.GroupBy(o => new { o.BLOOD_TYPE_ID, o.VOLUME }).ToList();
                    foreach (var item in groupBlood)
                    {
                        ExpMestBloods expBlood = new ExpMestBloods();
                        List<V_HIS_EXP_MEST_BLTY_REQ> blty = null;
                        decimal amount = 0;
                        if (rdo.ExpMestBltyReqList != null && rdo.ExpMestBltyReqList.Count > 0)
                        {
                            blty = rdo.ExpMestBltyReqList.Where(o => item.Select(p => p.EXP_MEST_BLTY_REQ_ID).Contains(o.ID)).ToList();
                        }
                        amount = blty != null && blty.Count > 0 ? blty.Sum(o => o.AMOUNT) : 0;

                        expBlood.BLOOD_TYPE_NAME = item.FirstOrDefault().BLOOD_TYPE_NAME;
                        expBlood.VOLUME = item.FirstOrDefault().VOLUME;
                        expBlood.AMOUNT = amount;

                        expBlood.DESCRIPTION = String.Format("Tên: {0} - DT: {1} - SL: {2}; ", item.FirstOrDefault().BLOOD_TYPE_NAME, item.FirstOrDefault().VOLUME, amount);

                        ExpMestBloods.Add(expBlood);
                        blood += String.Format("Tên: {0} - DT: {1} - SL: {2}; ", item.FirstOrDefault().BLOOD_TYPE_NAME, item.FirstOrDefault().VOLUME, amount);
                    }
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.EXP_MEST_BLOOD_LIST, blood)));
                }
                this.ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                this.ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                if (rdo.ExpMestMedicineList != null && rdo.ExpMestMedicineList.Count > 0)
                {
                    string medicine = "";
                    var groupMedicine = rdo.ExpMestMedicineList.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.CONCENTRA }).ToList();
                    foreach (var item in groupMedicine)
                    {
                        V_HIS_EXP_MEST_MEDICINE expMedicine = new V_HIS_EXP_MEST_MEDICINE();
                        expMedicine.MEDICINE_TYPE_NAME = item.FirstOrDefault().MEDICINE_TYPE_NAME;
                        expMedicine.CONCENTRA = item.FirstOrDefault().CONCENTRA;
                        expMedicine.AMOUNT = item.Sum(o => o.AMOUNT);
                        expMedicine.SERVICE_UNIT_NAME = item.FirstOrDefault().SERVICE_UNIT_NAME;
                        expMedicine.TUTORIAL = item.FirstOrDefault().TUTORIAL;

                        expMedicine.DESCRIPTION = String.Format("Tên: {0} - HL: {1} - SL: {2} - ĐVT: {3} - HDSD: {4}; ", item.FirstOrDefault().MEDICINE_TYPE_NAME
                            , item.FirstOrDefault().CONCENTRA
                            , item.Sum(o => o.AMOUNT)
                            , item.FirstOrDefault().SERVICE_UNIT_NAME
                            , item.FirstOrDefault().TUTORIAL);

                        this.ExpMestMedicines.Add(expMedicine);

                        medicine += String.Format("Tên: {0} - HL: {1} - SL: {2} - ĐVT: {3} - HDSD: {4}; ", item.FirstOrDefault().MEDICINE_TYPE_NAME
                            , item.FirstOrDefault().CONCENTRA
                            , item.Sum(o => o.AMOUNT)
                            , item.FirstOrDefault().SERVICE_UNIT_NAME
                            , item.FirstOrDefault().TUTORIAL);
                    }
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.EXP_MEST_MEDICINE_LIST, medicine)));
                }


                if (rdo.ExpMestMaterialList != null && rdo.ExpMestMaterialList.Count > 0)
                {
                    string material = "";
                    var groupMaterial = rdo.ExpMestMaterialList.GroupBy(o => new { o.TDL_MATERIAL_TYPE_ID }).ToList();
                    foreach (var item in groupMaterial)
                    {
                        V_HIS_EXP_MEST_MATERIAL expMaterial = new V_HIS_EXP_MEST_MATERIAL();
                        expMaterial.MATERIAL_TYPE_NAME = item.FirstOrDefault().MATERIAL_TYPE_NAME;
                        expMaterial.AMOUNT = item.Sum(o => o.AMOUNT);
                        expMaterial.SERVICE_UNIT_NAME = item.FirstOrDefault().SERVICE_UNIT_NAME;

                        expMaterial.DESCRIPTION = String.Format("Tên: {0} - SL: {1} - ĐVT: {2}; ", item.FirstOrDefault().MATERIAL_TYPE_NAME
                            , item.Sum(o => o.AMOUNT)
                            , item.FirstOrDefault().SERVICE_UNIT_NAME);
                        this.ExpMestMaterials.Add(expMaterial);

                        material += String.Format("Tên: {0} - SL: {1} - ĐVT: {2}; ", item.FirstOrDefault().MATERIAL_TYPE_NAME
                            , item.Sum(o => o.AMOUNT)
                            , item.FirstOrDefault().SERVICE_UNIT_NAME);
                    }
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.EXP_MEST_MATERIAL_LIST, material)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
