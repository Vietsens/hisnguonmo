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
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000304.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000304.ADO;
using Newtonsoft.Json;

namespace MPS.Processor.Mps000304
{
    public partial class Mps000304Processor : AbstractProcessor
    {
        internal void DataInputProcess_DepaRoom()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("DataInputProcess_DepaRoom()");
                this.sereServADOs_DepaRoom = new List<SereServADO>();
                var sereServADOTemps = new List<SereServADO>();
                GroupType groupType = GroupType.DepaRoom;
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, rdo.SereServs, rdo.SereServExts, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.Departments, rdo.medicineTypes, rdo.MedicineLines, rdo.materialTypes, rdo.PatientTypeCFG, rdo.HisConfigValue, rdo.HisServiceUnit, rdo.ListOtherPaySource, rdo.Treatment, groupType));

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADOTemps), sereServADOTemps));

                sereServADOTemps = sereServADOTemps
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT
                        && o.PRICE_BHYT > 0
                        && o.IS_NO_EXECUTE != 1
                        && o.IS_EXPEND != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ToList();
                //sereServ la bhyt, gom nhom
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
                        o.GROUP_DEPARTMENT_ID__DepaRoom
                    }).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    SereServADO sereServ = sereServBHYTGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServBHYTGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = sereServBHYTGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    sereServ.TOTAL_PRICE_BHYT = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServ.TOTAL_PRICE_PATIENT_SELF = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    sereServ.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE);
                    sereServ.TOTAL_PATIENT_PRICE_LEFT = sereServBHYTGroup.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    sereServ.TOTAL_PATIENT_LEFT = sereServBHYTGroup.Sum(o => o.TOTAL_PATIENT_LEFT);
                    sereServ.TOTAL_PRICE_VP = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_VP);
                    sereServADOs_DepaRoom.Add(sereServ);
                }

                this.sereServADOs_DepaRoom = sereServADOs_DepaRoom.OrderBy(o => o.SERVICE_NAME).ToList();

                this.PatyAlterProcess_DepaRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void GroupDisplayProcess_DepaRoom()
        {
            try
            {
                this.HeinServiceTypeProcess_DepaRoom();

                sereServADOs_DepaRoom.ForEach(o =>
                {
                    if (o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT)
                    {
                        long? heinServiceTypeId = o.HEIN_SERVICE_TYPE_ID;
                        o.HEIN_SERVICE_TYPE_PARENT_1_ID = heinServiceTypeId;
                        o.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.BED__ID;
                    }
                });

                this.MedicineLineProcesss_DepaRoom();

                this.HeinServiceTypeBedProcess_DepaRoom();

                this.GroupDepartmentProcess_DepaExeRoom();

                this.GroupDepartmentProcess_DepaRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gom nhóm theo loại dịch vụ thuật toán xử lý như sau
        /// - Giường
        /// Đây là xử lý gom nhóm loại cha là Giường nên sẽ không lấy các nhóm giường con (giường nội trú và ngoại trú, ...) 
        /// tạo 1 đối tượng là giường được gán ID hashcode trong class HeinServiceTypeExt
        /// - Thuốc
        /// Các thuốc trong danh mục, ngoài danh mục, cũng gom vào và tạo 1 đối tượng thuốc trong HeinServiceTypeExt
        /// - Vật tư
        /// --Vật tư y tế
        /// Tương tự thuốc (gom vào và tạo đối tượng với ID, NAME hashcode)
        /// --Gói vật tư
        /// Vì cần gom nhóm theo dịch vụ là gói nên ta gán HEIN_SERVICE_TYPE_ID của dịch vụ bằng chính PARENT_ID của các dịch vụ con
        /// từ đó để tách loại dịch vụ
        /// - Các dịch vụ khác vẫn lấy theo HEIN_SERVICE_TYPE_ID trong danh mục
        /// 
        /// </summary>
        internal void HeinServiceTypeProcess_DepaRoom()
        {
            try
            {
                heinServiceTypeADOs_DepaRoom = new List<HeinServiceTypeADO>();
                var sereServBHYTGroups = sereServADOs_DepaRoom.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .ThenBy(o => o.TDL_INTRUCTION_TIME).GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID__DepaRoom, o.GROUP_ROOM_ID__ExeRoom }).ToList();

                List<long> parentIdVTs = this.sereServADOs_DepaRoom.Where(o => o.HEIN_SERVICE_TYPE_ID == o.PARENT_ID).Select(p => p.PARENT_ID ?? 0).Distinct().ToList();

                int indexGoiVatTuYTe = 1;
                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();

                    heinServiceType.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
                    heinServiceType.GROUP_DEPARTMENT_ID__DepaRoom = sereServBHYT.GROUP_DEPARTMENT_ID__DepaRoom;
                    heinServiceType.GROUP_DEPARTMENT_ROOM_CODE = sereServBHYT.GROUP_DEPARTMENT_ROOM_CODE;
                    heinServiceType.GROUP_DEPARTMENT_ROOM_NAME = sereServBHYT.GROUP_DEPARTMENT_ROOM_NAME;
                    heinServiceType.GROUP_ROOM_ID__ExeRoom = sereServBHYT.GROUP_ROOM_ID__ExeRoom;
                    heinServiceType.GROUP_ROOM_CODE = sereServBHYT.GROUP_ROOM_CODE;
                    heinServiceType.GROUP_ROOM_NAME = sereServBHYT.GROUP_ROOM_NAME;
                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    heinServiceType.OTHER_SOURCE_PRICE = sereServBHYTGroup.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                    heinServiceType.TOTAL_PATIENT_PRICE_LEFT = sereServBHYTGroup.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    heinServiceType.TOTAL_PATIENT_LEFT = sereServBHYTGroup.Sum(o => o.TOTAL_PATIENT_LEFT);
                    heinServiceType.TOTAL_PRICE_VP = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_VP);

                    heinServiceType.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE + heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_PRICE = heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_HEIN_PRICE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                    heinServiceType.TOTAL_PATIENT_PRICE_SELF = heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;

                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        if (parentIdVTs.Contains(sereServBHYT.HEIN_SERVICE_TYPE_ID.Value))
                        {
                            //thêm 1 dòng 10. gói vật tư y tế cộng dồn các gói lại
                            HeinServiceTypeADO goi = heinServiceTypeADOs_DepaRoom.FirstOrDefault(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.GOI_VT_Y_TE__ID && o.GROUP_DEPARTMENT_ID__DepaRoom == heinServiceType.GROUP_DEPARTMENT_ID__DepaRoom);
                            if (goi != null)
                            {
                                goi.TOTAL_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                goi.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE += heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                goi.OTHER_SOURCE_PRICE += heinServiceType.OTHER_SOURCE_PRICE;
                                goi.TOTAL_BHYT_PRICE += heinServiceType.TOTAL_BHYT_PRICE;
                                goi.TOTAL_PRICE += heinServiceType.TOTAL_PRICE;
                                goi.TOTAL_HEIN_PRICE += heinServiceType.TOTAL_HEIN_PRICE;
                                goi.TOTAL_PATIENT_PRICE_SELF += heinServiceType.TOTAL_PATIENT_PRICE_SELF;
                                goi.TOTAL_PATIENT_PRICE_LEFT += heinServiceType.TOTAL_PATIENT_PRICE_LEFT;
                                goi.TOTAL_PATIENT_LEFT += heinServiceType.TOTAL_PATIENT_LEFT;
                                goi.TOTAL_PRICE_VP += heinServiceType.TOTAL_PRICE_VP;
                            }
                            else
                            {
                                goi = new HeinServiceTypeADO();
                                goi.KEY_PATY_ALTER = sereServBHYT.KEY_PATY_ALTER;
                                goi.GROUP_DEPARTMENT_ID__DepaRoom = sereServBHYT.GROUP_DEPARTMENT_ID__DepaRoom;
                                goi.GROUP_DEPARTMENT_ROOM_CODE = sereServBHYT.GROUP_DEPARTMENT_ROOM_CODE;
                                goi.GROUP_DEPARTMENT_ROOM_NAME = sereServBHYT.GROUP_DEPARTMENT_ROOM_NAME;
                                goi.GROUP_ROOM_ID__ExeRoom = sereServBHYT.GROUP_ROOM_ID__ExeRoom;
                                goi.GROUP_ROOM_CODE = sereServBHYT.GROUP_ROOM_CODE;
                                goi.GROUP_ROOM_NAME = sereServBHYT.GROUP_ROOM_NAME;
                                goi.ID = HeinServiceTypeExt.GOI_VT_Y_TE__ID;
                                goi.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.GOI_VT_Y_TE__NAME;
                                goi.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                                goi.TOTAL_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                goi.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                goi.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                goi.OTHER_SOURCE_PRICE = heinServiceType.OTHER_SOURCE_PRICE;
                                goi.TOTAL_BHYT_PRICE = heinServiceType.TOTAL_BHYT_PRICE;
                                goi.TOTAL_PRICE = heinServiceType.TOTAL_PRICE;
                                goi.TOTAL_HEIN_PRICE = heinServiceType.TOTAL_HEIN_PRICE;
                                goi.TOTAL_PATIENT_PRICE_SELF = heinServiceType.TOTAL_PATIENT_PRICE_SELF;
                                goi.TOTAL_PATIENT_PRICE_LEFT = heinServiceType.TOTAL_PATIENT_PRICE_LEFT;
                                goi.TOTAL_PATIENT_LEFT += heinServiceType.TOTAL_PATIENT_LEFT;
                                goi.TOTAL_PRICE_VP += heinServiceType.TOTAL_PRICE_VP;
                                heinServiceTypeADOs_DepaRoom.Add(goi);
                            }

                            //chi tiết gói không kèm stent 2 trở đi
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
                            heinServiceType.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = indexGoiVatTuYTe;

                            HIS_SERE_SERV sereServParent = rdo.SereServs.FirstOrDefault(o => o.ID == sereServBHYT.HEIN_SERVICE_TYPE_ID.Value);
                            string heinServiceTypeName = String.Format("{0} {1}({2})", sereServBHYT.HEIN_SERVICE_TYPE_NAME, indexGoiVatTuYTe, sereServParent.TDL_HEIN_SERVICE_BHYT_NAME);
                            heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceTypeName;
                            heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
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
                            || sereServBHYT.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT))
                    {
                        var lstGiuong = heinServiceTypeADOs_DepaRoom.Where(o => o.KEY_PATY_ALTER == heinServiceType.KEY_PATY_ALTER && o.ID == HeinServiceTypeExt.BED__ID && o.GROUP_DEPARTMENT_ID__DepaRoom == heinServiceType.GROUP_DEPARTMENT_ID__DepaRoom).ToList();
                        if (lstGiuong != null && lstGiuong.Count > 0)
                            continue;
                        else
                        {
                            heinServiceType.ID = HeinServiceTypeExt.BED__ID;
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.BED__NAME;
                            heinServiceType.NUM_ORDER = (int)sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                        }
                    }

                    heinServiceTypeADOs_DepaRoom.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void HeinServiceTypeBedProcess_DepaRoom()
        {
            try
            {
                this.HeinServiceTypeBeds_DepaRoom = new List<HeinServiceTypeADO>();

                var sereServBHYTGroups = sereServADOs_DepaRoom.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_PARENT_1_ID, o.GROUP_DEPARTMENT_ID__DepaRoom, o.GROUP_ROOM_ID__ExeRoom }).ToList();

                foreach (var g in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    heinServiceType.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;

                    heinServiceType.PARENT_ID = g.First().HEIN_SERVICE_TYPE_ID;
                    heinServiceType.ID = g.First().HEIN_SERVICE_TYPE_PARENT_1_ID;
                    heinServiceType.MEDICINE_LINE_ID = g.First().MEDICINE_LINE_ID;
                    heinServiceType.GROUP_DEPARTMENT_ID__DepaRoom = g.First().GROUP_DEPARTMENT_ID__DepaRoom;
                    heinServiceType.GROUP_DEPARTMENT_ROOM_CODE = g.First().GROUP_DEPARTMENT_ROOM_CODE;
                    heinServiceType.GROUP_DEPARTMENT_ROOM_NAME = g.First().GROUP_DEPARTMENT_ROOM_NAME;
                    heinServiceType.GROUP_ROOM_ID__ExeRoom = g.First().GROUP_ROOM_ID__ExeRoom;
                    heinServiceType.GROUP_ROOM_CODE = g.First().GROUP_ROOM_CODE;
                    heinServiceType.GROUP_ROOM_NAME = g.First().GROUP_ROOM_NAME;
                    if (heinServiceType.PARENT_ID.HasValue && heinServiceType.PARENT_ID == HeinServiceTypeExt.BED__ID)
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = g.First().HEIN_SERVICE_TYPE_NAME;
                        heinServiceType.NUM_ORDER = g.First().HEIN_SERVICE_TYPE_NUM_ORDER;
                        heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = g
                            .Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g
                            .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        heinServiceType.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g
                           .Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        heinServiceType.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        heinServiceType.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        heinServiceType.TOTAL_PATIENT_LEFT = g.Sum(o => o.TOTAL_PATIENT_LEFT);
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
        /// Phai xu ly truoc khi gom nhom vi, gom nhom khong theo service_req
        /// </summary>
        /// <param name="sereServADOTemps"></param>
        internal void MedicineLineProcesss_DepaRoom()
        {
            try
            {
                medicineLineADOs_DepaRoom = new List<MedicineLineADO>();
                var sereServGroups = sereServADOs_DepaRoom
                    .OrderBy(o => o.MEDICINE_LINE_ID)
                    .GroupBy(o => new { o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_ID, o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID__DepaRoom, o.GROUP_ROOM_ID__ExeRoom }).ToList();
                foreach (var sereServs in sereServGroups)
                {
                    SereServADO sereServADO = sereServs.FirstOrDefault();
                    MedicineLineADO medicineLineADO = new MedicineLineADO();
                    medicineLineADO.ID = sereServADO.MEDICINE_LINE_ID;
                    medicineLineADO.HEIN_SERVICE_TYPE_ID = sereServADO.HEIN_SERVICE_TYPE_ID;
                    medicineLineADO.KEY_PATY_ALTER = sereServADO.KEY_PATY_ALTER;
                    medicineLineADO.MEDICINE_LINE_CODE = sereServADO.MEDICINE_LINE_CODE;
                    medicineLineADO.MEDICINE_LINE_NAME = sereServADO.MEDICINE_LINE_NAME;
                    if (sereServADO.MEDICINE_LINE_ID <= 0 && sereServADO.HEIN_SERVICE_TYPE_ID > 0)
                    {
                        medicineLineADO.MEDICINE_LINE_CODE = "Chưa xác định";
                        medicineLineADO.MEDICINE_LINE_NAME = "Chưa xác định";
                    }

                    //Tinh so thang
                    if (rdo.ServiceReqs != null && rdo.ServiceReqs.Count > 0)
                    {
                        List<long> serviceReqIds = sereServs.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        List<HIS_SERVICE_REQ> serviceReqTemps = rdo.ServiceReqs.Where(o => serviceReqIds.Contains(o.ID) && o.REMEDY_COUNT.HasValue).ToList();
                        if (serviceReqTemps != null && serviceReqTemps.Count > 0)
                        {
                            medicineLineADO.REMEDY_COUNT = serviceReqTemps.Sum(o => o.REMEDY_COUNT ?? 0);
                        }
                    }

                    medicineLineADO.GROUP_DEPARTMENT_ID = sereServADO.GROUP_DEPARTMENT_ID;
                    medicineLineADO.GROUP_DEPARTMENT_ID__DepaRoom = sereServADO.GROUP_DEPARTMENT_ID__DepaRoom;
                    medicineLineADO.GROUP_ROOM_ID__ExeRoom = sereServADO.GROUP_ROOM_ID__ExeRoom;

                    medicineLineADOs_DepaRoom.Add(medicineLineADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void PatyAlterProcess_DepaRoom()
        {
            try
            {
                this.patyAlterBHYTADOs_DepaRoom = new List<PatyAlterBhytADO>();
                if (sereServADOs_DepaRoom != null && sereServADOs_DepaRoom.Count > 0)
                {
                    var ssGroup = sereServADOs_DepaRoom.GroupBy(o => o.KEY_PATY_ALTER);
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
                        ado.TOTAL_PATIENT_LEFT = g.Sum(o => o.TOTAL_PATIENT_LEFT);
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

        private void GroupDepartmentProcess_DepaRoom()
        {
            try
            {
                this.ServiceGroupByDepa = new List<GroupDepartmentADO>();
                if (sereServADOs_DepaRoom != null && sereServADOs_DepaRoom.Count > 0)
                {
                    var ssGroup = sereServADOs_DepaRoom.GroupBy(o => new { o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID__DepaRoom }).ToList();
                    foreach (var g in ssGroup)
                    {
                        GroupDepartmentADO ado = new GroupDepartmentADO();
                        ado.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;
                        ado.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        ado.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        ado.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        ado.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        ado.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        ado.GROUP_DEPARTMENT_ID__DepaRoom = g.First().GROUP_DEPARTMENT_ID__DepaRoom;
                        ado.DEPARTMENT_CODE = g.First().GROUP_DEPARTMENT_ROOM_CODE;
                        ado.DEPARTMENT_NAME = g.First().GROUP_DEPARTMENT_ROOM_NAME;
                        ado.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                        ado.TOTAL_PATIENT_LEFT = g.Sum(o => o.TOTAL_PATIENT_LEFT);

                        if (rdo.Departments != null && rdo.Departments.Count > 0 && g.First().GROUP_DEPARTMENT_ID__DepaRoom > 0)
                        {
                            HIS_DEPARTMENT department = rdo.Departments.FirstOrDefault(o => o.ID == g.First().GROUP_DEPARTMENT_ID__DepaRoom);

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

        private void GroupDepartmentProcess_DepaExeRoom()
        {
            try
            {
                this.ServiceGroupByRoom = new List<GroupDepartmentADO>();
                if (sereServADOs_DepaRoom != null && sereServADOs_DepaRoom.Count > 0)
                {
                    var ssGroup = sereServADOs_DepaRoom.GroupBy(o => new { o.KEY_PATY_ALTER, o.GROUP_DEPARTMENT_ID__DepaRoom, o.GROUP_ROOM_ID__ExeRoom }).ToList();
                    foreach (var g in ssGroup)
                    {
                        GroupDepartmentADO ado = new GroupDepartmentADO();
                        ado.KEY_PATY_ALTER = g.First().KEY_PATY_ALTER;
                        ado.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        ado.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        ado.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                        ado.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
                        ado.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        ado.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                        ado.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        ado.GROUP_ROOM_ID__ExeRoom = g.First().GROUP_ROOM_ID__ExeRoom;
                        ado.GROUP_DEPARTMENT_ID__DepaRoom = g.First().GROUP_DEPARTMENT_ID__DepaRoom;
                        ado.ROOM_CODE = g.First().GROUP_ROOM_CODE;
                        ado.ROOM_NAME = g.First().GROUP_ROOM_NAME;
                        ado.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                        ado.TOTAL_PATIENT_LEFT = g.Sum(o => o.TOTAL_PATIENT_LEFT);
                        ado.GROUP_ROOM_CODE = g.First().GROUP_ROOM_CODE;
                        ado.GROUP_ROOM_NAME = g.First().GROUP_ROOM_NAME;

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
    }
}
