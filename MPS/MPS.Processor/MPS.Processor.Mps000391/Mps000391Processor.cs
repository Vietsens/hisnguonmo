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
using MPS.Processor.Mps000391.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000391
{
    public class Mps000391Processor : AbstractProcessor
    {
        Mps000391PDO rdo;
        public Mps000391Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000391PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
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

        private void SetSingleKey()
        {
            try
            {
                if (rdo != null)
                {
                    if (rdo.Treatment != null)
                    {
                        SetSingleKey((new KeyValue(Mps000391ExtendSingleKey.DOB_YEAR, rdo.Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4))));
                        SetSingleKey((new KeyValue(Mps000391ExtendSingleKey.GENDER_MALE, rdo.Treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? "X" : "")));
                        SetSingleKey((new KeyValue(Mps000391ExtendSingleKey.GENDER_FEMALE, rdo.Treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE ? "X" : "")));
                        SetSingleKey((new KeyValue(Mps000391ExtendSingleKey.IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Treatment.IN_TIME))));
                        SetSingleKey((new KeyValue(Mps000391ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Treatment.CLINICAL_IN_TIME ?? 0))));
                        AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.Treatment, false);
                    }

                    if (rdo.SereServPttt != null)
                    {
                        AddObjectKeyIntoListkey<V_HIS_SERE_SERV_PTTT>(rdo.SereServPttt, false);
                    }

                    if (rdo.SereServExt != null)
                    {
                        if (rdo.SereServExt.BEGIN_TIME.HasValue)
                        {
                            SetSingleKey((new KeyValue(Mps000391ExtendSingleKey.BEGIN_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.SereServExt.BEGIN_TIME.Value))));
                            string hour = string.Format("{0}:{1}", rdo.SereServExt.BEGIN_TIME.Value.ToString().Substring(8, 2), rdo.SereServExt.BEGIN_TIME.Value.ToString().Substring(10, 2));
                            SetSingleKey((new KeyValue(Mps000391ExtendSingleKey.BEGIN_HOUR_STR, hour)));
                        }

                        AddObjectKeyIntoListkey<HIS_SERE_SERV_EXT>(rdo.SereServExt, false);
                    }

                    if (rdo.EyeSurgryDesc != null)
                    {
                        AddObjectKeyIntoListkey<HIS_EYE_SURGRY_DESC>(rdo.EyeSurgryDesc, false);
                    }

                    if (rdo.ListUser != null)
                    {
                        var roleGoup = rdo.ListUser.GroupBy(o => o.EXECUTE_ROLE_CODE).ToList();
                        foreach (var item in roleGoup)
                        {
                            SetSingleKey(new KeyValue("EXECUTE_ROLE_" + item.Key, string.Join(", ", item.Select(s => s.USERNAME))));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
