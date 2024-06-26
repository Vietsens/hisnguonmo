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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisExpMestTemplate.ADO
{
    class MediMatyTypeADO : MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE
    {
        public int DataType { get; set; }
        public long? HTU_ID { get; set; }
        public string Sang { get; set; }
        public string Trua { get; set; }
        public string Chieu { get; set; }
        public string Toi { get; set; }
        public decimal? AMOUNT { get; set; }
        public short? IsOutMediStock { get; set; }

        public MediMatyTypeADO()
        {

        }

        public MediMatyTypeADO(V_HIS_MEDICINE_TYPE MedicineType)
        {
            try
            {
                if (MedicineType != null )
                {
                     Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, MedicineType);
                     this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        public MediMatyTypeADO(V_HIS_MATERIAL_TYPE MaterialType)
        {
            try
            {
                if (MaterialType != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, MaterialType);

                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                    this.ID = MaterialType.ID;
                    this.MEDICINE_TYPE_CODE = MaterialType.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = MaterialType.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_ID = MaterialType.TDL_SERVICE_UNIT_ID;
                    this.SERVICE_UNIT_NAME = MaterialType.SERVICE_UNIT_NAME;
                    this.SERVICE_UNIT_CODE = MaterialType.SERVICE_UNIT_CODE;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EMTE_MEDICINE_TYPE inputData)
        {
            try
            {
                var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == inputData.MEDICINE_TYPE_ID);
                if (mety != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, mety);

                    this.SERVICE_UNIT_ID = (inputData.SERVICE_UNIT_ID ?? 0);
                    this.SERVICE_UNIT_CODE = inputData.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                    this.TUTORIAL = (String.IsNullOrEmpty(inputData.TUTORIAL) ? mety.TUTORIAL : inputData.TUTORIAL);

                    this.AMOUNT = inputData.AMOUNT;

                    this.CONTRAINDICATION = mety.CONTRAINDICATION;
                    this.IsOutMediStock = inputData.IS_OUT_MEDI_STOCK;

                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC;
                    this.ID = inputData.MEDICINE_TYPE_ID ?? 0;

                    if (!String.IsNullOrEmpty(inputData.MORNING))
                    {
                        this.Sang = ConvertNumber.ProcessNumberInterger(inputData.MORNING);
                    }
                    if (!String.IsNullOrEmpty(inputData.NOON))
                    {
                        this.Trua = ConvertNumber.ProcessNumberInterger(inputData.NOON);
                    }
                    if (!String.IsNullOrEmpty(inputData.AFTERNOON))
                    {
                        this.Chieu = ConvertNumber.ProcessNumberInterger(inputData.AFTERNOON);
                    }
                    if (!String.IsNullOrEmpty(inputData.EVENING))
                    {
                        this.Toi = ConvertNumber.ProcessNumberInterger(inputData.EVENING);
                    }
                    this.HTU_ID = inputData.HTU_ID;

                    this.IS_AUTO_EXPEND = inputData.IS_EXPEND;


                    this.HTU_ID = inputData.HTU_ID;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Create by V_HIS_EMTE_MEDICINE_TYPE fail => Khong tim thay thuoc theo id = " + this.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this), this));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMatyTypeADO(MOS.EFMODEL.DataModels.V_HIS_EMTE_MATERIAL_TYPE inputData)
        {
            try
            {
                var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == inputData.MATERIAL_TYPE_ID);
                if (maty != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this, maty);
                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                    this.ID = (inputData.MATERIAL_TYPE_ID ?? 0);
                    this.MEDICINE_TYPE_CODE = inputData.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = inputData.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_ID = (inputData.SERVICE_UNIT_ID ?? 0);
                    this.SERVICE_UNIT_NAME = inputData.SERVICE_UNIT_NAME;
                    this.SERVICE_UNIT_CODE = inputData.SERVICE_UNIT_CODE;

                    this.AMOUNT = inputData.AMOUNT;

                    this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU;
                    this.ID = (inputData.MATERIAL_TYPE_ID ?? 0);

                    this.TUTORIAL = "";
                    this.MEDICINE_USE_FORM_ID = null;
                    this.MEDICINE_USE_FORM_NAME = "";
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Create by V_HIS_EMTE_MEDICINE_TYPE fail => Khong tim thay thuoc theo id = " + this.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this), this));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
