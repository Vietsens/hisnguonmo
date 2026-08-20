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
using FlexCel.Report;
using Inventec.Common.Logging;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000324.PDO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System.Linq;

namespace MPS.Processor.Mps000324
{
    class Mps000324Processor : AbstractProcessor
    {
        Mps000324PDO rdo;

        public Mps000324Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000324PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.treatment == null) return;

                AddBarcode(Mps000324ExtendSingleKey.BARCODE_IN_CODE_STR, rdo.treatment.IN_CODE);
                AddBarcode(Mps000324ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, rdo.treatment.TREATMENT_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Sinh anh barcode Code128 va nap vao dicImage.
        /// Bo qua khi gia tri rong hoac key da ton tai de goi nhieu lan van an toan.
        /// </summary>
        private void AddBarcode(string key, string value)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(value)) return;
                if (dicImage == null || dicImage.ContainsKey(key)) return;

                Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(value);
                barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcode.Width = 120;
                barcode.Height = 40;
                barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcode.IncludeLabel = true;

                dicImage.Add(key, barcode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(
                    "Khong sinh duoc barcode. key=" + key
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => value), value),
                    ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                SetSingleKey();
                ProcessListSereServ();
                BuildDetailData();
                SetSingleKeyExtend();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ekipUser", rdo.ekipUsers);
                objectTag.AddObjectData(store, "SereServFollow", rdo.SereServFollows);
                objectTag.AddObjectData(store, "ServiceTypes", _ServiceTypes);

                objectTag.AddRelationship(store, "ServiceTypes", "SereServFollow", "ID", "TDL_SERVICE_TYPE_ID");

                // Dataset bo sung — mau cu khong tham chieu den nen khong bi anh huong
                if (rdo.EkipRoles != null)
                {
                    objectTag.AddObjectData(store, Mps000324ExtendSingleKey.OBJECT_TAG_EKIP_ROLES, rdo.EkipRoles);
                }

                if (rdo.Groups != null && rdo.Items != null)
                {
                    objectTag.AddObjectData(store, Mps000324ExtendSingleKey.OBJECT_TAG_GROUPS, rdo.Groups);
                    objectTag.AddObjectData(store, Mps000324ExtendSingleKey.OBJECT_TAG_ITEMS, rdo.Items);
                    objectTag.AddRelationship(store,
                        Mps000324ExtendSingleKey.OBJECT_TAG_GROUPS,
                        Mps000324ExtendSingleKey.OBJECT_TAG_ITEMS,
                        "ID", "GROUP_ID");
                }

                SetBarcodeKey();
                if (dicImage != null && dicImage.Count > 0)
                {
                    barCodeTag.ProcessData(store, dicImage);
                }

                singleTag.ProcessData(store, singleValueDictionary);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public List<HIS_SERVICE_TYPE> _ServiceTypes { get; set; }

