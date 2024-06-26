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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000146
{
    public class Mps000146RDO : RDOBase
    {
        V_HIS_INFUSION_SUM _InfusionSum = null;
        V_HIS_TREATMENT_2 _Treatment = null;
        List<V_HIS_INFUSION> _ListInfusion = null;

        internal List<Mps000146ADO> _ListAdo = new List<Mps000146ADO>();

        public Mps000146RDO(V_HIS_INFUSION_SUM infusionSum, V_HIS_TREATMENT_2 treatment, List<V_HIS_INFUSION> listInfusion)
        {
            this._InfusionSum = infusionSum;
            this._ListInfusion = listInfusion;
            this._Treatment = treatment;
        }

        internal override void SetSingleKey()
        {
            try
            {
                string departmentName = "";
                string departmentCode = "";
                string roomCode = "";
                string roomName = "";
                if (this._InfusionSum != null)
                {
                    string icdName = String.IsNullOrEmpty(this._InfusionSum.ICD_MAIN_TEXT) ? this._InfusionSum.ICD_NAME : this._InfusionSum.ICD_MAIN_TEXT;
                    departmentName = this._InfusionSum.DEPARTMENT_NAME;
                    departmentCode = this._InfusionSum.DEPARTMENT_CODE;
                    roomCode = this._InfusionSum.ROOM_CODE;
                    roomName = this._InfusionSum.ROOM_NAME;
                    keyValues.Add(new KeyValue(Mps000146ExtendSingleKey.ICD_NAME, icdName));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_INFUSION_SUM>(this._InfusionSum, keyValues, false);
                }
                if (this._Treatment != null)
                {
                    keyValues.Add(new KeyValue(Mps000146ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.Age(this._Treatment.DOB)));
                    keyValues.Add(new KeyValue(Mps000146ExtendSingleKey.YEAR, this._Treatment.DOB.ToString().Substring(0, 4)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT_2>(this._Treatment, keyValues, false);
                }

                if (this._ListInfusion != null && this._ListInfusion.Count > 0)
                {
                    foreach (var item in this._ListInfusion)
                    {
                        Mps000146ADO ado = new Mps000146ADO(item);
                        ado.EXECUTE_DEPARTMENT_NAME = departmentName;
                        ado.EXECUTE_DEPARTMENT_CODE = departmentCode;
                        ado.EXECUTE_ROOM_CODE = roomCode;
                        ado.EXECUTE_ROOM_NAME = roomName;
                        _ListAdo.Add(ado);
                    }
                    _ListAdo = _ListAdo.OrderBy(o => o.CREATE_TIME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
