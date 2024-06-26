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
using MPS.Processor.Mps000156.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000156
{
    public class Mps000156Processor : AbstractProcessor
    {
        Mps000156PDO rdo;
        public Mps000156Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000156PDO)rdoBase;
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
                objectTag.AddObjectData(store, "ServiceMediReact", rdo.expMestMediReact);

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
                if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.VIR_PATIENT_NAME, rdo.currentTreatment.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.ICD_CODE, rdo.currentTreatment.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.ICD_SUB_CODE, rdo.currentTreatment.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.ICD_TEXT, rdo.currentTreatment.ICD_TEXT));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.currentTreatment.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.GENDER_NAME, rdo.currentTreatment.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.VIR_ADDRESS, rdo.currentTreatment.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.EXECUTE_DEPARTMENT_NAME, rdo.thisRoom.DEPARTMENT_NAME));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.EXECUTE_ROOM_NAME, rdo.thisRoom.ROOM_NAME));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.ICD_MAIN_TEXT, rdo.currentTreatment.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.IN_CODE, rdo.currentTreatment.IN_CODE));
                }
                if (rdo.bedLog != null)
                {
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.BED_CODE, rdo.bedLog.BED_CODE));
                    SetSingleKey(new KeyValue(Mps000156ExtendSingleKey.BED_NAME, rdo.bedLog.BED_NAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class CalculateMergerData : TFlexCelUserFunction
        {
            long typeId = 0;
            long mediMateTypeId = 0;

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    long servicetypeId = Convert.ToInt64(parameters[0]);
                    long mediMateId = Convert.ToInt64(parameters[1]);

                    if (servicetypeId > 0 && mediMateId > 0)
                    {
                        if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
                        {
                            return true;
                        }
                        else
                        {
                            this.typeId = servicetypeId;
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
