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
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000173.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000173
{
    class Mps000173PDOBase : Mps000173PDO
    {
        public Mps000173PDOBase(V_HIS_TREATMENT_2 treatment, HIS_PATY_ALTER_BHYT patyAlterBhyt)
            : base(treatment, patyAlterBhyt)
        {

        }
        public override void SetSingleKey()
        {
            try
            {
                if (this._Treatment != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT_2>(this._Treatment, keyValues, false);
                    keyValues.Add(new KeyValue(Mps000173ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Treatment.DOB)));
                    keyValues.Add(new KeyValue(Mps000173ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._Treatment.CREATE_TIME ?? 0)));
                }
                else
                {
                    throw new Exception("Nguoi dung khong truyen vao V_HIS_DEPOSIT: Mps000173. ");
                }

                if (this._PatyAlterBhyt != null)
                {
                    MPS.ProcessorBase.GlobalQuery.AddObjectKeyIntoListkey<HIS_PATY_ALTER_BHYT>(this._PatyAlterBhyt, keyValues, false);
                    keyValues.Add(new KeyValue(Mps000173ExtendSingleKey.HEIN_CARD_FROM_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._PatyAlterBhyt.HEIN_CARD_FROM_TIME)));
                    keyValues.Add(new KeyValue(Mps000173ExtendSingleKey.HEIN_CARD_TO_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._PatyAlterBhyt.HEIN_CARD_TO_TIME)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
