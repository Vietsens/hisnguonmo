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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using AutoMapper;
using HIS.Desktop.Common;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.ImpMestAggregate
{
    public partial class UCImpMestAggregate : HIS.Desktop.Utility.UserControlBase
    {
        private static List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE> LoadDetailImpMestMatyMetyByImpMestId(long impMestId)
        {
            //ma, ten, dvt, so luong ,solo
            List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE> impMestManuMetyMatys = new List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>();//luu tat ca thuoc va vat tu
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestMedicineViewFilter impMestMedicineViewFilter = new MOS.Filter.HisImpMestMedicineViewFilter();
                impMestMedicineViewFilter.IMP_MEST_ID = impMestId;
                var impMestMedicines = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);
                if (impMestMedicines != null)
                {
                    impMestManuMetyMatys.AddRange(impMestMedicines);
                }

                HisImpMestMaterialViewFilter impMestMaterialViewFilter = new MOS.Filter.HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.IMP_MEST_ID = impMestId;
                var impMestMaterials = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);
                if (impMestMaterials != null)
                {
                    foreach (var item_impMaterial in impMestMaterials)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE impMestMaterial = new MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE();

                        Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MATERIAL, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>();
                        impMestMaterial = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MATERIAL, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>(item_impMaterial);

                        impMestMaterial.MEDICINE_TYPE_ID = item_impMaterial.MATERIAL_TYPE_ID;
                        impMestMaterial.MEDICINE_TYPE_CODE = item_impMaterial.MATERIAL_TYPE_CODE;
                        impMestMaterial.MEDICINE_TYPE_NAME = item_impMaterial.MATERIAL_TYPE_NAME;
                        impMestMaterial.MEDICINE_ID = item_impMaterial.MATERIAL_ID;

                        impMestManuMetyMatys.Add(impMestMaterial);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return impMestManuMetyMatys;
        }

    }
}
