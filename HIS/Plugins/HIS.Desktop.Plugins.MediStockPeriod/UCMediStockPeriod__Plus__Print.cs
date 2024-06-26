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
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Print;

namespace HIS.Desktop.Plugins.MediStockPeriod
{
    public partial class UCMediStockPeriod : UserControl
    {
        internal enum PrintType
        {
            IN_BIEN_BAN_KIEM_KE_T_VT_M,
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_BIEN_BAN_KIEM_KE_T_VT_M:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEN_BAN_KIEM_KE_THUOC_VT_MAU__MPS000132, DelegateRunPrinter);
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
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEN_BAN_KIEM_KE_THUOC_VT_MAU__MPS000132:
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
                //medistockSelectList
                List<V_HIS_MEST_INVE_USER> listUser = new List<V_HIS_MEST_INVE_USER>();

                var hisMediStockPeriod = (V_HIS_MEDI_STOCK_PERIOD)gridViewMediStockPeriod.GetFocusedRow();
                if (hisMediStockPeriod == null) return;

                if (hisMediStockPeriod != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMestInventoryFilter mestInventoryFilter = new MOS.Filter.HisMestInventoryFilter();
                    mestInventoryFilter.MEDI_STOCK_PERIOD_ID = hisMediStockPeriod.ID;
                    var rsMestInventory = new BackendAdapter(param).Get<List<HIS_MEST_INVENTORY>>("api/HisMestInventory/Get", ApiConsumers.MosConsumer, mestInventoryFilter, param);

                    if (rsMestInventory != null && rsMestInventory.Count > 0)
                    {
                        MOS.Filter.HisMestInveUserViewFilter userFilter = new MOS.Filter.HisMestInveUserViewFilter();
                        userFilter.MEST_INVENTORY_ID = rsMestInventory.FirstOrDefault().ID;
                        listUser = new BackendAdapter(param).Get<List<V_HIS_MEST_INVE_USER>>("api/HisMestInveUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                    }
                }


                if (mestPeriodMetiList != null && mestPeriodMetiList.Count() > 0 && medistockSelectList != null && medistockSelectList.Count() > 0)
                {
                    mestPeriodMetiList = mestPeriodMetiList.Where(o => medistockSelectList.Contains(o.MEDI_STOCK_ID ?? 0)).ToList();
                }
                if (mestPeriodMateList != null && mestPeriodMateList.Count() > 0 && medistockSelectList != null && medistockSelectList.Count() > 0)
                {
                    mestPeriodMateList = mestPeriodMateList.Where(o => medistockSelectList.Contains(o.MEDI_STOCK_ID ?? 0)).ToList();
                }

                List<HIS_MEDICINE_GROUP> medicineGroups = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_GROUP>();

                List<HIS_MEDI_STOCK_METY> mediStockMetyList = new List<HIS_MEDI_STOCK_METY>();
                if (mestPeriodMetiList != null && mestPeriodMetiList.Count() > 0)
                {
                    MOS.Filter.HisMediStockMetyFilter medistockMetyFilter = new MOS.Filter.HisMediStockMetyFilter();
                    medistockMetyFilter.MEDICINE_TYPE_IDs = mestPeriodMetiList.Select(o => o.MEDICINE_TYPE_ID).ToList();
                    mediStockMetyList = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_STOCK_METY>>("api/HisMediStockMety/Get", ApiConsumer.ApiConsumers.MosConsumer, medistockMetyFilter, null);
                }

                List<HIS_MEDI_STOCK_MATY> mediStockMatyList = new List<HIS_MEDI_STOCK_MATY>();
                if (mestPeriodMateList != null && mestPeriodMateList.Count() > 0)
                {
                    MOS.Filter.HisMediStockMatyFilter medistockMetyFilter = new MOS.Filter.HisMediStockMatyFilter();
                    medistockMetyFilter.MATERIAL_TYPE_IDs = mestPeriodMateList.Select(o => o.MATERIAL_TYPE_ID).ToList();
                    mediStockMatyList = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_STOCK_MATY>>("api/HisMediStockMaty/Get", ApiConsumer.ApiConsumers.MosConsumer, medistockMetyFilter, null);
                }

                MPS.Processor.Mps000132.PDO.Mps000132PDO mps000132RDO = new MPS.Processor.Mps000132.PDO.Mps000132PDO(
                    hisMediStockPeriod,
                    listUser,
                    mestPeriodMetiList,
                    mestPeriodMateList,
                    medicineGroups,
                    medistockSelectList,
                    mediStockMetyList,
                    mediStockMatyList
               );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000132RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000132RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
