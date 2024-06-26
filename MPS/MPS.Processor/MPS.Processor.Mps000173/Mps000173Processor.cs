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
using MPS.Processor.Mps000173.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000173
{
    class Mps000173Processor : AbstractProcessor
    {
        Mps000173PDO rdo;
        public Mps000173Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000173PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                this.SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public void SetSingleKey()
        {
            try
            {

                if (rdo._PatyAlterBhyt != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatyAlterBhyt, false);
                    SetSingleKey(new KeyValue(Mps000173ExtendSingleKey.HEIN_CARD_FROM_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000173ExtendSingleKey.HEIN_CARD_TO_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                }

                if (rdo._Treatment != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(rdo._Treatment, false);
                    //SetSingleKey(new KeyValue(Mps000173ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Treatment.DOB)));
                    SetSingleKey(new KeyValue(Mps000173ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Treatment.CREATE_TIME ?? 0)));
                }
                else
                {
                    throw new Exception("Nguoi dung khong truyen vao V_HIS_TREATMENT_FEE: Mps000173. ");
                }
                if (rdo._DepartmentTran != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo._DepartmentTran, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
