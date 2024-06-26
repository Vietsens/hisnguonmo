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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.InfomationExecute.ADO
{
    class InformationADO : HIS_SERE_SERV
    {
        public string TDL_SERVICE_TYPE_NAME { get; set; }


//+Với dịch vụ là "Máu" hiển thị "Tên: - dung tích: - số lượng: "
//+Với dịch vụ là "Thuốc" hiển thị "Tên: - hàm lượng: - số lượng: - đơn vị tính: - HDSD: "
//+Với dịc vụ là "Vật tư" hiển thị "Tên: - số lượng: - đơn vị tính: "

        public long ID { get; set; }
        public decimal? VOLUME { get; set; } //dung tích
        public decimal AMOUNT { get; set; } //Số lượng

        public string CONCENTRA { get; set; } //hàm lượng

        public string TUTORIAL { get; set; }//HDSD
        public string SERVICE_UNIT_NAME { get; set; }//Đơn vị tính
    
        public InformationADO() { }
        public InformationADO(HIS_SERE_SERV sereServ)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<InformationADO>(this, sereServ);
            
            this.TDL_SERVICE_TYPE_NAME = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == sereServ.TDL_SERVICE_TYPE_ID).SERVICE_TYPE_NAME ?? "";

        }

        public InformationADO(List<V_HIS_EXP_MEST_MEDICINE> medicine)
        {
            this.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
            this.TDL_SERVICE_TYPE_NAME = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).SERVICE_TYPE_NAME ?? "";

            this.TDL_SERVICE_NAME = medicine.FirstOrDefault().MEDICINE_TYPE_NAME;   
            this.CONCENTRA = medicine.FirstOrDefault().CONCENTRA;
            this.AMOUNT = medicine.Sum(o=>o.AMOUNT);
            this.SERVICE_UNIT_NAME = medicine.FirstOrDefault().SERVICE_UNIT_NAME;
            this.TUTORIAL = medicine.FirstOrDefault().TUTORIAL;
        }

        public InformationADO(List<V_HIS_EXP_MEST_MATERIAL> material)
        {
           
            this.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
            this.TDL_SERVICE_TYPE_NAME = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).SERVICE_TYPE_NAME ?? "";

            this.TDL_SERVICE_NAME = material.FirstOrDefault().MATERIAL_TYPE_NAME;
            this.AMOUNT = material.Sum(o=>o.AMOUNT);
            this.SERVICE_UNIT_NAME = material.FirstOrDefault().SERVICE_UNIT_NAME;
        }

        public InformationADO(List<V_HIS_EXP_MEST_BLOOD> blood)
        {
            this.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
            this.TDL_SERVICE_TYPE_NAME = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).SERVICE_TYPE_NAME ?? "";

            this.TDL_SERVICE_NAME = blood.FirstOrDefault().BLOOD_TYPE_NAME;
            this.VOLUME = blood.FirstOrDefault().VOLUME;
            this.AMOUNT = 1;
        }

    }
}
