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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ExpMestMedicineGrid;
using MOS.SDO;
using HIS.UC.ExpMestMedicineGrid.ADO;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.ExpMestOtherExport.ADO;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMaterialGrid.ADO;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestOtherExport.Validation;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.ExpMestOtherExport.Base;

namespace HIS.Desktop.Plugins.ExpMestOtherExport
{
    public partial class UCExpMestOtherExport : HIS.Desktop.Utility.UserControlBase
    {
        private void LoadMedicineTypeFromStock()
        {
            try
            {
                var medicineTypeList = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                CommonParam param = new CommonParam();
                HisMedicineStockViewFilter medicineFilter = new HisMedicineStockViewFilter();
                medicineFilter.MEDI_STOCK_ID = mediStock.ID;
                medicineFilter.IS_LEAF = 1;
                medicineFilter.MEDICINE_TYPE_IS_ACTIVE = true;
                medicineFilter.ORDER_DIRECTION = "ASC";
                medicineFilter.ORDER_FIELD = "MEDICINE_TYPE_NAME";
                if (this.mediStock != null && this.mediStock.IS_BUSINESS == 1)
                {
                    this.listMediInStock = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisMedicineInStockSDO>>("/api/HisMedicine/GetInStockMedicine", ApiConsumers.MosConsumer, medicineFilter, param).Where(o => o.AvailableAmount > 0 && (medicineTypeList.FirstOrDefault(p => p.ID == o.MEDICINE_TYPE_ID) != null ? medicineTypeList.FirstOrDefault(p => p.ID == o.MEDICINE_TYPE_ID).IS_BUSINESS : null) == 1).ToList();
                }
                else if (this.mediStock != null && this.mediStock.IS_BUSINESS != 1)
                {
                    this.listMediInStock = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisMedicineInStockSDO>>("/api/HisMedicine/GetInStockMedicine", ApiConsumers.MosConsumer, medicineFilter, param).Where(o => o.AvailableAmount > 0 && (medicineTypeList.FirstOrDefault(p => p.ID == o.MEDICINE_TYPE_ID) != null ? medicineTypeList.FirstOrDefault(p => p.ID == o.MEDICINE_TYPE_ID).IS_BUSINESS : null) != 1).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMaterialTypeFromStock()
        {
            try
            {
                var materialTypeList = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                HisMaterialStockViewFilter mateFilter = new HisMaterialStockViewFilter();
                mateFilter.MEDI_STOCK_ID = mediStock.ID;
                mateFilter.MATERIAL_TYPE_IS_ACTIVE = true;
                mateFilter.IS_LEAF = 1;
                mateFilter.ORDER_DIRECTION = "ASC";
                mateFilter.ORDER_FIELD = "MATERIAL_TYPE_NAME";
                this.listMateInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialInStockSDO>>("/api/HisMaterial/GetInStockMaterial", ApiConsumers.MosConsumer, mateFilter, null).Where(o => o.AvailableAmount > 0 && o.IS_ACTIVE == 1).ToList();
                if (this.mediStock != null && this.mediStock.IS_BUSINESS == 1)
                {
                    this.listMateInStock = this.listMateInStock.Where(o =>
                        o.AvailableAmount > 0
                        && o.IS_ACTIVE == 1
                        && IsCheckMaterial(materialTypeList, true, o.MATERIAL_TYPE_ID)
                        ).ToList();
                }
                else if (this.mediStock != null && this.mediStock.IS_BUSINESS != 1)
                {
                    this.listMateInStock = this.listMateInStock.Where(o =>
                        o.AvailableAmount > 0
                        && o.IS_ACTIVE == 1
                        && IsCheckMaterial(materialTypeList, false, o.MATERIAL_TYPE_ID)
                        ).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsCheckMaterial(List<V_HIS_MATERIAL_TYPE> _materialTypes, bool isBusiness, long materialTypeId)
        {
            bool result = false;
            try
            {
                var data = _materialTypes.FirstOrDefault(p => p.ID == materialTypeId);
                if (data != null && data.IS_REUSABLE != 1)
                {
                    if (isBusiness)
                    {
                        if (data.IS_BUSINESS == 1)
                            result = true;
                    }
                    else
                    {
                        if (data.IS_BUSINESS != 1)
                            result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadBloodFromStock()
        {
            try
            {
                HisBloodViewFilter bloodFilter = new HisBloodViewFilter();
                bloodFilter.MEDI_STOCK_ID = mediStock.ID;
                bloodFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                bloodFilter.ORDER_DIRECTION = "ASC";
                bloodFilter.ORDER_FIELD = "BLOOD_TYPE_NAME";
                listBloodInStock = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BLOOD>>("api/HisBlood/GetView", ApiConsumers.MosConsumer, bloodFilter, null).Where(p => p.IS_ACTIVE == 1).ToList();

                LoadMaterialTypeReuseFromStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<V_HIS_MATERIAL_BEAN_1> _vMaterialBeans { get; set; }
        private void LoadMaterialTypeReuseFromStock()
        {
            try
            {
                _vMaterialBeans = new List<V_HIS_MATERIAL_BEAN_1>();
                MOS.Filter.HisMaterialBeanView1Filter filter = new HisMaterialBeanView1Filter();
                filter.MEDI_STOCK_ID = this.mediStock.ID;
                filter.IS_REUSABLE = true;
                filter.IS_ACTIVE = (short)1;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "MATERIAL_TYPE_CODE";

                _vMaterialBeans = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MATERIAL_BEAN_1>>("/api/HisMaterialBean/GetView1", ApiConsumers.MosConsumer, filter, null);
                if (_vMaterialBeans != null && _vMaterialBeans.Count > 0)
                {
                    _vMaterialBeans = _vMaterialBeans.Where(p => !String.IsNullOrEmpty(p.SERIAL_NUMBER)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlPriceAndAVT(bool enable)
        {
            try
            {
                spinExpPrice.Enabled = enable;
                spinPriceVAT.Enabled = enable;
                //spinExpPrice.EditValue = null;
                //spinPriceVAT.EditValue = null;
                if (!enable)
                {
                    SetValueByMediMateADO();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