        void SetSingleKey()
        {
            try
            {
                _ServiceTypes = new List<HIS_SERVICE_TYPE>();
                if (rdo.SereServFollows != null && rdo.SereServFollows.Count > 0)
                {
                    List<long> _serviceTpeIds = rdo.SereServFollows.Select(p => p.TDL_SERVICE_TYPE_ID).Distinct().ToList();

                    _ServiceTypes = rdo.ServiceTypes.Where(p => _serviceTpeIds.Contains(p.ID)).ToList();
                    _ServiceTypes = _ServiceTypes.OrderBy(p => p.ID).ToList();
                }
                if (rdo.ServiceReqPrint != null)
                {
                    //keyValues.Add(new KeyValue(Mps000324ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.LOCK_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000324ExtendSingleKey.START_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.START_TIME ?? 0)));
                    if (rdo.ServiceReqPrint.FINISH_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000324ExtendSingleKey.FINISH_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.FINISH_TIME ?? 0)));
                }
                else
                {
                    //keyValues.Add(new KeyValue(Mps000324ExtendSingleKey.OPEN_TIME_SEPARATE_STR, ""));
                    SetSingleKey(new KeyValue(Mps000324ExtendSingleKey.START_TIME_STR, ""));
                }

                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000324ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.treatment.IN_TIME)));
                }

                foreach (var ekipUser in rdo.ekipUsers)
                {
                    SetSingleKey(new KeyValue("USERNAME_EXECUTE_ROLE_" + ekipUser.EXECUTE_ROLE_CODE, ekipUser.USERNAME));
                    SetSingleKey(new KeyValue("LOGIN_NAME_EXECUTE_ROLE_" + ekipUser.EXECUTE_ROLE_CODE, ekipUser.LOGINNAME));
                }

                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.treatment, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_PTTT>(rdo.sereServsPttt, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_5>(rdo.sereServ, false);
                AddObjectKeyIntoListkey<HIS_SERE_SERV_EXT>(rdo.SereServExt, false);
                AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo.departmentTran, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReqPrint, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.Patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListSereServ()
        {
            try
            {
                if (rdo.SereServFollows == null || rdo.SereServFollows.Count == 0) return;

                Dictionary<long, HIS_SERVICE_UNIT> unitDic = BuildServiceUnitDictionary();
                foreach (var item in rdo.SereServFollows)
                {
                    HIS_SERVICE_UNIT unit = null;
                    unitDic.TryGetValue(item.TDL_SERVICE_UNIT_ID, out unit);
                    item.SERVICE_UNIT_NAME = unit != null ? unit.SERVICE_UNIT_NAME : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dung tu dien don vi tinh mot lan de tra cuu O(1) thay vi FirstOrDefault trong vong lap.
        /// </summary>
        private Dictionary<long, HIS_SERVICE_UNIT> BuildServiceUnitDictionary()
        {
            Dictionary<long, HIS_SERVICE_UNIT> result = new Dictionary<long, HIS_SERVICE_UNIT>();
            try
            {
                if (rdo.ServiceUnit == null) return result;

                foreach (var unit in rdo.ServiceUnit)
                {
                    if (unit != null && !result.ContainsKey(unit.ID)) result.Add(unit.ID, unit);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Gom SereServFollows thanh nhom theo loai dich vu, tinh san STT, thanh tien,
        /// tong tien tung nhom va tong toan phieu.
        /// Chi ghi vao rdo.Groups / rdo.Items — khong dong den SereServFollows nen mau cu giu nguyen.
        /// </summary>
        private void BuildDetailData()
        {
            try
            {
                rdo.Groups = new List<Mps000324GroupADO>();
                rdo.Items = new List<Mps000324ItemADO>();

                if (rdo.SereServFollows == null || rdo.SereServFollows.Count == 0)
                {
                    SetSingleKey(Mps000324ExtendSingleKey.GRAND_TOTAL_AMOUNT, (decimal)0);
                    return;
                }

                Dictionary<long, HIS_SERVICE_UNIT> unitDic = BuildServiceUnitDictionary();
                Dictionary<long, HIS_SERVICE_TYPE> typeDic = new Dictionary<long, HIS_SERVICE_TYPE>();
                if (rdo.ServiceTypes != null)
                {
                    foreach (var type in rdo.ServiceTypes)
                    {
                        if (type != null && !typeDic.ContainsKey(type.ID)) typeDic.Add(type.ID, type);
                    }
                }

                int numOrder = 0;
                int groupNumOrder = 0;
                decimal grandTotal = 0;

                var groupedList = rdo.SereServFollows
                    .GroupBy(o => o.TDL_SERVICE_TYPE_ID)
                    .OrderBy(g => g.Key)
                    .ToList();

                foreach (var group in groupedList)
                {
                    groupNumOrder++;
                    int numOrderInGroup = 0;
                    decimal groupTotal = 0;
                    bool hasPrice = false;

                    var itemsInGroup = group
                        .OrderBy(o => o.TDL_SERVICE_NAME, StringComparer.CurrentCulture)
                        .ToList();

                    foreach (var sereServ in itemsInGroup)
                    {
                        numOrder++;
                        numOrderInGroup++;

                        decimal intoMoney = sereServ.AMOUNT * sereServ.PRICE;
                        if (sereServ.PRICE != 0)
                        {
                            hasPrice = true;
                            groupTotal += intoMoney;
                        }

                        HIS_SERVICE_UNIT unit = null;
                        unitDic.TryGetValue(sereServ.TDL_SERVICE_UNIT_ID, out unit);

                        short isExpend = sereServ.IS_EXPEND ?? 0;

                        rdo.Items.Add(new Mps000324ItemADO
                        {
                            GROUP_ID = group.Key,
                            NUM_ORDER = numOrder,
                            NUM_ORDER_IN_GROUP = numOrderInGroup,
                            SERE_SERV_ID = sereServ.ID,
                            SERVICE_CODE = sereServ.TDL_SERVICE_CODE,
                            SERVICE_NAME = sereServ.TDL_SERVICE_NAME,
                            SERVICE_UNIT_NAME = unit != null ? unit.SERVICE_UNIT_NAME : null,
                            AMOUNT = sereServ.AMOUNT,
                            PRICE = sereServ.PRICE != 0 ? sereServ.PRICE : (decimal?)null,
                            INTO_MONEY = sereServ.PRICE != 0 ? intoMoney : (decimal?)null,
                            IS_EXPEND = isExpend,
                            NOTE = isExpend == 1 ? "Hao Phí" : "Thu Phí"
                        });
                    }

                    grandTotal += groupTotal;

                    HIS_SERVICE_TYPE serviceType = null;
                    typeDic.TryGetValue(group.Key, out serviceType);

                    rdo.Groups.Add(new Mps000324GroupADO
                    {
                        ID = group.Key,
                        NUM_ORDER = groupNumOrder,
                        NUM_ORDER_ROMAN = ToRoman(groupNumOrder),
                        SERVICE_TYPE_ROMAN = ToRoman((int)group.Key),
                        SERVICE_TYPE_CODE = serviceType != null ? serviceType.SERVICE_TYPE_CODE : null,
                        SERVICE_TYPE_NAME = serviceType != null ? serviceType.SERVICE_TYPE_NAME : null,
                        ITEM_COUNT = itemsInGroup.Count,
                        TOTAL_AMOUNT = hasPrice ? groupTotal : (decimal?)null
                    });
                }

                SetSingleKey(Mps000324ExtendSingleKey.GRAND_TOTAL_AMOUNT, grandTotal);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bo sung cac key moi cho mau in. Dung SetSingleKey nen key da co san
        /// (do SetSingleKey/AddObjectKeyIntoListkey sinh truoc do) se KHONG bi ghi de.
        /// </summary>
        private void SetSingleKeyExtend()
        {
            try
            {
                SetEkipUserGroupKey();
                SetBedKey();

                if (rdo.ServiceReqPrint != null)
                {
                    SetSingleKey(Mps000324ExtendSingleKey.START_TIME_SEPARATE_STR,
                        ToTimeSeparateString(rdo.ServiceReqPrint.START_TIME));
                    SetSingleKey(Mps000324ExtendSingleKey.FINISH_TIME_SEPARATE_STR,
                        ToTimeSeparateString(rdo.ServiceReqPrint.FINISH_TIME));
                    SetSingleKey(Mps000324ExtendSingleKey.TICKET_NUMBER_STR,
                        String.Format("{0} - {1}",
                            rdo.ServiceReqPrint.SERVICE_REQ_CODE,
                            rdo.ServiceReqPrint.NUM_ORDER));
                }

                if (rdo.sereServsPttt != null)
                {
                    SetSingleKey(Mps000324ExtendSingleKey.PTTT_NOTE_STR, rdo.sereServsPttt.OTHER);
                    SetSingleKey(Mps000324ExtendSingleKey.REAL_PTTT_METHOD_STR,
                        String.Join(" ", new[]
                        {
                            rdo.sereServsPttt.REAL_PTTT_METHOD_CODE,
                            rdo.sereServsPttt.REAL_PTTT_METHOD_NAME
                        }.Where(o => !String.IsNullOrWhiteSpace(o))));
                }

                if (rdo.sereServ != null)
                {
                    SetSingleKey(Mps000324ExtendSingleKey.MAIN_SERVICE_NAME_STR, rdo.sereServ.TDL_SERVICE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dung khoi vai tro kip mo TU DANH MUC HIS_EXECUTE_ROLE — mau in khong hardcode ma vai tro.
        /// Vai tro khong co nguoi van duoc liet ke (USER_COUNT = 0) de mau in tu quyet dinh an/hien.
        /// Dong thoi set key rieng theo ma vai tro (USERNAMES_/LOGIN_NAMES_/EXECUTE_ROLE_NAME_)
        /// cho cac o chu ky co vi tri co dinh. Key USERNAME_/LOGIN_NAME_ cu giu nguyen gia tri.
        /// </summary>
        private void SetEkipUserGroupKey()
        {
            try
            {
                rdo.EkipRoles = new List<Mps000324EkipRoleADO>();

                List<V_HIS_EKIP_USER> ekipUsers = rdo.ekipUsers ?? new List<V_HIS_EKIP_USER>();

                // Gom thanh vien theo ma vai tro — mot vai co the co nhieu nguoi
                var usersByRoleCode = ekipUsers
                    .Where(o => o != null && !String.IsNullOrWhiteSpace(o.EXECUTE_ROLE_CODE))
                    .GroupBy(o => o.EXECUTE_ROLE_CODE)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Danh muc vai tro dang dung. Thieu danh muc thi lay tam cac vai co trong kip
                var roles = (rdo.ExecuteRoles ?? new List<HIS_EXECUTE_ROLE>())
                    .Where(o => o != null && o.IS_ACTIVE == 1 && o.IS_DELETE == 0)
                    .OrderBy(o => o.EXECUTE_ROLE_CODE, StringComparer.Ordinal)
                    .ThenBy(o => o.ID)
                    .ToList();

                if (roles.Count > 0)
                {
                    int numOrder = 0;
                    foreach (var role in roles)
                    {
                        numOrder++;
                        List<V_HIS_EKIP_USER> users = null;
                        if (String.IsNullOrWhiteSpace(role.EXECUTE_ROLE_CODE)
                            || !usersByRoleCode.TryGetValue(role.EXECUTE_ROLE_CODE, out users))
                        {
                            users = new List<V_HIS_EKIP_USER>();
                        }

                        rdo.EkipRoles.Add(new Mps000324EkipRoleADO
                        {
                            EXECUTE_ROLE_ID = role.ID,
                            EXECUTE_ROLE_CODE = role.EXECUTE_ROLE_CODE,
                            EXECUTE_ROLE_NAME = role.EXECUTE_ROLE_NAME,
                            NUM_ORDER = numOrder,
                            USER_COUNT = users.Count,
                            USERNAMES = JoinName(users.Select(o => o.USERNAME)),
                            LOGINNAMES = JoinName(users.Select(o => o.LOGINNAME)),
                            IS_SURG_MAIN = role.IS_SURG_MAIN ?? 0
                        });

                        SetSingleKey(
                            Mps000324ExtendSingleKey.PREFIX_EXECUTE_ROLE_NAME + role.EXECUTE_ROLE_CODE,
                            role.EXECUTE_ROLE_NAME);
                    }
                }
                else
                {
                    // Khong lay duoc danh muc — van dung duoc khoi ekip tu chinh du lieu kip mo
                    Inventec.Common.Logging.LogSystem.Warn(
                        "Mps000324: khong co danh muc HIS_EXECUTE_ROLE, dung ten vai tro tu V_HIS_EKIP_USER.");

                    int numOrder = 0;
                    foreach (var roleGroup in usersByRoleCode.OrderBy(o => o.Key, StringComparer.Ordinal))
                    {
                        numOrder++;
                        var first = roleGroup.Value.FirstOrDefault();
                        rdo.EkipRoles.Add(new Mps000324EkipRoleADO
                        {
                            EXECUTE_ROLE_ID = first != null ? (first.EXECUTE_ROLE_ID) : 0,
                            EXECUTE_ROLE_CODE = roleGroup.Key,
                            EXECUTE_ROLE_NAME = first != null ? first.EXECUTE_ROLE_NAME : null,
                            NUM_ORDER = numOrder,
                            USER_COUNT = roleGroup.Value.Count,
                            USERNAMES = JoinName(roleGroup.Value.Select(o => o.USERNAME)),
                            LOGINNAMES = JoinName(roleGroup.Value.Select(o => o.LOGINNAME)),
                            IS_SURG_MAIN = first != null ? (first.IS_SURG_MAIN ?? 0) : (short)0
                        });

                        if (first != null)
                        {
                            SetSingleKey(
                                Mps000324ExtendSingleKey.PREFIX_EXECUTE_ROLE_NAME + roleGroup.Key,
                                first.EXECUTE_ROLE_NAME);
                        }
                    }
                }

                // Key theo ma vai tro cho cac o co vi tri co dinh tren mau
                foreach (var roleGroup in usersByRoleCode)
                {
                    SetSingleKey(
                        Mps000324ExtendSingleKey.PREFIX_USERNAMES_EXECUTE_ROLE + roleGroup.Key,
                        JoinName(roleGroup.Value.Select(o => o.USERNAME)));

                    SetSingleKey(
                        Mps000324ExtendSingleKey.PREFIX_LOGIN_NAMES_EXECUTE_ROLE + roleGroup.Key,
                        JoinName(roleGroup.Value.Select(o => o.LOGINNAME)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Ghep ten, bo gia tri rong, ngan cach bang ", "</summary>
        private static string JoinName(IEnumerable<string> names)
        {
            string result = "";
            try
            {
                if (names == null) return result;
                result = String.Join(", ", names.Where(o => !String.IsNullOrWhiteSpace(o)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Set key buong/giuong. Khong dung AddObjectKeyIntoListkey de tranh bom hang chuc key
        /// trung ten voi cac doi tuong khac vao singleValueDictionary.
        /// </summary>
        private void SetBedKey()
        {
            try
            {
                if (rdo.bedLog == null) return;

                SetSingleKey(Mps000324ExtendSingleKey.BED_CODE_STR, rdo.bedLog.BED_CODE);
                SetSingleKey(Mps000324ExtendSingleKey.BED_NAME_STR, rdo.bedLog.BED_NAME);
                SetSingleKey(Mps000324ExtendSingleKey.BED_ROOM_NAME_STR, rdo.bedLog.BED_ROOM_NAME);
                SetSingleKey(Mps000324ExtendSingleKey.BED_ROOM_BED_STR,
                    String.Join(" - ", new[] { rdo.bedLog.BED_ROOM_NAME, rdo.bedLog.BED_NAME }
                        .Where(o => !String.IsNullOrWhiteSpace(o))));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dinh dang thoi gian dang "08 giờ 00 phút, Ngày 27 tháng 05 năm 2026".
        /// Tra ve chuoi rong khi khong co gia tri.
        /// </summary>
        private string ToTimeSeparateString(long? time)
        {
            string result = "";
            try
            {
                if (!time.HasValue || time.Value <= 0) return result;

                DateTime? value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(time.Value);
                if (!value.HasValue) return result;

                result = String.Format("{0:00} giờ {1:00} phút, Ngày {2:00} tháng {3:00} năm {4:0000}",
                    value.Value.Hour, value.Value.Minute,
                    value.Value.Day, value.Value.Month, value.Value.Year);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Doi so nguyen duong sang so La Ma. Tra ve chuoi rong khi ngoai khoang 1..3999.
        /// </summary>
        private static string ToRoman(int number)
        {
            string result = "";
            try
            {
                if (number <= 0 || number > 3999) return result;

                int[] values = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
                string[] symbols = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

                StringBuilder builder = new StringBuilder();
                int remain = number;
                for (int i = 0; i < values.Length; i++)
                {
                    while (remain >= values[i])
                    {
                        builder.Append(symbols[i]);
                        remain -= values[i];
                    }
                }
                result = builder.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
