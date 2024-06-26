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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientExam
{
    class ProcessRoomNumOrder
    {
        private Dictionary<long, long> DicRoomCol;
        private List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ> ServiceReq;
        private Color CLX2 = Color.Red;
        private Color CLX1 = Color.Yellow;
        private Color DXL = Color.Green;

        public ProcessRoomNumOrder(Dictionary<long, long> dicRooCol, List<HIS_SERVICE_REQ> serviceReq)
        {
            this.DicRoomCol = dicRooCol;
            if (serviceReq != null && serviceReq.Count > 0)
                this.ServiceReq = serviceReq;
        }

        internal List<ADO.RoomNumOrderADO> Run()
        {
            List<ADO.RoomNumOrderADO> result = null;
            try
            {
                if (DicRoomCol != null && ServiceReq != null && ServiceReq.Count > 0)
                {
                    Dictionary<long, List<HIS_SERVICE_REQ>> dicServiceReq = new Dictionary<long, List<HIS_SERVICE_REQ>>();
                    foreach (var item in ServiceReq)
                    {
                        if (!dicServiceReq.ContainsKey(item.EXECUTE_ROOM_ID))
                            dicServiceReq[item.EXECUTE_ROOM_ID] = new List<HIS_SERVICE_REQ>();
                        dicServiceReq[item.EXECUTE_ROOM_ID].Add(item);
                    }

                    var maxRow = 0;
                    foreach (var item in DicRoomCol)
                    {
                        if (dicServiceReq.ContainsKey(item.Key))
                        {
                            if (dicServiceReq[item.Key].Count > maxRow)
                            {
                                maxRow = dicServiceReq[item.Key].Count;
                            }
                        }
                    }

                    if (maxRow > 0)
                    {
                        result = new List<ADO.RoomNumOrderADO>();
                        for (int i = 0; i < maxRow; i++)
                        {
                            result.Add(new ADO.RoomNumOrderADO());
                        }

                        foreach (var col in DicRoomCol)
                        {
                            if (dicServiceReq.ContainsKey(col.Key))
                            {
                                switch (col.Value)
                                {
                                    case 1:
                                        ProcessColValue1(dicServiceReq[col.Key], ref result);
                                        break;
                                    case 2:
                                        ProcessColValue2(dicServiceReq[col.Key], ref result);
                                        break;
                                    case 3:
                                        ProcessColValue3(dicServiceReq[col.Key], ref result);
                                        break;
                                    case 4:
                                        ProcessColValue4(dicServiceReq[col.Key], ref result);
                                        break;
                                    case 5:
                                        ProcessColValue5(dicServiceReq[col.Key], ref result);
                                        break;
                                    case 6:
                                        ProcessColValue6(dicServiceReq[col.Key], ref result);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessColValue1(List<HIS_SERVICE_REQ> list, ref List<ADO.RoomNumOrderADO> result)
        {
            try
            {
                if (result != null && result.Count > 0 && list != null && list.Count > 0)
                {
                    var listReq = ProcessOrderListReq(list);
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (i >= listReq.Count) break;
                        result[i].COL_VALUE_1 = listReq[i].NUM_ORDER;
                        result[i].COL_VALUE_1_COLOR = CLX2;//red
                        if (i == 0 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            result[i].COL_VALUE_1_COLOR = DXL;
                        }
                        else if (i < 3 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && result.Count(o => o.COL_VALUE_1_COLOR == CLX1) < 2)
                        {
                            result[i].COL_VALUE_1_COLOR = CLX1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessColValue2(List<HIS_SERVICE_REQ> list, ref List<ADO.RoomNumOrderADO> result)
        {
            try
            {
                if (result != null && result.Count > 0 && list != null && list.Count > 0)
                {
                    var listReq = ProcessOrderListReq(list);
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (i >= listReq.Count) break;
                        result[i].COL_VALUE_2 = listReq[i].NUM_ORDER;
                        result[i].COL_VALUE_2_COLOR = CLX2;//red
                        if (i == 0 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            result[i].COL_VALUE_2_COLOR = DXL;
                        }
                        else if (i < 3 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && result.Count(o => o.COL_VALUE_2_COLOR == CLX1) < 2)
                        {
                            result[i].COL_VALUE_2_COLOR = CLX1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessColValue3(List<HIS_SERVICE_REQ> list, ref List<ADO.RoomNumOrderADO> result)
        {
            try
            {
                if (result != null && result.Count > 0 && list != null && list.Count > 0)
                {
                    var listReq = ProcessOrderListReq(list);
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (i >= listReq.Count) break;
                        result[i].COL_VALUE_3 = listReq[i].NUM_ORDER;
                        result[i].COL_VALUE_3_COLOR = CLX2;//red
                        if (i == 0 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            result[i].COL_VALUE_3_COLOR = DXL;
                        }
                        else if (i < 3 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && result.Count(o => o.COL_VALUE_3_COLOR == CLX1) < 2)
                        {
                            result[i].COL_VALUE_3_COLOR = CLX1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessColValue4(List<HIS_SERVICE_REQ> list, ref List<ADO.RoomNumOrderADO> result)
        {
            try
            {
                if (result != null && result.Count > 0 && list != null && list.Count > 0)
                {
                    var listReq = ProcessOrderListReq(list);
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (i >= listReq.Count) break;
                        result[i].COL_VALUE_4 = listReq[i].NUM_ORDER;
                        result[i].COL_VALUE_4_COLOR = CLX2;//red
                        if (i == 0 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            result[i].COL_VALUE_4_COLOR = DXL;
                        }
                        else if (i < 3 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && result.Count(o => o.COL_VALUE_4_COLOR == CLX1) < 2)
                        {
                            result[i].COL_VALUE_4_COLOR = CLX1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessColValue5(List<HIS_SERVICE_REQ> list, ref List<ADO.RoomNumOrderADO> result)
        {
            try
            {
                if (result != null && result.Count > 0 && list != null && list.Count > 0)
                {
                    var listReq = ProcessOrderListReq(list);
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (i >= listReq.Count) break;
                        result[i].COL_VALUE_5 = listReq[i].NUM_ORDER;
                        result[i].COL_VALUE_5_COLOR = CLX2;//red
                        if (i == 0 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            result[i].COL_VALUE_5_COLOR = DXL;
                        }
                        else if (i < 3 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && result.Count(o => o.COL_VALUE_5_COLOR == CLX1) < 2)
                        {
                            result[i].COL_VALUE_5_COLOR = CLX1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessColValue6(List<HIS_SERVICE_REQ> list, ref List<ADO.RoomNumOrderADO> result)
        {
            try
            {
                if (result != null && result.Count > 0 && list != null && list.Count > 0)
                {
                    var listReq = ProcessOrderListReq(list);
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (i >= list.Count) break;
                        result[i].COL_VALUE_6 = listReq[i].NUM_ORDER;
                        result[i].COL_VALUE_6_COLOR = CLX2;//red
                        if (i == 0 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            result[i].COL_VALUE_6_COLOR = DXL;
                        }
                        else if (i < 3 && listReq[i].SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && result.Count(o => o.COL_VALUE_6_COLOR == CLX1) < 2)
                        {
                            result[i].COL_VALUE_6_COLOR = CLX1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // chỉ có 1 dòng màu xanh đầu tiên
        // thời gian sửa mới nhất trạng thái đang xử lý đưa lên đầu
        // tiếp theo là chưa xử lý sếp theo ưu tiên và số tt
        // cuối là đã xử lý (bn đi làm cls)
        private List<HIS_SERVICE_REQ> ProcessOrderListReq(List<HIS_SERVICE_REQ> list)
        {
            List<HIS_SERVICE_REQ> result = new List<HIS_SERVICE_REQ>();
            try
            {
                if (list != null && list.Count > 0)
                {
                    List<HIS_SERVICE_REQ> lstDxl = new List<HIS_SERVICE_REQ>();
                    List<HIS_SERVICE_REQ> lstCxl = new List<HIS_SERVICE_REQ>();

                    var ldxl = list.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL).ToList();
                    if (ldxl != null && ldxl.Count > 0) lstDxl.AddRange(ldxl);

                    var lcxl = list.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL).ToList();
                    if (lcxl != null && lcxl.Count > 0) lstCxl.AddRange(lcxl);

                    // thời gian sửa mới nhất trạng thái đang xử lý đưa lên đầu. Thời gian trong khoảng 15' tính đến thời điểm hiện tại
                    if (lstDxl != null && lstDxl.Count > 0)
                    {
                        lstDxl = lstDxl.OrderByDescending(o => o.MODIFY_TIME).ToList();
                        var reqfirst = lstDxl.First();
                        DateTime modifiTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(reqfirst.MODIFY_TIME ?? 0) ?? DateTime.Now;
                        var diff = DateTime.Now - modifiTime;
                        if (diff.Minutes < 15)
                        {
                            result.Add(reqfirst);
                            lstDxl.Remove(reqfirst);
                        }
                    }

                    // tiếp theo là chưa xử lý sếp theo ưu tiên và số tt
                    if (lstCxl != null && lstCxl.Count > 0)
                    {
                        lstCxl = lstCxl.OrderByDescending(o => o.PRIORITY ?? 0).ThenBy(o => o.NUM_ORDER).ToList();
                        result.AddRange(lstCxl);
                    }

                    if (lstDxl != null && lstDxl.Count > 0)
                    {
                        lstDxl = lstDxl.OrderByDescending(o => o.PRIORITY ?? 0).ThenBy(o => o.NUM_ORDER).ToList();
                        result.AddRange(lstDxl);
                    }
                }
            }
            catch (Exception ex)
            {
                result = list;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
