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
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqList.ADO
{
    public class ListMedicineADO : MOS.EFMODEL.DataModels.HIS_SERE_SERV
    {
        public long ExpMestMedicineId { get; set; }
        public long NUM_ORDER { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public int kind { get; set; }
        public string HuongDanSuDung { get; set; }
        public string CachDung { get; set; }
        public decimal? TocDoTruyen { get; set; }
        public int type { get; set; } //1: thuốc, vật tư  //2: máu
        public short? subPress { get; set; }
        public short? isStartMark { get; set; }

        public decimal? CONVERT_RATIO { get; set; }
        public string CONVERT_UNIT_NAME { get; set; }
        public decimal? CONVERT_AMOUNT { get; set; }
        public long? USE_TIME_TO { get; set; }
        public HIS_SERVICE_REQ_METY serviceReqMety { get; set; }
        public long? PREVIOUS_USE_DAY { get; set; }
        public HIS_EXP_MEST_MEDICINE xpMestMedicine { get; set; }
        public short? IS_RATION { get; set; }           // = 1 : Suất ăn 
        public string PATIENT_TYPE_NAME { get; set; }   //Mức ăn/ĐTTT
        public decimal? PRES_AMOUNT { get; set; }
        public long? USE_TIME { get; set; }
        public string PTTT_GROUP_NAME { get; set; }
        //0: trong dm; 1: ngoài dm; 2: khác
        //public String IN_REQ_EXECUTE { get; set; } // 1: IN_REQUEST, 2:IN_EXECUTE

        public ListMedicineADO() { }

        public ListMedicineADO(MOS.EFMODEL.DataModels.HIS_SERE_SERV data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(data)));
                    }
                    CommonParam param = new CommonParam();
                    var unit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == data.TDL_SERVICE_UNIT_ID);
                    if (unit != null)
                    {
                        this.SERVICE_UNIT_NAME = unit.SERVICE_UNIT_NAME;
                        this.CONVERT_RATIO = unit.CONVERT_RATIO;

                        if (unit.CONVERT_ID.HasValue && unit.CONVERT_RATIO.HasValue)
                        {
                            var conv = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == unit.CONVERT_ID);
                            if (conv != null)
                            {
                                this.CONVERT_UNIT_NAME = conv.SERVICE_UNIT_NAME;
                                this.CONVERT_AMOUNT = this.AMOUNT * unit.CONVERT_RATIO.Value;
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public ListMedicineADO(MPS.Processor.Mps000014.PDO.SereServNumOder data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(data)));
                    }

                    var unit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == data.TDL_SERVICE_UNIT_ID);
                    if (unit != null)
                    {
                        this.SERVICE_UNIT_NAME = unit.SERVICE_UNIT_NAME;
                        this.CONVERT_RATIO = unit.CONVERT_RATIO;

                        if (unit.CONVERT_ID.HasValue && unit.CONVERT_RATIO.HasValue)
                        {
                            var conv = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == unit.CONVERT_ID);
                            if (conv != null)
                            {
                                this.CONVERT_UNIT_NAME = conv.SERVICE_UNIT_NAME;
                                this.CONVERT_AMOUNT = this.AMOUNT * unit.CONVERT_RATIO.Value;
                            }
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
