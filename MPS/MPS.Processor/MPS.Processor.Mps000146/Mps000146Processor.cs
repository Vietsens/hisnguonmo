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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000146.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000146
{
    public class Mps000146Processor : AbstractProcessor
    {
        Mps000146PDO rdo;
        public Mps000146Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000146PDO)rdoBase;
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
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                ProcessSingleKey();
                this.SetSignatureKeyImageByCFG();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ServicesInfusion", rdo._ListAdo);
                objectTag.AddObjectData(store, "MixedMedicines", rdo._ListMixedMedicines);
                objectTag.AddRelationship(store, "ServicesInfusion", "MixedMedicines", "ID", "INFUSION_ID");
                objectTag.SetUserFunction(store, "FuncSameTitleCol", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol2", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol3", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol4", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol5", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol6", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol7", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol8", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol9", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol10", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol11", new CustomerFuncMergeSameData());
                objectTag.SetUserFunction(store, "FuncSameTitleCol12", new CustomerFuncMergeSameData());
                store.SetCommonFunctions();


                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo._Treatment != null && rdo._ListInfusion != null && rdo._InfusionSum != null)
                    result = String.Format("PRINT_TYPE_CODE:{0} TREATMENT_CODE:{1} HIS_INFUSION:{2} ICD_CODE:{3}", printTypeCode, rdo._Treatment.TREATMENT_CODE, string.Join(",", rdo._ListInfusion.Select(o => o.ID).ToList()), rdo._InfusionSum.ICD_CODE);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                string departmentName = "";
                string departmentCode = "";
                string roomCode = "";
                string roomName = "";
                string strMixedMedicine = "";
                if (rdo._InfusionSum != null)
                {
                    string icdName = String.IsNullOrEmpty(rdo._InfusionSum.ICD_NAME) ? rdo._InfusionSum.ICD_NAME : rdo._InfusionSum.ICD_NAME;
                    departmentName = rdo._InfusionSum.DEPARTMENT_NAME;
                    departmentCode = rdo._InfusionSum.DEPARTMENT_CODE;
                    roomCode = rdo._InfusionSum.ROOM_CODE;
                    roomName = rdo._InfusionSum.ROOM_NAME;
                    SetSingleKey(new KeyValue(Mps000146ExtendSingleKey.ICD_NAME, icdName));
                    AddObjectKeyIntoListkey<V_HIS_INFUSION_SUM>(rdo._InfusionSum, false);
                }
                if (rdo._Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000146ExtendSingleKey.GENDER_NAME, rdo._Treatment.TDL_PATIENT_GENDER_NAME));

                    SetSingleKey(new KeyValue(Mps000146ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.Age(rdo._Treatment.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000146ExtendSingleKey.YEAR, rdo._Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_2>(rdo._Treatment, false);
                }

                if (rdo._ListInfusion != null && rdo._ListInfusion.Count > 0)
                {
                    foreach (var item in rdo._ListInfusion)
                    {
                        MPS.Processor.Mps000146.PDO.Mps000146PDO.Mps000146ADO ado = new MPS.Processor.Mps000146.PDO.Mps000146PDO.Mps000146ADO(item);
                        ado.EXECUTE_DEPARTMENT_NAME = departmentName;
                        ado.EXECUTE_DEPARTMENT_CODE = departmentCode;
                        ado.EXECUTE_ROOM_CODE = roomCode;
                        ado.EXECUTE_ROOM_NAME = roomName;
                        if (item.MEDICINE_ID != null && item.MEDICINE_ID > 0)
                        {
                            HIS_MEDICINE medi = BackendDataWorker.Get<HIS_MEDICINE>().FirstOrDefault(md => md.ID == item.MEDICINE_ID);
                            if (medi != null)
                            {
                                var medicineType = rdo._ListMedicineType.FirstOrDefault(o => o.ID == medi.MEDICINE_TYPE_ID);
                                
                                if (medicineType != null)
                                {
                                    ado.CONCENTRA = medicineType.CONCENTRA;
                                    ado.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                                }
                            }
                        }
                        // minhnq
                        if (rdo.lstMixedMedicine != null && rdo.lstMixedMedicine.Count > 0)
                        {
                            strMixedMedicine = string.Join(";", rdo.lstMixedMedicine.Select(o => o.MEDICINE_TYPE_NAME).ToList());
                            foreach (var mixedMedicine in rdo.lstMixedMedicine)
                            {
                                if (mixedMedicine.INFUSION_ID == item.ID)
                                {
                                    MPS.Processor.Mps000146.PDO.Mps000146PDO.Mps000146ADO mixedMedicineAdo = new MPS.Processor.Mps000146.PDO.Mps000146PDO.Mps000146ADO(mixedMedicine);

                                    var medicineType = rdo._ListMedicineType.FirstOrDefault(o => o.ID == mixedMedicine.MEDICINE_TYPE_ID);
                                    mixedMedicineAdo.CONCENTRA = medicineType != null ? medicineType.CONCENTRA : null;
                                    rdo._ListMixedMedicines.Add(mixedMedicineAdo);
                                }
                            }
                        }


                        if (!rdo._ListMixedMedicines.Exists(o => o.INFUSION_ID == item.ID))
                        {
                            MPS.Processor.Mps000146.PDO.Mps000146PDO.Mps000146ADO mixedMedicineAdo = new MPS.Processor.Mps000146.PDO.Mps000146PDO.Mps000146ADO();
                            mixedMedicineAdo.INFUSION_ID = item.ID;
                            rdo._ListMixedMedicines.Add(mixedMedicineAdo);
                        }

                        ado.MIXED_MEDICINE = strMixedMedicine;
                        rdo._ListAdo.Add(ado);
                    }

                    long? startTime = rdo._ListInfusion.OrderBy(o => o.START_TIME).FirstOrDefault().START_TIME;
                    long? finishTime = rdo._ListInfusion.OrderByDescending(o => o.FINISH_TIME).FirstOrDefault().FINISH_TIME;
                    SetSingleKey(new KeyValue(Mps000146ExtendSingleKey.START_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(startTime ?? 0)));
                    SetSingleKey(new KeyValue(Mps000146ExtendSingleKey.FINISH_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(finishTime ?? 0)));

                    rdo._ListAdo = rdo._ListAdo.OrderBy(o => o.START_TIME).ToList();
                }



                if (rdo.TreatmentBedRoom != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_BED_ROOM>(rdo.TreatmentBedRoom, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //class CalculateMergerData : TFlexCelUserFunction
        //{
        //    long typeId = 0;
        //    long mediMateTypeId = 0;

        //    public override object Evaluate(object[] parameters)
        //    {
        //        if (parameters == null || parameters.Length <= 0)
        //            throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
        //        bool result = false;
        //        try
        //        {
        //            long servicetypeId = Convert.ToInt64(parameters[0]);
        //            long mediMateId = Convert.ToInt64(parameters[1]);

        //            if (servicetypeId > 0 && mediMateId > 0)
        //            {
        //                if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
        //                {
        //                    return true;
        //                }
        //                else
        //                {
        //                    this.typeId = servicetypeId;
        //                    this.mediMateTypeId = mediMateId;
        //                    return false;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Error(ex);
        //            result = false;
        //        }
        //        return result;
        //    }
        //}

        class CustomerFuncMergeSameData : TFlexCelUserFunction
        {
            long InfusionId;
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    long _InfusionId = Convert.ToInt64(parameters[0]);
                    if (_InfusionId != null)
                    {
                        if (InfusionId == _InfusionId)
                        {
                            return true;
                        }
                        else
                        {
                            InfusionId = _InfusionId;
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                    Inventec.Common.Logging.LogSystem.Debug(ex);
                }

                return result;
            }
        }
    }
}
