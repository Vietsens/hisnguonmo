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
using AutoMapper;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.ADO
{
    public class SereServADO : V_HIS_SERE_SERV
    {
        public decimal? VIR_TOTAL_HEIN_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PATIENT_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PRICE_NO_EXPEND_SUM { get; set; }
        public decimal? PRICE_BHYT { get; set; }
        public long? SERVICE_PACKAGE_ID { get; set; }
        public string DEPARTMENT__SERVICE_GROUP__ID { get; set; }

        public string patientIdQr { get; set; }
        public byte[] bPatientQr { get; set; }

        public string patientNameQr { get; set; }
        public byte[] bPatientNameQr { get; set; }

        public string studyDescriptionQr { get; set; }
        public byte[] bStudyDescriptionQr { get; set; }

        public SereServADO()
        {

        }
        public SereServADO(V_HIS_SERE_SERV data)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                SetSumServiceChildInServicePackage(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetSumServiceChildInServicePackage(V_HIS_SERE_SERV data)
        {
            try
            {
                VIR_TOTAL_PRICE_NO_EXPEND_SUM = null;

               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
