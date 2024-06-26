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
 
using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;
using MOS.SDO;

namespace MPS.Core.Mps000104
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000104RDO : RDOBase
    {
        internal PatientADO patientADO { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_BILL hisBill {get;set;}
        
        public Mps000104RDO(
            PatientADO patientADO,
            MOS.EFMODEL.DataModels.V_HIS_BILL hisBill

            )
        {
            try
            {
                this.patientADO = patientADO;
                this.hisBill = hisBill;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (hisBill != null)
                {
                    string amount = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.hisBill.AMOUNT));
                    string amountSeparate = Inventec.Common.Number.Convert.NumberToStringRoundMax4(this.hisBill.AMOUNT);
                    keyValues.Add(new KeyValue(Mps000104ExtendSingleKey.AMOUNT, amountSeparate));
                    keyValues.Add(new KeyValue(Mps000104ExtendSingleKey.AMOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(amount)));
                    keyValues.Add(new KeyValue(Mps000104ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST,UppercaseFirst(Inventec.Common.String.Convert.CurrencyToVneseString(amount))));

                    string exemptionSeparate = Inventec.Common.Number.Convert.NumberToStringRoundMax4((this.hisBill.EXEMPTION ?? 0) * 100 / this.hisBill.AMOUNT);
                    keyValues.Add(new KeyValue(Mps000104ExtendSingleKey.HEIN_RATIO, exemptionSeparate));

                }
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_BILL>(hisBill, keyValues, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

    }
}
