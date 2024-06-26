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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PregnancyRest.ADO
{
    class BabyADO : HisBabySDO
    {
        public DateTime? BornTimeDt { get; set; }

        public BabyADO()
        {
        }

        public BabyADO(HisBabySDO data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<BabyADO>(this, data);
            if (!string.IsNullOrEmpty(data.EthnicCode))
            {
                var ethnic = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == data.EthnicCode);
                if (ethnic != null)
                    this.EthnicName = ethnic.ETHNIC_NAME;
            }
            this.BornTimeDt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.BornTime ?? 0);
        }

        public BabyADO(MOS.EFMODEL.DataModels.HIS_BABY data)
        {
            try
            {
                if (data != null)
                {
                    this.BabyName = data.BABY_NAME;
                    this.BabyOrder = data.BABY_ORDER;
                    this.BornPositionId = data.BORN_POSITION_ID;
                    this.BornResultId = data.BORN_RESULT_ID;
                    this.BornTime = data.BORN_TIME;
                    this.BornTimeDt = data.BORN_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.BORN_TIME ?? 0) : null;
                    this.BornTypeId = data.BORN_TYPE_ID;
                    this.EthnicCode = data.ETHNIC_CODE;
                    this.EthnicName = data.ETHNIC_NAME;
                    this.FatherName = data.FATHER_NAME;
                    this.GenderId = data.GENDER_ID;
                    this.Head = data.HEAD;
                    this.Height = data.HEIGHT;
                    this.Midwife = data.MIDWIFE;
                    this.MonthCount = data.MONTH_COUNT;
                    this.WeekCount = data.WEEK_COUNT;
                    this.Weight = data.WEIGHT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
