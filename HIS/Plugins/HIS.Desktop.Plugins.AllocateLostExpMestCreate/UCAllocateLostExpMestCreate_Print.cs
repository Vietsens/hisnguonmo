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

namespace HIS.Desktop.Plugins.AllocateLostExpMestCreate
{
    public partial class UCAllocateLostExpMestCreate : UserControl
    {
        internal enum PrintType
        {
            PHIEU_XUAT_MAT_MAT,
            BIEN_BAN_XUAT_MAT_MAT
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                WaitingManager.Show();
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPhieuXuatMatMat = new DXMenuItem("Phiếu xuất mất mát", new EventHandler(OnClickInPhieuXuatKho));
                itemPhieuXuatMatMat.Tag = PrintType.PHIEU_XUAT_MAT_MAT;
                DXMenuItem itemBienBanPhieuXuatMatMat = new DXMenuItem("BIÊN BẢN XÁC NHẬN THUỐC/HÓA CHẤT/VẬT TƯ Y TẾ MẤT/HỎNG/VỠ", new EventHandler(OnClickInBienBan));
                itemBienBanPhieuXuatMatMat.Tag = PrintType.BIEN_BAN_XUAT_MAT_MAT;

                menu.Items.Add(itemPhieuXuatMatMat);
                menu.Items.Add(itemBienBanPhieuXuatMatMat);

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
                    case PrintType.PHIEU_XUAT_MAT_MAT:
                        richEditorMain.RunPrintTemplate(HisRequestUriStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_MAT_MAT__MPS000168, DelegateRunPrinter);
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
                    case HisRequestUriStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_MAT_MAT__MPS000168:
                        InPhieuXuatMatMat(printTypeCode, fileName, ref result);
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

        private void InPhieuXuatMatMat(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.resultSdo == null || this.resultSdo.LostExpMest == null)
                {
                    return;
                }
                WaitingManager.Show();
                MOS.Filter.HisLostExpMestViewFilter expeExpMestFilter = new MOS.Filter.HisLostExpMestViewFilter();
                expeExpMestFilter.ID = this.resultSdo.LostExpMest.ID;
                var lostExpMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_LOST_EXP_MEST>>(HIS.Desktop.Plugins.AllocateLostExpMestCreate.HisRequestUriStore.MOSHIS_LOST_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expeExpMestFilter, param).FirstOrDefault();

                MOS.Filter.HisExpMestView1Filter expMestViewFilter = new MOS.Filter.HisExpMestView1Filter();
                expMestViewFilter.ID = this.resultSdo.ExpMest.ID;
                var expMest1 = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_1>>("api/HisExpMest/GetView1", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param).FirstOrDefault();

                MOS.Filter.HisExpMestUserViewFilter expMestUserViewFilter = new MOS.Filter.HisExpMestUserViewFilter();
                expMestUserViewFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                var expMestUser = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_USER>>("api/HisExpMestUser/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestUserViewFilter, param);

                MPS.Processor.Mps000168.PDO.Mps000168PDO pdo = new MPS.Processor.Mps000168.PDO.Mps000168PDO(
                 lostExpMest,
                 this.resultSdo.ExpMedicines,
                 this.resultSdo.ExpMaterials,
                 expMest1
                    //expMestUser
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

        #region Bien Ban

        private void OnClickInBienBan(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcessBienBan(type);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        void PrintProcessBienBan(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.PHIEU_XUAT_MAT_MAT:
                        richEditorMain.RunPrintTemplate(HisRequestUriStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_MAT_MAT__MPS000168, DelegateRunPrinter);
                        break;
                    case PrintType.BIEN_BAN_XUAT_MAT_MAT:
                        richEditorMain.RunPrintTemplate(HisRequestUriStore.PRINT_TYPE_CODE__BIEUMAU__HOI_DONG_PHIEU_XUAT_MAT_MAT__MPS000205, DelegateRunPrinterBienBan);
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

        private bool DelegateRunPrinterBienBan(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case HisRequestUriStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_MAT_MAT__MPS000168:
                        InPhieuXuatMatMat(printTypeCode, fileName, ref result);
                        break;
                        case HisRequestUriStore.PRINT_TYPE_CODE__BIEUMAU__HOI_DONG_PHIEU_XUAT_MAT_MAT__MPS000205:
                        InBienBanPhieuXuatMatMat(printTypeCode, fileName, ref result);
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

        private void InBienBanPhieuXuatMatMat(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.resultSdo == null || this.resultSdo.LostExpMest == null)
                {
                    return;
                }
                WaitingManager.Show();
                MOS.Filter.HisLostExpMestViewFilter expeExpMestFilter = new MOS.Filter.HisLostExpMestViewFilter();
                expeExpMestFilter.ID = this.resultSdo.LostExpMest.ID;
                var lostExpMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_LOST_EXP_MEST>>(HIS.Desktop.Plugins.AllocateLostExpMestCreate.HisRequestUriStore.MOSHIS_LOST_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expeExpMestFilter, param).FirstOrDefault();

                MOS.Filter.HisExpMestView1Filter expMestViewFilter = new MOS.Filter.HisExpMestView1Filter();
                expMestViewFilter.ID = this.resultSdo.ExpMest.ID;
                var expMest1 = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_1>>("api/HisExpMest/GetView1", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param).FirstOrDefault();

                MOS.Filter.HisExpMestUserViewFilter expMestUserViewFilter = new MOS.Filter.HisExpMestUserViewFilter();
                expMestUserViewFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                var expMestUser = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_USER>>("api/HisExpMestUser/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestUserViewFilter, param);

                MPS.Processor.Mps000205.PDO.Mps000205PDO pdo = new MPS.Processor.Mps000205.PDO.Mps000205PDO(
                    expMest1,
                 lostExpMest,
                 this.resultSdo.ExpMedicines,
                 this.resultSdo.ExpMaterials,
                  expMestUser
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
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
