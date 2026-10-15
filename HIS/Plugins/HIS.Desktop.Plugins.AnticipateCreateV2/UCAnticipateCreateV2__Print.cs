/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong 52461 - Tạo dự trù v2: In phiếu dự trù đã lưu (Mps000117), gom nhóm theo Loại.
 * Tham chiếu UCAnticipateCreate_Print.cs (màn cũ) — bỏ nhánh Máu (V2 chưa Bổ sung máu),
 * dùng danh sách kho (mediStockIds) thay 1 kho, Type = int (THUOC=1/VATTU=2/MAU=3).
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
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

        /// <summary>
        /// Build danh sách dòng in (gom nhóm Thuốc/Vật tư qua ado.Type) từ chi tiết phiếu đã lưu.
        /// Chi tiết lấy theo ANTICIPATE_ID (khi mở từ Danh sách dự trù → Sửa, anticipatePrint chỉ có ID, không có collection con).
        /// BV HAGL — gán thêm key Mps000117: Nhóm cha + Ghi chú loại (danh mục cache), Tồn đầu/Nhập/Xuất/Tồn cuối/Xuất max
        /// (số liệu kỳ Từ ngày–Đến ngày đang hiển thị ở GRID DỰ TRÙ), Ghi chú dự trù (NOTE bảng, view không có cột này).
        /// </summary>
        private List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO> LoadPrintLines(HIS_ANTICIPATE anticipate)
        {
            CommonParam param = new CommonParam();
            var lines = new List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>();
            try
            {
                if (anticipate == null) return lines;

                // Thuốc
                HisAnticipateMetyViewFilter fMety = new HisAnticipateMetyViewFilter();
                fMety.ANTICIPATE_ID = anticipate.ID;
                var listMety = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_METY>>(
                    RequestUriStore.HIS_ANTICIPATE_METY_GETVIEW, ApiConsumers.MosConsumer, fMety, param);
                if (listMety != null && listMety.Count > 0)
                {
                    HisMedicineStockViewFilter mediFilter = new HisMedicineStockViewFilter();
                    mediFilter.MEDI_STOCK_IDs = this.mediStockIds;
                    var lstMedi = new BackendAdapter(param).Get<List<MOS.SDO.HisMedicineInStockSDO>>(
                        HisRequestUriStore.HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE, ApiConsumers.MosConsumer, mediFilter, param);

                    var dicNote = new Dictionary<long, string>();
                    var tblMety = new BackendAdapter(param).Get<List<HIS_ANTICIPATE_METY>>(
                        RequestUriStore.HIS_ANTICIPATE_METY_GET, ApiConsumers.MosConsumer, new HisAnticipateMetyFilter { ANTICIPATE_ID = anticipate.ID }, param);
                    if (tblMety != null) foreach (var t in tblMety) dicNote[t.ID] = t.NOTE;
                    var medicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();

                    foreach (var item in listMety)
                    {
                        var inStock = lstMedi != null ? lstMedi.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID) : null;
                        var ado = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>(ado, item);
                        ado.Type = PRINT_TYPE_THUOC;
                        ado.TotalMoney = ado.AMOUNT * (ado.IMP_PRICE ?? 0);
                        ado.IN_STOCK_AMOUNT = (inStock != null && inStock.TotalAmount.HasValue) ? inStock.TotalAmount.Value : 0;

                        var mety = medicineTypes != null ? medicineTypes.FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID) : null;
                        if (mety != null)
                        {
                            ado.PARENT_CODE = mety.PARENT_CODE;
                            ado.PARENT_NAME = mety.PARENT_NAME;
                            ado.DESCRIPTION = mety.DESCRIPTION;
                        }
                        FillPrintLineFigures(ado, ADO.AnticipateLineType.THUOC, item.MEDICINE_TYPE_ID, item.BID_ID, item.SUPPLIER_ID, item.ID, dicNote);
                        lines.Add(ado);
                    }
                }

                // Vật tư
                HisAnticipateMatyViewFilter fMaty = new HisAnticipateMatyViewFilter();
                fMaty.ANTICIPATE_ID = anticipate.ID;
                var listMaty = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_MATY>>(
                    RequestUriStore.HIS_ANTICIPATE_MATY_GETVIEW, ApiConsumers.MosConsumer, fMaty, param);
                if (listMaty != null && listMaty.Count > 0)
                {
                    HisMaterialStockViewFilter mateFilter = new HisMaterialStockViewFilter();
                    mateFilter.MEDI_STOCK_IDs = this.mediStockIds;
                    var lstMate = new BackendAdapter(param).Get<List<MOS.SDO.HisMaterialInStockSDO>>(
                        HisRequestUriStore.HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE, ApiConsumers.MosConsumer, mateFilter, param);

                    var dicNote = new Dictionary<long, string>();
                    var tblMaty = new BackendAdapter(param).Get<List<HIS_ANTICIPATE_MATY>>(
                        RequestUriStore.HIS_ANTICIPATE_MATY_GET, ApiConsumers.MosConsumer, new HisAnticipateMatyFilter { ANTICIPATE_ID = anticipate.ID }, param);
                    if (tblMaty != null) foreach (var t in tblMaty) dicNote[t.ID] = t.NOTE;
                    var materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();

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

                        var maty = materialTypes != null ? materialTypes.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID) : null;
                        if (maty != null)
                        {
                            ado.PARENT_CODE = maty.PARENT_CODE;
                            ado.PARENT_NAME = maty.PARENT_NAME;
                            ado.DESCRIPTION = maty.DESCRIPTION;
                        }
                        FillPrintLineFigures(ado, ADO.AnticipateLineType.VATTU, item.MATERIAL_TYPE_ID, item.BID_ID, item.SUPPLIER_ID, item.ID, dicNote);
                        lines.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lines;
        }

        /// <summary>
        /// BV HAGL — điền Tồn đầu/Nhập/Xuất/Tồn cuối/Xuất max từ dòng tương ứng ở GRID DỰ TRÙ (anticipateLines:
        /// khớp Loại + TypeId + Thầu + NCC, không có thì khớp Loại + TypeId) và Ghi chú dự trù từ dicNote (ID chi tiết → NOTE).
        /// Grid chưa có số liệu (chưa Tìm) thì các key này để trống.
        /// </summary>
        private void FillPrintLineFigures(MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO ado, string lineType, long typeId,
            long? bidId, long? supplierId, long detailId, Dictionary<long, string> dicNote)
        {
            try
            {
                if (anticipateLines != null)
                {
                    var line = anticipateLines.FirstOrDefault(o => o != null && o.Type == lineType && o.TypeId == typeId
                                    && o.BidId == bidId && o.SupplierId == supplierId)
                            ?? anticipateLines.FirstOrDefault(o => o != null && o.Type == lineType && o.TypeId == typeId);
                    if (line != null)
                    {
                        ado.OPEN_QUANTITY = line.OpenQuantity;
                        ado.NEW_IMPORT_QUANTITY = line.NewImport;
                        ado.USED_QUANTITY = line.Used;
                        ado.CLOSE_QUANTITY = line.CloseQuantity;
                        ado.MAX_EXPORT_QUANTITY = line.MaxExport;
                        ado.MAX_EXPORT_MONTH = line.MaxExportMonth > 0 ? (int?)line.MaxExportMonth : null;
                        ado.MAX_EXPORT_DISPLAY = line.MaxExportDisplay;
                    }
                }

                string note;
                if (dicNote != null && dicNote.TryGetValue(detailId, out note)) ado.NOTE = note;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
