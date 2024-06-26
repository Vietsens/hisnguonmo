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
using DevExpress.Utils.Menu;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AllocateLiquExpMestCreate
{
    public partial class UCAllocateLiquExpMestCreate : UserControl
    {
        internal enum PrintType
        {
            PHIEU_XUAT_THANH_LY
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                WaitingManager.Show();
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPhieuXuatMatMat = new DXMenuItem("Phiếu xuất thanh lý", new EventHandler(OnClickInPhieuXuatKho));
                itemPhieuXuatMatMat.Tag = PrintType.PHIEU_XUAT_THANH_LY;
                menu.Items.Add(itemPhieuXuatMatMat);

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuXuatKho(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.PHIEU_XUAT_THANH_LY:
                        richEditorMain.RunPrintTemplate(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_THANH_LY__MPS000155, DelegateRunPrinter);
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

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_THANH_LY__MPS000155:
                        InPhieuXuatThanhLy(printTypeCode, fileName, ref result);
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

        private void InPhieuXuatThanhLy(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.resultSdo == null || this.resultSdo.LiquExpMest == null)
                {
                    return;
                }
                WaitingManager.Show();
                // get liquExpMest
                MOS.Filter.HisLiquExpMestViewFilter expeExpMestFilter = new MOS.Filter.HisLiquExpMestViewFilter();
                expeExpMestFilter.ID = this.resultSdo.LiquExpMest.ID;
                var liquExpMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_LIQU_EXP_MEST>>(HIS.Desktop.Plugins.AllocateLiquExpMestCreate.HisRequestUriStore.MOSHIS_LIQU_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expeExpMestFilter, param).FirstOrDefault();

                // get expMest
                MOS.Filter.HisExpMestView1Filter hisExpMestView1Filter = new MOS.Filter.HisExpMestView1Filter();
                hisExpMestView1Filter.ID = this.resultSdo.ExpMest.ID;
                var expMestView1 = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_1>>(HIS.Desktop.Plugins.AllocateLiquExpMestCreate.HisRequestUriStore.MOSHIS_EXP_MEST_GETVIEW_1, ApiConsumer.ApiConsumers.MosConsumer, hisExpMestView1Filter, param).FirstOrDefault();

                MPS.Processor.Mps000155.PDO.Mps000155PDO pdo = new MPS.Processor.Mps000155.PDO.Mps000155PDO(
                 liquExpMest,
                 this.resultSdo.ExpMedicines,
                 this.resultSdo.ExpMaterials,
                 expMestView1
                 );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
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
