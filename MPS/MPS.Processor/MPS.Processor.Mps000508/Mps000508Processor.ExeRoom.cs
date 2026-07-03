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
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000508.ADO;
using MPS.Processor.Mps000508.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000508
{
    public partial class Mps000508Processor : AbstractProcessor
    {
        // Bộ dịch vụ được gom thêm chiều PHÒNG XỬ LÝ (dedup có phòng) - tương tự _ExeRoom của Mps000304. 
        private List<SereServADO> sereServADOs_ExeRoom { get; set; }
        // Master gom theo khoa / phòng xử lý (port từ Mps000512).
        private List<GroupDepartmentADO> ServiceGroupByDepa { get; set; }
        private List<GroupDepartmentADO> ServiceGroupByRoom { get; set; }
        // Loại dịch vụ (BHYT) gom theo phòng xử lý.
        private List<HeinServiceTypeADO> heinServiceTypeADOs_ExeRoom { get; set; }

        private List<PatyAlterBhytADO> patyAlterBHYTADOs_DepaRoom { get; set; }

        private List<HeinServiceTypeADO> HeinServiceTypeBeds_DepaRoom { get; set; }

        /// <summary>
        /// Orchestrator cho bộ gom theo phòng xử lý. Độc lập với luồng báo cáo chính (không sửa sereServADOs gốc).
        /// </summary>
        internal void ExeRoomProcess()
        {
            try
            {
                this.DataInputProcess_ExeRoom();

                this.PatyAlterProcess_DepaRoom();

                this.heinServiceTypeADOs_ExeRoom = this.HeinServiceTypeProcess_ExeRoom(this.sereServADOs_ExeRoom);

                // Sau khi đã gom loại dịch vụ, đưa các loại giường con về 1 loại "Giường" (giống GroupDisplayProcess).
                this.sereServADOs_ExeRoom.ForEach(o =>
                {
                    if (o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_BN
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_L)
                    {
                        long? heinServiceTypeId = o.HEIN_SERVICE_TYPE_ID;
                        o.HEIN_SERVICE_TYPE_PARENT_1_ID = heinServiceTypeId;
                        o.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.BED__ID;
                    }
                });


                this.HeinServiceTypeBedProcess_DepaRoom();

                this.GroupDepartmentProcess_DepaExeRoom();

                this.BuildDepartmentRoomGroups_ExeRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GroupDepartmentProcess_DepaExeRoom()
        {
            try
            {
                this.ServiceGroupByRoom = new List<GroupDepartmentADO>();
                if (sereServADOs_ExeRoom != null && sereServADOs_ExeRoom.Count > 0)
                {
                    var ssGroup = sereServADOs_ExeRoom.GroupBy(o => new { o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID, o.GROUP_ROOM_ID }).ToList();
                    foreach (var g in ssGroup)
                    {
                        GroupDepartmentADO ado = new GroupDepartmentADO();
                        ado.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;
                        ado.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                        ado.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        ado.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        ado.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                        ado.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        ado.GROUP_ROOM_ID = g.First().GROUP_ROOM_ID;
                        ado.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                        ado.ROOM_CODE = g.First().GROUP_ROOM_CODE;
                        ado.ROOM_NAME = g.First().GROUP_ROOM_NAME;
                        ado.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                        this.ServiceGroupByRoom.Add(ado);
                    }
                }

                this.ServiceGroupByRoom = this.ServiceGroupByRoom.OrderBy(o => o.ROOM_NAME).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void PatyAlterProcess_DepaRoom()
        {
            try
            {
                this.patyAlterBHYTADOs_DepaRoom = new List<PatyAlterBhytADO>();
                if (sereServADOs_ExeRoom != null && sereServADOs_ExeRoom.Count > 0)
                {
                    var ssGroup = sereServADOs_ExeRoom.GroupBy(o => o.KEY_PATY_ALTER);
                    foreach (var g in ssGroup)
                    {
                        PatyAlterBhytADO ado = new PatyAlterBhytADO();
                        ado = DataRawProcess.PatyAlterBHYTRawToADO(g.First().PatientTypeAlter, rdo.PatientTypeAlterAlls, rdo.Treatment, rdo.Branch, rdo.TreatmentTypes, rdo.CurrentPatyAlter, g.ToList());
                        ado.KEY = g.First().KEY_PATY_ALTER;
                        ado.TOTAL_PRICE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        ado.TOTAL_PRICE_BHYT = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        ado.TOTAL_PRICE_HEIN = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        ado.TOTAL_PRICE_PATIENT = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        ado.TOTAL_PRICE_PATIENT_SELF = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        ado.TOTAL_PRICE_OTHER = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        ado.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        ado.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                        if (g.First().PatientTypeAlter.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE
                            && g.First().PatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE
                            && rdo.Treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                        {
                            //gán lại RATIO_STR theo HEIN_RATIO được gom nhóm.
                            ado.RATIO_STR = ((int)(((g.FirstOrDefault(o => o.HEIN_RATIO.HasValue && !o.STENT_ORDER.HasValue) ?? g.First()).HEIN_RATIO ?? 0) * 100)) + "%";
                        }
                        patyAlterBHYTADOs_DepaRoom.Add(ado);
                    }

                    if (patyAlterBHYTADOs_DepaRoom != null && patyAlterBHYTADOs_DepaRoom.Count > 0)
                    {
                        patyAlterBHYTADOs_DepaRoom = patyAlterBHYTADOs_DepaRoom.OrderBy(o => o.LOG_TIME).ThenBy(o => o.KEY).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void HeinServiceTypeBedProcess_DepaRoom()
        {
            try
            {
                this.HeinServiceTypeBeds_DepaRoom = new List<HeinServiceTypeADO>();

                var sereServBHYTGroups = sereServADOs_ExeRoom.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_PARENT_1_ID, o.GROUP_DEPARTMENT_ID }).ToList();

                foreach (var g in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    heinServiceType.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;

                    heinServiceType.PARENT_ID = g.First().HEIN_SERVICE_TYPE_ID;
                    heinServiceType.ID = g.First().HEIN_SERVICE_TYPE_PARENT_1_ID;
                    heinServiceType.MEDICINE_LINE_ID = g.First().MEDICINE_LINE_ID;
                    heinServiceType.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                    heinServiceType.GROUP_DEPARTMENT_CODE = g.First().GROUP_DEPARTMENT_CODE;
                    heinServiceType.GROUP_DEPARTMENT_NAME = g.First().GROUP_DEPARTMENT_NAME;
                    if (heinServiceType.PARENT_ID.HasValue && heinServiceType.PARENT_ID == HeinServiceTypeExt.BED__ID)
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = g.First().HEIN_SERVICE_TYPE_NAME;
                        heinServiceType.NUM_ORDER = g.First().HEIN_SERVICE_TYPE_NUM_ORDER;
                        heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        heinServiceType.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        heinServiceType.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        heinServiceType.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                    }

                    this.HeinServiceTypeBeds_DepaRoom.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dựng bộ dịch vụ dedup CÓ phòng/khoa trong key (khác bộ chính dedup không có phòng -> tránh sai tổng theo phòng).
        /// </summary>
        private void DataInputProcess_ExeRoom()
        {
            this.sereServADOs_ExeRoom = new List<SereServADO>();
            try
            {
                var allSereServs = rdo.SereServs;
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, allSereServs, rdo.SereServExts, rdo.HeinServiceTypes,
                                          rdo.Services, rdo.Departments, rdo.Rooms, rdo.medicineTypes, rdo.MedicineLines, rdo.materialTypes, rdo.PatientTypeCFG,
                                          rdo.HisConfigValue, rdo.HisServiceUnit, rdo.Treatment,
                                          rdo.ServiceReqs,
                                          rdo.PatientTypeAlterAlls
                                          ));

                sereServADOTemps = sereServADOTemps
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT
                        && o.PRICE_BHYT > 0
                        && o.IS_NO_EXECUTE != 1
                        && o.IS_EXPEND != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER ?? 99999).ToList();

                // Dedup CÓ thêm GROUP_DEPARTMENT_ID + GROUP_ROOM_ID -> không gộp dịch vụ giữa các phòng khác nhau. 
                var sereServBHYTGroups = sereServADOTemps
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.PRIMARY_PRICE,
                        o.PRICE_BHYT,
                        o.SERVICE_PAY_RATE,
                        o.BHYT_PAY_RATE,
                        o.IS_EXPEND,
                        o.NUMBER_OF_FILM,
                        o.KEY_PATY_ALTER,
                        o.HEIN_SERVICE_TYPE_ID,
                        o.STENT_ORDER,
                        o.GROUP_DEPARTMENT_ID,
                        o.GROUP_ROOM_ID
                    }).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    SereServADO sereServ = sereServBHYTGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServBHYTGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    sereServ.TOTAL_PRICE_BHYT = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServ.TOTAL_PRICE_PATIENT_SELF = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    sereServ.TOTAL_PRICE_PATIENT_NO_PAY_RATE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_NO_PAY_RATE);
                    sereServ.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE);
                    sereServ.TOTAL_PATIENT_PRICE_LEFT = sereServBHYTGroup.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    sereServ.TOTAL_PRICE_VP = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_VP);
                    this.sereServADOs_ExeRoom.Add(sereServ);

                    if (sereServ.STENT_ORDER.HasValue && sereServ.STENT_ORDER.Value > 1)
                    {
                        decimal quyBHTT = sereServ.VIR_TOTAL_HEIN_PRICE ?? 0;
                        decimal bnCungChiTra = sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0;
                        decimal nguonKhac = sereServ.OTHER_SOURCE_PRICE ?? 0;

                        decimal bnHoacNguonKhac = bnCungChiTra > 0 ? bnCungChiTra : nguonKhac;

                        sereServ.TOTAL_PRICE_BHYT = quyBHTT + bnHoacNguonKhac;
                    }
                }

                // Mã/tên khoa + phòng đã được set trong SereServADO (lấy theo phòng chỉ định/thực hiện - giống Mps000304).
                this.sereServADOs_ExeRoom = this.sereServADOs_ExeRoom.OrderBy(o => o.STENT_ORDER ?? 0).ThenBy(o => o.SERVICE_NAME).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gom loại dịch vụ theo phòng. Cùng logic xử lý đặc biệt (Gói VTYT, Giường) như HeinServiceTypeProcess,
        /// nhưng key gom thêm GROUP_DEPARTMENT_ID + GROUP_ROOM_ID, các lookup gói/giường giới hạn trong cùng phòng.
        /// </summary>
        private List<HeinServiceTypeADO> HeinServiceTypeProcess_ExeRoom(List<SereServADO> sereServAdos)
        {
            List<HeinServiceTypeADO> heinServiceTypeADOs = new List<HeinServiceTypeADO>();
            try
            {
                var sereServBHYTGroups = sereServAdos.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER ?? 99999)
                    .ThenBy(o => o.TDL_INTRUCTION_TIME)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID, o.GROUP_ROOM_ID }).ToList();

                List<long> parentIdVTs = sereServAdos.Where(o => o.HEIN_SERVICE_TYPE_ID == o.PARENT_ID).Select(p => p.PARENT_ID ?? 0).Distinct().ToList();

                int indexGoiVatTuYTe = 1;
                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();

                    heinServiceType.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
                    heinServiceType.GROUP_ROOM_ID__ExeRoom = sereServBHYT.GROUP_ROOM_ID;
                    heinServiceType.GROUP_ROOM_CODE = sereServBHYT.GROUP_ROOM_CODE;
                    heinServiceType.GROUP_ROOM_NAME = sereServBHYT.GROUP_ROOM_NAME;
                    heinServiceType.GROUP_DEPARTMENT_ID = sereServBHYT.GROUP_DEPARTMENT_ID;
                    heinServiceType.GROUP_DEPARTMENT_CODE = sereServBHYT.GROUP_DEPARTMENT_CODE;
                    heinServiceType.GROUP_DEPARTMENT_NAME = sereServBHYT.GROUP_DEPARTMENT_NAME;
                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);

                    heinServiceType.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_NO_PAY_RATE ?? 0);
                    heinServiceType.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_LEFT = sereServBHYTGroup.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    heinServiceType.TOTAL_PRICE_VP = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_VP);

                    heinServiceType.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE + heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_PRICE = heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_HEIN_PRICE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF = heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;

                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        if (parentIdVTs.Contains(sereServBHYT.HEIN_SERVICE_TYPE_ID.Value))
                        {
                            // Gói vật tư y tế: cộng dồn các gói trong CÙNG phòng.
                            HeinServiceTypeADO goi = heinServiceTypeADOs.FirstOrDefault(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.GOI_VT_Y_TE__ID && o.GROUP_ROOM_ID__ExeRoom == heinServiceType.GROUP_ROOM_ID__ExeRoom);
                            if (goi != null)
                            {
                                goi.TOTAL_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                goi.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE;
                                goi.OTHER_SOURCE_PRICE += heinServiceType.OTHER_SOURCE_PRICE;
                                goi.TOTAL_BHYT_PRICE += heinServiceType.TOTAL_BHYT_PRICE;
                                goi.TOTAL_PRICE += heinServiceType.TOTAL_PRICE;
                                goi.TOTAL_HEIN_PRICE += heinServiceType.TOTAL_HEIN_PRICE;
                                goi.TOTAL_PATIENT_PRICE_SELF += heinServiceType.TOTAL_PATIENT_PRICE_SELF;
                                goi.TOTAL_PATIENT_PRICE_LEFT += heinServiceType.TOTAL_PATIENT_PRICE_LEFT;
                                goi.TOTAL_PRICE_VP += heinServiceType.TOTAL_PRICE_VP;
                            }
                            else
                            {
                                goi = new HeinServiceTypeADO();
                                goi.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
                                goi.GROUP_ROOM_ID__ExeRoom = sereServBHYT.GROUP_ROOM_ID;
                                goi.GROUP_ROOM_CODE = sereServBHYT.GROUP_ROOM_CODE;
                                goi.GROUP_ROOM_NAME = sereServBHYT.GROUP_ROOM_NAME;
                                goi.GROUP_DEPARTMENT_ID = sereServBHYT.GROUP_DEPARTMENT_ID;
                                goi.GROUP_DEPARTMENT_CODE = sereServBHYT.GROUP_DEPARTMENT_CODE;
                                goi.GROUP_DEPARTMENT_NAME = sereServBHYT.GROUP_DEPARTMENT_NAME;
                                goi.ID = HeinServiceTypeExt.GOI_VT_Y_TE__ID;
                                goi.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.GOI_VT_Y_TE__NAME;
                                goi.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                                goi.TOTAL_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                goi.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_PATIENT_NO_PAY_RATE_HEIN_SERVICE_TYPE;
                                goi.OTHER_SOURCE_PRICE = heinServiceType.OTHER_SOURCE_PRICE;
                                goi.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_BHYT_PRICE;
                                goi.TOTAL_PRICE = heinServiceType.TOTAL_PRICE;
                                goi.TOTAL_HEIN_PRICE = heinServiceType.TOTAL_HEIN_PRICE;
                                goi.TOTAL_PATIENT_PRICE_SELF = heinServiceType.TOTAL_PATIENT_PRICE_SELF;
                                goi.TOTAL_PATIENT_PRICE_LEFT = heinServiceType.TOTAL_PATIENT_PRICE_LEFT;
                                goi.TOTAL_PRICE_VP = heinServiceType.TOTAL_PRICE_VP;
                                heinServiceTypeADOs.Add(goi);
                            }

                            var sereServNoStent = sereServBHYTGroup.Where(o => !o.STENT_ORDER.HasValue).ToList();
                            var stent = sereServBHYTGroup.Where(o => o.STENT_ORDER.HasValue).OrderBy(o => o.STENT_ORDER).FirstOrDefault();
                            if (stent != null)
                            {
                                sereServNoStent.Add(stent);
                            }
                            heinServiceType.TOTAL_PRICE = sereServNoStent.Sum(s => s.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                            heinServiceType.TOTAL_HEIN_PRICE = sereServNoStent.Sum(s => s.VIR_TOTAL_HEIN_PRICE ?? 0);
                            heinServiceType.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_HEIN_PRICE + heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                            heinServiceType.TOTAL_PATIENT_PRICE_SELF = sereServNoStent.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);

                            //cắt stent 2 trở đi cho đồng bộ với Mps000302/ProcessorPlus (trước đây ExeRoom thiếu 2 field này nên loại con cộng đủ)
                            heinServiceType.TOTAL_HEIN_PRICE = sereServNoStent.Sum(s => s.VIR_TOTAL_HEIN_PRICE ?? 0);
                            heinServiceType.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_HEIN_PRICE + heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                            heinServiceType.TOTAL_PRICE_VP = sereServNoStent.Sum(s => s.TOTAL_PRICE_VP);
                            heinServiceType.TOTAL_PATIENT_PRICE_LEFT = sereServNoStent.Sum(s => s.TOTAL_PATIENT_PRICE_LEFT);

                            HIS_SERE_SERV sereServParent = rdo.SereServs.FirstOrDefault(o => o.ID == sereServBHYT.HEIN_SERVICE_TYPE_ID.Value);
                            string heinServiceTypeName = String.Format("{0} {1}({2})", sereServBHYT.HEIN_SERVICE_TYPE_NAME, indexGoiVatTuYTe, sereServParent != null ? sereServParent.TDL_HEIN_SERVICE_BHYT_NAME : null);
                            heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceTypeName;
                            heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                            heinServiceType.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = indexGoiVatTuYTe;
                            indexGoiVatTuYTe++;
                        }
                        else
                        {
                            heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServBHYT.HEIN_SERVICE_TYPE_NAME;
                            heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                        }
                    }
                    else
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }

                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue
                        && (sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_BN
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_L))
                    {
                        var lstGiuong = heinServiceTypeADOs.Where(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.BED__ID && o.GROUP_ROOM_ID__ExeRoom == heinServiceType.GROUP_ROOM_ID__ExeRoom).ToList();
                        if (lstGiuong != null && lstGiuong.Count > 0)
                            continue;
                        else
                        {
                            heinServiceType.ID = HeinServiceTypeExt.BED__ID;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.BED__NAME;
                            heinServiceType.NUM_ORDER = (int)sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                        }
                    }

                    heinServiceTypeADOs.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return heinServiceTypeADOs;
        }

        /// <summary>
        /// Dựng 2 bộ master gom theo khoa xử lý và phòng xử lý từ bộ ServiceExeRoom (port từ Mps000512).
        /// </summary>
        private void BuildDepartmentRoomGroups_ExeRoom()
        {
            try
            {
                this.ServiceGroupByDepa = new List<GroupDepartmentADO>();
                if (sereServADOs_ExeRoom != null && sereServADOs_ExeRoom.Count > 0)
                {
                    var ssGroup = sereServADOs_ExeRoom.GroupBy(o => new { o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID }).ToList();
                    foreach (var g in ssGroup)
                    {
                        GroupDepartmentADO ado = new GroupDepartmentADO();
                        ado.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;
                        ado.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                        ado.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        ado.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                        ado.VIR_TOTAL_PATIENT_PRICE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                        ado.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        ado.GROUP_DEPARTMENT_ID = g.First().GROUP_DEPARTMENT_ID;
                        ado.DEPARTMENT_CODE = g.First().GROUP_DEPARTMENT_CODE;
                        ado.DEPARTMENT_NAME = g.First().GROUP_DEPARTMENT_NAME;
                        ado.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);

                        if (rdo.Departments != null && rdo.Departments.Count > 0 && g.First().GROUP_DEPARTMENT_ID > 0)
                        {
                            HIS_DEPARTMENT department = rdo.Departments.FirstOrDefault(o => o.ID == g.First().GROUP_DEPARTMENT_ID);

                            if (department != null)
                            {
                                ado.IS_CLINICAL = department.IS_CLINICAL;
                            }
                        }

                        this.ServiceGroupByDepa.Add(ado);
                    }
                }

                this.ServiceGroupByDepa = this.ServiceGroupByDepa.OrderBy(o => o.IS_CLINICAL != 1 ? 0 : 1).ThenBy(o => o.DEPARTMENT_NAME).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static GroupDepartmentADO NewGroupTotals(IEnumerable<SereServADO> g)
        {
            decimal totalPriceBhyt = g.Sum(o => o.TOTAL_PRICE_BHYT);
            decimal totalHeinPrice = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
            decimal totalPatientPrice = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
            return new GroupDepartmentADO
            {
                TOTAL_PRICE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0),
                TOTAL_PRICE_BHYT = totalPriceBhyt,
                TOTAL_HEIN_PRICE = totalHeinPrice,
                TOTAL_PATIENT_PRICE = totalPatientPrice,
                TOTAL_PATIENT_PRICE_SELF = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF),
                OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0),
                TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP),
                TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT),
                // Alias trùng tên bộ HeinServiceType
                TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = totalPriceBhyt,
                TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = totalHeinPrice,
                TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = totalPatientPrice
            };
        }
    }
}
