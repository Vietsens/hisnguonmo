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
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000091
{
    /// <summary>
    /// In yeu cau kham.
    /// Dau vao:
    /// PatyAlterBhyt: doi tuong the bhyt
    /// ServiceReq: yeu cau dich vu
    /// PatientType: doi tuong benh nhan
    /// </summary>
    public class Mps000091RDO : RDOBase
    {
        internal PatientADO currentPatient { get; set; }
        internal V_HIS_DEPOSIT_REQ depositReq { get; set; }
        internal PatyAlterBhytADO patyAlterBhytADO { get; set; }

        public Mps000091RDO(
            PatientADO currentPatient,
            V_HIS_DEPOSIT_REQ depositReq,
            PatyAlterBhytADO patyAlterBhytADO)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.depositReq = depositReq;
                this.patyAlterBhytADO = patyAlterBhytADO;
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
                if (depositReq != null)
                {
                    keyValues.Add(new KeyValue(Mps000091ExtendSingleKey.STR_AMOUNT, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(depositReq.AMOUNT)));
                    keyValues.Add(new KeyValue(Mps000091ExtendSingleKey.STR_AMOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(depositReq.AMOUNT).ToString())));
                }
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(currentPatient, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPOSIT_REQ>(depositReq, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        
    }
}
