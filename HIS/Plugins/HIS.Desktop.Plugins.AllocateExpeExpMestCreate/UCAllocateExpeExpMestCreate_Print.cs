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

namespace HIS.Desktop.Plugins.AllocateExpeExpMestCreate
{
    public partial class UCAllocateExpeExpMestCreate : UserControl
    {
        internal enum PrintType
        {
            PHIEU_XUAT_HAO_PHI,
            BIEN_BAN_XAC_NHAN_HONG
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                WaitingManager.Show();
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemBienBanXacNhanHong = new DXMenuItem("In biên bản xác nhận hỏng", new EventHandler(OnClickInPhieuXuatKho));
                itemBienBanXacNhanHong.Tag = PrintType.BIEN_BAN_XAC_NHAN_HONG;
                menu.Items.Add(itemBienBanXacNhanHong);

                DXMenuItem itemPhieuXuatHaoPhi = new DXMenuItem("Phiếu xuất hao phí", new EventHandler(OnClickInPhieuXuatKho));
                itemPhieuXuatHaoPhi.Tag = PrintType.PHIEU_XUAT_HAO_PHI;
                menu.Items.Add(itemPhieuXuatHaoPhi);

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
                    case PrintType.PHIEU_XUAT_HAO_PHI:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_HAO_PHI__MPS000154, DelegateRunPrinter);
                        break;
                    case PrintType.BIEN_BAN_XAC_NHAN_HONG:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__BIEN_BAN_XAC_NHAN_HONG__MPS000166, DelegateRunPrinter);
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
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_HAO_PHI__MPS000154:
                        InPhieuXuatHaoPhi(printTypeCode, fileName, ref result);
                        break;
                    case HIS.Desktop.Plugins.HisBloodGroup.HisRequestUriStore.PRINT_TYPE_CODE__BIEUMAU__BIEN_BAN_XAC_NHAN_HONG__MPS000166:
                        InBienBanXacNhanHong(printTypeCode, fileName, ref result);
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

        private void InPhieuXuatHaoPhi(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.resultSdo == null || this.resultSdo.ExpeExpMest == null)
                {
                    return;
                }
                WaitingManager.Show();
                MOS.Filter.HisExpeExpMestViewFilter expeExpMestFilter = new MOS.Filter.HisExpeExpMestViewFilter();
                expeExpMestFilter.ID = this.resultSdo.ExpeExpMest.ID;
                var expeExpMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXPE_EXP_MEST>>(HIS.Desktop.Plugins.HisBloodGroup.HisRequestUriStore.MOSHIS_EXPE_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expeExpMestFilter, param).FirstOrDefault();
          
                MPS.Processor.Mps000154.PDO.Mps000154PDO pdo = new MPS.Processor.Mps000154.PDO.Mps000154PDO(
                 expeExpMest,
                 this.resultSdo.ExpMedicines,
                 this.resultSdo.ExpMaterials
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

        private void InBienBanXacNhanHong(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.resultSdo == null || this.resultSdo.ExpeExpMest == null)
                {
                    return;
                }
                WaitingManager.Show();
                MOS.Filter.HisExpeExpMestViewFilter expeExpMestFilter = new MOS.Filter.HisExpeExpMestViewFilter();
                expeExpMestFilter.ID = this.resultSdo.ExpeExpMest.ID;
                var expeExpMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXPE_EXP_MEST>>(HIS.Desktop.Plugins.HisBloodGroup.HisRequestUriStore.MOSHIS_EXPE_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expeExpMestFilter, param).FirstOrDefault();

                MPS.Processor.Mps000166.PDO.Mps000166PDO pdo = new MPS.Processor.Mps000166.PDO.Mps000166PDO(
                 expeExpMest,
                 this.resultSdo.ExpMedicines,
                 this.resultSdo.ExpMaterials
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
