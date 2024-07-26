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
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.Base
{
    public class PatientTypeAlterADO : HIS_PATIENT_TYPE_ALTER
    {
        public long HEIN_CARD_TO_TIME_CAL { get; set; }
        public string RENDERER_HEIN_CARD_NUMBER { get; set; }
        public string RENDERER_FROM_DATE_TODATE { get; set; }
        public PatientTypeAlterADO(HIS_PATIENT_TYPE_ALTER data)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_TYPE_ALTER>(this, data);

                if (data != null)
                {
                    string chkhong = "", chmot = "", chhai = "", chba = "", chbon = "", chnam = "";
                    try
                    {
                        chkhong = data.HEIN_CARD_NUMBER.Substring(0, 2);
                        chmot = data.HEIN_CARD_NUMBER.Substring(2, 1);
                        chhai = data.HEIN_CARD_NUMBER.Substring(3, 2);
                        chba = data.HEIN_CARD_NUMBER.Substring(5, 2);
                        chbon = data.HEIN_CARD_NUMBER.Substring(7, 3);
                        chnam = data.HEIN_CARD_NUMBER.Substring(10, 5);
                    }
                    catch (Exception exx)
                    {
                        LogSystem.Warn("Gan chuoi RENDERER_HEIN_CARD_NUMBER the BHYT loi", exx);
                    }

                    this.RENDERER_HEIN_CARD_NUMBER = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", chkhong, chmot, chhai, chba, chbon, chnam);
                }

                if (data != null && data.HEIN_CARD_FROM_TIME > 0 && data.HEIN_CARD_TO_TIME > 0)
                {
                    string tu = Inventec.Common.DateTime.Convert.TimeNumberToDateString((data.HEIN_CARD_FROM_TIME ?? 0));
                    string den = Inventec.Common.DateTime.Convert.TimeNumberToDateString((data.HEIN_CARD_TO_TIME ?? 0));
                    this.RENDERER_FROM_DATE_TODATE = "" + tu + " - " + den;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
