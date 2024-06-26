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
using HIS.Desktop.Library.CacheClient.ControlState;
using Inventec.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Library.CacheClient
{
    public class ControlStateWorker
    {
        public static List<ControlStateRDO> sessionControlStateRDO = new List<ControlStateRDO>();

        internal TableCreateWorker TableCreateWorker { get { return (TableCreateWorker)Worker.Get<TableCreateWorker>(); } }

        public List<ControlStateRDO> GetAllData()
        {
            List<ControlStateRDO> datas = null;
            try
            {
                List<ControlStateRDO> rs = DatabaseCSWorker.DatabaseCS.GetList<ControlStateRDO>(ControlStateCFG.TableName__CONTROL_STATE);
                if (rs != null && rs.Count > 0)
                {
                    datas = rs.ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return datas;
        }

        public List<ControlStateRDO> GetData(string modulelink)
        {
            List<ControlStateRDO> datas = null;
            try
            {
                List<ControlStateRDO> rs = DatabaseCSWorker.DatabaseCS.GetList<ControlStateRDO>(ControlStateCFG.TableName__CONTROL_STATE);
                if (rs != null && rs.Count > 0)
                {
                    datas = rs.Where(o => o.MODULE_LINK == modulelink).ToList();
                }
                Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.GetData(" + modulelink + "). rs.count=" + (rs != null ? rs.Count : 0));
                Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.GetData. datas.count=" + (datas != null ? datas.Count : 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return datas;
        }

        public bool SetData(List<ControlStateRDO> data)
        {
            bool success = false, successCreate = true, successUpdate = true, successDelete = true;
            try
            {
                if (data == null || data.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Du lieu dau vao khong hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    return false;
                }

                string moduleLink = data.First().MODULE_LINK;
                string dataKey = ControlStateCFG.TableName__CONTROL_STATE;

                if (TableCreateWorker.CreateTableNew<ControlStateRDO>(dataKey))
                {
                    List<ControlStateRDO> controlStateCreates = new List<ControlStateRDO>();
                    List<ControlStateRDO> controlStateUpdates = new List<ControlStateRDO>();
                    List<ControlStateRDO> controlStateDeletes = new List<ControlStateRDO>();

                    List<ControlStateRDO> rs = DatabaseCSWorker.DatabaseCS.GetList<ControlStateRDO>(dataKey);
                    Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.SetData. rs.count=" + (rs != null ? rs.Count : 0));
                    rs = rs != null ? rs.Where(o => o.MODULE_LINK == moduleLink).ToList() : null;
                    if (rs != null && rs.Count > 0)
                    {
                        var oldKeys = rs.Select(o => o.KEY).Distinct().ToList();
                        var currentKeys = data.Select(o => o.KEY).Distinct().ToList();
                        controlStateCreates = data.Where(o => !oldKeys.Contains(o.KEY)).ToList();
                        controlStateUpdates = data.Where(o => oldKeys.Contains(o.KEY)).ToList();
                        controlStateDeletes = rs.Where(o => !currentKeys.Contains(o.KEY)).ToList();
                    }
                    else
                    {
                        controlStateCreates.AddRange(data);
                    }

                    Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.SetData. data.count=" + (data != null ? data.Count : 0));
                    Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.SetData. controlStateCreates.count=" + (controlStateCreates != null ? controlStateCreates.Count : 0));
                    Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.SetData. controlStateUpdates.count=" + (controlStateUpdates != null ? controlStateUpdates.Count : 0));
                    Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.SetData. controlStateDeletes.count=" + (controlStateDeletes != null ? controlStateDeletes.Count : 0));
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => controlStateCreates), controlStateCreates)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => controlStateUpdates), controlStateUpdates)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => controlStateDeletes), controlStateDeletes));

                    if (controlStateCreates != null && controlStateCreates.Count > 0)
                    {
                        try
                        {
                            Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.controlStateCreates. start call ExecuteNonQuery");
                            successCreate = DatabaseCSWorker.DatabaseCS.ExecuteWithData<ControlStateRDO>(controlStateCreates, dataKey, true);
                            Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.controlStateCreates. end call ExecuteNonQuery");
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            successCreate = false;
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => successCreate), successCreate));
                    }

                    if (controlStateUpdates != null && controlStateUpdates.Count > 0)
                    {
                        string cmdUpdate = "";
                        foreach (var item in controlStateUpdates)
                        {
                            cmdUpdate += string.Format("update {0} set {1} where {2};", dataKey, ControlStateCFG.VALUE + "='" + item.VALUE + "'", ControlStateCFG.KEY + "='" + item.KEY + "' and " + ControlStateCFG.MODULE_LINK + "='" + moduleLink + "'");
                        }
                        int rsUpdate = -1;
                        try
                        {
                            Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.controlStateUpdates. start call ExecuteNonQuery");
                            rsUpdate = DatabaseCSWorker.DatabaseCS.ExecuteNonQuery(cmdUpdate);
                            Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.controlStateUpdates. end call ExecuteNonQuery");
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            successUpdate = false;
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsUpdate), rsUpdate));
                        successUpdate = true;
                    }

                    if (controlStateDeletes != null && controlStateDeletes.Count > 0)
                    {
                        string cmdDelete = "";
                        foreach (var item in controlStateDeletes)
                        {
                            cmdDelete += string.Format("delete from {0} where {1};", dataKey, ControlStateCFG.KEY + "='" + item.KEY + "' and " + ControlStateCFG.MODULE_LINK + "='" + moduleLink + "'");
                        }
                        int rsDelete = -1;
                        try
                        {
                            Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.controlStateDeletes. start call ExecuteNonQuery");
                            rsDelete = DatabaseCSWorker.DatabaseCS.ExecuteNonQuery(cmdDelete);
                            Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.controlStateDeletes. end call ExecuteNonQuery");
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            successDelete = false;
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsDelete), rsDelete));
                        successDelete = true;
                    }
                    success = successDelete && successUpdate && successCreate;
                    Inventec.Common.Logging.LogSystem.Debug("ControlStateWorker.SetData." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        public bool ResetData(string moduleLink)
        {
            bool success = false, successDelete = true;
            try
            {
                if (String.IsNullOrEmpty(moduleLink))
                {
                    Inventec.Common.Logging.LogSystem.Error("Du lieu dau vao khong hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleLink), moduleLink));
                    return false;
                }

                string dataKey = ControlStateCFG.TableName__CONTROL_STATE;

                if (TableCreateWorker.CreateTableNew<ControlStateRDO>(dataKey))
                {
                    List<ControlStateRDO> controlStateDeletes = new List<ControlStateRDO>();

                    List<ControlStateRDO> rs = DatabaseCSWorker.DatabaseCS.GetList<ControlStateRDO>(dataKey);
                    controlStateDeletes = rs != null ? rs.Where(o => o.MODULE_LINK == moduleLink).ToList() : null;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => controlStateDeletes), controlStateDeletes));

                    if (controlStateDeletes != null && controlStateDeletes.Count > 0)
                    {
                        string cmdDelete = "";
                        foreach (var item in controlStateDeletes)
                        {
                            cmdDelete += string.Format("delete from {0} where {1};", dataKey, ControlStateCFG.KEY + "='" + item.KEY + "' and " + ControlStateCFG.MODULE_LINK + "='" + moduleLink + "'");
                        }
                        int rsDelete = -1;
                        try
                        {
                            rsDelete = DatabaseCSWorker.DatabaseCS.ExecuteNonQuery(cmdDelete);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            successDelete = false;
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cmdDelete), cmdDelete) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsDelete), rsDelete));
                        successDelete = true;
                    }
                    success = successDelete;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        public List<ControlStateRDO> GetDataBySession(string modulelink)
        {
            List<ControlStateRDO> datas = null;
            try
            {
                if (sessionControlStateRDO != null && sessionControlStateRDO.Count > 0)
                {
                    datas = sessionControlStateRDO.Where(o => o.MODULE_LINK == modulelink).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return datas;
        }

        public bool SetDataBySession(List<ControlStateRDO> data)
        {
            bool success = false, successCreate = true, successUpdate = true, successDelete = true;
            try
            {
                if (data == null || data.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Du lieu dau vao khong hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    return false;
                }

                string moduleLink = data.First().MODULE_LINK;
                string dataKey = ControlStateCFG.TableName__CONTROL_STATE;


                List<ControlStateRDO> controlStateCreates = new List<ControlStateRDO>();
                List<ControlStateRDO> controlStateUpdates = new List<ControlStateRDO>();
                List<ControlStateRDO> controlStateDeletes = new List<ControlStateRDO>();

                if (sessionControlStateRDO == null)
                {
                    sessionControlStateRDO = new List<ControlStateRDO>();
                }

                var rs = (sessionControlStateRDO != null && sessionControlStateRDO.Count > 0) ? sessionControlStateRDO.Where(o => o.MODULE_LINK == moduleLink).ToList() : null;
                if (rs != null && rs.Count > 0)
                {
                    var oldKeys = rs.Select(o => o.KEY).Distinct().ToList();
                    var currentKeys = data.Select(o => o.KEY).Distinct().ToList();
                    controlStateCreates = data.Where(o => !oldKeys.Contains(o.KEY)).ToList();
                    controlStateUpdates = data.Where(o => oldKeys.Contains(o.KEY)).ToList();
                    controlStateDeletes = rs.Where(o => !currentKeys.Contains(o.KEY)).ToList();
                }
                else
                {
                    controlStateCreates.AddRange(data);
                }

                Inventec.Common.Logging.LogSystem.Debug("SetDataBySession____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => controlStateCreates), controlStateCreates)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => controlStateUpdates), controlStateUpdates)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => controlStateDeletes), controlStateDeletes));

                if (controlStateCreates != null && controlStateCreates.Count > 0)
                {
                    try
                    {
                        sessionControlStateRDO.AddRange(controlStateCreates);
                        successCreate = sessionControlStateRDO.Count > 0;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        successCreate = false;
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => successCreate), successCreate));
                }

                if (controlStateUpdates != null && controlStateUpdates.Count > 0)
                {
                    foreach (var item in controlStateUpdates)
                    {
                        var cs = sessionControlStateRDO.Where(o => o.KEY == item.KEY && o.MODULE_LINK == item.MODULE_LINK).FirstOrDefault();
                        if (cs != null)
                        {
                            cs.VALUE = item.VALUE;
                            successUpdate = true;
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => successUpdate), successUpdate));

                if (controlStateDeletes != null && controlStateDeletes.Count > 0)
                {
                    foreach (var item in controlStateDeletes)
                    {
                        var cs = sessionControlStateRDO.Where(o => o.KEY == item.KEY && o.MODULE_LINK == item.MODULE_LINK).FirstOrDefault();
                        if (cs != null)
                        {
                            sessionControlStateRDO.Remove(cs);
                            successDelete = true;
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => successDelete), successDelete));
                success = successDelete && successUpdate && successCreate;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        public bool ResetDataBySession(string moduleLink)
        {
            bool success = false;
            try
            {
                if (String.IsNullOrEmpty(moduleLink))
                {
                    Inventec.Common.Logging.LogSystem.Error("Du lieu dau vao khong hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleLink), moduleLink));
                    return false;
                }

                var rs = (sessionControlStateRDO != null && sessionControlStateRDO.Count > 0) ? sessionControlStateRDO.Where(o => o.MODULE_LINK == moduleLink).ToList() : null;
                if (rs != null && rs.Count > 0)
                {
                    foreach (var item in rs)
                    {
                        sessionControlStateRDO.Remove(item);
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }
    }
}
