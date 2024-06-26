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
using MPS.Processor.Mps000284.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000284
{
    public class Mps000284Processor : AbstractProcessor
    {
        Mps000284PDO rdo;
        public Mps000284Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000284PDO)rdoBase;
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
                ProcessSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

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

        private void ProcessSingleKey()
        {
            try
            {
                if (rdo.PatientView.CMND_NUMBER != null)
                {
                    SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.ID_DATE_STR, this.rdo.PatientView.CMND_DATE ?? 0));
                    SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.ID_NUMBER_STR, this.rdo.PatientView.CMND_NUMBER));
                    SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.ID_PLACE_STR, this.rdo.PatientView.CMND_PLACE));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.ID_DATE_STR, this.rdo.PatientView.CCCD_DATE ?? 0));
                    SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.ID_NUMBER_STR, this.rdo.PatientView.CCCD_NUMBER));
                    SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.ID_PLACE_STR, this.rdo.PatientView.CCCD_PLACE));
                }
                SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.HOSPITALIZATION_REASON, this.rdo.Treatment.HOSPITALIZATION_REASON));
                if (this.rdo.accidentHurt != null) 
                {
                    SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.ICD_CODE, this.rdo.accidentHurt.ICD_CODE ));
                    SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.ICD_NAME, this.rdo.accidentHurt.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.ICD_SUB_CODE, this.rdo.accidentHurt.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000284ExtendSingleKey.ICD_TEXT, this.rdo.accidentHurt.ICD_TEXT));
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
                AddObjectKeyIntoListkey(rdo.PatientView, false);
                AddObjectKeyIntoListkey(rdo.accidentHurt, false);
                AddObjectKeyIntoListkey(rdo.Treatment, false);
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
