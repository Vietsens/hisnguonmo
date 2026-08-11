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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000086.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using Inventec.Common.Logging;

namespace MPS.Processor.Mps000086
{
    class Mps000086Processor : AbstractProcessor
    {
        Mps000086PDO rdo;
        public Mps000086Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000086PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                List<Mps000086ADO> listAdoPrint = new List<Mps000086ADO>();
                List<Mps000086ADO> listAdoPrintSplitedByPackage = new List<Mps000086ADO>();
                List<Mps000086ADO> otherPaySourceList = new List<Mps000086ADO>();
                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    if (rdo._keyMert == 0)
                    {
                        //listAdoPrint ProcessData
                        var dataGroup = rdo.listAdo.GroupBy(p => new { p.TYPE_ID, p.MEDI_MATE_TYPE_ID, p.PRICE, p.IMP_PRICE, p.IMP_VAT_RATIO }).ToList();
                        foreach (var itemGroup in dataGroup)
                        {
                            Mps000086ADO ado = new Mps000086ADO();
                            //Inventec.Common.Mapper.DataObjectMapper.Map<Mps000086ADO>(ado,itemGroup.FirstOrDefault());
                            System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<Mps000086ADO>();
                            try
                            {
                                foreach (var item in pi)
                                {
                                    item.SetValue(ado, (item.GetValue(itemGroup.FirstOrDefault())));
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }

                            ado.PACKAGE_NUMBER_TOTAL = string.Join(", ", itemGroup.Select(s => s.PACKAGE_NUMBER).Distinct().ToList());
                            ado.TOTAL_AMOUNT = itemGroup.FirstOrDefault().TOTAL_AMOUNT;
                            ado.TOTAL_AMOUNT_IN_EXECUTE = itemGroup.Sum(p => p.TOTAL_AMOUNT_IN_EXECUTE);
                            ado.TOTAL_AMOUNT_IN_EXPORT = itemGroup.Sum(p => p.TOTAL_AMOUNT_IN_EXPORT);
                            ado.TOTAL_AMOUNT_IN_REQUEST = itemGroup.FirstOrDefault().TOTAL_AMOUNT_IN_REQUEST;
                            ado.BID_NUMBER = string.Join(",", itemGroup.Select(p => p.BID_NUMBER).Distinct().ToList());
                            ado.BID_PACKAGE_CODE = string.Join(",", itemGroup.Select(p => p.BID_PACKAGE_CODE).Distinct().ToList());
                            ado.BID_GROUP_CODE = string.Join(",", itemGroup.Select(p => p.BID_GROUP_CODE).Distinct().ToList());
                            ado.BID_YEAR = string.Join(",", itemGroup.Select(p => p.BID_YEAR).Distinct().ToList());


                            ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_REQUEST)));
                            ado.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_EXECUTE)));
                            ado.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_EXPORT)));
                            ado.EXP_PRICE_VP = itemGroup.FirstOrDefault().EXP_PRICE_VP;
                            ado.EXP_VAT_RATIO_VP = itemGroup.FirstOrDefault().EXP_VAT_RATIO_VP;
                            listAdoPrint.Add(ado);
                        }
                        //listAdoPrintSplitedByPackage ProcessData
                        var dataGroupSplitedByPackage = rdo.listAdo.GroupBy(p => new { p.TYPE_ID, p.MEDI_MATE_TYPE_ID, p.PRICE, p.IMP_PRICE, p.IMP_VAT_RATIO, p.PACKAGE_NUMBER, p.EXPIRED_DATE_STR }).ToList();
                        foreach (var itemGroup in dataGroupSplitedByPackage)
                        {
                            Mps000086ADO ado = new Mps000086ADO();
                            //Inventec.Common.Mapper.DataObjectMapper.Map<Mps000086ADO>(ado,itemGroup.FirstOrDefault());
                            System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<Mps000086ADO>();
                            try
                            {
                                foreach (var item in pi)
                                {
                                    item.SetValue(ado, (item.GetValue(itemGroup.FirstOrDefault())));
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }

                            ado.PACKAGE_NUMBER_TOTAL = string.Join(", ", itemGroup.Select(s => s.PACKAGE_NUMBER).Distinct().ToList());
                            ado.TOTAL_AMOUNT = itemGroup.FirstOrDefault().TOTAL_AMOUNT;
                            ado.TOTAL_AMOUNT_IN_EXECUTE = itemGroup.Sum(p => p.TOTAL_AMOUNT_IN_EXECUTE);
                            ado.TOTAL_AMOUNT_IN_EXPORT = itemGroup.Sum(p => p.TOTAL_AMOUNT_IN_EXPORT);
                            ado.TOTAL_AMOUNT_IN_REQUEST = itemGroup.FirstOrDefault().TOTAL_AMOUNT_IN_REQUEST;
                            ado.BID_NUMBER = string.Join(",", itemGroup.Select(p => p.BID_NUMBER).Distinct().ToList());
                            ado.BID_PACKAGE_CODE = string.Join(",", itemGroup.Select(p => p.BID_PACKAGE_CODE).Distinct().ToList());
                            ado.BID_GROUP_CODE = string.Join(",", itemGroup.Select(p => p.BID_GROUP_CODE).Distinct().ToList());
                            ado.BID_YEAR = string.Join(",", itemGroup.Select(p => p.BID_YEAR).Distinct().ToList());

                            ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_REQUEST)));
                            ado.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_EXECUTE)));
                            ado.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_EXPORT)));
                            ado.EXP_PRICE_VP = itemGroup.FirstOrDefault().EXP_PRICE_VP;
                            ado.EXP_VAT_RATIO_VP = itemGroup.FirstOrDefault().EXP_VAT_RATIO_VP;
                            listAdoPrintSplitedByPackage.Add(ado);
                        }
                    }
                    else
                    {
                        listAdoPrint = rdo.listAdo.ToList();
                        // Khi cau hinh gop (MERGER_DATA) tat: rdo.listAdo da la 1 dong/1 lo san.
                        // Van publish dataset SplitedByPackage de template PhieuXuatChuyenKho luon
                        // hien thi moi lo 1 dong rieng, khong phu thuoc gia tri _keyMert.
                        listAdoPrintSplitedByPackage = rdo.listAdo.ToList();
                    }
                    Inventec.Common.Logging.LogSystem.Debug("rdo.OderKey:____" + rdo.OrderKey);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listAdoPrint:____", listAdoPrint));
                    if (rdo.OrderKey != 0)
                    {
                        switch (rdo.OrderKey)
                        {
                            case 1:
                                listAdoPrint = listAdoPrint.OrderBy(p => p.TYPE_ID).ThenBy(p => p.MEDI_MATE_NUM_ORDER ?? 0).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                                break;
                            case 2:
                                listAdoPrint = listAdoPrint.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 3:
                                listAdoPrint = listAdoPrint.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                                break;
                            case 4:
                                listAdoPrint = listAdoPrint.OrderBy(p => p.SERVICE_UNIT_NAME).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                                break;
                            case 5:

                               
                                listAdoPrint = listAdoPrint
                                    .OrderBy(p => p.TYPE_ID == 1
                                        ? (string.IsNullOrWhiteSpace(p.PARENT_MEDICINE_TYPE_NAME) ? 2 : 0)
                                        : 1)
                                    .ThenBy(p => p.TYPE_ID == 1
                                        ? (p.PARENT_MEDICINE_TYPE_NAME ?? string.Empty)
                                        : string.Empty)
                                    .ThenBy(p => p.TYPE_ID == 1
                                        ? (p.MEDICINE_TYPE_NAME ?? string.Empty)
                                        : (p.MEDI_MATE_TYPE_NAME ?? string.Empty))
                                    .ToList();
                              BuildParentGroupName(listAdoPrint);
                                break;
                                 
                            case 6:
                                listAdoPrint = listAdoPrint.OrderBy(p => p.MEDI_MATE_NUM_ORDER.HasValue ? p.MEDI_MATE_NUM_ORDER.Value : listAdoPrint.Max(s =>s.MEDI_MATE_NUM_ORDER ?? 0) + 1).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                                break;
                            case 7:
                                // Sắp xếp theo TT 20/2022/TT-BYT Phụ lục I — phân Parent1/2/3 + Child + Other
                                // Logic dựng datasets nằm dưới ProcessData (khối BuildTT20Datasets) — case này chỉ đánh dấu OrderKey.
                                break;
                        }
                    }

                }

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                // === 3 datasets TT20 (theo yêu cầu TT 20/2022/TT-BYT Phụ lục I) ===
                // - TT20Group: gộp phẳng tất cả nhóm cha có STT, sort tuple (cấp1, cấp2, cấp3)
                // - TT20Child: thuốc lá thuộc TT20, link với cha trực tiếp qua PARENT_ID
                // - TT20Other: thuốc lá ngoài TT20 + vật tư/máu, sort A-Z
                // Dùng listAdoPrintSplitedByPackage (đã split theo PACKAGE_NUMBER + EXPIRED_DATE_STR)
                // → 1 thuốc/vt có nhiều lô/hạn sẽ tách thành nhiều dòng. Fallback về listAdoPrint nếu chưa split.
                List<Mps000086ADO> tt20Source = (listAdoPrintSplitedByPackage != null && listAdoPrintSplitedByPackage.Count > 0)
                    ? listAdoPrintSplitedByPackage
                    : listAdoPrint;
                List<Mps000086ADO> tt20Group = new List<Mps000086ADO>();
                List<Mps000086ADO> tt20Child = new List<Mps000086ADO>();
                List<Mps000086ADO> tt20Other = new List<Mps000086ADO>();
                BuildAllTT20Datasets(tt20Source, tt20Group, tt20Child, tt20Other);
                // Header "Thuốc khác (ngoài danh mục TT20)" — SingleKey, chỉ hiện khi có data Other
                SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.TT20_OTHER_HEADER,
                    tt20Other.Count > 0 ? "Thuốc khác (ngoài danh mục TT20)" : string.Empty));

                singleTag.ProcessData(store, singleValueDictionary);

              
                var listAdoPackagePrint = (rdo.listAdo ?? new List<Mps000086ADO>()).ToList();

                
                if (rdo.OrderKey == 5)
                {
                    listAdoPackagePrint = listAdoPackagePrint
                        .OrderBy(p => p.TYPE_ID == 1
                            ? (string.IsNullOrWhiteSpace(p.PARENT_MEDICINE_TYPE_NAME) ? 2 : 0)
                            : 1)
                        .ThenBy(p => p.TYPE_ID == 1
                            ? (p.PARENT_MEDICINE_TYPE_NAME ?? string.Empty)
                            : string.Empty)
                        .ThenBy(p => p.TYPE_ID == 1
                            ? (p.MEDICINE_TYPE_NAME ?? string.Empty)
                            : (p.MEDI_MATE_TYPE_NAME ?? string.Empty))
                        .ToList();
                    foreach (var item in listAdoPackagePrint)
                    {
                        if (string.IsNullOrWhiteSpace(item.PARENT_MEDICINE_TYPE_NAME))
                        {
                            item.PARENT_MEDICINE_TYPE_NAME = "Không phân nhóm";
                        }
                    }
                    BuildParentGroupName(listAdoPackagePrint);
                }

                objectTag.AddObjectData(store, "ListMediMate1", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMate2", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMate3", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMatePackage1", listAdoPackagePrint);
                objectTag.AddObjectData(store, "ListMediMatePackage2", listAdoPackagePrint);
                objectTag.AddObjectData(store, "ListMediMatePackage3", listAdoPackagePrint);
                objectTag.AddObjectData(store, "ListMediMateSplitedByPackage", listAdoPrintSplitedByPackage);

                var parentList = listAdoPackagePrint
    .GroupBy(p => p.PARENT_MEDICINE_TYPE_NAME)
    .Select(g => new Mps000086ADO
    {
        PARENT_MEDICINE_TYPE_NAME = g.Key
    })
    .ToList();

                objectTag.AddObjectData(store, "ListMediMateParent", parentList);
                objectTag.AddRelationship(store, "ListMediMateParent", "ListMediMatePackage1", "PARENT_MEDICINE_TYPE_NAME", "PARENT_MEDICINE_TYPE_NAME");

                // 3 datasets TT20 + 1 relationship Group → Child qua PARENT_ID = ID nhóm cha
                objectTag.AddObjectData(store, "ListMediMateTT20Group", tt20Group);
                objectTag.AddObjectData(store, "ListMediMateTT20Child", tt20Child);
                objectTag.AddObjectData(store, "ListMediMateTT20Other", tt20Other);
                objectTag.AddRelationship(store, "ListMediMateTT20Group", "ListMediMateTT20Child", "ORIGINAL_MEDI_MATE_TYPE_ID", "TT20_PARENT_ID");

                var otherPaySourceGroup1 = listAdoPrint
                        .GroupBy(o => new
                        {
                            OTHER_PAY_SOURCE_ID = o.OTHER_PAY_SOURCE_ID ?? 0,
                            OTHER_PAY_SOURCE_CODE = o.OTHER_PAY_SOURCE_CODE ?? "",
                            OTHER_PAY_SOURCE_NAME = o.OTHER_PAY_SOURCE_NAME ?? ""
                        });

                foreach (var item in otherPaySourceGroup1)
                {
                    otherPaySourceList.Add(item.First());
                }
                objectTag.AddObjectData(store, "OtherPaySourceGroup", otherPaySourceList);
                objectTag.AddRelationship(store, "OtherPaySourceGroup", "ListMediMate1", "OTHER_PAY_SOURCE_ID", "OTHER_PAY_SOURCE_ID");

                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData13", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData14", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData21", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData22", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData23", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData24", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData31", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData32", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData33", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData34", new CalculateMergerData());
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void BuildParentGroupName(List<Mps000086ADO> list)
        {
            string lastParent = null;

            foreach (var item in list)
            {
                var currentParent = item.TYPE_ID == 1
                    ? item.PARENT_MEDICINE_TYPE_NAME
                    : null;

                if (!string.IsNullOrWhiteSpace(currentParent) && currentParent != lastParent)
                {

                    lastParent = currentParent;
                }
                else
                {
                    item.PARENT_MEDICINE_TYPE_NAME = "";
                }
            }
        }

        #region TT20 sorting (TT 20/2022/TT-BYT Phụ lục I)

        // Regex: match prefix dạng "6.", "6.1.", "6.2.1.", "160." theo sau là khoảng trắng
        // Nhóm 1 capture phần số: "6", "6.1", "6.2.1", "160"
        private static readonly Regex Tt20PrefixRegex =
            new Regex(@"^\s*(\d+(?:\.\d+)*)\s*\.\s*", RegexOptions.Compiled);

        /// <summary>
        /// Build 3 datasets TT20 từ listAdoPrint (chỉ chứa thuốc lá):
        /// - TT20Group: gộp PHẲNG tất cả nhóm cha có STT (cấp 1, 2, 3, ...) — sort tuple (PART1, PART2, PART3) tăng dần
        /// - TT20Child: thuốc lá có ít nhất 1 cha có STT — sort A-Z theo MEDICINE_TYPE_NAME (trong cùng cha qua relationship)
        /// - TT20Other: thuốc lá KHÔNG có cha nào có STT + vật tư/máu — sort A-Z
        /// Relationship duy nhất: TT20Group.ORIGINAL_MEDI_MATE_TYPE_ID = TT20Child.TT20_PARENT_ID
        /// </summary>
        private void BuildAllTT20Datasets(
            List<Mps000086ADO> listAdoPrint,
            List<Mps000086ADO> groupOut,
            List<Mps000086ADO> childOut,
            List<Mps000086ADO> otherOut)
        {
            try
            {
                if (listAdoPrint == null || listAdoPrint.Count == 0) return;

                var medTypeById = BuildMedicineTypeByIdDict();
                var medTypeByCode = BuildMedicineTypeByCodeDict();

                // Dedup: mỗi group có STT chỉ thêm 1 lần dù xuất hiện trong nhiều cây
                var groupSeen = new HashSet<long>();

                foreach (var leaf in listAdoPrint)
                {
                    if (leaf.TYPE_ID != 1)
                    {
                        otherOut.Add(leaf);
                        continue;
                    }

                    long? originalId = ResolveOriginalMedicineTypeId(leaf, medTypeByCode);
                    leaf.ORIGINAL_MEDI_MATE_TYPE_ID = originalId;

                    if (originalId == null)
                    {
                        otherOut.Add(leaf);
                        continue;
                    }

                    var chain = BuildAncestorChain(originalId, medTypeById);

                    // Kiểm tra CHÍNH RECORD này có STT prefix trên tên không (chain[0] = self)
                    long[] ownParts = (chain.Count > 0)
                        ? ParseTt20Prefix(chain[0].MEDICINE_TYPE_NAME)
                        : null;
                    bool ownHasStt = ownParts != null;

                    // Gom record vào Group nếu chính nó có STT prefix
                    if (ownHasStt && !groupSeen.Contains(chain[0].ID))
                    {
                        groupSeen.Add(chain[0].ID);
                        groupOut.Add(BuildGroupAdo(chain[0], ownParts));
                    }

                    // Gom TẤT CẢ ancestor có STT prefix vào Group (mọi cấp)
                    bool hasAnySttAncestor = false;
                    for (int i = 1; i < chain.Count; i++)
                    {
                        var ancestor = chain[i];
                        var parts = ParseTt20Prefix(ancestor.MEDICINE_TYPE_NAME);
                        if (parts == null) continue;

                        hasAnySttAncestor = true;
                        if (!groupSeen.Contains(ancestor.ID))
                        {
                            groupSeen.Add(ancestor.ID);
                            groupOut.Add(BuildGroupAdo(ancestor, parts));
                        }
                    }

                    // Phân loại record:
                    // - Có STT prefix → đã vào Group, KHÔNG vào Child
                    // - Không STT prefix nhưng có ancestor có STT → Child
                    // - Không STT, không ancestor có STT → Other
                    if (ownHasStt)
                    {
                        continue; // đã thêm vào Group
                    }

                    if (!hasAnySttAncestor)
                    {
                        otherOut.Add(leaf);
                        continue;
                    }

                    // Child link với cha trực tiếp trong cây — relationship: Group.ID = Child.PARENT_ID
                    leaf.TT20_PARENT_ID = chain.Count >= 2 ? (long?)chain[1].ID : null;
                    childOut.Add(leaf);
                }

                // Sort Group theo tuple (PART1, PART2, PART3) tăng dần
                groupOut.Sort((a, b) =>
                {
                    int c = a.TT20_SORT_PART1.CompareTo(b.TT20_SORT_PART1);
                    if (c != 0) return c;
                    c = a.TT20_SORT_PART2.CompareTo(b.TT20_SORT_PART2);
                    if (c != 0) return c;
                    return a.TT20_SORT_PART3.CompareTo(b.TT20_SORT_PART3);
                });

                // Sort Child A-Z theo MEDICINE_TYPE_NAME (FlexCel filter theo relationship sẽ giữ thứ tự A-Z trong cùng cha)
                childOut.Sort((a, b) => string.Compare(
                    a.MEDICINE_TYPE_NAME ?? a.MEDI_MATE_TYPE_NAME ?? string.Empty,
                    b.MEDICINE_TYPE_NAME ?? b.MEDI_MATE_TYPE_NAME ?? string.Empty,
                    StringComparison.OrdinalIgnoreCase));

                // Sort Other A-Z theo MEDICINE_TYPE_NAME
                otherOut.Sort((a, b) => string.Compare(
                    a.MEDICINE_TYPE_NAME ?? a.MEDI_MATE_TYPE_NAME ?? string.Empty,
                    b.MEDICINE_TYPE_NAME ?? b.MEDI_MATE_TYPE_NAME ?? string.Empty,
                    StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Build ADO record cho 1 Group entry (nhóm có STT) — copy thông tin từ V_HIS_MEDICINE_TYPE.</summary>
        private Mps000086ADO BuildGroupAdo(V_HIS_MEDICINE_TYPE mt, long[] parts)
        {
            return new Mps000086ADO
            {
                TYPE_ID = 1,
                ORIGINAL_MEDI_MATE_TYPE_ID = mt.ID,
                MEDI_MATE_TYPE_CODE = mt.MEDICINE_TYPE_CODE,
                MEDI_MATE_TYPE_NAME = mt.MEDICINE_TYPE_NAME,
                MEDICINE_TYPE_NAME = mt.MEDICINE_TYPE_NAME,
                TT20_PARENT_ID = mt.PARENT_ID,
                TT20_SORT_PART1 = parts != null && parts.Length > 0 ? parts[0] : 0,
                TT20_SORT_PART2 = parts != null && parts.Length > 1 ? parts[1] : 0,
                TT20_SORT_PART3 = parts != null && parts.Length > 2 ? parts[2] : 0,
            };
        }

        private Dictionary<long, V_HIS_MEDICINE_TYPE> BuildMedicineTypeByIdDict()
        {
            var dict = new Dictionary<long, V_HIS_MEDICINE_TYPE>();
            if (rdo._MedicineTypes == null) return dict;
            foreach (var mt in rdo._MedicineTypes)
            {
                if (!dict.ContainsKey(mt.ID)) dict[mt.ID] = mt;
            }
            return dict;
        }

        private Dictionary<string, V_HIS_MEDICINE_TYPE> BuildMedicineTypeByCodeDict()
        {
            var dict = new Dictionary<string, V_HIS_MEDICINE_TYPE>();
            if (rdo._MedicineTypes == null) return dict;
            foreach (var mt in rdo._MedicineTypes)
            {
                if (!string.IsNullOrEmpty(mt.MEDICINE_TYPE_CODE) && !dict.ContainsKey(mt.MEDICINE_TYPE_CODE))
                {
                    dict[mt.MEDICINE_TYPE_CODE] = mt;
                }
            }
            return dict;
        }

        private long? ResolveOriginalMedicineTypeId(Mps000086ADO ado, Dictionary<string, V_HIS_MEDICINE_TYPE> medTypeByCode)
        {
            if (ado.ORIGINAL_MEDI_MATE_TYPE_ID.HasValue) return ado.ORIGINAL_MEDI_MATE_TYPE_ID;
            V_HIS_MEDICINE_TYPE mt;
            if (!string.IsNullOrEmpty(ado.MEDI_MATE_TYPE_CODE)
                && medTypeByCode.TryGetValue(ado.MEDI_MATE_TYPE_CODE, out mt))
            {
                return mt.ID;
            }
            return null;
        }

        private List<V_HIS_MEDICINE_TYPE> BuildAncestorChain(long? selfId, Dictionary<long, V_HIS_MEDICINE_TYPE> medTypeById)
        {
            // chain[0] = node chính nó (leaf), chain[last] = root
            var chain = new List<V_HIS_MEDICINE_TYPE>();
            long? currentId = selfId;
            int safety = 0;
            while (currentId.HasValue && safety < 10)
            {
                V_HIS_MEDICINE_TYPE mt;
                if (!medTypeById.TryGetValue(currentId.Value, out mt)) break;
                chain.Add(mt);
                currentId = mt.PARENT_ID;
                safety++;
            }
            return chain;
        }

        /// <summary>Parse STT prefix như "6.", "6.1.", "6.2.1.", "160." → trả về mảng số [6], [6,1], [6,2,1], [160]. Null nếu không khớp.</summary>
        private long[] ParseTt20Prefix(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var match = Tt20PrefixRegex.Match(name);
            if (!match.Success) return null;
            var raw = match.Groups[1].Value;
            var tokens = raw.Split('.');
            var result = new long[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                long v;
                if (!long.TryParse(tokens[i], out v)) return null;
                result[i] = v;
            }
            return result;
        }

        #endregion
        void ProcessSingleKey()
        {
            try
            {
                decimal totalPrice = 0;
                decimal totalPriceNoVat = 0;
                if (this.rdo._ChmsExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey(this.rdo._ChmsExpMest, false);
                    if (rdo._MediStocks != null && rdo._MediStocks.Count > 0)
                    {
                        var dataImpMedi = rdo._MediStocks.FirstOrDefault(p => p.ID == this.rdo._ChmsExpMest.IMP_MEDI_STOCK_ID);
                        if (dataImpMedi != null)
                        {
                            SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.IMP_MEDI_STOCK_CODE, dataImpMedi.MEDI_STOCK_CODE));
                            SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.IMP_MEDI_STOCK_NAME, dataImpMedi.MEDI_STOCK_NAME));
                        }
                        var data = rdo._MediStocks.FirstOrDefault(p => p.ID == this.rdo._ChmsExpMest.MEDI_STOCK_ID);
                        if (data != null)
                        {
                            SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.EXP_IS_CABINET, data.IS_CABINET));
                        }
                    }
                }

                #region Thuoc
                if (this.rdo._ExpMestMetyReqs != null && this.rdo._ExpMestMetyReqs.Count > 0)
                {
                    if (this.rdo._ExpMestMedicines != null && this.rdo._ExpMestMedicines.Count > 0)
                    {
                        var dataMediByMetyReqs = this.rdo._ExpMestMedicines.Where(p => this.rdo._ExpMestMetyReqs.Select(o => o.MEDICINE_TYPE_ID).ToList().Contains(p.MEDICINE_TYPE_ID)).ToList();
                        if (dataMediByMetyReqs != null && dataMediByMetyReqs.Count > 0)
                        {
                            var Group = dataMediByMetyReqs.GroupBy(g => new
                            {
                                g.MEDICINE_ID,
                                g.PACKAGE_NUMBER,
                                g.SUPPLIER_ID,
                                g.IMP_PRICE,
                                g.IMP_VAT_RATIO
                            }).ToList();

                            foreach (var mediGr in Group)
                            {
                                Mps000086ADO adoMediGr = new Mps000086ADO();
                                adoMediGr.TYPE_ID = 1;
                                adoMediGr.TOTAL_AMOUNT_IN_REQUEST = this.rdo._ExpMestMetyReqs.Where(p => p.MEDICINE_TYPE_ID
                                     == mediGr.First().MEDICINE_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                                adoMediGr.TOTAL_AMOUNT = adoMediGr.TOTAL_AMOUNT_IN_REQUEST;
                                adoMediGr.MEDI_MATE_TYPE_CODE = mediGr.First().MEDICINE_TYPE_CODE;
                                adoMediGr.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(mediGr.First().MEDICINE_TYPE_ID.ToString() + adoMediGr.TYPE_ID.ToString());
                                adoMediGr.MEDI_MATE_TYPE_NAME = mediGr.First().MEDICINE_TYPE_NAME;
                                adoMediGr.REGISTER_NUMBER = mediGr.First().REGISTER_NUMBER;
                                adoMediGr.SERVICE_UNIT_CODE = mediGr.First().SERVICE_UNIT_CODE;
                                adoMediGr.SERVICE_UNIT_NAME = mediGr.First().SERVICE_UNIT_NAME;
                                adoMediGr.TOTAL_AMOUNT_IN_EXECUTE = mediGr.Sum(p => p.AMOUNT);
                                adoMediGr.PACKAGE_NUMBER = mediGr.First().PACKAGE_NUMBER;
                                adoMediGr.SUPPLIER_CODE = mediGr.First().SUPPLIER_CODE;
                                adoMediGr.SUPPLIER_NAME = mediGr.First().SUPPLIER_NAME;
                                //adoMediGr.OTHER_PAY_SOURCE_ID = mediGr.First().OTHER_PAY_SOURCE_ID;
                                //adoMediGr.OTHER_PAY_SOURCE_CODE = mediGr.First().OTHER_PAY_SOURCE_CODE;
                                //adoMediGr.OTHER_PAY_SOURCE_NAME = mediGr.First().OTHER_PAY_SOURCE_NAME;

                                adoMediGr.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(mediGr.First().EXPIRED_DATE ?? 0);
                                adoMediGr.PRICE = mediGr.First().PRICE;
                                adoMediGr.IMP_PRICE = mediGr.First().IMP_PRICE;
                                adoMediGr.IMP_VAT_RATIO = mediGr.First().IMP_VAT_RATIO;
                                adoMediGr.IMP_PRICE_RATIO = mediGr.First().IMP_PRICE + mediGr.First().IMP_PRICE * mediGr.First().IMP_VAT_RATIO;
                                adoMediGr.DESCRIPTION = mediGr.First().DESCRIPTION;
                                adoMediGr.MEDI_MATE_NUM_ORDER = mediGr.First().MEDICINE_NUM_ORDER;
                                adoMediGr.NUM_ORDER = mediGr.First().NUM_ORDER;
                                adoMediGr.MEDICINE_TYPE_NAME = mediGr.First().MEDICINE_TYPE_NAME;
                                adoMediGr.MEDICINE_USE_FORM_NUM_ORDER = mediGr.First().MEDICINE_USE_FORM_NUM_ORDER;
                                adoMediGr.ACTIVE_INGR_BHYT_CODE = mediGr.First().ACTIVE_INGR_BHYT_CODE;
                                adoMediGr.ACTIVE_INGR_BHYT_NAME = mediGr.First().ACTIVE_INGR_BHYT_NAME;
                                adoMediGr.VIR_PRICE = mediGr.Sum(p => p.VIR_PRICE);
                                adoMediGr.BID_NUMBER = string.Join(",", mediGr.Select(p => p.BID_NUMBER).Distinct().ToList());

                                var _dataMedi = rdo._MedicineTypes.FirstOrDefault(p => p.ID == mediGr.First().MEDICINE_TYPE_ID);
                                if (_dataMedi != null)
                                {
                                    adoMediGr.NATIONAL_NAME = _dataMedi.NATIONAL_NAME;
                                    adoMediGr.PACKING_TYPE_NAME = _dataMedi.PACKING_TYPE_NAME;
                                    adoMediGr.MEDICINE_USE_FORM_NAME = _dataMedi.MEDICINE_USE_FORM_NAME;
                                    adoMediGr.CONCENTRA = _dataMedi.CONCENTRA;
                                    adoMediGr.MANUFACTURER_CODE = _dataMedi.MEDICINE_GROUP_CODE;
                                    adoMediGr.MANUFACTURER_NAME = _dataMedi.MANUFACTURER_NAME;
                                    adoMediGr.OTHER_PAY_SOURCE_NAME = _dataMedi.OTHER_PAY_SOURCE_NAME;
                                    adoMediGr.OTHER_PAY_SOURCE_ID = _dataMedi.OTHER_PAY_SOURCE_ID;
                                    adoMediGr.OTHER_PAY_SOURCE_CODE = _dataMedi.OTHER_PAY_SOURCE_CODE;


                                    var _parentMedicineType = rdo._MedicineTypes.FirstOrDefault(p => p.ID == _dataMedi.PARENT_ID);
                                    if (_parentMedicineType != null)
                                    {
                                        adoMediGr.PARENT_MEDICINE_TYPE_NAME = _parentMedicineType.MEDICINE_TYPE_NAME;
                                    }

                                }

                                var medicines = rdo._Medicines.Where(p => mediGr.Select(x => x.MEDICINE_ID).ToList().Contains(p.ID)).ToList();
                                if (medicines != null)
                                {
                                    //bo sung gia 
                                    var medi = medicines.FirstOrDefault();
                                    if (medi.IS_SALE_EQUAL_IMP_PRICE != 1)
                                    {
                                        if (rdo.listConfig == null)
                                        {
                                            LogSystem.Debug("Danh sach cau hinh null.(HIS_PATIENT_TYPE.PATIENT_TYPE_CODE)");
                                        }
                                        else if (rdo.mediPaty == null)
                                        {
                                            LogSystem.Debug("Danh sach V_HIS_MEDICINE_PATY null");
                                        }
                                        else
                                        {
                                            LogSystem.Debug("Danh sach cau hinh " + rdo.listConfig.Count + " danh sach medicine_paty: " + rdo.mediPaty.Count);

                                            var config = rdo.listConfig.FirstOrDefault(s => s.KEY == "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE");
                                            if (config != null)
                                            {
                                                var paty = rdo.mediPaty.Where(s => s.MEDICINE_ID == medi.ID && config.VALUE == s.PATIENT_TYPE_CODE).OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                                                if (paty != null)
                                                {
                                                    adoMediGr.EXP_PRICE_VP = paty.EXP_PRICE;
                                                    adoMediGr.EXP_VAT_RATIO_VP = paty.EXP_VAT_RATIO;
                                                }
                                            }
                                        }


                                    }
                                    else
                                    {
                                        adoMediGr.EXP_PRICE_VP = medi.IMP_PRICE;
                                        adoMediGr.EXP_VAT_RATIO_VP = medi.IMP_VAT_RATIO;
                                    }
                                }
                                adoMediGr.VIR_IMP_PRICE = medicines.Sum(p => p.VIR_IMP_PRICE);
                                adoMediGr.BID_PACKAGE_CODE = string.Join(",", medicines.Select(p => p.TDL_BID_PACKAGE_CODE).Distinct().ToList());
                                adoMediGr.BID_GROUP_CODE = string.Join(",", medicines.Select(p => p.TDL_BID_GROUP_CODE).Distinct().ToList());
                                adoMediGr.BID_YEAR = string.Join(",", medicines.Select(p => p.TDL_BID_YEAR).Distinct().ToList());

                                if (rdo._ChmsExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                                {
                                    adoMediGr.TOTAL_AMOUNT_IN_EXPORT = adoMediGr.TOTAL_AMOUNT_IN_EXECUTE;
                                }

                                adoMediGr.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_REQUEST)));
                                adoMediGr.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_EXECUTE)));
                                adoMediGr.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_EXPORT)));

                                rdo.listAdo.Add(adoMediGr);

                                var listByGroup = mediGr.ToList<V_HIS_EXP_MEST_MEDICINE>();
                                foreach (var item in listByGroup)
                                {
                                    totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                                    totalPriceNoVat += (item.AMOUNT * (item.IMP_PRICE)) - (item.DISCOUNT ?? 0);
                                }

                            }
                        }
                    }
                    else
                    {
                        var Groups = this.rdo._ExpMestMetyReqs.GroupBy(g => new
                        {
                            g.MEDICINE_TYPE_ID
                        }).Select(p => p.ToList()).ToList();

                        foreach (var itemGr in Groups)
                        {
                            Mps000086ADO ado = new Mps000086ADO();
                            ado.TYPE_ID = 1;
                            ado.DESCRIPTION = itemGr[0].DESCRIPTION;
                            var data = rdo._MedicineTypes.FirstOrDefault(p => p.ID == itemGr[0].MEDICINE_TYPE_ID);
                            if (data != null)
                            {
                                ado.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(itemGr.First().MEDICINE_TYPE_ID.ToString() + ado.TYPE_ID.ToString());
                                ado.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                                ado.REGISTER_NUMBER = data.REGISTER_NUMBER;
                                ado.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                                ado.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                                ado.MEDI_MATE_NUM_ORDER = data.MEDICINE_NUM_ORDER;
                                ado.NATIONAL_NAME = data.NATIONAL_NAME;
                                ado.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                                ado.MEDICINE_USE_FORM_NAME = data.MEDICINE_USE_FORM_NAME;
                                ado.MEDICINE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                                ado.CONCENTRA = data.CONCENTRA;
                                ado.ACTIVE_INGR_BHYT_CODE = data.ACTIVE_INGR_BHYT_CODE;
                                ado.ACTIVE_INGR_BHYT_NAME = data.ACTIVE_INGR_BHYT_NAME;
                                ado.MANUFACTURER_CODE = data.MANUFACTURER_CODE;
                                ado.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                                ado.OTHER_PAY_SOURCE_ID = data.OTHER_PAY_SOURCE_ID;
                                ado.OTHER_PAY_SOURCE_CODE = data.OTHER_PAY_SOURCE_CODE;
                                ado.OTHER_PAY_SOURCE_NAME = data.OTHER_PAY_SOURCE_NAME;
                                if (rdo._MedicineUserForms != null && rdo._MedicineUserForms.Count > 0)
                                {
                                    ado.MEDICINE_USE_FORM_NUM_ORDER = rdo._MedicineUserForms.Where(o => o.ID == data.MEDICINE_USE_FORM_ID).FirstOrDefault()!= null ? rdo._MedicineUserForms.Where(o => o.ID == data.MEDICINE_USE_FORM_ID).FirstOrDefault().NUM_ORDER : 0;
                                }

                            }
                            if (rdo._Medicines != null)
                            {
                                var medi = rdo._Medicines.Where(s => itemGr.Select(p => p.MEDICINE_TYPE_ID).ToList().Contains(s.MEDICINE_TYPE_ID));

                            }
                            ado.TOTAL_AMOUNT_IN_REQUEST = itemGr.Sum(o => o.AMOUNT);
                            ado.TOTAL_AMOUNT = ado.TOTAL_AMOUNT_IN_REQUEST;
                            ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_REQUEST)));

                            rdo.listAdo.Add(ado);
                        }
                    }
                }
                #endregion

                #region VatTu
                if (this.rdo._ExpMestMatyReqs != null && this.rdo._ExpMestMatyReqs.Count > 0)
                {
                    if (this.rdo._ExpMestMaterials != null && this.rdo._ExpMestMaterials.Count > 0)
                    {
                        var dataMateByMatyReqs = this.rdo._ExpMestMaterials.Where(p => this.rdo._ExpMestMatyReqs.Select(o => o.MATERIAL_TYPE_ID).ToList().Contains(p.MATERIAL_TYPE_ID)).ToList();
                        if (dataMateByMatyReqs != null && dataMateByMatyReqs.Count > 0)
                        {
                            var Group = dataMateByMatyReqs.GroupBy(g => new
                            {
                                g.MATERIAL_TYPE_ID,
                                g.PACKAGE_NUMBER,
                                g.SUPPLIER_ID,
                                g.IMP_PRICE,
                                g.IMP_VAT_RATIO,
                                g.PRICE
                            }).ToList();

                            foreach (var mediGr in Group)
                            {
                                Mps000086ADO adoMediGr = new Mps000086ADO();
                                adoMediGr.TYPE_ID = 2;
                                adoMediGr.TOTAL_AMOUNT_IN_REQUEST = this.rdo._ExpMestMatyReqs.Where(p => p.MATERIAL_TYPE_ID
                                     == mediGr.First().MATERIAL_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                                adoMediGr.TOTAL_AMOUNT = adoMediGr.TOTAL_AMOUNT_IN_REQUEST;
                                adoMediGr.MEDI_MATE_TYPE_CODE = mediGr.First().MATERIAL_TYPE_CODE;
                                adoMediGr.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(mediGr.First().MATERIAL_TYPE_ID.ToString() + adoMediGr.TYPE_ID.ToString());
                                adoMediGr.MEDI_MATE_TYPE_NAME = mediGr.First().MATERIAL_TYPE_NAME;
                                adoMediGr.SERVICE_UNIT_CODE = mediGr.First().SERVICE_UNIT_CODE;
                                adoMediGr.SERVICE_UNIT_NAME = mediGr.First().SERVICE_UNIT_NAME;
                                adoMediGr.TOTAL_AMOUNT_IN_EXECUTE = mediGr.Sum(p => p.AMOUNT);
                                adoMediGr.PACKAGE_NUMBER = mediGr.First().PACKAGE_NUMBER;
                                adoMediGr.SUPPLIER_CODE = mediGr.First().SUPPLIER_CODE;
                                adoMediGr.SUPPLIER_NAME = mediGr.First().SUPPLIER_NAME;
                                adoMediGr.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(mediGr.First().EXPIRED_DATE ?? 0);
                                adoMediGr.PRICE = mediGr.First().PRICE;
                                adoMediGr.IMP_PRICE = mediGr.First().IMP_PRICE;
                                adoMediGr.MEDICINE_TYPE_NAME = mediGr.First().MATERIAL_TYPE_NAME;
                                //adoMediGr.OTHER_PAY_SOURCE_ID = mediGr.First().OTHER_PAY_SOURCE_ID;
                                //adoMediGr.OTHER_PAY_SOURCE_CODE = mediGr.First().OTHER_PAY_SOURCE_CODE;
                                //adoMediGr.OTHER_PAY_SOURCE_NAME = mediGr.First().OTHER_PAY_SOURCE_NAME;
                                adoMediGr.IMP_VAT_RATIO = mediGr.First().IMP_VAT_RATIO;
                                adoMediGr.IMP_PRICE_RATIO = mediGr.First().IMP_PRICE + mediGr.First().IMP_PRICE * mediGr.First().IMP_VAT_RATIO;
                                adoMediGr.DESCRIPTION = mediGr.First().DESCRIPTION;
                                adoMediGr.MEDI_MATE_NUM_ORDER = mediGr.First().MEDICINE_NUM_ORDER ?? 0;
                                adoMediGr.NUM_ORDER = mediGr.First().NUM_ORDER;
                                adoMediGr.VIR_PRICE = mediGr.Sum(p => p.VIR_PRICE);
                                adoMediGr.BID_NUMBER = string.Join(",", mediGr.Select(p => p.BID_NUMBER).Distinct().ToList());

                                var _dataMate = rdo._MaterialTypes.FirstOrDefault(p => p.ID == mediGr.First().MATERIAL_TYPE_ID);
                                if (_dataMate != null)
                                {
                                    adoMediGr.NATIONAL_NAME = _dataMate.NATIONAL_NAME;
                                    adoMediGr.PACKING_TYPE_NAME = _dataMate.PACKING_TYPE_NAME;
                                    adoMediGr.CONCENTRA = _dataMate.CONCENTRA;
                                    adoMediGr.MANUFACTURER_CODE = _dataMate.MANUFACTURER_CODE;
                                    adoMediGr.MANUFACTURER_NAME = _dataMate.MANUFACTURER_NAME;
                                    adoMediGr.REGISTER_NUMBER = _dataMate.REGISTER_NUMBER;
                                    adoMediGr.OTHER_PAY_SOURCE_ID = _dataMate.OTHER_PAY_SOURCE_ID;
                                    adoMediGr.OTHER_PAY_SOURCE_CODE = _dataMate.OTHER_PAY_SOURCE_CODE;
                                    adoMediGr.OTHER_PAY_SOURCE_NAME = _dataMate.OTHER_PAY_SOURCE_NAME;

                                }

                                var materials = rdo._Materials.Where(p => mediGr.Select(x => x.MATERIAL_ID).ToList().Contains(p.ID)).ToList();
                                if (materials != null)
                                {
                                    var mate = materials.FirstOrDefault();
                                    //bo sung gia 
                                    if (mate.IS_SALE_EQUAL_IMP_PRICE != 1)
                                    {
                                        if (rdo.listConfig == null)
                                        {
                                            LogSystem.Debug("Danh sach cau hinh null.(HIS_PATIENT_TYPE.PATIENT_TYPE_CODE)");
                                        }
                                        else if (rdo.matePaty == null)
                                        {
                                            LogSystem.Debug("Danh sach V_HIS_MATERIAL_PATY null");
                                        }
                                        else
                                        {
                                            LogSystem.Debug("Danh sach cau hinh " + rdo.listConfig.Count + " danh sach material_paty " + rdo.matePaty.Count);
                                            var config = rdo.listConfig.FirstOrDefault(s => s.KEY == "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE");
                                            if (config != null)
                                            {
                                                var paty = rdo.matePaty.Where(s => s.MATERIAL_ID == mate.ID && config.VALUE == s.PATIENT_TYPE_CODE).OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                                                if (paty != null)
                                                {
                                                    adoMediGr.EXP_PRICE_VP = paty.EXP_PRICE;
                                                    adoMediGr.EXP_VAT_RATIO_VP = paty.EXP_VAT_RATIO;
                                                }
                                            }
                                        }


                                    }
                                    else
                                    {
                                        adoMediGr.EXP_PRICE_VP = mate.IMP_PRICE;
                                        adoMediGr.EXP_VAT_RATIO_VP = mate.IMP_VAT_RATIO;
                                    }
                                }
                                adoMediGr.VIR_IMP_PRICE = materials.Sum(p => p.VIR_IMP_PRICE);
                                adoMediGr.BID_PACKAGE_CODE = string.Join(",", materials.Select(p => p.TDL_BID_PACKAGE_CODE).Distinct().ToList());
                                adoMediGr.BID_GROUP_CODE = string.Join(",", materials.Select(p => p.TDL_BID_GROUP_CODE).Distinct().ToList());
                                adoMediGr.BID_YEAR = string.Join(",", materials.Select(p => p.TDL_BID_YEAR).Distinct().ToList());

                                if (rdo._ChmsExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                                {
                                    adoMediGr.TOTAL_AMOUNT_IN_EXPORT = adoMediGr.TOTAL_AMOUNT_IN_EXECUTE;
                                }

                                adoMediGr.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_REQUEST)));
                                adoMediGr.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_EXECUTE)));
                                adoMediGr.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_EXPORT)));

                                rdo.listAdo.Add(adoMediGr);

                                var listByGroup = mediGr.ToList<V_HIS_EXP_MEST_MATERIAL>();
                                foreach (var item in listByGroup)
                                {
                                    totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                                    totalPriceNoVat += (item.AMOUNT * (item.IMP_PRICE)) - (item.DISCOUNT ?? 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = this.rdo._ExpMestMatyReqs.GroupBy(g => new
                        {
                            g.MATERIAL_TYPE_ID
                        }).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in Groups)
                        {
                            Mps000086ADO ado = new Mps000086ADO();
                            ado.TYPE_ID = 2;
                            ado.DESCRIPTION = itemGr[0].DESCRIPTION;
                            var data = rdo._MaterialTypes.FirstOrDefault(p => p.ID == itemGr[0].MATERIAL_TYPE_ID);
                            if (data != null)
                            {
                                ado.OTHER_PAY_SOURCE_ID = data.OTHER_PAY_SOURCE_ID;
                                ado.OTHER_PAY_SOURCE_CODE = data.OTHER_PAY_SOURCE_CODE;
                                ado.OTHER_PAY_SOURCE_NAME = data.OTHER_PAY_SOURCE_NAME;
                                ado.MEDI_MATE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(itemGr.First().MATERIAL_TYPE_ID.ToString() + ado.TYPE_ID.ToString());
                                ado.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                                ado.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                                ado.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                                ado.MANUFACTURER_CODE = data.MANUFACTURER_CODE;
                                ado.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                                ado.MEDICINE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                                ado.NATIONAL_NAME = data.NATIONAL_NAME;
                                ado.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                                ado.CONCENTRA = data.CONCENTRA;
                                ado.REGISTER_NUMBER = data.REGISTER_NUMBER;

                            }

                            ado.TOTAL_AMOUNT_IN_REQUEST = itemGr.Sum(o => o.AMOUNT);
                            ado.TOTAL_AMOUNT = ado.TOTAL_AMOUNT_IN_REQUEST;
                            ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_REQUEST)));

                            rdo.listAdo.Add(ado);
                        }
                    }
                }
                #endregion

                #region Mau
                if (this.rdo._ExpMestBltyReqs != null && this.rdo._ExpMestBltyReqs.Count > 0)
                {
                    if (this.rdo._ExpMestBloods != null && this.rdo._ExpMestBloods.Count > 0)
                    {
                        var dataBloodByBltyReqs = this.rdo._ExpMestBloods.Where(p => this.rdo._ExpMestBltyReqs.Select(o => o.BLOOD_TYPE_ID).ToList().Contains(p.BLOOD_TYPE_ID)).ToList();
                        if (dataBloodByBltyReqs != null && dataBloodByBltyReqs.Count > 0)
                        {
                            var Group = dataBloodByBltyReqs.GroupBy(g => new
                            {
                                g.BLOOD_TYPE_ID,
                                g.PACKAGE_NUMBER,
                                g.SUPPLIER_ID,
                                g.IMP_PRICE,
                                g.IMP_VAT_RATIO,
                                g.PRICE
                            }).ToList();

                            foreach (var bloodGr in Group)
                            {
                                Mps000086ADO adoBloodGr = new Mps000086ADO();
                                adoBloodGr.TYPE_ID = 3;

                                adoBloodGr.DESCRIPTION = bloodGr.First().DESCRIPTION;
                                adoBloodGr.BLOOD_ADO_CODE = bloodGr.First().BLOOD_ABO_CODE;
                                adoBloodGr.BLOOD_RH_CODE = bloodGr.First().BLOOD_RH_CODE;
                                adoBloodGr.MEDI_MATE_TYPE_CODE = bloodGr.First().BLOOD_TYPE_CODE;
                                adoBloodGr.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(bloodGr.First().BLOOD_TYPE_ID.ToString() + adoBloodGr.TYPE_ID.ToString());
                                adoBloodGr.MEDI_MATE_TYPE_NAME = bloodGr.First().BLOOD_TYPE_NAME;
                                adoBloodGr.SERVICE_UNIT_CODE = bloodGr.First().SERVICE_UNIT_CODE;
                                adoBloodGr.SERVICE_UNIT_NAME = bloodGr.First().SERVICE_UNIT_NAME;
                                adoBloodGr.VOLUME = bloodGr.First().VOLUME;
                                adoBloodGr.TOTAL_AMOUNT_IN_EXECUTE = bloodGr.Count();
                                adoBloodGr.PACKAGE_NUMBER = bloodGr.First().PACKAGE_NUMBER;
                                adoBloodGr.SUPPLIER_CODE = bloodGr.First().SUPPLIER_CODE;
                                adoBloodGr.SUPPLIER_NAME = bloodGr.First().SUPPLIER_NAME;
                                adoBloodGr.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(bloodGr.First().EXPIRED_DATE ?? 0);
                                adoBloodGr.MEDICINE_TYPE_NAME = bloodGr.First().BLOOD_TYPE_NAME;
                                adoBloodGr.PRICE = bloodGr.First().PRICE;
                                adoBloodGr.IMP_PRICE = bloodGr.First().IMP_PRICE;
                                adoBloodGr.IMP_VAT_RATIO = bloodGr.First().IMP_VAT_RATIO * 100;
                                adoBloodGr.DESCRIPTION = bloodGr.First().DESCRIPTION;
                                adoBloodGr.MEDI_MATE_NUM_ORDER = 9999;
                                adoBloodGr.NUM_ORDER = bloodGr.First().NUM_ORDER;
                                adoBloodGr.VIR_PRICE = bloodGr.Sum(p => p.VIR_PRICE);
                                adoBloodGr.BID_NUMBER = string.Join(",", bloodGr.Select(p => p.BID_NUMBER).Distinct().ToList());

                                var _dataMate = rdo._BloodTypes.FirstOrDefault(p => p.ID == bloodGr.First().BLOOD_TYPE_ID);
                                if (_dataMate != null)
                                {
                                    adoBloodGr.PACKING_TYPE_NAME = _dataMate.PACKING_TYPE_NAME;
                                }

                                var materials = rdo._Bloods.Where(p => bloodGr.Select(x => x.BLOOD_ID).ToList().Contains(p.ID)).ToList();
                                adoBloodGr.VIR_IMP_PRICE = materials.Sum(p => p.VIR_IMP_PRICE);
                                if (rdo._ChmsExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                                {
                                    adoBloodGr.TOTAL_AMOUNT_IN_EXPORT = adoBloodGr.TOTAL_AMOUNT_IN_EXECUTE;
                                    adoBloodGr.MANUFACTURER_CODE = adoBloodGr.MANUFACTURER_CODE;
                                    adoBloodGr.MANUFACTURER_NAME = adoBloodGr.MANUFACTURER_NAME;

                                }

                                adoBloodGr.TOTAL_AMOUNT_IN_REQUEST = this.rdo._ExpMestBltyReqs.Where(p => p.BLOOD_TYPE_ID
                                     == bloodGr.First().BLOOD_TYPE_ID).ToList().Sum(o => o.AMOUNT);

                                adoBloodGr.TOTAL_AMOUNT = adoBloodGr.TOTAL_AMOUNT_IN_REQUEST;
                                adoBloodGr.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoBloodGr.TOTAL_AMOUNT_IN_REQUEST)));
                                adoBloodGr.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoBloodGr.TOTAL_AMOUNT_IN_EXECUTE)));
                                adoBloodGr.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoBloodGr.TOTAL_AMOUNT_IN_EXPORT)));

                                rdo.listAdo.Add(adoBloodGr);

                                var listByGroup = bloodGr.ToList<V_HIS_EXP_MEST_BLOOD>();
                                foreach (var item in listByGroup)
                                {
                                    totalPrice += (1 * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                                    totalPriceNoVat += (1 * (item.IMP_PRICE)) - (item.DISCOUNT ?? 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = this.rdo._ExpMestBltyReqs.GroupBy(g => new
                        {
                            g.BLOOD_TYPE_ID,
                            g.IS_EXPEND
                        }).Select(p => p.ToList()).ToList();

                        rdo.listAdo.AddRange(from r in Groups
                                             select new Mps000086ADO(rdo._ChmsExpMest,
                                                 r,
                                                 rdo._BloodTypes,
                                                 rdo._ExpMestBloods,
                                                 rdo._BloodABOs,
                                                 rdo._BloodRHs,
                                                 rdo.expMesttSttId__Approval,
                                                 rdo.expMesttSttId__Export));
                    }
                }
                #endregion

                SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.IS_PLAY_CHECK, rdo._keyPhieuTra));
                SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.REQ_DEPARTMENT_NAME, rdo.Req_Department_Name));
                SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.REQ_ROOM_NAME, rdo.Req_Room_Name));
                SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.EXP_DEPARTMENT_NAME, rdo.Exp_Department_Name));
                SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.KEY_NAMES, rdo.KeyNames));
                SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.SUM_TOTAL_PRICE_NO_VAT, totalPriceNoVat));
                //string sumText = String.Format("0:0.####", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice)))));
                SetSingleKey(new KeyValue(Mps000086ExtendSingleKey.SUM_TOTAL_PRICE_NO_VAT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPriceNoVat)))));

                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (rdo != null && rdo._ChmsExpMest != null)
                {
                    log = LogDataImpMest("", rdo._ChmsExpMest.EXP_MEST_CODE, rdo._ChmsExpMest.REQ_DEPARTMENT_NAME);
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
                int countMedicine = 0;
                int countMaterial = 0;
                int countBlood = 0;
                if (rdo._ExpMestMedicines != null)
                {
                    countMedicine = rdo._ExpMestMedicines.Count;
                }

                if (rdo._ExpMestMaterials != null)
                {
                    countMaterial = rdo._ExpMestMaterials.Count;
                }

                if (rdo._ExpMestBloods != null)
                {
                    countBlood = rdo._ExpMestBloods.Count;
                }

                if (rdo != null && rdo._ChmsExpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", printTypeCode, rdo._ChmsExpMest.EXP_MEST_CODE, rdo._ChmsExpMest.MEDI_STOCK_CODE, countMedicine, countMaterial, countBlood, rdo.KeyNames);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //Phieu xuat chuyen kho -> kho xuat = kho cua phieu, kho nhap = kho dich
        public override string ProcessEmrStockData()
        {
            try
            {
                if (rdo == null || rdo._ChmsExpMest == null) return null;
                var expMest = rdo._ChmsExpMest;
                return BuildEmrStockData(
                    expMest.EXP_MEST_CODE,
                    null,
                    expMest.MEDI_STOCK_CODE,
                    GetMediStockCodeById(expMest.IMP_MEDI_STOCK_ID),
                    expMest.REQ_DEPARTMENT_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }

    class CalculateMergerData : TFlexCelUserFunction
    {
        long typeId = 0;
        long mediMateTypeId = 0;

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
            bool result = false;
            try
            {
                long servicetypeId = Convert.ToInt64(parameters[0]);
                long mediMateId = Convert.ToInt64(parameters[1]);

                if (servicetypeId > 0 && mediMateId > 0)
                {
                    if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
                    {
                        return true;
                    }
                    else
                    {
                        this.typeId = servicetypeId;
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
