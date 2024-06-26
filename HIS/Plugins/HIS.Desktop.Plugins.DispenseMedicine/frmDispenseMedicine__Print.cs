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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.DispenseMedicine.ADO;
using HIS.Desktop.Plugins.DispenseMedicine.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DispenseMedicine
{
    public partial class frmDispenseMedicine : FormBase
    {
        private void LoadInBaoCheThuoc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {

                if (hisDispenseResultSDO == null)
                {
                    throw new Exception("Khong co thong tin ket qua tra ve hisDispenseResultSDO");
                }

                V_HIS_EXP_MEST expMest = null;
                V_HIS_IMP_MEST impMest = null;
                List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines = null;
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = null;
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = null;

                WaitingManager.Show();
                if (hisDispenseResultSDO.HisExpMest != null)
                {
                    HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                    expMestFilter.ID = hisDispenseResultSDO.HisExpMest.ID;
                    expMest = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, new CommonParam()).FirstOrDefault();

                    HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.EXP_MEST_ID = hisDispenseResultSDO.HisExpMest.ID;
                    expMestMaterials = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, new CommonParam());

                    HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_ID = hisDispenseResultSDO.HisExpMest.ID;
                    expMestMedicines = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, new CommonParam());

                }
                if (hisDispenseResultSDO.HisImpMest != null)
                {
                    HisImpMestViewFilter impMestFilter = new HisImpMestViewFilter();
                    impMestFilter.ID = hisDispenseResultSDO.HisImpMest.ID;
                    impMest = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestFilter, new CommonParam()).FirstOrDefault();

                    HisImpMestMedicineViewFilter impMestMedicineFilter = new HisImpMestMedicineViewFilter();
                    impMestMedicineFilter.IMP_MEST_ID = hisDispenseResultSDO.HisImpMest.ID;
                    impMestMedicines = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumers.MosConsumer, impMestMedicineFilter, new CommonParam());
                }
                WaitingManager.Hide();
                List<HIS_MEDI_STOCK> listMediStock = new List<HIS_MEDI_STOCK> { this.mediStock };
                MPS.Processor.Mps000244.PDO.Mps000244PDO rdo = new MPS.Processor.Mps000244.PDO.Mps000244PDO(hisDispenseResultSDO.HisDispense, listMediStock, impMestMedicines, expMestMaterials, expMestMedicines);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, ""));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                LoadInBaoCheThuoc(printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

    }
}
