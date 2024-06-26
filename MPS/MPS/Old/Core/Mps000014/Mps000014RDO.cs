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

namespace MPS.Core.Mps000014
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000014RDO : RDOBase
    {
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal PatientADO Patient { get; set; }
        internal V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter { get; set; }
        internal V_HIS_TRAN_PATI TranPaties { get; set; }
        internal List<MPS.ADO.ExeSereServ> ExesereServs { get; set; }
        internal V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        internal List<MPS.ADO.HisSereServTeinADO> lstSereServTeinPrint { get; set; }

        public Mps000014RDO(
            PatientADO patient,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            PatyAlterBhytADO patyAlterBhyt,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter,
            V_HIS_TRAN_PATI tranPaties,
            List<MPS.ADO.ExeSereServ> ExesereServs
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.ServiceReqPrint = ServiceReqPrint;
                this.currentHispatientTypeAlter = currentHispatientTypeAlter;
                this.TranPaties = tranPaties;
                this.ExesereServs = ExesereServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000014RDO(
            PatientADO patient,
            PatyAlterBhytADO patyAlterBhyt,
            List<MPS.ADO.HisSereServTeinADO> lstSereServTeinPrint
            )
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.lstSereServTeinPrint = lstSereServTeinPrint;
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
                if (PatyAlterBhyt != null)
                {
                    if (PatyAlterBhyt.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    //Dia chi the
                    keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.HEIN_CARD_ADDRESS, PatyAlterBhyt.ADDRESS));
                }
                else
                    keyValues.Add(new KeyValue(Mps000014ExtendSingleKey.IS_NOT_HEIN, "X"));

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(departmentTran, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(currentHispatientTypeAlter, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(TranPaties, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReqPrint, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
