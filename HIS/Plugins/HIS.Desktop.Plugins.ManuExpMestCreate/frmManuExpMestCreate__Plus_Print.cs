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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.SdaConfigKey;
using HIS.Desktop.Plugins.ManuExpMestCreate.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.ManuExpMestCreate
{
    public partial class frmManuExpMestCreate : HIS.Desktop.Utility.FormBase
    {
        internal enum PrintType
        {
            IN_PHIEU_TRA_NHA_CUNG_CAP,
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_PHIEU_TRA_NHA_CUNG_CAP:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_XUAT_TRA_NHA_CUNG_CAP__MPS000130, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_XUAT_TRA_NHA_CUNG_CAP__MPS000130:
                        LoadDataPrint(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadDataPrint(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                List<V_HIS_EXP_MEST_MEDICINE> lstExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> lstExpMestMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();
                List<V_HIS_EXP_MEST_BLOOD> lstExpMestBlood = new List<V_HIS_EXP_MEST_BLOOD>();
                CommonParam param = new CommonParam();
                if (manuExpMestResult != null)
                {
                    if (manuExpMestResult.ExpMedicines != null)
                    {
                        List<long> ExpMestIds = manuExpMestResult.ExpMedicines.Select(p => p.EXP_MEST_ID ??0).ToList();
                        MOS.Filter.HisExpMestMedicineViewFilter filter = new MOS.Filter.HisExpMestMedicineViewFilter();
                        filter.EXP_MEST_IDs = ExpMestIds;
                        lstExpMestMedicine = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                    }
                    if (manuExpMestResult.ExpBloods != null)
                    {
                        List<long> ExpMestIds = manuExpMestResult.ExpBloods.Select(p => p.EXP_MEST_ID).ToList();
                        MOS.Filter.HisExpMestBloodViewFilter filter = new MOS.Filter.HisExpMestBloodViewFilter();
                        filter.EXP_MEST_IDs = ExpMestIds;
                        lstExpMestBlood = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, filter, param);
                    }

                    if (manuExpMestResult.ExpMaterials != null)
                    {
                        List<long> ExpMestIds = manuExpMestResult.ExpMaterials.Select(p => p.EXP_MEST_ID ??0).ToList();
                        MOS.Filter.HisExpMestMaterialViewFilter filter = new MOS.Filter.HisExpMestMaterialViewFilter();
                        filter.EXP_MEST_IDs = ExpMestIds;
                        lstExpMestMaterial = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                    }
                }

                MPS.Processor.Mps000130.PDO.Mps000130PDO mps000130RDO = new MPS.Processor.Mps000130.PDO.Mps000130PDO(
                    manuExpMestResult.ExpMest,
                    lstExpMestMedicine,
                    lstExpMestMaterial,
                    lstExpMestBlood,
                    BackendDataWorker.Get<HIS_SUPPLIER>(),
                    BackendDataWorker.Get<V_HIS_MEDI_STOCK>()
                                );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000130RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000130RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);


            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
