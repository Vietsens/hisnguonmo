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
using FlexCel.Report;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000049.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000049
{
    public class Mps000049Processor : AbstractProcessor
    {
        Mps000049PDO rdo;
        List<ExpMestADO> ExpMestADOs;
        List<Mps000049ADO> ExpMestADOsSplit;
        List<MedicineUseFormADO> medicineUseForms;
        List<ExpMestADO> listMedicineType = new List<ExpMestADO>();
        List<ExpMestADO> listMedicineParent = new List<ExpMestADO>();
        List<ExpMestADO> listOtherPaySource = new List<ExpMestADO>();

        // Tên các phiếu lĩnh dùng để gom nhóm
        const string PHIEU_LINH_THUOC_THUONG = "PHIẾU LĨNH THUỐC THƯỜNG";
        const string PHIEU_LINH_SAN_PHAM_KHONG_PHAI_THUOC = "PHIẾU LĨNH SẢN PHẨM KHÔNG PHẢI LÀ THUỐC";

        // Sentinel cho MEDICINE_GROUP_ID của phiếu SPKPLT — giá trị âm để không trùng nhóm thuốc thật (luôn dương)
        const long SPKPLT_MEDICINE_GROUP_ID = -2778;

        #region Nhóm trang in (PageGroups): thuốc thường vs thuốc kiểm soát đặc biệt

        // Nhóm trang 1: thuốc thường (tất cả nhóm còn lại)
        const long PAGE_GROUP_ID__NORMAL = 1;
        // Nhóm trang 2: gây nghiện, hướng thần, tiền chất (kể cả dạng phối hợp)
        const long PAGE_GROUP_ID__CONTROLLED = 2;

        const string PAGE_GROUP_NAME__NORMAL = "THUỐC THƯỜNG";
        const string PAGE_GROUP_NAME__CONTROLLED = "THUỐC GÂY NGHIỆN, HƯỚNG THẦN, TIỀN CHẤT";

        // Tiền tố tiêu đề phiếu, ghép với tên nhóm thành PAGE_GROUP_TITLE
        const string PAGE_GROUP_TITLE_PREFIX = "PHIẾU XUẤT KHO ";

        #endregion

        public Mps000049Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000049PDO)rdoBase;
        }

        /// <summary>
        /// Kiểm tra cấu hình MOS.HIS_MEDICINE_TYPE.SEPARATE_FUNCTIONAL_FOOD_PRINTING có BẬT (= 1) hay không.
        /// </summary>
        private bool IsSeparateFunctionalFood()
        {
            return rdo != null && rdo.ConfigMps49 != null && rdo.ConfigMps49._ConfigKeySeparateFunctionalFood == 1;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrExpMest.EXP_MEST_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000049ExtendSingleKey.EXP_MEST_CODE_BAR, barcodePatientCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                ProcessListADO();
                ProcessListMedicineUse();

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetBarcodeKey();
                SetSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                // DEBUG 2778: in tên template thật sự được render (file nào crash sẽ là dòng ngay trước Invalid row index)
                Inventec.Common.Logging.LogSystem.Debug("MPS49_TEMPLATE____" + System.IO.Path.GetFullPath(fileName));
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);



                if (ExpMestADOs != null && ExpMestADOs.Count > 0)
                {
                    ExpMestADOs = ExpMestADOs.OrderBy(o => o.TDL_PATIENT_FIRST_NAME).ToList();
                }

                barCodeTag.ProcessData(store, dicImage);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("rdo.keyName____", rdo.keyName));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("rdo.listAdo1__", rdo.listAdo));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("medicineUseForms__", medicineUseForms));

                GetMedicineGroup();
                GetMedicineParent();
                GetOtherPaySourceGroup();
                objectTag.AddObjectData(store, "OtherPaySourceGroup", listOtherPaySource);
                List<Mps000049ADO> listAdoFilter = new List<Mps000049ADO>();
                foreach (var item in rdo.listAdoArrangeM_T_N)
                {
                    // Kiểm tra nếu MEDICINE_GROUP_NAME là null hoặc không chứa các từ khóa cụ thể
                    string name = item.MEDICINE_GROUP_NAME?.Trim().ToUpper();
                    if (string.IsNullOrEmpty(name) ||
                        (!name.Contains("GÂY NGHIỆN") && !name.Contains("HƯỚNG THẦN") &&
                         !name.Contains("LAO")))
                    {
                        //bool needSeparate = item.MEDICINE_GROUP_ID.HasValue &&
                        //   CheckSaperate(item.MEDICINE_GROUP_ID.Value);
                        if (string.IsNullOrEmpty(name) || (!name.Contains("KHÁNG SINH") || (name.Contains("KHÁNG SINH") && !CheckSaperate(item.MEDICINE_GROUP_ID.Value))))
                        {
                            // Tìm phần tử trong listMedicineType có MEDICINE_GROUP_NAME = "PHIẾU LĨNH THUỐC THƯỜNG"
                            var commonMedicineType = listMedicineType.FirstOrDefault(o => o.MEDICINE_GROUP_NAME == PHIEU_LINH_THUOC_THUONG);
                            if (commonMedicineType != null)
                            {
                                // Gán MEDICINE_GROUP_ID từ commonMedicineType
                                item.MEDICINE_GROUP_ID = commonMedicineType.MEDICINE_GROUP_ID;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Không tìm thấy phần tử trong listMedicineType với MEDICINE_GROUP_NAME = 'PHIẾU LĨNH THUỐC THƯỜNG'");
                            }
                        }

                    }
                    listAdoFilter.Add(item);
                }
                List<ExpMestADO> listParentFilter = new List<ExpMestADO>();
                foreach (var item in listMedicineParent)
                {
                    string name = item.MEDICINE_GROUP_NAME?.Trim().ToUpper();
                    if (string.IsNullOrEmpty(name) ||
                        (!name.Contains("GÂY NGHIỆN") && !name.Contains("HƯỚNG THẦN") &&
                         !name.Contains("LAO")))
                    {
                        //bool needSeparate = item.MEDICINE_GROUP_ID.HasValue &&
                        //   CheckSaperate(item.MEDICINE_GROUP_ID.Value);
                        if (string.IsNullOrEmpty(name) || (!name.Contains("KHÁNG SINH") || (name.Contains("KHÁNG SINH") && !CheckSaperate(item.MEDICINE_GROUP_ID.Value))))
                        {
                            var commonMedicineType = listMedicineType.FirstOrDefault(o => o.MEDICINE_GROUP_NAME == PHIEU_LINH_THUOC_THUONG);
                            if (commonMedicineType != null)
                            {
                                item.MEDICINE_GROUP_ID = commonMedicineType.MEDICINE_GROUP_ID;
                            }
                        }

                    }
                    listParentFilter.Add(item);
                }
                if (rdo.ConfigMps49._ConfigKeyOderOption != 6)
                {
                    listAdoFilter = listAdoFilter
                                    .OrderBy(p => p.MEDI_MATE_NUM_ORDER == 0 ? 1 : 0)
                                    .ThenBy(p => p.MEDI_MATE_NUM_ORDER)
                                    .ThenBy(p => p.MEDICINE_TYPE_NAME)
                                    .ToList();
                }
                else
                {
                    listAdoFilter.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
                }

                // DEBUG 2778: đếm số bản ghi đẩy vào band để xác định FlexCel có bị phình dòng không
                Inventec.Common.Logging.LogSystem.Debug(String.Format(
                    "MPS49_COUNT____listAdo={0}; listAdoFilter={1}; ExpMestADOs={2}; ExpMestADOsSplit={3}; MedicineUseForms={4}; MedicineGroup(listMedicineType)={5}; MedicineParent(listParentFilter)={6}; OtherPaySourceGroup={7}",
                    rdo.listAdo != null ? rdo.listAdo.Count : -1,
                    listAdoFilter != null ? listAdoFilter.Count : -1,
                    this.ExpMestADOs != null ? this.ExpMestADOs.Count : -1,
                    this.ExpMestADOsSplit != null ? this.ExpMestADOsSplit.Count : -1,
                    medicineUseForms != null ? medicineUseForms.Count : -1,
                    listMedicineType != null ? listMedicineType.Count : -1,
                    listParentFilter != null ? listParentFilter.Count : -1,
                    listOtherPaySource != null ? listOtherPaySource.Count : -1));

                objectTag.AddObjectData(store, "ExpMestAggregates", rdo.listAdo);
                objectTag.AddObjectData(store, "ExpMestAggregates1", listAdoFilter);
                objectTag.AddObjectData(store, "ExpMests", this.ExpMestADOs);
                //Bổ sung key gom theo lô
                objectTag.AddObjectData(store, "ExpMestsSplit", this.ExpMestADOsSplit);
                objectTag.AddObjectData(store, "MedicineUseForms", medicineUseForms);
                objectTag.AddObjectData(store, "MedicineGroup", listMedicineType);
                objectTag.AddObjectData(store, "MedicineParent", listParentFilter);

                // Tách trang theo nhóm trang (thuốc thường / thuốc kiểm soát đặc biệt).
                // Bộ dataset + quan hệ này là THÊM MỚI, độc lập với khối quan hệ đang tạm tắt bên dưới.
                ProcessPageGroups(objectTag);

                // ===== DIAGNOSTIC 2778: TẠM TẮT toàn bộ relationship để khoanh vùng lỗi FlexCel DeleteRange 1048577 =====
                // Nếu in được sau khi tắt -> quan hệ master-detail thiếu band master trong template là nguyên nhân.
                // Nếu vẫn lỗi -> do cấu trúc band/template. ĐÂY LÀ TEST TẠM, sẽ khôi phục sau.
                /*
                objectTag.AddRelationship(store, "ExpMestAggregates", "ExpMests", new string[] { "MEDI_MATE_TYPE_ID", "TYPE_ID" }, new string[] { "MEDI_MATE_TYPE_ID", "TYPE_ID" });
                objectTag.AddRelationship(store, "ExpMestAggregates1", "ExpMests", new string[] { "MEDI_MATE_TYPE_ID", "TYPE_ID" }, new string[] { "MEDI_MATE_TYPE_ID", "TYPE_ID" });

                objectTag.AddRelationship(store, "OtherPaySourceGroup", "ExpMestsSplit", "OTHER_PAY_SOURCE_ID", "OTHER_PAY_SOURCE_ID");

                objectTag.AddRelationship(store, "OtherPaySourceGroup", "ExpMestAggregates", "OTHER_PAY_SOURCE_ID", "OTHER_PAY_SOURCE_ID");
                objectTag.AddRelationship(store, "OtherPaySourceGroup", "ExpMestAggregates1", "OTHER_PAY_SOURCE_ID", "OTHER_PAY_SOURCE_ID");

                objectTag.AddRelationship(store, "OtherPaySourceGroup", "ExpMests", "OTHER_PAY_SOURCE_ID", "OTHER_PAY_SOURCE_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "MedicineParent", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineUseForms", "ExpMests", "MEDICINE_USE_FORM_CODE", "MEDICINE_USE_FORM_CODE");

                objectTag.AddRelationship(store, "MedicineUseForms", "ExpMestAggregates", "MEDICINE_USE_FORM_CODE", "MEDICINE_USE_FORM_CODE");
                objectTag.AddRelationship(store, "MedicineUseForms", "ExpMestAggregates1", "MEDICINE_USE_FORM_CODE", "MEDICINE_USE_FORM_CODE");

                objectTag.AddRelationship(store, "MedicineGroup", "ExpMestAggregates", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "ExpMestAggregates1", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");

                objectTag.AddRelationship(store, "MedicineGroup", "ExpMests", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");

                objectTag.AddRelationship(store, "MedicineParent", "ExpMestAggregates", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ExpMestAggregates1", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");

                objectTag.AddRelationship(store, "MedicineParent", "ExpMests", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                */
                // ===== HẾT phần tạm tắt =====

                // 2778: Khi cấu hình BẬT → loại "Sản phẩm không phải là thuốc" (IS_FUNCTIONAL_FOOD = 1)
                // khỏi các nhóm dòng thuốc, để đưa sang phiếu lĩnh riêng (NHÓM 4 bên dưới).
                bool separateFunctionalFood = IsSeparateFunctionalFood();

                // --- NHÓM 1: VT YHCT (MedicineVT_YHCT) ---
                var vtData = rdo.listAdo.Where(o => o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT
                    && (!separateFunctionalFood || o.IS_FUNCTIONAL_FOOD != 1)).ToList();
                var vtParentIds = vtData.Select(o => o.MEDICINE_PARENT_ID).Distinct().ToList();
                var vtParents = listParentFilter.Where(o => vtParentIds.Contains(o.MEDICINE_PARENT_ID)).ToList();

                var page1Checker = new List<object>();

                if (vtData.Count > 0 )
                {
                    page1Checker.Add(new { HasData = true });
                }

                objectTag.AddObjectData(store, "Page1Control", page1Checker);
                objectTag.AddObjectData(store, "MedicineVT_YHCT", vtData);
                objectTag.AddObjectData(store, "MedicineVT_YHCTParent", vtParents);
                objectTag.AddRelationship(store, "MedicineVT_YHCTParent", "MedicineVT_YHCT", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");

                // --- NHÓM 2: TÂN DƯỢC (MedicineTTD) ---
                var ttdData = rdo.listAdo.Where(o => o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__TTD
                    && (!separateFunctionalFood || o.IS_FUNCTIONAL_FOOD != 1)).ToList();
                var ttdParentIds = ttdData.Select(o => o.MEDICINE_PARENT_ID).Distinct().ToList();
                var ttdParents = listParentFilter.Where(o => ttdParentIds.Contains(o.MEDICINE_PARENT_ID)).ToList();

                objectTag.AddObjectData(store, "MedicineTTD", ttdData);
                objectTag.AddObjectData(store, "MedicineTTDParent", ttdParents);
                objectTag.AddRelationship(store, "MedicineTTDParent", "MedicineTTD", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");

                // --- NHÓM 3: CHẾ PHẨM YHCT (MedicineCP_YHCT) ---
                var cpData = rdo.listAdo.Where(o => o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__CP_YHCT
                    && (!separateFunctionalFood || o.IS_FUNCTIONAL_FOOD != 1)).ToList();
                var cpParentIds = cpData.Select(o => o.MEDICINE_PARENT_ID).Distinct().ToList();
                var cpParents = listParentFilter.Where(o => cpParentIds.Contains(o.MEDICINE_PARENT_ID)).ToList();

                objectTag.AddObjectData(store, "MedicineCP_YHCT", cpData);
                objectTag.AddObjectData(store, "MedicineCP_YHCTParent", cpParents);
                objectTag.AddRelationship(store, "MedicineCP_YHCTParent", "MedicineCP_YHCT", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");

                var page2Checker = new List<object>();

                if (ttdData.Count > 0 || cpData.Count > 0)
                {
                    page2Checker.Add(new { HasData = true });
                }

                objectTag.AddObjectData(store, "Page2Control", page2Checker);

                // --- NHÓM 4 (2778): SẢN PHẨM KHÔNG PHẢI LÀ THUỐC (MedicineFunctionalFood) ---
                // Chỉ có dữ liệu khi cấu hình BẬT; template thêm 1 trang riêng bind dataset này,
                // ẩn/hiện theo Page3Control. Khi TẮT → rỗng → trang ẩn → phiếu cũ giữ nguyên.
                var ffData = separateFunctionalFood
                    ? rdo.listAdo.Where(o => o.IS_FUNCTIONAL_FOOD == 1).ToList()
                    : new List<Mps000049ADO>();
                var ffParentIds = ffData.Select(o => o.MEDICINE_PARENT_ID).Distinct().ToList();
                var ffParents = listParentFilter.Where(o => ffParentIds.Contains(o.MEDICINE_PARENT_ID)).ToList();

                var page3Checker = new List<object>();
                if (ffData.Count > 0)
                {
                    page3Checker.Add(new { HasData = true });
                }

                objectTag.AddObjectData(store, "Page3Control", page3Checker);
                objectTag.AddObjectData(store, "MedicineFunctionalFood", ffData);
                objectTag.AddObjectData(store, "MedicineFunctionalFoodParent", ffParents);
                objectTag.AddRelationship(store, "MedicineFunctionalFoodParent", "MedicineFunctionalFood", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");

                // Slot phụ trong template Trang 3 (clone từ "Chế phẩm YHCT") — luôn rỗng để tự ẩn.
                // Vẫn phải đăng ký dataset để FlexCel không lỗi "dataset not found".
                var ffCpData = new List<Mps000049ADO>();
                var ffCpParents = new List<ExpMestADO>();
                objectTag.AddObjectData(store, "MedicineFunctionalFoodCp", ffCpData);
                objectTag.AddObjectData(store, "MedicineFunctionalFoodCpParent", ffCpParents);
                objectTag.AddRelationship(store, "MedicineFunctionalFoodCpParent", "MedicineFunctionalFoodCp", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");

                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData13", new CalculateMergerData());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        /// <summary>
        /// Sinh dataset "PageGroups" (mỗi bản ghi = 1 trang phiếu) và quan hệ tới các dòng thuốc,
        /// để template bind band phủ cả trang vào __PageGroups__ và ngắt trang theo nhóm:
        /// trang 1 = thuốc thường, trang 2 = thuốc gây nghiện/hướng thần/tiền chất.
        /// Nhóm nào không có dữ liệu thì không sinh trang.
        /// </summary>
        private void ProcessPageGroups(Inventec.Common.FlexCellExport.ProcessObjectTag objectTag)
        {
            try
            {
                List<Mps000049ADO> all = (rdo.listAdo != null) ? rdo.listAdo : new List<Mps000049ADO>();

                // Gán khóa nhóm trang + tính sẵn số lượng in và thành tiền cho từng dòng.
                // listAdo và listAdoArrangeM_T_N dùng CHUNG instance nên chỉ cần gán 1 lần.
                foreach (var item in all)
                {
                    StampPageGroupFields(item);
                }
                if (ExpMestADOsSplit != null)
                {
                    foreach (var item in ExpMestADOsSplit)
                    {
                        StampPageGroupFields(item);
                    }
                }

                List<Mps000049PageGroupADO> pageGroups = new List<Mps000049PageGroupADO>();
                AddPageGroup(pageGroups, all, PAGE_GROUP_ID__NORMAL, PAGE_GROUP_NAME__NORMAL);
                AddPageGroup(pageGroups, all, PAGE_GROUP_ID__CONTROLLED, PAGE_GROUP_NAME__CONTROLLED);

                objectTag.AddObjectData(store, "PageGroups", pageGroups);
                objectTag.AddRelationship(store, "PageGroups", "ExpMestAggregates", "PAGE_GROUP_ID", "PAGE_GROUP_ID");
                objectTag.AddRelationship(store, "PageGroups", "ExpMestAggregates1", "PAGE_GROUP_ID", "PAGE_GROUP_ID");

                Inventec.Common.Logging.LogSystem.Debug(String.Format(
                    "MPS49_PAGEGROUPS____so trang={0}; chi tiet={1}",
                    pageGroups.Count,
                    String.Join(" | ", pageGroups.Select(o => o.PAGE_GROUP_NAME + ": " + o.ITEM_COUNT + " khoan, tong tien " + o.TOTAL_PRICE))));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gán nhóm trang, số lượng in và thành tiền cho 1 dòng thuốc/vật tư.
        /// </summary>
        private static void StampPageGroupFields(Mps000049ADO ado)
        {
            try
            {
                if (ado == null) return;

                ado.PAGE_GROUP_ID = IsControlledMedicine(ado) ? PAGE_GROUP_ID__CONTROLLED : PAGE_GROUP_ID__NORMAL;
                // Phiếu chưa duyệt/xuất thì AMOUNT_EXCUTE = 0 -> lấy số yêu cầu để phiếu không in ra số 0
                ado.AMOUNT_PRINT = (ado.AMOUNT_EXCUTE > 0) ? ado.AMOUNT_EXCUTE : ado.AMOUNT_REQUEST;
                ado.TOTAL_PRICE = ado.AMOUNT_PRINT * (ado.PRICE ?? 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Thuốc phải kiểm soát đặc biệt: gây nghiện, hướng thần, tiền chất và các dạng phối hợp.
        /// Ưu tiên so theo ID nhóm thuốc; fallback theo tên nhóm cho dữ liệu cũ/nhóm tự tạo.
        /// </summary>
        private static bool IsControlledMedicine(Mps000049ADO ado)
        {
            if (ado == null) return false;

            long groupId = ado.MEDICINE_GROUP_ID ?? 0;
            if (groupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                || groupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                || groupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC
                || groupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN
                || groupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT)
            {
                return true;
            }

            string name = (ado.MEDICINE_GROUP_NAME ?? "").Trim().ToUpper();
            return name.Contains("GÂY NGHIỆN") || name.Contains("HƯỚNG THẦN") || name.Contains("TIỀN CHẤT");
        }

        /// <summary>
        /// Tạo 1 bản ghi nhóm trang kèm các số tổng tính sẵn. Không có dòng nào thì bỏ qua (không in trang rỗng).
        /// </summary>
        private static void AddPageGroup(List<Mps000049PageGroupADO> result, List<Mps000049ADO> all, long pageGroupId, string pageGroupName)
        {
            try
            {
                List<Mps000049ADO> rows = all.Where(o => o.PAGE_GROUP_ID == pageGroupId).ToList();
                if (rows.Count <= 0) return;

                Mps000049PageGroupADO ado = new Mps000049PageGroupADO();
                ado.PAGE_GROUP_ID = pageGroupId;
                ado.PAGE_GROUP_NAME = pageGroupName;
                ado.PAGE_GROUP_TITLE = PAGE_GROUP_TITLE_PREFIX + pageGroupName;
                ado.ITEM_COUNT = rows.Count;
                ado.TOTAL_AMOUNT = rows.Sum(o => o.AMOUNT_PRINT);
                ado.TOTAL_PRICE = rows.Sum(o => o.TOTAL_PRICE ?? 0);
                ado.TOTAL_PRICE_TEXT = Inventec.Common.String.Convert.CurrencyToVneseString(
                    string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_PRICE)));

                result.Add(ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListMedicineUse()
        {
            try
            {
                medicineUseForms = new List<MedicineUseFormADO>();
                Inventec.Common.Logging.LogSystem.Info("rdo.keyName:" + rdo.keyName + "  keyTitles.phieuLinhThuocThuong:" + keyTitles.phieuLinhThuocThuong);

                if (rdo.keyName == keyTitles.phieuLinhThuocThuong)
                {
                    Inventec.Common.Logging.LogSystem.Info("true__");

                    var lstAdo = rdo.listAdo.Distinct();

                    foreach (var item in rdo.listAdo)
                    {

                        if (!medicineUseForms.Exists(o => o.MEDICINE_USE_FORM_ID == item.MEDICINE_USE_FORM_ID))
                        {
                            MedicineUseFormADO medicineUseForm = new MedicineUseFormADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MedicineUseFormADO>(medicineUseForm, item);
                            medicineUseForms.Add(medicineUseForm);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetSingleKey()
        {
            try
            {

                if (rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {
                    var minTime = rdo._ExpMests_Print.Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._ExpMests_Print.Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));

                }

                switch (rdo.keyName)
                {
                    case keyTitles.phieuLinhTongHop:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "TỔNG HỢP"));
                        break;
                    case keyTitles.phieuLinhThuocThuong:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC THƯỜNG"));
                        break;
                    case keyTitles.Corticoid:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC CORTICOID"));
                        break;
                    case keyTitles.KhangSinh:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC KHÁNG SINH"));
                        break;
                    case keyTitles.Lao:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC LAO"));
                        break;
                    case keyTitles.DichTruyen:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC DỊCH TRUYỀN"));
                        break;
                    case keyTitles.TienChat:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, "THUỐC TIỀN CHẤT"));
                        break;
                    default:
                        SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KEY_NAME_TITLES, ""));
                        break;
                }

                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>(rdo.AggrExpMest, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));

                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.PARENT_TYPE_CODE, rdo.ConfigMps49.PARENT_TYPE_CODE));
                if (this.rdo.ListAggrExpMest != null && this.rdo.ListAggrExpMest.Count > 0)
                {

                    string expMestCodes = string.Join(",", rdo.ListAggrExpMest.Select(s => s.EXP_MEST_CODE).Distinct().ToList());
                    SetSingleKey((new KeyValue(Mps000049ExtendSingleKey.EXP_MEST_CODEs, expMestCodes)));

                    List<string> createTime = new List<string>();
                    var list = this.rdo._ExpMestMedicines.Select(o => o.AGGR_EXP_MEST_ID).ToList();
                    this.rdo.ListAggrExpMest = this.rdo.ListAggrExpMest.OrderBy(o => o.CREATE_TIME).Where(o => o.MEDI_STOCK_CODE == rdo.AggrExpMest.MEDI_STOCK_CODE).ToList();
                    foreach (var item in this.rdo.ListAggrExpMest)
                    {
                        if (list != null && list.Count > 0 && list.Contains(item.ID))
                        {
                            createTime.Add(Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.CREATE_DATE));
                        }
                    }
                    createTime = createTime.Distinct().ToList();
                    var createTimeStr = String.Join(",", createTime);
                    SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.CREATE_TIME_AGGR_STR, createTimeStr));
                }

                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.TOTAL_REQ_ROOM_NAME_DISPLAY, totalReqRoomName));

                // Ca chạy thận (KIDNEY_SHIFT) — header phiếu lĩnh tổng hợp.
                // Ưu tiên lấy từ phiếu lĩnh tổng (AggrExpMest); fallback các phiếu xuất con (đơn dự trù cùng 1 ca).
                long? kidneyShift = (rdo.AggrExpMest != null) ? rdo.AggrExpMest.KIDNEY_SHIFT : null;
                if ((kidneyShift == null || kidneyShift <= 0) && rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {
                    kidneyShift = rdo._ExpMests_Print
                        .Where(o => o.KIDNEY_SHIFT != null && o.KIDNEY_SHIFT > 0)
                        .Select(o => o.KIDNEY_SHIFT)
                        .FirstOrDefault();
                }
                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.KIDNEY_SHIFT,
                    (kidneyShift != null && kidneyShift > 0) ? kidneyShift.ToString() : ""));

                // 2778: cờ cấu hình để template gate tiêu đề SPKPLT (chỉ áp dụng khi BẬT)
                SetSingleKey(new KeyValue(Mps000049ExtendSingleKey.SEPARATE_FUNCTIONAL_FOOD, IsSeparateFunctionalFood() ? "1" : "0"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string totalReqRoomName = "";
        private bool CheckSaperate(long medicineGroupId)
        {
            var medicineGroup = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().Where(o => o.ID == medicineGroupId).FirstOrDefault();
            if (medicineGroup.IS_SEPARATE_PRINTING == 1)
            {
                return true;
            }
            return false;
        }
        private void GetMedicineGroup()
        {
            try
            {
                if (ExpMestADOs != null && ExpMestADOs.Count > 0)
                {
                    var group = ExpMestADOs
                        .GroupBy(o =>
                        {
                            string name = (o.MEDICINE_GROUP_NAME ?? "").Trim().ToUpper();
                            if (name.Contains("GÂY NGHIỆN"))
                            {
                                if (name.Contains("CHỨA DƯỢC CHẤT GÂY NGHIỆN"))
                                    return "PHIẾU LĨNH THUỐC GÂY NGHIỆN Ở DẠNG PHỐI HỢP";
                                else
                                    return "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                            }
                            else if (name.Contains("HƯỚNG THẦN"))
                            {
                                return "PHIẾU LĨNH THUỐC HƯỚNG THẦN";
                            }
                            else if (name.Contains("LAO"))
                            {
                                return "PHIẾU LĨNH THUỐC ĐIỀU TRỊ LAO";
                            }
                            else if (name.Contains("KHÁNG SINH") && CheckSaperate(o.MEDICINE_GROUP_ID.Value)) // những thuốc thuộc nhóm KS luôn có MEDICINE_GROUP_ID nên không cần check null
                            {
                                return "PHIẾU LĨNH THUỐC KHÁNG SINH";
                            }
                            else
                            {
                                // Gom tất cả thuốc còn lại vào 1 nhóm duy nhất
                                return PHIEU_LINH_THUOC_THUONG;
                            }
                        })
                        .OrderBy(g => g.Key);

                    foreach (var item in group)
                    {
                        var firstItem = item.First();
                        firstItem.MEDICINE_GROUP_NAME = item.Key; // Gán lại tên nhóm cho đúng loại phiếu
                        //if (item.Key == "PHIẾU LĨNH THUỐC THƯỜNG")
                        //    firstItem.MEDICINE_GROUP_ID = -1;
                        listMedicineType.Add(firstItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetOtherPaySourceGroup()
        {
            try
            {
                if (ExpMestADOs != null && ExpMestADOs.Count > 0)
                {
                    var group = ExpMestADOs.GroupBy(o => new { o.OTHER_PAY_SOURCE_ID, o.OTHER_PAY_SOURCE_CODE, o.OTHER_PAY_SOURCE_NAME });
                    foreach (var item in group)
                    {
                        listOtherPaySource.Add(item.ToList().First());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //private void GetMedicineParent()
        //{
        //    try
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug("ExpMestADOs: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", ExpMestADOs.Count));

        //        if (ExpMestADOs != null && ExpMestADOs.Count > 0)
        //        {
        //            //var group = ExpMestADOs.GroupBy(o => new { o.MEDICINE_PARENT_ID }).OrderBy(g => g.First().MEDICINE_PARENT_NAME);
        //            //foreach (var item in group)
        //            //{
        //            //    listMedicineParent.Add(item.ToList().First());
        //            //}
        //            var group = ExpMestADOs
        //            .GroupBy(o => new { o.MEDICINE_PARENT_ID })
        //            .OrderBy(g =>
        //            {
        //                var name = g.First().MEDICINE_PARENT_NAME?.Trim() ?? "";
        //                //Lấy phần số ở đầu chuỗi, VD: "10.2.Thuốc" => "10.2"
        //                var match = System.Text.RegularExpressions.Regex.Match(name, @"^(\d+(\.\d+)*)");
        //                if (match.Success)
        //                {
        //                    string value = match.Value;

        //                    //Lấy phần trước dấu chấm cuối cùng, nếu có
        //                    int lastDot = value.LastIndexOf('.');
        //                    string prefix = lastDot > 0 ? value.Substring(0, lastDot) : value;

        //                    //Chuyển thành số thực để so sánh
        //                    if (decimal.TryParse(prefix, out var num))
        //                        return num;
        //                }
        //                // Không có số ở đầu => cho ra cuối danh sách
        //                return decimal.MaxValue;
        //            })
        //            .ThenBy(g => g.First().MEDICINE_PARENT_NAME); // nếu trùng số, sắp theo tên
        //            foreach (var item in group)
        //            {
        //                listMedicineParent.Add(item.ToList().First());
        //            }
        //        }
        //        Inventec.Common.Logging.LogSystem.Debug("listMedicineParent: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", listMedicineParent.Count));
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}


        private void GetMedicineParent()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ExpMestADOs: " +
                    Inventec.Common.Logging.LogUtil.TraceData("DataA", ExpMestADOs?.Count));

                if (ExpMestADOs != null && ExpMestADOs.Count > 0)
                {
                    var group = ExpMestADOs
                        .GroupBy(o => o.MEDICINE_PARENT_ID)
                        .OrderBy(g =>
                        {
                            var name = g.FirstOrDefault()?.MEDICINE_PARENT_NAME?.Trim() ?? "";
                            var match = System.Text.RegularExpressions.Regex.Match(name, @"^(\d+(\.\d+)*)");

                            if (match.Success)
                            {
                        // Tách phần số: "6.2.1" => ["6", "2", "1"]
                        string[] parts = match.Value.Split('.');

                        // PadLeft để so sánh đúng theo số học
                        return string.Join(".", parts.Select(p => p.PadLeft(5, '0')));
                            }

                    // Không có số ở đầu => cho ra cuối danh sách
                    return "99999.99999.99999";
                        })
                        .ThenBy(g => g.FirstOrDefault()?.MEDICINE_PARENT_NAME);

                    foreach (var item in group)
                    {
                        listMedicineParent.Add(item.First());
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("listMedicineParent: " +
                    Inventec.Common.Logging.LogUtil.TraceData("DataA", listMedicineParent.Select(o => o.MEDICINE_PARENT_NAME)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessListADO()
        {
            try
            {
                List<long> reqRoomIds = new List<long>();
                ExpMestADOs = new List<ExpMestADO>();
                ExpMestADOsSplit = new List<Mps000049ADO>();
                if (rdo.IsMedicine && rdo._ExpMestMedicines != null && rdo._ExpMestMedicines.Count > 0)
                {
                    var query = rdo._ExpMestMedicines.ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        query = query.Where(p => rdo.RoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                    }

                    query = query.Where(p => Check(p)).ToList();

                    if (query != null && query.Count > 0)
                    {
                        reqRoomIds.AddRange(query.Select(s => s.REQ_ROOM_ID).ToList());
                    }

                    List<Mps000049ADO> dataMedis = new List<Mps000049ADO>();
                    if (rdo.ConfigMps49._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new { g.MEDICINE_ID, g.CONCENTRA }).Select(p => p.ToList()).ToList();
                        dataMedis.AddRange(from r in Groups
                                           select new Mps000049ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._MedicineTypes,
                                               rdo.ConfigMps49._ExpMestSttId__Approved,
                                               rdo.ConfigMps49._ExpMestSttId__Exported,
                                               rdo.ConfigMps49.PatientTypeId__BHYT));
                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();

                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._MedicineTypes, rdo.ConfigMps49._ExpMestSttId__Approved, rdo.ConfigMps49._ExpMestSttId__Exported, rdo.ConfigMps49.PatientTypeId__BHYT, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.CONCENTRA }).Select(p => p.ToList()).ToList();
                        dataMedis.AddRange(from r in Groups
                                           select new Mps000049ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._MedicineTypes,
                                               rdo.ConfigMps49._ExpMestSttId__Approved,
                                               rdo.ConfigMps49._ExpMestSttId__Exported,
                                               rdo.ConfigMps49.PatientTypeId__BHYT));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {

                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();

                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._MedicineTypes, rdo.ConfigMps49._ExpMestSttId__Approved, rdo.ConfigMps49._ExpMestSttId__Exported, rdo.ConfigMps49.PatientTypeId__BHYT, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }

                    if (dataMedis != null && dataMedis.Count > 0)
                    {
                        switch (rdo.ConfigMps49._ConfigKeyOderOption)
                        {
                            case 1:
                                dataMedis = dataMedis.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 2:
                                dataMedis = dataMedis.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 3:
                                dataMedis = dataMedis.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 4:
                                dataMedis = dataMedis.OrderBy(p => p.SERVICE_UNIT_NAME).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 5:
                                dataMedis = dataMedis
                                    .OrderBy(p => p.MEDICINE_TYPE_NUM_ORDER == null ? 1 : 0)
                                    .ThenBy(p => p.MEDICINE_TYPE_NUM_ORDER)
                                    .ThenBy(p => p.MEDICINE_TYPE_NAME)
                                    .ToList();
                                break;
                            case 6:
                                dataMedis = dataMedis
                                    .OrderBy(p => p.MEDI_MATE_NUM_ORDER == 0 ? 1 : 0)
                                    .ThenBy(p => p.MEDI_MATE_NUM_ORDER)
                                    .ThenBy(p => p.MEDICINE_TYPE_NAME)
                                    .ToList();
                                break;
                        }
                        rdo.listAdo.AddRange(dataMedis);
                        rdo.listAdoArrangeM_T_N.AddRange(dataMedis);
                    }

                    #region Group Split
                    var GroupsSplit = query.GroupBy(g => new { g.MEDICINE_ID, g.CONCENTRA }).Select(p => p.ToList()).ToList();
                    ExpMestADOsSplit.AddRange(from r in GroupsSplit
                                              select new Mps000049ADO(rdo.AggrExpMest,
                                                  r,
                                                  rdo._MedicineTypes,
                                                  rdo.ConfigMps49._ExpMestSttId__Approved,
                                                  rdo.ConfigMps49._ExpMestSttId__Exported,
                                                  rdo.ConfigMps49.PatientTypeId__BHYT));
                    #endregion
                }

                //TODO VT
                if (rdo._ExpMestMaterials != null && rdo._ExpMestMaterials.Count > 0)
                {
                    var query = rdo._ExpMestMaterials.ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0 && rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                    {
                        query = query.Where(p => rdo.RoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                    }

                    query = query.Where(p => Check(p)).ToList();

                    if (query != null && query.Count > 0)
                    {
                        reqRoomIds.AddRange(query.Select(s => s.REQ_ROOM_ID).ToList());
                    }

                    List<Mps000049ADO> dataMates = new List<Mps000049ADO>();
                    if (rdo.ConfigMps49._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new { g.MATERIAL_ID, g.IS_CHEMICAL_SUBSTANCE }).Select(p => p.ToList()).ToList();
                        dataMates.AddRange(from r in Groups
                                           select new Mps000049ADO(rdo.AggrExpMest,
                                               r,
                                               rdo.ConfigMps49._ExpMestSttId__Approved,
                                               rdo.ConfigMps49._ExpMestSttId__Exported,
                                               rdo.ConfigMps49.PatientTypeId__BHYT));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo.ConfigMps49._ExpMestSttId__Approved, rdo.ConfigMps49._ExpMestSttId__Exported, rdo.ConfigMps49.PatientTypeId__BHYT, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.IS_CHEMICAL_SUBSTANCE }).Select(p => p.ToList()).ToList();
                        dataMates.AddRange(from r in Groups
                                           select new Mps000049ADO(rdo.AggrExpMest,
                                               r,
                                               rdo.ConfigMps49._ExpMestSttId__Approved,
                                               rdo.ConfigMps49._ExpMestSttId__Exported,
                                               rdo.ConfigMps49.PatientTypeId__BHYT));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo.ConfigMps49._ExpMestSttId__Approved, rdo.ConfigMps49._ExpMestSttId__Exported, rdo.ConfigMps49.PatientTypeId__BHYT, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }

                    if (dataMates != null && dataMates.Count > 0)
                    {
                        if (rdo.ConfigMps49._ConfigKeyOderOption == 6)
                        {
                            dataMates = dataMates
                                    .OrderBy(p => p.MEDI_MATE_NUM_ORDER == 0 ? 1 : 0)
                                    .ThenBy(p => p.MEDI_MATE_NUM_ORDER)
                                    .ThenBy(p => p.MEDICINE_TYPE_NAME)
                                    .ToList();
                        }
                        else
                        {
                            dataMates = dataMates.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        }
                        rdo.listAdo.AddRange(dataMates);
                        rdo.listAdoArrangeM_T_N.AddRange(dataMates);
                    }

                    var GroupsSplit = query.GroupBy(g => new { g.MATERIAL_ID, g.IS_CHEMICAL_SUBSTANCE }).Select(p => p.ToList()).ToList();
                    ExpMestADOsSplit.AddRange(from r in GroupsSplit
                                              select new Mps000049ADO(rdo.AggrExpMest,
                                           r,
                                           rdo.ConfigMps49._ExpMestSttId__Approved,
                                           rdo.ConfigMps49._ExpMestSttId__Exported,
                                           rdo.ConfigMps49.PatientTypeId__BHYT));

                    if (ExpMestADOsSplit != null && ExpMestADOsSplit.Count > 0)
                    {
                        switch (rdo.ConfigMps49._ConfigKeyOderOption)
                        {
                            case 1:
                                ExpMestADOsSplit = ExpMestADOsSplit.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 2:
                                ExpMestADOsSplit = ExpMestADOsSplit.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 3:
                                ExpMestADOsSplit = ExpMestADOsSplit.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 4:
                                ExpMestADOsSplit = ExpMestADOsSplit.OrderBy(p => p.SERVICE_UNIT_NAME).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 5:
                                ExpMestADOsSplit = ExpMestADOsSplit
                                    .OrderBy(p => p.MEDICINE_TYPE_NUM_ORDER == null ? 1 : 0)
                                    .ThenBy(p => p.MEDICINE_TYPE_NUM_ORDER)
                                    .ThenBy(p => p.MEDICINE_TYPE_NAME)
                                    .ToList();
                                break;
                        }
                    }
                }

                if (reqRoomIds.Count > 0)
                {
                    var reqrooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => reqRoomIds.Contains(o.ID)).ToList();
                    if (reqrooms != null && reqrooms.Count > 0)
                    {
                        totalReqRoomName = String.Join("; ", reqrooms.Select(s => s.ROOM_NAME).OrderBy(o => o));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool Check(V_HIS_EXP_MEST_MEDICINE _expMestMedicine)
        {
            bool result = false;
            try
            {
                var data = rdo._MedicineTypes.FirstOrDefault(p => p.ID == _expMestMedicine.MEDICINE_TYPE_ID);
                if (data != null)
                {
                    if (rdo.ServiceUnitIds != null
                        && rdo.ServiceUnitIds.Count > 0)
                    {
                        if (rdo.ServiceUnitIds.Contains(data.SERVICE_UNIT_ID))
                            result = true;
                    }
                    if (data.MEDICINE_USE_FORM_ID > 0)
                    {
                        if (rdo.UseFormIds != null
                    && rdo.UseFormIds.Count > 0 && rdo.UseFormIds.Contains(data.MEDICINE_USE_FORM_ID ?? 0))
                        {
                            result = result && true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        bool Check(V_HIS_EXP_MEST_MATERIAL _expMestMaterial)
        {
            bool result = false;
            try
            {
                if (_expMestMaterial != null)
                {
                    if (rdo.ServiceUnitIds != null && rdo.ServiceUnitIds.Count > 0 && (rdo.Ismaterial || rdo.IsChemicalSustance))
                    {
                        if (rdo.ServiceUnitIds.Contains(_expMestMaterial.SERVICE_UNIT_ID))
                            result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }


        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (rdo != null && rdo._ExpMests_Print != null)
                {
                    List<string> treatmentCodes = rdo._ExpMests_Print.Select(s => s.TDL_TREATMENT_CODE).Distinct().ToList();
                    List<string> expMestCodes = rdo._ExpMests_Print.Select(s => s.EXP_MEST_CODE).Distinct().ToList();
                    log = LogDataExpMest(string.Join(";", treatmentCodes), string.Join(";", expMestCodes), "");
                }
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.AggrExpMest != null && rdo._ExpMests_Print != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", printTypeCode, rdo.AggrExpMest.EXP_MEST_CODE, rdo.AggrExpMest.MEDI_STOCK_CODE, rdo._ExpMests_Print.Count(), "Phiếu lĩnh", rdo.listAdo.FirstOrDefault().MEDICINE_TYPE_CODE, rdo.listAdo.Count());
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        class CalculateMergerData : TFlexCelUserFunction
        {
            long mediMateTypeId = 0;

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    long mediMateId = Convert.ToInt64(parameters[0]);

                    if (mediMateId > 0)
                    {
                        if (this.mediMateTypeId == mediMateId)
                        {
                            return true;
                        }
                        else
                        {
                            this.mediMateTypeId = mediMateId;
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    result = false;
                }
                return result;
            }
        }
    }
}
