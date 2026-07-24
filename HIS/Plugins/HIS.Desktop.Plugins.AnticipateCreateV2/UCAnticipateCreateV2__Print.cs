/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong 52461 - Tạo dự trù v2: In phiếu dự trù đã lưu (Mps000117), gom nhóm theo Loại.
 * Tham chiếu UCAnticipateCreate_Print.cs (màn cũ) — bỏ nhánh Máu (V2 chưa Bổ sung máu),
 * dùng danh sách kho (mediStockIds) thay 1 kho, Type = int (THUOC=1/VATTU=2/MAU=3).
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AnticipateCreateV2
{
    public partial class UCAnticipateCreateV2
    {
        // Type cho biểu in Mps000117 (khớp template màn cũ: THUOC=1, VATTU=2, MAU=3).
        private const int PRINT_TYPE_THUOC = 1;
        private const int PRINT_TYPE_VATTU = 2;

        /// <summary>In phiếu dự trù vừa lưu (Mps000117). Cần đã Lưu (anticipatePrint != null).</summary>
        internal void PrintAnticipate()
        {
            try
            {
                if (anticipatePrint == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa có phiếu dự trù đã lưu để in. Vui lòng Lưu trước.", "Thông báo");
                    return;
                }
                Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR,
                    LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);
                richEditor.RunPrintTemplate(MPS.Processor.Mps000117.PDO.Mps000117PDO.printTypeCode, DelegateRunPrinterAntc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinterAntc(string printTypeCode, string fileName)
        {
            bool result = true;
            try
            {
                if (anticipatePrint == null) return false;
                WaitingManager.Show();

                HisAnticipateViewFilter filter = new HisAnticipateViewFilter();
                filter.ID = anticipatePrint.ID;
                var apiresult = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ANTICIPATE>>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ANTICIPATE_GETVIEW, ApiConsumers.MosConsumer, filter, new CommonParam());
                V_HIS_ANTICIPATE dataPrint = apiresult != null ? apiresult.FirstOrDefault() : null;

                var mps117Rdo = new MPS.Processor.Mps000117.PDO.Mps000117PDO(dataPrint, LoadPrintLines(anticipatePrint));

                WaitingManager.Hide();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    printerName = GlobalVariables.dicPrinter[printTypeCode];

                MPS.ProcessorBase.Core.PrintData printData;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps117Rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                else
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps117Rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);

                result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Build danh sách dòng in (gom nhóm Thuốc/Vật tư qua ado.Type) từ chi tiết phiếu đã lưu.</summary>
        private List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO> LoadPrintLines(HIS_ANTICIPATE anticipate)
        {
            CommonParam param = new CommonParam();
            var lines = new List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>();
            try
            {
                if (anticipate == null) return lines;

                // Thuốc
                if (anticipate.HIS_ANTICIPATE_METY != null && anticipate.HIS_ANTICIPATE_METY.Count > 0)
                {
                    HisMedicineStockViewFilter mediFilter = new HisMedicineStockViewFilter();
                    mediFilter.MEDI_STOCK_IDs = this.mediStockIds;
                    var lstMedi = new BackendAdapter(param).Get<List<MOS.SDO.HisMedicineInStockSDO>>(
                        HisRequestUriStore.HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE, ApiConsumers.MosConsumer, mediFilter, param);

                    HisAnticipateMetyViewFilter fMety = new HisAnticipateMetyViewFilter();
                    fMety.IDs = anticipate.HIS_ANTICIPATE_METY.Select(o => o.ID).ToList();
                    var listMety = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_METY>>(
                        "api/HisAnticipateMety/GetView", ApiConsumers.MosConsumer, fMety, param);
                    if (listMety != null)
                    {
                        foreach (var item in listMety)
                        {
                            var inStock = lstMedi != null ? lstMedi.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID) : null;
                            var ado = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>(ado, item);
                            ado.Type = PRINT_TYPE_THUOC;
                            ado.TotalMoney = ado.AMOUNT * (ado.IMP_PRICE ?? 0);
                            ado.IN_STOCK_AMOUNT = (inStock != null && inStock.TotalAmount.HasValue) ? inStock.TotalAmount.Value : 0;
                            lines.Add(ado);
                        }
                    }
                }

                // Vật tư
                if (anticipate.HIS_ANTICIPATE_MATY != null && anticipate.HIS_ANTICIPATE_MATY.Count > 0)
                {
                    HisMaterialStockViewFilter mateFilter = new HisMaterialStockViewFilter();
                    mateFilter.MEDI_STOCK_IDs = this.mediStockIds;
                    var lstMate = new BackendAdapter(param).Get<List<MOS.SDO.HisMaterialInStockSDO>>(
                        HisRequestUriStore.HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE, ApiConsumers.MosConsumer, mateFilter, param);

                    HisAnticipateMatyViewFilter fMaty = new HisAnticipateMatyViewFilter();
                    fMaty.IDs = anticipate.HIS_ANTICIPATE_MATY.Select(o => o.ID).ToList();
                    var listMaty = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_MATY>>(
                        "api/HisAnticipateMaty/GetView", ApiConsumers.MosConsumer, fMaty, param);
                    if (listMaty != null)
                    {
                        foreach (var item in listMaty)
                        {
                            var inStock = lstMate != null ? lstMate.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID) : null;
                            var ado = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>(ado, item);
                            ado.Type = PRINT_TYPE_VATTU;
                            ado.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                            ado.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                            ado.TotalMoney = ado.AMOUNT * (ado.IMP_PRICE ?? 0);
                            ado.IN_STOCK_AMOUNT = (inStock != null && inStock.TotalAmount.HasValue) ? inStock.TotalAmount.Value : 0;
                            lines.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lines;
        }
    }
}
